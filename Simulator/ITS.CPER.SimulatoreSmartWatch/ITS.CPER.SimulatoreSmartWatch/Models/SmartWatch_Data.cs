using ITS.CPER.SimulatoreSmartWatch.Service;
using System.Text;

namespace ITS.CPER.SimulatoreSmartWatch.Models;

public class SmartWatch_Data
{
    private readonly IDataAccess _dataAccess;
    public SmartWatch_Data(IDataAccess dataAccess)
    {
        _dataAccess = dataAccess;
    }
    public Guid SmartWatch_Id { get; set; }
    public Guid Activity_Id { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int Heartbeat { get; set; }
    public int NumberOfPoolLaps { get; set; }
    public double Distance { get; set; }
    public async Task ApiPost(SmartWatch_Data details)
    {
        var client = new HttpClient();
        var apiUrl = new Uri("http://localhost:7210/api/Function1");

        var json = System.Text.Json.JsonSerializer.Serialize(details);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(apiUrl, content);
        response.EnsureSuccessStatusCode();
    }
    public Dictionary<int, Guid> DictionaryOfSmartWatches()
    {
        Dictionary<int, Guid> serialNumber = new Dictionary<int, Guid>();
        var smartwatches = _dataAccess.ListOfSmartWatches();
        for (int i = 0; i < smartwatches.Count; i++)
        {
            serialNumber.Add(i, smartwatches[i]);
        }
        return serialNumber;
    }
}
