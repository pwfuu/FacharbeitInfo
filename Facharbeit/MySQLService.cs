using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;



namespace Facharbeit
{
    public static class MySQLService
    {
        const string connectionString = "Server=localhost;Database=Facharbeit;Uid=root;Pwd=Arthursql1!;";
        public static void InsertWeight(double gewicht, DateTime date, int personID) //man soll das Gewicht, Datum, und personID in die Datenbank speichern.
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString)) //...
            {
                conn.Open(); //...

                string dateString = date.Year + "-" + date.Month + "-" + date.Day; //die DateTime variable wird jetzt in das richtige Format aufgeteilt, was
                                                                                            //die Datenbank dieses Format braucht
                string gewichtString = gewicht.ToString(CultureInfo.InvariantCulture);     //damit aus kommas punkte werden für die datenbank int werden  



                string query = $"INSERT INTO Weight (personID, datum, value) VALUES ({personID}, '{dateString}', {gewichtString})"; //...

                using (MySqlCommand cmd = new MySqlCommand(query, conn)) //...
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static String getUsername(int personID) //von dem personID soll man einen username bekommen
        {
            string name = "";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM person WHERE personID = " + personID + "";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            name = reader.GetString("username");  //da wo die personID die bestimmte ist, soll der username returned werden.
                        }

                    }
                }
            }
            return name;
        }

        public static (Boolean succesfull, int id) login(String username, String password)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString)) //erstellt eine neue Connection mit MySQLConnector
            {


                connection.Open(); //Hier wird die Connection geöffnet

                string query = "SELECT * FROM person WHERE username = '" + username + "'"; //das ist der Befehl, der in der Datenbank ausgeführt wird, ähnlich wie in DBeaver
                                                                                           
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32("personID");
                            string name = reader.GetString("username");
                            string passwordActual = reader.GetString("password");   //es wird in der Datenbank geschaut,
                                                                                    // ob zu dem Username das eingegebene Passwort mit dem in der Datenbank übereinstimmt
                            if (password == passwordActual)
                            {
                                return (true, id);                                  //wenn das übereinstimmt, dann wird true und die ID dieser Person weitergegeben
                            }

                        }

                    }
                }
            }
            return (false, 0); //wenn nicht, dann wird false zurückgesendet und die ID ist dann egal weil es sowieso keine Anmeldung gibt.
        }

        public static bool Register(String username, String password, DateTime date, String passwordSafe) // bool, damit überprüft wird, ob die beiden passwörter übereinstimmen
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString)) 
            {
                conn.Open(); //Connection wird geöffnet

                String queryDate = date.ToString("yyyy-MM-dd HH:mm:ss"); // Es soll auch ein Erstellungsdatum gespeichert werde, was der Benutzer selbst nicht eingeben muss



                if (password == passwordSafe) //Überprüfen ob die Passwörter übereinstimmen
                {
                    string query = $"INSERT INTO person (username, password, createdDate) VALUES ('{username}', '{password}', '{queryDate}')"; //Befehl für die Datenbank
                    using (MySqlCommand cmd = new MySqlCommand(query, conn)) //MySql Befehl um Befehl auszuführen
                    {
                        cmd.ExecuteNonQuery(); //wird ausgeführt
                        return true;           
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public static List<(double weight, DateTime date)> getData(int personID)
        {
            List<(double weight, DateTime date)> result = new List<(double weight, DateTime date)>();
            String query = $"SELECT * FROM weight WHERE personID = {personID}";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand command = new MySqlCommand(query, conn))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read()) //die while schleife liest durch jedes Gewicht, was der person gehört
                        {
                            double weight = reader.GetDouble("value");
                            DateTime date = reader.GetDateTime("datum");

                            result.Add((weight, date)); // dann wird das gespeichert und geht von vorne los
                        }

                    }
                }

            }
            return result.OrderBy(x => x.date).ToList(); //Weiß nicht wie dass funktioniert, aber ich wollte
                                                         //nach dem Datum sortieren und das war der einfachste Weg

            
        }
        public static List<(double weight, DateTime date)> getData30(int personID)
        {
            List<(double weight, DateTime date)> result = new List<(double weight, DateTime date)>();
            String query = $"SELECT * FROM weight WHERE personID = {personID} AND datum >= DATE_SUB(CURDATE(), INTERVAL 30 DAY)"; //nur Daten, wo das jetzige 
                                                                                                                                  //Datum minus 30 Tage zutrifft
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand command = new MySqlCommand(query, conn))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read()) //die while schleife liest durch jedes Gewicht, was der person gehört
                        {
                            double weight = reader.GetDouble("value");
                            DateTime date = reader.GetDateTime("datum");

                            result.Add((weight, date)); // dann wird das gespeichert und geht von vorne los
                        }

                    }
                }

            }
            return result.OrderBy(x => x.date).ToList(); //Weiß nicht wie dass funktioniert, aber ich wollte
                                                         //nach dem Datum sortieren und das war der einfachste Weg

        }

        public static List<(double weight, DateTime date)> getData90(int personID)
        {
            List<(double weight, DateTime date)> result = new List<(double weight, DateTime date)>();
            String query = $"SELECT * FROM weight WHERE personID = {personID} AND datum >= DATE_SUB(CURDATE(), INTERVAL 90 DAY)";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand command = new MySqlCommand(query, conn))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            double weight = reader.GetDouble("value");
                            DateTime date = reader.GetDateTime("datum");

                            result.Add((weight, date));
                        }

                    }
                }

            }
            return result.OrderBy(x => x.date).ToList();

        }

        public static List<(int personID, String name)> getAllUsers()
        {
            List<(int personID, String name)> result = new List<(int personID, String name)>();
            String query = $"SELECT * FROM person";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand command = new MySqlCommand(query, conn))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                           int personID = reader.GetInt32("personID");
                           String username = reader.GetString("username");

                            result.Add((personID, username));
                        }

                    }
                }

            }
            return result;

        }
    }
}

