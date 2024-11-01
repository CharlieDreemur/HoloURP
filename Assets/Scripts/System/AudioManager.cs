using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
    }

    public List<Sound> sounds;
    private Dictionary<string, AudioClip> soundDictionary;
    private List<AudioSource> audioSourcePool;
    private int poolSize = 10;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        // Initialize the dictionary
        soundDictionary = new Dictionary<string, AudioClip>();
        foreach (Sound sound in sounds)
        {
            if (!soundDictionary.ContainsKey(sound.name))
            {
                soundDictionary.Add(sound.name, sound.clip);
            }
            else
            {
                Debug.LogWarning($"Duplicate sound name '{sound.name}' detected. Only the first will be used.");
            }
        }

        // Create an audio source pool
        audioSourcePool = new List<AudioSource>();
        for (int i = 0; i < poolSize; i++)
        {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            audioSourcePool.Add(newSource);
        }
    }

    public void Play(string soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out AudioClip clip))
        {
            Play(clip);
        }
        else
        {
            Debug.LogWarning($"Sound '{soundName}' not found in AudioManager.");
        }
    }

    public void Play(AudioClip clip)
    {
        // Find an available audio source in the pool
        AudioSource availableSource = audioSourcePool.Find(source => !source.isPlaying);

        // If no source is available, create a temporary one-shot audio source
        if (availableSource == null)
        {
            availableSource = gameObject.AddComponent<AudioSource>();
            availableSource.playOnAwake = false;
        }

        availableSource.clip = clip;
        availableSource.Play();
    }

}
