using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shiny.Jobs;


namespace Todo.Data
{
    class SyncJob : IJob
    {
        readonly TodoSqliteConnection data;
        readonly ITodoService todoService;
        readonly IApiClient apiClient;


        public SyncJob(IApiClient apiClient,
                       ITodoService todoService,
                       TodoSqliteConnection data)
        {
            this.apiClient = apiClient;
            this.todoService = todoService;
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
                await this.todoService.Save(todo);

            return todos.Any();
        }
    }
}
