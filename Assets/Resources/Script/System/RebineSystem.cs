using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;

namespace Holdem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class RebineSystem : UdonSharpBehaviour
    {
        [SerializeField] InstanceData instanceData;
        [SerializeField] PlayerData playerData;
        [SerializeField] Text textRebineTimer;

        [UdonSynced] double dRebineTimer;
        [UdonSynced] double dPrevTime;
        TimeSpan timeSpan; 

        public void Awake()
        {
            dRebineTimer = 0;
            dPrevTime = 0;
        }

        #region Rebine
        public void FixedUpdate()
        {
            if (Networking.IsOwner(gameObject))
            {
                timeSpan = new TimeSpan(DateTime.Now.Ticks);
                if (dRebineTimer > dPrevTime && dPrevTime != timeSpan.TotalSeconds)
                {
                    dPrevTime = timeSpan.TotalSeconds;
                    RequestSerialization();
                }
            }

            if (dRebineTimer > dPrevTime && playerData.rebine <= 0)
                textRebineTimer.text = $"다음 리바인권 구매까지 {Math.Round(dRebineTimer - dPrevTime)} 초 남았어요!";
            else
                textRebineTimer.text = "리바인권 구매 가능!";
        }

        public void Purchase_Rebine()
        {
            if (playerData.isPlayGame)
                return;

            if (playerData.chip > 2000)
                return;

            if (playerData.rebine > 0)
            {
                playerData.chip = 20000;
                playerData.rebine -= 1;
            }
            else if (playerData.coin >= 20)
            {
                if (dRebineTimer > dPrevTime)
                    return;
                playerData.chip = 20000;
                playerData.coin -= 20;

                if (!Networking.IsOwner(gameObject))
                    Networking.SetOwner(Networking.LocalPlayer, gameObject);

                timeSpan = new TimeSpan(DateTime.Now.Ticks);
                dRebineTimer = timeSpan.TotalSeconds + 120;
                dPrevTime = timeSpan.TotalSeconds;
                RequestSerialization();
            }
        }
        #endregion
    }
}