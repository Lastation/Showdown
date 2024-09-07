using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace Holdem
{
    enum BlindCheck
    {
        None = 0,
        SB = 1,
        BB = 2,
    }

    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class PlayerSystem : UdonSharpBehaviour
    {
        [SerializeField] MainSystem mainSystem;
        [SerializeField] DealerSystem dealerSystem;
        [SerializeField] PlayerUI playerUI;
        [SerializeField] AudioSource asVoice;
        [SerializeField] Transform[] _tfCardPos;
        [SerializeField] Transform _tfDealerPos;
        [SerializeField] int tableNumber;

        [UdonSynced] int actionSize = 0;
        [UdonSynced, FieldChangeCallback(nameof(displayName))] string _displayName = "";
        [UdonSynced] int playerID = 0;
        [UdonSynced] int _chip = 0;
        [UdonSynced] bool isAction = false;
        [UdonSynced] int _betSize = 0;
        [UdonSynced] SE_Table_Type voiceType = SE_Table_Type.Basic;

        TableState tableState = TableState.Wait;
        BlindCheck blindCheck = BlindCheck.None;
        bool isTurn = false;
        float fTurnTimer = 0.0f;
      
        #region Sync Function
        public string displayName
        {
            get => _displayName;
            set
            {
                _displayName = value;

                if (!Networking.IsOwner(dealerSystem.gameObject))
                    return;
                if (value == "")
                    dealerSystem.Exit_Player(tableNumber);
            }
        }
        public int chip 
        {
            get => _chip;
            set
            {
                _chip = value;
            }
        }
        public int betSize
        {
            get => _betSize;
            set
            {
                _betSize = value;
            }
        }
        #endregion
        public Transform[] tfCardPos => _tfCardPos;
        public Transform tfDealerPos => _tfDealerPos;
        public bool isPlay => playerID != 0 ? true : false;

        public PlayerUI getPlayerUI => playerUI;
        void Start()
        {
            tableNumber = transform.GetSiblingIndex();
            actionSize = 0;
            _displayName = "";
            playerID = 0;
            _chip = 0;
            isAction = false;
            betSize = 0;
            voiceType = SE_Table_Type.Basic;
        }
        public void FixedUpdated()
        {
            playerUI.Set_Rotation();

            if (!Networking.IsOwner(gameObject))
                return;

            if (!isTurn)
                return;

            fTurnTimer += Time.deltaTime;
            playerUI.Update_Timer(fTurnTimer);

            if (fTurnTimer > 30.0f)
                Exit_Table();
        }

        public void Add_Chip()
        {
            if (!Networking.IsOwner(gameObject)) return;
            mainSystem.playerData.coin += 10;
        }
        #region Enter & Exit
        public void Enter_Game()
        {
            VRCPlayerApi player = Networking.LocalPlayer;

            if (playerID != 0)
                return;

            if (mainSystem.playerData.chip < 200)
                return;

            if (mainSystem.playerData.isPlayGame)
                return;

            Set_Owner(player);

            displayName = player.displayName;
            playerID = player.playerId;
            chip = mainSystem.playerData.chip;
            mainSystem.playerData.isPlayGame = true;

            mainSystem.playerUI = playerUI;
            playerUI.Set_DisplayActive(true);
            playerUI.Set_UI_Height(true);
            playerUI.Set_Button_Color(false);
            isAction = false;
            isTurn = false;
            DoSync();
        }

        public void Exit_Table()
        {
            if (!Networking.IsOwner(gameObject)) return;
            displayName = "";
            playerID = 0;
            chip = 0;
            mainSystem.playerData.isPlayGame = false;

            mainSystem.playerUI = playerUI;
            isAction = false;
            isTurn = false;
            betSize = 0;

            playerUI.Set_DisplayActive(false);
            playerUI.Set_UI_Height(false);
            playerUI.Set_Button_Color(isTurn);

            Add_RaiseChipSize_Reset();
            DoSync();
        }
        #endregion
        #region Voice
        public void Play_Voice(SE_Voice_Index index)
        {
            switch (index)
            {
                case SE_Voice_Index.Fold:
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Play_Voice_Fold");
                    return;
                case SE_Voice_Index.Call:
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Play_Voice_Call");
                    return;
                case SE_Voice_Index.Check:
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Play_Voice_Check");
                    return;
                case SE_Voice_Index.Raise:
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Play_Voice_Raise");
                    return;
                case SE_Voice_Index.Allin:
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Play_Voice_Allin");
                    return;
                case SE_Voice_Index.Bet:
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Play_Voice_Bet");
                    return;
            }
        }

        public void Play_Voice_Fold() => asVoice.PlayOneShot(mainSystem.GetAudioClip(voiceType, SE_Voice_Index.Fold));
        public void Play_Voice_Call() => asVoice.PlayOneShot(mainSystem.GetAudioClip(voiceType, SE_Voice_Index.Call));
        public void Play_Voice_Check() => asVoice.PlayOneShot(mainSystem.GetAudioClip(voiceType, SE_Voice_Index.Check));
        public void Play_Voice_Raise() => asVoice.PlayOneShot(mainSystem.GetAudioClip(voiceType, SE_Voice_Index.Raise));
        public void Play_Voice_Allin() => asVoice.PlayOneShot(mainSystem.GetAudioClip(voiceType, SE_Voice_Index.Allin));
        public void Play_Voice_Bet() => asVoice.PlayOneShot(mainSystem.GetAudioClip(voiceType, SE_Voice_Index.Bet));
        #endregion
        #region EndGame
        public void Add_EndGamePot()
        {
            if (!Networking.IsOwner(gameObject)) return;
            if (actionSize == -1) return;

            HandRanking rank = (HandRanking)(playerUI.handRank / 1000);
            int result = 0;

            switch (rank)
            {
                case HandRanking.HighCard:
                    result = 1;
                    break;
                case HandRanking.OnePair:
                    result = 1;
                    break;
                case HandRanking.TwoPair:
                    result = 2;
                    break;
                case HandRanking.ThreeOfAKind:
                    result = 3;
                    break;
                case HandRanking.Straight:
                    result = 5;
                    break;
                case HandRanking.Flush:
                    result = 13;
                    break;
                case HandRanking.FullHouse:
                    result = 21;
                    break;
                case HandRanking.FourOfAKind:
                    result = 34;
                    break;
                case HandRanking.StraightFlush:
                    result = 55;
                    break;
                case HandRanking.RoyalFlush:
                    result = 100;
                    break;
            }
            if (dealerSystem.getTablePot(tableNumber) > 0)
                result *= 2;

            mainSystem.playerData.coin += result;
            mainSystem.playerData.chip += (dealerSystem.getTablePot(tableNumber));
            chip = mainSystem.playerData.chip;
            DoSync();
        }
        #endregion
        #region Turn
        public void Set_PaySB()
        {
            if (!Networking.IsOwner(gameObject)) return;
            betSize = 100;
            mainSystem.playerData.chip -= 100;
            chip = mainSystem.playerData.chip;
            blindCheck = BlindCheck.SB;
            DoSync();
        }
        public void Set_PayBB()
        {
            if (!Networking.IsOwner(gameObject)) return;
            betSize = 200;
            mainSystem.playerData.chip -= 200;
            chip = mainSystem.playerData.chip; 
            blindCheck = BlindCheck.BB;
            DoSync();
        }
        public void Set_Turn()
        {
            if (!Networking.IsOwner(gameObject)) return;

            if (tableState != dealerSystem.tableState)
            {
                tableState = dealerSystem.tableState;
                betSize = 0;

                if (tableState == TableState.Flop)
                {
                    switch (blindCheck)
                    {
                        case BlindCheck.SB:
                            betSize = 100;
                            blindCheck = BlindCheck.None;
                            break;
                        case BlindCheck.BB:
                            betSize = 200;
                            blindCheck = BlindCheck.None;
                            break;
                    }
                }
            }

            isTurn = true;
            fTurnTimer = 0;
            isAction = false;
            voiceType = mainSystem.voiceType;   
            playerUI.Set_Button_Color(isTurn);
            if (mainSystem.playerData.chip <= dealerSystem.tableCallSize - _betSize)
                playerUI.Set_CallText_Allin(mainSystem.playerData.chip);
            else
                playerUI.Set_CallText(dealerSystem.tableCallSize - _betSize);
            Add_RaiseChipSize_Reset();
        }

        public void Action_Call()
        {
            if (!Networking.IsOwner(gameObject))
                return;

            if (!isTurn)
                return;

            int value = dealerSystem.tableCallSize - _betSize;

            if (value > mainSystem.playerData.chip) value = mainSystem.playerData.chip;
            else value = dealerSystem.tableCallSize;
            Action_Sync(value);
        }
        public void Action_Raise()
        {
            if (!Networking.IsOwner(gameObject)) return;
            if (!isTurn) return;
            Action_Sync(actionSize);
        }
        public void Action_Fold()
        {
            if (!Networking.IsOwner(gameObject)) return;
            if (!isTurn) return;

            Action_Sync(-1);
        }
        public void Action_Sync(int value)
        {
            actionSize = value;
            if (actionSize != -1)
            {
                if (mainSystem.playerData.chip == value)
                    mainSystem.playerData.chip -= value;
                else
                    mainSystem.playerData.chip -= actionSize - _betSize;
            }

            betSize = value == -1 ? 0 : actionSize;

            if (actionSize == -1) Play_Voice(SE_Voice_Index.Fold);
            else if (actionSize == 0) Play_Voice(SE_Voice_Index.Check);
            else if (mainSystem.playerData.chip == 0) Play_Voice(SE_Voice_Index.Allin);
            else if (dealerSystem.tableCallSize == 0 && actionSize > 0) Play_Voice(SE_Voice_Index.Bet);
            else if (actionSize == dealerSystem.tableCallSize) Play_Voice(SE_Voice_Index.Call);
            else if (actionSize > dealerSystem.tableCallSize) Play_Voice(SE_Voice_Index.Raise);

            isAction = true;
            isTurn = false;
            playerUI.Set_Button_Color(isTurn);
            chip = mainSystem.playerData.chip;
            DoSync();
        }
        public void Action_Reset()
        {
            if (!Networking.IsOwner(gameObject))    return;
            if (playerID == 0)                      return;

            isTurn = false;
            isAction = false;
            betSize = 0;
            playerUI.Set_Button_Color(isTurn);
            Add_RaiseChipSize_Reset();
            DoSync();
        }
        #endregion
        #region Raise
        public void Add_RaiseChipSize_Reset()
        {
            int size = 0;
            if (dealerSystem.tableCallSize <= 100)
                size = 400 - dealerSystem.tableCallSize;

            else size = dealerSystem.tableCallSize;
            Set_RaiseChipSize(size, false);
        }
        public void Add_RaiseChipSize_3x() => Set_RaiseChipSize((dealerSystem.tableCallSize == 0 ? 200 : dealerSystem.tableCallSize) * 2, false);
        public void Add_RaiseChipSize_4x() => Set_RaiseChipSize((dealerSystem.tableCallSize == 0 ? 200 : dealerSystem.tableCallSize) * 3, false);
        public void Add_RaiseChipSize_100() => Set_RaiseChipSize(100, true);
        public void Add_RaiseChipSize_500() => Set_RaiseChipSize(500, true);
        public void Add_RaiseChipSize_1000() => Set_RaiseChipSize(1000, true);
        public void Add_RaiseChipSize_5000() => Set_RaiseChipSize(5000, true);
        public void Add_RaiseChipSize_10000() => Set_RaiseChipSize(10000, true);
        public void Add_RaiseChipSize_Allin() => Set_RaiseChipSize(mainSystem.playerData.chip, true);
        public void Set_RaiseChipSize(int value, bool isAdd)
        {
            if (!isTurn)
                return;

            if (isAdd)
                actionSize += value + _betSize;
            else
                actionSize = dealerSystem.tableCallSize + value;

            actionSize = Mathf.Min(actionSize - _betSize, mainSystem.playerData.chip);

            if (mainSystem.playerData.chip <= actionSize)
                playerUI.Set_RaiseText_Allin(actionSize);
            else
                playerUI.Set_RaiseText(actionSize);
            DoSync();
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
            playerUI.Update_DisplayName(_displayName);
            playerUI.Update_Chip(_chip);
            playerUI.Set_BetSize(betSize);
            Update_Action();
        }
        public void Update_Action()
        {
            if (!Networking.IsOwner(dealerSystem.gameObject)) return;
            if (playerID == 0) dealerSystem.Exit_Player(tableNumber);
            if (isAction) dealerSystem.Set_BetAction(tableNumber, actionSize);
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (!Networking.IsOwner(gameObject)) return;
        }
        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            if (!Networking.IsOwner(gameObject)) return;
            if (player.playerId != playerID) return;
            Exit_Table();
            DoSync();
        }
        public void Set_Owner(VRCPlayerApi player)
        {
            if (player.IsOwner(gameObject)) return;
            Networking.SetOwner(player, gameObject);
            playerUI.Set_Owner(player);
        }
        #endregion
    }
}
