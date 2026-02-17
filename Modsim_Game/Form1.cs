using System.Text.RegularExpressions;
using System.Windows.Input;

namespace Modsim_Game
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            txtAgi.TB.BackColor = Color.White;
            txtSTR.TB.BackColor = Color.White;
            txtVit.TB.BackColor = Color.White;
            txtLuk.TB.BackColor = Color.White;
            txtDex.TB.BackColor = Color.White;
            txtInt.TB.BackColor = Color.White;
            txtBaseLevel.TB.BackColor = Color.White;
            BaseStats();
        }

        private void hopePictureBox2_Click(object sender, EventArgs e)
        {

        }




        void BaseStats()
        {
            txtSTR.Text = "1";
            txtAgi.Text = "1";
            txtVit.Text = "1";
            txtInt.Text = "1";
            txtDex.Text = "1";
            txtLuk.Text = "1";
            txtBaseLevel.Text = "1";
        }

        private void UpdateAllStats()
        {
            // 1. Parse all inputs (Default to 0 if invalid or empty)
            int.TryParse(txtSTR.Text, out int str);
            int.TryParse(txtDex.Text, out int dex);
            int.TryParse(txtVit.Text, out int vit);
            int.TryParse(txtInt.Text, out int intel);
            int.TryParse(txtAgi.Text, out int agi);
            int.TryParse(txtLuk.Text, out int luk);
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
                txtDex.TB.ReadOnly = true;
                txtVit.TB.ReadOnly = true;
                txtLuk.TB.ReadOnly = true;
                txtInt.TB.ReadOnly = true;
                txtAgi.TB.ReadOnly = true;
            }
            else
            {
                lblPointsRemaining.ForeColor = System.Drawing.Color.Black;
                lblPointsRemaining.Text = remainingPoints.ToString();
                txtSTR.TB.ReadOnly = false;
                txtDex.TB.ReadOnly = false;
                txtVit.TB.ReadOnly = false;
                txtLuk.TB.ReadOnly = false;
                txtInt.TB.ReadOnly = false;
                txtAgi.TB.ReadOnly = false;
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
            lblAtk.Text = (totalStrDamage + meleeBonusFromDex + meleeBonusFromLuk).ToString();
            lblWeight.Text = weightLimit.ToString();
            lblHit.Text = dex.ToString();
            lblRangedAtk.Text = rangedAtk.ToString();
            lblCastReduction.Text = $"{castReduction:F1}%";
            lblDef.Text = finalDef.ToString();
            lblBaseHp.Text = Math.Floor(totalHp).ToString();
            lblMinMatk.Text = minMatk.ToString();
            lblMaxMatk.Text = maxMatk.ToString();
            lblFlee.Text = totalFlee.ToString();
            lblAspd.Text = Math.Floor(totalAspd).ToString();
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


        private void crownDropDownList1_Click(object sender, EventArgs e)
        {
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void aloneComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedGif = aloneComboBox2.SelectedItem.ToString();
            switch (selectedGif)
            {
                case "Novice":
                    hopePictureBox1.Image = Properties.Resources.GIF_0ne;
                    lblJobTitle.Text = selectedGif;
                    int weight = Convert.ToInt32(lblWeight.Text);
                    weight = 2030;
                    break;
                case "Swordsman":
                    hopePictureBox1.Image = Properties.Resources.swordsman;
                    lblJobTitle.Text = selectedGif;
                    break;
                case "Magician":
                    hopePictureBox1.Image = Properties.Resources.magician2;
                    lblJobTitle.Text = selectedGif;
                    break;
                case "Archer":
                    hopePictureBox1.Image = Properties.Resources.archer;
                    lblJobTitle.Text = selectedGif;
                    break;
                case "Acolyte":
                    hopePictureBox1.Image = Properties.Resources.magician;
                    lblJobTitle.Text = selectedGif;
                    break;
                case "Merchant":
                    hopePictureBox1.Image = Properties.Resources.merchant;
                    lblJobTitle.Text = selectedGif;
                    break;
                case "Thieft":
                    hopePictureBox1.Image = Properties.Resources.thieft;
                    lblJobTitle.Text = selectedGif;
                    break;
                case "Knight":
                    hopePictureBox1.Image = Properties.Resources.knight;
                    lblJobTitle.Text = selectedGif;
                    break;
                case "Priest":
                    hopePictureBox1.Image = Properties.Resources.knight;
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
            txtAgi.Text = "1";
            txtVit.Text = "1";
            txtInt.Text = "1";
            txtDex.Text = "1";
            txtLuk.Text = "1";
        }

    }
}
