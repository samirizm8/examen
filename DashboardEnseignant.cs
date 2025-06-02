using System;
using System.Windows.Forms;
using System.Linq;

namespace GestionExamensApp
{
    public class DashboardEnseignant : Form
    {
        public DashboardEnseignant()
        {
            this.Text = "Espace Enseignant";
            this.Width = 600;
            this.Height = 400;

            ComboBox examenCombo = new ComboBox { Left = 20, Top = 20, Width = 300 };
            TextBox noteBox = new TextBox { Left = 340, Top = 20, Width = 50 };
            Button ajouterNote = new Button { Text = "Ajouter Note", Left = 400, Top = 20 };

            foreach (var ex in FakeDatabase.Examens)
                examenCombo.Items.Add(ex.Matiere);

            ajouterNote.Click += (s, e) =>
            {
                var examen = FakeDatabase.Examens.FirstOrDefault(x => x.Matiere == (string)examenCombo.SelectedItem);
                if (examen != null && float.TryParse(noteBox.Text, out float note))
                {
                    examen.Notes["ET001"] = note;
                    MessageBox.Show("Note ajoutée !");
                }
            };

            this.Controls.Add(examenCombo);
            this.Controls.Add(noteBox);
            this.Controls.Add(ajouterNote);
        }
    }
}