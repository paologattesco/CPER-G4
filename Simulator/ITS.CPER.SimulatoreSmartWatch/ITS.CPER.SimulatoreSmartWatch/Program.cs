using ITS.CPER.SimulatoreSmartWatch.Data;
using ITS.CPER.SimulatoreSmartWatch.Models;

GeneratorOfCoordinates coordinates = new GeneratorOfCoordinates();

while (true)
{
    Random rand = new Random();
    var activity = rand.Next(0, 2);
    var NewActivity = Guid.NewGuid();

    switch (activity)
    {
        case 0:
            Console.WriteLine("Workout in progress...\n");
            coordinates.Training(0.0, 0.0, NewActivity);
            Console.WriteLine();
            break;
        case 1:
            Console.WriteLine("Resting...");
            Heartbeat restingHeartbeat = new Heartbeat();
            var heart = restingHeartbeat.RestingHeartbeat();
            Console.WriteLine($"Pulse rate: {heart}\n");
            GeneratorOfCoordinates SmartWatchDetail = new GeneratorOfCoordinates();
            var SmartWatchId = SmartWatchDetail.SelectSmartWatch();
            SmartWatch_Data newData = new SmartWatch_Data()
            {
                SmartWatch_Id = SmartWatchId,
                Activity_Id = NewActivity,
                Latitude = 0,
                Longitude = 0,
                Heartbeat = heart,
                NumberOfPoolLaps = 0
            };
            newData.ApiPost(newData);
            break;
    }

}

