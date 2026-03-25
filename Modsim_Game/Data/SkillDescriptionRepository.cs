using System;
using System.Collections.Generic;
using System.Linq;
using Modsim_Game.Models;

namespace Modsim_Game.Data
{
    public static class SkillDescriptionRepository
    {
        private static readonly Dictionary<string, SkillDescription> _descriptions = new Dictionary<string, SkillDescription>(StringComparer.OrdinalIgnoreCase);

        static SkillDescriptionRepository()
        {
            // --- NOVICE ---
            Add("Basic Skill", "Passive", "n_basicSkills.png", 
                "Enable to apply Basic Interface Skills.", 
                (lv, hp, sp) => new List<SkillEffect> {
                    new SkillEffect { Label = "Lv 1", Value = "Enable Trade — exchange items with other characters.", Locked = lv < 1 },
                    new SkillEffect { Label = "Lv 2", Value = "Enable Emotions — express emotions using Alt+0~9.", Locked = lv < 2 },
                    new SkillEffect { Label = "Lv 3", Value = "Enable Sit — regenerate HP/SP 2× faster while sitting.", Locked = lv < 3 },
                    new SkillEffect { Label = "Lv 4", Value = "Enable Chat Room — create a chat room with Alt+C.", Locked = lv < 4 },
                    new SkillEffect { Label = "Lv 5", Value = "Enable Party — join a party.", Locked = lv < 5 },
                    new SkillEffect { Label = "Lv 6", Value = "Enable Kafra Storage — access an extra 300-slot inventory.", Locked = lv < 6 },
                    new SkillEffect { Label = "Lv 7", Value = "Enable /organize — create your own party.", Locked = lv < 7 },
                    new SkillEffect { Label = "Lv 9", Value = "Enable Job Change — allows changing into a 1st class profession.", Locked = lv < 9 }
                });

            Add("First Aid", "Active", "n_firstAid.png", "Heal yourself for 5 HP. Not a crazy powerful skill, but mages seem to like it for saving money on healing items.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "HP Restored", Value = "5" } });
            Add("Trick Dead", "Active", "n_playDead.png", "You lay on the ground like you were dead and aggressive monsters wont target you.\nYou cant recover HP or SP while pretending to be dead.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Status", Value = "Pretend Dead" } });

            // --- SWORDSMAN ---
            Add("Sword Mastery", "Passive, Physical", "sw_swordMastery.png", "Increases damage with Daggers and Swords (1-handed only) by 4*SkillLV. This damage ignores modification from Armor and VIT defense.", (lv, hp, sp) => new List<SkillEffect> { 
                new SkillEffect { Label = "Mastery ATK", Value = $"+{4 * lv}", Locked = false }
            });
            Add("Two-Handed Sword Mastery", "Passive, Physical", "sw_twoHandedSwordMastery.png", "Increases damage with Two-Handed Swords by 4*SkillLV. This damage ignores modification from Armor and VIT defense.", (lv, hp, sp) => new List<SkillEffect> { 
                new SkillEffect { Label = "Mastery ATK Bonus", Value = $"+{4 * lv}", Locked = false }
            });
            Add("Increase Recuperative Power", "Passive", "sw_increaseRecuperativePower.png", "Heals ((5*SkillLV) + (Maximum HP*0.002*SkillLV)) HP per 10 full seconds spent standing on one cell. Increases the effect of healing items by (10*SkillLV)%.", (lv, hp, sp) => new List<SkillEffect> {
                new SkillEffect { Label = "Standing Recovery", Value = $"{5 * lv} + {(hp * 0.002 * lv):F1} HP / 10s" },
                new SkillEffect { Label = "Healing Item Effectiveness", Value = $"+{10 * lv}%" }
            });
            Add("Bash", "Offensive, Physical", "sw_bash.png", "A melee attack with ATK equal to (100+30*SkillLV)%. There is a HIT bonus of 5*SkillLV.", (lv, hp, sp) => new List<SkillEffect> { 
                new SkillEffect { Label = "Damage", Value = $"{100 + 30 * lv}% ATK" },
                new SkillEffect { Label = "HIT Bonus", Value = $"+{5 * lv}" }
            });
            Add("Provoke", "Active", "sw_provoke.png", "Lowers the enemy DEF and VIT DEF by (5+5*SkillLV)% and increases their ATK by (2+3*SkillLV)%. Undead property and Boss monsters are not affected.", (lv, hp, sp) => new List<SkillEffect> {
                new SkillEffect { Label = "Success Chance", Value = $"{50 + 3 * lv}%" },
                new SkillEffect { Label = "Defense Reduction", Value = $"-{5 + 5 * lv}%" },
                new SkillEffect { Label = "Enemy ATK Increase", Value = $"+{2 + 3 * lv}%" }
            });
            Add("Moving HP Recovery", "Passive", "sw_movingHpRecovery.png", "Character regenerates HP while walking. Rate is 50% of standing recovery.", (lv, hp, sp) => new List<SkillEffect>());
            Add("Fatal Blow", "Passive, Physical", "sw_fatalBlow.png", "Adds chance of causing stun on target when using Bash level 6 or above. Base Stun Chance is 5%*(Bash SkillLV - 5).", (lv, hp, sp) => new List<SkillEffect> { 
                new SkillEffect { Label = "Base Stun Chance", Value = lv < 6 ? "0% (Needs Bash 6+)" : $"{5 * (lv - 5)}%" } 
            });
            Add("Auto Berserk", "Active, Physical", "sw_autoBerserk.png", "When your HP goes below 25%, you gain the effect of Provoke L10 on yourself.", (lv, hp, sp) => new List<SkillEffect>());
            Add("Magnum Break", "Active, Physical", "sw_magnumBreak.png", "5x5 cells, Fire property splash attack with ATK of (100+20*SkillLV)% and a +10*SkillLV bonus to HIT.", (lv, hp, sp) => new List<SkillEffect> {
                new SkillEffect { Label = "Splash Damage", Value = $"{100 + 20 * lv}% ATK" },
                new SkillEffect { Label = "HIT Bonus", Value = $"+{10 * lv}" },
                new SkillEffect { Label = "Elemental Buff", Value = "+20% Fire ATK (10s)" }
            });
            Add("Endure", "Active, Physical", "sw_endure.png", "Makes character skip 'flinch' animation when hit. Provides a +1*SkillLV bonus to MDEF.", (lv, hp, sp) => new List<SkillEffect> { 
                new SkillEffect { Label = "Stay Duration", Value = $"{7 + 3 * lv} sec" },
                new SkillEffect { Label = "MDEF Bonus", Value = $"+{lv}" } 
            });

            // --- MAGICIAN ---
            Add("Increase Spiritual Power", "Passive", "mg_increaseSpiritualPower.png", "Recovers (Maximum SP/500 + 3)*SkillLV SP per 10 full seconds when standing still and increases the efficiency of SP recovering items by +2% per SkillLV.", (lv, hp, sp) => new List<SkillEffect> {
                new SkillEffect { Label = "SP / 10s still", Value = $"+{(int)((sp / 500.0 + 3) * lv)}" },
                new SkillEffect { Label = "SP Item Bonus", Value = $"+{2 * lv}%" }
            });
            Add("Sight", "Active", "mg_sight.png", "Nullifies the Hide, Tunnel Drive and Cloaking effects within range.", (lv, hp, sp) => new List<SkillEffect>());
            Add("Napalm Beat", "Offensive, Magic", "mg_napalmBeat.png", "Hits every Enemy in a 3x3 area around the target for an MATK of (70+10*SkillLV)% using Ghost Element. This damage is spread equally between all targets.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "MATK Damage", Value = $"{70 + 10 * lv}%" } });
            Add("Cold Bolt", "Active", "mg_coldBolt.png", "Hits the targeted enemy with 1 Water property Bolt per SkillLV for 1*MATK damage each.", (lv, hp, sp) => new List<SkillEffect> {
                new SkillEffect { Label = "Hits", Value = lv.ToString() },
                new SkillEffect { Label = "Total MATK", Value = $"{100 * lv}%" }
            });
            Add("Stone Curse", "Active", "mg_stoneCurse.png", "Attempts to inflict Stone Curse status on the target.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Success Rate", Value = $"{14 + 2 * lv}%" } });
            Add("Fire Bolt", "Offensive, Magic", "mg_fireBolt.png", "Hits the targeted enemy with 1 Fire Element Bolt per SkillLV for 1*MATK each.", (lv, hp, sp) => new List<SkillEffect> {
                new SkillEffect { Label = "Hits", Value = lv.ToString() },
                new SkillEffect { Label = "Total MATK", Value = $"{100 * lv}%" }
            });
            Add("Lightning Bolt", "Offensive, Magic", "mg_lightningBolt.png", "Hits the targeted enemy with 1 Wind Element Bolt per SkillLV for 1*MATK each.", (lv, hp, sp) => new List<SkillEffect> {
                new SkillEffect { Label = "Hits", Value = lv.ToString() },
                new SkillEffect { Label = "Total MATK", Value = $"{100 * lv}%" }
            });
            Add("Energy Coat", "Active, Magic", "mg_energyCoat.png", "Reduces damage from Physical attacks by draining SP. Damage reduction is better and SP lost is higher with higher SP.", (lv, hp, sp) => new List<SkillEffect>());
            Add("Soul Strike", "Offensive, Magic", "mg_soulStrike.png", "Hits the target with (1+SkillLV/2) bolts for 1*MATK using Ghost Element. Does extra 5% damage per SkillLV to Undead property Monsters.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Spirit Bolts", Value = ((int)(1 + lv / 2.0)).ToString() } });
            Add("Frost Diver", "Offensive, Magic", "mg_frostDiver.png", "Hits the target for an MATK of (100+10*SkillLV)% Water Element. In addition, has a (35+3*SkillLV)% chance of causing the Frozen status.", (lv, hp, sp) => new List<SkillEffect> {
                new SkillEffect { Label = "MATK Damage", Value = $"{100 + 10 * lv}%" },
                new SkillEffect { Label = "Freeze Chance", Value = $"{35 + 3 * lv}%" }
            });
            Add("Fire Ball", "Offensive, Magic", "mg_fireBall.png", "Hits every enemy in a 5x5 area around the target with an MATK of (70+10*SkillLV)% and Fire Element.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "MATK Damage", Value = $"{70 + 10 * lv}%" } });
            Add("Fire Wall", "Offensive, Magic", "mg_fireWall.png", "Creates 3 cells of the Fire Wall effect. Each cell can deliver up to 4+SkillLV Fire Element hits at MATK*0.5.", (lv, hp, sp) => new List<SkillEffect> {
                new SkillEffect { Label = "Max Hits per cell", Value = (4 + lv).ToString() },
                new SkillEffect { Label = "Duration", Value = $"{4 + lv}s" }
            });
            Add("Thunder Storm", "Offensive, Magic", "mg_thunderStorm.png", "Hits every Enemy in a 5x5 area around the targeted cell with 1 Wind Element Bolt per level.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Total Bolts", Value = lv.ToString() }, new SkillEffect { Label = "MATK per bolt", Value = "80%" } } );
            Add("Safety Wall", "Active, Magic", "mg_safetyWall.png", "Creates a protective barrier on a cell that absorbs melee attacks.", (lv, hp, sp) => new List<SkillEffect> {
                new SkillEffect { Label = "Protected Hits", Value = (1 + lv).ToString() }
            });

            // --- ARCHER ---
            Add("Owl's Eye", "Passive", "ac_owlsEye.png", "Increases DEX, improving HIT rate, ranged ATK, and cast times.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "DEX Bonus", Value = $"+{lv}" } });
            Add("Double Strafing", "Offensive, Physical", "ac_doubleStrafing.png", "Ranged attack, that fires two arrows and hits with an ATK of (180+20*SkillLV)%. Requires a bow.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Damage ATK", Value = $"{180 + 20 * lv}%" } });
            Add("Making Arrow", "Active, Physical", "ac_makingArrow.png", "Creates arrows from an item. Different items give different amounts and types of arrows.", (lv, hp, sp) => new List<SkillEffect>());
            Add("Charge Arrow", "Active", "ac_chargeArrow.png", "Ranged attack at 150% ATK. The target is pushed back 6 cells.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Damage ATK", Value = "150%" }, new SkillEffect { Label = "Knockback", Value = "6 cells" } });
            Add("Vulture's Eye", "Passive", "ac_vulturesEye.png", "Increases range with bows by 1*SkillLV cells and increases HIT by 1 per SkillLV.", (lv, hp, sp) => new List<SkillEffect> {
                new SkillEffect { Label = "Range Bonus", Value = $"+{lv} cells" },
                new SkillEffect { Label = "HIT Bonus", Value = $"+{lv}" }
            });
            Add("Attention Concentrate", "Active, Physical", "ac_attentionConcentrate.png", "Increases DEX and AGI of the casting character by (2+1*SkillLV)%. Detects hidden characters in 3 cells range.", (lv, hp, sp) => new List<SkillEffect> {
                new SkillEffect { Label = "AGI/DEX Bonus", Value = $"+{2 + lv}%" }
            });
            Add("Arrow Shower", "Offensive, Physical", "ac_arrowShower.png", "3x3 cells, ranged splash attack with an ATK of (75+5*SkillLV)%. Enemies are pushed back 2 cells.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Damage ATK", Value = $"{75 + 5 * lv}%" } });

            // --- ACOLYTE ---
            Add("Divine Protection", "Passive, Physical", "al_divineProtection.png", "Reduces damage from Undead property and Demon family monsters by (3*SkillLV)+[0.04*(BaseLV + 1)].", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "DEF Bonus", Value = $"+{3 * lv}" } });
            Add("Ruwach", "Active, Magic", "al_ruwach.png", "Reveals Hiding and Cloaking players and monsters within range. Revealed targets take MATK*1.45 Holy damage.", (lv, hp, sp) => new List<SkillEffect>());
            Add("Heal", "Support / Buff, Magic", "al_heal.png", "Heals a targets HP for [(BaseLV+INT)/8]*(4+8*SkillLV). Deals Holy damage to Undead.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Multiplier", Value = (4 + 8 * lv).ToString() } });
            Add("Aqua Benedicta", "Active, Magic", "al_holyWater.png", "Creates 1 Holy Water. Caster must stand in water for the skill to succeed.", (lv, hp, sp) => new List<SkillEffect>());
            Add("Holy Light", "Offensive, Magic", "al_holyLight.png", "Does a single Holy element hit for 125% of your MATK.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Damage MATK", Value = "125%" } });
            Add("Demon Bane", "Passive, Physical", "al_demonBane.png", "Increases damage against Undead property and Demon family monsters by (3*SkillLV)+[0.05*(BaseLV + 1)].", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "ATK Bonus", Value = $"+{3 * lv}" } });
            Add("Teleportation", "Active, Magic", "al_teleportation.png", "Lv 1: Teleport to a random spot on the same map. Lv 2: Teleport to your save point.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Lv 1", Value = "Random", Locked = lv < 1 }, new SkillEffect { Label = "Lv 2", Value = "Save Point", Locked = lv < 2 } });
            Add("Warp Portal", "Active, Magic", "al_warpPortal.png", "Opens a portal to a saved location. Max capacity: 8 people.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Memo points", Value = lv.ToString() } });
            Add("Pneuma", "Active, Magic", "al_pneuma.png", "Creates a 3x3 cell cloud that blocks all ranged Physical attacks for 10 seconds.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Duration", Value = "10s" } });
            Add("Increase Agility", "Support / Buff, Magic", "al_increaseAgility.png", "Increases AGI of target by 2+SkillLV and increases movement speed by 25%. Dispels Decrease Agility.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "AGI Bonus", Value = $"+{2 + lv}" } });
            Add("Decrease Agility", "Active, Magic", "al_decreaseAgility.png", "Decreases AGI of target by 2+SkillLV and reduces movement speed by 25%.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "AGI Reduction", Value = $"-{2 + lv}" } });
            Add("Signum Crucis", "Active, Magic", "al_signumCrucis.png", "Reduces the DEF of Undead and Demon family monsters on screen by (10+4*SkillLV)%.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "DEF Reduction", Value = $"-{10 + 4 * lv}%" } });
            Add("Angelus", "Active", "al_angelus.png", "Increases the DEF from VIT of all party members on screen by (5*SkillLV)%.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "VIT DEF Bonus", Value = $"+{5 * lv}%" } });
            Add("Blessing", "Support / Buff, Magic", "al_blessing.png", "Increases STR, DEX and INT of target. Halves stats of Undead/Demon monsters.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Stat Bonus", Value = $"+{lv}" } });
            Add("Cure", "Support / Buff, Magic", "al_cure.png", "Cures Blind, Confusion and Silence.", (lv, hp, sp) => new List<SkillEffect>());

            // --- MERCHANT ---
            Add("Enlarge Weight Limit", "Passive", "mc_enlargeWeightLimit.png", "Increases maximum carrying capacity by 200*SkillLV.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Weight Limit", Value = $"+{200 * lv}" } });
            Add("Identify", "Active", "mc_identify.png", "Identifies an unidentified item. Must be in inventory (not cart).", (lv, hp, sp) => new List<SkillEffect>());
            Add("Mammonite", "Offensive, Physical", "mc_mammonite.png", "Uses 100z*SkillLV to increase ATK to (100+50*SkillLV)% for the next attack.", (lv, hp, sp) => new List<SkillEffect> { 
                new SkillEffect { Label = "Damage ATK", Value = $"{100 + 50 * lv}%" },
                new SkillEffect { Label = "Zeny Cost", Value = $"{100 * lv}z" }
            });
            Add("Cart Revolution", "Offensive, Physical", "mc_cartRevolution.png", "Does ATK*150% neutral-property damage to 3x3 area. Pushes back 2 cells.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Damage ATK", Value = "150%" }, new SkillEffect { Label = "Knockback", Value = "2 cells" } });
            Add("Change Cart", "Active", "mc_changeCart.png", "Lets you change the appearance of your cart based on base level.", (lv, hp, sp) => new List<SkillEffect>());
            Add("Loud Exclamation", "Active, Physical", "mc_loudExclamation.png", "Increases the caster's STR for a duration.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "STR Bonus", Value = "+4" } });
            Add("Cart Decoration", "Active", "mc_cartDecoration.png", "Change Pushcart appearance decoration.", (lv, hp, sp) => new List<SkillEffect>());
            Add("Discount", "Passive", "mc_discount.png", "Allows buying items at reduced prices from NPC shops by (3+2*SkillLV)%.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Price Reduc.", Value = $"-{3 + 2 * lv}%" } });
            Add("Overcharge", "Passive", "mc_overcharge.png", "Increases the sell price of items at NPC shops by (5+2*SkillLV)%.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Sell Bonus", Value = $"+{5 + 2 * lv}%" } });
            Add("Pushcart", "Passive", "mc_pushcart.png", "Allows using a Pushcart for storage. Restores movement speed by level.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Move Speed", Value = $"{50 + 5 * lv}%" } });
            Add("Vending", "Active", "mc_vending.png", "Allows the character to set up a shop. Items must be in the pushcart.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Item Slots", Value = (2 + lv).ToString() } });
            Add("Buying Store", "Active", "mc_buyingStore.png", "Enables the ability to open a purchase stall to buy various kinds of items.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Item Slots", Value = "5" } });

            // --- THIEF ---
            Add("Double Attack", "Passive, Physical", "tf_doubleAttack.png", "Gives chance to double swing a Dagger. Adds +1 HIT per SkillLV.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Double Chance", Value = $"{5 * lv}%" }, new SkillEffect { Label = "HIT Bonus", Value = $"+{lv}" } });
            Add("Increase Dodge", "Passive, Physical", "tf_increaseDodge.png", "Increases Flee Rate by +3*SkillLV. Higher bonus for Assassins/Rogues.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Flee Rate", Value = $"+{3 * lv}" } });
            Add("Steal", "Active, Physical", "tf_steal.png", "Attempts to steal an item from a monster.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Success Chance", Value = $"{10 + 6 * lv}%" } });
            Add("Envenom", "Active", "tf_envenom.png", "Adds 15*SkillLV to ATK and has a chance to inflict Poison status.", (lv, hp, sp) => new List<SkillEffect> { 
                new SkillEffect { Label = "ATK Bonus", Value = $"+{15 * lv}" },
                new SkillEffect { Label = "Poison Chance", Value = $"{5 + 4 * lv}%" }
            });
            Add("Sprinkle Sand", "Offensive, Physical", "tf_sprinkleSand.png", "130% ATK damage with a 20% chance to cause blind effect. Earth property.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Blind Chance", Value = "20%" }, new SkillEffect { Label = "Damage ATK", Value = "130%" } });
            Add("Back Sliding", "Active, Physical", "tf_backSliding.png", "Moves you backwards 5 cells instantly.", (lv, hp, sp) => new List<SkillEffect>());
            Add("Pick Stone", "Active", "tf_pickStone.png", "Adds one Stone item to your inventory.", (lv, hp, sp) => new List<SkillEffect>());
            Add("Throw Stone", "Offensive, Misc", "tf_throwStone.png", "Does 50 fixed damage and has a 3% chance of causing stun. Consumes 1 stone.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Fixed Damage", Value = "50" }, new SkillEffect { Label = "Stun Chance", Value = "3%" } });
            Add("Hiding", "Active", "tf_hiding.png", "Makes the character invisible to most players and monsters. Drains SP.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Duration", Value = $"{30 * lv}s" } });
            Add("Detoxify", "Support / Buff, Physical", "tf_detoxify.png", "Cures poison status on target.", (lv, hp, sp) => new List<SkillEffect>());
        }

        private static void Add(string name, string type, string iconFileName, string desc, SkillEffectCalculator calc)
        {
            _descriptions[name] = new SkillDescription { 
                Name = name, 
                Type = type,
                IconPath = iconFileName, 
                Description = desc, 
                EffectCalculator = calc 
            };
        }

        public static SkillDescription Get(string name)
        {
            if (_descriptions.TryGetValue(name, out var d)) return d;
            return new SkillDescription { Name = name, Description = "(No Description Available)" };
        }
    }
}
