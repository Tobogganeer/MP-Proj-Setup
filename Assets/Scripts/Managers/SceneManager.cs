using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;
using Scene = UnityEngine.SceneManagement.Scene;

public class SceneManager : MonoBehaviour
{
    public static SceneManager instance;
    public static Level CurrentLevel = Level.MainMenu;
    public static GameStage CurrentGameStage = GameStage.MainMenu;

    private void Awake()
    {
        if (instance == null) instance = this;

        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        scenes.Clear();
        buildIndices.Clear();

        foreach (InspectorLevel level in levels)
        {
            scenes.Add(level.level, level.scene);

            int index = UnityEngine.SceneManagement.SceneUtility.GetBuildIndexByScenePath($"Assets/Scenes/{level.scene}.unity");
            buildIndices.Add(index, level.level);
            //for (int i = 0; i < UnitySceneManager.sceneCountInBuildSettings; i++)
            //{
            //    Scene scene = UnitySceneManager.GetSceneByBuildIndex(i);
            //    if (level.scene == scene.name)
            //    {
            //        buildIndices.Add(i, level.level);
            //        break;
            //    }
            //}
        }

        DSMSteamManager.OnDisconnectedFromServer += () => LoadLevel(Level.MainMenu);
    }

    private static Dictionary<Level, string> scenes = new Dictionary<Level, string>();
    private static Dictionary<int, Level> buildIndices = new Dictionary<int, Level>();
    public InspectorLevel[] levels;

    public static void LoadLevel(Level level)
    {
        if (CurrentLevel == level)
        {
            Debug.Log($"Skipping level change to {level} as that is the current level");
            return;
        }    

        if (!scenes.TryGetValue(level, out string scene))
        {
            Debug.LogError($"Tried to load {level}, but there is not scene assigned to that level!");
            return;
        }

        CurrentLevel = level;
        UnitySceneManager.LoadScene(scene);
    }

    public static void ReloadCurrentLevel()
    {
        UnitySceneManager.LoadScene(UnitySceneManager.GetActiveScene().buildIndex);
    }

    public static void ChangeToScene(int buildIndex)
    {
        if (!buildIndices.TryGetValue(buildIndex, out Level level))
        {
            Debug.LogWarning("Could not get level for build index " + buildIndex);
            return;
        }

        LoadLevel(level);
    }

    public static string NameOf(Level level)
    {
        return scenes[level];
    }


    private void OnValidate()
    {
        if (levels == null || levels.Length == 0) return;

        foreach (InspectorLevel level in levels)
        {
            level.name = level.scene ?? "Unset";
        }
    }

    [ContextMenu("Dump Indices")]
    private void DumpIndices()
    {
        foreach (KeyValuePair<int, Level> pair in buildIndices)
        {
            Debug.Log($"Index {pair.Key} => {pair.Value}");
        }
    }
}

[System.Serializable]
public class InspectorLevel
{
    // Used to assign scenes to the enums in the inspector
    [HideInInspector]
    public string name; // Just for inspector
    public Level level; // The level enum
    [Scene] public string scene; // The actual scene
}
