using System.Text;
using TSDCOilPriceMonitoring_REV02.Class;
using TSDCOilPriceMonitoring_REV02.Class.Repository;
using TSDCOilPriceMonitoring_REV02.Models;

namespace TSDCOilPriceMonitoring_REV02
{
    public partial class wFormGridView : Form
    {
        private cFuelPriceDetailRepository oRepo;
        private cLogService oLog;
        private wFilterPanel oFilterPanel;
        private Panel opnContent;
        private DataGridView ogdData;

        private ComboBox ocbLimit;

        public wFormGridView()
        {
            try
            {
                oRepo = new cFuelPriceDetailRepository();
                oLog = new cLogService();
                W_PRCxSetupUI();
                this.Load += (s, e) => W_PRCxLoadGridData();
            }
            catch (Exception oEx)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog
                {
                    tFTProcessName = "wFormGridView.Constructor",
                    tFTErrorMessage = oEx.Message,
                    tFTStackTrace = oEx.StackTrace
                });
            }
        }

        private void W_PRCxLoadGridData()
        {
            try
            {
                List<cmlFuelPriceDetail> oDetails = oRepo.C_PRCaoGetFuelPriceDetails(
                    oFilterPanel.dStartDate,
                    oFilterPanel.dEndDate.AddDays(1).AddTicks(-1),
                    oFilterPanel.aStationIds,
                    oFilterPanel.aFuelIds
                );

                if (oDetails != null)
                {
                    oDetails.ForEach(oItem =>
                    {
                        if (!string.IsNullOrEmpty(oItem.tStationName))
                        {
                            oItem.tStationName = oItem.tStationName.ToUpper();
                        }
                    });
                }

                int nLimit = 0;
                if (ocbLimit.SelectedItem != null && ocbLimit.SelectedItem.ToString() != "All")
                {
                    int.TryParse(ocbLimit.SelectedItem.ToString(), out nLimit);
                }

                var oDisplayData = (nLimit > 0 && oDetails != null) ? oDetails.Take(nLimit).ToList() : oDetails;

                ogdData.DataSource = (oDisplayData != null && oDisplayData.Count > 0) ? oDisplayData : null;
                if (oDisplayData != null && oDisplayData.Count > 0) ogdData.ClearSelection();
            }
            catch (Exception oEx)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog
                {
                    tFTProcessName = "wFormGridView.W_PRCxLoadGridData",
                    tFTErrorMessage = oEx.Message,
                    tFTStackTrace = oEx.StackTrace
                });
            }
        }

        private void W_PRCxBtnExport_Click(object oSender, EventArgs oE)
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
            catch (Exception oEx)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog
                {
                    tFTProcessName = "wFormGridView.W_PRCxBtnExport_Click",
                    tFTErrorMessage = oEx.Message,
                    tFTStackTrace = oEx.StackTrace
                });
            }
        }

        private void W_PRCxSetupUI()
        {
            try
            {
                this.BackColor = Color.FromArgb(244, 247, 252);
                this.Size = new Size(1000, 600);

                oFilterPanel = new wFilterPanel();
                oFilterPanel.W_PRCxInitialize(true);
                oFilterPanel.oOnSearchClicked += (s, e) => W_PRCxLoadGridData();
                oFilterPanel.oOnExportClicked += W_PRCxBtnExport_Click;

                opnContent = new Panel
                {
                    Dock = DockStyle.Fill,
                    Padding = new Padding(25)
                };

                Panel opnTopHeader = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 45,
                    Padding = new Padding(0, 0, 0, 10)
                };

                Label olaTitle = new Label
                {
                    Text = "Fuel Price Details List",
                    Font = new Font("Segoe UI", 18, FontStyle.Bold),
                    ForeColor = Color.FromArgb(15, 32, 67),
                    AutoSize = true,
                    Dock = DockStyle.Left
                };

                Panel opnLimitControls = new Panel
                {
                    Dock = DockStyle.Right,
                    Width = 300
                };
                Label olaLimit = new Label
                {
                    Text = "Show records:",
                    AutoSize = true,
                    Location = new Point(10, 5), 
                    Font = new Font("Segoe UI", 10.5f),
                    ForeColor = Color.FromArgb(64, 64, 64)
                };

                ocbLimit = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Width = 100,
                    Location = new Point(140, 2),  
                    Font = new Font("Segoe UI", 10.5f)
                };
                ocbLimit.Items.AddRange(new string[] { "50", "100", "500", "All" });
                ocbLimit.SelectedIndex = 1;

                ocbLimit.SelectedIndexChanged += (s, e) => W_PRCxLoadGridData();

                opnLimitControls.Controls.Add(olaLimit);
                opnLimitControls.Controls.Add(ocbLimit);

                opnTopHeader.Controls.Add(olaTitle);
                opnTopHeader.Controls.Add(opnLimitControls);

                Panel opnGridContainer = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.White,
                    Padding = new Padding(2)
                };

                ogdData = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    BackgroundColor = Color.White,
                    ReadOnly = true,
                    RowHeadersVisible = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    AutoGenerateColumns = false,
                    BorderStyle = BorderStyle.None,
                    CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                    EnableHeadersVisualStyles = false,
                    GridColor = Color.FromArgb(235, 235, 235)
                };

                ogdData.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 32, 67);
                ogdData.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                ogdData.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                ogdData.ColumnHeadersHeight = 45;
                ogdData.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

                ogdData.DefaultCellStyle.Font = new Font("Segoe UI", 10.5f);
                ogdData.DefaultCellStyle.Padding = new Padding(5, 0, 5, 0);
                ogdData.DefaultCellStyle.SelectionBackColor = Color.FromArgb(215, 235, 250);
                ogdData.DefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 32, 67);
                ogdData.RowTemplate.Height = 40;

                ogdData.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 252);

                ogdData.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "Effective Date",
                    DataPropertyName = "dEffectiveDate",
                    DefaultCellStyle = { Format = "dd MMM yyyy" }
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
                    HeaderText = "Price (THB)",
                    DataPropertyName = "cPricedPrice",
                    DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N2" }
                });

                opnGridContainer.Controls.Add(ogdData);

                opnContent.Controls.Add(opnGridContainer);
                opnContent.Controls.Add(opnTopHeader);

                this.Controls.Add(opnContent); this.Controls.Add(oFilterPanel);
                oFilterPanel.SendToBack();
            }
            catch (Exception oEx)
            {
                oLog?.C_PRCxWriteErrorLog(new cmlErrorLog
                {
                    tFTProcessName = "wFormGridView.W_PRCxSetupUI",
                    tFTErrorMessage = oEx.Message,
                    tFTStackTrace = oEx.StackTrace
                });
            }
        }
    }
}