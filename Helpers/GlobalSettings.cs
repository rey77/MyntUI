using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace MyntUI.Helpers
{
    public class GlobalSettings
    {
      public static void Init()
      {
        Global.RuntimeSettings["platform"] = new JObject();
        Global.RuntimeSettings["platform"]["os"] = GetOs();
        Global.RuntimeSettings["platform"]["computerName"] = Environment.MachineName;
        Global.RuntimeSettings["platform"]["userName"] = Environment.UserName;
        Global.RuntimeSettings["platform"]["webInitialized"] = false;
        Global.RuntimeSettings["platform"]["settingsInitialized"] = false;
        Global.RuntimeSettings["signalrClients"] = new JObject();
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
