﻿using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace Holdem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class DealerUI : UdonSharpBehaviour
    {
        [SerializeField] MainSystem mainSystem;

        [SerializeField] TextMeshProUGUI text_Dealer;
        [SerializeField] TextMeshProUGUI text_TableState;
        [SerializeField] TextMeshProUGUI text_TablePot;
        [SerializeField] TextMeshProUGUI[] text_TablePlayerName;
        [SerializeField] TextMeshPro text_spectator;
        [SerializeField] TextMeshPro[] text_TablePlayerPot;

        public void FixedUpdate() => Set_Rotation();
        private void Set_Rotation()
        {
            Vector3 relativePos = Networking.LocalPlayer.GetPosition() - transform.position;
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            transform.rotation = rotation;
        }

        public void Set_Dealer_Displayname(string value)
        {
            text_Dealer.text = $"Dealer : {value}";
            Set_Table_Spectator();
        }
        public void Set_TableState(int count, bool isProgress)
        {
            switch (isProgress)
            {
                case true:
                    text_TableState.text = mainSystem.sTableStateProgress;
                    break;
                case false:
                    text_TableState.text = $"{mainSystem.sTableStateWait} : {count}";
                    break;
            }
            Set_Table_Spectator();
        }
        public void Set_TablePot(int value)
        {
            text_TablePot.text = value == 0 ? "" : $"Table Pot : {value}";
            Set_Table_Spectator();
        }

        public void Set_Table_Spectator() => text_spectator.text = $"{text_Dealer.text}\n{text_TableState.text}\n{text_TablePot.text}";

        public void Set_Table_PlayerPot(int[] tableSidePot)
        {
            for (int i = 0; i < text_TablePlayerPot.Length; i++) text_TablePlayerPot[i].text = $"totalPot : {tableSidePot[i]}";
        }
        public void Set_PlayerName(string value, int index) => text_TablePlayerName[index].text = value;
    }
}