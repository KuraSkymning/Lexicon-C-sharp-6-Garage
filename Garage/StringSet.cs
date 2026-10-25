using System.Collections.Generic;

namespace Garage
{
    // En mängd av strängar.
    // Klassen är ineffektiv eftersom kraven anger att man inte får använda
    // någon annan collection än List<T> och man inte får använda IEnumerable<T>.
    public class StringSet
    {
        private readonly List<string> _values = new(8);

        // Hämta antalet strängvärden i mängden.
        public int Count => _values.Count;

        // Lägg till ett strängvärde i mängden.
        public void Add(string value)
        {
            if (!Contains(value))
            {
                _values.Add(value);
            }
        }

        // Kontrollera om mängden innehåller ett visst strängvärde.
        public bool Contains(string value)
        {
            return _values.IndexOf(value) >= 0;
        }

        // Hämta alla strängvärden i mängden.
        // Om det have varit tillåtet att använda IEnumerable<T>
        // hade det varit bättre att returnera en IEnumerable<string> istället.
        // Man hade också kunnat implementera en indexer istället,
        // men det är oklart huruvida det är tillåtet.
        public List<string> Values()
        {
            return [.. _values];
        }

        public override string ToString()
        {
            return string.Join(", ", _values);
        }
    }
}
