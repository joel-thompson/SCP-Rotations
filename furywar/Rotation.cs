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
        // Enable Rending and maintaining the rend dot
        private static bool ENABLE_REND = false;
        // The health % at which we want to stop maintaining the rend dot
        private static int REND_HEALTH_THRESHOLD = 70;

        // Enable hamstringing and maintaining the hamstring dot
        private static bool ENABLE_HAMSTRING = false;
        // The health % at which we want to start maintaining the hamstring
        private static int HAMSTRING_HEALTH_THRESHOLD = 30;
        
        // Enable using execute as highest priority rage dump.
        private static bool ENABLE_EXECUTE = true;
        // The health % at which we want to stop trying to execute
        private static int EXECUTE_MIN_HEALTH = 5;

        // minimum rage for casting HS or sunder for rage dump
        private static int RAGE_DUMP_THRESHOLD = 40;
        private static int RAGE_DUMP_HS_SUNDER_LINE = 70;

        // enable using whatever food or water you have in your inventory when OOC to attempt to recover mana and health
        private static bool ENABLE_EATING = true;

        private static bool ENABLE_FIRST_AID = true;
        private static string BANDAGE_NAME = "Netherweave Bandage";

        // the target health and mana, in percent, that we'd like to achieve in OOC before continuing
        private static int OOC_HEALTH_TARGET = 90;

        
        // the number, in percent, below our target OOC where we want to utilize foor or water to help regen faster
        // prevents wasting water and food for regen amounts that are relatively small
        // EXAMPLES
        // your OOC_MANA_TARGET is 80, and OOC_FOOD_TARGET_DIFF is 0, and you end a fight at 79%, the bot will drink water to regain 1% of mana before continuing
        // your OOC_MANA_TARGET is 80, and OOC_FOOD_TARGET_DIFF is 25, and you end a fight at 79%, the bot will wait for natural regen to gain 1% of mana before continuing
        // your OOC_MANA_TARGET is 80, and OOC_FOOD_TARGET_DIFF is 25, and you end a fight at 56%, the bot will wait for natural regen to gain 24% of mana before continuing
        // your OOC_MANA_TARGET is 80, and OOC_FOOD_TARGET_DIFF is 25, and you end a fight at 54%, the bot will drink water to regain 26% of mana before continuing
        // default setting is 25
        private static int OOC_FOOD_TARGET_DIFF = 25;


        // somewhat comprehensive list of regen items. useful if you want to enable OOC to eat/drink
        // eating/drinking will be prioritized left to right
        // we can't make the list completely comprehensive, as there is a limit to the number of items that you can register with the bot at once.
        // if a food or drink you want is not on thei list, remove an entry and replace it with the one you would like.
        private static List<string> FOOD_TYPES = new List<string>{ "Clefthoof Ribs", "Sporeggar Mushroom", "Bladespire Bagel", "Telaari Grapes", "Mag'har Mild Cheese", "Zangar Trout", "Smoked Talbuk Venison", "Sunspring Carp", "Zangar Caps", "Mag'har Grainbread", "Garadar Sharp", "Marsh Lichen", "Alterac Swiss", "Roasted Quail", "Dried King Bolete"};


        // to see what the bot is doing in combat logs, enable this.
        // don't leave on as it will spit a LOT of logs over a period of time
        private static bool DEBUG_ROTATION = false;

        public string stance = "battle";
        
        public override void Initialize()
        {
            Expansion = WowExpac.BurningCrusade;
            FastTick = 300;
            SlowTick = 600;
            
            Logger.Write("=============================================================================================================================================================================================================================",Color.FromArgb(255, 128, 0));
            Logger.Write("=============================================================================================================================================================================================================================",Color.FromArgb(255, 128, 0));
            Logger.Write("=============================================================================================================================================================================================================================",Color.FromArgb(255, 128, 0));
            Logger.Write("=== fakeuser580 Advanced Warrior levelling combat rotation. Useful for all warrior characters. Please view the Rotation.cs in the 'fakeuser580 Warrior Master v1' folder to configure the rotation to your specification. ===", Color.FromArgb(255, 128, 0));
            Logger.Write("=============================================================================================================================================================================================================================",Color.FromArgb(255, 128, 0));
            Logger.Write("========================================================================================================= CONFIGS ===========================================================================================================",Color.FromArgb(255, 128, 0));
            Logger.Write("=============================================================================================================================================================================================================================",Color.FromArgb(255, 128, 0));
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
            Spellbook.Add(new Spell("Rend"));
            Spellbook.Add(new Spell("Hamstring"));
            Spellbook.Add(new Spell("Heroic Strike"));
            Spellbook.Add(new Spell("Bloodthirst"));
            Spellbook.Add(new Spell("Whirlwind"));
            Spellbook.Add(new Spell("Sunder Armor"));
            Spellbook.Add(new Spell("Battle Stance"));
            Spellbook.Add(new Spell("Berserker Stance"));
            Spellbook.Add(new Spell("Execute"));
            Spellbook.Add(new Spell("Bloodrage"));
            Spellbook.Add(new Spell("Blood Fury"));
            Spellbook.Add(new Spell("Berserker Rage"));
            Spellbook.Add(new Spell("Charge"));
            
            // Player Buffs;
            PlayerBuffs.Add(new Buff("Battle Shout"));
            PlayerBuffs.Add(new Buff("Food"));
            PlayerBuffs.Add(new Buff("First Aid"));

            // player debuffs
            PlayerDebuffs.Add(new Debuff("Recently Bandaged"));

            // bandage
            Inventory.Add(new Item(BANDAGE_NAME));

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
            DebugLogging(String.Format("Target found :: {0}.   Target health :: {1}, rage: {2}, stance: {3}", Burning.Target.ID(), Burning.Target.Health(true), Burning.Player.Power(true), stance), Color.FromArgb(0, 0, 0));


            DebugLogging("Checking to see if we can execute",  Color.FromArgb(0, 128, 0));
            if(Burning.Target.Health(true) < 20)
            {
                DebugLogging("We can cast execute, checking to see if we should.", Color.FromArgb(0, 128, 0));
                if (ENABLE_EXECUTE && Burning.Player.Power(true) > 15 && Burning.Target.Health(true) > EXECUTE_MIN_HEALTH)
                {
                    DebugLogging("We should cast execute, casting execute and returning.", Color.FromArgb(0, 128, 0));
                    Burning.Cast("Execute");

                } else {
                    DebugLogging("We should not execute. Either execute is disabled or we do not have the rage.", Color.FromArgb(0, 128, 0));
                }
            } else
            {
                DebugLogging("did not execute, target health not under 20", Color.FromArgb(0, 128, 0));
            }

            // if out of combat, charge up, not a ton of rage - swap to battle
            if (!Burning.Player.InCombat() && Burning.Player.Power(true) <= 30 && Burning.SpellCooldown("Charge") == 0)
            {
                Burning.Cast("Battle Stance");
                stance = "battle";
                // return true;
            }

            // if in combat and battle stance - swap to zerk
            if (Burning.Player.InCombat())
            {
                Burning.Cast("Berserker Stance");
                stance = "berserker";
                // return true;
            }

            // if out of combat and in battle - try to charge
            if (!Burning.Player.InCombat() && stance == "battle" && Burning.CanCast("Charge", false, true, true, true, true))
            {
                DebugLogging(String.Format("Checking to see if we should cast charge. Target Distance is {0}", Burning.Target.MinRange()), Color.FromArgb(0, 128, 0));
                if (true) // placeholder due to range not being available
                {
                    DebugLogging("Target is far enough away. Charging and returning", Color.FromArgb(0, 128, 0));
                    Burning.Cast("Charge");
                    return true;
                } else {
                    DebugLogging("Target is too close. Not charging. Continuing.", Color.FromArgb(0, 128, 0));
                }
            }

            DebugLogging("Checking if we can cast battleshout",Color.FromArgb(0, 128, 0));
            if (Burning.Player.Power(true) >= 10)
            {
                DebugLogging("We have the rage to battleshout, checking to see if we should battleshout",Color.FromArgb(0, 128, 0));
                if (!Burning.HasBuff("Battle Shout", "Player"))
                {
                    DebugLogging("We do not have the battleshout buff, so we are castin battleshout and returning.",Color.FromArgb(0, 128, 0));
                    Burning.Cast("Battle Shout");
                    return true;
                } else {
                    DebugLogging("Battleshout is up. No reason to cast. Continuing.",Color.FromArgb(0, 128, 0));
                }
            }

            DebugLogging("Checking if we can cast Bloodthirst",Color.FromArgb(0, 128, 0));
            if (Burning.Player.Power(true) >= 30  && Burning.SpellCooldown("Bloodthirst") == 0)
            {
                DebugLogging("We can and should should cast Bloodthirst. bloodthirsting and returning.", Color.FromArgb(0, 128, 0));
                Burning.Cast("Bloodthirst");
                return true;
            } else 
            {
                DebugLogging("-- cannot cast bloodthirst", Color.FromArgb(0, 128, 0));
            }





            DebugLogging("Checking if we can cast Blood Fury",Color.FromArgb(0, 128, 0));
            if (Burning.Target.Health(true) > 60 && Burning.Player.Health(true) >= 20 && Burning.SpellCooldown("Blood Fury") == 0 && Burning.Player.InCombat())
            {
                Burning.Cast("Blood Fury");
                return true;
            } else 
            {
                DebugLogging("Blood Fury is not available. Continuing.",Color.FromArgb(0, 128, 0));
            }

            DebugLogging("Checking if we can cast bloodrage",Color.FromArgb(0, 128, 0));
            if (Burning.Target.Health(true) > 60 && Burning.Player.Health(true) >= 20 && Burning.SpellCooldown("Bloodrage") == 0 && stance == "berserker")
            {
                Burning.Cast("Bloodrage");
                return true;
            } else 
            {
                DebugLogging("Bloodrage is not available. Continuing.",Color.FromArgb(0, 128, 0));
            }

            DebugLogging("Checking if we can cast Berserker Rage",Color.FromArgb(0, 128, 0));
            if (Burning.Target.Health(true) > 60 && Burning.SpellCooldown("Berserker Rage") == 0 && Burning.Player.InCombat() && stance == "berserker")
            {
                Burning.Cast("Berserker Rage");
                return true;
            } else 
            {
                DebugLogging("Berserker Rage is not available. Continuing.",Color.FromArgb(0, 128, 0));
            }



            DebugLogging("Checking if we can cast Hamstring",Color.FromArgb(0, 128, 0));
            if (Burning.CanCast("Hamstring", false, true, true, true, true) && Burning.Player.Power(true) >= 10)
            {
                DebugLogging("Checking if we should cast Hamstring",Color.FromArgb(0, 128, 0));
                if ( ENABLE_HAMSTRING && Burning.Target.Health(true) < HAMSTRING_HEALTH_THRESHOLD   &&  !Burning.HasDebuff("Hamstring", "Target"))
                {
                    DebugLogging("We should cast Hamstring. Hamstringing and returning.", Color.FromArgb(0, 128, 0));
                    Burning.Cast("Hamstring");
                    return true;
                } else {
                    DebugLogging("We shouldn't cast Hamstring. Either teh target is already Hamstringed, the target health is too low, or we've disabled Hamstring as something to maintain.",Color.FromArgb(0, 128, 0));
                }
            }

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



            DebugLogging("Checking if we can cast Whirlwind",Color.FromArgb(0, 128, 0));
            if (Burning.Player.Power(true) >= 25  && Burning.SpellCooldown("Whirlwind") == 0 && Burning.Player.InCombat() && Burning.SpellCooldown("Bloodthirst") > 0 && stance == "berserker")
            {
                DebugLogging("We can and should cast Whirlwind. Whirlwinding and returning.", Color.FromArgb(0, 128, 0));
                Burning.Cast("Whirlwind");
                return true;
            } else {
                DebugLogging("cannot cast whirlwind",Color.FromArgb(0, 128, 0));
            }

            DebugLogging("Checking if we have enough rage to dump.",Color.FromArgb(0, 128, 0));
            if (Burning.Player.Power(true) >= RAGE_DUMP_THRESHOLD)
            {     
                if (Burning.SpellIsUsable("Sunder Armor") && Burning.Target.Health(true) >= RAGE_DUMP_HS_SUNDER_LINE)
                {     
                    DebugLogging("We have excess rage. Sundering and returning.",Color.FromArgb(0, 128, 0));          
                    Burning.Cast("Sunder Armor");
                    return true;
                } else if (Burning.SpellIsUsable("Heroic Strike"))
                {     
                    DebugLogging("We have excess rage. HSing and returning.",Color.FromArgb(0, 128, 0));          
                    Burning.Cast("Heroic Strike");
                    return true;
                }
            }

            DebugLogging("No actions taken this tick.",Color.FromArgb(0, 128, 0)); 
            return false;
        }
        
        public override bool OutOfCombatTick()
        {
            // if out of combat, charge up, not a ton of rage - swap to battle
            if (!Burning.Player.InCombat() && Burning.Player.Power(true) <= 30)
            {
                Burning.Cast("Battle Stance");
                stance = "battle";
                // return true;
            }

            // no recently bandaged
            // use bandage
            DebugLogging("Checking if we want to first aid.", Color.FromArgb(0, 0, 128));
            if (ENABLE_FIRST_AID && !Burning.Player.InCombat() && Burning.DebuffRemaining("Recently Bandaged", "Player") == 0 && Burning.Player.Health(true) < (OOC_HEALTH_TARGET - OOC_FOOD_TARGET_DIFF))
            {
                DebugLogging("-- Using first aid", Color.FromArgb(0, 0, 128));
                Burning.Use(BANDAGE_NAME);
            }

            // // just debugging - leaving in case it breaks again later
            if (Burning.DebuffRemaining("Recently Bandaged", "Player") == 0)
            {
                DebugLogging("++ is NOT recently bandaged ++", Color.FromArgb(0, 0, 128));
            } else
            {
                DebugLogging("++ is recently bandaged ++", Color.FromArgb(0, 0, 128));
            }

            // recently bandaged and no bangage currently
            // eat
            if (!Burning.HasBuff("Food", "Player") && (!ENABLE_FIRST_AID || Burning.DebuffRemaining("Recently Bandaged", "Player") > 0) && !Burning.HasBuff("First Aid", "Player")) // 
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
            if (Burning.Player.Health(true) < 100 && Burning.HasBuff("First Aid", "Player"))
            {
                return false;
            }
            if (((OOC_HEALTH_TARGET - OOC_FOOD_TARGET_DIFF) < Burning.Player.Health(true)) && (Burning.Player.Health(true) < OOC_HEALTH_TARGET)) return !Burning.HasBuff("Food", "Player") && !Burning.HasBuff("First Aid", "Player");
            return true;
        }
    }
}
