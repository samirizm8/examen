using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace GestionExamensApp
{
    public partial class ValidationNotesForm : Form
    {
        private int enseignantId;
        private ComboBox examensCombo;
        private DataGridView notesGrid;
        private Button btnValider;

        private string connectionString = "Server=localhost;Database=gestion_examens;Uid=root;Pwd=ton_mot_de_passe;";

        public ValidationNotesForm(int enseignantId)
        {
            this.enseignantId = enseignantId;

            this.Text = "Validation des Notes";
            this.Width = 700;
            this.Height = 500;

            examensCombo = new ComboBox() { Left = 20, Top = 20, Width = 400 };
            examensCombo.SelectedIndexChanged += ExamensCombo_SelectedIndexChanged;

            notesGrid = new DataGridView()
            {
                Left = 20,
                Top = 60,
                Width = 640,
                Height = 350,
                AllowUserToAddRows = false,
                AutoGenerateColumns = false
            };

            // Colonne étudiant
            var colEtudiant = new DataGridViewTextBoxColumn()
            {
                HeaderText = "Étudiant",
                DataPropertyName = "NomEtudiant",
                ReadOnly = true,
                Width = 300
            };
            notesGrid.Columns.Add(colEtudiant);

            // Colonne note
            var colNote = new DataGridViewTextBoxColumn()
            {
                HeaderText = "Note",
                DataPropertyName = "Valeur",
                Width = 100
            };
            notesGrid.Columns.Add(colNote);

            btnValider = new Button() { Text = "Valider les notes", Left = 20, Top = 420, Width = 200 };
            btnValider.Click += BtnValider_Click;

            this.Controls.Add(examensCombo);
            this.Controls.Add(notesGrid);
            this.Controls.Add(btnValider);

            ChargerExamens();
        }

        private void ChargerExamens()
        {
            examensCombo.Items.Clear();

            MySqlConnection conn = null;
            MySqlCommand cmd = null;
            MySqlDataReader reader = null;

            try
            {
                conn = new MySqlConnection(connectionString);
                conn.Open();

                // Récupère les examens liés aux matières de cet enseignant
                string query = @"
                    SELECT examens.id, matieres.nom AS matiere_nom, examens.date_exam 
                    FROM examens
                    INNER JOIN matieres ON examens.matiere_id = matieres.id
                    WHERE matieres.enseignant_id = @enseignantId
                    ORDER BY examens.date_exam DESC";

                cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@enseignantId", enseignantId);

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int idExam = reader.GetInt32("id");
                    string matiereNom = reader.GetString("matiere_nom");
                    DateTime dateExam = reader.GetDateTime("date_exam");

                    examensCombo.Items.Add(new ExamItem
                    {
                        Id = idExam,
                        Description = $"{matiereNom} - {dateExam.ToShortDateString()}"
                    });
                }
            }
            finally
            {
                if (reader != null) reader.Close();
                if (cmd != null) cmd.Dispose();
                if (conn != null) conn.Close();
            }

            if (examensCombo.Items.Count > 0)
                examensCombo.SelectedIndex = 0;
        }

        private void ExamensCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (examensCombo.SelectedItem is ExamItem exam)
            {
                ChargerNotes(exam.Id);
            }
        }

        private void ChargerNotes(int examenId)
        {
            var notes = new List<NoteEtudiant>();

            MySqlConnection conn = null;
            MySqlCommand cmd = null;
            MySqlDataReader reader = null;

            try
            {
                conn = new MySqlConnection(connectionString);
                conn.Open();

                string query = @"
                    SELECT notes.id, etudiants.nom AS nom_etudiant, notes.valeur
                    FROM notes
                    INNER JOIN etudiants ON notes.etudiant_id = etudiants.id
                    WHERE notes.examen_id = @examenId";

                cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@examenId", examenId);

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    notes.Add(new NoteEtudiant
                    {
                        Id = reader.GetInt32("id"),
                        NomEtudiant = reader.GetString("nom_etudiant"),
                        Valeur = reader.GetFloat("valeur")
                    });
                }
            }
            finally
            {
                if (reader != null) reader.Close();
                if (cmd != null) cmd.Dispose();
                if (conn != null) conn.Close();
            }

            notesGrid.DataSource = null;
            notesGrid.DataSource = notes;
        }

        private void BtnValider_Click(object sender, EventArgs e)
        {
            if (examensCombo.SelectedItem is ExamItem exam)
            {
                var notes = (List<NoteEtudiant>)notesGrid.DataSource;
                if (notes == null) return;

                MySqlConnection conn = null;
                MySqlCommand cmd = null;

                try
                {
                    conn = new MySqlConnection(connectionString);
                    conn.Open();

                    foreach (var note in notes)
                    {
                        string query = "UPDATE notes SET valeur = @valeur WHERE id = @id";
                        cmd = new MySqlCommand(query, conn);

                        cmd.Parameters.AddWithValue("@valeur", note.Valeur);
                        cmd.Parameters.AddWithValue("@id", note.Id);

                        cmd.ExecuteNonQuery();

                        cmd.Dispose();
                        cmd = null;
                    }

                    MessageBox.Show("Notes validées avec succès !");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur lors de la validation : " + ex.Message);
                }
                finally
                {
                    if (cmd != null) cmd.Dispose();
                    if (conn != null) conn.Close();
                }
            }
        }

        // Classes pour la liaison des données

        private class ExamItem
        {
            public int Id { get; set; }
            public string Description { get; set; }
            public override string ToString() => Description;
        }

        private class NoteEtudiant
        {
            public int Id { get; set; }
            public string NomEtudiant { get; set; }
            public float Valeur { get; set; }
        }
    }
}
