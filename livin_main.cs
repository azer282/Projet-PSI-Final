using System;
using System.Data.SqlClient;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics.Metrics;
using MySql.Data.MySqlClient;
using Mysqlx.Expr;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using Projet_PSI;

namespace Projet_PSI
{
    class livin_main
    {
        private static Graphe<int> graphe;
        public static List<MetroStation> stations;

        static void Main(string[] args)
        {
            string fichier = "../../MetroParisArc.csv";
            fichier = Path.GetFullPath(fichier);
            graphe = ImporterGraphe(fichier);

            if (graphe == null)
            {
                Console.WriteLine("erreur de chargement du graphe");
                return;
            }

            // Charger les stations de métro
            stations = MetroStation.LoadStations("MetroParis(Noeuds).csv");
            Console.WriteLine($"{stations.Count} stations de métro chargées.");

            Console.WriteLine("=== Programme de gestion du métro parisien ===");
            Console.WriteLine("Que souhaitez-vous faire ?");
            Console.WriteLine("1. Visualiser les stations de métro");
            Console.WriteLine("2. Accéder à la base de données");
            Console.WriteLine("3. Exporter/Importer des données");
            Console.Write("\nVotre choix : ");

            if (int.TryParse(Console.ReadLine(), out int choixPrincipal))
            {
                switch (choixPrincipal)
                {
                    case 1:
                        AfficherMenuMetro();
                        break;

                    case 2:
                        GestionBaseDeDonnees();
                        break;

                    case 3:
                        GestionExportImport();
                        break;

                    default:
                        Console.WriteLine("Choix invalide. Veuillez entrer 1, 2 ou 3.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Veuillez entrer un nombre valide.");
            }

            Console.WriteLine("\nAppuyez sur une touche pour quitter...");
            Console.ReadKey();
        }

        static void AfficherMenuMetro()
        {
            bool continuer = true;
            while (continuer)
            {
                Console.WriteLine("\nMenu des stations de métro :");
                Console.WriteLine("1. Afficher toutes les stations");
                Console.WriteLine("2. Afficher les stations par ligne");
                Console.WriteLine("3. Rechercher une station");
                Console.WriteLine("4. Générer la carte du métro");
                Console.WriteLine("5. Retour au menu principal");
                Console.Write("\nVotre choix : ");

                if (int.TryParse(Console.ReadLine(), out int choix))
                {
                    switch (choix)
                    {
                        case 1:
                            Console.WriteLine("\n=== Liste de toutes les stations ===");
                            foreach (var station in stations)
                            {
                                station.DisplayStation();
                            }
                            break;

                        case 2:
                            Console.WriteLine("\n=== Stations par ligne ===");
                            MetroStation.DisplayStationsByLine(stations);
                            break;

                        case 3:
                            Console.Write("\nEntrez un terme de recherche (nom de station, ligne ou commune) : ");
                            string searchTerm = Console.ReadLine();
                            MetroStation.SearchStation(stations, searchTerm);
                            break;

                        case 4:
                            Console.WriteLine("\nGénération de la carte du métro...");
                            MetroStation.CreateMetroMap(stations);
                            Console.WriteLine("La carte a été générée dans le fichier MetroMap.png");
                            break;

                        case 5:
                            continuer = false;
                            break;

                        default:
                            Console.WriteLine("Choix invalide. Veuillez entrer un nombre entre 1 et 5.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Veuillez entrer un nombre valide.");
                }

                if (continuer)
                {
                    Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                    Console.ReadKey();
                }
            }
        }

        static void GestionBaseDeDonnees()
        {
            MySqlConnection maConnexion = null;
            try
            {
                Console.WriteLine("Es-tu un client ou un cuisinier ? (ou 'admin' pour voir les clients ou gérer les cuisiniers)");
                string role = Console.ReadLine()?.Trim().ToLower() ?? "inconnu";

                if (role == "metro")
                {
                    AfficherMenuMetro();
                    return;
                }

                string rootConnectionString = "server=localhost;port=3306;database=psi_niauma;uid=root;password=Beauville68!;";
                maConnexion = new MySqlConnection(rootConnectionString);
                maConnexion.Open();

                if (role == "client" || role == "cuisinier")
                {
                    // Demander les infos de l'utilisateur
                    Console.Write("Quel est ton pseudo ? ");
                    string pseudo = Console.ReadLine();
                    Console.Write("Ton mot de passe ? ");
                    string motDePasse = Console.ReadLine();
                    Console.Write("Ton email ? ");
                    string email = Console.ReadLine();

                    // vérifier si l'utilisateur existe dans Client ou Cuisinier (pas dans mysql.user)
                    string checkUserQuery = role == "client"
                        ? "select count(*) from client where pseudocl = @pseudo"
                        : "select count(*) from cuisinier where pseudocu = @pseudo";
                    MySqlCommand cmdCheckUser = new MySqlCommand(checkUserQuery, maConnexion);
                    cmdCheckUser.Parameters.AddWithValue("@pseudo", pseudo);
                    long userExists = (long)cmdCheckUser.ExecuteScalar();

                    string userId = null;

                    if (userExists > 0) //  si utilisateur existe
                    {
                        string getIdQuery = role == "client"
                            ? "select id_client from client where pseudocl = @pseudo and mot_de_passe = @mdp"
                            : "select id_cuisinier from cuisinier where pseudocu = @pseudo and mot_de_passe = @mdp";
                        MySqlCommand cmdGetId = new MySqlCommand(getIdQuery, maConnexion);
                        cmdGetId.Parameters.AddWithValue("@pseudo", pseudo);
                        cmdGetId.Parameters.AddWithValue("@mdp", motDePasse);
                        userId = cmdGetId.ExecuteScalar()?.ToString();

                        if (userId == null)
                        {
                            Console.WriteLine("Mauvais mot de passe ou utilisateur non trouvé.");
                            return;
                        }
                        Console.WriteLine($"Tu es connecté(e) comme '{pseudo}'.");
                    }
                    else // nouveau utilisateur
                    {
                        userId = Guid.NewGuid().ToString();
                        string stationMetro = "";

                        /*
                        Console.WriteLine("choisir pt de départ");
                        int ptDepart = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("choisir pt d'arrivée");
                        int ptArrivee = Convert.ToInt32(Console.ReadLine());
                        Algo_de_recherche Recherche2 = new Algo_de_recherche(graphe.matrice, ptDepart, ptArrivee, graphe.matrice.GetLength(0));
                        Console.WriteLine("Algo de recherche Dijkstra : ");
                        Recherche2.Dijkstra();*/


                        string insertParticulier = "insert into particulier (pseudo, email) values (@pseudo, @email)";
                        MySqlCommand cmdInsertParticulier = new MySqlCommand(insertParticulier, maConnexion);
                        cmdInsertParticulier.Parameters.AddWithValue("@pseudo", pseudo);
                        cmdInsertParticulier.Parameters.AddWithValue("@email", email);
                        cmdInsertParticulier.ExecuteNonQuery();

                        if (role == "client")
                        {
                            string insertClient = "insert into client (id_client, pseudocl, mot_de_passe, metrocl) values (@id, @pseudo, @mdp, @metro)";
                            MySqlCommand cmdInsertClient = new MySqlCommand(insertClient, maConnexion);
                            cmdInsertClient.Parameters.AddWithValue("@id", userId);
                            cmdInsertClient.Parameters.AddWithValue("@pseudo", pseudo);
                            cmdInsertClient.Parameters.AddWithValue("@mdp", motDePasse);
                            cmdInsertClient.Parameters.AddWithValue("@metro", stationMetro);
                            cmdInsertClient.ExecuteNonQuery();

                            string insertEstClient = "insert into est_client (identifiant, id_client) values (@email, @id)";
                            MySqlCommand cmdInsertEstClient = new MySqlCommand(insertEstClient, maConnexion);
                            cmdInsertEstClient.Parameters.AddWithValue("@email", email);
                            cmdInsertEstClient.Parameters.AddWithValue("@id", userId);
                            cmdInsertEstClient.ExecuteNonQuery();
                        }
                        else if (role == "cuisinier")
                        {
                            string insertCuisinier = "insert into cuisinier (id_cuisinier, pseudocu, mot_de_passe, metrocui) values (@id, @pseudo, @mdp, @metro)";
                            MySqlCommand cmdInsertCuisinier = new MySqlCommand(insertCuisinier, maConnexion);
                            cmdInsertCuisinier.Parameters.AddWithValue("@id", userId);
                            cmdInsertCuisinier.Parameters.AddWithValue("@pseudo", pseudo);
                            cmdInsertCuisinier.Parameters.AddWithValue("@mdp", motDePasse);
                            cmdInsertCuisinier.Parameters.AddWithValue("@metro", stationMetro);
                            cmdInsertCuisinier.ExecuteNonQuery();

                            string insertEstCuisinier = "insert into est_cuisinier (identifiant, id_cuisinier) values (@email, @id)";
                            MySqlCommand cmdInsertEstCuisinier = new MySqlCommand(insertEstCuisinier, maConnexion);
                            cmdInsertEstCuisinier.Parameters.AddWithValue("@email", email);
                            cmdInsertEstCuisinier.Parameters.AddWithValue("@id", userId);
                            cmdInsertEstCuisinier.ExecuteNonQuery();
                        }
                        Console.WriteLine($"Bienvenue, '{pseudo}' ! Tu es maintenant inscrit(e) et connecté(e).");
                    }


                    if (role == "client")
                    {
                        Console.WriteLine("Que veux-tu faire ? Commander un plat ou voir tes commandes ? (commander/voir)");
                        string choix = Console.ReadLine()?.ToLower() ?? "";

                        if (choix == "commander")
                        {
                            Console.WriteLine("Voici les plats disponibles :");
                            string queryPlats = "select nom_plat_,PPP from plat";
                            MySqlCommand cmdPlats = new MySqlCommand(queryPlats, maConnexion);
                            using (MySqlDataReader reader = cmdPlats.ExecuteReader())
                            {
                                if (!reader.HasRows)
                                {
                                    Console.WriteLine("Aucun plat n’est disponible pour l’instant.");
                                    return;
                                }
                                while (reader.Read())
                                {
                                    string nomPlat = reader["nom_plat_"]?.ToString() ?? "Inconnu";
                                    string PPP = reader["PPP"]?.ToString() ?? "Inconnu";
                                    Console.WriteLine($"- {nomPlat} : Prix --> {PPP}");
                                }
                            }
                            string idCommande = Guid.NewGuid().ToString();
                            string insertCommande = "insert into commande (id_commande) values (@id)";
                            MySqlCommand cmdInsertCommande = new MySqlCommand(insertCommande, maConnexion);
                            cmdInsertCommande.Parameters.AddWithValue("@id", idCommande);
                            cmdInsertCommande.ExecuteNonQuery();
                            bool ajouterPlat = true;
                            while (ajouterPlat)
                            {
                                bool platOk = false;
                                string plat = null;
                                while (!platOk)
                                {
                                    Console.Write("Quel plat veux-tu commander ? (ou 'annuler' pour arrêter) : ");
                                    plat = Console.ReadLine();
                                    if (plat?.ToLower() == "annuler")
                                    {
                                        Console.WriteLine("Commande annulée.");
                                        return;
                                    }
                                    string checkPlat = "select count(*) from plat where nom_plat_ = @nom";
                                    MySqlCommand cmdCheckPlat = new MySqlCommand(checkPlat, maConnexion);
                                    cmdCheckPlat.Parameters.AddWithValue("@nom", plat);
                                    long existe = (long)cmdCheckPlat.ExecuteScalar();
                                    if (existe == 0)
                                    {
                                        Console.WriteLine("Ce plat n'existe pas.");
                                    }
                                    else
                                    {
                                        platOk = true;
                                    }
                                }
                                string insertContient = "insert into contient (nom_plat_, id_commande) values (@nom, @id)";
                                MySqlCommand cmdInsertContient = new MySqlCommand(insertContient, maConnexion);
                                cmdInsertContient.Parameters.AddWithValue("@nom", plat);
                                cmdInsertContient.Parameters.AddWithValue("@id", idCommande);
                                cmdInsertContient.ExecuteNonQuery();
                                Console.WriteLine($"'{plat}' a été ajouté à ta commande !");
                                Console.WriteLine("Veux-tu ajouter un autre plat ? (oui/non)");
                                string reponse = Console.ReadLine()?.ToLower() ?? "";
                                if (reponse != "oui")
                                {
                                    ajouterPlat = false;
                                }
                            }
                            // ajouter les infos de livraison
                            Console.Write("Quand veux-tu être livré (ex. 2025-01-12) ? ");
                            string dateLivraison = Console.ReadLine();


                            string insertLivre = "insert into livre (id_commande, date_livraison, id_cuisinier, id_client) values (@id_commande, @date, '1', @id_client)";
                            MySqlCommand cmdInsertLivre = new MySqlCommand(insertLivre, maConnexion);
                            cmdInsertLivre.Parameters.AddWithValue("@id_commande", idCommande);
                            cmdInsertLivre.Parameters.AddWithValue("@date", dateLivraison);
                            cmdInsertLivre.Parameters.AddWithValue("@id_client", userId);
                            cmdInsertLivre.ExecuteNonQuery();


                            Console.WriteLine("choisir pt de départ (indice de la station du client) : ");
                            int ptDepart = Convert.ToInt32(Console.ReadLine());
                            Console.WriteLine("choisir pt d'arrivée (indice de la station du cuisinier) : ");
                            int ptArrivee = Convert.ToInt32(Console.ReadLine());
                            Algo_de_recherche Recherche2 = new Algo_de_recherche(graphe.matrice, ptDepart, ptArrivee, graphe.matrice.GetLength(0));
                            Console.WriteLine("algo de recherche dijkstra : ");
                            Recherche2.Dijkstra();


                            Console.WriteLine("ta commande est prête avec livraison ! voici les détails :");
                            string queryCommande = "select nom_plat_ from contient where id_commande = @id";
                            MySqlCommand cmdCommande = new MySqlCommand(queryCommande, maConnexion);
                            cmdCommande.Parameters.AddWithValue("@id", idCommande);
                            using (MySqlDataReader reader = cmdCommande.ExecuteReader())
                            {
                                if (!reader.HasRows)
                                {
                                    Console.WriteLine("aucun plat dans cette commande (erreur inattendue).");
                                }
                                while (reader.Read())
                                {
                                    string nomPlat = reader["nom_plat_"]?.ToString() ?? "inconnu";
                                    Console.WriteLine($"- {nomPlat}");
                                }
                            }
                            string queryLivraison = @"select l.date_livraison, cl.metrocl, cu.metrocui from livre l join client cl on l.id_client = cl.id_client join cuisinier cu on l.id_cuisinier = cu.id_cuisinier where l.id_commande = @id";
                            MySqlCommand cmdLivraison = new MySqlCommand(queryLivraison, maConnexion);
                            cmdLivraison.Parameters.AddWithValue("@id", idCommande);
                            using (MySqlDataReader reader = cmdLivraison.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    string date = reader["date_livraison"]?.ToString() ?? "inconnue";
                                    string stationCl = reader["metrocl"]?.ToString() ?? "inconnue";
                                    string stationCu = reader["metrocui"]?.ToString() ?? "inconnue";
                                    Console.WriteLine($"livraison prévue le {date} à {stationCl} (cuisinier : {stationCu})");
                                }
                                else
                                {
                                    Console.WriteLine("erreur : impossible de récupérer les détails de livraison.");
                                }
                            }
                        }
                        else if (choix == "voir")
                        {
                            Console.WriteLine("Voici toutes tes commandes :");
                            string queryCommandes = "select c.nom_plat_, l.date_livraison, l.id_cuisinier, cl.metrocl, cmd.id_commande from contient c join commande cmd on c.id_commande = cmd.id_commande join livre l on cmd.id_commande = l.id_commande join client cl on l.id_client = cl.id_client where l.id_client = @id";
                            MySqlCommand cmdCommandes = new MySqlCommand(queryCommandes, maConnexion);
                            cmdCommandes.Parameters.AddWithValue("@id", userId);

                            using (MySqlDataReader reader = cmdCommandes.ExecuteReader())
                            {
                                if (!reader.HasRows)
                                {
                                    Console.WriteLine("Tu n'as encore aucune commande avec livraison planifiée.");
                                }
                                while (reader.Read())
                                {
                                    string nomPlat = reader["nom_plat_"]?.ToString() ?? "Inconnu";
                                    string date = reader["date_livraison"]?.ToString() ?? "Inconnue";
                                    string idCuisinier = reader["id_cuisinier"]?.ToString() ?? "Inconnu";
                                    string stationCl = reader["metrocl"]?.ToString() ?? "Non précisée";
                                    string idCommande = reader["id_commande"]?.ToString() ?? "Inconnu";

                                    /// requête pour station du cuisinier
                                    string requeteCuisinier = "select c.metrocui from commande co join livre l on co.id_commande = l.id_commande join cuisinier c on l.id_cuisinier = c.id_cuisinier where co.id_commande = @idcommande";
                                    MySqlCommand cmdCuisinier = maConnexion.CreateCommand();
                                    cmdCuisinier.CommandText = requeteCuisinier;
                                    cmdCuisinier.Parameters.AddWithValue("@idcommande", idCommande);
                                    string stationCuisinier = cmdCuisinier.ExecuteScalar()?.ToString() ?? "Station non trouvée";

                                    Console.WriteLine($"- {nomPlat} (Livraison : {date}, Cuisinier ID : {idCuisinier}, Station client : {stationCl}, Station cuisinier : {stationCuisinier})");
                                }
                            }
                        }
                    }
                    else if (role == "cuisinier")
                    {
                        Console.WriteLine("Que voulez-vous faire ? (ajouter/modifier/supprimer/voir)");
                        string action = Console.ReadLine()?.ToLower() ?? "";

                        switch (action)
                        {
                            case "ajouter":
                                Console.Write("Nom du plat à ajouter : ");
                                string platAjouter = Console.ReadLine();
                                Console.Write("définir un prix ");
                                string PPP = Console.ReadLine();
                                string insertQuery = "insert into  plat (nom_plat_,PPP) values (@nom_plat,@PPP)";
                                MySqlCommand cmdInsert = new MySqlCommand(insertQuery, maConnexion);
                                cmdInsert.Parameters.AddWithValue("@nom_plat", platAjouter);
                                cmdInsert.Parameters.AddWithValue("@PPP", PPP);
                                cmdInsert.ExecuteNonQuery();

                                string insertPublieQuery = "insert into publie (nom_plat_, id_cuisinier) values (@nom_plat, @id_cuisinier)";
                                MySqlCommand cmdInsertPublie = new MySqlCommand(insertPublieQuery, maConnexion);
                                cmdInsertPublie.Parameters.AddWithValue("@nom_plat", platAjouter);
                                cmdInsertPublie.Parameters.AddWithValue("@id_cuisinier", userId);
                                cmdInsertPublie.ExecuteNonQuery();

                                Console.WriteLine("Plat ajouté !");
                                break;

                            case "modifier":
                                Console.WriteLine("Voici les plats disponibles :");
                                string queryPlats = "select nom_plat_, PPP from plat";
                                MySqlCommand cmdPlats = new MySqlCommand(queryPlats, maConnexion);
                                using (MySqlDataReader reader = cmdPlats.ExecuteReader())
                                {
                                    if (!reader.HasRows)
                                    {
                                        Console.WriteLine("Aucun plat n’est disponible pour l’instant.");
                                        break;
                                    }
                                    while (reader.Read())
                                    {
                                        string nomPlat = reader["nom_plat_"]?.ToString() ?? "Inconnu";
                                        string PPP1 = reader["PPP"]?.ToString() ?? "Inconnu";
                                        Console.WriteLine($"- {nomPlat} : Prix --> {PPP1}");
                                    }
                                }

                                Console.Write("Nom du plat à modifier : ");
                                string platModif = Console.ReadLine();

                                string checkQuery = "select count(*) from plat where nom_plat_ = @nom_plat";
                                MySqlCommand cmdCheck = new MySqlCommand(checkQuery, maConnexion);
                                cmdCheck.Parameters.AddWithValue("@nom_plat", platModif);
                                long platExiste = (long)cmdCheck.ExecuteScalar();

                                if (platExiste == 0)
                                {
                                    Console.WriteLine("Erreur : Ce plat n'existe pas.");
                                    break;
                                }

                                Console.Write("Nouveau nom : ");
                                string nouveauNom = Console.ReadLine();
                                Console.Write("Nouveau prix : ");
                                if (!decimal.TryParse(Console.ReadLine(), out decimal nouveauPrix))
                                {
                                    Console.WriteLine("Erreur : Le prix doit être un nombre valide (ex. 12.50).");
                                    break;
                                }

                                try
                                {
                                    string updateQuery = "update plat set nom_plat_ = @nouveau_nom, PPP = @nouveau_prix where nom_plat_ = @nom_plat";
                                    MySqlCommand cmdUpdate = new MySqlCommand(updateQuery, maConnexion);
                                    cmdUpdate.Parameters.AddWithValue("@nouveau_nom", nouveauNom);
                                    cmdUpdate.Parameters.AddWithValue("@nouveau_prix", nouveauPrix);
                                    cmdUpdate.Parameters.AddWithValue("@nom_plat", platModif);

                                    int rowsAffected = cmdUpdate.ExecuteNonQuery();
                                    if (rowsAffected > 0)
                                    {
                                        Console.WriteLine("Plat modifié avec succès !");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Erreur : La modification a échoué.");
                                    }
                                }
                                catch (MySqlException ex)
                                {
                                    Console.WriteLine($"Erreur lors de la modification : {ex.Message}");
                                }
                                break;

                            case "supprimer":
                                string requete3 = "select nom_plat_ from plat;";
                                MySqlCommand cmdDetails = new MySqlCommand(requete3, maConnexion);
                                cmdDetails.Parameters.AddWithValue("@id", maConnexion);

                                using (MySqlDataReader reader = cmdDetails.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        Console.WriteLine($"- {reader["nom_plat_"]}");
                                    }
                                }
                                Console.Write("Nom du plat à supprimer : ");
                                string platSupp = Console.ReadLine();


                                try
                                {
                                    /// Supprimer les références dans publie
                                    string deletePublieQuery = "delete from publie where nom_plat_ = @nom_plat";
                                    MySqlCommand cmdDeletePublie = new MySqlCommand(deletePublieQuery, maConnexion);
                                    cmdDeletePublie.Parameters.AddWithValue("@nom_plat", platSupp);
                                    int rowsPublie = cmdDeletePublie.ExecuteNonQuery();

                                    /// Supprimer les références dans contient
                                    string deleteContientQuery = "delete from contient where nom_plat_ = @nom_plat";
                                    MySqlCommand cmdDeleteContient = new MySqlCommand(deleteContientQuery, maConnexion);
                                    cmdDeleteContient.Parameters.AddWithValue("@nom_plat", platSupp);
                                    int rowsContient = cmdDeleteContient.ExecuteNonQuery();

                                    /// Supprimer les références dans est_composé_de
                                    string deleteEstComposeDeQuery = "delete from est_composé_de where nom_plat_ = @nom_plat";
                                    MySqlCommand cmdDeleteEstComposeDe = new MySqlCommand(deleteEstComposeDeQuery, maConnexion);
                                    cmdDeleteEstComposeDe.Parameters.AddWithValue("@nom_plat", platSupp);
                                    int rowsEstComposeDe = cmdDeleteEstComposeDe.ExecuteNonQuery();

                                    /// Supprimer le plat de plat
                                    string deletePlatQuery = "delete from plat where nom_plat_ = @nom_plat";
                                    MySqlCommand cmdDeletePlat = new MySqlCommand(deletePlatQuery, maConnexion);
                                    cmdDeletePlat.Parameters.AddWithValue("@nom_plat", platSupp);
                                    int rowsPlat = cmdDeletePlat.ExecuteNonQuery();

                                    if (rowsPlat > 0)
                                    {
                                        Console.WriteLine($"Plat '{platSupp}' supprimé avec succès ! ({rowsPublie} refs dans publie, {rowsContient} refs dans contient, {rowsEstComposeDe} refs dans est_composé_de supprimées)");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Plat non trouvé.");
                                    }
                                }
                                catch (MySqlException ex)
                                {
                                    Console.WriteLine($"Erreur lors de la suppression : {ex.Message}");
                                }
                                break;

                            case "voir":
                                Console.WriteLine("Liste des plats :");
                                string voirPlatsQuery = "select nom_plat_ from plat";
                                MySqlCommand cmdVoirPlats = new MySqlCommand(voirPlatsQuery, maConnexion);
                                using (MySqlDataReader reader = cmdVoirPlats.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        string nomPlat = reader["nom_plat_"] ?.ToString() ?? "NULL";
                                        Console.WriteLine($"- {nomPlat}");
                                    }
                                }
                                break;
                        }
                    }

                    

                }
                else if (role == "admin")
                {
                    Console.WriteLine("Que veux-tu faire ?");
                    Console.WriteLine("1. Gérer les clients");
                    Console.WriteLine("2. Gérer les cuisiniers");
                    Console.WriteLine("3. Module Statistiques");
                    string choixAdmin = Console.ReadLine();


                    if (choixAdmin == "1") ///gérér les cuisiniers 
                    {
                        Console.WriteLine("Comment veux-tu trier les clients ?");
                        Console.WriteLine("1. Par ordre alphabétique (pseudo)");
                        Console.WriteLine("2. Par station de métro");
                        Console.WriteLine("3. Par nombre de plats commandés");
                        Console.WriteLine("4. Combiner plusieurs critères (ex: '1,3' )");
                        string choixTri = Console.ReadLine();

                        string queryClients = "";
                        if (choixTri.Contains("1") && choixTri.Contains("2") && choixTri.Contains("3"))
                        {
                            queryClients = "select cl.pseudocl, cl.metrocl, count(co.nom_plat_) as plats_commandes from client cl left join livre l on cl.id_client = l.id_client left join contient co on l.id_commande = co.id_commande group by cl.id_client, cl.pseudocl, cl.metrocl order by cl.pseudocl asc, cl.metrocl asc, plats_commandes desc";
                        }
                        else if (choixTri.Contains("1") && choixTri.Contains("2"))
                        {
                            queryClients = "select cl.pseudocl, cl.metrocl from client cl order by cl.pseudocl asc, cl.metrocl asc";
                        }
                        else if (choixTri.Contains("1") && choixTri.Contains("3"))
                        {
                            queryClients = "select cl.pseudocl, count(co.nom_plat_) as plats_commandes from client cl left join livre l on cl.id_client = l.id_client left join contient co on l.id_commande = co.id_commande group by cl.id_client, cl.pseudocl order by cl.pseudocl asc, plats_commandes desc";
                        }
                        else if (choixTri.Contains("2") && choixTri.Contains("3"))
                        {
                            queryClients = "select cl.pseudocl, cl.metrocl, count(co.nom_plat_) as plats_commandes from client cl left join livre l on cl.id_client = l.id_client left join contient co on l.id_commande = co.id_commande group by cl.id_client, cl.pseudocl, cl.metrocl order by cl.metrocl asc, plats_commandes desc";
                        }
                        else if (choixTri == "1")
                        {
                            queryClients = "select cl.pseudocl, cl.metrocl from client cl order by cl.pseudocl asc";
                        }
                        else if (choixTri == "2")
                        {
                            queryClients = "select cl.pseudocl, cl.metrocl from client cl order by cl.metrocl asc";
                        }
                        else if (choixTri == "3")
                        {
                            queryClients = "select cl.pseudocl, count(co.nom_plat_) as plats_commandes from client cl left join livre l on cl.id_client = l.id_client left join contient co on l.id_commande = co.id_commande group by cl.id_client, cl.pseudocl order by plats_commandes desc";
                        }
                        else
                        {
                            Console.WriteLine("Choix invalide. Utilise 1, 2, 3 ou une combinaison (ex: '1,3').");
                            return;
                        }
                        MySqlCommand cmdClients = new MySqlCommand(queryClients, maConnexion);

                        Console.WriteLine("Liste des clients :");
                        using (MySqlDataReader reader = cmdClients.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                Console.WriteLine("Aucun client trouvé.");
                            }
                            while (reader.Read())
                            {
                                string pseudoCl = reader["pseudocl"]?.ToString() ?? "Inconnu";
                                string metroCl = reader["metrocl"]?.ToString() ?? "Non précisée";
                                string platsCommandes = reader.FieldCount > 2 ? (reader["plats_commandes"]?.ToString() ?? "0") : "N/A";

                                if (choixTri == "1")
                                    Console.WriteLine($"- {pseudoCl} (Station: {metroCl})");
                                else if (choixTri == "2")
                                    Console.WriteLine($"- {pseudoCl} (Station: {metroCl})");
                                else if (choixTri == "3")
                                    Console.WriteLine($"- {pseudoCl} (Plats commandés: {platsCommandes})");
                                else
                                    Console.WriteLine($"- {pseudoCl} (Station: {metroCl}, Plats commandés: {platsCommandes})");
                            }
                        }
                    }
                    else if (choixAdmin == "2")
                    {
                        Console.WriteLine("1. Ajouter un cuisinier");
                        Console.WriteLine("2. Modifier un cuisinier");
                        Console.WriteLine("3. Supprimer un cuisinier");
                        Console.WriteLine("4. Ajouter depuis un fichier");
                        Console.WriteLine("5. Voir les clients servis par un cuisinier");
                        string choixCuisinier = Console.ReadLine();

                        if (choixCuisinier == "1") /// ajouter cuisinier
                        {
                            Console.Write("Pseudo du cuisinier : ");
                            string pseudoCu = Console.ReadLine();
                            Console.Write("Mot de passe : ");
                            string motDePasse = Console.ReadLine();
                            Console.Write("Station de métro : ");
                            string metroCu = Console.ReadLine();
                            string idCuisinier = Guid.NewGuid().ToString();

                            string insertCuisinier = "insert into cuisinier (id_cuisinier, mot_de_passe, pseudocu, metrocui) values (@id, @mdp, @pseudo, @metro)";
                            MySqlCommand cmdInsert = new MySqlCommand(insertCuisinier, maConnexion);
                            cmdInsert.Parameters.AddWithValue("@id", idCuisinier);
                            cmdInsert.Parameters.AddWithValue("@mdp", motDePasse);
                            cmdInsert.Parameters.AddWithValue("@pseudo", pseudoCu);
                            cmdInsert.Parameters.AddWithValue("@metro", metroCu);
                            cmdInsert.ExecuteNonQuery();

                            Console.WriteLine($"Cuisinier '{pseudoCu}' ajouté avec succès !");
                        }
                        else if (choixCuisinier == "2") ///modifier un cuisinier
                        {
                            Console.Write("Pseudo du cuisinier à modifier : ");
                            string pseudoCu = Console.ReadLine();
                            Console.Write("Nouveau mot de passe (laisser vide pour ne pas changer) : ");
                            string motDePasse = Console.ReadLine();
                            Console.Write("Nouvelle station de métro (laisser vide pour ne pas changer) : ");
                            string metroCu = Console.ReadLine();

                            
                            if (string.IsNullOrEmpty(motDePasse) && string.IsNullOrEmpty(metroCu))
                            {
                                Console.WriteLine("Aucune modification demandée.");
                                return;
                            }

                            
                            List<string> updates = new List<string>();
                            MySqlCommand cmdUpdate = new MySqlCommand();
                            cmdUpdate.Connection = maConnexion;

                            if (!string.IsNullOrEmpty(motDePasse))
                            {
                                updates.Add("mot_de_passe = @mdp");
                                cmdUpdate.Parameters.AddWithValue("@mdp", motDePasse);
                            }

                            if (!string.IsNullOrEmpty(metroCu))
                            {
                                updates.Add("metrocui = @metro");
                                cmdUpdate.Parameters.AddWithValue("@metro", metroCu);
                            }

                            string updateCuisinier = $"update cuisinier set {string.Join(", ", updates)} where pseudocu = @pseudo";
                            cmdUpdate.CommandText = updateCuisinier;
                            cmdUpdate.Parameters.AddWithValue("@pseudo", pseudoCu);

                            try
                            {
                                int rowsAffected = cmdUpdate.ExecuteNonQuery();
                                if (rowsAffected > 0)
                                    Console.WriteLine($"Cuisinier '{pseudoCu}' modifié avec succès !");
                                else
                                    Console.WriteLine("Cuisinier non trouvé.");
                            }
                            catch (MySqlException ex)
                            {
                                Console.WriteLine($"Erreur lors de la modification : {ex.Message}");
                            }
                        }
                        else if (choixCuisinier == "3") /// supprimer un cuisinier
                        {
                            Console.WriteLine("Voici les cuisiniers disponibles :");
                            string queryCuisiniers = "select id_cuisinier, pseudocu from cuisinier";
                            MySqlCommand cmdCuisiniers = new MySqlCommand(queryCuisiniers, maConnexion);
                            using (MySqlDataReader reader = cmdCuisiniers.ExecuteReader())
                            {
                                if (!reader.HasRows)
                                {
                                    Console.WriteLine("Aucun cuisinier n’est disponible.");

                                }
                                while (reader.Read())
                                {
                                    string idCuisinier = reader["id_cuisinier"]?.ToString() ?? "Inconnu";
                                    string pseudo = reader["pseudocu"]?.ToString() ?? "Inconnu";
                                    Console.WriteLine($"- ID : {idCuisinier}, Pseudo : {pseudo}");
                                }
                            }

                            Console.Write("ID du cuisinier à supprimer : ");
                            string idCuisinierSuppr = Console.ReadLine();

                            
                            string checkQuery = "select count(*) from cuisinier where id_cuisinier = @id_cuisinier";
                            MySqlCommand cmdCheck = new MySqlCommand(checkQuery, maConnexion);
                            cmdCheck.Parameters.AddWithValue("@id_cuisinier", idCuisinierSuppr);
                            long cuisinierExiste = (long)cmdCheck.ExecuteScalar();

                            if (cuisinierExiste == 0)
                            {
                                Console.WriteLine("Erreur : Ce cuisinier n'existe pas.");

                            }

                            try
                            {
                                /// Supprimer les références dans publie
                                string deletePublieQuery = "delete from publie where id_cuisinier = @id_cuisinier";
                                MySqlCommand cmdDeletePublie = new MySqlCommand(deletePublieQuery, maConnexion);
                                cmdDeletePublie.Parameters.AddWithValue("@id_cuisinier", idCuisinierSuppr);
                                cmdDeletePublie.ExecuteNonQuery();

                                /// Supprimer les références dans livre
                                string deleteLivreQuery = "delete from livre where id_cuisinier = @id_cuisinier";
                                MySqlCommand cmdDeleteLivre = new MySqlCommand(deleteLivreQuery, maConnexion);
                                cmdDeleteLivre.Parameters.AddWithValue("@id_cuisinier", idCuisinierSuppr);
                                cmdDeleteLivre.ExecuteNonQuery();

                                /// Supprimer les références dans avis_entreprise
                                string deleteAvisEntrepriseQuery = "delete from avis_entreprise where id_cuisinier = @id_cuisinier";
                                MySqlCommand cmdDeleteAvisEntreprise = new MySqlCommand(deleteAvisEntrepriseQuery, maConnexion);
                                cmdDeleteAvisEntreprise.Parameters.AddWithValue("@id_cuisinier", idCuisinierSuppr);
                                cmdDeleteAvisEntreprise.ExecuteNonQuery();

                                /// Supprimer les références dans est_cuisinier
                                string deleteEstCuisinierQuery = "delete from est_cuisinier where id_cuisinier = @id_cuisinier";
                                MySqlCommand cmdDeleteEstCuisinier = new MySqlCommand(deleteEstCuisinierQuery, maConnexion);
                                cmdDeleteEstCuisinier.Parameters.AddWithValue("@id_cuisinier", idCuisinierSuppr);
                                cmdDeleteEstCuisinier.ExecuteNonQuery();

                                /// Supprimer les références dans avis_client
                                string deleteAvisClientQuery = "delete from avis_client where id_cuisinier = @id_cuisinier";
                                MySqlCommand cmdDeleteAvisClient = new MySqlCommand(deleteAvisClientQuery, maConnexion);
                                cmdDeleteAvisClient.Parameters.AddWithValue("@id_cuisinier", idCuisinierSuppr);
                                cmdDeleteAvisClient.ExecuteNonQuery();

                                ///Supprimer le cuisinier
                                string deleteCuisinierQuery = "delete from cuisinier where id_cuisinier = @id_cuisinier";
                                MySqlCommand cmdDeleteCuisinier = new MySqlCommand(deleteCuisinierQuery, maConnexion);
                                cmdDeleteCuisinier.Parameters.AddWithValue("@id_cuisinier", idCuisinierSuppr);
                                int rowsAffected = cmdDeleteCuisinier.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    Console.WriteLine("Cuisinier supprimé avec succès !");
                                }
                                else
                                {
                                    Console.WriteLine("Erreur : La suppression du cuisinier a échoué.");
                                }
                            }
                            catch (MySqlException ex)
                            {
                                Console.WriteLine($"Erreur lors de la suppression : {ex.Message}");
                            }
                        }

                        else if (choixCuisinier == "5")
                        {
                            Console.Write("Pseudo du cuisinier : ");
                            string pseudoCu = Console.ReadLine();
                            Console.Write("Filtrer par date ? (oui/non) : ");
                            string filtreDate = Console.ReadLine()?.ToLower();
                            string dateDebut = null, dateFin = null;

                            if (filtreDate == "oui")
                            {
                                Console.Write("Date de début (ex. 2025-01-01) : ");
                                dateDebut = Console.ReadLine();
                                Console.Write("Date de fin (ex. 2025-12-31) : ");
                                dateFin = Console.ReadLine();
                            }

                            string queryClientsServis = "select cl.pseudocl, cl.metrocl, l.date_livraison from cuisinier cu join livre l on cu.id_cuisinier = l.id_cuisinier join client cl on l.id_client = cl.id_client where cu.pseudocu = @pseudo";
                            if (filtreDate == "oui")
                                queryClientsServis += " and l.date_livraison between @debut and @fin";

                            MySqlCommand cmdClientsServis = new MySqlCommand(queryClientsServis, maConnexion);
                            cmdClientsServis.Parameters.AddWithValue("@pseudo", pseudoCu);
                            if (filtreDate == "oui")
                            {
                                cmdClientsServis.Parameters.AddWithValue("@debut", dateDebut);
                                cmdClientsServis.Parameters.AddWithValue("@fin", dateFin);
                            }

                            Console.WriteLine($"Clients servis par '{pseudoCu}' :");
                            using (MySqlDataReader reader = cmdClientsServis.ExecuteReader())
                            {
                                if (!reader.HasRows)
                                {
                                    Console.WriteLine("Aucun client servi trouvé.");
                                }
                                while (reader.Read())
                                {
                                    string pseudoCl = reader["pseudocl"]?.ToString() ?? "Inconnu";
                                    string metroCl = reader["metrocl"]?.ToString() ?? "Non précisée";
                                    string dateLivraison = reader["date_livraison"]?.ToString() ?? "Inconnue";
                                    Console.WriteLine($"- {pseudoCl} (Station: {metroCl}, Livré le: {dateLivraison})");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Choix invalide.");
                        }
                        
                    }


                    else if (choixAdmin == "3")
                    {
                        Statistiques.ModuleStatistiques(maConnexion); // Correction : Utiliser Statistiques.ModuleStatistiques
                    }
                }
               

            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
            finally
            {
                if (maConnexion != null && maConnexion.State == System.Data.ConnectionState.Open)
                    maConnexion.Close();
            }
        }

        static void GestionExportImport()
        {
            string connectionString = "server=localhost;port=3306;database=psi_niauma;uid=root;password=Beauville68!;";
            DataExporter exporter = new DataExporter(connectionString);

            /// Créer le dossier exports s'il n'existe pas
            string exportDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "exports");
            Directory.CreateDirectory(exportDir);

            Console.WriteLine("\n=== Export des données ===");
            Console.WriteLine("1. Exporter en XML");
            Console.WriteLine("2. Exporter en JSON");
            Console.Write("\nVotre choix : ");

            if (int.TryParse(Console.ReadLine(), out int choix))
            {
                try
                {
                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    string filePath;

                    switch (choix)
                    {
                        case 1:
                            filePath = Path.Combine(exportDir, $"export_{timestamp}.xml");
                            exporter.ExportToXml(filePath);
                            Console.WriteLine($"Export XML réussi ! Fichier créé : {filePath}");
                            break;

                        case 2:
                            filePath = Path.Combine(exportDir, $"export_{timestamp}.json");
                            exporter.ExportToJson(filePath);
                            Console.WriteLine($"Export JSON réussi ! Fichier créé : {filePath}");
                            break;

                        default:
                            Console.WriteLine("Choix invalide.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur : {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Veuillez entrer un nombre valide.");
            }
        }

        static Graphe<int> ImporterGraphe(string chemin)
        {
            if (!File.Exists(chemin))                                        ///virifie si il peut acceder au fichier
            {
                Console.WriteLine($"ne trouve pas le fichier {chemin}");
                return null;
            }

            string[] lignes = File.ReadAllLines(chemin);
            if (lignes.Length == 0)                                   ///regarde toutes les lignes et test si elles sont vides
            {
                Console.WriteLine("fichir est vide");
                return null;
            }
            
            int nbnoeuds;
            
            if (!int.TryParse(lignes[1].Split(';')[0], out nbnoeuds) || nbnoeuds <= 0)
            {
                Console.WriteLine("nb de noeud pas valide");
                return null;
            }
            nbnoeuds = lignes.Length-1;
            Graphe<int> graphe = new Graphe<int>(nbnoeuds);
            graphe.ChargerListeDeNoeuds(chemin);
            return graphe;
        }
    }
}


