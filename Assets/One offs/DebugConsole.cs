using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugConsole : MonoBehaviour
{
    public static DebugConsole instance;

    private GUIStyle boxStyle;
    private GUIStyle textStyle;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Application.logMessageReceived += LogErrors;
    }

    private bool showConsole = false;

    private bool logErrors = true;
    private bool logWarnings = true;
    private bool logMessages = true;

    [Space]
    public int maxMessages = 10;
    public int messageHeight = 40;
    public int messageSpacing = 5;

    [Space]
    public int consoleXOffset = 0;
    //public int buttonXAdjustment = 250;
    //public int buttonWidth = 200;

    private List<ConsoleMessage> messages = new List<ConsoleMessage>(16);

    [Space]
    public Color errorMessageColour = Color.red;
    public Color warningMessageColour = Color.yellow;
    public Color logMessageColour = Color.white;

    [Space]
    public Color mainBGColour = Color.black;
    public Color messageBGColour = Color.gray;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote)) showConsole = !showConsole;
    }

    private void OnGUI()
    {
        if (!showConsole) return;

        if (boxStyle == null)
        {
            boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.normal.background = Texture2D.whiteTexture;
        }

        if (textStyle == null)
        {
            textStyle = new GUIStyle(GUI.skin.box);
            textStyle.alignment = TextAnchor.UpperLeft;
            textStyle.normal.background = Texture2D.whiteTexture;
        }

        int width = Screen.width / 2;

        GUI.backgroundColor = mainBGColour;
        //GUI.Box(new Rect(width - width / 2 + consoleXOffset, 0, width, messageHeight * maxMessages + 40), "", boxStyle);
        GUI.Box(new Rect(consoleXOffset, 0, width, (messageHeight + messageSpacing) * maxMessages + 40), "", boxStyle);

        //int buttonX = width + width / 2;

        //textStyle.normal.textColor = logMessageColour;
        //logErrors = GUI.Toggle(new Rect(buttonXAdjustment, 20, buttonWidth, 20), logErrors, new GUIContent("Log Errors", "Log errors to the console"), textStyle);
        //logWarnings = GUI.Toggle(new Rect(buttonXAdjustment, 40, buttonWidth, 20), logWarnings, new GUIContent("Log Warnings", "Log warnings to the console"), textStyle);
        //logMessages = GUI.Toggle(new Rect(buttonXAdjustment, 60, buttonWidth, 20), logMessages, new GUIContent("Log Messages", "Log messages to the console"), textStyle);
        //
        //GUI.backgroundColor = mainBGColour;

        int numMessages = Mathf.Min(maxMessages, messages.Count);

        GUI.backgroundColor = messageBGColour;

        for (int i = 0; i < numMessages; i++)
        {
            ConsoleMessage consoleMessage = messages[i];
            string message = "";

            switch (consoleMessage.type)
            {
                case LogType.Error:
                    textStyle.normal.textColor = errorMessageColour;
                    message = $"ERROR: {consoleMessage.condition} - STACKTRACE: {consoleMessage.stackTrace}";
                    break;
                case LogType.Exception:
                    textStyle.normal.textColor = errorMessageColour;
                    message = $"EXCEPTION: {consoleMessage.condition} - STACKTRACE: {consoleMessage.stackTrace}";
                    break;
                case LogType.Warning:
                    textStyle.normal.textColor = warningMessageColour;
                    message = $"WARNING: {consoleMessage.condition} - STACKTRACE: {consoleMessage.stackTrace}";
                    break;
                case LogType.Log:
                    textStyle.normal.textColor = logMessageColour;
                    message = $"{consoleMessage.condition}";
                    break;
            }

            if (i < messages.Count)
                GUI.Box(new Rect(5 + consoleXOffset, i * (messageHeight + messageSpacing) + 10, width - 10, messageHeight), message, textStyle);
            //GUI.Box(new Rect(width - width / 2 + 5 + consoleXOffset, i * messageHeight + 20, width - buttonXAdjustment - 5, messageHeight), message, style);
        }

        //if (GameManager.verboseLoggingEnabled) Debug.Log("");
    }

    private void LogErrors(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Error && logErrors)
        {
            messages.Add(new ConsoleMessage(condition, stackTrace, type));
        }
        else if (type == LogType.Exception && logErrors)
        {
            messages.Add(new ConsoleMessage(condition, stackTrace, type));
        }
        else if (type == LogType.Warning && logWarnings)
        {
            messages.Add(new ConsoleMessage(condition, stackTrace, type));
        }
        else if (type == LogType.Log && logMessages)
        {
            messages.Add(new ConsoleMessage(condition, stackTrace, type));
        }

        while (messages.Count > maxMessages)
        {
            messages.RemoveAt(0);
        }

        //Debug.Log($"Message Received: {condition}: {stackTrace}");
    }

    #region Message Log Tests

    [ContextMenu("Log Test Message")]
    public void LogTestMessage()
    {
        Debug.Log("Test Message");
    }

    [ContextMenu("Log Test Warning")]
    public void LogTestWarning()
    {
        Debug.LogWarning("Test Warning");
    }

    [ContextMenu("Log Test Error")]
    public void LogTestError()
    {
        Debug.LogError("Test Error");
    }

    [ContextMenu("Log Test Exception")]
    public void LogTestException()
    {
        Debug.LogException(new System.Exception("Test exception"));
    }
    #endregion

    //[ConsoleCommand(Description = "Spawns a player with <health> health")]
    //public void SpawnPlayer(int health) { }
}

public struct ConsoleMessage
{
    public LogType type;
    public string condition, stackTrace;

    public ConsoleMessage(string condition, string stackTrace, LogType type)
    {
        this.type = type;
        this.condition = condition;
        this.stackTrace = stackTrace;
    }
}

//[AttributeUsage(AttributeTargets.Method)]
//public class ConsoleCommand : Attribute
//{
//    public string Description { get; set; }
//}
