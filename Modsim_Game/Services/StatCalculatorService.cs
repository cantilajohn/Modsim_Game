using System;
using Modsim_Game.Jobs;
using Modsim_Game.Models;

namespace Modsim_Game.Services
{
    public class StatCalculatorService
    {
        public DerivedStats Calculate(CharacterInfo charInfo, IJobClass jobClass)
        {
            var stats = new DerivedStats();

            // Get Job Bonuses
            stats.BonusStr = jobClass.GetStatBonus("STR", charInfo.JobLevel);
            stats.BonusAgi = jobClass.GetStatBonus("AGI", charInfo.JobLevel);
            stats.BonusVit = jobClass.GetStatBonus("VIT", charInfo.JobLevel);
            stats.BonusInt = jobClass.GetStatBonus("INT", charInfo.JobLevel);
            stats.BonusDex = jobClass.GetStatBonus("DEX", charInfo.JobLevel);
            stats.BonusLuk = jobClass.GetStatBonus("LUK", charInfo.JobLevel);

            // Skills Passive Bonuses (Standardized Names)
            int swordMasteryLv = GetSkillLevel(charInfo, "Sword Mastery");
            int twoHandSwordMasteryLv = GetSkillLevel(charInfo, "Two-Handed Sword Mastery");
            int weightLimitLv = GetSkillLevel(charInfo, "Increase Weight Limit");
            int hpRecoveryLv = GetSkillLevel(charInfo, "Increase HP Recovery");
            int spRecoveryLv = GetSkillLevel(charInfo, "Increase SP Recovery");
            int owlsEyeLv = GetSkillLevel(charInfo, "Owl's Eye");
            int vulturesEyeLv = GetSkillLevel(charInfo, "Vulture's Eye");
            int dodgeLv = GetSkillLevel(charInfo, "Improve Dodge");
            int divineProtLv = GetSkillLevel(charInfo, "Divine Protection");
            int demonBaneLv = GetSkillLevel(charInfo, "Demon Bane");
            int doubleAttackLv = GetSkillLevel(charInfo, "Double Attack");

            int tStr = charInfo.Str + stats.BonusStr; 
            int tAgi = charInfo.Agi + stats.BonusAgi;
            int tVit = charInfo.Vit + stats.BonusVit;
            int tInt = charInfo.Int + stats.BonusInt;
            int tDex = charInfo.Dex + stats.BonusDex + owlsEyeLv; 
            int tLuk = charInfo.Luk + stats.BonusLuk;

            // HP & SP Calculation
            int tableBaseHP = jobClass.GetMaxHp(charInfo.BaseLevel);
            stats.MaxHp = Math.Floor(tableBaseHP * (1 + (tVit * 0.01)));

            int jobBaseSP = 10;
            if (jobClass.Name == "Acolyte") jobBaseSP = 15; 

            double BASE_SP = jobBaseSP + (charInfo.BaseLevel * jobClass.SpJobModifier);
            double MAX_SP = Math.Floor(BASE_SP * (1 + tInt * 0.01));
            
            stats.MaxSp = Math.Floor(MAX_SP);

            // Regen
            double hpr = 1.0 + Math.Floor(stats.MaxHp / 200.0);
            hpr += Math.Floor(tVit / 5.0);
            if (hpRecoveryLv > 0) hpr += (5 * hpRecoveryLv) + (stats.MaxHp * 0.002 * hpRecoveryLv); // Formula: (5*lv) + (MaxHP * 0.002 * lv)
            hpr = Math.Max(1.0, hpr);
            stats.HpRegen = hpr;

            double spr = Math.Floor(stats.MaxSp / 100.0) + Math.Floor(tInt / 6.0) + 1.0;
            if (spRecoveryLv > 0) spr += (int)((stats.MaxSp / 500.0 + 3) * spRecoveryLv); // Formula: ((MaxSP / 500 + 3) * lv)
            stats.SpRegen = spr;

            // Battle Stats
            int atkBonus = 0;
            // Sword Mastery applies to One-Handed Swords AND Daggers
            if (charInfo.WeaponName == "One-Handed-Sword" || charInfo.WeaponName == "Dagger") 
                atkBonus += (swordMasteryLv * 4);
            
            if (charInfo.WeaponName == "Two-Handed-Sword") atkBonus += (twoHandSwordMasteryLv * 4);

            stats.Atk = tStr + (int)Math.Pow(tStr / 10, 2) + (tDex / 5) + (tLuk / 5) + atkBonus + (demonBaneLv * 3);
            stats.RangedAtk = tDex + (int)Math.Pow(tDex / 10, 2);
            stats.Hit = charInfo.BaseLevel + tDex + vulturesEyeLv + doubleAttackLv; // Passive Hit bonuses
            stats.Flee = charInfo.BaseLevel + tAgi + (dodgeLv * 3); // Increase Dodge adds Flee
            
            stats.MinMatk1 = tInt + (int)Math.Pow(tInt / 7, 2);
            stats.MinMatk2 = tInt + (int)Math.Pow(tInt / 5, 2);

            stats.Crit = Math.Floor((tLuk * 0.3) + 1);
            stats.PerfectDodge = tLuk * 0.1;

            stats.CastReductionPercent = Math.Min(100, (tDex / 150.0) * 100);

            // ASPD Calculation
            double btba = jobClass.GetWeaponDelay(charInfo.WeaponName);
            double wd = 50.0 * btba; // Weapon Delay
            double sm = 0.0;         // Speed Modifier

            double agiContrib = Math.Round((wd * tAgi) / 25.0);
            double dexContrib = Math.Round((wd * tDex) / 100.0);
            double finalASPD = 200.0 - (wd - (agiContrib + dexContrib) / 10.0) * (1.0 - sm);
            stats.Aspd = Math.Min(190.0, finalASPD); // Cap at 190 max

            // Defenses and Capacity
            double softDef = tVit;

            stats.Def = (int)Math.Floor(softDef) + (divineProtLv * 3); // Divine Protection adds soft DEF
            stats.Mdef = tInt;
            stats.WeightLimit = jobClass.BaseWeightLimit + (tStr * 30) + (weightLimitLv * 200);

            // Progression Tracker
            stats.RequiredStrNext = ProgressionService.GetRequiredPointsForNextLevel(charInfo.Str);
            stats.RequiredAgiNext = ProgressionService.GetRequiredPointsForNextLevel(charInfo.Agi);
            stats.RequiredVitNext = ProgressionService.GetRequiredPointsForNextLevel(charInfo.Vit);
            stats.RequiredIntNext = ProgressionService.GetRequiredPointsForNextLevel(charInfo.Int);
            stats.RequiredDexNext = ProgressionService.GetRequiredPointsForNextLevel(charInfo.Dex);
            stats.RequiredLukNext = ProgressionService.GetRequiredPointsForNextLevel(charInfo.Luk);

            int currentSpent = ProgressionService.CalculateStatCost(charInfo.Str) +
                               ProgressionService.CalculateStatCost(charInfo.Agi) +
                               ProgressionService.CalculateStatCost(charInfo.Vit) +
                               ProgressionService.CalculateStatCost(charInfo.Int) +
                               ProgressionService.CalculateStatCost(charInfo.Dex) +
                               ProgressionService.CalculateStatCost(charInfo.Luk);

            stats.PointsRemaining = ProgressionService.CalculateTotalAvailablePoints(charInfo.BaseLevel) - currentSpent;

            return stats;
        }

        private int GetSkillLevel(CharacterInfo charInfo, string skillName)
        {
            if (charInfo.SkillTree == null) return 0;
            var skill = charInfo.SkillTree.Unlocked.FirstOrDefault(s => string.Equals(s.Name, skillName, StringComparison.OrdinalIgnoreCase));
            return skill?.CurrentLevel ?? 0;
        }
    }
}
