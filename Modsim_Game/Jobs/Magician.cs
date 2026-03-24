using Modsim_Game.Models;
using System.Collections.Generic;

namespace Modsim_Game.Jobs
{
    public class Magician : IJobClass
    {
        public string Name => "Magician";
        public int BaseWeightLimit => 2200;
        public double SpJobModifier => 4.0;
        public string[] Skills => new[] { "Increase Spiritual Power", "Sight", "Napalm Beat", "Safety Wall", "Soul Strike", "Cold Bolt", "Frost Diver", "Stone Curse", "Fire Ball", "Fire Wall", "Fire Bolt", "Lightning Bolt", "Thunder Storm", "Energy Coat" };
        public string[] AllowedWeapons => new[] { "Hand", "Dagger", "Rod&Staff", "Two-Handed-Staff" };
        public int GetMaxHp(int baseLevel) => JobStatTable.GetMaxHP(Name, baseLevel);
        public int GetStatBonus(string stat, int jobLevel) => JobStatTable.GetBonus(Name, stat, jobLevel);

        public double GetWeaponDelay(string weaponName)
        {
            return weaponName switch
            {
                "Hand" => 1.0,
                "Dagger" => 1.2,
                "Rod&Staff" => 1.1,
                "Two-Handed-Staff" => 1.3,
                _ => 1.0
            };
        }

        public JobSkillTree GetSkillTree()
        {
            var tree = new JobSkillTree
            {
                JobLabel = "Magician",
                Unlocked = new List<Skill>
                {
                    new Skill("Increase Spiritual Power", 0, 10, "passive"),
                    new Skill("Sight",                    0, 1,  "active"),
                    new Skill("Napalm Beat",              0, 10, "active"),
                    new Skill("Cold Bolt",                0, 10, "active"),
                    new Skill("Stone Curse",              0, 10, "active"),
                    new Skill("Fire Bolt",                0, 10, "active"),
                    new Skill("Lightning Bolt",           0, 10, "active"),
                    new Skill("Energy Coat",              1, 1,  "quest", "active"),
                },
                Locked = new List<LockedSkill>
                {
                    new LockedSkill("Soul Strike",   10, "Napalm Beat Lv 4",                   "active"),
                    new LockedSkill("Frost Diver",   10, "Cold Bolt Lv 5",                     "active"),
                    new LockedSkill("Fire Ball",     10, "Fire Bolt Lv 4",                     "active"),
                    new LockedSkill("Fire Wall",     10, "Sight Lv 1, Fire Ball Lv 5",         "active"),
                    new LockedSkill("Thunder Storm", 10, "Lightning Bolt Lv 4",                "active"),
                    new LockedSkill("Safety Wall",   10, "Napalm Beat Lv 7, Soul Strike Lv 5", "active"),
                }
            };
            tree.AllLockedSkillsMaster.AddRange(tree.Locked);
            return tree;
        }
    }
}
