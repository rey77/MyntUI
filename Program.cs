using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using MyntUI.Helpers;
using Newtonsoft.Json.Linq;

namespace MyntUI
{

  //Define Global Variables 
  public static class Global
  {
    public static JObject RuntimeSettings = new JObject();
  }

  public class Program
    {
        public static void Main(string[] args)
        {
          GlobalSettings.Init();
          var host = BuildWebHost(args);

#if NETCOREAPP2_1
            host.Run();
#else
            if (Debugger.IsAttached || args.Contains("--console"))
            {
                host.Run();
            }
            else
            {
                host.RunAsMyntWindowsService();
            }
#endif
        }
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(i => 
                    i.AddJsonFile("appsettings.overrides.json", true))
                .UseStartup<Startup>()
                .UseUrls("http://*:5000")
                .Build();
    }
}
