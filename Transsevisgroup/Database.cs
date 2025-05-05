using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

public static class Database
{
    private static string connectionString =
        "Data Source=HOME-PC\\SQLEXPRESS;Initial Catalog=TranssevisgroupDB;Integrated Security=True";

    public static SqlConnection GetConnection()
    {
        return new SqlConnection(connectionString);
    }
}