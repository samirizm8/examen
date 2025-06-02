using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace GestionExamensApp
{
    public class ResultatsForm : Form
    {
        private ComboBox cbExamens;
        private DataGridView dgvNotes;
        private Button btnEnregistrer;

        private string connectionString = "Server=localhost;Database=gestion_examens;Uid=root;Pwd=ton_mot_de_passe;";

        public ResultatsForm()
        {
            this.Text = "Gestion des Résultats";
            this.Width = 800;
            this.Height = 600;

            Label lblExamens = new Label() { Text = "Session d'examen :", Left = 20, Top = 20 };
            cbExamens = new ComboBox() { Left = 140, Top = 15, Width = 600 };
            cbExamens.SelectedIndexChanged += CbExamens_SelectedIndexChanged;

            dgvNotes = new DataGridView()
            {
                Left = 20,
                Top = 50,
                Width = 740,
                Height = 450,
                AllowUserToAddRows = false,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.CellSelect,
                EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2
            };

            // Colonnes du DataGridView
            dgvNotes.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "ID Note", DataPropertyName = "NoteId", Visible = false });
            dgvNotes.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "ID Étudiant", DataPropertyName = "EtudiantId", Visible = false });
            dgvNotes.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Nom Étudiant", DataPropertyName = "NomEtudiant", Width = 300, ReadOnly = true });
            dgvNotes.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Note", DataPropertyName = "Valeur", Width = 100 });

            btnEnregistrer = new Button() { Text = "Enregistrer les modifications", Left = 20, Top = 520, Width = 250 };
            btnEnregistrer.Click += BtnEnregistrer_Click;

            this.Controls.Add(lblExamens);
            this.Controls.Add(cbExamens);
            this.Controls.Add(dgvNotes);
            this.Controls.Add(btnEnregistrer);

            ChargerExamens();
        }

        private void ChargerExamens()
        {
            cbExamens.Items.Clear();

            MySqlConnection conn = null;
            MySqlCommand cmd = null;
            MySqlDataReader reader = null;

            try
            {
                conn = new MySqlConnection(connectionString);
                conn.Open();

                string query = @"
                    SELECT examens.id, matieres.nom AS nom_matiere, examens.date_exam
                    FROM examens
                    INNER JOIN matieres ON examens.matiere_id = matieres.id
                    ORDER BY examens.date_exam DESC";

                cmd = new MySqlCommand(query, conn);
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    cbExamens.Items.Add(new ExamenItem
                    {
                        Id = reader.GetInt32("id"),
                        Description = $"{reader.GetString("nom_matiere")} - {reader.GetDateTime("date_exam"):dd/MM/yyyy}"
                    });
                }
            }
            finally
            {
                if (reader != null) reader.Close();
                if (cmd != null) cmd.Dispose();
                if (conn != null) conn.Close();
            }

            if (cbExamens.Items.Count > 0)
                cbExamens.SelectedIndex = 0;
        }

        private void CbExamens_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbExamens.SelectedItem is ExamenItem examen)
            {
                ChargerNotes(examen.Id);
            }
        }

        private void ChargerNotes(int examenId)
        {
            List<NoteItem> notes = new List<NoteItem>();

            MySqlConnection conn = null;
            MySqlCommand cmd = null;
            MySqlDataReader reader = null;

            try
            {
                conn = new MySqlConnection(connectionString);
                conn.Open();

                // On récupère la liste des étudiants avec leur note (ou null si pas encore noté)
                string query = @"
                    SELECT 
                        etudiants.id AS etudiant_id,
                        etudiants.nom AS nom_etudiant,
                        notes.id AS note_id,
                        notes.valeur AS valeur
                    FROM etudiants
                    LEFT JOIN notes ON notes.etudiant_id = etudiants.id AND notes.examen_id = @examenId
                    ORDER BY etudiants.nom";

                cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@examenId", examenId);

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    notes.Add(new NoteItem
                    {
                        NoteId = reader.IsDBNull(reader.GetOrdinal("note_id")) ? 0 : reader.GetInt32("note_id"),
                        EtudiantId = reader.GetInt32("etudiant_id"),
                        NomEtudiant = reader.GetString("nom_etudiant"),
                        Valeur = reader.IsDBNull(reader.GetOrdinal("valeur")) ? (float?)null : reader.GetFloat("valeur")
                    });
                }
            }
            finally
            {
                if (reader != null) reader.Close();
                if (cmd != null) cmd.Dispose();
                if (conn != null) conn.Close();
            }

            dgvNotes.DataSource = null;
            dgvNotes.DataSource = notes;
        }

        private void BtnEnregistrer_Click(object sender, EventArgs e)
        {
            if (!(cbExamens.SelectedItem is ExamenItem examen))
            {
                MessageBox.Show("Veuillez sélectionner un examen."); 
                return;
            }


            var notes = dgvNotes.DataSource as List<NoteItem>;
            if (notes == null) return;

            MySqlConnection conn = null;
            MySqlCommand cmd = null;

            try
            {
                conn = new MySqlConnection(connectionString);
                conn.Open();

                foreach (var note in notes)
                {
                    // Si la note est null ou hors bornes, on l'ignore ou supprime la note existante
                    if (note.Valeur == null)
                    {
                        // Supprimer note existante si elle existe
                        if (note.NoteId != 0)
                        {
                            string deleteQuery = "DELETE FROM notes WHERE id = @id";
                            cmd = new MySqlCommand(deleteQuery, conn);
                            cmd.Parameters.AddWithValue("@id", note.NoteId);
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }
                        continue;
                    }

                    // Note valide => insert ou update
                    if (note.NoteId == 0)
                    {
                        string insertQuery = "INSERT INTO notes (examen_id, etudiant_id, valeur) VALUES (@examenId, @etudiantId, @valeur)";
                        cmd = new MySqlCommand(insertQuery, conn);
                        cmd.Parameters.AddWithValue("@examenId", examen.Id);
                        cmd.Parameters.AddWithValue("@etudiantId", note.EtudiantId);
                        cmd.Parameters.AddWithValue("@valeur", note.Valeur);
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                    else
                    {
                        string updateQuery = "UPDATE notes SET valeur = @valeur WHERE id = @id";
                        cmd = new MySqlCommand(updateQuery, conn);
                        cmd.Parameters.AddWithValue("@valeur", note.Valeur);
                        cmd.Parameters.AddWithValue("@id", note.NoteId);
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                }

                MessageBox.Show("Notes enregistrées avec succès !");
                ChargerNotes(examen.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de l'enregistrement : " + ex.Message);
            }
            finally
            {
                if (cmd != null) cmd.Dispose();
                if (conn != null) conn.Close();
            }
        }

        private class ExamenItem
        {
            public int Id { get; set; }
            public string Description { get; set; }
            public override string ToString() => Description;
        }

        private class NoteItem
        {
            public int NoteId { get; set; }
            public int EtudiantId { get; set; }
            public string NomEtudiant { get; set; }
            public float? Valeur { get; set; }
        }
    }
}
