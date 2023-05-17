﻿using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ITS.CPER.DataDequeue.Service;

public class DataAccess : IDataAccess
{
    private readonly string _InfluxToken;
    private readonly string _bucket;
    private readonly string _org;

    public DataAccess(IConfiguration configuration)
    {
        _InfluxToken = configuration.GetConnectionString("InfluxToken");
        _bucket = configuration.GetConnectionString("Bucket");
        _org = configuration.GetConnectionString("Org");
    }
    public void InsertHeartBeat(int Heartbeat)
    {
        using var client = new InfluxDBClient("https://westeurope-1.azure.cloud2.influxdata.com", _InfluxToken);
        string prova = $"SimulatoDB,heartbeat={Heartbeat}";
        using (var writeApi = client.GetWriteApi())
        {
            writeApi.WriteRecord(prova,WritePrecision.Ns, _bucket, _org);
        }
    }
}
