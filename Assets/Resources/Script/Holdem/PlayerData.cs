using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace Holdem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class PlayerData : UdonSharpBehaviour
    {
        [SerializeField] Text text_chip;
        [SerializeField] Text text_coin;
        [SerializeField] ChipUI chipUI;
        [SerializeField] Text textRebine;

        int _chip = 0;
        int _coin = 0;
        int _rebine = 0;
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
        public void Rebine()
        {
            if (chip > 200)
                return;

            if (rebine > 0)
            {
                rebine -= 1;
                chip = 20000;
            }
            if (coin >= 20)
            {
                coin -= 20;
                chip = 20000;
            }
        }
    }
}