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
            
            // Food Recipes (Cooking Fire)
            AddFoodRecipes();
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
        /// Add food crafting recipes (Cooking Fire)
        /// </summary>
        private void AddFoodRecipes()
        {
            // Fae Bread
            CraftingRecipe faeBread = new CraftingRecipe(
                "cook_fae_bread",
                "Bake Fae Bread",
                "food_fae_bread",
                2
            );
            faeBread.AddMaterial("crafting_wheat", 3);
            faeBread.AddMaterial("crafting_honey", 1);
            faeBread.requiredStation = CraftingStationType.CookingFire;
            faeBread.requiredLevel = 1;
            faeBread.craftingTime = 10;
            recipes[faeBread.recipeId] = faeBread;

            // Roasted Meat
            CraftingRecipe roastedMeat = new CraftingRecipe(
                "cook_roasted_meat",
                "Cook Roasted Meat",
                "food_roasted_meat",
                1
            );
            roastedMeat.AddMaterial("crafting_raw_meat", 1);
            roastedMeat.AddMaterial("crafting_herbs", 1);
            roastedMeat.requiredStation = CraftingStationType.CookingFire;
            roastedMeat.requiredLevel = 1;
            roastedMeat.craftingTime = 8;
            recipes[roastedMeat.recipeId] = roastedMeat;

            // Vegetable Stew
            CraftingRecipe vegStew = new CraftingRecipe(
                "cook_vegetable_stew",
                "Prepare Vegetable Stew",
                "food_vegetable_stew",
                1
            );
            vegStew.AddMaterial("crafting_vegetables", 3);
            vegStew.AddMaterial("crafting_water", 1);
            vegStew.AddMaterial("crafting_herbs", 1);
            vegStew.requiredStation = CraftingStationType.CookingFire;
            vegStew.requiredLevel = 2;
            vegStew.craftingTime = 12;
            recipes[vegStew.recipeId] = vegStew;

            // Honey Cakes
            CraftingRecipe honeyCakes = new CraftingRecipe(
                "cook_honey_cakes",
                "Bake Honey Cakes",
                "food_honey_cakes",
                3
            );
            honeyCakes.AddMaterial("crafting_wheat", 2);
            honeyCakes.AddMaterial("crafting_honey", 2);
            honeyCakes.AddMaterial("crafting_butter", 1);
            honeyCakes.requiredStation = CraftingStationType.CookingFire;
            honeyCakes.requiredLevel = 2;
            honeyCakes.craftingTime = 15;
            recipes[honeyCakes.recipeId] = honeyCakes;

            // Herbal Tea
            CraftingRecipe herbalTea = new CraftingRecipe(
                "cook_herbal_tea",
                "Brew Herbal Tea",
                "food_herbal_tea",
                1
            );
            herbalTea.AddMaterial("crafting_herbs", 2);
            herbalTea.AddMaterial("crafting_water", 1);
            herbalTea.requiredStation = CraftingStationType.CookingFire;
            herbalTea.requiredLevel = 1;
            herbalTea.craftingTime = 5;
            recipes[herbalTea.recipeId] = herbalTea;

            // Fae Wine
            CraftingRecipe faeWine = new CraftingRecipe(
                "cook_fae_wine",
                "Ferment Fae Wine",
                "food_starlight_wine",
                1
            );
            faeWine.AddMaterial("crafting_grapes", 5);
            faeWine.AddMaterial("crafting_sugar", 2);
            faeWine.AddMaterial("crafting_moonwater", 1);
            faeWine.requiredStation = CraftingStationType.CookingFire;
            faeWine.requiredLevel = 3;
            faeWine.craftingTime = 20;
            recipes[faeWine.recipeId] = faeWine;

            // Forest Mushroom Soup
            CraftingRecipe mushroomSoup = new CraftingRecipe(
                "cook_mushroom_soup",
                "Cook Mushroom Soup",
                "food_mushroom_soup",
                1
            );
            mushroomSoup.AddMaterial("crafting_mushrooms", 4);
            mushroomSoup.AddMaterial("crafting_water", 1);
            mushroomSoup.AddMaterial("crafting_herbs", 2);
            mushroomSoup.requiredStation = CraftingStationType.CookingFire;
            mushroomSoup.requiredLevel = 2;
            mushroomSoup.craftingTime = 10;
            recipes[mushroomSoup.recipeId] = mushroomSoup;

            // Strength Stew (Buff Food)
            CraftingRecipe strengthStew = new CraftingRecipe(
                "cook_strength_stew",
                "Prepare Strength Stew",
                "food_strength_stew",
                1
            );
            strengthStew.AddMaterial("crafting_raw_meat", 2);
            strengthStew.AddMaterial("crafting_vegetables", 2);
            strengthStew.AddMaterial("crafting_power_herb", 1);
            strengthStew.requiredStation = CraftingStationType.CookingFire;
            strengthStew.requiredLevel = 4;
            strengthStew.craftingTime = 18;
            recipes[strengthStew.recipeId] = strengthStew;

            // Mage's Delight (Magic Buff Food)
            CraftingRecipe magesDelight = new CraftingRecipe(
                "cook_mages_delight",
                "Prepare Mage's Delight",
                "food_mages_delight",
                1
            );
            magesDelight.AddMaterial("crafting_mushrooms", 3);
            magesDelight.AddMaterial("crafting_moonwater", 1);
            magesDelight.AddMaterial("crafting_magic_herb", 2);
            magesDelight.requiredStation = CraftingStationType.CookingFire;
            magesDelight.requiredLevel = 4;
            magesDelight.craftingTime = 18;
            recipes[magesDelight.recipeId] = magesDelight;

            // Traveler's Rations
            CraftingRecipe rations = new CraftingRecipe(
                "cook_travelers_rations",
                "Pack Traveler's Rations",
                "food_travelers_rations",
                3
            );
            rations.AddMaterial("food_fae_bread", 1);
            rations.AddMaterial("crafting_dried_meat", 1);
            rations.AddMaterial("crafting_nuts", 1);
            rations.requiredStation = CraftingStationType.CookingFire;
            rations.requiredLevel = 1;
            rations.craftingTime = 5;
            recipes[rations.recipeId] = rations;

            Debug.Log("Initialized 10 cooking recipes");
        }

        /// <summary>
        /// <summary>
        /// Check if a recipe can be crafted
        /// </summary>
        /// <param name="recipeId">ID of the recipe to check</param>
        /// <returns>True if the recipe can be crafted, false otherwise</returns>
        /// <remarks>
        /// v2.6.3: Added comprehensive error handling, structured logging, and XML documentation
        /// 
        /// This method validates whether the player can craft a specific recipe by checking:
        /// 1. Recipe exists in the database
        /// 2. Player meets level requirements
        /// 3. Player has required materials in inventory
        /// 
        /// This is called before CraftItem to validate prerequisites. Error handling prevents
        /// null reference exceptions if player or inventory is not initialized.
        /// </remarks>
        public bool CanCraftRecipe(string recipeId)
        {
            try
            {
                // Input validation
                if (string.IsNullOrEmpty(recipeId))
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, 
                        "Crafting", "Cannot check recipe: recipeId is null or empty");
                    return false;
                }

                if (recipes == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                        "Crafting", "Cannot check recipe: recipes dictionary is null");
                    return false;
                }

                if (!recipes.ContainsKey(recipeId))
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, 
                        "Crafting", $"Recipe not found: {recipeId}");
                    return false;
                }

                CraftingRecipe recipe = recipes[recipeId];
                if (recipe == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                        "Crafting", $"Recipe database contains null recipe for: {recipeId}");
                    return false;
                }

                // Check player validity
                if (player == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                        "Crafting", "Cannot check recipe: player is null");
                    return false;
                }

                // Check level requirement
                if (player.level < recipe.requiredLevel)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, 
                        "Crafting", $"Level {recipe.requiredLevel} required to craft {recipe.recipeName} (player level: {player.level})");
                    return false;
                }

                // Check inventory validity
                if (playerInventory == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                        "Crafting", "Cannot check recipe: player inventory is null");
                    return false;
                }

                // Check materials
                foreach (var material in recipe.requiredMaterials)
                {
                    if (!playerInventory.HasItem(material.Key, material.Value))
                    {
                        LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, 
                            "Crafting", $"Missing materials for {recipe.recipeName}: {material.Key} x{material.Value}");
                        return false;
                    }
                }

                return true;
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                    "Crafting", $"Exception in CanCraftRecipe: {ex.Message}\nStack: {ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// Craft an item from a recipe
        /// </summary>
        /// <param name="recipeId">ID of the recipe to craft</param>
        /// <param name="availableStation">The crafting station available for use</param>
        /// <returns>True if crafting succeeded, false otherwise</returns>
        /// <remarks>
        /// v2.6.3: Added comprehensive error handling, structured logging, and XML documentation
        /// 
        /// This method crafts an item by:
        /// 1. Validating the recipe exists and station is correct
        /// 2. Checking prerequisites via CanCraftRecipe
        /// 3. Consuming required materials from inventory
        /// 4. Adding the crafted item to inventory
        /// 
        /// CRITICAL: Material consumption must be atomic - if adding the result fails,
        /// materials are still consumed. Future enhancement could add transaction rollback.
        /// </remarks>
        public bool CraftItem(string recipeId, CraftingStationType availableStation)
        {
            try
            {
                // Input validation
                if (string.IsNullOrEmpty(recipeId))
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, 
                        "Crafting", "Cannot craft item: recipeId is null or empty");
                    return false;
                }

                if (recipes == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                        "Crafting", "Cannot craft item: recipes dictionary is null");
                    return false;
                }

                if (!recipes.ContainsKey(recipeId))
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, 
                        "Crafting", $"Recipe not found: {recipeId}");
                    return false;
                }

                CraftingRecipe recipe = recipes[recipeId];
                if (recipe == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                        "Crafting", $"Recipe database contains null recipe for: {recipeId}");
                    return false;
                }

                // Check station requirement
                if (recipe.requiredStation != availableStation)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, 
                        "Crafting", $"Requires {recipe.requiredStation} to craft {recipe.recipeName} (available: {availableStation})");
                    return false;
                }

                // Check if can craft
                if (!CanCraftRecipe(recipeId))
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, 
                        "Crafting", $"Cannot craft {recipe.recipeName}: prerequisites not met");
                    return false;
                }

                // Validate inventory
                if (playerInventory == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                        "Crafting", "Cannot craft item: player inventory is null");
                    return false;
                }

                // Consume materials
                foreach (var material in recipe.requiredMaterials)
                {
                    if (!playerInventory.RemoveItem(material.Key, material.Value))
                    {
                        LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                            "Crafting", $"Failed to remove material: {material.Key} x{material.Value}");
                        // Note: Some materials may already be removed. Future: Add rollback mechanism
                    }
                }

                // Add result
                if (!playerInventory.AddItem(recipe.resultItemId, recipe.resultQuantity))
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                        "Crafting", $"Failed to add crafted item: {recipe.resultItemId} x{recipe.resultQuantity}");
                    return false;
                }

                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, 
                    "Crafting", $"Successfully crafted {recipe.recipeName}! Gained {recipe.resultQuantity} x {recipe.resultItemId}");

                return true;
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                    "Crafting", $"Exception in CraftItem: {ex.Message}\nStack: {ex.StackTrace}");
                return false;
            }
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
