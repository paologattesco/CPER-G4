using ITS.CPER.SimulatoreSmartWatch.Models;
using System.Security.Cryptography;
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
    int TimeForTraining = 60000;

    private bool endTraining = false;
    private bool isStarted = false;
    private bool lastDate = false;
    Random rand = new Random();

    Heartbeat restingHeartbeat = new Heartbeat();
    Heartbeat trainingHeartbeat = new Heartbeat();

    private Guid NewActivity;
    private Guid SmartWatchId;

    SmartWatch_Data tmp = new SmartWatch_Data();

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

    public SmartWatch_Data PostFirstData(double lat, double lon, SmartWatch_Data smartwatch)
    {
        Console.WriteLine($"Latitude: {lat}");
        Console.WriteLine($"Longitude: {lon}");
        Console.WriteLine($"Pulse rate: {restingHeartbeat.RestingHeartbeat()} bpm");
        isStarted = true;
        SmartWatch_Data startData = new SmartWatch_Data()
        {
            SmartWatch_Id = smartwatch.SmartWatch_Id,
            Activity_Id = smartwatch.Activity_Id,
            Latitude = lat,
            Longitude = lon,
            Heartbeat = restingHeartbeat.RestingHeartbeat(),
            NumberOfPoolLaps = 0,
            Distance = 0,
            User_Id = smartwatch.User_Id
        };
        startData.ApiPost(startData);
        return startData;
    }

    public SmartWatch_Data PostNextData(double lat2, double longitude2, double distance, SmartWatch_Data smartwatch)
    {
        Console.WriteLine($"\nLatitude: {lat2}");
        Console.WriteLine($"Longitude: {longitude2}");
        Console.WriteLine($"Distance: {distance} meters");
        Console.WriteLine($"Pulse rate: {trainingHeartbeat.TrainingHeartbeat()} bpm");

        SmartWatch_Data newData = new SmartWatch_Data()
        {
            SmartWatch_Id = smartwatch.SmartWatch_Id,
            Activity_Id = smartwatch.Activity_Id,
            Latitude = lat2,
            Longitude = longitude2,
            Heartbeat = trainingHeartbeat.TrainingHeartbeat(),
            NumberOfPoolLaps = (int)CalculatePoolLaps(distance + smartwatch.Distance),
            Distance = distance + smartwatch.Distance,
            User_Id = smartwatch.User_Id
        };
        newData.ApiPost(newData);
        return newData;

    }

    // RECURSIVE FUNCTION TO GENERATE COORDINATES WITHIN 0 TO 50 METERS DISTANCE
    public SmartWatch_Data Training(SmartWatch_Data smartWatch)
    {
        timerForData.Start();
        // FIRST COORDINATES
        if (smartWatch.Latitude == 0 && smartWatch.Longitude == 0)
        {
            (smartWatch.Latitude, smartWatch.Longitude) = GenerateFirstCoordinates();

            // SECOND COORDINATES (DISTANCE BETWEEN 0 AND 50)
            (double latitude2, double longitude2) = GenerateNextCoordinates(smartWatch.Latitude, smartWatch.Longitude);

            // DISTANCE
            double distance = CalculateDistance(smartWatch.Latitude, smartWatch.Longitude, latitude2, longitude2);

            if (!isStarted)
            {
                smartWatch = PostFirstData(smartWatch.Latitude, smartWatch.Longitude, smartWatch);
            }
            Thread.Sleep(10000);
            smartWatch = PostNextData(latitude2, longitude2, distance, smartWatch);
            Training(smartWatch);
        }
        else
        {
            // SECOND COORDINATES (DISTANCE BETWEEN 0 AND 50)
            (double latitude2, double longitude2) = GenerateNextCoordinates(smartWatch.Latitude, smartWatch.Longitude);

            // DISTANCE
            double distance = CalculateDistance(smartWatch.Latitude, smartWatch.Longitude, latitude2, longitude2);

            Thread.Sleep(10000);
            smartWatch = PostNextData(latitude2, longitude2, distance, smartWatch);
            if(endTraining)
            {
                if (!lastDate)
                {
                    tmp = smartWatch;
                    lastDate = true;
                    endTraining = false;
                }
                return smartWatch;
            }
            else
            {
                Training(smartWatch);
            }
        }
        return tmp;
    }

    void SendData(object? sender, ElapsedEventArgs e)
    {
        endTraining = true;
        isStarted = false;
        lastDate = false;
        timerForData.Stop();
    }
}
