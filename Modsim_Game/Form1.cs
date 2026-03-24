using System;
using System.Reflection.Metadata;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using Modsim_Game.Jobs;
using Modsim_Game.Models;
using Modsim_Game.Services;

namespace Modsim_Game
{
    public partial class StatSimForm : Form
    {

        public StatSimForm()
        {
            InitializeComponent();

            cmbSkillSimulator.SelectedIndexChanged += CmbSkillSimulator_SelectedIndexChanged;

            BaseStats();

            skillsPanel.Hide();

            // Wire all textboxes to accept only integers
            txtSTR.TB.KeyPress += IntegerOnly_KeyPress;
            txtAGI.TB.KeyPress += IntegerOnly_KeyPress;
            txtVIT.TB.KeyPress += IntegerOnly_KeyPress;
            txtINT.TB.KeyPress += IntegerOnly_KeyPress;
            txtDEX.TB.KeyPress += IntegerOnly_KeyPress;
            txtLUK.TB.KeyPress += IntegerOnly_KeyPress;
            txtBaseLevel.TB.KeyPress += IntegerOnly_KeyPress;
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

        private void CmbSkillSimulator_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlContentHolder.Controls.Clear();

            string selectedClass = cmbSkillSimulator.SelectedItem?.ToString();

            switch (selectedClass)
            {
                case "Novice":
                    //Skill Unlocked
                    Novice noviceControl = new Novice();
                    noviceControl.Dock = DockStyle.Fill;
                    pnlContentHolder.Controls.Add(noviceControl);

                    break;
                case "Swordsman":
                    Swordman swordmanControl = new Swordman();
                    swordmanControl.Dock = DockStyle.Fill;
                    pnlContentHolder.Controls.Add(swordmanControl);
                    break;
                case "Magician":
                    Magician magicianControl = new Magician();
                    magicianControl.Dock = DockStyle.Fill;
                    pnlContentHolder.Controls.Add(magicianControl);
                    break;
                case "Archer":
                    Archer archerControl = new Archer();
                    archerControl.Dock = DockStyle.Fill;
                    pnlContentHolder.Controls.Add(archerControl);
                    break;
                case "Acolyte":
                    Acolyte acolyteControl = new Acolyte();
                    acolyteControl.Dock = DockStyle.Fill;
                    pnlContentHolder.Controls.Add(acolyteControl);
                    break;
                case "Merchant":
                    Merchant merchantControl = new Merchant();
                    merchantControl.Dock = DockStyle.Fill;
                    pnlContentHolder.Controls.Add(merchantControl);
                    break;
                case "Thief":
                    Thief thiefControl = new Thief();
                    thiefControl.Dock = DockStyle.Fill;
                    pnlContentHolder.Controls.Add(thiefControl);
                    break;
                default:
                    // If "-SELECT CLASS-" or anything else is selected, left empty as controls are already cleared
                    break;
            }
        }

        private void IntegerOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void hopeButton1_Click_1(object sender, EventArgs e)
        {
            mainPanel.Hide();
            secondPanel.Hide();
            thirdPanel.Hide();
            pnlStatusSimulatorControls.Hide();
            skillsPanel.Show();
        }
    }
}