using Modsim_Game.Models;
using System.Collections.Generic;

namespace Modsim_Game.Jobs
{
    public class Thief : IJobClass
    {
        public string Name => "Thief";
        public int BaseWeightLimit => 2000;
        public double SpJobModifier => 3.0;
        public string[] Skills => new[] { "Double Attack", "Improve Dodge", "Steal", "Hiding", "Envenom", "Detoxify", "Sprinkle Sand", "Back Sliding", "Pick Stone", "Throw Stone" };
        public string[] AllowedWeapons => new[] { "Hand", "Dagger", "One-Handed-Sword", "Bow" };
        public int GetMaxHp(int baseLevel) => JobStatTable.GetMaxHP(Name, baseLevel);
        public int GetStatBonus(string stat, int jobLevel) => JobStatTable.GetBonus(Name, stat, jobLevel);

        public double GetWeaponDelay(string weaponName)
        {
            return weaponName switch
            {
                "Hand" => 0.8,
                "Dagger" => 1.0,
                "One-Handed-Sword" => 1.3,
                "Bow" => 1.3,
                _ => 1.0
            };
        }

        public JobSkillTree GetSkillTree()
        {
            var tree = new JobSkillTree
            {
                JobLabel = "Thief",
                Unlocked = new List<Skill>
                {
                    new Skill("Double Attack",  0, 10, "passive"),
                    new Skill("Improve Dodge", 0, 10, "passive"),
                    new Skill("Steal",          0, 10, "active"),
                    new Skill("Envenom",        0, 10, "active"),
                    new Skill("Sprinkle Sand",  1, 1, "quest", "active"),
                    new Skill("Back Sliding",   1, 1, "quest", "active"),
                    new Skill("Pick Stone",     1, 1, "quest", "active"),
                    new Skill("Throw Stone",    1, 1, "quest", "active"),
                },
                Locked = new List<LockedSkill>
                {
                    new LockedSkill("Hiding",   10, "Steal Lv 5",   "active"),
                    new LockedSkill("Detoxify", 1,  "Envenom Lv 3", "active"),
                }
            };
            tree.AllLockedSkillsMaster.AddRange(tree.Locked);
            return tree;
        }
    }
}
