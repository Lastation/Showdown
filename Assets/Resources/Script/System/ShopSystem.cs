
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
        [SerializeField] Text textGachaOwner, textGachaInfo;
        [SerializeField] Text textPenaltyOwner, textPenaltyInfo;
        [SerializeField] Text textChamingOwner, textClothOwner;
        [UdonSynced] string sGachaResult = "";
        [UdonSynced] string sGachaOwner = "";

        [UdonSynced] string sPenaltyResult = "";
        [UdonSynced] string sPenaltyOwner = "";

        [UdonSynced] string sCharmingOwner = "";
        [UdonSynced] string sClothOwner = "";

        int[] percent_Gacha = new int[7] { 5000, 2500, 1000, 500, 500, 470, 30 };
        int[] percent_Penalty = new int[10] { 5500, 1500, 800, 400, 800, 400, 200, 100, 200, 100 };

        #region Gacha
        public void Purchase_Gacha()
        {
            if (sGachaOwner != "" || (playerData.coin < 10 && playerData.gacha <= 0)) return;

            Networking.SetOwner(Networking.LocalPlayer, gameObject);

            if (playerData.gacha > 0)
                playerData.gacha -= 1;
            else
                playerData.coin -= 10;

            int totalPercent = 0; 
            int result = 0;

            for (int i = 0; i <  percent_Gacha.Length; i++)
                totalPercent += percent_Gacha[i];

            int random = Random.Range(0, totalPercent + 1);
            totalPercent = 0;

            for (int i = 0; i < percent_Gacha.Length; i++)
            {
                totalPercent += percent_Gacha[i];
                if (random < totalPercent)
                {
                    if (sGachaResult != "")
                        break;

                    switch (i)
                    {
                        case 0: //1 - 10코인: 70 %
                            result = Random.Range(1, 6);
                            playerData.coin += result;
                            sGachaResult = $"코인 +{result}";
                            break;
                        case 1: //1 - 10코인: 70 %
                            result = Random.Range(5, 11);
                            playerData.coin += result;
                            sGachaResult = $"코인 +{result}";
                            break;
                        case 2: //10 - 15코인: 15 %
                            result = Random.Range(10, 16);
                            playerData.coin += result;
                            sGachaResult = $"코인 +{result}";
                            break;
                        case 3: //리바인권: 5 %
                            if (playerData.rebine >= 2)
                            {
                                playerData.coin += 20;
                                sGachaResult = $"코인 +20";
                            }
                            else
                            {
                                playerData.rebine += 1;
                                sGachaResult = $"리바인권";
                            }
                            break;
                        case 4: //벌칙룰렛 1회: 5 %
                            playerData.penalty += 1;
                            sGachaResult = $"벌칙룰렛 1회권";
                            break;
                        case 5: //15코인 - 50코인: 4.7 %
                            result = Random.Range(15, 51);
                            playerData.coin += result;
                            sGachaResult = $"코인 +{result}";
                            break;
                        case 6: //딜러 의상 변경: 0.3 %
                            sGachaResult = $"딜러 의상변경권";
                            break;
                    }
                }
            }
            sGachaOwner = Networking.LocalPlayer.displayName;
            Dosync();
        }

        public void Reset_Gacha()
        {
            if (!instanceData.DealerCheck(Networking.LocalPlayer.displayName))
                return;

            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            sGachaResult = "";
            sGachaOwner = "";
            Dosync();
        }
        #endregion
        #region Penalty
        public void Purchase_Penalty()
        {
            if (sPenaltyOwner != "" || (playerData.coin < 40 && playerData.penalty <= 0)) return;

            Networking.SetOwner(Networking.LocalPlayer, gameObject);

            if (playerData.penalty > 0)
                playerData.penalty -= 1;
            else
                playerData.coin -= 40;

            int totalPercent = 0;
            int result = 0;

            for (int i = 0; i < percent_Penalty.Length; i++)
                totalPercent += percent_Penalty[i];

            int random = Random.Range(0, totalPercent + 1);
            totalPercent = 0;

            for (int i = 0; i < percent_Penalty.Length; i++)
            {
                totalPercent += percent_Penalty[i];
                if (random < totalPercent)
                {
                    if (sPenaltyResult != "")
                        break;

                    switch (i)
                    {
                        case 0: //1 - 10코인: 70 %
                            result = Random.Range(1, 21);
                            playerData.coin += result;
                            sPenaltyResult = $"코인 +{result}";
                            break;
                        case 1: //10 - 15코인: 15 %
                            result = Random.Range(20, 41);
                            playerData.coin += result;
                            sPenaltyResult = $"코인 +{result}";
                            break;
                        case 2:
                            sPenaltyResult = $"냥체 3분";
                            break;
                        case 3: //벌칙룰렛 1회: 5 %
                            sPenaltyResult = $"냥체 5분";
                            break;
                        case 4: //15코인 - 50코인: 4 %
                            sPenaltyResult = $"3인칭 3분";
                            break;
                        case 5: //3인칭 5분: 0.7 %
                            sPenaltyResult = $"3인칭 5분";
                            break;
                        case 6: //딜러 의상 변경: 0.3 %
                            sPenaltyResult = $"냥체 3분 + 딜러의상변경 3분";
                            break;
                        case 7: //딜러 의상 변경: 0.3 %
                            sPenaltyResult = $"냥체 5분 + 딜러의상변경 5분";
                            break;
                        case 8: //딜러 의상 변경: 0.3 %
                            sPenaltyResult = $"3인칭 3분 + 딜러의상변경 3분";
                            break;
                        case 9: //딜러 의상 변경: 0.3 %
                            sPenaltyResult = $"3인칭 5분 + 딜러의상변경 5분";
                            break;
                    }
                }
            }
            sPenaltyOwner = Networking.LocalPlayer.displayName;
            Dosync();
        }

        public void Reset_Penalty()
        {
            if (!instanceData.DealerCheck(Networking.LocalPlayer.displayName))
                return;

            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            sPenaltyResult = "";
            sPenaltyOwner = "";
            Dosync();
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

        public void SyncAction()
        {
            textGachaOwner.text = sGachaOwner;
            textGachaInfo.text = sGachaResult;

            textPenaltyOwner.text = sPenaltyOwner;
            textPenaltyInfo.text = sPenaltyResult;

            if (textChamingOwner)
                textChamingOwner.text = sCharmingOwner;
            if (textClothOwner)
                textClothOwner.text = sClothOwner;
        }
    }
}
