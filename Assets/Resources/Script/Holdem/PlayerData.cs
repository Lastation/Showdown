using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.Udon.Serialization.OdinSerializer.Utilities;

namespace Holdem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class PlayerData : UdonSharpBehaviour
    {
        [SerializeField] Text text_chip;
        [SerializeField] Text text_coin;
        [SerializeField] ChipUI chipUI;
        [SerializeField] Text textRebine;
        [SerializeField] Text[] textGacha;
        [SerializeField] Text[] textGacha10;
        [SerializeField] Text[] textRenalty;

        int _chip = 0;
        int _coin = 0;
        int _gacha = 0;
        int _rebine = 0;
        int _penalty = 0;

        public int gacha
        {
            get => _gacha;
            set
            {
                _gacha = value;

                for (int i = 0; i < textGacha.Length; i++)
                    textGacha[i].text = _gacha > 0 ? $"티켓 1장" : "10 Coin";
                for (int i = 0; i < textGacha10.Length; i++)
                    textGacha10[i].text = _gacha > 0 ? $"티켓 {_gacha}장" : "100 Coin";
            }
        }
        public int rebine {
            get => _rebine;
            set
            {
                _rebine = value; 
                if (_rebine > 0)
                    textRebine.text = $"{_rebine} 개";
                else
                    textRebine.text = "20";
            }
        }
        public int penalty
        {
            get => _penalty;
            set
            {
                _penalty = value;

                for (int i = 0; i < textRenalty.Length; i++)
                    textRenalty[i].text = _penalty > 0 ? $"티켓 {_penalty}장" : "40 Coin";
            }
        }

        bool _isPlayGame = false;

        public int chip
        {
            get => _chip;
            set
            {
                _chip = value;
                text_chip.text = _chip.ToString();
                chipUI.ApplyText(_chip, _coin);
            }
        }
        public int coin
        {
            get => _coin;
            set
            {
                _coin = value;
                text_coin.text = _coin.ToString();
                chipUI.ApplyText(_chip, _coin);
            }
        }
        public bool isPlayGame
        {
            get => _isPlayGame;
            set
            {
                _isPlayGame = value;
            }
        }

        public void Start()
        {
            text_chip.text = _chip.ToString();
            text_coin.text = _coin.ToString();
        }

        public void Reset_Chip()
        {
            int Reward = Mathf.Min(Mathf.FloorToInt(chip / 10000), 20);

            gacha += Reward;
            chip = 20000;
        }
    }
}