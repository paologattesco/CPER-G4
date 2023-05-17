using System;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using ITS.CPER.DataDequeue.Models;
using ITS.CPER.DataDequeue.Service;
using Microsoft.Azure.Functions.Worker;

namespace ITS.CPER.DataDequeue;

public class Function1
{
    private readonly IDataAccess _dataAccess;
    private readonly ILogger<Function1> _logger;

    public Function1(IDataAccess dataAccess, ILogger<Function1> logger)
    {
        _dataAccess = dataAccess;
        _logger = logger;
    }

    [Function("Function1")]
    public void Run([QueueTrigger("queueg4", Connection = "storage")] byte[] encryptedmessage)
    {
        var dateToConvert = Encoding.UTF8.GetString(encryptedmessage);
        var date = JsonSerializer.Deserialize<SmartWatch_Data>(dateToConvert);
        _logger.LogInformation($"C# Queue trigger function processed: {date}");

        _dataAccess.InsertHeartbeat(date.SmartWatchId,date.Heartbeat);
    }
}
