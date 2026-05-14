using System.Text;
using TSDCOilPriceMonitoring_REV02.Models;

namespace TSDCOilPriceMonitoring_REV02.Class.QueryLog
{
    public class cEventLogQuery
    {
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