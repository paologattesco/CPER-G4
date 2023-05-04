using ITS.CPER.SimulatoreSmartWatch.Data;


GeneratorOfCoordinates coordinates = new GeneratorOfCoordinates();

while (true)
{
    Random rnd = new Random();
    var activity = rnd.Next(0, 2);

    switch (activity)
    {
        case 0:
            Console.WriteLine("Workout in progress...\n");
            coordinates.GenerateCoordinates(0.0, 0.0);
            Console.WriteLine();
            break;
        case 1:
            Console.WriteLine("Resting...");
            Heartbeat heartbeat = new Heartbeat();
            Console.WriteLine($"Pulse rate: {heartbeat.HeartbeatWithoutPressure()}");
            Console.WriteLine();
            break;
    }
}

