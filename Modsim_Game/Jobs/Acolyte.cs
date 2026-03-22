namespace Modsim_Game.Jobs
{
    public class Acolyte : IJobClass
    {
        public string Name => "Acolyte";
        public int BaseWeightLimit => 2200;
        public double SpJobModifier => 5.0;

        public string[] AllowedWeapons => new[] { "Hand", "One-Handed-Mace", "Two-Handed-Mace", "Rod&Staff", "Two-Handed-Staff" };

        public int GetMaxHp(int baseLevel) => JobStatTable.GetMaxHP(Name, baseLevel);
        public int GetStatBonus(string stat, int jobLevel) => JobStatTable.GetBonus(Name, stat, jobLevel);

        public double GetWeaponDelay(string weaponName)
        {
            return weaponName switch
            {
                "Hand" => 0.8,
                "One-Handed-Mace" => 1.2,
                "Two-Handed-Mace" => 1.2,
                "Rod&Staff" => 1.2,
                "Two-Handed-Staff" => 1.2,
                _ => 1.0
            };
        }
    }
}
