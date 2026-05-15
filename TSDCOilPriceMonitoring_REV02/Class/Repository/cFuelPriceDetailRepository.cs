using System.Text;
using TSDCOilPriceMonitoring_REV02.Models;

namespace TSDCOilPriceMonitoring_REV02.Class.Repository
{
    public class cFuelPriceDetailRepository
    {
        private readonly cLogService oLog = new cLogService();

        public List<cmlFuelPriceDetail> C_PRCaoGetFuelPriceDetails(DateTime pdStart, DateTime pdEnd, List<int> paStationIds, List<int> paFuelIds)
        {
            try
            {
                cDatabase oDB = new cDatabase();
                StringBuilder oSql = new StringBuilder();
                oSql.AppendLine("SELECT P.FDEffectiveDate AS dEffectiveDate, S.FTName AS tStationName, F.FTName AS tFuelName, P.FCPrice AS cPricedPrice");
                oSql.AppendLine($"FROM {cCS.tTbl_FuelPrices} P");
                oSql.AppendLine($"JOIN {cCS.tTbl_Stations} S ON P.FNStationId = S.FNStationId");
                oSql.AppendLine($"JOIN {cCS.tTbl_FuelTypes} F ON P.FNFuelTypeId = F.FNFuelTypeId");
                oSql.AppendLine("WHERE P.FDEffectiveDate BETWEEN @Start AND @End");

                if (paStationIds != null && paStationIds.Count > 0)
                {
                    oSql.AppendLine($"  AND P.FNStationId IN ({string.Join(",", paStationIds)})");
                }

                if (paFuelIds != null && paFuelIds.Count > 0)
                {
                    oSql.AppendLine($"  AND P.FNFuelTypeId IN ({string.Join(",", paFuelIds)})");
                }

                oSql.AppendLine("ORDER BY P.FDEffectiveDate DESC");

                var oParams = new { Start = pdStart, End = pdEnd };
                return oDB.C_PRCaQuerytoListObj<cmlFuelPriceDetail>(oSql.ToString(), oParams);
            }
            catch (Exception oEx)
            {
                oLog.C_PRCxWriteErrorLog(new cmlErrorLog
                {
                    tFTProcessName = "cFuelPriceDetailRepository.C_PRCaoGetFuelPriceDetails",
                    tFTErrorMessage = oEx.Message,
                    tFTStackTrace = oEx.StackTrace
                });
                return new List<cmlFuelPriceDetail>();
            }
        }
    }
}