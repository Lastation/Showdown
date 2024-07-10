using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace Holdem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class CardData : UdonSharpBehaviour
    {
        [SerializeField] VRCPickup vrcPickup;
        [SerializeField] SpriteRenderer sr_pattern;
        [SerializeField] MainSystem mainSystem;
        [UdonSynced] int i_cardIndex = 0;
        [UdonSynced] bool _isBlind = false;
        public bool isBlind
        {
            get => _isBlind;
            set
            {
                _isBlind = value;
                Update_Blind(isBlind);
            }
        }

        public void Start()
        {
            Set_Pickupable(false);
        }

        public void Update_Blind(bool value)
        {
            sr_pattern.sprite = value ? null : mainSystem.Get_CardPattern(i_cardIndex);
        }
        public void Set_Card_Pattern() => sr_pattern.sprite = mainSystem.Get_CardPattern(i_cardIndex);
        public void Set_Card_Index(int index)
        {
            i_cardIndex = index;
            RequestSerialization();
        }

        public int Get_CardIndex() => i_cardIndex;

        public Transform tfvrcPickup => vrcPickup.transform;
        public void Set_Rotation() => tfvrcPickup.eulerAngles = new Vector3(tfvrcPickup.eulerAngles.x + 180.0f, tfvrcPickup.eulerAngles.y, tfvrcPickup.eulerAngles.z);
        public void Set_Pickupable(bool value) => vrcPickup.pickupable = value;
        public void Set_Blind(bool value)
        {
            isBlind = value;
            RequestSerialization();
        }


        public override void OnDeserialization()
        {
            Set_Card_Pattern();
            Update_Blind(_isBlind);
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (!Networking.IsOwner(gameObject)) return;
            RequestSerialization();
        }

        public void Set_Owner(VRCPlayerApi value)
        {
            if (value.IsOwner(gameObject)) return;
            Networking.SetOwner(value, gameObject);
            Networking.SetOwner(value, vrcPickup.gameObject);
        }
    }
}