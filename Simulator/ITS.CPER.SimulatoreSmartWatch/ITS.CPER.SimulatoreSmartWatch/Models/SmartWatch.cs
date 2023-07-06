using System.Text;

namespace ITS.CPER.SimulatoreSmartWatch.Models;

public class SmartWatch
{
    public Guid SmartWatch_Id { get; set; }
    public Guid Activity_Id { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int Heartbeat { get; set; }
    public int NumberOfPoolLaps { get; set; }
    public double Distance { get; set; }
    public Guid User_Id { get; set; }

    public async Task ApiPost(SmartWatch details)
    {
        var client = new HttpClient();
        var apiUrl = new Uri("https://its-cper-g4-senddataqueue.azurewebsites.net/api/Function1?code=-VjKK5tw1POHjoqqNmWGD-fwE1bag2K2Ylm4U4L10MnMAzFuUUyA1g==");

        var json = System.Text.Json.JsonSerializer.Serialize(details);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(apiUrl, content);
        response.EnsureSuccessStatusCode();
    }
}
