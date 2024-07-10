using UdonSharp;
using Unity.Collections;

namespace Holdem
{
    public enum LocalizationType : int
    {
        KOR = 0,
        JP = 1,
        ENG = 2,
        Length,
    }

    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LocalizationData : UdonSharpBehaviour
    {
        [ReadOnly]
        public string[] sTipTextVR = new string[3]
        {
        "왼손 트리거를 빠르게 두 번 누르면 메뉴가 열립니다",
        "左手トリガーを素早く二回押すとメニューが開きます",
        "Quickly press the left hand trigger twice to open the menu"
        };
        [ReadOnly]
        public string[] sTipTextHand = new string[3]
        {
        "Tab키를 누르면서 메뉴를 조작해주세요",
        "Tabキーを押しながらメニューを操作してください",
        "Press the Tab key to manipulate the menu"
        };
        [ReadOnly]
        public string[] sHour = new string[3]
        {
        "시간",
        "時間",
        "Hour"
        };
        [ReadOnly]
        public string[] sMinute = new string[3]
        {
        "분",
        "分",
        "Min"
        };
        [ReadOnly]
        public string[] sBefore = new string[3]
        {
        " 전",
        "前",
        " ago"
        };

        [ReadOnly]
        public string[] sDayOfWeekKOR = new string[7]
        {
       "일요일", "월요일", "화요일", "수요일", "목요일", "금요일", "토요일"
        };
        [ReadOnly]
        public string[] sDayOfWeekJP = new string[7]
        {
       "日曜日", "月曜日", "火曜日", "水曜日", "木曜日", "金曜日", "土曜日"
        };
        [ReadOnly]
        public string[] sDayOfWeekENG = new string[7]
        {
       "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"
        };

        [ReadOnly]
        public string[] sJoinLog = new string[3]
        {
        "입장로그",
        "Joinログ",
        "Join log"
        };
        [ReadOnly]
        public string[] sVideoPlayer = new string[3]
        {
        "배경음악",
        "背景音楽",
        "background music"
        };
        [ReadOnly]
        public string[] sSwitch = new string[3]
        {
        "스위치",
        "スイッチ",
        "Switch"
        };

        [ReadOnly]
        public string[] sLanguage = new string[3]
        {
        "언어설정",
        "言語設定",
        "Language"
        };
        [ReadOnly]
        public string[] sMenu = new string[3]
        {
        "메뉴",
        "メニュー",
        "Menu"
        };
        [ReadOnly]
        public string[] sBack = new string[3]
        {
        "돌아가기",
        "戻る",
        "Back"
        };
        [ReadOnly]
        public string[] sCard = new string[3]
        {
        "카드 설정",
        "カード設定",
        "Card settings"
        };

        [ReadOnly]
        public string[] sDirectionalLight = new string[3]
        {
        "디렉셔널 라이트",
        "ディレクショナルライト",
        "Directional light"
        };
        [ReadOnly]
        public string[] sAvatarLight = new string[3]
        {
        "아바타 라이트",
        "アバターライト",
        "Avatar Light"
        };

        [ReadOnly]
        public string[] sTableStateProgress = new string[3]
        {
        "게임 진행중!",
        "ゲーム進行中",
        "In progress"
        };
        [ReadOnly]
        public string[] sTableStateWait = new string[3]
        {
        "게임 시작 대기중! 남은자리",
        "ゲーム開始待機中！残った席",
        "Waiting to start the game! Remaining seats"
        };

        [ReadOnly]
        public string[] sHankRankKOR = new string[11]
        {
        "하이카드",
        "원 페어",
        "투 페어",
        "트리플",
        "스트레이트",
        "플러쉬",
        "풀하우스",
        "포카드",
        "스트레이트 플러쉬",
        "백 스트레이트 플러쉬",
        "로얄 스트레이트 플러쉬"
        };
        [ReadOnly]
        public string[] sHankRankJP = new string[11]
        {
        "ハイカード",
        "ワンペア",
        "ツーペア",
        "スリーカード",
        "ストレート",
        "フラッシュ",
        "フルハウス",
        "フォーカード",
        "ストレートフラッシュ",
        "ストレートフラッシュ",
        "ロイヤルストレートフラッシュ"
        };
        [ReadOnly]
        public string[] sHankRankENG = new string[11]
        {
        "High card",
        "One pair",
        "Two pair",
        "Three of a kind",
        "Straight",
        "Flush",
        "Full house",
        "Four of a kind",
        "Straight flush",
        "Back Straight Flush",
        "Royal flush"
        };
    }
}