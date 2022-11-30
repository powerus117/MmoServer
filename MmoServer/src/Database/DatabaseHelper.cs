using System.Configuration;
using MySqlConnector;

namespace MmoServer.Database
{
    public static class DatabaseHelper
    {
        public static readonly string ConnectionString = new MySqlConnectionStringBuilder()
        {
            Server = ConfigurationManager.AppSettings["Server"],
            Database = ConfigurationManager.AppSettings["Database"],
            UserID = ConfigurationManager.AppSettings["UserID"],
            Password = ConfigurationManager.AppSettings["Password"],
        }.ConnectionString;
    }
}