
using System;
using System.Linq;
using System.Reflection;
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

        [UdonSynced] string[] sDisplayName = new string[100];
        [UdonSynced] int[] iChip = new int[100];
        [UdonSynced] int[] iCoin = new int[100];

        public int index = -1;

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
            for (int i = 0; i < sDisplayName.Length; i++)
            {
                sDisplayName[i] = "";
                iChip[i] = 0;
                iCoin[i] = 0;
            }
        }
        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (player == Networking.LocalPlayer) Load_Datas();

            if (!Networking.IsOwner(gameObject))
                return;

            for (int i = 0; i < sDisplayName.Length; i++)
            {
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
            if (index == -1) index = 0;
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

            for (int i = 0; i < sDisplayName.Length; i++)
            {
                if (Networking.LocalPlayer.displayName == sDisplayName[i])
                {
                    index = i;
                    playerData.chip = iChip[i];
                    playerData.coin = iCoin[i];
                    return;
                }
                if (sDisplayName[i] == "")
                {
                    index = i;
                    playerData.chip = 20000;
                    playerData.coin = 0;
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

        public int Get_DataLength()
        {
            for (int i = 0; i < sDisplayName.Length; i++)
            {
                if (sDisplayName[i] == "")
                    return i;
            }
            return sDisplayName.Length;
        }
    }
}