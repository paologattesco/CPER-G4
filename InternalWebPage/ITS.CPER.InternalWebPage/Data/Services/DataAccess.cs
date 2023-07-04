using Microsoft.Data.SqlClient;
using ITS.CPER.InternalWebPage.Data.Models;
using InfluxDB.Client;
using NodaTime;
using Npgsql;
using InfluxDB.Client.Core.Flux.Domain;

namespace ITS.CPER.InternalWebPage.Data.Services;

public class DataAccess : IDataAccess
{
    private readonly string _connectionDb;
    private readonly string _influxToken;
    private readonly string _bucket;
    private readonly string _org;
    private readonly string _host;
    private readonly string _user;
    private readonly string _dbname;
    private readonly string _password;
    private readonly string _port;
    public DataAccess(IConfiguration configuration)
    {
        _connectionDb = configuration.GetConnectionString("DbConnection");
        _influxToken = configuration.GetConnectionString("InfluxToken");
        _bucket = configuration.GetConnectionString("Bucket");
        _org = configuration.GetConnectionString("Org");
        _host = configuration.GetConnectionString("Host");
        _user = configuration.GetConnectionString("User");
        _dbname = configuration.GetConnectionString("Dbname");
        _password = configuration.GetConnectionString("Password");
        _port = configuration.GetConnectionString("Port");
    }

    public Task<Dictionary<Guid, Guid>> GetProductionBatch()
    {
        string connString = $"User ID={_user};Password={_password};Host={_host};Port={_port};Database={_dbname};";
        Guid smartwatch_id;
        Guid batch_id;
        Dictionary<Guid,Guid> ListOfPb = new Dictionary<Guid,Guid>();
        using (var conn = new NpgsqlConnection(connString))
        {
            conn.Open();
            var controlDate = new NpgsqlCommand("SELECT smartwatch_id, batch_id FROM ProductionBatch", conn);
            controlDate.ExecuteNonQuery();

            using (NpgsqlDataReader reader = controlDate.ExecuteReader())
            {
                while (reader.Read())
                {
                    smartwatch_id = Guid.Parse((string)reader["smartwatch_id"]);
                    batch_id = Guid.Parse((string)reader["batch_id"]);
                    ListOfPb.Add(smartwatch_id, batch_id);
                }
                reader.Close();
            }
        }
        return Task.FromResult(ListOfPb);
    }
    public async Task<List<SmartWatch>> GetSmartWatchesDataAsync()
    {
        using var connection = new SqlConnection(_connectionDb);
        connection.Open();
        SqlCommand sql = connection.CreateCommand();
        sql.CommandText = @"
            SELECT a.FK_SmartWatch_Id
            ,a.Id
            ,a.Initial_Latitude
            ,a.Initial_Longitude
            ,a.Final_Latitude
            ,a.Final_Longitude
            FROM [dbo].[Activities] AS a
            JOIN [dbo].[SmartWatches] AS s on (a.FK_SmartWatch_Id = s.Id)
            ";
        sql.ExecuteNonQuery();
        var result = new List<SmartWatch>();
        using (SqlDataReader reader = sql.ExecuteReader())
        {
            while (reader.Read())
            {
                SmartWatch smartWatch = new SmartWatch
                {
                    SmartWatch_Id = Guid.Parse((string)reader["FK_SmartWatch_Id"]),
                    Activity_Id = Guid.Parse((string)reader["Id"]),
                    Initial_Latitude = Convert.ToDouble(reader["Initial_Latitude"]),
                    Initial_Longitude = Convert.ToDouble(reader["Initial_Longitude"]),
                    Final_Latitude = Convert.ToDouble(reader["Final_Latitude"]),
                    Final_Longitude = Convert.ToDouble(reader["Final_Longitude"])
                };
                result.Add(smartWatch);
            }
            reader.Close();
        }
        return result;
    }

    public async Task<List<Activity>> GetActivitiesAsync(Guid id)
    {
        using var connection = new SqlConnection(_connectionDb);
        connection.Open();
        SqlCommand sql = connection.CreateCommand();
        sql.CommandText = @"
            SELECT a.FK_SmartWatch_Id
            ,a.Id
            ,a.Initial_Latitude
            ,a.Initial_Longitude
            ,a.Final_Latitude
            ,a.Final_Longitude
            FROM [dbo].[Activities] AS a
            JOIN [dbo].[SmartWatches] AS s on (a.FK_SmartWatch_Id = s.Id)
            WHERE a.FK_SmartWatch_Id = @id
            ";
        sql.Parameters.AddWithValue("@id", id);
        sql.ExecuteNonQuery();
        
        var result = new List<Activity>();
        using (SqlDataReader reader = sql.ExecuteReader())
        {
            while (reader.Read())
            {
                Activity activity = new Activity
                {
                    Activity_Id = Guid.Parse((string)reader["Id"]),
                    Initial_Latitude = Convert.ToDouble(reader["Initial_Latitude"]),
                    Initial_Longitude = Convert.ToDouble(reader["Initial_Longitude"]),
                    Final_Latitude = Convert.ToDouble(reader["Final_Latitude"]),
                    Final_Longitude = Convert.ToDouble(reader["Final_Longitude"])
                };
                result.Add(activity);
            }
            reader.Close();
        }
        return result;
    }

    public async Task<List<HeartBeat>> HeartbeatQuery(SmartWatch data)
    {
        using var client = new InfluxDBClient("https://westeurope-1.azure.cloud2.influxdata.com/", _influxToken);
        var activity_id = Convert.ToString(data.Activity_Id);
        var smartwatch_id = Convert.ToString(data.SmartWatch_Id);
        var flux = "from(bucket:\"SmartWatches\") " +
            "|> range(start: 0) \r\n" +
            "|> filter(fn: (r) => r[\"_measurement\"] == \"smartwatches\")\r\n  " +
            $"|> filter(fn: (r) => r[\"Activity_Id\"] == \"{activity_id}\")\r\n  " +
            $"|> filter(fn: (r) => r[\"SmartWatch_Id\"] == \"{smartwatch_id}\")\r\n  " +
            "|> filter(fn: (r) => r[\"_field\"] == \"Heartbeat\")\r\n  " +
            "|> keep(columns: [\"_time\", \"_value\"])\r\n  ";

        var queryApi = client.GetQueryApi();
        var fluxTables = await queryApi.QueryAsync(flux, _org);

        var heartbeat = new List<HeartBeat>();
        fluxTables.ForEach(fluxTable =>
        {
            var fluxRecords = fluxTable.Records;

            fluxRecords.ForEach(fluxRecord =>
            {
                HeartBeat newHearbeat = new HeartBeat()
                {
                    Time = (Instant)fluxRecord.GetTime(),
                    Heartbeat = Convert.ToInt32(fluxRecord.GetValue())

                };
                heartbeat.Add(newHearbeat);

            });
        });
        return heartbeat;
    }

    //QUERY INFLUX PER TUTTE LE ATTIVITA'
    //LONGITUDE è MESSO SUL BUCKET COME LONIGITUDE...è GIA' STATO CAMBIATA LA INSERT SUL SIMULATORE
    public async Task<List<Activity>> ActivitiesQuery(SmartWatch data)
    {
        using var client = new InfluxDBClient("https://westeurope-1.azure.cloud2.influxdata.com/", _influxToken);
        var activity_id = Convert.ToString(data.Activity_Id);
        var smartwatch_id = Convert.ToString(data.SmartWatch_Id);
        var HeartBeatQuery = "from(bucket:\"SmartWatches\") " +
            "|> range(start: 0) \r\n" +
            "|> filter(fn: (r) => r[\"_measurement\"] == \"smartwatches\")\r\n  " +
            $"|> filter(fn: (r) => r[\"Activity_Id\"] == \"{activity_id}\")\r\n  " +
            $"|> filter(fn: (r) => r[\"SmartWatch_Id\"] == \"{smartwatch_id}\")\r\n  " +
            "|> filter(fn: (r) => r[\"_field\"] == \"Heartbeat\")\r\n  " +
            "|> keep(columns: [\"_time\", \"_value\"])\r\n  ";

        var queryApi = client.GetQueryApi();
        var fluxTables = await queryApi.QueryAsync(HeartBeatQuery, _org);

        var Heartbeat = new List<int>();
        fluxTables.ForEach(fluxTable =>
        {
            var fluxRecords = fluxTable.Records;

            fluxRecords.ForEach(fluxRecord =>
            {

                Heartbeat.Add(Convert.ToInt32(fluxRecord.GetValue()));

            });
        });

        var FinalLongitude = "from(bucket:\"SmartWatches\") " +
           "|> range(start: 0) \r\n" +
           "|> filter(fn: (r) => r[\"_measurement\"] == \"smartwatches\")\r\n  " +
           $"|> filter(fn: (r) => r[\"Activity_Id\"] == \"{activity_id}\")\r\n  " +
           $"|> filter(fn: (r) => r[\"SmartWatch_Id\"] == \"{smartwatch_id}\")\r\n  " +
           "|> filter(fn: (r) => r[\"_field\"] == \"Lonigitude\")\r\n  " +
           "|> keep(columns: [\"_time\", \"_value\"])\r\n  ";

        queryApi = client.GetQueryApi();
        fluxTables = await queryApi.QueryAsync(FinalLongitude, _org);

        var Longitude = new List<double>();
        fluxTables.ForEach(fluxTable =>
        {
            var fluxRecords = fluxTable.Records;

            fluxRecords.ForEach(fluxRecord =>
            {

                Longitude.Add(Convert.ToInt32(fluxRecord.GetValue()));

            });
        });

        var FinalLatitude = "from(bucket:\"SmartWatches\") " +
          "|> range(start: 0) \r\n" +
          "|> filter(fn: (r) => r[\"_measurement\"] == \"smartwatches\")\r\n  " +
          $"|> filter(fn: (r) => r[\"Activity_Id\"] == \"{activity_id}\")\r\n  " +
          $"|> filter(fn: (r) => r[\"SmartWatch_Id\"] == \"{smartwatch_id}\")\r\n  " +
          "|> filter(fn: (r) => r[\"_field\"] == \"Latitude\")\r\n  " +
          "|> keep(columns: [\"_time\", \"_value\"])\r\n  ";

        queryApi = client.GetQueryApi();
        fluxTables = await queryApi.QueryAsync(FinalLatitude, _org);

        var Latitude = new List<double>();
        fluxTables.ForEach(fluxTable =>
        {
            var fluxRecords = fluxTable.Records;

            fluxRecords.ForEach(fluxRecord =>
            {

                Latitude.Add(Convert.ToInt32(fluxRecord.GetValue()));

            });
        });
        List<Activity> activity = new List<Activity>();
        for (int i = 0; i < Heartbeat.Count(); i++)
        {
            Activity NewActivity = new Activity()
            {
                Activity_Id = data.Activity_Id,
                Final_Latitude = Latitude[i],
                Final_Longitude = Longitude[i],
                Heartbeat = Heartbeat[i]
            };
            activity.Add(NewActivity);
        }
       
        return activity;
    }
}
