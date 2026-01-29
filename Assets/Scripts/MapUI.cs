using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ACOTAR
{
    /// <summary>
    /// Map system visualization for Prythian
    /// Shows all locations, their connections, and player's position
    /// </summary>
    public class MapUI : MonoBehaviour
    {
        public static MapUI Instance { get; private set; }

        [Header("UI References")]
        public GameObject mapPanel;
        public GameObject mapContainer;
        public GameObject locationMarkerPrefab;
        public Image mapBackground;
        public Text currentLocationText;
        public Text locationDetailsText;
        
        [Header("Location Display")]
        public GameObject locationInfoPanel;
        public Text locationNameText;
        public Text locationTypeText;
        public Text locationCourtText;
        public Text locationDescriptionText;
        public Button travelButton;
        public GameObject lockedOverlay;
        
        [Header("Legend")]
        public GameObject legendPanel;
        public Image playerMarkerLegend;
        public Image visitedMarkerLegend;
        public Image unvisitedMarkerLegend;
        public Image lockedMarkerLegend;
        
        [Header("Navigation")]
        public Button closeButton;
        public Button zoomInButton;
        public Button zoomOutButton;
        public Dropdown courtFilterDropdown;
        
        private Dictionary<string, GameObject> locationMarkers = new Dictionary<string, GameObject>();
        private string selectedLocation = null;
        private float currentZoom = 1.0f;
        private const float MIN_ZOOM = 0.5f;
        private const float MAX_ZOOM = 2.0f;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            SetupListeners();
            InitializeMap();
            HidePanel();
        }

        /// <summary>
        /// Setup UI event listeners
        /// </summary>
        private void SetupListeners()
        {
            if (closeButton != null) closeButton.onClick.AddListener(HidePanel);
            if (zoomInButton != null) zoomInButton.onClick.AddListener(ZoomIn);
            if (zoomOutButton != null) zoomOutButton.onClick.AddListener(ZoomOut);
            if (travelButton != null) travelButton.onClick.AddListener(OnTravelButtonClicked);
            if (courtFilterDropdown != null) courtFilterDropdown.onValueChanged.AddListener(OnCourtFilterChanged);
        }

        /// <summary>
        /// Initialize the map with all locations
        /// </summary>
        private void InitializeMap()
        {
            if (GameManager.Instance == null || GameManager.Instance.locationManager == null)
            {
                Debug.LogWarning("MapUI: LocationManager not available");
                return;
            }

            // Clear existing markers
            ClearMarkers();

            // Get all locations from LocationManager
            var allLocations = GameManager.Instance.locationManager.GetAllLocations();
            
            foreach (var location in allLocations)
            {
                CreateLocationMarker(location);
            }

            PopulateCourtFilter();
            UpdateCurrentLocationDisplay();
        }

        /// <summary>
        /// Create a marker for a location on the map
        /// </summary>
        private void CreateLocationMarker(Location location)
        {
            if (locationMarkerPrefab == null || mapContainer == null) return;

            GameObject marker = Instantiate(locationMarkerPrefab, mapContainer.transform);
            
            // Position marker based on location (simplified positioning)
            RectTransform rectTransform = marker.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                Vector2 position = GetLocationPosition(location);
                rectTransform.anchoredPosition = position;
            }

            // Setup marker visuals
            Image markerImage = marker.GetComponent<Image>();
            if (markerImage != null)
            {
                markerImage.color = GetLocationMarkerColor(location);
            }

            // Add click listener
            Button markerButton = marker.GetComponent<Button>();
            if (markerButton != null)
            {
                string locationName = location.name;
                markerButton.onClick.AddListener(() => OnLocationMarkerClicked(locationName));
            }

            // Add label
            Text labelText = marker.GetComponentInChildren<Text>();
            if (labelText != null)
            {
                labelText.text = location.name;
            }

            // Store reference
            locationMarkers[location.name] = marker;

            // Check if location is unlocked
            if (GameManager.Instance.storyManager != null)
            {
                bool isUnlocked = GameManager.Instance.storyManager.IsLocationUnlocked(location.name);
                if (!isUnlocked)
                {
                    // Gray out locked locations
                    if (markerImage != null)
                    {
                        Color lockedColor = markerImage.color;
                        lockedColor.a = 0.3f;
                        markerImage.color = lockedColor;
                    }
                }
            }
        }

        /// <summary>
        /// Get the 2D position for a location on the map
        /// This is a simplified positioning system
        /// </summary>
        private Vector2 GetLocationPosition(Location location)
        {
            // Map positions based on court and location type
            // These are approximate positions for Prythian's geography
            
            float x = 0f;
            float y = 0f;
            
            switch (location.court)
            {
                case Court.Spring:
                    x = -150f; y = -100f;
                    break;
                case Court.Summer:
                    x = 0f; y = -200f;
                    break;
                case Court.Autumn:
                    x = -200f; y = 100f;
                    break;
                case Court.Winter:
                    x = 200f; y = 150f;
                    break;
                case Court.Night:
                    x = 100f; y = -50f;
                    if (location.name == "Velaris") x = 150f;
                    if (location.name == "Hewn City") x = 80f;
                    break;
                case Court.Dawn:
                    x = -100f; y = 200f;
                    break;
                case Court.Day:
                    x = 200f; y = -100f;
                    break;
                case Court.None:
                    if (location.name.Contains("Human")) x = -300f;
                    if (location.name.Contains("Mountain")) x = 0f;
                    y = 0f;
                    break;
            }

            // Offset based on location type
            if (location.locationType == LocationType.City) y += 10f;
            else if (location.locationType == LocationType.Village) y -= 10f;
            
            return new Vector2(x, y);
        }

        /// <summary>
        /// Get marker color based on location status
        /// </summary>
        private Color GetLocationMarkerColor(Location location)
        {
            if (GameManager.Instance == null) return Color.white;

            string currentLoc = GameManager.Instance.currentLocation;
            
            // Current location - bright gold
            if (location.name == currentLoc)
            {
                return new Color(1f, 0.84f, 0f); // Gold
            }

            // Visited location - blue
            // (Would need to track visited locations in SaveData)
            
            // Unvisited but unlocked - white
            return Color.white;
        }

        /// <summary>
        /// Handle location marker click
        /// </summary>
        private void OnLocationMarkerClicked(string locationName)
        {
            selectedLocation = locationName;
            DisplayLocationInfo(locationName);
            
            // Highlight selected marker
            HighlightMarker(locationName);
        }

        /// <summary>
        /// Display detailed information about a location
        /// </summary>
        private void DisplayLocationInfo(string locationName)
        {
            if (GameManager.Instance == null || GameManager.Instance.locationManager == null)
                return;

            Location location = GameManager.Instance.locationManager.GetLocation(locationName);
            if (location == null) return;

            // Show info panel
            if (locationInfoPanel != null)
            {
                locationInfoPanel.SetActive(true);
            }

            // Update texts
            if (locationNameText != null) locationNameText.text = location.name;
            if (locationTypeText != null) locationTypeText.text = location.locationType.ToString();
            if (locationCourtText != null) locationCourtText.text = location.court.ToString() + " Court";
            if (locationDescriptionText != null) locationDescriptionText.text = location.description;

            // Check if location is unlocked
            bool isUnlocked = true;
            if (GameManager.Instance.storyManager != null)
            {
                isUnlocked = GameManager.Instance.storyManager.IsLocationUnlocked(locationName);
            }

            // Update travel button
            bool isCurrentLocation = (GameManager.Instance.currentLocation == locationName);
            if (travelButton != null)
            {
                travelButton.interactable = isUnlocked && !isCurrentLocation;
                
                if (isCurrentLocation)
                {
                    Text buttonText = travelButton.GetComponentInChildren<Text>();
                    if (buttonText != null) buttonText.text = "Current Location";
                }
                else if (!isUnlocked)
                {
                    Text buttonText = travelButton.GetComponentInChildren<Text>();
                    if (buttonText != null) buttonText.text = "Locked";
                }
                else
                {
                    Text buttonText = travelButton.GetComponentInChildren<Text>();
                    if (buttonText != null) buttonText.text = "Travel Here";
                }
            }

            // Show/hide locked overlay
            if (lockedOverlay != null)
            {
                lockedOverlay.SetActive(!isUnlocked);
            }
        }

        /// <summary>
        /// Highlight the selected location marker
        /// </summary>
        private void HighlightMarker(string locationName)
        {
            // Reset all markers to normal
            foreach (var kvp in locationMarkers)
            {
                Image img = kvp.Value.GetComponent<Image>();
                if (img != null)
                {
                    // Restore original color
                    Location loc = GameManager.Instance.locationManager.GetLocation(kvp.Key);
                    if (loc != null)
                    {
                        img.color = GetLocationMarkerColor(loc);
                    }
                }
            }

            // Highlight selected marker
            if (locationMarkers.ContainsKey(locationName))
            {
                Image img = locationMarkers[locationName].GetComponent<Image>();
                if (img != null)
                {
                    img.color = Color.yellow;
                }
            }
        }

        /// <summary>
        /// Handle travel button click
        /// </summary>
        private void OnTravelButtonClicked()
        {
            if (string.IsNullOrEmpty(selectedLocation)) return;
            if (GameManager.Instance == null) return;

            // Travel to selected location
            GameManager.Instance.TravelTo(selectedLocation);
            
            Debug.Log($"Traveled to {selectedLocation}");
            
            // Update map display
            UpdateCurrentLocationDisplay();
            RefreshMarkers();
            
            // Close map
            HidePanel();
        }

        /// <summary>
        /// Update the current location display
        /// </summary>
        private void UpdateCurrentLocationDisplay()
        {
            if (GameManager.Instance == null) return;

            if (currentLocationText != null)
            {
                currentLocationText.text = $"Current: {GameManager.Instance.currentLocation}";
            }
        }

        /// <summary>
        /// Refresh all location markers
        /// </summary>
        private void RefreshMarkers()
        {
            foreach (var kvp in locationMarkers)
            {
                Location location = GameManager.Instance.locationManager.GetLocation(kvp.Key);
                if (location != null)
                {
                    Image img = kvp.Value.GetComponent<Image>();
                    if (img != null)
                    {
                        img.color = GetLocationMarkerColor(location);
                    }
                }
            }
        }

        /// <summary>
        /// Clear all location markers
        /// </summary>
        private void ClearMarkers()
        {
            foreach (var marker in locationMarkers.Values)
            {
                if (marker != null)
                {
                    Destroy(marker);
                }
            }
            locationMarkers.Clear();
        }

        /// <summary>
        /// Populate court filter dropdown
        /// </summary>
        private void PopulateCourtFilter()
        {
            if (courtFilterDropdown == null) return;

            courtFilterDropdown.ClearOptions();
            var options = new List<string> { "All Courts" };
            
            foreach (Court court in System.Enum.GetValues(typeof(Court)))
            {
                if (court != Court.None)
                {
                    options.Add(court.ToString());
                }
            }
            
            courtFilterDropdown.AddOptions(options);
        }

        /// <summary>
        /// Handle court filter change
        /// </summary>
        private void OnCourtFilterChanged(int index)
        {
            if (index == 0)
            {
                // Show all locations
                foreach (var marker in locationMarkers.Values)
                {
                    marker.SetActive(true);
                }
            }
            else
            {
                // Show only selected court
                Court selectedCourt = (Court)(index);
                
                foreach (var kvp in locationMarkers)
                {
                    Location loc = GameManager.Instance.locationManager.GetLocation(kvp.Key);
                    if (loc != null)
                    {
                        kvp.Value.SetActive(loc.court == selectedCourt || loc.court == Court.None);
                    }
                }
            }
        }

        #region Zoom Controls

        private void ZoomIn()
        {
            currentZoom = Mathf.Min(currentZoom + 0.25f, MAX_ZOOM);
            ApplyZoom();
        }

        private void ZoomOut()
        {
            currentZoom = Mathf.Max(currentZoom - 0.25f, MIN_ZOOM);
            ApplyZoom();
        }

        private void ApplyZoom()
        {
            if (mapContainer != null)
            {
                mapContainer.transform.localScale = Vector3.one * currentZoom;
            }
        }

        #endregion

        #region Panel Management

        public void ShowPanel()
        {
            if (mapPanel != null)
            {
                mapPanel.SetActive(true);
                RefreshMarkers();
                UpdateCurrentLocationDisplay();
                
                // Hide location info by default
                if (locationInfoPanel != null)
                {
                    locationInfoPanel.SetActive(false);
                }
            }
        }

        public void HidePanel()
        {
            if (mapPanel != null)
            {
                mapPanel.SetActive(false);
                selectedLocation = null;
            }
        }

        public void TogglePanel()
        {
            if (mapPanel != null)
            {
                if (mapPanel.activeSelf)
                {
                    HidePanel();
                }
                else
                {
                    ShowPanel();
                }
            }
        }

        #endregion
    }
}
