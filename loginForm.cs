using SatprasDesktopApp.Config;
using System;
using System.Data;
using System.Data.SqlClient;
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
                using (var connection = DatabaseConfig.GetConnection())
                {
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    string query = "SELECT COUNT(1) FROM [master].users WHERE email = @Email AND password = @Password";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", emailUser);
                        command.Parameters.AddWithValue("@Password", passwordUser);

                        int userCount = Convert.ToInt32(command.ExecuteScalar());

                        if (userCount > 0)
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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sistem gagal melakukan autentikasi: " + ex.Message, "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}