using NodaTime;

namespace ITS.CPER.WebPage.Data.Models
{
    public class Heartbeat_Data
    {
        public Instant Time { get; set; }
        public int Heartbeat { get; set; }
    }
}
