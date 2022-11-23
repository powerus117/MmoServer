﻿using System.Configuration;
using MySql.Data.MySqlClient;

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
            MinimumPoolSize = 1,
            ConnectionLifeTime = 300
        }.ConnectionString;
    }
}