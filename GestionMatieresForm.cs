using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace GestionExamensApp
{
    public class Matiere
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public int? EnseignantId { get; set; }
        public string NomEnseignant { get; set; }
    }

    public class GestionMatieres
    {
        private string connectionString = "Server=localhost;Database=gestion_examens;Uid=root;Pwd=ton_mot_de_passe;";

        public List<Matiere> ListerMatieres()
        {
            var matieres = new List<Matiere>();
            MySqlConnection conn = null;
            MySqlCommand cmd = null;
            MySqlDataReader reader = null;

            try
            {
                conn = new MySqlConnection(connectionString);
                conn.Open();

                string query = @"
                    SELECT m.id, m.nom, m.enseignant_id, e.nom AS nom_enseignant
                    FROM matieres m
                    LEFT JOIN enseignants e ON m.enseignant_id = e.id";

                cmd = new MySqlCommand(query, conn);
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    matieres.Add(new Matiere
                    {
                        Id = reader.GetInt32("id"),
                        Nom = reader.GetString("nom"),
                        EnseignantId = reader.IsDBNull(reader.GetOrdinal("enseignant_id")) ? (int?)null : reader.GetInt32("enseignant_id"),
                        NomEnseignant = reader.IsDBNull(reader.GetOrdinal("nom_enseignant")) ? "Non assigné" : reader.GetString("nom_enseignant")
                    });
                }
            }
            finally
            {
                if (reader != null) reader.Close();
                if (cmd != null) cmd.Dispose();
                if (conn != null) conn.Close();
            }

            return matieres;
        }

        public void AjouterMatiere(string nomMatiere, int? enseignantId)
        {
            MySqlConnection conn = null;
            MySqlCommand cmd = null;

            try
            {
                conn = new MySqlConnection(connectionString);
                conn.Open();

                string query = "INSERT INTO matieres (nom, enseignant_id) VALUES (@nom, @enseignantId)";
                cmd = new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@nom", nomMatiere);
                if (enseignantId.HasValue)
                    cmd.Parameters.AddWithValue("@enseignantId", enseignantId.Value);
                else
                    cmd.Parameters.AddWithValue("@enseignantId", DBNull.Value);

                cmd.ExecuteNonQuery();
            }
            finally
            {
                if (cmd != null) cmd.Dispose();
                if (conn != null) conn.Close();
            }
        }

        public void ModifierMatiere(int id, string nouveauNom, int? nouveauEnseignantId)
        {
            MySqlConnection conn = null;
            MySqlCommand cmd = null;

            try
            {
                conn = new MySqlConnection(connectionString);
                conn.Open();

                string query = "UPDATE matieres SET nom = @nom, enseignant_id = @enseignantId WHERE id = @id";
                cmd = new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@nom", nouveauNom);
                if (nouveauEnseignantId.HasValue)
                    cmd.Parameters.AddWithValue("@enseignantId", nouveauEnseignantId.Value);
                else
                    cmd.Parameters.AddWithValue("@enseignantId", DBNull.Value);

                cmd.Parameters.AddWithValue("@id", id);

                cmd.ExecuteNonQuery();
            }
            finally
            {
                if (cmd != null) cmd.Dispose();
                if (conn != null) conn.Close();
            }
        }

        public void SupprimerMatiere(int id)
        {
            MySqlConnection conn = null;
            MySqlCommand cmd = null;

            try
            {
                conn = new MySqlConnection(connectionString);
                conn.Open();

                string query = "DELETE FROM matieres WHERE id = @id";
                cmd = new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@id", id);

                cmd.ExecuteNonQuery();
            }
            finally
            {
                if (cmd != null) cmd.Dispose();
                if (conn != null) conn.Close();
            }
        }

        public List<(int Id, string Nom)> ListerEnseignants()
        {
            var enseignants = new List<(int, string)>();
            MySqlConnection conn = null;
            MySqlCommand cmd = null;
            MySqlDataReader reader = null;

            try
            {
                conn = new MySqlConnection(connectionString);
                conn.Open();

                string query = "SELECT id, nom FROM enseignants";
                cmd = new MySqlCommand(query, conn);
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    enseignants.Add((reader.GetInt32("id"), reader.GetString("nom")));
                }
            }
            finally
            {
                if (reader != null) reader.Close();
                if (cmd != null) cmd.Dispose();
                if (conn != null) conn.Close();
            }

            return enseignants;
        }
    }
}
