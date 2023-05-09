using ITS.CPER.SimulatoreSmartWatch.Models;
using Microsoft.Extensions.Configuration;
using System.Timers;
//using ITS.CPER.SendDataQueue.Models;
namespace ITS.CPER.SimulatoreSmartWatch.Data;


public class GeneratorOfCoordinates
{
    private IConfiguration configuration;
    const double MIN_LATITUDE = -90.0;
    const double MAX_LATITUDE = 90.0;
    const double MIN_LONGITUDE = -180.0;
    const double MAX_LONGITUDE = 180.0;
    const double MAX_DISTANCE = 50.0;
    const double EARTH_RADIUS = 6371000.0; // in meters
    private System.Timers.Timer TimerForData = new System.Timers.Timer();
    private bool endTraining = false;
    private bool isStarted = false;
    public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        // HAVERSINE FORMULA
        double dLat = (lat2 - lat1) * Math.PI / 180;
        double dLon = (lon2 - lon1) * Math.PI / 180;

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        double dist = Math.Round(EARTH_RADIUS * c, 2);

        return dist;
    }

    // RECURSIVE FUNCTION TO GENERATE COORDINATES WITHIN 0 TO 50 METERS DISTANCE
    public void GenerateCoordinates(double latitude, double longitude)
    {
        TimerForData.Elapsed += new ElapsedEventHandler(SendData);
        TimerForData.Interval = 10010;
        TimerForData.Enabled = true;

        // FIRST COORDINATES
        if (latitude == 0 && longitude == 0)
        {
            Random rnd = new Random();
            latitude = Math.Round(rnd.NextDouble() * (MAX_LATITUDE - MIN_LATITUDE) + MIN_LATITUDE, 6);
            longitude = Math.Round(rnd.NextDouble() * (MAX_LONGITUDE - MIN_LONGITUDE) + MIN_LONGITUDE, 6);
        }

        // SECOND COORDINATES (DISTANCE BETWEEN 0 AND 50)
        Random rndDistance = new Random();
        double pool_distance = Math.Round(rndDistance.NextDouble() * MAX_DISTANCE, 2);
        double angle = Math.Round(rndDistance.NextDouble() * 360, 2);
        double latitude2 = Math.Round(Math.Asin(Math.Sin(latitude * Math.PI / 180) * Math.Cos(pool_distance / EARTH_RADIUS) + Math.Cos(latitude * Math.PI / 180) * Math.Sin(pool_distance / EARTH_RADIUS) * Math.Cos(angle * Math.PI / 180)) * 180 / Math.PI, 6);
        double longitude2 = Math.Round((longitude * Math.PI / 180 + Math.Atan2(Math.Sin(angle * Math.PI / 180) * Math.Sin(pool_distance / EARTH_RADIUS) * Math.Cos(latitude * Math.PI / 180), Math.Cos(pool_distance / EARTH_RADIUS) - Math.Sin(latitude * Math.PI / 180) * Math.Sin(latitude2 * Math.PI / 180))) * 180 / Math.PI, 6);

        // DISTANCE
        double distance = CalculateDistance(latitude, longitude, latitude2, longitude2);

        Heartbeat heartbeat = new Heartbeat();
        if (!isStarted)
        {
            Console.WriteLine($"Latitude: {latitude}");
            Console.WriteLine($"Longitude: {longitude}");
            isStarted = true;
        }

        Thread.Sleep(10000);
        Console.WriteLine($"\nLatitude: {latitude2}");
        Console.WriteLine($"Longitude: {longitude2}");
        Console.WriteLine($"Distance: {distance} meters");
        var heartbeatunderpression = heartbeat.HeartbeatUnderPressure();
        Console.WriteLine($"Pulse rate: {heartbeatunderpression} bpm");

        SmartWatch_Data newData = new SmartWatch_Data()
        {
            Guid = Guid.NewGuid(),
            Latitude = latitude2,
            Longitude = longitude2,
            Heartbeat = heartbeatunderpression,
            NumberOfPoolLaps = 0
        };

        if (endTraining)
        {
            isStarted = false;
            return;
        }
        else
        {
            GenerateCoordinates(latitude2, longitude2);
        }
        while (endTraining != true) ;
    }

    void SendData(object? sender, ElapsedEventArgs e)
    {
        endTraining = true;
    }
    
}
