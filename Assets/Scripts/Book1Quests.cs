using System.Collections.Generic;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Extended quest content for Book 1 storyline
    /// Adds detailed quests for the complete Under the Mountain arc
    /// </summary>
    public static class Book1Quests
    {
        /// <summary>
        /// Initialize all Book 1 quests into the quest manager
        /// </summary>
        public static void InitializeBook1Quests(Dictionary<string, Quest> quests)
        {
            // Main storyline already has main_001 through main_005
            // Let's add the detailed Under the Mountain quests

            // First Trial: The Worm
            Quest trial1 = new Quest(
                "main_006",
                "First Trial: The Worm",
                "Amarantha's first trial: Face the Middengard Wyrm in the flooded chamber.",
                QuestType.MainStory
            );
            trial1.objectives.Add("Survive the flooded chamber");
            trial1.objectives.Add("Defeat the Middengard Wyrm");
            trial1.objectives.Add("Prove your worth to Amarantha");
            trial1.experienceReward = 400;
            trial1.nextQuestId = "main_007";
            // Optional challenge objectives (v2.3.1)
            trial1.optionalObjectives.Add("Complete the trial without taking damage");
            trial1.optionalObjectives.Add("Defeat the Wyrm in under 10 turns");
            trial1.bonusExperienceReward = 100;
            trial1.bonusGoldReward = 50;
            quests[trial1.questId] = trial1;

            // Between Trials: Healing and Rhysand
            Quest between1 = new Quest(
                "main_007",
                "Nights Under the Mountain",
                "Recover from the first trial. Strange things happen at night...",
                QuestType.MainStory
            );
            between1.objectives.Add("Heal your wounds");
            between1.objectives.Add("Meet with Rhysand in the darkness");
            between1.objectives.Add("Learn about the true nature of the High Lord of Night");
            between1.experienceReward = 300;
            between1.nextQuestId = "main_008";
            quests[between1.questId] = between1;

            // Second Trial: The Naga
            Quest trial2 = new Quest(
                "main_008",
                "Second Trial: The Naga",
                "Amarantha's second trial: Read and answer three riddles, or face the Naga.",
                QuestType.MainStory
            );
            trial2.objectives.Add("Enter the chamber of riddles");
            trial2.objectives.Add("Face your illiteracy");
            trial2.objectives.Add("Survive the Naga's poison");
            trial2.experienceReward = 450;
            trial2.nextQuestId = "main_009";
            // Optional challenge objectives (v2.3.1)
            trial2.optionalObjectives.Add("Defeat the Naga without using healing items");
            trial2.optionalObjectives.Add("Take less than 50 damage during the fight");
            trial2.bonusExperienceReward = 125;
            trial2.bonusGoldReward = 75;
            quests[trial2.questId] = trial2;

            // Between Trials: Clare Beddor
            Quest between2 = new Quest(
                "main_009",
                "The Cost of Defiance",
                "Witness Amarantha's cruelty. Some prices are too high to pay.",
                QuestType.MainStory
            );
            between2.objectives.Add("Witness Clare Beddor's fate");
            between2.objectives.Add("Feel the weight of helplessness");
            between2.objectives.Add("Steel yourself for what's to come");
            between2.experienceReward = 200;
            between2.nextQuestId = "main_010";
            quests[between2.questId] = between2;

            // Third Trial: The Three Fae
            Quest trial3 = new Quest(
                "main_010",
                "Third Trial: Hearts of Stone",
                "Amarantha's third trial: Kill three innocent Fae to save Tamlin and Prythian.",
                QuestType.MainStory
            );
            trial3.objectives.Add("Face the three masked Fae");
            trial3.objectives.Add("Make an impossible choice");
            trial3.objectives.Add("Discover the truth behind the masks");
            trial3.experienceReward = 500;
            trial3.nextQuestId = "main_011";
            // Optional challenge objectives (v2.3.1)
            trial3.optionalObjectives.Add("Solve the riddle before making your choice");
            trial3.optionalObjectives.Add("Show mercy and compassion despite the circumstances");
            trial3.bonusExperienceReward = 150;
            trial3.bonusGoldReward = 100;
            quests[trial3.questId] = trial3;

            // The Riddle
            Quest riddle = new Quest(
                "main_011",
                "The Final Riddle",
                "One last chance: Solve Amarantha's riddle or die trying.",
                QuestType.MainStory
            );
            riddle.objectives.Add("Face Amarantha in her throne room");
            riddle.objectives.Add("Answer the riddle: 'Love'");
            riddle.objectives.Add("Pay the ultimate price");
            riddle.experienceReward = 600;
            riddle.nextQuestId = "main_012";
            // Optional challenge objectives (v2.3.1)
            riddle.optionalObjectives.Add("Answer the riddle correctly on first attempt");
            riddle.optionalObjectives.Add("Maintain dignity and courage before Amarantha");
            riddle.bonusExperienceReward = 200;
            riddle.bonusGoldReward = 150;
            quests[riddle.questId] = riddle;
            riddle.nextQuestId = "main_012";
            quests[riddle.questId] = riddle;

            // Breaking the Curse
            Quest curse = new Quest(
                "main_012",
                "Breaking the Curse",
                "Even in death, love proves stronger than Amarantha's curse.",
                QuestType.MainStory
            );
            curse.objectives.Add("Die for love");
            curse.objectives.Add("Be Made by the seven High Lords");
            curse.objectives.Add("Become High Fae");
            curse.objectives.Add("Kill Amarantha");
            curse.objectives.Add("Free Prythian from the curse");
            curse.experienceReward = 1500;
            curse.nextQuestId = "main_013";
            quests[curse.questId] = curse;

            // Return to Spring Court
            Quest aftermath1 = new Quest(
                "main_013",
                "Return to Spring",
                "Return to the Spring Court, forever changed.",
                QuestType.MainStory
            );
            aftermath1.objectives.Add("Leave Under the Mountain");
            aftermath1.objectives.Add("Return to Tamlin's manor");
            aftermath1.objectives.Add("Adjust to your new High Fae form");
            aftermath1.experienceReward = 300;
            aftermath1.nextQuestId = "main_014";
            quests[aftermath1.questId] = aftermath1;

            // Nightmares and Walls - BASE GAME ENDING
            Quest aftermath2 = new Quest(
                "main_014",
                "Nightmares and Walls",
                "The curse is broken, but its scars remain. Something is wrong.",
                QuestType.MainStory
            );
            aftermath2.objectives.Add("Try to adjust to life at the manor");
            aftermath2.objectives.Add("Deal with nightmares and trauma");
            aftermath2.objectives.Add("Notice the growing distance");
            aftermath2.experienceReward = 250;
            // Note: nextQuestId leads to DLC content - handled by DLCManager
            aftermath2.nextQuestId = "book2_001";  // DLC content - will be checked at runtime
            quests[aftermath2.questId] = aftermath2;

            // =====================================================
            // DLC CONTENT BRIDGE - Included but requires DLC
            // =====================================================
            
            // Bridge to Book 2 (DLC 1: A Court of Mist and Fury)
            Quest bridge = new Quest(
                "book2_001",
                "A Bargain Kept",
                "Rhysand comes to collect on your bargain. Your life is about to change again.",
                QuestType.MainStory
            );
            bridge.objectives.Add("Honor the bargain with Rhysand");
            bridge.objectives.Add("Leave the Spring Court");
            bridge.objectives.Add("Journey to the Night Court");
            bridge.experienceReward = 400;
            bridge.isDLCContent = true;  // Mark as DLC content
            quests[bridge.questId] = bridge;

            // Add more side quests for Book 1
            AddBook1SideQuests(quests);
        }

        /// <summary>
        /// Add side quests for Book 1
        /// </summary>
        private static void AddBook1SideQuests(Dictionary<string, Quest> quests)
        {
            // Learning to Read
            Quest reading = new Quest(
                "side_004",
                "Letters and Words",
                "Learn to read with help from unexpected friends.",
                QuestType.SideQuest
            );
            reading.objectives.Add("Accept help learning to read");
            reading.objectives.Add("Practice with children's books");
            reading.objectives.Add("Unlock the world of written words");
            reading.experienceReward = 200;
            quests[reading.questId] = reading;

            // Painting in Spring
            Quest painting = new Quest(
                "side_005",
                "Canvas and Color",
                "Rediscover your love of painting in the Spring Court.",
                QuestType.SideQuest
            );
            painting.objectives.Add("Find art supplies in the manor");
            painting.objectives.Add("Paint the beauty of Spring");
            painting.objectives.Add("Share your art with Tamlin");
            painting.experienceReward = 150;
            quests[painting.questId] = painting;

            // Alis's Kindness
            Quest alis = new Quest(
                "side_006",
                "A Servant's Wisdom",
                "Alis, the servant, proves that kindness exists even in dark times.",
                QuestType.SideQuest
            );
            alis.objectives.Add("Get to know Alis");
            alis.objectives.Add("Learn about life in the Spring Court");
            alis.objectives.Add("Find a friend in unexpected places");
            alis.experienceReward = 100;
            quests[alis.questId] = alis;

            // Starfall Memory
            Quest starfall = new Quest(
                "side_007",
                "Memory of Starlight",
                "Under the Mountain, Rhysand shares a precious memory with you.",
                QuestType.SideQuest
            );
            starfall.objectives.Add("Experience Rhysand's memory");
            starfall.objectives.Add("Witness Starfall through his eyes");
            starfall.objectives.Add("See the City of Starlight: Velaris");
            starfall.experienceReward = 250;
            quests[starfall.questId] = starfall;

            // The Bone Carver
            Quest carver = new Quest(
                "side_008",
                "The Bone Carver's Gift",
                "A mysterious prisoner offers cryptic aid.",
                QuestType.SideQuest
            );
            carver.objectives.Add("Visit the Bone Carver's cell");
            carver.objectives.Add("Answer his riddles");
            carver.objectives.Add("Receive his mysterious gift");
            carver.experienceReward = 300;
            quests[carver.questId] = carver;

            // Court of Nightmares Visit
            Quest hewn = new Quest(
                "side_009",
                "The Court of Nightmares",
                "Visit the dark side of the Night Court: Hewn City.",
                QuestType.CourtQuest
            );
            hewn.objectives.Add("Travel to Hewn City");
            hewn.objectives.Add("Witness Rhysand as the Lord of Nightmares");
            hewn.objectives.Add("Understand the duality of the Night Court");
            hewn.experienceReward = 350;
            quests[hewn.questId] = hewn;

            // NEW ENHANCED SIDE QUESTS FOR BASE GAME
            
            // The Spring Gardens
            Quest gardens = new Quest(
                "side_010_book1",
                "The Gardens of Spring",
                "Explore the beautiful gardens of the Spring Court manor and discover hidden secrets.",
                QuestType.SideQuest
            );
            gardens.objectives.Add("Explore all sections of the manor gardens");
            gardens.objectives.Add("Find the ancient rose bush");
            gardens.objectives.Add("Learn the history of the Spring Court from the garden keeper");
            gardens.experienceReward = 175;
            quests[gardens.questId] = gardens;

            // Lucien's Trust
            Quest lucienQuest = new Quest(
                "side_011_book1",
                "Fire and Friendship",
                "Earn Lucien's trust and learn about his mysterious past.",
                QuestType.CompanionQuest
            );
            lucienQuest.objectives.Add("Spend time with Lucien");
            lucienQuest.objectives.Add("Help him with court duties");
            lucienQuest.objectives.Add("Learn about his history with the Autumn Court");
            lucienQuest.experienceReward = 225;
            quests[lucienQuest.questId] = lucienQuest;

            // The Lesser Fae
            Quest lesserFae = new Quest(
                "side_012_book1",
                "Voices of the Forgotten",
                "The Lesser Fae of the Spring Court have stories to tell.",
                QuestType.SideQuest
            );
            lesserFae.objectives.Add("Meet with the Lesser Fae servants");
            lesserFae.objectives.Add("Learn about their lives under the curse");
            lesserFae.objectives.Add("Help them with their daily struggles");
            lesserFae.experienceReward = 150;
            quests[lesserFae.questId] = lesserFae;

            // The Wall's Secrets
            Quest wallSecrets = new Quest(
                "side_013_book1",
                "The Wall Between Worlds",
                "Investigate the magical barrier that separates the mortal and Fae realms.",
                QuestType.ExplorationQuest
            );
            wallSecrets.objectives.Add("Study the Wall from both sides");
            wallSecrets.objectives.Add("Learn about its creation");
            wallSecrets.objectives.Add("Discover hidden passages");
            wallSecrets.experienceReward = 200;
            quests[wallSecrets.questId] = wallSecrets;

            // The Hunter's Legacy
            Quest hunterLegacy = new Quest(
                "side_014_book1",
                "Hunter's Heritage",
                "Reflect on your mortal life and the skills that kept you alive.",
                QuestType.SideQuest
            );
            hunterLegacy.objectives.Add("Practice your archery skills");
            hunterLegacy.objectives.Add("Track wild game in the Spring lands");
            hunterLegacy.objectives.Add("Prove your hunting prowess to the Fae");
            hunterLegacy.experienceReward = 175;
            quests[hunterLegacy.questId] = hunterLegacy;

            // Under the Mountain - Survival
            Quest utmSurvival = new Quest(
                "side_015_book1",
                "Survival Under Stone",
                "Learn to navigate the treacherous politics and dangers Under the Mountain.",
                QuestType.SideQuest
            );
            utmSurvival.objectives.Add("Identify allies among the prisoners");
            utmSurvival.objectives.Add("Avoid Amarantha's attention when possible");
            utmSurvival.objectives.Add("Find sources of food and rest");
            utmSurvival.experienceReward = 250;
            quests[utmSurvival.questId] = utmSurvival;
        }

        /// <summary>
        /// Get quest progression recommendations
        /// </summary>
        public static string GetProgressionHint(string currentQuestId)
        {
            switch (currentQuestId)
            {
                case "main_005":
                    return "Continue with main_006 to begin the trials.";
                case "main_010":
                    return "This is the hardest trial. Steel yourself for what comes next.";
                case "main_011":
                    return "The answer to the riddle is 'Love'. Remember what truly matters.";
                case "main_012":
                    return "This is the climax of Book 1. Prepare for an epic encounter.";
                default:
                    return "Continue the main story to progress.";
            }
        }
    }
}
