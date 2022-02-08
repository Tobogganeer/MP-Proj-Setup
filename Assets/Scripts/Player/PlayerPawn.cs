using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualVoid.Net;

public class PlayerPawn : MonoBehaviour
{
    [Header("Both")]
    public Player player;
    public PlayerMovement movement;
    public FPSCamera cam;
    public VoiceOutput voiceOutput;
    public ClientNetworkTransform netTransform;

    public bool IsLocalPlayer => player.IsLocalPlayer;
    private void Start()
    {
        netTransform.ownerClient = player;

        if (IsLocalPlayer)
        {

        }
    }

    private void Update()
    {


        if (IsLocalPlayer)
        {

        }
    }
}
