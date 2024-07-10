using Newtonsoft.Json.Linq;
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;

namespace Holdem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class ChipsRankingPlayer : UdonSharpBehaviour
    {
        [SerializeField] Text PlayerRankText = null;
        [SerializeField] Text PlayerNameText = null;
        [SerializeField] Text PlayerChipsText = null;
        [SerializeField] Text PlayerCoinsText = null;
        [SerializeField] PlayerData playerData;

        [UdonSynced]
        int _index = 0;
        [UdonSynced]
        int _chip = 0;
        [UdonSynced]
        int _coin = 0;
        [UdonSynced]
        string _displayName = "";

        public int index
        {
            get => _index;
            set => _index = value;
        }

        public int chip
        {
            get => _chip;
            set
            {
                _chip = value;
                PlayerChipsText.text = value.ToString("F0");
            }
        }
        public int coin
        {
            get => _coin;
            set
            {
                _coin = value;
                PlayerCoinsText.text = value.ToString("F0");
            }
        }

        public string displayName
        {
            get => _displayName;
            set
            {
                _displayName = value;
                PlayerNameText.text = value;
                Dosync();
            }
        }
        public void ChangeColor(Color color, int index = 99)
        {
            if (PlayerNameText.text == "")
            {
                PlayerRankText.text = "";
                PlayerChipsText.text = "";
                PlayerCoinsText.text = "";
                return;
            }

            string sub = "th";
            PlayerRankText.color = color;
            switch (index)
            {
                case 1:
                case 21:
                case 31:
                    sub = "st";
                    break;
                case 2:
                case 22:
                case 32:
                    sub = "nd";
                    break;
                case 3:
                case 23:
                case 33:
                    sub = "rd";
                    break;
            }
            PlayerRankText.text = (index).ToString() + sub;
        }

        public void SetPlayer(int idx)
        {
            if (Networking.LocalPlayer.IsOwner(gameObject)) return;
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            index = idx;
            SyncData();
        }
        public void SyncData()
        {
            displayName = Networking.LocalPlayer.displayName;
            coin = playerData.coin;
            chip = playerData.chip;
            RequestSerialization();
        }
        public void Dosync()
        {
            RequestSerialization();
        }

        public override void OnDeserialization()
        {
            PlayerNameText.text = displayName;

            if (displayName != "")
            {
                PlayerChipsText.text = chip.ToString("F0");
                PlayerCoinsText.text = coin.ToString("F0");
            }
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (Networking.IsOwner(gameObject))
            {
                SendCustomEventDelayedSeconds(nameof(Dosync), 5f);
            }
        }
    }
}