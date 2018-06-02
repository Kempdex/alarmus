using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmusServer
{
    /// <summary>
    /// Класс, работающий с базой данных.
    /// Singleton.
    /// Требует вызова метода Initialize() перед началом работы
    /// </summary>
    public class DatabaseMaster
    {
        private static string databaseFile = "main_server_db.sqlite";
        private static string connectionString = "Data Source=" + databaseFile + ";Version=3";
        private static string databaseCreationSql = "CREATE TABLE 'Request' ('id' INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, 'owner' INTEGER, troubleData TEXT, 'troubleType' TEXT NOT NULL)";
        private static string usersCreationSql = "CREATE TABLE 'Users' ('id' INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, 'login' TEXT NOT NULL, 'password' TEXT NOT NULL)";

        private static SQLiteConnection connection = new SQLiteConnection(connectionString);

        /// <summary>
        /// Метод инициализации класса.
        /// Проверяет наличие базы данных. Создает ее, если она отсутсвует
        /// </summary>
        public static void Initialize()
        {
            if(File.Exists(databaseFile))
            {
                return;
            }

            SQLiteConnection.CreateFile(databaseFile);
            connection.Open();

            SQLiteCommand command = new SQLiteCommand(databaseCreationSql, connection);
            command.ExecuteNonQuery();
            SQLiteCommand createUsers = new SQLiteCommand(usersCreationSql, connection);
            createUsers.ExecuteNonQuery();

            connection.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        public static bool HasPassword(Alarmus.AutorizationMessage msg)
        {
            connection.Open();

            string passwordCommand = "SELECT * FROM Users WHERE login = '" + msg.userLogin + "' AND password = '" + msg.userPassword + "'";

            SQLiteCommand command = new SQLiteCommand(passwordCommand, connection);
            SQLiteDataReader reader = command.ExecuteReader();

            if(reader.HasRows)
            {
                connection.Close();
                return true;
            }

            connection.Close();
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        public static void AddRequest(Alarmus.RequestMessage request)
        {
            connection.Open();

            string addCommand = "INSERT INTO Request (troubleData, troubleType) VALUES ('" + request.troubleData + "', '" + request.typeOfTrouble + "')";

            SQLiteCommand command = new SQLiteCommand(addCommand, connection);
            command.ExecuteNonQuery();

            connection.Close();
        }

        public static List<Alarmus.RequestMessage> GetAllRequests()
        {
            connection.Open();

            string getAllRequestsCommand = "SELECT * FROM Request";

            SQLiteCommand command = new SQLiteCommand(getAllRequestsCommand, connection);

            SQLiteDataReader dataReader = command.ExecuteReader();

            List<Alarmus.RequestMessage> requestList = new List<Alarmus.RequestMessage>();

            while(dataReader.Read())
            {
                Alarmus.RequestMessage request = new Alarmus.RequestMessage(dataReader["troubleData"].ToString(), dataReader["troubleType"].ToString());
                requestList.Add(request);
            }

            connection.Close();

            return requestList;
        }
    }
}
