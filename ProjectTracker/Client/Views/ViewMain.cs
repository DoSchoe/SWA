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
    public partial class ViewMain : Form, IViewMain
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

        public void UpdateProjects(List<Project> projects)
        {
            comboBox1.DataSource = null;
            comboBox1.DisplayMember = "mProjectName";
            comboBox1.DataSource = projects;
        }

        private void btn_AddProject_Click(object sender, EventArgs e)
        {
            mController.AddProject();
        }

        private void btn_Evaluate_Click(object sender, EventArgs e)
        {
            mController.Evaluate();
        }

        private void btn_Record_Click(object sender, EventArgs e)
        {
            // when already recording, stop is displayed
            if (recording)
            {
                // change text to record, stop stopwatch and ask if time should be committed
                btn_Record.Text = "Record";
                mController.StopStopwatch();
                mController.CommitTime();
                recording = false;
            }
            // when not recording, start recording
            else
            {
                btn_Record.Text = "Stop";
                mController.StartStopwatch();
                recording = true;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // set currentProject
            mController.SetCurrentProject((Project)comboBox1.SelectedItem);
            // if valid project selected
            if ((Project)comboBox1.SelectedItem != null)
            {
                btn_Evaluate.Enabled = true;
                btn_Record.Enabled = true;
            }
            // no project selcted
            else
            {
                btn_Evaluate.Enabled = false;
                btn_Record.Enabled = false;
            }
        }


    }
}