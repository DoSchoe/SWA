using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Model;
using Client.Views;
using MyLib;
using System.Diagnostics;
using System.Windows.Forms;

namespace Client.Controller
{
    public interface IController
    {
        void addProject();
        void startStopwatch();
        void stopStopwatch();
        void commitTime(string project);
        void evaluate(Project project);
    }

    class ControllerMain:IController
    {
        private IModel mModelMain;
        private IView mViewMain;
        private IView mViewAddProject;
        private IView mViewEvaluation;
        private Stopwatch mStopWatch;

        public ControllerMain(IModel modelMain, IView viewMain, IView viewAdd, IView viewEvaluation)
        {
            mModelMain = modelMain;
            mViewMain = viewMain;
            mViewAddProject = viewAdd;
            mViewEvaluation = viewEvaluation;

            mViewMain.setController(this);
            mViewAddProject.setController(this);
            mViewEvaluation.setController(this);
        }

        public void addProject()
        {
            ViewAddProject addDialog = new ViewAddProject();
            if(addDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Project projectToAdd = new Project(addDialog.projectName,addDialog.projectedTime);
                string command = projectToAdd.ToString();
                publishChange(command);
            }
        }

        public void startStopwatch()
        {
            mStopWatch.Reset();
            mStopWatch.Start();
        }

        public void stopStopwatch()
        {
            mStopWatch.Stop();
        }

        public void commitTime(string project)
        {
            TimeSpan timeToAdd = mStopWatch.Elapsed;
            DialogResult dialogResult = MessageBox.Show("Do you want to commit " + timeToAdd + " to project " + project, "Commit", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                //Commit to Project
            }
        }

        public void evaluate(Project project)
        {
            ViewEvaluation formEvaluation = new ViewEvaluation();
            formEvaluation.Show();
            formEvaluation.displayEvaluation(project);
        }

        private void publishChange(string command)
        {
            //if connected
            //sendToServer
            //else
            //write to logfile
        }
    }
}
