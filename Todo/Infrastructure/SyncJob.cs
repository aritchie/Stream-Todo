using System;
using System.Threading;
using System.Threading.Tasks;
using Shiny.Jobs;


namespace Todo.Infrastructure
{
    public class SyncJob : IJob
    {
        readonly TodoSqliteConnection conn;


        public SyncJob(TodoSqliteConnection conn)
        {
            this.conn = conn;
        }


        public async Task<bool> Run(JobInfo jobInfo, CancellationToken cancelToken)
        {
            return false;
        }
    }
}
