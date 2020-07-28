using System;
using System.Windows.Forms;

namespace TranslateTool
{
    public partial class ResizeForm : Form
    {
        public static ResizeForm Form;
        private static Form1 form1;
        public ResizeForm()
        {
            InitializeComponent();
            ResizeForm.Form = this;
            form1 = new Form1();
            form1.Show(this);
        }

        public void Start()
        {
            ResizeNW.MouseDown += Form1.Form.ResizeXY_MouseDown;
            ResizeNE.MouseDown += Form1.Form.ResizeXY_MouseDown;
            ResizeSW.MouseDown += Form1.Form.ResizeXY_MouseDown;
            ResizeSE.MouseDown += Form1.Form.ResizeXY_MouseDown;
            ResizeW.MouseDown += Form1.Form.ResizeX_MouseDown;
            ResizeE.MouseDown += Form1.Form.ResizeX_MouseDown;
            ResizeN.MouseDown += Form1.Form.ResizeY_MouseDown;
            ResizeS.MouseDown += Form1.Form.ResizeY_MouseDown;
            ResizeNW.MouseUp += Form1.Form.ResizeXY_MouseUp;
            ResizeNE.MouseUp += Form1.Form.ResizeXY_MouseUp;
            ResizeSW.MouseUp += Form1.Form.ResizeXY_MouseUp;
            ResizeSE.MouseUp += Form1.Form.ResizeXY_MouseUp;
            ResizeW.MouseUp += Form1.Form.ResizeX_MouseUp;
            ResizeE.MouseUp += Form1.Form.ResizeX_MouseUp;
            ResizeN.MouseUp += Form1.Form.ResizeY_MouseUp;
            ResizeS.MouseUp += Form1.Form.ResizeY_MouseUp;

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (form1.Closed)
            {
                Close();
            }
        }
    }
}
