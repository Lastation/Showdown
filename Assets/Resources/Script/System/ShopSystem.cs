
using System.Linq.Expressions;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;

namespace Holdem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class ShopSystem : UdonSharpBehaviour
    {
        [SerializeField] InstanceData instanceData;
        [SerializeField] PlayerData playerData;
        [SerializeField] Text textOwner;
        [SerializeField] Text textGachaInfo;
        [UdonSynced] string sGachaResult = "";
        [UdonSynced] string sOwner = "";

        int[] percent = new int[7] { 7000, 1500, 500, 500, 400, 70, 30 };

        public void Purchase_Gacha()
        {
            if (sOwner != "" || playerData.coin < 10) return;

            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            playerData.coin -= 10;

            int totalPercent = 0; 
            int result = 0;

            for (int i = 0; i <  percent.Length; i++)
                totalPercent += percent[i];

            int random = Random.Range(0, totalPercent + 1);
            totalPercent = 0;

            for (int i = 0; i < percent.Length; i++)
            {
                totalPercent += percent[i];
                if (random < totalPercent)
                {
                    if (sGachaResult != "")
                        break;

                    switch (i)
                    {
                        case 0: //1 - 10코인: 70 %
                            result = Random.Range(1, 11);
                            playerData.coin += result;
                            sGachaResult = $"코인 +{result}";
                            break;
                        case 1: //10 - 15코인: 15 %
                            result = Random.Range(10, 16);
                            playerData.coin += result;
                            sGachaResult = $"코인 +{result}";
                            break;
                        case 2: //리바인권: 5 %
                            if (playerData.rebine >= 2)
                            {
                                playerData.coin += 20;
                                sGachaResult = $"코인 +{result}";
                            }
                            else
                            {
                                playerData.rebine += 1;
                                sGachaResult = $"리바인권";
                            }
                            break;
                        case 3: //벌칙룰렛 1회: 5 %
                            sGachaResult = $"벌칙룰렛 1회권";
                            break;
                        case 4: //15코인 - 50코인: 4 %
                            result = Random.Range(15, 51);
                            playerData.coin += result;
                            sGachaResult = $"코인 +{result}";
                            break;
                        case 5: //딜러바꿔: 0.7 %
                            sGachaResult = $"딜러 바꾸기권";
                            break;
                        case 6: //딜러 의상 변경: 0.3 %
                            sGachaResult = $"딜러 의상변경권";
                            break;
                    }
                }
            }
            sOwner = Networking.LocalPlayer.displayName;
            Dosync();
        }

        public void Reset_Gacha()
        {
            if (!instanceData.DealerCheck(Networking.LocalPlayer.displayName))
                return;

            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            sGachaResult = "";
            sOwner = "";
            Dosync();
        }

        public void Dosync()
        {
            SyncAction();
            RequestSerialization();
        }

        public override void OnDeserialization()
        {
            SyncAction();
        }

        public void SyncAction()
        {
            textOwner.text = sOwner;
            textGachaInfo.text = sGachaResult;
        }
    }
}
