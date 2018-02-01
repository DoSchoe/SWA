using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using MyLib;

namespace TestClient
{
    class Program
    {
        private static List<Project> mProjects = new List<Project>();
        private static IRemoteUpdate mRemoteUpdater;
        private static string Response;
        public const char SEPdata = '#';
        static void Main(string[] args)
        {

            try
            {
                // Connect to the service by using channel
                ChannelFactory<IRemoteUpdate> cFactory;
                cFactory = new ChannelFactory<IRemoteUpdate>("WSHttpBinding_IRemoteUpdate");
                mRemoteUpdater = cFactory.CreateChannel();
                Response = mRemoteUpdater.updateProjectListAsString();
                string[] parts = Response.Split(SEPdata);

                foreach (string part in parts)
                {
                    mProjects.Add(new Project(part));
                }
                OutputProjects(mProjects);
                mProjects.Clear();
                mProjects = mRemoteUpdater.updatedProjectList();
                OutputProjects(mProjects);
                Console.ReadKey();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void OutputProjects(List<Project> projects)
        {
            if (projects.Count == 0)
            {
                Console.WriteLine("Project file was empty or wasn't found!");
            }
            else
            {
                Console.WriteLine("CURRENT PROJECTS at the TESTCLIENT:");
                Console.WriteLine("========================================");
                foreach (Project project in projects)
                {
                    Console.WriteLine(project.ToString());
                }
                Console.WriteLine("========================================\n");
            }
        }
    }
}
