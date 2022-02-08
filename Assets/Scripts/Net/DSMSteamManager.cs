using Steamworks.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualVoid.Net;
using System;

public class DSMSteamManager : SteamManager
{
    public static DSMSteamManager Instance;
    protected override void OnAwake()
    {
        Instance = this;

        ClientHandle.Init();
        ServerHandle.Init();

        OnConnectionClosed += () => PopUp.Show("Connection Closed");
        OnLobbyJoinFailed += () => PopUp.Show("Lobby join failed");
        OnLobbyJoinStarted += () => PopUp.Show("Attempting connection...");
    }


    public static Player LocalPlayer;
    public static List<Player> players = new List<Player>();
    private static void UpdatePlayers()
    {
        players = GetAllClients<Player>();
        foreach (Player p in players)
        {
            if (p.IsLocalPlayer)
            {
                LocalPlayer = p;
                return;
            }
        }
    }



    protected override Client GetClient()
    {
        return new Player();
    }

    protected override bool ShouldJoinGame(Lobby lobby)
    {
        // Log to console

        if (SceneManager.CurrentLevel != Level.MainMenu)
        {
            Debug.LogWarning($"Attempted to join lobby of {lobby.Owner.Name}, but currently not in main menu level!");
            return false;
        }

        if (SceneManager.CurrentGameStage != GameStage.MainMenu)
        {
            Debug.LogWarning($"Attempted to join lobby of {lobby.Owner.Name}, but currently not in main menu game stage!");
            return false;
        }

        if (ConnectedToServer)
        {
            Debug.LogWarning($"Attempted to join lobby of {lobby.Owner.Name}, but currently connected to another server!");
            return false;
        }

        return true;
    }

    protected override bool CanJoinServer(Connection connection, ConnectionInfo data)
    {
        if (clients.Count == 0) return true;
        // So we can join

        // If server and in game level, return false
        if (SceneManager.CurrentGameStage != GameStage.Lobby) return false;
        if (SceneManager.CurrentLevel != Level.MainMenu) return false;

        return true;
    }

    protected override void ChangeToScene(int buildIndex)
    {
        SceneManager.ChangeToScene(buildIndex);
    }


    public static void ContinueFromLobby()
    {
        if (!IsServer) return;

        PopUp.Show("Fetching details...");

        SetCurrentLobbyPrivacy(SteamLobbyPrivacyMode.Private);

        //ServerSend.RequestPlayerDetails();
        // VVV
    }

    //public static void OnPlayerDetailsReceived()
    //{
    //    if (AllPlayersHaveDetails())
    //    {
    //        PopUp.Show("Starting game...");
    //        SceneManager.CurrentGameStage = GameStage.SpawnSelect;
    //        LoadScene(SceneManager.NameOf(Level.SpawnSelect));
    //    }
    //}

    protected override void OnUpdate()
    {
        if (clients.Count != players.Count)
        {
            UpdatePlayers();

            if (clients.Count != players.Count)
            {
                Debug.LogWarning("Number of players is not equal to number of clients!");
            }
        }
    }


    public static void StartGame()
    {
        SceneManager.CurrentGameStage = GameStage.Game;
        LoadScene(SceneManager.NameOf(Level.Game));
    }
}
