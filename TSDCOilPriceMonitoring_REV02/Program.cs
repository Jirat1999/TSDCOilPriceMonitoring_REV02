using Microsoft.Extensions.Configuration;
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
                cCS.tCS_ConStr = oDB.C_PRCxDatabase(oConnConfig);

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

                MessageBox.Show($"A critical error occurred while starting the application.:\n{ex.Message}", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            MessageBox.Show($"A system error has occurred.:\n{e.Exception.Message}\n\n(Details have been saved to the Logs folder.)", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            MessageBox.Show($"A critical system error has occurred. The application must close.:\n{ex.Message}\n\n(Details have been saved to the Logs folder.)", "Fatal System Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }
    }

}