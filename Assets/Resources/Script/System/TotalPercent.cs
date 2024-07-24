
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace Holdem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class TotalPercent : UdonSharpBehaviour
    {
        [UdonSynced] int[] gachaTotal = new int[9];
        [UdonSynced] int[] penaltyTotal = new int[12];

        [SerializeField] Text gachaText;
        [SerializeField] Text penaltyText;

        public void Start()
        {
            for (int i = 0; i < gachaTotal.Length; i++)
                gachaTotal[i] = 0;

            for (int i = 0; i < penaltyTotal.Length; i++)
                penaltyTotal[i] = 0;
        }

        public void Add_Gacha_Index(int idx, int value)
        {
            if (!Networking.IsOwner(gameObject))
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
            gachaTotal[idx] += value;
            Dosync();
        }

        public void SyncGachaText()
        {
            gachaText.text = string.Concat(
                "총 소모 코인: ", gachaTotal[0], "코인", "\n",
                "총 소모 티켓: ", gachaTotal[1], "개", "\n\n",
                "1 - 5코인: ", gachaTotal[2], "개", "\n",
                "5 - 10코인: ", gachaTotal[3], "개", "\n",
                "10- 15코인: ", gachaTotal[4], "개", "\n",
                "리바인권: ", gachaTotal[5], "개", "\n",
                "벌칙룰렛 1회: ", gachaTotal[6], "개", "\n",
                "15코인 - 50코인: ", gachaTotal[7], "개", "\n",
                "딜러 의상 변경: ", gachaTotal[8], "개");
        }

        public void Add_Penalty_Index(int idx, int value)
        {
            if (!Networking.IsOwner(gameObject))
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
            penaltyTotal[idx] += value;
            Dosync();
        }

        public void SyncPenaltyText()
        {
            penaltyText.text = string.Concat(
                "총 소모 코인: ", penaltyTotal[0], "코인", "\n",
                "총 소모 티켓: ", penaltyTotal[1], "개", "\n\n",
                "1 - 20코인: ", penaltyTotal[2], "개", "\n",
                "20 - 40코인: ", penaltyTotal[3], "개", "\n",
                "냥체 3분: ", penaltyTotal[4], "개", "\n",
                "냥체 5분: ", penaltyTotal[5], "개", "\n",
                "3인칭 3분: ", penaltyTotal[6], "개", "\n",
                "3인칭 5분: ", penaltyTotal[7], "개", "\n",
                "냥체 3분 + 딜러의상변경 3분: ", penaltyTotal[8], "개", "\n",
                "냥체 5분 + 딜러의상변경 5분: ", penaltyTotal[9], "개", "\n",
                "3인칭 3분 + 딜러의상변경 3분: ", penaltyTotal[10], "개", "\n",
                "3인칭 5분 + 딜러의상변경 5분: ", penaltyTotal[11], "개");
        }

        private void Dosync()
        {
            SyncGachaText();
            SyncPenaltyText();
            RequestSerialization();
        }

        public override void OnDeserialization()
        {
            SyncGachaText();
            SyncPenaltyText();
        }
    }
}