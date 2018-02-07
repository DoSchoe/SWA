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
        #region Members
        private string PROJECT_FILE = Directory.GetCurrentDirectory()+ @"\"+"Projects.txt";
        private int SERVER_PORT = 9050;
        private int HEARTBEAT_DELAY = 5000;
        public const char SEPdata = '#';
        private List<Project> mProjects;
        private static List<ClientData> mClients;
        private bool Run = true;
        private int ProjectListHash = 0;
        private int mClientNumber;

        public int ClientNumber
        {
            get
            {
                return mClientNumber;
            }

            private set
            {
                mClientNumber = value;
            }
        }
        #endregion

        /// <summary>
        /// CTor
        /// </summary>
        public ServerClass()
        {
            mProjects = readFile();
            UpdateProjectListHash();
            mClients = new List<ClientData>();
            mClientNumber = 1;
        }

        #region File methods
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
                if (projects.Length != 0)
                {
                    StreamReader sr = new StreamReader(projects.FullName);
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        tmp.Add(new Project(line));
                    }

                    sr.Close();
                }
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
            UpdateProjectListHash();
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
        #endregion

        #region Project-List methods
        /// <summary>
        /// Returns the current project list
        /// </summary>
        /// <returns></returns>
        public List<Project> GetProjectList()
        {
            return mProjects;
        }

        /// <summary>
        /// Sends the whole project list as string
        /// </summary>
        /// <returns></returns>
        public string SendProjectList()
        {
            if (mProjects.Count == 0)
            {
                return "";
            }
            else
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
        }

        /// <summary>
        /// Checks if the project list was changed
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Returns the hash-value of the project list
        /// </summary>
        /// <returns></returns>
        public int GetProjectListHash()
        {
            return ProjectListHash;
        }

        /// <summary>
        /// Updates the hash-value of the project list
        /// </summary>
        public void UpdateProjectListHash()
        {
            ProjectListHash = CalculateHash();
        }
        /// <summary>
        /// Updates the time of a project
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public bool UpdateTime(Project project)
        {
            int i = FindProjectIndex(project);
            if (-1 != i)
            {
                mProjects[i].AddTime(project.MTimeEffortCurrent);
                UpdateProjectListHash();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Calculates a hash value for the project list
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

        /// <summary>
        /// Adds a project
        /// </summary>
        /// <param name="newProject"></param>
        public void AddProject(Project newProject)
        {
            mProjects.Add(newProject);
            UpdateProjectListHash();
        }
        #endregion

        #region Client-List methods
        /// <summary>
        /// Finds the index of an client
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public int FindClientIndex(ClientData reference)
        {
            foreach (ClientData c in mClients)
            {
                if (c.Address.Equals(reference.Address) && c.Name.Equals(reference.Name) && c.Number.Equals(reference.Number))
                {
                    return mClients.IndexOf(c);
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns the current client list
        /// </summary>
        /// <returns></returns>
        public List<ClientData> GetClientList()
        {
            return mClients;
        }

        /// <summary>
        /// Adds a client
        /// </summary>
        /// <param name="newClient"></param>
        public void AddClient(ClientData newClient)
        {
            mClients.Add(newClient);
            mClientNumber = FindClientNumber();
        }

        /// <summary>
        /// Removes a client
        /// </summary>
        /// <param name="oldClient"></param>
        public void RemoveClient(ClientData oldClient)
        {
            mClients.Remove(oldClient);
        }

        private int FindClientNumber()
        {
            bool stop = false;
            List<int> clientnumbers = (from c in mClients select c.Number).ToList();
            int tmp = 1;
            while (!stop)
            {
                if (clientnumbers.Contains(tmp))
                {
                    tmp++;
                    if (tmp == Int32.MaxValue - 1)
                    {
                        stop = true;
                    }
                }
                else
                {
                    return tmp;
                }
            }

            return -1;
        }
        #endregion
    }
}
