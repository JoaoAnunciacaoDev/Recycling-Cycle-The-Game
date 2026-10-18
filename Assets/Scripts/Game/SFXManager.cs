using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [System.Serializable]
    public struct SoundEffect
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume;
        public bool randomPitch;
        [Range(0.5f, 1.5f)] public float minPitch;
        [Range(0.5f, 1.5f)] public float maxPitch;
    }

    [Header("Lista de Sons")]
    [SerializeField] private SoundEffect[] soundEffects;

    [Header("Atalhos Rápidos")]
    [SerializeField] private AudioClip errorClip;
    [SerializeField] private AudioClip hoverClip;
    [SerializeField] private AudioClip clickClip;
    [SerializeField] private AudioClip collectClip;
    [SerializeField] private AudioClip damageClip;

    [Header("Configurações do Pool")]
    [SerializeField] private int audioChannelCount = 10;
    [Range(0f, 1f)] [SerializeField] private float masterVolume = 1f;

    private Dictionary<string, SoundEffect> soundDictionary;
    private List<AudioSource> audioChannels;
    private int currentChannelIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        masterVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        transform.position = Vector3.zero;

        InitializeChannels();
        InitializeDictionary();
    }

    private void InitializeChannels()
    {
        audioChannels = new List<AudioSource>();

        for (int i = 0; i < audioChannelCount; i++)
        {
            GameObject channelObj = new GameObject($"AudioChannel_{i}");
            channelObj.transform.SetParent(transform);

            AudioSource source = channelObj.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f;
            source.ignoreListenerPause = true;

            audioChannels.Add(source);
        }
    }

    private void InitializeDictionary()
    {
        soundDictionary = new Dictionary<string, SoundEffect>(System.StringComparer.OrdinalIgnoreCase);

        if (soundEffects != null)
        {
            foreach (var sfx in soundEffects)
            {
                if (!string.IsNullOrEmpty(sfx.name) && sfx.clip != null)
                {
                    soundDictionary[sfx.name] = sfx;
                }
            }
        }

        RegisterQuickClip("erro", errorClip, 1f, false, 1f, 1f);
        RegisterQuickClip("btn_hover", hoverClip, 1f, true, 0.9f, 1.2f);
        RegisterQuickClip("click", clickClip, 1f, false, 1f, 1f);
        RegisterQuickClip("coleta", collectClip, 1f, true, 0.95f, 1.15f);
        RegisterQuickClip("collect", collectClip, 1f, true, 0.95f, 1.15f);
        RegisterQuickClip("dano", damageClip, 1f, true, 0.9f, 1.1f);
        RegisterQuickClip("damage", damageClip, 1f, true, 0.9f, 1.1f);
    }

    private void RegisterQuickClip(string sfxName, AudioClip clip, float volume, bool randomPitch, float minPitch, float maxPitch)
    {
        if (clip == null || soundDictionary.ContainsKey(sfxName)) return;

        soundDictionary[sfxName] = new SoundEffect
        {
            name = sfxName,
            clip = clip,
            volume = volume,
            randomPitch = randomPitch,
            minPitch = minPitch,
            maxPitch = maxPitch
        };
    }

    public void PlaySFX(string sfxName)
    {
        if (soundDictionary != null && soundDictionary.TryGetValue(sfxName, out SoundEffect sfx))
        {
            float pitch = sfx.randomPitch ? Random.Range(sfx.minPitch, sfx.maxPitch) : 1f;
            PlaySound(sfx.clip, sfx.volume, pitch);
        }
        else
        {
            Debug.LogWarning($"[SFXManager] Som '{sfxName}' não foi encontrado ou não tem AudioClip associado!");
        }
    }

    public void PlaySound(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (clip == null || audioChannels == null || audioChannels.Count == 0) return;

        AudioSource availableChannel = null;
        for (int i = 0; i < audioChannels.Count; i++)
        {
            int checkIndex = (currentChannelIndex + i) % audioChannels.Count;
            if (!audioChannels[checkIndex].isPlaying)
            {
                availableChannel = audioChannels[checkIndex];
                currentChannelIndex = (checkIndex + 1) % audioChannels.Count;
                break;
            }
        }

        if (availableChannel == null)
        {
            availableChannel = audioChannels[currentChannelIndex];
            currentChannelIndex = (currentChannelIndex + 1) % audioChannels.Count;
        }

        availableChannel.pitch = pitch;
        availableChannel.volume = volume * masterVolume;
        availableChannel.clip = clip;
        availableChannel.Play();
    }

    public void SetVolume(float newVolume)
    {
        masterVolume = Mathf.Clamp01(newVolume);
        PlayerPrefs.SetFloat("SFXVolume", masterVolume);
        PlayerPrefs.Save();
    }

    public float GetVolume() => masterVolume;
}
