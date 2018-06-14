using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Mynt.Core.Interfaces;
using Mynt.Core.TradeManagers;
using MyntUI.Models;
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
            // Create Statistic model
            var stat = new Statistics()
            {

            };


            // Get winner/loser currencies
            var coins = new Dictionary<string, decimal?>();
            foreach (var cT in await Globals.GlobalDataStore.GetClosedTradesAsync())
            {
                // Get profit per currency
                if (coins.ContainsKey(cT.Market))
                    coins[cT.Market] = coins[cT.Market].Value + cT.CloseProfitPercentage;
                else
                    coins.Add(cT.Market, cT.CloseProfitPercentage);

                // Profit-loss
                if (cT.CloseProfit != null) stat.ProfitLoss = stat.ProfitLoss + cT.CloseProfit.Value;
                if (cT.CloseProfitPercentage != null) stat.ProfitLossPercentage = stat.ProfitLossPercentage + cT.CloseProfitPercentage.Value;
            }

            // Coin performance
            stat.CoinPerformance = coins.ToList().OrderByDescending(c => c.Value);

            // Create some viewbags
            ViewBag.tradeOptions = Startup.Configuration.GetSection("TradeOptions").Get<TradeOptions>();
            ViewBag.stat = stat;

            return new JsonResult(ViewBag);
        }
    }
}
