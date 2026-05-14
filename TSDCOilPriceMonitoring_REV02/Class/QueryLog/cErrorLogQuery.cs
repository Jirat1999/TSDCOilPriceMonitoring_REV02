using System.Text;
using TSDCOilPriceMonitoring_REV02.Models;

namespace TSDCOilPriceMonitoring_REV02.Class.QueryLog
{
    public class cErrorLogQuery
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
    }
}