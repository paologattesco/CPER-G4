using ITS.CPER.SimulatoreSmartWatch.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ITS.CPER.SimulatoreSmartWatch.Service;

public class DataAccess : IDataAccess
{
    private readonly string _connectionString;
    private readonly string _host;
    private readonly string _user;
    private readonly string _dbname;
    private readonly string _password;
    private readonly string _port;

    public DataAccess(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("db");
        _host = configuration.GetConnectionString("Host");
        _user = configuration.GetConnectionString("User");
        _dbname = configuration.GetConnectionString("Dbname");
        _password = configuration.GetConnectionString("Password");
        _port = configuration.GetConnectionString("Port");

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

    public void InsertProductionBatch(Guid smartwatchId)
    {
        string connString = $"User ID={_user};Password={_password};Host={_host};Port={_port};Database={_dbname};";

        using (var conn = new NpgsqlConnection(connString))

        {
            conn.Open();
            using (var command = new NpgsqlCommand("INSERT INTO ProductionBatch (batch_id, smartwatch_id, date) VALUES (@batch_id, @smartwatch_id, @date)", conn))
            {
                command.Parameters.AddWithValue("@batch_id", Guid.NewGuid());
                command.Parameters.AddWithValue("@smartwatch_id", smartwatchId);
                command.Parameters.AddWithValue("@date", DateTime.Now);
                command.ExecuteNonQuery();
            }
            conn.Close();
        }
    }
}