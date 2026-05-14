using System.Text;
using TSDCOilPriceMonitoring_REV02.Models;

namespace TSDCOilPriceMonitoring_REV02.Class.Repository
{
    public class cFuelTypeRepository
    {
        private readonly cLogService oLog = new cLogService();

        public List<cmlDropdownItem> C_PRCaoGetFuelTypes()
        {
            try
            {
                cDatabase oDB = new cDatabase();
                StringBuilder oSql = new StringBuilder();
                oSql.AppendLine("SELECT FNFuelTypeId AS nId, FTName AS tName");
                oSql.AppendLine($"FROM {cCS.tTbl_FuelTypes} ORDER BY FTName");
                return oDB.C_PRCaQuerytoListObj<cmlDropdownItem>(oSql.ToString());
            }
            catch (Exception oEx)
            {
                oLog.C_PRCxWriteErrorLog(new cmlErrorLog
                {
                    tFTProcessName = "cFuelTypeRepository.C_PRCaoGetFuelTypes",
                    tFTErrorMessage = oEx.Message,
                    tFTStackTrace = oEx.StackTrace
                });
                return new List<cmlDropdownItem>();
            }
        }
    }
}