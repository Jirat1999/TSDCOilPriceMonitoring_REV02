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
        private Button ocnSearch, ocnExport;

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
                this.Dock = DockStyle.Top; this.Height = 80; this.BackColor = Color.White;

                Panel opnBottomLine = new Panel { Dock = DockStyle.Bottom, Height = 1, BackColor = Color.FromArgb(230, 230, 230) };
                this.Controls.Add(opnBottomLine);

                Label olaDate = new Label { Text = "Date:", AutoSize = true, Location = new Point(20, 30), Font = new Font("Segoe UI", 9.5f), ForeColor = Color.FromArgb(64, 64, 64) };
                odpStart = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 110, Location = new Point(65, 27), Font = new Font("Segoe UI", 9.5f) };
                Label olaTo = new Label { Text = "To:", AutoSize = true, Location = new Point(185, 30), Font = new Font("Segoe UI", 9.5f), ForeColor = Color.FromArgb(64, 64, 64) };
                odpEnd = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 110, Location = new Point(215, 27), Font = new Font("Segoe UI", 9.5f) };
                Label olaStation = new Label { Text = "Station:", AutoSize = true, Location = new Point(340, 30), Font = new Font("Segoe UI", 9.5f), ForeColor = Color.FromArgb(64, 64, 64) };
                ocbStation = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 160, Location = new Point(400, 27), Font = new Font("Segoe UI", 9.5f) };
                Label olaFuel = new Label { Text = "Fuel Type:", AutoSize = true, Location = new Point(580, 30), Font = new Font("Segoe UI", 9.5f), ForeColor = Color.FromArgb(64, 64, 64) };
                ocbFuelType = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 160, Location = new Point(655, 27), Font = new Font("Segoe UI", 9.5f) };

                ocnSearch = new Button { Text = "Search", Width = 100, Height = 36, Location = new Point(830, 22), BackColor = Color.FromArgb(0, 120, 212), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold) };
                ocnSearch.FlatAppearance.BorderSize = 0;
                W_PRCxApplyRoundedCorners(ocnSearch, 15); // ทำปุ่มขอบโค้ง
                ocnSearch.Click += (s, e) => oOnSearchClicked?.Invoke(this, e);

                ocnExport = new Button { Text = "Export", Width = 100, Height = 36, Location = new Point(940, 22), BackColor = Color.FromArgb(16, 124, 65), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold) };
                ocnExport.FlatAppearance.BorderSize = 0;
                W_PRCxApplyRoundedCorners(ocnExport, 15); // ทำปุ่มขอบโค้ง
                ocnExport.Click += (s, e) => oOnExportClicked?.Invoke(this, e);

                this.Controls.AddRange(new Control[] { olaDate, odpStart, olaTo, odpEnd, olaStation, ocbStation, olaFuel, ocbFuelType, ocnSearch, ocnExport });
            }
            catch (Exception oEx)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog { tFTProcessName = "wFilterPanel.W_PRCxSetupUI", tFTErrorMessage = oEx.Message, tFTStackTrace = oEx.StackTrace });
            }
        }

        private void W_PRCxApplyRoundedCorners(Control poControl, int pnRadius)
        {
            GraphicsPath oPath = new GraphicsPath();
            oPath.AddArc(0, 0, pnRadius, pnRadius, 180, 90);
            oPath.AddArc(poControl.Width - pnRadius, 0, pnRadius, pnRadius, 270, 90);
            oPath.AddArc(poControl.Width - pnRadius, poControl.Height - pnRadius, pnRadius, pnRadius, 0, 90);
            oPath.AddArc(0, poControl.Height - pnRadius, pnRadius, pnRadius, 90, 90);
            poControl.Region = new Region(oPath);
        }
    }

    
}
