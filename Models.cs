using System;
using System.Collections.Generic;

namespace GestionExamensApp
{
    public class Etudiant
    {
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Matricule { get; set; }
    }

    public class Examen
    {
        public string Matiere { get; set; }
        public DateTime Date { get; set; }
        public Dictionary<string, float> Notes { get; set; } = new Dictionary<string, float>();
    }

    public static class FakeDatabase
    {
        public static List<Etudiant> Etudiants = new List<Etudiant>
        {
            new Etudiant { Nom = "Hayar", Prenom = "Ilyass", Matricule = "ET001" },
            new Etudiant { Nom = "Badr", Prenom = "Zaid", Matricule = "ET002" }
        };

        public static List<Examen> Examens = new List<Examen>
        {
            new Examen { Matiere = "Mathématiques", Date = DateTime.Today.AddDays(2) },
            new Examen { Matiere = "Informatique", Date = DateTime.Today.AddDays(4) }
        };
    }
}