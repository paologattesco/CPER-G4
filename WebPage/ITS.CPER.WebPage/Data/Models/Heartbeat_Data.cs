using NodaTime;

namespace ITS.CPER.WebPage.Data.Models
{
    public class Heartbeat_Data
    {
        public Instant Time { get; set; }
        public double Heartbeat { get; set; }
    }
}
