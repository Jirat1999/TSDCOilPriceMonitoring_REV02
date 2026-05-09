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
    public partial class wFormDashBoard : Form
    {
        private cFuelRepository oRepo;
        private cLogService oLog;
        private wFilterPanel oFilterPanel;
        private FlowLayoutPanel opnDashBoard;

        public wFormDashBoard()
        {
            try
            {
                oRepo = new cFuelRepository();
                oLog = new cLogService();
                W_PRCxSetupUI();
                this.Load += (s, e) => W_PRCxLoadDashboardCard();
                oLog.C_PRCxWriteEventLog(new cmlEventLog { tFTEventName = "AppStart", tFTDescription = "Dashboard Initialized." });
            }
            catch (Exception ex)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog { tFTProcessName = "wFormDashBoard.Constructor", tFTErrorMessage = ex.Message, tFTStackTrace = ex.StackTrace });
            }
        }

        private void W_PRCxLoadDashboardCard()
        {
            try
            {
                foreach (Control ctrl in opnDashBoard.Controls) ctrl.Dispose();
                opnDashBoard.Controls.Clear();

                List<cmlFuelSummary> oSummaries = oRepo.GetFuelSummary(oFilterPanel.dStartDate, oFilterPanel.dEndDate.AddDays(1).AddTicks(-1), oFilterPanel.nStationId, oFilterPanel.nFuelId);

                if (oSummaries.Count > 0)
                {
                    foreach (cmlFuelSummary oItem in oSummaries)
                    {
                        string tDetail = $"Station : {oItem.tStationName}\n" +
                                         $"Averages : {oItem.cAvgPrice:F2} ฿\n" +
                                         $"Lowest Price : {oItem.cMinPrice:F2} ฿\n" +
                                         $"Highest Price : {oItem.cMaxPrice:F2} ฿\n" +
                                         $"Found {oItem.nTotalRecords} items";
                        Panel opnCard = W_PRCopnCreateCard($"{oItem.tStationName} - {oItem.tFuelName}", tDetail, Color.FromArgb(230, 126, 34));
                        opnDashBoard.Controls.Add(opnCard);
                    }
                }
                else
                {
                    Label olaNoData = new Label { Text = "No data found.", AutoSize = true, Font = new Font("Segoe UI", 12), ForeColor = Color.Gray, Margin = new Padding(20) };
                    opnDashBoard.Controls.Add(olaNoData);
                }
            }
            catch (Exception ex)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog { tFTProcessName = "wFormDashBoard.W_PRCxLoadDashboardCard", tFTErrorMessage = ex.Message, tFTStackTrace = ex.StackTrace });
                MessageBox.Show("เกิดข้อผิดพลาดในการโหลดข้อมูล: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Panel W_PRCopnCreateCard(string ptTitle, string ptDetails, Color poBgColor)
        {
            try
            {
                Panel opnCard = new Panel 
                { 
                    Width = 280, 
                    AutoSize = true, 
                    MinimumSize = new Size(280, 180), 
                    BackColor = poBgColor, 
                    Margin = new Padding(15), 
                    Padding = new Padding(15) 
                };
                Label olaTitle = new Label 
                { 
                    Text = ptTitle, 
                    ForeColor = Color.White, 
                    Font = new Font("Segoe UI", 14, FontStyle.Bold), 
                    AutoSize = true, 
                    MaximumSize = new Size(250, 0), 
                    Dock = DockStyle.Top 
                };
                Panel opnSpacer = new Panel 
                { 
                    Height = 10, 
                    Dock = DockStyle.Top 
                };
                Label olaDetails = new Label 
                { 
                    Text = ptDetails, 
                    ForeColor = Color.White, 
                    Font = new Font("Segoe UI", 11, FontStyle.Regular), 
                    AutoSize = true, 
                    MaximumSize = new Size(250, 0), 
                    Dock = DockStyle.Top 
                };

                opnCard.Controls.Add(olaDetails); opnCard.Controls.Add(opnSpacer); opnCard.Controls.Add(olaTitle);
                olaTitle.BringToFront(); opnSpacer.BringToFront(); olaDetails.BringToFront();
                return opnCard;
            }
            catch (Exception ex)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog { tFTProcessName = "wFormDashBoard.W_PRCopnCreateCard", tFTErrorMessage = ex.Message, tFTStackTrace = ex.StackTrace });
                return new Panel();
            }
        }

        private void W_PRCxSetupUI()
        {
            try
            {
                this.Size = new Size(1000, 600);
                oFilterPanel = new wFilterPanel();
                oFilterPanel.Dock = DockStyle.Top;
                oFilterPanel.W_PRCxInitialize(false);
                oFilterPanel.oOnSearchClicked += (s, e) => W_PRCxLoadDashboardCard();

                opnDashBoard = new FlowLayoutPanel 
                { 
                    Dock = DockStyle.Fill, 
                    AutoScroll = true, 
                    BackColor = Color.FromArgb(240, 242, 245), 
                    Padding = new Padding(10) 
                };

                this.Controls.Add(opnDashBoard); this.Controls.Add(oFilterPanel);
                oFilterPanel.SendToBack();
            }
            catch (Exception ex)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog { tFTProcessName = "wFormDashBoard.W_PRCxSetupUI", tFTErrorMessage = ex.Message, tFTStackTrace = ex.StackTrace });
            }
        }
    }
}
