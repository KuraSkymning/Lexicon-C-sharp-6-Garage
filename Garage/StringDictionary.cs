using System.Collections.Generic;

namespace Garage
{
    // En nyckel-värde-samling från strängar till mängder av strängar.
    // Klassen är ineffektiv eftersom kraven anger att man inte får använda
    // någon annan collection än List<T> och man inte får använda IEnumerable<T>.
    public class StringDictionary
    {
        private readonly List<StringDictionaryEntry> _entries = new(8);

        // Hämta antalet nyckel-värde-par i samlingen.
        public int Count => _entries.Count;

        // Lägg till en nyckel och ett strängvärde.
        // Returnerar mängden av strängar som är kopplad till nyckeln.
        public StringSet Add(string key, string value)
        {
            StringSet stringset = Add(key);
            stringset.Add(value);
            return stringset;
        }

        // Lägg till en nyckel.
        // Returnerar mängden av strängar som är kopplad till nyckeln.
        public StringSet Add(string key)
        {
            StringSet? stringset = Get(key);
            if (stringset == null)
            {
                stringset = new StringSet();
                StringDictionaryEntry entry = new(key, stringset);
                _entries.Add(entry);
            }
            return stringset;
        }

        // Hämta mängden av strängar som är kopplad till nyckeln.
        public StringSet? Get(string key)
        {
            for (int i = 0; i < _entries.Count; i++)
            {
                if (_entries[i].key == key)
                {
                    return _entries[i].value;
                }
            }
            return null;
        }

        // Hämta alla nycklar i samlingen.
        // Om det hade varit tillåtet att använda IEnumerable<T>
        // hade det varit bättre att returnera en IEnumerable<string> istället.
        // Man hade också kunnat implementera en indexer istället,
        // men det är oklart huruvida det är tillåtet.
        public List<string> Keys()
        {
            List<string> keys = new(_entries.Count);
            for (int i = 0; i < _entries.Count; i++)
            {
                keys.Add(_entries[i].key);
            }
            return keys;
        }

        public override string ToString()
        {
            string result = "";
            for (int i = 0; i < _entries.Count; i++)
            {
                result += $"- {_entries[i].key}: {_entries[i].value}\n";
            }
            return result;
        }
    }

    // En post i StringDictionary.
    internal readonly struct StringDictionaryEntry(string key, StringSet value)
    {
        internal readonly string key = key;
        internal readonly StringSet value = value;
    }
}
