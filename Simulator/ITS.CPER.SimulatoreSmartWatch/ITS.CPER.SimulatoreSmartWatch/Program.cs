using ITS.CPER.SimulatoreSmartWatch.Data;

GeneratorOfCoordinates coordinates = new GeneratorOfCoordinates();

while (true)
{
    Random rand = new Random();
    var activity = rand.Next(0, 2);

    switch (activity)
    {
        case 0:
            Console.WriteLine("Workout in progress...\n");
            coordinates.Training(0.0, 0.0);
            Console.WriteLine();
            break;
        case 1:
            Console.WriteLine("Resting...");
            Heartbeat restingHeartbeat = new Heartbeat();
            Console.WriteLine($"Pulse rate: {restingHeartbeat.RestingHeartbeat()}\n");
            break;
    }
}

