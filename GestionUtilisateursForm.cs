using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace GestionExamensApp
{
    public partial class GestionUtilisateursForm : Form
    {
        DataGridView dataGrid;
        TextBox txtEmail, txtMotDePasse;
        ComboBox cbRole;

        public GestionUtilisateursForm()
        {
            this.Text = "Gestion des utilisateurs";
            this.Width = 700;
            this.Height = 500;

            Label lblEmail = new Label { Text = "Email", Left = 20, Top = 20 };
            txtEmail = new TextBox { Left = 100, Top = 20, Width = 200 };

            Label lblPassword = new Label { Text = "Mot de passe", Left = 20, Top = 60 };
            txtMotDePasse = new TextBox { Left = 100, Top = 60, Width = 200 };

            Label lblRole = new Label { Text = "Rôle", Left = 20, Top = 100 };
            cbRole = new ComboBox { Left = 100, Top = 100, Width = 200 };
            cbRole.Items.AddRange(new string[] { "admin", "enseignant", "etudiant" });

            Button btnAjouter = new Button { Text = "Ajouter", Left = 320, Top = 20 };
            btnAjouter.Click += BtnAjouter_Click;

            Button btnSupprimer = new Button { Text = "Supprimer", Left = 320, Top = 60 };
            btnSupprimer.Click += BtnSupprimer_Click;

            dataGrid = new DataGridView { Left = 20, Top = 150, Width = 640, Height = 300 };
            dataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            this.Controls.Add(lblEmail);
            this.Controls.Add(txtEmail);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtMotDePasse);
            this.Controls.Add(lblRole);
            this.Controls.Add(cbRole);
            this.Controls.Add(btnAjouter);
            this.Controls.Add(btnSupprimer);
            this.Controls.Add(dataGrid);

            LoadUsers();
        }

        private void LoadUsers()
        {
            using (var conn = Database.GetConnection())
            {
                var cmd = new MySqlCommand("SELECT id, email, role FROM utilisateurs", conn);
                var adapter = new MySqlDataAdapter(cmd);
                var table = new DataTable();
                adapter.Fill(table);
                dataGrid.DataSource = table;
            }
        }

        private void BtnAjouter_Click(object sender, EventArgs e)
        {
            using (var conn = Database.GetConnection())
            {
                var cmd = new MySqlCommand("INSERT INTO utilisateurs (email, mot_de_passe, role) VALUES (@e, @p, @r)", conn);
                cmd.Parameters.AddWithValue("@e", txtEmail.Text);
                cmd.Parameters.AddWithValue("@p", txtMotDePasse.Text);
                cmd.Parameters.AddWithValue("@r", cbRole.SelectedItem?.ToString());

                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Utilisateur ajouté.");
                    LoadUsers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur : " + ex.Message);
                }
            }
        }

        private void BtnSupprimer_Click(object sender, EventArgs e)
        {
            if (dataGrid.SelectedRows.Count == 0)
                return;

            int id = Convert.ToInt32(dataGrid.SelectedRows[0].Cells["id"].Value);

            using (var conn = Database.GetConnection())
            {
                var cmd = new MySqlCommand("DELETE FROM utilisateurs WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Utilisateur supprimé.");
            LoadUsers();
        }
    }
}
