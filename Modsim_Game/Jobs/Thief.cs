namespace Modsim_Game.Jobs
{
    public class Thief : IJobClass
    {
        public string Name => "Thief";
        public int BaseWeightLimit => 2400;
        public double SpJobModifier => 2.0;

        public string[] AllowedWeapons => new[] { "Hand", "Dagger", "One-Handed-Sword", "One-Handed-Axe", "Bow" };

        public int GetMaxHp(int baseLevel) => JobStatTable.GetMaxHP(Name, baseLevel);
        public int GetStatBonus(string stat, int jobLevel) => JobStatTable.GetBonus(Name, stat, jobLevel);

        public double GetWeaponDelay(string weaponName)
        {
            return weaponName switch
            {
                "Hand" => 0.8,
                "Dagger" => 1.0,
                "One-Handed-Sword" => 1.3,
                "One-Handed-Axe" => 1.6,
                "Bow" => 1.6,
                _ => 1.0
            };
        }
    }
}
