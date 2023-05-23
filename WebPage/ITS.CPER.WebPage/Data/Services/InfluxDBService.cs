using System;
using System.Threading.Tasks;
using InfluxDB.Client;
using Microsoft.Extensions.Configuration;

namespace ITS.CPER.WebPage.Data.Services;

public class InfluxDBService
{
    private readonly string _influxToken;
    private readonly string _bucket;
    private readonly string _org;
    private readonly string _connectionString;

    public InfluxDBService(IConfiguration configuration)
    {
        _influxToken = configuration.GetConnectionString("InfluxToken");
        _bucket = configuration.GetConnectionString("Bucket");
        _org = configuration.GetConnectionString("Org");
        _connectionString = configuration.GetConnectionString("db");
    }

    public async Task<T> QueryAsync<T>(Func<QueryApi, Task<T>> action)
    {
        using var client = new InfluxDBClient("https://westeurope-1.azure.cloud2.influxdata.com", _influxToken);
        var query = client.GetQueryApi();
        return await action(query);
    }
}
