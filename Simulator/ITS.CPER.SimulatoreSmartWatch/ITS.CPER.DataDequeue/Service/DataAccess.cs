﻿using Google.Protobuf.WellKnownTypes;
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
        
        sql.CommandText = $"INSERT INTO [dbo].[SmartWatches]([SmartWatch_Id],[Activity_Id],[Initial_Latitude],[Initial_Longitude],[Distance],[NumberOfPoolLaps],[Final_Latitude],[Final_Longitude],[FK_UserId])VALUES(@SmartWatch_Id,@Activity_Id,@Initial_Latitude,@Initial_Longitude,@Distance,@NumberOfPoolLaps,@Final_Latitude,@Final_Longitude,@User_Id)";
        sql.Parameters.AddWithValue("@SmartWatch_Id", data.SmartWatch_Id);
        sql.Parameters.AddWithValue("@Activity_Id", data.Activity_Id);
        sql.Parameters.AddWithValue("@Initial_Latitude", data.Latitude);
        sql.Parameters.AddWithValue("@Initial_Longitude", data.Longitude);
        sql.Parameters.AddWithValue("@Distance", data.Distance);
        sql.Parameters.AddWithValue("@NumberOfPoolLaps", data.NumberOfPoolLaps);
        sql.Parameters.AddWithValue("@Final_Latitude", data.Latitude);
        sql.Parameters.AddWithValue("@Final_Longitude", data.Longitude);
        sql.Parameters.AddWithValue("@User_Id", data.User_Id);
        sql.ExecuteNonQuery();
    }

    public void UpdateActivity(SmartWatch_Data data, Guid ActivityId)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        SqlCommand sql = connection.CreateCommand();
        sql.CommandText = $"UPDATE [dbo].[SmartWatches] SET Distance = @Distance, NumberOfPoolLaps = @NumberOfPoolLaps, Final_Latitude = @Final_Latitude, Final_Longitude = @Final_Longitude WHERE Activity_Id = @Activity_Id";
        sql.Parameters.AddWithValue("@Distance", data.Distance);
        sql.Parameters.AddWithValue("@NumberOfPoolLaps", data.NumberOfPoolLaps);
        sql.Parameters.AddWithValue("@Final_Latitude", data.Latitude);
        sql.Parameters.AddWithValue("@Final_Longitude", data.Longitude);
        sql.Parameters.AddWithValue("@Activity_Id", ActivityId);
        sql.ExecuteNonQuery();
    }
    public bool GetActivityId(Guid ActivityId)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        SqlCommand sql = connection.CreateCommand();
        sql.CommandText = $"SELECT COUNT(*) FROM [dbo].[SmartWatches] WHERE Activity_Id=@Activity_Id";
        sql.Parameters.AddWithValue("@Activity_Id", ActivityId);
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
