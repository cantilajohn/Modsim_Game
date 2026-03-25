using Modsim_Game.Models;
using System.Collections.Generic;

namespace Modsim_Game.Jobs
{
    public class Swordsman : IJobClass
    {
        public string Name => "Swordsman";
        public int BaseWeightLimit => 2800;
        public double SpJobModifier => 2.0;
        public string[] Skills => new[] { "Sword Mastery", "Two-Handed Sword Mastery", "Increase Recuperative Power", "Bash", "Provoke", "Magnum Break", "Endure", "Moving HP Recovery", "Fatal Blow", "Auto Berserk" };
        public string[] AllowedWeapons => new[] { "Hand", "Dagger", "One-Handed-Sword", "Two-Handed-Sword", "One-Handed-Spear", "Two-Handed-Spear", "One-Handed-Axe", "Two-Handed-Axe", "One-Handed-Mace", "Two-Handed-Mace" };
        public int GetMaxHp(int baseLevel) => JobStatTable.GetMaxHP(Name, baseLevel);
        public int GetStatBonus(string stat, int jobLevel) => JobStatTable.GetBonus(Name, stat, jobLevel);

        public double GetWeaponDelay(string weaponName)
        {
            return weaponName switch
            {
                "Hand" => 0.8,
                "Dagger" => 1.0,
                "One-Handed-Sword" => 1.1,
                "Two-Handed-Sword" => 1.2,
                "One-Handed-Spear" => 1.3,
                "Two-Handed-Spear" => 1.4,
                "One-Handed-Axe" => 1.4,
                "Two-Handed-Axe" => 1.5,
                "One-Handed-Mace" => 1.3,
                "Two-Handed-Mace" => 1.4,
                _ => 1.0
            };
        }

        public JobSkillTree GetSkillTree()
        {
            var tree = new JobSkillTree
            {
                JobLabel = "Swordsman",
                Unlocked = new List<Skill>
                {
                    new Skill("Sword Mastery",               0, 10, "passive","Physical"),
                    new Skill("Increase Recuperative Power", 0, 10, "passive","Physical"),
                    new Skill("Bash",                        0, 10, "active"),
                    new Skill("Provoke",                     0, 10, "active"),
                    new Skill("Moving HP Recovery", 1, 1, "quest", "passive"),
                    new Skill("Fatal Blow",         1, 1, "quest", "passive"),
                    new Skill("Auto Berserk",       1, 1, "quest", "active"),
                },
                Locked = new List<LockedSkill>
                {
                    new LockedSkill("Two-Handed Sword Mastery", 10, "Sword Mastery Lv 1", "passive"),
                    new LockedSkill("Magnum Break",             10, "Bash Lv 5",          "active"),
                    new LockedSkill("Endure",                   10, "Provoke Lv 5",       "active"),
                }
            };
            tree.AllLockedSkillsMaster.AddRange(tree.Locked);
            return tree;
        }
    }
}
