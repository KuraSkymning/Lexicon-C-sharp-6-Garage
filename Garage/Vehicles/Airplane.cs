using System;
using System.Collections.Generic;
using System.IO;

namespace Garage.Vehicles
{
    // Ett flygplan.
    // Förutom basegenskaperna har ett flygplan även egenskaperna antal säten, antal motorer och längd.
    internal class Airplane : Vehicle, IVehicleWithSeats, IVehicleWithLength
    {
        public override VehicleType VType { get; } = VehicleType.Airplane;

        public override int? NumberOfWheels
        {
            get; protected set
            {
                field = value == null || value >= 0 && value <= 8 ?
                        value :
                        throw new ArgumentException("Antalet hjul på ett flygplan måste vara mellan 0 och 8.");
            }
        }

        public int? NumberOfSeats
        {
            get; set
            {
                field = value == null || value >= 1 && value <= 100 ?
                        value :
                        throw new ArgumentException("Antalet säten i ett flygplan måste vara mellan 1 och 100.");
            }
        }

        public int? NumberOfEngines
        {
            get; protected set
            {
                field = value == null || value >= 1 && value <= 4 ?
                        value :
                        throw new ArgumentException("Antalet motorer på ett flygplan måste vara mellan 1 och 4.");
            }
        }

        public double? Length
        {
            get; set
            {
                field = value == null || value >= 4 && value <= 20 ?
                        value :
                        throw new ArgumentException("Längden på ett flygplan måste vara mellan 4 och 20 meter.");
            }
        }

        public override List<string> Properties { get; } =
            [.. BaseProperties, "antal säten", "antal motorer", "längd"];

        public Airplane() : base() { }

        public Airplane(string registrationNumber) : base(registrationNumber) { }

        public override string GetPropertyValue(string property)
        {
            try
            {
                return base.GetPropertyValue(property);
            }
            catch (ArgumentException)
            {
                return property switch
                {
                    "antal motorer" => NumberOfEngines == null ? "okänt" : $"{NumberOfEngines}",
                    "antal säten" => NumberOfSeats == null ? "okänt" : $"{NumberOfSeats}",
                    "längd" => Length == null ? "okänd" : $"{Length}",
                    _ => throw new ArgumentException("Egenskapen finns inte hos fordonet."),
                };
            }
        }

        protected override void RandomizeValues()
        {
            base.RandomizeValues();
            NumberOfWheels = Random.Next(0, 9);
            NumberOfEngines = Random.Next(1, 3);
            NumberOfSeats = Random.Next(1, 100);
            Length = 4 + 0.5 * Random.Next(30);
        }

        public override void CustomizeValues()
        {
            base.CustomizeValues();
            CustomizeLength(this);
            CustomizeNumberOfSeats(this);

            while (true)
            {
                Console.Write("Ange antal motorer: ");
                string s = Console.ReadLine()?.Trim() ?? throw new IOException();
                if (s == "")
                {
                    break;
                }
                else if (int.TryParse(s, out int n))
                {
                    try
                    {
                        NumberOfEngines = n;
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

        public override string ToVerboseString()
        {
            string res = "";
            res += "Registreringsnummer: " + (RegistrationNumber ?? "okänt");
            res += "\nFordonstyp: " + GetPropertyValue("fordonstyp");
            res += "\nFärg: " + GetPropertyValue("färg");
            res += "\nAntal hjul: " + GetPropertyValue("antal hjul");
            res += "\nLängd: " + GetPropertyValue("längd") + (Length == null ? "" : " meter");
            res += "\nAntal säten: " + GetPropertyValue("antal säten");
            res += "\nAntal motorer: " + GetPropertyValue("antal motorer");
            return res;
        }

        public override string ToString()
        {
            string res = "";
            res += "Reg.nr: " + (RegistrationNumber ?? "okänt");
            res += "; " + GetPropertyValue("fordonstyp");
            res += "; Färg: " + GetPropertyValue("färg");
            res += "; Hjul: " + GetPropertyValue("antal hjul");
            res += "; Längd: " + GetPropertyValue("längd") + (Length == null ? "" : " m");
            res += "; Säten: " + GetPropertyValue("antal säten");
            res += "; Motorer: " + GetPropertyValue("antal motorer");
            return res;
        }
    }
}
