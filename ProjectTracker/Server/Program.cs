using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using MyLib;

namespace Server
{
    class Program
    {
        private const string PROJECT_FILE = @"c:\Temp\Projects.txt";
        private static List<Project> mProjects;

        static void Main()
        {
            mProjects = readFile();
            OutputProjects(mProjects);
            CreateThreads();

            Console.ReadKey();

        }

        private static List<Project> readFile()
        {
            FileInfo projects = new FileInfo(PROJECT_FILE);
            if (projects.Exists)
            {
                List<Project> tmp = new List<Project>();
                StreamReader sr = new StreamReader(projects.FullName);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    tmp.Add(new Project(line));
                }
                sr.Close();
                return tmp;
            }
            else
            {
                projects.Create();
                return new List<Project>();
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
                Console.WriteLine("CURRENT PROJECTS:");
                Console.WriteLine("====================");
                foreach (Project project in projects)
                {
                    Console.WriteLine(project.ToString());
                }
                Console.WriteLine("====================\n");
            }
        }

        private static void CreateThreads()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(ListenForClients));
            ThreadPool.QueueUserWorkItem(new WaitCallback(HeartBeat));

        }

        static void ListenForClients(Object stateInfo)
        {
            while (true)
            {
                Thread.Sleep(100);
                Console.WriteLine("Listen");
            }
        }
        static void HeartBeat(Object stateInfo)
        {
            while (true)
            {
                Thread.Sleep(1000);
                Console.WriteLine("HeartBeat");
            }
        }

    }
}
