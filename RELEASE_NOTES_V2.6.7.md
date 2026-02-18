# ACOTAR Fantasy RPG - Release Notes v2.6.7

**Version**: 2.6.7  
**Release Date**: February 18, 2026  
**Type**: Base Game Enhancement Update  

---

## 🎮 What's New

### Cascade Combo System 🔥
**Make every hit count with powerful cascade bonuses!**

- **Trigger cascades** every 5 consecutive hits for massive bonuses
- **50% damage boost** on cascade attacks
- **25% bonus XP** when defeating enemies during cascades
- **Visual feedback** with ⚡ symbols in combat log
- **Unlimited potential** - keep the combo going for multiple cascades!

*Perfect for: Boss fights, extended combat, skilled players*

---

### Equipment Set Bonuses ⚔️
**Wear matching equipment for powerful stat boosts!**

- **2 pieces**: +10% to all stats
- **3 pieces**: +25% to all stats
- **4+ pieces**: +50% to all stats

**Planned Equipment Sets:**
- Spring Court Set: Nature-focused, balanced stats
- Night Court Set: Magic and agility emphasis
- Illyrian Set: Strength and physical power
- More sets coming with future content!

*Perfect for: Min-maxers, collectors, strategic players*

---

### Mana Cost Reduction 🔮
**Cast more spells with magical equipment!**

- **Flat reduction**: Reduce mana cost by fixed amounts
- **Percentage reduction**: Scale cost reduction with spell power
- **Mix and match**: Both types stack from all equipment
- **Build variety**: Enable mage-focused playstyles

**Example:**
- Base spell cost: 50 mana
- Equipment: -10 flat, -20% reduction
- Actual cost: 30 mana (50 * 0.8 - 10)

*Perfect for: Mage builds, spell-heavy playstyles, strategic combat*

---

### Optional Quest Objectives 📜
**Take on extra challenges for bonus rewards!**

- **Bonus XP and Gold** for completing all optional objectives
- **Challenge yourself** with difficult optional goals
- **Replayability** - try quests again for perfection
- **Already available** in Book 1 trials!

**Examples:**
- "Defeat the Wyrm without taking damage" (+100 XP, +50 Gold)
- "Defeat the Naga in under 10 turns" (+125 XP, +75 Gold)

*Perfect for: Completionists, challenge seekers, replayers*

---

### Enhanced Combat Visuals 🎨
**See the impact of every attack!**

- **Color-coded damage** by type (fire is red-orange, ice is cyan, etc.)
- **Brighter critical hits** stand out more
- **Cascade notifications** in bold gold text
- **New damage types**: Nature (green) and Death (gray) colors added

*Perfect for: Everyone - makes combat clearer and more exciting*

---

## 📊 Detailed Changes

### Combat System
- Added cascade combo tracking (triggers every 5 hits)
- Cascade bonus: 50% damage, 25% XP
- Enhanced combo display with ⚡ symbols
- New methods: GetTotalComboHits(), WasLastAttackCascade()
- Integrated with existing combo system (still caps at 5 for normal combo bonus)

### Equipment System  
- Items now have equipmentSetId field
- Set bonuses: 10%/25%/50% for 2/3/4 pieces
- Items can have manaCostReduction and manaCostReductionPercent
- Caps: 20 flat reduction, 50% percentage reduction
- Minimum spell cost: 1 mana

### Quest System
- Optional objectives now tracked per quest
- CompleteOptionalObjective() method for tracking
- Bonus rewards only given when ALL optional objectives complete
- Works with existing quest structure

### Visual System
- Enhanced damage color palette (8 distinct colors)
- Critical hits 30% brighter
- New ShowComboCascade() method
- Better visual hierarchy

---

## 🔧 Technical Details

### Configuration Constants

**GameConfig.CombatSettings:**
```
COMBO_CASCADE_THRESHOLD = 5      // Hits per cascade
COMBO_CASCADE_BONUS = 0.5f       // 50% damage
COMBO_CASCADE_XP_BONUS = 1.25f   // 25% XP
```

**GameConfig.Equipment:**
```
SET_BONUS_2_PIECES = 2
SET_BONUS_3_PIECES = 3
SET_BONUS_4_PIECES = 4
SET_BONUS_2_PIECE_MULTIPLIER = 1.1f   // 10%
SET_BONUS_3_PIECE_MULTIPLIER = 1.25f  // 25%
SET_BONUS_4_PIECE_MULTIPLIER = 1.5f   // 50%
MAX_FLAT_MANA_REDUCTION = 20
MAX_PERCENT_MANA_REDUCTION = 0.5f
```

### Files Modified
- CombatSystem.cs
- DamageNumbersUI.cs
- QuestManager.cs
- InventorySystem.cs
- ManaSystem.cs
- GameConfig.cs
- Quest.cs (data structure)

---

## ⚖️ Balance Changes

### Combat
- Cascade combos reward extended fights
- No change to base damage or difficulty
- Adds skill-based damage scaling

### Equipment
- Set bonuses provide meaningful upgrade path
- Encourages completing equipment sets
- Multiple sets supported (no penalty for mixing initially)

### Magic
- Mana reduction makes high-cost spells more viable
- Enables mage-focused builds
- Doesn't affect combat balance significantly

### Quests
- Optional objectives are truly optional
- Bonus rewards don't affect main progression
- Encourages replayability without blocking content

---

## 🐛 Bug Fixes

None required - all features are new additions

---

## 💾 Save Compatibility

✅ **100% Backward Compatible**

- Existing saves work perfectly
- New fields initialize to safe defaults
- No data migration required
- Can load old saves and continue playing

**Details:**
- Optional objectives default to empty lists
- Equipment without setId works normally
- Mana costs unchanged if no equipment bonuses
- Combo system backward compatible

---

## 🎯 Known Issues

None currently identified

---

## 📝 How to Use New Features

### Cascade Combos
1. Attack consecutively with same character
2. Watch combo counter increase
3. Every 5th hit triggers cascade (⚡ symbol)
4. Cascade attacks deal 50% bonus damage
5. Keep combo going for multiple cascades!

**Tips:**
- Don't switch characters mid-combo
- Avoid defending or fleeing
- Boss fights are perfect for cascades
- Miss or dodge resets combo

### Equipment Sets
1. Check equipment for set name (when available)
2. Equip 2+ pieces from same set
3. Set bonus automatically applied
4. Check character stats to see bonus
5. Mix sets for highest bonus

**Tips:**
- Focus on one set for maximum bonus
- 4-piece bonus is worth pursuing
- Sets planned for each court
- Check item descriptions for set info

### Mana Cost Reduction
1. Equip items with mana reduction
2. Cost automatically reduced when casting
3. Stacks from all equipment
4. Check combat log to see reduction
5. Build mage character for maximum benefit

**Tips:**
- Percentage reduction better for high-cost spells
- Flat reduction better for low-cost spells
- Combine both for best results
- Rings and amulets may have mana bonuses

### Optional Objectives
1. Start quest normally
2. Check quest log for optional objectives
3. Complete main objectives first
4. Complete optional objectives for bonus
5. Finish quest to collect bonus rewards

**Tips:**
- Optional objectives are challenging
- Can replay quests to complete optionals
- Bonuses shown in quest description
- ⭐ symbol indicates all optionals complete

---

## 🎮 Tips & Strategies

### For Combat:
- Build combos on tough enemies for cascade bonus
- Switch characters strategically to maintain DPS
- Use cascade timing for boss phases
- Defend if you need to reset combo

### For Equipment:
- Focus on completing one set first
- Don't sacrifice stats for set bonus if < 3 pieces
- Mana reduction great for mage builds
- Physical builds benefit more from set bonuses

### For Quests:
- Try optionals on first playthrough if skilled
- Return later with better gear for challenges
- Bonus XP helps with leveling
- Bonus gold helps with equipment purchases

---

## 🙏 Credits

**Development**: S3OPS  
**Testing**: Community Playtesters  
**Inspired by**: Sarah J. Maas's ACOTAR series  

---

## 📞 Support & Feedback

**Found a bug?** Please report on our GitHub issues page  
**Have feedback?** We'd love to hear your thoughts!  
**Enjoying the game?** Leave us a review!

---

## 🚀 What's Next

### Planned for v2.6.8:
- Quest preparation hint display in CombatUI
- Mid-quest progress notifications
- Equipment comparison tooltips
- Confirmation dialogs for critical actions

### Future Updates:
- More equipment sets (one per court)
- Additional optional objectives for all quests
- Achievement system integration
- Visual effects for set bonuses and cascades

---

*"To the stars who listen—and the dreams that are answered."*

**Version**: 2.6.7  
**Status**: Released  
**Download**: Available Now  
**Compatibility**: Windows, Mac, Linux

Thank you for playing ACOTAR Fantasy RPG!
