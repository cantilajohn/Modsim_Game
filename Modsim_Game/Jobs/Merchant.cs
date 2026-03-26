using Modsim_Game.Models;
using System.Collections.Generic;

namespace Modsim_Game.Jobs
{
    public class Merchant : IJobClass
    {
        public string Name => "Merchant";
        public int BaseWeightLimit => 2500;
        public double SpJobModifier => 3.0;
        public string[] Skills => new[] { "Increase Weight Limit", "Discount", "Overcharge", "Pushcart", "Identify", "Vending", "Mammonite", "Cart Revolution", "Change Cart", "Loud Exclamation", "Buying Store", "Cart Decoration" };
        public string[] AllowedWeapons => new[] { "Hand", "Dagger", "One-Handed-Sword", "One-Handed-Axe", "Two-Handed-Axe", "One-Handed-Mace", "Two-Handed-Mace" };
        public int GetMaxHp(int baseLevel) => JobStatTable.GetMaxHP(Name, baseLevel);
        public int GetStatBonus(string stat, int jobLevel) => JobStatTable.GetBonus(Name, stat, jobLevel);

        public double GetWeaponDelay(string weaponName)
        {
            return weaponName switch
            {
                "Hand" => 0.8,
                "Dagger" => 1.2,
                "One-Handed-Sword" => 1.4,
                "One-Handed-Axe" => 1.4,
                "Two-Handed-Axe" => 1.5,
                "One-Handed-Mace" => 1.4,
                "Two-Handed-Mace" => 1.4,
                _ => 1.0
            };
        }

        public JobSkillTree GetSkillTree()
        {
            var tree = new JobSkillTree
            {
                JobLabel = "Merchant",
                Unlocked = new List<Skill>
                {
                    new Skill("Increase Weight Limit", 0, 10, "passive"),
                    new Skill("Identify",             0, 1,  "active"),
                    new Skill("Mammonite",            0, 10, "active"),
                    new Skill("Cart Revolution",      1, 1,  "quest", "active"),
                    new Skill("Change Cart",          1, 1,  "quest", "active"),
                    new Skill("Loud Exclamation",     1, 1,  "quest", "passive"),
                    new Skill("Cart Decoration",      1, 1,  "quest", "active"),
                },
                Locked = new List<LockedSkill>
                {
                    new LockedSkill("Discount",     10, "Increase Weight Limit Lv 3", "passive"),
                    new LockedSkill("Overcharge",   10, "Discount Lv 3",             "passive"),
                    new LockedSkill("Pushcart",     10, "Increase Weight Limit Lv 5", "passive"),
                    new LockedSkill("Vending",      10, "Pushcart Lv 3",             "active"),
                    new LockedSkill("Buying Store", 1,  "Vending Lv 1",              "active"),
                }
            };
            tree.AllLockedSkillsMaster.AddRange(tree.Locked);
            return tree;
        }
    }
}
