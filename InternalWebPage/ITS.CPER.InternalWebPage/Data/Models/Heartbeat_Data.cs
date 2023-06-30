using NodaTime;

namespace ITS.CPER.InternalWebPage.Data.Models
{
    public class Heartbeat_Data
    {
        public Instant Time { get; set; }
        public int Heartbeat { get; set; }
    }
}
