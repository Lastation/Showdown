
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace Holdem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class Mirror : UdonSharpBehaviour
    {
        [SerializeField] private GameObject[] objActive;
        [SerializeField] private Toggle[] objToggle;
        
        public override void OnPlayerTriggerExit(VRCPlayerApi player)
        {
            if (player != Networking.LocalPlayer)
                return;
            for (int i = 0; i < objActive.Length; i++) objActive[i].SetActive(false);
            for (int i = 0; i < objToggle.Length; i++) objToggle[i].isOn =false;
        }
    }
}