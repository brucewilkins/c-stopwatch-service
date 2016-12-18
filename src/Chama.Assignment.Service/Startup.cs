using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Chama.Assignment.Service.Extensions;
using Chama.Assignment.Service.Configuration;
using Chama.Assignment.Service.Data;
using AutoMapper;
using Swashbuckle.Swagger.Model;
using Chama.Assignment.Service.SignalR;
using Newtonsoft.Json;

namespace Chama.Assignment.Service
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsEnvironment("Development"))
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<Options>(Configuration);
            services.AddTransient<TimerEntryService>();
            services.AddSingleton<StopwatchTimer>();

            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Basic", new BasicAuthScheme());
            });

            services.AddMvc();
            services.AddAutoMapper();

            services.AddSignalR(options =>
            {
                options.Hubs.EnableDetailedErrors = true;
            });
            
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new SignalRContractResolver();

            var serializer = JsonSerializer.Create(settings);
            services.Add(
                new ServiceDescriptor(typeof(JsonSerializer), 
                provider => serializer, 
                ServiceLifetime.Transient));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();
            app.UseApplicationInsightsExceptionTelemetry();

            app.UseSwagger();
            app.UseSwaggerUi();

            app.UseBasicAuthentication();

            app.UseMvc();

            app.UseStaticFiles();
            app.UseWebSockets();
            app.UseSignalR();
        }
    }
}
