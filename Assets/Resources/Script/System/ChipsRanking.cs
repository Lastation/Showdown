
using System;
using System.Reflection;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace Holdem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class ChipsRanking : UdonSharpBehaviour
    {
        [SerializeField] private float UpdateSpan = 5f;
        [SerializeField] private bool isRanking = true;
        [SerializeField] private bool changingColor = true;
        [SerializeField] private Color goldColor = new Color(180f / 255f, 152f / 255f, 27f / 255f, 1f);
        [SerializeField] private Color silverColor = new Color(192f / 255f, 192f / 255f, 192f / 255f, 1f);
        [SerializeField] private Color copperColor = new Color(184f / 255f, 115f / 255f, 51f / 255f, 1f);
        [SerializeField] private Color otherColor = new Color(100f / 255f, 100f / 255f, 100f / 255f, 1f);
        [SerializeField] InstanceData instanceData;
        [SerializeField] ChipsRankingPlayer[] players;
        float nextTime = 0;
        private bool isStarted = false;
        private bool isLoaded = false;

        void Update()
        {
            if (!gameObject.activeSelf) return;
            if (isStarted == false) return;

            if (nextTime == 0f || Time.time >= nextTime)
            {
                nextTime = Time.time + UpdateSpan; 

                if (instanceData.index != -1)
                    players[instanceData.index].SyncData();

                if (isRanking) SortPlayers();
            }
        }

        int[] playersID = new int[100];
        int[] playersCoin = new int[100];

        #region QuickSort
        private void QuickSort(int low, int high)
        {
            if (low < high)
            {
                int pivotIndex = Partition(low, high);
                QuickSort(low, pivotIndex - 1);
                QuickSort(pivotIndex + 1, high);
            }
        }

        private int Partition(int low, int high)
        {
            int pivot = playersCoin[high];
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                if (playersCoin[j] <= pivot)
                {
                    i++;
                    Swap(i, j);
                }
            }
            Swap(i + 1, high);
            return i + 1;
        }

        private void Swap(int a, int b)
        {
            int tempCoin = playersCoin[a];
            playersCoin[a] = playersCoin[b];
            playersCoin[b] = tempCoin;

            int tempIndex = playersID[a];
            playersID[a] = playersID[b];
            playersID[b] = tempIndex;
        }
        public void SelectionSort(int dataCount)
        {
            int tmpID;
            int tmpUC;
            for (int i = 0; i < dataCount - 1; i++)
            {
                for (int j = dataCount - 1; j > i; j--)
                {
                    if (playersCoin[j] < playersCoin[j - 1])
                    {
                        tmpID = playersID[j];
                        tmpUC = playersCoin[j];
                        playersID[j] = playersID[j - 1];
                        playersCoin[j] = playersCoin[j - 1];
                        playersID[j - 1] = tmpID;
                        playersCoin[j - 1] = tmpUC;
                    }
                }
            }
        }
        #endregion

        public void SortPlayers()
        {
            int dataCount = instanceData.Get_DataLength();

            for (int i = 0; i < dataCount; i++)
            {
                playersID[i] = players[i].index;
                playersCoin[i] = players[i].coin;
            }

            SelectionSort(dataCount);
            //QuickSort(0, dataCount - 1);

            for (int i = 0; i < dataCount; i++)
            {
                if (players[playersID[i]].displayName == "") continue;

                players[playersID[i]].gameObject.transform.SetAsFirstSibling();
                if (changingColor == true)
                {
                    if (i == dataCount - 1)
                        players[playersID[i]].ChangeColor(goldColor, dataCount - i);
                    else if (i == dataCount - 2)
                        players[playersID[i]].ChangeColor(silverColor, dataCount - i);
                    else if (i == dataCount - 3)
                        players[playersID[i]].ChangeColor(copperColor, dataCount - i);
                    else
                        players[playersID[i]].ChangeColor(otherColor, dataCount - i);
                }
            }
        }

        #region PlayerChipReset
        public void Set_Reset()
        {
            if (!Networking.IsOwner(gameObject)) return;

            instanceData.Set_Chip_Reset();
            for (int i = 0; i < instanceData.Get_DataLength(); i++)
            {
                if (Networking.IsOwner(players[i].gameObject))
                    players[i].ResetChipLeft();
                else
                    players[i].SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ResetChip");
            }
        }
        #endregion
        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (player != Networking.LocalPlayer)
                return;
            Load_Datas();
        }
        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            if (!Networking.IsOwner(instanceData.gameObject))
                return;
            int index = instanceData.Get_DataIndex(player.displayName);
            if (index == -1) return;
            instanceData.Save_Datas(player.displayName, players[index].chip, players[index].coin);
        }
        public void Load_Datas()
        {
            if (isLoaded) return;
            if (instanceData.index == -1)
            {
                SendCustomEventDelayedSeconds("Load_Datas", 2.0f);
                return;
            }

            players[instanceData.index].SetPlayer(instanceData.index);
            isStarted = true;
            isLoaded = true;
        }
    }
}