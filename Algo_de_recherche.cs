using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;

namespace Projet_PSI
{
    internal class Algo_de_recherche
    {
        private int[,] matrice;
        private int depart;
        private int arrivee;
        private int tailleMat;
        private List<MetroStation> stations;

        public Algo_de_recherche(int[,] matrice, int depart, int arrivee, int tailleMat)
        {
            this.matrice = matrice;
            this.depart = depart - 1;
            this.arrivee = arrivee - 1;
            this.tailleMat = tailleMat;
            this.stations = livin_main.stations;

            // Vérification de la matrice
            Console.WriteLine("\nVérification de la matrice d'adjacence :");
            Console.WriteLine($"Taille de la matrice : {tailleMat}x{tailleMat}");
            Console.WriteLine($"Indices de départ et d'arrivée dans la matrice : {this.depart}, {this.arrivee}");

            // Vérifier si la matrice est vide
            bool matriceVide = true;
            for (int i = 0; i < tailleMat; i++)
            {
                for (int j = 0; j < tailleMat; j++)
                {
                    if (matrice[i, j] > 0)
                    {
                        matriceVide = false;
                        break;
                    }
                }
                if (!matriceVide) break;
            }

            if (matriceVide)
            {
                Console.WriteLine("ATTENTION : La matrice d'adjacence est vide !");
                Console.WriteLine("Vérifiez que la méthode ImplementerMatrice() a été appelée après le chargement des données.");
            }
            else
            {
                // Vérifier les connexions directes depuis la station de départ
                Console.WriteLine("\nConnexions directes depuis la station de départ :");
                for (int j = 0; j < tailleMat; j++)
                {
                    if (matrice[this.depart, j] > 0)
                    {
                        Console.WriteLine($"Station {this.depart + 1} -> Station {j + 1} (temps: {matrice[this.depart, j]} min)");
                    }
                }
            }
        }

        public void Dijkstra()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            bool[] visite = new bool[this.matrice.GetLength(0)];
            int[] dist = new int[this.matrice.GetLength(0)];
            int[] pred = new int[this.matrice.GetLength(0)];

            Console.WriteLine($"\nDébut de l'algorithme Dijkstra - Départ: {depart + 1}, Arrivée: {arrivee + 1}");

            for (int i = 0; i < tailleMat; i++)
            {
                pred[i] = -1;
                dist[i] = 999999999;
            }
            dist[depart] = 0;

            for (int i = 0; i < tailleMat - 1; i++)
            {
                int u = PPDistance(visite, dist);
                if (u == -1)
                {
                    Console.WriteLine("Aucun sommet non visité trouvé !");
                    break;
                }
                visite[u] = true;

                for (int j = 0; j < tailleMat; j++)
                {
                    if (!visite[j] && matrice[u, j] > 0 && dist[u] != 999999999 && dist[u] + matrice[u, j] < dist[j])
                    {
                        dist[j] = dist[u] + matrice[u, j];
                        pred[j] = u;
                        Console.WriteLine($"Mise à jour du prédécesseur de {j + 1} : {u + 1} (distance: {dist[j]})");
                    }
                }
            }

            // Vérification des distances trouvées
            Console.WriteLine("\nDistances trouvées :");
            for (int i = 0; i < tailleMat; i++)
            {
                if (dist[i] != 999999999)
                {
                    Console.WriteLine($"Station {i + 1} : distance = {dist[i]}");
                }
            }

            timer.Stop();
            ResultatRecherche(dist);
            Console.WriteLine();
            Console.WriteLine("Le temps mis par l'algo pour trouver le chemin est de : " + timer.ElapsedMilliseconds + " ms ");
            Console.WriteLine();

            AfficherChemin(pred);
        }

        public int PPDistance(bool[] visite, int[] dist)                          // méthode pour avoir le point le plus proche 
        {
            int distMin = 999999999;
            int index = -1;

            for (int i = 0; i < tailleMat - 1; i++)
            {
                if (!visite[i] && dist[i] <= distMin)
                {
                    distMin = dist[i];
                    index = i;
                }
            }
            return index;
        }

        public void BellemanFord()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            int[] dist = new int[this.matrice.GetLength(0)];
            int[] pred = new int[this.matrice.GetLength(0)];


            for (int i = 0; i < tailleMat; i++)
            {
                pred[i] = -1;
                dist[i] = 999999999;
            }
            dist[depart] = 0;

            for (int i = 0; i < tailleMat - 1; i++)     //comme Dijkstra, tailleMat-1 car on a besoin de n-1 étapes
            {
                for (int j = 0; j < tailleMat; j++)
                {
                    for (int k = 0; k < tailleMat; k++)
                    {
                        if (matrice[j, k] != 0 && dist[j] != 999999999 && dist[j] + matrice[j, k] < dist[k])
                        {
                            dist[k] = dist[j] + matrice[j, k];
                            pred[k] = j;
                        }
                    }
                }
            }
            for (int i = 0; i < tailleMat; i++)                          // detecte si il y a un cycle de point négatif
            {
                for (int j = 0; j < tailleMat; j++)
                {
                    if (matrice[i, j] != 0 && dist[i] != 999999999 && dist[i] + matrice[i, j] < dist[j])
                    {
                        return;
                    }
                }
            }
            timer.Stop();
            ResultatRecherche(dist);
            Console.WriteLine();
            Console.WriteLine("Le temps mis par l'algo pour trouver le chemin est de : " + timer.ElapsedMilliseconds + " ms ");
            Console.WriteLine();
            AfficherChemin(pred);

        }


        public void FloydWarshall()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            int n = tailleMat;
            int[,] dist = new int[n, n];
            int[,] pred = new int[n, n];


            for (int i = 0; i < n; i++)                         // initialisation dist et pred
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j)
                    {
                        dist[i, j] = 0;
                        pred[i, j] = -1;
                    }
                    else if (matrice[i, j] != 0)
                    {
                        dist[i, j] = matrice[i, j];
                        pred[i, j] = i;
                    }
                    else
                    {
                        dist[i, j] = int.MaxValue / 2;             // pas de depassement
                        pred[i, j] = -1;
                    }
                }
            }


            for (int k = 0; k < n; k++)                                //floyd warshall
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (dist[i, k] + dist[k, j] < dist[i, j])
                        {
                            dist[i, j] = dist[i, k] + dist[k, j];
                            pred[i, j] = pred[k, j];
                        }
                    }
                }
            }


            int[] distFinale = new int[n];
            int[] predFinal = new int[n];
            for (int i = 0; i < n; i++)
            {
                distFinale[i] = 999999999;
                predFinal[i] = -1;
            }
            for (int i = 0; i < n; i++)
            {
                distFinale[i] = dist[depart, i];
                predFinal[i] = pred[depart, i];
            }
            timer.Stop();

            ResultatRecherche(distFinale);
            Console.WriteLine();
            Console.WriteLine("Le temps mis par l'algo pour trouver le chemin est de : " + timer.ElapsedMilliseconds + " ms ");
            Console.WriteLine();
            AfficherChemin(predFinal);
        }




        public void ResultatRecherche(int[] dist)
        {
            Console.WriteLine("\n════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine("                          RÉSULTAT DE LA RECHERCHE");
            Console.WriteLine("════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine($"Temps de trajet : {dist[arrivee]} minutes");
            Console.WriteLine($"Station de départ : {stations.FirstOrDefault(s => s.IdStation == depart + 1)?.LibelleStation}");
            Console.WriteLine($"Station d'arrivée : {stations.FirstOrDefault(s => s.IdStation == arrivee + 1)?.LibelleStation}");
            Console.WriteLine("════════════════════════════════════════════════════════════════════════════════");
        }

        public void AfficherChemin(int[] pred)
        {
            if (pred[arrivee] == -1)
            {
                Console.WriteLine("\n❌ ERREUR : Aucun chemin trouvé vers la station d'arrivée !");
                return;
            }

            Stack<int> chemin = new Stack<int>();
            int actuel = arrivee;

            // Construire le chemin
            while (actuel != -1)
            {
                chemin.Push(actuel);
                actuel = pred[actuel];
            }

            // Afficher le chemin de manière plus esthétique
            Console.WriteLine("\n════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine("                          CHEMIN LE PLUS COURT");
            Console.WriteLine("════════════════════════════════════════════════════════════════════════════════");

            int etape = 1;
            List<MetroStation> stationsChemin = new List<MetroStation>();
            while (chemin.Count > 0)
            {
                int stationIndex = chemin.Pop();
                var station = stations.FirstOrDefault(s => s.IdStation == stationIndex + 1);
                if (station != null)
                {
                    stationsChemin.Add(station);
                    Console.Write($"Étape {etape++} : {station.LibelleStation}");
                    if (chemin.Count > 0)
                    {
                        Console.Write("\n↓\n");
                    }
                }
            }
            Console.WriteLine("\n════════════════════════════════════════════════════════════════════════════════");

            // Générer la carte du trajet
            GenererCarteTrajet(stationsChemin);
        }
        private void GenererCarteTrajet(List<MetroStation> stationsChemin)
        {
            if (stationsChemin == null || stationsChemin.Count == 0)
            {
                Console.WriteLine("Aucune station dans le chemin pour générer la carte.");
                return;
            }

            int width = 1200;
            int height = 800;

            Dictionary<string, Color> couleursLignes = new Dictionary<string, Color>
            {
                {"1", Color.FromArgb(255, 0, 0)}, {"2", Color.FromArgb(0, 0, 255)},
                {"3", Color.FromArgb(128, 0, 128)}, {"4", Color.FromArgb(255, 165, 0)},
                {"5", Color.FromArgb(255, 0, 255)}, {"6", Color.FromArgb(0, 128, 0)},
                {"7", Color.FromArgb(255, 0, 0)}, {"8", Color.FromArgb(128, 0, 0)},
                {"9", Color.FromArgb(255, 192, 203)}, {"10", Color.FromArgb(0, 128, 128)},
                {"11", Color.FromArgb(255, 255, 0)}, {"12", Color.FromArgb(0, 128, 0)},
                {"13", Color.FromArgb(128, 128, 128)}, {"14", Color.FromArgb(0, 0, 128)}
            };

            using (Bitmap bitmap = new Bitmap(width, height))
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);
                g.SmoothingMode = SmoothingMode.AntiAlias;

                double minLat = stations.Min(s => s.Latitude);
                double maxLat = stations.Max(s => s.Latitude);
                double minLon = stations.Min(s => s.Longitude);
                double maxLon = stations.Max(s => s.Longitude);

                double latMargin = (maxLat - minLat) * 0.1;
                double lonMargin = (maxLon - minLon) * 0.1;
                minLat -= latMargin; maxLat += latMargin;
                minLon -= lonMargin; maxLon += lonMargin;

                int X(double lon) => (int)((lon - minLon) / (maxLon - minLon) * (width - 100) + 50);
                int Y(double lat) => (int)((maxLat - lat) / (maxLat - minLat) * (height - 100) + 50);

                // Dessiner le réseau de fond avec une couleur plus foncée
                using (Brush brush = new SolidBrush(Color.Gray))
                using (Pen pen = new Pen(Color.Gray, 1))
                {
                    for (int i = 0; i < tailleMat; i++)
                        for (int j = i + 1; j < tailleMat; j++)
                            if (matrice[i, j] > 0 && matrice[i, j] < int.MaxValue / 2)
                            {
                                var s1 = stations.FirstOrDefault(s => s.IdStation == i + 1);
                                var s2 = stations.FirstOrDefault(s => s.IdStation == j + 1);
                                if (s1 != null && s2 != null)
                                    g.DrawLine(pen, X(s1.Longitude), Y(s1.Latitude), X(s2.Longitude), Y(s2.Latitude));
                            }
                    foreach (var s in stations)
                        g.FillEllipse(brush, X(s.Longitude) - 3, Y(s.Latitude) - 3, 6, 6);
                }

                // Normalisation de LibelleLine pour extraire le numéro de ligne
                string GetLineNumber(string libelleLine)
                {
                    if (string.IsNullOrEmpty(libelleLine)) return "1";
                    var parts = libelleLine.Split(new char[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var part in parts)
                    {
                        if (int.TryParse(part, out _))
                            return part;
                    }
                    return "1";
                }

                // Dessiner le trajet
                for (int i = 0; i < stationsChemin.Count - 1; i++)
                {
                    MetroStation current = stationsChemin[i];
                    MetroStation next = stationsChemin[i + 1];
                    string lineNumber = GetLineNumber(current.LibelleLine);
                    Color couleur = couleursLignes.ContainsKey(lineNumber) ? couleursLignes[lineNumber] : Color.Red;
                    using (Pen pen = new Pen(couleur, 3))
                        g.DrawLine(pen, X(current.Longitude), Y(current.Latitude), X(next.Longitude), Y(next.Latitude));
                }

                // Dessiner les stations du trajet avec un décalage pour éviter le chevauchement
                using (Font font = new Font("Arial", 8))
                {
                    float labelOffsetY = 0;
                    foreach (var s in stationsChemin)
                    {
                        int x = X(s.Longitude);
                        int y = Y(s.Latitude);
                        string lineNumber = GetLineNumber(s.LibelleLine);
                        Color couleur = couleursLignes.ContainsKey(lineNumber) ? couleursLignes[lineNumber] : Color.Red;
                        using (Brush brush = new SolidBrush(couleur))
                        {
                            g.FillEllipse(brush, x - 5, y - 5, 10, 10);
                            g.DrawString(s.LibelleStation, font, brush, x + 10, y - 10 + labelOffsetY);
                        }
                        labelOffsetY += 5;
                    }
                }

                // Légende
                using (Font font = new Font("Arial", 10))
                {
                    int yLegende = height - (couleursLignes.Count * 20 + 50);
                    g.DrawString("Légende des lignes :", font, Brushes.Black, 20, yLegende);
                    yLegende += 30;
                    foreach (var ligne in couleursLignes)
                    {
                        using (Brush brush = new SolidBrush(ligne.Value))
                        {
                            g.FillEllipse(brush, 20, yLegende, 10, 10);
                            g.DrawString($"Ligne {ligne.Key}", font, Brushes.Black, 35, yLegende);
                        }
                        yLegende += 20;
                    }
                }

                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string filename = $"Trajet_{timestamp}.png";
                bitmap.Save(filename, ImageFormat.Png);
                Console.WriteLine($"\nCarte du trajet générée : {filename}");

                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = filename,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nImpossible d'ouvrir l'image : {ex.Message}");
                }
            }
        }
    }
}