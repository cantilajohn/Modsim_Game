namespace Modsim_Game.Jobs
{
    public class Magician : IJobClass
    {
        public string Name => "Magician";
        public int BaseWeightLimit => 2200;
        public double SpJobModifier => 6.0;

        public string[] AllowedWeapons => new[] { "Hand", "Dagger", "Rod&Staff", "Two-Handed-Staff" };

        public int GetMaxHp(int baseLevel) => JobStatTable.GetMaxHP(Name, baseLevel);
        public int GetStatBonus(string stat, int jobLevel) => JobStatTable.GetBonus(Name, stat, jobLevel);

        public double GetWeaponDelay(string weaponName)
        {
            return weaponName switch
            {
                "Hand" => 1.0,
                "Dagger" => 1.2,
                "Rod&Staff" => 1.4,
                "Two-Handed-Staff" => 1.4,
                _ => 1.0
            };
        }
    }
}
