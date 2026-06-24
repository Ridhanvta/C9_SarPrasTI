using SatprasDesktopApp.Config;
using System;

using System.Windows.Forms;

namespace ManajemenSarPras
{
    public partial class loginForm : Form
    {
        public loginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string emailUser = txtEmail.Text.Trim();
            string passwordUser = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(emailUser) || string.IsNullOrEmpty(passwordUser))
            {
                MessageBox.Show("Email dan Password wajib diisi.", "Validasi Akses", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool isValid = DAL.AuthenticateUser(emailUser, passwordUser);

                if (isValid)
                {
                    dashboardPage home = new dashboardPage();
                    home.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Kredensial tidak valid. Email atau Password salah.", "Akses Ditolak", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sistem gagal melakukan autentikasi: " + ex.Message, "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}