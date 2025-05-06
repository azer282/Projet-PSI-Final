using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;

public class MetroStation
{
    public int IdStation { get; set; }
    public string LibelleLine { get; set; }
    public string LibelleStation { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public string CommuneNom { get; set; }
    public string CommuneCode { get; set; }

    public MetroStation(int idStation, string libelleLine, string libelleStation, 
                       double longitude, double latitude, string communeNom, string communeCode)
    {
        IdStation = idStation;
        LibelleLine = libelleLine;
        LibelleStation = libelleStation;
        Longitude = longitude;
        Latitude = latitude;
        CommuneNom = communeNom;
        CommuneCode = communeCode;
    }

    public static List<MetroStation> LoadStations(string filename)
    {
        var stations = new List<MetroStation>();
        var lines = File.ReadAllLines(filename);

        // Skip header
        foreach (var line in lines.Skip(1))
        {
            var parts = line.Split(';');
            if (parts.Length >= 7)
            {
                if (double.TryParse(parts[3], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double longitude) &&
                    double.TryParse(parts[4], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double latitude))
                {
                    var station = new MetroStation(
                        idStation: int.Parse(parts[0]),
                        libelleLine: parts[1],
                        libelleStation: parts[2],
                        longitude: longitude,
                        latitude: latitude,
                        communeNom: parts[5],
                        communeCode: parts[6]
                    );
                    stations.Add(station);
                }
                else
                {
                    Console.WriteLine($"Invalid data in line: {line}");
                }
            }
        }
        return stations;
    }

    public void DisplayStation()
    {
        Console.WriteLine($"Station: {LibelleStation}");
        Console.WriteLine($"Ligne: {LibelleLine}");
        Console.WriteLine($"Coordonnées: ({Longitude}, {Latitude})");
        Console.WriteLine($"Commune: {CommuneNom}");
        Console.WriteLine(new string('-', 50));
    }

    public static void DisplayStationsByLine(List<MetroStation> stations)
    {
        var stationsByLine = stations.GroupBy(s => s.LibelleLine)
                                   .OrderBy(g => int.Parse(g.Key));

        foreach (var lineGroup in stationsByLine)
        {
            Console.WriteLine($"\n=== Ligne {lineGroup.Key} ===");
            foreach (var station in lineGroup.OrderBy(s => s.IdStation))
            {
                Console.WriteLine($"- {station.LibelleStation} ({station.Longitude}, {station.Latitude})");
            }
        }
    }

    public static void SearchStation(List<MetroStation> stations, string searchTerm)
    {
        var results = stations.Where(s => 
            s.LibelleStation.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0 ||
            s.LibelleLine.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0 ||
            s.CommuneNom.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0
        ).ToList();

        if (results.Any())
        {
            Console.WriteLine($"\nRésultats de la recherche pour '{searchTerm}':");
            foreach (var station in results)
            {
                station.DisplayStation();
            }
        }
        else
        {
            Console.WriteLine($"\nAucune station trouvée pour '{searchTerm}'");
        }
    }

    public static void CreateMetroMap(List<MetroStation> stations)
    {
        // Dimensions de l'image
        int width = 1200;
        int height = 800;
        
        // Création de l'image
        using (Bitmap bitmap = new Bitmap(width, height))
        {
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                // Fond blanc
                g.Clear(Color.White);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Trouver les coordonnées min/max pour la mise à l'échelle
                double minLong = stations.Min(s => s.Longitude);
                double maxLong = stations.Max(s => s.Longitude);
                double minLat = stations.Min(s => s.Latitude);
                double maxLat = stations.Max(s => s.Latitude);

                // Fonction pour convertir les coordonnées géographiques en coordonnées d'écran
                PointF ConvertCoordinates(double longitude, double latitude)
                {
                    float x = (float)((longitude - minLong) / (maxLong - minLong) * (width - 100) + 50);
                    float y = (float)((maxLat - latitude) / (maxLat - minLat) * (height - 100) + 50);
                    return new PointF(x, y);
                }

                // Couleurs pour les différentes lignes
                Dictionary<string, Color> lineColors = new Dictionary<string, Color>
                {
                    {"1", Color.FromArgb(255, 0, 0)},      // Rouge
                    {"2", Color.FromArgb(0, 0, 255)},      // Bleu
                    {"3", Color.FromArgb(0, 128, 0)},      // Vert
                    {"4", Color.FromArgb(255, 255, 0)},    // Jaune
                    {"5", Color.FromArgb(128, 0, 128)},    // Violet
                    {"6", Color.FromArgb(255, 165, 0)},    // Orange
                    {"7", Color.FromArgb(255, 192, 203)},  // Rose
                    {"8", Color.FromArgb(165, 42, 42)},    // Marron
                    {"9", Color.FromArgb(128, 128, 128)},  // Gris
                    {"10", Color.FromArgb(0, 255, 255)},   // Cyan
                    {"11", Color.FromArgb(255, 0, 255)},   // Magenta
                    {"12", Color.FromArgb(0, 255, 0)},     // Vert clair
                    {"13", Color.FromArgb(255, 255, 255)}, // Blanc
                    {"14", Color.FromArgb(0, 0, 0)}        // Noir
                };

                // Grouper les stations par ligne
                var stationsByLine = stations.GroupBy(s => s.LibelleLine);

                // Dessiner les lignes et les stations
                foreach (var lineGroup in stationsByLine)
                {
                    string lineNumber = lineGroup.Key;
                    Color lineColor = lineColors.ContainsKey(lineNumber) ? lineColors[lineNumber] : Color.Gray;

                    // Dessiner les lignes entre les stations
                    var points = lineGroup.Select(s => ConvertCoordinates(s.Longitude, s.Latitude)).ToArray();
                    if (points.Length > 1)
                    {
                        using (Pen pen = new Pen(lineColor, 3))
                        {
                            g.DrawLines(pen, points);
                        }
                    }

                    // Dessiner les stations
                    foreach (var station in lineGroup)
                    {
                        PointF point = ConvertCoordinates(station.Longitude, station.Latitude);
                        
                        // Cercle pour la station
                        using (Brush brush = new SolidBrush(lineColor))
                        {
                            g.FillEllipse(brush, point.X - 5, point.Y - 5, 10, 10);
                        }

                        // Nom de la station
                        using (Font font = new Font("Arial", 8))
                        {
                            g.DrawString(station.LibelleStation, font, Brushes.Black, point.X + 10, point.Y - 10);
                        }
                    }
                }

                // Ajouter une légende
                int legendX = width - 200;
                int legendY = 50;
                using (Font font = new Font("Arial", 10))
                {
                    foreach (var lineColor in lineColors)
                    {
                        using (Brush brush = new SolidBrush(lineColor.Value))
                        {
                            g.FillRectangle(brush, legendX, legendY, 20, 20);
                            g.DrawString($"Ligne {lineColor.Key}", font, Brushes.Black, legendX + 30, legendY + 2);
                            legendY += 25;
                        }
                    }
                }
            }

            // Sauvegarder l'image
            string outputPath = "MetroMap.png";
            bitmap.Save(outputPath, ImageFormat.Png);
            Console.WriteLine($"Carte du métro sauvegardée dans {outputPath}");
        }
    }
} 