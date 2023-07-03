namespace ITS.CPER.SimulatoreSmartWatch.Data;

public class Heartbeat
{
    private readonly Random rnd;
    const int MIN_HEARBEAT = 20;
    const int RESTING_HEARTBEAT = 60;
    const int MAX_HEARTBEAT = 210;

    public Heartbeat() 
    {
        rnd = new Random();
    }

    public int TrainingHeartbeat()
    {
        return rnd.Next(MIN_HEARBEAT, MAX_HEARTBEAT + 1);
    }

    public int RestingHeartbeat()
    {
        int range = 30; // Range of randomization for resting heart rate
        return RESTING_HEARTBEAT - range / 5 + rnd.Next(range + 1);
    }
}
