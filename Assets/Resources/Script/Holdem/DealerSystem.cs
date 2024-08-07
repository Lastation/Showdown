using Newtonsoft.Json.Linq;
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace Holdem
{
    public enum TableState : int
    {
        Wait = 0,
        Hand = 1,
        Flop = 2,
        Turn = 3,
        River = 4,
        Open = 5,
    }
    public enum PlayerState : int
    {
        OutOfGame = 0,
        Wait = 1,
        Turn = 2,
        Call = 3,
        Check = 4,
        Raise = 5,
        ALLIN = 6,
        Fold = 7
    }
    public enum HandRanking : int
    {
        HighCard = 0,
        OnePair,
        TwoPair,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush,
        RoyalFlush
    }
    public enum HandSuit : int
    {
        Spade = 0,
        Diamond = 1,
        Heart = 2,
        Clover = 3
    }
    public enum HandNumber : int
    {
        Ace = 11,
        King = 11,
        Queen = 10,
        Jack = 9,
        Ten = 8,
        Nine = 7,
        Eight = 6,
        Seven = 5,
        Six = 4,
        Five = 3,
        Four = 2,
        Three = 1,
        Two = 0,
    }

    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class DealerSystem : UdonSharpBehaviour
    {
        /// Variables
        #region Sync Varialbes
        [UdonSynced] TableState _tableState;
        [UdonSynced] PlayerState[] playerState = new PlayerState[9];
        [UdonSynced] int tableTotalPot = 0;
        [UdonSynced] int[] tableSidePot = new int[9];
        [UdonSynced] int _tableCallSize = 200;
        [UdonSynced] int[] table_Cards = new int[23];
        [UdonSynced] int[] handRank = new int[9];
        #endregion

        #region Static Variables
        [SerializeField] int[] playerBetSize = new int[9];
        [SerializeField] int[] playerChip = new int[9];
        [SerializeField] MainSystem mainSystem;
        [SerializeField] AudioSource audioSource;
        [SerializeField] PlayerSystem[] playerSystem;
        [SerializeField] CardSystem cardSystem;
        [SerializeField] GameObject obj_dealerBtn;
        [SerializeField] GameObject obj_turnArrow;
        [SerializeField] DealerUI dealerUI;

        [SerializeField] GameObject[] chip_100;
        [SerializeField] GameObject[] chip_1000;
        [SerializeField] GameObject[] chip_10000;
        [SerializeField] GameObject[] chip_100000;
        #endregion

        int[] playerSideSize = new int[9];
        int turnIdx, dealerIdx = 0;
        TableState prevState;

        public DealerUI getDealerUI => dealerUI;

        public void Add_SidePot(int value, int index)
        {
            for (int i = 0; i < tableSidePot.Length; i++)
            {
                if (!isInGame(i)) continue;
                int sideSize = value;

                if (index != i)
                {
                    if (playerSystem[i].chip + playerSideSize[i] - playerSideSize[index] < 0)
                        sideSize = 0;
                    else if (playerSystem[i].chip + playerSideSize[i] - (playerSideSize[index] + value) < 0)
                        sideSize = playerSystem[i].chip + playerSideSize[i] - playerSideSize[index];
                    else
                        sideSize = value;
                }
                tableSidePot[i] += sideSize;
                if (tableSidePot[i] < 0) tableSidePot[i] = 0;
            }

            if (index != -1)
                playerSideSize[index] += value;
        }
        public void Sub_SidePot(int value)
        {
            for (int i = 0; i < tableSidePot.Length; i++)
            {
                if (hands[i] == -1 || !isInGame(i))
                    continue;
                tableSidePot[i] -= value;
                if (tableSidePot[i] < 0) tableSidePot[i] = 0;
            }
        }
        #region Sync
        public int getTablePot(int index) => tableSidePot[index];
        public bool isInGame(int index) => !(playerState[index] == PlayerState.OutOfGame || playerState[index] == PlayerState.Fold);
        public int Get_TurnIndex(int value) => value % 9;

        public int Get_TurnSettingIndex(int value)
        {
            for (int i = 0; i < playerState.Length; i++)
            {
                if (playerState[Get_TurnIndex(value + i)] != PlayerState.Wait)
                    continue;
                turnIdx = Get_TurnIndex(value + i);
                return turnIdx;
            }   
            return turnIdx;
        }
        public int Get_InGameCount()
        {
            int count = 0;
            for (int i = 0; i < playerSystem.Length; i++)
                if (isInGame(i))
                    count++;
            return count;
        }
        public int Get_PlayerCount()
        {
            int count = 0;
            for (int i = 0; i < playerSystem.Length; i++)
                if (playerSystem[i].isPlay)
                    count++;
            return count;
        }
        public void Set_TurnIndex(int value)
        {
            for (int i = 0; i < playerState.Length; i++)
            {
                if (playerState[Get_TurnIndex(i + value)] != PlayerState.Wait)
                    continue;

                turnIdx = Get_TurnIndex(i + value);
                Set_TurnPosition(turnIdx);
                playerState[turnIdx] = PlayerState.Turn;
                playerSystem[turnIdx].SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Set_Turn");
                return;
            }
            Set_PlayerStateWait();
            Set_GameAuto();
        }
        public int tableCallSize
        {
            get => _tableCallSize;
            set
            {
                _tableCallSize = value;
            }
        }

        public TableState tableState => _tableState;
        #endregion
        void Start()
        {
            for (int i = 0; i < playerState.Length; i++)
                playerState[i] = PlayerState.OutOfGame;

            for (int i = 0; i < tableSidePot.Length; i++)
                tableSidePot[i] = 0;

            for (int i = 0; i < table_Cards.Length; i++)
                table_Cards[i] = 0;

            for (int i = 0; i < handRank.Length; i++)
                handRank[i] = 0;
        }
        #region State Setting
        public void Set_TableState(TableState state)
        {
            _tableState = state;
            for (int i = 0; i < playerBetSize.Length; i++)
            {
                playerBetSize[i] = 0;
            }
            DoSync();
        }
        public void Set_TurnPosition(int idx)
        {
            obj_turnArrow.transform.position = playerSystem[idx].tfDealerPos.position;
            obj_turnArrow.transform.rotation = playerSystem[idx].tfDealerPos.rotation;
        }

        public void Set_PlayerStateWait()
        {
            for (int i = 0; i < playerState.Length; i++)
                if (isInGame(i) && playerState[i] != PlayerState.ALLIN)
                    playerState[i] = PlayerState.Wait;
        }
        public void Exit_Player(int index)
        {
            if (!Networking.IsOwner(gameObject)) return;
            playerState[index] = PlayerState.OutOfGame;
            handRank[index] = 0;
            hands[index] = 0;
            if (turnIdx == index) Set_TurnIndex(turnIdx);
            DoSync();
        }
        #endregion
        #region Audio

        public void Play_AudioClip_Draw() => audioSource.PlayOneShot(mainSystem.GetAudioClip(SE_Voice_Index.Draw));
        public void Play_AudioClip(SE_Voice_Index index)
        {

            switch (index)
            {
                case SE_Voice_Index.Draw:
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Play_AudioClip_Draw"); 
                    break;
            }
        }
        #endregion
        #region Game
        public void Set_GameReset()
        {
            _tableState = TableState.Wait;
            for (int i = 0; i < playerState.Length; i++)
                dealerUI.Set_PlayerName(playerSystem[i].displayName, i);
            Reset_Game();
            DoSync();
        }
        public void Set_GameStart()
        {
            if (tableState != TableState.Wait)
                return;

            if (Get_PlayerCount() < 2)
                return;

            Reset_Game();

            int dealerTurn = Get_TurnIndex(dealerIdx + 1);
            for (int i = 0; i < playerState.Length; i++)
            {
                if (playerState[Get_TurnIndex(dealerTurn + i)] != PlayerState.Wait)
                    continue;

                dealerIdx = Get_TurnIndex(dealerTurn + i);
                break;
            }

            _tableState = TableState.Hand;
            Set_GameAuto();
            DoSync();
        }
        private void Reset_Game()
        {
            for (int i = 0; i < playerSystem.Length; i++)
                if (playerSystem[i].isPlay && playerSystem[i].chip < 200)
                    playerSystem[i].SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Exit_Table");

            tableCallSize = 200;
            tableTotalPot = 0;
            for (int i = 0; i < table_Cards.Length; i++)
                table_Cards[i] = 52;
            cardSystem.Reset_Card();
            Reset_HandRank();

            for (int i = 0; i < playerSystem.Length; i++)
            {
                if (playerSystem[i].isPlay) playerState[i] = PlayerState.Wait;
                else playerState[i] = PlayerState.OutOfGame;
                playerBetSize[i] = 0;
                tableSidePot[i] = 0;
                playerSideSize[i] = 0;
                hands[i] = 0;
                handRank[i] = 0;
                playerSystem[i].SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Action_Reset");
            }

        }

        public void Set_GameAuto()
        {
            switch (tableState)
            {
                case TableState.Wait:
                    break;
                case TableState.Hand:
                    for (int i = 0; i < playerSystem.Length; i++)
                    {
                        playerChip[i] = playerSystem[i].chip;

                        if (!isInGame(i))
                            continue;
                        cardSystem.Set_CardPosition(i, playerSystem[i].tfCardPos[0]);
                        table_Cards[i] = cardSystem.Get_CardIndex(i);
                        cardSystem.Set_CardPosition(i + 9, playerSystem[i].tfCardPos[1]);
                        table_Cards[i + 9] = cardSystem.Get_CardIndex(i + 9);
                        dealerUI.Set_PlayerName(playerSystem[i].displayName, i);
                    }

                    obj_dealerBtn.transform.position = playerSystem[Get_TurnSettingIndex(dealerIdx)].tfDealerPos.position;

                    Set_HandRank();
                    Set_TableState(TableState.Flop);
                    Play_AudioClip(SE_Voice_Index.Draw);

                    int turn = Get_TurnSettingIndex(turnIdx + 1);
                    playerSystem[turn].SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Set_PaySB");
                    playerBetSize[turn] = 100;
                    Add_SidePot(100, turn);
                    turn = Get_TurnSettingIndex(turnIdx + 1);
                    playerSystem[turn].SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Set_PayBB");
                    playerBetSize[turn] = 200;
                    Add_SidePot(200, turn);
                    tableTotalPot += 300;
                    Set_TurnIndex(turn + 1);
                    break;
                case TableState.Flop:
                    for (int i = 0; i < 3; i++)
                    {
                        cardSystem.Set_CardPosition(i + 18, (CardPosition)(i + 1));
                        table_Cards[i + 18] = cardSystem.Get_CardIndex(i + 18);
                    }
                    Set_TableState(TableState.Turn);
                    Set_HandRank();
                    Play_AudioClip(SE_Voice_Index.Draw);
                    Set_TurnIndex(dealerIdx + 1);
                    break;
                case TableState.Turn:
                    cardSystem.Set_CardPosition(21, CardPosition.Turn);
                    table_Cards[21] = cardSystem.Get_CardIndex(21);
                    Set_TableState(TableState.River);
                    Set_HandRank();
                    Play_AudioClip(SE_Voice_Index.Draw);
                    Set_TurnIndex(dealerIdx + 1);
                    break;
                case TableState.River:
                    cardSystem.Set_CardPosition(22, CardPosition.River);
                    table_Cards[22] = cardSystem.Get_CardIndex(22);
                    Set_TableState(TableState.Open);
                    Set_HandRank();
                    Play_AudioClip(SE_Voice_Index.Draw);
                    Set_TurnIndex(dealerIdx + 1);
                    break;
                case TableState.Open:
                    if (Get_InGameCount() > 1)
                    {
                        for (int i = 0; i < 18; i++)
                        {
                            if (isInGame(i % 9))
                            {
                                cardSystem.Set_CardRotation(i);
                            }
                        }
                        Set_HandRank();
                    }
                    else
                        Reset_HandRank();

                    Set_TableState(TableState.Wait);
                    obj_turnArrow.transform.position = transform.position + Vector3.down;
                    Set_GameEnd();
                    break;
            }
            DoSync();
        }
        #endregion
        #region HandRank
        HandRanking[] p_handRank = new HandRanking[9];
        HandNumber[] p_handNumber = new HandNumber[9];
        HandSuit[] p_handSuit = new HandSuit[9];
        int[] hands = new int[9];
        int[] kiker = new int[9];
        int highHand = 0;

        public void Set_HandRank()
        {
            for (int i = 0; i < playerSystem.Length; i++)
            {
                if (!isInGame(i))
                {
                    handRank[i] = 0;
                    continue;
                }
                p_handRank[i] = Calculate_HandRank(i);
                handRank[i] = (int)p_handRank[i] * 1000 + (int)p_handNumber[i] * 10 + 3 - (int)p_handSuit[i];
            }
        }
        public void Reset_HandRank()
        {
            for (int i = 0; i < 9; i++)
            {
                p_handRank[i] = HandRanking.HighCard;
                p_handNumber[i] = HandNumber.Two;
                p_handSuit[i] = HandSuit.Clover;
            }
        }

        bool[] check = new bool[13];
        private HandRanking Calculate_HandRank(int playerID)
        {
            int index = 0, tableSuit, tableNumber;
            kiker[playerID] = 0;

            int[] pair = new int[13];
            int[] suit = new int[4];
            bool[] hand = new bool[52];

            for (int i = 0; i < 7; i++)
            {
                if (i == 0) index = playerID;
                else if (i == 1) index = playerID + 9;
                else index = i + 16;

                if (table_Cards[index] == 52)
                    continue;

                tableSuit = Mathf.FloorToInt(table_Cards[index] / 13);
                tableNumber = table_Cards[index] % 13;

                suit[tableSuit]++;
                pair[tableNumber]++;
                hand[table_Cards[index]] = true;

                if ((int)p_handNumber[playerID] < tableNumber)
                {
                    p_handSuit[playerID] = (HandSuit)tableSuit;
                    p_handNumber[playerID] = (HandNumber)tableNumber;
                }
            }

            for (int i = 0; i < check.Length; i++) check[i] = false;

            if (isRoyalFlush(hand, playerID)) return HandRanking.RoyalFlush;
            if (isStraightFlush(hand, playerID)) return HandRanking.StraightFlush;
            if (isFourOfAKind(pair, playerID)) return HandRanking.FourOfAKind;
            if (isFullHouse(hand, pair, playerID)) return HandRanking.FullHouse;
            if (isFlush(hand, suit, playerID)) return HandRanking.Flush;
            if (isStraight(hand, pair, playerID)) return HandRanking.Straight;
            if (isThreeOfAKind(hand, pair, playerID)) return HandRanking.ThreeOfAKind;
            if (isTwoPair(hand, pair, playerID)) return HandRanking.TwoPair;
            if (isOnePair(hand, pair, playerID)) return HandRanking.OnePair;
            KikerCheck(5, pair, playerID);
            return HandRanking.HighCard;
        }
        private bool isRoyalFlush(bool[] hand, int playerID)
        {
            for (int i = 0; i < 4; i++)
                if (hand[i * 13 + 8] == true &&
                    hand[i * 13 + 9] == true &&
                    hand[i * 13 + 10] == true &&
                    hand[i * 13 + 11] == true &&
                    hand[i * 13 + 12] == true)
                {
                    p_handSuit[playerID] = (HandSuit)i;
                    p_handNumber[playerID] = HandNumber.Ace;
                    return true;
                }
            return false;
        }
        private bool isStraightFlush(bool[] hand, int playerID)
        {
            for (int i = 0; i < 4; i++)
            {
                if (hand[i * 13 + 12] == true &&
                        hand[i * 13 + 0] == true &&
                        hand[i * 13 + 1] == true &&
                        hand[i * 13 + 2] == true &&
                        hand[i * 13 + 3] == true)
                {
                    p_handSuit[playerID] = (HandSuit)i;
                    p_handNumber[playerID] = HandNumber.Five;
                    return true;
                }
                for (int j = 0; j < 9; j++)
                    if (hand[i * 13 + j + 0] == true &&
                        hand[i * 13 + j + 1] == true &&
                        hand[i * 13 + j + 2] == true &&
                        hand[i * 13 + j + 3] == true &&
                        hand[i * 13 + j + 4] == true)
                    {
                        p_handSuit[playerID] = (HandSuit)i;
                        p_handNumber[playerID] = (HandNumber)(j + 4);
                        return true;
                    }
            }
            return false;
        }
        private bool isFourOfAKind(int[] pair, int playerID)
        {
            for (int i = pair.Length - 1; i >= 0; i--)
                if (pair[i] == 4)
                {
                    p_handSuit[playerID] = HandSuit.Spade;
                    p_handNumber[playerID] = (HandNumber)i;
                    check[i] = true;
                    KikerCheck(1, pair, playerID);
                    return true;
                }
            return false;
        }
        private bool isFullHouse(bool[] hand, int[] pair, int playerID)
        {
            int count = 0;
            if (isThreeOfAKind(hand, pair, playerID))
            {
                for (int i = 0; i < pair.Length; i++)
                    if (pair[i] >= 2)
                        count++;
                if (count >= 2)
                {
                    for (int i = pair.Length - 1; i >= 0; i--)
                    {
                        if (pair[i] >= 2)
                        {
                            if (p_handNumber[playerID] != (HandNumber)i)
                            {
                                kiker[playerID] = i;
                                break;
                            }
                        }
                    }
                    return true;
                }
            }
            return false;
        }
        private bool isFlush(bool[] hand, int[] suit, int playerID)
        {
            for (int i = 0; i < suit.Length; i++)
                if (suit[i] >= 5)
                {
                    p_handSuit[playerID] = (HandSuit)i;
                    for (int j = 12; j >= 0; j--)
                        if (hand[i * 13 + j] == true)
                        {
                            p_handNumber[playerID] = (HandNumber)j;
                            break;
                        }

                    int count = 5;
                    for (int j = 12; j >= 0; j--)
                    {
                        if (count <= 0)
                            break;
                        if (hand[i * 13 + j] == true)
                        {
                            kiker[playerID] += j;
                            count -= 1;
                        }
                    }
                    return true;
                }
            return false;
        }
        private bool isStraight(bool[] hand, int[] pair, int playerID)
        {
            for (int i = 8; i >= 0; i--)
                if (pair[i] != 0 && pair[i + 1] != 0 && pair[i + 2] != 0 && pair[i + 3] != 0 && pair[i + 4] != 0)
                {
                    if (hand[i + 04] == true) p_handSuit[playerID] = HandSuit.Spade;
                    else if (hand[i + 17] == true) p_handSuit[playerID] = HandSuit.Diamond;
                    else if (hand[i + 30] == true) p_handSuit[playerID] = HandSuit.Heart;
                    else if (hand[i + 43] == true) p_handSuit[playerID] = HandSuit.Clover;
                    p_handNumber[playerID] = (HandNumber)(i + 4);
                    kiker[playerID] = 0;
                    return true;
                }

            if (pair[0] != 0 && pair[1] != 0 && pair[2] != 0 && pair[3] != 0 && pair[12] != 0)
            {
                if (hand[0] == true) p_handSuit[playerID] = HandSuit.Spade;
                else if (hand[13] == true) p_handSuit[playerID] = HandSuit.Diamond;
                else if (hand[26] == true) p_handSuit[playerID] = HandSuit.Heart;
                else if (hand[39] == true) p_handSuit[playerID] = HandSuit.Clover;
                p_handNumber[playerID] = HandNumber.Five;
                kiker[playerID] = 0;
                return true;
            }
            return false;
        }
        private bool isThreeOfAKind(bool[] hand, int[] pair, int playerID)
        {
            for (int i = pair.Length - 1; i >= 0; i--)
                if (pair[i] == 3)
                {
                    if (hand[i + 00] == true) p_handSuit[playerID] = HandSuit.Spade;
                    else if (hand[i + 13] == true) p_handSuit[playerID] = HandSuit.Diamond;
                    else if (hand[i + 26] == true) p_handSuit[playerID] = HandSuit.Heart;
                    else if (hand[i + 39] == true) p_handSuit[playerID] = HandSuit.Clover;
                    p_handNumber[playerID] = (HandNumber)i;
                    check[i] = true;
                    KikerCheck(2, pair, playerID);
                    return true;
                }
            return false;
        }

        private bool isTwoPair(bool[] hand, int[] pair, int playerID)
        {
            int count = 0;
            for (int i = pair.Length - 1; i >= 0; i--)
                if (pair[i] == 2)
                {
                    if (count == 0)
                    {
                        if (hand[i + 00] == true) p_handSuit[playerID] = HandSuit.Spade;
                        else if (hand[i + 13] == true) p_handSuit[playerID] = HandSuit.Diamond;
                        else if (hand[i + 26] == true) p_handSuit[playerID] = HandSuit.Heart;
                        else if (hand[i + 39] == true) p_handSuit[playerID] = HandSuit.Clover;
                        p_handNumber[playerID] = (HandNumber)i;
                    }
                    count++;
                }
            if (count >= 2)
            {
                int cnt = 2;
                for (int i = pair.Length - 1; i >= 0; i--)
                    if (pair[i] == 2)
                    {
                        if (cnt > 0)
                        {
                            kiker[playerID] += i * 2;
                            check[i] = true;
                            cnt--;
                        }
                    }
                KikerCheck(1, pair, playerID);
                return true;
            }
            return false;
        }
        private bool isOnePair(bool[] hand, int[] pair, int playerID)
        {
            for (int i = pair.Length - 1; i >= 0; i--)
                if (pair[i] == 2)
                {
                    if (hand[i + 00] == true) p_handSuit[playerID] = HandSuit.Spade;
                    else if (hand[i + 13] == true) p_handSuit[playerID] = HandSuit.Diamond;
                    else if (hand[i + 26] == true) p_handSuit[playerID] = HandSuit.Heart;
                    else if (hand[i + 39] == true) p_handSuit[playerID] = HandSuit.Clover;
                    p_handNumber[playerID] = (HandNumber)i;
                    check[i] = true;
                    KikerCheck(3, pair, playerID);
                    return true;
                }
            return false;
        }

        private void KikerCheck(int count, int[] pair, int playerID)
        {
            for (int i = pair.Length - 1; i >= 0; i--)
            {
                if (check[i])
                    continue;
                if (count <= 0)
                    return;
                for (int j = 0; j < pair[i]; j++)
                {
                    if (count <= 0)
                        return;
                    kiker[playerID] += i;
                    count--;
                }
            }
        }
        #endregion
        #region GiveChip
        public void Add_Chip_P1() => Add_Chip_Player(0);
        public void Add_Chip_P2() => Add_Chip_Player(1);
        public void Add_Chip_P3() => Add_Chip_Player(2);
        public void Add_Chip_P4() => Add_Chip_Player(3);
        public void Add_Chip_P5() => Add_Chip_Player(4);
        public void Add_Chip_P6() => Add_Chip_Player(5);
        public void Add_Chip_P7() => Add_Chip_Player(6);
        public void Add_Chip_P8() => Add_Chip_Player(7);
        public void Add_Chip_P9() => Add_Chip_Player(8);

        public void Add_Chip_Player(int index)
        {
            if (!playerSystem[index].isPlay)
                return;
            playerSystem[index].SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Add_Chip");
        }

        public void Kick_Game_P1() => Kick_Game(0);
        public void Kick_Game_P2() => Kick_Game(1);
        public void Kick_Game_P3() => Kick_Game(2);
        public void Kick_Game_P4() => Kick_Game(3);
        public void Kick_Game_P5() => Kick_Game(4);
        public void Kick_Game_P6() => Kick_Game(5);
        public void Kick_Game_P7() => Kick_Game(6);
        public void Kick_Game_P8() => Kick_Game(7);
        public void Kick_Game_P9() => Kick_Game(8);

        public void Kick_Game(int idx)
        {
            if (!playerSystem[idx].isPlay) return;
            playerSystem[idx].SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Exit_Table");
            for (int i = 0; i < playerSystem.Length; i++) dealerUI.Set_PlayerName(playerSystem[i].displayName, i);
        }
        #endregion
        #region Action
        public void Set_BetAction(int index, int value)
        {
            if (value == -1)
            {
                playerState[index] = PlayerState.Fold;
                if (Get_InGameCount() <= 1)
                {
                    Set_TableState(TableState.Open);
                    Set_GameAuto();
                    return;
                }
                Set_NextTurn();
                return;
            }
            else
            {
                if (playerSystem[index].chip == 0)
                {
                    if (tableCallSize < value)
                        Set_PlayerStateWait();
                    playerState[index] = PlayerState.ALLIN;
                }
                else if (value == 0)
                    playerState[index] = PlayerState.Check;
                else if (value != tableCallSize)
                {
                    Set_PlayerStateWait();
                    playerState[index] = PlayerState.Raise;
                }
                else playerState[index] = PlayerState.Call;

                int betSize = playerBetSize[index];
                if (tableCallSize < value) tableCallSize = value;

                if (playerState[index] == PlayerState.ALLIN) playerBetSize[index] = value + betSize;
                else playerBetSize[index] = value;

                if (playerState[index] == PlayerState.ALLIN)
                {
                    tableTotalPot += value;
                    Add_SidePot(value, index);
                }
                else
                {
                    tableTotalPot += value - betSize;
                    Add_SidePot(value - betSize, index);
                }

                Set_NextTurn();
            }
        }

        public void Set_NextTurn()
        {
            bool isNextStep = true;
            bool isAllPlayerAllin = true;

            for (int i = 0; i < playerState.Length; i++)
            {
                if (playerState[i] == PlayerState.Wait)
                {
                    isNextStep = false;
                    break;
                }
            }
            for (int i = 0; i < playerState.Length; i++)
            {
                if (isInGame(i) && playerState[i] != PlayerState.ALLIN)
                {
                    isAllPlayerAllin = false;
                    break;
                }
            }
            if (isAllPlayerAllin)
            {
                while (tableState != TableState.Wait)
                    Set_GameAuto();
                return;
            }
            if (isNextStep)
            {
                _tableCallSize = 0;
                Set_PlayerStateWait();
                Set_GameAuto();
                DoSync();
                return;
            }

            if (tableState != TableState.Wait)
                Set_TurnIndex(turnIdx + 1);
            DoSync();
        }
        #endregion
        #region GameSet
        public string Get_CardName(int value)
        {
            if (value > 52 || value < 0)
                return "";

            int suit, number;

            number = value % 13 == 0 ? 13 : value % 13;
            suit = value / 13;

            return $"{mainSystem.sHandSuit(suit)}{mainSystem.sHandNumber(number)}";
        }
        public void Set_GameEnd()
        { 
            for (int i = 0; i < playerSystem.Length; i++)
            {
                if (!isInGame(i))
                    hands[i] = 0;
                else
                    hands[i] = (int)p_handRank[i] * 10000 + (int)p_handNumber[i] * 100 + kiker[i];
            }
            KikerCheck();
        }

        void KikerCheck()
        {
            if (tableTotalPot <= 0)
            {
                for (int i = 0; i < hands.Length; i++)
                    if (hands[i] != -1)
                        tableSidePot[i] = 0;
                DoSync();
                SendCustomEventDelayedSeconds("Add_EndGamePot", 1.0f);
                return;
            }
            else
            {
                int index = 0, count = 0;
                bool isKikerCheck = false;
                highHand = 0;

                for (int i = 0; i < playerSystem.Length; i++)
                {
                    if (tableSidePot[i] <= 0 || hands[i] < 0)
                        continue;

                    count++;

                    if (highHand < hands[i])
                    {
                        highHand = hands[i];
                        index = i;
                        isKikerCheck = false;
                    }
                    else if (highHand == hands[i])
                        isKikerCheck = true;
                }

                if (count == 0)
                {
                    tableTotalPot = 0;
                    KikerCheck();
                }
                else if (!isKikerCheck)
                    Set_SidePot(index);
                else
                    Add_EndGamePot_Chap();
            }
        }
        void Set_SidePot(int index)
        {
            int sidePotCalculate = tableSidePot[index];
            hands[index] = -1;
            tableTotalPot -= sidePotCalculate;
            Sub_SidePot(sidePotCalculate);
            KikerCheck();
        }

        public void Add_EndGamePot()
        {
            for (int i = 0; i < playerSystem.Length; i++)
                if (playerSystem[i].isPlay && playerState[i] != PlayerState.OutOfGame)
                    playerSystem[i].SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Add_EndGamePot");
        }
        public void Add_EndGamePot_Chap()
        {
            int chapCount = 0;
            int minSidePot = int.MaxValue;
            int maxSidePot = int.MinValue;

            for (int i = 0; i < playerSystem.Length; i++)
            {
                if (highHand != hands[i]) continue;
                if (hands[i] == -1 || playerState[i] == PlayerState.Fold) continue;

                if (minSidePot > tableSidePot[i]) minSidePot = tableSidePot[i];
                if (maxSidePot < tableSidePot[i]) maxSidePot = tableSidePot[i];

                chapCount++;
            }

            for (int i = 0; i < playerSystem.Length; i++)
            {
                if (highHand != hands[i]) continue;
                if (hands[i] == -1 || playerState[i] == PlayerState.Fold) continue;
                tableSidePot[i] = tableSidePot[i] - (minSidePot / chapCount);
                hands[i] = -1;
            }
            tableTotalPot -= maxSidePot;
            Sub_SidePot(maxSidePot);
            KikerCheck();
        }
        #endregion
        #region Networking
        public void DoSync()
        {
            UpdateSyncs();
            RequestSerialization();
        }

        public override void OnDeserialization() => UpdateSyncs();
        public void UpdateSyncs()
        {
            Update_PlayerUI();
            Update_PlayerReset();
            Update_Chips();
            dealerUI.Set_TableState(9 - Get_PlayerCount(), tableState != TableState.Wait);
            dealerUI.Set_TablePot(tableTotalPot < 0 ? 0 : tableTotalPot);
            dealerUI.Set_Table_PlayerPot(tableSidePot);
        }
        public void Update_PlayerUI()
        {
            for (int i = 0; i < playerSystem.Length; i++)
            {
                if (Get_InGameCount() > 1)
                    playerSystem[i].getPlayerUI.Set_StateText(playerState[i], tableState, handRank[i]);
                playerSystem[i].getPlayerUI.Set_CardImage(table_Cards, i);
            }
        }

        public void Update_Chips()
        {
            if (prevState == _tableState)
                return;

            prevState = _tableState;

            int bet100 = (tableTotalPot % 1000) / 100;
            int bet1000 = (tableTotalPot % 10000) / 1000;
            int bet10000 = (tableTotalPot % 100000) / 10000;
            int bet100000 = (tableTotalPot % 1000000) / 100000;

            for (int i = 0; i < chip_100.Length; i++)
            {
                if (i < bet100)
                {
                    chip_100[i].SetActive(true);
                    continue;
                }
                chip_100[i].SetActive(false);
            }
            for (int i = 0; i < chip_1000.Length; i++)
            {
                if (i < bet1000)
                {
                    chip_1000[i].SetActive(true);
                    continue;
                }
                chip_1000[i].SetActive(false);
            }
            for (int i = 0; i < chip_10000.Length; i++)
            {
                if (i < bet10000)
                {
                    chip_10000[i].SetActive(true);
                    continue;
                }
                chip_10000[i].SetActive(false);
            }
            for (int i = 0; i < chip_100000.Length; i++)
            {
                if (i < bet100000)
                {
                    chip_100000[i].SetActive(true);
                    continue;
                }
                chip_100000[i].SetActive(false);
            }

            for (int i = 0; i < playerSystem.Length; i++)
                playerSystem[i].Reset_BetSize();
        }

        public void Update_PlayerReset()
        {
            if (tableState != TableState.Wait)
                return;

            for (int i = 0; i < playerSystem.Length; i++)
            {
                if (!Networking.IsOwner(playerSystem[i].gameObject))
                    continue;

                playerSystem[i].Action_Reset();
            }
        }
        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (!Networking.IsOwner(gameObject)) return;
        }
        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            if (!Networking.IsOwner(gameObject)) return;
        }
        public void Set_Owner(VRCPlayerApi player)
        {
            if (player.IsOwner(gameObject)) return;
            Networking.SetOwner(player, gameObject);
            Networking.SetOwner(player, obj_dealerBtn);
            Networking.SetOwner(player, obj_turnArrow);
        }
        #endregion
    }
}