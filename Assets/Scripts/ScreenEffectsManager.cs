using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ACOTAR
{
    /// <summary>
    /// Types of screen effects
    /// </summary>
    public enum ScreenEffectType
    {
        Shake,
        Flash,
        Fade,
        Pulse,
        Vignette,
        ChromaticAberration,
        Blur,
        Distortion
    }

    /// <summary>
    /// Manages screen-level visual effects like shake, flash, fade
    /// </summary>
    public class ScreenEffectsManager : MonoBehaviour
    {
        public static ScreenEffectsManager Instance { get; private set; }

        [Header("Camera Reference")]
        [SerializeField] private Camera mainCamera;
        private Vector3 originalCameraPosition;

        [Header("Shake Settings")]
        [SerializeField] private float defaultShakeDuration = 0.3f;
        [SerializeField] private float defaultShakeIntensity = 0.1f;
        [SerializeField] private float shakeDecay = 0.9f;

        [Header("Flash Settings")]
        [SerializeField] private float defaultFlashDuration = 0.15f;
        [SerializeField] private Color defaultFlashColor = Color.white;

        [Header("Fade Settings")]
        [SerializeField] private float defaultFadeDuration = 0.5f;
        [SerializeField] private Color fadeColor = Color.black;

        [Header("UI Overlay")]
        [SerializeField] private CanvasGroup screenOverlay;

        // State tracking
        private bool isShaking = false;
        private bool isFading = false;
        private Coroutine currentShakeCoroutine;
        private Coroutine currentFadeCoroutine;
        private Coroutine currentFlashCoroutine;

        void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            InitializeScreenEffects();
        }

        void Start()
        {
            // Try to find main camera if not assigned
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            if (mainCamera != null)
            {
                originalCameraPosition = mainCamera.transform.localPosition;
            }
        }

        /// <summary>
        /// Initialize screen effects system
        /// </summary>
        private void InitializeScreenEffects()
        {
            Debug.Log("ScreenEffectsManager initialized");
        }

        #region Camera Shake

        /// <summary>
        /// Trigger camera shake with default settings
        /// </summary>
        public void Shake()
        {
            Shake(defaultShakeIntensity, defaultShakeDuration);
        }

        /// <summary>
        /// Trigger camera shake with custom settings
        /// </summary>
        public void Shake(float intensity, float duration)
        {
            if (!IsEffectEnabled()) return;
            if (mainCamera == null) return;

            if (currentShakeCoroutine != null)
            {
                StopCoroutine(currentShakeCoroutine);
            }

            currentShakeCoroutine = StartCoroutine(ShakeCoroutine(intensity, duration));
        }

        /// <summary>
        /// Combat-specific shake for hits
        /// </summary>
        public void CombatShake(bool isCritical = false)
        {
            float intensity = isCritical ? defaultShakeIntensity * 2f : defaultShakeIntensity;
            float duration = isCritical ? defaultShakeDuration * 1.5f : defaultShakeDuration;
            Shake(intensity, duration);
        }

        /// <summary>
        /// Heavy shake for boss attacks or major events
        /// </summary>
        public void HeavyShake()
        {
            Shake(defaultShakeIntensity * 3f, defaultShakeDuration * 2f);
        }

        private IEnumerator ShakeCoroutine(float intensity, float duration)
        {
            isShaking = true;
            float elapsed = 0f;
            float currentIntensity = intensity;

            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * currentIntensity;
                float y = Random.Range(-1f, 1f) * currentIntensity;

                mainCamera.transform.localPosition = originalCameraPosition + new Vector3(x, y, 0f);

                elapsed += Time.deltaTime;
                currentIntensity *= shakeDecay;

                yield return null;
            }

            mainCamera.transform.localPosition = originalCameraPosition;
            isShaking = false;
        }

        #endregion

        #region Screen Flash

        /// <summary>
        /// Flash screen with default white color
        /// </summary>
        public void Flash()
        {
            Flash(defaultFlashColor, defaultFlashDuration);
        }

        /// <summary>
        /// Flash screen with custom color
        /// </summary>
        public void Flash(Color color, float duration = 0.15f)
        {
            if (!IsEffectEnabled()) return;

            if (currentFlashCoroutine != null)
            {
                StopCoroutine(currentFlashCoroutine);
            }

            currentFlashCoroutine = StartCoroutine(FlashCoroutine(color, duration));
        }

        /// <summary>
        /// Damage flash (red tint)
        /// </summary>
        public void DamageFlash()
        {
            Flash(new Color(1f, 0f, 0f, 0.3f), 0.1f);
        }

        /// <summary>
        /// Heal flash (green tint)
        /// </summary>
        public void HealFlash()
        {
            Flash(new Color(0f, 1f, 0.5f, 0.3f), 0.2f);
        }

        /// <summary>
        /// Level up flash (gold)
        /// </summary>
        public void LevelUpFlash()
        {
            Flash(new Color(1f, 0.85f, 0f, 0.4f), 0.3f);
        }

        /// <summary>
        /// Critical hit flash (bright white)
        /// </summary>
        public void CriticalFlash()
        {
            Flash(new Color(1f, 1f, 1f, 0.5f), 0.1f);
        }

        /// <summary>
        /// Magic flash based on element
        /// </summary>
        public void MagicFlash(Element element)
        {
            if (VisualEffectsManager.Instance != null)
            {
                ElementColorTheme colors = VisualEffectsManager.Instance.GetElementColors(element);
                Color flashColor = colors.Core;
                flashColor.a = 0.3f;
                Flash(flashColor, 0.2f);
            }
        }

        private IEnumerator FlashCoroutine(Color color, float duration)
        {
            // In a full implementation, this would modify a UI overlay
            Debug.Log($"Screen flash: {color} for {duration}s");
            
            // Simulate flash with overlay if available
            if (screenOverlay != null)
            {
                // Would set overlay color and fade
            }

            yield return new WaitForSeconds(duration);
        }

        #endregion

        #region Screen Fade

        /// <summary>
        /// Fade to black
        /// </summary>
        public void FadeToBlack(float duration = 0.5f, System.Action onComplete = null)
        {
            FadeTo(Color.black, duration, onComplete);
        }

        /// <summary>
        /// Fade from black
        /// </summary>
        public void FadeFromBlack(float duration = 0.5f, System.Action onComplete = null)
        {
            FadeFrom(Color.black, duration, onComplete);
        }

        /// <summary>
        /// Fade to custom color
        /// </summary>
        public void FadeTo(Color color, float duration, System.Action onComplete = null)
        {
            if (currentFadeCoroutine != null)
            {
                StopCoroutine(currentFadeCoroutine);
            }

            currentFadeCoroutine = StartCoroutine(FadeToCoroutine(color, duration, onComplete));
        }

        /// <summary>
        /// Fade from custom color
        /// </summary>
        public void FadeFrom(Color color, float duration, System.Action onComplete = null)
        {
            if (currentFadeCoroutine != null)
            {
                StopCoroutine(currentFadeCoroutine);
            }

            currentFadeCoroutine = StartCoroutine(FadeFromCoroutine(color, duration, onComplete));
        }

        /// <summary>
        /// Full fade transition (fade out, action, fade in)
        /// </summary>
        public void FadeTransition(System.Action duringFade, float fadeOutDuration = 0.5f, float fadeInDuration = 0.5f)
        {
            StartCoroutine(FadeTransitionCoroutine(duringFade, fadeOutDuration, fadeInDuration));
        }

        private IEnumerator FadeToCoroutine(Color targetColor, float duration, System.Action onComplete)
        {
            isFading = true;
            float elapsed = 0f;

            Debug.Log($"Fading to {targetColor} over {duration}s");

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                // Would lerp overlay alpha
                elapsed += Time.deltaTime;
                yield return null;
            }

            isFading = false;
            onComplete?.Invoke();
        }

        private IEnumerator FadeFromCoroutine(Color fromColor, float duration, System.Action onComplete)
        {
            isFading = true;
            float elapsed = 0f;

            Debug.Log($"Fading from {fromColor} over {duration}s");

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                // Would lerp overlay alpha
                elapsed += Time.deltaTime;
                yield return null;
            }

            isFading = false;
            onComplete?.Invoke();
        }

        private IEnumerator FadeTransitionCoroutine(System.Action duringFade, float fadeOutDuration, float fadeInDuration)
        {
            // Fade out
            yield return FadeToCoroutine(Color.black, fadeOutDuration, null);
            
            // Execute action during black screen
            duringFade?.Invoke();
            
            yield return new WaitForSeconds(0.1f);
            
            // Fade in
            yield return FadeFromCoroutine(Color.black, fadeInDuration, null);
        }

        #endregion

        #region Screen Pulse

        /// <summary>
        /// Pulse the screen with a color (useful for combat rhythm or alerts)
        /// </summary>
        public void Pulse(Color color, float intensity = 0.2f, float speed = 2f, int pulseCount = 3)
        {
            if (!IsEffectEnabled()) return;
            StartCoroutine(PulseCoroutine(color, intensity, speed, pulseCount));
        }

        /// <summary>
        /// Alert pulse (red)
        /// </summary>
        public void AlertPulse()
        {
            Pulse(new Color(1f, 0f, 0f, 0.3f), 0.3f, 3f, 3);
        }

        /// <summary>
        /// Magic ready pulse
        /// </summary>
        public void MagicReadyPulse(Element element)
        {
            if (VisualEffectsManager.Instance != null)
            {
                ElementColorTheme colors = VisualEffectsManager.Instance.GetElementColors(element);
                Color pulseColor = colors.Core;
                pulseColor.a = 0.2f;
                Pulse(pulseColor, 0.2f, 2f, 2);
            }
        }

        private IEnumerator PulseCoroutine(Color color, float intensity, float speed, int pulseCount)
        {
            Debug.Log($"Screen pulse: {color}, {pulseCount} pulses at speed {speed}");

            for (int i = 0; i < pulseCount; i++)
            {
                float elapsed = 0f;
                float pulseDuration = 1f / speed;

                // Pulse in
                while (elapsed < pulseDuration / 2f)
                {
                    float t = elapsed / (pulseDuration / 2f);
                    // Would lerp overlay intensity
                    elapsed += Time.deltaTime;
                    yield return null;
                }

                // Pulse out
                while (elapsed < pulseDuration)
                {
                    float t = (elapsed - pulseDuration / 2f) / (pulseDuration / 2f);
                    // Would lerp overlay intensity back
                    elapsed += Time.deltaTime;
                    yield return null;
                }
            }
        }

        #endregion

        #region Vignette Effect

        /// <summary>
        /// Apply vignette effect (darkened edges)
        /// </summary>
        public void SetVignette(float intensity, float smoothness = 0.5f)
        {
            if (!IsEffectEnabled()) return;
            Debug.Log($"Vignette: intensity={intensity}, smoothness={smoothness}");
            // Would adjust post-processing vignette here
        }

        /// <summary>
        /// Damage vignette (red edges when low health)
        /// </summary>
        public void LowHealthVignette(float healthPercent)
        {
            if (healthPercent < 0.3f)
            {
                float intensity = Mathf.Lerp(0.3f, 0.6f, 1f - (healthPercent / 0.3f));
                SetVignette(intensity);
            }
            else
            {
                SetVignette(0f);
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Check if screen effects are enabled
        /// </summary>
        private bool IsEffectEnabled()
        {
            if (GraphicsManager.Instance != null)
            {
                return GraphicsManager.Instance.AreScreenEffectsEnabled;
            }
            return true;
        }

        /// <summary>
        /// Stop all active screen effects
        /// </summary>
        public void StopAllEffects()
        {
            StopAllCoroutines();
            
            if (mainCamera != null)
            {
                mainCamera.transform.localPosition = originalCameraPosition;
            }
            
            isShaking = false;
            isFading = false;

            Debug.Log("All screen effects stopped");
        }

        /// <summary>
        /// Set the main camera reference
        /// </summary>
        public void SetMainCamera(Camera camera)
        {
            mainCamera = camera;
            if (mainCamera != null)
            {
                originalCameraPosition = mainCamera.transform.localPosition;
            }
        }

        // Public state accessors
        public bool IsShaking => isShaking;
        public bool IsFading => isFading;

        #endregion

        #region Preset Combinations

        /// <summary>
        /// Combat hit feedback (shake + flash)
        /// </summary>
        public void CombatHitFeedback(bool isCritical = false, DamageType damageType = DamageType.Physical)
        {
            CombatShake(isCritical);
            
            if (isCritical)
            {
                CriticalFlash();
            }
            
            // Element-based flash
            Element element = GetElementFromDamageType(damageType);
            if (element != Element.None)
            {
                MagicFlash(element);
            }
        }

        /// <summary>
        /// Player takes damage feedback
        /// </summary>
        public void PlayerDamageFeedback(float damagePercent)
        {
            float shakeIntensity = Mathf.Lerp(0.05f, 0.2f, damagePercent);
            Shake(shakeIntensity, 0.2f);
            DamageFlash();
        }

        /// <summary>
        /// Player heals feedback
        /// </summary>
        public void PlayerHealFeedback()
        {
            HealFlash();
        }

        /// <summary>
        /// Level up celebration
        /// </summary>
        public void LevelUpFeedback()
        {
            LevelUpFlash();
            Shake(0.05f, 0.5f);
        }

        /// <summary>
        /// Quest complete feedback
        /// </summary>
        public void QuestCompleteFeedback()
        {
            Flash(new Color(1f, 0.9f, 0.4f, 0.3f), 0.4f);
        }

        /// <summary>
        /// Court transition effect
        /// </summary>
        public void CourtTransitionEffect(Court court)
        {
            if (VisualEffectsManager.Instance != null)
            {
                CourtColorTheme colors = VisualEffectsManager.Instance.GetCourtColors(court);
                Color transitionColor = colors.Primary;
                transitionColor.a = 0.5f;
                
                FadeTransition(() =>
                {
                    Flash(transitionColor, 0.5f);
                }, 0.5f, 0.8f);
            }
        }

        /// <summary>
        /// Get element from damage type for effects
        /// </summary>
        private Element GetElementFromDamageType(DamageType damageType)
        {
            return damageType switch
            {
                DamageType.Fire => Element.Fire,
                DamageType.Ice => Element.Ice,
                DamageType.Darkness => Element.Darkness,
                DamageType.Light => Element.Light,
                DamageType.Death => Element.Death,
                _ => Element.None
            };
        }

        #endregion
    }
}
