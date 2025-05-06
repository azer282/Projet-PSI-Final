using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Text.Json;
using MySql.Data.MySqlClient;

namespace Projet_PSI
{
    public class DataExporter
    {
        private string connectionString;

        public DataExporter(string connectionString)
        {
            this.connectionString = connectionString;
        }

        
        public class UserData
        {
            public List<Particulier> Particuliers { get; set; }
            public List<Client> Clients { get; set; }
            public List<Cuisinier> Cuisiniers { get; set; }
            public List<Commande> Commandes { get; set; }
        }

        public class Particulier
        {
            public string Email { get; set; }
            public string Pseudo { get; set; }
            public string Metro { get; set; }
        }

        public class Client
        {
            public string IdClient { get; set; }
            public string PseudoCl { get; set; }
            public string MetroCl { get; set; }
        }

        public class Cuisinier
        {
            public string IdCuisinier { get; set; }
            public string PseudoCu { get; set; }
            public string MetroCui { get; set; }
        }

        public class Commande
        {
            public string IdCommande { get; set; }
            public string IdClient { get; set; }
            public string IdCuisinier { get; set; }
            public string DateLivraison { get; set; }
        }

        public void ExportToXml(string filePath)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var data = new UserData();

                string queryParticulier = "SELECT Email, pseudo, Metro FROM Particulier";
                using (MySqlCommand cmd = new MySqlCommand(queryParticulier, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        data.Particuliers = new List<Particulier>();
                        while (reader.Read())
                        {
                            data.Particuliers.Add(new Particulier
                            {
                                Email = reader["Email"].ToString(),
                                Pseudo = reader["pseudo"].ToString(),
                                Metro = reader["Metro"].ToString()
                            });
                        }
                    }
                }

                
                string queryClient = "SELECT id_client, pseudoCl, metrocl FROM Client";
                using (MySqlCommand cmd = new MySqlCommand(queryClient, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        data.Clients = new List<Client>();
                        while (reader.Read())
                        {
                            data.Clients.Add(new Client
                            {
                                IdClient = reader["id_client"].ToString(),
                                PseudoCl = reader["pseudoCl"].ToString(),
                                MetroCl = reader["metrocl"].ToString()
                            });
                        }
                    }
                }

                
                string queryCuisinier = "SELECT id_cuisinier, pseudoCu, metroCui FROM Cuisinier";
                using (MySqlCommand cmd = new MySqlCommand(queryCuisinier, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        data.Cuisiniers = new List<Cuisinier>();
                        while (reader.Read())
                        {
                            data.Cuisiniers.Add(new Cuisinier
                            {
                                IdCuisinier = reader["id_cuisinier"].ToString(),
                                PseudoCu = reader["pseudoCu"].ToString(),
                                MetroCui = reader["metroCui"].ToString()
                            });
                        }
                    }
                }

               
                string queryCommande = @"
                    SELECT c.id_commande, l.id_client, l.id_cuisinier, l.date_livraison 
                    FROM Commande c 
                    JOIN livre l ON c.id_commande = l.id_commande";
                using (MySqlCommand cmd = new MySqlCommand(queryCommande, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        data.Commandes = new List<Commande>();
                        while (reader.Read())
                        {
                            data.Commandes.Add(new Commande
                            {
                                IdCommande = reader["id_commande"].ToString(),
                                IdClient = reader["id_client"].ToString(),
                                IdCuisinier = reader["id_cuisinier"].ToString(),
                                DateLivraison = reader["date_livraison"].ToString()
                            });
                        }
                    }
                }

                
                XmlSerializer serializer = new XmlSerializer(typeof(UserData));
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    serializer.Serialize(writer, data);
                }
            }
        }

        public void ExportToJson(string filePath)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var data = new UserData();

                // Récupérer les particuliers
                string queryParticulier = "SELECT Email, pseudo, Metro FROM Particulier";
                using (MySqlCommand cmd = new MySqlCommand(queryParticulier, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        data.Particuliers = new List<Particulier>();
                        while (reader.Read())
                        {
                            data.Particuliers.Add(new Particulier
                            {
                                Email = reader["Email"].ToString(),
                                Pseudo = reader["pseudo"].ToString(),
                                Metro = reader["Metro"].ToString()
                            });
                        }
                    }
                }

                // Récupérer les clients
                string queryClient = "SELECT id_client, pseudocl, metrocl FROM client";
                using (MySqlCommand cmd = new MySqlCommand(queryClient, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        data.Clients = new List<Client>();
                        while (reader.Read())
                        {
                            data.Clients.Add(new Client
                            {
                                IdClient = reader["id_client"].ToString(),
                                PseudoCl = reader["pseudocl"].ToString(),
                                MetroCl = reader["metrocl"].ToString()
                            });
                        }
                    }
                }

                // Récupérer les cuisiniers
                string queryCuisinier = "SELECT id_cuisinier, pseudocu, metrocui FROM cuisinier";
                using (MySqlCommand cmd = new MySqlCommand(queryCuisinier, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        data.Cuisiniers = new List<Cuisinier>();
                        while (reader.Read())
                        {
                            data.Cuisiniers.Add(new Cuisinier
                            {
                                IdCuisinier = reader["id_cuisinier"].ToString(),
                                PseudoCu = reader["pseudocu"].ToString(),
                                MetroCui = reader["metrocui"].ToString()
                            });
                        }
                    }
                }

                // Récupérer les commandes
                string queryCommande = @"
                    SELECT c.id_commande, l.id_client, l.id_cuisinier, l.date_livraison 
                    FROM Commande c 
                    JOIN livre l ON c.id_commande = l.id_commande";
                using (MySqlCommand cmd = new MySqlCommand(queryCommande, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        data.Commandes = new List<Commande>();
                        while (reader.Read())
                        {
                            data.Commandes.Add(new Commande
                            {
                                IdCommande = reader["id_commande"].ToString(),
                                IdClient = reader["id_client"].ToString(),
                                IdCuisinier = reader["id_cuisinier"].ToString(),
                                DateLivraison = reader["date_livraison"].ToString()
                            });
                        }
                    }
                }

                // Sérialiser en JSON
                string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
            }
        }

        public void ImportFromXml(string filePath)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Désérialiser le XML
                XmlSerializer serializer = new XmlSerializer(typeof(UserData));
                UserData data;
                using (StreamReader reader = new StreamReader(filePath))
                {
                    data = (UserData)serializer.Deserialize(reader);
                }

                // Importer les données dans la base
                ImportData(connection, data);
            }
        }

        public void ImportFromJson(string filePath)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Désérialiser le JSON
                string json = File.ReadAllText(filePath);
                UserData data = JsonSerializer.Deserialize<UserData>(json);

                // Importer les données dans la base
                ImportData(connection, data);
            }
        }

        private void ImportData(MySqlConnection connection, UserData data)
        {
            // Importer les particuliers
            foreach (var particulier in data.Particuliers)
            {
                string query = "INSERT INTO Particulier (Email, pseudo, Metro) VALUES (@email, @pseudo, @metro)";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@email", particulier.Email);
                    cmd.Parameters.AddWithValue("@pseudo", particulier.Pseudo);
                    cmd.Parameters.AddWithValue("@metro", particulier.Metro);
                    cmd.ExecuteNonQuery();
                }
            }

            // Importer les clients
            foreach (var client in data.Clients)
            {
                string query = "INSERT INTO Client (id_client, pseudoCl, metrocl) VALUES (@id, @pseudo, @metro)";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", client.IdClient);
                    cmd.Parameters.AddWithValue("@pseudo", client.PseudoCl);
                    cmd.Parameters.AddWithValue("@metro", client.MetroCl);
                    cmd.ExecuteNonQuery();
                }
            }

            // Importer les cuisiniers
            foreach (var cuisinier in data.Cuisiniers)
            {
                string query = "INSERT INTO Cuisinier (id_cuisinier, pseudoCu, metroCui) VALUES (@id, @pseudo, @metro)";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", cuisinier.IdCuisinier);
                    cmd.Parameters.AddWithValue("@pseudo", cuisinier.PseudoCu);
                    cmd.Parameters.AddWithValue("@metro", cuisinier.MetroCui);
                    cmd.ExecuteNonQuery();
                }
            }

            // Importer les commandes et livraisons
            foreach (var commande in data.Commandes)
            {
                // Insérer la commande
                string queryCommande = "INSERT INTO Commande (id_commande) VALUES (@id)";
                using (MySqlCommand cmd = new MySqlCommand(queryCommande, connection))
                {
                    cmd.Parameters.AddWithValue("@id", commande.IdCommande);
                    cmd.ExecuteNonQuery();
                }

                // Insérer la livraison
                string queryLivraison = "INSERT INTO livre (id_commande, id_client, id_cuisinier, date_livraison) VALUES (@id, @client, @cuisinier, @date)";
                using (MySqlCommand cmd = new MySqlCommand(queryLivraison, connection))
                {
                    cmd.Parameters.AddWithValue("@id", commande.IdCommande);
                    cmd.Parameters.AddWithValue("@client", commande.IdClient);
                    cmd.Parameters.AddWithValue("@cuisinier", commande.IdCuisinier);
                    cmd.Parameters.AddWithValue("@date", commande.DateLivraison);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
} 