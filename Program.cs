using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace MyntUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
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
