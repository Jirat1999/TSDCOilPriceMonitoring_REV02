using System.Windows.Forms.DataVisualization.Charting;
using TSDCOilPriceMonitoring_REV02.Class;
using TSDCOilPriceMonitoring_REV02.Class.Repository;
using TSDCOilPriceMonitoring_REV02.Models;

namespace TSDCOilPriceMonitoring_REV02
{
    public partial class wFormChartSummary : Form
    {
        private cFuelPriceDetailRepository oRepo;
        private cStationRepository oStationRepo;
        private cFuelTypeRepository oFuelRepo;
        private cLogService oLog;
        private Chart oChart;
        private wLoadingPanel oLoading;

        private ComboBox ocbStation, ocbFuelType;
        private DateTimePicker odtStart, odtEnd;

        private System.Windows.Forms.Timer oAnimTimer;
        private List<cmlChartAnimData> oAnimDataList;

        private PictureBox opicStationLogo;
        private Label olaChartTitle;

        public wFormChartSummary()
        {
            try
            {
                oRepo = new cFuelPriceDetailRepository();
                oStationRepo = new cStationRepository();
                oFuelRepo = new cFuelTypeRepository();
                oLog = new cLogService();

                W_PRCxSetupUI();
                W_PRCxLoadDropdownData();
                this.Load += (s, e) => W_PRCxLoadChartData();

                oLog.C_PRCxWriteEventLog(new cmlEventLog
                {
                    tFTEventName = "FormInit",
                    tFTDescription = "wFormChartSummary Initialized successfully."
                });
            }
            catch (Exception ex)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog
                {
                    tFTProcessName = "wFormChartSummary.Constructor",
                    tFTErrorMessage = ex.Message,
                    tFTStackTrace = ex.StackTrace
                });
            }
        }

        private void W_PRCxSetupUI()
        {
            try
            {
                this.BackColor = Color.FromArgb(244, 247, 252);
                this.Size = new Size(1100, 650);
                this.Text = "Fuel Price Trend Summary";

                Panel opnFilter = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 80,
                    BackColor = Color.White
                };

                Panel opnDivider = new Panel
                {
                    Dock = DockStyle.Bottom,
                    Height = 1,
                    BackColor = Color.FromArgb(230, 235, 240)
                };
                opnFilter.Controls.Add(opnDivider);

                Label olaPeriod = new Label
                {
                    Text = "Period",
                    Font = new Font("Segoe UI", 9, FontStyle.Regular),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Location = new Point(20, 15)
                };
                odtStart = new DateTimePicker
                {
                    Location = new Point(20, 35),
                    Width = 120,
                    Format = DateTimePickerFormat.Short,
                    Value = DateTime.Now.AddMonths(-12),
                    Font = new Font("Segoe UI", 10)
                };
                Label olaTo = new Label
                {
                    Text = "-",
                    Font = new Font("Segoe UI", 10),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Location = new Point(145, 38)
                };
                odtEnd = new DateTimePicker
                {
                    Location = new Point(165, 35),
                    Width = 120,
                    Format = DateTimePickerFormat.Short,
                    Value = DateTime.Now,
                    Font = new Font("Segoe UI", 10)
                };

                Label olaStation = new Label
                {
                    Text = "Station",
                    Font = new Font("Segoe UI", 9, FontStyle.Regular),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Location = new Point(310, 15)
                };
                ocbStation = new ComboBox
                {
                    Location = new Point(310, 35),
                    Width = 180,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Font = new Font("Segoe UI", 10)
                };

                Label olaFuel = new Label
                {
                    Text = "Fuel Type",
                    Font = new Font("Segoe UI", 9, FontStyle.Regular),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Location = new Point(510, 15)
                };
                ocbFuelType = new ComboBox
                {
                    Location = new Point(510, 35),
                    Width = 180,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Font = new Font("Segoe UI", 10)
                };

                FontAwesome.Sharp.IconButton btnSearch = new FontAwesome.Sharp.IconButton
                {
                    Text = " Search",
                    IconChar = FontAwesome.Sharp.IconChar.Search,
                    IconColor = Color.White,
                    IconSize = 18,
                    TextImageRelation = TextImageRelation.ImageBeforeText,
                    TextAlign = ContentAlignment.MiddleCenter,
                    ImageAlign = ContentAlignment.MiddleCenter,
                    Location = new Point(710, 32),
                    Width = 110,
                    Height = 33,
                    BackColor = Color.FromArgb(0, 120, 212),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };
                btnSearch.FlatAppearance.BorderSize = 0;
                btnSearch.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 90, 180);
                btnSearch.Click += (s, e) => W_PRCxLoadChartData();

                opnFilter.Controls.AddRange(new Control[] { olaPeriod, odtStart, olaTo, odtEnd, olaStation, ocbStation, olaFuel, ocbFuelType, btnSearch });

                Panel opnChartHeader = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 50,
                    BackColor = Color.White
                };

                FlowLayoutPanel opnTitleContainer = new FlowLayoutPanel
                {
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink, 
                    WrapContents = false,
                    BackColor = Color.Transparent,
                    FlowDirection = FlowDirection.LeftToRight
                };

                opicStationLogo = new PictureBox
                {
                    Size = new Size(35, 35),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BackColor = Color.Transparent,
                    Visible = false,
                    Margin = new Padding(0, 0, 10, 0) 
                };

                olaChartTitle = new Label
                {
                    Text = "Fuel Price Analysis",
                    Font = new Font("Segoe UI", 18, FontStyle.Bold),
                    ForeColor = Color.FromArgb(15, 32, 67),
                    AutoSize = true,
                    Margin = new Padding(0, 3, 0, 0) 
                };

                opnTitleContainer.Controls.Add(opicStationLogo);
                opnTitleContainer.Controls.Add(olaChartTitle);
                opnChartHeader.Controls.Add(opnTitleContainer);

                Action centerTitle = () =>
                {
                    opnTitleContainer.Left = (opnChartHeader.Width - opnTitleContainer.Width) / 2;
                    opnTitleContainer.Top = (opnChartHeader.Height - opnTitleContainer.Height) / 2;
                };

                opnChartHeader.Resize += (s, e) => centerTitle();
                opnTitleContainer.SizeChanged += (s, e) => centerTitle();

                oChart = new Chart
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.White,
                    Margin = new Padding(20)
                };

                ChartArea area = new ChartArea("MainArea");
                area.BackColor = Color.White;

                area.AxisX.MajorGrid.Enabled = true;
                area.AxisX.MajorGrid.LineColor = Color.FromArgb(242, 242, 242);
                area.AxisY.MajorGrid.LineColor = Color.FromArgb(235, 235, 235);
                area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;

                area.AxisX.LineColor = Color.LightGray;
                area.AxisY.LineColor = Color.LightGray;
                area.AxisX.LabelStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                area.AxisY.LabelStyle.Font = new Font("Segoe UI", 10);
                area.AxisX.LabelStyle.ForeColor = Color.FromArgb(50, 50, 50);
                area.AxisY.LabelStyle.ForeColor = Color.FromArgb(80, 80, 80);
                area.AxisY.Title = "Price (Baht / Litre)";
                area.AxisY.TitleFont = new Font("Segoe UI", 10, FontStyle.Bold);

                area.AxisY.Interval = 0.5;

                oChart.ChartAreas.Add(area);

                Series sMax = new Series("Highest")
                {
                    ChartType = SeriesChartType.Column,
                    Color = Color.FromArgb(220, 53, 69)
                };
                sMax.IsValueShownAsLabel = true;
                sMax.LabelFormat = "F2";
                sMax.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                sMax.LabelForeColor = Color.FromArgb(220, 53, 69);
                sMax.LabelAngle = -45;
                sMax.SmartLabelStyle.Enabled = false;

                Series oAvg = new Series("Average")
                {
                    ChartType = SeriesChartType.Column,
                    Color = Color.FromArgb(253, 126, 20)
                };
                oAvg.IsValueShownAsLabel = true;
                oAvg.LabelFormat = "F2";
                oAvg.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                oAvg.LabelForeColor = Color.FromArgb(253, 126, 20);
                oAvg.LabelAngle = -45;
                oAvg.SmartLabelStyle.Enabled = false;

                Series oMin = new Series("Lowest")
                {
                    ChartType = SeriesChartType.Column,
                    Color = Color.FromArgb(40, 167, 69)
                };
                oMin.IsValueShownAsLabel = true;
                oMin.LabelFormat = "F2";
                oMin.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                oMin.LabelForeColor = Color.FromArgb(40, 167, 69);
                oMin.LabelAngle = -45;
                oMin.SmartLabelStyle.Enabled = false;

                oChart.Series.Add(sMax); oChart.Series.Add(oAvg); oChart.Series.Add(oMin);

                Legend oLegend = new Legend
                {
                    Docking = Docking.Top,
                    Alignment = StringAlignment.Center,
                    Font = new Font("Segoe UI", 10)
                };
                oLegend.IsTextAutoFit = false;
                oChart.Legends.Add(oLegend);

                Panel opnContent = new Panel
                {
                    Dock = DockStyle.Fill,
                    Padding = new Padding(20)
                };
                opnContent.Controls.Add(oChart);
                opnContent.Controls.Add(opnChartHeader);
                oChart.BringToFront();

                oLoading = new wLoadingPanel();
                this.Controls.Add(oLoading);
                this.Controls.Add(opnContent);
                this.Controls.Add(opnFilter);
                oLoading.BringToFront();

                oAnimTimer = new System.Windows.Forms.Timer { Interval = 20 };
                oAnimTimer.Tick += W_PRCxAnimTimer_Tick;

                oLog.C_PRCxWriteEventLog(new cmlEventLog
                {
                    tFTEventName = "SetupUI",
                    tFTDescription = "UI components generated successfully."
                });
            }
            catch (Exception ex)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog
                {
                    tFTProcessName = "wFormChartSummary.W_PRCxSetupUI",
                    tFTErrorMessage = ex.Message,
                    tFTStackTrace = ex.StackTrace
                });
            }
        }

        private void W_PRCxLoadDropdownData()
        {
            try
            {
                List<cmlDropdownItem> stations = oStationRepo.C_PRCaoGetStations();
                stations.Insert(0, new cmlDropdownItem { nId = 0, tName = "All Stations" });

                ocbStation.DataSource = stations;
                ocbStation.DisplayMember = "tName";
                ocbStation.ValueMember = "nId";

                var oPtt = stations.FirstOrDefault(x => x.tName != null && x.tName.ToUpper().Contains("PTT"));
                if (oPtt != null) ocbStation.SelectedValue = oPtt.nId;

                List<cmlDropdownItem> fuels = oFuelRepo.C_PRCaoGetFuelTypes();
                fuels.Insert(0, new cmlDropdownItem { nId = 0, tName = "All Fuels" });

                ocbFuelType.DataSource = fuels;
                ocbFuelType.DisplayMember = "tName";
                ocbFuelType.ValueMember = "nId";

                var oGas91 = fuels.FirstOrDefault(x => x.tName != null && x.tName.Contains("แก๊สโซฮอล์ 91"));
                if (oGas91 != null) ocbFuelType.SelectedValue = oGas91.nId;

                oLog.C_PRCxWriteEventLog(new cmlEventLog { tFTEventName = "LoadDropdown", tFTDescription = "Dropdown data loaded successfully." });
            }
            catch (Exception ex)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog { tFTProcessName = "wFormChartSummary.W_PRCxLoadDropdownData", tFTErrorMessage = ex.Message, tFTStackTrace = ex.StackTrace });
            }
        }

        private async void W_PRCxLoadChartData()
        {
            try
            {
                oLoading.W_PRCxStart();
                oAnimTimer.Stop();

                oChart.Series["Highest"].Points.Clear();
                oChart.Series["Average"].Points.Clear();
                oChart.Series["Lowest"].Points.Clear();

                oChart.ChartAreas["MainArea"].AxisX.CustomLabels.Clear();

                List<int> sIds = (int)ocbStation.SelectedValue > 0 ? new List<int> { (int)ocbStation.SelectedValue } : null;
                List<int> fIds = (int)ocbFuelType.SelectedValue > 0 ? new List<int> { (int)ocbFuelType.SelectedValue } : null;

                DateTime dStartDate = odtStart.Value.Date;
                DateTime dEndDate = odtEnd.Value.Date.AddDays(1).AddTicks(-1);

                var details = await Task.Run(() => oRepo.C_PRCaoGetFuelPriceDetails(dStartDate, dEndDate, sIds, fIds));

                string tStationName = ocbStation.Text;
                olaChartTitle.Text = $"Fuel Price Trend: {tStationName.ToUpper()} - {ocbFuelType.Text}";

                if (opicStationLogo.Image != null)
                {
                    opicStationLogo.Image.Dispose();
                    opicStationLogo.Image = null;
                }

                string tImgFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img");
                if (System.IO.Directory.Exists(tImgFolder) && tStationName != "All Stations")
                {
                    string[] aFiles = System.IO.Directory.GetFiles(tImgFolder, $"{tStationName}.*", System.IO.SearchOption.TopDirectoryOnly);
                    if (aFiles.Length > 0)
                    {
                        opicStationLogo.Image = Image.FromFile(aFiles[0]);
                        opicStationLogo.Visible = true;
                    }
                    else
                    {
                        opicStationLogo.Visible = false;
                    }
                }
                else
                {
                    opicStationLogo.Visible = false;
                }

                if (details != null && details.Any())
                {
                    var monthlyData = details.GroupBy(x => new { x.dEffectiveDate.Year, x.dEffectiveDate.Month })
                                             .Select(g => new
                                             {
                                                 nYear = g.Key.Year,
                                                 nMonth = g.Key.Month,
                                                 dDate = new DateTime(g.Key.Year, g.Key.Month, 1),
                                                 Max = g.Max(x => x.cPricedPrice),
                                                 Min = g.Min(x => x.cPricedPrice),
                                                 Avg = g.Average(x => x.cPricedPrice)
                                             })
                                             .OrderBy(x => x.dDate)
                                             .ToList();

                    oAnimDataList = new List<cmlChartAnimData>();

                    decimal nGlobalMin = monthlyData.Min(x => x.Min);
                    decimal nGlobalMax = monthlyData.Max(x => x.Max);

                    double nYMinAxis = (double)(nGlobalMin > 5 ? nGlobalMin - 2 : 0);
                    double nYMaxAxis = (double)(nGlobalMax + 6m);

                    oChart.ChartAreas[0].AxisY.Minimum = nYMinAxis;
                    oChart.ChartAreas[0].AxisY.Maximum = nYMaxAxis;

                    string[] aThaiMonths = { "", "มกรา", "กุมภา", "มีนา", "เมษา", "พฤษภา", "มิถุนา", "กรกฎา", "สิงหา", "กันยา", "ตุลา", "พฤศจิกา", "ธันวา" };

                    double dXPosition = 1.0;

                    var oYearGroups = monthlyData.GroupBy(x => x.nYear);

                    foreach (var oYearGroup in oYearGroups)
                    {
                        double dYearStart = dXPosition - 0.5;

                        foreach (var item in oYearGroup)
                        {
                            oChart.ChartAreas["MainArea"].AxisX.CustomLabels.Add(
                                dXPosition - 0.5,
                                dXPosition + 0.5,
                                aThaiMonths[item.nMonth],
                                0,
                                LabelMarkStyle.None
                            );

                            oAnimDataList.Add(new cmlChartAnimData
                            {
                                tMonthLabel = aThaiMonths[item.nMonth],
                                cTargetMax = item.Max,
                                cTargetAvg = item.Avg,
                                cTargetMin = item.Min,
                                cCurMax = (decimal)nYMinAxis,
                                cCurAvg = (decimal)nYMinAxis,
                                cCurMin = (decimal)nYMinAxis
                            });

                            oChart.Series["Highest"].Points.AddXY(dXPosition, nYMinAxis);
                            oChart.Series["Average"].Points.AddXY(dXPosition, nYMinAxis);
                            oChart.Series["Lowest"].Points.AddXY(dXPosition, nYMinAxis);

                            dXPosition += 1.0;
                        }

                        double dYearEnd = dXPosition - 0.5;

                        oChart.ChartAreas["MainArea"].AxisX.CustomLabels.Add(
                            dYearStart,
                            dYearEnd,
                            oYearGroup.Key.ToString(),
                            1,
                            LabelMarkStyle.LineSideMark
                        );
                    }

                    oAnimTimer.Start();
                }

                oLog.C_PRCxWriteEventLog(new cmlEventLog { tFTEventName = "LoadChartData", tFTDescription = "Chart data generated and animation started." });
            }
            catch (Exception ex)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog { tFTProcessName = "wFormChartSummary.W_PRCxLoadChartData", tFTErrorMessage = ex.Message, tFTStackTrace = ex.StackTrace });
                MessageBox.Show("An error occurred while loading chart data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (oLoading != null) oLoading.W_PRCxStop();
            }
        }

        private void W_PRCxAnimTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                bool bIsFinished = true;

                for (int i = 0; i < oAnimDataList.Count; i++)
                {
                    var oData = oAnimDataList[i];

                    oData.cCurMax += (oData.cTargetMax - oData.cCurMax) * 0.15m;
                    oData.cCurAvg += (oData.cTargetAvg - oData.cCurAvg) * 0.15m;
                    oData.cCurMin += (oData.cTargetMin - oData.cCurMin) * 0.15m;

                    if (Math.Abs(oData.cTargetMax - oData.cCurMax) < 0.05m) oData.cCurMax = oData.cTargetMax;
                    else bIsFinished = false;

                    if (Math.Abs(oData.cTargetAvg - oData.cCurAvg) < 0.05m) oData.cCurAvg = oData.cTargetAvg;
                    else bIsFinished = false;

                    if (Math.Abs(oData.cTargetMin - oData.cCurMin) < 0.05m) oData.cCurMin = oData.cTargetMin;
                    else bIsFinished = false;

                    oChart.Series["Highest"].Points[i].YValues[0] = (double)oData.cCurMax;
                    oChart.Series["Average"].Points[i].YValues[0] = (double)oData.cCurAvg;
                    oChart.Series["Lowest"].Points[i].YValues[0] = (double)oData.cCurMin;
                }

                oChart.Invalidate();

                if (bIsFinished)
                {
                    oAnimTimer.Stop();
                }
            }
            catch (Exception ex)
            {
                oAnimTimer?.Stop();
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog
                {
                    tFTProcessName = "wFormChartSummary.W_PRCxAnimTimer_Tick",
                    tFTErrorMessage = ex.Message,
                    tFTStackTrace = ex.StackTrace
                });
            }
        }
    }
}