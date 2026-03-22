namespace Modsim_Game.Jobs
{
    public class Archer : IJobClass
    {
        public string Name => "Archer";
        public int BaseWeightLimit => 2330;
        public double SpJobModifier => 2.0;

        public string[] AllowedWeapons => new[] { "Hand", "Dagger", "Bow" };

        public int GetMaxHp(int baseLevel) => JobStatTable.GetMaxHP(Name, baseLevel);
        public int GetStatBonus(string stat, int jobLevel) => JobStatTable.GetBonus(Name, stat, jobLevel);

        public double GetWeaponDelay(string weaponName)
        {
            return weaponName switch
            {
                "Hand" => 0.8,
                "Dagger" => 1.2,
                "Bow" => 1.4,
                _ => 1.0
            };
        }
    }
}
