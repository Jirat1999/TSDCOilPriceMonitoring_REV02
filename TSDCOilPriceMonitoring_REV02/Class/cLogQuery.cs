using System.Text;
using TSDCOilPriceMonitoring_REV02.Models;

namespace TSDCOilPriceMonitoring_REV02.Class
{
    public class cLogQuery
    {
        public static string C_PRCtGetInsertErrorLog()
        {
            try
            {
                StringBuilder oSql = new StringBuilder();

                oSql.AppendLine($"INSERT INTO {cCS.tTbl_ErrorLogs} (");
                oSql.AppendLine("FTProcessName, ");
                oSql.AppendLine("FTErrorMessage, ");
                oSql.AppendLine("FTStackTrace");
                oSql.AppendLine(") VALUES (");
                oSql.AppendLine("@tFTProcessName,");
                oSql.AppendLine("@tFTErrorMessage,");
                oSql.AppendLine("@tFTStackTrace");
                oSql.AppendLine(")");

                return oSql.ToString();
            }
            catch (Exception ex)
            {
                new cLogService().C_PRCxWriteErrorLog(new cmlErrorLog
                {
                    tFTProcessName = "cLogQuery.C_PRCtGetInsertErrorLog",
                    tFTErrorMessage = ex.Message,
                    tFTStackTrace = ex.StackTrace
                });
                return string.Empty;
            }
        }

        public static string C_PRCtGetInsertEventLog()
        {
            try
            {
                StringBuilder oSql = new StringBuilder();

                oSql.AppendLine($"INSERT INTO {cCS.tTbl_EventLogs} (");
                oSql.AppendLine("FTEventName, ");
                oSql.AppendLine("FTDescription");
                oSql.AppendLine(") VALUES (");
                oSql.AppendLine("@tFTEventName, ");
                oSql.AppendLine("@tFTDescription");
                oSql.AppendLine(")");

                return oSql.ToString();
            }
            catch (Exception ex)
            {
                new cLogService().C_PRCxWriteErrorLog(new cmlErrorLog
                {
                    tFTProcessName = "cLogQuery.C_PRCtGetInsertEventLog",
                    tFTErrorMessage = ex.Message,
                    tFTStackTrace = ex.StackTrace
                });
                return string.Empty;
            }
        }
    }
}
