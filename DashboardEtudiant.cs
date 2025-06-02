using System;
using System.Windows.Forms;

namespace GestionExamensApp
{
    public class DashboardEtudiant : Form
    {
        public DashboardEtudiant()
        {
            this.Text = "Espace Étudiant";
            this.Width = 500;
            this.Height = 300;

            ListBox examensList = new ListBox { Width = 400, Height = 200, Top = 20, Left = 20 };
            Button voirNotes = new Button { Text = "Voir Notes", Top = 230, Left = 20 };

            foreach (var ex in FakeDatabase.Examens)
            {
                examensList.Items.Add($"{ex.Matiere} - {ex.Date.ToShortDateString()}");
            }

            voirNotes.Click += (s, e) =>
            {
                MessageBox.Show("Fonctionalité de notes (exemple) : Mathématiques: 14 Informatique: 16");
            };

            this.Controls.Add(examensList);
            this.Controls.Add(voirNotes);
        }
    }
}