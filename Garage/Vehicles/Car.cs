using System;
using System.Collections.Generic;

namespace Garage.Vehicles
{
    // Representerar en bil.
    // Förutom basegenskaperna har en bil även egenskaperna antal säten och bränsletyp.
    internal class Car : Vehicle, IVehicleWithFuel, IVehicleWithSeats
    {
        public override VehicleType VType { get; } = VehicleType.Car;

        public override int? NumberOfWheels { get; protected set; } = 4;

        public int? NumberOfSeats
        {
            get; set
            {
                field = value == null || value >= 1 && value <= 6 ?
                    value :
                    throw new ArgumentException("Antalet säten i en bil måste vara mellan 1 och 6.");
            }
        }

        public FuelType? Fueltype { get; set; }

        public override List<string> Properties { get; } =
            [.. BaseProperties, "bränsletyp", "antal säten"];

        public Car() : base()
        {
            RegistrationNumber = GetRandomRegistrationNumber();
        }

        public Car(string registrationNumber) : base()
        {
            RegistrationNumber = registrationNumber;
        }

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
                    _ => throw new ArgumentException("Egenskapen finns inte hos fordonet."),
                };
            }
        }

        protected override void RandomizeValues()
        {
            base.RandomizeValues();
            NumberOfSeats = Random.Next(1, 5);
            Fueltype = (FuelType)Random.Next(0, Enum.GetValues(typeof(FuelType)).Length);
        }

        public override void CustomizeValues()
        {
            base.CustomizeValues();
            CustomizeFuelType(this);
            CustomizeNumberOfSeats(this);
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
            return res;
        }
    }
}
