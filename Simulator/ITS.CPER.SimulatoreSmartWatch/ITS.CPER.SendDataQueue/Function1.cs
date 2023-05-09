using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using ITS.CPER.SendDataQueue.Service;
using ITS.CPER.SendDataQueue.Models;
using Azure;

namespace ITS.CPER.SendDataQueue;

public class Function1
{
    private readonly IConfiguration _configuration;
    public Function1(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    [FunctionName("Function1")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");
        string guid = req.Query["Guid"];
        string latitude = req.Query["Latitude"];
        string longitude = req.Query["Longitude"];
        string heartbeat = req.Query["Heartbeat"];
        string pools = req.Query["NumberOfPoolLaps"];
  
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);

        guid = guid ?? data?.Guid;
        latitude = latitude ?? data?.Latitude;
        longitude = longitude ?? data?.Longitude;
        heartbeat = heartbeat ?? data?.Heartbeat;
        pools = pools ?? data?.NumberOfPoolLaps;

        latitude = latitude.Replace(".", ",");
        longitude = longitude.Replace(".", ",");

        string responseMessage = string.IsNullOrEmpty(null)
            ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            : $"Hello This HTTP triggered function executed successfully.";
        QueueService queue = new QueueService(_configuration);
        SmartWatch_Data newData = new SmartWatch_Data()
        {
            Guid = Guid.Parse(guid),
            Latitude = Convert.ToDouble(latitude),
            Longitude = Convert.ToDouble(longitude),
            Heartbeat = Convert.ToInt32(heartbeat),
            NumberOfPoolLaps = Convert.ToInt32(pools)

        };
        queue.Send(newData);
        return new OkObjectResult(responseMessage);
    }

}
