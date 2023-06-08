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
        sql.CommandText = $"SELECT [Id] FROM [dbo].[SmartWatches]";
        var result = new List<Guid>();
        using (SqlDataReader reader = sql.ExecuteReader())
        {
            while (reader.Read())
            {
                result.Add(Guid.Parse((string)reader["Id"]));
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
        sql.CommandText = $"SELECT [FK_User_Id] FROM [dbo].[SmartWatches] WHERE Id = @Id";
        sql.Parameters.AddWithValue("@Id", smartwatchId);
        var result = new Guid();
        using (SqlDataReader reader = sql.ExecuteReader())
        {
            while (reader.Read())
            {
                result = Guid.Parse((string)reader["FK_User_Id"]);
            }
            reader.Close();
        }
        return result;
    }
}