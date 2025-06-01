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
    public partial class UserDetail : UserControl
    {
        private DbConnection dbConnect;
        UserControl _panel_userControl;
        public UserDetail()
        {
            InitializeComponent();

            Panel_main.Controls.Clear();

            _panel_userControl = new ViewUser();
            _panel_userControl.Dock = DockStyle.Fill;
            Panel_main.Controls.Add(_panel_userControl);
        }

        MySqlConnection con;

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Panel_main.Controls.Clear();

            _panel_userControl = new AddUser();
            _panel_userControl.Dock = DockStyle.Fill;
            Panel_main.Controls.Add(_panel_userControl);
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            Panel_main.Controls.Clear();

            _panel_userControl = new ViewUser();
            _panel_userControl.Dock = DockStyle.Fill;
            Panel_main.Controls.Add(_panel_userControl);
        }
    }
}
