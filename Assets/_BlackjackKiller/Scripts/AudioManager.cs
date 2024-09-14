using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool; // Unity's built-in pooling system

public class AudioManager : Singleton<AudioManager>
{
    [Header("Audio Sources")]
    public AudioSource ambienceSource;

    [Header("Card Sounds")]
    public List<AudioClip> cardDrawSounds;
    public List<AudioClip> cardDiscardSounds;

    [Header("Combat Sounds")]
    public List<AudioClip> attackSounds;
    public List<AudioClip> hitSounds;
    public List<AudioClip> specialSounds;
    public List<AudioClip> bulletDropSounds;
    public List<AudioClip> bulletFireSounds;

    [Header("Game Sounds")]
    public AudioClip startRoundSound;
    public AudioClip endRoundSound;

    [Header("Blackjack Sounds")]
    public List<AudioClip> blackjackSounds;

    [Header("Ambience and Environmental Sounds")]
    public AudioClip backgroundAmbience;
    public List<AudioClip> occasionalEnvSounds;
    public float envSoundInterval = 30f; // Time between environmental sounds

    [Header("Pooling Settings")]
    public int poolSize = 10;
    public GameObject audioSourcePrefab;

    // Object pool for AudioSource
    private ObjectPool<AudioSource> audioSourcePool;

    private float envSoundTimer;

    private void Start()
    {
        // Initialize AudioSource pool
        audioSourcePool = new ObjectPool<AudioSource>(
            CreateAudioSource,
            OnGetAudioSource,
            OnReleaseAudioSource,
            OnDestroyAudioSource,
            true, poolSize, poolSize * 3
        );        

        PlayBackgroundAmbience();
        envSoundTimer = envSoundInterval;
        StartCoroutine(PlayOccasionalEnvironmentalSounds());
    }

    private void Update()
    {
        // Count down to play environmental sounds
        envSoundTimer -= Time.deltaTime;
        if (envSoundTimer <= 0)
        {
            PlayRandomEnvSound();
            envSoundTimer = envSoundInterval;
        }
    }

    // Public methods to play specific sounds
    public void PlayCardDrawSound()
    {
        PlayRandomSound(cardDrawSounds);
    }

    public void PlayCardDiscardSound()
    {
        PlayRandomSound(cardDiscardSounds);
    }

    public void PlayAttackSound()
    {
        PlayRandomSoundWithPitch(attackSounds);
    }

    public void PlayHitSound()
    {
        PlayRandomSoundWithPitch(hitSounds);
    }

    public void PlaySpecialSound()
    {
        PlayRandomSound(specialSounds);
    }

    public void PlayBulletDropSound()
    {
        PlayRandomSoundWithPitch(bulletDropSounds);
    }

    public void PlayBulletFireSound()
    {
        PlayRandomSoundWithPitch(bulletFireSounds);
    }

    public void PlayStartRoundSound()
    {
        PlaySound(startRoundSound);
    }

    public void PlayEndRoundSound()
    {
        PlaySound(endRoundSound);
    }

    public void PlayBlackjackSound()
    {
        PlayRandomSound(blackjackSounds);
    }

    private void PlayBackgroundAmbience()
    {
        ambienceSource.clip = backgroundAmbience;
        ambienceSource.loop = true;
        ambienceSource.Play();
    }

    private IEnumerator PlayOccasionalEnvironmentalSounds()
    {
        while (true)
        {
            yield return new WaitForSeconds(envSoundInterval);
            PlayRandomEnvSound();
        }
    }

    private void PlayRandomEnvSound()
    {
        PlayRandomSound(occasionalEnvSounds);
    }

    // Helper methods to play sounds using pooled AudioSource
    private void PlayRandomSound(List<AudioClip> clips)
    {
        if (clips.Count > 0)
        {
            AudioClip clip = clips[Random.Range(0, clips.Count)];
            AudioSource source = audioSourcePool.Get(); // Get AudioSource from the pool
            source.pitch = 1f; // Default pitch
            source.PlayOneShot(clip);

            StartCoroutine(ReturnSourceToPoolAfterPlayback(source, clip.length));
        }
    }

    private void PlayRandomSoundWithPitch(List<AudioClip> clips)
    {
        if (clips.Count > 0)
        {
            AudioClip clip = clips[Random.Range(0, clips.Count)];
            AudioSource source = audioSourcePool.Get(); // Get AudioSource from the pool
            source.pitch = Random.Range(0.9f, 1.1f); // Random pitch variation
            source.PlayOneShot(clip);

            StartCoroutine(ReturnSourceToPoolAfterPlayback(source, clip.length));
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            AudioSource source = audioSourcePool.Get(); // Get AudioSource from the pool
            source.pitch = 1f; // Default pitch
            source.PlayOneShot(clip);

            StartCoroutine(ReturnSourceToPoolAfterPlayback(source, clip.length));
        }
    }

    // Coroutine to return the AudioSource to the pool after the sound finishes
    private IEnumerator ReturnSourceToPoolAfterPlayback(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSourcePool.Release(source); // Return the AudioSource to the pool
    }

    // AudioSource pool lifecycle management
    private AudioSource CreateAudioSource()
    {
        GameObject audioSourceObj = Instantiate(audioSourcePrefab, transform);
        return audioSourceObj.GetComponent<AudioSource>();
    }

    private void OnGetAudioSource(AudioSource source)
    {
        source.gameObject.SetActive(true);
    }

    private void OnReleaseAudioSource(AudioSource source)
    {
        source.Stop();
        source.gameObject.SetActive(false);
    }

    private void OnDestroyAudioSource(AudioSource source)
    {
        Destroy(source.gameObject);
    }
}
