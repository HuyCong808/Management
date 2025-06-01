using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using NU_Clinic.Button_Circular;
using NU_Clinic.ConnectToMysql;

namespace NU_Clinic.form
{
    public partial class Analysis : UserControl
    {

        public enum Defect_Name
        {
            Crack,
            Other,
            Particle,
            Contamination,
            Wrinkle
        };
        private DbConnection dbConnect;

        private DateTime startDate;
        private DateTime endDate;

        MySqlConnection con;
        string _name_model = "";


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
                using (var connection = dbConnect.GetConnection())
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

                        command.CommandText = @"select i1.defect_type
                                                from inspection i1 
                                                group by i1.defect_type;";
                        reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            ListDefectName.Add(reader[0].ToString());
                        }
                        reader.Close();
                    }
                }

                cbb_Model_Name.DataSource = new BindingSource(ListModels, null);
                cbb_Model_Name.DisplayMember = "Value"; // Show item names
                cbb_Model_Name.ValueMember = "Key";

                cbb_defectName.DataSource = new BindingSource(ListDefectName, null);
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
                using (var connection = dbConnect.GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand())
                    {
                        MySqlDataReader reader;
                        command.Connection = connection;
                        command.CommandText = @"select i1.defect_type, count(*) as total_defect from inspection i1
                                            group by i1.defect_type";
                        reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            TopDefectList.Add(
                                new KeyValuePair<string, int>(reader[0].ToString(), Convert.ToInt16(reader[1])));
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
                startDate = dtpStartDate.Value;
                endDate = dtpEndDate.Value;

                ListCoodinate = new List<KeyValuePair<string, PointF>>();
                using (var connection = dbConnect.GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand())
                    {
                        MySqlDataReader reader;
                        command.Connection = connection;
                        command.CommandText = @"select i1.defect_type, i1.defect_x,i1.defect_y from 
                                                 model m1 inner join process_d prd using(model_id)
                                                 inner join cell c1 using(model_id)
                                                 inner join inspection i1 using(cell_id)
                                                 where m1.model_name =@name_model";
                        command.Parameters.Add("@name_model", MySqlDbType.VarChar).Value = _modelName;

                        reader = command.ExecuteReader();
                        while (reader.Read())
                        {

                            PointF _p = new PointF((float)Convert.ToDouble(reader[1]), (float)Convert.ToDouble(reader[2]));

                            ListCoodinate.Add(
                                new KeyValuePair<string, PointF>(reader[0].ToString(), _p));
                        }
                        reader.Close();
                    }
                }

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
                        Enum.TryParse(_defect_type, out _defect_name);

                        switch (_defect_name)
                        {
                            case Defect_Name.Particle:
                                color = Color.Red;
                                break;
                            case Defect_Name.Other:
                                color = Color.Blue;
                                break;
                            case Defect_Name.Crack:
                                color = Color.Yellow;
                                break;
                            case Defect_Name.Contamination:
                                color = Color.OrangeRed;
                                break;
                            case Defect_Name.Wrinkle:
                                color = Color.OliveDrab;
                                break;
                        }
                        _p = kvp.Value;
                        var scatter = _scatterPlot.Plot.Add.Scatter(_p.X, _p.Y, new ScottPlot.Color(color.R, color.G, color.B));

                        //scatter.LegendText = _defect_type;
                    }
                    _scatterPlot.Refresh();
                }


                var keyCount = ListCoodinate
                           .GroupBy(pair => pair.Key)
                           .Select(group => new { Key = group.Key, Count = group.Count() })
                           .OrderByDescending(group => group.Count); // Sắp xếp giảm dần

                int idx = 1;

                if (keyCount != null && keyCount.Count() > 0)
                {
                    foreach (var item in keyCount)
                    {
                        _defect_type = item.Key;
                        Enum.TryParse(_defect_type, out _defect_name);

                        switch (_defect_name)
                        {
                            case Defect_Name.Particle:
                                color = Color.Red;
                                break;
                            case Defect_Name.Other:
                                color = Color.Blue;
                                break;
                            case Defect_Name.Crack:
                                color = Color.Yellow;
                                break;
                            case Defect_Name.Contamination:
                                color = Color.OrangeRed;
                                break;
                            case Defect_Name.Wrinkle:
                                color = Color.OliveDrab;
                                break;
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
                            Layout_Panel_CountDeffect.Controls[$"lbNameDefect{idx}"].ForeColor = color;
                            Layout_Panel_CountDeffect.Controls[$"lbNameDefect{idx}"].Text = _defect_type;

                        }
                        if (lb_Defect != null)
                        {
                            Layout_Panel_CountDeffect.Controls[$"lbCount{idx}"].ForeColor = color;
                            Layout_Panel_CountDeffect.Controls[$"lbCount{idx}"].Text = item.Count.ToString();
                        }

                        idx++;
                    }
                }
                else
                {
                    idx = 1;
                    foreach (var item in ListDefectName)
                    {
                        _defect_type = item;
                        Enum.TryParse(_defect_type, out _defect_name);

                        switch (_defect_name)
                        {
                            case Defect_Name.Particle:
                                color = Color.Red;
                                break;
                            case Defect_Name.Other:
                                color = Color.Blue;
                                break;
                            case Defect_Name.Crack:
                                color = Color.Yellow;
                                break;
                            case Defect_Name.Contamination:
                                color = Color.OrangeRed;
                                break;
                            case Defect_Name.Wrinkle:
                                color = Color.OliveDrab;
                                break;
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
                            Layout_Panel_CountDeffect.Controls[$"lbNameDefect{idx}"].ForeColor = color;
                            Layout_Panel_CountDeffect.Controls[$"lbNameDefect{idx}"].Text = _defect_type;

                        }
                        if (lb_Defect != null)
                        {
                            Layout_Panel_CountDeffect.Controls[$"lbCount{idx}"].ForeColor = color;
                            Layout_Panel_CountDeffect.Controls[$"lbCount{idx}"].Text = 0.ToString();
                        }
                        idx++;
                    }
                }

            }
            catch
            {

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
                                    case Defect_Name.Particle:
                                        color = Color.Red;
                                        break;
                                    case Defect_Name.Other:
                                        color = Color.Blue;
                                        break;
                                    case Defect_Name.Crack:
                                        color = Color.Yellow;
                                        break;
                                    case Defect_Name.Contamination:
                                        color = Color.OrangeRed;
                                        break;
                                    case Defect_Name.Wrinkle:
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
    }
}
