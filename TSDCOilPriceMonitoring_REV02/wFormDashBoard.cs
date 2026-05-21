using FontAwesome.Sharp; 
using System.Drawing.Drawing2D;
using System.Reflection;
using TSDCOilPriceMonitoring_REV02.Class;
using TSDCOilPriceMonitoring_REV02.Class.Repository;
using TSDCOilPriceMonitoring_REV02.Models;

namespace TSDCOilPriceMonitoring_REV02
{
    public partial class wFormDashBoard : Form
    {
        private cFuelSummaryRepository oRepo;
        private cLogService oLog;
        private wFilterPanel oFilterPanel;
        private FlowLayoutPanel opnDashBoard;
        private wLoadingPanel oLoading;

        private Queue<Panel> oAnimationQueue = new Queue<Panel>();
        private System.Windows.Forms.Timer oStaggerTimer;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        public wFormDashBoard()
        {
            try
            {
                this.DoubleBuffered = true;
                this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

                oRepo = new cFuelSummaryRepository();
                oLog = new cLogService();

                W_PRCxSetupUI();

                this.Load += (s, e) => W_PRCxLoadDashboardCard();

                oLog.C_PRCxWriteEventLog(new cmlEventLog
                {
                    tFTEventName = "AppStart",
                    tFTDescription = "Dashboard Initialized."
                });
            }
            catch (Exception ex)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog
                {
                    tFTProcessName = "wFormDashBoard.Constructor",
                    tFTErrorMessage = ex.Message,
                    tFTStackTrace = ex.StackTrace
                });
            }
        }

        private async void W_PRCxLoadDashboardCard()
        {
            try
            {
                oLoading.W_PRCxStart();
                opnDashBoard.Visible = false;

                foreach (Control ctrl in opnDashBoard.Controls) ctrl.Dispose();
                opnDashBoard.Controls.Clear();
                oAnimationQueue.Clear();
                if (oStaggerTimer != null) oStaggerTimer.Stop();

                DateTime dStart = oFilterPanel.dStartDate;
                DateTime dEnd = oFilterPanel.dEndDate.AddDays(1).AddTicks(-1);

                List<int> aStations = oFilterPanel.aStationIds;
                List<int> aFuels = oFilterPanel.aFuelIds;

                List<cmlFuelSummary> oSummaries = await Task.Run(() =>
                    oRepo.C_PRCaoGetFuelSummary(dStart, dEnd, aStations, aFuels)
                );

                oLoading.W_PRCxStop();
                opnDashBoard.Visible = true;
                opnDashBoard.SuspendLayout();

                if (oSummaries != null && oSummaries.Count > 0)
                {
                    foreach (cmlFuelSummary oItem in oSummaries)
                    {
                        Panel opnCard = W_PRCopnCreateCard(oItem, dStart, oFilterPanel.dEndDate);

                        opnCard.Visible = false;
                        opnCard.Tag = 0;
                        opnCard.Margin = new Padding(15, 120, 15, -90);

                        opnDashBoard.Controls.Add(opnCard);
                        oAnimationQueue.Enqueue(opnCard);
                    }

                    W_PRCxCenterDashboardContent();

                    if (oStaggerTimer == null)
                    {
                        int nMaxStep = 10; 
                        oStaggerTimer = new System.Windows.Forms.Timer { Interval = 15 };
                        oStaggerTimer.Tick += (s, e) =>
                        {
                            opnDashBoard.SuspendLayout();

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

                            opnDashBoard.ResumeLayout(true);
                        };
                    }
                    oStaggerTimer.Start();
                }
                else
                {
                    Label olaNoData = new Label
                    {
                        Text = "No data found.",
                        AutoSize = false,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Width = opnDashBoard.ClientSize.Width - 30,
                        Height = 100,
                        Font = new Font("Segoe UI", 12),
                        ForeColor = Color.Gray,
                        Margin = new Padding(15)
                    };
                    opnDashBoard.Controls.Add(olaNoData);
                    W_PRCxCenterDashboardContent();
                }

                opnDashBoard.ResumeLayout();
            }
            catch (Exception ex)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog
                {
                    tFTProcessName = "wFormDashBoard.W_PRCxLoadDashboardCard",
                    tFTErrorMessage = ex.Message,
                    tFTStackTrace = ex.StackTrace
                });
                if (oLoading != null) oLoading.W_PRCxStop();
                if (opnDashBoard != null) opnDashBoard.Visible = true;
            }
        }

        private void W_PRCxCenterDashboardContent()
        {
            try
            {
                if (opnDashBoard.Controls.Count == 0) return;

                if (opnDashBoard.Controls.Count == 1 && opnDashBoard.Controls[0] is Label lbl)
                {
                    lbl.Width = opnDashBoard.ClientSize.Width - 30;
                    opnDashBoard.Padding = new Padding(15);
                    return;
                }

                int nAvailableWidth = opnDashBoard.ClientSize.Width;
                int nCardWidth = 360;

                int nColumns = Math.Max(1, nAvailableWidth / nCardWidth);

                if (nColumns > 3) nColumns = 3;

                if (nColumns > opnDashBoard.Controls.Count)
                    nColumns = opnDashBoard.Controls.Count;

                int nTotalContentWidth = nColumns * 350;
                int nPaddingLeft = Math.Max(15, (nAvailableWidth - nTotalContentWidth) / 2);

                opnDashBoard.Padding = new Padding(nPaddingLeft, 15, 15, 15);
            }
            catch { }
        }

        private Panel W_PRCpanelCreateDetailRow(IconChar poIcon, string ptText, Color poColor)
        {
            Panel opnRow = new Panel 
            { 
                Width = 280, 
                Height = 28, 
                BackColor = Color.Transparent 
            };
            IconPictureBox opicIcon = new IconPictureBox
            {
                IconChar = poIcon,
                IconColor = poColor,
                Size = new Size(20, 20),
                Location = new Point(0, 2),
                SizeMode = PictureBoxSizeMode.Zoom
            };
            Label olaText = new Label
            {
                Text = ptText,
                Font = new Font("Segoe UI", 10.5f),
                ForeColor = Color.FromArgb(80, 80, 80),
                AutoSize = true,
                Location = new Point(30, 2)
            };
            opnRow.Controls.Add(opicIcon);
            opnRow.Controls.Add(olaText);
            return opnRow;
        }

        private Panel W_PRCopnCreateCard(cmlFuelSummary oItem, DateTime dStart, DateTime dEndDisplay)
        {
            try
            {
                string tStationName = oItem.tStationName.Trim();
                string tFuelName = oItem.tFuelName.Trim().ToUpper();

                Color oHeaderColor = Color.FromArgb(0, 120, 212);

                if (tFuelName.Contains("วี-เพาเวอร์ แก๊สโซฮอล์") || tFuelName.Contains("ซูเปอร์พาวเวอร์") || (tFuelName.Contains("95") && tFuelName.Contains("พรีเมียม"))) oHeaderColor = Color.FromArgb(183, 28, 28);
                else if (tFuelName.Contains("วี-เพาเวอร์ ดีเซล") || tFuelName.Contains("ดีเซลพรีเมียม")) oHeaderColor = Color.FromArgb(40, 40, 40);
                else if (tFuelName.Contains("เบนซิน 95") || tFuelName.Contains("BENZINE")) oHeaderColor = Color.FromArgb(245, 127, 23);
                else if (tFuelName.Contains("E20")) oHeaderColor = Color.FromArgb(0, 105, 92);
                else if (tFuelName.Contains("E85")) oHeaderColor = Color.FromArgb(106, 27, 154);
                else if (tFuelName.Contains("91")) oHeaderColor = Color.FromArgb(46, 125, 50);
                else if (tFuelName.Contains("95")) oHeaderColor = Color.FromArgb(230, 81, 0);
                else if (tFuelName.Contains("B20")) oHeaderColor = Color.FromArgb(198, 40, 40);
                else if (tFuelName.Contains("ดีเซล") || tFuelName.Contains("DIESEL") || tFuelName.Contains("DISEL") || tFuelName.Contains("ฟิวเซฟ")) oHeaderColor = Color.FromArgb(21, 101, 192);
                else if (tFuelName.Contains("NGV")) oHeaderColor = Color.FromArgb(130, 119, 23);

                Panel opnCard = new Panel
                {
                    Width = 320,
                    Height = 340, 
                    BackColor = Color.White,
                    Margin = new Padding(15)
                };

                Panel opnHeader = new Panel 
                { 
                    Dock = DockStyle.Top, 
                    Height = 80, 
                    BackColor = oHeaderColor 
                };

                PictureBox opicStationLogo = new PictureBox 
                { 
                    Size = new Size(50, 50),
                    Location = new Point(15, 15), 
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BackColor = Color.Transparent 
                };
                string tImgFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img");

                if (Directory.Exists(tImgFolder))
                {
                    string[] aFiles = Directory.GetFiles(tImgFolder, $"{tStationName}.*", SearchOption.TopDirectoryOnly);
                    if (aFiles.Length > 0) opicStationLogo.Image = Image.FromFile(aFiles[0]);
                    else
                    {
                        opicStationLogo.Paint += (s, e) =>
                        {
                            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                            e.Graphics.FillEllipse(new SolidBrush(Color.White), 0, 0, 50, 50);
                            e.Graphics.DrawString(tStationName.Substring(0, 1), new Font("Segoe UI", 16, FontStyle.Bold), Brushes.Blue, new PointF(12, 10));
                        };
                    }
                }
                opnHeader.Controls.Add(opicStationLogo);

                Label olaTitle = new Label 
                { 
                    Text = $"{tStationName} - {tFuelName}".ToUpper(), 
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold), 
                    AutoSize = false, 
                    Width = 230, 
                    Height = 30, 
                    AutoEllipsis = true,
                    Location = new Point(75, 25) 
                };
                opnHeader.Controls.Add(olaTitle);

                FlowLayoutPanel opnDetails = new FlowLayoutPanel 
                {
                    FlowDirection = FlowDirection.TopDown,
                    Location = new Point(15, 90), 
                    Width = 290, 
                    Height = 230 
                };

                opnDetails.Controls.Add(W_PRCpanelCreateDetailRow(
                        IconChar.GasPump, 
                        $"Station : {tStationName}",
                        Color.FromArgb(0, 120, 21)
                    ));
                opnDetails.Controls.Add(W_PRCpanelCreateDetailRow(
                        IconChar.CalendarAlt, 
                        $"Period : {dStart:dd/MM/yy} - {dEndDisplay:dd/MM/yy}",
                        Color.Gray
                    ));
                opnDetails.Controls.Add(W_PRCpanelCreateDetailRow(
                        IconChar.ChartLine, 
                        $"Averages :฿ {oItem.cAvgPrice:F2} Baht/Lite", 
                        Color.FromArgb(230, 81, 0)
                    ));
                opnDetails.Controls.Add(W_PRCpanelCreateDetailRow(
                        IconChar.ArrowDown, 
                        $"Lowest Price :฿ {oItem.cMinPrice:F2} Baht/Lite",
                        Color.Green
                    ));
                opnDetails.Controls.Add(W_PRCpanelCreateDetailRow(
                        IconChar.ArrowUp,
                        $"Highest Price :฿ {oItem.cMaxPrice:F2} Baht/Lite", 
                        Color.Red
                    ));
                opnDetails.Controls.Add(W_PRCpanelCreateDetailRow( 
                        IconChar.Database, 
                        $"Records found : {oItem.nTotalRecords} items", 
                        Color.DarkSlateGray
                    ));

                opnCard.Controls.Add(opnDetails);
                opnCard.Controls.Add(opnHeader);

                GraphicsPath oPath = new GraphicsPath();
                int nRadius = 20;
                oPath.AddArc(0, 0, nRadius, nRadius, 180, 90);
                oPath.AddArc(opnCard.Width - nRadius, 0, nRadius, nRadius, 270, 90);
                oPath.AddArc(opnCard.Width - nRadius, opnCard.Height - nRadius, nRadius, nRadius, 0, 90);
                oPath.AddArc(0, opnCard.Height - nRadius, nRadius, nRadius, 90, 90);
                oPath.CloseFigure();
                opnCard.Region = new Region(oPath);

                return opnCard;
            }
            catch { return new Panel(); }
        }

        private void W_PRCxSetupUI()
        {
            try
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
                opnDashBoard.AutoScrollMargin = new Size(0, 20);
                typeof(FlowLayoutPanel).GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(opnDashBoard, true, null);

                this.Shown += (s, e) => W_PRCxCenterDashboardContent();
                opnDashBoard.Resize += (s, e) => W_PRCxCenterDashboardContent();

                oLoading = new wLoadingPanel();
                this.Controls.Add(oLoading);
                oLoading.BringToFront();

                this.Controls.Add(opnDashBoard);
                this.Controls.Add(oFilterPanel);
                oFilterPanel.SendToBack();
            }
            catch { }
        }
    }
}