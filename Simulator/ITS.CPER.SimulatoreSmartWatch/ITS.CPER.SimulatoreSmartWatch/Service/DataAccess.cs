using ITS.CPER.SimulatoreSmartWatch.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITS.CPER.SimulatoreSmartWatch.Service;

public class DataAccess : IDataAccess
{
    private readonly string _connectionString;

    public DataAccess(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("db");
    }
    public List<Guid> GetSmartWatchesId()
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        SqlCommand sql = connection.CreateCommand();
        sql.CommandText = $"SELECT [SmartWatch_Id] FROM [dbo].[UserDetails]";
        var result = new List<Guid>();
        using (SqlDataReader reader = sql.ExecuteReader())
        {
            while (reader.Read())
            {
                result.Add(Guid.Parse((string)reader["SmartWatch_Id"]));
            }
            reader.Close();
        }
        return result;
    }

    public Guid GetUserId(Guid smartwatchId)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        SqlCommand sql = connection.CreateCommand();
        sql.CommandText = $"SELECT [User_Id] FROM [dbo].[UserDetails] WHERE SmartWatch_Id = @SmartWatch_Id";
        sql.Parameters.AddWithValue("@SmartWatch_Id", smartwatchId);
        var result = new Guid();
        using (SqlDataReader reader = sql.ExecuteReader())
        {
            while (reader.Read())
            {
                result = Guid.Parse((string)reader["User_Id"]);
            }
            reader.Close();
        }
        return result;
    }
}