using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Mynt.Core.Interfaces;
using Mynt.Core.TradeManagers;
using Newtonsoft.Json.Linq;

namespace MyntUI.Controllers
{

  [Route("api/mynt/tradersTester")]
  public class MyntControllerTradersTester : Controller
  {
    [HttpGet]
    public IActionResult MyntTradersTester()
    {
      JObject testJson = JObject.Parse(System.IO.File.ReadAllText("wwwroot/views/mynt_traders.json"));
      return new JsonResult(testJson);
    }
  }

  [Route("api/mynt/traders")]
  public class MyntApiController : Controller
  {
    [HttpGet]
    [Route("dashboard")]
    public async Task<IActionResult> Dashboard()
    {
      var tradeOptions = Startup.Configuration.GetSection("TradeOptions").Get<TradeOptions>();

      ViewBag.quoteCurrency = tradeOptions.QuoteCurrency;
      // Get active trades
      var activeTrades = await Globals.GlobalDataStore.GetActiveTradesAsync();
      ViewBag.activeTrades = activeTrades;

      // Get current prices
      //ExchangeSharp.

      // Get Traders
      var traders = await Globals.GlobalDataStore.GetTradersAsync();
      ViewBag.traders = traders;

      // Check if Trader has active trade
      foreach (var trader in traders)
      {
        if (activeTrades.Count > 0)
        {
          var activeTrade = activeTrades.Where(t => t.TraderId == trader.Identifier).ToList();
          if (activeTrade.Count >= 1)
          {
            trader.ActiveTrade = activeTrade.First();

            //Temp for shortened
            var actT = trader.ActiveTrade;

            // Get Tickers
            trader.ActiveTrade.TickerLast = await Globals.GlobalExchangeApi.GetTicker(actT.Market);
            trader.ActiveTrade.OpenProfit = actT.OpenRate - trader.ActiveTrade.TickerLast.Last;
            trader.ActiveTrade.OpenProfitPercentage =
              ((100 * trader.ActiveTrade.TickerLast.Last) / actT.OpenRate) - 100;
          }
        }

        // Check Profit/Loss
        trader.ProfitLoss = ((100 * trader.CurrentBalance) / trader.StakeAmount) - 100;
      }

      ViewBag.closedTrades = await Globals.GlobalDataStore.GetClosedTradesAsync();

      return new JsonResult(ViewBag);
    }


    [HttpGet]
    [Route("statistics")]
    public async Task<IActionResult> Statistic()
    {
      var tradeOptions = Startup.Configuration.GetSection("TradeOptions").Get<TradeOptions>();

      // QuoteCurrency
      ViewBag.quoteCurrency = tradeOptions.QuoteCurrency;

      // MarketBlackList
      ViewBag.quoteCurrency = tradeOptions.MarketBlackList;

      // OnlyTradeList
      ViewBag.quoteCurrency = tradeOptions.OnlyTradeList;

      //  AlwaysTradeList
      ViewBag.quoteCurrency = tradeOptions.AlwaysTradeList;

      // Get closed trades
      var closedTrades = await Globals.GlobalDataStore.GetClosedTradesAsync();
      ViewBag.closedTrades = closedTrades;

      // Get winner/loser currencies
      var coins = new Dictionary<string, decimal?>();

      foreach (var cT in closedTrades)
        if (coins.ContainsKey(cT.Market))
          coins[cT.Market] = coins[cT.Market].Value + cT.CloseProfitPercentage;
        else
          coins.Add(cT.Market, cT.CloseProfitPercentage);
      
      ViewBag.profitCoins = coins.ToList().OrderByDescending(c => c.Value);
      


      return new JsonResult(ViewBag);
    }
  }

  //[Route("api/mynt/test")]
  //public class MyntTestController : Controller
  //{
  //}

  public class MyntController : Controller
    {
        // GET: /<controller>/        
        public async Task<IActionResult> Dashboard()
        {
            var tradeOptions = Startup.Configuration.GetSection("TradeOptions").Get<TradeOptions>();

            ViewBag.quoteCurrency = tradeOptions.QuoteCurrency;
            // Get active trades
            var activeTrades = await Globals.GlobalDataStore.GetActiveTradesAsync();
            ViewBag.activeTrades = activeTrades;

            // Get current prices
            //ExchangeSharp.

            // Get Traders
            var traders = await Globals.GlobalDataStore.GetTradersAsync();
            ViewBag.traders = traders;

            // Check if Trader has active trade
            foreach (var trader in traders)
            {
                if (activeTrades.Count > 0)
                {
                    var activeTrade = activeTrades.Where(t => t.TraderId == trader.Identifier).ToList();
                    if (activeTrade.Count >= 1)
                    {
                        trader.ActiveTrade = activeTrade.First();

                        //Temp for shortened
                        var actT = trader.ActiveTrade;

                        // Get Tickers
                        trader.ActiveTrade.TickerLast = await Globals.GlobalExchangeApi.GetTicker(actT.Market);
                        trader.ActiveTrade.OpenProfit = actT.OpenRate - trader.ActiveTrade.TickerLast.Last;
                        trader.ActiveTrade.OpenProfitPercentage = ((100 * trader.ActiveTrade.TickerLast.Last) / actT.OpenRate) - 100;
                    }
                }

                // Check Profit/Loss
                trader.ProfitLoss = ((100 * trader.CurrentBalance) / trader.StakeAmount) - 100;
            }

            ViewBag.closedTrades = await Globals.GlobalDataStore.GetClosedTradesAsync();

            return View();
        }


        // GET: /<controller>/
        public IActionResult Log()
        {
            // Get log from today
            string date = DateTime.Now.ToString("yyyyMMdd");
            ViewBag.log = MyntUI.Controllers.Log.ReadTail("Logs/Mynt-" + date + ".log", 100);

            return View();
        }
    }
}
