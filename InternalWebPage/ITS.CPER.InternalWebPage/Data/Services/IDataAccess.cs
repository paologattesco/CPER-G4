using InfluxDB.Client;
using ITS.CPER.InternalWebPage.Data.Models;

namespace ITS.CPER.InternalWebPage.Data.Services;

public interface IDataAccess
{
    Task<List<SmartWatch_Data>> GetSmartWatchesDataAsync();
    Task<List<Heartbeat_Data>> HeartbeatQuery(SmartWatch_Data data);
    Task<Dictionary<Guid, Guid>> GetProductionBatch();
}
