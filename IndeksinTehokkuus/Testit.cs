using System;
using System.Diagnostics;
using Microsoft.Data.Sqlite;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;

namespace IndeksinTehokkuus
{
    class Testit {

        static DateTime minDT = new DateTime(1900, 1, 1);
        static DateTime maxDT = new DateTime(2000, 12, 31);

        internal static void Testi1()
        {
            using (SqliteConnection con = new SqliteConnection("Data Source=testi1.db"))
            {
                try
                {
                    TimeSpan lisaysAika;
                    TimeSpan hakuAika;                   
                    
                    con.Open();
                    CreateTable(con);
                    
                    Stopwatch stopWatch = new Stopwatch();                    
                    stopWatch.Start();
                    InsertMovies(con);
                    stopWatch.Stop();
                    lisaysAika = stopWatch.Elapsed;

                    stopWatch.Reset();
                    stopWatch.Start();
                    GetMoviesReleased(con);
                    stopWatch.Stop();
                    hakuAika = stopWatch.Elapsed;

                    PrintTimes(lisaysAika, hakuAika, null);

                    con.Close();
                }
                catch (SqliteException e)
                {
                    System.Console.WriteLine(e);
                }
            }
        }

        internal static void Testi2()
        {
            using (SqliteConnection con = new SqliteConnection("Data Source=testi2.db"))
            {
                try
                {
                    TimeSpan lisaysAika;
                    TimeSpan hakuAika;
                    TimeSpan indeksinLuontiAika;

                    con.Open();
                    CreateTable(con);

                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    CreateIndex(con);
                    stopWatch.Stop();
                    indeksinLuontiAika = stopWatch.Elapsed;

                    stopWatch.Reset();
                    stopWatch.Start();
                    InsertMovies(con);
                    stopWatch.Stop();
                    lisaysAika = stopWatch.Elapsed;

                    stopWatch.Reset();
                    stopWatch.Start();
                    GetMoviesReleased(con);
                    stopWatch.Stop();
                    hakuAika = stopWatch.Elapsed;

                    PrintTimes(lisaysAika, hakuAika, indeksinLuontiAika);

                    con.Close();
                }
                catch (SqliteException e)
                {
                    System.Console.WriteLine(e);
                }
            }
        }

        internal static void Testi3()
        {
            using (SqliteConnection con = new SqliteConnection("Data Source=testi3.db"))
            {
                try
                {
                    TimeSpan lisaysAika;
                    TimeSpan hakuAika;
                    TimeSpan indeksinLuontiAika;

                    con.Open();
                    CreateTable(con);

                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    InsertMovies(con);
                    stopWatch.Stop();
                    lisaysAika = stopWatch.Elapsed;

                    stopWatch.Reset();
                    stopWatch.Start();
                    CreateIndex(con);
                    stopWatch.Stop();
                    indeksinLuontiAika = stopWatch.Elapsed;

                    stopWatch.Reset();
                    stopWatch.Start();
                    GetMoviesReleased(con);
                    stopWatch.Stop();
                    hakuAika = stopWatch.Elapsed;

                    PrintTimes(lisaysAika, hakuAika, indeksinLuontiAika);

                    con.Close();
                }
                catch (SqliteException e)
                {
                    System.Console.WriteLine(e);
                }
            }
        }

        private static void CreateTable(SqliteConnection con)
        {
            SqliteCommand createTableCommand = con.CreateCommand();
            createTableCommand.CommandText =
            $@"
                        CREATE TABLE IF NOT EXISTS Elokuvat (
                            id INTEGER PRIMARY KEY,
                            nimi TEXT,
                            julkaisupv TEXT
                        );
                    ";
            createTableCommand.ExecuteNonQuery();
            System.Console.WriteLine("Taulu on luotu tai se on jo olemassa, ohjelma jatkaa...");
            System.Console.WriteLine("Aletaan lisäämään satunnaisia keksittyjä elokuvia transaktion kautta...");
        }

        private static void CreateIndex(SqliteConnection con)
        {
            System.Console.WriteLine("Luodaan indeksi julkaisupäivään");
            SqliteCommand createIndexCommand = con.CreateCommand();
            createIndexCommand.CommandText =
            $@"
                CREATE INDEX idx_julkaisupv ON Elokuvat (julkaisupv)
            ";
            createIndexCommand.ExecuteNonQuery();
            System.Console.WriteLine("Indeksi luotu");
        }

        private static void InsertMovies(SqliteConnection con)
        {
            IRandomizerDateTime randomizerDate = RandomizerFactory.GetRandomizer(new FieldOptionsDateTime { From = minDT, To = maxDT });

            using (SqliteTransaction transaction = con.BeginTransaction())
            {
                string lastWord = "";
                string word1 = "";
                string word2 = "";
                var randomizerWord = RandomizerFactory.GetRandomizer(new FieldOptionsTextWords { Min = 1, Max = 1 });
                SqliteCommand addCommand = con.CreateCommand();
                for (int i = 0; i <= 1000000; i++)
                {
                    do
                    {
                        word1 = randomizerWord.Generate();
                        word2 = randomizerWord.Generate();
                    } while (word1.Equals(lastWord));
                    lastWord = word1;
                    addCommand.CommandText =
                    $@"
                                INSERT INTO Elokuvat (nimi, julkaisupv) VALUES ('{word1} {word2}', '{randomizerDate.GenerateAsString()}')
                            ";
                    System.Console.WriteLine($@"{i}. INSERT INTO Elokuvat (nimi, julkaisupv) VALUES ('{word1} {word2}', '{randomizerDate.GenerateAsString()}')");
                    addCommand.ExecuteNonQuery();
                }
                transaction.Commit();
            }
        }

        private static void GetMoviesReleased(SqliteConnection con)
        {
            IRandomizerDateTime randomizerDate = RandomizerFactory.GetRandomizer(new FieldOptionsDateTime { From = minDT, To = maxDT });

            System.Console.WriteLine("Haetaan elokuvia satunnaisilta vuosilta...");
            int lastRandomYear = 0;
            for (int i = 0; i <= 1000; i++)
            {
                int randomYear;
                do
                {
                    DateTime randomDate = (DateTime)randomizerDate.Generate();
                    randomYear = randomDate.Year;
                } while (randomYear == lastRandomYear);
                SqliteCommand searchCommand = con.CreateCommand();
                searchCommand.CommandText =
                $@"
                            SELECT COUNT(*)
                            FROM Elokuvat
                            WHERE julkaisupv BETWEEN '{randomYear}-01-01' AND '{randomYear}-12-31'
                        ";

                using (var reader = searchCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        System.Console.WriteLine($"Julkaistujen elokuvien määrä vuonna {randomYear} on {reader.GetString(0)}");
                    }
                }
            }
        }

        private static void PrintTimes(TimeSpan lisaysAika, TimeSpan hakuAika, TimeSpan? indeksinLuontiAika)
        {
            System.Console.WriteLine($"Elokuvat lisätty ajassa {lisaysAika}");
            System.Console.WriteLine($"Elokuvat haettu ajassa {hakuAika}");
            if(indeksinLuontiAika != null)
            {
                System.Console.WriteLine($"Indeksi luotu ajassa {indeksinLuontiAika}");
            }
        }

    }
}
