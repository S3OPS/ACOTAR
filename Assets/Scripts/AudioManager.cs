using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace ACOTAR
{
    /// <summary>
    /// Central audio management system for ACOTAR RPG
    /// Handles music, sound effects, and ambient audio
    /// Integrates with SettingsUI for volume control
    /// 
    /// v2.5.3: Enhanced with property accessors and defensive programming
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        public AudioSource musicSource;
        public AudioSource ambientSource;
        public AudioSource sfxSource;
        public AudioSource uiSource;

        [Header("Audio Mixer")]
        public AudioMixer audioMixer;

        [Header("Sound Library")]
        public SoundLibrary soundLibrary;

        [Header("Settings")]
        [Range(0f, 1f)] public float masterVolume = 1.0f;
        [Range(0f, 1f)] public float musicVolume = 0.8f;
        [Range(0f, 1f)] public float sfxVolume = 1.0f;
        [Range(0f, 1f)] public float ambientVolume = 0.6f;
        [Range(0f, 1f)] public float uiVolume = 0.8f;
        public bool isMuted = false;

        // Current playing tracks
        private AudioClip currentMusic;
        private AudioClip currentAmbient;
        private Coroutine musicFadeCoroutine;

        // Audio pooling for performance
        private Queue<AudioSource> sfxPool = new Queue<AudioSource>();
        private const int SFX_POOL_SIZE = 10;

        // Public property accessors for cleaner code (v2.5.3)
        /// <summary>
        /// Get the master volume (0-1)
        /// </summary>
        public float MasterVolume => masterVolume;

        /// <summary>
        /// Get the music volume (0-1)
        /// </summary>
        public float MusicVolume => musicVolume;

        /// <summary>
        /// Get the SFX volume (0-1)
        /// </summary>
        public float SFXVolume => sfxVolume;

        /// <summary>
        /// Get the ambient volume (0-1)
        /// </summary>
        public float AmbientVolume => ambientVolume;

        /// <summary>
        /// Get the UI volume (0-1)
        /// </summary>
        public float UIVolume => uiVolume;

        /// <summary>
        /// Check if audio is muted
        /// </summary>
        public bool IsMuted => isMuted;

        /// <summary>
        /// Get the currently playing music clip
        /// </summary>
        public AudioClip CurrentMusic => currentMusic;

        /// <summary>
        /// Get the currently playing ambient clip
        /// </summary>
        public AudioClip CurrentAmbient => currentAmbient;

        /// <summary>
        /// Check if the audio system is properly initialized
        /// </summary>
        public bool IsInitialized => musicSource != null && ambientSource != null && 
                                      sfxSource != null && uiSource != null && sfxPool != null;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAudioSources();
                InitializeSFXPool();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            LoadAudioSettings();
            ApplyVolumeSettings();
        }

        /// <summary>
        /// Initialize audio sources if not assigned
        /// </summary>
        private void InitializeAudioSources()
        {
            if (musicSource == null)
            {
                GameObject musicObj = new GameObject("MusicSource");
                musicObj.transform.SetParent(transform);
                musicSource = musicObj.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }

            if (ambientSource == null)
            {
                GameObject ambientObj = new GameObject("AmbientSource");
                ambientObj.transform.SetParent(transform);
                ambientSource = ambientObj.AddComponent<AudioSource>();
                ambientSource.loop = true;
                ambientSource.playOnAwake = false;
            }

            if (sfxSource == null)
            {
                GameObject sfxObj = new GameObject("SFXSource");
                sfxObj.transform.SetParent(transform);
                sfxSource = sfxObj.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
            }

            if (uiSource == null)
            {
                GameObject uiObj = new GameObject("UISource");
                uiObj.transform.SetParent(transform);
                uiSource = uiObj.AddComponent<AudioSource>();
                uiSource.playOnAwake = false;
            }
        }

        /// <summary>
        /// Initialize SFX object pool for performance
        /// </summary>
        private void InitializeSFXPool()
        {
            for (int i = 0; i < SFX_POOL_SIZE; i++)
            {
                GameObject poolObj = new GameObject($"SFXPoolSource_{i}");
                poolObj.transform.SetParent(transform);
                AudioSource source = poolObj.AddComponent<AudioSource>();
                source.playOnAwake = false;
                sfxPool.Enqueue(source);
            }
        }

        #region Music Control

        /// <summary>
        /// Play music with optional crossfade
        /// v2.5.3: Enhanced with defensive checks
        /// </summary>
        public void PlayMusic(AudioClip clip, float fadeInTime = 1.0f, bool loop = true)
        {
            // Defensive checks (v2.5.3)
            if (!IsInitialized)
            {
                Debug.LogWarning("AudioManager: Cannot play music - system not initialized");
                return;
            }

            if (clip == null)
            {
                Debug.LogWarning("AudioManager: Cannot play null music clip");
                return;
            }

            if (clip == currentMusic)
            {
                return; // Already playing this track
            }

            if (musicFadeCoroutine != null)
            {
                StopCoroutine(musicFadeCoroutine);
            }

            musicFadeCoroutine = StartCoroutine(CrossfadeMusic(clip, fadeInTime, loop));
        }

        /// <summary>
        /// Play music by name from sound library
        /// v2.5.3: Enhanced with defensive checks
        /// </summary>
        public void PlayMusicByName(string musicName, float fadeInTime = 1.0f)
        {
            // Defensive checks (v2.5.3)
            if (!IsInitialized)
            {
                Debug.LogWarning("AudioManager: Cannot play music by name - system not initialized");
                return;
            }

            if (string.IsNullOrEmpty(musicName))
            {
                Debug.LogWarning("AudioManager: Cannot play music with null or empty name");
                return;
            }

            if (soundLibrary == null)
            {
                Debug.LogWarning("AudioManager: Sound library not assigned");
                return;
            }
            
            AudioClip clip = soundLibrary.GetMusicClip(musicName);
            if (clip != null)
            {
                PlayMusic(clip, fadeInTime);
            }
            else
            {
                Debug.LogWarning($"AudioManager: Music clip '{musicName}' not found in sound library");
            }
        }

        /// <summary>
        /// Stop music with optional fade out
        /// </summary>
        public void StopMusic(float fadeOutTime = 1.0f)
        {
            if (musicFadeCoroutine != null)
            {
                StopCoroutine(musicFadeCoroutine);
            }

            musicFadeCoroutine = StartCoroutine(FadeOutMusic(fadeOutTime));
        }

        /// <summary>
        /// Crossfade between music tracks
        /// </summary>
        private IEnumerator CrossfadeMusic(AudioClip newClip, float fadeTime, bool loop)
        {
            float startVolume = musicSource.volume;

            // Fade out current music
            if (musicSource.isPlaying)
            {
                float timer = 0f;
                while (timer < fadeTime / 2f)
                {
                    timer += Time.deltaTime;
                    musicSource.volume = Mathf.Lerp(startVolume, 0f, timer / (fadeTime / 2f));
                    yield return null;
                }
            }

            // Switch to new music
            musicSource.clip = newClip;
            musicSource.loop = loop;
            musicSource.Play();
            currentMusic = newClip;

            // Fade in new music
            float targetVolume = musicVolume * masterVolume;
            float timer2 = 0f;
            while (timer2 < fadeTime / 2f)
            {
                timer2 += Time.deltaTime;
                musicSource.volume = Mathf.Lerp(0f, targetVolume, timer2 / (fadeTime / 2f));
                yield return null;
            }

            musicSource.volume = targetVolume;
            musicFadeCoroutine = null;
        }

        /// <summary>
        /// Fade out music
        /// </summary>
        private IEnumerator FadeOutMusic(float fadeTime)
        {
            float startVolume = musicSource.volume;
            float timer = 0f;

            while (timer < fadeTime)
            {
                timer += Time.deltaTime;
                musicSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeTime);
                yield return null;
            }

            musicSource.Stop();
            musicSource.volume = startVolume;
            currentMusic = null;
            musicFadeCoroutine = null;
        }

        #endregion

        #region Ambient Sound Control

        /// <summary>
        /// Play ambient sound loop
        /// v2.5.3: Enhanced with defensive checks
        /// </summary>
        public void PlayAmbient(AudioClip clip, float fadeInTime = 2.0f)
        {
            // Defensive checks (v2.5.3)
            if (!IsInitialized)
            {
                Debug.LogWarning("AudioManager: Cannot play ambient - system not initialized");
                return;
            }

            if (clip == null)
            {
                Debug.LogWarning("AudioManager: Cannot play null ambient clip");
                return;
            }

            if (clip == currentAmbient)
            {
                return; // Already playing this ambient
            }

            StartCoroutine(FadeAmbient(clip, fadeInTime));
        }

        /// <summary>
        /// Play ambient by name from sound library
        /// v2.5.3: Enhanced with defensive checks
        /// </summary>
        public void PlayAmbientByName(string ambientName, float fadeInTime = 2.0f)
        {
            // Defensive checks (v2.5.3)
            if (!IsInitialized)
            {
                Debug.LogWarning("AudioManager: Cannot play ambient by name - system not initialized");
                return;
            }

            if (string.IsNullOrEmpty(ambientName))
            {
                Debug.LogWarning("AudioManager: Cannot play ambient with null or empty name");
                return;
            }

            if (soundLibrary == null)
            {
                Debug.LogWarning("AudioManager: Sound library not assigned");
                return;
            }

            AudioClip clip = soundLibrary.GetAmbientClip(ambientName);
            if (clip != null)
            {
                PlayAmbient(clip, fadeInTime);
            }
            else
            {
                Debug.LogWarning($"AudioManager: Ambient clip '{ambientName}' not found in sound library");
            }
        }

        /// <summary>
        /// Stop ambient sound
        /// </summary>
        public void StopAmbient(float fadeOutTime = 2.0f)
        {
            StartCoroutine(FadeOutAmbient(fadeOutTime));
        }

        /// <summary>
        /// Fade ambient sound in
        /// </summary>
        private IEnumerator FadeAmbient(AudioClip newClip, float fadeTime)
        {
            // Fade out current
            if (ambientSource.isPlaying)
            {
                float startVolume = ambientSource.volume;
                float timer = 0f;
                while (timer < fadeTime / 2f)
                {
                    timer += Time.deltaTime;
                    ambientSource.volume = Mathf.Lerp(startVolume, 0f, timer / (fadeTime / 2f));
                    yield return null;
                }
            }

            // Switch clip
            ambientSource.clip = newClip;
            ambientSource.loop = true;
            ambientSource.Play();
            currentAmbient = newClip;

            // Fade in new
            float targetVolume = ambientVolume * masterVolume;
            float timer2 = 0f;
            while (timer2 < fadeTime / 2f)
            {
                timer2 += Time.deltaTime;
                ambientSource.volume = Mathf.Lerp(0f, targetVolume, timer2 / (fadeTime / 2f));
                yield return null;
            }

            ambientSource.volume = targetVolume;
        }

        /// <summary>
        /// Fade ambient sound out
        /// </summary>
        private IEnumerator FadeOutAmbient(float fadeTime)
        {
            float startVolume = ambientSource.volume;
            float timer = 0f;

            while (timer < fadeTime)
            {
                timer += Time.deltaTime;
                ambientSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeTime);
                yield return null;
            }

            ambientSource.Stop();
            currentAmbient = null;
        }

        #endregion

        #region Sound Effects

        /// <summary>
        /// Play a sound effect once
        /// v2.5.3: Enhanced with defensive checks
        /// </summary>
        public void PlaySFX(AudioClip clip, float volumeScale = 1.0f, float pitch = 1.0f)
        {
            // Defensive checks (v2.5.3)
            if (!IsInitialized)
            {
                Debug.LogWarning("AudioManager: Cannot play SFX - system not initialized");
                return;
            }

            if (clip == null)
            {
                Debug.LogWarning("AudioManager: Cannot play null SFX clip");
                return;
            }

            if (isMuted)
            {
                return; // Audio is muted
            }

            if (sfxPool.Count > 0)
            {
                AudioSource pooledSource = sfxPool.Dequeue();
                StartCoroutine(PlayPooledSFX(pooledSource, clip, volumeScale, pitch));
            }
            else
            {
                // Fallback if pool is exhausted
                sfxSource.pitch = pitch;
                sfxSource.PlayOneShot(clip, sfxVolume * masterVolume * volumeScale);
            }
        }

        /// <summary>
        /// Play SFX by name from sound library
        /// v2.5.3: Enhanced with defensive checks
        /// </summary>
        public void PlaySFXByName(string sfxName, float volumeScale = 1.0f, float pitch = 1.0f)
        {
            // Defensive checks (v2.5.3)
            if (!IsInitialized)
            {
                Debug.LogWarning("AudioManager: Cannot play SFX by name - system not initialized");
                return;
            }

            if (string.IsNullOrEmpty(sfxName))
            {
                Debug.LogWarning("AudioManager: Cannot play SFX with null or empty name");
                return;
            }

            if (soundLibrary == null)
            {
                Debug.LogWarning("AudioManager: Sound library not assigned");
                return;
            }

            AudioClip clip = soundLibrary.GetSFXClip(sfxName);
            if (clip != null)
            {
                PlaySFX(clip, volumeScale, pitch);
            }
            else
            {
                Debug.LogWarning($"AudioManager: SFX clip '{sfxName}' not found in sound library");
            }
        }

        /// <summary>
        /// Play pooled SFX and return to pool
        /// </summary>
        private IEnumerator PlayPooledSFX(AudioSource source, AudioClip clip, float volumeScale, float pitch)
        {
            source.clip = clip;
            source.volume = sfxVolume * masterVolume * volumeScale;
            source.pitch = pitch;
            source.Play();

            yield return new WaitForSeconds(clip.length / pitch);

            source.Stop();
            sfxPool.Enqueue(source);
        }

        /// <summary>
        /// Play UI sound effect
        /// </summary>
        public void PlayUISFX(AudioClip clip, float volumeScale = 1.0f)
        {
            if (clip == null || isMuted) return;
            uiSource.PlayOneShot(clip, uiVolume * masterVolume * volumeScale);
        }

        /// <summary>
        /// Play UI SFX by name
        /// </summary>
        public void PlayUISFXByName(string sfxName, float volumeScale = 1.0f)
        {
            if (soundLibrary == null) return;

            AudioClip clip = soundLibrary.GetUISFXClip(sfxName);
            if (clip != null)
            {
                PlayUISFX(clip, volumeScale);
            }
        }

        #endregion

        #region Volume Control

        /// <summary>
        /// Set master volume
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            ApplyVolumeSettings();
            SaveAudioSettings();
        }

        /// <summary>
        /// Set music volume
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            musicSource.volume = musicVolume * masterVolume;
            SaveAudioSettings();
        }

        /// <summary>
        /// Set SFX volume
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            SaveAudioSettings();
        }

        /// <summary>
        /// Set ambient volume
        /// </summary>
        public void SetAmbientVolume(float volume)
        {
            ambientVolume = Mathf.Clamp01(volume);
            ambientSource.volume = ambientVolume * masterVolume;
            SaveAudioSettings();
        }

        /// <summary>
        /// Set UI volume
        /// </summary>
        public void SetUIVolume(float volume)
        {
            uiVolume = Mathf.Clamp01(volume);
            SaveAudioSettings();
        }

        /// <summary>
        /// Mute/unmute all audio
        /// </summary>
        public void SetMuted(bool muted)
        {
            isMuted = muted;
            AudioListener.volume = muted ? 0f : 1f;
            SaveAudioSettings();
        }

        /// <summary>
        /// Apply all volume settings
        /// </summary>
        private void ApplyVolumeSettings()
        {
            if (musicSource != null)
                musicSource.volume = musicVolume * masterVolume;
            
            if (ambientSource != null)
                ambientSource.volume = ambientVolume * masterVolume;
            
            AudioListener.volume = isMuted ? 0f : 1f;

            // Apply to AudioMixer if available
            if (audioMixer != null)
            {
                audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20);
                audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * 20);
                audioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * 20);
                audioMixer.SetFloat("AmbientVolume", Mathf.Log10(ambientVolume) * 20);
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Stop all audio
        /// </summary>
        public void StopAll()
        {
            musicSource.Stop();
            ambientSource.Stop();
            sfxSource.Stop();
            uiSource.Stop();
        }

        /// <summary>
        /// Pause all audio
        /// </summary>
        public void PauseAll()
        {
            musicSource.Pause();
            ambientSource.Pause();
        }

        /// <summary>
        /// Resume all audio
        /// </summary>
        public void ResumeAll()
        {
            musicSource.UnPause();
            ambientSource.UnPause();
        }

        /// <summary>
        /// Check if music is playing
        /// </summary>
        public bool IsMusicPlaying()
        {
            return musicSource != null && musicSource.isPlaying;
        }

        /// <summary>
        /// Get current music clip
        /// </summary>
        public AudioClip GetCurrentMusic()
        {
            return currentMusic;
        }

        #endregion

        #region Save/Load Settings

        /// <summary>
        /// Save audio settings to PlayerPrefs
        /// </summary>
        private void SaveAudioSettings()
        {
            PlayerPrefs.SetFloat("Audio_MasterVolume", masterVolume);
            PlayerPrefs.SetFloat("Audio_MusicVolume", musicVolume);
            PlayerPrefs.SetFloat("Audio_SFXVolume", sfxVolume);
            PlayerPrefs.SetFloat("Audio_AmbientVolume", ambientVolume);
            PlayerPrefs.SetFloat("Audio_UIVolume", uiVolume);
            PlayerPrefs.SetInt("Audio_Muted", isMuted ? 1 : 0);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Load audio settings from PlayerPrefs
        /// </summary>
        private void LoadAudioSettings()
        {
            masterVolume = PlayerPrefs.GetFloat("Audio_MasterVolume", 1.0f);
            musicVolume = PlayerPrefs.GetFloat("Audio_MusicVolume", 0.8f);
            sfxVolume = PlayerPrefs.GetFloat("Audio_SFXVolume", 1.0f);
            ambientVolume = PlayerPrefs.GetFloat("Audio_AmbientVolume", 0.6f);
            uiVolume = PlayerPrefs.GetFloat("Audio_UIVolume", 0.8f);
            isMuted = PlayerPrefs.GetInt("Audio_Muted", 0) == 1;
        }

        #endregion
    }

    /// <summary>
    /// Sound library to hold all audio clips organized by category
    /// </summary>
    [System.Serializable]
    public class SoundLibrary
    {
        [Header("Music")]
        public List<NamedAudioClip> musicTracks = new List<NamedAudioClip>();

        [Header("Ambient Sounds")]
        public List<NamedAudioClip> ambientSounds = new List<NamedAudioClip>();

        [Header("Sound Effects")]
        public List<NamedAudioClip> soundEffects = new List<NamedAudioClip>();

        [Header("UI Sounds")]
        public List<NamedAudioClip> uiSounds = new List<NamedAudioClip>();

        public AudioClip GetMusicClip(string name)
        {
            return GetClipByName(musicTracks, name);
        }

        public AudioClip GetAmbientClip(string name)
        {
            return GetClipByName(ambientSounds, name);
        }

        public AudioClip GetSFXClip(string name)
        {
            return GetClipByName(soundEffects, name);
        }

        public AudioClip GetUISFXClip(string name)
        {
            return GetClipByName(uiSounds, name);
        }

        private AudioClip GetClipByName(List<NamedAudioClip> list, string name)
        {
            foreach (var namedClip in list)
            {
                if (namedClip.name == name)
                    return namedClip.clip;
            }
            return null;
        }
    }

    /// <summary>
    /// Named audio clip for organization
    /// </summary>
    [System.Serializable]
    public class NamedAudioClip
    {
        public string name;
        public AudioClip clip;
    }
}
