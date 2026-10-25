using Garage.Vehicles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Garage
{
    // Fordonstyper.
    public enum VehicleType
    {
        Car,
        Bus,
        Boat,
        Airplane,
        Motorcycle,
    }

    // Bränsletyper.
    public enum FuelType
    {
        Gasoline,
        Diesel,
        Electricity,
    }

    // Basklass för fordon.
    abstract class Vehicle
    {
        // Fordonstyp.
        public abstract VehicleType VType { get; }

        // Fordonets registreringsnummer.
        // Personliga registreringsnummer hanteras ej i detta program,
        // eftersom uppgiften inte tycks tillåta sådana.
        public virtual string RegistrationNumber
        {
            get; protected set
            {
                field = value == "" ?
                    throw new ArgumentException("Registreringsnumret får inte vara tomt.") :
                    !IsStandardRegistrationNumber(value) ?
                    throw new ArgumentException("Registreringsnumret är inte på korrekt format.") :
                    value;
            }
        }

        // Färg på fordonet.
        public virtual string? Colour
        {
            get; set
            {
                field = value == "" || value == null ? null :
                    !Regex.IsMatch(value, @"^[a-zåäö]+$") ?
                    throw new ArgumentException("Ogiltigt färgvärde.") :
                    value;
            }
        }

        // Antal hjul på fordonet.
        public virtual int? NumberOfWheels
        {
            get; protected set
            {
                field = value < 0 ?
                    throw new ArgumentException("Antalet hjul kan inte vara ett negativt tal.") :
                    value;
            }
        }

        // Tillgängliga egenskaper på fordonet.
        // Borde egentligen implementeras som IEnumerable<string> om det hade varit tillåtet.
        public virtual List<string> Properties { get; } = BaseProperties;

        // Hjälpproperty som underklasser kan använda för att initiera Properties.
        // Borde egentligen implementeras som IEnumerable<string> om det hade varit tillåtet.
        protected static List<string> BaseProperties { get; } = ["fordonstyp", "färg", "antal hjul"];

        // Hjälpproperty för att generera slumptal.
        protected static Random Random { get; } = new();

        public Vehicle()
        {
            RegistrationNumber = GetRandomRegistrationNumber();
        }

        public Vehicle(string registrationNumber)
        {
            RegistrationNumber = registrationNumber;
        }

        // Hämta ett strängvärde för en egenskap.
        public virtual string GetPropertyValue(string property)
        {
            return property switch
            {
                "fordonstyp" => GetVehicleNaturalName(VType),
                "färg" => Colour ?? "okänd",
                "antal hjul" => NumberOfWheels == null ? "okänt" : $"{NumberOfWheels}",
                _ => throw new ArgumentException("Egenskapen finns inte hos fordonet."),
            };
        }

        // Kontrollera om en fordonsegenskap matchar något av värdena i listan.
        public bool Matches(string property, List<string> values)
        {
            try
            {
                string value = GetPropertyValue(property);
                return values.IndexOf(value) >= 0;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        // Kontrollera om fordonet matchar värdena i properties.
        // Varje nyckel i properties anger en fordonsegenskap,
        // och motsvarande värden anger värden att matcha mot.
        public bool Matches(StringDictionary properties)
        {
            List<string> keys = properties.Keys();
            for (int i = 0; i < keys.Count; i++)
            {
                string property = keys[i];
                List<string> values = properties.Get(property)!.Values();
                if (!Matches(property, values))
                {
                    return false;
                }
            }
            return true;
        }

        // Omvandla fordonet till en detaljerad sträng med fordonets egenskaper.
        public abstract string ToVerboseString();

        // Ange slumpmässiga värden för fordonets egenskaper.
        protected virtual void RandomizeValues()
        {
            Colour = GetRandomColour();
        }

        // Ändra värdena för fordonets egenskaper genom användarinput.
        public virtual void CustomizeValues()
        {
            CustomizeColour(this);
        }

        // Skapa ett fordon av en viss typ med ett visst registreringsnummer.
        public static Vehicle CreateVehicle(VehicleType type, string registrationNumber)
        {
            string regNr = NormalizeRegistrationNumber(registrationNumber);
            Vehicle vehicle = type switch
            {
                VehicleType.Motorcycle => new Motorcycle(regNr),
                VehicleType.Airplane => new Airplane(regNr),
                VehicleType.Car => new Car(regNr),
                VehicleType.Bus => new Bus(regNr),
                VehicleType.Boat => new Boat(regNr),
                _ => throw new ArgumentException("Okänd fordonstyp."),
            };
            return vehicle;
        }

        // Skapa ett slumpmässigt fordon.
        public static Vehicle CreateRandomVehicle(List<VehicleType> types)
        {
            Vehicle vehicle = types[Random.Next(types.Count)] switch
            {
                VehicleType.Motorcycle => new Motorcycle(),
                VehicleType.Airplane => new Airplane(),
                VehicleType.Car => new Car(),
                VehicleType.Bus => new Bus(),
                VehicleType.Boat => new Boat(),
                _ => throw new ArgumentException("Okänd fordonstyp."),
            };
            vehicle.RandomizeValues();
            return vehicle;
        }

        // Normalisera ett registreringsnummer.
        public static string NormalizeRegistrationNumber(string registrationNumber)
        {
            return registrationNumber.ToUpper();
        }

        // Generera ett slumpmässigt registreringsnummer.
        public virtual string GetRandomRegistrationNumber()
        {
            string regNr = "";
            regNr += (char)('A' + Random.Next(26));
            regNr += (char)('A' + Random.Next(26));
            regNr += (char)('A' + Random.Next(26));
            regNr += Random.Next(10);
            regNr += Random.Next(10);
            regNr += Random.Next(2) == 0 ? Random.Next(10) : "" + (char)('A' + Random.Next(26));
            regNr = regNr.Replace('I', (char)('A' + Random.Next(7)));
            regNr = regNr.Replace('Q', (char)('A' + Random.Next(7)));
            regNr = regNr.Replace('V', (char)('A' + Random.Next(7)));
            if (regNr[^1] == 'O')
            {
                regNr = regNr[..^1] + (char)('A' + Random.Next(7));
            }
            return NormalizeRegistrationNumber(regNr);
        }

        // Generera en slumpmässig färg.
        public static string GetRandomColour()
        {
            List<string> colours = [
                "blå",
                "brun",
                "grå",
                "grön",
                "gul",
                "lila",
                "mörkblå",
                "mörkgrön",
                "mörkröd",
                "orange",
                "rosa",
                "röd",
                "silver",
                "svart",
                "turkos",
                "vit",
            ];
            return colours[Random.Next(colours.Count)];
        }

        // Översätt en VehicleType till svenska.
        public static string GetVehicleNaturalName(VehicleType type)
        {
            return type switch
            {
                VehicleType.Motorcycle => "motorcykel",
                VehicleType.Airplane => "flygplan",
                VehicleType.Car => "bil",
                VehicleType.Bus => "buss",
                VehicleType.Boat => "båt",
                _ => "okänd",
            };
        }

        // Översätt från svenska till VehicleType.
        public static VehicleType? GetVehicleType(string type)
        {
            return type switch
            {
                "motorcykel" => VehicleType.Motorcycle,
                "flygplan" => VehicleType.Airplane,
                "bil" => VehicleType.Car,
                "buss" => VehicleType.Bus,
                "båt" => VehicleType.Boat,
                _ => null,
            };
        }

        // Översätt en FuelType till svenska.
        public static string GetFuelNaturalName(FuelType type)
        {
            return type switch
            {
                FuelType.Gasoline => "bensin",
                FuelType.Diesel => "diesel",
                FuelType.Electricity => "el",
                _ => "okänd",
            };
        }

        // Översätt från svenska till FuelType.
        public static FuelType? GetFuelType(string type)
        {
            return type switch
            {
                "bensin" => FuelType.Gasoline,
                "diesel" => FuelType.Diesel,
                "el" => FuelType.Electricity,
                _ => null,
            };
        }

        // Ange färg via användarinput.
        public static void CustomizeColour(Vehicle vehicle)
        {
            while (true)
            {
                Console.Write("Ange färg: ");
                string s = Console.ReadLine()?.Trim().ToLower() ?? throw new IOException();
                if (s == "")
                {
                    break;
                }
                try
                {
                    vehicle.Colour = s == "" ? null : s;
                    break;
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine("Ogiltigt värde. " + e.Message);
                }
            }
        }

        // Ange antal hjul via användarinput.
        public static void CustomizeNumberOfWheels(Vehicle vehicle)
        {
            while (true)
            {
                Console.Write("Ange antal hjul: ");
                string s = Console.ReadLine()?.Trim() ?? throw new IOException();
                if (s == "")
                {
                    break;
                }
                else if (int.TryParse(s, out int n))
                {
                    try
                    {
                        vehicle.NumberOfWheels = n;
                        break;
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine("Ogiltigt värde. " + e.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Ogiltigt värde. Du måste ange ett heltal.");
                }
            }
        }

        // Ange bränsletyp via användarinput.
        public static void CustomizeFuelType(IVehicleWithFuel vehicle)
        {
            while (true)
            {
                Console.Write("Ange bränsletyp: ");
                string s = Console.ReadLine()?.Trim().ToLower() ?? throw new IOException();
                if (s == "")
                {
                    break;
                }
                FuelType? ft = GetFuelType(s);
                if (ft != null)
                {
                    vehicle.Fueltype = ft;
                    break;
                }
                else
                {
                    Console.WriteLine("Ogiltigt värde. Giltiga värden är: bensin, diesel, el.");
                }
            }

        }

        // Ange antal säten via användarinput.
        public static void CustomizeNumberOfSeats(IVehicleWithSeats vehicle)
        {
            while (true)
            {
                Console.Write("Ange antal säten: ");
                string s = Console.ReadLine()?.Trim() ?? throw new IOException();
                if (s == "")
                {
                    break;
                }
                else if (int.TryParse(s, out int n))
                {
                    try
                    {
                        vehicle.NumberOfSeats = n;
                        break;
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine("Ogiltigt värde. " + e.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Ogiltigt värde. Du måste ange ett heltal.");
                }
            }
        }

        // Ange längd via användarinput.
        public static void CustomizeLength(IVehicleWithLength vehicle)
        {
            while (true)
            {
                Console.Write("Ange längd i meter: ");
                string s = Console.ReadLine()?.Trim() ?? throw new IOException();
                if (s == "")
                {
                    break;
                }
                else if (double.TryParse(s, out double n))
                {
                    try
                    {
                        vehicle.Length = n;
                        break;
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine("Ogiltigt värde. " + e.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Ogiltigt värde. Du måste ange ett tal.");
                }
            }
        }

        // Kontrollera om ett registreringsnummer följer det svenska standardformatet.
        protected static bool IsStandardRegistrationNumber(string registrationNumber)
        {
            if (registrationNumber.Contains('I') ||
                registrationNumber.Contains('Q') ||
                registrationNumber.Contains('V'))
            {
                return false;
            }
            if (!Regex.IsMatch(registrationNumber, @"^[A-Z][A-Z][A-Z][0-9][0-9][0-9A-Z]$"))
            {
                return false;
            }
            if (registrationNumber[5] == 'O')
            {
                return false;
            }
            return true;
        }

        // Kontrollera om ett registreringsnummer följer formatet
        // för personliga registreringsnummer.
        protected static bool IsCustomCarRegistrationNumber(string registrationNumber)
        {
            if (registrationNumber.Length < 2 || registrationNumber.Length > 7)
            {
                return false;
            }
            return Regex.IsMatch(registrationNumber, @"^[ A-ZÅÄÖ0-9]+$");
        }

        // Kontrollera om ett registreringsnummer följer formatet
        // för personliga registreringsnummer för motorcyklar.
        protected static bool IsCustomMCRegistrationNumber(string registrationNumber)
        {
            if (registrationNumber.Length < 2 || registrationNumber.Length > 6)
            {
                return false;
            }
            return Regex.IsMatch(registrationNumber, @"^[ A-ZÅÄÖ0-9]+$");
        }
    }

    public interface IVehicleWithFuel
    {
        FuelType? Fueltype { get; set; }
    }

    public interface IVehicleWithSeats
    {
        int? NumberOfSeats { get; set; }
    }

    public interface IVehicleWithLength
    {
        double? Length { get; set; }
    }
}
