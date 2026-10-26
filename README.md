# Review av Garaget 2.0

Menyn hanteras av klassen `Menu`, som innehåller en lista av `MenuItem`:s.
`MenuItem` är en struct som förutom texten att visa och tecken för att välja menyvalet
även innehåller en referens till en funktion som ska utföras när detta menyval väljs.
Denna funktion kan returnera en referens till en ny meny att visa efteråt,
om inte samma meny ska visas igen.
`MenuItem` innehåller även en funktion som ska returnera `true` eller `false` beroende på
om menyvalet är tillgängligt att välja eller inte.

`Menu.Show()` visar menyns alternativ och väntar på att användaren väljer ett menyval.
När användaren har valt ett menyval utförs motsvarande funktion
och efteråt visas samma eller nästa meny, beroende på om funktionen returnerar en meny eller inte.
Menyn loopas tills ett exception kastas.

Det finns två instanser av `Menu` i programmet,
en för att hantera flera garage, och en för att hantera ett enskilt garage.

Hanteringen av flera garage utförs av klassen `GarageList`.
Den har bland annat metoder för att skapa och ta bort garagen via användarinput.

Ett enskilt garage hanteras av klassen `Garage`.
I `Garage` finns bland annat fältet `_vehicles` som är en lista över parkerade fordon,
egenskapen `Capacity` som anger hur många platser det finns i garaget,
`FreeCapacity` som anger hur många platser som är lediga,
`Count` som anger hur många fordon som är parkerade,
och
`AllowedVehicles` som anger vilka fordonstyper som är tillåtna i garaget.

`Garage` har bland annat metoder för att lista garage, söka i garage,
lägga till eller ta bort fordon och lägga till slumpmässiga fordon.
För flera av funktionerna finns det en särskild metod för användarinput,
dessa metoder har inga inparametrar,
och en motsvarande metod med inparametrar som anropas av den andra metoden
när användarens input har lästs in.

Hur `Vehicle` och dess underklasser fungerar var ju tämligen strikt bestämt
i uppgiften, så jag tror inte jag behöver nämna så mycket om det.

Jag har ett par egna collections:
`StringDictionary` en dictionary från strängar till mängder av strängar,
och `StringSet`, en mängd av strängar.
Dessa använda främst för att hantera sökvärden.

För att starta programmet kan man skriva `dotnet run`.

## Särskilda saker ni kan ge feedback på

* I några fall använder jag mig av try-catch-block för felhantering,
genom att låta property-setters kontrollera om värdena är korrekta,
och om det blir ett exception skriver jag ut felmeddelandet från detta exception.
Tycker ni att det istället hade varit bättra att kontrollera värdena innan
jag sätter dem? Då hade jag sluppit använda try-catch-block.

* En sak som jag själv inte riktigt gillar i min lösning är hur jag har implementerat
`GetPropertyValue()` i fordonsklasserna. Jag har gjort det med try-catch-block, vilket
jag gärna hade undvikit.

* Jag kommer för tillfället inte på så mycket mer att fråga efter feedback om.

# Ursprunglig Readme-text följer nedan:

# Garaget 2.0

Detta är en lösning på en övningsuppgift under en kurs i .NET fullstackutveckling hos Lexicon.
Den följande texten är baserad på den ursprungliga uppgiftslydelsen.

Applikationen ska tillhandahålla den funktionalitet 
ett system kan behöva för att simulera ett enkelt garage.
Det ska alltså gå att parkera fordon, hämta ut fordon,
se vilka fordon som finns i garaget och vilka egenskaper de har. 
Allt detta ska ske i en konsolapplikation med huvudmeny och undermenyer.

Ett garage kan huvudsakligen delas upp i följande delar:
- Garaget: En representation av själva byggnaden.
  Garaget är en plats där ett antal fordon kan förvaras. 
  Garaget kan därför representeras som en samling av fordon.
- Fordon: Bilar, motorcyklar, enhjulingar eller andra typer av fordon som kan ställas in i garaget.
  Detta är de två huvudsakliga “objekttyperna” i ett fysiskt garage.
  Varje specifik fordonstyp ska vara en subklass till den generella fordonsklassen.

Utöver detta krävs funktionalitet som hanterar att fordon ställs in i garaget, 
att fordon kan tas ut ur garaget samt att vi kan presentera och söka bland fordonen.

I mer programmeringsvänliga termer ska det som minimum finnas:
- En kollektion av fordon: klassen Garage.
- En fordonsklass: klassen Vehicle.
- Ett antal subklasser till fordon.
- Ett användargränssnitt som låter användaren använda garagefunktionerna. 
  Här sker all interaktion med användaren.

## Kravspecifikation

Fordonen ska implementeras som klassen Vehicle och subklasser till den.

- Vehicle innehåller samtliga egenskaper som ska finnas i samtliga fordonstyper, 
  till exempel registreringsnummer, färg, antal hjul och andra egenskaper ni kan komma på.
- Registreringsnumret ska vara unikt.
  Det måste kontrolleras att registreringsnumret är skrivet på ett korrekt format.
- Det måste minst finnas följande subklasser:
	- Motorcycle
	- Airplane
	- Car
	- Bus
	- Boat

	Dessa subklasser ska implementera minst en egen egenskap var, till exempel:
	- Number of Engines
	- Cylinder volume
	- Fuel type (Gasoline/Diesel)
	- Number of seats
	- Length

Samlingen av fordon ska internt i klassen Garage hanteras som en List\<Vehicle\>. 
Listan ska vara privat. 
Vid instansieringen av ett nytt garage måste kapaciteten anges som argument till konstruktorn.

Man får inte använda:
- En array internt i Garage-klassen.
- Generics, exempelvis Garage\<T\> where T : Vehicle.
- LINQ.
- IEnumerable\<T\>.

## Funktionalitet

Det ska gå att:
- Lista samtliga parkerade fordon.
- Lista fordonstyper och hur många av varje typ som står i garaget.
- Lägga till och ta bort fordon ur garaget.
- Sätta en kapacitet, alltså antal parkeringsplatser, vid instansieringen av ett nytt garage.
- Möjlighet att populera garaget med ett antal fordon från start.
- Hitta ett specifikt fordon via registreringsnumret.
  Sökningen ska fungera oavsett om användaren skriver ABC123, Abc123 eller AbC123.
- Söka efter fordon utifrån en eller flera egenskaper från basklassen Vehicle. Till exempel:
	- Alla svarta fordon med fyra hjul.
	- Alla motorcyklar som är rosa och har tre hjul.
	- Alla lastbilar.
	- Alla röda fordon.

Användaren ska få tydlig feedback på om något gått bra eller dåligt. 
Till exempel ska användaren få bekräftelse när ett fordon har parkerats. 
Om det inte går ska användaren få veta varför.

## Användargränssnitt

Programmet ska vara en konsolapplikation med ett textbaserat användargränssnitt.
Från gränssnittet ska det gå att:
- Navigera till all funktionalitet i garaget.
- Skapa ett garage med en användarspecificerad storlek.
- Stänga av applikationen.

Applikationen ska hantera felaktiga indata på ett robust sätt,
så att den inte kraschar vid felaktig inmatning eller användning.

## Extra funktionalitet som har implementerats

- Möjlighet att även söka på de fordonsspecifika egenskaperna.
- Hantera flera garage med olika typer av fordon, till exempel en hangar, 
  ett vanligt garage och ett motorcykelgarage.
- Möjlighet att växla mellan olika garage i användargränssnittet, 
  så att tidigare nämnda funktioner bara utförs på det garage som för tillfället är valt.
- Tydlig visning av vilket garage användaren för närvarande arbetar med.
