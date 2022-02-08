using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    private static GameSettings settings;
    public static GameSettings CurrentSettings
    {
        get
        {
            if (settings == null)
                settings = new GameSettings();

            return settings;
        }
    }

    public static void Save()
    {
        try
        {
            WriteSettings();
            WriteInputs();
        }
        catch (System.Exception ex)
        {
            Debug.Log("Error saving settings: " + ex);
        }
    }

    public static void Load()
    {
        try
        {
            ReadSettings();
            ReadInputs();
        }
        catch (System.Exception ex)
        {
            Debug.Log("Error reading settings: " + ex);
        }
    }

    private static void WriteSettings()
    {
        SaveLoad.SaveJson("settings.json", settings ?? new GameSettings(), true);
    }

    private static void ReadSettings()
    {
        settings = SaveLoad.ReadJson<GameSettings>("settings.json", true);
        if (settings == null)
        {
            settings = new GameSettings();
            WriteSettings();
        }

        settings.Validate();
    }


    private static void WriteInputs()
    {
        SaveLoad.SaveJson("inputs.json", Inputs.Profile ?? new InputProfile(), true);
    }

    private static void ReadInputs()
    {
        Inputs.Profile = SaveLoad.ReadJson<InputProfile>("inputs.json", true);

        if (Inputs.Profile == null)
        {
            Inputs.Profile = new InputProfile();
            WriteInputs();
        }
    }


    public static void Apply()
    { 
        //QualityManager.SetQualityLevel(CurrentSettings.qualityLevel);
        //QualityManager.SetMaxFramerate(CurrentSettings.maxFramerate);
        //QualityManager.SetVSync(CurrentSettings.vsync);

        AudioManager.SetMasterVolume(CurrentSettings.masterVolume / 100f);
        AudioManager.SetAmbientVolume(CurrentSettings.ambientVolume / 100f);
        AudioManager.SetSFXVolume(CurrentSettings.sfxVolume / 100f);

        //FPSCamera.CurrentSensFromSettings = CurrentSettings.sensitivity;
        AudioManager.SetLowPass(1f);
    }
}
