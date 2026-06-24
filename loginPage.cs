using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SatprasDesktopApp.Config;
namespace ManajemenSarPras
{
    public partial class loginPage: Form
    {
        public loginPage()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            // Ambil input username dan password dari textbox
            string emailUser = txtEmail.Text;
            string passUser = txtPassword.Text;

            // validasi
            if (string.IsNullOrEmpty(emailUser) || string.IsNullOrEmpty(passUser))
            {
                MessageBox.Show("email atau password jangan kosong yaaw", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool isValid = DAL.AuthenticateUser(emailUser, passUser);

                if (isValid)
                {
                    MessageBox.Show("Login berhasil!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Buka form utama atau dashboard di sini
                    this.Hide();
                    dashboardPage dashboard = new dashboardPage();
                    dashboard.ShowDialog();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("email atau password salah yaaw", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
