
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

[UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
public class PrisonSystem : UdonSharpBehaviour
{
    #region Prison
    [SerializeField] GameObject obj_prison;

    public void Open_Prison()
    {
        if (!Networking.IsOwner(gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            Networking.SetOwner(Networking.LocalPlayer, obj_prison);
        }

        obj_prison.transform.rotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
        SendCustomEventDelayedSeconds("Close_Prison", 3.0f);
    }

    public void Close_Prison()
    {
        obj_prison.transform.rotation = Quaternion.Euler(-90.0f, 0.0f, 90.0f);
    }
    #endregion
}
