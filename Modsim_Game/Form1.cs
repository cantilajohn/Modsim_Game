using System;
using System.Reflection.Metadata;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace Modsim_Game
{
    public partial class StatSimForm : Form
    {
        public StatSimForm()
        {
            InitializeComponent();
            BaseStats();

            // Wire all textboxes to accept only integers
            txtSTR.TB.KeyPress += IntegerOnly_KeyPress;
            txtAGI.TB.KeyPress += IntegerOnly_KeyPress;
            txtVIT.TB.KeyPress += IntegerOnly_KeyPress;
            txtINT.TB.KeyPress += IntegerOnly_KeyPress;
            txtDEX.TB.KeyPress += IntegerOnly_KeyPress;
            txtLUK.TB.KeyPress += IntegerOnly_KeyPress;
            txtBaseLevel.TB.KeyPress += IntegerOnly_KeyPress;
        }

        //Label values for the base stats and calculations
        void BaseStats()
        {

            //Label values for the base stats and calculations
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

            //UI changes for the TextBoxes
            txtAGI.TB.BackColor = Color.White;
            txtSTR.TB.BackColor = Color.White;
            txtVIT.TB.BackColor = Color.White;
            txtLUK.TB.BackColor = Color.White;
            txtDEX.TB.BackColor = Color.White;
            txtINT.TB.BackColor = Color.White;
            txtBaseLevel.TB.BackColor = Color.White;

            //Job Bonus BASE
            lblJobBonus1.Text = "0";
            lblJobBonus2.Text = "0";
            lblJobBonus3.Text = "0";
            lblJobBonus4.Text = "0";
            lblJobBonus5.Text = "0";
            lblJobBonus6.Text = "0";
        }
        private void UpdateAllStats()
        {
            // 1. Parse all inputs
            if (!int.TryParse(txtSTR.Text, out int str)) str = 1;
            if (!int.TryParse(txtAGI.Text, out int agi)) agi = 1;
            if (!int.TryParse(txtVIT.Text, out int vit)) vit = 1;
            if (!int.TryParse(txtINT.Text, out int intel)) intel = 1;
            if (!int.TryParse(txtDEX.Text, out int dex)) dex = 1;
            if (!int.TryParse(txtLUK.Text, out int luk)) luk = 1;
            if (!int.TryParse(txtBaseLevel.Text, out int baseLevel)) baseLevel = 1;

            // Get Job Level from ComboBox
            string job = cmbSelectJob.SelectedItem?.ToString() ?? "Novice";

            //   STAT POINTS & UI STATE  
            int totalAvailablePoints = CalculateTotalAvailablePoints(baseLevel);
            int currentPointsSpent = CalculateStatCost(str) + CalculateStatCost(dex) +
                                     CalculateStatCost(vit) + CalculateStatCost(intel) +
                                     CalculateStatCost(agi) + CalculateStatCost(luk);
            int remainingPoints = totalAvailablePoints - currentPointsSpent;


            // Calculate required points for next increment and update labels
            int GetReqPoints(int statVal) => ((statVal - 1) / 10) + 2;
            lblReqSTR.Text = GetReqPoints(str).ToString();
            lblReqAGI.Text = GetReqPoints(agi).ToString();
            lblReqINT.Text = GetReqPoints(intel).ToString();
            lblReqDEX.Text = GetReqPoints(dex).ToString();
            lblReqVIT.Text = GetReqPoints(vit).ToString();
            lblReqLUK.Text = GetReqPoints(luk).ToString();

            // Update remaining points label and color
            lblPointsRemaining.Text = remainingPoints.ToString();
            lblPointsRemaining.ForeColor = (remainingPoints < 0) ? Color.Red : Color.Black;
            bool isOverspent = remainingPoints < 0;
            txtSTR.TB.ReadOnly = txtDEX.TB.ReadOnly = txtVIT.TB.ReadOnly = isOverspent;
            txtLUK.TB.ReadOnly = txtINT.TB.ReadOnly = txtAGI.TB.ReadOnly = isOverspent;

            //   BASE JOB PROPERTIES  
            string selectedJob = cmbSelectJob.SelectedItem?.ToString() ?? "Novice";

            // Get Job Level Safely from your ComboBox
            int jLvl = cmbJobLevel.SelectedItem != null ? int.Parse(cmbJobLevel.SelectedItem.ToString()) : 1;

            //   3. Get Bonuses (Now completely universal!)  
            int bSTR = JobStatTable.GetBonus(selectedJob, "STR", jLvl);
            int bAGI = JobStatTable.GetBonus(selectedJob, "AGI", jLvl);
            int bVIT = JobStatTable.GetBonus(selectedJob, "VIT", jLvl);
            int bINT = JobStatTable.GetBonus(selectedJob, "INT", jLvl);
            int bDEX = JobStatTable.GetBonus(selectedJob, "DEX", jLvl);
            int bLUK = JobStatTable.GetBonus(selectedJob, "LUK", jLvl);

            // Total stats for formulas
            int tStr = str + bSTR;
            int tAgi = agi + bAGI;
            int tVit = vit + bVIT;
            int tInt = intel + bINT;
            int tDex = dex + bDEX;
            int tLuk = luk + bLUK;

            //  UPDATED CALCULATIONS 
            // HP Calculation
            int tableBaseHP = JobStatTable.GetMaxHP(selectedJob, baseLevel);
            double totalHp = tableBaseHP * (1 + (tVit * 0.01)); // Use tVit

            // MAX SP Calculation
            double SP_JOB = JobStatTable.GetSpJobModifier(selectedJob);
            double BASE_SP = jobBaseSP + (baseLevel * SP_JOB);
            int SP_MOD_A = 0;
            int SP_MOD_B = 0;
            double MAX_SP = Math.Floor(BASE_SP * (1 + tInt * 0.01)); // Use tInt
            MAX_SP += SP_MOD_A;
            MAX_SP = Math.Floor(MAX_SP * (1 + SP_MOD_B * 0.01));

            // Battle Stats
            int totalStrDamage = tStr + (int)Math.Pow(tStr / 10, 2); // Use tStr
            int totalWeightLimit = jobBaseWeight + (tStr * 30);
            
            // ASPD Calculation
            double btba = 1.0;
            string weapon = cmbWeapon.SelectedItem?.ToString() ?? "Hand";
            if (JobWeaponDelay.TryGetValue((selectedJob, weapon), out double delayVal))
            {
                btba = delayVal;
            }

            double wd = 50.0 * btba; // Weapon Delay
            double sm = 0.0;         // Speed Modifier from Potion (Set to 0 by default)

            // Calculate contributions with Math.Round as per formula
            double agiContrib = Math.Round((wd * tAgi) / 25.0);
            double dexContrib = Math.Round((wd * tDex) / 100.0);

            // ASPD = 200 - (WD - ([WD*AGI/25] + [WD*DEX/100]) / 10) * (1 - SM)
            double finalASPD = 200.0 - (wd - (agiContrib + dexContrib) / 10.0) * (1.0 - sm);
            
            double totalAspd = Math.Min(190.0, finalASPD); // Cap at 190 max
            
            double castReduction = Math.Min(100, (tDex / 150.0) * 100);

            // DEF AND MDEF (Matches image: Soft Def = Total VIT, Soft Mdef = Total INT)
            int def = tVit;
            int mdef = tInt;

            //  FINAL UI UPDATE  
            lblTotalHP.Text = Math.Floor(totalHp).ToString();
            lblTotalSp.Text = MAX_SP.ToString();

            // Update Bonus Labels (The "+" values)
            lblJobBonus1.Text = $"+{bSTR}";
            lblJobBonus2.Text = $"+{bAGI}";
            lblJobBonus3.Text = $"+{bVIT}";
            lblJobBonus4.Text = $"+{bINT}";
            lblJobBonus5.Text = $"+{bDEX}";
            lblJobBonus6.Text = $"+{bLUK}";
            lblValueDEF2.Text = def.ToString();
            lblMDEFValue2.Text = mdef.ToString();
            lblAtk1.Text = (totalStrDamage + (tDex / 5) + (tLuk / 5)).ToString();
            lblWeight.Text = totalWeightLimit.ToString();
            lblHit.Text = (baseLevel + tDex).ToString(); // RO Formula Level + DEX
            lblRangedAtk.Text = (tDex + (int)Math.Pow(tDex / 10, 2)).ToString();
            lblCastReduction.Text = $"{castReduction:F1}%";
            
            // HP Regen Calculation
            double hpr = 1.0 + Math.Floor(totalHp / 200.0);
            hpr += Math.Floor(tVit / 5.0);
            double hprMod = 0.0; // Placeholder for HP recovery modifiers
            hpr = Math.Floor(hpr * (1.0 + hprMod * 0.01));
            hpr = Math.Max(1.0, hpr);

            double spRegen = Math.Floor(MAX_SP / 100.0) + Math.Floor(tInt / 6.0) + 1.0;
            string spRegenValue = $"{spRegen} per 8s standing (per 4s sitting)"; 

            lblHpRegen.Text = hpr.ToString();
            lblSpRegen.Text = spRegenValue;
            lblMinMatk1.Text = (tInt + (int)Math.Pow(tInt / 7, 2)).ToString();
            lblMinMatk2.Text = (tInt + (int)Math.Pow(tInt / 5, 2)).ToString();
            lblFLEE1.Text = (baseLevel + tAgi).ToString(); // RO Formula Level + AGI
            lblASPD.Text = Math.Floor(totalAspd).ToString();
            lblCrit.Text = Math.Floor((tLuk * 0.3) + 1).ToString("F1");
            lblPerfectDodge.Text = $"{(tLuk * 0.1):F1}%";
        }

        // Helper method for cost progression
        private int CalculateStatCost(int targetValue)
        {
            int totalCost = 0;
            for (int i = 1; i < targetValue; i++)
            {
                totalCost += ((i - 1) / 10) + 2;
            }
            return totalCost;
        }
        //Calculate total available points based on the level progression
        private int CalculateTotalAvailablePoints(int level)
        {
            // Starting points at Level 1
            int totalPoints = 48;
            // Loop from Level 1 up to the current Level
            for (int i = 1; i < level; i++)
            {
                // Gain baseLVL / 5 + 3 points per level up
                totalPoints += (i / 5) + 3;
            }
            return totalPoints;
        }

        // Map every TextChanged event to the same method:
        private void txtSTR_TextChanged(object sender, EventArgs e) => UpdateAllStats();
        private void txtDex_TextChanged(object sender, EventArgs e) => UpdateAllStats();
        private void txtInt_TextChanged(object sender, EventArgs e) => UpdateAllStats();
        private void txtBaseLevel_TextChanged(object sender, EventArgs e) => UpdateAllStats();
        private void txtAgi_TextChanged(object sender, EventArgs e) => UpdateAllStats();
        private void txtVit_TextChanged_1(object sender, EventArgs e) => UpdateAllStats();
        private void txtLuk_TextChanged(object sender, EventArgs e) => UpdateAllStats();



        int jobBaseWeight = -30;
        int jobBaseSP; // Base SP at Level 1
        int jobASPDModifier = 0; //bonus based on the Class
        int weaponBaseASPD = 0; // Default base ASPD 

        private void aloneComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSelectJob.SelectedItem == null) return;

            string selectedJob = cmbSelectJob.SelectedItem.ToString();
            cmbWeapon.Items.Clear();
            cmbJobLevel.Items.Clear();

            switch (selectedJob)
            {
                case "Novice":
                    pbJobs.Image = Properties.Resources.noviceRagnarok;
                    cmbWeapon.Items.AddRange(new string[] { "Hand", "Dagger", "One-Handed-Sword", "One-Handed-Axe", "One-Handed-Mace", "Two-Handed-Mace", "Rod&Staff", "Two-Handed-Staff" });
                    cmbJobLevel.Items.Clear();
                    for (int i = 1; i <= 10; i++)
                    {
                        cmbJobLevel.Items.Add(i.ToString());
                    }
                    cmbJobLevel.SelectedIndex = 0; // Default to Job Level 1
                    jobBaseWeight = 2000;
                    jobBaseSP = 10; // Matches  table
                    lblJobTitle.Text = "Novice";
                    break;
                case "Swordsman":
                    pbJobs.Image = Properties.Resources.swordmanRagnarok;
                    cmbWeapon.Items.AddRange(new string[] { "Hand", "Dagger", "One-Handed-Sword", "Two-Handed-Sword", "One-Handed-Spear", "Two-Handed-Spear", "One-Handed-Axe", "Two-Handed-Axe", "One-Handed-Mace", "Two-Handed-Mace" });
                    cmbJobLevel.Items.Clear();
                    for (int i = 1; i <= 50; i++)
                    {
                        cmbJobLevel.Items.Add(i.ToString());
                    }
                    cmbJobLevel.SelectedIndex = 0; // Default to Job Level 1
                    jobBaseWeight = 2800;
                    jobBaseSP = 10; // Matches  table
                    lblJobTitle.Text = "Swordman";

                    break;
                case "Magician":
                    pbJobs.Image = Properties.Resources.magicianRagnarok;
                    cmbWeapon.Items.AddRange(new string[] { "Hand", "Dagger", "Rod&Staff", "Two-Handed-Staff" });
                    cmbJobLevel.Items.Clear();
                    for (int i = 1; i <= 50; i++)
                    {
                        cmbJobLevel.Items.Add(i.ToString());
                    }
                    cmbJobLevel.SelectedIndex = 0; // Default to Job Level 1
                    jobBaseWeight = 2200;
                    jobBaseSP = 10; // Matches  table
                    lblJobTitle.Text = "Magician";

                    break;
                case "Archer":
                    pbJobs.Image = Properties.Resources.archerRagnarok;
                    cmbWeapon.Items.AddRange(new string[] { "Hand", "Dagger", "Bow" });
                    cmbJobLevel.Items.Clear();
                    for (int i = 1; i <= 50; i++)
                    {
                        cmbJobLevel.Items.Add(i.ToString());
                    }
                    cmbJobLevel.SelectedIndex = 0; // Default to Job Level 1
                    jobBaseWeight = 2330;
                    jobBaseSP = 10; // Matches  table
                    lblJobTitle.Text = "Archer";

                    break;
                case "Acolyte":
                    pbJobs.Image = Properties.Resources.AcolyteRagnarok2;
                    cmbWeapon.Items.AddRange(new string[] { "Hand", "One-Handed-Mace", "Two-Handed-Mace", "Rod&Staff", "Two-Handed-Staff" });
                    cmbJobLevel.Items.Clear();
                    for (int i = 1; i <= 50; i++)
                    {
                        cmbJobLevel.Items.Add(i.ToString());
                    }
                    cmbJobLevel.SelectedIndex = 0; // Default to Job Level 1
                    jobBaseWeight = 2200;
                    jobBaseSP = 15; // Matches  table
                    lblJobTitle.Text = "Acolyte";

                    break;
                case "Merchant":
                    pbJobs.Image = Properties.Resources.merchantRagnarok;
                    cmbWeapon.Items.AddRange(new string[] { "Hand", "Dagger", "One-Handed-Sword", "One-Handed-Axe", "Two-Handed-Axe", "One-Handed-Mace", "Two-Handed-Mace" });
                    cmbJobLevel.Items.Clear();
                    for (int i = 1; i <= 50; i++)
                    {
                        cmbJobLevel.Items.Add(i.ToString());
                    }
                    cmbJobLevel.SelectedIndex = 0; // Default to Job Level 1
                    jobBaseWeight = 2500;
                    jobBaseSP = 10; // Matches  table
                    lblJobTitle.Text = "Merchant";

                    break;
                case "Thief":
                    pbJobs.Image = Properties.Resources.thiefRagnarok;
                    cmbWeapon.Items.AddRange(new string[] { "Hand", "Dagger", "One-Handed-Sword", "One-Handed-Axe", "Bow" });
                    cmbJobLevel.Items.Clear();
                    for (int i = 1; i <= 50; i++)
                    {
                        cmbJobLevel.Items.Add(i.ToString());
                    }
                    cmbJobLevel.SelectedIndex = 0; // Default to Job Level 1
                    jobBaseWeight = 2400;
                    jobBaseSP = 10; // Matches  table
                    lblJobTitle.Text = "Thief";

                    break;
            }

            if (cmbWeapon.Items.Count > 0) cmbWeapon.SelectedIndex = 0;
            UpdateAllStats();
        }
        // Used for ASPD delay lookup based on Job and Weapon combo
        // Calculating Base ASPD requires applying the class weapon modifier value against the logic: 200 - (50 * modifier)
        private readonly Dictionary<(string Job, string Weapon), double> JobWeaponDelay = new Dictionary<(string, string), double>
        {
            // NOVICE
            { ("Novice", "Hand"), 1.0 }, { ("Novice", "Dagger"), 1.3 }, { ("Novice", "One-Handed-Sword"), 1.4 }, { ("Novice", "One-Handed-Axe"), 1.6 }, { ("Novice", "One-Handed-Mace"), 1.4 }, { ("Novice", "Two-Handed-Mace"), 1.4 }, { ("Novice", "Rod&Staff"), 1.3 }, { ("Novice", "Two-Handed-Staff"), 1.3 },
            // SWORDSMAN 
            { ("Swordsman", "Hand"), 0.8 }, { ("Swordsman", "Dagger"), 1.0 }, { ("Swordsman", "One-Handed-Sword"), 1.1 }, { ("Swordsman", "Two-Handed-Sword"), 1.2 }, { ("Swordsman", "One-Handed-Spear"), 1.3 }, { ("Swordsman", "Two-Handed-Spear"), 1.4 }, { ("Swordsman", "One-Handed-Axe"), 1.4 }, { ("Swordsman", "Two-Handed-Axe"), 1.5 }, { ("Swordsman", "One-Handed-Mace"), 1.3 }, { ("Swordsman", "Two-Handed-Mace"), 1.4 },
            // MAGICIAN
            { ("Magician", "Hand"), 1.0 }, { ("Magician", "Dagger"), 1.2 }, { ("Magician", "Rod&Staff"), 1.4 }, { ("Magician", "Two-Handed-Staff"), 1.4 },
            // ARCHER
            { ("Archer", "Hand"), 0.8 }, { ("Archer", "Dagger"), 1.2 }, { ("Archer", "Bow"), 1.4 },
            // ACOLYTE
            { ("Acolyte", "Hand"), 0.8 }, { ("Acolyte", "One-Handed-Mace"), 1.2 }, { ("Acolyte", "Two-Handed-Mace"), 1.2 }, { ("Acolyte", "Rod&Staff"), 1.2 }, { ("Acolyte", "Two-Handed-Staff"), 1.2 },
            // MERCHANT
            { ("Merchant", "Hand"), 0.8 }, { ("Merchant", "Dagger"), 1.2 }, { ("Merchant", "One-Handed-Sword"), 1.4 }, { ("Merchant", "One-Handed-Axe"), 1.4 }, { ("Merchant", "Two-Handed-Axe"), 1.5 }, { ("Merchant", "One-Handed-Mace"), 1.4 }, { ("Merchant", "Two-Handed-Mace"), 1.4 },
            // THIEF
            { ("Thief", "Hand"), 0.8 }, { ("Thief", "Dagger"), 1.0 }, { ("Thief", "One-Handed-Sword"), 1.3 }, { ("Thief", "One-Handed-Axe"), 1.6 }, { ("Thief", "Bow"), 1.6 }
        };

        // When either Job or Weapon changes, we need to look up the new ASPD and update stats
        private void cmbWeapon_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSelectJob.SelectedItem == null || cmbWeapon.SelectedItem == null) return;

            string job = cmbSelectJob.SelectedItem.ToString();
            string weapon = cmbWeapon.SelectedItem.ToString();

            // 3. The "Magic" Lookup: Find the specific ASPD delay for this Job/Weapon combo
            if (JobWeaponDelay.TryGetValue((job, weapon), out double baseDelay))
            {
                // Convert delay multiplier to base ASPD (e.g. 1.0 -> 150, 0.8 -> 160)
                weaponBaseASPD = (int)(200 - (50 * baseDelay));
            }
            else
            {
                weaponBaseASPD = 100; // Default fallback
            }

            UpdateAllStats();
        }

        // Reset button to set all stats back to 1
        private void hopeButton1_Click(object sender, EventArgs e)
        {
            txtSTR.Text = "1";
            txtAGI.Text = "1";
            txtVIT.Text = "1";
            txtINT.Text = "1";
            txtDEX.Text = "1";
            txtLUK.Text = "1";
        }

        // Helper to sum up costs of all current stat values
        private int CalculateTotalSpent()
        {
            int.TryParse(txtSTR.Text, out int s);
            int.TryParse(txtAGI.Text, out int a);
            int.TryParse(txtVIT.Text, out int v);
            int.TryParse(txtINT.Text, out int i);
            int.TryParse(txtDEX.Text, out int d);
            int.TryParse(txtLUK.Text, out int l);
            return CalculateStatCost(s) + CalculateStatCost(a) + CalculateStatCost(v) +
                   CalculateStatCost(i) + CalculateStatCost(d) + CalculateStatCost(l);
        }

        private void IncrementStat(Control txtStat)
        {
            if (!int.TryParse(txtStat.Text, out int currentStat)) currentStat = 1;
            if (!int.TryParse(txtBaseLevel.Text, out int baseLevel)) baseLevel = 1;

            int costForNextPoint = ((currentStat - 1) / 10) + 2;

            int totalAvailable = CalculateTotalAvailablePoints(baseLevel);
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

        // Increment buttons
        private void bigLabel2_Click(object sender, EventArgs e) => IncrementStat(txtSTR);
        private void bigLabel11_Click(object sender, EventArgs e) => IncrementStat(txtAGI);
        private void bigLabel12_Click(object sender, EventArgs e) => IncrementStat(txtVIT);
        private void bigLabel15_Click(object sender, EventArgs e) => IncrementStat(txtINT);
        private void bigLabel13_Click(object sender, EventArgs e) => IncrementStat(txtLUK);
        private void bigLabel14_Click(object sender, EventArgs e) => IncrementStat(txtDEX);

        // Decrement buttons
        private void bigLabel45_Click(object sender, EventArgs e) => DecrementStat(txtSTR);
        private void bigLabel37_Click(object sender, EventArgs e) => DecrementStat(txtAGI);
        private void bigLabel36_Click(object sender, EventArgs e) => DecrementStat(txtVIT);
        private void bigLabel35_Click(object sender, EventArgs e) => DecrementStat(txtINT);
        private void bigLabel34_Click_1(object sender, EventArgs e) => DecrementStat(txtDEX);
        private void bigLabel18_Click(object sender, EventArgs e) => DecrementStat(txtLUK);
        // Whenever the Job Level changes, we need to recalculate bonuses and stats
        private void cmbJobLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAllStats();
        }

        private void IntegerOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check if the character is NOT a digit and NOT a control key (like Backspace)
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // This "swallows" the keypress so it never appears
            }
        }
    }
}