namespace Modsim_Game.Models
{
    public class LockedSkill
    {
        public string Name { get; set; }
        public int MaxLevel { get; set; }
        /// <summary>
        /// Comma-separated prerequisite string, e.g. "Bash Lv 5" or "Napalm Beat Lv 7, Soul Strike Lv 5"
        /// </summary>
        public string Requirement { get; set; }
        /// <summary>"active" or "passive"</summary>
        public string LockedType { get; set; }

        public LockedSkill(string name, int max, string requirement, string lockedType = "active")
        {
            Name = name;
            MaxLevel = max;
            Requirement = requirement;
            LockedType = lockedType;
        }
    }
}
