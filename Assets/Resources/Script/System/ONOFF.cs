
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace Holdem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class ONOFF : UdonSharpBehaviour
    {
        [SerializeField] InstanceData instanceData;
        [SerializeField] GameObject obj_Switch;
        [SerializeField] GameObject obj_Target;
        [UdonSynced] bool switchState = false;

        void Start()
        {
            if (!instanceData.DealerCheck(Networking.LocalPlayer.displayName))
                obj_Switch.SetActive(false);
        }

        public void ON()
        {
            if (!instanceData.DealerCheck(Networking.LocalPlayer.displayName))
                return;

            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            switchState = true;
            Dosync();
        }
        public void OFF()
        {
            if (!instanceData.DealerCheck(Networking.LocalPlayer.displayName))
                return;

            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            switchState = false;
            Dosync();
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (Networking.IsOwner(gameObject))
                Dosync();
        }

        public void Dosync()
        {
            SyncAction();
            RequestSerialization();
        }

        public override void OnDeserialization()
        {
            SyncAction();
        }

        public void SyncAction()
        {
            if (obj_Target == null)
                return;
            obj_Target.SetActive(!switchState);
        }

    }
}
