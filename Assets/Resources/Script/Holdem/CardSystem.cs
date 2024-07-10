using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace Holdem
{
    public enum CardPosition : int
    {
        Deck = 0,
        Flop1 = 1,
        Flop2 = 2,
        Flop3 = 3,
        Turn = 4,
        River = 5,
    }

    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class CardSystem : UdonSharpBehaviour
    {
        [SerializeField] CardData[] cardData;
        [SerializeField] Transform[] tf_Position;
        int[] shuffleIndex = new int[52];
        int seed = 0;

        private void Start() => Init_Card();

        public void Init_Card()
        {
            Set_CardPattern();

            if (!Networking.IsOwner(gameObject))
                return;

            for (int i = 0; i < shuffleIndex.Length; i++)
                shuffleIndex[i] = i;
            for (int i = 0; i < cardData.Length; i++)
                cardData[i].Set_Blind(true);
        }
        public void Reset_Card()
        {
            Reset_CardPosition();
            Shuffle_Card();
        }
        public void Shuffle_Card()
        {
            if (!Networking.IsOwner(gameObject))
                return;

            seed = Random.Range(int.MinValue, int.MaxValue);
            Random.InitState(seed);

            for (int i = 0; i < shuffleIndex.Length; i++)
            {
                int rand = Random.Range(i, shuffleIndex.Length);
                int tmp = shuffleIndex[i];
                shuffleIndex[i] = shuffleIndex[rand];
                shuffleIndex[rand] = tmp;
            }
        }
        public void Reset_CardPosition(bool isBlind = true)
        {
            Set_CardPattern();

            for (int i = 0; i < cardData.Length; i++)
            {
                cardData[i].tfvrcPickup.rotation = tf_Position[(int)CardPosition.Deck].rotation;
                cardData[i].tfvrcPickup.position = tf_Position[(int)CardPosition.Deck].position;
                cardData[i].Set_Blind(isBlind);
            }
        }
        public void Set_Pickupable(bool isPickupable)
        {
            for (int i = 0; i < cardData.Length; i++)
                cardData[i].Set_Pickupable(isPickupable);
        }
        public void Set_CardPattern()
        {
            for (int i = 0; i < cardData.Length; i++)
                cardData[i].Set_Card_Pattern();
        }
        public void Set_BlackJack()
        {
            Shuffle_Card();
            for (int i = 0; i < cardData.Length; i++)
                Set_CardPosition(i, tf_Position[0], false);
        }

        public void Set_CardPosition(int index, Transform cardPosition, bool isBlind)
        {
            cardData[index].tfvrcPickup.rotation = cardPosition.rotation;
            cardData[index].tfvrcPickup.position = cardPosition.position;
            cardData[index].Set_Card_Index(shuffleIndex[index]);
            cardData[index].Set_Blind(isBlind);
        }

        public void Set_CardPosition(int index, CardPosition cardPosition, bool isBlind)
        {
            cardData[index].tfvrcPickup.rotation = tf_Position[(int)cardPosition].rotation;
            cardData[index].tfvrcPickup.position = tf_Position[(int)cardPosition].position;
            cardData[index].Set_Card_Index(shuffleIndex[index]);
            cardData[index].Set_Blind(isBlind);
        }
        public void Set_CardRotation(int index) => cardData[index].Set_Rotation();

        public void Set_Blind(int index, bool isBlind)
        {
            cardData[index].Set_Blind(isBlind);
        }
        public int Get_CardIndex(int index) => cardData[index].Get_CardIndex();
        public int Get_Seed() => seed;

        public void Set_Owner(VRCPlayerApi value)
        {
            for (int i = 0; i < cardData.Length; i++)
                cardData[i].Set_Owner(value);

            if (value.IsOwner(gameObject)) return;
            Networking.SetOwner(value, gameObject);
        }
    }
}