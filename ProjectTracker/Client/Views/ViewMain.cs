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
using MyLib;

namespace Client
{
    public partial class ViewMain : Form, IView
    {
        #region Members

        private IController mController;
        private bool recording = false;
        private Project currentProject;
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
            // replace with actual current project
            Project currentProject = new Project();
            mController.evaluate(currentProject);
        }

        private void btn_Record_Click(object sender, EventArgs e)
        {
            if (recording)
            {
                btn_Record.Text = "Record";
                mController.stopStopwatch();
                mController.commitTime(currentProject.ProjectName);
                recording = false;
            }
            else
            {
                btn_Record.Text = "Stop";
                mController.startStopwatch();
                recording = true;
            }
        }
    }
}