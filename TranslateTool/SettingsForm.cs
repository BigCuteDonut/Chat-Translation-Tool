using System;
using System.Drawing;
using System.Windows.Forms;
using TranslateTool.Properties;

namespace TranslateTool
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void CloseButton_Clicked(object sender, EventArgs e)
        {
            Form1.Form.SettingsShown = false;
            Hide();
        }

        private void SettingsForm_Deactivate(object sender, EventArgs e)
        {
            Form1.Form.SettingsShown = false;
            Hide();
        }

        private void CloseButton_MouseEnter(object sender, EventArgs e)
        {
            CloseHighlight();
        }

        private void CloseButton_MouseLeave(object sender, EventArgs e)
        {
            DisableCloseHighlight();
        }

        private void CloseHighlight()
        {
            CloseButton.BackColor = Color.FromArgb(36, 68, 100);
        }

        private void DisableCloseHighlight()
        {
            CloseButton.BackColor = Color.FromArgb(19, 41, 63);
        }

        private void InfoCheck_CheckedChanged(object sender, EventArgs e)
        {
            var checkbox = sender as CheckBox;

            if (checkbox.Checked)
            {
                Settings.Default.InputInfoEnabled = true;
                Translate.EnableAdditionalInputInfo();
            }
            else
            {
                Settings.Default.InputInfoEnabled = false;
                Translate.DisableAdditionalInputInfo();
            }
        }

        private void AutoHideCheck_CheckedChanged(object sender, EventArgs e)
        {
            var checkbox = sender as CheckBox;

            Settings.Default.PreventWindowTransparency = checkbox.Checked;
            Form1.Form.AutoHideEnabled = !checkbox.Checked;
        }
    }
}
