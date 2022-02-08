using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualVoid.Net;
using Steamworks;
using Steamworks.Data;

public static class ServerSend
{
    public static void SendAudio(AudioMessage audio, SteamId from)
    {
        Message message = Message.Create(SendType.Reliable, MessageIDs.Shared.AudioManager_Play);
        message.Add(audio);
        SteamManager.SendMessageToAllClients(from, message);
    }

    public static void SendSpawnPlayers()
    {
        SteamManager.SendMessageToAllClients(Message.Create(SendType.Reliable, MessageIDs.Server.SpawnPlayers));
    }

    public static void SendStartGame()
    {
        SteamManager.SendMessageToAllClients(Message.Create(SendType.Reliable, MessageIDs.Server.StartGame));
    }
}

public static class ServerHandle
{
    public static void Init()
    {
        SteamManager.AddHandler_FromClient(MessageIDs.Shared.AudioManager_Play, OnAudio);
        SteamManager.AddHandler_FromClient(MessageIDs.Client.Ready, OnReady);
    }

    private static void OnAudio(SteamId from, Message message)
    {
        AudioMessage audio = message.GetStruct<AudioMessage>();

        ServerSend.SendAudio(audio, from);
    }

    private static void OnReady(SteamId from, Message message)
    {
        if (!SteamManager.TryGetClient(from, out Player player)) return;

        player.ready = true;

        GameManager.OnPlayerReady();
    }
}
