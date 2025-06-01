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

        private new void Load()
        {
            //txtSearch.Text = "";
            //result.Visible = false;
            string query = "select u1 .id as 'id'" +
                ",u1.firstname as'Fristname'" +
                ",u1.lastname as 'Lastname'" +
                ",u1.middlename as 'Middlename'" +
                ",u1.username as 'UserName'" +
                ",u1.password as 'Password'" +
                ",r1.role_name as 'RoleName' " +
                "from user u1 inner join role_detail r1 on u1.role_id = r1.id";

            using (MySqlDataAdapter adpt = new MySqlDataAdapter(query, con))
            {

                DataSet dset = new DataSet();

                adpt.Fill(dset);

                dataGridView1.DataSource = dset.Tables[0];
            }
            con.Close();
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
