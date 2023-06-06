using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using static MudBlazor.CategoryTypes;

namespace ITS.CPER.WebPage.Data.Models;

public class Address
{
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? Country_Code { get; set; }
    public string? Suburb { get; set; }
    public string? Road { get; set; }
    public string? State_District { get; set; }
}
public class RootObject
{
    [DataMember]
    public string place_id { get; set; }
    [DataMember]
    public string licence { get; set; }
    [DataMember]
    public string osm_type { get; set; }
    [DataMember]
    public string osm_id { get; set; }
    [DataMember]
    public string lat { get; set; }
    [DataMember]
    public string lon { get; set; }
    [DataMember]
    public string display_name { get; set; }
    [DataMember]
    public Address address { get; set; }
    public async void GetAddress(double lat, double lon)
    {
        var client = new HttpClient();
        var apiUrl = new Uri($"http://nominatim.openstreetmap.org/reverse?format=json&lat={lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}&lon={lon.ToString(System.Globalization.CultureInfo.InvariantCulture)}");

        //var response = await client.GetAsync(apiUrl);
        //response.EnsureSuccessStatusCode();
        //var content = response.Content.ReadAsStringAsync();
        //var a = 0;
        //var json = System.Text.Json.JsonSerializer.Serialize(details);
        //var content = new StringContent(json, Encoding.UTF8, "application/json");

        //var response = await client.PostAsync(apiUrl, content);
        //response.EnsureSuccessStatusCode();
    }
}

