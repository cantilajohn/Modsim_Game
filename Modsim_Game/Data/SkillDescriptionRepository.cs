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
            Add("Basic Skill", "n_basicSkills.png", 
                "A foundational skill that unlocks core game functions.", 
                (lv, hp, sp) => new List<SkillEffect> {
                    new SkillEffect { Label = "Lv 1", Value = "Enable Trade", Locked = lv < 1 },
                    new SkillEffect { Label = "Lv 2", Value = "Enable Emotions", Locked = lv < 2 },
                    new SkillEffect { Label = "Lv 3", Value = "Enable Sit (HP/SP 2x)", Locked = lv < 3 },
                    new SkillEffect { Label = "Lv 4", Value = "Enable Chat Room", Locked = lv < 4 },
                    new SkillEffect { Label = "Lv 5", Value = "Enable Party Join", Locked = lv < 5 },
                    new SkillEffect { Label = "Lv 6", Value = "Enable Kafra Storage", Locked = lv < 6 },
                    new SkillEffect { Label = "Lv 7", Value = "Enable Party Organize", Locked = lv < 7 },
                    new SkillEffect { Label = "Lv 9", Value = "Enable Job Change", Locked = lv < 9 }
                });

            Add("First Aid", "n_firstAid.png", "Restores a small amount of HP.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "HP Restored", Value = "5" } });
            Add("Trick Dead", "n_playDead.png", "Immune from all attacks while toggled On.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Toggable", Value = "On/Off" } });

            // --- SWORDSMAN ---
            Add("Sword Mastery", "sw_swordMastery.png", "Increases damage with Daggers and One-Handed Swords.", (lv, hp, sp) => new List<SkillEffect> { 
                new SkillEffect { Label = "ATK Bonus", Value = $"+{4 * lv}" }
            });
            Add("Two-Handed Sword Mastery", "sw_twoHandedSwordMastery.png", "Increases damage with Two-Handed Swords.", (lv, hp, sp) => new List<SkillEffect> { 
                new SkillEffect { Label = "ATK Bonus", Value = $"+{4 * lv}" }
            });
            Add("Increase Recuperative Power", "sw_increaseRecuperativePower.png", "Heals HP every 10s while still. Increases item healing.", (lv, hp, sp) => new List<SkillEffect> {
                new SkillEffect { Label = "HP / 10s still", Value = $"+{(int)(5 * lv + hp * 0.002 * lv)}" },
                new SkillEffect { Label = "Heal Item Bonus", Value = $"+{10 * lv}%" }
            });
            Add("Bash", "sw_bash.png", "Powerful melee damage.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "ATK", Value = $"{130 + 30 * lv}%" } });
            Add("Provoke", "sw_provoke.png", "Taunts enemy: ATK up, DEF down.", (lv, hp, sp) => new List<SkillEffect> {
                new SkillEffect { Label = "Enemy ATK +", Value = $"{2 + 3 * lv}%" },
                new SkillEffect { Label = "Enemy DEF -", Value = $"{5 + 5 * lv}%" }
            });
            Add("Moving HP Recovery", "sw_movingHpRecovery.png", "Allows HP recovery while moving.", (lv, hp, sp) => new List<SkillEffect>());
            Add("Fatal Blow", "sw_fatalBlow.png", "Gives Bash a chance to stun.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Stun Chance", Value = "5%" } });
            Add("Auto Berserk", "sw_autoBerserk.png", "Provoke activates if HP < 25%.", (lv, hp, sp) => new List<SkillEffect>());
            Add("Magnum Break", "sw_magnumBreak.png", "Fire AoE + Pushback.", (lv, hp, sp) => new List<SkillEffect> {
                new SkillEffect { Label = "ATK", Value = $"{200 + 10 * lv}%" },
                new SkillEffect { Label = "Fire Bonus", Value = $"+{20 + 5 * lv}%" }
            });
            Add("Endure", "sw_endure.png", "Prevents knockback, MDEF up.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "MDEF +", Value = $"+{lv}" } });

            // --- MAGICIAN ---
            Add("Increase Spiritual Power", "mg_increaseSpiritualPower.png", "Increases SP recovery and item healing.", (lv, hp, sp) => new List<SkillEffect> {
                new SkillEffect { Label = "SP / 10s still", Value = $"+{(int)((sp / 500.0 + 3) * lv)}" },
                new SkillEffect { Label = "SP Item Bonus", Value = $"+{2 * lv}%" }
            });
            Add("Sight", "mg_sight.png", "Reveals hidden enemies.", (lv, hp, sp) => new List<SkillEffect>());
            Add("Napalm Beat", "mg_napalmBeat.png", "Ghost magic 3x3 AoE.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "MATK", Value = $"{100 + 10 * lv}%" } });
            Add("Cold Bolt", "mg_coldBolt.png", "Water magic multiple hits.", (lv, hp, sp) => new List<SkillEffect> {
                new SkillEffect { Label = "Hits", Value = lv.ToString() },
                new SkillEffect { Label = "MATK", Value = $"{100}% x {lv}" }
            });
            Add("Stone Curse", "mg_stoneCurse.png", "Attempts to inflict Stone Curse.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Success", Value = $"{14 + 2 * lv}%" } });
            Add("Fire Bolt", "mg_fireBolt.png", "Fire magic multiple hits.", (lv, hp, sp) => new List<SkillEffect> {
                new SkillEffect { Label = "Hits", Value = lv.ToString() },
                new SkillEffect { Label = "MATK", Value = $"{100}% x {lv}" }
            });
            Add("Lightning Bolt", "mg_lightningBolt.png", "Wind magic multiple hits.", (lv, hp, sp) => new List<SkillEffect> {
                new SkillEffect { Label = "Hits", Value = lv.ToString() },
                new SkillEffect { Label = "MATK", Value = $"{100}% x {lv}" }
            });
            Add("Energy Coat", "mg_energyCoat.png", "SP reduces incoming damage.", (lv, hp, sp) => new List<SkillEffect>());
            Add("Soul Strike", "mg_soulStrike.png", "Ghost damage (extra vs Undead).", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Hits", Value = ((int)Math.Ceiling(lv / 2.0)).ToString() } });
            Add("Frost Diver", "mg_frostDiver.png", "Water damage + Freezing chance.", (lv, hp, sp) => new List<SkillEffect> {
                new SkillEffect { Label = "MATK", Value = $"{100 + 10 * lv}%" },
                new SkillEffect { Label = "Freeze", Value = $"{30 + lv}%" }
            });
            Add("Fire Ball", "mg_fireBall.png", "Fire 5x5 AoE damage.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "MATK", Value = $"{100 + 20 * lv}%" } });
            Add("Fire Wall", "mg_fireWall.png", "Wall of fire that burns enemies.", (lv, hp, sp) => new List<SkillEffect> {
                new SkillEffect { Label = "Hits", Value = (lv + 1).ToString() },
                new SkillEffect { Label = "Duration", Value = $"{5 + 5 * lv}s" }
            });
            Add("Thunder Storm", "mg_thunderStorm.png", "Wind 5x5 AoE damage.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "MATK", Value = $"{100 + 40 * lv}%" } });
            Add("Safety Wall", "mg_safetyWall.png", "Blocks melee attacks.", (lv, hp, sp) => new List<SkillEffect> {
                new SkillEffect { Label = "Blocked", Value = (lv + 1).ToString() },
                new SkillEffect { Label = "Duration", Value = $"{lv * 5}s" }
            });

            // --- ARCHER ---
            Add("Owl's Eye", "ac_owlsEye.png", "Increases DEX natively.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "DEX +", Value = $"+{lv}" } });
            Add("Double Strafing", "ac_doubleStrafing.png", "Fires two arrows at once.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "ATK", Value = $"{180 + 20 * lv}%" } });
            Add("Making Arrow", "ac_makingArrow.png", "Crafts arrows from materials.", (lv, hp, sp) => new List<SkillEffect>());
            Add("Charge Arrow", "ac_chargeArrow.png", "Pushback arrow shot.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "ATK", Value = "150%" } });
            Add("Vulture's Eye", "ac_vulturesEye.png", "Range and HIT bonus.", (lv, hp, sp) => new List<SkillEffect> {
                new SkillEffect { Label = "Range", Value = $"+{lv}" },
                new SkillEffect { Label = "HIT", Value = $"+{lv}" }
            });
            Add("Attention Concentrate", "ac_attentionConcentrate.png", "Multiplies HIT and FLEE.", (lv, hp, sp) => new List<SkillEffect> {
                new SkillEffect { Label = "HIT +", Value = $"{4 + 2 * lv}%" },
                new SkillEffect { Label = "FLEE +", Value = $"{4 + 2 * lv}%" }
            });
            Add("Arrow Shower", "ac_arrowShower.png", "Volley 3x3 AoE.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "ATK", Value = $"{150 + 10 * lv}%" } });

            // --- ACOLYTE ---
            Add("Divine Protection", "al_divineProtection.png", "DEF vs Undead/Demons.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "DEF +", Value = $"+{3 * lv}" } });
            Add("Ruwach", "al_ruwach.png", "Reveals hidden enemies.", (lv, hp, sp) => new List<SkillEffect>());
            Add("Heal", "al_heal.png", "Restores HP.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Base Heal", Value = $"{lv * 10 * 2}-{lv * 10 * 2.2}" } });
            Add("Aqua Benedicta", "al_aquaBenedicta.png", "Creates Holy Water.", (lv, hp, sp) => new List<SkillEffect>());
            Add("Holy Light", "al_holyLight.png", "Single holy MATK.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "MATK", Value = "125%" } });
            Add("Demon Bane", "al_demonBane.png", "ATK vs Undead/Demons.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "ATK +", Value = $"+{3 * lv}" } });
            Add("Teleportation", "al_teleportation.png", "Teleports character.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Lv 1", Value = "Random", Locked = lv < 1 }, new SkillEffect { Label = "Lv 2", Value = "Save Point", Locked = lv < 2 } });
            Add("Warp Portal", "al_warpPortal.png", "Opens location portal.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Saved Locations", Value = lv.ToString() } });
            Add("Pneuma", "al_pneuma.png", "Blocks ranged attacks.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Duration", Value = "10s" } });
            Add("Increase Agility", "al_increaseAgility.png", "AGI and Speed boost.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "AGI +", Value = $"+{3 + lv}" } });
            Add("Decrease Agility", "al_decreaseAgility.png", "Reduces target AGI/Speed.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "AGI -", Value = $"{3 + lv}" } });
            Add("Signum Crucis", "al_signumCrucis.png", "Reduces Demon/Undead DEF.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "DEF Reduction", Value = $"{10 + 4 * lv}%" } });
            Add("Angelus", "al_angelus.png", "Party soft DEF bonus.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Soft DEF", Value = $"{5 + 5 * lv}%" } });
            Add("Blessing", "al_blessing.png", "STR/DEX/INT bonus.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "STR/DEX/INT +", Value = $"+{lv}" } });
            Add("Cure", "al_cure.png", "Removes Silence/Blind/Chaos.", (lv, hp, sp) => new List<SkillEffect>());

            // --- MERCHANT ---
            Add("Enlarge Weight Limit", "mc_enlargeWeightLimit.png", "Weight capacity up.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Weight", Value = $"+{200 * lv}" } });
            Add("Identify", "mc_identify.png", "ID items without magnifier.", (lv, hp, sp) => new List<SkillEffect>());
            Add("Mammonite", "mc_mammonite.png", "Powerful Zeny-based attack.", (lv, hp, sp) => new List<SkillEffect> { 
                new SkillEffect { Label = "ATK", Value = $"{100 + 100 * lv}%" },
                new SkillEffect { Label = "Cost", Value = $"{100 * lv}z" }
            });
            Add("Cart Revolution", "mc_cartRevolution.png", "Cart-based AoE attack.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "ATK", Value = "150%" } });
            Add("Change Cart", "mc_changeCart.png", "Cart visuals variation.", (lv, hp, sp) => new List<SkillEffect>());
            Add("Loud Exclamation", "mc_loudExclamation.png", "STR + 4.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "STR", Value = "+4" } });
            Add("Cart Decoration", "mc_cartDecoration.png", "Decorates your cart.", (lv, hp, sp) => new List<SkillEffect>());
            Add("Discount", "mc_discount.png", "Buy items cheaper.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Discount", Value = $"{3 + 3 * lv}%" } });
            Add("Overcharge", "mc_overcharge.png", "Sell items for more.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Sell Bonus", Value = $"{5 + 2 * lv}%" } });
            Add("Pushcart", "mc_pushcart.png", "Allows cart for storage.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Capacity", Value = (3000 + 500 * lv).ToString() } });
            Add("Vending", "mc_vending.png", "Open a trade shop.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Slots", Value = (2 + lv).ToString() } });
            Add("Buying Store", "mc_buyingStore.png", "Open a buying shop.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Slots", Value = "5" } });

            // --- THIEF ---
            Add("Double Attack", "tf_doubleAttack.png", "Chance for double dagger hits.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Trigger", Value = $"{5 * lv}%" } });
            Add("Increase Dodge", "tf_increaseDodge.png", "Increases FLEE natively.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "FLEE +", Value = $"+{3 * lv}" } });
            Add("Steal", "tf_steal.png", "Steal items from monsters.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Success", Value = $"{10 + 6 * lv}%" } });
            Add("Envenom", "tf_envenom.png", "Poison ATK + Poison status.", (lv, hp, sp) => new List<SkillEffect> { 
                new SkillEffect { Label = "ATK", Value = $"+{15 * lv}" },
                new SkillEffect { Label = "Chance", Value = $"{5 + 4 * lv}%" }
            });
            Add("Sprinkle Sand", "tf_sprinkleSand.png", "Blinds enemy / reduces HIT.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "HIT Reduc", Value = "-10" } });
            Add("Back Sliding", "tf_backSliding.png", "Moves back 5 cells.", (lv, hp, sp) => new List<SkillEffect>());
            Add("Pick Stone", "tf_pickStone.png", "Pick up a stone.", (lv, hp, sp) => new List<SkillEffect>());
            Add("Throw Stone", "tf_throwStone.png", "Throws a stone.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Dmg", Value = "50" } });
            Add("Hiding", "tf_hiding.png", "Hides from enemies.", (lv, hp, sp) => new List<SkillEffect> { new SkillEffect { Label = "Duration", Value = $"{30 + 30 * lv}s" } });
            Add("Detoxify", "tf_detoxify.png", "Removes poison.", (lv, hp, sp) => new List<SkillEffect>());
        }

        private static void Add(string name, string iconFileName, string desc, SkillEffectCalculator calc)
        {
            _descriptions[name] = new SkillDescription { 
                Name = name, 
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
