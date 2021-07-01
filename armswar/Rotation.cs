//------------------------------------------------------------------------------
// fakeuser580 custom rotations.
// NAME : warrior-master-v1
// CLASS : Warrior
// LAST EDITED : 6/22/2021
//------------------------------------------------------------------------------

namespace TartEngine.RotationManager
{
    using TartEngine;
    using TartEngine.API;
    using TartEngine.Helpers;
    using System;
    using System.IO;
    using System.Linq;
    using System.Drawing;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    
    
    public class Warrior_Example : Rotation
    {

        // Sunder target. Will use rage on sunder armor until this health % is met.
        // Disable by putting above 100 once you have mortal strike or other, more efficient rage dumps
        private static int SUNDER_ARMOR_TARGET = 70;

        // Enable Rending and maintaining the rend dot
        private static bool ENABLE_REND = true;

        private static bool ENABLE_SUNDER= false;

        // The health % at which we want to stop maintaining the rend dot
        private static int REND_HEALTH_THRESHOLD = 50;
        
        // Enable using execute as highest priority rage dump.
        private static bool ENABLE_EXECUTE = true;
        private static int EXECUTE_MIN_HEALTH = 5;

        // enable using whatever food or water you have in your inventory when OOC to attempt to recover mana and health
        private static bool ENABLE_EATING = true;

        // the target health and mana, in percent, that we'd like to achieve in OOC before continuing
        private static int OOC_HEALTH_TARGET = 80;

        // minimum rage for casting HS
        private static int HS_RAGE_THRESHOLD = 15;
        
        // the number, in percent, below our target OOC where we want to utilize foor or water to help regen faster
        // prevents wasting water and food for regen amounts that are relatively small
        // EXAMPLES
        // your OOC_MANA_TARGET is 80, and OOC_FOOD_TARGET_DIFF is 0, and you end a fight at 79%, the bot will drink water to regain 1% of mana before continuing
        // your OOC_MANA_TARGET is 80, and OOC_FOOD_TARGET_DIFF is 25, and you end a fight at 79%, the bot will wait for natural regen to gain 1% of mana before continuing
        // your OOC_MANA_TARGET is 80, and OOC_FOOD_TARGET_DIFF is 25, and you end a fight at 56%, the bot will wait for natural regen to gain 24% of mana before continuing
        // your OOC_MANA_TARGET is 80, and OOC_FOOD_TARGET_DIFF is 25, and you end a fight at 54%, the bot will drink water to regain 26% of mana before continuing
        // default setting is 25
        private static int OOC_FOOD_TARGET_DIFF = 10;


        // somewhat comprehensive list of regen items. useful if you want to enable OOC to eat/drink
        // eating/drinking will be prioritized left to right
        // we can't make the list completely comprehensive, as there is a limit to the number of items that you can register with the bot at once.
        // if a food or drink you want is not on thei list, remove an entry and replace it with the one you would like.
        private static List<string> FOOD_TYPES = new List<string>{ "Clefthoof Ribs", "Sporeggar Mushroom", "Bladespire Bagel", "Telaari Grapes", "Mag'har Mild Cheese", "Zangar Trout", "Smoked Talbuk Venison", "Sunspring Carp", "Zangar Caps", "Mag'har Grainbread", "Garadar Sharp", "Marsh Lichen", "Alterac Swiss", "Roasted Quail",  "Homemade Cherry Pie", "Snapvine Watermelon", "Red-speckled Mushroom", "Goldenbark Apple", "Mutton Chop", "Dwarven Mild", "Longjaw Mud Snapper", "Haunch of Meat", "Tough Jerky", "Shiny Red Apple", "Darnassian Bleu"};


        // to see what the bot is doing in combat logs, enable this.
        // don't leave on as it will spit a LOT of logs over a period of time
        private static bool DEBUG_ROTATION = false;
        
        public override void Initialize()
        {
            Expansion = WowExpac.BurningCrusade;
            FastTick = 650;
            SlowTick = 650;
            
            Logger.Write("=============================================================================================================================================================================================================================",Color.FromArgb(255, 128, 0));
            Logger.Write("=============================================================================================================================================================================================================================",Color.FromArgb(255, 128, 0));
            Logger.Write("=============================================================================================================================================================================================================================",Color.FromArgb(255, 128, 0));
            Logger.Write("=== fakeuser580 Advanced Warrior levelling combat rotation. Useful for all warrior characters. Please view the Rotation.cs in the 'fakeuser580 Warrior Master v1' folder to configure the rotation to your specification. ===", Color.FromArgb(255, 128, 0));
            Logger.Write("=============================================================================================================================================================================================================================",Color.FromArgb(255, 128, 0));
            Logger.Write("========================================================================================================= CONFIGS ===========================================================================================================",Color.FromArgb(255, 128, 0));
            Logger.Write("=============================================================================================================================================================================================================================",Color.FromArgb(255, 128, 0));
            Logger.Write(String.Format("SUNDER_ARMOR_TARGET = {0}", REND_HEALTH_THRESHOLD), Color.FromArgb(0, 128, 255));
            Logger.Write(String.Format("ENABLE_REND = {0}", ENABLE_REND), Color.FromArgb(0, 128, 255));
            Logger.Write(String.Format("REND_HEALTH_THRESHOLD = {0}", REND_HEALTH_THRESHOLD), Color.FromArgb(0, 128, 255));
            Logger.Write(String.Format("ENABLE_EXECUTE = {0}", ENABLE_EXECUTE), Color.FromArgb(0, 128, 255));


            Logger.Write(String.Format("ENABLE_EATING = {0}", ENABLE_EATING), Color.FromArgb(0, 128, 255));
            Logger.Write(String.Format("OOC_HEALTH_TARGET = {0}", OOC_HEALTH_TARGET), Color.FromArgb(0, 128, 255));
            Logger.Write(String.Format("OOC_FOOD_TARGET_DIFF = {0}", OOC_FOOD_TARGET_DIFF), Color.FromArgb(0, 128, 255));
            Logger.Write(String.Format("FOOD_TYPES = {0}", string.Join(",",FOOD_TYPES)), Color.FromArgb(0, 128, 255));
            Logger.Write("=============================================================================================================================================================================================================================",Color.FromArgb(255, 128, 0));
            Logger.Write("=============================================================================================================================================================================================================================",Color.FromArgb(255, 128, 0));
            Logger.Write("=============================================================================================================================================================================================================================",Color.FromArgb(255, 128, 0));

            
            // Active Spells;
            Spellbook.Add(new Spell("Battle Shout"));
            Spellbook.Add(new Spell("Charge"));
            Spellbook.Add(new Spell("Rend"));
            Spellbook.Add(new Spell("Heroic Strike"));
            Spellbook.Add(new Spell("Mortal Strike"));
            Spellbook.Add(new Spell("Whirlwind"));
            Spellbook.Add(new Spell("Sunder Armor"));
            Spellbook.Add(new Spell("Overpower"));
            Spellbook.Add(new Spell("Battle Stance"));
            Spellbook.Add(new Spell("Berserker Stance"));
            
            // Player Buffs;
            PlayerBuffs.Add(new Buff("Battle Shout"));
            PlayerBuffs.Add(new Buff("Food"));

            // food item list
            foreach(string food in FOOD_TYPES)
            {
                DebugLogging(String.Format("Configuring the following food type :: {0}", food), Color.FromArgb(0, 0, 0));
                Inventory.Add(new Item(food));
            }

            TargetDebuffs.Add(new Debuff("Rend"));
            
            
            Logger.Write(String.Format("====== Rotation is utilizing {0}/60 spells, {1}/40 items, and {2}/24 macros ====== ", Spellbook.Count, Inventory.Count, Macros.Count), Color.FromArgb(255, 0, 0));
            
        }
        
        public override bool CombatTick()
        {
            DebugLogging("============================STARTTICK===========================", Color.FromArgb(0, 0, 0));
            DebugLogging(String.Format("Target found :: {0}.   Target health :: {1} out of  {2}", Burning.Target.ID(), Burning.Target.Health(), Burning.Target.MaxHealth()), Color.FromArgb(0, 0, 0));


            DebugLogging("Checking to see if we can cast charge.", Color.FromArgb(0, 128, 0));
            if (Burning.CanCast("Charge", false, true, true, true, true))
            {
                DebugLogging(String.Format("Checking to see if we should cast charge. Target Distance is {0}", Burning.Target.MinRange()), Color.FromArgb(0, 128, 0));
                if ( Burning.Target.MinRange() >= 8)
                {
                    DebugLogging("Target is far enough away. Charging and returning", Color.FromArgb(0, 128, 0));
                    // Burning.Cast("Battle Stance");
                    Burning.Cast("Charge");
                    // Burning.Cast("Berserker Stance");
                    return true;
                } else {
                    DebugLogging("Target is too close. Not charging. Continuing.", Color.FromArgb(0, 128, 0));
                }
            }

            DebugLogging("Checking to see if we can execute",  Color.FromArgb(0, 128, 0));
            if(Burning.CanCast("Execute", true, true, true, true, true))
            {
                DebugLogging("We can cast execute, checking to see if we should.", Color.FromArgb(0, 128, 0));
                if (ENABLE_EXECUTE && Burning.Player.Power(true) > 15 && Burning.Target.Health(true) > EXECUTE_MIN_HEALTH)
                {
                    DebugLogging("We should cast execute, casting execute and returning.", Color.FromArgb(0, 128, 0));

                } else {
                    DebugLogging("We should not execute. Either execute is disabled or we do not have the rage.", Color.FromArgb(0, 128, 0));
                }

            }

            // DebugLogging("Checking to see if we can cast overpower",  Color.FromArgb(0, 128, 0));
            // if (Burning.CanCast("Overpower", true, true, true, true, true))
            // {
            //     DebugLogging("We can cast overpowr, checking to see if we have the rage.", Color.FromArgb(0, 128, 0));
            //     if (Burning.Player.Power(true) >= 10) 
            //     {
            //         DebugLogging("We are casting overpower and returning.", Color.FromArgb(0, 128, 0));
            //         Burning.Cast("Overpower");
            //         return true;
            //     } else {
            //         DebugLogging("We don't have the rage to overpower. Continuing.", Color.FromArgb(0, 128, 0));
            //     }
            // }

            DebugLogging("Checking if we can cast battleshout",Color.FromArgb(0, 128, 0));
            if (Burning.Player.Power(true) >= 10)
            {
                DebugLogging("We havbe the rage to battleshout, checking to see if we should battleshout",Color.FromArgb(0, 128, 0));
                if (!Burning.HasBuff("Battle Shout", "Player"))
                {
                    DebugLogging("We do not have the battleshout buff, so we are castin battleshout and returning.",Color.FromArgb(0, 128, 0));
                    Burning.Cast("Battle Shout");
                    return true;
                } else {
                    DebugLogging("Battleshout is up. No reason to cast. Continuing.",Color.FromArgb(0, 128, 0));
                }
            }

            // DebugLogging("Checking if we can cast enrage",Color.FromArgb(0, 128, 0));
            // if (Burning.CanCast("Enrage", true, true, true, true, true)) 
            // {
            //     DebugLogging("We can cast enrage, ensuring we are in combat, our health is okay, and we aren't ragecapped before casting.",Color.FromArgb(0, 128, 0));
            //     if (Burning.Player.Health(true) > 50 && Burning.Player.InCombat() && Burning.Player.Power(true) < 90)
            //     {
            //         DebugLogging("Health is okay, we are in combat, and we are not ragecapped. Casting enrage and returning.",Color.FromArgb(0, 128, 0));
            //         Burning.Cast("Enrage");
            //         return true;
            //     } else {
            //         DebugLogging("We are either OOC, ragecapped, or our health is too low to cast battle rage. Continuing.",Color.FromArgb(0, 128, 0));
            //     }
            // }

            DebugLogging("Checking if we can cast rend",Color.FromArgb(0, 128, 0));
            if (Burning.CanCast("Rend", false, true, true, true, true) && Burning.Player.Power(true) >= 10)
            {
                DebugLogging("Checking if we should cast rend",Color.FromArgb(0, 128, 0));
                if ( ENABLE_REND && Burning.Target.Health(true) > REND_HEALTH_THRESHOLD   &&  !Burning.HasDebuff("Rend", "Target"))
                {
                    DebugLogging("We should cast rend. Rending and returning.", Color.FromArgb(0, 128, 0));
                    Burning.Cast("Rend");
                    return true;
                } else {
                    DebugLogging("We shouldn't cast rend. Either teh target is already rended, the target health is too low, or we've disabled rend as something to maintain.",Color.FromArgb(0, 128, 0));
                }
            }

            DebugLogging("Checking if we can cast mortal strike",Color.FromArgb(0, 128, 0));
            if (Burning.CanCast("Mortal Strike", false, true, true, true, true) && Burning.Player.Power(true) >= 30)
            {
                DebugLogging("We can and should should cast mortal strike. mortal striking and returning.", Color.FromArgb(0, 128, 0));
                Burning.Cast("Mortal Strike");
                return true;
            }

            // DebugLogging("Checking if we can cast Whirlwind",Color.FromArgb(0, 128, 0));
            // if (Burning.CanCast("Whirlwind", false, true, true, true, true) && Burning.Player.Power(true) >= 25)
            // {
            //     DebugLogging("We can and should cast Whirlwind. Whirlwinding and returning.", Color.FromArgb(0, 128, 0));
            //     Burning.Cast("Whirlwind");
            //     return true;
            // }

            // DebugLogging("Checking if we can cast Sunder Armor",Color.FromArgb(0, 128, 0));
            // if (Burning.CanCast("Sunder Armor", false, true, true, true, true) &&  Burning.Player.Power(true) >= 15)
            // {
            //     DebugLogging("We can cast Sunder Armor, checking to see if we should.",Color.FromArgb(0, 128, 0));
            //     if ( ENABLE_SUNDER && Burning.Target.Health(true) > SUNDER_ARMOR_TARGET)
            //     {
            //         DebugLogging("We should cast Sunder Armor, casting and returning",Color.FromArgb(0, 128, 0));
            //         Burning.Cast("Sunder Armor");
            //         return true;
            //     } else {
            //         DebugLogging("We should not cast Sunder Armor. Target health is above the sunder threshold. Continuing.",Color.FromArgb(0, 128, 0));
            //     }
            // }

            DebugLogging("Checking if we have enough rage left over to heroic strike.",Color.FromArgb(0, 128, 0));
            if (Burning.CanCast("Heroic Strike", false, true, true, true, true) &&  Burning.Player.Power(true) >= HS_RAGE_THRESHOLD)
            {     
                DebugLogging("We have excess rage. Heroic Striking and returning.",Color.FromArgb(0, 128, 0));          
                Burning.Cast("Heroic Strike");
                return true;
            }

            DebugLogging("No actions taken this tick.",Color.FromArgb(0, 128, 0)); 
            return false;
        }
        
        public override bool OutOfCombatTick()
        {
            if (!Burning.HasBuff("Food", "Player"))
            {
                DebugLogging("Checking if we want to eat.", Color.FromArgb(0, 0, 128));
                if ( ENABLE_EATING && !Burning.HasBuff("Food", "Player")  &&  Burning.Player.Health(true) < (OOC_HEALTH_TARGET - OOC_FOOD_TARGET_DIFF) )
                {
                    DebugLogging("We want to eat. Checking if we have food available.", Color.FromArgb(0, 0, 128));
                    string foundFood = doWeHaveItem(FOOD_TYPES);
                    if (!String.IsNullOrEmpty(foundFood))
                    {
                        DebugLogging(String.Format("Food Found {0}. Eating {0}", foundFood), Color.FromArgb(0, 0, 128));
                        Burning.Use(foundFood);
                        return true;
                    }
                }
            }

            return false;
        }

        private void DebugLogging(string debugLog, Color color){
            if (DEBUG_ROTATION) {
                Logger.Write(debugLog, color);
            }
        }

        private string doWeHaveItem(List<string> itemList)
        {
            string itemFound = "";
            foreach (string item in itemList) {
                if (Burning.ItemCount(item) > 0)
                {
                    itemFound = item;
                    break;
                }
            }
            return itemFound;
        }
        
        
        public override bool TickCompleted()
        {
            if ((OOC_HEALTH_TARGET - OOC_FOOD_TARGET_DIFF) > Burning.Player.Health(true)) return false;
            if (((OOC_HEALTH_TARGET - OOC_FOOD_TARGET_DIFF) < Burning.Player.Health(true)) && (Burning.Player.Health(true) < OOC_HEALTH_TARGET)) return !Burning.HasBuff("Food", "Player");
            return true;
        }
    }
}
