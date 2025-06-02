using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace GestionExamensApp
{
    public class LoginForm : Form
    {
        private TextBox emailBox;
        private TextBox passwordBox;
        private Button loginButton;

        public LoginForm()
        {
            this.Text = "Connexion";
            this.Width = 400;
            this.Height = 250;

            Label emailLabel = new Label { Text = "Email :", Top = 30, Left = 30, Width = 100 };
            emailBox = new TextBox { Top = 30, Left = 130, Width = 200 };

            Label passwordLabel = new Label { Text = "Mot de passe :", Top = 70, Left = 30, Width = 100 };
            passwordBox = new TextBox { Top = 70, Left = 130, Width = 200, UseSystemPasswordChar = true };

            loginButton = new Button { Text = "Se connecter", Top = 120, Left = 130, Width = 200 };
            loginButton.Click += LoginButton_Click;

            this.Controls.Add(emailLabel);
            this.Controls.Add(emailBox);
            this.Controls.Add(passwordLabel);
            this.Controls.Add(passwordBox);
            this.Controls.Add(loginButton);
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            string email = emailBox.Text.Trim();
            string password = passwordBox.Text.Trim();

            try
            {
                using (var conn = Database.GetConnection())
                {
                    string query = "SELECT * FROM utilisateurs WHERE email = @email AND mot_de_passe = @password";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@password", password); 

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string role = reader["role"].ToString();

                            switch (role)
                            {
                                case "admin":
                                    new DashboardAdmin().Show();
                                    break;
                                case "enseignant":
                                    new DashboardEnseignant().Show();
                                    break;
                                case "etudiant":
                                    new DashboardEtudiant().Show();
                                    break;
                                default:
                                    MessageBox.Show("Rôle inconnu !");
                                    return;
                            }

                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Email ou mot de passe incorrect.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur de connexion : " + ex.Message);
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(454, 347);
            this.Name = "LoginForm";
            this.ResumeLayout(false);

        }
    }
}
