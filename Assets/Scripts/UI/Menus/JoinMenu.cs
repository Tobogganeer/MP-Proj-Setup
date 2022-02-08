using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Steamworks;
using Steamworks.Data;

public class JoinMenu : MonoBehaviour
{
    private void OnEnable()
    {
        lobbyIDInputField.text = "";
    }

    public TMP_InputField lobbyIDInputField;

    public void OnJoinLobbyButtonPressed()
    {
        if (lobbyIDInputField.text.Length < 8)
        {
            PopUp.Show("Input text too short");
            return;
        }

        if (!ulong.TryParse(lobbyIDInputField.text, out ulong id))
        {
            PopUp.Show("Invalid number entered");
            return;
        }

        SteamId steamID = id;

        if (!steamID.IsValid)
        {
            PopUp.Show("Invalid SteamID entered");
            return;
        }

        Lobby lobby = new Lobby(steamID);
        DSMSteamManager.instance.SteamFriends_OnGameLobbyJoinRequested(lobby, 0);
    }
}
