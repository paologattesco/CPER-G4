using ITS.CPER.WebPage.Data.Models;

namespace ITS.CPER.WebPage.Data.Services;

public interface IDataAccess
{
    Task<IEnumerable<SmartWatch_Data>> GetSmartWatchDataAsync();
}
