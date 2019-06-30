using System;
using Microsoft.Extensions.DependencyInjection;
using Shiny;
using Shiny.Notifications;
using Shiny.Locations;
using Shiny.Logging;
using Shiny.Prism;
using Prism.DryIoc;
using Todo.Data;
using Todo.Infrastructure;


namespace Todo
{
    public class ShinyStartup : PrismStartup
    {
        //public ShinyStartup() : base(new DryIocContainerExtension()) { }


        public override void ConfigureServices(IServiceCollection services)
        {
            services.UseNotifications(true);
            services.UseGeofencing<GeofenceDelegate>();
            services.RegisterStartupTask<GlobalExceptionHandler>();
            services.RegisterModule<DataModule>();

            Log.UseConsole();
            Log.UseDebug();
        }
    }
}
