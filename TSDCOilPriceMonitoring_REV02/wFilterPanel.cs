using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
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
            catch (Exception ex)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog { tFTProcessName = "wFilterPanel.W_PRCxInitialize", tFTErrorMessage = ex.Message, tFTStackTrace = ex.StackTrace });
            }
        }

        private void W_PRCxLoadMasterData()
        {
            try
            {
                List<cmlDropdownItem> oStaions = oRepo.GetStations();
                oStaions.Insert(0, new cmlDropdownItem { nId = 0, tName = "--- All Stations ---" });
                ocbStation.DataSource = oStaions;
                ocbStation.DisplayMember = "tName"; ocbStation.ValueMember = "nId";

                List<cmlDropdownItem> oFuelType = oRepo.GetFuelTypes();
                oFuelType.Insert(0, new cmlDropdownItem { nId = 0, tName = "--- All Fuel Types ---" });
                ocbFuelType.DataSource = oFuelType;
                ocbFuelType.DisplayMember = "tName"; ocbFuelType.ValueMember = "nId";
            }
            catch (Exception ex)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog { tFTProcessName = "wFilterPanel.W_PRCxLoadMasterData", tFTErrorMessage = ex.Message, tFTStackTrace = ex.StackTrace });
                MessageBox.Show("ไม่สามารถโหลดข้อมูลตัวกรองได้", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void W_PRCxSetupUI()
        {
            try
            {
                this.Dock = DockStyle.Top; this.Height = 60; this.BackColor = Color.White;
                Label olaDate = new Label 
                { 
                    Text = "Date:", 
                    AutoSize = true, 
                    Location = new Point(15, 20) 
                };
                odpStart = new DateTimePicker 
                { 
                    Format = DateTimePickerFormat.Short, 
                    Width = 110, 
                    Location = new Point(60, 16) 
                };
                Label olaTo = new Label 
                { 
                    Text = "To:", 
                    AutoSize = true, 
                    Location = new Point(175, 20) 
                };
                odpEnd = new DateTimePicker 
                { 
                    Format = DateTimePickerFormat.Short, 
                    Width = 110, 
                    Location = new Point(195, 16) 
                };
                Label olaStation = new Label 
                { 
                    Text = "Station:", 
                    AutoSize = true, 
                    Location = new Point(320, 20) 
                };
                ocbStation = new ComboBox 
                { 
                    DropDownStyle = ComboBoxStyle.DropDownList, 
                    Width = 150, 
                    Location = new Point(380, 16) 
                };
                Label olaFuel = new Label 
                { 
                    Text = "Fuel Type:",
                    AutoSize = true, 
                    Location = new Point(540, 20) 
                };
                ocbFuelType = new ComboBox 
                { 
                    DropDownStyle = ComboBoxStyle.DropDownList, 
                    Width = 150, 
                    Location = new Point(610, 16) };
                ocnSearch = new Button 
                { 
                    Text = "Search", 
                    Width = 80, 
                    Location = new Point(770, 15),
                    BackColor = Color.FromArgb(41, 128, 185), 
                    ForeColor = Color.White, 
                    FlatStyle = FlatStyle.Flat 
                };
                ocnSearch.Click += (s, e) => oOnSearchClicked?.Invoke(this, e);
                ocnExport = new Button 
                { 
                    Text = "Export",
                    Width = 80, 
                    Location = new Point(860, 15), 
                    BackColor = Color.FromArgb(46, 204, 113), 
                    ForeColor = Color.White, 
                    FlatStyle = FlatStyle.Flat 
                };
                ocnExport.Click += (s, e) => oOnExportClicked?.Invoke(this, e);
                this.Controls.AddRange(new Control[] { olaDate, odpStart, olaTo, odpEnd, olaStation, ocbStation, olaFuel, ocbFuelType, ocnSearch, ocnExport });
            }
            catch (Exception ex)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog { tFTProcessName = "wFilterPanel.W_PRCxSetupUI", tFTErrorMessage = ex.Message, tFTStackTrace = ex.StackTrace });
            }
        }

    }
}
