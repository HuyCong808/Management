using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NU_Clinic.Button_Circular
{
    public class ButtonElippse : Control
    {
        public Color CircleColor { get; set; } = Color.LightBlue;
        protected override void OnPaint(PaintEventArgs prevent)
        {

            //GraphicsPath grapphics = new GraphicsPath();
            //grapphics.AddEllipse(0, 0, ClientSize.Width, ClientSize.Height);
            //this.Region = new System.Drawing.Region(grapphics);
            //base.OnPaint(prevent);

            Graphics g = prevent.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int diameter = Math.Min(Width, Height) - 2;
            // Vẽ hình tròn
            g.FillEllipse(new SolidBrush(CircleColor), 0, 0, diameter, diameter);
            g.DrawEllipse(new Pen(CircleColor, 2), 0, 0, diameter, diameter);
        }
    }
}
