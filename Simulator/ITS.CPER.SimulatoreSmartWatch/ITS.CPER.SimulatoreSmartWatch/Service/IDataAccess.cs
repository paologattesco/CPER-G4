using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITS.CPER.SimulatoreSmartWatch.Service;

public interface IDataAccess
{
    List<Guid> GetSmartWatchesId();
    
    Guid GetUserId(Guid smartwatchId);

    void InsertProductionBatch(Guid smartwatchId);
}
