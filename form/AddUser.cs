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
using NU_Clinic.ConnectToMysql;

namespace NU_Clinic.form
{
    public partial class AddUser : UserControl
    {
        private DbConnection dbConnect;

        public List<KeyValuePair<int, string>> ListRole { get; private set; }
        public AddUser()
        {
            InitializeComponent();

            dbConnect = new DbConnection();
            con = dbConnect.GetConnection();

            Load_Role();
        }

        MySqlConnection con;

        private void Load_Role()
        {
            try
            {
                ListRole = new List<KeyValuePair<int, string>>();
                using (var connection = dbConnect.GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand())
                    {
                        MySqlDataReader reader;
                        command.Connection = connection;

                        command.CommandText = @"select * from role_detail";
                        reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            ListRole.Add(
                                new KeyValuePair<int, string>(Convert.ToInt16(reader[0]), reader[1].ToString()));
                        }
                        reader.Close();
                    }
                }

                cbbRoleName.DataSource = new BindingSource(ListRole, null);
                cbbRoleName.DisplayMember = "Value"; // Show item names
                cbbRoleName.ValueMember = "Key";
            }
            catch
            {
            }

        }

        private void bunifuFlatButton8_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtFristName.Text != "" && txtLastName.Text != "" && txtUserName.Text != "" && txtPassword.Text != "")
                {
                    int _role = cbbRoleName.SelectedIndex;

                    using (var connection = dbConnect.GetConnection())
                    {
                        connection.Open();
                        using (var command = new MySqlCommand())
                        {

                            command.Connection = connection;

                            command.CommandText = @"INSERT INTO user (firstname, lastname,middlename,username,password,role_id) 
                            VALUES (@firstname, @lastname,@middlename,@username,@password,@role_id)";

                            command.Parameters.Add("@firstname", MySqlDbType.VarChar).Value = txtFristName.Text;
                            command.Parameters.Add("@lastname", MySqlDbType.VarChar).Value = txtLastName.Text;
                            command.Parameters.Add("@middlename", MySqlDbType.VarChar).Value = txtMiddleName.Text;
                            command.Parameters.Add("@username", MySqlDbType.VarChar).Value = txtUserName.Text;
                            command.Parameters.Add("@password", MySqlDbType.VarChar).Value = txtPassword.Text;
                            command.Parameters.Add("@role_id", MySqlDbType.Int16).Value = _role;
                            var a = command.ExecuteReader();
                            MessageBox.Show("Insert done!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }

                }
                else
                {
                    MessageBox.Show("Please check validate textbox!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch { }
        }
    }
}
