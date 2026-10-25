using System;
using System.IO;

namespace Garage
{
    internal class Program
    {
        private static readonly GarageList _garageList = new();

        // Menyn som visas först för att skapa eller välja ett garage.
        private static readonly Menu _initMenu = new(InitMenuHeader, [
            new MenuItem("1", "Lägg till ett nytt garage",
                () => _garageList.AddGarage() ? _garageMenu : _initMenu),
            new MenuItem("2", "Lista alla garage",
                () => { _garageList.ListGarages(); return null; },
                () => _garageList.Count > 0),
            new MenuItem("3", "Välj garage",
                () => _garageList.SelectGarage() ? _garageMenu : _initMenu,
                () => _garageList.Count > 0),
            new MenuItem("4", "Ta bort ett garage",
                () => { _garageList.RemoveGarage(); return null; },
                () => _garageList.Count > 0),
            new MenuItem("5", "Ta bort alla garage",
                () => { _garageList.AskAndClear(); return null; },
                () => _garageList.Count > 0),
            new MenuItem("x", "Avsluta programmet", () => throw new ExitException()),
        ]);

        // Menyn som visas när ett garage har valts.
        private static readonly Menu _garageMenu = new(GarageMenuHeader, [
            new MenuItem("1", "Lista parkerade fordon",
                () => { _garageList.Current!.ListVehicles(); return null; },
                () => _garageList.Current!.Count > 0),
            new MenuItem("2", "Lista antal parkerade fordon per fordonstyp",
                () => { _garageList.Current!.ListVehicleTypes(); return null; },
                () => _garageList.Current!.Count > 0),
            new MenuItem("3", "Lägg till ett fordon",
                () => { _garageList.Current!.AddVehicle(); return null; },
                () => _garageList.Current!.FreeCapacity > 0),
            new MenuItem("4", "Lägg till slumpmässiga fordon",
                () => { _garageList.Current!.AddRandomVehicles(); return null; },
                () => _garageList.Current!.FreeCapacity > 0),
            new MenuItem("5", "Ta bort ett fordon",
                () => { _garageList.Current!.RemoveVehicle(); return null; },
                () => _garageList.Current!.Count > 0),
            new MenuItem("6", "Ta bort alla fordon",
                () => { _garageList.Current!.AskAndClear(); return null; },
                () => _garageList.Current!.Count > 0),
            new MenuItem("7", "Hitta fordon via registreringsnummer",
                () => { _garageList.Current!.FindVehicle(); return null; },
                () => _garageList.Current!.Count > 0),
            new MenuItem("8", "Hitta fordon via egenskaper",
                () => { _garageList.Current!.FindMatchingVehicles(); return null; },
                () => _garageList.Current!.Count > 0),
            new MenuItem("0", "Välj ett annat garage eller skapa ett nytt", () => _initMenu),
            new MenuItem("x", "Avsluta programmet", () => throw new ExitException()),
        ]);

        // Skriver ut rubriken för _initMenu.
        private static void InitMenuHeader()
        {
            OutputWithFrame("Huvudmeny");
        }

        // Skriver ut rubriken för _garageMenu.
        private static void GarageMenuHeader()
        {
            OutputWithFrame("Garagemeny");
            Console.WriteLine();

            if (_garageList.Current == null)
            {
                Console.WriteLine("Det finns inget aktuellt garage.");
            }
            else
            {
                Console.WriteLine("Aktuellt garage: " + _garageList.Current.Name);
                Console.WriteLine("Parkerade fordon: " + _garageList.Current.Count);
                Console.WriteLine("Lediga platser: " + _garageList.Current.FreeCapacity);
                Console.WriteLine("Tillåtna fordonstyper: " +
                    Garage.VehicleTypesToString(_garageList.Current.AllowedVehicles));
            }
        }

        // Skriver ut texten med en ram runt om.
        static void OutputWithFrame(string text)
        {
            const ConsoleColor frameColor = ConsoleColor.Cyan;
            Console.ForegroundColor = frameColor;
            for (int i = 0; i < text.Length + 4; i++)
            {
                Console.Write("*");
            }
            Console.Write("\n*");
            Console.ResetColor();
            Console.Write(" " + text + " ");
            Console.ForegroundColor = frameColor;
            Console.Write("*\n");
            for (int i = 0; i < text.Length + 4; i++)
            {
                Console.Write("*");
            }
            Console.ResetColor();
            Console.WriteLine();
        }

        static void Main()
        {
            OutputWithFrame("Välkommen till Garaget 2.0");
            try
            {
                Menu.Show(_initMenu);
            }
            catch (IOException)
            {
                Console.WriteLine("Programmet avslutas på grund av slut på indata eller ett I/O-fel.");
            }
            catch (ExitException)
            {
                Console.WriteLine("Programmet avslutas på begäran.");
            }
        }
    }

    // Exception att kasta för att avsluta programmet.
    class ExitException : Exception { }
}
