using System;
using System.Threading;
using System.Threading.Tasks;
using Shiny.Jobs;


namespace Todo.Data
{
    public class SyncJob : IJob
    {
        readonly IDataService data;
        readonly IApiClient apiClient;


        public SyncJob(IApiClient apiClient, IDataService data)
        {
            this.apiClient = apiClient;
            this.data = data;
        }


        public async Task<bool> Run(JobInfo jobInfo, CancellationToken cancelToken)
        {
            return false;
        }
    }
}
