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
        private ClientData mMyData;
        public List<Project> mProjects { get; set; }
        private int mClientProjectListHash;
        private int mServertProjectListHash;
        public Project mCurrentProject { get; set; }
        public ClientData MyData
        {
            get => mMyData;
            set => mMyData = value;
        }
        public int ClientProjectListHash
        {
            get => mClientProjectListHash;
            set => mClientProjectListHash = value;
        }

        public int ServerProjectListHash
        {
            get => mServertProjectListHash;
            set => mServertProjectListHash = value;
        }

        public ModelMain()
        {
            mMyData = new ClientData(System.Environment.MachineName, ClientStati.NotStarted);
            mProjects = new List<Project>();
            mClientProjectListHash = 0;
            mServertProjectListHash = 0;
        }
    }
}