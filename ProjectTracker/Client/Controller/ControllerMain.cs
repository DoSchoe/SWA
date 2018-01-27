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
        void AddProject();
        void StartStopwatch();
        void StopStopwatch();
        void CommitTime();
        void Evaluate();
        void SetCurrentProject(Project project);
        Project CurrentProject { get; }
        List<Project> mProjects { get; set; }
    }

    class ControllerMain:IController
    {
        private IModel mModelMain;
        private IView mViewMain;
        private IView mViewAddProject;
        private IView mViewEvaluation;
        private Stopwatch mStopWatch=new Stopwatch();
        public Project CurrentProject => mModelMain.mCurrentProject;
        public List<Project> mProjects { get; set; }

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

        public void AddProject()
        {
            ViewAddProject addDialog = new ViewAddProject();
            if(addDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Project projectToAdd = new Project(addDialog.projectName,addDialog.projectedTime);
                string command = projectToAdd.ToString();
                PublishChange(command);
            }
        }

        public void StartStopwatch()
        {
            mStopWatch.Reset();
            mStopWatch.Start();
        }

        public void StopStopwatch()
        {
            mStopWatch.Stop();
        }

        public void CommitTime()
        {
            TimeSpan timeToAdd = mStopWatch.Elapsed;
            DialogResult dialogResult = MessageBox.Show("Do you want to commit " + timeToAdd + " to project " + mModelMain.mCurrentProject.ProjectName, "Commit", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                //Commit to Project
            }
        }

        public void Evaluate()
        {
            ViewEvaluation formEvaluation = new ViewEvaluation();
            formEvaluation.Show();
            formEvaluation.displayEvaluation(mModelMain.mCurrentProject);
        }

        public void SetCurrentProject(Project project)
        {
            mModelMain.mCurrentProject = project;
        }


        private void PublishChange(string command)
        {
            MessageBox.Show(command);
            //if connected
            //sendToServer
            //else
            //write to logfile
        }
    }
}
