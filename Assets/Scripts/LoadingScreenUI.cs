using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace ACOTAR
{
    /// <summary>
    /// Loading screen system with lore tips and progress display
    /// Shows during scene transitions and long operations
    /// </summary>
    public class LoadingScreenUI : MonoBehaviour
    {
        public static LoadingScreenUI Instance { get; private set; }

        [Header("UI References")]
        public GameObject loadingPanel;
        public Image loadingBackground;
        public Slider progressBar;
        public Text progressText;
        public Text loadingTipText;
        public Text loadingStatusText;
        public Image spinnerImage;
        
        [Header("Loading Tips")]
        [TextArea(3, 6)]
        public string[] loreTips;

        private bool isLoading = false;
        private float currentProgress = 0f;
        private Coroutine spinnerCoroutine;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            InitializeLoreTips();
            HideLoadingScreen();
        }

        /// <summary>
        /// Initialize lore tips if not set in inspector
        /// </summary>
        private void InitializeLoreTips()
        {
            if (loreTips == null || loreTips.Length == 0)
            {
                loreTips = new string[]
                {
                    "The Wall was created by the Treaty to separate the mortal and Fae realms.",
                    "There are seven High Fae Courts in Prythian, each ruled by a High Lord.",
                    "Calanmai, or Fire Night, is an ancient Spring Court ritual held on the first full moon of spring.",
                    "Velaris, the City of Starlight, remained hidden for 5,000 years Under the Mountain.",
                    "The Night Court is split between two cities: Velaris and the Court of Nightmares.",
                    "Illyrians are a warrior race known for their bat-like wings and exceptional combat skills.",
                    "Winnowing is the Fae ability to teleport across distances instantaneously.",
                    "Daemati are rare Fae with the ability to read and control minds.",
                    "The Suriel are ancient creatures bound by fate to answer questions when captured.",
                    "Starfall is a celestial event unique to the Night Court, when spirits cross the sky.",
                    "The Bone Carver is an ancient being imprisoned in the Prison, capable of appearing as your greatest fear or desire.",
                    "The Weaver lives in the woods near Velaris and collects beautiful things... including heads.",
                    "Amren is not High Fae - she's an ancient being who chose a Fae body as her prison.",
                    "The Cauldron is the source of all magic and life in Prythian.",
                    "Being Made by the Cauldron transforms a mortal into High Fae with enhanced power.",
                    "Under the Mountain, Amarantha held all of Prythian captive for 49 years.",
                    "The three trials tested Feyre's strength, cunning, and heart.",
                    "Rhysand is the most powerful High Lord in Prythian's history.",
                    "The Inner Circle is Rhysand's closest friends and most trusted advisors.",
                    "Summer Court's palace in Adriata sits on crystal-clear waters overlooking the sea.",
                    "The Autumn Court is known for its fire magic and political intrigue.",
                    "The Winter Court thrives in eternal snow with powerful ice magic.",
                    "The Dawn Court specializes in healing magic and diplomatic relations.",
                    "The Day Court's High Lord Helion is known for his vast library and light magic.",
                    "Tamlin's beast form is a massive creature with emerald eyes and deadly claws.",
                    "Lucien is the son of the Autumn Court's High Lord but serves Spring.",
                    "Mor is the Court of Nightmares' heir but chose to live in Velaris.",
                    "Cassian commands the Illyrian armies and is the most skilled warrior alive.",
                    "Azriel is the Night Court's spymaster and a shadowsinger.",
                    "Nesta and Elain were Made by the Cauldron against their will.",
                    "The King of Hybern sought to break the wall and enslave humanity.",
                    "The Book of Breathings reveals truths about the Cauldron.",
                    "Magic is stronger during full moons and weaker during new moons.",
                    "Court tokens are rare currencies used only in specific High Fae courts.",
                    "Glamours are illusion spells used to hide one's true appearance.",
                    "Mates are rare soulmates whose bond transcends death itself."
                };
            }
        }

        /// <summary>
        /// Show loading screen with a random lore tip
        /// </summary>
        public void ShowLoadingScreen(string statusText = "Loading...")
        {
            if (loadingPanel != null)
            {
                loadingPanel.SetActive(true);
                isLoading = true;
                currentProgress = 0f;
                
                UpdateProgressBar(0f);
                UpdateStatusText(statusText);
                DisplayRandomTip();
                
                // Start spinner animation
                if (spinnerImage != null && spinnerCoroutine == null)
                {
                    spinnerCoroutine = StartCoroutine(SpinLoadingIcon());
                }
            }
        }

        /// <summary>
        /// Hide loading screen
        /// </summary>
        public void HideLoadingScreen()
        {
            if (loadingPanel != null)
            {
                loadingPanel.SetActive(false);
                isLoading = false;
                
                // Stop spinner
                if (spinnerCoroutine != null)
                {
                    StopCoroutine(spinnerCoroutine);
                    spinnerCoroutine = null;
                }
            }
        }

        /// <summary>
        /// Update loading progress (0-1)
        /// </summary>
        public void UpdateProgress(float progress, string status = null)
        {
            currentProgress = Mathf.Clamp01(progress);
            UpdateProgressBar(currentProgress);
            
            if (!string.IsNullOrEmpty(status))
            {
                UpdateStatusText(status);
            }
        }

        /// <summary>
        /// Update the progress bar visual
        /// </summary>
        private void UpdateProgressBar(float progress)
        {
            if (progressBar != null)
            {
                progressBar.value = progress;
            }
            
            if (progressText != null)
            {
                progressText.text = $"{Mathf.RoundToInt(progress * 100)}%";
            }
        }

        /// <summary>
        /// Update status text
        /// </summary>
        private void UpdateStatusText(string status)
        {
            if (loadingStatusText != null)
            {
                loadingStatusText.text = status;
            }
        }

        /// <summary>
        /// Display a random lore tip
        /// </summary>
        private void DisplayRandomTip()
        {
            if (loadingTipText != null && loreTips != null && loreTips.Length > 0)
            {
                int randomIndex = Random.Range(0, loreTips.Length);
                loadingTipText.text = $"Tip: {loreTips[randomIndex]}";
            }
        }

        /// <summary>
        /// Animate the loading spinner
        /// </summary>
        private IEnumerator SpinLoadingIcon()
        {
            if (spinnerImage == null) yield break;
            
            while (isLoading)
            {
                spinnerImage.transform.Rotate(0f, 0f, -360f * Time.deltaTime);
                yield return null;
            }
        }

        /// <summary>
        /// Load a scene asynchronously with loading screen
        /// </summary>
        public void LoadSceneAsync(string sceneName)
        {
            StartCoroutine(LoadSceneAsyncCoroutine(sceneName));
        }

        /// <summary>
        /// Coroutine to load scene with progress tracking
        /// </summary>
        private IEnumerator LoadSceneAsyncCoroutine(string sceneName)
        {
            ShowLoadingScreen($"Loading {sceneName}...");
            
            // Begin async load
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;
            
            // Wait for load to complete
            while (!asyncLoad.isDone)
            {
                // Progress goes from 0 to 0.9 during loading
                float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
                UpdateProgress(progress);
                
                // When loading is complete
                if (asyncLoad.progress >= 0.9f)
                {
                    UpdateProgress(1f, "Press any key to continue...");
                    
                    // Wait for player input
                    yield return new WaitUntil(() => Input.anyKeyDown);
                    
                    // Activate the scene
                    asyncLoad.allowSceneActivation = true;
                }
                
                yield return null;
            }
            
            HideLoadingScreen();
        }

        /// <summary>
        /// Show loading screen for a long operation
        /// </summary>
        public IEnumerator ShowLoadingForOperation(System.Action operation, string statusText = "Processing...")
        {
            ShowLoadingScreen(statusText);
            
            // Simulate progress
            float elapsedTime = 0f;
            float estimatedDuration = 2f; // 2 seconds estimated
            
            while (elapsedTime < estimatedDuration * 0.9f)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / estimatedDuration;
                UpdateProgress(progress);
                yield return null;
            }
            
            // Perform the operation
            operation?.Invoke();
            
            // Complete progress
            UpdateProgress(1f, "Complete!");
            yield return new WaitForSeconds(0.5f);
            
            HideLoadingScreen();
        }

        /// <summary>
        /// Update loading tip during long loads
        /// </summary>
        public void ChangeTip()
        {
            DisplayRandomTip();
        }
    }
}
