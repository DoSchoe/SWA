using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Model;
using Client.Views;
using MyLib;

namespace Client.Controller
{
    public interface IController
    {
        void addProject();
    }

    class ControllerMain:IController
    {
        private IModel mModelMain;
        private IView mViewMain;
        private IView mViewAddProject;
        private IView mViewEvaluation;

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
                //if connected
                //sendToServer
                //else
                //write to logfile
            }
        }

        public void startStopwatch()
        {

        }

        public void stopStopwatch()
        {

        }
    }
}
