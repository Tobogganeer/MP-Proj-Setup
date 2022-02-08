using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualVoid.Net;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private void Awake()
    {
        instance = this;

        SceneManager.CurrentGameStage = GameStage.Game;
        SceneManager.CurrentLevel = Level.Game;

        PopUp.Show("Waiting for all players...");
    }


    public static bool GameStarted = false;

    public GameObject localPlayerPrefab;
    public GameObject otherPlayerPrefab;

    public static event Action OnPlayersSpawned;



    private void OnEnable()
    {
        SteamManager.OnAllClientsSceneLoaded += SteamManager_OnAllClientsSceneLoaded;
        SceneManager.CurrentGameStage = GameStage.Game;
        SceneManager.CurrentLevel = Level.Game;

        GameStarted = false;
    }

    private void OnDisable()
    {
        SteamManager.OnAllClientsSceneLoaded -= SteamManager_OnAllClientsSceneLoaded;

        GameStarted = false;
    }


    private void SteamManager_OnAllClientsSceneLoaded()
    {
        if (SteamManager.IsServer)
        {
            ServerSend.SendSpawnPlayers();
        }
    }

    public static void OnPlayerSpawnsReceived()
    {
        // Client side
        PopUp.Show("Ready...");

        // Spawn players
        Destroy(instance.GetComponent<AudioListener>());

        foreach (Player p in DSMSteamManager.players)
        {
            Destroy(p.CurrentPawn);

            GameObject pawn = Spawn(p);

            p.SetPlayerPawn(pawn.GetComponent<PlayerPawn>());
        }

        OnPlayersSpawned?.Invoke();

        ClientSend.SendReady();
    }

    private static GameObject Spawn(Player p)
    {
        Transform spawnLoc = SpawnPointManager.Spawns[p.clientID];

        if (p.IsLocalPlayer) return Instantiate(instance.localPlayerPrefab, spawnLoc.position, spawnLoc.rotation);
        else return Instantiate(instance.otherPlayerPrefab, spawnLoc.position, spawnLoc.rotation);
    }

    public static void OnPlayerReady()
    {
        if (!SteamManager.IsServer) return;

        bool anyNotReady = false;

        foreach (Player p in DSMSteamManager.players)
        {
            if (!p.ready) anyNotReady = true;
        }

        if (anyNotReady) return;

        ServerSend.SendStartGame();
    }

    public static void SpawnPlayers()
    {
        throw new NotImplementedException();
    }

    public static void StartGame()
    {
        PopUp.Show("Game Start!");

        GameStarted = true;
        // Other crap
    }
}
