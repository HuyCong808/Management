using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
//using Managerment.form;
using MySql.Data.MySqlClient;
using NU_Clinic.Button_Circular;
using NU_Clinic.ConnectToMysql;

namespace NU_Clinic.form
{
    public partial class Analysis : UserControl
    {

        public enum Defect_Name
        {
            crack,
            other,
            particle,
            stain,
            wrinkle
        };
        private DbConnection dbConnect;

        private DateTime startDate;
        private DateTime endDate;
      //  private DetailForm detailForm = null;

        MySqlConnection con;
        string _name_model = "";

        private DataGridView dataGridViewDetails;
        public List<KeyValuePair<string, int>> TopDefectList { get; private set; }
        public List<KeyValuePair<int, string>> ListModels { get; private set; }
        public List<KeyValuePair<int, string>> ListProcess { get; private set; }
        public List<string> ListDefectName { get; private set; }
        public List<KeyValuePair<string, PointF>> ListCoodinate { get; private set; }

        public Analysis()
        {
            InitializeComponent();
            dbConnect = new DbConnection();
            con = dbConnect.GetConnection();

            LoadData();

            Load_Model_Process_Name();

        }
      
        private void Combobox_ModelName_SelectIndexChaged(object sender, EventArgs e)
        {
            try
            {
                System.Windows.Forms.ComboBox cbb = (System.Windows.Forms.ComboBox)sender;

                KeyValuePair<int, string> selectedEntry = (KeyValuePair<int, string>)cbb.SelectedItem;
                if (_name_model != selectedEntry.Value.ToString())
                {
                    _name_model = selectedEntry.Value.ToString();

                    GetCoordinateDefectName(_name_model);
                }
            }
            catch (Exception ex) { }
        }


        private void Load_Model_Process_Name()
        {
            try
            {
                ListModels = new List<KeyValuePair<int, string>>();
                ListDefectName = new List<string>();

                string connectionString = "server=127.0.0.1;user=root;password=123456789;database=screen_production;AllowPublicKeyRetrieval=True;SslMode=none;";

                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new MySqlCommand())
                    {
                        MySqlDataReader reader;
                        command.Connection = connection;

                        command.CommandText = @"select * from model";
                        reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            ListModels.Add(
                                new KeyValuePair<int, string>(Convert.ToInt16(reader[0]), reader[1].ToString()));
                        }
                        reader.Close();

                        command.CommandText = @"select i1.defect_type from inspection i1 group by i1.defect_type;";
                        reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            ListDefectName.Add(reader[0].ToString());
                        }
                        reader.Close();
                    }
                }

                //cbb_Model_Name.DataSource = new BindingSource(ListModels, null);
                //cbb_Model_Name.DisplayMember = "Value"; // Show item names
                //cbb_Model_Name.ValueMember = "Key";

                //   cbb_VendorName.DataSource = new BindingSource(ListDefectName, null);
            }
            catch
            {

            }
        }

        private void GetProductAnalisys()
        {
            try
            {
                startDate = dtpStartDate.Value;
                endDate = dtpEndDate.Value;

                TopDefectList = new List<KeyValuePair<string, int>>();

                string connectionString = "server=127.0.0.1;user=root;password=123456789;database=screen_production;AllowPublicKeyRetrieval=True;SslMode=none;";

                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new MySqlCommand())
                    {
                        MySqlDataReader reader;
                        command.Connection = connection;
                        command.CommandText = @"select i1.defect_type, count(*) as total_defect from inspection i1 group by i1.defect_type";
                        reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            TopDefectList.Add(new KeyValuePair<string, int>(reader[0].ToString(), Convert.ToInt16(reader[1])));
                        }
                        reader.Close();
                    }
                }
            }
            catch
            {

            }
        }


        private void LoadData()
        {
            GetProductAnalisys();
            chartTopDefects.DataSource = TopDefectList;
            chartTopDefects.Series[0].XValueMember = "Key";
            chartTopDefects.Series[0].YValueMembers = "Value";
            chartTopDefects.DataBind();
        }

        private void GetCoordinateDefectName(string _modelName = "")
        {
            try
            {
                _modelName = cbb_Model_Name.Text.Trim();
                DateTime startDate = dtpStartDate.Value.Date;
                DateTime endDate = dtpEndDate.Value.Date.AddDays(1).AddTicks(-1); // Bao gồm cả ngày kết thúc
                string ProcessName = "Process M";
                ListCoodinate = new List<KeyValuePair<string, PointF>>();

                string connectionString = "server=127.0.0.1;user=root;password=123456789;database=screen_production;AllowPublicKeyRetrieval=True;SslMode=none;";

                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new MySqlCommand())
                    {
                        command.Connection = connection;

                        // Query lấy dữ liệu lỗi (defect) theo model và thời gian lọc dựa trên process_d.input_time hoặc output_time
                        command.CommandText = @"
                                                SELECT
                                                    i.defect_type,
                                                    i.defect_x,
                                                    i.defect_y
                                                FROM inspection i
                                                JOIN cell c ON i.cell_id = c.cell_id
                                                JOIN model m ON c.model_id = m.model_id
                                                JOIN process_d pd ON i.cell_id = pd.cell_id
                                                WHERE
                                                    m.model_name = @name_model
                                                    AND i.result = 'NG'
                                                    AND (
                                                        (pd.input_time BETWEEN @startDate AND @endDate)
                                                        OR (pd.output_time BETWEEN @startDate AND @endDate)
                                                    );";

                        command.Parameters.Add("@name_model", MySqlDbType.VarChar).Value = _modelName;
                        command.Parameters.Add("@startDate", MySqlDbType.DateTime).Value = startDate;
                        command.Parameters.Add("@endDate", MySqlDbType.DateTime).Value = endDate;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string defectType = reader["defect_type"].ToString();
                                float x = Convert.ToSingle(reader["defect_x"]);
                                float y = Convert.ToSingle(reader["defect_y"]);

                                ListCoodinate.Add(new KeyValuePair<string, PointF>(defectType, new PointF(x, y)));
                            }
                        }
                    }
                }

                // Xóa và refresh lại plot
                _scatterPlot.Plot.Clear();

                _scatterPlot.Refresh();

                _scatterPlot.Plot.Legend.Alignment = ScottPlot.Alignment.LowerLeft;

                if (ListCoodinate != null && ListCoodinate.Count > 0)
                {
                    foreach (var kvp in ListCoodinate)
                    {
                        string defectType = kvp.Key;
                        PointF point = kvp.Value;
                        Color color = Color.OrangeRed; // Màu mặc định

                        // Chuyển defectType sang enum và chọn màu tương ứng
                        if (Enum.TryParse(defectType, out Defect_Name defectName))
                        {
                            switch (defectName)
                            {
                                case Defect_Name.particle:
                                    color = Color.Red;
                                    break;
                                case Defect_Name.other:
                                    color = Color.Blue;
                                    break;
                                case Defect_Name.crack:
                                    color = Color.Yellow;
                                    break;
                                case Defect_Name.stain:
                                    color = Color.OrangeRed;
                                    break;
                                case Defect_Name.wrinkle:
                                    color = Color.OliveDrab;
                                    break;
                            }
                        }

                        _scatterPlot.Plot.Add.Scatter(point.X, point.Y, new ScottPlot.Color(color.R, color.G, color.B));
                    }

                    _scatterPlot.Refresh();
                }

                // Đếm số lần xuất hiện mỗi defect_type
                var keyCount = ListCoodinate
                    .GroupBy(pair => pair.Key)
                    .Select(group => new { Key = group.Key, Count = group.Count() })
                    .OrderByDescending(group => group.Count);

                int idx = 1;

                if (keyCount.Any())
                {
                    foreach (var item in keyCount)
                    {
                        string defectType = item.Key;
                        Color color = Color.OrangeRed;

                        if (Enum.TryParse(defectType, out Defect_Name defectName))
                        {
                            switch (defectName)
                            {
                                case Defect_Name.particle:
                                    color = Color.Red;
                                    break;
                                case Defect_Name.other:
                                    color = Color.Blue;
                                    break;
                                case Defect_Name.crack:
                                    color = Color.Yellow;
                                    break;
                                case Defect_Name.stain:
                                    color = Color.OrangeRed;
                                    break;
                                case Defect_Name.wrinkle:
                                    color = Color.OliveDrab;
                                    break;
                            }
                        }

                        ButtonElippse btn = Layout_Panel_CountDeffect.Controls[$"btn_Circle{idx}"] as ButtonElippse;
                        Label lb_Defect = Layout_Panel_CountDeffect.Controls[$"lbNameDefect{idx}"] as Label;
                        Label lb_Count = Layout_Panel_CountDeffect.Controls[$"lbCount{idx}"] as Label;

                        if (btn != null)
                        {
                            btn.CircleColor = color;
                            btn.Invalidate();
                        }

                        if (lb_Defect != null)
                        {
                            lb_Defect.ForeColor = color;
                            lb_Defect.Text = defectType;
                        }

                        if (lb_Count != null)
                        {
                            lb_Count.ForeColor = color;
                            lb_Count.Text = item.Count.ToString();
                        }

                        idx++;
                    }
                }
                else
                {
                    idx = 1;
                    foreach (var defectType in ListDefectName)
                    {
                        Color color = Color.OrangeRed;

                        if (Enum.TryParse(defectType, out Defect_Name defectName))
                        {
                            switch (defectName)
                            {
                                case Defect_Name.particle:
                                    color = Color.Red;
                                    break;
                                case Defect_Name.other:
                                    color = Color.Blue;
                                    break;
                                case Defect_Name.crack:
                                    color = Color.Yellow;
                                    break;
                                case Defect_Name.stain:
                                    color = Color.OrangeRed;
                                    break;
                                case Defect_Name.wrinkle:
                                    color = Color.OliveDrab;
                                    break;
                            }
                        }

                        ButtonElippse btn = Layout_Panel_CountDeffect.Controls[$"btn_Circle{idx}"] as ButtonElippse;
                        Label lb_Defect = Layout_Panel_CountDeffect.Controls[$"lbNameDefect{idx}"] as Label;
                        Label lb_Count = Layout_Panel_CountDeffect.Controls[$"lbCount{idx}"] as Label;

                        if (btn != null)
                        {
                            btn.CircleColor = color;
                            btn.Invalidate();
                        }

                        if (lb_Defect != null)
                        {
                            lb_Defect.ForeColor = color;
                            lb_Defect.Text = defectType;
                        }

                        if (lb_Count != null)
                        {
                            lb_Count.ForeColor = color;
                            lb_Count.Text = "0";
                        }

                        idx++;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }





        private void btn_Circle1_Click(object sender, EventArgs e)
        {
            try
            {
                ButtonElippse btn = sender as ButtonElippse;

                if (btn != null && btn.Name != btnShowAll.Name)
                {

                    string number = new string(btn.Name.Where(char.IsDigit).ToArray());
                    Label lb_Defect = Layout_Panel_CountDeffect.Controls[$"lbNameDefect{number}"] as Label;

                    _scatterPlot.Plot.Clear();
                    _scatterPlot.Refresh();
                    _scatterPlot.Plot.Legend.Alignment = ScottPlot.Alignment.LowerLeft;

                    string _defect_type = "";
                    Color color = Color.OrangeRed;
                    Defect_Name _defect_name;

                    if (ListCoodinate != null && ListCoodinate.Count > 0)
                    {
                        PointF _p;

                        foreach (var kvp in ListCoodinate)
                        {
                            _defect_type = kvp.Key;

                            if (_defect_type == lb_Defect.Text)
                            {
                                Enum.TryParse(_defect_type, out _defect_name);

                                switch (_defect_name)
                                {
                                    case Defect_Name.particle:
                                        color = Color.Red;
                                        break;
                                    case Defect_Name.other:
                                        color = Color.Blue;
                                        break;
                                    case Defect_Name.crack:
                                        color = Color.Yellow;
                                        break;
                                    case Defect_Name.wrinkle:
                                        color = Color.OliveDrab;
                                        break;
                                }
                                _p = kvp.Value;
                                var scatter = _scatterPlot.Plot.Add.Scatter(_p.X, _p.Y, new ScottPlot.Color(color.R, color.G, color.B));
                            }
                            else continue;
                            //scatter.LegendText = _defect_type;
                        }
                        _scatterPlot.Refresh();
                    }
                }

                if (btn.Name == btnShowAll.Name) GetCoordinateDefectName(_name_model);
            }
            catch { }
        }
        #region TINH TOAN VA VE CAC BIEU DO
        private void DrawPieChart()
        {
            var defectCounts = ListCoodinate
                .GroupBy(kvp => kvp.Key)
                .Select(g => new { DefectType = g.Key, Count = g.Count() })
                .ToList();

            int total = defectCounts.Sum(x => x.Count);

            chartTopDefects.Series.Clear();
            chartTopDefects.ChartAreas.Clear();
            chartTopDefects.Titles.Clear();

            ChartArea area = new ChartArea("PieArea");
            chartTopDefects.ChartAreas.Add(area);

            Title title = new Title("Chart Top Defect", Docking.Top, new Font("Arial", 12, FontStyle.Bold), Color.Black);
            chartTopDefects.Titles.Add(title);

            Series series = new Series
            {
                Name = "DefectCounts",
                ChartType = SeriesChartType.Pie,
                ChartArea = "PieArea"
            };

            foreach (var item in defectCounts)
            {
                double percent = total > 0 ? (item.Count * 100.0 / total) : 0;
                var point = series.Points.Add(item.Count);
                point.Label = $"{percent:F1}%";
                point.LegendText = item.DefectType;
            }

            series["PieLabelStyle"] = "Inside";
            series["PieDrawingStyle"] = "Default";
            series.Font = new Font("Arial", 9);
            chartTopDefects.Series.Add(series);
        }
        private void LoadChartByLineSafe(string modelName)
        {
            Dictionary<int, int> lineCounts = new Dictionary<int, int>();

            string connectionString = "server=127.0.0.1;user=root;password=123456789;database=screen_production;AllowPublicKeyRetrieval=True;SslMode=none;";


            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = @"
                                    SELECT pm.line_no, COUNT(*) AS defect_count
                                    FROM inspection i
                                    INNER JOIN cell c ON i.cell_id = c.cell_id
                                    INNER JOIN process_M pm ON c.process_m_id = pm.process_m_id
                                    INNER JOIN model m ON pm.model_id = m.model_id
                                    WHERE m.model_name = @name_model
                                    GROUP BY pm.line_no
                                    ORDER BY pm.line_no
            ";

                    command.Parameters.Add("@name_model", MySqlDbType.VarChar).Value = modelName;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int line = reader.GetInt32("line_no");
                            int count = reader.GetInt32("defect_count");

                            lineCounts[line] = count;
                        }
                    }
                }
            }

            ChartByLine.Series.Clear();
            ChartByLine.Titles.Clear();
            ChartByLine.Titles.Add("Chart Top Defect by Line");

            var series = new Series("Defect Count");
            series.ChartType = SeriesChartType.Line;
            series.Color = Color.Blue;
            series.MarkerStyle = MarkerStyle.Circle;
            series.MarkerSize = 8;
            series.Font = new Font("Arial", 9, FontStyle.Bold);

            foreach (var item in lineCounts)
            {
                int line = item.Key;
                int count = item.Value;

                series.Points.AddXY($"Line {line}", count);
            }

            ChartByLine.Series.Add(series);
        }


        private void GenerateDefectChart(string modelName, DateTime startDate, DateTime endDate)
        {
            string connectionString = "server=127.0.0.1;user=root;password=123456789;database=screen_production;AllowPublicKeyRetrieval=True;SslMode=none;";
            var defectData = new Dictionary<string, Dictionary<string, int>>();

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
            SELECT 
                DATE(pm.input_time) AS defect_date,
                i.defect_type,
                COUNT(*) AS defect_count
            FROM inspection i
            JOIN cell c ON i.cell_id = c.cell_id
            JOIN model m ON c.model_id = m.model_id
            JOIN process_m pm ON i.cell_id = pm.cell_id
            WHERE m.model_name = @name_model
              AND i.result = 'NG'
              AND (
                  pm.input_time BETWEEN @startDate AND @endDate
                  OR pm.output_time BETWEEN @startDate AND @endDate
              )
            GROUP BY defect_date, i.defect_type
            ORDER BY defect_date;
        ";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@name_model", modelName);
                    cmd.Parameters.AddWithValue("@startDate", startDate.Date);
                    cmd.Parameters.AddWithValue("@endDate", endDate.Date.AddDays(1).AddTicks(-1));

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DateTime date = reader.GetDateTime("defect_date");
                            string defectType = reader.GetString("defect_type");
                            int count = reader.GetInt32("defect_count");

                            string dateKey = date.ToString("MM-dd");

                            if (!defectData.ContainsKey(defectType))
                                defectData[defectType] = new Dictionary<string, int>();

                            if (!defectData[defectType].ContainsKey(dateKey))
                                defectData[defectType][dateKey] = 0;

                            defectData[defectType][dateKey] += count;
                        }
                    }
                }
            }

            chartGrossRevenue.Series.Clear();
            chartGrossRevenue.Titles.Clear();
            chartGrossRevenue.Titles.Add("Defects by Day");
            chartGrossRevenue.ChartAreas[0].AxisX.Interval = 1;
            chartGrossRevenue.ChartAreas[0].AxisY.Title = "Defect Count";

            List<string> allDates = new List<string>();
            for (DateTime dt = startDate.Date; dt <= endDate.Date; dt = dt.AddDays(1))
            {
                allDates.Add(dt.ToString("MM-dd"));
            }

            foreach (var defect in defectData)
            {
                Series series = new Series(defect.Key)
                {
                    ChartType = SeriesChartType.StackedColumn,
                    IsValueShownAsLabel = false
                };

                foreach (string day in allDates)
                {
                    int value = defect.Value.ContainsKey(day) ? defect.Value[day] : 0;
                    int pointIndex = series.Points.AddXY(day, value);

                    if (value > 0)
                    {
                        DataPoint point = series.Points[pointIndex];
                        point.Label = value.ToString();
                        point.LabelForeColor = Color.White;
                        point.Font = new Font("Arial", 8, FontStyle.Bold); // Nhỏ hơn và in đậm
                    }
                }

                chartGrossRevenue.Series.Add(series);
            }
        }
        private void GenerateTop5LinesByProduction(string modelName, DateTime startDate, DateTime endDate)
        {
            string connectionString = "server=127.0.0.1;user=root;password=123456789;database=screen_production;AllowPublicKeyRetrieval=True;SslMode=none;";

            var lineNames = new List<string>();
            var totalCells = new List<int>();
            var ngRates = new List<double>();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
            SELECT
              pd.line_no,
              COUNT(pd.cell_id) AS total_cells,
              SUM(CASE WHEN i.result = 'NG' THEN 1 ELSE 0 END) AS total_ng_cells,
              ROUND(
                SUM(CASE WHEN i.result = 'NG' THEN 1 ELSE 0 END) * 100.0
                / COUNT(pd.cell_id),
                2
              ) AS ng_rate_percent
            FROM process_d pd
            JOIN cell c ON pd.cell_id = c.cell_id
            JOIN model m ON c.model_id = m.model_id
            LEFT JOIN inspection i ON pd.cell_id = i.cell_id
            WHERE m.model_name = @modelName
              AND (
                  pd.input_time BETWEEN @start AND @end
                  OR pd.output_time BETWEEN @start AND @end
              )
            GROUP BY pd.line_no
            ORDER BY total_ng_cells DESC
            LIMIT 5;";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@modelName", modelName);
                    cmd.Parameters.AddWithValue("@start", startDate.Date);
                    cmd.Parameters.AddWithValue("@end", endDate.Date.AddDays(1).AddTicks(-1));

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string line = "Line " + reader["line_no"].ToString();
                            int total = Convert.ToInt32(reader["total_cells"]);
                            double rate = Convert.ToDouble(reader["ng_rate_percent"]);

                            lineNames.Add(line);
                            totalCells.Add(total);
                            ngRates.Add(rate);
                        }
                    }
                }
            }

            // Clear chart
            ChartByLine.Series.Clear();
            ChartByLine.ChartAreas.Clear();

            // Create two chart areas
            var areaLine = new ChartArea("LineArea");
            var areaColumn = new ChartArea("ColumnArea");

            // Positioning: Line on top, Column below
            areaLine.Position = new ElementPosition(5, 5, 90, 45);    // chiều cao 45%
            areaLine.InnerPlotPosition = new ElementPosition(10, 10, 80, 80);

            areaColumn.Position = new ElementPosition(5, 52, 90, 43); // kéo lên gần hơn (từ 55 xuống 52)
            areaColumn.InnerPlotPosition = new ElementPosition(10, 10, 80, 80);

            // Line chart axis Y: ẩn hết
            areaLine.AxisY.Enabled = AxisEnabled.False;
            areaLine.AxisY.MajorGrid.Enabled = false;
            areaLine.AxisY.LabelStyle.Enabled = false;
            areaLine.AxisY.LineWidth = 0;
            areaLine.AxisY.MajorTickMark.Enabled = false;
            areaLine.AxisY.MinorTickMark.Enabled = false;
            areaLine.AxisY.Title = "";

            // Line chart axis X: ẩn label, ẩn các vạch kẻ
            areaLine.AxisX.LabelStyle.Enabled = false;
            areaLine.AxisX.MajorGrid.Enabled = false;
            areaLine.AxisX.MajorTickMark.Enabled = false;
            areaLine.AxisX.MinorTickMark.Enabled = false;
            areaLine.AxisX.LineWidth = 0;

            // Column chart axis X setup
            areaColumn.AxisX.Interval = 1;
            areaColumn.AxisX.MajorGrid.Enabled = false;
            areaColumn.AxisX.MajorTickMark.Enabled = true;
            areaColumn.AxisX.MinorTickMark.Enabled = false;

            // Column chart axis Y setup
            areaColumn.AxisY.Title = "Production Qty";
            areaColumn.AxisY.MajorGrid.Enabled = false;

            // Đồng bộ trục X
            areaLine.AlignWithChartArea = "ColumnArea";

            // Thêm chart areas vào Chart
            ChartByLine.ChartAreas.Add(areaLine);
            ChartByLine.ChartAreas.Add(areaColumn);

            // Column series
            Series productionSeries = new Series("Production")
            {
                ChartType = SeriesChartType.Column,
                ChartArea = "ColumnArea",
                IsValueShownAsLabel = true,
                Font = new Font("Arial", 8),
                LabelForeColor = Color.White,
                Color = Color.DodgerBlue
            };

            // Line series
            Series ngRateSeries = new Series("NG Rate (%)")
            {
                ChartType = SeriesChartType.Line,
                ChartArea = "LineArea",
                Color = Color.Red,
                BorderWidth = 2,
                IsValueShownAsLabel = true,
                Font = new Font("Arial", 8),
                LabelForeColor = Color.Red,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 6
            };

            // Add data
            for (int i = 0; i < lineNames.Count; i++)
            {
                string line = lineNames[i];
                int total = totalCells[i];
                double rate = ngRates[i];

                // Column data
                int colIndex = productionSeries.Points.AddXY(line, total);
                productionSeries.Points[colIndex].Label = total.ToString();

                // Line data - giá trị là % nên dùng trực tiếp
                int lineIndex = ngRateSeries.Points.AddXY(line, rate);
                ngRateSeries.Points[lineIndex].Label = $"{Math.Round(rate, 1)}%";
            }

            ChartByLine.Series.Add(productionSeries);
            ChartByLine.Series.Add(ngRateSeries);
            // Tăng chiều cao Line, kéo Column lên sát phía dưới
            areaLine.Position = new ElementPosition(5, 5, 90, 25);   // chiều cao 25%
            areaLine.InnerPlotPosition = new ElementPosition(10, 10, 80, 80);

            areaColumn.Position = new ElementPosition(5, 30, 90, 65); // bắt đầu từ 30%, cao 65% để tổng là 90%
            areaColumn.InnerPlotPosition = new ElementPosition(10, 10, 80, 80);



        }

        #endregion
        private void cbb_Model_Name_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetCoordinateDefectName(_name_model);
            DrawPieChart();
            GenerateDefectChart(cbb_Model_Name.Text, dtpStartDate.Value, dtpEndDate.Value);
            GenerateTop5LinesByProduction(cbb_Model_Name.Text, dtpStartDate.Value, dtpEndDate.Value);
        }

        #region CLICK VAO CHART GROSS
        private void chartGrossRevenue_MouseClick(object sender, MouseEventArgs e)
        {
            // Xử lý khi click vào chart
            HitTestResult result = chartGrossRevenue.HitTest(e.X, e.Y);
            if (result.ChartElementType == ChartElementType.DataPoint)
            {
                int pointIndex = result.PointIndex;
                Series series = result.Series;

                string defectType = series.Name;
                string dateLabel = series.Points[pointIndex].AxisLabel;

                if (!DateTime.TryParseExact(dateLabel, "MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime defectDate))
                    return;
                defectDate = new DateTime(DateTime.Now.Year, defectDate.Month, defectDate.Day);

                ShowDetailForm(defectType, defectDate);
            }
            else
            {
                //detailForm?.Close();
                //detailForm = null;
            }
        }

        private void ShowDetailForm(string defectType, DateTime defectDate)
        {
            //detailForm?.Close();
            //detailForm = null;

            //DataTable data = GetDetailData(defectType, defectDate); // Bạn tự implement truy vấn DB trả về DataTable

            //detailForm = new DetailForm(data);
            //detailForm.Show();
        }
        #endregion
        private DataTable GetDetailData(string defectType, DateTime defectDate)
        {
            string query = @"
                        SELECT 
                    i.cell_id,
                    i.defect_type,
                    i.defect_x,
                    i.defect_y,
                    i.size_mm,
                    pm.*,       -- Lấy tất cả cột trong bảng process_m
                    pd.*        -- Lấy tất cả cột trong bảng process_d
                FROM inspection i
                JOIN cell c ON i.cell_id = c.cell_id
                JOIN model m ON c.model_id = m.model_id
                JOIN process_m pm ON i.cell_id = pm.cell_id
                LEFT JOIN process_d pd ON i.cell_id = pd.cell_id
                WHERE i.result = 'NG'
                  AND i.defect_type = @defectType
                  AND DATE(pm.input_time) = @defectDate
                  AND m.model_name = @modelName
    ";

            string connectionString = "server=127.0.0.1;user=root;password=123456789;database=screen_production;AllowPublicKeyRetrieval=True;SslMode=none;";
            DataTable dt = new DataTable();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@defectType", defectType);
                    cmd.Parameters.AddWithValue("@defectDate", defectDate.Date);
                    cmd.Parameters.AddWithValue("@modelName", cbb_Model_Name.Text); // Bạn cần lưu biến này bên ngoài

                    using (var adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }

            return dt;
        }

        #region CLICK BANG CHART BY LINE
        private void ChartByLine_MouseClick(object sender, MouseEventArgs e)
        {
            var result = ChartByLine.HitTest(e.X, e.Y);
            if (result.ChartElementType == ChartElementType.DataPoint)
            {
                int pointIndex = result.PointIndex;
                Series series = result.Series;

                string lineLabel = series.Points[pointIndex].AxisLabel; // "Line 1", "Line 2", ...
                                                                        // Lấy số line từ chuỗi
                int lineNo = int.Parse(lineLabel.Replace("Line ", ""));

                ShowDetailFormByLine(lineNo);
            }
            else
            {
                //detailForm?.Close();
                //detailForm = null;
            }
        }
        private void ShowDetailFormByLine(int lineNo)
        {
            //detailForm?.Close();
            //detailForm = null;

            //DataTable dt = GetDetailDataByLine(lineNo);

            //detailForm = new DetailForm(dt);
            //detailForm.Show();
        }

        private DataTable GetDetailDataByLine(int lineNo)
        {
            string query = @"SELECT 
                        i.cell_id,
                        i.defect_type,
                        i.defect_x,
                        i.defect_y,
                        i.size_mm,
                         pm.*,       -- Lấy tất cả cột trong bảng process_m
                    pd.*        -- Lấy tất cả cột trong bảng process_d
                FROM inspection i
                JOIN cell c ON i.cell_id = c.cell_id
                JOIN model m ON c.model_id = m.model_id
                JOIN process_m pm ON i.cell_id = pm.cell_id
                LEFT JOIN process_d pd ON i.cell_id = pd.cell_id
                    WHERE pm.line_no = @lineNo
                      AND m.model_name = @modelName
                      AND i.result = 'NG'";

            string connectionString = "server=127.0.0.1;user=root;password=123456789;database=screen_production;AllowPublicKeyRetrieval=True;SslMode=none;";

            DataTable dt = new DataTable();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@lineNo", lineNo);
                    cmd.Parameters.AddWithValue("@modelName", cbb_Model_Name.Text); // Hoặc biến modelName bạn truyền

                    using (var adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }

            return dt;
        }

        #endregion

    }

}