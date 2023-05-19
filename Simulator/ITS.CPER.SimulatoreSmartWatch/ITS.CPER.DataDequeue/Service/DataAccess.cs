using Google.Protobuf.WellKnownTypes;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using ITS.CPER.DataDequeue.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Globalization;

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
        SqlCommand sql = (SqlCommand)connection.CreateCommand();
        
        sql.CommandText = $"INSERT INTO [dbo].[SmartWatches]([SmartWatch_Id],[Activity_Id],[Initial_Latitude],[Initial_Longitude],[Distance],[NumberOfPoolLaps],[Final_Latitude],[Final_Longitude])VALUES(@SmartWatch_Id,@Activity_Id,@Initial_Latitude,@Initial_Longitude,@Distance,@NumberOfPoolLaps,@Final_Latitude,@Final_Longitude)";
        sql.Parameters.AddWithValue("@SmartWatch_Id", data.SmartWatch_Id);
        sql.Parameters.AddWithValue("@Activity_Id", data.Activity_Id);
        sql.Parameters.AddWithValue("@Initial_Latitude", data.Latitude);
        sql.Parameters.AddWithValue("@Initial_Longitude", data.Longitude);
        sql.Parameters.AddWithValue("@Distance", data.Distance);
        sql.Parameters.AddWithValue("@NumberOfPoolLaps", data.NumberOfPoolLaps);
        sql.Parameters.AddWithValue("@Final_Latitude", data.Latitude);
        sql.Parameters.AddWithValue("@Final_Longitude", data.Longitude);
        sql.ExecuteNonQuery();
    }

    public void InsertInfluxDb(SmartWatch_Data data)
    {
        using var client = new InfluxDBClient("https://westeurope-1.azure.cloud2.influxdata.com", _influxToken);
        
        var query = $"smartwatches,Smartwatch_Id={data.SmartWatch_Id},Activity_Id={data.Activity_Id} " +
                    $"Latitude={data.Latitude.ToString(CultureInfo.InvariantCulture)},Longitude={data.Longitude.ToString(CultureInfo.InvariantCulture)},Heartbeat={data.Heartbeat},NumberOfPoolLaps={data.NumberOfPoolLaps}";
        using (var writeApi = client.GetWriteApi())
        {
            writeApi.WriteRecord(query, WritePrecision.Ns, _bucket, _org);
        }
        client.Dispose();
    }
}
