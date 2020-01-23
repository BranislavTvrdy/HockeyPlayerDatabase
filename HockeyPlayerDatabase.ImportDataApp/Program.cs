
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HockeyPlayerDatabase.Interfaces;

namespace HockeyPlayerDatabase.ImportDataApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var clubFlag = false;
            var playersFlag = false;
            var clearDatabase = false;
            var clubsPath = "";
            var playersPath = "";
            CheckArgs(args, ref clubFlag,ref playersFlag,ref clubsPath,ref playersPath,ref clearDatabase);
            var clubs = new List<Club>();
            var players = new List<Player>();
            if (clubFlag && !string.IsNullOrWhiteSpace(clubsPath))
            {
                ReadClubs(ref clubs, clubsPath);
            }
            if (playersFlag && !string.IsNullOrWhiteSpace(playersPath))
            {
                ReadPlayers(ref players, playersPath, ref clubs);
            }
            #region InicializationDB
            using (var db_context = new HockeyContext())
            {
                if (clearDatabase)
                {
                    db_context.Database.ExecuteSqlCommand("DELETE FROM Player");
                    db_context.Database.ExecuteSqlCommand("DELETE FROM Club");
                }
                foreach (var club in clubs)
                {
                    db_context.Clubs.Add(club);
                }
                foreach (var player in players)
                {
                    db_context.Players.Add(player);
                }
                db_context.SaveChanges();
                foreach (var club in db_context.GetClubs())
                {
                    Console.WriteLine(club);
                }
                foreach (var player in db_context.GetPlayers())
                {
                    Console.WriteLine(player);
                }
            }
            Console.WriteLine("DB Loaded!");
            Console.ReadLine();
            #endregion
        }

        public static void CheckArgs(string[] args, ref bool clubFlag, ref bool playersFlag, ref string clubsPath, ref string playersPath, ref bool clearDatabase)
        {
            if (args.Length != 0)
            {
                foreach (var arg in args)
                {
                    if (arg.Contains(".exe")) continue;
                    if (arg.Contains("-clubs"))
                    {
                        clubFlag = true;
                    }
                    else
                    {
                        if (arg.Contains("-players"))
                        {
                            playersFlag = true;
                        }
                        else
                        {
                            if (arg.Contains("-clearDatabase"))
                            {
                                clearDatabase = true;
                            }
                            if (arg.StartsWith("-")) continue;
                            if (arg.Contains(".csv"))
                            {
                                clubsPath = arg;
                            }
                            else
                            {
                                if (arg.Contains(".tsv"))
                                {
                                    playersPath = arg;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Console.Error.WriteLine("NEBOL ZADANY VSTUP V PARAMETROCH!");
            }
        }

        /// <summary>
        /// Reads all clubs from file.
        /// </summary>
        /// <param name="paClubs">List to save clubs to.</param>
        /// <param name="clubsPath">Path to the clubs file.</param>
        public static void ReadClubs(ref List<Club> paClubs, string clubsPath)
        {
            var reader = new StreamReader(clubsPath);
            reader.ReadLine();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (!string.IsNullOrWhiteSpace(line))
                {
                    var values = line.Split(';');
                    paClubs.Add(new Club(paClubs.Count, values[0], values[1], values[2]));
                }
            }
            reader.Close();
        }

        /// <summary>
        /// Reads all players from file.
        /// </summary>
        /// <param name="paPlayers">List to save players to.</param>
        /// <param name="playersPath">Path to the players file.</param>
        /// <param name="paClubs">List of clubs for matching players with clubs.</param>
        public static void ReadPlayers(ref List<Player> paPlayers, string playersPath, ref List<Club> paClubs)
        {
            var reader = new StreamReader(playersPath);
            reader.ReadLine();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (!string.IsNullOrWhiteSpace(line))
                {
                    var values = line.Split('\t');
                    //https://stackoverflow.com/questions/1943273/convert-all-first-letter-to-upper-case-rest-lower-for-each-word/1943293
                    var lastName = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(values[0].ToLower());
                    AgeCategory? ageCategory = null;
                    switch (values[6])
                    {
                        case "Juniori":
                            ageCategory = AgeCategory.Junior;
                            break;
                        case "Dorastenci":
                            ageCategory = AgeCategory.Midgest;
                            break;
                        case "Kadeti":
                            ageCategory = AgeCategory.Cadet;
                            break;
                        case "Seniori":
                            ageCategory = AgeCategory.Senior;
                            break;
                        case null:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    var newPlayer = new Player(paPlayers.Count, values[1], lastName, values[2],
                        int.Parse(values[3]), int.Parse(values[4]), ageCategory, GetClubIdFromName(paClubs, values[5]));
                    paPlayers.Add(newPlayer);
                    AsignPlayerToClub(ref paClubs, newPlayer);

                }
            }
            reader.Close();
        }

        /// <summary>
        /// Gets Id of Club based on its Name
        /// </summary>
        /// <param name="paClubs">List of Clubs.</param>
        /// <param name="paSearching">Name of the Club.</param>
        /// <returns>Club Id.</returns>
        public static int GetClubIdFromName(List<Club> paClubs, string paSearching)
        {
            foreach (var club in paClubs.Where(club => club.Name == paSearching))
            {
                return club.Id;
            }
            return -1;
        }

        /// <summary>
        /// Assigns Player to Club
        /// </summary>
        /// <param name="paClubs">List of Clubs.</param>
        /// <param name="paPlayer">Assigning Player.</param>
        public static void AsignPlayerToClub(ref List<Club> paClubs, Player paPlayer)
        {
            foreach (var club in paClubs.Where(club => club.Id == paPlayer.ClubId))
            {
                club.Players.Add(paPlayer);
                return;
            }
        }
    }
}
