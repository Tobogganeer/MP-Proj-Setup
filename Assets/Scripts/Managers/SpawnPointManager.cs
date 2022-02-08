using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
    public static SpawnPointManager instance;
    private void Awake()
    {
        instance = this;

        Spawns = spawns;
    }

    public Transform[] spawns;

    public static Transform[] Spawns;

}
