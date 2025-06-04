using MySql.Data.MySqlClient;
using NU_Clinic.ConnectToMysql;
using NU_Clinic.form;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NU_Clinic
{
    public partial class frmLogin : Form
    {
        private DbConnection dbConnect;
        public frmLogin()
        {
            InitializeComponent();
            this.ActiveControl = txtUser;
            txtUser.Focus();

            dbConnect = new DbConnection();
            con = dbConnect.GetConnection();
            txtUser.Text = "admin1";
            txtPass.Text = "1234";
        }

        MySqlConnection con;

        int count;
        private string username, password;


        private void btnLogin_Click(object sender, EventArgs e)
        {
            login();
        }

        private void login()
        {
            try
            {
                string username = txtUser.Text.Trim();
                string password = txtPass.Text.Trim();

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    lblWarning.Text = "Username and Password can't be blank";
                    return;
                }

                string connectionString = "server=127.0.0.1;user=root;password=123456789;database=screen_production;AllowPublicKeyRetrieval=True;SslMode=none;";

                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open(); // Mở kết nối

                    string query = "SELECT * FROM user WHERE username = @username AND password = @password";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);

                        using (MySqlDataAdapter data = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            data.Fill(dt);

                            if (dt.Rows.Count == 1)
                            {
                                DashBoard dashBorad = new DashBoard();
                                dashBorad.Firstname = dt.Rows[0]["firstname"].ToString();
                                dashBorad.Middlename = dt.Rows[0]["middlename"].ToString();
                                dashBorad.Lastname = dt.Rows[0]["lastname"].ToString();
                                dashBorad.Username = dt.Rows[0]["username"].ToString();
                                dashBorad.Show();
                                dashBorad.Show_information();

                                this.Hide();
                            }
                            else
                            {
                                lblWarning.Text = "Please try again";
                                txtUser.Clear();
                                txtPass.Clear();
                                txtUser.Focus();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Warning: " + ex.Message);
            }
        }



        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            txtUser.Clear();
            txtPass.Clear();
        }



        private void pictureBox2_Click_1(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Do you want to exit the application?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                Application.Exit();
            }
            else if (dialogResult == DialogResult.No)
            {
                //do something else
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            login();
            //select();
        }

        private void frmLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnLogin_Click(sender, e);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblWarning.Text = "";
        }

        private void bunifuTileButton1_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Do you want to exit the application?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                Application.Exit();
            }
            else if (dialogResult == DialogResult.No)
            {
                //do something else
            }
        }

        public void select()
        {
            // MySqlConnection con = new MySqlConnection("server = localhost; database = nuclinic; username = root; password = ;");
            SqlConnection Cn = new SqlConnection("server = localhost; database = nuclinic; username = root; password = ;");
            SqlCommand Cmd = Cn.CreateCommand();
            try
            {
                SqlDataReader myReader = null;
                SqlCommand myCommand = new SqlCommand("select * firstname from user", Cn);
                myReader = myCommand.ExecuteReader();
                while (myReader.Read())
                {
                    // textBox1.Text = myReader["Column1"].ToString();

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }



    }
}
