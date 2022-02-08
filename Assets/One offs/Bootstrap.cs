using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    private void Start()
    {
        //GetReferences();
        Initialize();
    }

    //private void GetReferences()
    //{
    //    if (AudioManager.instance == null)
    //        AudioManager.instance = FindObjectOfType<AudioManager>();
    //
    //    // Settings.Apply() calls AudioManager stuff for audio, just making sure it aint null
    //}

    private void Initialize()
    {
        Settings.Load();
        Settings.Apply();
        Debug.Log("Bootstrap Initialized");
    }
}
