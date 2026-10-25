using System;
using System.Collections.Generic;

namespace Garage.Vehicles
{
    // En buss.
    // Förutom basegenskaperna har en buss även egenskaperna antal säten, längd och bränsletyp.
    internal class Bus : Vehicle, IVehicleWithFuel, IVehicleWithSeats, IVehicleWithLength
    {
        public override VehicleType VType { get; } = VehicleType.Bus;

        public override int? NumberOfWheels
        {
            get; protected set
            {
                field = value == null || value >= 4 && value <= 8 && value % 2 == 0 ?
                    value :
                    throw new ArgumentException("Antalet hjul på en buss måste vara ett jämnt antal mellan 4 och 8.");
            }
        }

        public FuelType? Fueltype { get; set; }

        public int? NumberOfSeats
        {
            get; set
            {
                field = value == null || value >= 1 && value <= 200 ?
                        value :
                        throw new ArgumentException("Antalet säten i en buss måste vara mellan 1 och 200.");
            }
        }

        public double? Length
        {
            get; set
            {
                field = value == null || value >= 4 && value <= 20 ?
                        value :
                        throw new ArgumentException("Längden på en buss måste vara mellan 4 och 20 meter.");
            }
        }

        public override List<string> Properties { get; } =
            [.. BaseProperties, "bränsletyp", "antal säten", "längd"];

        public Bus() : base() { }

        public Bus(string registrationNumber) : base(registrationNumber) { }

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
                    "bränsletyp" => Fueltype == null ? "okänd" : GetFuelNaturalName(Fueltype.Value),
                    "antal säten" => NumberOfSeats == null ? "okänt" : $"{NumberOfSeats}",
                    "längd" => Length == null ? "okänd" : $"{Length}",
                    _ => throw new ArgumentException("Egenskapen finns inte hos fordonet."),
                };
            }
        }

        protected override void RandomizeValues()
        {
            base.RandomizeValues();
            Fueltype = (FuelType)Random.Next(0, Enum.GetValues(typeof(FuelType)).Length);
            NumberOfSeats = Random.Next(1, 100);
            Length = 4 + 0.5 * Random.Next(30);
            NumberOfWheels = 4 + 2 * Random.Next(3);
        }

        public override void CustomizeValues()
        {
            base.CustomizeValues();
            CustomizeFuelType(this);
            CustomizeNumberOfSeats(this);
            CustomizeLength(this);
            CustomizeNumberOfWheels(this);
        }

        public override string ToVerboseString()
        {
            string res = "";
            res += "Registreringsnummer: " + (RegistrationNumber ?? "okänt");
            res += "\nFordonstyp: " + GetPropertyValue("fordonstyp");
            res += "\nFärg: " + GetPropertyValue("färg");
            res += "\nAntal säten: " + GetPropertyValue("antal säten");
            res += "\nBränsletyp: " + GetPropertyValue("bränsletyp");
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
            res += "; Säten: " + GetPropertyValue("antal säten");
            res += "; Bränsletyp: " + GetPropertyValue("bränsletyp");
            res += "; Hjul: " + GetPropertyValue("antal hjul");
            res += "; Längd: " + GetPropertyValue("längd") + (Length == null ? "" : " m");
            return res;
        }
    }
}
