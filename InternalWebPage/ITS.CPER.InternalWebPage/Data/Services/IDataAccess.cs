using InfluxDB.Client;
using ITS.CPER.InternalWebPage.Data.Models;

namespace ITS.CPER.InternalWebPage.Data.Services;

public interface IDataAccess
{
    Task<List<SmartWatch>> GetSmartWatchesDataAsync();
    Task<List<HeartBeat>> HeartbeatQuery(SmartWatch data);
    Task<Dictionary<Guid, Guid>> GetProductionBatch();
}
