using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Garage
{
    // Ett garage.
    internal class Garage
    {
        private readonly List<Vehicle> _vehicles = [];

        private static readonly StringComparer _comparer =
            StringComparer.Create(CultureInfo.CurrentUICulture, CompareOptions.NumericOrdering);

        // Namnet på garaget.
        public string Name
        {
            get; init
            {
                field = value.Length > 0 ? value :
                    throw new ArgumentException("Namnet får inte vara tomt.");
            }
        } = "Garage 1";

        // Max antal parkeringsplatser i ett garage.
        public static int MaxCapacity => 100000;

        // Antal parkeringsplatser i garaget.
        public int Capacity
        {
            get; init
            {
                field = value >= 1 && value <= MaxCapacity ? value :
                    throw new ArgumentException($"Kapacitet får inte vara mindre än 1 eller större än {MaxCapacity}.");
            }
        }

        // Antal lediga parkeringsplatser i garaget.
        public int FreeCapacity => Capacity - _vehicles.Count;

        // Antal parkerade fordon i garaget.
        public int Count => _vehicles.Count;

        // Fordonstyper som är tillåtna i garaget.
        public List<VehicleType> AllowedVehicles { get; init; } = [.. Enum.GetValues<VehicleType>()];

        // Skapa ett garage med en viss kapacitet.
        public Garage(int capacity)
        {
            if (capacity < 1 || capacity > MaxCapacity)
            {
                throw new ArgumentException($"Antalet parkeringsplatser måste vara mellan 1 och {MaxCapacity}.");
            }
            Capacity = capacity;
        }

        // Lista samtliga parkerade fordon i garaget.
        public void ListVehicles()
        {
            if (Count == 0)
            {
                Console.WriteLine("Det finns inga fordon i garaget.");
                return;
            }

            Console.WriteLine($"Det finns {Count} fordon parkerade i garaget.");
            ListVehicles(_vehicles);
        }

        // Lista fordon.
        public static void ListVehicles(List<Vehicle> vehicles)
        {
            Console.WriteLine("Vill du lista fordonen med (1) kompakt format, eller (2) långt format?");
            bool verbose;
            while (true)
            {
                Console.Write("Ange 1 för kompakt eller 2 för långt (1): ");
                string s = Console.ReadLine()?.Trim() ?? throw new IOException();
                if (s == "" || s == "1")
                {
                    verbose = false;
                    break;
                }
                else if (s == "2")
                {
                    verbose = true;
                    break;
                }
                else
                {
                    Console.WriteLine("Ogiltigt värde. Ange 1 för kompakt format, eller 2 för långt format.");
                }
            }
            if (verbose)
            {
                ListVehiclesVerbose(vehicles);
            }
            else
            {
                ListVehiclesCompact(vehicles);
            }
        }

        // Lista fordon med kompakt format.
        private static void ListVehiclesCompact(List<Vehicle> vehicles)
        {
            if (vehicles.Count == 0)
            {
                Console.WriteLine("Det finns inga fordon att lista.");
                return;
            }

            for (int i = 0; i < vehicles.Count; i++)
            {
                WriteStringInColour(vehicles[i].ToString() + "\n");
            }
        }

        // Lista fordon med långt format.
        private static void ListVehiclesVerbose(List<Vehicle> vehicles)
        {
            if (vehicles.Count == 0)
            {
                Console.WriteLine("Det finns inga fordon att lista.");
                return;
            }

            int pagesize = 5;
            Console.Write($"Ange hur många fordon du vill visa åt gången ({pagesize}): ");
            string s = Console.ReadLine()?.Trim() ?? throw new IOException();
            if (int.TryParse(s, out int n))
            {
                if (n > 0)
                {
                    pagesize = n;
                }
                else
                {
                    Console.WriteLine("Värdet 0 angivet. Avbryter.");
                    return;
                }
            }
            else
            {
                Console.WriteLine("Ogiltigt värde angivet. Använder det föreslagna värdet: 5");
            }

            ListVehiclesInPages(vehicles, pagesize);
        }

        // Lista fordon några åt gången med långt format.
        private static void ListVehiclesInPages(List<Vehicle> vehicles, int pagesize)
        {
            for (int i = 0, j = 0; i < vehicles.Count; i++)
            {
                Console.WriteLine();
                WriteStringInColour(vehicles[i].ToVerboseString() + "\n");
                if (i == vehicles.Count - 1)
                {
                    break;
                }
                if (j++ == pagesize - 1)
                {
                    Console.WriteLine();
                    Console.WriteLine($"{i + 1} av {vehicles.Count} fordon har visats. Tryck Enter för att fortsätta.");
                    Console.WriteLine("Du kan ange en siffra för att ändra hur många som visas åt gången. Ange 0 för att avbryta.");
                    string s = Console.ReadLine()?.Trim() ?? throw new IOException();
                    if (int.TryParse(s, out int n))
                    {
                        if (n == 0)
                        {
                            Console.WriteLine("Avbryter.");
                            return;
                        }
                        else if (n > 0)
                        {
                            pagesize = n;
                        }
                    }
                    j = 0;
                }
            }

            Console.WriteLine($"\nAlla {vehicles.Count} fordon har visats.");
        }

        // Skapar en sträng innehållande fordonstyper separerade med kommatecken.
        public static string VehicleTypesToString(List<VehicleType> types)
        {
            string s = "";
            for (int i = 0; i < types.Count; i++)
            {
                if (i > 0)
                {
                    s += ", ";
                }
                s += $"{Vehicle.GetVehicleNaturalName(types[i])}";
            }
            return s;
        }

        // Lista fordonstyper och hur många fordon av varje typ som står i garaget.
        // Implementationen förutsätter att VehicleType har värdena 0, 1, 2, ... utan luckor.
        public void ListVehicleTypes()
        {
            if (Count == 0)
            {
                Console.WriteLine("Det finns inga fordon i garaget.");
                return;
            }

            Console.WriteLine("Följande fordonstyper finns i garaget. Siffrorna anger antal fordon av varje typ.\n");

            // Detta skulle ha varit en array om det hade varit tillåtet.
            List<VehicleType> types = [.. Enum.GetValues<VehicleType>()];
            List<int> numberOfVehicles = new(types.Count);

            for (int i = 0; i < types.Count; i++)
            {
                numberOfVehicles.Add(0);
            }

            for (int i = 0; i < _vehicles.Count; i++)
            {
                numberOfVehicles[(int)_vehicles[i].VType]++;
            }

            for (int i = 0; i < types.Count; i++)
            {
                if (numberOfVehicles[i] > 0)
                {
                    Console.WriteLine($"- {Vehicle.GetVehicleNaturalName(types[i])}: {numberOfVehicles[i]}");
                }
            }
        }

        // Lägg till ett fordon i garaget.
        public void AddVehicle(Vehicle vehicle)
        {
            if (FindVehicle(vehicle.RegistrationNumber) != null)
            {
                throw new ArgumentException("Ett fordon med det registreringsnumret finns redan i garaget.");
            }
            if (FreeCapacity == 0)
            {
                throw new ArgumentException("Det finns inga lediga parkeringsplatser i garaget.");
            }
            if (AllowedVehicles.IndexOf(vehicle.VType) < 0)
            {
                throw new ArgumentException("Fordon av den här fordonstypen kan inte parkeras i det här garaget.");
            }
            _vehicles.Add(vehicle);
        }

        // Lägg till ett fordon i garaget via användarinput.
        public void AddVehicle()
        {
            if (FreeCapacity == 0)
            {
                Console.WriteLine("Det finns inga lediga parkeringsplatser i garaget.");
                return;
            }

            Console.WriteLine("Vilken fordonstyp vill du lägga till?");
            Console.Write("Tillgängliga fordonstyper är: ");
            for (int i = 0; i < AllowedVehicles.Count; i++)
            {
                if (i > 0)
                {
                    Console.Write(", ");
                }
                Console.Write($"{i + 1} {Vehicle.GetVehicleNaturalName(AllowedVehicles[i])}");
            }
            Console.WriteLine();

            VehicleType vehicleType;
            while (true)
            {
                Console.Write("Ange fordonstyp: ");
                string s = Console.ReadLine()?.Trim() ?? throw new IOException();
                if (s == "")
                {
                    Console.WriteLine("Du angav ingen fordonstyp. Avbryter.");
                    return;
                }
                else if (int.TryParse(s, out int n))
                {
                    if (n >= 1 && n <= AllowedVehicles.Count)
                    {
                        vehicleType = AllowedVehicles[n - 1];
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Ogiltigt värde. Du måste ange en giltig fordonstyp.");
                    }
                }
                else
                {
                    VehicleType? vt = Vehicle.GetVehicleType(s);
                    if (vt == null)
                    {
                        Console.WriteLine("Ogiltigt värde. Du måste ange en giltig fordonstyp.");
                    }
                    else if (!AllowedVehicles.Contains((VehicleType)vt))
                    {
                        Console.WriteLine($"Fordon av typen {Vehicle.GetVehicleNaturalName((VehicleType)vt)} är inte tillåtna i det här garaget.");
                    }
                    else
                    {
                        vehicleType = (VehicleType)vt;
                        break;
                    }
                }
            }

            string registrationNumber;
            while (true)
            {
                Console.Write("Ange registreringsnummer: ");
                registrationNumber = Console.ReadLine()?.Trim() ?? throw new IOException();
                if (registrationNumber == "")
                {
                    Console.WriteLine("Du angav inget registreringsnummer. Avbryter.");
                    return;
                }
                else if (FindVehicle(registrationNumber) != null)
                {
                    Console.WriteLine($"Ett fordon med registreringsnummer {registrationNumber} finns redan i garaget.");
                }
                else
                {
                    break;
                }
            }

            try
            {
                Vehicle vehicle = Vehicle.CreateVehicle(vehicleType, registrationNumber);

                Console.WriteLine("\nÖvrigt egenskaper är frivilliga att lägga till.");
                Console.WriteLine("Tryck bara Enter om du inte vill lägga till en viss egenskap.\n");

                vehicle.CustomizeValues();
                AddVehicle(vehicle);

                Console.WriteLine($"\nDu har lagt till följande fordon:");
                Console.WriteLine(vehicle.ToVerboseString());
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Det gick inte att lägga till ett fordon. " + ex.Message);
            }
        }

        // Ta bort ett fordon ur garaget.
        public void RemoveVehicle(string registrationNumber)
        {
            int n = FindIndexOfVehicle(registrationNumber);
            if (n >= 0)
            {
                _vehicles.RemoveAt(n);
            }
            else
            {
                throw new ArgumentException("Något fordon med angivet registreringsnummer finns inte i garaget.");
            }
        }

        // Ta bort ett fordon ur garaget via användarinput.
        public void RemoveVehicle()
        {
            if (Count == 0)
            {
                Console.WriteLine("Det finns inga fordon i garaget.");
                return;
            }

            Console.Write("Ange registreringsnumret för fordonet att ta bort: ");
            string s = Console.ReadLine()?.Trim() ?? throw new IOException();
            if (s == "")
            {
                Console.WriteLine("Inget registreringsnummer angivet. Avbryter.");
                return;
            }

            try
            {
                RemoveVehicle(s);
                Console.WriteLine($"Fordonet med registreringsnummer {s} har tagits bort.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Det gick inte att ta bort fordonet. " + ex.Message);
            }
        }

        // Lägg till slumpmässiga fordon i garaget.
        // Om det inte går att hitta på n unika registreringsnummer kan
        // färre än n fordon läggas till, och ett exception kastas.
        public void AddRandomVehicles(int n)
        {
            if (n < 0)
            {
                throw new ArgumentException("Antalet fordon att lägga till får inte vara ett negativt tal.");
            }
            else if (n > FreeCapacity)
            {
                throw new ArgumentException("Det finns inte tillräckligt många lediga platser i garaget.");
            }

            for (int i = 0; i < n; i++)
            {
                int tries = 0;
                while (tries < 100)
                {
                    Vehicle vehicle = Vehicle.CreateRandomVehicle(AllowedVehicles);
                    if (FindVehicle(vehicle.RegistrationNumber) == null)
                    {
                        AddVehicle(vehicle);
                        break;
                    }
                    tries++;
                }
                if (tries == 100)
                {
                    throw new InvalidOperationException("Det gick inte att hitta något unikt registreringsnummer.");
                }
            }
        }

        // Returnerar "1 ledig plats" eller $"{n} lediga platser" beroende på värdet av n.
        private static string LedigaPlatser(int n)
        {
            return n == 1 ? "1 ledig plats" : $"{n} lediga platser";
        }

        // Lägg till slumpmässiga fordon i garaget via användarinput.
        public void AddRandomVehicles()
        {
            if (FreeCapacity == 0)
            {
                Console.WriteLine("Det går inte att lägga till fler fordon eftersom garaget är fullt.");
                return;
            }

            Console.WriteLine($"Garaget har {LedigaPlatser(FreeCapacity)}.");

            int nRandom;
            int defaultRandom = FreeCapacity / 2;
            if (defaultRandom < 1)
            {
                defaultRandom = 1;
            }
            while (true)
            {
                Console.Write($"Ange antal slumpmässiga fordon att lägga till ({defaultRandom}): ");
                string s = Console.ReadLine()?.Trim() ?? throw new IOException();
                if (s == "")
                {
                    Console.WriteLine($"Använder det föreslagna värdet: {defaultRandom}.");
                    nRandom = defaultRandom;
                    break;
                }
                else if (!int.TryParse(s, out nRandom) || nRandom < 0)
                {
                    Console.WriteLine("Ogiltigt värde. Ange ett icke-negativt heltal.");
                }
                else if (nRandom > FreeCapacity)
                {
                    Console.WriteLine($"Du kan inte lägga till så många fordon. Garaget har bara {LedigaPlatser(FreeCapacity)}.");
                }
                else
                {
                    break;
                }
            }

            if (nRandom == 0)
            {
                Console.WriteLine("Inga fordon kommer att läggas till.");
                return;
            }

            Console.WriteLine();

            int oldCount = Count;
            try
            {
                AddRandomVehicles(nRandom);
                Console.WriteLine($"Du lade till {nRandom} fordon i garaget.");
            }
            catch (Exception ex)
            {
                if (Count > oldCount)
                {
                    Console.WriteLine($"Du lade till {Count - oldCount} fordon i garaget.");
                    Console.WriteLine("Det gick inte att lägga till alla fordon. " + ex.Message);
                }
                else
                {
                    Console.WriteLine("Det gick inte att lägga till några fordon i garaget. " + ex.Message);
                }
            }
        }

        // Hitta index i garaget för ett fordon via registreringsnumret.
        // Returnerar -1 om fordonet inte finns i garaget.
        public int FindIndexOfVehicle(string registrationNumber)
        {
            string regNr = Vehicle.NormalizeRegistrationNumber(registrationNumber);
            return _vehicles.FindIndex(v => v.RegistrationNumber == regNr);
        }

        // Hitta ett fordon via registreringsnumret.
        // Returnerar null om fordonet inte finns i garaget.
        public Vehicle? FindVehicle(string registrationNumber)
        {
            string regNr = Vehicle.NormalizeRegistrationNumber(registrationNumber);
            return _vehicles.Find(v => v.RegistrationNumber == regNr);
        }

        // Hitta ett fordon via registreringsnumret via användarinput.
        public void FindVehicle()
        {
            Console.Write("Ange registreringsnummer: ");
            string registrationNumber = Console.ReadLine()?.Trim() ?? throw new IOException();

            if (registrationNumber == "")
            {
                Console.WriteLine("Inget registreringsnummer angavs. Avbryter.");
                return;
            }

            Vehicle? vehicle = FindVehicle(registrationNumber);
            if (vehicle == null)
            {
                Console.WriteLine($"Det finns inget fordon med registreringsnummer {registrationNumber} i garaget.");
            }
            else
            {
                Console.WriteLine("\nDet sökta fordonet finns i garaget:");
                WriteStringInColour(vehicle.ToVerboseString() + "\n");
            }
        }

        // Skriver ut texten med vissa färgord i färg.
        public static void WriteStringInColour(string text)
        {
            // Detta skulle ha varit en array om det hade varit tillåtet.
            List<string> lines = [.. text.Split("\n")];
            for (int j = 0; j < lines.Count; j++)
            {
                if (j > 0)
                {
                    Console.Write("\n");
                }

                // Detta skulle ha varit en array om det hade varit tillåtet.
                List<string> words = [.. lines[j].Split(" ")];
                for (int i = 0; i < words.Count; i++)
                {
                    if (i > 0)
                    {
                        Console.Write(" ");
                    }

                    string word = words[i];
                    if (word.Length > 0)
                    {
                        if (word[^1] == ';' || word[^1] == ',')
                        {
                            WriteWordInColour(word[..^1]);
                            Console.Write(word[^1]);
                        }
                        else
                        {
                            WriteWordInColour(word);
                        }
                    }
                }
            }
        }

        // Skriv ut texten. Om hela textsträngen är lika med
        // vissa färgord skrivs texten ut i motsvarande färg.
        public static void WriteWordInColour(string text)
        {
            ConsoleColor? colour = text switch
            {
                "blå" => ConsoleColor.Blue,
                "grön" => ConsoleColor.Green,
                "gul" => ConsoleColor.Yellow,
                "grå" => ConsoleColor.DarkGray,
                "lila" => ConsoleColor.DarkMagenta,
                "mörkblå" => ConsoleColor.DarkBlue,
                "mörkgrön" => ConsoleColor.DarkGreen,
                "mörkröd" => ConsoleColor.DarkRed,
                "orange" => ConsoleColor.DarkYellow,
                "rosa" => ConsoleColor.Magenta,
                "röd" => ConsoleColor.Red,
                "turkos" => ConsoleColor.Cyan,
                _ => null
            };
            if (colour == null)
            {
                Console.Write(text);
            }
            else
            {
                Console.ForegroundColor = (ConsoleColor)colour;
                Console.Write(text);
                Console.ResetColor();
            }
        }

        // Ta emot sökvärden för en fordonsegenskap via användarinput.
        public static void InputSearchProperty(string property, StringDictionary properties,
            StringDictionary searchValues)
        {
            StringSet? available = properties.Get(property);
            if (available != null && available.Count > 0)
            {
                Console.Write($"Möjliga värden för {property}: ");
                const int maxDisplayed = 50;
                List<string> availableValues = available.Values();
                availableValues.Sort(_comparer);
                int length = Math.Min(availableValues.Count, maxDisplayed);

                if (property == "färg")
                {
                    for (int i = 0; i < availableValues.Count; i++)
                    {
                        if (i > 0)
                        {
                            Console.Write(", ");
                        }
                        WriteWordInColour(availableValues[i]);
                    }
                }
                else
                {
                    Console.Write(String.Join(", ", availableValues[0..length]));
                }

                if (availableValues.Count > maxDisplayed)
                {
                    Console.Write(", ...");
                    Console.Write("\nAntalet möjliga värden är så många att alla inte visas här.");
                }

                Console.WriteLine();

                Console.WriteLine($"Om du vill ange flera sökvärden, separera dem med mellanslag.");
                Console.Write($"Ange ett eller flera sökvärden för {property}: ");
                string s = Console.ReadLine()?.Trim().ToLower() ?? throw new IOException();
                if (s == "")
                {
                    Console.WriteLine("Inget värde angivet.");
                    return;
                }
                // Detta skulle ha varit en array om det hade varit tillåtet.
                List<string> values = [.. s.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)];
                for (int i = 0; i < values.Count; i++)
                {
                    if (!availableValues.Contains(values[i]))
                    {
                        Console.WriteLine($"Värdet {values[i]} finns inte bland de möjliga värdena.");
                        Console.WriteLine("Eftersom värdet inte skulle matcha några fordon har det ignorerats.");
                    }
                    else
                    {
                        searchValues.Add(property, values[i]);
                    }
                }
            }
        }

        // Hitta fordon utifrån egenskaper.
        public List<Vehicle> FindMatchingVehicles(StringDictionary searchValues)
        {
            List<Vehicle> result = [];
            for (int i = 0; i < _vehicles.Count; i++)
            {
                if (_vehicles[i].Matches(searchValues))
                {
                    result.Add(_vehicles[i]);
                }
            }
            return result;
        }

        // Hitta fordon utifrån egenskaper via användarinput.
        public void FindMatchingVehicles()
        {
            Console.WriteLine("För att hitta fordon, ange värden för de egenskaper du vill söka på.");
            Console.WriteLine("Om du anger flera värden för samma egenskap får du matchningar\nsom stämmer överens med något av värdena.");
            Console.WriteLine("Tryck Enter när du inte vill ange fler sökvärden.");

            if (_vehicles.Count > 70)
            {
                Console.WriteLine("Vänta, förbereder sökning...");
            }

            StringDictionary properties = new();
            for (int i = 0; i < _vehicles.Count; i++)
            {
                Vehicle vehicle = _vehicles[i];
                for (int j = 0; j < vehicle.Properties.Count; j++)
                {
                    string property = vehicle.Properties[j];
                    properties.Add(property, vehicle.GetPropertyValue(property));
                }
            }

            StringDictionary searchValues = new();
            List<string> propertyKeys = properties.Keys();
            propertyKeys.Sort(_comparer);

            while (true)
            {
                Console.WriteLine();

                Console.Write("Följande egenskaper är sökbara: ");
                Console.WriteLine(String.Join(", ", propertyKeys));

                bool hasValues = searchValues.Count > 0;
                Console.Write($"Ange{(hasValues ? " en till" : "")} egenskap att söka på: ");
                string s = Console.ReadLine()?.Trim().ToLower() ?? throw new IOException();
                if (s == "")
                {
                    break;
                }
                string? property = propertyKeys.Find(p => p == s);
                if (property == null)
                {
                    Console.WriteLine("Ogiltigt värde. Ange en giltig egenskap, eller tryck Enter för att inte ange fler egenskaper.");
                }
                else
                {
                    InputSearchProperty(property, properties, searchValues);

                    if (searchValues.Count > 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Sökningen innehåller för närvarande följande sökvärden:");
                        Console.Write(searchValues);
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine("Lägger inte till fler sökvärden.");
            Console.WriteLine();

            if (searchValues.Count == 0)
            {
                Console.WriteLine("Du har inte angivit några sökvärden. Avbryter.");
                return;
            }

            Console.WriteLine("Du har angivit följande sökvärden:");
            Console.WriteLine(searchValues);

            List<Vehicle> matchingVehicles = FindMatchingVehicles(searchValues);
            if (matchingVehicles.Count == 0)
            {
                Console.WriteLine("Inga fordon matchade sökningen.");
            }
            else
            {
                Console.WriteLine($"{matchingVehicles.Count} fordon matchar sökningen.");
                ListVehicles(matchingVehicles);
            }
        }

        // Ta bort alla fordon från garaget.
        public void Clear()
        {
            _vehicles.Clear();
        }

        // Ta bort alla fordon från garaget via användarinput.
        public void AskAndClear()
        {
            Console.WriteLine("Är du säker på att du vill ta bort alla fordon? Skriv i så fall: ja");
            string s = Console.ReadLine()?.Trim() ?? throw new IOException();
            if (s == "ja")
            {
                Clear();
                Console.WriteLine("Alla fordon togs bort.");
            }
            else
            {
                Console.WriteLine("Avbryter. Inga fordon togs bort.");
            }
        }
    }
}
