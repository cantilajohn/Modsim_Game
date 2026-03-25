using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Modsim_Game.Models;
using Modsim_Game.Data;

namespace Modsim_Game.Controls
{
    /// <summary>
    /// A custom GDI+ painted panel that renders a skill tree as connected circular nodes.
    /// </summary>
    public class SkillTreePanel : Panel
    {
        // ── Theme ──
        private static readonly Color BgColor = Color.FromArgb(30, 30, 36);
        private static readonly Color LineColor = Color.FromArgb(90, 90, 100);
        private static readonly Color NodeLockedBg = Color.FromArgb(60, 60, 68);
        private static readonly Color NodeActiveBg = Color.FromArgb(50, 110, 160);
        private static readonly Color NodeMaxedBg = Color.FromArgb(60, 160, 80);
        private static readonly Color NodeQuestBg = Color.FromArgb(170, 140, 50);
        private static readonly Color NodeBorder = Color.FromArgb(120, 120, 130);
        private static readonly Color NodeLockedBorder = Color.FromArgb(70, 70, 78);
        private static readonly Color TextWhite = Color.White;
        private static readonly Color TextMuted = Color.FromArgb(140, 140, 140);
        private static readonly Color HoverGlow = Color.FromArgb(60, 255, 255, 255);

        private List<SkillNode> _allNodes = new List<SkillNode>();
        private List<SkillNode> _roots = new List<SkillNode>();
        private SkillNode? _hoveredNode = null;
        private SkillNode? _selectedNode = null;
        private JobSkillTree? _skillTree;

        // Icon Cache
        private Dictionary<string, Image> _iconCache = new Dictionary<string, Image>(StringComparer.OrdinalIgnoreCase);

        public event EventHandler? SkillChanged;
        public event EventHandler<SkillNode>? NodeSelected;

        public SkillNode? SelectedNode => _selectedNode;
        public List<SkillNode> AllNodes => _allNodes;
        
        public int TargetWidth { get; set; } = 940;

        public SkillTreePanel()
        {
            DoubleBuffered = true;
            BackColor = Color.Transparent;
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ComputeLayout();
            Invalidate();
        }

        /// <summary>
        /// Load a JobSkillTree and build the visual node graph.
        /// </summary>
        public void LoadSkillTree(JobSkillTree skillTree)
        {
            _skillTree = skillTree;
            BuildNodeGraph();
            ComputeLayout();
            Invalidate();
        }

        public JobSkillTree? GetSkillTree() => _skillTree;

        //  BUILD NODE GRAPH from JobSkillTree

        public void BuildNodeGraph()
        {
            _allNodes.Clear();
            _roots.Clear();
            if (_skillTree == null) return;

            var lookup = new Dictionary<string, SkillNode>(StringComparer.OrdinalIgnoreCase);

            // Create nodes for unlocked skills
            foreach (var s in _skillTree.Unlocked)
            {
                // Retrieve original requirement string to prevent broken graph edges
                string originalReq = _skillTree.AllLockedSkillsMaster
                    .FirstOrDefault(ls => ls.Name.Equals(s.Name, StringComparison.OrdinalIgnoreCase))?.Requirement ?? string.Empty;

                var node = new SkillNode
                {
                    Name = s.Name,
                    Icon = GetIcon(s.Name),
                    CurrentLevel = s.CurrentLevel,
                    MaxLevel = s.MaxLevel,
                    Type = s.Type,
                    QuestType = s.QuestType,
                    IsLocked = false,
                    Requirement = originalReq,
                };
                lookup[s.Name] = node;
                _allNodes.Add(node);
            }

            // Create nodes for locked skills
            foreach (var ls in _skillTree.Locked)
            {
                var node = new SkillNode
                {
                    Name = ls.Name,
                    Icon = GetIcon(ls.Name),
                    CurrentLevel = 0,
                    MaxLevel = ls.MaxLevel,
                    Type = ls.LockedType, // e.g. "active" or "passive"
                    IsLocked = !_skillTree.CanUnlock(ls),
                    Requirement = ls.Requirement,
                };
                lookup[ls.Name] = node;
                _allNodes.Add(node);
            }

            // Wire parent-child from requirements
            foreach (var node in _allNodes)
            {
                if (string.IsNullOrEmpty(node.Requirement)) continue;

                var reqParts = node.Requirement.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in reqParts)
                {
                    var match = Regex.Match(part, @"^(.+?)\s+Lv\s*(\d+)$", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        string parentName = match.Groups[1].Value.Trim();
                        if (lookup.TryGetValue(parentName, out var parentNode))
                        {
                            node.Parents.Add(parentNode);
                            if (!parentNode.Children.Contains(node))
                                parentNode.Children.Add(node);
                        }
                    }
                }
            }

            // Roots = nodes with no parents
            _roots = _allNodes.Where(n => n.Parents.Count == 0).ToList();
        }

        //  LAYOUT — assign X,Y positions to each node
        public void ComputeLayout()
        {
            if (_allNodes.Count == 0 || this.Width <= 0) return;

            int nodeSize = SkillNode.NodeRadius * 2;
            int hSpacing = 140;
            int vSpacing = 120;
            int topMargin = 50;
            int leftMargin = 50;

            // Assign tree depths via BFS/Level Order
            var queue = new Queue<SkillNode>();
            
            // Re-identify roots just in case
            _roots = _allNodes.Where(n => n.Parents.Count == 0).ToList();
            foreach (var node in _allNodes) node.TreeDepth = -1; // Reset

            foreach (var root in _roots) 
            { 
                root.TreeDepth = 0; 
                queue.Enqueue(root); 
            }

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                foreach (var child in node.Children)
                {
                    int newDepth = node.TreeDepth + 1;
                    if (newDepth > child.TreeDepth)
                    {
                        child.TreeDepth = newDepth;
                        if (!queue.Contains(child)) queue.Enqueue(child);
                    }
                }
            }

            // Ensure no orphan has depth -1
            foreach (var node in _allNodes) if (node.TreeDepth < 0) node.TreeDepth = 0;

            // Calculate density per depth to adjust hSpacing if needed
            int maxDepth = _allNodes.Count > 0 ? _allNodes.Max(n => n.TreeDepth) : 0;
            int maxNodesAtOneDepth = 0;
            for (int d = 0; d <= maxDepth; d++)
                maxNodesAtOneDepth = Math.Max(maxNodesAtOneDepth, _allNodes.Count(n => n.TreeDepth == d));

            // DYNAMIC SPACING: If too many nodes for width, squeeze them
            int maxPanelWidth = TargetWidth > 0 ? TargetWidth : 940; 
            float layoutWidthBounds = maxPanelWidth - (leftMargin * 2);

            if (maxNodesAtOneDepth > 1)
            {
                float requiredWidth = (maxNodesAtOneDepth - 1) * hSpacing + nodeSize;
                if (requiredWidth > layoutWidthBounds)
                {
                    hSpacing = (int)(layoutWidthBounds / (maxNodesAtOneDepth - 1));
                    // ABSOLUTE MINIMUM SPACING: Don't allow overlap!
                    hSpacing = Math.Max(hSpacing, nodeSize + 5); 
                }
            }

            // Group by depth and assign positions
            float totalWidth = 0;
            float totalHeight = 0;

            for (int depth = 0; depth <= maxDepth; depth++)
            {
                var nodesAtDepth = _allNodes.Where(n => n.TreeDepth == depth).ToList();
                int count = nodesAtDepth.Count;
                if (count == 0) continue;

                // Center each row
                float rowWidth = (count - 1) * hSpacing + nodeSize;
                float startX = leftMargin + Math.Max(0, (layoutWidthBounds - rowWidth) / 2) + (nodeSize / 2f);

                for (int i = 0; i < count; i++)
                {
                    float x = startX + i * hSpacing;
                    float y = topMargin + depth * vSpacing + (nodeSize / 2f);
                    nodesAtDepth[i].Position = new PointF(x, y);
                    totalWidth = Math.Max(totalWidth, x + (nodeSize / 2f));
                    totalHeight = Math.Max(totalHeight, y + (nodeSize / 2f));
                }
            }

            // SET SIZE strictly
            int panelWidth = Math.Max((int)totalWidth + leftMargin, maxPanelWidth);
            int panelHeight = (int)totalHeight + topMargin + 20;

            this.Size = new Size(panelWidth, panelHeight);
            this.MinimumSize = new Size(panelWidth, panelHeight);
        }

        // ═════════════════════════════════════════════
        //  PAINT
        // ═════════════════════════════════════════════

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            // Draw connecting lines first (behind nodes)
            using (var linePen = new Pen(LineColor, 1.5f))
            {
                foreach (var node in _allNodes)
                {
                    foreach (var parent in node.Parents)
                    {
                        g.DrawLine(linePen, parent.Position, node.Position);
                    }
                }
            }

            // Draw each node
            foreach (var node in _allNodes)
            {
                DrawNode(g, node);
            }

            // Draw tooltip for hovered node
            if (_hoveredNode != null)
            {
                DrawTooltip(g, _hoveredNode);
            }
        }

        private void DrawNode(Graphics g, SkillNode node)
        {
            float r = SkillNode.NodeRadius;
            float x = node.Position.X;
            float y = node.Position.Y;
            var rect = new RectangleF(x - r, y - r, r * 2, r * 2);

            // Determine colors
            Color bgColor;
            Color borderColor;

            if (node.IsLocked)
            {
                bgColor = NodeLockedBg;
                borderColor = NodeLockedBorder;
            }
            else if (node.Type == "quest")
            {
                bgColor = NodeQuestBg;
                borderColor = NodeBorder;
            }
            else if (node.CurrentLevel >= node.MaxLevel)
            {
                bgColor = NodeMaxedBg;
                borderColor = Color.FromArgb(100, 200, 120);
            }
            else if (node.CurrentLevel > 0)
            {
                bgColor = NodeActiveBg;
                borderColor = Color.FromArgb(80, 160, 220);
            }
            else
            {
                bgColor = Color.FromArgb(55, 55, 65);
                borderColor = NodeBorder;
            }

            // Hover glow
            bool isHovered = node == _hoveredNode;
            if (isHovered)
            {
                using (var glowBrush = new SolidBrush(HoverGlow))
                {
                    var glowRect = new RectangleF(x - r - 4, y - r - 4, r * 2 + 8, r * 2 + 8);
                    g.FillEllipse(glowBrush, glowRect);
                }
            }

            // Fill circle
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillEllipse(bgBrush, rect);
            }

            // Draw Icon
            if (node.Icon != null)
            {
                float iconSize = r * 1.4f;
                var iconRect = new RectangleF(x - iconSize / 2, y - iconSize / 2, iconSize, iconSize);

                if (node.IsLocked)
                {
                    // Create a desaturated and semi-transparent effect for locked skills
                    using (var attributes = new System.Drawing.Imaging.ImageAttributes())
                    {
                        var matrix = new System.Drawing.Imaging.ColorMatrix(new float[][]
                        {
                            new float[] {0.3f, 0.3f, 0.3f, 0, 0},
                            new float[] {0.3f, 0.3f, 0.3f, 0, 0},
                            new float[] {0.3f, 0.3f, 0.3f, 0, 0},
                            new float[] {0,    0,    0,    0.4f, 0}, // 40% transparency
                            new float[] {0,    0,    0,    0, 1}
                        });
                        attributes.SetColorMatrix(matrix);

                        // Use the destination rectangle to ensure perfect centering and scaling
                        g.DrawImage(node.Icon,
                            new Rectangle((int)iconRect.X, (int)iconRect.Y, (int)iconRect.Width, (int)iconRect.Height),
                            0, 0, node.Icon.Width, node.Icon.Height,
                            GraphicsUnit.Pixel, attributes);
                    }
                }
                else
                {
                    g.DrawImage(node.Icon, iconRect);
                }
            }
            else if (node.Type != "quest")
            {
                var iconSize = r * 1.2f;
                var iconRect = new RectangleF(x - iconSize / 2, y - iconSize / 2, iconSize, iconSize);
                DrawPlaceholderIcon(g, iconRect);
            }

            // Level Progress Ring
            if (!node.IsLocked && node.MaxLevel > 0)
            {
                float angle = (node.CurrentLevel / (float)node.MaxLevel) * 360f;
                using (var ringPen = new Pen(Color.FromArgb(100, 255, 200, 0), 4f))
                {
                    g.DrawArc(ringPen, rect.X - 2, rect.Y - 2, rect.Width + 4, rect.Height + 4, -90, angle);
                }
            }

            // Border
            float penWidth = (node == _selectedNode) ? 3f : 2f;
            Color actualBorder = (node == _selectedNode) ? Color.FromArgb(255, 220, 100) : borderColor;
            using (var pen = new Pen(actualBorder, penWidth))
            {
                g.DrawEllipse(pen, rect);
            }

            using (var font = new Font("Segoe UI", 8f, FontStyle.Bold))
            using (var textBrush = new SolidBrush(node.IsLocked ? TextMuted : TextWhite))
            {
                // Level is now drawn as a progress ring and a small overlay, 
                // Skill level text at bottom-right
                if (node.Type != "quest")
                {
                    using (var lvFont = new Font("Segoe UI", 7f, FontStyle.Bold))
                    using (var lvBrush = new SolidBrush(Color.Gold))
                    {
                        string lvStr = $"{node.CurrentLevel}/{node.MaxLevel}";
                        g.DrawString(lvStr, lvFont, lvBrush, x + r - 10, y + r - 10);
                    }
                }

                // --- Type Badge (Top-Right) ---
                string displayType = node.Type;
                if (node.Type == "quest" && !string.IsNullOrEmpty(node.QuestType))
                    displayType = node.QuestType;

                string badgeChar = displayType.StartsWith("p", StringComparison.OrdinalIgnoreCase) ? "P" : "A";
                Color badgeColor = node.Type == "quest" ? Color.SkyBlue : (badgeChar == "P" ? Color.LightGreen : Color.OrangeRed);

                using (var badgeBrush = new SolidBrush(Color.FromArgb(200, badgeColor)))
                using (var badgeFont = new Font("Segoe UI", 6f, FontStyle.Bold))
                {
                    var badgeRect = new RectangleF(x + r - 12, y - r - 2, 16, 12);
                    g.FillRectangle(badgeBrush, badgeRect); // Standard GDI+

                    using (var badgeTextBrush = new SolidBrush(Color.Black))
                    {
                        var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                        g.DrawString(badgeChar, badgeFont, badgeTextBrush, badgeRect, sf);
                    }
                }
            }

            // Skill name below circle (with subtle shadow for premium look)
            using (var nameFont = new Font("Segoe UI", 7.5f, FontStyle.Bold))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center };
                var nameRect = new RectangleF(x - 60, y + r + 5, 120, 30);

                // Shadow
                using (var shadowBrush = new SolidBrush(Color.FromArgb(150, 0, 0, 0)))
                {
                    g.DrawString(node.Name, nameFont, shadowBrush,
                        new RectangleF(nameRect.X + 1, nameRect.Y + 1, nameRect.Width, nameRect.Height), sf);
                }

                using (var nameBrush = new SolidBrush(node.IsLocked ? TextMuted : TextWhite))
                {
                    g.DrawString(node.Name, nameFont, nameBrush, nameRect, sf);
                }
            }
        }

        private void DrawTooltip(Graphics g, SkillNode node)
        {
            string info = $"{node.Name}\n";
            if (node.Type == "quest")
                info += "Quest Skill (fixed)";
            else
                info += $"Level: {node.CurrentLevel} / {node.MaxLevel}";

            if (!string.IsNullOrEmpty(node.Requirement))
                info += $"\nRequires: {node.Requirement}";

            if (node.IsLocked)
                info += "\n[LOCKED]";

            using (var font = new Font("Segoe UI", 8f))
            {
                var size = g.MeasureString(info, font, 200);
                float tx = node.Position.X + SkillNode.NodeRadius + 10;
                float ty = node.Position.Y - size.Height / 2;

                // Keep inside bounds
                if (tx + size.Width + 10 > Width) tx = node.Position.X - SkillNode.NodeRadius - size.Width - 10;
                if (ty < 5) ty = 5;

                var tipRect = new RectangleF(tx - 4, ty - 4, size.Width + 8, size.Height + 8);
                using (var bg = new SolidBrush(Color.FromArgb(230, 20, 20, 25)))
                using (var border = new Pen(Color.FromArgb(100, 100, 110), 1))
                {
                    g.FillRectangle(bg, tipRect);
                    g.DrawRectangle(border, tipRect.X, tipRect.Y, tipRect.Width, tipRect.Height);
                }

                using (var brush = new SolidBrush(TextWhite))
                {
                    g.DrawString(info, font, brush, new RectangleF(tx, ty, size.Width, size.Height));
                }
            }
        }

        //  MOUSE INTERACTION
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            var pt = new PointF(e.X, e.Y);
            var newHover = _allNodes.FirstOrDefault(n => n.ContainsPoint(pt));
            if (newHover != _hoveredNode)
            {
                _hoveredNode = newHover;
                Cursor = _hoveredNode != null ? Cursors.Hand : Cursors.Default;
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (_hoveredNode != null)
            {
                _hoveredNode = null;
                Cursor = Cursors.Default;
                Invalidate();
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            var pt = new PointF(e.X, e.Y);
            var clicked = _allNodes.FirstOrDefault(n => n.ContainsPoint(pt));

            if (clicked == null)
            {
                _selectedNode = null;
                Invalidate();
                return;
            }

            _selectedNode = clicked;
            NodeSelected?.Invoke(this, clicked);

            if (_skillTree == null) return;

            if (e.Button == MouseButtons.Left)
            {
                // Left click = select only (show description panel)
                // Increment/unlock is handled via sidebar buttons
            }
            else if (e.Button == MouseButtons.Right)
            {
                // Right click = decrement
                if (clicked.CanDecrement)
                {
                    DecrementLevel(clicked);
                }
            }

            Invalidate();
        }

        public void IncrementLevel(SkillNode node)
        {
            if (_skillTree == null || node.IsLocked || !node.CanIncrement || _skillTree.SkillPointsRemaining <= 0) return;

            node.CurrentLevel++;
            SyncNodeToSkillTree(node);

            // Trigger recursive auto-updates
            _skillTree.AutoUpdateUnlocks();
            _skillTree.AutoUpdateLocks();

            // Rebuild graph to show new nodes/state
            BuildNodeGraph();
            ComputeLayout();
            Invalidate();

            SkillChanged?.Invoke(this, EventArgs.Empty);
        }

        public void DecrementLevel(SkillNode node)
        {
            if (_skillTree == null || !node.CanDecrement) return;

            node.CurrentLevel--;
            SyncNodeToSkillTree(node);

            // Trigger recursive auto-updates (important for cascades)
            _skillTree.AutoUpdateUnlocks();
            _skillTree.AutoUpdateLocks();

            // Rebuild graph
            BuildNodeGraph();
            ComputeLayout();
            Invalidate();

            SkillChanged?.Invoke(this, EventArgs.Empty);
        }

        private void TryUnlockNode(SkillNode node)
        {
            if (_skillTree == null) return;
            // Find the matching LockedSkill in _skillTree
            var lockedSkill = _skillTree.Locked
                .FirstOrDefault(ls => ls.Name.Equals(node.Name, StringComparison.OrdinalIgnoreCase));

            if (lockedSkill != null && _skillTree.TryUnlock(lockedSkill))
            {
                node.IsLocked = false;
                node.CurrentLevel = 0;
                
                _skillTree.AutoUpdateUnlocks();
                _skillTree.AutoUpdateLocks();
                
                BuildNodeGraph();
                ComputeLayout();
                Invalidate();
                
                SkillChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Auto-fulfill all prerequisites for a locked skill, leveling parent skills as needed,
        /// then unlock the target skill.
        /// </summary>
        public bool AutoFulfillRequirements(SkillNode node)
        {
            if (_skillTree == null || !node.IsLocked) return false;

            // Find the locked skill entry
            var lockedSkill = _skillTree.Locked
                .FirstOrDefault(ls => ls.Name.Equals(node.Name, StringComparison.OrdinalIgnoreCase));
            if (lockedSkill == null) return false;

            // Recursively fulfill all prerequisites
            if (!FulfillRequirementsRecursive(lockedSkill.Requirement))
                return false;

            // Now unlock the target skill
            _skillTree.AutoUpdateUnlocks();
            _skillTree.AutoUpdateLocks();

            // Rebuild the graph to reflect new state
            BuildNodeGraph();
            ComputeLayout();
            Invalidate();
            SkillChanged?.Invoke(this, EventArgs.Empty);
            return true;
        }

        /// <summary>
        /// Recursively parses a requirement string and ensures each prerequisite skill
        /// is unlocked and leveled to meet the requirement.
        /// </summary>
        private bool FulfillRequirementsRecursive(string requirement)
        {
            if (_skillTree == null || string.IsNullOrWhiteSpace(requirement)) return true;

            var parts = requirement.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var part in parts)
            {
                var match = Regex.Match(part, @"^(.+?)\s+Lv\s*(\d+)$", RegexOptions.IgnoreCase);
                if (!match.Success) continue;

                string reqName = match.Groups[1].Value.Trim();
                int reqLevel = int.Parse(match.Groups[2].Value);

                // Check if the prerequisite is still locked
                var lockedPrereq = _skillTree.Locked
                    .FirstOrDefault(ls => ls.Name.Equals(reqName, StringComparison.OrdinalIgnoreCase));
                if (lockedPrereq != null)
                {
                    // Recursively fulfill ITS prerequisites first
                    if (!FulfillRequirementsRecursive(lockedPrereq.Requirement))
                        return false;

                    // After fulfilling sub-prerequisites, auto-update unlocks
                    _skillTree.AutoUpdateUnlocks();
                }

                // Now the skill should be in Unlocked — set its level
                var unlockedSkill = _skillTree.Unlocked
                    .FirstOrDefault(s => s.Name.Equals(reqName, StringComparison.OrdinalIgnoreCase));
                if (unlockedSkill == null) return false; // Should not happen

                if (unlockedSkill.CurrentLevel < reqLevel)
                {
                    // Calculate cost: how many extra points needed
                    int levelsNeeded = reqLevel - unlockedSkill.CurrentLevel;
                    if (_skillTree.SkillPointsRemaining < levelsNeeded)
                        return false; // Not enough skill points

                    unlockedSkill.CurrentLevel = reqLevel;
                }
            }
            return true;
        }

        /// <summary>
        /// Sync a node's CurrentLevel back to the underlying JobSkillTree Unlocked list.
        /// </summary>
        private void SyncNodeToSkillTree(SkillNode node)
        {
            if (_skillTree == null) return;
            var skill = _skillTree.Unlocked
                .FirstOrDefault(s => s.Name.Equals(node.Name, StringComparison.OrdinalIgnoreCase));
            if (skill != null)
            {
                skill.CurrentLevel = node.CurrentLevel;
            }
        }

        private static string? _skillsFolderPath = null;
        private Image? GetIcon(string skillName)
        {
            if (_iconCache.TryGetValue(skillName, out var cached)) return cached;

            var desc = SkillDescriptionRepository.Get(skillName);
            if (string.IsNullOrEmpty(desc.IconPath)) return null;

            try
            {
                if (_skillsFolderPath == null)
                {
                    // Search for "skills" folder upwards from EXE
                    string current = AppDomain.CurrentDomain.BaseDirectory;
                    while (!string.IsNullOrEmpty(current))
                    {
                        string test = System.IO.Path.Combine(current, "skills");
                        if (System.IO.Directory.Exists(test))
                        {
                            _skillsFolderPath = test;
                            break;
                        }
                        current = System.IO.Path.GetDirectoryName(current);
                    }

                    // Try current directory as fallback
                    if (_skillsFolderPath == null && System.IO.Directory.Exists("skills"))
                        _skillsFolderPath = System.IO.Path.GetFullPath("skills");
                }

                if (_skillsFolderPath != null)
                {
                    string fullPath = System.IO.Path.Combine(_skillsFolderPath, desc.IconPath);
                    if (System.IO.File.Exists(fullPath))
                    {
                        var img = Image.FromFile(fullPath);
                        _iconCache[skillName] = img;
                        return img;
                    }
                }
            }
            catch { }

            return null;
        }

        private void DrawPlaceholderIcon(Graphics g, RectangleF rect)
        {
            using (var font = new Font("Segoe UI", 12f, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.FromArgb(100, 100, 100)))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("?", font, brush, rect, sf);
            }
        }
    }
}
