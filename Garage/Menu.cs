using System;
using System.Collections.Generic;
using System.IO;

namespace Garage
{
    // En meny med menyval.
    // header är en funktion som skriver ut menyns rubrik och
    // eventuell annan information före menyvalen.
    // items är en lista med menyval som ska visas i menyn.
    class Menu(Action header, List<MenuItem> items)
    {
        private readonly Action _header = header;

        private readonly List<MenuItem> _items = items;

        // Visa menyn och hantera användarens menyval.
        // Loopar tills ett exception kastas.
        public static void Show(Menu menu)
        {
            while (true)
            {
                Console.WriteLine();
                menu._header();
                Console.WriteLine();

                for (int i = 0; i < menu._items.Count; i++)
                {
                    MenuItem item = menu._items[i];
                    if (item.isAvailable())
                    {
                        Console.WriteLine(item.label + ". " + item.title);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("(" + item.label + ". " + item.title + ")");
                        Console.ResetColor();
                    }
                }

                bool ok = false;
                while (!ok)
                {
                    Console.WriteLine();
                    Console.Write("Ange ett menyval: ");
                    string s = Console.ReadLine()?.Trim() ?? throw new IOException();
                    if (s == "")
                    {
                        break;
                    }

                    Console.WriteLine();

                    for (int i = 0; i < menu._items.Count; i++)
                    {
                        MenuItem item = menu._items[i];
                        if (s == item.label)
                        {
                            ok = true;

                            Console.WriteLine("Du valde: " + item.title);
                            if (!item.isAvailable())
                            {
                                Console.WriteLine("Detta menyval är inte tillgängligt.");
                            }
                            else
                            {
                                Menu? m = item.action();
                                if (m != null)
                                {
                                    menu = m;
                                }
                            }

                            Console.WriteLine();
                            Console.WriteLine("Tryck Enter för att fortsätta.");
                            Console.ReadLine();
                            break;
                        }
                    }

                    if (!ok)
                    {
                        Console.WriteLine("Du angav ett ogiltigt menyval.");
                    }
                }
            }
        }
    }

    // Ett menyval i en meny.
    internal readonly struct MenuItem(string label, string title, Func<Menu?> action, Func<bool>? isAvailable = null)
    {
        // label är det som användaren ska skriva in för att välja detta menyval.
        public readonly string label = label;

        // title är texten som visas i menyn för detta menyval.
        public readonly string title = title;

        // action är en funktion som utförs när användaren väljer detta menyval.
        // Den kan returnera en ny meny som ska visas efteråt, eller null om samma meny ska visas igen.
        public readonly Func<Menu?> action = action;

        // isAvailable är en funktion som ska returnera true om menyvalet ska vara tillgängligt.
        public readonly Func<bool> isAvailable = isAvailable ?? (() => true);
    }
}
