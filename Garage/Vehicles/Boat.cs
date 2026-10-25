using System;
using System.Collections.Generic;

namespace Garage.Vehicles
{
    // En båt.
    // Förutom basegenskaperna har en båt även egenskapen längd.
    internal class Boat : Vehicle, IVehicleWithLength
    {
        public override VehicleType VType { get; } = VehicleType.Boat;

        public override int? NumberOfWheels { get; protected set; } = 0;

        public double? Length
        {
            get; set
            {
                field = value == null || value >= 1.5 && value <= 30 ?
                        value :
                        throw new ArgumentException("Längden på en båt måste vara mellan 1,5 och 30 meter.");
            }
        }

        public override List<string> Properties { get; } = [.. BaseProperties, "längd"];

        public Boat() : base() { }

        public Boat(string registrationNumber) : base(registrationNumber) { }

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
                    "längd" => Length == null ? "okänd" : $"{Length}",
                    _ => throw new ArgumentException("Egenskapen finns inte hos fordonet."),
                };
            }
        }

        protected override void RandomizeValues()
        {
            base.RandomizeValues();
            Length = 2 + 0.5 * Random.Next(40);
        }

        public override void CustomizeValues()
        {
            base.CustomizeValues();
            CustomizeLength(this);
        }

        public override string ToVerboseString()
        {
            string res = "";
            res += "Registreringsnummer: " + (RegistrationNumber ?? "okänt");
            res += "\nFordonstyp: " + GetPropertyValue("fordonstyp");
            res += "\nFärg: " + GetPropertyValue("färg");
            res += "\nAntal hjul: " + GetPropertyValue("antal hjul");
            res += "\nLängd: " + GetPropertyValue("längd") + (Length == null ? "" : " meter");
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
            return res;
        }
    }
}
