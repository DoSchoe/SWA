using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client.Controller;

namespace Client.Views
{
    public partial class ViewAddProject : Form, IView
    {
        #region Members

        private IController mController;
        #endregion

        public string projectName;
        public TimeSpan projectedTime;

        public ViewAddProject()
        {
            InitializeComponent();
        }

        public void setController(IController controller)
        {
            mController = controller;
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            // read Input values
            projectName = tbx_ProjectName.Text;
            projectedTime = new TimeSpan(Convert.ToInt32(nud_projectedEffort.Value), 0, 0);
        }

        private void tbx_ProjectName_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Seperator kommt nicht vor
            if (e.KeyChar.ToString().Contains("$") || e.KeyChar.ToString().Contains("?") || e.KeyChar.ToString().Contains("#"))
                e.Handled = true;
        }

        private void tbx_ProjectName_TextChanged(object sender, EventArgs e)
        {
            //Long enough
            if (tbx_ProjectName.Text.Length > 4)
                //List not empty
                if (mController.GetProjects() != null)
                {
                    //No duplicate name
                    if (mController.GetProjects().Where(p => p.ProjectName == tbx_ProjectName.Text).Count() == 0)
                        btn_OK.Enabled = true;
                    else
                        btn_OK.Enabled = false;
                }
                //List is empty
                else
                    btn_OK.Enabled = true;
            //Too short
            else
                btn_OK.Enabled = false;
        }
    }
}