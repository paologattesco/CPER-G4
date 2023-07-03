using ITS.CPER.SimulatoreSmartWatch.Data;
using ITS.CPER.SimulatoreSmartWatch.Models;
using ITS.CPER.SimulatoreSmartWatch.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ITS.CPER.SimulatoreSmartWatch;

public class Program
{
    private static IDataAccess _dataAccess;
    private static GeneratorOfCoordinates _coordinates;
    private static Random _rand;

    public static void Main(string[] args)
    {
        _rand = new Random();
        _coordinates = new GeneratorOfCoordinates();

        while (true)
        {
            List<SmartWatch> smartwatches = GetSmartWatches();
            var selectSmartWatch = _rand.Next(0, smartwatches.Count);
            Console.WriteLine("Workout in progress...\n");
            smartwatches[selectSmartWatch] = _coordinates.Training(smartwatches[selectSmartWatch]);
            Console.WriteLine();
        }
    }

    private static IDataAccess InitializeDataAccess()
    {
        var serviceProvider = GetConfiguration();
        return serviceProvider.GetRequiredService<IDataAccess>();
    }

    private static Dictionary<int, Guid> DictionaryOfSmartWatches()
    {
        _dataAccess = InitializeDataAccess();

        Dictionary<int, Guid> serialNumber = new Dictionary<int, Guid>();
        var smartwatches = _dataAccess.GetSmartWatchesId();

        for (int i = 0; i < smartwatches.Count; i++)
        {
            serialNumber.Add(i, smartwatches[i]);
        }

        return serialNumber;
    }

    private static List<SmartWatch> GetSmartWatches()
    {
        _dataAccess = InitializeDataAccess();

        var serialNumbers = DictionaryOfSmartWatches();
        List<SmartWatch> smartwatches = new List<SmartWatch>();

        foreach (var serialNumber in serialNumbers)
        {
            SmartWatch newData = new SmartWatch()
            {
                SmartWatch_Id = serialNumber.Value,
                Activity_Id = Guid.NewGuid(),
                Latitude = 0,
                Longitude = 0,
                Heartbeat = 0,
                NumberOfPoolLaps = 0,
                Distance = 0,
                User_Id = _dataAccess.GetUserId(serialNumber.Value)
            };
            smartwatches.Add(newData);
        }

        return smartwatches;
    }

    private static ServiceProvider GetConfiguration()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddUserSecrets<Program>(optional: true, reloadOnChange: false)
            .Build();

        var serviceProvider = new ServiceCollection()
            .AddSingleton<IDataAccess, DataAccess>()
            .AddSingleton<IConfiguration>(configuration)
            .BuildServiceProvider();

        return serviceProvider;
    }
}