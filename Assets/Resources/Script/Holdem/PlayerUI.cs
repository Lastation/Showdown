using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;

namespace Holdem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class PlayerUI : UdonSharpBehaviour
    {
        [SerializeField] MainSystem mainSystem;
        [SerializeField] TextMeshProUGUI textDisplayName, textChip;
        [SerializeField] TextMeshProUGUI textCall, textRaise;
        [SerializeField] TextMeshProUGUI textHandrank, textTime;

        [SerializeField] Image[] img_button;
        [SerializeField] Image[] img_cards;

        [SerializeField] TextMeshProUGUI[] textRaiseOption;
        [SerializeField] TextMeshPro text_spectator;
        [SerializeField] TextMeshPro textState, textPotSize;

        [SerializeField] GameObject objUIPlay, objUIJoin, objUIGame;

        int _handRank = 0;
        public int handRank
        {
            get => _handRank;
            set
            {
                _handRank = value;
                textHandrank.text = mainSystem.Get_HandRank(_handRank);
            }
        }
        bool isDisplayToggle = false;

        Color color_button = new Color(78.0f/255.0f, 119.0f / 255.0f, 193.0f / 255.0f, 1f);

        string[] s_playerState = new string[8] { "", "Wait", "Turn", "Call", "Check", "Raise", "ALLIN", "Fold" };

        public void Set_Rotation()
        {
            Vector3 relativePos = Networking.LocalPlayer.GetPosition() - objUIGame.transform.position;
            relativePos.y = 1.0f;
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            objUIGame.transform.rotation = rotation;
        }
        public void Update_Timer(float time)
        {
            textTime.text = (30.0f - time).ToString("0.0");
            if (time > 25) textTime.color = Color.red;
            else if (time > 20) textTime.color = Color.yellow;
            else if (time > 10) textTime.color = Color.white;
            else textTime.color = Color.green;
        }
        public void Update_DisplayName(string displayName)
        {
            textDisplayName.text = displayName == "" ? "Join" : displayName;
            Set_Player_Spectator();
        }
        public void Update_Chip(int tablePlayerChip)
        {
            textChip.text = textDisplayName.text == "" ? "" : tablePlayerChip.ToString();
            Set_Player_Spectator();
        }

        public void Set_StateText(PlayerState playerState, TableState tableState)
        {
            if (textChip.text == "")
            {
                textState.text = "";
                textState.color = Color.white;
                return;
            }

            if (tableState == TableState.Wait && playerState != PlayerState.OutOfGame && playerState != PlayerState.Fold)
                textState.text = mainSystem.Get_HandRank(handRank);
            else
                textState.text = $"{s_playerState[(int)playerState]}";

            if (textState.text == "Fold")
                textState.color = Color.red;
            else if (textState.text == "Wait")
                textState.color = Color.yellow;
            else
                textState.color = Color.white;

            Set_Player_Spectator();
        }

        public void UpdateHandRank()
        {
            textState.text = mainSystem.Get_HandRank(handRank);
        }
        public void Set_BetSize(int size)
        {
            textPotSize.text = size == 0 ? "" : $"{size}";
            Set_Player_Spectator();
        }
        public void Set_Player_Spectator()
        {
            text_spectator.text = $"{textDisplayName.text}\n{textState.text}\n{textPotSize.text}";

            if (textState.text == "Fold")
                text_spectator.color = Color.red;
            else if (textState.text == "Wait")
                text_spectator.color = Color.yellow;
            else
                text_spectator.color = Color.white;
        }
        public void Set_Button_Color(bool isTurn)
        {
            for (int i = 0; i < img_button.Length; i++)
            {
                if (i == 2) // fold
                    img_button[i].color = isTurn ? new Color(1.0f, 0.427451f, 0.4494184f, 1f) : Color.black;
                else if (i == 3) // reset
                    img_button[i].color = isTurn ? new Color(0.8784314f, 0.7647059f, 0.07058824f, 1f) : Color.black;
                else
                    img_button[i].color = isTurn ? color_button : Color.black;
            }
        }
        public void Set_CallText(int value)
        {
            if (!isDisplayToggle)
                textCall.text = value == 0 ? $"check" : $"call [ {value} ]";
            else
                textCall.text = value == 0 ? $"check" : $"call [ {(double)value / 200}BB ]";
        }
        public void Set_CallText_Allin(int value)
        {
            if (!isDisplayToggle)
                textCall.text = $"all in [ {value} ]";
            else
                textCall.text = $"all in [ {(double)value / 200}BB ]";
        }
        public void Set_RaiseText(int value)
        {
            if (!isDisplayToggle)
                textRaise.text = $"raise [ {value} ]";
            else
                textRaise.text = $"raise [ {(double)value / 200}BB ]";
        }
        public void Set_RaiseText_Allin(int value)
        {
            if (!isDisplayToggle)
                textRaise.text = $"all in [ {value} ]";
            else
                textRaise.text = $"all in [ {(double)value / 200}BB ]";
        }
        public void Set_CardImage(int[] table_Cards, int idx)
        {
            if (!Networking.IsOwner(gameObject))
                return;

            img_cards[0].sprite = mainSystem.Get_CardPattern(table_Cards[0 + idx]);
            img_cards[1].sprite = mainSystem.Get_CardPattern(table_Cards[9 + idx]);
            img_cards[2].sprite = mainSystem.Get_CardPattern(table_Cards[18]);
            img_cards[3].sprite = mainSystem.Get_CardPattern(table_Cards[19]);
            img_cards[4].sprite = mainSystem.Get_CardPattern(table_Cards[20]);
            img_cards[5].sprite = mainSystem.Get_CardPattern(table_Cards[21]);
            img_cards[6].sprite = mainSystem.Get_CardPattern(table_Cards[22]);

            img_cards[0].material = mainSystem.Get_CardMaterial(table_Cards[0 + idx], 1);
            img_cards[1].material = mainSystem.Get_CardMaterial(table_Cards[9 + idx], 1);
            img_cards[2].material = mainSystem.Get_CardMaterial(table_Cards[18], 1);
            img_cards[3].material = mainSystem.Get_CardMaterial(table_Cards[19], 1);
            img_cards[4].material = mainSystem.Get_CardMaterial(table_Cards[20], 1);
            img_cards[5].material = mainSystem.Get_CardMaterial(table_Cards[21], 1);
            img_cards[6].material = mainSystem.Get_CardMaterial(table_Cards[22], 1);
        }
        public void Set_DisplayActive(bool value) => objUIPlay.SetActive(value);
        public void Set_UI_Height(bool value)
        {
            float height = Networking.LocalPlayer.GetAvatarEyeHeightAsMeters() - 0.75f;
            objUIJoin.transform.localPosition =
                new Vector3(
                    objUIJoin.transform.localPosition.x,
                    value ? height + mainSystem.Get_Display_Height() : .0f,
                    objUIJoin.transform.localPosition.z);
            objUIPlay.transform.localPosition =
                new Vector3(
                    objUIPlay.transform.localPosition.x,
                    value ? height + mainSystem.Get_Display_Height() - 0.025f : -0.025f,
                    objUIPlay.transform.localPosition.z);
        }
        public void Set_Owner(VRCPlayerApi player)
        {
            if (player.IsOwner(gameObject)) return;
            Networking.SetOwner(player, gameObject);
        }
    }
}