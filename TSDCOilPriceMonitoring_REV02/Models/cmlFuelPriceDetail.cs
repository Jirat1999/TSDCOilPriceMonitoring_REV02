using System;
using System.Collections.Generic;
using System.Text;

namespace TSDCOilPriceMonitoring_REV02.Models
{
    public class cmlFuelPriceDetail
    {
        public DateTime dEffectiveDate { get; set; }
        public string tStationName { get; set; }
        public string tFuelName { get; set; }
        public decimal cPricedPrice { get; set; }
    }
}
