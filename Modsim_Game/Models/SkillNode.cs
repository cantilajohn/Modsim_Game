using System;
using System.Collections.Generic;
using System.Drawing;

namespace Modsim_Game.Models
{
    /// <summary>
    /// A unified skill tree node used for visual tree rendering.
    /// Combines unlocked + locked concepts into one node with parent/child relationships.
    /// </summary>
    public class SkillNode
    {
        public string Name { get; set; } = string.Empty;
        public Image? Icon { get; set; }
        public int CurrentLevel { get; set; }
        public int MaxLevel { get; set; }
        /// <summary>"active" or "quest"</summary>
        public string Type { get; set; }
        /// <summary>True if skill is still locked (prerequisites not met or not yet unlocked).</summary>
        public bool IsLocked { get; set; }
        /// <summary>Requirement string, e.g. "Bash Lv 5". Empty for root skills.</summary>
        public string Requirement { get; set; } = string.Empty;
        /// <summary>"active" or "passive" badge for quest skills</summary>
        public string QuestType { get; set; } = string.Empty;

        // ── Tree structure ──
        public List<SkillNode> Children { get; set; } = new List<SkillNode>();
        public List<SkillNode> Parents { get; set; } = new List<SkillNode>();

        // ── Layout (set by layout algorithm) ──
        public PointF Position { get; set; }
        public int TreeDepth { get; set; }

        /// <summary>Radius of the drawn circle in pixels.</summary>
        public const int NodeRadius = 30;

        public bool ContainsPoint(PointF pt)
        {
            float dx = pt.X - Position.X;
            float dy = pt.Y - Position.Y;
            return (dx * dx + dy * dy) <= (NodeRadius * NodeRadius);
        }

        public bool CanIncrement => !IsLocked && Type != "quest" && CurrentLevel < MaxLevel;
        public bool CanDecrement => !IsLocked && Type != "quest" && CurrentLevel > 0;
    }
}
