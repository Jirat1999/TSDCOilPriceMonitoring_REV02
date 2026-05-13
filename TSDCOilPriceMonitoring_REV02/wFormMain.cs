using TSDCOilPriceMonitoring_REV02.Class;
using TSDCOilPriceMonitoring_REV02.Models;

namespace TSDCOilPriceMonitoring_REV02
{
    public partial class wFormMain : Form
    {
        private Panel opnSidebar, opnContent;
        private Button ocnDashBoard, ocnGridView;
        private Panel opnActiveIndicator; 
        private Form oActiveForm = null;
        private cLogService oLog = new cLogService();
        private System.Windows.Forms.Timer oFadeTimer;

        private Color oSidebarBg = Color.FromArgb(15, 32, 67);
        private Color oActiveBtn = Color.FromArgb(0, 120, 212);
        private Color oHoverBtn = Color.FromArgb(25, 50, 100);
        private Color oContentBg = Color.FromArgb(244, 247, 252);

        public wFormMain()
        {
            try
            {
                this.Opacity = 0;

                W_PRCxSetupUI();
                W_PRCxHighlightButton(ocnDashBoard);
                W_PRCxOpenChildForm(new wFormDashBoard());

                oFadeTimer = new System.Windows.Forms.Timer { Interval = 15 };
                oFadeTimer.Tick += (s, e) =>
                {
                    if (this.Opacity < 1) this.Opacity += 0.05;
                    else oFadeTimer.Stop();
                };
                oFadeTimer.Start();
            }
            catch (Exception oEx)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog 
                { 
                    tFTProcessName = "wFormMain.Constructor", 
                    tFTErrorMessage = oEx.Message, 
                    tFTStackTrace = oEx.StackTrace 
                });
            }
        }

        private void W_PRCxOpenChildForm(Form poChildForm)
        {
            try
            {
                if (oActiveForm != null) 
                { 
                    oActiveForm.Close(); 
                    oActiveForm.Dispose(); 
                }
                oActiveForm = poChildForm;
                poChildForm.TopLevel = false; 
                poChildForm.FormBorderStyle = FormBorderStyle.None; 
                poChildForm.Dock = DockStyle.Fill;
                opnContent.Controls.Add(poChildForm); 
                opnContent.Tag = poChildForm; 
                poChildForm.BringToFront(); 
                poChildForm.Show();
            }
            catch (Exception oEx)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog 
                { 
                    tFTProcessName = "wFormMain.W_PRCxOpenChildForm", 
                    tFTErrorMessage = oEx.Message, 
                    tFTStackTrace = oEx.StackTrace 
                });
            }
        }

        private void W_PRCxHighlightButton(Button poActiveBtn)
        {
            try
            {
                ocnDashBoard.BackColor = oSidebarBg;
                ocnGridView.BackColor = oSidebarBg;

                if (poActiveBtn != null)
                {
                    poActiveBtn.BackColor = oHoverBtn;
                    opnActiveIndicator.Height = poActiveBtn.Height;
                    opnActiveIndicator.Top = poActiveBtn.Top;
                    opnActiveIndicator.BringToFront();
                }
            }
            catch (Exception oEx)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog 
                { 
                    tFTProcessName = "wFormMain.W_PRCxHighlightButton", 
                    tFTErrorMessage = oEx.Message, 
                    tFTStackTrace = oEx.StackTrace 
                });
            }
        }

        private void W_PRCxSetupUI()
        {
            try
            {
                this.Text = "TSDC Oil Price Monitoring";
                this.Size = new Size(1200, 750);
                this.StartPosition = FormStartPosition.CenterScreen;

                opnSidebar = new Panel 
                { 
                    Dock = DockStyle.Left, 
                    Width = 230, 
                    BackColor = oSidebarBg 
                };
                Label olaAppTitle = new Label 
                { 
                    Text = "TSDC System", 
                    Font = new Font("Segoe UI", 18, FontStyle.Bold), 
                    ForeColor = Color.FromArgb(144, 202, 249), 
                    TextAlign = ContentAlignment.MiddleCenter, 
                    Dock = DockStyle.Top, 
                    Height = 100 
                };

                opnActiveIndicator = new Panel 
                {
                    Width = 5, 
                    BackColor = oActiveBtn, 
                    Left = 0 
                };
                opnSidebar.Controls.Add(opnActiveIndicator);

                ocnDashBoard = W_PRCopnCreateMenuButton("🏠  Dashboard", 100);
                ocnDashBoard.Click += (s, e) => { W_PRCxHighlightButton((Button)s); W_PRCxOpenChildForm(new wFormDashBoard()); };

                ocnGridView = W_PRCopnCreateMenuButton("📊  Grid View", 160);
                ocnGridView.Click += (s, e) => { W_PRCxHighlightButton((Button)s); W_PRCxOpenChildForm(new wFormGridView()); };

                opnSidebar.Controls.Add(ocnGridView); opnSidebar.Controls.Add(ocnDashBoard); opnSidebar.Controls.Add(olaAppTitle);

                Panel opnShadow = new Panel { Dock = DockStyle.Left, Width = 1, BackColor = Color.FromArgb(10, 20, 40) };

                opnContent = new Panel { Dock = DockStyle.Fill, BackColor = oContentBg };

                this.Controls.Add(opnContent); this.Controls.Add(opnShadow); this.Controls.Add(opnSidebar);
            }
            catch (Exception oEx)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog 
                { 
                    tFTProcessName = "wFormMain.W_PRCxSetupUI", 
                    tFTErrorMessage = oEx.Message,
                    tFTStackTrace = oEx.StackTrace 
                });
            }
        }

        private Button W_PRCopnCreateMenuButton(string ptText, int pnPositionY)
        {
            Button oBtn = new Button();
            try
            {
                oBtn = new Button 
                { 
                    Text = ptText, 
                    Font = new Font("Segoe UI", 11, FontStyle.Regular), 
                    ForeColor = Color.White, 
                    BackColor = oSidebarBg, 
                    FlatStyle = FlatStyle.Flat, 
                    TextAlign = ContentAlignment.MiddleLeft, 
                    Padding = new Padding(25, 0, 0, 0), 
                    Location = new Point(0, pnPositionY), 
                    Size = new Size(230, 60), 
                    Cursor = Cursors.Hand 
                };
                oBtn.FlatAppearance.BorderSize = 0;
                oBtn.FlatAppearance.MouseOverBackColor = oHoverBtn;
            }
            catch (Exception oEx)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog 
                { 
                    tFTProcessName = "wFormMain.W_PRCopnCreateMenuButton", 
                    tFTErrorMessage = oEx.Message, 
                    tFTStackTrace = oEx.StackTrace 
                });
            }
            return oBtn;
        }
    }
}
