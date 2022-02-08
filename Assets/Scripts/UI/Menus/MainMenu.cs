using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private static MainMenu _instance;
    private static MainMenu instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<MainMenu>();

            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }

    public static GameObject MenuCanvas
    {
        get
        {
            return instance != null ? instance.gameObject : null;
        }
    }

    public GameObject mainPanel;
    public GameObject settingsPanel;
    public GameObject playMenuPanel;
    public GameObject hostPanel;
    public GameObject joinPanel;

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        ReturnToMainMenu();
        AudioManager.SetLowPass(1f);
    }

    public void OnSettingsButtonPressed()
    {
        CloseAllPanels();
        settingsPanel.SetActive(true);
        MainSettingsMenu.instance.OnOpen();
    }

    public void OnPlayButtonPressed()
    {
        CloseAllPanels();
        playMenuPanel.SetActive(true);
    }

    public void OnHostButtonPressed()
    {
        CloseAllPanels();
        hostPanel.SetActive(true);
    }

    public void OnJoinButtonPressed()
    {
        CloseAllPanels();
        joinPanel.SetActive(true);
    }

    public void OnReturnToMainMenuButtonPressed()
    {
        ReturnToMainMenu();
    }

    public void OnQuitButtonPressed()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }


    public static void ReturnToMainMenu()
    {
        if (instance == null) return;

        instance.CloseAllPanels();
        instance.mainPanel.SetActive(true);

        DSMSteamManager.Leave();
        SceneManager.CurrentGameStage = GameStage.MainMenu;
    }

    private void CloseAllPanels()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(false);
        playMenuPanel.SetActive(false);
        hostPanel.SetActive(false);
        joinPanel.SetActive(false);
    }
}
