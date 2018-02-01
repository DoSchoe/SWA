using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MyLib;

namespace Server
{
    public class ServerClass
    {
        private string PROJECT_FILE = @"c:\Temp\Projects.txt";
        private int SERVER_PORT = 9050;
        private int HEARTBEAT_DELAY = 5000;
        public const char SEPdata = '#';
        private List<Project> mProjects;
        private List<IPEndPoint> mClients;
        private bool Run = true;
        private int ProjectListHash = 0;

        /// <summary>
        /// CTor
        /// </summary>
        public ServerClass()
        {
            mProjects = readFile();


        }

        public List<Project> GetProjectList()
        {
            return mProjects;
        }

        public string SendProjectList()
        {
            StringBuilder tmp = new StringBuilder();
            tmp.Append(mProjects[0].ToString());
            for (int i = 1; i < mProjects.Count; i++)
            {
                tmp.Append(SEPdata);
                tmp.Append(mProjects[i].ToString());
            }

            return tmp.ToString();
        }

        public bool ListChanged()
        {
            if (ProjectListHash != CalculateHash())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void UpdateProjectListHash()
        {
            ProjectListHash = CalculateHash();
        }

        public bool UpdateTime(Project project)
        {
            int i = FindProjectIndex(project);
            if (-1 != i)
            {
                mProjects[i].AddTime(project.MTimeEffortCurrent);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Reads a saved project file
        /// </summary>
        /// <returns>List of projects</returns>
        private List<Project> readFile()
        {
            List<Project> tmp = new List<Project>();
            FileInfo projects = new FileInfo(PROJECT_FILE);
            if (projects.Exists)
            {
                StreamReader sr = new StreamReader(projects.FullName);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    tmp.Add(new Project(line));
                }

                sr.Close();
            }
            else
            {
                projects.Create();
            }

            return tmp;
        }

        /// <summary>
        /// Loads the file again
        /// </summary>
        public void LoadFile()
        {
            mProjects.Clear();
            mProjects = readFile();
        }

        /// <summary>
        /// Saves the file
        /// </summary>
        public void SaveFile()
        {
            StreamWriter sw = new StreamWriter(PROJECT_FILE);
            foreach (Project project in mProjects)
            {
                sw.WriteLine(project.ToString());
            }
            sw.Close();
        }

        /// <summary>
        /// Calculates a hash value for the list
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private int CalculateHash()
        {
            string tmp = mProjects.Count.ToString();
            foreach (Project p in mProjects)
            {
                tmp = tmp + p.ProjectName + p.MTimeEffortCurrent.ToString() + p.MTimeEffortProjected.ToString();
            }

            return tmp.GetHashCode();
        }

        /// <summary>
        /// Finds a project in the list by its name
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        private int FindProjectIndex(Project reference)
        {
            foreach (Project p in mProjects)
            {
                if (p.ProjectName.Equals(reference.ProjectName))
                {
                    return mProjects.IndexOf(p);
                }
            }

            return -1;
        }

        public void AddProject(Project newProject)
        {
            mProjects.Add(newProject);
        }
    }
}
