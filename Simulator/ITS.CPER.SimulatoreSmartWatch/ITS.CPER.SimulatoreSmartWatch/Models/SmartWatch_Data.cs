using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ITS.CPER.SimulatoreSmartWatch.Models;

public class SmartWatch_Data
{
    public Guid SmartWatchId { get; set; }
    public Guid ActivityGuid { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int Heartbeat { get; set; }
    public int NumberOfPoolLaps { get; set; }

    public async Task ApiPost(SmartWatch_Data details)
    {
        var client = new HttpClient();
        var apiUrl = new Uri("http://localhost:7210/api/Function1");

        var json = System.Text.Json.JsonSerializer.Serialize(details);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(apiUrl, content);
        response.EnsureSuccessStatusCode();
    }    
}
