
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Teleport_col : UdonSharpBehaviour
{
    [SerializeField] Transform teleportPos;

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player != Networking.LocalPlayer) return;
        player.TeleportTo(teleportPos.position, teleportPos.rotation);
    }
}
