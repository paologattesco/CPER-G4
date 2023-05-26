using ITS.CPER.SimulatoreSmartWatch.Data;
using ITS.CPER.SimulatoreSmartWatch.Models;
using System.Diagnostics;

Random rand = new Random();
var serialNumbers = DictionaryOfSmartWatches();
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
    smartWatches[selectSmartWatch] = coordinates.Training(smartWatches[selectSmartWatch]);
    Console.WriteLine();
}



