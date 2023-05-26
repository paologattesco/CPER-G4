namespace ITS.CPER.WebPage.Data.Models;

public class SmartWatch_Data
{
    public Guid SmartWatch_Id { get; set; }
    public Guid Activity_Id { get; set; }
    public double Initial_Latitude { get; set; }
    public double Initial_Longitude { get; set; }
    public double Distance { get; set; }
    public int Heartbeat { get; set; }
    public int NumberOfPoolLaps { get; set; }
    public double Final_Latitude { get; set; }
    public double Final_Longitude { get; set; }
}
