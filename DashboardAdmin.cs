using System;
using System.Windows.Forms;
using ProjetCsharp;

namespace GestionExamensApp
{
    public class DashboardAdmin : Form
    {
        private Panel panelMenu;
        private Panel panelContent;

        private Button btnGestionMatieres;
        private Button btnAffectation;
        private Button btnGestionUtilisateurs;
        private Button btnRapports;
        private Button btnResultats;
        private Button btnSessionsExamens;
        private Button btnValidationNotes;
        private Button btnDeconnexion;

        public DashboardAdmin()
        {
            this.Text = "Dashboard Admin";
            this.Width = 1000;
            this.Height = 700;
            this.StartPosition = FormStartPosition.CenterScreen;

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // Panel Menu à gauche
            panelMenu = new Panel
            {
                Dock = DockStyle.Left,
                Width = 200,
                BackColor = System.Drawing.Color.FromArgb(41, 53, 65)
            };
            this.Controls.Add(panelMenu);

            // Panel Content (zone principale)
            panelContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.White
            };
            this.Controls.Add(panelContent);

            // Création des boutons menu
            btnGestionMatieres = CreateMenuButton("Gestion matières");
            btnGestionMatieres.Click += (s, e) => AfficherForm(new GestionMatieresForm());

            btnAffectation = CreateMenuButton("Affectation");
            btnAffectation.Click += (s, e) => AfficherForm(new AffectationForm());

            btnGestionUtilisateurs = CreateMenuButton("Gestion utilisateurs");
            btnGestionUtilisateurs.Click += (s, e) => AfficherForm(new GestionUtilisateursForm());

            btnRapports = CreateMenuButton("Rapports");
            btnRapports.Click += (s, e) => AfficherForm(new RapportForm());

            btnResultats = CreateMenuButton("Résultats");
            btnResultats.Click += (s, e) => AfficherForm(new ResultatsForm());

            btnSessionsExamens = CreateMenuButton("Sessions examens");
            btnSessionsExamens.Click += (s, e) => AfficherForm(new SessionsExamensForm());

            btnValidationNotes = CreateMenuButton("Validation notes");
            int monId = 1; // Pour tester, tu peux mettre 1 par exemple
            btnValidationNotes.Click += (s, e) => AfficherForm(new ValidationNotesForm(monId));

            btnDeconnexion = new Button
            {
                Text = "Déconnexion",
                Dock = DockStyle.Bottom,
                Height = 50,
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                BackColor = System.Drawing.Color.FromArgb(231, 76, 60)
            };
            btnDeconnexion.FlatAppearance.BorderSize = 0;
            btnDeconnexion.Click += BtnDeconnexion_Click;

            // Ajout des boutons au panelMenu (ordre du haut vers le bas)
            panelMenu.Controls.Add(btnDeconnexion);
            panelMenu.Controls.Add(btnValidationNotes);
            panelMenu.Controls.Add(btnSessionsExamens);
            panelMenu.Controls.Add(btnResultats);
            panelMenu.Controls.Add(btnRapports);
            panelMenu.Controls.Add(btnGestionUtilisateurs);
            panelMenu.Controls.Add(btnAffectation);
            panelMenu.Controls.Add(btnGestionMatieres);

            // Afficher une page d'accueil au lancement
            AfficherMessage("Bienvenue sur le Dashboard Admin");
        }

        private Button CreateMenuButton(string text)
        {
            var btn = new Button
            {
                Text = text,
                Dock = DockStyle.Top,
                Height = 50,
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                BackColor = System.Drawing.Color.FromArgb(31, 41, 51)
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void AfficherForm(Form form)
        {
            panelContent.Controls.Clear();

            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;

            panelContent.Controls.Add(form);
            form.Show();
        }

        private void AfficherMessage(string message)
        {
            panelContent.Controls.Clear();

            Label lbl = new Label
            {
                Text = message,
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Arial", 24, System.Drawing.FontStyle.Bold),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            panelContent.Controls.Add(lbl);
        }

        private void BtnDeconnexion_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Voulez-vous vraiment vous déconnecter ?", "Déconnexion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Close();
                
            }
        }
    }
}
