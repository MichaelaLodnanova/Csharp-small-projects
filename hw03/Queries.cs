using PV178.Homeworks.HW03.DataLoading.DataContext;
using PV178.Homeworks.HW03.DataLoading.Factory;
using PV178.Homeworks.HW03.Model;
using PV178.Homeworks.HW03.Model.Enums;
using System.Text;

namespace PV178.Homeworks.HW03
{
    public class Queries
    {
        private IDataContext? _dataContext;
        public IDataContext DataContext => _dataContext ??= new DataContextFactory().CreateDataContext();

        /// <summary>
        /// Ukážkové query na pripomenutie základnej LINQ syntaxe a operátorov. Výsledky nie je nutné vracať
        /// pomocou jedného príkazu, pri zložitejších queries je vhodné si vytvoriť pomocné premenné cez `var`.
        /// Toto query nie je súčasťou hodnotenia.
        /// </summary>
        /// <returns>The query result</returns>
        public int SampleQuery()
        {
            return DataContext.Countries
                .Where(a => a.Name?[0] >= 'A' && a.Name?[0] <= 'G')
                .Join(DataContext.SharkAttacks,
                    country => country.Id,
                    attack => attack.CountryId,
                    (country, attack) => new { country, attack }
                )
                .Join(DataContext.AttackedPeople,
                    ca => ca.attack.AttackedPersonId,
                    person => person.Id,
                    (ca, person) => new { ca, person }
                )
                .Where(p => p.person.Sex == Sex.Male)
                .Count(a => a.person.Age >= 15 && a.person.Age <= 40);
        }

        /// <summary>
        /// Úloha č. 1
        ///
        /// Vráťte zoznam, v ktorom je textová informácia o každom človeku,
        /// na ktorého v štáte Bahamas zaútočil žralok s latinským názvom začínajúcim sa 
        /// na písmeno I alebo N.
        /// 
        /// Túto informáciu uveďte v tvare:
        /// "{meno človeka} was attacked in Bahamas by {latinský názov žraloka}"
        /// </summary>
        /// <returns>The query result</returns>
        public List<string> InfoAboutPeopleThatNamesStartsWithCAndWasInBahamasQuery()
        {
            return DataContext.SharkAttacks
                .Join(DataContext.SharkSpecies,
                    attack => attack.SharkSpeciesId,
                    species => species.Id,
                    (attack, species) => new { attack, species.LatinName }
                )
                .Join(DataContext.AttackedPeople,
                    atSp => atSp.attack.AttackedPersonId,
                    attacked => attacked.Id,
                    (atSp, attacked) => new { atSp, attacked }
                )
                .Join(DataContext.Countries,
                    atspA => atspA.atSp.attack.CountryId,
                    country => country.Id,
                    (atspA, country) => new { atspA, country })
                .Where(c => c.country.Name == "Bahamas"
                && (c.atspA.atSp.LatinName.StartsWith('I')
                || c.atspA.atSp.LatinName.StartsWith('N')))
                .Select(p => $"{p.atspA.attacked.Name} was attacked in Bahamas by {p.atspA.atSp.LatinName}").ToList();
        }

        /// <summary>
        /// Úloha č. 2
        ///
        /// Firma by chcela expandovať do krajín s nedemokratickou formou vlády – monarchie alebo teritória.
        /// Pre účely propagačnej kampane by chcela ukázať, že žraloky v týchto krajinách na ľudí neútočia
        /// s úmyslom zabiť, ale chcú sa s nimi iba hrať.
        /// 
        /// Vráťte súhrnný počet žraločích utokov, pri ktorých nebolo preukázané, že skončili fatálne.
        /// 
        /// Požadovany súčet vykonajte iba pre krajiny s vládnou formou typu 'Monarchy' alebo 'Territory'.
        /// </summary>
        /// <returns>The query result</returns>
        public int FortunateSharkAttacksSumWithinMonarchyOrTerritoryQuery()
        {
            return DataContext.SharkAttacks
                .Where(a => a.AttackSeverenity != AttackSeverenity.Fatal)
                .Join(DataContext.Countries,
                    attackCountry => attackCountry.CountryId,
                    country => country.Id,
                    (attackedCountry, country) => new { attackedCountry, country.GovernmentForm })
                .Count(c => c.GovernmentForm == GovernmentForm.Monarchy || c.GovernmentForm == GovernmentForm.Territory);
        }

        /// <summary>
        /// Úloha č. 3
        ///
        /// Marketingovému oddeleniu dochádzajú nápady ako pomenovávať nové produkty.
        /// 
        /// Inšpirovať sa chcú prezývkami žralokov, ktorí majú na svedomí najviac
        /// útokov v krajinách na kontinente 'South America'. Pre pochopenie potrebujú 
        /// tieto informácie vo formáte slovníku:
        /// 
        /// (názov krajiny) -> (prezývka žraloka s najviac útokmi v danej krajine)
        /// </summary>
        /// <returns>The query result</returns>
        public Dictionary<string, string> MostProlificNicknamesInCountriesQuery()
        {
            Dictionary<string, string> d = DataContext.Countries
                .Where(c => c.Continent == "South America")
                .Join(DataContext.SharkAttacks,
                    country => country.Id,
                    sAttack => sAttack.CountryId,
                    (country, sAttack) => new { country, sAttack })
                .Join(DataContext.SharkSpecies,
                    sac => sac.sAttack.SharkSpeciesId,
                    species => species.Id,
                    (sac, species) => new { Country = sac.country.Name ?? "", AlsoKnownAs = species.AlsoKnownAs ?? "" })
                .Where(cn => !string.IsNullOrEmpty(cn.AlsoKnownAs))
                .GroupBy(x => x.Country)
                .Select(g => new
                {
                    Country = g.Key,
                    AlsoKnownAs = g.GroupBy(x => x.AlsoKnownAs)
                        .OrderByDescending(x => x.Count()).First().Key
                })
                .ToDictionary(x => x.Country, x => x.AlsoKnownAs);
            return d;

        }

        /// <summary>
        /// Úloha č. 4
        ///
        /// Firma chce začať kompenzačnú kampaň a potrebuje k tomu dáta.
        ///
        /// Preto zistite, ktoré žraloky útočia najviac na mužov. 
        /// Vráťte ID prvých troch žralokov, zoradených zostupne podľa počtu útokov na mužoch.
        /// </summary>
        /// <returns>The query result</returns>
        public List<int> ThreeSharksOrderedByNumberOfAttacksOnMenQuery()
        {
            return DataContext.SharkAttacks
                   .Join(DataContext.AttackedPeople,
                        attack => attack.AttackedPersonId,
                        person => person.Id,
                        (attack, person) => new { attack.SharkSpeciesId, person })
                    .Join(DataContext.SharkSpecies,
                        attack => attack.SharkSpeciesId,
                        species => species.Id,
                        (attack, species) => new { attack.SharkSpeciesId, attack.person, species })
                    .GroupBy(x => x.species.Id)
                    .OrderByDescending(g => g.Count(x => x.person.Sex == Sex.Male))
                    .Select(g => g.Key)
                    .Take(3).ToList();
        }

        /// <summary>
        /// Úloha č. 5
        ///
        /// Oslovila nás medzinárodná plavecká organizácia. Chce svojich plavcov motivovať možnosťou
        /// úteku pred útokom žraloka.
        ///
        /// Potrebuje preto informácie o priemernej rýchlosti žralokov, ktorí
        /// útočili na plávajúcich ľudí (informácie o aktivite počas útoku obsahuje "Swimming" alebo "swimming").
        /// 
        /// Pozor, dáta požadajú oddeliť podľa jednotlivých kontinentov. Ignorujte útoky takých druhov žralokov,
        /// u ktorých nie je známa maximálná rýchlosť. Priemerné rýchlosti budú zaokrúhlené na dve desatinné miesta.
        /// </summary>
        /// <returns>The query result</returns>
        public Dictionary<string, double> SwimmerAttacksSharkAverageSpeedQuery()
        {
            return DataContext.SharkAttacks
                .Where(x => (x.Activity?.Contains("Swimming") ?? false) ||
                        (x.Activity?.Contains("swimming") ?? false))
                .Join(DataContext.Countries,
                    attack => attack.CountryId,
                    country => country.Id,
                    (attack, country) => new { Attack = attack, Country = country })
                .Where(x => x.Country.Continent != null)
                .Join(DataContext.SharkSpecies.Where(x => x.TopSpeed != null),
                    attack => attack.Attack.SharkSpeciesId,
                    species => species.Id,
                    (attack, species) => new { attack.Country, Species = species })
       
                .GroupBy(x => x.Country.Continent)
                .ToDictionary(g => g.Key!, g => Math.Round(g.Average(g => g.Species.TopSpeed ?? 0), 2));
        }

        /// <summary>
        /// Úloha č. 6
        ///
        /// Zistite všetky nefatálne (AttackSeverenity.NonFatal) útoky spôsobené pri člnkovaní 
        /// (AttackType.Boating), ktoré mal na vine žralok s prezývkou "Zambesi shark".
        /// Do výsledku počítajte iba útoky z obdobia po 3. 3. 1960 (vrátane) a ľudí,
        /// ktorých meno začína na písmeno z intervalu <D, K> (tiež vrátane).
        /// 
        /// Výsledný zoznam mien zoraďte abecedne.
        /// </summary>
        /// <returns>The query result</returns>
        public List<string> NonFatalAttemptOfZambeziSharkOnPeopleBetweenDAndKQuery()
        {
            DateTime startDate = new DateTime(1960, 3, 3);
            return DataContext.SharkAttacks
                .Where(attack => attack.AttackSeverenity == AttackSeverenity.NonFatal
                        && attack.Type == AttackType.Boating
                        && attack.DateTime >= startDate)
                .Join(DataContext.SharkSpecies,
                        attack => attack.SharkSpeciesId,
                        shark => shark.Id,
                        (attack, shark) => new {attack.AttackedPersonId, AlsoKnownAs = shark.AlsoKnownAs ?? "" })
                .Where(shark =>!string.IsNullOrEmpty(shark.AlsoKnownAs) && shark.AlsoKnownAs == "Zambesi shark")
                .Join(DataContext.AttackedPeople,
                        attack => attack.AttackedPersonId,
                        person => person.Id,
                        (attack, person) => new {attack, person.Name})
                .Where(person => !string.IsNullOrEmpty(person.Name))
                .Where(p => !string.IsNullOrEmpty(p.Name) && p.Name.StartsWith("D") || p.Name.StartsWith("E") || p.Name.StartsWith("F")
                    || p.Name.StartsWith("G") || p.Name.StartsWith("H") || p.Name.StartsWith("I")
                    || p.Name.StartsWith("J") || p.Name.StartsWith("K"))
                .Select(p => p.Name)
                .OrderBy(n => n)
                .ToList();
        }

        /// <summary>
        /// Úloha č. 7
        ///
        /// Zistilo sa, že desať najľahších žralokov sa správalo veľmi podozrivo počas útokov v štátoch Južnej Ameriky.
        /// 
        /// Vráťte preto zoznam dvojíc, kde pre každý štát z Južnej Ameriky bude uvedený zoznam žralokov,
        /// ktorí v tom štáte útočili. V tomto zozname môžu figurovať len vyššie spomínaných desať najľahších žralokov.
        /// 
        /// Pokiaľ v nejakom štáte neútočil žiaden z najľahších žralokov, zoznam žralokov u takého štátu bude prázdny.
        /// </summary>
        /// <returns>The query result</returns>
        public List<Tuple<string, List<SharkSpecies>>> LightestSharksInSouthAmericaQuery()
        {
            var lightSharks = DataContext.SharkSpecies
                .OrderBy(s => s.Weight)
                .Take(10);
            return DataContext.Countries
                .GroupJoin(DataContext.SharkAttacks,
                country => country.Id,
                attack => attack.CountryId,
                (country, attacks) => new { country, attacks })
                .Where(c => !string.IsNullOrEmpty(c.country.Name) && c.country.Continent == "South America")
                .Select(o => Tuple.Create
                (
                    o.country.Name ?? "",
                    o.attacks
                        .Join(lightSharks,
                        attack => attack.SharkSpeciesId,
                        shark => shark.Id,
                        (attack, shark) => shark).DistinctBy(shark => shark.Id).ToList()
                )).ToList();
        }

        /// <summary>
        /// Úloha č. 8
        ///
        /// Napísať hocijaký LINQ dotaz musí byť pre Vás už triviálne. Riaditeľ firmy vás preto chce
        /// využiť na testovanie svojich šialených hypotéz.
        /// 
        /// Zistite, či každý žralok, ktorý má maximálnu rýchlosť aspoň 56 km/h zaútočil aspoň raz na
        /// človeka, ktorý mal viac ako 56 rokov. Výsledok reprezentujte ako pravdivostnú hodnotu.
        /// </summary>
        /// <returns>The query result</returns>
        public bool FiftySixMaxSpeedAndAgeQuery()
        {
            var lightSharks = DataContext.SharkSpecies
                .Where(s => s.TopSpeed >= 56);
            var oldPeopleAttack = DataContext.AttackedPeople
                .Join(DataContext.SharkAttacks,
                person => person.Id,
                attack => attack.AttackedPersonId,
                (person, attack) => new { person, attack })
                .Where(o => o.person.Age > 56);
            var sharkAttacks = lightSharks.GroupJoin(oldPeopleAttack,
                shark => shark.Id,
                attack => attack.attack.SharkSpeciesId,
                (shark, attacks) => new {shark, AttackCount = attacks.Count()}).All(o => o.AttackCount > 0);
            
            return sharkAttacks;
        }

        /// <summary>
        /// Úloha č. 9
        ///
        /// Ohromili ste svojim výkonom natoľko, že si od Vás objednali rovno textové výpisy.
        /// Samozrejme, že sa to dá zvladnúť pomocou LINQ.
        /// 
        /// Chcú, aby ste pre všetky fatálne útoky v štátoch začínajúcich na písmeno 'B' alebo 'R' urobili výpis v podobe: 
        /// "{Meno obete} was attacked in {názov štátu} by {latinský názov žraloka}"
        /// 
        /// Záznam, u ktorého obeť nemá meno
        /// (údaj neexistuje, nejde o vlastné meno začínajúce na veľké písmeno, či údaj začína číslovkou)
        /// do výsledku nezaraďujte. Získané pole zoraďte abecedne a vraťte prvých 5 viet.
        /// </summary>
        /// <returns>The query result</returns>
        
        private bool HasValidName(string? name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }
            if (char.IsDigit(name[0]))
            {
                return false;
            }
            if (!char.IsUpper(name[0]))
            {
                return false;
            }
            return true;
        }
        public List<string> InfoAboutPeopleAndCountriesOnBorRAndFatalAttacksQuery()
        {
            var people = DataContext.AttackedPeople
                .Where(person => HasValidName(person.Name));
            var countries = DataContext.Countries
                .Where(country => !string.IsNullOrEmpty(country.Name) && (country.Name.StartsWith('B') || country.Name.StartsWith('R')));
            var query = DataContext.SharkAttacks
                .Where(a => a.AttackSeverenity == AttackSeverenity.Fatal)
                .Join(people,
                    attack => attack.AttackedPersonId,
                    person => person.Id,
                    (attack, person) => new { attack, person })
                .Where(a => a.attack.AttackSeverenity == AttackSeverenity.Fatal)
                .Join(countries,
                    personAttack => personAttack.attack.CountryId,
                    country => country.Id,
                    (personAttack, country) => new { Attack = personAttack.attack, Person = personAttack.person, country })
                .Join(DataContext.SharkSpecies,
                    o => o.Attack.SharkSpeciesId,
                    shark => shark.Id,
                    (o, shark) => new { Person = o.Person, Country = o.country, shark })
                .OrderBy(o => o.Person.Name)
                .Take(5);
            
            List<string> info = new List<string>();
            foreach (var data in query)
            {
                info.Add($"{data.Person.Name} was attacked in {data.Country.Name} by {data.shark.LatinName}");
            }
            return info;
        }

        /// <summary>
        /// Úloha č. 10
        ///
        /// Nedávno vyšiel zákon, že každá krajina Európy začínajúca na písmeno A až L (vrátane)
        /// musí zaplatiť pokutu 250 jednotiek svojej meny za každý žraločí útok na ich území.
        /// Pokiaľ bol tento útok smrteľný, musia dokonca zaplatiť 300 jednotiek. Ak sa nezachovali
        /// údaje o tom, či bol daný útok smrteľný alebo nie, nemusia platiť nič.
        /// Áno, tento zákon je spravodlivý...
        /// 
        /// Vráťte informácie o výške pokuty európskych krajín začínajúcich na A až L (vrátane).
        /// Tieto informácie zoraďte zostupne podľa počtu peňazí, ktoré musia tieto krajiny zaplatiť.
        /// Vo finále vráťte 5 záznamov s najvyššou hodnotou pokuty.
        /// 
        /// V nasledujúcej sekcii môžete vidieť príklad výstupu v prípade, keby na Slovensku boli 2 smrteľné útoky,
        /// v Česku jeden fatálny + jeden nefatálny a v Maďarsku žiadny:
        /// <code>
        /// Slovakia: 600 EUR
        /// Czech Republic: 550 CZK
        /// Hungary: 0 HUF
        /// </code>
        /// 
        /// </summary>
        /// <returns>The query result</returns>
        
        private int Penalty(SharkAttack attack)
        {
            switch (attack.AttackSeverenity)
            {
                case AttackSeverenity.Fatal:
                    return 300;
                case AttackSeverenity.NonFatal:
                    return 250;
                default:
                    return 0;
            }
        }

        public List<string> InfoAboutFinesInEuropeQuery()
        {
            return DataContext.Countries
                .Where(a => a.Name?[0] >= 'A' && a.Name?[0] <= 'L')
                .Where(c => c.Continent == "Europe")
                .GroupJoin(DataContext.SharkAttacks,
                    country => country.Id,
                    attack => attack.CountryId,
                    (country, attacks) => new { country, Penalty = attacks.Aggregate(
                        0, (acc, a) => acc + Penalty(a)) })
                    .OrderByDescending(o => o.Penalty)
                    .Take(5)
                    .Select(o => $"{o.country.Name}: {o.Penalty} {o.country.CurrencyCode}").ToList();
        }

        /// <summary>
        /// Úloha č. 11
        ///
        /// Organizácia spojených žraločích národov výhlásila súťaž: 5 druhov žralokov, 
        /// ktoré sú najviac agresívne získa hodnotné ceny.
        /// 
        /// Nájdite 5 žraločích druhov, ktoré majú na svedomí najviac ľudských životov,
        /// druhy zoraďte podľa počtu obetí.
        ///
        /// Výsledok vráťte vo forme slovníku, kde
        /// kľúčom je meno žraločieho druhu a
        /// hodnotou je súhrnný počet obetí spôsobený daným druhom žraloka.
        /// </summary>
        /// <returns>The query result</returns>
        public Dictionary<string, int> FiveSharkNamesWithMostFatalitiesQuery()
        {
            return
                DataContext.SharkAttacks.Where(attack => attack.AttackSeverenity == AttackSeverenity.Fatal)
                    .Join(DataContext.SharkSpecies,
                            attack => attack.SharkSpeciesId,
                            shark => shark.Id,  
                            (attack, shark) => new { shark.Name, attack.AttackedPersonId})
                    .GroupBy(shark => shark.Name, 
                        (Key, items) => Tuple.Create(Key, items.Count())
                    )
                    .OrderByDescending(c => c.Item2)
                    .Take(5)
                    .ToDictionary(o => o.Item1 ?? "", o => o.Item2);
        }

        /// <summary>
        /// Úloha č. 12
        ///
        /// Riaditeľ firmy si chce podmaňiť čo najviac krajín na svete. Chce preto zistiť,
        /// na aký druh vlády sa má zamerať, aby prebral čo najviac krajín.
        /// 
        /// Preto od Vás chce, aby ste mu pomohli zistiť, aké percentuálne zastúpenie majú jednotlivé typy vlád.
        /// Požaduje to ako jeden string:
        /// "{1. typ vlády}: {jej percentuálne zastúpenie}%, {2. typ vlády}: {jej percentuálne zastúpenie}%, ...".
        /// 
        /// Výstup je potrebný mať zoradený od najväčších percent po najmenšie,
        /// pričom percentá riaditeľ vyžaduje zaokrúhľovať na jedno desatinné miesto.
        /// Pre zlúčenie musíte podľa jeho slov použiť metódu `Aggregate`.
        /// </summary>
        /// <returns>The query result</returns>
        public string StatisticsAboutGovernmentsQuery()
        {
            var governments = DataContext.Countries
                .Select(c => c.GovernmentForm).Distinct();
            var allCountries = DataContext.Countries.Count();
            var query = governments.
                GroupJoin(DataContext.Countries,
                    government => government,
                    country => country.GovernmentForm,
                    (government, countries) => new { government, percent = (decimal)countries.Count() * 100 / allCountries})
                .OrderByDescending(q => q.percent);
            string result = query.Aggregate("", (acc, o) => acc + $"{o.government}: {o.percent:F1}%, ");
            return result.Substring(0, result.Length - 2);
        }

        /// <summary>
        /// Úloha č. 13
        ///
        /// Firma zistila, že výrobky s tigrovaným vzorom sú veľmi populárne. Chce to preto aplikovať
        /// na svoju doménu.
        ///
        /// Nájdite informácie o ľudoch, ktorí boli obeťami útoku žraloka s menom "Tiger shark"
        /// a útok sa odohral v roku 2001.
        /// Výpis majte vo formáte:
        /// "{meno obete} was tiggered in {krajina, kde sa útok odohral}".
        /// V prípade, že chýba informácia o krajine útoku, uveďte namiesto názvu krajiny reťazec "Unknown country".
        /// V prípade, že informácie o obete vôbec neexistuje, útok ignorujte.
        ///
        /// Ale pozor! Váš nový nadriadený má panický strach z operácie `Join` alebo `GroupJoin`.
        /// Informácie musíte zistiť bez spojenia hocijakých dvoch tabuliek. Skúste sa napríklad zamyslieť,
        /// či by vám pomohla metóda `Zip`.
        /// </summary>
        /// <returns>The query result</returns>
        public List<string> TigerSharkAttackZipQuery()
        {
            var tigerShark = DataContext.SharkSpecies
                .Where(s => s.Name == "Tiger shark").First();
            var attacks = DataContext.SharkAttacks.
                Where(a => a.DateTime.Value.Year == 2001 && a.SharkSpeciesId == tigerShark.Id)
                .Select(a => Enumerable.Repeat(a, DataContext.Countries.Count()).Zip(DataContext.Countries))
                .SelectMany(a => a.Where(attack => attack.First.CountryId == attack.Second.Id || attack.First.CountryId == null));
            return DataContext.AttackedPeople
                .Select(p => Enumerable.Repeat(p, attacks.Count()).Zip(attacks))
                .SelectMany(p => p.Where(r => r.First.Id == r.Second.First.AttackedPersonId))
                .Select(o => $"{o.First.Name} was tiggered in {(o.Second.First.CountryId == null ? "Unknown country" : o.Second.Second.Name )}").Distinct().ToList();
        }

        /// <summary>
        /// Úloha č. 14
        ///
        /// Vedúci oddelenia prišiel s ďalšou zaujímavou hypotézou. Myslí si, že veľkosť žraloka nejako 
        /// súvisí s jeho apetítom na ľudí.
        ///
        /// Zistite pre neho údaj, koľko percent útokov má na svedomí najväčší a koľko najmenší žralok.
        /// Veľkosť v tomto prípade uvažujeme podľa dĺžky.
        /// 
        /// Výstup vytvorte vo formáte: "{percentuálne zastúpenie najväčšieho}% vs {percentuálne zastúpenie najmenšieho}%"
        /// Percentuálne zastúpenie zaokrúhlite na jedno desatinné miesto.
        /// </summary>
        /// <returns>The query result</returns>
        public string LongestVsShortestSharkQuery()
        {
            var q = DataContext.SharkSpecies
                .OrderByDescending(s => s.Length);
            var longest = q.First();
            var shortest = q.Last();
            var longestAttacks = DataContext.SharkAttacks
                .Where(a => a.SharkSpeciesId == longest.Id).Count();
            var shortestAttacks = DataContext.SharkAttacks
                .Where(a => a.SharkSpeciesId == shortest.Id).Count();
            return $"{((double)longestAttacks * 100) / DataContext.SharkAttacks.Count():F1}% vs {(double)shortestAttacks * 100 / DataContext.SharkAttacks.Count():F1}%";

        }

        /// <summary>
        /// Úloha č. 15
        ///
        /// Na koniec vašej kariéry Vám chceme všetci poďakovať a pripomenúť Vám vašu mlčanlivosť.
        /// 
        /// Ako výstup požadujeme počet krajín, v ktorých žralok nespôsobil smrť (útok nebol fatálny).
        /// Berte do úvahy aj tie krajiny, kde žralok vôbec neútočil.
        /// </summary>
        /// <returns>The query result</returns>
        public int SafeCountriesQuery()
        {
            var allCountries = DataContext.Countries.Select(c => c.Name).ToHashSet();
            var attackedCountries = DataContext.SharkAttacks
                    .Join(DataContext.Countries,
                        attack => attack.CountryId,
                        country => country.Id,
                        (attack, country) => new { attack, country })
                    .ToHashSet();
            var countriesFatalAttack = attackedCountries
                    .Where(attack => attack.attack.AttackSeverenity == AttackSeverenity.Fatal)
                    .Select(c => c.country.Name).ToHashSet();
            return allCountries.Except(countriesFatalAttack).Count();
        }
    }
}
