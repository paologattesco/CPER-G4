using Microsoft.Data.SqlClient;
using ITS.CPER.WebPage.Data.Models;
using Dapper;
using InfluxDB.Client;
using InfluxDB.Client.Writes;
using System.Diagnostics;

namespace ITS.CPER.WebPage.Data.Services;

public class DataAccess : IDataAccess
{
    private readonly string _connectionDb;
    private readonly string _influxToken;
    private readonly string _bucket;
    private readonly string _org;
    public DataAccess(IConfiguration configuration)
    {
        _connectionDb = configuration.GetConnectionString("DbConnection");
        _influxToken = configuration.GetConnectionString("InfluxToken");
        _bucket = configuration.GetConnectionString("Bucket");
        _org = configuration.GetConnectionString("Org");
    }

    public async Task<SmartWatch_Data> GetSmartWatchDataAsync(Guid id)
    {
        using var connection = new SqlConnection(_connectionDb);
        connection.Open();
        SqlCommand sql = connection.CreateCommand();
        sql.CommandText = @"
            SELECT [SmartWatch_Id]
                ,[Activity_Id]
                ,[Initial_Latitude]
                ,[Initial_Longitude]
                ,[Distance]
                ,[NumberOfPoolLaps]
                ,[Final_Latitude]
                ,[Final_Longitude]
            FROM [dbo].[SmartWatches]
            WHERE FK_UserId = @id
            ";
        sql.Parameters.AddWithValue("@id", id);
        sql.ExecuteNonQuery();
        var result = new SmartWatch_Data();
        using (SqlDataReader reader = sql.ExecuteReader())
        {
            while (reader.Read())
            {
                SmartWatch_Data smartWatch = new SmartWatch_Data
                {
                    SmartWatch_Id = Guid.Parse((string)reader["SmartWatch_Id"]),
                    Activity_Id = Guid.Parse((string)reader["Activity_Id"]),
                    Initial_Latitude = Convert.ToDouble(reader["Initial_Latitude"]),
                    Initial_Longitude = Convert.ToDouble(reader["Initial_Longitude"]),
                    Distance = Convert.ToDouble(reader["Distance"]),
                    NumberOfPoolLaps = Convert.ToInt32(reader["NumberOfPoolLaps"]),
                    Final_Latitude = Convert.ToDouble(reader["Final_Latitude"]),
                    Final_Longitude = Convert.ToDouble(reader["Final_Longitude"]),
                };
                result = smartWatch;
            }
            reader.Close();
        }
            return result;
    }

    public void InsertNewUser(Guid id)
    {
        using var connection = new SqlConnection(_connectionDb);
        connection.Open();
        SqlCommand sql = connection.CreateCommand();
        sql.CommandText = @"
            INSERT INTO [dbo].[UserDetails]([User_Id],[SmartWatch_Id])VALUES(@id,@guid)";
        sql.Parameters.AddWithValue("@id", id);
        sql.Parameters.AddWithValue("@guid",Guid.NewGuid());
        sql.ExecuteNonQuery();
    }
    public void HeartbeatQuery(SmartWatch_Data data)
    {
        using var client = new InfluxDBClient("https://westeurope-1.azure.cloud2.influxdata.com", _influxToken);

        var flux = "from(bucket:\"SmartWatches\") " +
            "|> range(start: 0) \r\n" +
            "|> filter(fn: (r) => r[\"_measurement\"] == \"smartwatches\")\r\n  " +
            $"|> filter(fn: (r) => r[\"Activity_Id\"] == {data.Activity_Id})\r\n  " +
            $"|> filter(fn: (r) => r[\"SmartWatch_Id\"] == {data.SmartWatch_Id})\r\n  " +
            "|> filter(fn: (r) => r[\"_field\"] == \"Heartbeat\")";

        var select = client.GetQueryApi();
        var prova = select.QueryAsync(flux);
        var a = 0;
    }

    public Guid GetUserId(string userName)
    {
        using var connection = new SqlConnection(_connectionDb);
        connection.Open();
        SqlCommand sql = connection.CreateCommand();
        sql.CommandText = $"SELECT [Id] FROM [dbo].[AspNetUsers] WHERE UserName = @userName";
        sql.Parameters.AddWithValue("@userName", userName);
        var result = new Guid();
        using (SqlDataReader reader = sql.ExecuteReader())
        {
            while (reader.Read())
            {
                result = Guid.Parse((string)reader["Id"]);
                
            }
            reader.Close();
        }
        return result;
    }
}
