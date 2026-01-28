using System;
using System.Collections.Generic;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Elemental affinities in the ACOTAR world
    /// Based on the magic types and court powers
    /// </summary>
    public enum Element
    {
        None,
        Fire,       // Fire manipulation
        Ice,        // Ice manipulation
        Water,      // Water manipulation
        Wind,       // Wind manipulation
        Light,      // Light manipulation, Day Court
        Darkness,   // Darkness manipulation, Night Court
        Nature,     // Spring Court powers
        Death       // Nesta's power from the Cauldron
    }

    /// <summary>
    /// Elemental resistance/weakness level
    /// </summary>
    public enum ElementalAffinity
    {
        VeryWeak = -2,      // Takes 200% damage
        Weak = -1,          // Takes 150% damage
        Neutral = 0,        // Takes 100% damage
        Resistant = 1,      // Takes 50% damage
        Immune = 2          // Takes 0% damage
    }

    /// <summary>
    /// Manages elemental interactions for combat
    /// </summary>
    public static class ElementalSystem
    {
        // Element interaction table
        private static Dictionary<Element, Dictionary<Element, ElementalAffinity>> elementTable;

        static ElementalSystem()
        {
            InitializeElementTable();
        }

        /// <summary>
        /// Initialize the elemental interaction table
        /// </summary>
        private static void InitializeElementTable()
        {
            elementTable = new Dictionary<Element, Dictionary<Element, ElementalAffinity>>();

            // Fire interactions
            elementTable[Element.Fire] = new Dictionary<Element, ElementalAffinity>
            {
                { Element.Ice, ElementalAffinity.Resistant },      // Fire melts ice
                { Element.Nature, ElementalAffinity.Resistant },   // Fire burns nature
                { Element.Water, ElementalAffinity.Weak },         // Water extinguishes fire
                { Element.Wind, ElementalAffinity.Weak },          // Wind can blow out fire
                { Element.Fire, ElementalAffinity.Resistant },     // Fire vs Fire = resistant
                { Element.Light, ElementalAffinity.Neutral },
                { Element.Darkness, ElementalAffinity.Neutral },
                { Element.Death, ElementalAffinity.Neutral }
            };

            // Ice interactions
            elementTable[Element.Ice] = new Dictionary<Element, ElementalAffinity>
            {
                { Element.Fire, ElementalAffinity.VeryWeak },      // Ice melted by fire
                { Element.Water, ElementalAffinity.Resistant },    // Ice and water are related
                { Element.Wind, ElementalAffinity.Neutral },
                { Element.Nature, ElementalAffinity.Resistant },   // Cold kills plants
                { Element.Ice, ElementalAffinity.Immune },         // Ice vs Ice = immune
                { Element.Light, ElementalAffinity.Neutral },
                { Element.Darkness, ElementalAffinity.Neutral },
                { Element.Death, ElementalAffinity.Weak }
            };

            // Water interactions
            elementTable[Element.Water] = new Dictionary<Element, ElementalAffinity>
            {
                { Element.Fire, ElementalAffinity.Resistant },     // Water beats fire
                { Element.Ice, ElementalAffinity.Weak },           // Water freezes
                { Element.Wind, ElementalAffinity.Neutral },
                { Element.Nature, ElementalAffinity.Weak },        // Plants absorb water
                { Element.Water, ElementalAffinity.Resistant },    // Water vs Water
                { Element.Light, ElementalAffinity.Neutral },
                { Element.Darkness, ElementalAffinity.Neutral },
                { Element.Death, ElementalAffinity.Neutral }
            };

            // Wind interactions
            elementTable[Element.Wind] = new Dictionary<Element, ElementalAffinity>
            {
                { Element.Fire, ElementalAffinity.Resistant },     // Wind feeds fire
                { Element.Ice, ElementalAffinity.Neutral },
                { Element.Water, ElementalAffinity.Neutral },
                { Element.Nature, ElementalAffinity.Neutral },
                { Element.Wind, ElementalAffinity.Neutral },
                { Element.Light, ElementalAffinity.Neutral },
                { Element.Darkness, ElementalAffinity.Weak },      // Wind disperses shadows
                { Element.Death, ElementalAffinity.Neutral }
            };

            // Light interactions (Day Court power)
            elementTable[Element.Light] = new Dictionary<Element, ElementalAffinity>
            {
                { Element.Fire, ElementalAffinity.Neutral },
                { Element.Ice, ElementalAffinity.Neutral },
                { Element.Water, ElementalAffinity.Neutral },
                { Element.Wind, ElementalAffinity.Neutral },
                { Element.Nature, ElementalAffinity.Resistant },   // Light helps growth
                { Element.Light, ElementalAffinity.Immune },       // Light vs Light
                { Element.Darkness, ElementalAffinity.VeryWeak },  // Light dispels darkness
                { Element.Death, ElementalAffinity.Weak }          // Life vs Death
            };

            // Darkness interactions (Night Court power)
            elementTable[Element.Darkness] = new Dictionary<Element, ElementalAffinity>
            {
                { Element.Fire, ElementalAffinity.Weak },          // Fire creates light
                { Element.Ice, ElementalAffinity.Neutral },
                { Element.Water, ElementalAffinity.Neutral },
                { Element.Wind, ElementalAffinity.Resistant },     // Shadows resist wind
                { Element.Nature, ElementalAffinity.Neutral },
                { Element.Light, ElementalAffinity.VeryWeak },     // Darkness fears light
                { Element.Darkness, ElementalAffinity.Immune },    // Darkness vs Darkness
                { Element.Death, ElementalAffinity.Resistant }     // Darkness embraces death
            };

            // Nature interactions (Spring Court power)
            elementTable[Element.Nature] = new Dictionary<Element, ElementalAffinity>
            {
                { Element.Fire, ElementalAffinity.VeryWeak },      // Plants burn
                { Element.Ice, ElementalAffinity.Weak },           // Cold kills plants
                { Element.Water, ElementalAffinity.Resistant },    // Water nourishes
                { Element.Wind, ElementalAffinity.Neutral },
                { Element.Light, ElementalAffinity.Resistant },    // Light helps growth
                { Element.Darkness, ElementalAffinity.Neutral },
                { Element.Nature, ElementalAffinity.Immune },      // Nature vs Nature
                { Element.Death, ElementalAffinity.VeryWeak }      // Death kills life
            };

            // Death interactions (Nesta's power)
            elementTable[Element.Death] = new Dictionary<Element, ElementalAffinity>
            {
                { Element.Fire, ElementalAffinity.Neutral },
                { Element.Ice, ElementalAffinity.Resistant },
                { Element.Water, ElementalAffinity.Neutral },
                { Element.Wind, ElementalAffinity.Neutral },
                { Element.Light, ElementalAffinity.Weak },         // Light opposes death
                { Element.Darkness, ElementalAffinity.Resistant }, // Darkness embraces death
                { Element.Nature, ElementalAffinity.Resistant },   // Death kills life
                { Element.Death, ElementalAffinity.Immune }        // Death vs Death
            };

            // None element - neutral to everything
            elementTable[Element.None] = new Dictionary<Element, ElementalAffinity>();
            foreach (Element e in Enum.GetValues(typeof(Element)))
            {
                elementTable[Element.None][e] = ElementalAffinity.Neutral;
            }
        }

        /// <summary>
        /// Get damage multiplier based on attacker and defender elements
        /// </summary>
        public static float GetDamageMultiplier(Element attackElement, Element defenderElement)
        {
            if (attackElement == Element.None || defenderElement == Element.None)
                return 1.0f;

            if (!elementTable.ContainsKey(defenderElement))
                return 1.0f;

            if (!elementTable[defenderElement].ContainsKey(attackElement))
                return 1.0f;

            ElementalAffinity affinity = elementTable[defenderElement][attackElement];

            switch (affinity)
            {
                case ElementalAffinity.VeryWeak:
                    return 2.0f;    // Takes 200% damage
                case ElementalAffinity.Weak:
                    return 1.5f;    // Takes 150% damage
                case ElementalAffinity.Neutral:
                    return 1.0f;    // Takes 100% damage
                case ElementalAffinity.Resistant:
                    return 0.5f;    // Takes 50% damage
                case ElementalAffinity.Immune:
                    return 0.0f;    // Takes 0% damage
                default:
                    return 1.0f;
            }
        }

        /// <summary>
        /// Get element from magic type
        /// </summary>
        public static Element GetElementFromMagic(MagicType magic)
        {
            switch (magic)
            {
                case MagicType.FireManipulation:
                    return Element.Fire;
                case MagicType.IceManipulation:
                    return Element.Ice;
                case MagicType.WaterManipulation:
                    return Element.Water;
                case MagicType.WindManipulation:
                    return Element.Wind;
                case MagicType.LightManipulation:
                    return Element.Light;
                case MagicType.DarknessManipulation:
                    return Element.Darkness;
                case MagicType.DeathManifestation:
                    return Element.Death;
                default:
                    return Element.None;
            }
        }

        /// <summary>
        /// Get element from court
        /// </summary>
        public static Element GetElementFromCourt(Court court)
        {
            switch (court)
            {
                case Court.Spring:
                    return Element.Nature;
                case Court.Summer:
                    return Element.Fire;
                case Court.Autumn:
                    return Element.Fire;  // Autumn court - fall colors, fire
                case Court.Winter:
                    return Element.Ice;
                case Court.Night:
                    return Element.Darkness;
                case Court.Dawn:
                    return Element.Light;
                case Court.Day:
                    return Element.Light;
                default:
                    return Element.None;
            }
        }

        /// <summary>
        /// Get effectiveness message for combat feedback
        /// </summary>
        public static string GetEffectivenessMessage(Element attackElement, Element defenderElement)
        {
            float multiplier = GetDamageMultiplier(attackElement, defenderElement);

            if (multiplier >= 2.0f)
                return "It's super effective!";
            else if (multiplier >= 1.5f)
                return "It's effective!";
            else if (multiplier <= 0.0f)
                return "It has no effect...";
            else if (multiplier <= 0.5f)
                return "It's not very effective...";
            else
                return "";
        }

        /// <summary>
        /// Get all weaknesses for an element
        /// </summary>
        public static List<Element> GetWeaknesses(Element element)
        {
            List<Element> weaknesses = new List<Element>();

            if (!elementTable.ContainsKey(element))
                return weaknesses;

            foreach (var kvp in elementTable[element])
            {
                if (kvp.Value == ElementalAffinity.Weak || kvp.Value == ElementalAffinity.VeryWeak)
                {
                    weaknesses.Add(kvp.Key);
                }
            }

            return weaknesses;
        }

        /// <summary>
        /// Get all resistances for an element
        /// </summary>
        public static List<Element> GetResistances(Element element)
        {
            List<Element> resistances = new List<Element>();

            if (!elementTable.ContainsKey(element))
                return resistances;

            foreach (var kvp in elementTable[element])
            {
                if (kvp.Value == ElementalAffinity.Resistant || kvp.Value == ElementalAffinity.Immune)
                {
                    resistances.Add(kvp.Key);
                }
            }

            return resistances;
        }

        /// <summary>
        /// Display elemental information for an element
        /// </summary>
        public static void DisplayElementInfo(Element element)
        {
            Debug.Log($"\n=== {element} Element Info ===");
            
            var weaknesses = GetWeaknesses(element);
            Debug.Log("Weak against: " + (weaknesses.Count > 0 ? string.Join(", ", weaknesses) : "None"));
            
            var resistances = GetResistances(element);
            Debug.Log("Resistant to: " + (resistances.Count > 0 ? string.Join(", ", resistances) : "None"));
            
            Debug.Log("===========================\n");
        }
    }
}
