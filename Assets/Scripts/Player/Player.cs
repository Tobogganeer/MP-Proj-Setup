using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualVoid.Net;
using System.Threading.Tasks;

public class Player : Client
{
    private Texture2D pfp;

    public bool ready = false;
    public PlayerPawn playerPawnComp;

    protected override void OnConnect()
    {
        // IDK
        CurrentPawn = new GameObject(SteamName + " Lobby VC");
        AudioSource source = CurrentPawn.AddComponent<AudioSource>();
        VoiceOutput = CurrentPawn.AddComponent<VoiceOutput>();
        VoiceOutput.source = source;
        Object.DontDestroyOnLoad(CurrentPawn);
    }

    protected override void OnDisconnect()
    {
        base.OnDisconnect();
    }

    protected override void OnSceneLoaded()
    {
        // If SceneManager.CurrentLevel == Level.Game
        //    Spawn pawn
    }

    public async Task<Texture2D> GetPFP()
    {
        if (pfp == null)
            pfp = await SteamImageUtil.GetMediumPFP(SteamID);

        return pfp;
    }


    public void SetPlayerPawn(PlayerPawn pawn)
    {
        CurrentPawn = pawn.gameObject;
        VoiceOutput = pawn.voiceOutput;
        playerPawnComp = pawn;
        pawn.player = this;
        NetworkTransform = pawn.netTransform;
    }


    public static Player Local => DSMSteamManager.LocalPlayer;
}
