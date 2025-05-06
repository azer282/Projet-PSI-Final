using MySql.Data.MySqlClient;
using System;

namespace Projet_PSI
{
    static class Statistiques
    {
        public static void ModuleStatistiques(MySqlConnection connexion)
        {
            Console.WriteLine("\nModule Statistiques");
            Console.WriteLine("1. Nombre de livraisons par cuisinier");
            Console.WriteLine("2. Commandes par période");
            Console.WriteLine("3. Moyenne des prix des commandes");
            Console.WriteLine("4. Moyenne des comptes clients");
            Console.WriteLine("5. Commandes par nationalité et période");
            Console.Write("Choix : ");

            string choix = Console.ReadLine();

            switch (choix)
            {
                case "1":
                    AfficherLivraisonsParCuisinier(connexion);
                    break;
                case "2":
                    AfficherCommandesParPeriode(connexion);
                    break;
                case "3":
                    AfficherMoyennePrixCommandes(connexion);
                    break;
                case "4":
                    AfficherMoyenneComptesClients(connexion);
                    break;
                case "5":
                    AfficherCommandesParNationalite(connexion);
                    break;
                default:
                    Console.WriteLine("Option invalide");
                    break;
            }
        }

        private static void AfficherLivraisonsParCuisinier(MySqlConnection connexion)
        {
            string query = @"select c.pseudocu, count(l.id_commande) as nb_livraisons
                   from cuisinier c
                   left JOIN livre l ON c.id_cuisinier = l.id_cuisinier
                   group by c.id_cuisinier
                   order by nb_livraisons DESC";

            Console.WriteLine("\nNombre de livraisons par cuisinier :");
            MySqlCommand cmd = new MySqlCommand(query, connexion);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"- {reader["pseudocu"]} : {reader["nb_livraisons"]} livraisons");
                }
            }
        }

        private static void AfficherCommandesParPeriode(MySqlConnection connexion)
        {
            Console.Write("Date de début (AAAA-MM-JJ) : ");
            string dateDebut = Console.ReadLine();
            if (!DateTime.TryParse(dateDebut, out DateTime debut))
            {
                Console.WriteLine("Erreur : Date de début invalide. Utilisez AAAA-MM-JJ (ex. 2025-01-01).");
                return;
            }

            Console.Write("Date de fin (AAAA-MM-JJ) : ");
            string dateFin = Console.ReadLine();
            if (!DateTime.TryParse(dateFin, out DateTime fin))
            {
                Console.WriteLine("Erreur : Date de fin invalide. Utilisez AAAA-MM-JJ (ex. 2025-12-31).");
                return;
            }

            if (debut > fin)
            {
                Console.WriteLine("Erreur : La date de début doit être antérieure à la date de fin.");
                return;
            }

            string query = @"select date(l.date_livraison) as date, count(*) as nb_commandes
                   from livre l
                   where l.date_livraison BETWEEN @debut AND @fin
                   group by DATE(l.date_livraison)
                   order by date";

            MySqlCommand cmd = new MySqlCommand(query, connexion);
            cmd.Parameters.AddWithValue("@debut", dateDebut);
            cmd.Parameters.AddWithValue("@fin", dateFin);

            Console.WriteLine($"\nCommandes entre {dateDebut} et {dateFin} :");
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"- {reader["date"]} : {reader["nb_commandes"]} commandes");
                }
            }
        }

        private static void AfficherMoyennePrixCommandes(MySqlConnection connexion)
        {
            string query = @"select avg(total) as moyenne
                   from (
                       select sum(p.PPP) as total
                       from contient c
                       join plat p ON c.nom_plat_ = p.nom_plat_
                       group by c.id_commande
                   ) as prix_commandes";

            MySqlCommand cmd = new MySqlCommand(query, connexion);
            object result = cmd.ExecuteScalar();
            decimal moyenne = result != null ? Convert.ToDecimal(result) : 0;
            Console.WriteLine($"\nMoyenne des prix des commandes : {Math.Round(moyenne, 2)}€");
        }

        private static void AfficherMoyenneComptesClients(MySqlConnection connexion)
        {
            string query = @"select avg(montant) as moyenne
                   from (
                       select SUM(p.PPP) as montant
                       from client cl
                       join livre l ON cl.id_client = l.id_client
                       join contient c ON l.id_commande = c.id_commande
                       join plat p ON c.nom_plat_ = p.nom_plat_
                       group by cl.id_client
                   ) as montants_clients";

            MySqlCommand cmd = new MySqlCommand(query, connexion);
            object result = cmd.ExecuteScalar();
            decimal moyenne = result != null ? Convert.ToDecimal(result) : 0;
            Console.WriteLine($"\nMoyenne des comptes clients : {Math.Round(moyenne, 2)}€");
        }

        private static void AfficherCommandesParNationalite(MySqlConnection connexion)
        {
            Console.Write("Nationalité des plats : ");
            string nationalite = Console.ReadLine();
            Console.Write("Date de début (AAAA-MM-JJ) : ");
            string dateDebut = Console.ReadLine();
            if (!DateTime.TryParse(dateDebut, out DateTime debut))
            {
                Console.WriteLine("Erreur : Date de début invalide. Utilisez AAAA-MM-JJ (ex. 2025-01-01).");
                return;
            }

            Console.Write("Date de fin (AAAA-MM-JJ) : ");
            string dateFin = Console.ReadLine();
            if (!DateTime.TryParse(dateFin, out DateTime fin))
            {
                Console.WriteLine("Erreur : Date de fin invalide. Utilisez AAAA-MM-JJ (ex. 2025-12-31).");
                return;
            }

            if (debut > fin)
            {
                Console.WriteLine("Erreur : La date de début doit être antérieure à la date de fin.");
                return;
            }

            string query = @"select cl.pseudocl, COUNT(*) as nb_commandes
                   from client cl
                   join livre l ON cl.id_client = l.id_client
                   join contient c ON l.id_commande = c.id_commande
                   join plat p ON c.nom_plat_ = p.nom_plat_
                   where p.origine = @nationalite
                   and l.date_livraison BETWEEN @debut AND @fin
                   group by cl.id_client
                   order by nb_commandes DESC";

            MySqlCommand cmd = new MySqlCommand(query, connexion);
            cmd.Parameters.AddWithValue("@nationalite", nationalite);
            cmd.Parameters.AddWithValue("@debut", dateDebut);
            cmd.Parameters.AddWithValue("@fin", dateFin);

            Console.WriteLine($"\nCommandes de plats {nationalite} entre {dateDebut} et {dateFin} :");
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"- Client {reader["pseudocl"]} : {reader["nb_commandes"]} commandes");
                }
            }
        }
    }
}