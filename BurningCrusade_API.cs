
public class Burning
{
    // Attempts to cast a spell by name.
    public static bool Cast(string Name);

    // Attempts to consume an item by name.
    public static bool Use(string Name);

    // Attempts to trigger a macro by name.
    public static bool Trigger(string Name);

    public static bool CanCast(string Spell, bool Moving, bool HasTarget, bool InRange, bool Cooldown, bool Casting);
    public static bool CanConsume(string Item, bool Count, bool Cooldown);

    // Returns the amount of units in a given range.
    // => MaxRange: Will ignore all nameplates that are outside this range
    // => ReactionType: *See Below* Ignores all units that dont match the selected reaction
    // => InCombat => Ignores all units in, or out of combat.
    public static int UnitsNearby(int MaxRange, ReactionType Reaction, bool InCombat);

    // Returns the amount of units in a given range.
    // => MaxRange: Will ignore all nameplates that are outside this range
    // => Neutral: counts all units at reaction type Neutral, or lower. Else starts at unfriendly.
    // => InCombat => Ignores all units in, or out of combat.
    public static int EnemyNearby(int MaxRange, bool Neutral = true, bool InCombat);

    // Returns the amount of units in a given range.
    // => MaxRange: Will ignore all nameplates that are outside this range
    // => Neutral: counts all units at reaction type Neutral, or higher. Else starts at friendly.
    // => InCombat => Ignores all units in, or out of combat.
    public static int FriendNearby(int MaxRange, bool Neutral = false, bool InCombat);

    // Returns true if the unit has the buff. Accepts Player and Target only.
    public static bool HasBuff(string Name, string Unit = "Player");

    // Returns true if the unit has the debuff. Accepts Player and Target only.
    public static bool HasDebuff(string Name, string Unit = "Player");

    // Returns the time remaining on a buff in milliseconds
    public static int BuffRemaining(string Name, string Unit);

    // Returns the time remaining on a debuff in milliseconds
    public static int DebuffRemaining(string Name, string Unit);

    // Returns true if your spell is in range of your target. False if no target exists, or otherwise.
    public static bool SpellInRange(string Name);

    // returns the minimum range in yards, needed for a spell to be used.
    public static int SpellMinRange(string Name);

    // returns the minimum range in yards, needed for a spell to be used.
    public static int SpellMaxRange(string Name);

    // returns true if the spell is being used. Always returns true on spells that are "Toggled" or summoned pets etc.
    public static bool SpellInUse(string Name);

    // Returns true if the spell is meant to damage player target, false otherwise
    public static bool SpellDoesHarm(string Name);

    // Returns true if the spell is meant to help player target, false otherwise
    public static bool SpellDoesHelp(string Name);

    // Returns number of spell stacks/charges. 0 if its a single use spell.
    public static int SpellCharges(string Name);

    // Returns the cooldown of a spell in milliseconds
    public static int SpellCooldown(string Name);

    // Returns true if you can use the item. Will return false on food/drink in combat, and flase on combat items while not in combat etc.
    public static bool CanUseItem(string Name);

    // Returns true if the item is in use. Has a VERY brief window to be flagged. Not worth using in most instances.
    public static bool ItemInUse(string Name);

    // Returns the minimum level needed to use an item. 0 if no level is required.
    public static int ItemMinLevel(string Name);

    // Returns the amount of an item in a stack. 
    public static int ItemCount(string Name);

    // Returns true if the 
    public static int ItemCooldown(string Name);

    // Returns true if the selected command is active, false otherwise.
    public static bool CommandActive(string Command);

    // Player Unit Calls
    public static PlayerUnit Player = new PlayerUnit();

    // Pet Unit Calls
    public static Unit Pet = new Unit();

    // Focus Unit Calls
    public static Unit Focus = new Unit();

    // Target Unit Calls
    public static Unit Target = new Unit();

    // Pet Target Unit Calls
    public static Unit PetTarget = new Unit();
}

public class PlayerUnit
{
    // Returns the players current class.
    public string Class();

    // Returns the players current level.
    public int Level();

    // Returns true if the player is alive, false otherwise.
    public bool IsAlive();

    // Returns true if the player is dead, but not a ghost, false otherwise.
    public bool IsDead();

    // Returns true if the player is dead, and a ghost, false otherwise.
    public bool IsGhost();

    // Returns true if the player is in combat.
    public bool InCombat();

    // returns the players maximum health up to 17 million.
    public int MaxHealth();

    // Percent = true: Returns the players health percent from 1 to 100, 0 if unit is dead.
    // Percent = false:  Returns the players health value from 1 to 17 million, 0 if unit is dead.
    public int Health(bool Percent = false);

    // Returns the players maximum power up to 17 million (Mana, Energy, Rage etc).
    public int MaxPower();

    // Percent = true: Returns the players power percent from 1 to 100, 0 if unit is dead.
    // Percent = false:  Returns the players power value from 1 to 17 million, 0 if unit is dead.
    public int Power(bool Percent = false);

    // Returns the players combat points, 0 if players class doenst use combo points.
    public int CombatPoints();

    // Returns true if the player is moving, flase otherwise.
    public bool IsMoving();

    // Returns true if the player has full control of their character, false otherwise (ie: Stunned, Charmed, incapacitated etc).
    public bool InControl();

    // returns true if the player is casting, false otherwise.
    public bool IsCasting();

    // returns the spell id of the player cast.
    public int CastID();

    // returns the amount of time until the player cast/channel completes in milliseconds.
    public int CastRemaining();
}

public class Unit
{
    // Gets wow unit ID. Returns 0 if nil, or unit does not exist.
    public int ID();

    // Returns true if the unit exists, false otherwise
    public bool Exists();

    // Returns reaction type of the unit. *See reactions below.
    public ReactionType Reaction();

    // Returns true if the unit is in combat, false otherwise
    public bool InCombat();

    // Returns true if the unit has a target, false otherwise. 
    public bool HasTarget();

    // Returns true if the selected unit is targeting the player.
    public bool TargetingMe();

    // Returns the units level from 1-70, 0 if unit does not exist.
    public int Level();

    // Returns the units maximum health (up to 17 million), 0 if unit does not exist.
    public int MaxHealth();

    // Percent = true: Returns the units health percent from 1 to 100, 0 if unit is dead.
    // Percent = false:  Returns the units health value from 1 to 17 million, 0 if unit is dead.
    public int Health(bool Percent = false);

    // Returns the units maximum power (mana, energy, rage etc) (up to 17 million), 0 if unit does not exist.
    public int MaxPower();

    // Percent = true: Returns the units power percent from 1 to 100, 0 if unit is dead.
    // Percent = false:  Returns the units power value from 1 to 17 million, 0 if unit is dead.
    public int Power(bool Percent = false);

    // returns true if the unit is moving, false otherwise.
    public bool IsMoving();

    // returns the estimated minimum range of the target in yards.
    public int MinRange();

    // Returns the estimated Maximum range of the unit in yards.
    // **This is the call that should be used for range checking.** //
    public int MaxRange();

    // Returns true if the unit is casting, false otherwise.
    public bool IsCasting();

    // Returns the spell id of the unit's cast.
    public int CastID();

    // Returns the amount of time until the unit's cast/channel completes in milliseconds.
    public int CastRemaining();
}

public enum ReactionType : short
{
    Nil,        // => 0
    Hated,      // => 1
    Hostile,    // => 2
    Unfriendly, // => 3
    Neutral,    // => 4
    Friendly,   // => 5
    Honored,    // => 6
    Revered,    // => 7
    Exhalted    // => 8
}

// NECCESSARY Usings.
using System;
using System.Collections.Generic;
using TartEngine.RotationManager;

namespace TartEngine // => Using tart namespace
{
    public class ExampleRotation : Rotation  // => Including Rotation abstract class
    {
        // Initialize is called on rotation load.
        // Register Spells, Items, auras etc etc here.
        public override void Initialize()
        {
            // Select an expansion to write to
            Expansion = WowExpac.BurningCrusade;

            FastTick = 350;
            SlowTick = 600;

            // Add spells to the spell book
            Spellbook.Add(new Spell("Name"));
            Spellbook.Add(new Spell("Name", "Rank"));

            // Add items to the inventory
            Inventory.Add(new Item("Name"));

            // Add macros to the tracker
            Macros.Add(new Macro("Name", "/run print(\\\"Hello!\\\");"));

            // Add player/target aura's to the tracker
            PlayerBuffs.Add(new Buff("Name"));
            PlayerDebuffs.Add(new Debuff("Name"));
            TargetBuffs.Add(new Buff("Name"));
            TargetDebuffs.Add(new Debuff("Name"));
            
            // Add custom commands to the tracker
            CustomCommands.Add("Command Name");
        }

        // Ticks while in combat.
        // If tick returns false, Waits for FastTick + 10-15 ms before ticking again
        // If tick returns true , waits for SlowTick + 35 - 50 ms before ticking again.
        public override bool CombatTick()
        {
            return false;
        }

        // Ticks while in combat.
        // If tick returns false, Waits for FastTick + 10-15 ms before ticking again
        // If tick returns true , waits for SlowTick + 35 - 50 ms before ticking again.
        // Ticks until TickCompleted returns true.
        public override bool OutOfCombatTick()
        {
            return false;
        }

        public override bool TickCompleted()
        {
            return true;
        }
    }
}