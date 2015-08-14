using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Authentication.Facebook;
using Microsoft.AspNet.Authentication.Google;
using Microsoft.AspNet.Authentication.MicrosoftAccount;
using Microsoft.AspNet.Authentication.Twitter;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Diagnostics.Entity;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Routing;
using Microsoft.Data.Entity;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Microsoft.Framework.Logging.Console;
using Microsoft.Framework.Runtime;
using RealTimeDataSample.Hubs;
using RealTimeDataSample.Services;

namespace RealTimeDataSample
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
			// Setup configuration sources.
			var builder = new ConfigurationBuilder(appEnv.ApplicationBasePath)
				.AddJsonFile("config.json")
				.AddJsonFile($"config.{env.EnvironmentName}.json", optional: true);

			if (env.IsEnvironment("Development"))
            {
				// This reads the configuration keys from the secret store.
				// For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
				builder.AddUserSecrets();
            }
			builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
			services.AddSingleton(typeof(IPerformanceBroadcaster), typeof(PerformanceBroadcaster));
			services.AddSingleton(typeof(PerformanceGenerator));

            // Add Application settings to the services container.
            //services.Configure<AppSettings>(Configuration.GetSubKey("AppSettings"));

			services.AddSignalR();

            // Add MVC services to the services container.
            services.AddMvc();

            // Uncomment the following line to add Web API services which makes it easier to port Web API 2 controllers.
            // You will also need to add the Microsoft.AspNet.Mvc.WebApiCompatShim package to the 'dependencies' section of project.json.
            // services.AddWebApiConventions();
        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
			loggerFactory.MinimumLevel = LogLevel.Information;
			loggerFactory.AddConsole();

			// Configure the HTTP request pipeline.

			// Add the following to the request pipeline only in development environment.
			if (env.IsDevelopment())
			{
				app.UseBrowserLink();
				app.UseErrorPage();
				app.UseDatabaseErrorPage(DatabaseErrorPageOptions.ShowAll);
			}
			else
			{
				// Add Error handling middleware which catches all application specific errors and
				// sends the request to the following path or controller action.
				app.UseErrorHandler("/index.html");
			}

			// Add static files to the request pipeline.
			app.UseStaticFiles();

			// Add SignalR to the request pipeline.
			app.UseSignalR();

            // Add MVC to the request pipeline.
            app.UseMvc();

			app.ApplicationServices.GetRequiredService<PerformanceGenerator>();
        }
    }
}
