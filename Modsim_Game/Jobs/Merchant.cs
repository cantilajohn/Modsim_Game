namespace Modsim_Game.Jobs
{
    public class Merchant : IJobClass
    {
        public string Name => "Merchant";
        public int BaseWeightLimit => 2500;
        public double SpJobModifier => 3.0;

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
    }
}
