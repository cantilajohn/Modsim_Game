using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Modsim_Game.Models
{
    public class JobSkillTree
    {
        public string JobLabel { get; set; } = string.Empty;
        public List<Skill> Unlocked { get; set; } = new List<Skill>();
        public List<LockedSkill> Locked { get; set; } = new List<LockedSkill>();

        public int JobLevel { get; set; } = 50;

        /// <summary>
        /// Max skill points available (JobLevel - 1).
        /// </summary>
        public int MaxSkillPoints => JobLevel - 1;

        /// <summary>
        /// Total skill points spent on active and passive (non-quest) unlocked skills.
        /// </summary>
        public int SkillPointsUsed =>
            Unlocked.Where(s => s.Type != "quest").Sum(s => s.CurrentLevel);

        /// <summary>
        /// Points remaining to allocate.
        /// </summary>
        public int SkillPointsRemaining => MaxSkillPoints - SkillPointsUsed;

        // Master list of all potential locked skills and their requirements (for demotion checks)
        public List<LockedSkill> AllLockedSkillsMaster { get; set; } = new List<LockedSkill>();

        /// <summary>
        /// Check whether all prerequisites of a locked skill are currently met.
        /// </summary>
        public bool CanUnlock(LockedSkill lockedSkill)
        {
            if (string.IsNullOrWhiteSpace(lockedSkill.Requirement))
                return true;

            var parts = lockedSkill.Requirement.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var part in parts)
            {
                var match = Regex.Match(part, @"^(.+?)\s+Lv\s*(\d+)$", RegexOptions.IgnoreCase);
                if (!match.Success) continue;

                string reqName = match.Groups[1].Value.Trim();
                int reqLevel = int.Parse(match.Groups[2].Value);

                var found = Unlocked.FirstOrDefault(s => string.Equals(s.Name.Trim(), reqName, StringComparison.OrdinalIgnoreCase));
                if (found == null || found.CurrentLevel < reqLevel)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Attempt to unlock a locked skill. Returns true if successful.
        /// Moves it from Locked → Unlocked.
        /// </summary>
        public bool TryUnlock(LockedSkill lockedSkill)
        {
            if (!CanUnlock(lockedSkill)) return false;

            Locked.Remove(lockedSkill);
            Unlocked.Add(new Skill(lockedSkill.Name, 0, lockedSkill.MaxLevel, lockedSkill.LockedType));
            return true;
        }

        /// <summary>
        /// Automatically promotes any locked skills whose prerequisites are now met.
        /// Recursively checks to handle "chain reactions".
        /// </summary>
        public void AutoUpdateUnlocks()
        {
            bool anyPromoted = true;
            while (anyPromoted)
            {
                anyPromoted = false;
                var toUnlock = Locked.Where(ls => CanUnlock(ls)).ToList();
                foreach (var ls in toUnlock)
                {
                    Locked.Remove(ls);
                    Unlocked.Add(new Skill(ls.Name, 0, ls.MaxLevel, ls.LockedType));
                    anyPromoted = true;
                }
            }
        }

        /// <summary>
        /// Automatically relocks any unlocked skills whose prerequisites are no longer met.
        /// Resets their level to 0 and recurses to handle "cascades".
        /// </summary>
        public void AutoUpdateLocks()
        {
            bool anyDemoted = true;
            while (anyDemoted)
            {
                anyDemoted = false;
                var toLock = Unlocked.Where(s => {
                    var master = AllLockedSkillsMaster.FirstOrDefault(m => string.Equals(m.Name, s.Name, StringComparison.OrdinalIgnoreCase));
                    return master != null && !CanUnlock(master);
                }).ToList();

                foreach (var s in toLock)
                {
                    var master = AllLockedSkillsMaster.First(m => string.Equals(m.Name, s.Name, StringComparison.OrdinalIgnoreCase));
                    Unlocked.Remove(s);
                    Locked.Add(new LockedSkill(master.Name, master.MaxLevel, master.Requirement, master.LockedType));
                    anyDemoted = true;
                }
            }
        }

        /// <summary>
        /// Reset all active skill levels back to 0 and demote all
        /// previously-unlocked locked skills back to the locked list.
        /// </summary>
        public void ResetSkills()
        {
            foreach (var s in Unlocked)
                s.CurrentLevel = 0;
            
            AutoUpdateLocks(); // This will demote everything that has requirements
        }
    }
}
