using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shiny.Jobs;


namespace Todo.Data
{
    public class SyncJob : IJob
    {
        readonly TodoSqliteConnection data;
        readonly IApiClient apiClient;


        public SyncJob(IApiClient apiClient, TodoSqliteConnection data)
        {
            this.apiClient = apiClient;
            this.data = data;
        }


        public async Task<bool> Run(JobInfo jobInfo, CancellationToken cancelToken)
        {
            var pushed = await this.Push(jobInfo, cancelToken);
            var pulled = await this.Pull(jobInfo, cancelToken);
            return pushed || pulled;
        }


        async Task<bool> Push(JobInfo jobInfo, CancellationToken cancelToken)
        {
            var todos = await this.data
                .Todos
                .Where(x => x.DateUpdatedUtc > jobInfo.LastRunUtc)
                .ToListAsync();

            var en = todos.GetEnumerator();
            while (!cancelToken.IsCancellationRequested && en.MoveNext())
                await this.apiClient.Save(en.Current);

            return todos.Any();
        }


        async Task<bool> Pull(JobInfo jobInfo, CancellationToken cancelToken)
        {
            var todos = await this.apiClient.Get(true, jobInfo.LastRunUtc);
            foreach (var todo in todos)
                await this.data.InsertOrReplaceAsync(todo);

            return todos.Any();
        }
    }
}
