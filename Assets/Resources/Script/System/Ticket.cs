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
            "냥체",
            "뿅체",
            "멍체",
            "선택 애교권",
            "랜덤 애교권",
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
    }
}
