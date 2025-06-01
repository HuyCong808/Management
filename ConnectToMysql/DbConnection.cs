using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnectionStringBuilder = MySql.Data.MySqlClient.MySqlConnectionStringBuilder;

namespace NU_Clinic.ConnectToMysql
{
    public class DbConnection
    {
        MySqlConnectionStringBuilder connc = new MySqlConnectionStringBuilder();

        bool _connected = false;
        MySqlConnection connector;

        public bool Connected
        {
            get { return _connected; }
            set { _connected = value; }
        }

        public DbConnection()
        {
            connc.Server = "localhost";
            connc.Port = 3306;
            connc.UserID = "root";
            connc.Password = "CongSon98@";
            connc.Database = "screen_factory";
            //connc.Database = "nuclinic";
            connc.SslMode = MySql.Data.MySqlClient.MySqlSslMode.None;
        }

        public MySql.Data.MySqlClient.MySqlConnection GetConnection()
        {
            return new MySql.Data.MySqlClient.MySqlConnection(connc.ToString());
        }

        public bool ConnectToMySql()
        {
            try
            {
                if(connector ==null)
                    connector = GetConnection();

                connector.Open();
                _connected = true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DisconnectToMySql()
        {
            try
            {
                if (connector == null)
                    connector = GetConnection();

                connector.Close();
                _connected = false;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
