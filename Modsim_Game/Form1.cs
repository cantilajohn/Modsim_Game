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
        }

        private void hopePictureBox2_Click(object sender, EventArgs e)
        {

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
    }
}
