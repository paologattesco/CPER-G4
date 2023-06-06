using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using static MudBlazor.CategoryTypes;

namespace ITS.CPER.WebPage.Data.Models;

public class Address
{
    public string? country { get; set; }
    public string? state { get; set; }
    public string? city { get; set; }


    public async Task<Address> GetAddress(double lat, double lon)
    {
        Address address = new Address(); 
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri($"https://api.geoapify.com/v1/geocode/reverse?lat={lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}&lon={lon.ToString(System.Globalization.CultureInfo.InvariantCulture)}&apiKey=ebce859ec6564aa0a534a22be6361d1c");

            // Setting content type.  
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Initialization.  
            HttpResponseMessage response = new HttpResponseMessage();

            // HTTP GET  
            response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                JObject jObj = JObject.Parse(result);

                address.country = (string?)jObj.SelectToken("features[0].properties.country");
                address.state = (string?)jObj.SelectToken("features[0].properties.state");
                address.city = (string?)jObj.SelectToken("features[0].properties.city");

            }
        }
        return address;
    }
}




