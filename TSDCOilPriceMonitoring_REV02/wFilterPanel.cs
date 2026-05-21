using System.Drawing.Drawing2D;
using TSDCOilPriceMonitoring_REV02.Class;
using TSDCOilPriceMonitoring_REV02.Class.Repository;
using TSDCOilPriceMonitoring_REV02.Models;

namespace TSDCOilPriceMonitoring_REV02
{
    public partial class wFilterPanel : UserControl
    {
        private cStationRepository oStationRepo;
        private cFuelTypeRepository oFuelTypeRepo;
        private cLogService oLog;

        private DateTimePicker odpStart, odpEnd;
        private Button ocnSearch, ocnReset, ocnExport;

        private Button obtnStation, obtnFuelType;
        private CheckedListBox oclbStation, oclbFuelType;
        private ToolStripDropDown oDropStation, oDropFuelType;

        private bool bIsUpdatingStation = false;
        private bool bIsUpdatingFuel = false;

        public event EventHandler oOnSearchClicked;
        public event EventHandler oOnExportClicked;

        public DateTime dStartDate => odpStart.Value.Date;
        public DateTime dEndDate => odpEnd.Value.Date;

        public List<int> aStationIds
        {
            get
            {
                List<int> oList = new List<int>();
                foreach (cmlDropdownItem oItem in oclbStation.CheckedItems)
                {
                    if (oItem.nId > 0) oList.Add(oItem.nId);
                }
                return oList;
            }
        }

        public List<int> aFuelIds
        {
            get
            {
                List<int> oList = new List<int>();
                foreach (cmlDropdownItem oItem in oclbFuelType.CheckedItems)
                {
                    if (oItem.nId > 0) oList.Add(oItem.nId);
                }
                return oList;
            }
        }

        public void W_PRCxInitialize(bool pbShowExport = true)
        {
            try
            {
                oStationRepo = new cStationRepository();
                oFuelTypeRepo = new cFuelTypeRepository();
                oLog = new cLogService();

                W_PRCxSetupUI();
                ocnExport.Visible = pbShowExport;
                odpStart.Value = DateTime.Now.AddDays(-7);
                odpEnd.Value = DateTime.Now;
                W_PRCxLoadMasterData();
            }
            catch (Exception oEx)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog
                {
                    tFTProcessName = "wFilterPanel.W_PRCxInitialize",
                    tFTErrorMessage = oEx.Message,
                    tFTStackTrace = oEx.StackTrace
                });
            }
        }

        private void W_PRCxLoadMasterData()
        {
            try
            {
                List<cmlDropdownItem> oStaions = oStationRepo.C_PRCaoGetStations();
                oclbStation.Items.Clear();
                oclbStation.DisplayMember = "tName";
                oclbStation.ValueMember = "nId";

                oclbStation.Items.Add(new cmlDropdownItem { nId = 0, tName = "--- All Stations ---" });
                if (oStaions != null)
                {
                    foreach (var oItem in oStaions) oclbStation.Items.Add(oItem);
                }

                List<cmlDropdownItem> oFuelType = oFuelTypeRepo.C_PRCaoGetFuelTypes();
                oclbFuelType.Items.Clear();
                oclbFuelType.DisplayMember = "tName";
                oclbFuelType.ValueMember = "nId";

                oclbFuelType.Items.Add(new cmlDropdownItem { nId = 0, tName = "--- All Fuel Types ---" });
                if (oFuelType != null)
                {
                    foreach (var oItem in oFuelType) oclbFuelType.Items.Add(oItem);
                }

                bIsUpdatingStation = true;
                bool bFoundPTT = false;

                for (int i = 1; i < oclbStation.Items.Count; i++)
                {
                    cmlDropdownItem oItem = (cmlDropdownItem)oclbStation.Items[i];
                    if (oItem.tName != null && oItem.tName.ToUpper().Contains("PTT"))
                    {
                        oclbStation.SetItemChecked(i, true);
                        obtnStation.Text = "1 Selected";
                        bFoundPTT = true;
                        break;
                    }
                }

                if (!bFoundPTT && oclbStation.Items.Count > 0)
                {
                    oclbStation.SetItemChecked(0, true);
                    obtnStation.Text = "--- All Stations ---";
                }
                bIsUpdatingStation = false;

                bIsUpdatingFuel = true;
                if (oclbFuelType.Items.Count > 0)
                {
                    oclbFuelType.SetItemChecked(0, true);
                    obtnFuelType.Text = "--- All Fuel Types ---";
                }
                bIsUpdatingFuel = false;
            }
            catch (Exception oEx)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog
                {
                    tFTProcessName = "wFilterPanel.W_PRCxLoadMasterData",
                    tFTErrorMessage = oEx.Message,
                    tFTStackTrace = oEx.StackTrace
                });
            }
        }

        private void W_PRCxSetupUI()
        {
            try
            {
                this.Dock = DockStyle.Top;
                this.Height = 120;
                this.BackColor = Color.White;

                Panel opnBottomLine = new Panel
                {
                    Dock = DockStyle.Bottom,
                    Height = 1,
                    BackColor = Color.FromArgb(224, 224, 224)
                };
                this.Controls.Add(opnBottomLine);

                Label olaDate = new Label 
                { 
                    Text = "Date:", 
                    AutoSize = true,
                    Location = new Point(15, 23), 
                    ForeColor = Color.FromArgb(64, 64, 64)
                };
                odpStart = new DateTimePicker 
                { 
                    Format = DateTimePickerFormat.Short, 
                    Width = 140, 
                    Location = new Point(65, 20), 
                    Font = new Font("Segoe UI", 9.5f)
                };

                Label olaTo = new Label 
                { 
                    Text = "To:", 
                    AutoSize = true, 
                    Location = new Point(225, 23),
                    ForeColor = Color.FromArgb(64, 64, 64) 
                };
                odpEnd = new DateTimePicker 
                { 
                    Format = DateTimePickerFormat.Short,
                    Width = 140, 
                    Location = new Point(260, 20), 
                    Font = new Font("Segoe UI", 9.5f) 
                };

                Label olaStation = new Label 
                { 
                    Text = "Station:", 
                    AutoSize = true,
                    Location = new Point(15, 73),
                    ForeColor = Color.FromArgb(64, 64, 64)
                };

                obtnStation = new Button
                {
                    Text = "--- All Stations ---",
                    Width = 220,
                    Height = 32,
                    Location = new Point(80, 68),
                    BackColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 9.5f)
                };
                obtnStation.FlatAppearance.BorderColor = Color.LightGray;
                oDropStation = W_PRCoCreateMultiSelectDropdown(obtnStation, out oclbStation, "Station");

                Label olaFuel = new Label 
                { 
                    Text = "Fuel:", 
                    AutoSize = true, Location = new Point(320, 73), 
                    ForeColor = Color.FromArgb(64, 64, 64)
                };

                obtnFuelType = new Button
                {
                    Text = "--- All Fuel Types ---",
                    Width = 220,
                    Height = 32,
                    Location = new Point(365, 68),
                    BackColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 9.5f)
                };
                obtnFuelType.FlatAppearance.BorderColor = Color.LightGray;
                oDropFuelType = W_PRCoCreateMultiSelectDropdown(obtnFuelType, out oclbFuelType, "Fuel");

                ocnSearch = new Button
                {
                    Text = "Search",
                    Width = 100,
                    Height = 40,
                    Location = new Point(610, 63),
                    BackColor = Color.FromArgb(0, 120, 212),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Font = new Font("Segoe UI", 9.5f)
                };
                ocnSearch.FlatAppearance.BorderSize = 0;
                W_PRCxApplyRoundedCorners(ocnSearch, 8);
                ocnSearch.Click += (s, e) => oOnSearchClicked?.Invoke(this, e);

                ocnReset = new Button
                {
                    Text = "Reset",
                    Width = 100,
                    Height = 40,
                    Location = new Point(720, 63),
                    BackColor = Color.FromArgb(108, 117, 125),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Font = new Font("Segoe UI", 9.5f)
                };
                ocnReset.FlatAppearance.BorderSize = 0;
                W_PRCxApplyRoundedCorners(ocnReset, 8);

                ocnReset.Click += (oSender, oE) =>
                {
                    odpStart.Value = DateTime.Now.AddDays(-7);
                    odpEnd.Value = DateTime.Now;

                    bIsUpdatingStation = true;
                    bool bFoundPTT = false;

                    for (int i = 0; i < oclbStation.Items.Count; i++) oclbStation.SetItemChecked(i, false);

                    for (int i = 1; i < oclbStation.Items.Count; i++)
                    {
                        cmlDropdownItem oItem = (cmlDropdownItem)oclbStation.Items[i];
                        if (oItem.tName != null && oItem.tName.ToUpper().Contains("PTT"))
                        {
                            oclbStation.SetItemChecked(i, true);
                            obtnStation.Text = "1 Selected";
                            bFoundPTT = true;
                            break;
                        }
                    }

                    if (!bFoundPTT && oclbStation.Items.Count > 0)
                    {
                        oclbStation.SetItemChecked(0, true);
                        obtnStation.Text = "--- All Stations ---";
                    }
                    bIsUpdatingStation = false;

                    bIsUpdatingFuel = true;
                    for (int i = 0; i < oclbFuelType.Items.Count; i++) oclbFuelType.SetItemChecked(i, i == 0);
                    bIsUpdatingFuel = false;
                    obtnFuelType.Text = "--- All Fuel Types ---";

                    oOnSearchClicked?.Invoke(this, oE);
                };

                ocnExport = new Button
                {
                    Text = "Export",
                    Width = 100,
                    Height = 40,
                    Location = new Point(830, 63),
                    BackColor = Color.FromArgb(3, 169, 244),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Font = new Font("Segoe UI", 9.5f)
                };
                ocnExport.FlatAppearance.BorderSize = 0;
                W_PRCxApplyRoundedCorners(ocnExport, 8);
                ocnExport.Click += (s, e) => oOnExportClicked?.Invoke(this, e);

                this.Controls.AddRange(new Control[] { olaDate, odpStart, olaTo, odpEnd, olaStation, obtnStation, olaFuel, obtnFuelType, ocnSearch, ocnReset, ocnExport });
            }
            catch (Exception oEx)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog 
                { 
                    tFTProcessName = "wFilterPanel.W_PRCxSetupUI",
                    tFTErrorMessage = oEx.Message,
                    tFTStackTrace = oEx.StackTrace 
                });
            }
        }

        private ToolStripDropDown W_PRCoCreateMultiSelectDropdown(Button poButton, out CheckedListBox poClb, string ptType)
        {
            CheckedListBox oClb = new CheckedListBox
            {
                CheckOnClick = true,
                Width = poButton.Width + 80,
                Height = 150,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 9.5f)
            };

            ToolStripControlHost oHost = new ToolStripControlHost(oClb);
            oHost.Margin = Padding.Empty;
            oHost.Padding = Padding.Empty;

            ToolStripDropDown oDrop = new ToolStripDropDown();
            oDrop.Items.Add(oHost);
            oDrop.Padding = Padding.Empty;

            poButton.Click += (s, e) => oDrop.Show(poButton, new Point(0, poButton.Height));

            oClb.ItemCheck += (s, e) =>
            {
                bool bIsUpdating = ptType == "Station" ? bIsUpdatingStation : bIsUpdatingFuel;
                if (bIsUpdating) return;

                BeginInvoke(new Action(() =>
                {
                    if (ptType == "Station") bIsUpdatingStation = true;
                    else bIsUpdatingFuel = true;

                    if (e.Index == 0 && e.NewValue == CheckState.Checked)
                    {
                        for (int i = 1; i < oClb.Items.Count; i++) oClb.SetItemChecked(i, false);
                    }
                    else if (e.Index > 0 && e.NewValue == CheckState.Checked)
                    {
                        oClb.SetItemChecked(0, false);
                    }

                    int nCheckedCount = oClb.CheckedItems.Count;
                    bool bIsAllChecked = oClb.GetItemChecked(0);

                    if (bIsAllChecked || nCheckedCount == 0)
                    {
                        poButton.Text = ptType == "Station" ? "--- All Stations ---" : "--- All Fuel Types ---";
                        if (nCheckedCount == 0) oClb.SetItemChecked(0, true);
                    }
                    else
                    {
                        poButton.Text = $"{nCheckedCount} Selected";
                    }

                    if (ptType == "Station") bIsUpdatingStation = false;
                    else bIsUpdatingFuel = false;
                }));
            };

            poClb = oClb;
            return oDrop;
        }

        private void W_PRCxApplyRoundedCorners(Control poControl, int pnRadius)
        {
            try
            {
                GraphicsPath oPath = new GraphicsPath();
                oPath.AddArc(0, 0, pnRadius, pnRadius, 180, 90);
                oPath.AddArc(poControl.Width - pnRadius, 0, pnRadius, pnRadius, 270, 90);
                oPath.AddArc(poControl.Width - pnRadius, poControl.Height - pnRadius, pnRadius, pnRadius, 0, 90);
                oPath.AddArc(0, poControl.Height - pnRadius, pnRadius, pnRadius, 90, 90);
                poControl.Region = new Region(oPath);
            }
            catch (Exception oEx)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog
                {
                    tFTProcessName = "wFilterPanel.W_PRCxApplyRoundedCorners",
                    tFTErrorMessage = oEx.Message,
                    tFTStackTrace = oEx.StackTrace
                });
            }
        }
    }
}