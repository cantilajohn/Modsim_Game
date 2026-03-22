namespace Modsim_Game.Jobs
{
    public interface IJobClass
    {
        string Name { get; }
        int BaseWeightLimit { get; }
        double SpJobModifier { get; }
        
        string[] AllowedWeapons { get; }

        int GetMaxHp(int baseLevel);
        int GetStatBonus(string stat, int jobLevel);
        double GetWeaponDelay(string weaponName);
    }
}
