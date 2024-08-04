using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace Holdem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class MicrophoneData : UdonSharpBehaviour
    {
        public InstanceData instanceData;
        [SerializeField] VRCPickup vrcPickup;
        [SerializeField] AudioSource asVoice;
        [SerializeField] bool isSound = true;


        public override void OnPickup()
        {
            if (!Networking.IsOwner(gameObject)) return;
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "SetPlayerVoice");
        }
        public override void OnDrop()
        {
            if (!Networking.IsOwner(gameObject)) return;
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ReSetPlayerVoice");
        }
        public void SetPlayerVoice()
        {
            if (isSound)
                asVoice.PlayOneShot(instanceData.getVoice(0));

            Networking.GetOwner(gameObject).SetVoiceDistanceFar(10000);
            Networking.GetOwner(gameObject).SetVoiceVolumetricRadius(1000);
            Networking.GetOwner(gameObject).SetVoiceGain(20);
        }
        public void ReSetPlayerVoice()
        {
            if (isSound)
                asVoice.PlayOneShot(instanceData.getVoice(1));

            Networking.GetOwner(gameObject).SetVoiceDistanceFar(25);
            Networking.GetOwner(gameObject).SetVoiceVolumetricRadius(0);
            Networking.GetOwner(gameObject).SetVoiceGain(15);
        }
        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (Networking.LocalPlayer.displayName != player.displayName) return;
            if (instanceData.DealerCheck(player.displayName)) vrcPickup.pickupable = true;
        }
    }
}