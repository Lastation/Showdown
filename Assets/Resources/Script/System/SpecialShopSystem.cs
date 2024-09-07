
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;

namespace Holdem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SpecialShopSystem : UdonSharpBehaviour
    {
        [SerializeField] InstanceData instanceData;
        [SerializeField] PlayerData playerData;
        [SerializeField] Text textChamingOwner, textClothOwner, textTrophyOwner;
        [UdonSynced] string sCharmingOwner = "";
        [UdonSynced] string sClothOwner = "";
        [UdonSynced] string sTrophyOwner = "";

        [SerializeField] GameObject obj_shop;
        [UdonSynced] bool isOpen = false;

        #region Charming
        public void Purchase_Charming()
        {
            if (!textChamingOwner) return;

            if (sCharmingOwner != "" || playerData.coin < 300) return;
            playerData.coin -= 300;
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            sCharmingOwner = Networking.LocalPlayer.displayName;
            Dosync();
        }

        public void Reset_Charming()
        {
            if (!textChamingOwner) return;

            if (!instanceData.DealerCheck(Networking.LocalPlayer.displayName)) return;
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            sCharmingOwner = "";
            Dosync();
        }
        #endregion
        #region Cloth
        public void Purchase_Cloth()
        {
            if (!textClothOwner) return;

            if (sClothOwner != "" || playerData.coin < 200) return;
            playerData.coin -= 200;
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            sClothOwner = Networking.LocalPlayer.displayName;
            Dosync();
        }

        public void Reset_Cloth()
        {
            if (!textClothOwner) return;

            if (!instanceData.DealerCheck(Networking.LocalPlayer.displayName)) return;
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            sClothOwner = "";
            Dosync();
        }
        #endregion
        #region Trophy
        public void Purchase_Trophy()
        {
            if (!textTrophyOwner) return;

            if (sTrophyOwner != "" || playerData.coin < 500) return;
            playerData.coin -= 500;
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            sTrophyOwner = Networking.LocalPlayer.displayName;
            Dosync();
        }
        #endregion

        public void Dosync()
        {
            SyncAction();
            RequestSerialization();
        }

        public override void OnDeserialization()
        {
            SyncAction();
        }

        public void Shop_ON()
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            isOpen = true;
            Dosync();
        }
        public void Shop_OFF()
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            isOpen = false;
            Dosync();
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (Networking.IsOwner(gameObject))
                Dosync();
        }

        public void SyncAction()
        {
            if (textChamingOwner)
                textChamingOwner.text = sCharmingOwner;
            if (textClothOwner)
                textClothOwner.text = sClothOwner;
            if (textTrophyOwner)
                textTrophyOwner.text = sTrophyOwner;
            if (obj_shop)
                obj_shop.SetActive(isOpen);
        }
    }
}