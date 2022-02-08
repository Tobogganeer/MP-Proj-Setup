using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using VirtualVoid.Net;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
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

        clips.Clear();
        foreach (AudioClips aclips in audioClips)
        {
            clips.Add(aclips.array, aclips.clips);
        }

        //nullClip = AudioClip.Create("Null", 5, 1, 4096, false);
    }

    //public GameObject audioSourcePrefab;

    private static Dictionary<AudioArray, AudioClip[]> clips = new Dictionary<AudioArray, AudioClip[]>();
    public AudioClips[] audioClips;
    //private static AudioClip nullClip;

    public AudioMixer masterMixer;
    public AudioMixerGroup masterGroup;
    public AudioMixerGroup sfxGroup;
    public AudioMixerGroup ambientGroup;

    //public AudioMixerSnapshot defaultSnapshot;
    //public AudioMixerSnapshot pausedSnapshot;
    //public AudioMixerSnapshot outsideSnapshot;

    public const float DEFAULT_MIN_PITCH = 0.85f;
    public const float DEFAULT_MAX_PITCH = 1.10f;

    public bool updateInspectorGroupNames = false;



    public static void Play(AudioArray sound, Vector3 position, Transform parent = null, float maxDistance = 10, AudioCategory category = AudioCategory.SFX, float volume = 1, float minPitch = DEFAULT_MIN_PITCH, float maxPitch = DEFAULT_MAX_PITCH, float _3dAmount = 1f)
    {
        if (!clips.ContainsKey(sound) || clips[sound].Length == 0)
        {
            Debug.LogWarning("Could not find clip for " + sound);
            return;
        }

        byte clip = (byte)Random.Range(0, clips[sound].Length);
        AudioMessage message = GetAudioMessage(sound, clip, position, parent, maxDistance, category, volume, minPitch, maxPitch, _3dAmount);

        if (DSMSteamManager.IsServer)
        {
            ServerSend.SendAudio(message, DSMSteamManager.SteamID);
        }
        else
        {
            if (DSMSteamManager.ConnectedToServer)
                ClientSend.SendAudio(message);
        }

        PlaySpecificAudioArrayLocal(sound, clip, position, parent, maxDistance, category, volume, minPitch, maxPitch, _3dAmount);
    }

    public static void Play2D(AudioArray sound, AudioCategory category = AudioCategory.SFX, float volume = 1, float minPitch = DEFAULT_MIN_PITCH, float maxPitch = DEFAULT_MAX_PITCH)
    {
        Play(sound, Vector3.zero, null, 10, category, volume, minPitch, maxPitch, 0f);
    }

    public static void Play2DLocal(AudioArray sound, AudioCategory category = AudioCategory.SFX, float volume = 1, float minPitch = DEFAULT_MIN_PITCH, float maxPitch = DEFAULT_MAX_PITCH)
    {
        PlayLocal(sound, Vector3.zero, null, 10, category, volume, minPitch, maxPitch, 0f);
    }


    public static void PlaySpecificAudioArrayLocal(AudioArray sound, byte clipIndex, Vector3 position, Transform parent = null, float maxDistance = 10, AudioCategory category = AudioCategory.SFX, float volume = 1, float minPitch = DEFAULT_MIN_PITCH, float maxPitch = DEFAULT_MAX_PITCH, float _3dAmount = 1f)
    {
        PlayAudioClipLocal(clips[sound][clipIndex], position, parent, maxDistance, category, volume, minPitch, maxPitch, _3dAmount);
    }

    public static void PlayLocal(AudioArray sound, Vector3 position, Transform parent = null, float maxDistance = 10, AudioCategory category = AudioCategory.SFX, float volume = 1, float minPitch = DEFAULT_MIN_PITCH, float maxPitch = DEFAULT_MAX_PITCH, float _3dAmount = 1f)
    {
        if (!clips.ContainsKey(sound) || clips[sound].Length == 0)
        {
            Debug.LogWarning("Could not find clip for " + sound);
            return;
        }

        byte clip = (byte)Random.Range(0, clips[sound].Length);

        PlaySpecificAudioArrayLocal(sound, clip, position, parent, maxDistance, category, volume, minPitch, maxPitch, _3dAmount);
    }

    public static void PlayAudioClipLocal(AudioClip clip, Vector3 position, Transform parent = null, float maxDistance = 10, AudioCategory category = AudioCategory.SFX, float volume = 1, float minPitch = DEFAULT_MIN_PITCH, float maxPitch = DEFAULT_MAX_PITCH, float _3dAmount = 1f)
    {
        //AudioSource source = Instantiate(instance.audioSourcePrefab, position, Quaternion.identity, parent).GetComponent<AudioSource>();
        GameObject sourceObj = ObjectPoolManager.GetObject(PooledObject.AudioSource);
        if (sourceObj != null)
        {
            if (parent != null && !parent.gameObject.activeInHierarchy)
            {
                sourceObj.SetActive(false);
                return;
            }
            sourceObj.transform.SetParent(parent);
            sourceObj.transform.position = position;

            AudioSource source = sourceObj.GetComponent<AudioSource>();

            source.clip = clip;
            source.maxDistance = maxDistance;
            float pitch = Random.Range(minPitch, maxPitch);
            source.pitch = pitch;
            source.volume = volume;
            source.spatialBlend = _3dAmount;
            source.outputAudioMixerGroup = instance.GetGroup(category);
            source.Play();

            // MASSIVE OOPS: Realized I should have been dividing by pitch instead of multiplying (for like every game!) - Nov 25/21
            sourceObj.GetComponent<PooledAudioSource>().DisableAfterTime(source.clip.length / pitch + 0.3f);
            //Destroy(source.gameObject, source.clip.length / (pitch + 0.1f));
        }
        else
        {
            Debug.Log($"Couldn't play audio as received null audio source from pool");
        }
    }

    public static void OnNetworkAudio(AudioMessage message)
    {
        Transform parent = message.parent != null ? message.parent.transform.parent : null;
        float _3dAmount = message.flags.HasFlag(AudioMessage.AudioMessageFlags.Global) ? 0f : 1f;

        PlaySpecificAudioArrayLocal(message.sound, message.audioIndex, message.position, parent, message.maxDistance, message.category, message.volume, message.minPitch, message.maxPitch, _3dAmount);
    }

    private static AudioMessage GetAudioMessage(AudioArray sound, byte clipIndex, Vector3 position, Transform parent = null, float maxDistance = 10, AudioCategory category = AudioCategory.SFX, float volume = 1, float minPitch = DEFAULT_MIN_PITCH, float maxPitch = DEFAULT_MAX_PITCH, float _3dAmount = 1f)
    {
        AudioMessage audioMessage = new AudioMessage(sound, clipIndex, position, null, maxDistance, category, volume, minPitch, maxPitch);

        bool is3d = _3dAmount > 0.5f;

        if (!is3d) audioMessage.IsGlobal();

        else
        {

            if (parent != null && parent.TryGetComponent(out NetworkID networkID))
            {
                audioMessage.parent = networkID;
                audioMessage.UseParent();
            }

            if (maxDistance != 10f)
                audioMessage.UseDistance();
        }

        if (volume != 1f)
            audioMessage.UseVolume();

        if (minPitch != DEFAULT_MIN_PITCH || maxPitch != DEFAULT_MAX_PITCH)
            audioMessage.UsePitch();

        return audioMessage;
    }



    private const string MASTER_VOLUME_PARAM = "master_volume";
    private const string AMBIENT_VOLUME_PARAM = "ambient_volume";
    private const string SFX_VOLUME_PARAM = "sfx_volume";


    private const float MIN_AUDIO_DB = -70;
    private const float MAX_AUDIO_DB = 5;

    private const float MIN_LOG_OUT = -60;
    private const float MAX_LOG_OUT = 0;

    public static void SetMasterVolume(float volume0_1) => SetVolume(MASTER_VOLUME_PARAM, volume0_1);

    public static void SetAmbientVolume(float volume0_1) => SetVolume(AMBIENT_VOLUME_PARAM, volume0_1);

    public static void SetSFXVolume(float volume0_1) => SetVolume(SFX_VOLUME_PARAM, volume0_1);



    private static void SetVolume(string paramName, float volume0_1)
    {
        //Debug.Log($"Set {paramName} to {GetVolume(volume0_1)} (from {volume0_1})");
        instance.masterMixer.SetFloat(paramName, GetVolume(volume0_1));
    }


    private static float GetVolume(float volume0_1)
    {
        //float remappedVolume = Remap.Float(volume0_1, 0, 1, MIN_AUDIO_DB, MAX_AUDIO_DB);

        float clamped = Mathf.Clamp(volume0_1, 0.001f, 1f);

        float remapped = 20f * Mathf.Log10(clamped);
        remapped = Remap.Float(remapped, MIN_LOG_OUT, MAX_LOG_OUT, MIN_AUDIO_DB, MAX_AUDIO_DB);

        //Debug.Log($"IN: {clamped} - OUT: {remapped}");

        return remapped;
    }

    private void OnValidate()
    {
        if (audioClips == null || !updateInspectorGroupNames) return;

        for (int i = 0; i < audioClips.Length; i++)
        {
            audioClips[i].name = audioClips[i].array.ToString();
        }
    }

    private AudioMixerGroup GetGroup(AudioCategory category)
    {
        switch (category)
        {
            case AudioCategory.Master:
                return masterGroup;
            case AudioCategory.SFX:
                return sfxGroup;
            case AudioCategory.Ambient:
                return ambientGroup;
        }

        return null;
    }


    private const string CUTOFF_FREQ_PARAM = "cutoff_freq";

    static float oldPercent;
    public static void SetLowPass(float percent0_1)
    {
        if (oldPercent == percent0_1) return;

        else oldPercent = percent0_1;

        float remapped = Remap.Float(percent0_1, 0, 1, 10, 22000);

        instance.masterMixer.SetFloat(CUTOFF_FREQ_PARAM, remapped);
    }
}



[System.Serializable]
public struct AudioClips
{
    [SerializeField, HideInInspector]
    public string name;
    public AudioArray array;
    public AudioClip[] clips;
}

public enum AudioCategory : byte
{
    Master,
    SFX,
    Ambient
}

