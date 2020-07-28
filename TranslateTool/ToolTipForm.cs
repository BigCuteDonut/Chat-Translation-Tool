using System;
using System.Windows.Forms;

namespace TranslateTool
{
    public partial class ToolTipForm : Form
    {
        public ToolTipForm()
        {
            InitializeComponent();
        }

        private void ToolTipForm_Load(object sender, EventArgs e)
        {
            var oldStyle = Form1.GetWindowLong(Handle, -20);
            Form1.SetWindowLong(Handle, -20, oldStyle | 0x80000 | 0x20);

            Opacity = 0;
        }

        public void ShowText(string text)
        {
            var oldStyle = Form1.GetWindowLong(Handle, -20);
            Form1.SetWindowLong(Handle, -20, oldStyle | 0x80000 | 0x20);
            textBox1.Text = text;
        }

        public string GetText()
        {
            return textBox1.Text;
        }
    }
}
