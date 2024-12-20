﻿
using UdonSharp;
using UnityEngine;
using VRC.SDK3.ClientSim;
using VRC.SDKBase;

[UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
public class Teleport : UdonSharpBehaviour
{
    [SerializeField] Transform teleportPos;

    public override void Interact()
    {
        Networking.LocalPlayer.TeleportTo(teleportPos.position, teleportPos.rotation);
    }
}