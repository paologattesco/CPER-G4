namespace ITS.CPER.InternalWebPage.Data.Models;

public class Activity
{
    public Guid Activity_Id { get; set; }
    public double Initial_Latitude { get; set; }
    public double Initial_Longitude { get; set; }
    public double Final_Latitude { get; set; }
    public double Final_Longitude { get; set; }
    public int Heartbeat { get; set; }
    public bool ShowActivityDetails { get; set; }
}
