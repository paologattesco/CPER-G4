using ITS.CPER.DataDequeue.Service;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ITS.CPER.DataDequeue;

[assembly: WebJobsStartup(typeof(Startup))]

namespace ITS.CPER.DataDequeue
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.Services.TryAddScoped<IDataAccess, DataAccess>();
        }
    }

}

