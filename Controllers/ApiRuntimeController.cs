using System;
using System.DrawingCore;
using System.DrawingCore.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace MyntUI.Controllers
{
    [Route("api/runtime")]
    public class ApiCoreConfigRuntime : Controller
    {
        [HttpGet]
        public ActionResult Get()
        {
            return new JsonResult(Globals.RuntimeSettings);
        }
    }

    [Route("api/core/globals/avatar")]
    public class ApiCoreGlobalsAvatar : Controller
    {
        [HttpGet]
        public FileContentResult Get()
        {
            if ((string)Globals.RuntimeSettings["platform"]["os"] == "Windows")
            {
                using (var ms = new MemoryStream())
                {
                    Image userAvatarImage = GetUserAvatar.GetUserTile((string)Globals.RuntimeSettings["platform"]["userName"]);
                    userAvatarImage.Save(ms, ImageFormat.Jpeg);
                    ArraySegment<byte> buffer;
                    if (!ms.TryGetBuffer(out buffer)) throw new ArgumentException();
                    return File(buffer.Array, "image/png");
                }
            }
            byte[] filedata = System.IO.File.ReadAllBytes("/wwwroot/img/user_avatar_default.png");
            return File(filedata, "image/png");
        }

        public class GetUserAvatar
        {
            [DllImport("shell32.dll", EntryPoint = "#261", CharSet = CharSet.Unicode, PreserveSig = false)]
            public static extern void GetUserTilePath(
              string username,
              UInt32 whatever, // 0x80000000
              StringBuilder picpath, int maxLength);

            public static string GetUserTilePath(string username)
            {
                // username: use null for current user
                var sb = new StringBuilder(1000);
                GetUserTilePath(username, 0x80000000, sb, sb.Capacity);
                return sb.ToString();
            }

            public static Image GetUserTile(string username)
            {
                return Image.FromFile(GetUserTilePath(username));
            }
        }
    }
}
