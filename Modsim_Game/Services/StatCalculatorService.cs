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

            int tStr = charInfo.Str + stats.BonusStr;
            int tAgi = charInfo.Agi + stats.BonusAgi;
            int tVit = charInfo.Vit + stats.BonusVit;
            int tInt = charInfo.Int + stats.BonusInt;
            int tDex = charInfo.Dex + stats.BonusDex;
            int tLuk = charInfo.Luk + stats.BonusLuk;

            // HP & SP Calculation
            int tableBaseHP = jobClass.GetMaxHp(charInfo.BaseLevel);
            stats.MaxHp = Math.Floor(tableBaseHP * (1 + (tVit * 0.01)));

            int jobBaseSP = 10;
            if (jobClass.Name == "Acolyte") jobBaseSP = 15; // From Form1.cs defaults

            double BASE_SP = jobBaseSP + (charInfo.BaseLevel * jobClass.SpJobModifier);
            double MAX_SP = Math.Floor(BASE_SP * (1 + tInt * 0.01));
            stats.MaxSp = Math.Floor(MAX_SP);

            // Regen
            double hpr = 1.0 + Math.Floor(stats.MaxHp / 200.0);
            hpr += Math.Floor(tVit / 5.0);
            hpr = Math.Max(1.0, hpr);
            stats.HpRegen = hpr;

            stats.SpRegen = Math.Floor(stats.MaxSp / 100.0) + Math.Floor(tInt / 6.0) + 1.0;

            // Battle Stats
            stats.Atk = tStr + (int)Math.Pow(tStr / 10, 2) + (tDex / 5) + (tLuk / 5);
            stats.RangedAtk = tDex + (int)Math.Pow(tDex / 10, 2);
            stats.Hit = charInfo.BaseLevel + tDex;
            stats.Flee = charInfo.BaseLevel + tAgi;
            
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
            stats.Def = tVit;
            stats.Mdef = tInt;
            stats.WeightLimit = jobClass.BaseWeightLimit + (tStr * 30);

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
    }
}
