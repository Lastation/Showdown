
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

        [UdonSynced] string sGachaResult = "";
        [UdonSynced] string sGachaOwner = "";

        [UdonSynced] string sPenaltyResult = "";
        [UdonSynced] string sPenaltyOwner = "";

        [SerializeField] GameObject obj_shop;
        [SerializeField] GameObject obj_result;

        [UdonSynced] bool isOpen = false;

        int[] percent_Gacha = new int[7] { 5000, 2500, 1000, 500, 500, 470, 30 };
        int[] percent_Penalty = new int[10] { 5500, 1500, 800, 400, 800, 400, 200, 100, 200, 100 };

        #region Gacha
        public void Purchase_Gacha()
        {
            if (sGachaOwner != "" || (playerData.coin < 10 && playerData.gacha <= 0)) return;
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            Gacha(1);
        }

        public void Purchase_Gacha10()
        {
            if (sGachaOwner != "" || (playerData.coin < 100 && playerData.gacha <= 0)) return;
            Networking.SetOwner(Networking.LocalPlayer, gameObject);

            if (playerData.gacha > 0)   Gacha(Mathf.Min(playerData.gacha, 10));
            else                        Gacha(10);
        }

        public void Gacha(int num)
        {
            if (playerData.gacha > 0)
            {
                instanceData.GetTotalPercent().Add_Gacha_Index(1, num);
                playerData.gacha -= num;
            }
            else
            {
                instanceData.GetTotalPercent().Add_Gacha_Index(0, 10 * num);
                playerData.coin -= 10 * num;
            }

            int totalPercent = 0; 
            sGachaResult = "";

            for (int i = 0; i <  percent_Gacha.Length; i++)
                totalPercent += percent_Gacha[i];

            sGachaOwner = Networking.LocalPlayer.displayName;

            for (int count = 0; count < num; count++)
            {
                sGachaResult += $"{count + 1} - {GachaResult(totalPercent)}\n";
            }
            Dosync();
        }

        private string GachaResult(int totalPercent)
        {
            int random = Random.Range(0, totalPercent + 1);
            int randomStep = 0;
            int coin = 0;

            for (int i = 0; i < percent_Gacha.Length; i++)
            {
                randomStep += percent_Gacha[i];

                if (random < randomStep)
                {
                    switch (i)
                    {
                        case 0: //1 - 10코인: 70 %
                            coin = Random.Range(1, 6);
                            playerData.coin += coin;
                            instanceData.GetTotalPercent().Add_Gacha_Index(2, 1);
                            return $"코인 +{coin}";
                        case 1: //1 - 10코인: 70 %
                            coin = Random.Range(5, 11);
                            instanceData.GetTotalPercent().Add_Gacha_Index(3, 1);
                            playerData.coin += coin;
                            return $"코인 +{coin}";
                        case 2: //10 - 15코인: 15 %
                            coin = Random.Range(10, 16);
                            instanceData.GetTotalPercent().Add_Gacha_Index(4, 1);
                            playerData.coin += coin;
                            return $"코인 +{coin}";
                        case 3: //리바인권: 5 %
                            instanceData.GetTotalPercent().Add_Gacha_Index(5, 1);
                            if (playerData.rebine >= 2)
                            {
                                playerData.coin += 20;
                                return $"코인 +20";
                            }
                            else
                            {
                                playerData.rebine += 1;
                                return $"리바인권";
                            }
                        case 4: //벌칙룰렛 1회: 5 %
                            playerData.penalty += 1;
                            instanceData.GetTotalPercent().Add_Gacha_Index(6, 1);
                            return $"벌칙룰렛 1회권";
                        case 5: //15코인 - 50코인: 4.7 %
                            coin = Random.Range(15, 51);
                            instanceData.GetTotalPercent().Add_Gacha_Index(7, 1);
                            playerData.coin += coin;
                            sGachaResult += $"코인 +{coin}";
                            return $"코인 +{coin}";
                        case 6: //딜러 의상 변경: 0.3 %
                            instanceData.GetTotalPercent().Add_Gacha_Index(8, 1);
                            return $"딜러 의상변경권";
                    }
                }
            }
            return "";
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
            {
                instanceData.GetTotalPercent().Add_Penalty_Index(1, 1);
                playerData.penalty -= 1;
            }
            else
            {
                instanceData.GetTotalPercent().Add_Penalty_Index(0, 40);
                playerData.coin -= 40;
            }

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
                            instanceData.GetTotalPercent().Add_Penalty_Index(2, 1);
                            break;
                        case 1: //10 - 15코인: 15 %
                            result = Random.Range(20, 41);
                            playerData.coin += result;
                            sPenaltyResult = $"코인 +{result}";
                            instanceData.GetTotalPercent().Add_Penalty_Index(3, 1);
                            break;
                        case 2:
                            sPenaltyResult = $"냥체 3분";
                            instanceData.GetTotalPercent().Add_Penalty_Index(4, 1);
                            break;
                        case 3: //벌칙룰렛 1회: 5 %
                            sPenaltyResult = $"냥체 5분";
                            instanceData.GetTotalPercent().Add_Penalty_Index(5, 1);
                            break;
                        case 4: //15코인 - 50코인: 4 %
                            sPenaltyResult = $"3인칭 3분";
                            instanceData.GetTotalPercent().Add_Penalty_Index(6, 1);
                            break;
                        case 5: //3인칭 5분: 0.7 %
                            sPenaltyResult = $"3인칭 5분";
                            instanceData.GetTotalPercent().Add_Penalty_Index(7, 1);
                            break;
                        case 6: //딜러 의상 변경: 0.3 %
                            sPenaltyResult = $"냥체 3분 + 딜러의상변경 3분";
                            instanceData.GetTotalPercent().Add_Penalty_Index(8, 1);
                            break;
                        case 7: //딜러 의상 변경: 0.3 %
                            sPenaltyResult = $"냥체 5분 + 딜러의상변경 5분";
                            instanceData.GetTotalPercent().Add_Penalty_Index(9, 1);
                            break;
                        case 8: //딜러 의상 변경: 0.3 %
                            sPenaltyResult = $"3인칭 3분 + 딜러의상변경 3분";
                            instanceData.GetTotalPercent().Add_Penalty_Index(10, 1);
                            break;
                        case 9: //딜러 의상 변경: 0.3 %
                            sPenaltyResult = $"3인칭 5분 + 딜러의상변경 5분";
                            instanceData.GetTotalPercent().Add_Penalty_Index(11, 1);
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
            if (textGachaOwner)
                textGachaOwner.text = sGachaOwner;
            if (textGachaInfo)
                textGachaInfo.text = sGachaResult;

            if (textPenaltyOwner)
                textPenaltyOwner.text = sPenaltyOwner;
            if (textPenaltyInfo)
                textPenaltyInfo.text = sPenaltyResult;

            if (obj_shop)
                obj_shop.SetActive(isOpen);
            if (obj_result)
                obj_result.SetActive(isOpen);
        }
    }
}
