using Microsoft.AspNetCore.Mvc;

namespace MyntUI.Controllers
{
  [Route("index.html")]
  [Route("", Order = -1)]
  public class IndexController : Controller
  {
    [HttpGet]
    public IActionResult File()
    {
      return File("~/index.html", "text/html");
    }
  }

  [Route("favicon.ico")]
  public class FaviconController : Controller
  {
    [HttpGet]
    public IActionResult File()
    {
      return File("~/img/favicon.ico", "image/x-icon");
    }
  }
}
