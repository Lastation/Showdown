
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;

namespace Holdem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class DealerAttend : UdonSharpBehaviour
    {
        [SerializeField] InstanceData instanceData;
        [SerializeField] GameObject objActive, objAttend;
        [SerializeField] Text textName, textCharming, textCloth, textPanalty;
        [UdonSynced] bool isAttend = false;
        [UdonSynced] bool isCharming = false;
        [UdonSynced] bool isCloth = false;
        [UdonSynced] int iPanalty = 5;

        public void Start()
        {
            textName.text = gameObject.name;
            SyncFunction();
        }

        public void SetAttend()
        {
            if (!instanceData.DealerCheck(Networking.LocalPlayer.displayName))
                return;

            if (!Networking.IsOwner(gameObject))
                Networking.SetOwner(Networking.LocalPlayer, gameObject);

            isAttend = !isAttend;
            Dosync();
        }
        public void SetCharming()
        {
            if (!instanceData.DealerCheck(Networking.LocalPlayer.displayName))
                return;

            if (!Networking.IsOwner(gameObject))
                Networking.SetOwner(Networking.LocalPlayer, gameObject);

            isCharming = !isCharming;
            Dosync();
        }

        public void SetCloth()
        {
            if (!instanceData.DealerCheck(Networking.LocalPlayer.displayName))
                return; 

            if (!Networking.IsOwner(gameObject))
                Networking.SetOwner(Networking.LocalPlayer, gameObject);

            isCloth = !isCloth;
            Dosync();
        }
        public void SetPanalty()
        {
            if (!instanceData.DealerCheck(Networking.LocalPlayer.displayName))
                return;

            if (!Networking.IsOwner(gameObject))
                Networking.SetOwner(Networking.LocalPlayer, gameObject);

            iPanalty = iPanalty > 0 ? iPanalty - 1 : 5;
            Dosync();
        }
        public void Dosync()
        {
            SyncFunction();
            RequestSerialization();
        }

        public override void OnDeserialization()
        {
            SyncFunction();
        }
        public void SyncFunction()
        {
            objAttend.SetActive(!isAttend);
            if (isAttend)
            {
                textCharming.text = isCharming ? "애교 O" : "애교 X";
                textCloth.text = isCloth ? "의상변경 O" : "의상변경 X";
                textPanalty.text = $"사용가능한 벌칙 티켓 : {iPanalty}";
            }
            else
            {
                textCharming.text = "";
                textCloth.text = "";
                textPanalty.text = "";
            }
        }
    }
}
