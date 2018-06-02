#if !NETCOREAPP2_1
using System.ServiceProcess;
using Microsoft.AspNetCore.Hosting;

namespace MyntUI.Host.Hosting
{
    public static class WebHostExtensions
    {
        public static void RunAsMyntWindowsService(this IWebHost host)
        {
            var webHostService = new MyntWebHostService(host);
            ServiceBase.Run(webHostService);
        }
    }
}
#endif
