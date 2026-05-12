using System.Text;
using TSDCOilPriceMonitoring_REV02.Models;

namespace TSDCOilPriceMonitoring_REV02.Class
{
    public class cFuelRepository
    {
        public List<cmlDropdownItem> C_PRCaoGetStations()
        {
            cDatabase oDB = new cDatabase();
            StringBuilder oSql = new StringBuilder();
            oSql.AppendLine("SELECT FNStationId AS nId, FTName AS tName");
            oSql.AppendLine($"FROM {cCS.tTbl_Stations} ORDER BY FTName");
            return oDB.C_PRCaQuerytoListObj<cmlDropdownItem>(oSql.ToString());
        }

        public List<cmlDropdownItem> C_PRCaoGetFuelTypes()
        {
            cDatabase oDB = new cDatabase();
            StringBuilder oSql = new StringBuilder();
            oSql.AppendLine("SELECT FNFuelTypeId AS nId, FTName AS tName");
            oSql.AppendLine($"FROM {cCS.tTbl_FuelTypes} ORDER BY FTName");
            return oDB.C_PRCaQuerytoListObj<cmlDropdownItem>(oSql.ToString());
        }

        public List<cmlFuelSummary> C_PRCaoGetFuelSummary(DateTime pdStart, DateTime pdEnd, int pnStationId, int pnFuelId)
        {
            cDatabase oDB = new cDatabase();
            StringBuilder oSql = new StringBuilder();
            oSql.AppendLine("SELECT S.FTName AS tStationName, F.FTName AS tFuelName,");
            oSql.AppendLine("AVG(P.FCPrice) AS cAvgPrice, MIN(P.FCPrice) AS cMinPrice, MAX(P.FCPrice) AS cMaxPrice, COUNT(P.FNPriceId) AS nTotalRecords");
            oSql.AppendLine($"FROM {cCS.tTbl_FuelPrices} P");
            oSql.AppendLine($"JOIN {cCS.tTbl_FuelTypes} F ON P.FNFuelTypeId = F.FNFuelTypeId");
            oSql.AppendLine($"JOIN {cCS.tTbl_Stations} S ON P.FNStationId = S.FNStationId");
            oSql.AppendLine("WHERE P.FDEffectiveDate BETWEEN @Start AND @End");

            if (pnStationId > 0) oSql.AppendLine("  AND P.FNStationId = @StationId");
            if (pnFuelId > 0) oSql.AppendLine("  AND P.FNFuelTypeId = @FuelId");
            oSql.AppendLine("GROUP BY S.FTName, F.FTName");

            var oParams = new { Start = pdStart, End = pdEnd, StationId = pnStationId, FuelId = pnFuelId };
            return oDB.C_PRCaQuerytoListObj<cmlFuelSummary>(oSql.ToString(), oParams);
        }

        public List<cmlFuelPriceDetail> C_PRCaoGetFuelPriceDetails(DateTime pdStart, DateTime pdEnd, int pnStationId, int pnFuelId)
        {
            cDatabase oDB = new cDatabase();
            StringBuilder oSql = new StringBuilder();
            oSql.AppendLine("SELECT P.FDEffectiveDate AS dEffectiveDate, S.FTName AS tStationName, F.FTName AS tFuelName, P.FCPrice AS cPricedPrice");
            oSql.AppendLine($"FROM {cCS.tTbl_FuelPrices} P");
            oSql.AppendLine($"JOIN {cCS.tTbl_Stations} S ON P.FNStationId = S.FNStationId");
            oSql.AppendLine($"JOIN {cCS.tTbl_FuelTypes} F ON P.FNFuelTypeId = F.FNFuelTypeId");
            oSql.AppendLine("WHERE P.FDEffectiveDate BETWEEN @Start AND @End");

            if (pnStationId > 0) oSql.AppendLine("  AND P.FNStationId = @StationId");
            if (pnFuelId > 0) oSql.AppendLine("  AND P.FNFuelTypeId = @FuelId");
            oSql.AppendLine("ORDER BY P.FDEffectiveDate DESC");

            var oParams = new { Start = pdStart, End = pdEnd, StationId = pnStationId, FuelId = pnFuelId };
            return oDB.C_PRCaQuerytoListObj<cmlFuelPriceDetail>(oSql.ToString(), oParams);
        }
    }
}
