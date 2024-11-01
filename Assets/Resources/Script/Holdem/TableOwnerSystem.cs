using System.Diagnostics;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace Holdem
{
    public class TableOwnerSystem : UdonSharpBehaviour
    {
        [UdonSynced] string displayName;
        [UdonSynced] int playerId;

        [SerializeField] DealerSystem dealerSystem = null;
        [SerializeField] CardSystem cardSystem;
        [SerializeField] TableOwnerUI tableOwnerUI;
        [SerializeField] InstanceData instanceData;


        #region Enter & Exit
        public void Enter_Table()
        {
            VRCPlayerApi localPlayer = Networking.LocalPlayer;

            if (!instanceData.DealerCheck(localPlayer.displayName))
                return;

            if (displayName != "")
            {
                if (displayName != localPlayer.displayName)
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "Exit_Table");
                return;
            }

            Set_Owner(localPlayer);

            if (dealerSystem != null) dealerSystem.Set_Owner(localPlayer);
            tableOwnerUI.Set_Owner(localPlayer);
            cardSystem.Set_Owner(localPlayer);

            displayName = localPlayer.displayName;
            playerId = localPlayer.playerId;
            tableOwnerUI.Set_TableDealerUI(true);
            cardSystem.Set_Pickupable(true);
            cardSystem.Init_Card();
            DoSync();
        }
        public void Exit_Table()
        {
            displayName = "";
            tableOwnerUI.Set_TableDealerUI(false);
            cardSystem.Set_Pickupable(false);
            DoSync();
        }
        public void Set_Owner(VRCPlayerApi value)
        {
            if (value.IsOwner(gameObject)) return;
            Networking.SetOwner(value, gameObject);
        }
        #endregion

        public TableOwnerUI Get_TableOwnerUI() => tableOwnerUI;

        #region Sync
        public void Start()
        {
            displayName = "";
            playerId = 0;
        }
        public void DoSync()
        {
            UpdateSync();
            RequestSerialization();
        }
        public override void OnDeserialization()
        {
            UpdateSync();
        }
        public void UpdateSync()
        {
            Update_DisplayText();
        }
        public void Update_DisplayText()
        {
            if (dealerSystem != null) dealerSystem.getDealerUI.Set_Dealer_Displayname(displayName);
            tableOwnerUI.Update_UI(displayName);
        }
        #endregion
        #region Networking
        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (!Networking.IsOwner(gameObject)) return;
            DoSync();
        }
        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            if (!Networking.IsOwner(gameObject)) return;
            if (player.playerId != playerId) return;
            displayName = "";
            playerId = 0;
            DoSync();
        }
        #endregion
    }
}
