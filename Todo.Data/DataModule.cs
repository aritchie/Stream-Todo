using System;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Shiny;
using Shiny.Jobs;


namespace Todo.Data
{
    public class DataModule : Module
    {
        public override void Register(IServiceCollection services)
        {
            // TODO: startup job to pull initial data/run initial job right away?
            services.AddSingleton(_ => RestService.For<IApiClient>(Constants.BaseApiUri));
            services.AddSingleton<TodoSqliteConnection>();

            services.RegisterJob(new JobInfo
            {
                Identifier = nameof(SyncJob),
                Type = typeof(SyncJob),
                BatteryNotLow = true,
                RequiredInternetAccess = InternetAccess.Any
            });
        }
    }
}
