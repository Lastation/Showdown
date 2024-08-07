
using UdonSharp;
using UnityEngine;

namespace Holdem
{
    public class ChipSystem : UdonSharpBehaviour
    {
        DealerSystem dealerSystem;

        [SerializeField] GameObject[] obj_chip_100;
        [SerializeField] GameObject[] obj_chip_1k;
        [SerializeField] GameObject[] obj_chip_10k;
        [SerializeField] GameObject[] obj_chip_100k;

        public void UpadateChip()
        {
            for (int i = 0; i < dealerSystem.getPlayerSystem.Length; i++)
            {
                PlayerSystem playerSystem = dealerSystem.getPlayerSystem[i];
                int betSize = playerSystem.betSize;
                int chipSize = playerSystem.chip;
            }
        }
    }
}