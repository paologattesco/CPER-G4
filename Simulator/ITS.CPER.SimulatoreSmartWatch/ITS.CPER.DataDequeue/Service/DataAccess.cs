using Google.Protobuf.WellKnownTypes;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using ITS.CPER.DataDequeue.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Drawing;
using InfluxDB.Client.Writes;
using System;
using System.Collections.Generic;

namespace ITS.CPER.DataDequeue.Service;

public class DataAccess : IDataAccess
{
    private readonly string _influxToken;
    private readonly string _bucket;
    private readonly string _org;
    private readonly string _connectionString;

    public DataAccess(IConfiguration configuration)
    {
        _influxToken = configuration.GetConnectionString("InfluxToken");
        _bucket = configuration.GetConnectionString("Bucket");
        _org = configuration.GetConnectionString("Org");
        _connectionString = configuration.GetConnectionString("db");
    }

    public void InsertSqlManagement(SmartWatch_Data data)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        SqlCommand sql = connection.CreateCommand();
        
        sql.CommandText = $"INSERT INTO [dbo].[Activities]([Id],[Initial_Latitude],[Initial_Longitude],[Distance],[NumberOfPoolLaps],[Final_Latitude],[Final_Longitude],[FK_SmartWatch_Id])VALUES(@Id,@Initial_Latitude,@Initial_Longitude,@Distance,@NumberOfPoolLaps,@Final_Latitude,@Final_Longitude,@FK_SmartWatch_Id)";
        sql.Parameters.AddWithValue("@Id", data.Activity_Id);
        sql.Parameters.AddWithValue("@Initial_Latitude", data.Latitude);
        sql.Parameters.AddWithValue("@Initial_Longitude", data.Longitude);
        sql.Parameters.AddWithValue("@Distance", data.Distance);
        sql.Parameters.AddWithValue("@NumberOfPoolLaps", data.NumberOfPoolLaps);
        sql.Parameters.AddWithValue("@Final_Latitude", data.Latitude);
        sql.Parameters.AddWithValue("@Final_Longitude", data.Longitude);
        sql.Parameters.AddWithValue("@FK_SmartWatch_Id", data.SmartWatch_Id);
        sql.ExecuteNonQuery();
    }

    public void UpdateActivity(SmartWatch_Data data, Guid ActivityId)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        SqlCommand sql = connection.CreateCommand();
        sql.CommandText = $"UPDATE [dbo].[Activities] SET Distance = @Distance, NumberOfPoolLaps = @NumberOfPoolLaps, Final_Latitude = @Final_Latitude, Final_Longitude = @Final_Longitude WHERE Id = @Id";
        sql.Parameters.AddWithValue("@Distance", data.Distance);
        sql.Parameters.AddWithValue("@NumberOfPoolLaps", data.NumberOfPoolLaps);
        sql.Parameters.AddWithValue("@Final_Latitude", data.Latitude);
        sql.Parameters.AddWithValue("@Final_Longitude", data.Longitude);
        sql.Parameters.AddWithValue("@Id", ActivityId);
        sql.ExecuteNonQuery();
    }
    public bool GetActivityId(Guid ActivityId)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        SqlCommand sql = connection.CreateCommand();
        sql.CommandText = $"SELECT COUNT(*) FROM [dbo].[Activities] WHERE Id=@Id";
        sql.Parameters.AddWithValue("@Id", ActivityId);
        int count = (int)sql.ExecuteScalar();
        bool hasResults = count > 0;
        return hasResults;
    }
    public void InsertInfluxDb(SmartWatch_Data data)
    {
        using var client = new InfluxDBClient("https://westeurope-1.azure.cloud2.influxdata.com", _influxToken);

        var query = PointData.Measurement("smartwatches")
            .Tag("User_Id", data.User_Id.ToString())
            .Tag("SmartWatch_Id", data.SmartWatch_Id.ToString())
            .Tag("Activity_Id", data.Activity_Id.ToString())
            .Field("Latitude", data.Latitude)
            .Field("Lonigitude", data.Longitude)
            .Field("Heartbeat", data.Heartbeat)
            .Field("NumberOfPoolLaps", data.NumberOfPoolLaps)
            .Timestamp(DateTime.UtcNow, WritePrecision.Ns); 
        using (var writeApi = client.GetWriteApi())
        {
            writeApi.WritePoint(query, _bucket, _org);
        }
        client.Dispose();
    }

}
