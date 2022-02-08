using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscordController : MonoBehaviour
{
    public static DiscordController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private const long CLIENT_ID = 916069195790307338;
    public Discord.Discord discord;
    Discord.ActivityManager activityManager;

    public bool logActivityResults = true;
    
    private void Start()
    {
        try
        {
            discord = new Discord.Discord(CLIENT_ID, (ulong)Discord.CreateFlags.NoRequireDiscord);
        }
        catch (Discord.ResultException)
        {
            Debug.Log("Discord not detected.");
            discord = null;
            return;
        }

        activityManager = discord.GetActivityManager();
    
        UpdateActivity("In Development", ":)");
    }
    
    public static void UpdateActivity(string details, string state)
    {
        if (instance == null || instance.discord == null || instance.activityManager == null) return;

        var activity = new Discord.Activity
        {
            Details = details,
            State = state,
            Assets =
            {
                LargeImage = "main_logo",
                LargeText = "DSM",
            }
        };

        instance.activityManager.UpdateActivity(activity, (res) => 
        {
            if (!instance.logActivityResults) return;
            Debug.Log("Activity update result: " + res.ToString());
        });
    }
    
    private void Update()
    {
        discord?.RunCallbacks();

        if (Time.frameCount % 1000 == 0 && discord == null)
        {
            try
            {
                discord = new Discord.Discord(CLIENT_ID, (ulong)Discord.CreateFlags.NoRequireDiscord);
            }
            catch (Discord.ResultException)
            {
                //Debug.Log("Discord not detected.");
                discord = null;
            }
        }
    }
    
    private void OnApplicationQuit()
    {
        discord?.Dispose();
    }
}