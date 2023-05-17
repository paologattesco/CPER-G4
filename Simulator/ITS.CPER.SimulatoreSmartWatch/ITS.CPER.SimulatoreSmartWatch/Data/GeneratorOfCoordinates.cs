using ITS.CPER.SimulatoreSmartWatch.Models;
using System.Timers;

namespace ITS.CPER.SimulatoreSmartWatch.Data;

public class GeneratorOfCoordinates
{
    const double MIN_LATITUDE = -90.0;
    const double MAX_LATITUDE = 90.0;
    const double MIN_LONGITUDE = -180.0;
    const double MAX_LONGITUDE = 180.0;
    const double MAX_DISTANCE = 50.0;
    const double EARTH_RADIUS = 6371000.0; // in meters
    private System.Timers.Timer timerForData = new System.Timers.Timer();
    private bool endTraining = false;
    private bool isStarted = false;
    private double totalDistance = 0.0;
    Random rand = new Random();
    Heartbeat heartbeat = new Heartbeat();

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

    public (double, double) GenerateFirstCoordinates()
    {
        double latitude = Math.Round(rand.NextDouble() * (MAX_LATITUDE - MIN_LATITUDE) + MIN_LATITUDE, 6);
        double longitude = Math.Round(rand.NextDouble() * (MAX_LONGITUDE - MIN_LONGITUDE) + MIN_LONGITUDE, 6);
        return (latitude, longitude);
    }

    public (double, double) GenerateNextCoordinates(double lat, double lon)
    {
        double poolDistance = Math.Round(rand.NextDouble() * MAX_DISTANCE, 2);
        double angle = Math.Round(rand.NextDouble() * 360, 2);
        double latitude2 = Math.Round(Math.Asin(Math.Sin(lat * Math.PI / 180) * Math.Cos(poolDistance / EARTH_RADIUS) + Math.Cos(lat * Math.PI / 180) * Math.Sin(poolDistance / EARTH_RADIUS) * Math.Cos(angle * Math.PI / 180)) * 180 / Math.PI, 6);
        double longitude2 = Math.Round((lon * Math.PI / 180 + Math.Atan2(Math.Sin(angle * Math.PI / 180) * Math.Sin(poolDistance / EARTH_RADIUS) * Math.Cos(lat * Math.PI / 180), Math.Cos(poolDistance / EARTH_RADIUS) - Math.Sin(lat * Math.PI / 180) * Math.Sin(latitude2 * Math.PI / 180))) * 180 / Math.PI, 6);
        return (latitude2, longitude2);
    }

    public double CalculatePoolLaps(double totalDistance)
    {
        double poolLaps = totalDistance / MAX_DISTANCE;
        return Math.Floor(poolLaps); // Round down to the nearest integer
    }

    public void PostFirstData(double lat, double lon)
    {
        Console.WriteLine($"Latitude: {lat}");
        Console.WriteLine($"Longitude: {lon}");
        var heartbeatWithoutPressure = heartbeat.HeartbeatWithoutPressure();
        Console.WriteLine($"Pulse rate: {heartbeatWithoutPressure} bpm");
        isStarted = true;
        string idSmartwatch = SelectSmartWatch();

        SmartWatch_Data startData = new SmartWatch_Data()
        {
            SmartWatchId = Guid.Parse(idSmartwatch),
            ActivityGuid = Guid.NewGuid(),
            Latitude = lat,
            Longitude = lon,
            Heartbeat = heartbeatWithoutPressure,
            NumberOfPoolLaps = 0
        };
        startData.ApiPost(startData);
    }

    public void PostNextData(double lat2, double longitude2, double distance)
    {
        string idSmartwatch = SelectSmartWatch();
        Console.WriteLine($"\nLatitude: {lat2}");
        Console.WriteLine($"Longitude: {longitude2}");
        Console.WriteLine($"Distance: {distance} meters");
        var heartbeatUnderPressure = heartbeat.HeartbeatUnderPressure();
        Console.WriteLine($"Pulse rate: {heartbeatUnderPressure} bpm");
        totalDistance += distance;

        SmartWatch_Data newData = new SmartWatch_Data()
        {
            SmartWatchId = Guid.Parse(idSmartwatch),
            ActivityGuid = Guid.NewGuid(),
            Latitude = lat2,
            Longitude = longitude2,
            Heartbeat = heartbeatUnderPressure,
            NumberOfPoolLaps = (int)CalculatePoolLaps(totalDistance)
        };
        newData.ApiPost(newData);
    }

    // RECURSIVE FUNCTION TO GENERATE COORDINATES WITHIN 0 TO 50 METERS DISTANCE
    public void Training(double latitude, double longitude)
    {
        timerForData.Elapsed += new ElapsedEventHandler(SendData);
        timerForData.Interval = 10010;
        timerForData.Enabled = true;

        // FIRST COORDINATES
        if (latitude == 0 && longitude == 0)
        {
            (latitude, longitude) = GenerateFirstCoordinates();
        }

        // SECOND COORDINATES (DISTANCE BETWEEN 0 AND 50)
        (double latitude2, double longitude2) = GenerateNextCoordinates(latitude, longitude);

        // DISTANCE
        double distance = CalculateDistance(latitude, longitude, latitude2, longitude2);

        if (!isStarted)
        {
            PostFirstData(latitude, longitude);
        }

        Thread.Sleep(10000);
        PostNextData(latitude2, longitude2, distance);

        if (endTraining)
        {
            isStarted = false;
            return;
        }
        else
        {
            Training(latitude2, longitude2);
        }
        while (endTraining != true) ;
    }

    void SendData(object? sender, ElapsedEventArgs e)
    {
        endTraining = true;
    }

    public string SelectSmartWatch()
    {
        Dictionary<int, string> serialNumber = new Dictionary<int, string>();
        serialNumber.Add(1, "fd130b2b-0c1d-491d-88f9-b726a5080831");
        serialNumber.Add(2, "249c864b-61f9-40dd-a8e6-5c6b60c02045");
        serialNumber.Add(3, "298435af-2010-4a9a-aea7-003977e8dcda");

        var randomSmartWatch = rand.Next(1, serialNumber.Count() + 1);
        return serialNumber[randomSmartWatch];
    }
}
