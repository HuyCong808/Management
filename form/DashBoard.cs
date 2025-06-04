//using Managerment.form;
using MySql.Data.MySqlClient;
using NU_Clinic.ConnectToMysql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace NU_Clinic.form
{
    public partial class DashBoard : Form
    {
        private DbConnection dbConnect;

        private DateTime startDate;
        private DateTime endDate;

        UserControl _panel_userControl;
        MySqlConnection con;


        private string firstname, lastname, middlename, username;

        public string Lastname
        {
            get { return lastname; }
            set { lastname = value; }
        }

        public string Firstname
        {
            get { return firstname; }
            set { firstname = value; }
        }

        public string Middlename
        {
            get { return middlename; }
            set { middlename = value; }
        }

        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        public DashBoard()
        {
            InitializeComponent();
            _panel_Content.Controls.Clear();

            _panel_userControl = new Analysis();
            _panel_userControl.Dock = DockStyle.Fill;
            _panel_Content.Controls.Add(_panel_userControl);
        }

        private void Setbackground()
        {
            bunifuFlatButton2.BackColor = Color.FromArgb(26, 42, 64); // Với Form
            bunifuFlatButton1.BackColor = Color.FromArgb(26, 42, 64); // Với Form
            bunifuFlatButton3.BackColor = Color.FromArgb(26, 42, 64); // Với Form
        }
        private void bunifuFlatButton3_Click(object sender, EventArgs e)
        {
            Setbackground();

            bunifuFlatButton3.BackColor = Color.FromArgb(19, 30, 46); // Với Form 


            _panel_Content.Controls.Clear();

            _panel_userControl = new UserDetail();
            _panel_userControl.Dock = DockStyle.Fill;
            _panel_Content.Controls.Add(_panel_userControl);
        }

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            _panel_Content.Controls.Clear();

            _panel_userControl = new Analysis();
            _panel_userControl.Dock = DockStyle.Fill;
            _panel_Content.Controls.Add(_panel_userControl);
        }

        private void bunifuFlatButton2_Click(object sender, EventArgs e)
        {
            Setbackground();
            bunifuFlatButton2.BackColor = Color.FromArgb(19, 30, 46); // Với Form 
        }

        private void bunifuFlatButton4_Click(object sender, EventArgs e)
        {
            Setbackground();

            _panel_Content.Controls.Clear();
            //_panel_userControl = new Mapping();
            _panel_userControl.Dock = DockStyle.Fill;
            _panel_Content.Controls.Add(_panel_userControl);
        }

        public void Show_information()
        {
            try
            {
                date.Text = DateTime.Now.ToLongDateString();
                time.Text = DateTime.Now.ToLongTimeString();
                label3.Text = username;
                label6.Text = Firstname + " " + Middlename + " " + Lastname;
            }
            catch (Exception ex) { }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Do you want to logout " + Firstname + "?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                frmLogin frm = new frmLogin();
                this.Hide();
                frm.Show();
            }
            else if (dialogResult == DialogResult.No)
            {
                //do something else
            }
        }

        public void Change_Form()
        {
            _panel_Content.Controls.Clear();
        }
    }
}
