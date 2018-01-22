using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Model;
using Client.Views;

namespace Client.Controller
{
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
    }
}
