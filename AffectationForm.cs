using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace GestionExamensApp
{
    public partial class AffectationForm : Form
    {
        private ComboBox cbEtudiants;
        private ComboBox cbMatieres;
        private Button btnAffecter;

        private string connectionString = "Server=localhost;Database=gestion_examens;Uid=root;Pwd=ton_mot_de_passe;";

        public AffectationForm()
        {
            this.Text = "Affectation Étudiant - Matière";
            this.Width = 400;
            this.Height = 200;

            Label lblEtudiant = new Label() { Text = "Étudiant :", Left = 20, Top = 20, Width = 100 };
            cbEtudiants = new ComboBox() { Left = 120, Top = 15, Width = 220 };

            Label lblMatiere = new Label() { Text = "Matière :", Left = 20, Top = 60, Width = 100 };
            cbMatieres = new ComboBox() { Left = 120, Top = 55, Width = 220 };

            btnAffecter = new Button() { Text = "Affecter", Left = 120, Top = 100, Width = 220 };
            btnAffecter.Click += BtnAffecter_Click;

            this.Controls.Add(lblEtudiant);
            this.Controls.Add(cbEtudiants);
            this.Controls.Add(lblMatiere);
            this.Controls.Add(cbMatieres);
            this.Controls.Add(btnAffecter);

            ChargerEtudiants();
            ChargerMatieres();
        }

        private void ChargerEtudiants()
        {
            cbEtudiants.Items.Clear();

            using (var conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT id, nom FROM etudiants ORDER BY nom";
                    var cmd = new MySqlCommand(query, conn);
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        cbEtudiants.Items.Add(new Item { Id = reader.GetInt32("id"), Nom = reader.GetString("nom") });
                    }

                    if (cbEtudiants.Items.Count > 0)
                        cbEtudiants.SelectedIndex = 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur chargement étudiants : " + ex.Message);
                }
            }
        }

        private void ChargerMatieres()
        {
            cbMatieres.Items.Clear();

            using (var conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT id, nom FROM matieres ORDER BY nom";
                    var cmd = new MySqlCommand(query, conn);
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        cbMatieres.Items.Add(new Item { Id = reader.GetInt32("id"), Nom = reader.GetString("nom") });
                    }

                    if (cbMatieres.Items.Count > 0)
                        cbMatieres.SelectedIndex = 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur chargement matières : " + ex.Message);
                }
            }
        }

        private void BtnAffecter_Click(object sender, EventArgs e)
        {
            if (!(cbEtudiants.SelectedItem is Item etudiant) || !(cbMatieres.SelectedItem is Item matiere))
            {
                MessageBox.Show("Veuillez sélectionner un étudiant et une matière.");
                return;
            }

            using (var conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Exemple : insertion dans une table affectations (à adapter selon ta base)
                    string query = "INSERT INTO affectations (etudiant_id, matiere_id) VALUES (@etudiantId, @matiereId)";

                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@etudiantId", etudiant.Id);
                    cmd.Parameters.AddWithValue("@matiereId", matiere.Id);

                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                        MessageBox.Show("Affectation enregistrée avec succès !");
                    else
                        MessageBox.Show("Erreur lors de l'affectation.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur base de données : " + ex.Message);
                }
            }
        }

        private class Item
        {
            public int Id { get; set; }
            public string Nom { get; set; }
            public override string ToString() => Nom;
        }
    }
}
