using System.Collections.Generic;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Locations in Prythian from ACOTAR lore
    /// </summary>
    [System.Serializable]
    public class Location
    {
        public string name;
        public string description;
        public Court rulingCourt;
        public LocationType type;
        public bool isProtected;

        public Location(string name, string desc, Court court, LocationType type)
        {
            this.name = name;
            this.description = desc;
            this.rulingCourt = court;
            this.type = type;
            this.isProtected = false;
        }
    }

    public enum LocationType
    {
        Court,
        City,
        Village,
        Manor,
        MountainRange,
        Forest,
        UnderTheMountain,
        HumanLands
    }

    /// <summary>
    /// Manages all locations in Prythian
    /// Optimized with caching for court-based lookups
    /// 
    /// v2.5.3: Enhanced with property accessors and defensive programming
    /// </summary>
    public class LocationManager : MonoBehaviour
    {
        private Dictionary<string, Location> locations;
        private Dictionary<Court, List<Location>> courtLocationCache;
        private List<string> locationNamesCache;

        // Public property accessors for cleaner code (v2.5.3)
        /// <summary>
        /// Get the total number of locations defined in the system
        /// </summary>
        public int LocationCount => locations?.Count ?? 0;

        /// <summary>
        /// Check if the location system is properly initialized
        /// </summary>
        public bool IsInitialized => locations != null && locations.Count > 0;

        void Awake()
        {
            InitializeLocations();
        }

        private void InitializeLocations()
        {
            locations = new Dictionary<string, Location>();
            courtLocationCache = new Dictionary<Court, List<Location>>();

            // Spring Court locations
            AddLocation(new Location(
                "Spring Court Manor",
                "The elegant manor of the High Lord Tamlin, surrounded by eternal gardens that bloom year-round with roses and vibrant flowers",
                Court.Spring,
                LocationType.Manor
            ));

            // Night Court locations
            AddLocation(new Location(
                "Velaris",
                "The City of Starlight, hidden jewel of the Night Court, untouched by war for millennia",
                Court.Night,
                LocationType.City
            ));

            AddLocation(new Location(
                "Hewn City",
                "The Court of Nightmares, the dark underbelly of the Night Court",
                Court.Night,
                LocationType.City
            ));

            AddLocation(new Location(
                "Illyrian Mountains",
                "The war camps and training grounds of the Illyrian warriors",
                Court.Night,
                LocationType.MountainRange
            ));

            AddLocation(new Location(
                "House of Wind",
                "The library fortress perched atop a mountain in Velaris",
                Court.Night,
                LocationType.Manor
            ));

            // Summer Court
            AddLocation(new Location(
                "Summer Court",
                "A land of eternal warmth, beaches, and ocean power",
                Court.Summer,
                LocationType.Court
            ));

            AddLocation(new Location(
                "Adriata",
                "The capital city of the Summer Court, built along pristine coasts",
                Court.Summer,
                LocationType.City
            ));

            // Autumn Court
            AddLocation(new Location(
                "Autumn Court",
                "A land of eternal fall, with forests of gold, red, and orange",
                Court.Autumn,
                LocationType.Court
            ));

            // Winter Court
            AddLocation(new Location(
                "Winter Court",
                "A frozen realm of ice and snow, home to powerful ice magic",
                Court.Winter,
                LocationType.Court
            ));

            // Day Court
            AddLocation(new Location(
                "Day Court",
                "A land bathed in eternal sunlight, known for its scholars and libraries",
                Court.Day,
                LocationType.Court
            ));

            // Dawn Court
            AddLocation(new Location(
                "Dawn Court",
                "A land of perpetual sunrise, known for its healing magic",
                Court.Dawn,
                LocationType.Court
            ));

            // Special locations - not owned by any single court
            AddLocation(new Location(
                "Under the Mountain",
                "The cursed realm where Amarantha held court for 49 years. Located beneath the central mountain of Prythian.",
                Court.None,
                LocationType.UnderTheMountain
            ));

            AddLocation(new Location(
                "Human Lands",
                "The mortal realm south of the Wall, where humans live without magic. Home to the Archeron family.",
                Court.None,
                LocationType.HumanLands
            ));

            AddLocation(new Location(
                "The Wall",
                "The magical barrier separating the mortal lands from Prythian. Created by the Treaty after the War.",
                Court.None,
                LocationType.Forest
            ));

            // Additional lore-accurate locations
            AddLocation(new Location(
                "The Prison",
                "Ancient prison carved into a mountain, holding the most dangerous beings in existence.",
                Court.Night,
                LocationType.MountainRange
            ));

            AddLocation(new Location(
                "The Middle",
                "Treacherous lands in the heart of Prythian where the Weaver dwells.",
                Court.None,
                LocationType.Forest
            ));

            AddLocation(new Location(
                "Hybern",
                "Island kingdom across the sea, ruled by the King of Hybern. Enemy of Prythian.",
                Court.None,
                LocationType.Court
            ));
        }

        private void AddLocation(Location location)
        {
            // Defensive check (v2.5.3)
            if (location == null)
            {
                Debug.LogWarning("LocationManager: Attempted to add null location");
                return;
            }

            if (string.IsNullOrEmpty(location.name))
            {
                Debug.LogWarning("LocationManager: Attempted to add location with null or empty name");
                return;
            }

            // Check for duplicate to prevent overwriting existing locations
            if (locations.ContainsKey(location.name))
            {
                Debug.LogWarning($"LocationManager: Location '{location.name}' already exists. Skipping duplicate.");
                return;
            }

            locations[location.name] = location;
            
            // Invalidate cache when adding new location
            courtLocationCache.Clear();
            locationNamesCache = null;
        }

        /// <summary>
        /// Get a location by name
        /// v2.5.3: Enhanced with defensive checks and informative warnings
        /// </summary>
        /// <param name="name">The name of the location to retrieve</param>
        /// <returns>The location if found, null otherwise</returns>
        public Location GetLocation(string name)
        {
            // Defensive checks (v2.5.3)
            if (!IsInitialized)
            {
                Debug.LogWarning("LocationManager: Cannot get location - system not initialized");
                return null;
            }

            if (string.IsNullOrEmpty(name))
            {
                Debug.LogWarning("LocationManager: Cannot get location with null or empty name");
                return null;
            }

            if (locations.ContainsKey(name))
            {
                return locations[name];
            }
            
            // Location not found - this is normal, not a warning
            return null;
        }

        /// <summary>
        /// Get locations by court with caching for optimization
        /// v2.5.3: Enhanced with defensive checks
        /// </summary>
        public List<Location> GetLocationsByCourt(Court court)
        {
            // Defensive checks (v2.5.3)
            if (!IsInitialized)
            {
                Debug.LogWarning("LocationManager: Cannot get locations by court - system not initialized");
                return new List<Location>();
            }

            // Check cache first
            if (courtLocationCache.ContainsKey(court))
            {
                return new List<Location>(courtLocationCache[court]);
            }

            // Build cache entry
            List<Location> courtLocations = new List<Location>();
            foreach (var location in locations.Values)
            {
                if (location.rulingCourt == court)
                {
                    courtLocations.Add(location);
                }
            }
            
            courtLocationCache[court] = courtLocations;
            return new List<Location>(courtLocations);
        }

        /// <summary>
        /// Get all location names with caching for optimization
        /// v2.5.3: Enhanced with defensive checks
        /// </summary>
        public List<string> GetAllLocationNames()
        {
            // Defensive check (v2.5.3)
            if (!IsInitialized)
            {
                Debug.LogWarning("LocationManager: Cannot get location names - system not initialized");
                return new List<string>();
            }

            if (locationNamesCache == null)
            {
                locationNamesCache = new List<string>(locations.Keys);
            }
            return new List<string>(locationNamesCache);
        }

        /// <summary>
        /// Check if a specific location exists in the system
        /// v2.5.3: New helper method
        /// </summary>
        /// <param name="locationName">The name of the location to check</param>
        /// <returns>True if the location exists, false otherwise</returns>
        public bool LocationExists(string locationName)
        {
            if (!IsInitialized || string.IsNullOrEmpty(locationName))
            {
                return false;
            }
            return locations.ContainsKey(locationName);
        }

        /// <summary>
        /// Get all locations in the system
        /// v2.5.3: New helper method for complete location access
        /// </summary>
        /// <returns>List of all locations</returns>
        public List<Location> GetAllLocations()
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("LocationManager: Cannot get all locations - system not initialized");
                return new List<Location>();
            }
            return new List<Location>(locations.Values);
        }
    }
}
