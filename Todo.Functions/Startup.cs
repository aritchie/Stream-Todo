using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Todo.Functions.Data;

[assembly: FunctionsStartup(typeof(Todo.Functions.Startup))]


namespace Todo.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();
            builder.Services.AddSingleton<IConfiguration>(config);
            builder.Services.AddDbContext<TodoDbContext>(opts =>
                opts.UseSqlServer(config.GetConnectionString("Database"))
            );
        }
    }
}
