using System;


namespace IndeksinTehokkuus
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Tehokkuustestin osat ovat: ");
            System.Console.WriteLine("Ohjelma luo taulun Elokuvat, jossa on sarakkeet id, nimi ja vuosi");
            System.Console.WriteLine("Ohjelma lisää tauluun miljoona riviä. Jokaisen rivin kohdalla nimeksi valitaan satunnainen merkkijono ja vuodeksi valitaan satunnainen kokonaisluku väliltä 1900–2000.");
            System.Console.WriteLine("Ohjelma suorittaa tuhat kertaa kyselyn, jossa haetaan elokuvien määrä vuonna x. Jokaisessa kyselyssä x valitaan satunnaisesti väliltä 1900–2000.");
            while(true)
            {
                System.Console.WriteLine("Valitse testin suoritustapa: ");
                System.Console.WriteLine("1: Tauluun ei lisätä kyselyitä tehostavaa indeksiä.");
                System.Console.WriteLine("2: Tauluun lisätään kyselyitä tehostava indeksi ennen rivien lisäämistä.");
                System.Console.WriteLine("3: Tauluun lisätään kyselyitä tehostava indeksi ennen kyselyiden suoritusta.");
                System.Console.WriteLine("4: Sulje ohjelma");
                int number = HankiNumeroKäyttäjältä("Valitse Toiminto: ");
                switch (number)
                {
                    case 1:
                        Testit.Testi1();
                        break;
                    case 2:
                        Testit.Testi2();
                        break;
                    case 3:
                        Testit.Testi3();
                        break;
                    case 4:
                        System.Console.WriteLine("Ohjelma sammuu...");
                        return;
                    default:
                        System.Console.WriteLine("Anna numero väliltä 1-4");
                        break;
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

        
    }
}
