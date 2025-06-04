using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Managerment.form
{
    public partial class DetailForm : Form
    {
        public DetailForm()
        {
            InitializeComponent();
        }
        public DetailForm(DataTable data) : this()
        {
            dataGridView1.DataSource = data;
            // Nền đen
            dataGridView1.BackgroundColor = Color.FromArgb(50, 50, 50);

            // Nền của các hàng và cột (cell) cũng thành đen
            dataGridView1.DefaultCellStyle.BackColor = Color.FromArgb(50, 50, 50);

            // Chữ trắng
            dataGridView1.DefaultCellStyle.ForeColor = Color.White;

            // Header chữ trắng nền đen
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(50, 50, 50);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            // Tắt grid lines
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dataGridView1.GridColor = Color.FromArgb(50, 50, 50); // grid màu đen (ẩn)

            // Nếu muốn bỏ cả các line giữa các hàng và cột (không thấy các đường kẻ)
            dataGridView1.RowHeadersVisible = false;

            // Tùy chọn font, bạn có thể set font dễ nhìn
            dataGridView1.DefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Regular);
            MakeColumnHeadersUpperCase();
        }
        private void MakeColumnHeadersUpperCase()
        {
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.HeaderText = col.HeaderText.ToUpperInvariant();
            }
        }

    }
}
