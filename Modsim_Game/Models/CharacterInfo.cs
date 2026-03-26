namespace Modsim_Game.Models
{
    public class CharacterInfo
    {
        public int BaseLevel { get; set; } = 1;
        public int JobLevel { get; set; } = 1;
        
        public int Str { get; set; } = 1;
        public int Agi { get; set; } = 1;
        public int Vit { get; set; } = 1;
        public int Int { get; set; } = 1;
        public int Dex { get; set; } = 1;
        public int Luk { get; set; } = 1;

        public string JobName { get; set; } = "Novice";
        public string WeaponName { get; set; } = "Hand";

        public JobSkillTree? SkillTree { get; set; }
    }
}
