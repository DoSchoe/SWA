using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyLib;

namespace Client.Model
{
    class ModelMain:IModel
    {
        public List<Project> mProjects { get; set; }
        public Project mCurrentProject { get; set; }
    }
}