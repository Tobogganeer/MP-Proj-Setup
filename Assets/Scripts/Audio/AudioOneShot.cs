using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioOneShot : MonoBehaviour
{
    public AudioArray audioArray;
    public float range = 10;
    public AudioCategory category;
    public float volume = 1;
    public float minPitch = AudioManager.DEFAULT_MIN_PITCH;
    public float maxPitch = AudioManager.DEFAULT_MAX_PITCH;

    public bool parentToThis = false;

    private void Start()
    {
        Transform parent = parentToThis ? transform : null;
        
        if (audioArray != AudioArray.Null)
            AudioManager.Play(audioArray, transform.position, parent, range, category, volume, minPitch, maxPitch);
    }
}
