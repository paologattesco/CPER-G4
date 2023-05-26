using ITS.CPER.SimulatoreSmartWatch.Data;
using ITS.CPER.SimulatoreSmartWatch.Models;
using ITS.CPER.SimulatoreSmartWatch.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


List<SmartWatch_Data> smartWatches = ListOfSmartWatch();
Random rand = new Random();
GeneratorOfCoordinates coordinates = new GeneratorOfCoordinates();

while (true)
{
    var selectSmartWatch = rand.Next(0, smartWatches.Count());
    Console.WriteLine("Workout in progress...\n");
    smartWatches[selectSmartWatch] = coordinates.Training(smartWatches[selectSmartWatch]);
    Console.WriteLine();
}



Dictionary<int, Guid> DictionaryOfSmartWatches()
{
    var serviceProvider = GetConfiguration();
    var _dataAccess = serviceProvider.GetRequiredService<IDataAccess>();

    Dictionary<int, Guid> serialNumber = new Dictionary<int, Guid>();
    var smartwatches = _dataAccess.GetSmartWatchesId();
    for (int i = 0; i < smartwatches.Count; i++)
    {
        serialNumber.Add(i, smartwatches[i]);
    }
    return serialNumber;
}

List<SmartWatch_Data> ListOfSmartWatch()
{
    var serialNumbers = DictionaryOfSmartWatches();
    List<SmartWatch_Data> smartWatches = new List<SmartWatch_Data>();

    for (int i = 0; i < serialNumbers.Count(); i++)
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
    return smartWatches;
}

ServiceProvider GetConfiguration()
{
    IConfiguration configuration;
    configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddUserSecrets<Program>(optional: true, reloadOnChange: false)
                    .Build();

    var serviceProvider = new ServiceCollection()
                    .AddSingleton<IDataAccess, DataAccess>()
                    .AddSingleton<IConfiguration>(configuration)
                    .BuildServiceProvider();

    return serviceProvider;
}