using System;
using System.Collections.Generic;
using System.Linq;
using Mynt.Core.Models;
using Mynt.Core.TradeManagers;

namespace MyntUI.Models
{
    public class Statistics
    {
        // Profit - loss
        public decimal? ProfitLoss { get; set; } = 0;
        public decimal? ProfitLossPercentage { get; set; } = 0;

        //public decimal? ProfitLossToday { get; set; }
        //public decimal? ProfitLossTodayPercentage { get; set; }

        //public decimal? ProfitLossWeek { get; set; }
        //public decimal? ProfitLossWeekPercentage { get; set; }

        //public decimal? ProfitLossMonth { get; set; }
        //public decimal? ProfitLossMonthPercentage { get; set; }

        public IOrderedEnumerable<KeyValuePair<string, decimal?>> CoinPerformance { get; set; }
    }
}
