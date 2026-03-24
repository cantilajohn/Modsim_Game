namespace Modsim_Game.Models
{
    public class Skill
    {
        public string Name { get; set; }
        public int CurrentLevel { get; set; }
        public int MaxLevel { get; set; }
        /// <summary>"active", "passive", or "quest"</summary>
        public string Type { get; set; }
        /// <summary>"active" or "passive" badge for quest skills</summary>
        public string QuestType { get; set; } = string.Empty;

        public Skill(string name, int cur, int max, string type, string questType = "")
        {
            Name = name;
            CurrentLevel = cur;
            MaxLevel = max;
            Type = type;
            QuestType = questType;
        }
    }
}
