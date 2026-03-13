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

            // --- STAT POINTS & UI STATE ---
            int totalAvailablePoints = CalculateTotalAvailablePoints(baseLevel) + 48;
            int currentPointsSpent = CalculateStatCost(str) + CalculateStatCost(dex) +
                                     CalculateStatCost(vit) + CalculateStatCost(intel) +
                                     CalculateStatCost(agi) + CalculateStatCost(luk);
            int remainingPoints = totalAvailablePoints - currentPointsSpent;


            int increment = 2 + (str >= 11 ? ((str - 11) / 10) + 1 : 0);
            lblReqSTR.Text = increment.ToString();

            int increment2 = 2 + (agi >= 11 ? ((agi - 11) / 10) + 1 : 0);
            lblReqAGI.Text = increment2.ToString();

            int increment3 = 2 + (intel >= 11 ? ((intel - 11) / 10) + 1 : 0);
            lblReqINT.Text = increment3.ToString();

            int increment4 = 2 + (dex >= 11 ? ((dex - 11) / 10) + 1 : 0);
            lblReqDEX.Text = increment4.ToString();

            int increment5 = 2 + (vit >= 11 ? ((vit - 11) / 10) + 1 : 0);
            lblReqVIT.Text = increment5.ToString();

            int increment6 = 2 + (luk >= 11 ? ((luk - 11) / 10) + 1 : 0);
            lblReqLUK.Text = increment6.ToString();


            lblPointsRemaining.Text = remainingPoints.ToString();
            lblPointsRemaining.ForeColor = (remainingPoints < 0) ? Color.Red : Color.Black;
            bool isOverspent = remainingPoints < 0;
            txtSTR.TB.ReadOnly = txtDEX.TB.ReadOnly = txtVIT.TB.ReadOnly =
            txtLUK.TB.ReadOnly = txtINT.TB.ReadOnly = txtAGI.TB.ReadOnly = isOverspent;

            // --- BASE JOB PROPERTIES ---
            string selectedJob = cmbSelectJob.SelectedItem?.ToString() ?? "Novice";

            // --- HP CALCULATION (Static Table) ---
            int tableBaseHP = JobStatTable.GetMaxHP(selectedJob, baseLevel);
           /* if (chkBaby.Checked) tableBaseHP = (int)(tableBaseHP * 0.7);*/
            double totalHp = tableBaseHP * (1 + (vit * 0.01));

            // --- MAX SP CALCULATION (Using your formula) ---
            double SP_JOB = JobStatTable.GetSpJobModifier(selectedJob);
            double BASE_SP = jobBaseSP + (baseLevel * SP_JOB);
            int SP_MOD_A = 0; // Flat bonuses from equipment
            int SP_MOD_B = 0; // % bonuses from equipment

            // Step 2: Apply INT bonus
            double MAX_SP = Math.Floor(BASE_SP * (1 + intel * 0.01));

            // Step 3: Additive modifiers
            MAX_SP += SP_MOD_A;

            // Step 4: Multiplicative modifiers
            MAX_SP = Math.Floor(MAX_SP * (1 + SP_MOD_B * 0.01));

            // Step 5: Baby Penalty (if applicable)
           /* if (chkBaby.Checked) MAX_SP = Math.Floor(MAX_SP * 0.7);*/

            // --- OTHER STATS ---
            int totalStrDamage = str + (int)Math.Pow(str / 10, 2);
            int totalWeightLimit = jobBaseWeight + (str * 30);
            double totalAspd = weaponBaseASPD + jobASPDModifier + (agi / 5.0);
            double castReduction = Math.Min(100, (dex / 150.0) * 100);
            double softDef = (vit <= 50) ? (vit * 0.8) : (vit * 0.85);

            // --- DEF AND MDEF ---
            int def = vit;
            int mdef = intel;



            lblValueDEF2.Text = def.ToString();
            lblMDEFValue2.Text = mdef.ToString();

            // --- FINAL UI UPDATE ---
            lblTotalHP.Text = Math.Floor(totalHp).ToString();
            lblTotalSp.Text = MAX_SP.ToString();

            lblAtk1.Text = (totalStrDamage + (dex / 5) + (luk / 5)).ToString();
            lblWeight.Text = totalWeightLimit.ToString();
            lblHit.Text = dex.ToString();
            lblRangedAtk.Text = (dex + (int)Math.Pow(dex / 10, 2)).ToString();
            lblCastReduction.Text = $"{castReduction:F1}%";
            lblValueDEF2.Text = Math.Max(1, Math.Floor(softDef)).ToString();
            lblHpRegen.Text = (1 + (vit / 5)).ToString();
            lblSpRegen.Text = (1 + (intel / 6)).ToString();
            lblMinMatk1.Text = (intel + (int)Math.Pow(intel / 7, 2)).ToString();
            lblMinMatk2.Text = (intel + (int)Math.Pow(intel / 5, 2)).ToString();
            lblFLEE1.Text = (2 + agi).ToString();
            lblASPD.Text = Math.Floor(totalAspd).ToString();
            lblCrit.Text = ((luk * 0.3) + 1).ToString("F1");
            lblPerfectDodge.Text = $"{(luk * 0.1):F1}%";
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


        public static class JobStatTable
        {
            // Arrays representing the HP columns from your image (Index 0 = Level 1)
            private static readonly int[] NoviceHP = { 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100, 105, 110, 115, 120, 125, 130, 135, 140, 145, 150, 155, 160, 165, 170, 175, 180, 185, 190, 195, 200, 205, 210, 215, 220, 225, 230, 235, 240, 245, 250, 255, 260, 265, 270, 275, 280, 285, 290, 295, 300, 305, 310, 315, 320, 325, 330, 335, 340, 345, 350, 355, 360, 365, 370, 375, 380, 385, 390, 395, 400, 405, 410, 415, 420, 425, 430, 435, 440, 445, 450, 455, 460, 465, 470, 475, 480, 485, 490, 495, 500, 505, 510, 515, 520, 525, 530 };

            private static readonly int[] SwordsmanHP = { 40, 46, 53, 61, 70, 79, 89, 100, 111, 123, 136, 149, 163, 178, 194, 210, 227, 245, 263, 282, 302, 322, 343, 365, 388, 411, 435, 460, 485, 511, 538, 565, 593, 622, 652, 682, 713, 745, 777, 810, 844, 878, 913, 949, 986, 1023, 1061, 1100, 1139, 1179, 1220, 1261, 1303, 1346, 1390, 1434, 1479, 1525, 1571, 1618, 1666, 1714, 1763, 1813, 1864, 1915, 1967, 2020, 2073, 2127, 2182, 2237, 2293, 2350, 2408, 2466, 2525, 2585, 2645, 2706, 2768, 2830, 2893, 2957, 3022, 3087, 3153, 3220, 3287, 3355, 3424, 3493, 3563, 3634, 3706, 3778, 3851, 3925, 3999 };

            private static readonly int[] MagicianHP = { 40, 46, 52, 58, 65, 72, 79, 86, 94, 102, 110, 119, 128, 137, 147, 157, 167, 177, 188, 199, 210, 222, 234, 246, 259, 272, 285, 298, 312, 326, 340, 355, 370, 385, 401, 417, 433, 449, 466, 483, 500, 518, 536, 554, 573, 592, 611, 630, 650, 670, 690, 711, 732, 753, 775, 797, 819, 841, 864, 887, 910, 934, 958, 982, 1007, 1032, 1057, 1082, 1108, 1134, 1160, 1187, 1214, 1241, 1269, 1297, 1325, 1353, 1382, 1411, 1440, 1470, 1500, 1530, 1561, 1592, 1623, 1654, 1686, 1718, 1750, 1783, 1816, 1849, 1883, 1917, 1951, 1985, 2020 };

            private static readonly int[] ArcherThiefHP = { 40, 46, 53, 60, 68, 76, 85, 94, 104, 114, 125, 136, 148, 160, 173, 186, 200, 214, 229, 244, 260, 276, 293, 310, 328, 346, 365, 384, 404, 424, 445, 466, 488, 510, 533, 556, 580, 604, 629, 654, 680, 706, 733, 760, 788, 816, 845, 874, 904, 934, 965, 996, 1028, 1060, 1093, 1126, 1160, 1194, 1229, 1264, 1300, 1336, 1373, 1410, 1448, 1486, 1525, 1564, 1604, 1644, 1685, 1726, 1768, 1810, 1853, 1896, 1940, 1984, 2029, 2074, 2120, 2166, 2213, 2260, 2308, 2356, 2405, 2454, 2504, 2554, 2605, 2656, 2708, 2760, 2813, 2866, 2920, 2974, 3029 };

            private static readonly int[] AcolyteMerchantHP = { 40, 46, 52, 59, 66, 73, 81, 89, 98, 107, 116, 126, 136, 147, 158, 169, 181, 193, 206, 219, 232, 246, 260, 275, 290, 305, 321, 337, 354, 371, 388, 406, 424, 443, 462, 481, 501, 521, 542, 563, 584, 606, 628, 651, 674, 697, 721, 745, 770, 795, 820, 846, 872, 899, 926, 953, 981, 1009, 1038, 1067, 1096, 1126, 1156, 1187, 1218, 1249, 1281, 1313, 1346, 1379, 1412, 1446, 1480, 1515, 1550, 1585, 1621, 1657, 1694, 1731, 1768, 1806, 1844, 1883, 1922, 1961, 2001, 2041, 2082, 2123, 2164, 2206, 2248, 2291, 2334, 2377, 2421, 2465, 2510 };

            public static int GetMaxHP(string jobClass, int level)
            {
                // Safety checks
                if (level < 1) level = 1;
                if (level > 99) level = 99;
                int index = level - 1;

                switch (jobClass)
                {
                    case "Swordsman": return SwordsmanHP[index];
                    case "Magician": return MagicianHP[index];
                    case "Archer":
                    case "Thief": return ArcherThiefHP[index];
                    case "Acolyte":
                    case "Merchant": return AcolyteMerchantHP[index];
                    default: return NoviceHP[index];
                }
            }

            public static double GetSpJobModifier(string jobClass)
            {
                switch (jobClass)
                {
                    case "Magician": return 6.0;  // Reaches ~900 SP
                    case "Acolyte": return 5.0;  // Reaches ~700 SP
                    case "Archer": return 2.0;  // Reaches ~400 SP
                    case "Thief": return 2.0;  // Reaches ~300 SP
                    case "Merchant": return 3.0;  // Reaches ~300 SP
                    case "Swordsman": return 2.0; // Hits exactly 210 SP at lvl 99
                    default: return 1.0; // Novice: Hits 110 SP
                }
            }
        }




        int jobBaseWeight = -30;
        int jobBaseSP; // Base SP at Level 1
        int jobASPDModifier = 0; //bonus based on the Class
        int weaponBaseASPD = 100; // Default base ASPD 
        double hpGainMultiplier = 0;
        int baseHpIncrement = 5; // The standard gain   
        private void aloneComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSelectJob.SelectedItem == null) return;

            string selectedJob = cmbSelectJob.SelectedItem.ToString();
            cmbWeapon.Items.Clear();

            switch (selectedJob)
            {
                case "Novice":
                    pbJobs.Image = Properties.Resources.noviceRagnarok;
                    cmbWeapon.Items.AddRange(new string[] { "Hand", "Dagger", "One-Handed-Sword", "One-Handed-Axe", "One-Handed-Mace", "Two-Handed-Mace", "Rod&Staff", "Two-Handed-Staff" });
                    jobBaseWeight = 2000;
                    jobBaseSP = 10; // Matches image table
                    break;

                case "Swordsman":
                    pbJobs.Image = Properties.Resources.swordmanRagnarok;
                    cmbWeapon.Items.AddRange(new string[] { "Hand", "Dagger", "One-Handed-Sword", "Two-Handed-Sword", "One-Handed-Spear", "Two-Handed-Spear", "One-Handed-Axe", "Two-Handed-Axe", "One-Handed-Mace", "Two-Handed-Mace" });
                    jobBaseWeight = 2800;
                    jobBaseSP = 10; // Matches image table
                    break;

                case "Magician":
                    pbJobs.Image = Properties.Resources.magicianRagnarok;
                    cmbWeapon.Items.AddRange(new string[] { "Hand", "Dagger", "Rod&Staff", "Two-Handed-Staff" });
                    jobBaseWeight = 2200;
                    jobBaseSP = 10; // Matches image table
                    break;

                case "Archer":
                    pbJobs.Image = Properties.Resources.archerRagnarok;
                    cmbWeapon.Items.AddRange(new string[] { "Hand", "Dagger", "Bow" });
                    jobBaseWeight = 2330;
                    jobBaseSP = 10; // Matches image table
                    break;

                case "Acolyte":
                    pbJobs.Image = Properties.Resources.AcolyteRagnarok2;
                    cmbWeapon.Items.AddRange(new string[] { "Hand", "One-Handed-Mace", "Two-Handed-Mace", "Rod&Staff", "Two-Handed-Staff" });
                    jobBaseWeight = 2200;
                    jobBaseSP = 15; // Matches image table
                    break;

                case "Merchant":
                    pbJobs.Image = Properties.Resources.merchantRagnarok;
                    cmbWeapon.Items.AddRange(new string[] { "Hand", "Dagger", "One-Handed-Sword", "One-Handed-Axe", "Two-Handed-Axe", "One-Handed-Mace", "Two-Handed-Mace" });
                    jobBaseWeight = 2500;
                    jobBaseSP = 10; // Matches image table
                    break;

                case "Thief":
                    pbJobs.Image = Properties.Resources.thiefRagnarok;
                    cmbWeapon.Items.AddRange(new string[] { "Hand", "Dagger", "One-Handed-Sword", "One-Handed-Axe", "Bow" });
                    jobBaseWeight = 2400;
                    jobBaseSP = 10; // Matches image table
                    break;
            }

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