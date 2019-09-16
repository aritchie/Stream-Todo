using System;
using Microsoft.Extensions.DependencyInjection;
using Shiny;
using Shiny.Logging;
using Todo.Data;
using Todo.Infrastructure;
using Acr.UserDialogs.Forms;


namespace Todo
{
    public class Startup : ShinyStartup
    {
        public override void ConfigureServices(IServiceCollection services)
        {
#if DEBUG
            Log.UseConsole();
            Log.UseDebug();
#endif

            services.UseNotifications(true);
            services.UseGeofencing<GeofenceDelegate>();
            services.UseGps();
            services.AddSingleton<ITodoService, TodoService>();
            services.UseAppCenterLogging(Constants.AppCenterSecret);

            services.RegisterModule<DataModule>();
            services.AddSingleton<GlobalExceptionHandler>();
            services.AddSingleton<IUserDialogs, UserDialogs>();
        }
    }
}
