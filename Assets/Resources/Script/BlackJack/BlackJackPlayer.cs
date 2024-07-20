
using Holdem;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class BlackJackPlayer : UdonSharpBehaviour
{
    [SerializeField] MainSystem mainSystem;
    [SerializeField] BlackJackSystem blackJackSystem;
    [SerializeField] TextMeshProUGUI textDisplayName, textCoin;
    [SerializeField] GameObject obj_enter, obj_exit;

    [UdonSynced] string _displayName = "";
    [UdonSynced] int playerID = 0;
    [UdonSynced] int _coin = 0;
    public int coin => _coin;

    public string displayName => _displayName;

    public bool isPlay => playerID != 0 ? true : false;

    #region Enter & Exit
    public void Enter_Game()
    {
        VRCPlayerApi player = Networking.LocalPlayer;

        if (playerID != 0)
            return;

        if (mainSystem.playerData.coin > 20)
            return;

        if (mainSystem.playerData.isPlayGame)
            return;

        Set_Owner(player);
        obj_enter.SetActive(false);
        obj_exit.SetActive(true);

        _displayName = player.displayName;
        playerID = player.playerId;
        mainSystem.playerData.isPlayGame = true;
        _coin = mainSystem.playerData.coin;
        DoSync();
    }

    public void Exit_Table()
    {
        if (!Networking.IsOwner(gameObject)) return;

        obj_enter.SetActive(true);
        obj_exit.SetActive(false);

        _displayName = "join";
        playerID = 0;
        mainSystem.playerData.isPlayGame = false;
        _coin = 0;
        DoSync();
    }
    #endregion

    public void Add_Coin()
    {
        if (!Networking.IsOwner(gameObject)) return;
        if (playerID == 0) return;

        mainSystem.playerData.coin += 5;
        _coin = mainSystem.playerData.coin;
        if (_coin > 20)
            Exit_Table();
        DoSync();
    }
    public void Add_Coin2x()
    {
        if (!Networking.IsOwner(gameObject)) return;
        if (playerID == 0) return;

        mainSystem.playerData.coin += 10;
        _coin = mainSystem.playerData.coin;
        if (_coin > 20)
            Exit_Table();
        DoSync();
    }

    #region Networking
    public void DoSync()
    {
        UpdateSyncs();
        RequestSerialization();
    }
    public override void OnDeserialization() => UpdateSyncs();
    public void UpdateSyncs()
    {
        textDisplayName.text = _displayName;
        textCoin.text = _coin.ToString();
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
    }
    #endregion
}
