using System;
using System.Collections.Generic;
using System.IO;

namespace Garage.Vehicles
{
    // Representerar en motorcykel.
    // Förutom basegenskaperna har en motorcykel även egenskapen cylindervolym.
    internal class Motorcycle : Vehicle
    {
        public override VehicleType VType { get; } = VehicleType.Motorcycle;

        public override int? NumberOfWheels
        {
            get; protected set
            {
                field = value == null || value == 2 || value == 3 ?
                    value :
                    throw new ArgumentException("Antalet hjul på en motorcykel måste vara 2 eller 3.");
            }
        }

        public int? CylinderVolume
        {
            get; protected set
            {
                field = value == null || value >= 1 && value <= 3000 ?
                    value :
                    throw new ArgumentException("Cylindervolymen på en motorcykel måste vara mellan 1 och 3000 kubikcentimeter.");
            }
        }

        public override List<string> Properties { get; } = [.. BaseProperties, "cylindervolym"];

        public Motorcycle() : base() { }

        public Motorcycle(string registrationNumber) : base(registrationNumber) { }

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
                    "cylindervolym" => CylinderVolume == null ? "okänd" : $"{CylinderVolume}",
                    _ => throw new ArgumentException("Egenskapen finns inte hos fordonet."),
                };
            }
        }

        protected override void RandomizeValues()
        {
            base.RandomizeValues();
            List<int> cvs = [50, 75, 125, 250, 500, 700, 1000, 1200, 1500, 2000, 2500];
            CylinderVolume = cvs[Random.Next(cvs.Count)];
            NumberOfWheels = Random.Next(2, 4);
        }

        public override void CustomizeValues()
        {
            base.CustomizeValues();
            CustomizeNumberOfWheels(this);

            while (true)
            {
                Console.Write("Ange cylindervolym: ");
                string s = Console.ReadLine()?.Trim() ?? throw new IOException();
                if (s == "")
                {
                    break;
                }
                if (int.TryParse(s, out int cv))
                {
                    try
                    {
                        CylinderVolume = cv;
                        break;
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine("Ogiltigt värde. " + e.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Ogiltigt värde. Du måste ange ett heltal mellan 1 och 3000.");
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
            res += "\nCylindervolym: " + GetPropertyValue("cylindervolym") + (CylinderVolume == null ? "" : " kubikcentimeter");
            return res;
        }

        public override string ToString()
        {
            string res = "";
            res += "Reg.nr: " + (RegistrationNumber ?? "okänt");
            res += "; " + GetPropertyValue("fordonstyp");
            res += "; Färg: " + GetPropertyValue("färg");
            res += "; Hjul: " + GetPropertyValue("antal hjul");
            res += "; Cylindervolym: " + GetPropertyValue("cylindervolym") + (CylinderVolume == null ? "" : " cm³");
            return res;
        }
    }
}
