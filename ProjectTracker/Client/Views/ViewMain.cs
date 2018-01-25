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
using Client.Views;

namespace Client
{
    public partial class ViewMain : Form, IView
    {
        #region Members

        private IController mController;
        private bool recording = false;
        #endregion
        public ViewMain()
        {
            InitializeComponent();
        }

        public void setController(IController controller)
        {
            mController = controller;
        }

        private void btn_AddProject_Click(object sender, EventArgs e)
        {
            mController.addProject();
        }

        private void btn_Evaluate_Click(object sender, EventArgs e)
        {

        }

        private void btn_Record_Click(object sender, EventArgs e)
        {
            if (recording)
            {
                btn_Record.Text = "Record";
                //StopStopwatch
                //ShowDialogCommit
                recording = false;
            }
            else
            {
                btn_Record.Text = "Stop";
                //StartStopwatch
                recording = true;
            }
        }
    }
}