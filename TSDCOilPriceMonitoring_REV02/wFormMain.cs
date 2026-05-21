using FontAwesome.Sharp;
using System.Drawing.Drawing2D;
using TSDCOilPriceMonitoring_REV02.Class;
using TSDCOilPriceMonitoring_REV02.Models;

namespace TSDCOilPriceMonitoring_REV02
{
    public partial class wFormMain : Form
    {
        private Panel opnSidebar, opnContent;
        private Button ocnDashBoard, ocnGridView, ocnChartSummary;
        private Panel opnActiveIndicator;
        private Form? oActiveForm = null;
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
                if (ocnChartSummary != null) ocnChartSummary.BackColor = oSidebarBg;

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

                Panel opnLogoArea = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 180
                };

                PictureBox opicLogo = new PictureBox
                {
                    Size = new Size(80, 80),
                    Location = new Point((230 - 80) / 2, 10),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BackColor = Color.Transparent
                };
                System.Drawing.Drawing2D.GraphicsPath oPath = new GraphicsPath();
                oPath.AddEllipse(0, 0, opicLogo.Width, opicLogo.Height);
                opicLogo.Region = new Region(oPath);

                try
                {
                    string tFileName = "TsdcLogoBlue.jpg";
                    string tImgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img", tFileName);

                    if (File.Exists(tImgPath))
                    {
                        opicLogo.Image = Image.FromFile(tImgPath);
                    }
                    else
                    {
                        opicLogo.Paint += (s, e) =>
                        {
                            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                            using (SolidBrush oBrush = new SolidBrush(Color.FromArgb(144, 202, 249)))
                            using (Font oFont = new Font("Segoe UI", 12, FontStyle.Bold))
                            {
                                e.Graphics.FillEllipse(oBrush, 10, 10, 60, 60);
                                e.Graphics.DrawString("TSDC", oFont, Brushes.DarkBlue, new PointF(15, 30));
                            }
                        };
                    }
                }
                catch (Exception oImgEx)
                {
                    oLog?.C_PRCxWriteErrorLog(new cmlErrorLog
                    {
                        tFTProcessName = "wFormMain.LoadLogo",
                        tFTErrorMessage = oImgEx.Message,
                        tFTStackTrace = oImgEx.StackTrace
                    });
                }

                Label olaAppTitle = new Label
                {
                    Text = "TSDC Oil Price\nMonitoring",
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    ForeColor = Color.FromArgb(144, 202, 249),
                    TextAlign = ContentAlignment.MiddleCenter,
                    AutoSize = false,
                    Height = 75,
                    Dock = DockStyle.Bottom,
                    Padding = new Padding(10, 0, 10, 10)
                };

                opnLogoArea.Controls.Add(opicLogo);
                opnLogoArea.Controls.Add(olaAppTitle);

                opnActiveIndicator = new Panel
                {
                    Width = 5,
                    BackColor = oActiveBtn,
                    Left = 0
                };
                opnSidebar.Controls.Add(opnActiveIndicator);

                ocnDashBoard = W_PRCopnCreateMenuButton("Dashboard", 180, FontAwesome.Sharp.IconChar.Home);
                ocnDashBoard.Click += (s, e) =>
                {
                    W_PRCxHighlightButton((Button)s);
                    W_PRCxOpenChildForm(new wFormDashBoard());
                };

                ocnGridView = W_PRCopnCreateMenuButton("Grid View", 240, FontAwesome.Sharp.IconChar.Table);
                ocnGridView.Click += (s, e) =>
                {
                    W_PRCxHighlightButton((Button)s);
                    W_PRCxOpenChildForm(new wFormGridView());
                };

                ocnChartSummary = W_PRCopnCreateMenuButton("Chart Summary", 300, FontAwesome.Sharp.IconChar.ChartBar);
                ocnChartSummary.Click += (s, e) =>
                {
                    W_PRCxHighlightButton((Button)s);
                    W_PRCxOpenChildForm(new wFormChartSummary());
                };

                opnSidebar.Controls.Add(ocnChartSummary);
                opnSidebar.Controls.Add(ocnGridView);
                opnSidebar.Controls.Add(ocnDashBoard);
                opnSidebar.Controls.Add(opnLogoArea);

                Panel opnShadow = new Panel
                {
                    Dock = DockStyle.Left,
                    Width = 1,
                    BackColor = Color.FromArgb(10, 20, 40)
                };

                opnContent = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = oContentBg
                };

                this.Controls.Add(opnContent);
                this.Controls.Add(opnShadow);
                this.Controls.Add(opnSidebar);
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

        private Button W_PRCopnCreateMenuButton(string ptText, int pnPositionY, IconChar poIcon)
        {
            FontAwesome.Sharp.IconButton oBtn = new FontAwesome.Sharp.IconButton();
            try
            {
                oBtn = new FontAwesome.Sharp.IconButton
                {
                    Text = "  " + ptText, 
                    IconChar = poIcon,
                    IconColor = Color.White,
                    IconSize = 30, 
                    Font = new Font("Segoe UI", 11, FontStyle.Regular),
                    ForeColor = Color.White,
                    BackColor = oSidebarBg,
                    FlatStyle = FlatStyle.Flat,

                    TextAlign = ContentAlignment.MiddleLeft,
                    ImageAlign = ContentAlignment.MiddleLeft,
                    TextImageRelation = TextImageRelation.ImageBeforeText,

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