using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mynt.Core.Interfaces;
using Mynt.Core.Models;

namespace MyntUI.Controllers
{
    [Route("api/[controller]")]
    public class TradersController : Controller
    {
        [HttpGet]
        public async Task<List<Trader>> Get()
        {
            return await Globals.GlobalDataStore.GetTradersAsync();
        }
    }
}
