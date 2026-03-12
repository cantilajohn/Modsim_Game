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
            txtAGI.TB.BackColor = Color.White;
            txtSTR.TB.BackColor = Color.White;
            txtVIT.TB.BackColor = Color.White;
            txtLUK.TB.BackColor = Color.White;
            txtDEX.TB.BackColor = Color.White;
            txtINT.TB.BackColor = Color.White;
            txtBaseLevel.TB.BackColor = Color.White;

            BaseStats();
            lblFLEE2.Text = "1";
            lblMDEFValue2.Text = "1";
            lblValueDEF1.Text = "0";
            lblAtk2.Text = "0";
            lblMDEFValue1.Text = "0";

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

            // --- STAT POINTS CALCULATION ---
            int totalPointsGained = 0;
            for (int i = 1; i < baseLevel; i++)
            {
                totalPointsGained += (i / 5) + 3;
            }
            int totalAvailablePoints = totalPointsGained + 48;
            int currentPointsSpent = CalculateStatCost(str) + CalculateStatCost(dex) +
                                     CalculateStatCost(vit) + CalculateStatCost(intel) +
                                     CalculateStatCost(agi) + CalculateStatCost(luk);
            int remainingPoints = totalAvailablePoints - currentPointsSpent;

            // --- VALIDATION ---
            if (remainingPoints < 0)
            {
                lblPointsRemaining.ForeColor = System.Drawing.Color.Red;
                lblPointsRemaining.Text = $"Overspent: Reset";
                txtSTR.TB.ReadOnly = txtDEX.TB.ReadOnly = txtVIT.TB.ReadOnly =
                txtLUK.TB.ReadOnly = txtINT.TB.ReadOnly = txtAGI.TB.ReadOnly = true;
            }
            else
            {
                lblPointsRemaining.ForeColor = System.Drawing.Color.Black;
                lblPointsRemaining.Text = remainingPoints.ToString();
                txtSTR.TB.ReadOnly = txtDEX.TB.ReadOnly = txtVIT.TB.ReadOnly =
                txtLUK.TB.ReadOnly = txtINT.TB.ReadOnly = txtAGI.TB.ReadOnly = false;
            }

            // --- Required stats for next increment ---
            lblReqSTR.Text = (2 + (str >= 11 ? ((str - 11) / 10) + 1 : 0)).ToString();
            lblReqAGI.Text = (2 + (agi >= 11 ? ((agi - 11) / 10) + 1 : 0)).ToString();
            lblReqINT.Text = (2 + (intel >= 11 ? ((intel - 11) / 10) + 1 : 0)).ToString();
            lblReqDEX.Text = (2 + (dex >= 11 ? ((dex - 11) / 10) + 1 : 0)).ToString();
            lblReqVIT.Text = (2 + (vit >= 11 ? ((vit - 11) / 10) + 1 : 0)).ToString();
            lblReqLUK.Text = (2 + (luk >= 11 ? ((luk - 11) / 10) + 1 : 0)).ToString();




            // --- STR Calculations ---
            // Bonus Atk: Every 10 STR adds [STR/10]^2
            int strTier = str / 10;
            int totalStrDamage = str + (strTier * strTier);
            // 2. Calculate the Bonus: 30 weight units for every 1 point of STR
            int strWeightBonus = str * 30;

            // 3. Add to the Job's Base Weight (e.g., 2030 for Novice, 2830 for Swordsman)
            int totalWeightLimit = jobBaseWeight + strWeightBonus;

            // --- DEX Calculations ---
            // Ranged ATK (Bows, Guns, etc): DEX + [DEX/10]^2
            int dexTier = dex / 10;
            int rangedAtk = dex + (dexTier * dexTier);
            // Melee Bonus: +1 ATK every 5 DEX
            int meleeBonusFromDex = dex / 5;
            // Cast Time Reduction: -1/150 per point (150 = Instant)
            double castReduction = Math.Min(100, (dex / 150.0) * 100);

            // --- AGI & ASPD (Weapon + Job + Stats) ---
            // Flee: Base 2 + 1 per AGI
            int totalFlee = 2 + agi;

            // Stat Bonus: +1 ASPD every 5 AGI
            int aspdStatBonus = agi / 5;

            // Formula: Weapon Base + Job Modifier + AGI/5 Stat Bonus
            double totalAspd = weaponBaseASPD + jobASPDModifier + aspdStatBonus;

            // Update UI
            lblASPD.Text = Math.Floor(totalAspd).ToString();

            // --- LUK Calculations ---
            // Critical Rate: [LUK * 0.3] + 1
            double critValue = (luk * 0.3) + 1;
            // Perfect Dodge: +0.1% per point
            double perfectDodge = luk * 0.1;
            // Melee Bonus: +1 ATK every 5 LUK
            int meleeBonusFromLuk = luk / 5;

            // --- VIT & HP ---
            // Linear HP: Starting 40 at Level 1, +5 per level increment
            double baseHP = 40 + ((baseLevel - 1) * 5);
            // Apply VIT bonus: +1% per point
            double totalHp = baseHP * (1 + (vit * 0.01));
            // HP regen: +1 base, +1 per 5 VIT
            int hpRegen = 1 + (vit / 5);

            // --- INT & SP ---
            // Linear SP: Job Base SP + (Level - 1)
            int baseSP = jobBaseSP + (baseLevel - 1);
            // Apply INT bonus: +1% per point
            double totalSP = baseSP * (1 + (intel * 0.01));
            // SP regen: +1 base, +1 per 6 INT
            int spRegen = 1 + (intel / 6);
            // MATK: Min bonus every 7, Max bonus every 5
            int minMatk = intel + (int)Math.Pow(intel / 7, 2);
            int maxMatk = intel + (int)Math.Pow(intel / 5, 2);

            // --- DEF ---
            // Soft DEF logic based on VIT thresholds
            double softDef = (vit <= 50) ? (vit * 0.8) : (vit * 0.85);
            double finalDef = Math.Max(1, Math.Floor(softDef));

            // --- UPDATE UI ---
            lblAtk1.Text = (totalStrDamage + meleeBonusFromDex + meleeBonusFromLuk).ToString();
            lblWeight.Text = totalWeightLimit.ToString();
            lblHit.Text = dex.ToString();
            lblRangedAtk.Text = rangedAtk.ToString();
            lblCastReduction.Text = $"{castReduction:F1}%";
            lblValueDEF2.Text = finalDef.ToString();
            lblTotalHP.Text = Math.Floor(totalHp).ToString();
            lblTotalSp.Text = Math.Floor(totalSP).ToString();
            lblHpRegen.Text = hpRegen.ToString();
            lblSpRegen.Text = spRegen.ToString();
            lblMinMatk1.Text = minMatk.ToString();
            lblMinMatk2.Text = maxMatk.ToString();
            lblFLEE1.Text = totalFlee.ToString();
            lblASPD.Text = Math.Floor(totalAspd).ToString();
            lblCrit.Text = Math.Floor(critValue).ToString("F1");
            lblPerfectDodge.Text = $"{perfectDodge:F1}%";

            // Update Remaining Points Label
            int remaining = CalculateTotalAvailablePoints(baseLevel) - CalculateTotalSpent();
            lblPointsRemaining.Text = remaining.ToString();
            lblPointsRemaining.ForeColor = (remaining < 0) ? Color.Red : Color.Black;
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
        private int CalculateTotalAvailablePoints(int level)
        {
            // Starting points at Level 1
            int totalPoints = 48;

            // Loop from Level 1 up to the current Level
            for (int i = 1; i < level; i++)
            {
                // Gain (Level / 5) + 3 points per level up
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
        int jobBaseSP = 11; // Base SP at Level 1
        int jobASPDModifier = 0; //bonus based on the Class
        int weaponBaseASPD = 100; // Default base ASPD 
        private void aloneComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSelectJob.SelectedItem == null) return;

            string selectedJob = cmbSelectJob.SelectedItem.ToString();
            cmbWeapon.Items.Clear();

            // 2. Set Job-specific properties and Weapon Lists
            switch (selectedJob)
            {
                case "Novice":
                    cmbWeapon.Items.AddRange(new string[] { "Hand", "Dagger", "One-Handed-Sword", "One-Handed-Axe", "One-Handed-Mace", "Two-Handed-Mace", "Rod&Staff", "Two-Handed-Staff" });
                    jobBaseWeight = 2000; jobBaseSP = 11; 
                    break;

                case "Swordsman":
                    cmbWeapon.Items.AddRange(new string[] { "Hand", "Dagger", "One-Handed-Sword", "Two-Handed-Sword", "One-Handed-Spear", "Two-Handed-Spear", "One-Handed-Axe", "Two-Handed-Axe", "One-Handed-Mace", "Two-Handed-Mace" });
                    jobBaseWeight = 2800; jobBaseSP = 10;  
                    break;
                
                case "Magician":
                    cmbWeapon.Items.AddRange(new string[] { "Hand", "Dagger", "Rod&Staff", "Two-Handed-Staff" });
                    jobBaseWeight = 2200; jobBaseSP = 15;  
                    break;
                case "Archer":
                    cmbWeapon.Items.AddRange(new string[] { "Hand", "Dagger", "Bow" });
                    jobBaseWeight = 2330; jobBaseSP = 12;  
                    break;

                case "Acolyte":
                    cmbWeapon.Items.AddRange(new string[] { "Hand", "One-Handed-Mace", "Two-Handed-Mace", "Rod&Staff", "Two-Handed-Staff" });
                    jobBaseWeight = 2200; jobBaseSP = 14;  
                    break;

                case "Merchant":
                    cmbWeapon.Items.AddRange(new string[] { "Hand", "Dagger", "One-Handed-Sword", "One-Handed-Axe", "Two-Handed-Axe", "One-Handed-Mace", "Two-Handed-Mace" });
                    jobBaseWeight = 2500; jobBaseSP = 12;  
                    break;

                case "Thief":
                    cmbWeapon.Items.AddRange(new string[] { "Hand", "Dagger", "One-Handed-Sword", "One-Handed-Axe", "Bow" });
                    jobBaseWeight = 2400; jobBaseSP = 14;  
                    break;

                    // Add other cases (Magician, Archer, etc.)
            }

            // Auto-select first weapon to avoid null errors
            if (cmbWeapon.Items.Count > 0) cmbWeapon.SelectedIndex = 0;

            UpdateAllStats();
        }

        //ASPD table 
        private readonly Dictionary<(string Job, string Weapon), int> JobWeaponASPD = new Dictionary<(string, string), int>
        {
                 // NOVICE
                { ("Novice", "Hand"), 150 }, { ("Novice", "Dagger"), 135 }, { ("Novice", "One-Handed-Sword"), 130 },{ ("Novice", "One-Handed-Axe"), 120 },{ ("Novice", "One-Handed-Mace"), 130 },{ ("Novice", "Two-Handed-Mace"), 130 },{ ("Novice", "Rod&Staff"), 135 },{ ("Novice", "Two-Handed-Staff"), 135 },
                 // SWORDSMAN (Example values - adjust as needed)
                { ("Swordsman", "Hand"), 160 }, { ("Swordsman", "Dagger"), 150 }, { ("Swordsman", "One-Handed-Sword"), 145 }, { ("Swordsman", "Two-Handed-Sword"), 140 },{ ("Swordsman", "One-Handed-Spear"), 135 },{ ("Swordsman", "Two-Handed-Spear"), 130 },{ ("Swordsman", "One-Handed-Axe"), 130 },{ ("Swordsman", "Two-Handed-Axe"), 125 },{ ("Swordsman", "One-Handed-Maxe"), 135 },{ ("Swordsman", "Two-Handed-Mace"), 130 },
                 // MAGICIAN
                 { ("Magician", "Hand"), 140 }, { ("Magician", "Dagger"), 130 }, { ("Magician", "Rod&Staff"), 120 }, { ("Magician", "Two-Handed-Staff"), 130 },
                 // ARCHER
                 { ("Archer", "Hand"), 150 }, { ("Archer", "Dagger"), 140 }, { ("Archer", "Bow"), 130 },
                 // ACOLYTE
                 { ("Acolyte", "Hand"), 160 }, { ("Acolyte", "One-Handed-Mace"), 140 }, { ("Acolyte", "Two-Handed-Mace"), 140 }, { ("Acolyte", "Rod&Staff"), 140 },{ ("Merchant", "Two-Handed-Staff"), 140 },
                 // MERCHANT
                 { ("Merchant", "Hand"), 160 }, { ("Merchant", "Dagger"), 140 }, { ("Merchant", "One-Handed-Sword"), 130 },{ ("Merchant", "One-Handed-Axe"), 130 }, { ("Merchant", "Two-Handed-Axe"), 125 }, { ("Merchant", "One-Handed-Mace"), 130 }, { ("Merchant", "Two-Handed-Mace"), 130 },
                 // THIEF
                { ("Thief", "Hand"), 160 }, { ("Thief", "Dagger"), 150 }, { ("Thief", "One-Handed-Sword"), 135 }, { ("Thief", "One-Handed-Axe"), 120 }, { ("Thief", "Bow"), 120 },
        };

        private void cmbWeapon_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSelectJob.SelectedItem == null || cmbWeapon.SelectedItem == null) return;

            string job = cmbSelectJob.SelectedItem.ToString();
            string weapon = cmbWeapon.SelectedItem.ToString();

            // 3. The "Magic" Lookup: Find the specific ASPD for this Job/Weapon combo
            if (JobWeaponASPD.TryGetValue((job, weapon), out int baseAspd))
            {
                weaponBaseASPD = baseAspd;
            }
            else
            {
                weaponBaseASPD = 100; // Default fallback
            }

            UpdateAllStats();
        }

        private void CmbWeapon_ControlAdded(object? sender, ControlEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void bigLabel34_Click(object sender, EventArgs e)
        {

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

        //For Increment buttons STR
        private void bigLabel2_Click(object sender, EventArgs e)
        {
            // 1. Parse current STR and Base Level
            if (!int.TryParse(txtSTR.Text, out int currentStr)) currentStr = 1;
            if (!int.TryParse(txtBaseLevel.Text, out int baseLevel)) baseLevel = 1;


            int costForNextPoint = (currentStr / 10) + 2;

            int totalAvailable = 48; // Starting points for normal novice
            for (int i = 1; i < baseLevel; i++)
            {
                totalAvailable += (i / 5) + 3;
            }

            // Calculate total currently spent on all stats
            int currentSpent = CalculateTotalSpent();

            // 4. CONSTRAINT: Only increment if we can afford the next point
            if (currentSpent + costForNextPoint <= totalAvailable)
            {
                txtSTR.Text = (currentStr + 1).ToString();
            }
            else
            {
                // Optional: Provide feedback that points are depleted
                lblPointsRemaining.ForeColor = System.Drawing.Color.Red;
            }
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


        //For Increment buttons AGI
        private void bigLabel11_Click(object sender, EventArgs e)
        {
            // 1. Parse current STR and Base Level
            if (!int.TryParse(txtAGI.Text, out int currentStr)) currentStr = 1;
            if (!int.TryParse(txtBaseLevel.Text, out int baseLevel)) baseLevel = 1;


            int costForNextPoint = (currentStr / 10) + 2;

            int totalAvailable = 48; // Starting points for normal novice
            for (int i = 1; i < baseLevel; i++)
            {
                totalAvailable += (i / 5) + 3;
            }

            // Calculate total currently spent on all stats
            int currentSpent = CalculateTotalSpent();

            if (currentSpent + costForNextPoint <= totalAvailable)
            {
                txtAGI.Text = (currentStr + 1).ToString();
            }
            else
            {
                // Optional: Provide feedback that points are depleted
                lblPointsRemaining.ForeColor = System.Drawing.Color.Red;
            }
        }

        //For Increment buttons VIT
        private void bigLabel12_Click(object sender, EventArgs e)
        {
            // 1. Parse current STR and Base Level
            if (!int.TryParse(txtVIT.Text, out int currentStr)) currentStr = 1;
            if (!int.TryParse(txtBaseLevel.Text, out int baseLevel)) baseLevel = 1;


            int costForNextPoint = (currentStr / 10) + 2;

            int totalAvailable = 48; // Starting points for normal novice
            for (int i = 1; i < baseLevel; i++)
            {
                totalAvailable += (i / 5) + 3;
            }

            // Calculate total currently spent on all stats
            int currentSpent = CalculateTotalSpent();

            if (currentSpent + costForNextPoint <= totalAvailable)
            {
                txtVIT.Text = (currentStr + 1).ToString();
            }
            else
            {
                // Optional: Provide feedback that points are depleted
                lblPointsRemaining.ForeColor = System.Drawing.Color.Red;
            }
        }

        //For Increment buttons INT
        private void bigLabel15_Click(object sender, EventArgs e)
        {
            // 1. Parse current STR and Base Level
            if (!int.TryParse(txtINT.Text, out int currentStr)) currentStr = 1;
            if (!int.TryParse(txtBaseLevel.Text, out int baseLevel)) baseLevel = 1;


            int costForNextPoint = (currentStr / 10) + 2;

            int totalAvailable = 48; // Starting points for normal novice
            for (int i = 1; i < baseLevel; i++)
            {
                totalAvailable += (i / 5) + 3;
            }

            // Calculate total currently spent on all stats
            int currentSpent = CalculateTotalSpent();

            if (currentSpent + costForNextPoint <= totalAvailable)
            {
                txtINT.Text = (currentStr + 1).ToString();
            }
            else
            {
                // Optional: Provide feedback that points are depleted
                lblPointsRemaining.ForeColor = System.Drawing.Color.Red;
            }
        }

        //For Increment buttons LUK
        private void bigLabel13_Click(object sender, EventArgs e)
        {
            // 1. Parse current STR and Base Level
            if (!int.TryParse(txtLUK.Text, out int currentStr)) currentStr = 1;
            if (!int.TryParse(txtBaseLevel.Text, out int baseLevel)) baseLevel = 1;


            int costForNextPoint = (currentStr / 10) + 2;

            int totalAvailable = 48; // Starting points for normal novice
            for (int i = 1; i < baseLevel; i++)
            {
                totalAvailable += (i / 5) + 3;
            }

            // Calculate total currently spent on all stats
            int currentSpent = CalculateTotalSpent();

            if (currentSpent + costForNextPoint <= totalAvailable)
            {
                txtLUK.Text = (currentStr + 1).ToString();
            }
            else
            {
                // Optional: Provide feedback that points are depleted
                lblPointsRemaining.ForeColor = System.Drawing.Color.Red;
            }
        }


        //For Increment buttons DEX
        private void bigLabel14_Click(object sender, EventArgs e)
        {
            // 1. Parse current STR and Base Level
            if (!int.TryParse(txtDEX.Text, out int currentStr)) currentStr = 1;
            if (!int.TryParse(txtBaseLevel.Text, out int baseLevel)) baseLevel = 1;


            int costForNextPoint = (currentStr / 10) + 2;

            int totalAvailable = 48; // Starting points for normal novice
            for (int i = 1; i < baseLevel; i++)
            {
                totalAvailable += (i / 5) + 3;
            }

            // Calculate total currently spent on all stats
            int currentSpent = CalculateTotalSpent();

            if (currentSpent + costForNextPoint <= totalAvailable)
            {
                txtDEX.Text = (currentStr + 1).ToString();
            }
            else
            {
                // Optional Provide feedback that points are depleted
                lblPointsRemaining.ForeColor = System.Drawing.Color.Red;
            }
        }

        //For Decrement buttons STR
        private void bigLabel45_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtSTR.Text, out int currentStr))
            {
                currentStr = 1;
            }
            int nextStr = currentStr - 1;
            txtSTR.Text = nextStr.ToString();
        }

        //For Decrement buttons LUK
        private void bigLabel18_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtLUK.Text, out int currentStr))
            {
                currentStr = 1;
            }
            int nextStr = currentStr - 1;
            txtLUK.Text = nextStr.ToString();
        }

        //For Decrement buttons AGI
        private void bigLabel37_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtAGI.Text, out int currentStr))
            {
                currentStr = 1;
            }
            int nextStr = currentStr - 1;
            txtAGI.Text = nextStr.ToString();
        }

        //For Decrement buttons VIT
        private void bigLabel36_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtVIT.Text, out int currentStr))
            {
                currentStr = 1;
            }
            int nextStr = currentStr - 1;
            txtVIT.Text = nextStr.ToString();
        }

        //For Decrement buttons INT
        private void bigLabel35_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtINT.Text, out int currentStr))
            {
                currentStr = 1;
            }
            int nextStr = currentStr - 1;
            txtINT.Text = nextStr.ToString();
        }

        //For Decrement buttons DEX
        private void bigLabel34_Click_1(object sender, EventArgs e)
        {
            if (!int.TryParse(txtDEX.Text, out int currentStr))
            {
                currentStr = 1;
            }
            int nextStr = currentStr - 1;
            txtDEX.Text = nextStr.ToString();
        }

    }
}