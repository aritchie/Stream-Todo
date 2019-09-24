using System;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Shiny;


namespace Todo.Data
{
    public class DataModule : ShinyModule
    {
        public override void Register(IServiceCollection services)
        {
            services.AddSingleton(_ => RestService.For<IApiClient>(Constants.BaseApiUri));
            services.AddSingleton<TodoSqliteConnection>();
            services.AddSingleton<IGeocoder, GeocoderImpl>();
            services.AddSingleton<IDataService, SqliteDataService>();

            //services.RegisterJob(new JobInfo
            //{
            //    Identifier = nameof(SyncJob),
            //    Type = typeof(SyncJob),
            //    BatteryNotLow = true,
            //    RequiredInternetAccess = InternetAccess.Any
            //});
        }
    }
}
