using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace GestionExamensApp
{
    public partial class RapportForm : Form
    {
        private ComboBox cbMatieres;
        private DataGridView dgvRapport;
        private Button btnCharger;

        private string connectionString = "Server=localhost;Database=gestion_examens;Uid=root;Pwd=ton_mot_de_passe;";

        public RapportForm()
        {
            this.Text = "Rapport des résultats";
            this.Width = 800;
            this.Height = 600;

            Label lblMatiere = new Label() { Text = "Sélectionner une matière :", Left = 20, Top = 20 };
            cbMatieres = new ComboBox() { Left = 180, Top = 15, Width = 300 };

            btnCharger = new Button() { Text = "Afficher le rapport", Left = 500, Top = 15 };
            btnCharger.Click += BtnCharger_Click;

            dgvRapport = new DataGridView()
            {
                Left = 20,
                Top = 50,
                Width = 740,
                Height = 500,
                AutoGenerateColumns = true,
                ReadOnly = true,
                AllowUserToAddRows = false
            };

            this.Controls.Add(lblMatiere);
            this.Controls.Add(cbMatieres);
            this.Controls.Add(btnCharger);
            this.Controls.Add(dgvRapport);

            ChargerMatieres();
        }

        private void ChargerMatieres()
        {
            cbMatieres.Items.Clear();
            cbMatieres.Items.Add(new MatiereItem { Id = 0, Nom = "Toutes les matières" });

            MySqlConnection conn = null;
            MySqlCommand cmd = null;
            MySqlDataReader reader = null;

            try
            {
                conn = new MySqlConnection(connectionString);
                conn.Open();

                string query = "SELECT id, nom FROM matieres ORDER BY nom";
                cmd = new MySqlCommand(query, conn);
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    cbMatieres.Items.Add(new MatiereItem
                    {
                        Id = reader.GetInt32("id"),
                        Nom = reader.GetString("nom")
                    });
                }

                cbMatieres.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement des matières : " + ex.Message);
            }
            finally
            {
                if (reader != null) reader.Close();
                if (cmd != null) cmd.Dispose();
                if (conn != null) conn.Close();
            }
        }

        private void BtnCharger_Click(object sender, EventArgs e)
        {
            if (!(cbMatieres.SelectedItem is MatiereItem matiere))
            {
                return;
            }


            ChargerRapport(matiere.Id);
        }

        private void ChargerRapport(int matiereId)
        {
            DataTable dt = new DataTable();

            MySqlConnection conn = null;
            MySqlCommand cmd = null;

            try
            {
                conn = new MySqlConnection(connectionString);
                conn.Open();

                string query;

                if (matiereId == 0)
                {
                    // Rapport global : Moyenne par matière et par étudiant
                    query = @"
                        SELECT 
                            m.nom AS Matiere,
                            e.nom AS Etudiant,
                            ROUND(AVG(n.valeur), 2) AS Moyenne
                        FROM notes n
                        INNER JOIN examens ex ON n.examen_id = ex.id
                        INNER JOIN matieres m ON ex.matiere_id = m.id
                        INNER JOIN etudiants e ON n.etudiant_id = e.id
                        GROUP BY m.nom, e.nom
                        ORDER BY m.nom, e.nom";
                }
                else
                {
                    // Rapport pour une matière spécifique
                    query = @"
                        SELECT 
                            e.nom AS Etudiant,
                            ROUND(AVG(n.valeur), 2) AS Moyenne
                        FROM notes n
                        INNER JOIN examens ex ON n.examen_id = ex.id
                        INNER JOIN etudiants e ON n.etudiant_id = e.id
                        WHERE ex.matiere_id = @matiereId
                        GROUP BY e.nom
                        ORDER BY e.nom";
                }

                cmd = new MySqlCommand(query, conn);

                if (matiereId != 0)
                    cmd.Parameters.AddWithValue("@matiereId", matiereId);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dt);

                dgvRapport.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement du rapport : " + ex.Message);
            }
            finally
            {
                if (cmd != null) cmd.Dispose();
                if (conn != null) conn.Close();
            }
        }

        private class MatiereItem
        {
            public int Id { get; set; }
            public string Nom { get; set; }
            public override string ToString() => Nom;
        }
    }
}
