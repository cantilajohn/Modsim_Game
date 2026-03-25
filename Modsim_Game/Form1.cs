using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using Modsim_Game.Jobs;
using Modsim_Game.Models;
using Modsim_Game.Services;
using Modsim_Game.Data;

namespace Modsim_Game
{
    public partial class StatSimForm : Form
    {
        // ── Skill Simulator state ──
        private JobSkillTree? _currentSkillTree;
        private Modsim_Game.Controls.SkillTreePanel? _skillTreePanel;
        private Panel? _skillSidebar;
        private Label? _lblSkillPointsInfo;

        public StatSimForm()
        {
            InitializeComponent();
            BaseStats();

            SkillsBackPanel.Hide();

            // Wire all textboxes to accept only integers
            txtSTR.TB.KeyPress += IntegerOnly_KeyPress;
            txtAGI.TB.KeyPress += IntegerOnly_KeyPress;
            txtVIT.TB.KeyPress += IntegerOnly_KeyPress;
            txtINT.TB.KeyPress += IntegerOnly_KeyPress;
            txtDEX.TB.KeyPress += IntegerOnly_KeyPress;
            txtLUK.TB.KeyPress += IntegerOnly_KeyPress;
            txtBaseLevel.TB.KeyPress += IntegerOnly_KeyPress;

            // Wire skills panel job selector
            aloneComboBox1.SelectedIndexChanged += SkillJobSelector_SelectedIndexChanged;
        }

        void BaseStats()
        {
            txtSTR.Text = "1";
            txtAGI.Text = "1";
            txtVIT.Text = "1";
            txtINT.Text = "1";
            txtDEX.Text = "1";
            txtLUK.Text = "1";
            txtBaseLevel.Text = "1";
            lblHit.Text = "0";
            lblValueDEF2.Text = "0";
            lblValueDEF1.Text = "0";
            lblReqSTR.Text = "0";
            lblReqAGI.Text = "0";
            lblReqVIT.Text = "0";
            lblReqINT.Text = "0";
            lblReqDEX.Text = "0";
            lblReqLUK.Text = "0";
            lblAtk1.Text = "0";
            lblAtk2.Text = "0";
            lblMinMatk1.Text = "0";
            lblMinMatk2.Text = "0";
            lblFLEE1.Text = "0";
            lblFLEE2.Text = "0";
            lblCrit.Text = "0";
            lblMDEFValue1.Text = "0";
            lblMDEFValue2.Text = "0";
            lblSpRegen.Text = "0";
            lblHpRegen.Text = "0";

            txtAGI.TB.BackColor = Color.White;
            txtSTR.TB.BackColor = Color.White;
            txtVIT.TB.BackColor = Color.White;
            txtLUK.TB.BackColor = Color.White;
            txtDEX.TB.BackColor = Color.White;
            txtINT.TB.BackColor = Color.White;
            txtBaseLevel.TB.BackColor = Color.White;

            lblJobBonus1.Text = "0";
            lblJobBonus2.Text = "0";
            lblJobBonus3.Text = "0";
            lblJobBonus4.Text = "0";
            lblJobBonus5.Text = "0";
            lblJobBonus6.Text = "0";
        }

        private void UpdateAllStats()
        {
            if (!int.TryParse(txtSTR.Text, out int str)) str = 1;
            if (!int.TryParse(txtAGI.Text, out int agi)) agi = 1;
            if (!int.TryParse(txtVIT.Text, out int vit)) vit = 1;
            if (!int.TryParse(txtINT.Text, out int intel)) intel = 1;
            if (!int.TryParse(txtDEX.Text, out int dex)) dex = 1;
            if (!int.TryParse(txtLUK.Text, out int luk)) luk = 1;
            if (!int.TryParse(txtBaseLevel.Text, out int baseLevel)) baseLevel = 1;

            int jLvl = cmbJobLevel.SelectedItem != null ? int.Parse(cmbJobLevel.SelectedItem.ToString()) : 1;
            string selectedJobName = cmbSelectJob.SelectedItem?.ToString() ?? "Novice";
            string weaponName = cmbWeapon.SelectedItem?.ToString() ?? "Hand";

            // Assemble CharacterInfo
            var charInfo = new CharacterInfo
            {
                BaseLevel = baseLevel,
                JobLevel = jLvl,
                Str = str,
                Agi = agi,
                Vit = vit,
                Int = intel,
                Dex = dex,
                Luk = luk,
                JobName = selectedJobName,
                WeaponName = weaponName
            };

            //  get Job & Calculate
            var jobClass = JobFactory.GetJob(selectedJobName);
            var calcService = new StatCalculatorService();
            var stats = calcService.Calculate(charInfo, jobClass);

            // Update UI
            lblReqSTR.Text = stats.RequiredStrNext.ToString();
            lblReqAGI.Text = stats.RequiredAgiNext.ToString();
            lblReqINT.Text = stats.RequiredIntNext.ToString();
            lblReqDEX.Text = stats.RequiredDexNext.ToString();
            lblReqVIT.Text = stats.RequiredVitNext.ToString();
            lblReqLUK.Text = stats.RequiredLukNext.ToString();

            lblPointsRemaining.Text = stats.PointsRemaining.ToString();
            lblPointsRemaining.ForeColor = (stats.PointsRemaining < 0) ? Color.Red : Color.Black;
            bool isOverspent = stats.PointsRemaining < 0;
            txtSTR.TB.ReadOnly = txtDEX.TB.ReadOnly = txtVIT.TB.ReadOnly = isOverspent;
            txtLUK.TB.ReadOnly = txtINT.TB.ReadOnly = txtAGI.TB.ReadOnly = isOverspent;

            lblTotalHP.Text = stats.MaxHp.ToString();
            lblTotalSp.Text = stats.MaxSp.ToString();

            lblJobBonus1.Text = $"+{stats.BonusStr}";
            lblJobBonus2.Text = $"+{stats.BonusAgi}";
            lblJobBonus3.Text = $"+{stats.BonusVit}";
            lblJobBonus4.Text = $"+{stats.BonusInt}";
            lblJobBonus5.Text = $"+{stats.BonusDex}";
            lblJobBonus6.Text = $"+{stats.BonusLuk}";

            lblValueDEF2.Text = stats.Def.ToString();
            lblMDEFValue2.Text = stats.Mdef.ToString();
            lblAtk1.Text = stats.Atk.ToString();
            lblWeight.Text = stats.WeightLimit.ToString();
            lblHit.Text = stats.Hit.ToString();
            lblRangedAtk.Text = stats.RangedAtk.ToString();
            lblCastReduction.Text = $"{stats.CastReductionPercent:F1}%";

            lblHpRegen.Text = stats.HpRegen.ToString() + "per 6s standing (per 3s sitting)";
            lblSpRegen.Text = $"{stats.SpRegen} per 8s standing (per 4s sitting)";

            lblMinMatk1.Text = stats.MinMatk1.ToString();
            lblMinMatk2.Text = stats.MinMatk2.ToString();
            lblFLEE1.Text = stats.Flee.ToString();
            lblASPD.Text = Math.Floor(stats.Aspd).ToString();
            lblCrit.Text = stats.Crit.ToString("F1");
            lblPerfectDodge.Text = $"{stats.PerfectDodge:F1}%";
        }

        private void txtSTR_TextChanged(object sender, EventArgs e) => UpdateAllStats();
        private void txtDex_TextChanged(object sender, EventArgs e) => UpdateAllStats();
        private void txtInt_TextChanged(object sender, EventArgs e) => UpdateAllStats();
        private void txtBaseLevel_TextChanged(object sender, EventArgs e) => UpdateAllStats();
        private void txtAgi_TextChanged(object sender, EventArgs e) => UpdateAllStats();
        private void txtVit_TextChanged_1(object sender, EventArgs e) => UpdateAllStats();
        private void txtLuk_TextChanged(object sender, EventArgs e) => UpdateAllStats();

        private void aloneComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSelectJob.SelectedItem == null) return;
            string selectedJob = cmbSelectJob.SelectedItem.ToString();

            var job = JobFactory.GetJob(selectedJob);

            cmbWeapon.Items.Clear();
            cmbJobLevel.Items.Clear();

            int maxJobLevel = (job.Name == "Novice") ? 10 : 50;
            for (int i = 1; i <= maxJobLevel; i++)
            {
                cmbJobLevel.Items.Add(i.ToString());
            }
            cmbJobLevel.SelectedIndex = 0;

            cmbWeapon.Items.AddRange(job.AllowedWeapons);
            if (cmbWeapon.Items.Count > 0) cmbWeapon.SelectedIndex = 0;

            lblJobTitle.Text = job.Name;

            pbJobs.Image = job.Name switch
            {
                "Novice" => Properties.Resources.noviceRagnarok,
                "Swordsman" => Properties.Resources.swordmanRagnarok,
                "Magician" => Properties.Resources.magicianRagnarok,
                "Archer" => Properties.Resources.archerRagnarok,
                "Acolyte" => Properties.Resources.AcolyteRagnarok2,
                "Merchant" => Properties.Resources.merchantRagnarok,
                "Thief" => Properties.Resources.thiefRagnarok,
                _ => Properties.Resources.noviceRagnarok
            };

            UpdateAllStats();
        }

        private void cmbWeapon_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSelectJob.SelectedItem == null || cmbWeapon.SelectedItem == null) return;
            UpdateAllStats();
        }

        private void hopeButton1_Click(object sender, EventArgs e)
        {
            txtSTR.Text = "1";
            txtAGI.Text = "1";
            txtVIT.Text = "1";
            txtINT.Text = "1";
            txtDEX.Text = "1";
            txtLUK.Text = "1";
        }

        private int CalculateTotalSpent()
        {
            int.TryParse(txtSTR.Text, out int s);
            int.TryParse(txtAGI.Text, out int a);
            int.TryParse(txtVIT.Text, out int v);
            int.TryParse(txtINT.Text, out int i);
            int.TryParse(txtDEX.Text, out int d);
            int.TryParse(txtLUK.Text, out int l);
            return ProgressionService.CalculateStatCost(s) + ProgressionService.CalculateStatCost(a) + ProgressionService.CalculateStatCost(v) +
                   ProgressionService.CalculateStatCost(i) + ProgressionService.CalculateStatCost(d) + ProgressionService.CalculateStatCost(l);
        }

        private void IncrementStat(Control txtStat)
        {
            if (!int.TryParse(txtStat.Text, out int currentStat)) currentStat = 1;
            if (!int.TryParse(txtBaseLevel.Text, out int baseLevel)) baseLevel = 1;

            int costForNextPoint = ProgressionService.GetRequiredPointsForNextLevel(currentStat);

            int totalAvailable = ProgressionService.CalculateTotalAvailablePoints(baseLevel);
            int currentSpent = CalculateTotalSpent();

            if (currentSpent + costForNextPoint <= totalAvailable)
            {
                txtStat.Text = (currentStat + 1).ToString();
            }
            else
            {
                lblPointsRemaining.ForeColor = Color.Red;
            }
        }

        private void DecrementStat(Control txtStat)
        {
            if (!int.TryParse(txtStat.Text, out int currentStat)) currentStat = 1;
            int nextStat = currentStat - 1;
            if (nextStat < 1) nextStat = 1; // RO stats typically don't drop below 1
            txtStat.Text = nextStat.ToString();
        }

        private void bigLabel2_Click(object sender, EventArgs e) => IncrementStat(txtSTR);
        private void bigLabel11_Click(object sender, EventArgs e) => IncrementStat(txtAGI);
        private void bigLabel12_Click(object sender, EventArgs e) => IncrementStat(txtVIT);
        private void bigLabel15_Click(object sender, EventArgs e) => IncrementStat(txtINT);
        private void bigLabel13_Click(object sender, EventArgs e) => IncrementStat(txtLUK);
        private void bigLabel14_Click(object sender, EventArgs e) => IncrementStat(txtDEX);

        private void bigLabel45_Click(object sender, EventArgs e) => DecrementStat(txtSTR);
        private void bigLabel37_Click(object sender, EventArgs e) => DecrementStat(txtAGI);
        private void bigLabel36_Click(object sender, EventArgs e) => DecrementStat(txtVIT);
        private void bigLabel35_Click(object sender, EventArgs e) => DecrementStat(txtINT);
        private void bigLabel34_Click_1(object sender, EventArgs e) => DecrementStat(txtDEX);
        private void bigLabel18_Click(object sender, EventArgs e) => DecrementStat(txtLUK);

        private void cmbJobLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAllStats();
        }

        private void IntegerOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        // ── Navigate to Skills Panel ──
        private void hopeButton1_Click_1(object sender, EventArgs e)
        {
            mainPanel.Hide();
            secondPanel.Hide();
            thirdPanel.Hide();
            pnlStatusSimulatorControls.Hide();
            SkillsBackPanel.Show();
        }

        // ── Navigate back from Skills Panel ──
        private void hopeButton2_Click(object sender, EventArgs e)
        {
            mainPanel.Show();
            secondPanel.Show();
            thirdPanel.Show();
            pnlStatusSimulatorControls.Show();
            SkillsBackPanel.Hide();
        }


        //  SKILLS SIMULATOR — Dynamic Panel UI 
        private void SkillJobSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (aloneComboBox1.SelectedItem == null) return;
            string selected = aloneComboBox1.SelectedItem.ToString();
            if (selected == "-SELECT CLASS") return;

            var job = JobFactory.GetJob(selected);
            _currentSkillTree = job.GetSkillTree();
            RebuildSkillsUI();
        }

        /// <summary>
        /// Clears and rebuilds the skills panel with the GDI+ SkillTreePanel.
        /// </summary>
        private void RebuildSkillsUI()
        {
            // Remove old  controls
            var toRemove = new List<Control>();
            foreach (Control c in SkillsBackPanel.Controls)
            {
                if (c != bigLabel2 && c != aloneComboBox1 && c != hopeButton2)
                    toRemove.Add(c);
            }
            foreach (var c in toRemove) { SkillsBackPanel.Controls.Remove(c); c.Dispose(); }

            if (_currentSkillTree == null) return;

            // ── Section Header ──
            var lblJobHeader = new Label
            {
                Text = $"{_currentSkillTree.JobLabel}",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point(20, 100)
            };
            SkillsBackPanel.Controls.Add(lblJobHeader);

            // ── Job Level ComboBox ──
            var lblJobLvl = new Label
            {
                Text = "Job level:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(180, 180, 180),
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point(lblJobHeader.Right + 10, 102)
            };
            SkillsBackPanel.Controls.Add(lblJobLvl);

            var cmbSkillJobLvl = new ComboBox
            {
                Location = new Point(lblJobLvl.Right + 5, 100),
                Size = new Size(60, 22),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.White,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat
            };

            // Populate based on job — Novice is 1-9, others are 1-50
            int endLvl = (_currentSkillTree.JobLabel == "Novice") ? 9 : 50;
            for (int i = 1; i <= endLvl; i++) cmbSkillJobLvl.Items.Add(i);

            cmbSkillJobLvl.SelectedItem = endLvl;
            _currentSkillTree.JobLevel = endLvl;

            cmbSkillJobLvl.SelectedIndexChanged += (s, ev) =>
            {
                if (cmbSkillJobLvl.SelectedItem is int lvl)
                {
                    _currentSkillTree.JobLevel = lvl;
                    UpdateSkillPointsLabel();
                    _skillTreePanel?.Invalidate(); // Refresh visuals if needed
                }
            };
            SkillsBackPanel.Controls.Add(cmbSkillJobLvl);

            // ── Skill Points Info ──
            _lblSkillPointsInfo = new Label
            {
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(180, 180, 180),
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(20, 143)
            };
            SkillsBackPanel.Controls.Add(_lblSkillPointsInfo);
            UpdateSkillPointsLabel();

            int sidebarWidth = 340;
            int sidebarX = Math.Max(960, SkillsBackPanel.Width - sidebarWidth - 20);

            // Calculate vertical bounds precisely above the Back button
            int bottomY = hopeButton2.Location.Y > 0 ? hopeButton2.Location.Y : 872;
            int maxPanelBottom = bottomY - 15;

            // ── Skill Sidebar (Description) ──
            _skillSidebar = new Panel
            {
                Location = new Point(sidebarX, 40),
                Size = new Size(sidebarWidth, maxPanelBottom - 40),
                BackColor = Color.FromArgb(240, 240, 240),
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false // Initially hidden until selection
            };
            SkillsBackPanel.Controls.Add(_skillSidebar);
            _skillSidebar.BringToFront(); // Ensure it's never overshadowed

            // ── The Skill Tree Scroll Container ──
            var skillTreeContainer = new Panel
            {
                Location = new Point(20, 160),
                Size = new Size(sidebarX - 40, maxPanelBottom - 160),
                AutoScroll = true,
                BorderStyle = BorderStyle.FixedSingle
            };
            SkillsBackPanel.Controls.Add(skillTreeContainer);

            // ── The Skill Tree Panel ──
            _skillTreePanel = new Modsim_Game.Controls.SkillTreePanel
            {
                Location = new Point(0, 0),
                TargetWidth = skillTreeContainer.Width - 20 // Account for scrollbar space
            };
            _skillTreePanel.LoadSkillTree(_currentSkillTree);
            _skillTreePanel.SkillChanged += (s, ev) => UpdateSkillPointsLabel();
            _skillTreePanel.NodeSelected += (s, node) => UpdateSkillSidebar(node);
            skillTreeContainer.Controls.Add(_skillTreePanel);

            // Set the panel as the ActiveControl of the form
            this.ActiveControl = skillTreeContainer;

            // ── Controls Help Label ──
            var lblHelp = new Label
            {
                Text = "[L-Click: Add/Unlock]  [R-Click: Subtract]  [Click: View Details]",
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                ForeColor = Color.FromArgb(140, 140, 140),
                AutoSize = true,
                Location = new Point(20, SkillsBackPanel.Height - 18)
            };
            SkillsBackPanel.Controls.Add(lblHelp);

            // ── Reset Skills Button ──
            var btnReset = new Button
            {
                Text = "⟲  Reset Skills",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Size = new Size(140, 32),
                Location = new Point(hopeButton2.Left - 140 - 15, bottomY),
                BackColor = Color.FromArgb(200, 70, 70),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
            };
            btnReset.FlatAppearance.BorderColor = Color.FromArgb(70, 70, 78);
            btnReset.FlatAppearance.BorderSize = 1;
            btnReset.Click += (s, ev) =>
            {
                string sel = aloneComboBox1.SelectedItem?.ToString();
                if (sel != null && sel != "-SELECT CLASS")
                {
                    _currentSkillTree = JobFactory.GetJob(sel).GetSkillTree();
                    _skillTreePanel.LoadSkillTree(_currentSkillTree);
                    UpdateSkillPointsLabel();
                    UpdateSkillSidebar(null); // Clear description panel
                }
            };
            SkillsBackPanel.Controls.Add(btnReset);
        }

        private void UpdateSkillPointsLabel()
        {
            if (_lblSkillPointsInfo == null || _currentSkillTree == null) return;
            int remaining = _currentSkillTree.SkillPointsRemaining;
            _lblSkillPointsInfo.Text =
                $"{_currentSkillTree.JobLabel} Skill Points Used: {_currentSkillTree.SkillPointsUsed}     " +
                $"Remaining: {remaining}";

            // Turn red if 0 or negative
            _lblSkillPointsInfo.ForeColor = remaining <= 0
                ? Color.FromArgb(220, 70, 70) // Bright red
                : Color.FromArgb(180, 180, 180);
        }

        private void panel8_Paint(object sender, PaintEventArgs e)
        {

        }
        private void UpdateSkillSidebar(SkillNode? node)
        {
            if (_skillSidebar == null) return;
            _skillSidebar.Controls.Clear();
            if (node == null)
            {
                _skillSidebar.Visible = false;
                return;
            }

            _skillSidebar.Visible = true;
            _skillSidebar.AutoScroll = true;

            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(5, 10, 10, 10),
                BackColor = SystemColors.ActiveBorder
            };
            _skillSidebar.Controls.Add(flow);

            // 1. Skill Icon (New, at the top)
            var iconContainer = new Panel { Width = 300, Height = 60, Margin = new Padding(0) };
            if (node.Icon != null)
            {
                var picIcon = new PictureBox
                {
                    Image = node.Icon,
                    Size = new Size(48, 48),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Location = new Point((iconContainer.Width - 48) / 2, 0)
                };
                iconContainer.Controls.Add(picIcon);
            }
            else
            {
                var lblPlace = new Label
                {
                    Text = "?",
                    Font = new Font("Segoe UI", 24, FontStyle.Bold),
                    ForeColor = Color.Gray,
                    Size = new Size(48, 48),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Location = new Point((iconContainer.Width - 48) / 2, 0),
                    BorderStyle = BorderStyle.FixedSingle
                };
                iconContainer.Controls.Add(lblPlace);
            }
            flow.Controls.Add(iconContainer);

            // 2. Skill Name Header
            var lblName = new Label
            {
                Text = node.Name,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 2),
                Width = 300,
                TextAlign = ContentAlignment.TopCenter
            };
            flow.Controls.Add(lblName);

            // Get Description Data
            var desc = SkillDescriptionRepository.Get(node.Name);

            // Skill Type Label
            var lblType = new Label
            {
                Text = $"Type: {desc.Type}",
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.FromArgb(120, 120, 120),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 10),
                Width = 300,
                TextAlign = ContentAlignment.TopCenter
            };
            flow.Controls.Add(lblType);

            // Level Controls (+ / -) — only for unlocked, non-quest skills
            if (!node.IsLocked && node.Type != "quest")
            {
                var levelFlow = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.LeftToRight, Margin = new Padding(50, 0, 0, 15) };
                var btnPlus = new Button { Text = "+", Size = new Size(25, 25), BackColor = Color.FromArgb(40, 180, 100), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
                var lblLv = new Label { Text = $"Lv {node.CurrentLevel}/{node.MaxLevel}", Font = new Font("Segoe UI", 10), AutoSize = true, Margin = new Padding(5, 4, 5, 0) };
                var btnMinus = new Button { Text = "-", Size = new Size(25, 25), BackColor = Color.FromArgb(200, 60, 60), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

                btnPlus.Click += (s, e) =>
                {
                    if (_skillTreePanel != null)
                    {
                        _skillTreePanel.IncrementLevel(node);
                        var fresh = _skillTreePanel.AllNodes.FirstOrDefault(n => n.Name == node.Name);
                        UpdateSkillSidebar(fresh ?? node);
                    }
                };
                btnMinus.Click += (s, e) =>
                {
                    if (_skillTreePanel != null)
                    {
                        _skillTreePanel.DecrementLevel(node);
                        var fresh = _skillTreePanel.AllNodes.FirstOrDefault(n => n.Name == node.Name);
                        UpdateSkillSidebar(fresh ?? node);
                    }
                };

                levelFlow.Controls.Add(btnPlus);
                levelFlow.Controls.Add(lblLv);
                levelFlow.Controls.Add(btnMinus);
                flow.Controls.Add(levelFlow);
            }
            else if (node.IsLocked)
            {
                // Show locked status with level info
                var lblLocked = new Label
                {
                    Text = $"🔒  LOCKED  (Max Lv {node.MaxLevel})",
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = Color.FromArgb(200, 70, 70),
                    AutoSize = true,
                    Margin = new Padding(0, 0, 0, 5),
                    Width = 300,
                    TextAlign = ContentAlignment.TopCenter
                };
                flow.Controls.Add(lblLocked);

                // Show requirement details
                if (!string.IsNullOrEmpty(node.Requirement))
                {
                    var lblReqHeader = new Label
                    {
                        Text = "Requirements:",
                        Font = new Font("Segoe UI", 9, FontStyle.Bold),
                        ForeColor = Color.FromArgb(180, 130, 50),
                        AutoSize = true,
                        Margin = new Padding(0, 0, 0, 2)
                    };
                    flow.Controls.Add(lblReqHeader);

                    var lblReqText = new Label
                    {
                        Text = node.Requirement,
                        Font = new Font("Segoe UI", 9),
                        ForeColor = Color.FromArgb(100, 100, 100),
                        AutoSize = true,
                        MaximumSize = new Size(280, 0),
                        Margin = new Padding(10, 0, 0, 10)
                    };
                    flow.Controls.Add(lblReqText);
                }

                // Auto-Fulfill Requirements Button
                var btnAutoFulfill = new Button
                {
                    Text = "⚡ Auto-Fulfill Requirements",
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Size = new Size(280, 38),
                    BackColor = Color.FromArgb(50, 130, 200),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Margin = new Padding(0, 5, 0, 10)
                };
                btnAutoFulfill.FlatAppearance.BorderColor = Color.FromArgb(40, 100, 170);
                btnAutoFulfill.FlatAppearance.BorderSize = 1;

                btnAutoFulfill.Click += (s, e) =>
                {
                    if (_skillTreePanel != null)
                    {
                        bool success = _skillTreePanel.AutoFulfillRequirements(node);
                        if (success)
                        {
                            UpdateSkillPointsLabel();
                            // Refresh sidebar with the now-unlocked node
                            var fresh = _skillTreePanel.AllNodes.FirstOrDefault(n => n.Name == node.Name);
                            UpdateSkillSidebar(fresh);
                        }
                        else
                        {
                            // Show failure feedback
                            btnAutoFulfill.Text = "❌ Not Enough Skill Points!";
                            btnAutoFulfill.BackColor = Color.FromArgb(180, 50, 50);
                            var timer = new System.Windows.Forms.Timer { Interval = 2000 };
                            timer.Tick += (ts, te) =>
                            {
                                btnAutoFulfill.Text = "⚡ Auto-Fulfill Requirements";
                                btnAutoFulfill.BackColor = Color.FromArgb(50, 130, 200);
                                timer.Stop();
                                timer.Dispose();
                            };
                            timer.Start();
                        }
                    }
                };
                flow.Controls.Add(btnAutoFulfill);
            }

            // (Data already fetched above for the Type label)

            // 2. Skill Description Box
            var descBox = new FlowLayoutPanel { Width = 295, MinimumSize = new Size(295, 0), AutoSize = true, BorderStyle = BorderStyle.FixedSingle, FlowDirection = FlowDirection.TopDown, WrapContents = false, Margin = new Padding(0, 0, 0, 10), Padding = new Padding(8), BackColor = Color.White };
            descBox.Controls.Add(new Label { Text = "Skill Description:", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.FromArgb(100, 160, 255), AutoSize = true });
            descBox.Controls.Add(new Label { Text = desc.Description, Font = new Font("Segoe UI", 9), ForeColor = Color.FromArgb(80, 80, 80), AutoSize = true, MaximumSize = new Size(270, 0) });
            flow.Controls.Add(descBox);

            // 2.5 Required For Box
            if (desc.RequiredFor != null && desc.RequiredFor.Count > 0)
            {
                var reqBox = new FlowLayoutPanel { Width = 295, MinimumSize = new Size(295, 0), AutoSize = true, BorderStyle = BorderStyle.FixedSingle, FlowDirection = FlowDirection.TopDown, WrapContents = false, Margin = new Padding(0, 0, 0, 10), Padding = new Padding(8), BackColor = Color.White };
                reqBox.Controls.Add(new Label { Text = "Required for:", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.FromArgb(200, 60, 60), AutoSize = true });
                foreach (var req in desc.RequiredFor)
                {
                    reqBox.Controls.Add(new Label { Text = "• " + req, Font = new Font("Segoe UI", 8.5f), ForeColor = Color.FromArgb(80, 80, 80), AutoSize = true });
                }
                flow.Controls.Add(reqBox);
            }

            // 3. Skill Effects Box
            var effectBox = new FlowLayoutPanel { Width = 295, MinimumSize = new Size(295, 0), AutoSize = true, BorderStyle = BorderStyle.FixedSingle, FlowDirection = FlowDirection.TopDown, WrapContents = false, Margin = new Padding(0, 0, 0, 10), Padding = new Padding(8), BackColor = Color.White };
            effectBox.Controls.Add(new Label { Text = "Skill Effects:", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.FromArgb(100, 160, 255), AutoSize = true });

            // Calculate current effects
            int hp = int.TryParse(lblTotalHP.Text, out var h) ? h : 0;
            int sp = int.TryParse(lblTotalSp.Text, out var spVal) ? spVal : 0;

            var currentEffects = desc.EffectCalculator?.Invoke(node.CurrentLevel, hp, sp);
            if (currentEffects != null && currentEffects.Count > 0)
            {
                foreach (var eff in currentEffects)
                {
                    var effLabel = new Label
                    {
                        Text = $"{eff.Label}: {eff.Value}",
                        Font = new Font("Segoe UI", 9),
                        ForeColor = eff.Locked ? Color.DarkGray : Color.FromArgb(80, 80, 80),
                        AutoSize = true,
                        MaximumSize = new Size(270, 0)
                    };
                    effectBox.Controls.Add(effLabel);
                }
            }
            else
            {
                effectBox.Controls.Add(new Label { Text = "(No data for current level)", Font = new Font("Segoe UI", 8, FontStyle.Italic), ForeColor = Color.Gray, AutoSize = true });
            }
            flow.Controls.Add(effectBox);
        }
    }
}
