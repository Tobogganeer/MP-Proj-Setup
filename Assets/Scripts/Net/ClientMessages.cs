using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualVoid.Net;
using Steamworks;
using Steamworks.Data;

public static class ClientSend
{
    public static void SendAudio(AudioMessage audio)
    {
        Message message = Message.Create(SendType.Reliable, MessageIDs.Shared.AudioManager_Play);
        message.Add(audio);
        SteamManager.SendMessageToServer(message);
    }

    public static void SendReady()
    {
        SteamManager.SendMessageToServer(Message.Create(SendType.Reliable, MessageIDs.Client.Ready));
    }
}

public static class ClientHandle
{
    public static void Init()
    {
        SteamManager.AddHandler_FromServer(MessageIDs.Shared.AudioManager_Play, OnAudio);
        SteamManager.AddHandler_FromServer(MessageIDs.Server.SpawnPlayers, OnSpawnPlayers);
        SteamManager.AddHandler_FromServer(MessageIDs.Server.StartGame, OnStartGame);
    }

    private static void OnAudio(Message message)
    {
        AudioMessage audio = message.GetStruct<AudioMessage>();

        AudioManager.OnNetworkAudio(audio);
    }

    private static void OnSpawnPlayers(Message message)
    {
        GameManager.SpawnPlayers();
    }

    private static void OnStartGame(Message message)
    {
        GameManager.StartGame();
    }
}
