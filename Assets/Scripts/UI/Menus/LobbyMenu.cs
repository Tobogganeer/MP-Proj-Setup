using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VirtualVoid.Net;

public class LobbyMenu : MonoBehaviour
{
    public static LobbyMenu instance;
    private void Awake()
    {
        instance = this;
        SteamManager.OnClientConnected += OnClientConnected;
        SteamManager.OnClientDisconnected += OnClientsChanged;

        SteamManager.OnConnectedToServer += WhenConnectedToServer;
        SteamManager.OnDisconnectedFromServer += WhenDisconnectedFromServer;

        for (int i = 0; i < entries.Length; i++)
        {
            entries[i].index = i;
        }

        DisableAllLobbyUIs();

        EnableMainMenu(true, true);
    }

    private void OnDestroy()
    {
        SteamManager.OnClientConnected -= OnClientConnected;
        SteamManager.OnClientDisconnected -= OnClientsChanged;

        SteamManager.OnConnectedToServer -= WhenConnectedToServer;
        SteamManager.OnDisconnectedFromServer -= WhenDisconnectedFromServer;
    }

    public static bool IsOpen => instance != null && instance.gameObject != null && instance.gameObject.activeSelf;


    public LobbyUI[] entries;
    public TMP_Text lobbyHeaderText;
    public TMP_Text lobbyCodeText;
    private Camera mainCam;

    public GameObject hostOnlyObj;

    public LayerMask lobbyUILayermask;


    public static void OnScourgeChanged()
    {
        instance.OnClientsChanged();
    }

    private void OnClientsChanged(Client client)
    {
        OnClientsChanged();
    }

    [ContextMenu("Refresh Clients")]
    private void OnClientsChanged()
    {
        try
        {
            if (lobbyCodeText != null)
            {
                if (SteamManager.IsServer)
                    lobbyHeaderText.text = SteamManager.SteamName + "'s Lobby";
                else
                    lobbyHeaderText.text = SteamManager.CurrentLobby.Owner.Name + "'s Lobby";
            }

            if (lobbyCodeText != null)
                lobbyCodeText.text = SteamManager.CurrentLobby.Id.ToString();

            //Debug.Log("Lobby Clients Changed");

            DisableAllLobbyUIs();

            if (!SteamManager.ConnectedToServer)
            {
                Debug.Log("Skipping Lobby refresh because not connected");
                return;
            }

            foreach (Player player in SteamManager.GetAllClients<Player>())
            {
                if (player != null)
                    EnableLobbyUI(player.clientID, player);
            }

            if (hostOnlyObj != null)
                hostOnlyObj.SetActive(SteamManager.IsServer);
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("Error updating lobby clients: " + ex);
            //throw;
        }
    }

    private void OnClientConnected(Client client)
    {
        //if (SteamManager.IsServer)
        //{
        //    ServerSend.SendScourgeIDs(GetScourgeIDs(), client.SteamID);
        //}

        OnClientsChanged();
    }

    // not using OnConnectedToServer as its a unity magic method
    private void WhenConnectedToServer()
    {
        EnableMainMenu(false);

        CancelInvoke();
        Invoke(nameof(RefreshClientsAfterDelay), 1f);
    }

    private void WhenDisconnectedFromServer()
    {
        EnableMainMenu(true);
    }

    private void RefreshClientsAfterDelay()
    {
        OnClientsChanged();
    }

    //private void OnEnable()
    //{
    //    if (!DSMSteamManager.ConnectedToServer)
    //        EnableMainMenu(true);
    //}

    //private void OnDisable()
    //{
    //    EnableMainMenu(true);
    //}

    public static void EnableMainMenu(bool active, bool bypassStageChecks = false)
    {
        GameStage newStage = active ? GameStage.MainMenu : GameStage.Lobby;

        if (newStage == SceneManager.CurrentGameStage && !bypassStageChecks)
        {
            Debug.Log("Skipping main menu <=> lobby transition, as already in " + newStage);
            return;
        }

        if (instance.hostOnlyObj != null)
            instance.hostOnlyObj.SetActive(SteamManager.IsServer);

        if (MainMenu.MenuCanvas != null)
            MainMenu.MenuCanvas.SetActive(active);

        if (instance != null && instance.gameObject != null)
            instance.gameObject.SetActive(!active);

        if (active == false)
            instance.DisableAllLobbyUIs();

        //Debug.Log("Lobby changing to main menu? " + active);

        SceneManager.CurrentGameStage = newStage;
    }

    public void OnReturnButtonPressed()
    {
        DSMSteamManager.Leave();
    }


    private async void EnableLobbyUI(int index, Player player)
    {
        //if (index > SteamManager.MaxPlayers - 1)
        //    Debug.LogWarning("Lobby index overflow attempt");

        LobbyUI ui = entries[index];

        if (ui == null || ui.IsNull())
        {
            Debug.LogWarning("Null lobbyUI obj for index" + index);
            return;
        }

        ui.gameObject.SetActive(true);
        ui.steamNameText.text = player.SteamName;

        ui.pfpImage.texture = await player.GetPFP();
    }

    private void DisableAllLobbyUIs()
    {
        foreach (LobbyUI ui in entries)
        {
            if (ui != null && ui.gameObject != null)
                ui.gameObject.SetActive(false);
        }
    }

    [ContextMenu("Log Players")]
    private void LogPlayers()
    {
        System.Text.StringBuilder output = new System.Text.StringBuilder();

        output.AppendLine("Clients: " + SteamManager.clients.Count);
        foreach (Client client in SteamManager.GetAllClients())
        {
            output.AppendLine("-" + client.SteamName + " / index / " + client.clientID);
        }

        List<Player> players = SteamManager.GetAllClients<Player>();
        output.AppendLine("\nPlayers: " + players.Count);

        foreach (Player player in players)
        {
            output.AppendLine("-" + player.SteamName + " / index / " + player.clientID);
        }

        Debug.Log(output.ToString());
    }


    public void OnStartButtonPressed()
    {
        if (!SteamManager.IsServer)
        {
            hostOnlyObj.SetActive(false);
            return;
        }

        DSMSteamManager.ContinueFromLobby();
    }

    public void OnInviteButtonPressed()
    {
        SteamManager.OpenSteamOverlayLobbyInvite();
    }

    public void OnCopyLobbyCodeButtonPressed()
    {
        GUIUtility.systemCopyBuffer = SteamManager.CurrentLobby.Id.ToString();
    }

    public void OnRefreshLobbyButtonPressed()
    {
        PopUp.Show("Refreshing...", 1);
        OnClientsChanged();
    }
}
