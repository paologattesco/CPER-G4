using System;
using System.Text;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using ITS.CPER.DataDequeue.Models;

namespace ITS.CPER.DataDequeue;

public class Function1
{
    [FunctionName("Function1")]
    public void Run([QueueTrigger("queueg4", Connection = "storage")] byte[] encryptedmessage, ILogger log)
    {
        var dateToConvert = Encoding.UTF8.GetString(encryptedmessage);
        var date = JsonSerializer.Deserialize<SmartWatch_Data>(dateToConvert);
        Console.WriteLine(date);
        log.LogInformation($"C# Queue trigger function processed: {date}");
    }
}
