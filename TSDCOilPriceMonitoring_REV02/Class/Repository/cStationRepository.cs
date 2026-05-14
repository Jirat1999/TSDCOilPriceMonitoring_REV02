using System.Text;
using TSDCOilPriceMonitoring_REV02.Models;

namespace TSDCOilPriceMonitoring_REV02.Class.Repository
{
    public class cStationRepository
    {
        private readonly cLogService oLog = new cLogService();

        public List<cmlDropdownItem> C_PRCaoGetStations()
        {
            try
            {
                cDatabase oDB = new cDatabase();
                StringBuilder oSql = new StringBuilder();
                oSql.AppendLine("SELECT FNStationId AS nId, FTName AS tName");
                oSql.AppendLine($"FROM {cCS.tTbl_Stations} ORDER BY FTName");
                return oDB.C_PRCaQuerytoListObj<cmlDropdownItem>(oSql.ToString());
            }
            catch (Exception oEx)
            {
                oLog.C_PRCxWriteErrorLog(new cmlErrorLog
                {
                    tFTProcessName = "cStationRepository.C_PRCaoGetStations",
                    tFTErrorMessage = oEx.Message,
                    tFTStackTrace = oEx.StackTrace
                });
                return new List<cmlDropdownItem>();
            }
        }
    }
}