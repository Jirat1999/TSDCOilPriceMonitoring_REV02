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
    public partial class wFormGridView : Form
    {
        private cFuelRepository oRepo;
        private cLogService oLog;
        private wFilterPanel oFilterPanel;
        private Panel opnContent;
        private DataGridView ogdData;

        public wFormGridView()
        {
            try
            {
                oRepo = new cFuelRepository();
                oLog = new cLogService();
                W_PRCxSetupUI();
                this.Load += (s, e) => W_PRCxLoadGridData();
            }
            catch (Exception ex)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog { tFTProcessName = "wFormGridView.Constructor", tFTErrorMessage = ex.Message, tFTStackTrace = ex.StackTrace });
            }
        }

        private void W_PRCxLoadGridData()
        {
            try
            {
                List<cmlFuelPriceDetail> oDetails = oRepo.GetFuelPriceDetails(oFilterPanel.dStartDate, oFilterPanel.dEndDate.AddDays(1).AddTicks(-1), oFilterPanel.nStationId, oFilterPanel.nFuelId);
                ogdData.DataSource = oDetails.Count > 0 ? oDetails : null;
                if (oDetails.Count > 0) ogdData.ClearSelection();
            }
            catch (Exception ex)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog { tFTProcessName = "wFormGridView.W_PRCxLoadGridData", tFTErrorMessage = ex.Message, tFTStackTrace = ex.StackTrace });
                MessageBox.Show("เกิดข้อผิดพลาดในการดึงข้อมูล", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void W_PRCxBtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (ogdData.Rows.Count == 0) return;

                using (SaveFileDialog oSfd = new SaveFileDialog() { Filter = "CSV File|*.csv", FileName = $"Export_{DateTime.Now:yyyyMMdd_HHmmss}.csv" })
                {
                    if (oSfd.ShowDialog() == DialogResult.OK)
                    {
                        using (StreamWriter oSw = new StreamWriter(oSfd.FileName, false, new UTF8Encoding(true)))
                        {
                            string[] tHeaders = new string[ogdData.Columns.Count];
                            for (int i = 0; i < ogdData.Columns.Count; i++) tHeaders[i] = $"\"{ogdData.Columns[i].HeaderText}\"";
                            oSw.WriteLine(string.Join(",", tHeaders));

                            foreach (DataGridViewRow oRow in ogdData.Rows)
                            {
                                string[] tCells = new string[ogdData.Columns.Count];
                                for (int i = 0; i < ogdData.Columns.Count; i++) tCells[i] = $"\"{oRow.Cells[i].Value?.ToString() ?? ""}\"";
                                oSw.WriteLine(string.Join(",", tCells));
                            }
                        }
                        MessageBox.Show("Export สำเร็จ!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog { tFTProcessName = "wFormGridView.W_PRCxBtnExport_Click", tFTErrorMessage = ex.Message, tFTStackTrace = ex.StackTrace });
                MessageBox.Show("เกิดข้อผิดพลาดในการส่งออกไฟล์", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void W_PRCxSetupUI()
        {
            try
            {
                this.BackColor = Color.FromArgb(240, 242, 255); this.Size = new Size(1000, 600);

                oFilterPanel = new wFilterPanel();
                oFilterPanel.W_PRCxInitialize(true);
                oFilterPanel.oOnSearchClicked += (s, e) => W_PRCxLoadGridData();
                oFilterPanel.oOnExportClicked += W_PRCxBtnExport_Click;

                opnContent = new Panel 
                { 
                    Dock = DockStyle.Fill, 
                    Padding = new Padding(20) 
                };
                Label olaTitle = new Label 
                { 
                    Text = "Fuel Price Details List", 
                    Font = new Font("Segoe UI", 16, FontStyle.Bold), 
                    AutoSize = true, 
                    Dock = DockStyle.Top, 
                    Padding = new Padding(0, 0, 0, 15) 
                };

                ogdData = new DataGridView 
                { 
                    Dock = DockStyle.Fill, 
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, 
                    BackgroundColor = Color.White, 
                    ReadOnly = true, 
                    RowHeadersVisible = false, 
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect, 
                    AutoGenerateColumns = false 
                };
                ogdData.Columns.Add(new DataGridViewTextBoxColumn 
                { 
                    HeaderText = "Effective Date", 
                    DataPropertyName = "dEffectiveDate", 
                    DefaultCellStyle = { Format = "dd/MM/yyyy" } 
                });
                ogdData.Columns.Add(new DataGridViewTextBoxColumn 
                { 
                    HeaderText = "Station Name", 
                    DataPropertyName = "tStationName" 
                });
                ogdData.Columns.Add(new DataGridViewTextBoxColumn 
                { 
                    HeaderText = "Fuel Type", 
                    DataPropertyName = "tFuelName" 
                });
                ogdData.Columns.Add(new DataGridViewTextBoxColumn 
                { 
                    HeaderText = "Price", 
                    DataPropertyName = "cPricedPrice", 
                    DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, 
                    Format = "N2" } 
                });

                opnContent.Controls.Add(ogdData); opnContent.Controls.Add(olaTitle);
                this.Controls.Add(opnContent); this.Controls.Add(oFilterPanel);
                oFilterPanel.SendToBack();
            }
            catch (Exception ex)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog { tFTProcessName = "wFormGridView.W_PRCxSetupUI", tFTErrorMessage = ex.Message, tFTStackTrace = ex.StackTrace });
            }
        }
    }
}
