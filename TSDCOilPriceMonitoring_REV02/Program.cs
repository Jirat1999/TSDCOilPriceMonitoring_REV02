using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Windows.Forms;
using TSDCOilPriceMonitoring_REV02.Class;
using TSDCOilPriceMonitoring_REV02.Models;

namespace TSDCOilPriceMonitoring_REV02
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.ThreadException += new ThreadExceptionEventHandler(W_PRCxGlobalThreadExceptionHandler);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(W_PRCxGlobalUnhandledExceptionHandler);

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                cConfig.W_PRCbLoadConfig();

                ConfigurationBuilder oBuilder = new ConfigurationBuilder();
                oBuilder.SetBasePath(Directory.GetCurrentDirectory());
                oBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                IConfiguration oConfig = oBuilder.Build();

                cmlAppConfig oAppConfig = new cmlAppConfig();
                oConfig.GetSection("Settings").Bind(oAppConfig);
                cCS.tMode = oAppConfig.tMode;

                cmlConnectionConfig oConnConfig = new cmlConnectionConfig();
                oConfig.GetSection("ConnectionConfig").Bind(oConnConfig);

                cDatabase oDB = new cDatabase();
                cCS.tCS_ConStr = oDB.C_CONtDatabase(oConnConfig);

                Application.Run(new wFormMain());
            }
            catch (Exception ex)
            {
                new cLogService().C_PRCxWriteErrorLog(new cmlErrorLog
                {
                    tFTProcessName = "Program.Main (Startup Error)",
                    tFTErrorMessage = ex.Message,
                    tFTStackTrace = ex.StackTrace
                });

                MessageBox.Show($"พบข้อผิดพลาดระดับร้ายแรงขณะเริ่มโปรแกรม:\n{ex.Message}", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void W_PRCxGlobalThreadExceptionHandler(object sender, ThreadExceptionEventArgs e)
        {
            new cLogService().C_PRCxWriteErrorLog(new cmlErrorLog
            {
                tFTProcessName = "Global System Error (UI Thread)",
                tFTErrorMessage = e.Exception.Message,
                tFTStackTrace = e.Exception.StackTrace
            });

            MessageBox.Show($"พบข้อผิดพลาดของระบบ:\n{e.Exception.Message}\n\n(รายละเอียดถูกบันทึกไว้ในโฟลเดอร์ Logs แล้ว)", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void W_PRCxGlobalUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            new cLogService().C_PRCxWriteErrorLog(new cmlErrorLog
            {
                tFTProcessName = "Global System Error (Background Thread)",
                tFTErrorMessage = ex.Message,
                tFTStackTrace = ex.StackTrace
            });

            MessageBox.Show($"พบข้อผิดพลาดร้ายแรงของระบบ โปรแกรมจำเป็นต้องปิดตัวลง:\n{ex.Message}\n\n(รายละเอียดถูกบันทึกไว้ในโฟลเดอร์ Logs แล้ว)", "Fatal System Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }
    }
    
}