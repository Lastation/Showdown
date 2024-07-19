
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
        [SerializeField] Text textRebineTimer, textChamingOwner, textClothOwner;
        [UdonSynced] string sCharmingOwner = "";
        [UdonSynced] string sClothOwner = "";

        [SerializeField] GameObject obj_shop;
        [UdonSynced] bool isOpen = false;
        [UdonSynced] int dRebineTimer;

        #region Rebine
        public void FixedUpdate()
        {
            if (dRebineTimer > DateTime.Now.Second && playerData.rebine <= 0)
                textRebineTimer.text = $"다음 리바인권 구매까지 {dRebineTimer - DateTime.Now.Second} 초 남았어요!";
            else
            {
                textRebineTimer.text = "리바인권 구매 가능!";
            }
        }

        public void Purchase_Rebine()
        {
            if (playerData.chip > 2000)
                return;

            if (playerData.rebine > 0)
            {
                playerData.chip = 20000;
                playerData.rebine -= 1;
            }
            else if (playerData.coin >= 20)
            {
                if (dRebineTimer > DateTime.Now.Second)
                    return;
                playerData.chip = 20000;
                playerData.coin -= 20;
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
                dRebineTimer = DateTime.Now.AddSeconds(120).Second;
                Dosync();
            }
        }
        #endregion

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
            if (obj_shop)
                obj_shop.SetActive(isOpen);
        }
    }
}