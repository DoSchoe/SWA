using MyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Model
{
    public interface IModel
    {
        Project mCurrentProject { get; set; }
        List<Project> mProjects { get; set; }
    }
}
