using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mynt.Core.Enums;
using Mynt.Core.Exchanges;
using Mynt.Core.Interfaces;
using Mynt.Core.Notifications;
using Mynt.Core.Strategies;
using Mynt.Core.TradeManagers;
using Mynt.Data.LiteDB;
using MyntUI.Hosting;
using MyntUI.Hubs;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace MyntUI
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public static IConfiguration Configuration { get; set; }

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

      // Set up exchange - TODO
      var exchangeOptions = Configuration.Get<ExchangeOptions>();
      exchangeOptions.Exchange = Exchange.Binance;
      services.AddSingleton<IExchangeApi>(i => new BaseExchange(exchangeOptions));

      var tradeOptions = Configuration.GetSection("TradeOptions").Get<TradeOptions>();

      var type = Type.GetType($"Mynt.Core.Strategies.{tradeOptions.DefaultStrategy}, Mynt.Core", true, true);

      // Major TODO, coming soon
      services.AddSingleton(s => Activator.CreateInstance(type) as ITradingStrategy ?? new TheScalper())
          .AddSingleton<INotificationManager, TelegramNotificationManager>()
          .AddSingleton(i => Configuration.GetSection("Telegram").Get<TelegramNotificationOptions>()) // TODO

          //.AddSingleton<IDataStore, SqlServerDataStore>()
          //.AddSingleton(i => Configuration.GetSection("SqlServerOptions").Get<SqlServerOptions>()) // TODO

          .AddSingleton<IDataStore, LiteDBDataStore>()
          .AddSingleton(i => Configuration.GetSection("LiteDBOptions").Get<LiteDBOptions>()) // TODO
          .AddSingleton<ITradeManager, PaperTradeManager>()
          .AddSingleton(i => tradeOptions)

          .AddSingleton<ILogger>(i => i.GetRequiredService<ILoggerFactory>().CreateLogger<MyntHostedService>())

          .AddSingleton<IHostedService, MyntHostedService>()
          .Configure<MyntHostedServiceOptions>(Configuration.GetSection("Hosting"))

          ;
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
    {
      app.UseStaticFiles();

      if (env.IsDevelopment())
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
    }
  }
}
