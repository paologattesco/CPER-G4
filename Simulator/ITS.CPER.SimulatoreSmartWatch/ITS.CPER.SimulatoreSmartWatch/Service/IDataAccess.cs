namespace ITS.CPER.SimulatoreSmartWatch.Service;

public interface IDataAccess
{
    List<Guid> GetSmartWatchesId();

    Guid GetUserId(Guid smartwatchId);
}
