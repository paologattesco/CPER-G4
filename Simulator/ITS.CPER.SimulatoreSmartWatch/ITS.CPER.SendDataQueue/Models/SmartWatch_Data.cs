using System;

namespace ITS.CPER.SendDataQueue.Models;

public class SmartWatch_Data
{
    public Guid SmartWatchId { get; set; }
    public Guid ActivityGuid { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int Heartbeat { get; set; }
    public int NumberOfPoolLaps { get; set; }

}
