using System.Text.RegularExpressions;
using System.Windows.Input;

namespace Modsim_Game
{
    public partial class Form1 : Form
    {
        public Form1()
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
            // 1. Parse all inputs (Default to 0 if invalid or empty)
            int.TryParse(txtSTR.Text, out int str);
            int.TryParse(txtDEX.Text, out int dex);
            int.TryParse(txtVIT.Text, out int vit);
            int.TryParse(txtINT.Text, out int intel);
            int.TryParse(txtAGI.Text, out int agi);
            int.TryParse(txtLUK.Text, out int luk);
            int.TryParse(txtBaseLevel.Text, out int baseLevel);

            //   STAT POINTS CALCULATION   
            // Leveling formula: ROUNDDOWN(x / 5) + 3 points gained per level
            int totalPointsGained = 0;
            for (int i = 1; i < baseLevel; i++)
            {
                totalPointsGained += (i / 5) + 3;
            }

            // Normal starting points
            int totalAvailablePoints = totalPointsGained + 48;

            // Calculate cost based on: [(stat - 1) / 10] + 2
            int currentPointsSpent = CalculateStatCost(str) + CalculateStatCost(dex) +
                                     CalculateStatCost(vit) + CalculateStatCost(intel) +
                                     CalculateStatCost(agi) + CalculateStatCost(luk);

            int remainingPoints = totalAvailablePoints - currentPointsSpent;

            //    VALIDATION CHECK   
            if (remainingPoints < 0)
            {
                lblPointsRemaining.ForeColor = System.Drawing.Color.Red;
                lblPointsRemaining.Text = $"Overspent: Reset";
                // Optional: Block further calculation or show a warning
                txtSTR.TB.ReadOnly = true;
                txtDEX.TB.ReadOnly = true;
                txtVIT.TB.ReadOnly = true;
                txtLUK.TB.ReadOnly = true;
                txtINT.TB.ReadOnly = true;
                txtAGI.TB.ReadOnly = true;
            }
            else
            {
                lblPointsRemaining.ForeColor = System.Drawing.Color.Black;
                lblPointsRemaining.Text = remainingPoints.ToString();
                txtSTR.TB.ReadOnly = false;
                txtDEX.TB.ReadOnly = false;
                txtVIT.TB.ReadOnly = false;
                txtLUK.TB.ReadOnly = false;
                txtINT.TB.ReadOnly = false;
                txtAGI.TB.ReadOnly = false;
            }

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

            //    STR CALCULATIONS   
            int strTier = str / 10;
            int totalStrDamage = str + (strTier * strTier);
            int weightLimit = 2030 + (str * 30); // Base + 30 per STR

            //    DEX CALCULATIONS   
            int dexTier = dex / 10;
            int rangedAtk = dex + (dexTier * dexTier);
            int meleeBonusFromDex = dex / 5;
            double castReduction = Math.Min(100, (dex / 150.0) * 100);

            //    AGI & ASPD CALCULATIONS   
            int totalFlee = 2 + agi;
            double aspdPercentReduction = (agi * 0.4) + (dex * 0.1);
            double totalAspd = 150 + (150 * (aspdPercentReduction / 100));

            //    LUK CALCULATIONS   
            double critRate = (luk * 0.3) + 1;
            double perfectDodge = luk * 0.1;
            int meleeBonusFromLuk = luk / 5;

            // VIT & HP CALCULATIONS
            double hpJobA = 5.0;
            int hpJobB = 5;

            double baseHP = 35 + (baseLevel * hpJobB);

            // Cumulative growth
            for (int i = 2; i <= baseLevel; i++)
            {
                baseHP += Math.Round(hpJobA * i);
            }

            double totalHp = baseHP * (1 + (vit * 0.01));

            // Soft DEF calculation
            double softDef = (vit <= 50) ? (vit * 0.8) : (vit * 0.85);

            //    Ensure minimum DEF = 1   
            double finalDef = Math.Max(1, Math.Floor(softDef));

            //    Output   

            /*double hpJobA = 5.0;
            int hpJobB = 5;
            double baseHP = 35 + (baseLevel * hpJobB); // Base HP formula
            for (int i = 2; i <= baseLevel; i++)
            {
                baseHP += Math.Round(hpJobA * i); // Cumulative growth
            }
            double totalHp = baseHP * (1 + (vit * 0.01));
            double softDef = (vit <= 50) ? (vit * 0.8) : (vit * 0.85);*/

            //    INT CALCULATIONS   
            // Min MATK bonus every 7, Max every 5
            int minMatk = intel + (int)Math.Pow(intel / 7, 2);
            int maxMatk = intel + (int)Math.Pow(intel / 5, 2);

            //    UPDATE UI   
            lblAtk1.Text = (totalStrDamage + meleeBonusFromDex + meleeBonusFromLuk).ToString();
            lblWeight.Text = weightLimit.ToString();
            lblHit.Text = dex.ToString();
            lblRangedAtk.Text = rangedAtk.ToString();
            lblCastReduction.Text = $"{castReduction:F1}%";
            lblValueDEF2.Text = finalDef.ToString();
            lblBaseHp.Text = Math.Floor(totalHp).ToString();
            lblMinMatk1.Text = minMatk.ToString();
            lblMinMatk2.Text = maxMatk.ToString();
            lblFLEE1.Text = totalFlee.ToString();
            lblASPD.Text = Math.Floor(totalAspd).ToString();
            lblCrit.Text = $"{critRate:F1}";
            lblPerfectDodge.Text = $"{perfectDodge:F1}%";
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

        // Map every TextChanged event to the same method:
        private void txtSTR_TextChanged(object sender, EventArgs e) => UpdateAllStats();
        private void txtDex_TextChanged(object sender, EventArgs e) => UpdateAllStats();
        private void txtInt_TextChanged(object sender, EventArgs e) => UpdateAllStats();
        private void txtBaseLevel_TextChanged(object sender, EventArgs e) => UpdateAllStats();
        private void txtAgi_TextChanged(object sender, EventArgs e) => UpdateAllStats();
        private void txtVit_TextChanged_1(object sender, EventArgs e) => UpdateAllStats();
        private void txtLuk_TextChanged(object sender, EventArgs e) => UpdateAllStats();


        private void aloneComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedGif = cmbSelectJob.SelectedItem.ToString();
            switch (selectedGif)
            {
                case "Novice":
                    pbJobs.Image = Properties.Resources.GIF_0ne;
                    lblJobTitle.Text = selectedGif;
                    int weight = Convert.ToInt32(lblWeight.Text);
                    weight = 2030;
                    break;
                case "Swordsman":
                    pbJobs.Image = Properties.Resources.swordsman;
                    lblJobTitle.Text = selectedGif;
                    break;
                case "Magician":
                    pbJobs.Image = Properties.Resources.magician2;
                    lblJobTitle.Text = selectedGif;
                    break;
                case "Archer":
                    pbJobs.Image = Properties.Resources.archer;
                    lblJobTitle.Text = selectedGif;
                    break;
                case "Acolyte":
                    pbJobs.Image = Properties.Resources.magician;
                    lblJobTitle.Text = selectedGif;
                    break;
                case "Merchant":
                    pbJobs.Image = Properties.Resources.merchant;
                    lblJobTitle.Text = selectedGif;
                    break;
                case "Thieft":
                    pbJobs.Image = Properties.Resources.thieft;
                    lblJobTitle.Text = selectedGif;
                    break;
                case "Knight":
                    pbJobs.Image = Properties.Resources.knight;
                    lblJobTitle.Text = selectedGif;
                    break;
                case "Priest":
                    pbJobs.Image = Properties.Resources.knight;
                    lblJobTitle.Text = selectedGif;
                    break;

            }
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
        void onclick()
        {
            if (!int.TryParse(txtSTR.Text, out int currentStr))
            {
                currentStr = 1;
            }
            int nextStr = currentStr + 1;
            txtSTR.Text = nextStr.ToString();
        }
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
                // Optional: Provide feedback that points are depleted
                lblPointsRemaining.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void bigLabel45_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtSTR.Text, out int currentStr))
            {
                currentStr = 1;
            }
            int nextStr = currentStr - 1;
            txtSTR.Text = nextStr.ToString();
        }

        private void bigLabel18_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtLUK.Text, out int currentStr))
            {
                currentStr = 1;
            }
            int nextStr = currentStr - 1;
            txtLUK.Text = nextStr.ToString();
        }

        private void bigLabel37_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtAGI.Text, out int currentStr))
            {
                currentStr = 1;
            }
            int nextStr = currentStr - 1;
            txtAGI.Text = nextStr.ToString();
        }

        private void bigLabel36_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtVIT.Text, out int currentStr))
            {
                currentStr = 1;
            }
            int nextStr = currentStr - 1;
            txtVIT.Text = nextStr.ToString();
        }

        private void bigLabel35_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtINT.Text, out int currentStr))
            {
                currentStr = 1;
            }
            int nextStr = currentStr - 1;
            txtINT.Text = nextStr.ToString();
        }

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
