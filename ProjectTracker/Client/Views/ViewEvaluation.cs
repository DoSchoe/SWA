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
using MyLib;

namespace Client.Views
{
    
    public partial class ViewEvaluation : Form,IView
    {
        #region Members

        private IController mController;
        #endregion
        public ViewEvaluation()
        {
            InitializeComponent();
        }

        public void setController(IController controller)
        {
            mController = controller;
        }

        public void displayEvaluation(Project projectToEvaluate)
        {
            tbx_ProjectName.Text = projectToEvaluate.ProjectName;
            tbx_ProjectedEffort.Text = projectToEvaluate.MTimeEffortProjected.ToString();
            tbx_CurrentEffort.Text = projectToEvaluate.MTimeEffortCurrent.ToString();
            tbx_Difference.Text = (projectToEvaluate.MTimeEffortCurrent - projectToEvaluate.MTimeEffortProjected).ToString();
        }
    }
}
