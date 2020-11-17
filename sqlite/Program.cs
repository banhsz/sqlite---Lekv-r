using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sqlite
{
    class Program
    {
        static void Main(string[] args)
        {

            using (var conn = new SQLiteConnection("Data Source=mydb.db"))
            {
                //Tábla létrehozás
                conn.Open();
                var createComm = conn.CreateCommand();
                createComm.CommandText = @"
                CREATE TABLE IF NOT EXISTS lekvar(
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    meret DOUBLE NOT NULL,     
                    tipus VARCHAR(1000) NOT NULL
                );
                ";
                createComm.ExecuteNonQuery();

                //Tábla feltöltés
                string parancs = "init";
                while (parancs != "")
                {
                    parancs = "reset";
                    while (!(parancs == "i" || parancs == ""))
                    {
                        Console.WriteLine("Új üveg = i , Vége = enter");
                        parancs = Console.ReadLine();
                    }

                    if (parancs=="i")
                    {
                        Console.WriteLine("új üveg:");
                        Console.Write("üveg mérete (l): ");
                        double meret = Convert.ToDouble(Console.ReadLine());
                        Console.Write("lekvár típusa: ");
                        string tipus = Console.ReadLine();

                        var insertComm = conn.CreateCommand();
                        insertComm.CommandText = @"
                        INSERT INTO lekvar(id,meret,tipus)
                        VALUES (null,@meret,@tipus)
                        ";
                        insertComm.Parameters.AddWithValue("@meret", meret);
                        insertComm.Parameters.AddWithValue("@tipus", tipus);
                        insertComm.ExecuteNonQuery();
                    }

                }

                //Kiírás (táblázatszerű)
                var selectComm = conn.CreateCommand();
                selectComm.CommandText = @"
                    SELECT * FROM lekvar
                    ";
                SQLiteDataReader reader =  selectComm.ExecuteReader();
                Console.WriteLine("ID\tMéret(l)\tTípus");
                Console.WriteLine("----------------------------------------------------");
                while (reader.Read())
                {
                    Console.WriteLine(reader.GetInt32(0) + "\t"+reader.GetDouble(1) + "\t\t" + reader.GetString(2));
                }

                //Összes lekvár mennyisége
                selectComm = conn.CreateCommand();
                selectComm.CommandText = @"
                    SELECT SUM(meret) FROM lekvar
                    ";
                reader = selectComm.ExecuteReader();
                Console.WriteLine("\nÖsszes lekvár(l)");
                Console.WriteLine("----------------");
                while (reader.Read())
                {
                    Console.WriteLine(reader.GetDouble(0));
                }

                //Fajtánként a mennyiségek
                selectComm = conn.CreateCommand();
                selectComm.CommandText = @"
                    SELECT tipus,SUM(meret) FROM lekvar
                    GROUP BY tipus
                    ";
                reader = selectComm.ExecuteReader();
                Console.WriteLine("\nTípus\tÖsszes lekvár(l)");
                Console.WriteLine("----------------------------");
                while (reader.Read())
                {
                    Console.WriteLine(reader.GetString(0).ToString()+"\t"+reader.GetDouble(1));
                }

                //Átlagos üvegméret
                selectComm = conn.CreateCommand();
                selectComm.CommandText = @"
                    SELECT AVG(meret) FROM lekvar
                    ";
                reader = selectComm.ExecuteReader();
                Console.WriteLine("\nÜvegek átlagos mérete(l)");
                Console.WriteLine("------------------------");
                while (reader.Read())
                {
                    Console.WriteLine(Math.Round(reader.GetDouble(0),2));
                }

            }
            Console.ReadKey();

        }
    }
}
