using MySql.Data.MySqlClient;
using NU_Clinic.ConnectToMysql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace NU_Clinic.form
{
    public partial class ViewUser : UserControl
    {
        private DbConnection dbConnect;
        public ViewUser()
        {
            InitializeComponent();

            label2.Text = dataGridView1.RowCount.ToString();
            dbConnect = new DbConnection();
            con = dbConnect.GetConnection();

            Load();
        }

        MySqlConnection con;

        private void Load()
        {
            string connectionString = "server=127.0.0.1;user=root;password=123456789;database=screen_production;AllowPublicKeyRetrieval=True;SslMode=none;";

            string query = @"
        SELECT 
            u1.id AS 'ID',
            u1.firstname AS 'Firstname',
            u1.lastname AS 'Lastname',
            u1.middlename AS 'Middlename',
            u1.username AS 'Username',
            u1.password AS 'Password',
            r1.role_name AS 'RoleName'
        FROM user u1
        INNER JOIN role_detail r1 ON u1.role_id = r1.id";

            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    using (MySqlDataAdapter adpt = new MySqlDataAdapter(query, con))
                    {
                        DataSet dset = new DataSet();
                        adpt.Fill(dset);
                        dataGridView1.DataSource = dset.Tables[0];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dataGridView1.Columns[e.ColumnIndex].Name == "Delete")
                {
                    int id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["id"].Value.ToString());
                    MySqlDataAdapter ada = new MySqlDataAdapter(
                                     "select id as 'id'," +
                                "firstname as'Fristname'," +
                                "lastname as 'Lastname'," +
                                "middlename as 'Middlename'," +
                                "username as 'UserName'," +
                                "password as 'Password' from user where id= '" + id + "'", con);

                    dataGridView1.CurrentCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    dataGridView1.Rows[e.RowIndex].Selected = true;
                    dataGridView1.Focus();
                    DataTable dt = new DataTable();
                    ada.Fill(dt);
                    dataGridView1.DataSource = dt;

                    foreach (DataRow dr in dt.Rows)
                    {
                        textBox1.Text = dr["id"].ToString();

                    }

                    if (MessageBox.Show("Are you sure you want to delete this?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {

                        try
                        {

                            MySqlCommand cmd = new MySqlCommand();
                            cmd.Connection = con;
                            cmd.CommandText = "delete from user where id = '" + textBox1.Text + "'";

                            con.Open();
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Item Deleted");
                            con.Close();
                            Load();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                    else
                    {
                        Load();
                    }
                }
                else if (dataGridView1.Columns[e.ColumnIndex].Name == "View")
                {
                    if (dataGridView1.Columns[e.ColumnIndex].Name == "View")
                    {

                        int id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["id"].Value.ToString());

                        MySqlDataAdapter ada = new MySqlDataAdapter(
                                     "select id as 'id'," +
                                "firstname as'Fristname'," +
                                "lastname as 'Lastname'," +
                                "middlename as 'Middlename'," +
                                "username as 'UserName'," +
                                "password as 'Password' from user where id= '" + id + "'", con);

                        dataGridView1.CurrentCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                        dataGridView1.Rows[e.RowIndex].Selected = true;


                        DataTable dt = new DataTable();
                        ada.Fill(dt);
                        dataGridView1.DataSource = dt;

                        foreach (DataRow dr in dt.Rows)
                        {



                        }
                    }
                    else
                    {
                        Load();
                    }
                }
                else if (dataGridView1.Columns[e.ColumnIndex].Name == "Edit")
                {
                    if (dataGridView1.Columns[e.ColumnIndex].Name == "Edit")
                    {

                        int id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["id"].Value.ToString());


                    }
                }
            }
            catch { }
        }
    }
}
