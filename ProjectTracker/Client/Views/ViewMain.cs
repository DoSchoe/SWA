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
        #endregion
        public ViewMain()
        {
            InitializeComponent();
        }

        public void setController(IController controller)
        {
            mController = controller;
        }
    }
}
