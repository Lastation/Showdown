using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;

namespace Holdem
{
    public enum handMenuIndex : int
    {
        JoinLog = 0,
        VideoPlayer = 1,
        Switch = 2,
        Menu = 3,
        Language = 4,
        Back = 5,
        Card = 6,
        DirectionalLight = 7,
        AvatarLight = 8,
    }
    public enum SE_Voice_Index : int
    {
        NULL = -1,
        Fold = 0,
        Call = 1,
        Check = 2,
        Raise = 3,
        Allin = 4,
        Bet = 5,
        Win = 6,
        Draw = 7,
        Turn = 8,
    }
    public enum SE_Table_Type : int
    {
        Basic = 0,
        Ming = 1,
        Doomchit = 2,
        Traial = 3,
        Sobar= 4,
        Koosuri = 5,
        Seriji = 6,
        Marine =7,
    }

    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class MainSystem : UdonSharpBehaviour
    {
        [SerializeField] PlayerData _playerData;
        public PlayerData playerData =>_playerData;

        private void Start()
        {
            switch (VRCPlayerApi.GetCurrentLanguage())
            {
                case "en":
                    Set_Language_ENG();
                    languageToggle[0].isOn = true;
                    break;
                case "ja":
                case "ja-JP":
                    Set_Language_JP();
                    languageToggle[1].isOn = true;
                    break;
                case "ko":
                case "ko-KR":
                    Set_Language_KOR();
                    languageToggle[2].isOn = true;
                    break;
                default:
                    Set_Language_ENG();
                    languageToggle[0].isOn = true;
                    break;
            }
        }
        #region Localization
        [SerializeField] LocalizationData localizationData;
        [SerializeField] Text[] textMenu;

        private LocalizationType _localizationType = LocalizationType.KOR;
        public LocalizationType localizationType
        {
            get => _localizationType;
            set
            {
                _localizationType = value;
                Update_Language(value);
            }
        }
        public void Set_Language_KOR() => localizationType = LocalizationType.KOR;
        public void Set_Language_JP() => localizationType = LocalizationType.JP;
        public void Set_Language_ENG() => localizationType = LocalizationType.ENG;
        public void Set_Language() => localizationType = (int)_localizationType + 1 >= (int)LocalizationType.Length ? LocalizationType.KOR : (LocalizationType)((int)_localizationType + 1);
        public LocalizationType Get_Language => _localizationType;
        public Toggle[] languageToggle;

        public void Update_Language(LocalizationType type)
        {
            textMenu[(int)handMenuIndex.JoinLog].text = localizationData.sJoinLog[(int)type];
            textMenu[(int)handMenuIndex.VideoPlayer].text = localizationData.sVideoPlayer[(int)type];
            textMenu[(int)handMenuIndex.Switch].text = localizationData.sSwitch[(int)type];
            textMenu[(int)handMenuIndex.Menu].text = localizationData.sMenu[(int)type];
            textMenu[(int)handMenuIndex.Language].text = localizationData.sLanguage[(int)type];
            textMenu[(int)handMenuIndex.Back].text = localizationData.sBack[(int)type];
            textMenu[(int)handMenuIndex.Card].text = localizationData.sCard[(int)type];
            textMenu[(int)handMenuIndex.DirectionalLight].text = localizationData.sDirectionalLight[(int)type];
            textMenu[(int)handMenuIndex.AvatarLight].text = localizationData.sAvatarLight[(int)type];
        }
        public string sTipTextVR => localizationData.sTipTextVR[(int)_localizationType];
        public string sTipTextHand => localizationData.sTipTextHand[(int)_localizationType];
        public string sHour => localizationData.sHour[(int)_localizationType];
        public string sMinute => localizationData.sMinute[(int)_localizationType];
        public string sBefore => localizationData.sBefore[(int)_localizationType];
        public string sDayOfWeek(int idx)
        {
            switch (_localizationType)
            {
                case LocalizationType.KOR:
                    return localizationData.sDayOfWeekKOR[idx];
                case LocalizationType.JP:
                    return localizationData.sDayOfWeekJP[idx];
                case LocalizationType.ENG:
                    return localizationData.sDayOfWeekENG[idx];
                default:
                    return null;
            }
        }
        public string sDirectionalLight => localizationData.sDirectionalLight[(int)_localizationType];
        public string sAvatarLight => localizationData.sAvatarLight[(int)_localizationType];
        public string sTableStateProgress => localizationData.sTableStateProgress[(int)_localizationType];
        public string sTableStateWait => localizationData.sTableStateWait[(int)_localizationType];

        public string sHandSuit(int idx)
        {
            switch (idx)
            {
                case 0:
                    return "♠";
                case 1:
                    return "♦";
                case 2:
                    return "♥";
                case 3:
                    return "♣";
            };
            return "";
        }
        public string sHandNumber(int idx)
        {
            switch (idx)
            {
                case 0:
                    return "2";
                case 1:
                    return "3";
                case 2:
                    return "4";
                case 3:
                    return "5";
                case 4:
                    return "6";
                case 5:
                    return "7";
                case 6:
                    return "8";
                case 7:
                    return "9";
                case 8:
                    return "10";
                case 9:
                    return "J";
                case 10:
                    return "Q";
                case 11:
                    return "K";
                case 12:
                    return "A";
            };
            return "";
        }
        public string sHankRank(int idx)
        {
            switch (_localizationType)
            {
                case LocalizationType.KOR:
                    return localizationData.sHankRankKOR[idx];
                case LocalizationType.JP:
                    return localizationData.sHankRankJP[idx];
                case LocalizationType.ENG:
                    return localizationData.sHankRankENG[idx];
                default:
                    return null;
            }
        }
        public string Get_HandRank(int value)
        {
            if (value == 0)
                return "";

            int rank = value / 1000;
            int number = (value % 1000) / 10;
            int suit = value % 10;

            return $"{sHandSuit(3 - suit)}{sHandNumber(number)} {sHankRank(rank)}";
        }
        #endregion

        #region Sound Effect
        [SerializeField] AudioClip[] acVoiceBasic;
        [SerializeField] AudioClip[] acVoiceMing;
        [SerializeField] AudioClip[] acVoiceDoomchit;
        [SerializeField] AudioClip[] acVoiceTraial;
        [SerializeField] AudioClip[] acVoiceSobar;
        [SerializeField] AudioClip[] acVoiceKoosuri;
        [SerializeField] AudioClip[] acVoiceSeriji;
        [SerializeField] AudioClip[] acVoiceMarine;
        [SerializeField] AudioSource asVoicePreview;

        SE_Table_Type _voiceType = SE_Table_Type.Basic;
        public SE_Table_Type voiceType
        {
            get => _voiceType;
            set => _voiceType = value;
        }

        public void Set_VoiceType_Basic() => Set_VoiceType(SE_Table_Type.Basic);
        public void Set_VoiceType_Ming() => Set_VoiceType(SE_Table_Type.Ming);
        public void Set_VoiceType_Doomchit() => Set_VoiceType(SE_Table_Type.Doomchit);
        public void Set_VoiceType_Traial() => Set_VoiceType(SE_Table_Type.Traial);
        public void Set_VoiceType_Sobar() => Set_VoiceType(SE_Table_Type.Sobar);
        public void Set_VoiceType_Koosuri() => Set_VoiceType(SE_Table_Type.Koosuri);
        public void Set_VoiceType_Seriji() => Set_VoiceType(SE_Table_Type.Seriji);
        public void Set_VoiceType_Marine() => Set_VoiceType(SE_Table_Type.Marine);
        public void Set_VoiceType(SE_Table_Type type)
        {
            voiceType = type;
            asVoicePreview.clip = GetAudioClip(SE_Voice_Index.Call);
            asVoicePreview.Play();
        }
        private AudioClip Get_AudioClip_Basic(SE_Voice_Index index) => acVoiceBasic[(int)index];
        private AudioClip Get_AudioClip_Ming(SE_Voice_Index index)
        {
            if (acVoiceMing.Length - 1 < (int)index) return Get_AudioClip_Basic(index);
            if (acVoiceMing[(int)index] != null)
                return acVoiceMing[(int)index];
            return Get_AudioClip_Basic(index);
        }
        private AudioClip Get_AudioClip_Doomchit(SE_Voice_Index index)
        {
            if (acVoiceDoomchit.Length - 1 < (int)index) return Get_AudioClip_Basic(index);
            if (acVoiceDoomchit[(int)index] != null)
                return acVoiceDoomchit[(int)index];
            return Get_AudioClip_Basic(index);
        }
        private AudioClip Get_AudioClip_Traial(SE_Voice_Index index)
        {
            if (acVoiceTraial.Length - 1 < (int)index) return Get_AudioClip_Basic(index);
            if (acVoiceTraial[(int)index] != null)
                return acVoiceTraial[(int)index]; 
            return Get_AudioClip_Basic(index);
        }
        private AudioClip Get_AudioClip_Sobar(SE_Voice_Index index)
        {
            if (acVoiceSobar.Length - 1 < (int)index) return Get_AudioClip_Basic(index);
            if (acVoiceSobar[(int)index] != null)
                return acVoiceSobar[(int)index];
            return Get_AudioClip_Basic(index);
        }
        private AudioClip Get_AudioClip_Koosuri(SE_Voice_Index index)
        {
            if (acVoiceKoosuri.Length - 1 < (int)index) return Get_AudioClip_Basic(index);
            if (acVoiceKoosuri[(int)index] != null)
                return acVoiceKoosuri[(int)index];
            return Get_AudioClip_Basic(index);
        }
        private AudioClip Get_AudioClip_Seriji(SE_Voice_Index index)
        {
            if (acVoiceSeriji.Length - 1 < (int)index) return Get_AudioClip_Basic(index);
            if (acVoiceSeriji[(int)index] != null)
                return acVoiceSeriji[(int)index];
            return Get_AudioClip_Basic(index);
        }
        private AudioClip Get_AudioClip_Marine(SE_Voice_Index index)
        {
            if (acVoiceMarine.Length - 1 < (int)index) return Get_AudioClip_Basic(index);
            if (acVoiceMarine[(int)index] != null)
                return acVoiceMarine[(int)index];
            return Get_AudioClip_Basic(index);
        }
        public AudioClip GetAudioClip(SE_Table_Type voiceType, SE_Voice_Index index)
        {
            switch (voiceType)
            {
                case SE_Table_Type.Basic:
                    return Get_AudioClip_Basic(index);
                case SE_Table_Type.Ming:
                    return Get_AudioClip_Ming(index);
                case SE_Table_Type.Doomchit:
                    return Get_AudioClip_Doomchit(index);
                case SE_Table_Type.Traial:
                    return Get_AudioClip_Traial(index);
                case SE_Table_Type.Sobar:
                    return Get_AudioClip_Sobar(index);
                case SE_Table_Type.Koosuri:
                    return Get_AudioClip_Koosuri(index);
                case SE_Table_Type.Seriji:
                    return Get_AudioClip_Seriji(index);
                case SE_Table_Type.Marine:
                    return Get_AudioClip_Marine(index);
                default:
                    return Get_AudioClip_Basic(index);
            }
        }

        public AudioClip GetAudioClip(SE_Voice_Index index) => GetAudioClip(_voiceType, index);
        #endregion

        #region Card Sprite
        [SerializeField] Sprite[] imgPattern;
        [SerializeField] Color[] colorCard;
        [SerializeField] Material[] matCardPattern;
        [SerializeField] Slider sliderCardEmission;
        int iPatternIndex = 0;

        public void Set_Color_Pattern_White() => Set_Color_Pattern(0);
        public void Set_Color_Pattern_Blue() => Set_Color_Pattern(1);
        public void Set_Color_Pattern_Red() => Set_Color_Pattern(2);
        public void Set_Color_Pattern_Orange() => Set_Color_Pattern(3);
        public void Set_Color_Pattern_Green() => Set_Color_Pattern(4);

        public void Set_Color_Pattern(int index)
        {
            iPatternIndex = index;
            Set_Color();
        }
        public void Set_Color()
        {
            matCardPattern[0].SetColor("_EmissionColor", colorCard[iPatternIndex] * sliderCardEmission.value);
            matCardPattern[1].SetColor("_Color", colorCard[iPatternIndex] * sliderCardEmission.value);
        }
        public Sprite Get_CardPattern(int idx) => imgPattern[idx];
        #endregion

        #region PlayerConfig
        public PlayerUI playerUI { get; set; }
        [SerializeField] Slider sliderDisplayHeight;
        [SerializeField] Slider sliderColliderHeight;
        [SerializeField] GameObject objColliderHeight;
        public void Set_Display_Height()
        {
            if (playerUI == null) return;
            playerUI.Set_UI_Height(true);
        }
        public float Get_Display_Height() => sliderDisplayHeight.value - 0.5f + objColliderHeight.transform.localPosition.y;
        public void Set_Collider_Height()
        {
            objColliderHeight.transform.localPosition = new Vector3(0, sliderColliderHeight.value, 0);

            if (playerUI == null) return;
            playerUI.Set_UI_Height(true);
        }
        #endregion
    }
}