using System;
using Microsoft.Data.Sqlite;

namespace Kurssisuoritukset
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var connection = new SqliteConnection("Data Source=kurssit.db"))
            {
                try
                {
                    connection.Open();
                    System.Console.WriteLine("Toiminnot: ");
                    System.Console.WriteLine("1: Laske annettuna vuonna saatujen opintopisteiden yhteismäärä.");
                    System.Console.WriteLine("2: Tulosta annetun opiskelijan kaikki suoritukset aikajärjestyksessä.");
                    System.Console.WriteLine("3: Tulosta annetun kurssin suoritusten arvosanojen jakauma.");
                    System.Console.WriteLine("4: Tulosta top x eniten opintopisteitä antaneet opettajat.");
                    System.Console.WriteLine("5: Sulje ohjelma.\n");
                    while(true)
                    {
                        int number = HankiNumeroKäyttäjältä("Valitse Toiminto: ");                        
                        switch (number)
                        {
                            case 1:
                                TulostaVuodenKokonaisPisteet(connection);
                                break;
                            case 2:
                                TulostaOpiskelijanSuoritukset(connection);
                                break;
                            case 3:
                                TulostaKurssinArvosanaJakauma(connection);
                                break;
                            case 4:
                                TulostaTopOpintopisteet(connection);
                                break;
                            case 5:
                                System.Console.WriteLine("Ohjelma sammuu...");
                                return;
                            default:
                                System.Console.WriteLine("Anna numero väliltä 1-5");
                                break;
                        }                        
                    }
                }
                catch(SqliteException e)
                {
                    System.Console.WriteLine(e);
                }
            }
        }

        static int HankiNumeroKäyttäjältä(string pyyntö)
        {
            do
            {
                Console.Write(pyyntö);
                string input = System.Console.ReadLine();
                int number;
                if (int.TryParse(input, out number))
                {
                    return number;
                }
                else
                {
                    System.Console.WriteLine("Syötetyn arvon täytyy olla numero!");
                }
            } while (true);           
        }

        static void TulostaVuodenKokonaisPisteet(SqliteConnection connection)
        {
            int vuosi = HankiNumeroKäyttäjältä("Anna vuosi: ");
            
            var command = connection.CreateCommand();
            command.CommandText =
            $@"
                SELECT SUM(K.laajuus)
                FROM Suoritukset S, Kurssit K
                WHERE paivays BETWEEN '{vuosi}-01-01' AND '{vuosi}-12-31'
                AND S.kurssi_id = K.id
            ";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    System.Console.WriteLine($"Opintopisteiden määrä: {reader.GetString(0)}");
                }
            }            
        }

        static void TulostaOpiskelijanSuoritukset(SqliteConnection connection)
        {
            Console.Write("Anna opiskelijan nimi: ");
            string nimi = Console.ReadLine();

            var command = connection.CreateCommand();
            command.CommandText =
            $@"
                SELECT K.nimi, K.laajuus, S.paivays, S.arvosana
                FROM Suoritukset S, Kurssit K, Opiskelijat O
                WHERE S.opiskelija_id = O.id AND O.nimi='{nimi}' 
                AND S.kurssi_id = K.id
                ORDER BY paivays
            ";

            using (var reader = command.ExecuteReader())
            {
                Console.WriteLine("{0,-10} {1,-10} {2,-10} {3,10}\n", "kurssi", "op", "päiväys", "arvosana");
                while (reader.Read())
                {
                    var kurssi = reader.GetString(0);
                    var op = reader.GetString(1);
                    var päiväys = reader.GetString(2);
                    var arvosana = reader.GetString(3);
                    Console.WriteLine("{0,-10} {1,-5} {2,-10} {3,5}",
                        kurssi,op,päiväys,arvosana);
                }
            }
        }

        static void TulostaKurssinArvosanaJakauma(SqliteConnection connection)
        {
            Console.Write("Anna kurssin kurssikoodi: ");
            string kurssikoodi = Console.ReadLine();

            var command = connection.CreateCommand();
            command.CommandText =
            $@"
                SELECT COUNT(S.arvosana)
                FROM Suoritukset S, Kurssit K
                WHERE S.kurssi_id = K.id AND K.nimi = '{kurssikoodi}'
                GROUP BY S.arvosana
            ";

            using (var reader = command.ExecuteReader())
            {
                int arvosana = 1;
                while (reader.Read())
                {
                    System.Console.WriteLine($"Arvosana {arvosana}: {reader.GetString(0)} kpl");
                    arvosana++;
                }
            }
        }

        static void TulostaTopOpintopisteet(SqliteConnection connection)
        {
            int opettajienMaara = HankiNumeroKäyttäjältä("Anna opettajien määrä: ");

            var command = connection.CreateCommand();
            command.CommandText =
            $@"
                SELECT O.nimi, SUM(K.laajuus) AS summa
                FROM Suoritukset S, Kurssit K, Opettajat O
                WHERE S.kurssi_id = K.id AND K.opettaja_id = O.id
                GROUP BY O.nimi
                ORDER BY summa DESC
                LIMIT {opettajienMaara}
            ";

            using (var reader = command.ExecuteReader())
            {
                Console.WriteLine("{0,-10} {1,10}\n", "Opettaja", "OP");
                while (reader.Read())
                {
                    var opettaja = reader.GetString(0);
                    var op = reader.GetString(1);
                    Console.WriteLine("{0,-20} {1,10}",
                        opettaja, op);
                }
            }
        }
    }
}
