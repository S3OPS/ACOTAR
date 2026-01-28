using System.Collections.Generic;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Companion roles in party
    /// </summary>
    public enum CompanionRole
    {
        Tank,       // High defense, draws aggro
        DPS,        // Damage dealer
        Support,    // Healer/buffer
        Balanced    // All-rounder
    }

    /// <summary>
    /// Companion character that can join the player's party
    /// Represents major characters from ACOTAR who can fight alongside player
    /// </summary>
    [System.Serializable]
    public class Companion : Character
    {
        public CompanionRole role;
        public int loyalty;  // 0-100, affects performance and availability
        public string backstory;
        public List<string> dialoguePool;
        public bool isRecruited;

        /// <summary>
        /// Create a new companion
        /// </summary>
        public Companion(string name, CharacterClass charClass, Court court, CompanionRole role)
            : base(name, charClass, court)
        {
            this.role = role;
            this.loyalty = 50; // Neutral starting loyalty
            this.backstory = "";
            this.dialoguePool = new List<string>();
            this.isRecruited = false;

            // Companions start at level 2
            level = 2;
            GainExperience(0); // Update stats for level
        }

        /// <summary>
        /// Increase companion loyalty
        /// </summary>
        public void IncreaseLoyalty(int amount)
        {
            loyalty += amount;
            if (loyalty > 100) loyalty = 100;

            if (loyalty >= 80)
            {
                Debug.Log($"{name}'s loyalty is now Very High!");
            }
        }

        /// <summary>
        /// Decrease companion loyalty
        /// </summary>
        public void DecreaseLoyalty(int amount)
        {
            loyalty -= amount;
            if (loyalty < 0) loyalty = 0;

            if (loyalty <= 20)
            {
                Debug.LogWarning($"{name}'s loyalty is dangerously low!");
            }
        }

        /// <summary>
        /// Get combat effectiveness based on loyalty
        /// </summary>
        public float GetLoyaltyBonus()
        {
            if (loyalty >= 80) return 1.2f;      // 20% bonus
            if (loyalty >= 60) return 1.1f;      // 10% bonus
            if (loyalty >= 40) return 1.0f;      // No bonus
            if (loyalty >= 20) return 0.9f;      // 10% penalty
            return 0.8f;                          // 20% penalty
        }
    }

    /// <summary>
    /// Factory for creating iconic ACOTAR companions
    /// </summary>
    public static class CompanionFactory
    {
        /// <summary>
        /// Create Rhysand - High Lord of the Night Court
        /// </summary>
        public static Companion CreateRhysand()
        {
            Companion rhys = new Companion("Rhysand", CharacterClass.HighFae, Court.Night, CompanionRole.Balanced);
            rhys.level = 10; // Powerful High Lord
            rhys.backstory = "High Lord of the Night Court, the most powerful court in Prythian.";
            rhys.LearnAbility(MagicType.DarknessManipulation);
            rhys.LearnAbility(MagicType.Daemati);
            rhys.LearnAbility(MagicType.Winnowing);
            rhys.LearnAbility(MagicType.ShieldCreation);
            rhys.dialoguePool.Add("To the stars who listen.");
            rhys.dialoguePool.Add("I'm rather good at what I do.");
            return rhys;
        }

        /// <summary>
        /// Create Cassian - Illyrian General
        /// </summary>
        public static Companion CreateCassian()
        {
            Companion cassian = new Companion("Cassian", CharacterClass.Illyrian, Court.Night, CompanionRole.Tank);
            cassian.level = 8;
            cassian.backstory = "General of the Illyrian armies and member of Rhysand's Inner Circle.";
            cassian.LearnAbility(MagicType.ShieldCreation);
            cassian.dialoguePool.Add("I don't need luck. I have skill.");
            cassian.dialoguePool.Add("Let's get this fight started!");
            return cassian;
        }

        /// <summary>
        /// Create Azriel - Spymaster and Shadowsinger
        /// </summary>
        public static Companion CreateAzriel()
        {
            Companion azriel = new Companion("Azriel", CharacterClass.Illyrian, Court.Night, CompanionRole.DPS);
            azriel.level = 8;
            azriel.backstory = "Spymaster of the Night Court and master of shadows.";
            azriel.LearnAbility(MagicType.DarknessManipulation);
            azriel.LearnAbility(MagicType.Winnowing);
            azriel.dialoguePool.Add("...");
            azriel.dialoguePool.Add("The shadows tell me everything.");
            return azriel;
        }

        /// <summary>
        /// Create Mor - Third in Command of Night Court
        /// </summary>
        public static Companion CreateMor()
        {
            Companion mor = new Companion("Morrigan", CharacterClass.HighFae, Court.Night, CompanionRole.DPS);
            mor.level = 7;
            mor.backstory = "Morrigan, cousin of Rhysand and wielder of Truth.";
            mor.LearnAbility(MagicType.LightManipulation);
            mor.LearnAbility(MagicType.Winnowing);
            mor.dialoguePool.Add("I am not afraid.");
            mor.dialoguePool.Add("Let's show them what we're made of.");
            return mor;
        }

        /// <summary>
        /// Create Amren - Ancient creature in High Fae form
        /// </summary>
        public static Companion CreateAmren()
        {
            Companion amren = new Companion("Amren", CharacterClass.HighFae, Court.Night, CompanionRole.Support);
            amren.level = 12; // Ancient and powerful
            amren.backstory = "An ancient being of immense power, trapped in High Fae form.";
            amren.LearnAbility(MagicType.DarknessManipulation);
            amren.LearnAbility(MagicType.LightManipulation);
            amren.LearnAbility(MagicType.ShieldCreation);
            amren.dialoguePool.Add("I have been here since the beginning.");
            amren.dialoguePool.Add("Do not test my patience.");
            return amren;
        }

        /// <summary>
        /// Create Lucien - Emissary of Spring Court
        /// </summary>
        public static Companion CreateLucien()
        {
            Companion lucien = new Companion("Lucien", CharacterClass.HighFae, Court.Spring, CompanionRole.Balanced);
            lucien.level = 6;
            lucien.backstory = "Emissary of the Spring Court and son of the Autumn Court High Lord.";
            lucien.LearnAbility(MagicType.FireManipulation);
            lucien.LearnAbility(MagicType.ShieldCreation);
            lucien.dialoguePool.Add("I've been through worse.");
            lucien.dialoguePool.Add("Let me help you with that.");
            return lucien;
        }

        /// <summary>
        /// Create Tamlin - High Lord of Spring Court
        /// </summary>
        public static Companion CreateTamlin()
        {
            Companion tamlin = new Companion("Tamlin", CharacterClass.HighFae, Court.Spring, CompanionRole.Tank);
            tamlin.level = 9;
            tamlin.backstory = "High Lord of the Spring Court, capable of shapeshifting.";
            tamlin.LearnAbility(MagicType.Shapeshifting);
            tamlin.LearnAbility(MagicType.ShieldCreation);
            tamlin.dialoguePool.Add("I will protect what is mine.");
            tamlin.dialoguePool.Add("The Spring Court endures.");
            return tamlin;
        }

        /// <summary>
        /// Create Nesta - Feyre's sister (High Fae after Made)
        /// </summary>
        public static Companion CreateNesta()
        {
            Companion nesta = new Companion("Nesta Archeron", CharacterClass.HighFae, Court.Night, CompanionRole.DPS);
            nesta.level = 5;
            nesta.isMadeByTheCauldron = true;
            nesta.backstory = "Feyre's eldest sister, Made by the Cauldron with devastating power.";
            nesta.LearnAbility(MagicType.ShieldCreation);
            nesta.LearnAbility(MagicType.DarknessManipulation);
            nesta.dialoguePool.Add("I am not afraid of you.");
            nesta.dialoguePool.Add("Stand back.");
            return nesta;
        }

        /// <summary>
        /// Create Elain - Feyre's sister (High Fae after Made)
        /// </summary>
        public static Companion CreateElain()
        {
            Companion elain = new Companion("Elain Archeron", CharacterClass.HighFae, Court.Night, CompanionRole.Support);
            elain.level = 4;
            elain.isMadeByTheCauldron = true;
            elain.backstory = "Feyre's gentle sister, Made by the Cauldron with Seer abilities.";
            elain.LearnAbility(MagicType.Seer);
            elain.LearnAbility(MagicType.Healing);
            elain.dialoguePool.Add("I see... something.");
            elain.dialoguePool.Add("Let me help you.");
            return elain;
        }
    }

    /// <summary>
    /// Manages the player's party of companions
    /// Handles recruitment, party composition, and companion interactions
    /// </summary>
    public class CompanionManager : MonoBehaviour
    {
        private List<Companion> availableCompanions;
        private List<Companion> activeParty;
        private const int MAX_PARTY_SIZE = 3;

        void Awake()
        {
            availableCompanions = new List<Companion>();
            activeParty = new List<Companion>();
            InitializeCompanions();
        }

        /// <summary>
        /// Initialize all available companions
        /// </summary>
        private void InitializeCompanions()
        {
            // Inner Circle members
            availableCompanions.Add(CompanionFactory.CreateRhysand());
            availableCompanions.Add(CompanionFactory.CreateCassian());
            availableCompanions.Add(CompanionFactory.CreateAzriel());
            availableCompanions.Add(CompanionFactory.CreateMor());
            availableCompanions.Add(CompanionFactory.CreateAmren());

            // Other companions
            availableCompanions.Add(CompanionFactory.CreateLucien());
            availableCompanions.Add(CompanionFactory.CreateTamlin());
            availableCompanions.Add(CompanionFactory.CreateNesta());
            availableCompanions.Add(CompanionFactory.CreateElain());
        }

        /// <summary>
        /// Recruit a companion by name
        /// </summary>
        public bool RecruitCompanion(string companionName)
        {
            Companion companion = availableCompanions.Find(c => c.name == companionName);
            if (companion != null && !companion.isRecruited)
            {
                companion.isRecruited = true;
                Debug.Log($"{companionName} has joined your cause!");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Add companion to active party
        /// </summary>
        public bool AddToParty(string companionName)
        {
            if (activeParty.Count >= MAX_PARTY_SIZE)
            {
                Debug.LogWarning("Party is full! Remove a companion first.");
                return false;
            }

            Companion companion = availableCompanions.Find(c => c.name == companionName && c.isRecruited);
            if (companion != null && !activeParty.Contains(companion))
            {
                activeParty.Add(companion);
                Debug.Log($"{companionName} has joined your party!");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove companion from active party
        /// </summary>
        public bool RemoveFromParty(string companionName)
        {
            Companion companion = activeParty.Find(c => c.name == companionName);
            if (companion != null)
            {
                activeParty.Remove(companion);
                Debug.Log($"{companionName} has left your party.");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get all recruited companions
        /// </summary>
        public List<Companion> GetRecruitedCompanions()
        {
            return availableCompanions.FindAll(c => c.isRecruited);
        }

        /// <summary>
        /// Get active party members
        /// </summary>
        public List<Companion> GetActiveParty()
        {
            return new List<Companion>(activeParty);
        }

        /// <summary>
        /// Get companion by name
        /// </summary>
        public Companion GetCompanion(string companionName)
        {
            return availableCompanions.Find(c => c.name == companionName);
        }
    }
}
