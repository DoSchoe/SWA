using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client.Controller;
using Client.Model;
using Client.Views;

namespace Client
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            IModel _modelMain = new ModelMain();
            ViewMain _viewMain = new ViewMain();
            ViewAddProject _viewAddProject = new ViewAddProject();
            ViewEvaluation _viewEvaluation = new ViewEvaluation();
            IController cnt = new ControllerMain(_modelMain, _viewMain, _viewAddProject, _viewEvaluation);
            Application.Run(_viewMain);
        }
    }
}
