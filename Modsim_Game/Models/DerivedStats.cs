namespace Modsim_Game.Models
{
    public class DerivedStats
    {
        // HP & SP
        public double MaxHp { get; set; }
        public double MaxSp { get; set; }
        
        // Regen
        public double HpRegen { get; set; }
        public double SpRegen { get; set; }

        // Battle Stats
        public int Atk { get; set; }
        public int RangedAtk { get; set; }
        public int MinMatk1 { get; set; }
        public int MinMatk2 { get; set; }
        
        public double Aspd { get; set; }
        public double CastReductionPercent { get; set; }
        
        public int Hit { get; set; }
        public int Flee { get; set; }
        
        public double Crit { get; set; }
        public double PerfectDodge { get; set; }
        
        // Defense
        public int Def { get; set; }
        public int Mdef { get; set; }
        
        // Weight
        public int WeightLimit { get; set; }
        
        // Job Stat Modifiers
        public int BonusStr { get; set; }
        public int BonusAgi { get; set; }
        public int BonusVit { get; set; }
        public int BonusInt { get; set; }
        public int BonusDex { get; set; }
        public int BonusLuk { get; set; }

        // Points
        public int RequiredStrNext { get; set; }
        public int RequiredAgiNext { get; set; }
        public int RequiredVitNext { get; set; }
        public int RequiredIntNext { get; set; }
        public int RequiredDexNext { get; set; }
        public int RequiredLukNext { get; set; }
        
        public int PointsRemaining { get; set; }
    }
}
