using System.Collections.Generic;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Quest content for Book 3 (A Court of Wings and Ruin) storyline
    /// Covers the war against Hybern and the final battle for Prythian
    /// </summary>
    public static class Book3Quests
    {
        /// <summary>
        /// Initialize all Book 3 quests into the quest manager
        /// </summary>
        public static void InitializeBook3Quests(Dictionary<string, Quest> quests)
        {
            // Spring Court Return Arc
            AddSpringCourtArc(quests);
            
            // Alliance Building Arc
            AddAllianceQuests(quests);
            
            // War Preparation Arc
            AddWarPreparationQuests(quests);
            
            // Final Battle Arc
            AddFinalBattleQuests(quests);
            
            // Book 3 Side Quests
            AddBook3SideQuests(quests);
        }

        /// <summary>
        /// Spring Court infiltration quests
        /// </summary>
        private static void AddSpringCourtArc(Dictionary<string, Quest> quests)
        {
            // Return to Spring Court
            Quest returnSpring = new Quest(
                "book3_001",
                "Return to Spring",
                "You must return to the Spring Court as a spy - pretending Rhysand abandoned you.",
                QuestType.MainStory
            );
            returnSpring.objectives.Add("Travel to the Spring Court");
            returnSpring.objectives.Add("Convince Tamlin you were abandoned");
            returnSpring.objectives.Add("Regain his trust");
            returnSpring.experienceReward = 400;
            returnSpring.nextQuestId = "book3_002";
            quests[returnSpring.questId] = returnSpring;

            // Spy in Spring
            Quest spySpring = new Quest(
                "book3_002",
                "The Spy",
                "Gather intelligence from within the Spring Court while Tamlin hosts Hybern forces.",
                QuestType.MainStory
            );
            spySpring.objectives.Add("Observe Hybern's movements");
            spySpring.objectives.Add("Gather military intelligence");
            spySpring.objectives.Add("Maintain your cover");
            spySpring.experienceReward = 450;
            spySpring.nextQuestId = "book3_003";
            quests[spySpring.questId] = spySpring;

            // Ianthe's Treachery
            Quest ianthe = new Quest(
                "book3_003",
                "The Priestess",
                "Deal with Ianthe, the treacherous priestess who betrayed your sisters to Hybern.",
                QuestType.MainStory
            );
            ianthe.objectives.Add("Confront Ianthe");
            ianthe.objectives.Add("Learn the full extent of her betrayal");
            ianthe.objectives.Add("Decide her fate");
            ianthe.experienceReward = 350;
            ianthe.nextQuestId = "book3_004";
            quests[ianthe.questId] = ianthe;

            // Escape from Spring
            Quest escapeSpring = new Quest(
                "book3_004",
                "Burning Bridges",
                "Your cover is blown. Escape the Spring Court and reveal Tamlin's alliance with Hybern to all of Prythian.",
                QuestType.MainStory
            );
            escapeSpring.objectives.Add("Destroy the Spring Court's defenses");
            escapeSpring.objectives.Add("Unleash your powers");
            escapeSpring.objectives.Add("Escape back to the Night Court");
            escapeSpring.experienceReward = 600;
            escapeSpring.nextQuestId = "book3_005";
            quests[escapeSpring.questId] = escapeSpring;
        }

        /// <summary>
        /// Alliance building quests
        /// </summary>
        private static void AddAllianceQuests(Dictionary<string, Quest> quests)
        {
            // High Lords Meeting
            Quest highLords = new Quest(
                "book3_005",
                "The High Lords' Meeting",
                "Call a meeting of all seven High Lords to unite against Hybern.",
                QuestType.MainStory
            );
            highLords.objectives.Add("Send summons to all High Lords");
            highLords.objectives.Add("Host the meeting in the Dawn Court");
            highLords.objectives.Add("Convince the courts to unite");
            highLords.experienceReward = 700;
            highLords.nextQuestId = "book3_006";
            quests[highLords.questId] = highLords;

            // Convincing Beron
            Quest beron = new Quest(
                "book3_006",
                "The Autumn Court",
                "Beron, High Lord of Autumn, is reluctant to join. Find a way to secure his support.",
                QuestType.MainStory
            );
            beron.objectives.Add("Navigate Autumn Court politics");
            beron.objectives.Add("Deal with Eris's machinations");
            beron.objectives.Add("Secure Autumn's support");
            beron.experienceReward = 450;
            beron.nextQuestId = "book3_007";
            quests[beron.questId] = beron;

            // The Day Court
            Quest dayCourtAlliance = new Quest(
                "book3_007",
                "Light and Knowledge",
                "Seek the support of Helion, High Lord of the Day Court, and his vast libraries.",
                QuestType.MainStory
            );
            dayCourtAlliance.objectives.Add("Meet High Lord Helion");
            dayCourtAlliance.objectives.Add("Learn about Day Court magic");
            dayCourtAlliance.objectives.Add("Discover important secrets about Lucien's parentage");
            dayCourtAlliance.experienceReward = 400;
            dayCourtAlliance.nextQuestId = "book3_008";
            quests[dayCourtAlliance.questId] = dayCourtAlliance;

            // Winter Court Support
            Quest winterAlliance = new Quest(
                "book3_008",
                "Ice and Honor",
                "Gain the support of Kallias and Viviane of the Winter Court.",
                QuestType.MainStory
            );
            winterAlliance.objectives.Add("Travel to the Winter Court");
            winterAlliance.objectives.Add("Meet High Lord Kallias and Lady Viviane");
            winterAlliance.objectives.Add("Secure their military support");
            winterAlliance.experienceReward = 350;
            winterAlliance.nextQuestId = "book3_009";
            quests[winterAlliance.questId] = winterAlliance;

            // Making Amends with Summer
            Quest summerAmends = new Quest(
                "book3_009",
                "Mending Bridges",
                "Attempt to make amends with Tarquin and the Summer Court for the stolen book.",
                QuestType.MainStory
            );
            summerAmends.objectives.Add("Return to the Summer Court");
            summerAmends.objectives.Add("Face Tarquin's anger");
            summerAmends.objectives.Add("Work toward forgiveness");
            summerAmends.experienceReward = 400;
            summerAmends.nextQuestId = "book3_010";
            quests[summerAmends.questId] = summerAmends;
        }

        /// <summary>
        /// War preparation quests
        /// </summary>
        private static void AddWarPreparationQuests(Dictionary<string, Quest> quests)
        {
            // Nesta's Powers
            Quest nestaPowers = new Quest(
                "book3_010",
                "The Cauldron's Gift",
                "Help Nesta understand and control the powers she took from the Cauldron.",
                QuestType.MainStory
            );
            nestaPowers.objectives.Add("Train with Nesta");
            nestaPowers.objectives.Add("Help her control her rage");
            nestaPowers.objectives.Add("Prepare her for war");
            nestaPowers.experienceReward = 400;
            nestaPowers.nextQuestId = "book3_011";
            quests[nestaPowers.questId] = nestaPowers;

            // Elain's Visions
            Quest elainVisions = new Quest(
                "book3_011",
                "The Seer",
                "Elain has become a Seer. Help her cope with her visions and use them for the war effort.",
                QuestType.MainStory
            );
            elainVisions.objectives.Add("Support Elain through her adjustment");
            elainVisions.objectives.Add("Interpret her visions");
            elainVisions.objectives.Add("Use her sight to plan for battle");
            elainVisions.experienceReward = 350;
            elainVisions.nextQuestId = "book3_012";
            quests[elainVisions.questId] = elainVisions;

            // The Cauldron's Location
            Quest findCauldron = new Quest(
                "book3_012",
                "Tracking the Cauldron",
                "Locate the Cauldron to prevent Hybern from using it further.",
                QuestType.MainStory
            );
            findCauldron.objectives.Add("Use Elain's visions to track the Cauldron");
            findCauldron.objectives.Add("Plan a strike team");
            findCauldron.objectives.Add("Prepare to nullify its power");
            findCauldron.experienceReward = 450;
            findCauldron.nextQuestId = "book3_013";
            quests[findCauldron.questId] = findCauldron;

            // Bone Carver's Bargain
            Quest boneCarver = new Quest(
                "book3_013",
                "The Bone Carver's Price",
                "The Bone Carver will fight for Prythian - but at a terrible price.",
                QuestType.MainStory
            );
            boneCarver.objectives.Add("Visit the Bone Carver's cell");
            boneCarver.objectives.Add("Negotiate his release");
            boneCarver.objectives.Add("Learn his price for joining the war");
            boneCarver.experienceReward = 400;
            boneCarver.nextQuestId = "book3_014";
            quests[boneCarver.questId] = boneCarver;

            // Freeing the Prisoners
            Quest bryaxis = new Quest(
                "book3_014",
                "Darkness Unleashed",
                "Free the ancient beings imprisoned in the Prison to fight against Hybern.",
                QuestType.MainStory
            );
            bryaxis.objectives.Add("Descend into the Prison");
            bryaxis.objectives.Add("Release Bryaxis");
            bryaxis.objectives.Add("Unleash the horrors within against Hybern");
            bryaxis.experienceReward = 500;
            bryaxis.nextQuestId = "book3_015";
            quests[bryaxis.questId] = bryaxis;
        }

        /// <summary>
        /// Final battle quests
        /// </summary>
        private static void AddFinalBattleQuests(Dictionary<string, Quest> quests)
        {
            // First Battle
            Quest firstBattle = new Quest(
                "book3_015",
                "The First Clash",
                "The forces of Prythian meet Hybern's army in the first major battle of the war.",
                QuestType.MainStory
            );
            firstBattle.objectives.Add("Lead Night Court forces into battle");
            firstBattle.objectives.Add("Fight alongside your allies");
            firstBattle.objectives.Add("Survive the first engagement");
            firstBattle.experienceReward = 600;
            firstBattle.nextQuestId = "book3_016";
            quests[firstBattle.questId] = firstBattle;

            // Cauldron Strike
            Quest cauldronStrike = new Quest(
                "book3_016",
                "Into the Heart",
                "Lead a strike team to nullify the Cauldron's power during the battle.",
                QuestType.MainStory
            );
            cauldronStrike.objectives.Add("Infiltrate the Hybern camp");
            cauldronStrike.objectives.Add("Reach the Cauldron");
            cauldronStrike.objectives.Add("Use Nesta's power to nullify it");
            cauldronStrike.experienceReward = 700;
            cauldronStrike.nextQuestId = "book3_017";
            quests[cauldronStrike.questId] = cauldronStrike;

            // Amren's Sacrifice
            Quest amrenSacrifice = new Quest(
                "book3_017",
                "Unleashed",
                "Amren must return to her true form to turn the tide of battle.",
                QuestType.MainStory
            );
            amrenSacrifice.objectives.Add("Witness Amren's transformation");
            amrenSacrifice.objectives.Add("Support her decision");
            amrenSacrifice.objectives.Add("Watch her devastate Hybern's forces");
            amrenSacrifice.experienceReward = 600;
            amrenSacrifice.nextQuestId = "book3_018";
            quests[amrenSacrifice.questId] = amrenSacrifice;

            // Final Battle
            Quest finalBattle = new Quest(
                "book3_018",
                "The Final Battle",
                "All of Prythian stands united against the King of Hybern. End this war.",
                QuestType.MainStory
            );
            finalBattle.objectives.Add("Lead the unified forces of Prythian");
            finalBattle.objectives.Add("Confront the King of Hybern");
            finalBattle.objectives.Add("Fight for the future of your world");
            finalBattle.experienceReward = 800;
            finalBattle.nextQuestId = "book3_019";
            quests[finalBattle.questId] = finalBattle;

            // Rhysand's Death
            Quest rhysandDeath = new Quest(
                "book3_019",
                "Death and Resurrection",
                "Rhysand gives everything to seal the Cauldron. Pay any price to bring him back.",
                QuestType.MainStory
            );
            rhysandDeath.objectives.Add("Witness Rhysand's sacrifice");
            rhysandDeath.objectives.Add("Feel the mating bond shatter");
            rhysandDeath.objectives.Add("Demand the High Lords bring him back");
            rhysandDeath.experienceReward = 700;
            rhysandDeath.nextQuestId = "book3_020";
            quests[rhysandDeath.questId] = rhysandDeath;

            // King of Hybern Falls
            Quest killKing = new Quest(
                "book3_020",
                "The King Falls",
                "Kill the King of Hybern and end the war.",
                QuestType.MainStory
            );
            killKing.objectives.Add("Face the King of Hybern");
            killKing.objectives.Add("Deliver the killing blow");
            killKing.objectives.Add("Watch Nesta take his head");
            killKing.experienceReward = 1000;
            killKing.nextQuestId = "book3_021";
            quests[killKing.questId] = killKing;

            // Victory
            Quest victory = new Quest(
                "book3_021",
                "A Court of Wings and Ruin",
                "The war is over. Prythian is forever changed, but it survives.",
                QuestType.MainStory
            );
            victory.objectives.Add("Mourn the fallen");
            victory.objectives.Add("Celebrate the victory");
            victory.objectives.Add("Begin to rebuild");
            victory.objectives.Add("Look toward the future");
            victory.experienceReward = 1500;
            quests[victory.questId] = victory;
        }

        /// <summary>
        /// Book 3 side quests
        /// </summary>
        private static void AddBook3SideQuests(Dictionary<string, Quest> quests)
        {
            // Lucien's Quest
            Quest lucienQuest = new Quest(
                "companion_006",
                "The Autumn Prince",
                "Help Lucien discover the truth about his parentage and his connection to the Day Court.",
                QuestType.CompanionQuest
            );
            lucienQuest.objectives.Add("Support Lucien's search for truth");
            lucienQuest.objectives.Add("Uncover his true father's identity");
            lucienQuest.objectives.Add("Help him accept who he really is");
            lucienQuest.experienceReward = 400;
            quests[lucienQuest.questId] = lucienQuest;

            // Nesta's Companion Quest
            Quest nestaCompanion = new Quest(
                "companion_007",
                "Forged in Fire",
                "Bond with your sister Nesta as she struggles with her transformation.",
                QuestType.CompanionQuest
            );
            nestaCompanion.objectives.Add("Reach out to Nesta");
            nestaCompanion.objectives.Add("Understand her anger");
            nestaCompanion.objectives.Add("Fight alongside her");
            nestaCompanion.experienceReward = 350;
            quests[nestaCompanion.questId] = nestaCompanion;

            // Elain's Garden
            Quest elainGarden = new Quest(
                "companion_008",
                "The Garden Seer",
                "Help Elain find peace through gardening while she adjusts to her new Fae nature.",
                QuestType.CompanionQuest
            );
            elainGarden.objectives.Add("Create a garden with Elain");
            elainGarden.objectives.Add("Help her process her transformation");
            elainGarden.objectives.Add("Support her growing confidence");
            elainGarden.experienceReward = 250;
            quests[elainGarden.questId] = elainGarden;

            // Court of Dreams Defense
            Quest velarisDefense = new Quest(
                "side_016",
                "Defending Velaris",
                "Protect Velaris from Hybern's attempt to destroy the City of Starlight.",
                QuestType.SideQuest
            );
            velarisDefense.objectives.Add("Fortify Velaris's defenses");
            velarisDefense.objectives.Add("Repel the Hybern attack");
            velarisDefense.objectives.Add("Protect the citizens");
            velarisDefense.experienceReward = 500;
            quests[velarisDefense.questId] = velarisDefense;

            // The Mortal Lands
            Quest mortalLands = new Quest(
                "side_017",
                "Protecting the Mortals",
                "Help defend the mortal lands now that the Wall has fallen.",
                QuestType.SideQuest
            );
            mortalLands.objectives.Add("Organize mortal defenses");
            mortalLands.objectives.Add("Evacuate vulnerable villages");
            mortalLands.objectives.Add("Establish peace between mortals and Fae");
            mortalLands.experienceReward = 400;
            quests[mortalLands.questId] = mortalLands;

            // Dawn Court Healing
            Quest dawnHealing = new Quest(
                "side_018",
                "The Healers of Dawn",
                "Learn healing magic from the Dawn Court to help the wounded.",
                QuestType.CourtQuest
            );
            dawnHealing.objectives.Add("Travel to the Dawn Court");
            dawnHealing.objectives.Add("Study with their healers");
            dawnHealing.objectives.Add("Use your power to heal allies");
            dawnHealing.experienceReward = 350;
            quests[dawnHealing.questId] = dawnHealing;

            // Illyrian Legions
            Quest illyrianLegions = new Quest(
                "side_019",
                "Rallying the Legions",
                "Convince all Illyrian war camps to fight for Prythian.",
                QuestType.CourtQuest
            );
            illyrianLegions.objectives.Add("Visit each war camp");
            illyrianLegions.objectives.Add("Deal with rebellious war-lords");
            illyrianLegions.objectives.Add("Unite the Illyrian legions");
            illyrianLegions.experienceReward = 450;
            quests[illyrianLegions.questId] = illyrianLegions;

            // Jurian's Redemption
            Quest jurian = new Quest(
                "side_020",
                "The Human General",
                "Discover if Jurian, the resurrected human general, can be trusted.",
                QuestType.SideQuest
            );
            jurian.objectives.Add("Investigate Jurian's loyalties");
            jurian.objectives.Add("Uncover his true motives");
            jurian.objectives.Add("Decide if he's ally or enemy");
            jurian.experienceReward = 300;
            quests[jurian.questId] = jurian;

            // Vassa the Firebird
            Quest vassa = new Quest(
                "side_021",
                "The Cursed Queen",
                "Meet Vassa, the mortal queen cursed to be a firebird by day.",
                QuestType.SideQuest
            );
            vassa.objectives.Add("Find the firebird queen");
            vassa.objectives.Add("Learn about her curse");
            vassa.objectives.Add("Offer to help break her curse");
            vassa.experienceReward = 350;
            quests[vassa.questId] = vassa;
        }

        /// <summary>
        /// Get quest progression recommendations for Book 3
        /// </summary>
        public static string GetProgressionHint(string currentQuestId)
        {
            switch (currentQuestId)
            {
                case "book3_001":
                    return "Returning to Spring as a spy requires you to hide everything - even your love.";
                case "book3_005":
                    return "Uniting the High Lords is nearly impossible. You must be diplomat, warrior, and ruler.";
                case "book3_015":
                    return "The first battle will test everything you've learned. Trust your training.";
                case "book3_019":
                    return "When the mating bond shatters, demand they return what is yours.";
                case "book3_021":
                    return "Congratulations! You have completed the main ACOTAR trilogy storyline.";
                default:
                    return "Continue through Book 3 to experience A Court of Wings and Ruin.";
            }
        }
    }
}
