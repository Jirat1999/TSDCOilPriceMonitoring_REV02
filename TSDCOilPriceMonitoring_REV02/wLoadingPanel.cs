using TSDCOilPriceMonitoring_REV02.Class;
using TSDCOilPriceMonitoring_REV02.Models;

namespace TSDCOilPriceMonitoring_REV02
{
    public partial class wLoadingPanel : UserControl
    {
        private Label olaLoadingText;
        private System.Windows.Forms.Timer oLoadingTimer;
        private int nDotCount = 0;

        private cLogService oLog;

        public wLoadingPanel()
        {
            try
            {
                oLog = new cLogService();
                W_PRCxSetupUI();
            }
            catch (Exception ex)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog
                {
                    tFTProcessName = "wLoadingPanel.Constructor",
                    tFTErrorMessage = ex.Message,
                    tFTStackTrace = ex.StackTrace
                });
            }
        }

        private void W_PRCxSetupUI()
        {
            try
            {
                this.Dock = DockStyle.Fill;
                this.BackColor = Color.FromArgb(244, 247, 252);
                this.Visible = false;

                olaLoadingText = new Label
                {
                    Text = "Loading Data",
                    Font = new Font("Segoe UI", 18, FontStyle.Bold),
                    ForeColor = Color.FromArgb(0, 120, 212),
                    AutoSize = true
                };

                this.Controls.Add(olaLoadingText);

                this.Resize += (s, e) =>
                {
                    try
                    {
                        olaLoadingText.Left = (this.Width - olaLoadingText.Width) / 2;
                        olaLoadingText.Top = (this.Height - olaLoadingText.Height) / 2;
                    }
                    catch (Exception oEx)
                    {
                        oLog?.C_PRCxWriteErrorLog(new cmlErrorLog
                        {
                            tFTProcessName = "wLoadingPanel.Resize",
                            tFTErrorMessage = oEx.Message,
                            tFTStackTrace = oEx.StackTrace
                        });
                    }
                };

                oLoadingTimer = new System.Windows.Forms.Timer { Interval = 400 };
                oLoadingTimer.Tick += (s, e) =>
                {
                    try
                    {
                        nDotCount = (nDotCount + 1) % 4;
                        olaLoadingText.Text = "Loading Data" + new string('.', nDotCount);
                    }
                    catch (Exception oEx)
                    {
                        oLog?.C_PRCxWriteErrorLog(new cmlErrorLog
                        {
                            tFTProcessName = "wLoadingPanel.Tick",
                            tFTErrorMessage = oEx.Message,
                            tFTStackTrace = oEx.StackTrace
                        });
                    }
                };
            }
            catch (Exception oEx)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog
                {
                    tFTProcessName = "wLoadingPanel.W_PRCxSetupUI",
                    tFTErrorMessage = oEx.Message,
                    tFTStackTrace = oEx.StackTrace
                });
            }
        }

        public void W_PRCxStart()
        {
            try
            {
                this.Visible = true;
                this.BringToFront();
                oLoadingTimer.Start();
            }
            catch (Exception oEx)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog
                {
                    tFTProcessName = "wLoadingPanel.W_PRCxStart",
                    tFTErrorMessage = oEx.Message,
                    tFTStackTrace = oEx.StackTrace
                });
            }
        }

        public void W_PRCxStop()
        {
            try
            {
                oLoadingTimer.Stop();
                this.Visible = false;
            }
            catch (Exception oEx)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog
                {
                    tFTProcessName = "wLoadingPanel.W_PRCxStop",
                    tFTErrorMessage = oEx.Message,
                    tFTStackTrace = oEx.StackTrace
                });
            }
        }
    }
}