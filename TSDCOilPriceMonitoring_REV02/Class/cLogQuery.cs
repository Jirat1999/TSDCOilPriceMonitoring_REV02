using System.Text;
using TSDCOilPriceMonitoring_REV02.Models;

namespace TSDCOilPriceMonitoring_REV02.Class
{
    public class cLogQuery
    {
        public static bool C_PRCoInsertErrorLog(cmlErrorLog poErrorLog)
        {
            try
            {
                cDatabase oDB = new cDatabase();
                StringBuilder oSql = new StringBuilder();

                oSql.AppendLine($"INSERT INTO {cCS.tTbl_ErrorLogs} (");
                oSql.AppendLine("FTProcessName, FTErrorMessage, FTStackTrace");
                oSql.AppendLine(") VALUES (");
                oSql.AppendLine("@tFTProcessName, @tFTErrorMessage, @tFTStackTrace");
                oSql.AppendLine(")");

                return oDB.C_PRCnExecuteNoQuery(oSql.ToString(), poErrorLog);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool C_PRCbInsertEventLog(cmlEventLog poEventLog)
        {
            try
            {
                cDatabase oDB = new cDatabase();
                StringBuilder oSql = new StringBuilder();

                oSql.AppendLine($"INSERT INTO {cCS.tTbl_EventLogs} (");
                oSql.AppendLine("FTEventName, FTDescription");
                oSql.AppendLine(") VALUES (");
                oSql.AppendLine("@tFTEventName, @tFTDescription");
                oSql.AppendLine(")");

                return oDB.C_PRCnExecuteNoQuery(oSql.ToString(), poEventLog);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}