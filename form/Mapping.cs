using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Managerment.form
{
    public partial class Mapping : UserControl
    {
        public Mapping()
        {
            InitializeComponent();
        }

        private void button6_Click(object sender, EventArgs e)
        {



        }

        private void LoadData()
        {
            DateTime start = dtpStartDate.Value.Date;
            DateTime end = dtpEndDate.Value.Date.AddDays(1).AddTicks(-1); // đến hết ngày

            string connectionString = "server=127.0.0.1;user=root;password=123456789;database=screen_factory;AllowPublicKeyRetrieval=True;SslMode=none;";
            string selectedModel = cbx_model.Text;

            string query = @"
        SELECT 
            ROW_NUMBER() OVER () AS STT,
            c.cell_id AS 'Cell ID',
            m.model_name AS 'Model Name',
            d.line_no AS 'D-line no',
            m2.line_no AS 'M-line no',
            TIME(d.output_time) AS 'D-time',
            TIME(m2.output_time) AS 'M-time',
            i.defect_x AS 'tọa độ X',
            i.defect_y AS 'tọa độ Y',
            i.size_mm AS 'kích thước dài',
            i.size_mm AS 'kích thước rộng',
            gl.lot_no AS 'Lot NVLlayer',
            oa.lot_no AS 'Layer2'
        FROM 
            cell c
        JOIN model m ON c.model_id = m.model_id
        JOIN process_M m2 ON c.process_m_id = m2.process_m_id
        JOIN process_D d ON m2.input_d_output_lot_no = d.output_lot_no
        LEFT JOIN inspection i ON c.cell_id = i.cell_id
        LEFT JOIN material_lot gl ON d.input_gl_lot_id = gl.lot_id
        LEFT JOIN material_lot oa ON d.input_oa_lot_id = oa.lot_id
        WHERE m.model_name = @modelName
          AND d.output_time BETWEEN @startDate AND @endDate
        ORDER BY d.output_time DESC
        LIMIT 3000;";

            List<DefectInfo> defectList = new List<DefectInfo>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@modelName", selectedModel);
                cmd.Parameters.AddWithValue("@startDate", start);
                cmd.Parameters.AddWithValue("@endDate", end);

                conn.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    int stt = 1;
                    while (reader.Read())
                    {
                        defectList.Add(new DefectInfo
                        {
                            STT = stt++,
                            CellID = reader["Cell ID"].ToString(),
                            DLineNo = Convert.ToInt32(reader["D-line no"]),
                            MLineNo = Convert.ToInt32(reader["M-line no"]),
                            DTime = reader["D-time"].ToString(),
                            MTime = reader["M-time"].ToString(),
                            DefectX = reader["tọa độ X"] != DBNull.Value ? Convert.ToDecimal(reader["tọa độ X"]) : 0,
                            DefectY = reader["tọa độ Y"] != DBNull.Value ? Convert.ToDecimal(reader["tọa độ Y"]) : 0,
                            Length = reader["kích thước dài"] != DBNull.Value ? Convert.ToDecimal(reader["kích thước dài"]) : 0,
                            Width = reader["kích thước rộng"] != DBNull.Value ? Convert.ToDecimal(reader["kích thước rộng"]) : 0,
                            LotNVLLayer = reader["Lot NVLlayer"]?.ToString(),
                            Layer2 = reader["Layer2"]?.ToString()
                        });
                    }
                }
            }

            ShowDefectMap(defectList); // Hàm vẽ lên PictureBox
        }
        
        private void button5_Click(object sender, EventArgs e)
        {
            LoadData();
        }
        private void ShowDefectMap(List<DefectInfo> defectList)
        {
            chart1.Visible = true;
            // Khởi tạo series dạng Scatter
            chart1.Series.Clear();
            // Đổi nền ChartArea (vùng vẽ)
            chart1.ChartAreas[0].BackColor = Color.FromArgb(50, 50, 51);

            // Nếu muốn đổi cả nền ngoài vùng vẽ (toàn bộ Chart)
            chart1.BackColor = Color.FromArgb(50, 50, 51);

            var series = new Series("Defect Points")
            {
                ChartType = SeriesChartType.Point,
                Color = Color.OrangeRed,
                MarkerSize = 8,
                MarkerStyle = MarkerStyle.Circle,
            };
            chart1.Series.Add(series);
            chart1.ChartAreas[0].AxisX.MinorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisY.MinorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

            // Màu chữ cho trục X và Y
            chart1.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.White;
            chart1.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.White;

            // Màu dòng tiêu đề (tiêu đề trục nếu có)
            chart1.ChartAreas[0].AxisX.TitleForeColor = Color.White;
            chart1.ChartAreas[0].AxisY.TitleForeColor = Color.White;

            // Màu trục (đường trục)
            chart1.ChartAreas[0].AxisX.LineColor = Color.White;
            chart1.ChartAreas[0].AxisY.LineColor = Color.White;

            // Màu các tick marks
            chart1.ChartAreas[0].AxisX.MajorTickMark.LineColor = Color.White;
            chart1.ChartAreas[0].AxisY.MajorTickMark.LineColor = Color.White;


            // Cấu hình trục X, Y (ví dụ)
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.Maximum = 120; // maxX thực tế
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            chart1.ChartAreas[0].AxisY.Maximum = 120; // maxY thực tế

            // Thêm điểm vào series từ list defect
            foreach (var defect in defectList)
            {
                if (defect.DefectX != 0 && defect.DefectY != 0)
                {
                    series.Points.AddXY((double)defect.DefectX, (double)defect.DefectY);
                }

            }
        }
    }
}
public class DefectInfo
{
    public int STT { get; set; }
    public string CellID { get; set; }
    public int DLineNo { get; set; }
    public int MLineNo { get; set; }
    public string DTime { get; set; }
    public string MTime { get; set; }
    public decimal DefectX { get; set; }
    public decimal DefectY { get; set; }
    public decimal Length { get; set; }
    public decimal Width { get; set; }
    public string LotNVLLayer { get; set; }
    public string Layer2 { get; set; }
}



