using NodaTime;

namespace ITS.CPER.InternalWebPage.Data.Models
{
    public class HeartBeat
    {
        public Instant Time { get; set; }
        public int Heartbeat { get; set; }
    }
}
