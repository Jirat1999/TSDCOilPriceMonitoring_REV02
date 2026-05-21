using System.Collections.Generic;
using System.Linq;
using TSDCOilPriceMonitoring_REV02.Models;

namespace TSDCOilPriceMonitoring_REV02.Class
{
    public class cFuelCalculatorService
    {
        public List<cmlFuelPriceDetail> C_PRClstCalculatePriceDiff(List<cmlFuelPriceDetail> poDetails)
        {
            if (poDetails == null || poDetails.Count == 0) return poDetails;

            var oSortedData = poDetails.OrderBy(x => x.tStationName)
                                      .ThenBy(x => x.tFuelName)
                                      .ThenBy(x => x.dEffectiveDate)
                                      .ToList();

            for (int i = 1; i < oSortedData.Count; i++)
            {
                if (oSortedData[i].tStationName == oSortedData[i - 1].tStationName &&
                    oSortedData[i].tFuelName == oSortedData[i - 1].tFuelName)
                {
                    oSortedData[i].cPriceDiff = oSortedData[i].cPricedPrice - oSortedData[i - 1].cPricedPrice;
                }
                else
                {
                    oSortedData[i].cPriceDiff = 0;
                }
            }

            return oSortedData;
        }
    }
}