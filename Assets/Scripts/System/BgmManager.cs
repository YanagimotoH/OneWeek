using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BgmManager : MonoBehaviour
{
    [Serializable]
    public class SceneBgm
    {
        public string sceneName;
        public AudioClip clip;
    }

    [SerializeField] SceneBgm[] bgms;
    [SerializeField] float defaultVolume = 0.7f;
    [SerializeField] string volumePrefsKey = "BgmVolume";
    [SerializeField] bool saveVolume = true;

    static BgmManager instance;
    AudioSource audioSource;

    public static BgmManager Instance => instance;

    public float Volume
    {
        get => audioSource != null ? audioSource.volume : defaultVolume;
        set
        {
            float clamped = Mathf.Clamp01(value);
            if (audioSource != null)
            {
                audioSource.volume = clamped;
            }

            if (saveVolume)
            {
                PlayerPrefs.SetFloat(volumePrefsKey, clamped);
            }
        }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.loop = true;
        audioSource.playOnAwake = false;

        if (saveVolume && PlayerPrefs.HasKey(volumePrefsKey))
        {
            audioSource.volume = Mathf.Clamp01(PlayerPrefs.GetFloat(volumePrefsKey));
        }
        else
        {
            audioSource.volume = Mathf.Clamp01(defaultVolume);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        ApplySceneBgm(SceneManager.GetActiveScene().name);
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            instance = null;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplySceneBgm(scene.name);
    }

    void ApplySceneBgm(string sceneName)
    {
        AudioClip clip = GetClipForScene(sceneName);
        if (clip == null)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            audioSource.clip = null;
            return;
        }

        if (audioSource.clip == clip && audioSource.isPlaying)
        {
            return;
        }

        audioSource.clip = clip;
        audioSource.Play();
    }

    AudioClip GetClipForScene(string sceneName)
    {
        if (bgms == null)
        {
            return null;
        }

        foreach (SceneBgm entry in bgms)
        {
            if (entry != null && !string.IsNullOrEmpty(entry.sceneName) && entry.sceneName == sceneName)
            {
                return entry.clip;
            }
        }

        return null;
    }
}
