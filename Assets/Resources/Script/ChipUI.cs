using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

[UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
public class ChipUI : UdonSharpBehaviour
{
    [SerializeField]
    private Text tmp_chip = null;
    [SerializeField]
    private Text tmp_coin = null;

    [SerializeField]
    private Animator animator;

    int nowMoney = 0, prevMoney = 0;
    int nowCoin = 0, prevCoin = 0;

    public void ApplyText(int chip, int coin)
    {
        int subChip = chip - prevMoney;
        int subCoin = coin - prevCoin;

        animator.SetTrigger("ShowUI");
        nowMoney = chip;
        nowCoin = coin;
        prevMoney = nowMoney;
        prevCoin = nowCoin;

        tmp_chip.text = string.Concat("<b>", prevMoney, "</b> [", (subChip < 0 ? "<color=#FF7B5A>-" : "<color=#FFE223>+"), Mathf.Abs(subChip), "</color>]");
        tmp_coin.text = string.Concat("<b>", prevCoin, "</b> [", (subCoin < 0 ? "<color=#FF7B5A>-" : "<color=#FFE223>+"), Mathf.Abs(subCoin), "</color>]");
    }
}