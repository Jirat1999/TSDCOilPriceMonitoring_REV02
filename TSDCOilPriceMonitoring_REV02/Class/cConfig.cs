using System;
using System.Collections.Generic;
using System.Text;

namespace TSDCOilPriceMonitoring_REV02.Class
{
    public class cConfig
    {
        public static bool W_PRCbLoadConfig()
        {
            try
            {
                cCS.tTbl_FuelPrices = "TCNM_PRICE_FuelPrices";
                cCS.tTbl_Stations = "TCNM_MASTER_Stations";
                cCS.tTbl_FuelTypes = "TCNM_MASTER_FuelTypes";
                cCS.tTbl_ErrorLogs = "TCNM_ERROR_ErrorLogs";
                cCS.tTbl_EventLogs = "TCNM_EVENT_EventLogs";
                return true;
            }
            catch { return false; }
        }
    }
}
