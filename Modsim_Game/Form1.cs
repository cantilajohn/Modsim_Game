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

        }

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
                    hopePictureBox1.Image = Properties.Resources.novice;
                    lblJobTitle.Text = selectedGif;
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

        private void txtSTR_TextChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(txtSTR.Text, out int strValue) || strValue < 0)
            {
                lblAtk.Text = "0";
                lblStrBonus.Text = "0";
                lblTotalDamage.Text = "0";
                lblWeight.Text = "0";
                lblRanged.Text = "0";
                return;
            }

            // Base attack from STR (+1 per STR)
            int baseAttack = strValue;

            // STR tier (every 10 STR)
            int tier = strValue / 10;

            // Total bonus damage = (STR / 10)^2
            int totalBonus = tier * tier;

            // Total melee damage from STR
            int totalDamage = strValue + totalBonus;

            // Weight limit
            int weight = strValue * 30;

            // Ranged bonus (every 5 STR)
            int rangedBonus = strValue / 5;

            lblAtk.Text = baseAttack.ToString();
            lblStrBonus.Text = totalBonus.ToString();
            lblTotalDamage.Text = totalDamage.ToString();
            lblWeight.Text = weight.ToString();
            lblRanged.Text = rangedBonus.ToString();
        }
    }
}
