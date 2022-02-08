using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HostMenu : MonoBehaviour
{
    public Slider maxPlayersSlider;
    public TMP_Dropdown lobbyModeDropdown;

    public TMP_Text maxPlayersText;

    private void Start()
    {
        maxPlayersSlider.value = 4;
        lobbyModeDropdown.value = 2;
    }

    public void OnHostButtonPressed()
    {
        DSMSteamManager.SetMaxPlayers((uint)Mathf.Clamp(Mathf.RoundToInt(maxPlayersSlider.value), 2, 8));
        VirtualVoid.Net.SteamLobbyPrivacyMode mode = (VirtualVoid.Net.SteamLobbyPrivacyMode)lobbyModeDropdown.value;
        DSMSteamManager.Host(mode);
        Debug.Log($"Hosting {mode} lobby - Max players: {DSMSteamManager.MaxPlayers}");
    }

    public void OnMaxPlayersSliderChanged()
    {
        maxPlayersText.text = maxPlayersSlider.value.ToString();
    }
}
