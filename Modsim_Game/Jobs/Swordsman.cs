namespace Modsim_Game.Jobs
{
    public class Swordsman : IJobClass
    {
        public string Name => "Swordsman";
        public int BaseWeightLimit => 2800;
        public double SpJobModifier => 2.0;

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
    }
}
