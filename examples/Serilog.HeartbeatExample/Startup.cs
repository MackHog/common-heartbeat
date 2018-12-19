using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Collector.Common.Heartbeat;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Core;

namespace Serilog.HeartbeatExample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHeartbeatMonitor, HeartbeatMonitor>();
            services.AddScoped<ILogger>(p =>
            {
                var loggerConfig = new LoggerConfiguration()
                    .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm} [{Level}] [{CorrelationId}]: {Message}{NewLine}")
                    .WriteTo.Trace(outputTemplate: "{Timestamp:HH:mm} [{Level}] [{CorrelationId}]: {Message}{NewLine}")
                    .CreateLogger();
                var logger = loggerConfig.ForContext("CorrelationId", "123456789");
                var loggerFactory = p.GetRequiredService<ILoggerFactory>();
            });
            services.AddScoped<Microsoft.Extensions.Logging.ILogger>(p =>
            {
                var loggerFactory = p.GetRequiredService<ILoggerFactory>();
                var serilogger = p.GetRequiredService<ILogger>();
                return loggerFactory.AddSerilog(serilogger).CreateLogger<HeartbeatMonitor>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHeartbeat<IHeartbeatMonitor>(x => x.RunAsync(), options =>
            {
                options.ApiKey = "Secret"; // Default = string.Empty / None
                //options.ApiKeyHeaderKey = "SomeHeaderName"; // Default = "DiagnosticsAPIKey"
                //options.HeartbeatRoute = "/api/otherroute"; // Default = "/api/heartbeat"
            });

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
