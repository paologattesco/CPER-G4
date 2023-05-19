using System;

namespace ITS.CPER.DataDequeue.Models;

public class SmartWatch_Data
{
    public Guid SmartWatch_Id { get; set; }
    public Guid Activity_Id { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int Heartbeat { get; set; }
    public int NumberOfPoolLaps { get; set; }
    public double Distance { get; set; }
}
