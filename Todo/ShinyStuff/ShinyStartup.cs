using System;
using Microsoft.Extensions.DependencyInjection;
using Shiny;
using Shiny.Logging;
using Shiny.Prism;
using Todo.Data;
using Todo.Infrastructure;
using Acr.UserDialogs.Forms;
using Prism.DryIoc;


namespace Todo
{
    public class ShinyStartup : PrismStartup
    {
        public ShinyStartup() : base(PrismContainerExtension.Current) { }


        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IUserDialogs, UserDialogs>();

            services.UseNotifications(true);
            services.UseGeofencing<GeofenceDelegate>();
            //services.UseGps<>

            services.AddSingleton<GlobalExceptionHandler>();
            services.RegisterModule<DataModule>();

            services.UseAppCenterLogging(Constants.AppCenterSecret);
#if DEBUG
            Log.UseConsole();
            Log.UseDebug();
#endif
        }
    }
}
