using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace GestionExamensApp
{
    public class SessionsExamensForm : Form
    {
        private ComboBox cbMatieres;
        private DateTimePicker dtpDateExam;
        private DataGridView dgvExamens;
        private Button btnAjouter, btnModifier, btnSupprimer;

        private string connectionString = "Server=localhost;Database=gestion_examens;Uid=root;Pwd=ton_mot_de_passe;";

        public SessionsExamensForm()
        {
            this.Text = "Gestion des Sessions d'Examens";
            this.Width = 700;
            this.Height = 500;

            Label lblMatiere = new Label() { Text = "Matière :", Left = 20, Top = 20 };
            cbMatieres = new ComboBox() { Left = 100, Top = 15, Width = 300 };

            Label lblDate = new Label() { Text = "Date examen :", Left = 420, Top = 20 };
            dtpDateExam = new DateTimePicker() { Left = 520, Top = 15, Width = 150 };

            dgvExamens = new DataGridView()
            {
                Left = 20,
                Top = 50,
                Width = 650,
                Height = 350,
                AllowUserToAddRows = false,
                AutoGenerateColumns = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            // Colonnes
            var colId = new DataGridViewTextBoxColumn() { HeaderText = "ID", DataPropertyName = "Id", Visible = false };
            var colMatiere = new DataGridViewTextBoxColumn() { HeaderText = "Matière", DataPropertyName = "NomMatiere", Width = 300 };
            var colDate = new DataGridViewTextBoxColumn() { HeaderText = "Date de l'examen", DataPropertyName = "DateExam", Width = 200 };

            dgvExamens.Columns.Add(colId);
            dgvExamens.Columns.Add(colMatiere);
            dgvExamens.Columns.Add(colDate);

            btnAjouter = new Button() { Text = "Ajouter", Left = 20, Top = 420, Width = 100 };
            btnModifier = new Button() { Text = "Modifier", Left = 130, Top = 420, Width = 100 };
            btnSupprimer = new Button() { Text = "Supprimer", Left = 240, Top = 420, Width = 100 };

            btnAjouter.Click += BtnAjouter_Click;
            btnModifier.Click += BtnModifier_Click;
            btnSupprimer.Click += BtnSupprimer_Click;

            this.Controls.Add(lblMatiere);
            this.Controls.Add(cbMatieres);
            this.Controls.Add(lblDate);
            this.Controls.Add(dtpDateExam);
            this.Controls.Add(dgvExamens);
            this.Controls.Add(btnAjouter);
            this.Controls.Add(btnModifier);
            this.Controls.Add(btnSupprimer);

            ChargerMatieres();
            ChargerExamens();
        }

        private void ChargerMatieres()
        {
            cbMatieres.Items.Clear();

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
            }
            finally
            {
                if (reader != null) reader.Close();
                if (cmd != null) cmd.Dispose();
                if (conn != null) conn.Close();
            }

            if (cbMatieres.Items.Count > 0)
                cbMatieres.SelectedIndex = 0;
        }

        private void ChargerExamens()
        {
            List<ExamenItem> examens = new List<ExamenItem>();

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
                    examens.Add(new ExamenItem
                    {
                        Id = reader.GetInt32("id"),
                        NomMatiere = reader.GetString("nom_matiere"),
                        DateExam = reader.GetDateTime("date_exam").ToShortDateString()
                    });
                }
            }
            finally
            {
                if (reader != null) reader.Close();
                if (cmd != null) cmd.Dispose();
                if (conn != null) conn.Close();
            }

            dgvExamens.DataSource = null;
            dgvExamens.DataSource = examens;
        }

        private void BtnAjouter_Click(object sender, EventArgs e)
        {
            if (cbMatieres.SelectedItem is MatiereItem matiere)
            {
                MySqlConnection conn = null;
                MySqlCommand cmd = null;

                try
                {
                    conn = new MySqlConnection(connectionString);
                    conn.Open();

                    string query = "INSERT INTO examens (matiere_id, date_exam) VALUES (@matiereId, @dateExam)";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@matiereId", matiere.Id);
                    cmd.Parameters.AddWithValue("@dateExam", dtpDateExam.Value.Date);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Session d'examen ajoutée !");
                    ChargerExamens();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur lors de l'ajout : " + ex.Message);
                }
                finally
                {
                    if (cmd != null) cmd.Dispose();
                    if (conn != null) conn.Close();
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une matière.");
            }
        }

        private void BtnModifier_Click(object sender, EventArgs e)
        {
            if (dgvExamens.SelectedRows.Count == 0)
            {
                MessageBox.Show("Veuillez sélectionner une session d'examen à modifier.");
                return;
            }

            var selectedExamen = dgvExamens.SelectedRows[0].DataBoundItem as ExamenItem;
            if (selectedExamen == null) return;

            MySqlConnection conn = null;
            MySqlCommand cmd = null;

            try
            {
                conn = new MySqlConnection(connectionString);
                conn.Open();

                string query = "UPDATE examens SET date_exam = @dateExam WHERE id = @id";

                cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@dateExam", dtpDateExam.Value.Date);
                cmd.Parameters.AddWithValue("@id", selectedExamen.Id);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Session d'examen modifiée !");
                ChargerExamens();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la modification : " + ex.Message);
            }
            finally
            {
                if (cmd != null) cmd.Dispose();
                if (conn != null) conn.Close();
            }
        }

        private void BtnSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvExamens.SelectedRows.Count == 0)
            {
                MessageBox.Show("Veuillez sélectionner une session d'examen à supprimer.");
                return;
            }

            var selectedExamen = dgvExamens.SelectedRows[0].DataBoundItem as ExamenItem;
            if (selectedExamen == null) return;

            if (MessageBox.Show("Voulez-vous vraiment supprimer cette session d'examen ?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                MySqlConnection conn = null;
                MySqlCommand cmd = null;

                try
                {
                    conn = new MySqlConnection(connectionString);
                    conn.Open();

                    string query = "DELETE FROM examens WHERE id = @id";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", selectedExamen.Id);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Session d'examen supprimée !");
                    ChargerExamens();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur lors de la suppression : " + ex.Message);
                }
                finally
                {
                    if (cmd != null) cmd.Dispose();
                    if (conn != null) conn.Close();
                }
            }
        }

        // Classes pour liaison des données
        private class MatiereItem
        {
            public int Id { get; set; }
            public string Nom { get; set; }
            public override string ToString() => Nom;
        }

        private class ExamenItem
        {
            public int Id { get; set; }
            public string NomMatiere { get; set; }
            public string DateExam { get; set; }
        }
    }
}
