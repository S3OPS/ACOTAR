using System;
using System.Collections.Generic;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Types of dialogue nodes
    /// </summary>
    public enum DialogueNodeType
    {
        Standard,       // Regular dialogue
        Question,       // Player chooses response
        Information,    // Expository dialogue
        QuestOffer,     // NPC offers quest
        Shop,          // Opens shop interface
        End            // Ends conversation
    }

    /// <summary>
    /// Represents a single node in a dialogue tree
    /// </summary>
    [Serializable]
    public class DialogueNode
    {
        public string nodeId;
        public string speakerName;
        public string dialogueText;
        public DialogueNodeType nodeType;
        public List<DialogueChoice> choices;
        public string nextNodeId; // For linear dialogue
        
        // Conditions
        public int requiredReputationLevel;
        public string requiredQuestId;
        public bool requiresCompanion;

        public DialogueNode(string id, string speaker, string text, DialogueNodeType type = DialogueNodeType.Standard)
        {
            this.nodeId = id;
            this.speakerName = speaker;
            this.dialogueText = text;
            this.nodeType = type;
            this.choices = new List<DialogueChoice>();
            this.nextNodeId = "";
            this.requiredReputationLevel = -1;
            this.requiredQuestId = "";
            this.requiresCompanion = false;
        }

        /// <summary>
        /// Add a choice to this dialogue node
        /// </summary>
        public void AddChoice(DialogueChoice choice)
        {
            choices.Add(choice);
        }
    }

    /// <summary>
    /// Represents a player choice in dialogue
    /// </summary>
    [Serializable]
    public class DialogueChoice
    {
        public string choiceText;
        public string targetNodeId;
        public int reputationImpact; // Can be positive or negative
        public Court? affectedCourt;
        public string questToStart;
        public Action onSelected; // Callback when choice is selected

        public DialogueChoice(string text, string targetNode)
        {
            this.choiceText = text;
            this.targetNodeId = targetNode;
            this.reputationImpact = 0;
            this.affectedCourt = null;
            this.questToStart = "";
        }
    }

    /// <summary>
    /// Represents a complete dialogue tree with an NPC
    /// </summary>
    [Serializable]
    public class DialogueTree
    {
        public string treeId;
        public string npcName;
        public Dictionary<string, DialogueNode> nodes;
        public string rootNodeId;

        public DialogueTree(string id, string npc, string rootNode)
        {
            this.treeId = id;
            this.npcName = npc;
            this.rootNodeId = rootNode;
            this.nodes = new Dictionary<string, DialogueNode>();
        }

        /// <summary>
        /// Add a node to the dialogue tree
        /// </summary>
        public void AddNode(DialogueNode node)
        {
            nodes[node.nodeId] = node;
        }

        /// <summary>
        /// Get a node by ID
        /// </summary>
        public DialogueNode GetNode(string nodeId)
        {
            return nodes.ContainsKey(nodeId) ? nodes[nodeId] : null;
        }

        /// <summary>
        /// Get the root node (starting point)
        /// </summary>
        public DialogueNode GetRootNode()
        {
            return GetNode(rootNodeId);
        }
    }

    /// <summary>
    /// Manages dialogue system and NPC conversations
    /// Handles branching dialogue trees, player choices, and consequences
    /// </summary>
    public class DialogueSystem : MonoBehaviour
    {
        private Dictionary<string, DialogueTree> dialogueTrees;
        private DialogueTree currentDialogue;
        private DialogueNode currentNode;
        private Character player;
        private ReputationSystem reputationSystem;

        void Awake()
        {
            dialogueTrees = new Dictionary<string, DialogueTree>();
            InitializeDialogues();
        }

        /// <summary>
        /// Initialize dialogue trees for NPCs
        /// </summary>
        private void InitializeDialogues()
        {
            CreateRhysandDialogue();
            CreateTamlinDialogue();
            CreateLucienDialogue();
            CreateSurielDialogue();
        }

        /// <summary>
        /// Create Rhysand's dialogue tree
        /// </summary>
        private void CreateRhysandDialogue()
        {
            DialogueTree tree = new DialogueTree("rhysand_greeting", "Rhysand", "rhys_intro");

            // Introduction node
            DialogueNode intro = new DialogueNode(
                "rhys_intro",
                "Rhysand",
                "Well, well. What do we have here? A curious soul wandering into my territory.",
                DialogueNodeType.Question
            );
            intro.AddChoice(new DialogueChoice("Who are you?", "rhys_identity"));
            intro.AddChoice(new DialogueChoice("I'm not afraid of you.", "rhys_brave"));
            intro.AddChoice(new DialogueChoice("[Leave]", "END"));
            tree.AddNode(intro);

            // Identity reveal
            DialogueNode identity = new DialogueNode(
                "rhys_identity",
                "Rhysand",
                "I am Rhysand, High Lord of the Night Court. Some call me the most powerful High Lord in Prythian.",
                DialogueNodeType.Standard
            );
            identity.nextNodeId = "rhys_offer";
            tree.AddNode(identity);

            // Brave response
            DialogueNode brave = new DialogueNode(
                "rhys_brave",
                "Rhysand",
                "*smirks* I like you already. Courage is a rare quality these days.",
                DialogueNodeType.Standard
            );
            brave.nextNodeId = "rhys_offer";
            tree.AddNode(brave);

            // Offer to help
            DialogueNode offer = new DialogueNode(
                "rhys_offer",
                "Rhysand",
                "I have a proposition for you. The Night Court could use someone with your... potential.",
                DialogueNodeType.Question
            );
            offer.AddChoice(new DialogueChoice("Tell me more.", "rhys_details") { reputationImpact = 5, affectedCourt = Court.Night });
            offer.AddChoice(new DialogueChoice("I'm not interested.", "rhys_refused") { reputationImpact = -5, affectedCourt = Court.Night });
            tree.AddNode(offer);

            // Details
            DialogueNode details = new DialogueNode(
                "rhys_details",
                "Rhysand",
                "Join me in Velaris. I can teach you to harness your power, to become more than you ever imagined.",
                DialogueNodeType.End
            );
            tree.AddNode(details);

            // Refused
            DialogueNode refused = new DialogueNode(
                "rhys_refused",
                "Rhysand",
                "A pity. The offer stands, should you change your mind. To the stars who listen.",
                DialogueNodeType.End
            );
            tree.AddNode(refused);

            dialogueTrees[tree.treeId] = tree;
        }

        /// <summary>
        /// Create Tamlin's dialogue tree
        /// </summary>
        private void CreateTamlinDialogue()
        {
            DialogueTree tree = new DialogueTree("tamlin_greeting", "Tamlin", "tamlin_intro");

            DialogueNode intro = new DialogueNode(
                "tamlin_intro",
                "Tamlin",
                "You shouldn't be here. The Spring Court is dangerous for mortals.",
                DialogueNodeType.Question
            );
            intro.AddChoice(new DialogueChoice("I can take care of myself.", "tamlin_respect"));
            intro.AddChoice(new DialogueChoice("Please, I need help.", "tamlin_help"));
            tree.AddNode(intro);

            DialogueNode respect = new DialogueNode(
                "tamlin_respect",
                "Tamlin",
                "Perhaps you can. Very well, stay if you must. But know that these lands are under my protection.",
                DialogueNodeType.End
            );
            tree.AddNode(respect);

            DialogueNode help = new DialogueNode(
                "tamlin_help",
                "Tamlin",
                "What kind of help do you need?",
                DialogueNodeType.Information
            );
            help.nextNodeId = "END";
            tree.AddNode(help);

            dialogueTrees[tree.treeId] = tree;
        }

        /// <summary>
        /// Create Lucien's dialogue tree
        /// </summary>
        private void CreateLucienDialogue()
        {
            DialogueTree tree = new DialogueTree("lucien_greeting", "Lucien", "lucien_intro");

            DialogueNode intro = new DialogueNode(
                "lucien_intro",
                "Lucien",
                "*russet eye gleaming* Welcome to the Spring Court manor. I'm Lucien, the High Lord's emissary.",
                DialogueNodeType.Question
            );
            intro.AddChoice(new DialogueChoice("What can you tell me about this place?", "lucien_info"));
            intro.AddChoice(new DialogueChoice("You have an interesting eye.", "lucien_eye"));
            intro.AddChoice(new DialogueChoice("[Nod and leave]", "END"));
            tree.AddNode(intro);

            DialogueNode info = new DialogueNode(
                "lucien_info",
                "Lucien",
                "The Spring Court has been... troubled lately. But Tamlin is doing his best to keep us safe.",
                DialogueNodeType.End
            );
            tree.AddNode(info);

            DialogueNode eye = new DialogueNode(
                "lucien_eye",
                "Lucien",
                "*touches his mechanical eye* A gift from the Autumn Court. Long story. Not one I care to repeat.",
                DialogueNodeType.End
            );
            tree.AddNode(eye);

            dialogueTrees[tree.treeId] = tree;
        }

        /// <summary>
        /// Create Suriel's dialogue tree
        /// </summary>
        private void CreateSurielDialogue()
        {
            DialogueTree tree = new DialogueTree("suriel_captured", "Suriel", "suriel_intro");

            DialogueNode intro = new DialogueNode(
                "suriel_intro",
                "Suriel",
                "*in a crackling voice* You have captured me with ash wood. Ask your questions, mortal.",
                DialogueNodeType.Question
            );
            intro.AddChoice(new DialogueChoice("What is happening in Prythian?", "suriel_prythian"));
            intro.AddChoice(new DialogueChoice("How can I break the curse?", "suriel_curse"));
            intro.AddChoice(new DialogueChoice("Tell me about the High Lords.", "suriel_lords"));
            intro.AddChoice(new DialogueChoice("[Release it]", "suriel_release"));
            tree.AddNode(intro);

            DialogueNode prythian = new DialogueNode(
                "suriel_prythian",
                "Suriel",
                "A great curse plagues the land. All of Prythian suffers under Amarantha's rule. Only true love can break it.",
                DialogueNodeType.End
            );
            tree.AddNode(prythian);

            DialogueNode curse = new DialogueNode(
                "suriel_curse",
                "Suriel",
                "Three trials you must face Under the Mountain. And a riddle you must solve. Love is the key.",
                DialogueNodeType.End
            );
            tree.AddNode(curse);

            DialogueNode lords = new DialogueNode(
                "suriel_lords",
                "Suriel",
                "Seven High Lords rule seven courts. But one is most powerful... The Lord of Night and Dreams.",
                DialogueNodeType.End
            );
            tree.AddNode(lords);

            DialogueNode release = new DialogueNode(
                "suriel_release",
                "Suriel",
                "*quickly disappears* Your kindness will be remembered, child of mortality.",
                DialogueNodeType.End
            );
            tree.AddNode(release);

            dialogueTrees[tree.treeId] = tree;
        }

        /// <summary>
        /// Start a dialogue with an NPC
        /// </summary>
        public bool StartDialogue(string dialogueTreeId, Character player, ReputationSystem repSystem = null)
        {
            if (!dialogueTrees.ContainsKey(dialogueTreeId))
            {
                Debug.LogWarning($"Dialogue tree not found: {dialogueTreeId}");
                return false;
            }

            this.player = player;
            this.reputationSystem = repSystem;
            currentDialogue = dialogueTrees[dialogueTreeId];
            currentNode = currentDialogue.GetRootNode();

            if (currentNode != null)
            {
                DisplayCurrentNode();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Select a dialogue choice
        /// v2.6.1: Enhanced with error handling and logging
        /// </summary>
        public void SelectChoice(int choiceIndex)
        {
            try
            {
                if (currentNode == null || currentNode.choices == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Dialogue", "No active dialogue or choices available");
                    return;
                }
                
                if (choiceIndex < 0 || choiceIndex >= currentNode.choices.Count)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Dialogue", 
                        $"Invalid choice index: {choiceIndex}, valid range: 0-{currentNode.choices.Count - 1}");
                    return;
                }

            DialogueChoice choice = currentNode.choices[choiceIndex];

            // Apply reputation impact
            if (choice.affectedCourt.HasValue && choice.reputationImpact != 0 && reputationSystem != null)
            {
                if (choice.reputationImpact > 0)
                {
                    reputationSystem.GainReputation(choice.affectedCourt.Value, choice.reputationImpact);
                }
                else
                {
                    reputationSystem.LoseReputation(choice.affectedCourt.Value, Mathf.Abs(choice.reputationImpact));
                }
            }

            // Start quest if specified
            if (!string.IsNullOrEmpty(choice.questToStart))
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, "Dialogue", 
                    $"Quest started: {choice.questToStart}");
                // Would integrate with QuestManager here
            }

            // Execute callback
            choice.onSelected?.Invoke();

            // Move to next node
            if (choice.targetNodeId == "END")
            {
                EndDialogue();
            }
            else
            {
                currentNode = currentDialogue.GetNode(choice.targetNodeId);
                if (currentNode != null)
                {
                    DisplayCurrentNode();
                }
                else
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Dialogue", 
                        $"Target node not found: {choice.targetNodeId}");
                    EndDialogue();
                }
            }
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, "Dialogue", 
                    $"Exception in SelectChoice({choiceIndex}): {ex.Message}\nStack: {ex.StackTrace}");
                EndDialogue();
            }
        }

        /// <summary>
        /// Continue to next node (for linear dialogue)
        /// </summary>
        public void Continue()
        {
            if (currentNode == null || string.IsNullOrEmpty(currentNode.nextNodeId))
            {
                EndDialogue();
                return;
            }

            if (currentNode.nextNodeId == "END")
            {
                EndDialogue();
            }
            else
            {
                currentNode = currentDialogue.GetNode(currentNode.nextNodeId);
                if (currentNode != null)
                {
                    DisplayCurrentNode();
                }
                else
                {
                    EndDialogue();
                }
            }
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, "Dialogue", 
                    $"Exception in Continue: {ex.Message}");
                EndDialogue();
            }
        }

        /// <summary>
        /// Display the current dialogue node
        /// </summary>
        private void DisplayCurrentNode()
        {
            if (currentNode == null) return;

            Debug.Log($"\n--- {currentNode.speakerName} ---");
            Debug.Log(currentNode.dialogueText);

            if (currentNode.choices.Count > 0)
            {
                Debug.Log("\nChoices:");
                for (int i = 0; i < currentNode.choices.Count; i++)
                {
                    Debug.Log($"  {i + 1}. {currentNode.choices[i].choiceText}");
                }
            }
            else if (!string.IsNullOrEmpty(currentNode.nextNodeId))
            {
                Debug.Log("\n[Press any key to continue]");
            }
        }

        /// <summary>
        /// End the current dialogue
        /// </summary>
        private void EndDialogue()
        {
            Debug.Log("\n--- Dialogue Ended ---\n");
            currentDialogue = null;
            currentNode = null;
        }

        /// <summary>
        /// Get available dialogue tree for NPC
        /// </summary>
        public DialogueTree GetDialogueTree(string treeId)
        {
            return dialogueTrees.ContainsKey(treeId) ? dialogueTrees[treeId] : null;
        }
    }
}
