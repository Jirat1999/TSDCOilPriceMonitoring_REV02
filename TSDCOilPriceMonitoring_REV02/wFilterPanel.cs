using System.Drawing.Drawing2D;
using TSDCOilPriceMonitoring_REV02.Class;
using TSDCOilPriceMonitoring_REV02.Models;

namespace TSDCOilPriceMonitoring_REV02
{
    public partial class wFilterPanel : UserControl
    {
        private cFuelRepository oRepo;
        private cLogService oLog;
        private DateTimePicker odpStart, odpEnd;
        private ComboBox ocbStation, ocbFuelType;
        private Button ocnSearch, ocnReset, ocnExport;

        public event EventHandler oOnSearchClicked;
        public event EventHandler oOnExportClicked;

        public DateTime dStartDate => odpStart.Value.Date;
        public DateTime dEndDate => odpEnd.Value.Date;
        public int nStationId => ocbStation.SelectedIndex > 0 ? (int)ocbStation.SelectedValue : 0;
        public int nFuelId => ocbFuelType.SelectedIndex > 0 ? (int)ocbFuelType.SelectedValue : 0;

        public void W_PRCxInitialize(bool pbShowExport = true)
        {
            try
            {
                oRepo = new cFuelRepository();
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
                List<cmlDropdownItem> oStaions = oRepo.C_PRCaoGetStations();
                oStaions.Insert(0, new cmlDropdownItem { nId = 0, tName = "--- All Stations ---" });
                ocbStation.DataSource = oStaions;
                ocbStation.DisplayMember = "tName"; ocbStation.ValueMember = "nId";

                List<cmlDropdownItem> oFuelType = oRepo.C_PRCaoGetFuelTypes();
                oFuelType.Insert(0, new cmlDropdownItem { nId = 0, tName = "--- All Fuel Types ---" });
                ocbFuelType.DataSource = oFuelType;
                ocbFuelType.DisplayMember = "tName"; ocbFuelType.ValueMember = "nId";
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
                this.Dock = DockStyle.Top; this.Height = 70; this.BackColor = Color.White;

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
                    Location = new Point(10, 25), 
                    ForeColor = Color.FromArgb(64, 64, 64) 
                };
                odpStart = new DateTimePicker 
                { 
                    Format = DateTimePickerFormat.Short, 
                    Width = 100, 
                    Location = new Point(50, 21), 
                    Font = new Font("Segoe UI", 9.5f) 
                };

                Label olaTo = new Label 
                { 
                    Text = "To:", 
                    AutoSize = true, 
                    Location = new Point(155, 25), 
                    ForeColor = Color.FromArgb(64, 64, 64) 
                };
                odpEnd = new DateTimePicker 
                { 
                    Format = DateTimePickerFormat.Short,
                    Width = 100, 
                    Location = new Point(180, 21), 
                    Font = new Font("Segoe UI", 9.5f) 
                };

                Label olaStation = new Label 
                { 
                    Text = "Station:", 
                    AutoSize = true, 
                    Location = new Point(285, 25), 
                    ForeColor = Color.FromArgb(64, 64, 64) 
                };
                ocbStation = new ComboBox 
                { 
                    DropDownStyle = ComboBoxStyle.DropDownList, 
                    Width = 130, 
                    Location = new Point(340, 21),
                    Font = new Font("Segoe UI", 9.5f) 
                };

                Label olaFuel = new Label 
                { 
                    Text = "Fuel:", 
                    AutoSize = true, 
                    Location = new Point(475, 25), 
                    ForeColor = Color.FromArgb(64, 64, 64) 
                };
                ocbFuelType = new ComboBox 
                { 
                    DropDownStyle = ComboBoxStyle.DropDownList, 
                    Width = 130, 
                    Location = new Point(515, 21), 
                    Font = new Font("Segoe UI", 9.5f) 
                };

                ocnSearch = new Button 
                { 
                    Text = "Search", 
                    Width = 90, 
                    Height = 36, 
                    Location = new Point(655, 18), 
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
                    Width = 90, 
                    Height = 36,
                    Location = new Point(750, 18), 
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
                    if (ocbStation.Items.Count > 0) ocbStation.SelectedIndex = 0;
                    if (ocbFuelType.Items.Count > 0) ocbFuelType.SelectedIndex = 0;
                    oOnSearchClicked?.Invoke(this, oE);
                };

                ocnExport = new Button 
                { 
                    Text = "Export", 
                    Width = 90, 
                    Height = 36,
                    Location = new Point(845, 18), 
                    BackColor = Color.FromArgb(3, 169, 244), 
                    ForeColor = Color.White, 
                    FlatStyle = FlatStyle.Flat, 
                    Cursor = Cursors.Hand, 
                    Font = new Font("Segoe UI", 9.5f) 
                };
                ocnExport.FlatAppearance.BorderSize = 0;
                W_PRCxApplyRoundedCorners(ocnExport, 8);
                ocnExport.Click += (s, e) => oOnExportClicked?.Invoke(this, e);

                this.Controls.AddRange(new Control[] { olaDate, odpStart, olaTo, odpEnd, olaStation, ocbStation, olaFuel, ocbFuelType, ocnSearch, ocnReset, ocnExport });
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
                    tFTProcessName = "wFilterPanel.W_PRCxSetupUI",
                    tFTErrorMessage = oEx.Message,
                    tFTStackTrace = oEx.StackTrace
                });
            }
        }
    }
}