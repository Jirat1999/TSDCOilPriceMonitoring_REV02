using System.Text;
using TSDCOilPriceMonitoring_REV02.Models;

namespace TSDCOilPriceMonitoring_REV02.Class.Repository
{
    public class cFuelSummaryRepository
    {
        private readonly cLogService oLog = new cLogService();

        public List<cmlFuelSummary> C_PRCaoGetFuelSummary(DateTime pdStart, DateTime pdEnd, List<int> paStationIds, List<int> paFuelIds)
        {
            try
            {
                cDatabase oDB = new cDatabase();
                StringBuilder oSql = new StringBuilder();
                oSql.AppendLine("SELECT S.FTName AS tStationName, F.FTName AS tFuelName,");
                oSql.AppendLine("AVG(P.FCPrice) AS cAvgPrice, MIN(P.FCPrice) AS cMinPrice, MAX(P.FCPrice) AS cMaxPrice, COUNT(P.FNPriceId) AS nTotalRecords");
                oSql.AppendLine($"FROM {cCS.tTbl_FuelPrices} P");
                oSql.AppendLine($"JOIN {cCS.tTbl_FuelTypes} F ON P.FNFuelTypeId = F.FNFuelTypeId");
                oSql.AppendLine($"JOIN {cCS.tTbl_Stations} S ON P.FNStationId = S.FNStationId");
                oSql.AppendLine("WHERE P.FDEffectiveDate BETWEEN @Start AND @End");

                if (paStationIds != null && paStationIds.Count > 0)
                {
                    oSql.AppendLine($"  AND P.FNStationId IN ({string.Join(",", paStationIds)})");
                }

                if (paFuelIds != null && paFuelIds.Count > 0)
                {
                    oSql.AppendLine($"  AND P.FNFuelTypeId IN ({string.Join(",", paFuelIds)})");
                }

                oSql.AppendLine("GROUP BY S.FTName, F.FTName");

                var oParams = new { Start = pdStart, End = pdEnd };
                return oDB.C_PRCaQuerytoListObj<cmlFuelSummary>(oSql.ToString(), oParams);
            }
            catch (Exception oEx)
            {
                oLog.C_PRCxWriteErrorLog(new cmlErrorLog
                {
                    tFTProcessName = "cFuelSummaryRepository.C_PRCaoGetFuelSummary",
                    tFTErrorMessage = oEx.Message,
                    tFTStackTrace = oEx.StackTrace
                });
                return new List<cmlFuelSummary>();
            }
        }
    }
}