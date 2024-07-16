using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace Holdem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class Dealer_Weapon : UdonSharpBehaviour
    {
        [SerializeField] InstanceData instanceData;
        [SerializeField] VRCPickup vrcPickup;
        [SerializeField] GotoHell gotoHell;

        void Start()
        {
            if (instanceData.DealerCheck(Networking.LocalPlayer.displayName))
                vrcPickup.pickupable = true;
        }

        public override void OnPickupUseUp()
        {
            if (!Networking.IsOwner(gotoHell.gameObject))
                Networking.SetOwner(Networking.LocalPlayer, gotoHell.gameObject);
            gotoHell.SetUseUp();
        }
        public override void OnDrop()
        {
            gotoHell.SetUseDown();
        }
    }
}