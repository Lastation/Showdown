using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace Holdem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class InstanceData : UdonSharpBehaviour
    {
        [SerializeField] private string[] sDealer;          // 쇼다운 딜러
        [SerializeField] private PlayerData playerData;     // 플레이어 데이터

        [SerializeField] AudioClip[] acVoice;

        [UdonSynced] string[] sDisplayName = new string[100];
        [UdonSynced] int[] iChip = new int[100];
        [UdonSynced] int[] iCoin = new int[100];
        [UdonSynced] int[] dealerID = new int[20];
        [SerializeField] GameObject[] obj_dealerTag;

        public int index = -1;

        public AudioClip getVoice(int index)
        {
            if (index < 0 || index >= acVoice.Length) return null;
            return acVoice[index];
        }

        public string getDisplayName(int idx)
        {
            if (idx < 0 || idx >= sDisplayName.Length)
                return "";
            return sDisplayName[idx];
        }
        public int getChip(int idx) => iChip[idx];
        public int getCoin(int idx) => iCoin[idx];

        void Start()
        {
            for (int i = 0; i < dealerID.Length; i++)
                dealerID[i] = 0;

            for (int i = 0; i < sDisplayName.Length; i++)
            {
                sDisplayName[i] = "";
                iChip[i] = 0;
                iCoin[i] = 0;
            }

            if (Networking.IsOwner(gameObject))
            {
                int index = Get_DataCheckIndex(Networking.LocalPlayer.displayName);
                if (index != -1)
                    dealerID[index] = Networking.LocalPlayer.playerId;

                for (int i = 0; i < sDisplayName.Length; i++)
                {
                    if (sDisplayName[i] == Networking.LocalPlayer.displayName)
                    {
                        sDisplayName[i] = Networking.LocalPlayer.displayName;
                        playerData.chip = iChip[i];
                        playerData.coin = iCoin[i];
                        return;
                    }
                }

                sDisplayName[0] = Networking.LocalPlayer.displayName;
                iChip[0] = 20000;
                iCoin[0] = 0;
            }
        }

        public void LateUpdate()
        {
            for (int i = 0; i < dealerID.Length; i++)
            {
                if (dealerID[i] == 0) continue;
                VRCPlayerApi dealerApi = VRCPlayerApi.GetPlayerById(dealerID[i]);
                if (dealerApi != null)  
                {
                    obj_dealerTag[i].transform.position = dealerApi.GetPosition();
                }
                else obj_dealerTag[i].transform.position = new Vector3(10000, 0, 0);
            }
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (player == Networking.LocalPlayer) Load_Datas();

            if (!Networking.IsOwner(gameObject))
                return;

            for (int i = 0; i < sDisplayName.Length; i++)
            {
                int index = Get_DataCheckIndex(player.displayName);
                if (index != -1) dealerID[index] = player.playerId;

                if (sDisplayName[i] == player.displayName)
                {
                    Debug.Log($"[{i}] - {sDisplayName[i]} : chip = {iChip[i]}, coin = {iCoin[i]} Loaded");
                    break;
                }
                else if (sDisplayName[i] == "")
                {
                    sDisplayName[i] = player.displayName;
                    iChip[i] = 20000;
                    iCoin[i] = 0;
                    Debug.Log($"[{i}] - {sDisplayName[i]} : chip = {iChip[i]}, coin = {iCoin[i]} Init");
                    break;
                }
            }
            RequestSerialization();
        }

        public bool DealerCheck(string displayName)
        {
            for (int i = 0; i < sDealer.Length; i++)
                if (displayName == sDealer[i])
                    return true;
            return false;
        }

        public void Save_Datas(string displayName, int chip, int coin)
        {
            for (int i = 0; i < sDisplayName.Length; i++)
            {
                if (sDisplayName[i] == displayName)
                {
                    iChip[i] = chip;
                    iCoin[i] = coin;
                    Debug.Log($"[{i}] - {displayName} : chip = {chip}, coin = {coin} saved");
                    RequestSerialization();
                    return;
                }
            }
        }

        public void Load_Datas()
        {
            if (sDisplayName[0] == "")
            {
                SendCustomEventDelayedSeconds("Load_Datas", 2.0f);
                return;
            }
            if (Get_DataLength() == sDisplayName.Length)
                index = sDisplayName.Length;

            for (int i = 0; i < sDisplayName.Length; i++)
            {
                if (Networking.LocalPlayer.displayName == sDisplayName[i])
                {
                    index = i;
                    if (iCoin[i] == 0 && iChip[i] == 0)
                    {
                        playerData.chip = 20000;
                        playerData.coin = 0;
                    }
                    else
                    {
                        playerData.chip = iChip[i];
                        playerData.coin = iCoin[i];
                    }
                    return;
                }
            }
            index = -1;
        }

        public int Get_DataIndex(string name)
        {
            for (int i = 0; i < sDisplayName.Length; i++)
            {
                if (sDisplayName[i] == name)
                    return i;
            }
            return -1;
        }
        public int Get_DataCheckIndex(string name)
        {
            for (int i = 0; i < sDealer.Length; i++)
            {
                if (sDealer[i] == name)
                    return i;
            }
            return -1;
        }

        public int Get_DataLength()
        {
            for (int i = 0; i < sDisplayName.Length; i++)
            {
                if (sDisplayName[i] == "")
                    return i;
            }
            return sDisplayName.Length;
        }

        public void Set_Chip_Reset()
        {
            for (int i = 0; i < iChip.Length; i++)
                iChip[i] = 20000;
        }
    }
}