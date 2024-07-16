
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

namespace Holdem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class GotoHell : UdonSharpBehaviour
    {
        [UdonSynced] bool isUse = false;
        [SerializeField] Transform tf_prison;
        [SerializeField] Transform tf_spawnPoint;
        [SerializeField] InstanceData instanceData;

        public void SetUseUp()
        {
            if (!Networking.IsOwner(gameObject)) return;
            isUse = true;
            RequestSerialization();
        }
        public void SetUseDown()
        {
            if (!Networking.IsOwner(gameObject)) return;
            isUse = false;
            RequestSerialization();
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (!Networking.IsOwner(gameObject)) return;
            RequestSerialization();
        }

        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            if (!isUse) return;
            if (Networking.IsOwner(gameObject)) return;
            if (player != Networking.LocalPlayer) return;
            if (instanceData.DealerCheck(player.displayName)) return;
            player.TeleportTo(tf_prison.position, tf_prison.rotation);
            tf_spawnPoint.transform.position = tf_prison.position;
        }
    }
}