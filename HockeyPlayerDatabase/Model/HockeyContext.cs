using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using HockeyPlayerDatabase.Interfaces;


namespace HockeyPlayerDatabase
{
    public class HockeyContext : DbContext, IHockeyReport<Club, Player>
    {
        private static readonly int Year = DateTime.Now.Year;
        public virtual DbSet<Player> Players { get; set; }
        public virtual DbSet<Club> Clubs { get; set; }

        public HockeyContext() : base("name=BaliContext")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>().ToTable("Player"); 

            modelBuilder.Entity<Club>().ToTable("Club");

            modelBuilder.Entity<Club>().HasMany(club => club.Players);
        }

        /// <summary>
        /// Vrati vsetky kluby z databazy.
        /// </summary>
        public IQueryable<Club> GetClubs()
        {
            //Enumerable.Empty<Club>().AsQueryable();
            return this.Clubs.AsQueryable();
        }

        /// <summary>
        /// Vrati vsetkych hracov z databazy.
        /// </summary>
        public IQueryable<Player> GetPlayers()
        {
            return this.Players.AsQueryable();
        }

        /// <summary>
        /// Vrati utriedeny zoznam klubov podla poctu priradenych hracov zostupne
        /// (od klubu s najvacsim poctom hracov po najmensi), pricom vrati prvych n klubov
        /// zadanych podla vstupneho prametra <paramref name="maxResultCount" />.
        /// </summary>
        /// <param name="maxResultCount">Celkovy pocet klubov, ktore vrati.</param>
        public IEnumerable<Club> GetSortedClubs(int maxResultCount)
        {
            //https://stackoverflow.com/questions/319973/how-to-get-first-n-elements-of-a-list-in-c
            return this.Clubs.OrderByDescending(c => c.Players.Count).Take(maxResultCount).AsEnumerable();
        }

        /// <summary>
        /// Vrati utriedeny zoznam hracov, najskor podla priezviska vzostupne (A - Z),
        /// potom podla mena zostupne (Z - A), pricom vrati prvych n hracov zadanych
        /// podla vstupneho parametra <paramref name="maxResultCount" />.
        /// </summary>
        /// <param name="maxResultCount">Celkovy pocet hracov, ktorych vrati vo vysledku.</param>
        public IEnumerable<Player> GetSortedPlayers(int maxResultCount)
        {
            return this.Players.OrderBy(p => p.LastName).ThenByDescending(p => p.FirstName).Take(maxResultCount).AsEnumerable();
        }

        /// <summary>
        /// Vrati priemerny vek vsetkych hracov.
        /// </summary>
        public double GetPlayerAverageAge()
        {
            return Players.ToList().Average(p => (Year - p.YearOfBirth));
//            var allPlayers = Players.ToList();
//            var averageAge = allPlayers.Sum(player => (year - player.YearOfBirth));
//            return averageAge /= allPlayers.Count;
        }

        /// <summary>
        /// Vrati najmladsieho hraca zo vsetkych. Ak je viac hracov, ktori maju rovnaky najmladsi vek,
        /// vrati hraca, ktory ma najvacsie <see cref="P:HockeyPlayerDatabase.Interfaces.IPlayer.KrpId" />.
        /// </summary>
        public Player GetYoungestPlayer()
        {
            return this.Players.OrderByDescending(a => a.YearOfBirth).ThenByDescending(a => a.KrpId).First();
        }

        /// <summary>
        /// Vrati najstarsieho hraca zo vsetkych. Ak je viac hracov, ktori maju rovnaky najstarsi vek,
        /// vrati hraca, ktory ma najmensie <see cref="P:HockeyPlayerDatabase.Interfaces.IPlayer.KrpId" />.
        /// </summary>
        public Player GetOldestPlayer()
        {
            //TODO: vymysli
            var oldPlayers = this.Players.OrderBy(a => a.YearOfBirth);
            return oldPlayers.First().YearOfBirth == oldPlayers.ElementAt(1).YearOfBirth ? this.Players.OrderBy(a => a.YearOfBirth).ThenBy(a => a.KrpId).First() : oldPlayers.First();
        }

        /// <summary>
        /// Vrati vek hracov z najvacsieho klubu (ktory ma najvacsi pocet hracov) bez duplicit.
        /// </summary>
        public IEnumerable<int> GetBiggestClubPlayerAges()
        {
            //https://stackoverflow.com/questions/998066/linq-distinct-values
            return this.Clubs.OrderByDescending(c => c.Players.Count).First().Players.Select(player => (Year - player.YearOfBirth)).AsEnumerable().Distinct();
        }

        /// <summary>
        /// Vrati vsetkych hracov zoskupenych podla roku narodenia.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IGrouping<int, Player>> GetGroupedPlayersByYearOfBirth()
        {
            //https://docs.microsoft.com/en-us/dotnet/api/system.linq.igrouping-2?view=netframework-4.8
            //https://mtaulty.com/2007/09/28/m_9836/
            //https://stackoverflow.com/questions/7325278/group-by-in-linq
            //https://stackoverflow.com/questions/8521025/how-to-get-values-from-igrouping
            //IEnumerable<IGrouping<int, Player>> grouped = this.Players.GroupBy(p => (year - p.YearOfBirth));
            return this.Players.GroupBy(p => (Year - p.YearOfBirth)); //new IGrouping<int, Player>(p.YearOfBirth, p)).AsEnumerable<IGrouping<int,Player>();
        }

        /// <summary>
        /// Vrati vsetkych hracov, ktorych vek je v rozmedzi od <paramref name="minAge" /> vratane do <paramref name="maxAge" /> vratane.
        /// </summary>
        /// <param name="minAge">Minimalny vek, ktory musia splnat vysledny hraci.</param>
        /// <param name="maxAge">Maximalny vek, ktory musia splnat vysledny hraci.</param>
        /// <returns></returns>
        public IEnumerable<Player> GetPlayersByAge(int minAge, int maxAge)
        {
            return this.Players.Where(p => p.YearOfBirth >= minAge && p.YearOfBirth <= maxAge).AsEnumerable();
        }

        /// <summary>Vrati report jedneho klubu.</summary>
        /// <param name="clubId">Id klubu, z ktoreho sa urobi report.</param>
        /// <returns></returns>
        public ReportResult GetReportByClub(int clubId)
        {
            Club foundClub = this.Clubs.First(c => c.Id == clubId);

            var averageAge = foundClub.Players.Average(p => (Year - p.YearOfBirth));
            
            var youngestPlayer = foundClub.Players.OrderByDescending(a => a.YearOfBirth).ThenByDescending(a => a.KrpId).First();
            
            var oldPlayers = foundClub.Players.OrderBy(a => a.YearOfBirth);
            var oldestPlayer = oldPlayers.First().YearOfBirth == oldPlayers.ElementAt(1).YearOfBirth
                ? this.Players.OrderBy(a => a.YearOfBirth).ThenBy(a => a.KrpId).First()
                : oldPlayers.First();

            ReportResult ret = new ReportResult(foundClub.Players.Count, averageAge,
                youngestPlayer.FullName, oldestPlayer.FullName,
                Year - youngestPlayer.YearOfBirth, Year - oldestPlayer.YearOfBirth);

            return ret;
        }

        /// <summary>Vrati reporty podla vekovej kategorie.</summary>
        /// <returns></returns>
        public IDictionary<AgeCategory, ReportResult> GetReportByAgeCategory()
        {
            
            IQueryable<IGrouping<AgeCategory?, Player>> grouped = this.Players.GroupBy(p => p.AgeCategory);
            //List<Player> cadetsList = grouped.AsEnumerable().SelectMany(c => { return c.Key == AgeCategory.Cadet; }).ToList<Player>();
            var cadetsList = new List<Player>();
            var juniorList = new List<Player>();
            var midgestList = new List<Player>();
            var seniorList = new List<Player>();
            foreach (var grouping in grouped)
            {
                switch (grouping.Key)
                {
                    case AgeCategory.Cadet:
                        cadetsList.AddRange(grouping.AsEnumerable());
                        break;
                    case AgeCategory.Junior:
                        juniorList.AddRange(grouping.AsEnumerable());
                        break;
                    case AgeCategory.Midgest:
                        midgestList.AddRange(grouping.AsEnumerable());
                        break;
                    case AgeCategory.Senior:
                        seniorList.AddRange(grouping.AsEnumerable());
                        break;
                    case null:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            IDictionary<AgeCategory, ReportResult> ret = new Dictionary<AgeCategory, ReportResult>
            {
                { AgeCategory.Cadet, ReportMeBaby(cadetsList) },
                { AgeCategory.Junior, ReportMeBaby(juniorList) },
                { AgeCategory.Midgest, ReportMeBaby(midgestList) },
                { AgeCategory.Senior, ReportMeBaby(seniorList) }
            };
            return ret;
        }

        private ReportResult ReportMeBaby(List<Player> paList)
        {
            var averageAge = paList.Average(p => (Year - p.YearOfBirth));

            var youngestPlayer = paList.OrderByDescending(a => a.YearOfBirth).ThenByDescending(a => a.KrpId).First();

            var oldPlayers = paList.OrderBy(a => a.YearOfBirth);
            var oldestPlayer = oldPlayers.First().YearOfBirth == oldPlayers.ElementAt(1).YearOfBirth
                ? this.Players.OrderBy(a => a.YearOfBirth).ThenBy(a => a.KrpId).First()
                : oldPlayers.First();

            return new ReportResult(paList.Count, averageAge,
                youngestPlayer.FullName, oldestPlayer.FullName,
                Year - youngestPlayer.YearOfBirth, Year - oldestPlayer.YearOfBirth);
        }

        /// <summary>Ulozi zoznam klubov a hracov do XML suboru.</summary>
        /// <param name="fileName">Nazov XML suboru, do ktoreho sa zoserializuje zoznam klubov a hracov.</param>
        public void SaveToXml(string fileName)
        {
            XmlWriter writer = XmlWriter.Create(fileName);
            writer.WriteStartDocument();
            writer.WriteStartElement("root");
            foreach (var club in Clubs)
            {
                writer.WriteStartElement("group");
                writer.WriteAttributeString("clubId", club.Id.ToString());

                    writer.WriteStartElement("name");
                    writer.WriteValue(club.Name);
                    writer.WriteEndElement();

                    writer.WriteStartElement("adress");
                    writer.WriteValue(club.Address);
                    writer.WriteEndElement();

                    writer.WriteStartElement("url");
                    writer.WriteValue(club.Url);
                    writer.WriteEndElement();

                    writer.WriteStartElement("players");
                    //var akumulator_clenovia = "";
                    foreach(var clubPlayer in club.Players)
                    {
                        //akumulator_clenovia += clubPlayer + ";";
                        writer.WriteStartElement("player");
                            writer.WriteAttributeString("playerId", clubPlayer.Id.ToString());
                            writer.WriteAttributeString("playerId", clubPlayer.FirstName);
                            writer.WriteAttributeString("playerId", clubPlayer.LastName);
                            writer.WriteAttributeString("playerId", clubPlayer.FullName);
                            writer.WriteAttributeString("playerId", clubPlayer.TitleBefore);
                            writer.WriteAttributeString("playerId", clubPlayer.YearOfBirth.ToString());
                            writer.WriteAttributeString("playerId", clubPlayer.KrpId.ToString());
                            writer.WriteAttributeString("playerId", clubPlayer.AgeCategory.ToString());
                            writer.WriteAttributeString("playerId", clubPlayer.ClubId.ToString());
                        writer.WriteEndElement();
                    }
                    //writer.WriteValue(akumulator_clenovia);
                    writer.WriteEndElement();

                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
            writer.Flush();
/*

            XmlDocument dokument = new XmlDocument();
            dokument.Load(paNazov);
            foreach (XmlNode node in dokument.DocumentElement)
            {
                string meno = node.Attributes[0].InnerText;
                XmlNode decko = node.FirstChild;
                string veduciSkupiny = decko.InnerText;
                decko = decko.NextSibling;
                string typ = decko.InnerText;
                fTyp typ_ozaj = 0x0000000;
                Enum.TryParse<fTyp>(typ, out typ_ozaj);
                decko = decko.NextSibling;
                string poznamka = decko.InnerText;
                decko = decko.NextSibling;
                string[] podskupiny = decko.InnerText.Split(';');
                decko = decko.NextSibling;
                string[] clenovia = decko.InnerText.Split(';');
                HashSet<string> podskupiny_polo = new HashSet<string>();
                HashSet<string> clenovia_polo = new HashSet<string>();
                foreach (string polozka in podskupiny) { podskupiny_polo.Add(polozka); }
                foreach (string polozka in clenovia) { clenovia_polo.Add(polozka); }
                Skupina nova = new Skupina(meno, typ_ozaj, poznamka, veduciSkupiny);
                nova.Podskupiny = podskupiny_polo;
                nova.Clenovia = clenovia_polo;
                t_Skupiny.Add(nova);
            }
*/
        }

    }
}
