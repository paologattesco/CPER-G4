namespace ITS.CPER.SimulatoreSmartWatch.Data;

public class Heartbeat
{
    const int MIN_HEARTBEAT = 40;
    const int MAX_HEARTBEAT = 200;

    public int HeartbeatUnderPressure()
    {
        int min_heartbeat = 100;
        int max_heartbeat = 200;
        return NewRange(min_heartbeat, max_heartbeat);
    }

    public int HeartbeatWithoutPressure()
    {
        int min_heartbeat = 40;
        int max_heartbeat = 100;
        return NewRange(min_heartbeat, max_heartbeat);
    }

    int NewRange(int min, int max)
    {
        Random rnd = new Random();
        return rnd.Next(min, max);
    }
}
