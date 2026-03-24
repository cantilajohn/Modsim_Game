using Modsim_Game.Models;
using System.Collections.Generic;

namespace Modsim_Game.Jobs
{
    public class Archer : IJobClass
    {
        public string Name => "Archer";
        public int BaseWeightLimit => 2300;
        public double SpJobModifier => 1.5;
        public string[] Skills => new[] { "Owl's Eye", "Vulture's Eye", "Double Strafing", "Attention Concentrate", "Arrow Shower", "Making Arrow", "Charge Arrow" };
        public string[] AllowedWeapons => new[] { "Hand", "Dagger", "Bow" };
        public int GetMaxHp(int baseLevel) => JobStatTable.GetMaxHP(Name, baseLevel);
        public int GetStatBonus(string stat, int jobLevel) => JobStatTable.GetBonus(Name, stat, jobLevel);

        public double GetWeaponDelay(string weaponName)
        {
            return weaponName switch
            {
                "Hand" => 0.8,
                "Dagger" => 1.1,
                "Bow" => 1.4,
                _ => 1.0
            };
        }

        public JobSkillTree GetSkillTree()
        {
            var tree = new JobSkillTree
            {
                JobLabel = "Archer",
                Unlocked = new List<Skill>
                {
                    new Skill("Owl's Eye",       0, 10, "passive"),
                    new Skill("Double Strafing", 0, 10, "active"),
                    new Skill("Making Arrow",    1, 1,  "quest", "active"),
                    new Skill("Charge Arrow",    1, 1,  "quest", "active"),
                },
                Locked = new List<LockedSkill>
                {
                    new LockedSkill("Vulture's Eye",         10, "Owl's Eye Lv 3",       "passive"),
                    new LockedSkill("Attention Concentrate", 10, "Vulture's Eye Lv 1",   "active"),
                    new LockedSkill("Arrow Shower",          10, "Double Strafing Lv 5", "active"),
                }
            };
            tree.AllLockedSkillsMaster.AddRange(tree.Locked);
            return tree;
        }
    }
}
