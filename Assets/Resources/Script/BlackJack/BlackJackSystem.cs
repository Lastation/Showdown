
using Holdem;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class BlackJackSystem : UdonSharpBehaviour
{
    [SerializeField] MainSystem mainSystem;
    [SerializeField] BlackJackPlayer[] players;
    [SerializeField] TextMeshProUGUI[] text_TablePlayerName;
    [SerializeField] CardSystem cardSystem;
    [SerializeField] TextMeshPro text_TableState;
    [UdonSynced] string displayName = "";

    #region GiveCoin
    public void Add_Coin_P1() => Add_Coin_Player(0);
    public void Add_Coin_P2() => Add_Coin_Player(1);
    public void Add_Coin_P3() => Add_Coin_Player(2);
    public void Add_Coin_P4() => Add_Coin_Player(3);
    public void Add_Coin_P5() => Add_Coin_Player(4);


    public void Kick_Game_P1() => Kick_Game(0);
    public void Kick_Game_P2() => Kick_Game(1);
    public void Kick_Game_P3() => Kick_Game(2);
    public void Kick_Game_P4() => Kick_Game(3);
    public void Kick_Game_P5() => Kick_Game(4);

    public void Kick_Game(int idx)
    {
        if (!players[idx].isPlay) return;
        players[idx].SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Exit_Table");
        for (int i = 0; i < players.Length; i++) Set_PlayerName(players[i].displayName, i);
    }

    public void Reset_Game()
    {
        displayName = Networking.LocalPlayer.displayName;

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].coin > 20)
                players[i].SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Exit_Table");

            Set_PlayerName(players[i].displayName, i);
        }
        cardSystem.Set_BlackJack();
        DoSync();
    }

    public void Add_Coin_Player(int index)
    {
        if (!players[index].isPlay)
            return;
        mainSystem.playerData.coin += 1;
        players[index].SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Add_Coin");
    }
    #endregion
    public void Set_PlayerName(string value, int index) => text_TablePlayerName[index].text = value;

    public void DoSync()
    {
        Set_TableState();
        RequestSerialization();
    }

    public override void OnDeserialization()
    {
        Set_TableState();
    }

    public void Set_TableState()
    {
        int count = 0;
        for (int i = 0; i < players.Length; i++)
            if (!players[i].isPlay)
                count++;
        text_TableState.text = $"Dealer : {displayName}\n남은자리 : {count}";
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (!Networking.IsOwner(gameObject))
            return;
        DoSync();
    }

    public void Set_Owner(VRCPlayerApi player)
    {
        if (player.IsOwner(gameObject)) return;
        Networking.SetOwner(player, gameObject);
        cardSystem.Set_Owner(player);
    }
}
