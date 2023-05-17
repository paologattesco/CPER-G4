using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITS.CPER.SimulatoreSmartWatch.Data;

public class Heartbeat
{
    private readonly Random rnd;
    const int RESTING_HEARTBEAT = 60;
    const int MAX_HEARTBEAT = 200;

    public Heartbeat() 
    {
        rnd = new Random();
    }

    public int TrainingHeartbeat()
    {
        int maxRange = MAX_HEARTBEAT - RESTING_HEARTBEAT;
        return RESTING_HEARTBEAT + rnd.Next(maxRange + 1);
    }

    public int RestingHeartbeat()
    {
        int range = 30; // Range of randomization for resting heart rate
        return RESTING_HEARTBEAT - range / 5 + rnd.Next(range + 1);
    }
}
