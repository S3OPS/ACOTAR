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
    /// </summary>
    public class LocationManager : MonoBehaviour
    {
        private Dictionary<string, Location> locations;

        void Awake()
        {
            InitializeLocations();
        }

        private void InitializeLocations()
        {
            locations = new Dictionary<string, Location>();

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

            // Special locations
            AddLocation(new Location(
                "Under the Mountain",
                "The cursed realm where Amarantha held court for 49 years",
                Court.Spring,
                LocationType.UnderTheMountain
            ));

            AddLocation(new Location(
                "Human Lands",
                "The mortal realm south of the Wall, where humans live without magic",
                Court.Spring,
                LocationType.HumanLands
            ));

            AddLocation(new Location(
                "The Wall",
                "The magical barrier separating the mortal lands from Prythian",
                Court.Spring,
                LocationType.Forest
            ));
        }

        private void AddLocation(Location location)
        {
            locations[location.name] = location;
        }

        public Location GetLocation(string name)
        {
            if (locations.ContainsKey(name))
            {
                return locations[name];
            }
            return null;
        }

        public List<Location> GetLocationsByCourt(Court court)
        {
            List<Location> courtLocations = new List<Location>();
            foreach (var location in locations.Values)
            {
                if (location.rulingCourt == court)
                {
                    courtLocations.Add(location);
                }
            }
            return courtLocations;
        }

        public List<string> GetAllLocationNames()
        {
            return new List<string>(locations.Keys);
        }
    }
}
