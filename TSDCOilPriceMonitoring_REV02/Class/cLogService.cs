using System.Text.Json;
using TSDCOilPriceMonitoring_REV02.Models;

namespace TSDCOilPriceMonitoring_REV02.Class
{
    public class cLogService
    {
        public void C_PRCxWriteErrorLog(cmlErrorLog poErrorLog)
        {
            C_PRCxWriteErrorToJsonFile(poErrorLog);

            try
            {
                cDatabase oDB = new cDatabase();
                oDB.C_PRCbExecuteNoQuery(cLogQuery.C_PRCtGetInsertErrorLog(), poErrorLog);
            }
            catch (Exception)
            {

            }
        }

        public void C_PRCxWriteEventLog(cmlEventLog poEventLog)
        {
            try
            {
                cDatabase oDB = new cDatabase();
                oDB.C_PRCbExecuteNoQuery(cLogQuery.C_PRCtGetInsertEventLog(), poEventLog);
            }
            catch (Exception) { }
        }


        private void C_PRCxWriteErrorToJsonFile(cmlErrorLog poErrorLog)
        {
            try
            {
                string tLogFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                if (!Directory.Exists(tLogFolder))
                {
                    Directory.CreateDirectory(tLogFolder);
                }

                string tFileName = $"ErrorLog_{DateTime.Now:yyyyMMdd}.json";
                string tFilePath = Path.Combine(tLogFolder, tFileName);

                var oLogData = new
                {
                    dLogDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    tProcessName = poErrorLog.tFTProcessName,
                    tErrorMessage = poErrorLog.tFTErrorMessage,
                    tStackTrace = poErrorLog.tFTStackTrace
                };

                string tJsonData = JsonSerializer.Serialize(oLogData) + Environment.NewLine;
                File.AppendAllText(tFilePath, tJsonData);
            }
            catch (Exception)
            {

            }
        }
    }
}
