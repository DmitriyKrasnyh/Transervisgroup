using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

public static class Database
{
    private static string dbFile = "Transsevisgroup.db";

    private static string connectionString =
        $"Data Source={Path.Combine(Application.StartupPath, dbFile)};Version=3;";

    public static SQLiteConnection GetConnection()
    {
        return new SQLiteConnection(connectionString);
    }
}