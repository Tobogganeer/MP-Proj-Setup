using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainSettingsMenu : MonoBehaviour
{
    public static MainSettingsMenu instance;
    private void Awake()
    {
        instance = this;
    }

    #region UI Elements
    public Slider sensitivitySlider;
    public TMP_Text sensitivityText;
    [Space]

    public Slider masterVolumeSlider;
    public TMP_Text masterVolumeText;
    [Space]

    public Slider sfxVolumeSlider;
    public TMP_Text sfxVolumeText;
    [Space]

    public Slider ambientVolumeSlider;
    public TMP_Text ambientVolumeText;
    [Space]

    public TMP_Dropdown graphicsQualityDropdown;
    [Space]

    public Slider maxFramerateSlider;
    public TMP_Text maxFramerateText;
    [Space]

    public Toggle vsyncToggle;
    #endregion

    private void Start()
    {
        OnOpen();
    }

    public void OnOpen()
    {
        ApplyCurrentSettingsToUI();
    }

    public void ApplyCurrentSettingsToUI()
    {
        GameSettings s = Settings.CurrentSettings;

        sensitivitySlider.value = s.sensitivity;
        sensitivityText.text = Mathf.Round(s.sensitivity).ToString();

        masterVolumeSlider.value = s.masterVolume / 100f;
        masterVolumeText.text = s.masterVolume.ToString();

        sfxVolumeSlider.value = s.sfxVolume / 100f;
        sfxVolumeText.text = s.sfxVolume.ToString();

        ambientVolumeSlider.value = s.ambientVolume / 100f;
        ambientVolumeText.text = s.ambientVolume.ToString();

        graphicsQualityDropdown.value = (int)s.qualityLevel;

        maxFramerateSlider.value = s.maxFramerate;
        maxFramerateText.text = s.maxFramerate.ToString();

        vsyncToggle.isOn = s.vsync;
    }

    public void ApplyUIToCurrentSettings()
    {
        GameSettings s = Settings.CurrentSettings;
        s.sensitivity = sensitivitySlider.value;

        s.masterVolume = Mathf.RoundToInt(masterVolumeSlider.value * 100f);
        s.ambientVolume = Mathf.RoundToInt(ambientVolumeSlider.value * 100f);
        s.sfxVolume = Mathf.RoundToInt(sfxVolumeSlider.value * 100f);

        s.qualityLevel = (GraphicsQualityLevels)graphicsQualityDropdown.value;

        s.maxFramerate = (int)maxFramerateSlider.value;

        s.vsync = vsyncToggle.isOn;

        s.Validate();
    }


    public void OnSaveButtonPressed()
    {
        ApplyUIToCurrentSettings();
        Settings.Save();
        Settings.Apply();

        Debug.Log("Saved settings.");
        MainMenu.ReturnToMainMenu();
    }

    public void OnDiscardButtonPressed()
    {
        Settings.Apply();

        MainMenu.ReturnToMainMenu();
    }

    public void OnSetDefaultsButtonPressed()
    {
        Settings.CurrentSettings.SetDefaults();
        Settings.Save();
        Settings.Apply();

        ApplyCurrentSettingsToUI();
    }

    #region UI Element Callbacks

    public void OnSensitivitySliderChanged()
    {
        sensitivityText.text = Mathf.Round(sensitivitySlider.value).ToString();
    }

    public void OnAudioSliderChanged()
    {
        masterVolumeText.text = Mathf.Round(masterVolumeSlider.value * 100f).ToString();
        ambientVolumeText.text = Mathf.Round(ambientVolumeSlider.value * 100f).ToString();
        sfxVolumeText.text = Mathf.Round(sfxVolumeSlider.value * 100f).ToString();
    }

    public void OnMaxFramerateSliderChanged()
    {
        maxFramerateText.text = maxFramerateSlider.value.ToString();
    }

    #endregion

}


