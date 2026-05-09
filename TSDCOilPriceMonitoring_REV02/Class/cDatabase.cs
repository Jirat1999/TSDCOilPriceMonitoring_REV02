using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using TSDCOilPriceMonitoring_REV02.Models;
using Dapper;

namespace TSDCOilPriceMonitoring_REV02.Class
{
    public class cDatabase
    {
        public string C_CONtDatabase(cmlConnectionConfig poConfig)
        {
            try
            {
                SqlConnectionStringBuilder oBuilder = new SqlConnectionStringBuilder();
                oBuilder.DataSource = poConfig.tServerDB;
                oBuilder.InitialCatalog = poConfig.tNameDB;
                oBuilder.IntegratedSecurity = poConfig.bIntegratedSecurity;
                oBuilder.Encrypt = poConfig.bEncrypt;
                oBuilder.TrustServerCertificate = poConfig.bTrustServerCertificate;

                if (!poConfig.bIntegratedSecurity)
                {
                    oBuilder.UserID = poConfig.tUser;
                    oBuilder.Password = poConfig.tPassword;
                }
                return oBuilder.ConnectionString;
            }
            catch (Exception ex)
            {
                new cLogService().C_PRCxWriteErrorLog(new cmlErrorLog
                {
                    tFTProcessName = "cDatabase.C_CONtDatabase",
                    tFTErrorMessage = ex.Message,
                    tFTStackTrace = ex.StackTrace
                });
                return string.Empty;
            }
        }

        public List<T> C_GETaQuerytoListObj<T>(string ptSqlCmd, object poParam = null)
        {
            try
            {
                using (SqlConnection oConn = new SqlConnection(cCS.tCS_ConStr))
                {
                    return oConn.Query<T>(ptSqlCmd, poParam).ToList();
                }
            }
            catch (Exception ex)
            {
                new cLogService().C_PRCxWriteErrorLog(new cmlErrorLog
                {
                    tFTProcessName = "cDatabase.C_GETaQuerytoListObj",
                    tFTErrorMessage = ex.Message,
                    tFTStackTrace = ex.StackTrace
                });

                if (cCS.tMode == "DEV") throw ex;
                return new List<T>();
            }
        }

        public bool C_PRCbExecuteNoQuery(string ptSqlCmd, object poParam = null)
        {
            try
            {
                using (SqlConnection oConn = new SqlConnection(cCS.tCS_ConStr))
                {
                    int nRowEffect = oConn.Execute(ptSqlCmd, poParam);
                    return nRowEffect > 0;
                }
            }
            catch (Exception ex)
            {
                new cLogService().C_PRCxWriteErrorLog(new cmlErrorLog
                {
                    tFTProcessName = "cDatabase.C_PRCbExecuteNoQuery",
                    tFTErrorMessage = ex.Message,
                    tFTStackTrace = ex.StackTrace
                });

                if (cCS.tMode == "DEV") throw ex;
                return false;
            }
        }
    }
}
