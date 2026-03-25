using System;
using System.Collections.Generic;

namespace Modsim_Game.Models
{
    public class SkillEffect
    {
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public bool Locked { get; set; } = false;
    }

    public delegate List<SkillEffect> SkillEffectCalculator(int lv, int maxHP, int maxSP);

    public class SkillDescription
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string IconPath { get; set; } = string.Empty;

        /// <summary>
        /// A function that calculates a list of effects based on current level and stats.
        /// </summary>
        public SkillEffectCalculator? EffectCalculator { get; set; }
    }
}
