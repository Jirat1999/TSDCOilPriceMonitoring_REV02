using System.Drawing.Drawing2D;
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
        private Queue<Panel> oAnimationQueue = new Queue<Panel>(); 
        private System.Windows.Forms.Timer oStaggerTimer;
        private const int nMaxStep = 15;

        public wFormDashBoard()
        {
            try
            {
                this.DoubleBuffered = true; this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
                oRepo = new cFuelRepository(); 
                oLog = new cLogService();
                W_PRCxSetupUI(); 
                this.Load += (s, e) => W_PRCxLoadDashboardCard();
            }
            catch (Exception ex) 
            { 
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog 
                { 
                    tFTProcessName = "wFormDashBoard.Constructor", 
                    tFTErrorMessage = ex.Message 
                }); 
            }
        }

        private void W_PRCxLoadDashboardCard()
        {
            try
            {
                foreach (Control ctrl in opnDashBoard.Controls) ctrl.Dispose();
                opnDashBoard.Controls.Clear(); 
                oAnimationQueue.Clear();
                List<cmlFuelSummary> oSummaries = oRepo.C_PRCaoGetFuelSummary(oFilterPanel.dStartDate, oFilterPanel.dEndDate.AddDays(1).AddTicks(-1), oFilterPanel.nStationId, oFilterPanel.nFuelId);

                if (oSummaries.Count > 0)
                {
                    foreach (cmlFuelSummary oItem in oSummaries)
                    {
                        string tDetail = $"🏷️ Station : {oItem.tStationName}\n\n📈 Averages : {oItem.cAvgPrice:F2} ฿\n📉 Lowest Price : {oItem.cMinPrice:F2} ฿\n🚀 Highest Price : {oItem.cMaxPrice:F2} ฿\n\n📊 Records found : {oItem.nTotalRecords} items";
                        Panel opnCard = W_PRCopnCreateCard($"{oItem.tStationName} - {oItem.tFuelName}", tDetail);
                        opnCard.Visible = false; opnCard.Tag = 0; 
                        opnCard.Margin = new Padding(15, 120, 15, -90);
                        opnDashBoard.Controls.Add(opnCard); 
                        oAnimationQueue.Enqueue(opnCard);
                    }
                    if (oStaggerTimer == null)
                    {
                        oStaggerTimer = new System.Windows.Forms.Timer { Interval = 15 };
                        oStaggerTimer.Tick += (s, e) =>
                        {
                            if (oAnimationQueue.Count > 0)
                            {
                                Panel oCard = oAnimationQueue.Peek(); 
                                oCard.Visible = true; 
                                int nCurStep = (int)oCard.Tag;
                                if (nCurStep <= nMaxStep)
                                {
                                    float fProgress = (float)nCurStep / nMaxStep; 
                                    float fEaseOut = 1f - (float)Math.Pow(1f - fProgress, 2);
                                    int nMoveY = 120 - (int)(fEaseOut * 105); 
                                    oCard.Margin = new Padding(15, nMoveY, 15, 30 - nMoveY);
                                    oCard.Tag = nCurStep + 1;
                                }
                                else 
                                { 
                                    oCard.Margin = new Padding(15); 
                                    oAnimationQueue.Dequeue(); 
                                }
                            }
                            else 
                            { 
                                oStaggerTimer.Stop(); 
                            }
                        };
                    }
                    oStaggerTimer.Start();
                }
                else
                {
                    opnDashBoard.Controls.Add(new Label 
                    { 
                        Text = "No data found.", 
                        AutoSize = true, 
                        Font = new Font("Segoe UI", 12), 
                        ForeColor = Color.Gray, 
                        Margin = new Padding(20) 
                    });
                }
            }
            catch (Exception ex) 
            { 
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog 
                { 
                    tFTProcessName = "wFormDashBoard.Load", 
                    tFTErrorMessage = ex.Message 
                }); 
            }
        }

        private Panel W_PRCopnCreateCard(string ptTitle, string ptDetails)
        {
            Panel opnCard = new Panel 
            { 
                Width = 320, 
                AutoSize = true, 
                MinimumSize = new Size(320, 210),
                BackColor = Color.White, 
                Margin = new Padding(15) 
            };
            Panel opnHeader = new Panel 
            { 
                Dock = DockStyle.Top, 
                Height = 55, 
                BackColor = Color.FromArgb(0, 120, 212)
            };
            Label olaTitle = new Label 
            { 
                Text = ptTitle, 
                ForeColor = Color.White, 
                Font = new Font("Segoe UI", 13, FontStyle.Bold), 
                AutoSize = true, 
                Location = new Point(15, 15) 
            };
            Label olaDetails = new Label 
            { 
                Text = ptDetails, 
                ForeColor = Color.FromArgb(80, 80, 80), 
                Font = new Font("Segoe UI", 11), 
                AutoSize = true, 
                Location = new Point(15, 75), 
                Padding = new Padding(0, 0, 0, 20) 
            };
            opnHeader.Controls.Add(olaTitle); 
            opnCard.Controls.Add(olaDetails); 
            opnCard.Controls.Add(opnHeader);

            opnCard.Paint += (s, e) => { 
                GraphicsPath oPath = new GraphicsPath(); 
                int nRadius = 20; oPath.AddArc(0, 0, nRadius, nRadius, 180, 90); 
                oPath.AddArc(opnCard.Width - nRadius, 0, nRadius, nRadius, 270, 90); 
                oPath.AddArc(opnCard.Width - nRadius, opnCard.Height - nRadius, nRadius, nRadius, 0, 90); 
                oPath.AddArc(0, opnCard.Height - nRadius, nRadius, nRadius, 90, 90); 
                opnCard.Region = new Region(oPath); 
            };
            EventHandler oHoverEnter = (s, e) => { 
                opnCard.BackColor = Color.FromArgb(248, 250, 255); 
                opnHeader.BackColor = Color.FromArgb(0, 100, 190); 
                Cursor.Current = Cursors.Hand; 
            };
            EventHandler oHoverLeave = (s, e) => { 
                opnCard.BackColor = Color.White; 
                opnHeader.BackColor = Color.FromArgb(0, 120, 212); 
                Cursor.Current = Cursors.Default; 
            };
            opnCard.MouseEnter += oHoverEnter; 
            opnCard.MouseLeave += oHoverLeave; 
            opnHeader.MouseEnter += oHoverEnter; 
            opnHeader.MouseLeave += oHoverLeave; 
            olaTitle.MouseEnter += oHoverEnter; 
            olaTitle.MouseLeave += oHoverLeave; 
            olaDetails.MouseEnter += oHoverEnter; 
            olaDetails.MouseLeave += oHoverLeave;
            return opnCard;
        }

        private void W_PRCxSetupUI()
        {
            this.BackColor = Color.FromArgb(244, 247, 252); 
            this.Size = new Size(1000, 600);

            oFilterPanel = new wFilterPanel(); 
            oFilterPanel.Dock = DockStyle.Top; 
            oFilterPanel.W_PRCxInitialize(false); 
            oFilterPanel.oOnSearchClicked += (s, e) => W_PRCxLoadDashboardCard();
            opnDashBoard = new FlowLayoutPanel 
            { 
                Dock = DockStyle.Fill, 
                AutoScroll = true, 
                BackColor = Color.Transparent, 
                Padding = new Padding(15) 
            };

            this.Controls.Add(opnDashBoard); 
            this.Controls.Add(oFilterPanel); 
            oFilterPanel.SendToBack();
        }
    }
}