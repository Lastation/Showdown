using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace Holdem
{

    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class Ticket : UdonSharpBehaviour
    {
        [SerializeField] TextMeshPro tmp_text;
        [SerializeField] InstanceData instanceData;
        private int index = 0;

        private string[] ticketText = new string[]
        {
            "냥체 3분",
            "냥체 5분",
            "3인칭 3분",
            "3인칭 5분",
            "냥체 3분 +\n딜러의상변경 3분",
            "3인칭 3분 +\n딜러의상변경 3분",
            "냥체 5분 +\n딜러의상변경 5분",
            "3인칭 5분 +\n딜러의상변경 5분",
            "딜러의상변경",
            "애교권",
        };

        public override void OnPickupUseUp()
        {
            if (!instanceData.DealerCheck(Networking.LocalPlayer.displayName))
                return;

            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Text_" + index);
            index = index == ticketText.Length - 1 ? 0 : index + 1;
        }

        public void Text_0() => tmp_text.text = ticketText[0];
        public void Text_1() => tmp_text.text = ticketText[1];
        public void Text_2() => tmp_text.text = ticketText[2];
        public void Text_3() => tmp_text.text = ticketText[3];
        public void Text_4() => tmp_text.text = ticketText[4];
        public void Text_5() => tmp_text.text = ticketText[5];
        public void Text_6() => tmp_text.text = ticketText[6];
        public void Text_7() => tmp_text.text = ticketText[7];
        public void Text_8() => tmp_text.text = ticketText[8];
        public void Text_9() => tmp_text.text = ticketText[9];
    }
}
