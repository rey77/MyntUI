using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyntUI.Hubs;
using Serilog;
using System.Net;
using Microsoft.AspNetCore;

namespace MyntUI
{
  public class Startup
  {
    public static IServiceScope ServiceScope { get; private set; }
    public static IConfiguration Configuration { get; set; }

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddConnections();

      services.AddSignalR();

      services.AddCors(o =>
      {
        o.AddPolicy("Everything", p =>
        {
          p.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin()
            .AllowCredentials();
        });
      });

      // Configure serilog from appsettings.json
      var serilogger = new LoggerConfiguration()
          .ReadFrom.Configuration(Configuration)
          .CreateLogger();

      services.AddLogging(b => { b.AddSerilog(serilogger); });

      services.AddMvc();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment hostingEnvironment, ILoggerFactory loggerFactory)
    {
      ServiceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
      Globals.GlobalServiceScope = ServiceScope;
      Globals.GlobalLoggerFactory = loggerFactory;
      Globals.GlobalApplicationBuilder = app;

      app.UseStaticFiles();

      if (hostingEnvironment.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseWebSockets();

      app.UseSignalR(routes =>
      {
        routes.MapHub<HubMainIndex>("/signalr/HubMainIndex");
        routes.MapHub<HubMyntTraders>("/signalr/HubMyntTraders");
      });

      app.UseMvc(routes =>
      {
        routes.MapRoute(
          name: "default",
          template: "{controller=Home}/{action=Index}/{id?}");

        routes.MapSpaFallbackRoute(
          name: "spa-fallback",
          defaults: new {controller = "Home", action = "Index"});
      });

      // DI is ready - Init 
      GlobalSettings.Init();
    }

    public static void RunWebHost()
    {

      IWebHostBuilder webHostBuilder = WebHost.CreateDefaultBuilder()
      .UseKestrel(options =>
      {
        options.Listen(IPAddress.Any, 5000);
      })
      .UseStartup<Startup>()
      .ConfigureAppConfiguration(i =>
          i.AddJsonFile("appsettings.overrides.json", true));

      IWebHost webHost = webHostBuilder.Build();
      webHost.Run();

    }
  }
}
