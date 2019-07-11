using System;
using Microsoft.Extensions.DependencyInjection;
using Shiny;
using Shiny.Notifications;
using Shiny.Locations;
using Shiny.Logging;
using Todo.Data;
using Todo.Infrastructure;
using Acr.UserDialogs.Forms;


namespace Todo
{
    public class ShinyStartup : Startup
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IUserDialogs, UserDialogs>();

            services.UseNotifications(true);
            services.UseGeofencing<GeofenceDelegate>();
            services.UseGps();

            services.RegisterStartupTask<GlobalExceptionHandler>();
            services.RegisterModule<DataModule>();

            services.UseAppCenterLogging(Constants.AppCenterSecret);
#if DEBUG
            Log.UseConsole();
            Log.UseDebug();
#endif
        }
    }
}
