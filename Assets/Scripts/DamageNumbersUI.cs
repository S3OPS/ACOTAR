using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ACOTAR
{
    /// <summary>
    /// Manages floating damage/heal numbers that appear during combat
    /// Provides visual feedback for damage dealt, healing received, and status effects
    /// Phase 8 Enhancement - Combat Polish
    /// </summary>
    public class DamageNumbersUI : MonoBehaviour
    {
        [Header("Prefab References")]
        [SerializeField] private GameObject damageNumberPrefab;
        [SerializeField] private Transform damageNumbersContainer;

        [Header("Settings")]
        [SerializeField] private float floatSpeed = 100f;
        [SerializeField] private float fadeDuration = 1.5f;
        [SerializeField] private float horizontalSpread = 50f;
        [SerializeField] private float verticalOffset = 30f;

        // Singleton pattern
        private static DamageNumbersUI _instance;
        public static DamageNumbersUI Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<DamageNumbersUI>();
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
        }

        /// <summary>
        /// Show a damage number at the specified position
        /// </summary>
        /// <param name="damage">Amount of damage to display</param>
        /// <param name="worldPosition">Position in world space</param>
        /// <param name="isCritical">Whether this is a critical hit</param>
        /// <param name="damageType">Type of damage for color coding</param>
        public void ShowDamage(int damage, Vector3 worldPosition, bool isCritical = false, DamageType damageType = DamageType.Physical)
        {
            if (damageNumberPrefab == null || damageNumbersContainer == null)
            {
                Debug.LogWarning("DamageNumbersUI: Missing prefab or container reference");
                return;
            }

            // Create damage number
            GameObject numberObj = Instantiate(damageNumberPrefab, damageNumbersContainer);
            Text numberText = numberObj.GetComponent<Text>();
            
            if (numberText == null)
            {
                Debug.LogError("DamageNumbersUI: Damage number prefab missing Text component");
                Destroy(numberObj);
                return;
            }

            // Set text and format
            string displayText = damage.ToString();
            if (isCritical)
            {
                displayText = $"CRIT! {damage}";
                numberText.fontSize += 6;
            }
            numberText.text = displayText;

            // Set color based on damage type
            numberText.color = GetDamageColor(damageType, isCritical);

            // Position on screen
            RectTransform rectTransform = numberObj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                // Add random horizontal spread
                float randomSpread = Random.Range(-horizontalSpread, horizontalSpread);
                Vector3 offset = new Vector3(randomSpread, verticalOffset, 0);
                
                // Convert world position to screen position
                Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
                rectTransform.position = screenPos + offset;
            }

            // Start animation
            StartCoroutine(AnimateDamageNumber(numberObj, numberText));
        }

        /// <summary>
        /// Show a healing number at the specified position
        /// </summary>
        public void ShowHealing(int healing, Vector3 worldPosition)
        {
            if (damageNumberPrefab == null || damageNumbersContainer == null)
            {
                return;
            }

            GameObject numberObj = Instantiate(damageNumberPrefab, damageNumbersContainer);
            Text numberText = numberObj.GetComponent<Text>();
            
            if (numberText == null)
            {
                Destroy(numberObj);
                return;
            }

            // Format as healing
            numberText.text = $"+{healing}";
            numberText.color = new Color(0.2f, 1f, 0.2f); // Bright green
            numberText.fontSize += 2;

            // Position on screen
            RectTransform rectTransform = numberObj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                float randomSpread = Random.Range(-horizontalSpread, horizontalSpread);
                Vector3 offset = new Vector3(randomSpread, verticalOffset, 0);
                Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
                rectTransform.position = screenPos + offset;
            }

            StartCoroutine(AnimateDamageNumber(numberObj, numberText));
        }

        /// <summary>
        /// Show a status effect notification
        /// </summary>
        public void ShowStatusEffect(string effectName, Vector3 worldPosition, bool isPositive = false)
        {
            if (damageNumberPrefab == null || damageNumbersContainer == null)
            {
                return;
            }

            GameObject numberObj = Instantiate(damageNumberPrefab, damageNumbersContainer);
            Text numberText = numberObj.GetComponent<Text>();
            
            if (numberText == null)
            {
                Destroy(numberObj);
                return;
            }

            // Format effect text
            numberText.text = effectName;
            numberText.color = isPositive ? new Color(0.3f, 0.7f, 1f) : new Color(0.9f, 0.4f, 0.9f);
            numberText.fontSize -= 2;

            // Position on screen
            RectTransform rectTransform = numberObj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
                rectTransform.position = screenPos + new Vector3(0, verticalOffset + 20, 0);
            }

            StartCoroutine(AnimateDamageNumber(numberObj, numberText));
        }

        /// <summary>
        /// Animate the damage number - float up and fade out
        /// </summary>
        private IEnumerator AnimateDamageNumber(GameObject numberObj, Text numberText)
        {
            float elapsedTime = 0f;
            RectTransform rectTransform = numberObj.GetComponent<RectTransform>();
            Vector3 startPos = rectTransform.position;
            Color startColor = numberText.color;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / fadeDuration;

                // Float upward
                if (rectTransform != null)
                {
                    rectTransform.position = startPos + Vector3.up * (floatSpeed * progress);
                }

                // Fade out
                if (numberText != null)
                {
                    Color newColor = startColor;
                    newColor.a = 1f - progress;
                    numberText.color = newColor;
                }

                yield return null;
            }

            // Clean up
            if (numberObj != null)
            {
                Destroy(numberObj);
            }
        }

        /// <summary>
        /// Get color for damage type
        /// </summary>
        private Color GetDamageColor(DamageType damageType, bool isCritical)
        {
            Color baseColor;
            
            switch (damageType)
            {
                case DamageType.Physical:
                    baseColor = new Color(1f, 0.9f, 0.7f); // Light yellow
                    break;
                case DamageType.Magical:
                    baseColor = new Color(0.8f, 0.6f, 1f); // Purple
                    break;
                case DamageType.Fire:
                    baseColor = new Color(1f, 0.4f, 0.2f); // Orange-red
                    break;
                case DamageType.Ice:
                    baseColor = new Color(0.6f, 0.9f, 1f); // Cyan
                    break;
                case DamageType.Darkness:
                    baseColor = new Color(0.5f, 0.3f, 0.7f); // Dark purple
                    break;
                case DamageType.Light:
                    baseColor = new Color(1f, 1f, 0.8f); // Bright yellow
                    break;
                case DamageType.Nature:
                    baseColor = new Color(0.4f, 0.9f, 0.4f); // Bright green
                    break;
                case DamageType.Death:
                    baseColor = new Color(0.3f, 0.3f, 0.3f); // Dark gray
                    break;
                default:
                    baseColor = Color.white;
                    break;
            }

            // v2.6.7: Make critical hits brighter and more vibrant
            if (isCritical)
            {
                baseColor = Color.Lerp(baseColor, Color.white, 0.3f);
                baseColor.a = 1f;
            }

            return baseColor;
        }

        /// <summary>
        /// Show mana cost notification
        /// </summary>
        public void ShowManaCost(int manaCost, Vector3 worldPosition)
        {
            if (damageNumberPrefab == null || damageNumbersContainer == null)
            {
                return;
            }

            GameObject numberObj = Instantiate(damageNumberPrefab, damageNumbersContainer);
            Text numberText = numberObj.GetComponent<Text>();
            
            if (numberText == null)
            {
                Destroy(numberObj);
                return;
            }

            // Format as mana cost
            numberText.text = $"-{manaCost} MP";
            numberText.color = new Color(0.5f, 0.7f, 1f); // Blue
            numberText.fontSize -= 2;

            // Position on screen
            RectTransform rectTransform = numberObj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
                rectTransform.position = screenPos + new Vector3(0, verticalOffset - 20, 0);
            }

            StartCoroutine(AnimateDamageNumber(numberObj, numberText));
        }
        
        /// <summary>
        /// Show combo cascade notification
        /// v2.6.7: NEW - Visual feedback for combo cascade bonuses
        /// </summary>
        /// <param name="comboCount">Total combo hits that triggered cascade</param>
        /// <param name="worldPosition">Position in world space</param>
        public void ShowComboCascade(int comboCount, Vector3 worldPosition)
        {
            if (damageNumberPrefab == null || damageNumbersContainer == null)
            {
                return;
            }

            GameObject numberObj = Instantiate(damageNumberPrefab, damageNumbersContainer);
            Text numberText = numberObj.GetComponent<Text>();
            
            if (numberText == null)
            {
                Destroy(numberObj);
                return;
            }

            // Format as cascade
            numberText.text = $"⚡ CASCADE {comboCount}x! ⚡";
            numberText.color = new Color(1f, 0.9f, 0.2f); // Bright gold
            numberText.fontSize += 8; // Make it bigger than normal
            
            // Apply bold style if possible
            numberText.fontStyle = FontStyle.Bold;

            // Position on screen
            RectTransform rectTransform = numberObj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
                rectTransform.position = screenPos + new Vector3(0, verticalOffset + 20, 0); // Higher than normal
            }

            StartCoroutine(AnimateDamageNumber(numberObj, numberText));
        }

        /// <summary>
        /// Show combo counter
        /// </summary>
        public void ShowCombo(int comboCount, Vector3 worldPosition)
        {
            if (damageNumberPrefab == null || damageNumbersContainer == null)
            {
                return;
            }

            GameObject numberObj = Instantiate(damageNumberPrefab, damageNumbersContainer);
            Text numberText = numberObj.GetComponent<Text>();
            
            if (numberText == null)
            {
                Destroy(numberObj);
                return;
            }

            // Format combo text
            numberText.text = $"COMBO x{comboCount}!";
            numberText.color = new Color(1f, 0.8f, 0.2f); // Gold
            numberText.fontSize += 4;

            // Position on screen
            RectTransform rectTransform = numberObj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
                rectTransform.position = screenPos + new Vector3(0, verticalOffset + 40, 0);
            }

            StartCoroutine(AnimateDamageNumber(numberObj, numberText));
        }
    }
}
