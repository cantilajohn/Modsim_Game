using Modsim_Game.Models;

namespace Modsim_Game.Jobs
{
    public interface IJobClass
    {
        string Name { get; }
        int BaseWeightLimit { get; }
        double SpJobModifier { get; }
        string[] Skills { get; }
        string[] AllowedWeapons { get; }

        int GetMaxHp(int baseLevel);
        int GetStatBonus(string stat, int jobLevel);
        double GetWeaponDelay(string weaponName);

        /// <summary>
        /// Returns a fresh JobSkillTree with the full unlocked/locked skill data for this job.
        /// </summary>
        JobSkillTree GetSkillTree();
    }
}
