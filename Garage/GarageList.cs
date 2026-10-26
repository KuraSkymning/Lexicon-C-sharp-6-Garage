using System;
using System.Collections.Generic;
using System.IO;

namespace Garage
{
    // En lista med garage.
    internal class GarageList
    {
        private List<Garage> _garages = [];

        // Det aktuella garaget som är valt.
        public Garage? Current { get; private set; }

        // Antal garage i listan.
        public int Count => _garages.Count;

        // Max antal garage i listan.
        public static int MaxCount => 10000;

        // Lista alla garage.
        public void ListGarages()
        {
            for (int i = 0; i < _garages.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_garages[i].Name}");
            }
        }

        // Hitta garage med namn.
        // Returnerar null om inget garage med det namnet finns.
        private Garage? FindGarage(string name)
        {
            return _garages.Find(g => g.Name == name);
        }

        // Generera ett namnförslag för ett nytt garage.
        // Namnet är unikt inom garagelistan.
        private string GenerateSuggestedGarageName()
        {
            string name;
            int counter = 1;
            do
            {
                name = $"Garage {counter}";
                counter++;
            } while (FindGarage(name) != null && counter <= MaxCount);
            return name;
        }

        // Lägg till garage via användarinput.
        // Returnerar true om ett garage skapades, annars false.
        public bool AddGarage()
        {
            if (_garages.Count >= MaxCount)
            {
                Console.WriteLine("Max antal garage uppnått. Det går inte att lägga till fler garage.");
                return false;
            }

            string name;
            string suggestedName = GenerateSuggestedGarageName();
            while (true)
            {
                Console.Write($"Ange garagets namn ({suggestedName}): ");
                name = Console.ReadLine()?.Trim() ?? throw new IOException();
                if (name == "")
                {
                    Console.WriteLine($"Använder det föreslagna namnet: {suggestedName}.");
                    name = suggestedName;
                    break;
                }
                else if (FindGarage(name) != null)
                {
                    Console.WriteLine("Ett garage med det namnet finns redan.");
                }
                else
                {
                    break;
                }
            }

            int capacity;
            while (true)
            {
                int defaultCapacity = 100;
                Console.Write($"Ange antal parkeringsplatser ({defaultCapacity}): ");
                string inp = Console.ReadLine()?.Trim() ?? throw new IOException();
                if (inp == "")
                {
                    Console.WriteLine($"Använder det föreslagna värdet: {defaultCapacity}.");
                    capacity = defaultCapacity;
                    break;
                }
                else if (!int.TryParse(inp, out capacity))
                {
                    Console.WriteLine("Ogiltigt värde. Ange ett giltigt heltal.");
                }
                else if (capacity < 1 || capacity > Garage.MaxCapacity)
                {
                    Console.WriteLine($"Antalet platser måste vara mellan 1 och {Garage.MaxCapacity}.");
                }
                else
                {
                    break;
                }
            }

            Console.WriteLine("Vilka fordonstyper får stå i garaget?");
            Console.WriteLine("Om flera fordonstyper ska vara tillåtna, separera dem med mellanslag.");
            Console.WriteLine("För att tillåta alla fordonstyper, skriv ordet alla.");

            List<VehicleType> allTypes = [.. Enum.GetValues<VehicleType>()];
            List<VehicleType> vehicleTypes = [];
            bool loop = true;
            while (loop)
            {
                loop = false;

                Console.WriteLine("Tillgängliga fordonstyper är: " + Garage.VehicleTypesToString(allTypes));
                Console.Write("Ange tillåtna fordonstyper (alla): ");
                string s = Console.ReadLine()?.Trim().ToLower() ?? throw new IOException();
                if (s == "")
                {
                    Console.WriteLine("Använder det föreslagna värdet: alla");
                    vehicleTypes = allTypes;
                    break;
                }
                else if (s == "alla")
                {
                    vehicleTypes = allTypes;
                    break;
                }

                // Detta skulle ha varit en array om det hade varit tillåtet.
                List<string> values = [.. s.Split()];
                for (int i = 0; i < values.Count; i++)
                {
                    if (values[i] == "alla")
                    {
                        vehicleTypes = allTypes;
                        break;
                    }

                    VehicleType? vt = Vehicle.GetVehicleType(values[i]);
                    if (vt == null)
                    {
                        Console.WriteLine($"Ogiltigt värde. {values[i]} är inte en giltig fordonstyp.");
                        loop = true;
                    }
                    else
                    {
                        if (!vehicleTypes.Contains((VehicleType)vt))
                        {
                            vehicleTypes.Add((VehicleType)vt);
                        }
                    }
                }
            }

            Garage garage = new(capacity) { Name = name, AllowedVehicles = vehicleTypes };
            _garages.Add(garage);
            Current = garage;

            Console.WriteLine();
            Console.WriteLine($"Du har skapat ett nytt garage med namnet {garage.Name}.");
            Console.WriteLine($"Antal parkeringsplatser är {garage.Capacity}.");
            Console.WriteLine("Tillåtna fordonstyper är: " + Garage.VehicleTypesToString(garage.AllowedVehicles));
            Console.WriteLine("Det nya garaget väljs nu som aktuellt garage.");
            Console.WriteLine();
            Console.WriteLine("Om du vill kan du lägga till slumpmässiga fordon till det nya garaget.");
            garage.AddRandomVehicles();
            Console.WriteLine($"Antalet lediga platser är {garage.FreeCapacity}.");

            return true;
        }

        // Ta bort garage.
        private void RemoveGarage(int n)
        {
            if (n >= 0 && n < _garages.Count)
            {
                if (_garages[n] == Current)
                {
                    Current = null;
                }
                _garages.RemoveAt(n);
            }
            else
            {
                throw new ArgumentException("Det finns inget sådant garage.");
            }
        }

        // Ta bort garage via användarinput.
        public void RemoveGarage()
        {
            if (Count == 0)
            {
                Console.WriteLine("Det finns ännu inget garage tillagt. Avbryter.");
                return;
            }

            Console.WriteLine("Tillgängliga garage:");
            ListGarages();

            while (true)
            {
                Console.Write("Ange numret på garaget att ta bort: ");
                string s = Console.ReadLine()?.Trim() ?? throw new IOException();
                if (s == "")
                {
                    Console.WriteLine("Avbryter. Inget garage togs bort.");
                    return;
                }
                else if (!int.TryParse(s, out int n))
                {
                    Console.WriteLine("Ogiltig inmatning. Du måste ange ett giltigt garagenummer.");
                }
                else if (n < 1 || n > _garages.Count)
                {
                    Console.WriteLine("Ogiltigt nummer. Det finns inget garage med det numret.");
                }
                else
                {
                    string name = _garages[n - 1].Name;
                    RemoveGarage(n - 1);
                    Console.WriteLine($"Garaget {name} togs bort.");
                    return;
                }
            }
        }

        // Välj garage.
        private void SelectGarage(int index)
        {
            if (index < 0 || index >= _garages.Count)
            {
                throw new IndexOutOfRangeException();
            }
            Current = _garages[index];
        }

        // Välj garage via användarinput.
        public bool SelectGarage()
        {
            if (Count == 0)
            {
                Console.WriteLine("Det finns ännu inget garage tillagt. Avbryter.");
                return false;
            }

            Console.WriteLine("Tillgängliga garage:");
            ListGarages();

            while (true)
            {
                Console.Write("Ange numret på garaget att välja (1): ");
                string s = Console.ReadLine()?.Trim() ?? throw new IOException();
                int n;
                if (s == "")
                {
                    Console.WriteLine("Använder föreslaget värde: 1");
                    n = 1;
                }
                else if (!int.TryParse(s, out n))
                {
                    Console.WriteLine("Ogiltig inmatning. Du måste ange ett giltigt garagenummer.");
                    continue;
                }
                else if (n < 1 || n > _garages.Count)
                {
                    Console.WriteLine("Ogiltigt nummer. Det finns inget garage med det numret.");
                    continue;
                }

                try
                {
                    SelectGarage(n - 1);
                    Console.WriteLine("Du valde garaget " + Current!.Name + ".");
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Det gick inte att välja det angivna garaget. " + e.Message);
                    return false;
                }
            }
        }

        // Ta bort alla garage.
        public void Clear()
        {
            Current = null;
            _garages = [];
        }

        // Ta bort alla garage via användarinput.
        public void AskAndClear()
        {
            Console.WriteLine("Är du säker på att du vill ta bort alla garage? Skriv i så fall: ja");
            string s = Console.ReadLine()?.Trim() ?? throw new IOException();
            if (s == "ja")
            {
                Clear();
                Console.WriteLine("Alla garage togs bort.");
            }
            else
            {
                Console.WriteLine("Avbryter. Inga garage togs bort.");
            }
        }
    }
}
