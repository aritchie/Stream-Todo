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
#if DEBUG
            Log.UseConsole();
            Log.UseDebug();
#endif

            services.UseNotifications(true);
            services.UseGeofencing<GeofenceDelegate>();
            services.AddSingleton<ITodoService, TodoService>();
            services.UseAppCenterLogging(Constants.AppCenterSecret);

            services.RegisterModule<DataModule>();
            services.AddSingleton<GlobalExceptionHandler>();
            services.AddSingleton<IUserDialogs, UserDialogs>();
        }
    }
}
