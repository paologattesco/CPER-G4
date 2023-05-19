using ITS.CPER.SimulatoreSmartWatch.Data;
using ITS.CPER.SimulatoreSmartWatch.Models;
using System.Diagnostics;

Random rand = new Random();
var serialNumbers = SmartWatchesDict();
GeneratorOfCoordinates coordinates = new GeneratorOfCoordinates();
List<SmartWatch_Data> smartWatches = new List<SmartWatch_Data>();
for(int i = 0; i < serialNumbers.Count(); i++)
{
   SmartWatch_Data newData = new SmartWatch_Data()
        {
            SmartWatch_Id = serialNumbers[i],
            Activity_Id = Guid.NewGuid(),
            Latitude = 0,
            Longitude = 0,
            Heartbeat = 0,
            NumberOfPoolLaps = 0,
            Distance = 0
        };
    smartWatches.Add(newData);
}

while (true)
{
    var selectSmartWatch = rand.Next(0, smartWatches.Count());
    Console.WriteLine("Workout in progress...\n");
    var LastSmartWatchDetails = coordinates.Training(smartWatches[selectSmartWatch]);
    if(LastSmartWatchDetails.SmartWatch_Id == smartWatches[selectSmartWatch].SmartWatch_Id)
    {
        smartWatches[selectSmartWatch] = LastSmartWatchDetails;
    }
    Console.WriteLine();
}



Dictionary<int,Guid> SmartWatchesDict()
{

    Dictionary<int, Guid> serialNumber = new Dictionary<int, Guid>();
    serialNumber.Add(0, Guid.Parse("fd130b2b-0c1d-491d-88f9-b726a5080831"));
    serialNumber.Add(1, Guid.Parse("249c864b-61f9-40dd-a8e6-5c6b60c02045"));
    serialNumber.Add(2, Guid.Parse("298435af-2010-4a9a-aea7-003977e8dcda"));

    return serialNumber;
}


//var resting = "b6a2c180-659c-49a1-9be1-f0487fec2d24";

//while (true)
//{
//    Random rand = new Random();
//    var activity = rand.Next(0, 2);
//    var NewActivity = Guid.NewGuid();

//    switch (activity)
//    {
//        case 0:
//            Console.WriteLine("Workout in progress...\n");
//            coordinates.Training(0.0, 0.0, NewActivity);
//            Console.WriteLine();
//            break;
//        case 1:
//            Console.WriteLine("Resting...");
//            Heartbeat restingHeartbeat = new Heartbeat();
//            var heart = restingHeartbeat.RestingHeartbeat();
//            Console.WriteLine($"Pulse rate: {heart}\n");
//            GeneratorOfCoordinates SmartWatchDetail = new GeneratorOfCoordinates();
//            var SmartWatchId = SmartWatchDetail.SelectSmartWatch();
//            SmartWatch_Data newData = new SmartWatch_Data()
//            {
//                SmartWatch_Id = SmartWatchId,
//                Activity_Id = Guid.Parse(resting),
//                Latitude = 0,
//                Longitude = 0,
//                Heartbeat = heart,
//                NumberOfPoolLaps = 0
//            };
//            newData.ApiPost(newData);
//            Thread.Sleep(10000);
//            break;
//    }

//}

