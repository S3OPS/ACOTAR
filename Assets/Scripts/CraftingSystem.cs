using System;
using System.Collections.Generic;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Crafting station types
    /// </summary>
    public enum CraftingStationType
    {
        Workbench,      // Basic crafting
        Forge,          // Weapons and armor
        AlchemyTable,   // Potions and elixirs
        EnchantingTable, // Magical enhancements
        CookingFire     // Food and consumables
    }

    /// <summary>
    /// Recipe for crafting an item
    /// </summary>
    [Serializable]
    public class CraftingRecipe
    {
        public string recipeId;
        public string recipeName;
        public string resultItemId;
        public int resultQuantity;
        public Dictionary<string, int> requiredMaterials; // itemId -> quantity
        public CraftingStationType requiredStation;
        public int requiredLevel;
        public int craftingTime; // In seconds

        public CraftingRecipe(string id, string name, string resultItem, int quantity)
        {
            this.recipeId = id;
            this.recipeName = name;
            this.resultItemId = resultItem;
            this.resultQuantity = quantity;
            this.requiredMaterials = new Dictionary<string, int>();
            this.requiredStation = CraftingStationType.Workbench;
            this.requiredLevel = 1;
            this.craftingTime = 5;
        }

        /// <summary>
        /// Add a required material
        /// </summary>
        public void AddMaterial(string itemId, int quantity)
        {
            requiredMaterials[itemId] = quantity;
        }
    }

    /// <summary>
    /// Manages item crafting system
    /// Handles recipes, materials, crafting stations, and item creation
    /// </summary>
    public class CraftingSystem
    {
        private Dictionary<string, CraftingRecipe> recipes;
        private InventorySystem playerInventory;
        private Character player;

        public CraftingSystem(InventorySystem inventory, Character player)
        {
            this.playerInventory = inventory;
            this.player = player;
            this.recipes = new Dictionary<string, CraftingRecipe>();
            InitializeRecipes();
        }

        /// <summary>
        /// Initialize all crafting recipes
        /// </summary>
        private void InitializeRecipes()
        {
            // Weapon Recipes
            AddWeaponRecipes();
            
            // Potion Recipes
            AddPotionRecipes();
            
            // Armor Recipes
            AddArmorRecipes();
            
            // Magical Item Recipes
            AddMagicalRecipes();
        }

        /// <summary>
        /// Add weapon crafting recipes
        /// </summary>
        private void AddWeaponRecipes()
        {
            // Ash Wood Dagger
            CraftingRecipe ashDagger = new CraftingRecipe(
                "craft_ash_dagger",
                "Craft Ash Wood Dagger",
                "weapon_ash_dagger",
                1
            );
            ashDagger.AddMaterial("crafting_ash_wood", 3);
            ashDagger.AddMaterial("crafting_iron_ingot", 1);
            ashDagger.requiredStation = CraftingStationType.Forge;
            ashDagger.craftingTime = 10;
            recipes[ashDagger.recipeId] = ashDagger;

            // Illyrian Blade
            CraftingRecipe illyrianBlade = new CraftingRecipe(
                "craft_illyrian_blade",
                "Craft Illyrian Blade",
                "weapon_illyrian_blade",
                1
            );
            illyrianBlade.AddMaterial("crafting_illyrian_steel", 5);
            illyrianBlade.AddMaterial("crafting_leather", 2);
            illyrianBlade.requiredStation = CraftingStationType.Forge;
            illyrianBlade.requiredLevel = 5;
            illyrianBlade.craftingTime = 30;
            recipes[illyrianBlade.recipeId] = illyrianBlade;

            // Enchanted Fae Sword
            CraftingRecipe faeSword = new CraftingRecipe(
                "craft_fae_sword",
                "Craft Fae Sword",
                "weapon_fae_sword",
                1
            );
            faeSword.AddMaterial("crafting_moonstone", 2);
            faeSword.AddMaterial("crafting_mithril", 3);
            faeSword.AddMaterial("magical_fae_essence", 1);
            faeSword.requiredStation = CraftingStationType.EnchantingTable;
            faeSword.requiredLevel = 8;
            faeSword.craftingTime = 60;
            recipes[faeSword.recipeId] = faeSword;
        }

        /// <summary>
        /// Add potion crafting recipes
        /// </summary>
        private void AddPotionRecipes()
        {
            // Basic Healing Potion
            CraftingRecipe healingPotion = new CraftingRecipe(
                "craft_healing_potion",
                "Brew Healing Potion",
                "potion_healing",
                3
            );
            healingPotion.AddMaterial("crafting_healing_herb", 5);
            healingPotion.AddMaterial("crafting_water_vial", 3);
            healingPotion.requiredStation = CraftingStationType.AlchemyTable;
            healingPotion.craftingTime = 15;
            recipes[healingPotion.recipeId] = healingPotion;

            // Magic Elixir
            CraftingRecipe magicElixir = new CraftingRecipe(
                "craft_magic_elixir",
                "Brew Magic Elixir",
                "potion_magic",
                2
            );
            magicElixir.AddMaterial("crafting_moonflower", 3);
            magicElixir.AddMaterial("crafting_starlight_essence", 1);
            magicElixir.AddMaterial("crafting_water_vial", 2);
            magicElixir.requiredStation = CraftingStationType.AlchemyTable;
            magicElixir.requiredLevel = 3;
            magicElixir.craftingTime = 20;
            recipes[magicElixir.recipeId] = magicElixir;

            // Suriel's Blessing Potion
            CraftingRecipe surielPotion = new CraftingRecipe(
                "craft_suriel_blessing",
                "Brew Suriel's Blessing",
                "magical_suriel_blessing",
                1
            );
            surielPotion.AddMaterial("crafting_suriel_feather", 2);
            surielPotion.AddMaterial("crafting_moonflower", 5);
            surielPotion.AddMaterial("crafting_starlight_essence", 2);
            surielPotion.requiredStation = CraftingStationType.AlchemyTable;
            surielPotion.requiredLevel = 7;
            surielPotion.craftingTime = 45;
            recipes[surielPotion.recipeId] = surielPotion;
        }

        /// <summary>
        /// Add armor crafting recipes
        /// </summary>
        private void AddArmorRecipes()
        {
            // Illyrian Leathers
            CraftingRecipe illyrianArmor = new CraftingRecipe(
                "craft_illyrian_leathers",
                "Craft Illyrian Leathers",
                "armor_illyrian_leathers",
                1
            );
            illyrianArmor.AddMaterial("crafting_leather", 10);
            illyrianArmor.AddMaterial("crafting_iron_ingot", 3);
            illyrianArmor.requiredStation = CraftingStationType.Forge;
            illyrianArmor.requiredLevel = 4;
            illyrianArmor.craftingTime = 40;
            recipes[illyrianArmor.recipeId] = illyrianArmor;

            // Fae Robes
            CraftingRecipe faeRobes = new CraftingRecipe(
                "craft_fae_robes",
                "Craft Fae Robes",
                "armor_fae_robes",
                1
            );
            faeRobes.AddMaterial("crafting_silk", 8);
            faeRobes.AddMaterial("magical_fae_essence", 2);
            faeRobes.requiredStation = CraftingStationType.Workbench;
            faeRobes.requiredLevel = 5;
            faeRobes.craftingTime = 30;
            recipes[faeRobes.recipeId] = faeRobes;
        }

        /// <summary>
        /// Add magical item recipes
        /// </summary>
        private void AddMagicalRecipes()
        {
            // Glamour Stone
            CraftingRecipe glamourStone = new CraftingRecipe(
                "craft_glamour_stone",
                "Enchant Glamour Stone",
                "magical_glamour_stone",
                1
            );
            glamourStone.AddMaterial("crafting_moonstone", 3);
            glamourStone.AddMaterial("magical_fae_essence", 3);
            glamourStone.requiredStation = CraftingStationType.EnchantingTable;
            glamourStone.requiredLevel = 6;
            glamourStone.craftingTime = 50;
            recipes[glamourStone.recipeId] = glamourStone;

            // Protection Amulet
            CraftingRecipe amulet = new CraftingRecipe(
                "craft_protection_amulet",
                "Enchant Protection Amulet",
                "magical_protection_amulet",
                1
            );
            amulet.AddMaterial("crafting_silver", 2);
            amulet.AddMaterial("crafting_moonstone", 1);
            amulet.AddMaterial("magical_fae_essence", 1);
            amulet.requiredStation = CraftingStationType.EnchantingTable;
            amulet.requiredLevel = 4;
            amulet.craftingTime = 35;
            recipes[amulet.recipeId] = amulet;
        }

        /// <summary>
        /// Check if a recipe can be crafted
        /// </summary>
        public bool CanCraftRecipe(string recipeId)
        {
            if (!recipes.ContainsKey(recipeId))
            {
                Debug.LogWarning($"Recipe not found: {recipeId}");
                return false;
            }

            CraftingRecipe recipe = recipes[recipeId];

            // Check level requirement
            if (player.level < recipe.requiredLevel)
            {
                Debug.LogWarning($"Level {recipe.requiredLevel} required to craft {recipe.recipeName}");
                return false;
            }

            // Check materials
            foreach (var material in recipe.requiredMaterials)
            {
                if (!playerInventory.HasItem(material.Key, material.Value))
                {
                    Debug.LogWarning($"Missing materials: {material.Key} x{material.Value}");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Craft an item from a recipe
        /// </summary>
        public bool CraftItem(string recipeId, CraftingStationType availableStation)
        {
            if (!recipes.ContainsKey(recipeId))
            {
                Debug.LogWarning($"Recipe not found: {recipeId}");
                return false;
            }

            CraftingRecipe recipe = recipes[recipeId];

            // Check station requirement
            if (recipe.requiredStation != availableStation)
            {
                Debug.LogWarning($"Requires {recipe.requiredStation} to craft {recipe.recipeName}");
                return false;
            }

            // Check if can craft
            if (!CanCraftRecipe(recipeId))
            {
                return false;
            }

            // Consume materials
            foreach (var material in recipe.requiredMaterials)
            {
                playerInventory.RemoveItem(material.Key, material.Value);
            }

            // Add result
            playerInventory.AddItem(recipe.resultItemId, recipe.resultQuantity);

            Debug.Log($"Successfully crafted {recipe.recipeName}!");
            Debug.Log($"Gained {recipe.resultQuantity} x {recipe.resultItemId}");

            return true;
        }

        /// <summary>
        /// Get all recipes the player knows
        /// </summary>
        public List<CraftingRecipe> GetKnownRecipes()
        {
            // For now, all recipes are known
            // Could add recipe learning system later
            return new List<CraftingRecipe>(recipes.Values);
        }

        /// <summary>
        /// Get recipes for a specific crafting station
        /// </summary>
        public List<CraftingRecipe> GetRecipesForStation(CraftingStationType station)
        {
            List<CraftingRecipe> stationRecipes = new List<CraftingRecipe>();
            foreach (var recipe in recipes.Values)
            {
                if (recipe.requiredStation == station)
                {
                    stationRecipes.Add(recipe);
                }
            }
            return stationRecipes;
        }

        /// <summary>
        /// Get recipe by ID
        /// </summary>
        public CraftingRecipe GetRecipe(string recipeId)
        {
            return recipes.ContainsKey(recipeId) ? recipes[recipeId] : null;
        }

        /// <summary>
        /// Display all available recipes
        /// </summary>
        public void DisplayRecipes()
        {
            Debug.Log("\n=== Crafting Recipes ===");
            foreach (var recipe in recipes.Values)
            {
                bool canCraft = CanCraftRecipe(recipe.recipeId);
                string status = canCraft ? "[CAN CRAFT]" : "[CANNOT CRAFT]";
                
                Debug.Log($"\n{status} {recipe.recipeName}");
                Debug.Log($"  Station: {recipe.requiredStation}");
                Debug.Log($"  Level Required: {recipe.requiredLevel}");
                Debug.Log($"  Crafting Time: {recipe.craftingTime}s");
                Debug.Log($"  Result: {recipe.resultItemId} x{recipe.resultQuantity}");
                Debug.Log("  Materials:");
                foreach (var mat in recipe.requiredMaterials)
                {
                    bool hasMaterial = playerInventory.HasItem(mat.Key, mat.Value);
                    string matStatus = hasMaterial ? "✓" : "✗";
                    Debug.Log($"    {matStatus} {mat.Key} x{mat.Value}");
                }
            }
            Debug.Log("=======================\n");
        }
    }
}
