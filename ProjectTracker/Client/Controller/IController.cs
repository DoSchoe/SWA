using MyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        List<Project> GetProjects();
    }
}
