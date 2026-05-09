using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TSDCOilPriceMonitoring_REV02.Class;
using TSDCOilPriceMonitoring_REV02.Models;

namespace TSDCOilPriceMonitoring_REV02
{
    public partial class wFormMain : Form
    {
        private Panel opnSidebar, opnContent;
        private Button ocnDashBoard, ocnGridView;
        private Form oActiveForm = null;
        private cLogService oLog = new cLogService();

        public wFormMain()
        {
            try
            {
                W_PRCxSetupUI();
                W_PRCxHighlightButton(ocnDashBoard);
                W_PRCxOpenChildForm(new wFormDashBoard());
            }
            catch (Exception ex)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog { tFTProcessName = "wFormMain.Constructor", tFTErrorMessage = ex.Message, tFTStackTrace = ex.StackTrace });
            }
        }

        private void W_PRCxOpenChildForm(Form poChildForm)
        {
            try
            {
                if (oActiveForm != null) { oActiveForm.Close(); oActiveForm.Dispose(); }
                oActiveForm = poChildForm;
                poChildForm.TopLevel = false; poChildForm.FormBorderStyle = FormBorderStyle.None; poChildForm.Dock = DockStyle.Fill;
                opnContent.Controls.Add(poChildForm); opnContent.Tag = poChildForm; poChildForm.BringToFront(); poChildForm.Show();
            }
            catch (Exception ex)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog { tFTProcessName = "wFormMain.W_PRCxOpenChildForm", tFTErrorMessage = ex.Message, tFTStackTrace = ex.StackTrace });
            }
        }

        private void W_PRCxHighlightButton(Button poActiveBtn)
        {
            try
            {
                ocnDashBoard.BackColor = Color.FromArgb(52, 73, 94); ocnGridView.BackColor = Color.FromArgb(52, 73, 94);
                if (poActiveBtn != null) poActiveBtn.BackColor = Color.FromArgb(41, 128, 185);
            }
            catch (Exception ex)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog { tFTProcessName = "wFormMain.W_PRCxHighlightButton", tFTErrorMessage = ex.Message, tFTStackTrace = ex.StackTrace });
            }
        }

        private void W_PRCxSetupUI()
        {
            try
            {
                this.Text = "TSDC Oil Price Monitoring - Main System"; this.Size = new Size(1200, 700); this.StartPosition = FormStartPosition.CenterScreen;

                opnSidebar = new Panel 
                { 
                    Dock = DockStyle.Left, 
                    Width = 220, 
                    BackColor = Color.FromArgb(44, 62, 80) 
                };
                Label olaAppTitle = new Label 
                { 
                    Text = "TSDC System", 
                    Font = new Font("Segoe UI", 16, FontStyle.Bold), 
                    ForeColor = Color.White, 
                    TextAlign = ContentAlignment.MiddleCenter, 
                    Dock = DockStyle.Top, 
                    Height = 80 
                };

                ocnDashBoard = W_PRCopnCreateMenuButton("🏠 Dashboard", 80);
                ocnDashBoard.Click += (s, e) => { W_PRCxHighlightButton((Button)s); W_PRCxOpenChildForm(new wFormDashBoard()); };

                ocnGridView = W_PRCopnCreateMenuButton("📊 Grid View", 130);
                ocnGridView.Click += (s, e) => { W_PRCxHighlightButton((Button)s); W_PRCxOpenChildForm(new wFormGridView()); };

                opnSidebar.Controls.Add(ocnGridView); opnSidebar.Controls.Add(ocnDashBoard); opnSidebar.Controls.Add(olaAppTitle);
                opnContent = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(240, 242, 245) };

                this.Controls.Add(opnContent); this.Controls.Add(opnSidebar);
            }
            catch (Exception ex)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog { tFTProcessName = "wFormMain.W_PRCxSetupUI", tFTErrorMessage = ex.Message, tFTStackTrace = ex.StackTrace });
            }
        }

        private Button W_PRCopnCreateMenuButton(string ptText, int pnPositionY)
        {
            try
            {
                return new Button { Text = ptText, Font = new Font("Segoe UI", 11), ForeColor = Color.White, BackColor = Color.FromArgb(52, 73, 94), FlatStyle = FlatStyle.Flat, FlatAppearance = { BorderSize = 0 }, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(20, 0, 0, 0), Location = new Point(0, pnPositionY), Size = new Size(220, 50), Cursor = Cursors.Hand };
            }
            catch (Exception ex)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog { tFTProcessName = "wFormMain.W_PRCopnCreateMenuButton", tFTErrorMessage = ex.Message, tFTStackTrace = ex.StackTrace });
                return new Button();
            }
        }
    }
}
