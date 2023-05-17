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

    public void InsertData(SmartWatch_Data data)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        SqlCommand sql = (SqlCommand)connection.CreateCommand();
        sql.CommandText = $"INSERT INTO [dbo].[SmartWatches]([SmartWatch_Id],[Activity_Id],[Latitude],[Longitude],[HeartBeat],[NumberOfPoolLaps])VALUES(@SmartWatch_Id,@Activity_Id,@Latitude,@Longitude,@HeartBeat,@NumberOfPoolLaps)";
        sql.Parameters.AddWithValue("@SmartWatch_Id", data.SmartWatch_Id);
        sql.Parameters.AddWithValue("@Activity_Id", data.Activity_Id);
        sql.Parameters.AddWithValue("@Latitude", data.Latitude);
        sql.Parameters.AddWithValue("@Longitude", data.Longitude);
        sql.Parameters.AddWithValue("@HeartBeat", data.Heartbeat);
        sql.Parameters.AddWithValue("@NumberOfPoolLaps", data.NumberOfPoolLaps);
        sql.ExecuteNonQuery();
    }

    public void InsertHeartbeat(SmartWatch_Data data)
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
