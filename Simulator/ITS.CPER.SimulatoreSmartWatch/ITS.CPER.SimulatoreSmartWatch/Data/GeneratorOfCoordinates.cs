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
    int TimeForTraining = 30000;
    private bool endTraining = false;
    private bool isStarted = false;
    private double totalDistance = 0.0;
    Random rand = new Random();
    Heartbeat restingHeartbeat = new Heartbeat();
    Heartbeat trainingHeartbeat = new Heartbeat();
    private Guid NewActivity;
    private Guid SmartWatchId;
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
        endTraining = false;
        timerForData = new System.Timers.Timer(TimeForTraining);
        timerForData.Elapsed += SendData;
        timerForData.Start();
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

    public void PostFirstData(double lat, double lon, Guid NewActivity)
    {
        Console.WriteLine($"Latitude: {lat}");
        Console.WriteLine($"Longitude: {lon}");
        Console.WriteLine($"Pulse rate: {restingHeartbeat.RestingHeartbeat()} bpm");
        isStarted = true;
        SmartWatchId = SelectSmartWatch();
        SmartWatch_Data startData = new SmartWatch_Data()
        {
            SmartWatch_Id = SmartWatchId,
            Activity_Id = NewActivity,
            Latitude = lat,
            Longitude = lon,
            Heartbeat = restingHeartbeat.RestingHeartbeat(),
            NumberOfPoolLaps = 0,
            Distance = 0
        };
        startData.ApiPost(startData);
    }

    public void PostNextData(double lat2, double longitude2, double distance, Guid NewActivity)
    {
        Console.WriteLine($"\nLatitude: {lat2}");
        Console.WriteLine($"Longitude: {longitude2}");
        Console.WriteLine($"Distance: {distance} meters");
        Console.WriteLine($"Pulse rate: {trainingHeartbeat.TrainingHeartbeat()} bpm");
        totalDistance += distance;

        SmartWatch_Data newData = new SmartWatch_Data()
        {
            SmartWatch_Id = SmartWatchId,
            Activity_Id = NewActivity,
            Latitude = lat2,
            Longitude = longitude2,
            Heartbeat = trainingHeartbeat.TrainingHeartbeat(),
            NumberOfPoolLaps = (int)CalculatePoolLaps(totalDistance),
            Distance = totalDistance
        };
        newData.ApiPost(newData);

    }

    // RECURSIVE FUNCTION TO GENERATE COORDINATES WITHIN 0 TO 50 METERS DISTANCE
    public void Training(double latitude, double longitude, Guid NewActivity)
    {

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
            PostFirstData(latitude, longitude, NewActivity);
        }

        Thread.Sleep(10000);
        PostNextData(latitude2, longitude2, distance, NewActivity);
        if(endTraining) { return; }
        Training(latitude2, longitude2, NewActivity);        
    }

    void SendData(object? sender, ElapsedEventArgs e)
    {
        endTraining = true;
        isStarted = false;
        timerForData.Stop();
    }

    public Guid SelectSmartWatch()
    {
        Dictionary<int, Guid> serialNumber = new Dictionary<int, Guid>();
        serialNumber.Add(1, Guid.Parse("fd130b2b-0c1d-491d-88f9-b726a5080831"));
        serialNumber.Add(2, Guid.Parse("249c864b-61f9-40dd-a8e6-5c6b60c02045"));
        serialNumber.Add(3, Guid.Parse("298435af-2010-4a9a-aea7-003977e8dcda"));

        var randomSmartWatch = rand.Next(1, serialNumber.Count() + 1);
        return serialNumber[randomSmartWatch];
    }

}
