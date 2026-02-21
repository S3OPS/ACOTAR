using System.Collections.Generic;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// DLC 1: Quest content for Book 2 (A Court of Mist and Fury) storyline
    /// 
    /// REQUIRES: DLC Package ACOMAF_MistAndFury
    /// 
    /// Covers Feyre's journey from Spring Court to the Night Court and the war with Hybern.
    /// Includes Night Court discovery, Inner Circle introduction, training, Summer Court heist,
    /// and the confrontation with Hybern.
    /// </summary>
    public static class Book2Quests
    {
        /// <summary>
        /// Initialize all Book 2 quests into the quest manager
        /// NOTE: This is DLC content - check DLCManager.IsDLCOwned before calling
        /// </summary>
        public static void InitializeBook2Quests(Dictionary<string, Quest> quests)
        {
            Debug.Log("Loading DLC 1: A Court of Mist and Fury content...");
            
            // Night Court Arc
            AddNightCourtQuests(quests);
            
            // Training and Powers Arc
            AddTrainingQuests(quests);
            
            // Summer Court Mission Arc
            AddSummerCourtQuests(quests);
            
            // Hybern Arc
            AddHybernQuests(quests);
            
            // Book 2 Side Quests
            AddBook2SideQuests(quests);
            
            Debug.Log("DLC 1 content loaded successfully.");
        }

        /// <summary>
        /// Night Court introduction quests
        /// </summary>
        private static void AddNightCourtQuests(Dictionary<string, Quest> quests)
        {
            // Arrival at Night Court
            Quest arrival = new Quest(
                "book2_002",
                "The City of Starlight",
                "Rhysand takes you to Velaris, the hidden jewel of the Night Court that no one knows exists.",
                QuestType.MainStory
            );
            arrival.objectives.Add("Arrive in Velaris for the first time");
            arrival.objectives.Add("See the Rainbow district");
            arrival.objectives.Add("Realize the Night Court's true nature");
            arrival.experienceReward = 350;
            arrival.nextQuestId = "book2_003";
            quests[arrival.questId] = arrival;

            // Meeting the Inner Circle
            Quest innerCircle = new Quest(
                "book2_003",
                "The Inner Circle",
                "Meet Rhysand's family: the legendary Inner Circle of the Night Court.",
                QuestType.MainStory
            );
            innerCircle.objectives.Add("Meet Cassian, the General");
            innerCircle.objectives.Add("Meet Azriel, the Spymaster");
            innerCircle.objectives.Add("Meet Mor, the Morrigan");
            innerCircle.objectives.Add("Meet Amren, the ancient one");
            innerCircle.experienceReward = 400;
            innerCircle.nextQuestId = "book2_004";
            quests[innerCircle.questId] = innerCircle;

            // The Town House
            Quest townhouse = new Quest(
                "book2_004",
                "A Place to Heal",
                "Settle into the town house in Velaris. Begin your recovery from Under the Mountain.",
                QuestType.MainStory
            );
            townhouse.objectives.Add("Explore the town house");
            townhouse.objectives.Add("Find peace in the garden");
            townhouse.objectives.Add("Start to feel like yourself again");
            townhouse.experienceReward = 250;
            townhouse.nextQuestId = "book2_005";
            quests[townhouse.questId] = townhouse;

            // The Mating Bond
            Quest matingBond = new Quest(
                "book2_005",
                "A Bond Revealed",
                "Discover the truth about the bond between you and Rhysand - something far deeper than a bargain.",
                QuestType.MainStory
            );
            matingBond.objectives.Add("Feel the strange connection to Rhysand");
            matingBond.objectives.Add("Learn about mating bonds");
            matingBond.objectives.Add("Accept or reject the bond");
            matingBond.experienceReward = 500;
            matingBond.nextQuestId = "book2_006";
            quests[matingBond.questId] = matingBond;
        }

        /// <summary>
        /// Training and powers development quests
        /// </summary>
        private static void AddTrainingQuests(Dictionary<string, Quest> quests)
        {
            // Learning to Read Again
            Quest reading = new Quest(
                "book2_006",
                "Words of Power",
                "Continue learning to read with proper tutoring. Knowledge is power in Prythian.",
                QuestType.MainStory
            );
            reading.objectives.Add("Study with Rhysand in the library");
            reading.objectives.Add("Read your first full book");
            reading.objectives.Add("Discover the pleasure of reading");
            reading.experienceReward = 200;
            reading.nextQuestId = "book2_007";
            quests[reading.questId] = reading;

            // Discovering Powers
            Quest powers = new Quest(
                "book2_007",
                "Gifts of the High Lords",
                "Discover that you have powers from all seven High Lords - gifts given when you were Made.",
                QuestType.MainStory
            );
            powers.objectives.Add("Manifest unexpected powers");
            powers.objectives.Add("Learn you have gifts from all High Lords");
            powers.objectives.Add("Begin to understand your potential");
            powers.experienceReward = 450;
            powers.nextQuestId = "book2_008";
            quests[powers.questId] = powers;

            // Training with Cassian
            Quest cassianTraining = new Quest(
                "book2_008",
                "The Warrior's Way",
                "Train with Cassian to master physical combat and hone your body as a weapon.",
                QuestType.MainStory
            );
            cassianTraining.objectives.Add("Begin training at the House of Wind");
            cassianTraining.objectives.Add("Learn Illyrian fighting techniques");
            cassianTraining.objectives.Add("Build strength and endurance");
            cassianTraining.experienceReward = 350;
            cassianTraining.nextQuestId = "book2_009";
            quests[cassianTraining.questId] = cassianTraining;

            // Mental Shields
            Quest shields = new Quest(
                "book2_009",
                "Shields of the Mind",
                "Learn to build mental shields to protect yourself from Daemati powers.",
                QuestType.MainStory
            );
            shields.objectives.Add("Learn about Daemati powers");
            shields.objectives.Add("Build your mental shields");
            shields.objectives.Add("Practice defending against Rhysand's probing");
            shields.experienceReward = 400;
            shields.nextQuestId = "book2_010";
            quests[shields.questId] = shields;

            // Winnowing Lessons
            Quest winnowing = new Quest(
                "book2_010",
                "Learning to Fly",
                "Master the art of winnowing - teleporting through space in the blink of an eye.",
                QuestType.MainStory
            );
            winnowing.objectives.Add("Understand the theory of winnowing");
            winnowing.objectives.Add("Make your first winnow (short distance)");
            winnowing.objectives.Add("Successfully winnow across Velaris");
            winnowing.experienceReward = 450;
            winnowing.nextQuestId = "book2_011";
            quests[winnowing.questId] = winnowing;
        }

        /// <summary>
        /// Summer Court mission quests
        /// </summary>
        private static void AddSummerCourtQuests(Dictionary<string, Quest> quests)
        {
            // Journey to Summer
            Quest summerJourney = new Quest(
                "book2_011",
                "Across the Sea",
                "Travel to the Summer Court to seek an alliance with High Lord Tarquin.",
                QuestType.MainStory
            );
            summerJourney.objectives.Add("Prepare for the diplomatic mission");
            summerJourney.objectives.Add("Travel to Adriata, the Summer Court capital");
            summerJourney.objectives.Add("Arrive at Tarquin's palace");
            summerJourney.experienceReward = 300;
            summerJourney.nextQuestId = "book2_012";
            quests[summerJourney.questId] = summerJourney;

            // Meeting Tarquin
            Quest tarquin = new Quest(
                "book2_012",
                "The Youngest High Lord",
                "Meet Tarquin, the newly-ascended High Lord of the Summer Court.",
                QuestType.MainStory
            );
            tarquin.objectives.Add("Be presented at the Summer Court");
            tarquin.objectives.Add("Meet High Lord Tarquin");
            tarquin.objectives.Add("Begin alliance negotiations");
            tarquin.experienceReward = 350;
            tarquin.nextQuestId = "book2_013";
            quests[tarquin.questId] = tarquin;

            // The Book of Breathings
            Quest bookHeist = new Quest(
                "book2_013",
                "The Theft",
                "The Night Court needs half of the Book of Breathings. Unfortunately, it's in the Summer Court's treasury.",
                QuestType.MainStory
            );
            bookHeist.objectives.Add("Locate the Book of Breathings");
            bookHeist.objectives.Add("Plan the theft");
            bookHeist.objectives.Add("Steal the book and escape");
            bookHeist.experienceReward = 600;
            bookHeist.nextQuestId = "book2_014";
            quests[bookHeist.questId] = bookHeist;

            // Blood Rubies
            Quest betrayal = new Quest(
                "book2_014",
                "Blood Rubies",
                "Tarquin sends blood rubies - a death promise. The price of betrayal is steep.",
                QuestType.MainStory
            );
            betrayal.objectives.Add("Receive Tarquin's blood rubies");
            betrayal.objectives.Add("Face the consequences of your actions");
            betrayal.objectives.Add("Return to the Night Court");
            betrayal.experienceReward = 250;
            betrayal.nextQuestId = "book2_015";
            quests[betrayal.questId] = betrayal;

            // Finding the Other Half
            Quest otherHalf = new Quest(
                "book2_015",
                "The Other Half",
                "The second half of the Book is in the mortal realm - with the queens.",
                QuestType.MainStory
            );
            otherHalf.objectives.Add("Learn about the mortal queens");
            otherHalf.objectives.Add("Plan to retrieve the other half");
            otherHalf.objectives.Add("Make contact with the queens");
            otherHalf.experienceReward = 350;
            otherHalf.nextQuestId = "book2_016";
            quests[otherHalf.questId] = otherHalf;
        }

        /// <summary>
        /// Hybern confrontation quests
        /// </summary>
        private static void AddHybernQuests(Dictionary<string, Quest> quests)
        {
            // Meeting with Queens
            Quest queens = new Quest(
                "book2_016",
                "The Mortal Queens",
                "Meet with the mortal queens to negotiate for the other half of the Book.",
                QuestType.MainStory
            );
            queens.objectives.Add("Host the mortal queens at the Night Court");
            queens.objectives.Add("Present your case against Hybern");
            queens.objectives.Add("Face their rejection");
            queens.experienceReward = 400;
            queens.nextQuestId = "book2_017";
            quests[queens.questId] = queens;

            // Discovering Hybern's Plans
            Quest hybernPlans = new Quest(
                "book2_017",
                "The Enemy's Design",
                "Learn that the King of Hybern plans to use the Cauldron to break the Wall.",
                QuestType.MainStory
            );
            hybernPlans.objectives.Add("Gather intelligence on Hybern");
            hybernPlans.objectives.Add("Discover the Cauldron's location");
            hybernPlans.objectives.Add("Learn about the Wall's vulnerability");
            hybernPlans.experienceReward = 450;
            hybernPlans.nextQuestId = "book2_018";
            quests[hybernPlans.questId] = hybernPlans;

            // Journey to Hybern
            Quest hybernJourney = new Quest(
                "book2_018",
                "Into the Enemy's Lair",
                "Infiltrate Hybern to destroy the Cauldron before the King can use it.",
                QuestType.MainStory
            );
            hybernJourney.objectives.Add("Cross the sea to Hybern");
            hybernJourney.objectives.Add("Infiltrate the King's castle");
            hybernJourney.objectives.Add("Locate the Cauldron");
            hybernJourney.experienceReward = 500;
            hybernJourney.nextQuestId = "book2_019";
            // v2.6.9: Preparation hint for stealth infiltration
            hybernJourney.preparationHint = "🕵️ INFILTRATION MISSION\nLocation: Hybern (Enemy Territory)\nDanger: King's guards, Cauldron wards\nTips: Go in as a small team. Use stealth first, force only as a last resort. Trust Rhysand's plan - your group's powers must stay concealed until the critical moment.";
            quests[hybernJourney.questId] = hybernJourney;

            // The Cauldron
            Quest cauldron = new Quest(
                "book2_019",
                "The Cauldron",
                "Face the ancient Cauldron - the object that Made all things.",
                QuestType.MainStory
            );
            cauldron.objectives.Add("Confront the Cauldron");
            cauldron.objectives.Add("Witness Nesta and Elain being Made");
            cauldron.objectives.Add("Feel the Cauldron's recognition");
            cauldron.experienceReward = 600;
            cauldron.nextQuestId = "book2_020";
            // v2.6.9: Preparation hint for the Cauldron confrontation
            cauldron.preparationHint = "⚗️ ANCIENT POWER ENCOUNTER\nLocation: Hybern Throne Room - The Cauldron\nDanger: Overwhelming magical force, King of Hybern\nTips: You cannot fight the Cauldron directly. Protect Nesta and Elain at all costs. The Cauldron sees everything - keep your mind shielded. Escape is the priority.";
            quests[cauldron.questId] = cauldron;

            // Sisters Made
            Quest sisters = new Quest(
                "book2_020",
                "Sisters Transformed",
                "Your sisters have been forced into the Cauldron. They are High Fae now - and broken.",
                QuestType.MainStory
            );
            sisters.objectives.Add("Rescue your sisters");
            sisters.objectives.Add("Escape from Hybern");
            sisters.objectives.Add("Return safely to the Night Court");
            sisters.experienceReward = 700;
            sisters.nextQuestId = "book2_021";
            quests[sisters.questId] = sisters;

            // The Wall Falls
            Quest wallFalls = new Quest(
                "book2_021",
                "The Wall Falls",
                "Despite your efforts, the King of Hybern breaks the Wall. War is inevitable.",
                QuestType.MainStory
            );
            wallFalls.objectives.Add("Witness the destruction of the Wall");
            wallFalls.objectives.Add("Realize war has begun");
            wallFalls.objectives.Add("Rally the Night Court for war");
            wallFalls.experienceReward = 500;
            wallFalls.nextQuestId = "book2_022";
            quests[wallFalls.questId] = wallFalls;

            // Book 2 Climax
            Quest climax = new Quest(
                "book2_022",
                "High Lady of the Night Court",
                "Accept your role as Rhysand's equal - the first High Lady in Prythian history.",
                QuestType.MainStory
            );
            climax.objectives.Add("Accept the mating bond");
            climax.objectives.Add("Be crowned High Lady");
            climax.objectives.Add("Stand beside Rhysand as an equal");
            climax.objectives.Add("Prepare for the coming war");
            climax.experienceReward = 1000;
            quests[climax.questId] = climax;
        }

        /// <summary>
        /// Book 2 side quests
        /// </summary>
        private static void AddBook2SideQuests(Dictionary<string, Quest> quests)
        {
            // Starfall Festival
            Quest starfall = new Quest(
                "side_010",
                "Starfall",
                "Experience the magical Starfall festival in Velaris.",
                QuestType.SideQuest
            );
            starfall.objectives.Add("Attend the Starfall celebration");
            starfall.objectives.Add("Watch the falling stars");
            starfall.objectives.Add("Share the moment with the Inner Circle");
            starfall.experienceReward = 200;
            quests[starfall.questId] = starfall;

            // Rita's Bar
            Quest ritas = new Quest(
                "side_011",
                "A Night at Rita's",
                "Experience the famous nightlife of Velaris at Rita's bar.",
                QuestType.SideQuest
            );
            ritas.objectives.Add("Visit Rita's with the Inner Circle");
            ritas.objectives.Add("Dance until dawn");
            ritas.objectives.Add("Bond with your new family");
            ritas.experienceReward = 150;
            quests[ritas.questId] = ritas;

            // Court of Nightmares
            Quest nightmares = new Quest(
                "side_012",
                "Return to the Hewn City",
                "Visit the Court of Nightmares again, this time as Rhysand's equal.",
                QuestType.CourtQuest
            );
            nightmares.objectives.Add("Descend into the Hewn City");
            nightmares.objectives.Add("Play the role of the dark High Lady");
            nightmares.objectives.Add("Maintain the necessary illusion");
            nightmares.experienceReward = 350;
            quests[nightmares.questId] = nightmares;

            // The Weaver
            Quest weaver = new Quest(
                "side_013",
                "The Weaver's Ring",
                "Retrieve a ring from the Weaver, an ancient immortal in the Middle.",
                QuestType.SideQuest
            );
            weaver.objectives.Add("Enter the Weaver's territory");
            weaver.objectives.Add("Find the ring");
            weaver.objectives.Add("Escape with your life");
            weaver.experienceReward = 400;
            quests[weaver.questId] = weaver;

            // The Suriel Again
            Quest surielBook2 = new Quest(
                "side_014",
                "Wisdom from the Suriel",
                "Seek the Suriel's wisdom about the upcoming war.",
                QuestType.SideQuest
            );
            surielBook2.objectives.Add("Find and trap a Suriel");
            surielBook2.objectives.Add("Ask about Hybern's weaknesses");
            surielBook2.objectives.Add("Learn crucial information");
            surielBook2.experienceReward = 250;
            quests[surielBook2.questId] = surielBook2;

            // Illyrian War Camps
            Quest illyrians = new Quest(
                "side_015",
                "Wings and Embers",
                "Visit the Illyrian war camps to secure their support for the coming war.",
                QuestType.CourtQuest
            );
            illyrians.objectives.Add("Travel to the Illyrian Mountains");
            illyrians.objectives.Add("Meet with the war lords");
            illyrians.objectives.Add("Witness the treatment of Illyrian females");
            illyrians.objectives.Add("Vow to change things");
            illyrians.experienceReward = 400;
            quests[illyrians.questId] = illyrians;

            // Mor's Story
            Quest morQuest = new Quest(
                "companion_003",
                "The Truth of Mor",
                "Learn about Mor's painful past and her escape from the Court of Nightmares.",
                QuestType.CompanionQuest
            );
            morQuest.objectives.Add("Spend time with Mor");
            morQuest.objectives.Add("Hear her story");
            morQuest.objectives.Add("Understand her strength");
            morQuest.experienceReward = 300;
            quests[morQuest.questId] = morQuest;

            // Azriel's Shadows
            Quest azrielQuest = new Quest(
                "companion_004",
                "Shadows and Secrets",
                "Learn about Azriel, the mysterious Spymaster of the Night Court.",
                QuestType.CompanionQuest
            );
            azrielQuest.objectives.Add("Watch Azriel work");
            azrielQuest.objectives.Add("Learn about his scarred past");
            azrielQuest.objectives.Add("Understand his loyalty to Rhysand");
            azrielQuest.experienceReward = 300;
            quests[azrielQuest.questId] = azrielQuest;

            // Amren's Nature
            Quest amrenQuest = new Quest(
                "companion_005",
                "The Ancient One",
                "Discover the truth about Amren - what she was before she was bound to this form.",
                QuestType.CompanionQuest
            );
            amrenQuest.objectives.Add("Research Amren's origins");
            amrenQuest.objectives.Add("Learn about her true nature");
            amrenQuest.objectives.Add("Understand her sacrifice");
            amrenQuest.experienceReward = 350;
            quests[amrenQuest.questId] = amrenQuest;

            // NEW ENHANCED DLC 1 SIDE QUESTS
            
            // Velaris Exploration
            Quest velarisExplore = new Quest(
                "side_016_book2",
                "Rainbow of Velaris",
                "Explore the Rainbow district of Velaris and meet the artists and craftspeople.",
                QuestType.ExplorationQuest
            );
            velarisExplore.objectives.Add("Visit the art galleries");
            velarisExplore.objectives.Add("Meet the local artisans");
            velarisExplore.objectives.Add("Commission a painting");
            velarisExplore.experienceReward = 200;
            velarisExplore.isDLCContent = true;
            quests[velarisExplore.questId] = velarisExplore;

            // House of Wind
            Quest houseOfWind = new Quest(
                "side_017_book2",
                "The Ten Thousand Steps",
                "Master the climb to the House of Wind and discover its secrets.",
                QuestType.SideQuest
            );
            houseOfWind.objectives.Add("Climb all ten thousand steps");
            houseOfWind.objectives.Add("Explore the House of Wind");
            houseOfWind.objectives.Add("Learn to winnow to save time");
            houseOfWind.experienceReward = 225;
            houseOfWind.isDLCContent = true;
            quests[houseOfWind.questId] = houseOfWind;

            // Cassian Training Advanced
            Quest cassianAdvanced = new Quest(
                "side_018_book2",
                "Wings of War",
                "Undergo advanced combat training with Cassian and the Illyrians.",
                QuestType.CompanionQuest
            );
            cassianAdvanced.objectives.Add("Learn Illyrian battle techniques");
            cassianAdvanced.objectives.Add("Spar with experienced warriors");
            cassianAdvanced.objectives.Add("Earn the respect of the legions");
            cassianAdvanced.experienceReward = 300;
            cassianAdvanced.isDLCContent = true;
            quests[cassianAdvanced.questId] = cassianAdvanced;

            // Library of Velaris
            Quest library = new Quest(
                "side_019_book2",
                "The Library Below",
                "Discover the vast library beneath the House of Wind and its priestesses.",
                QuestType.ExplorationQuest
            );
            library.objectives.Add("Explore the library levels");
            library.objectives.Add("Meet the priestess scholars");
            library.objectives.Add("Research ancient magical texts");
            library.experienceReward = 250;
            library.isDLCContent = true;
            quests[library.questId] = library;

            // Night Court Politics
            Quest politics = new Quest(
                "side_020_book2",
                "Court of Dreams",
                "Learn to navigate the complex politics of the Night Court's two faces.",
                QuestType.CourtQuest
            );
            politics.objectives.Add("Attend a Court of Dreams gathering");
            politics.objectives.Add("Balance Velaris and Hewn City politics");
            politics.objectives.Add("Understand Rhysand's dual role");
            politics.experienceReward = 275;
            politics.isDLCContent = true;
            quests[politics.questId] = politics;

            // Adriata Adventure
            Quest adriata = new Quest(
                "side_021_book2",
                "By the Summer Sea",
                "Explore Adriata, the jewel of the Summer Court, before the heist.",
                QuestType.ExplorationQuest
            );
            adriata.objectives.Add("Tour the Summer Court capital");
            adriata.objectives.Add("Visit the famous beaches");
            adriata.objectives.Add("Sample Summer Court cuisine");
            adriata.experienceReward = 200;
            adriata.isDLCContent = true;
            quests[adriata.questId] = adriata;
        }

        /// <summary>
        /// Get quest progression recommendations for Book 2
        /// </summary>
        public static string GetProgressionHint(string currentQuestId)
        {
            switch (currentQuestId)
            {
                case "book2_001":
                    return "Your journey to the Night Court begins. Prepare for everything to change.";
                case "book2_005":
                    return "The mating bond is sacred among Fae. Consider carefully before accepting or rejecting.";
                case "book2_013":
                    return "Stealing from allies has consequences. Are you prepared for the fallout?";
                case "book2_019":
                    return "The Cauldron Made all things - including you. It remembers.";
                case "book2_022":
                    return "You have become something new - the first High Lady. Embrace your power.";
                default:
                    return "Continue through Book 2 to experience A Court of Mist and Fury.";
            }
        }
    }
}
