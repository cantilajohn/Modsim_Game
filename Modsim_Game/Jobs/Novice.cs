using Modsim_Game.Models;
using System.Collections.Generic;

namespace Modsim_Game.Jobs
{
    public class Novice : IJobClass
    {
        public string Name => "Novice";
        public int BaseWeightLimit => 2000;
        public double SpJobModifier => 1.0;
        public string[] Skills => new[] { "Basic Skill", "First Aid", "Trick Dead" };
        public string[] AllowedWeapons => new[] { "Hand", "Dagger", "One-Handed-Sword", "One-Handed-Axe", "One-Handed-Mace", "Two-Handed-Mace", "Rod&Staff", "Two-Handed-Staff" };

        public int GetMaxHp(int baseLevel) => JobStatTable.GetMaxHP(Name, baseLevel);
        public int GetStatBonus(string stat, int jobLevel) => JobStatTable.GetBonus(Name, stat, jobLevel);

        public double GetWeaponDelay(string weaponName)
        {
            return weaponName switch
            {
                "Hand" => 1.0,
                "Dagger" => 1.3,
                "One-Handed-Sword" => 1.4,
                "One-Handed-Axe" => 1.6,
                "One-Handed-Mace" => 1.4,
                "Two-Handed-Mace" => 1.4,
                "Rod&Staff" => 1.3,
                "Two-Handed-Staff" => 1.3,
                _ => 1.0
            };
        }

        public JobSkillTree GetSkillTree()
        {
            var tree = new JobSkillTree
            {
                JobLabel = "Novice",
                Unlocked = new List<Skill>
                {
                    new Skill("Basic Skill", 0, 9, "passive"),
                    new Skill("First Aid",   1, 1, "quest", "active"),
                    new Skill("Trick Dead",  1, 1, "quest", "active"),
                },
                Locked = new List<LockedSkill>()
            };
            tree.AllLockedSkillsMaster.AddRange(tree.Locked);
            return tree;
        }
    }
}
