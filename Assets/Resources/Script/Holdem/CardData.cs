using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon.Common;

namespace Holdem
{
    public enum TCardtype : int
    {
        normal = 0,
        blackjack = 1,
    }

    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class CardData : UdonSharpBehaviour
    {
        [SerializeField] VRCPickup vrcPickup;
        [SerializeField] SpriteRenderer sr_pattern;
        [SerializeField] MainSystem mainSystem;
        [UdonSynced] int i_cardIndex = 0;
        [UdonSynced] TCardtype cardtype = TCardtype.normal;

        public void Start()
        {
            Set_Pickupable(false);
        }
        public void Set_Card_Pattern()
        {
            switch (cardtype)
            {
                case TCardtype.normal:
                    sr_pattern.sprite = mainSystem.Get_CardPattern(i_cardIndex);
                    sr_pattern.material = mainSystem.Get_CardMaterial(i_cardIndex);
                    break;
                case TCardtype.blackjack:
                    sr_pattern.sprite = mainSystem.Get_CardPattern2(i_cardIndex);
                    sr_pattern.material = mainSystem.Get_CardMaterial(i_cardIndex, 2);
                    break;
            }
        }
        public void Set_Card_Index(int index, TCardtype cardtype)
        {
            i_cardIndex = index;
            this.cardtype = cardtype;
            Dosync();
        }

        public int Get_CardIndex() => i_cardIndex;

        public Transform tfvrcPickup => vrcPickup.transform;
        public void Set_Rotation() => tfvrcPickup.eulerAngles = new Vector3(tfvrcPickup.eulerAngles.x + 180.0f, tfvrcPickup.eulerAngles.y, tfvrcPickup.eulerAngles.z);
        public void Set_Pickupable(bool value) => vrcPickup.pickupable = value;

        public void Dosync()
        {
            Set_Card_Pattern();
            RequestSerialization();
        }

        public override void OnDeserialization()
        {
            Set_Card_Pattern();
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (!Networking.IsOwner(gameObject)) return;
            Dosync();
        }

        public void Set_Owner(VRCPlayerApi value)
        {
            if (value.IsOwner(gameObject)) return;
            Networking.SetOwner(value, gameObject);
            Networking.SetOwner(value, vrcPickup.gameObject);
        }
    }
}