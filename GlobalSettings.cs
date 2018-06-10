using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
using Newtonsoft.Json.Linq;

namespace MyntUI
{
  public static class Globals
  {
    public static IApplicationBuilder GlobalApplicationBuilder;
    public static IServiceScope GlobalServiceScope { get; set; }
    public static IConfiguration GlobalConfiguration { get; set; }
    public static IDataStore GlobalDataStore { get; set; }
    public static TradeOptions GlobalTradeOptions { get; set; }
    public static MyntHostedServiceOptions GlobalMyntHostedServiceOptions { get; set; }
    public static ExchangeOptions GlobalExchangeOptions { get; set; }
    public static IExchangeApi GlobalExchangeApi { get; set; }
    public static ILoggerFactory GlobalLoggerFactory { get; set; }
    public static CancellationToken GlobalTimerCancellationToken = new CancellationToken();
    public static IHubContext<HubMyntTraders> GlobalHubMyntTraders;
    public static JObject RuntimeSettings = new JObject();

  }

  public class GlobalSettings
  {
    public async static void Init()
    {
      Globals.RuntimeSettings["platform"] = new JObject();
      Globals.RuntimeSettings["platform"]["os"] = GetOs();
      Globals.RuntimeSettings["platform"]["computerName"] = Environment.MachineName;
      Globals.RuntimeSettings["platform"]["userName"] = Environment.UserName;
      Globals.RuntimeSettings["platform"]["webInitialized"] = false;
      Globals.RuntimeSettings["platform"]["settingsInitialized"] = false;
      Globals.RuntimeSettings["signalrClients"] = new JObject();

      var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true);
      Globals.GlobalConfiguration = builder.Build();
      Globals.GlobalTradeOptions = Globals.GlobalConfiguration.GetSection("TradeOptions").Get<TradeOptions>();
      Globals.GlobalExchangeOptions = Globals.GlobalConfiguration.Get<ExchangeOptions>();
      Globals.GlobalMyntHostedServiceOptions = Globals.GlobalConfiguration.GetSection("Hosting").Get<MyntHostedServiceOptions>();

      LiteDBOptions databaseOptions = new LiteDBOptions();
      Globals.GlobalDataStore = new LiteDBDataStore(databaseOptions);

      var exchangeOptions = Globals.GlobalConfiguration.Get<ExchangeOptions>();
      exchangeOptions.Exchange = Exchange.Binance;
      
      Globals.GlobalHubMyntTraders = Globals.GlobalServiceScope.ServiceProvider.GetService<IHubContext<HubMyntTraders>>();

      Globals.GlobalExchangeApi = new BaseExchange(exchangeOptions);

      ILogger paperTradeLogger = Globals.GlobalLoggerFactory.CreateLogger<PaperTradeManager>();

      PaperTradeManager paperTradeManager = new PaperTradeManager(new BaseExchange(exchangeOptions), new FreqClassic(), new SignalrNotificationManager(), paperTradeLogger, Globals.GlobalTradeOptions, Globals.GlobalDataStore);

      var runTimer = new MyntHostedService(paperTradeManager, Globals.GlobalMyntHostedServiceOptions);

      await runTimer.StartAsync(Globals.GlobalTimerCancellationToken);
    }

    public static string GetOs()
    {
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
      {
        return "Windows";
      }

      if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
      {
        return "Linux";
      }

      if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
      {
        return "OSX";
      }

      return "Unknown";
    }

  }
}
