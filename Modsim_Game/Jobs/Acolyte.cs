using Modsim_Game.Models;
using System.Collections.Generic;

namespace Modsim_Game.Jobs
{
    public class Acolyte : IJobClass
    {
        public string Name => "Acolyte";
        public int BaseWeightLimit => 2000;
        public double SpJobModifier => 4.5;
        public string[] Skills => new[] { "Divine Protection", "Demon Bane", "Heal", "Cure", "Increase Agility", "Decrease Agility", "Signum Crucis", "Angelus", "Blessing", "Ruwach", "Teleportation", "Warp Portal", "Pneuma", "Aqua Benedicta", "Holy Light" };
        public string[] AllowedWeapons => new[] { "Hand", "One-Handed-Mace", "Two-Handed-Mace", "Rod&Staff" };
        public int GetMaxHp(int baseLevel) => JobStatTable.GetMaxHP(Name, baseLevel);
        public int GetStatBonus(string stat, int jobLevel) => JobStatTable.GetBonus(Name, stat, jobLevel);

        public double GetWeaponDelay(string weaponName)
        {
            return weaponName switch
            {
                "Hand" => 0.8,
                "One-Handed-Mace" => 1.3,
                "Two-Handed-Mace" => 1.4,
                "Rod&Staff" => 1.3,
                _ => 1.0
            };
        }

        public JobSkillTree GetSkillTree()
        {
            var tree = new JobSkillTree
            {
                JobLabel = "Acolyte",
                Unlocked = new List<Skill>
                {
                    new Skill("Divine Protection", 0, 10, "passive"),
                    new Skill("Ruwach",            0, 1,  "active"),
                    new Skill("Heal",              0, 10, "active"),
                    new Skill("Aqua Benedicta",    0, 1,  "active"),
                    new Skill("Holy Light",        1, 1,  "quest", "active"),
                },
                Locked = new List<LockedSkill>
                {
                    new LockedSkill("Demon Bane",       10, "Divine Protection Lv 3", "passive"),
                    new LockedSkill("Teleportation",    2,  "Ruwach Lv 1",            "active"),
                    new LockedSkill("Warp Portal",      4,  "Teleportation Lv 2",     "active"),
                    new LockedSkill("Pneuma",           1,  "Warp Portal Lv 4",       "active"),
                    new LockedSkill("Increase Agility", 10, "Heal Lv 3",              "active"),
                    new LockedSkill("Decrease Agility", 10, "Increase Agility Lv 1",  "active"),
                    new LockedSkill("Signum Crucis",    10, "Demon Bane Lv 3",        "active"),
                    new LockedSkill("Angelus",          10, "Divine Protection Lv 3", "active"),
                    new LockedSkill("Blessing",         10, "Divine Protection Lv 5", "active"),
                    new LockedSkill("Cure",             1,  "Heal Lv 2",              "active"),
                }
            };
            tree.AllLockedSkillsMaster.AddRange(tree.Locked);
            return tree;
        }
    }
}
