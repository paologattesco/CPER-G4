using Azure.Storage.Queues;
using ITS.CPER.SendDataQueue.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Text.Json;

namespace ITS.CPER.SendDataQueue.Service;

public class QueueService
{
    private readonly IConfiguration _configuration;
    private readonly QueueClient _queueClient;
    public QueueService(IConfiguration configuration)
    {
        _configuration = configuration;
        var cs = configuration.GetConnectionString("storage");
        _queueClient = new QueueClient(cs, "queueg4");
        _queueClient.CreateIfNotExists();
    }
    public void Send(SmartWatch_Data newdata)
    {
        var data = JsonSerializer.Serialize(newdata);
        var sender_data = Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(data));
        _queueClient.SendMessage(sender_data);
    }
}
