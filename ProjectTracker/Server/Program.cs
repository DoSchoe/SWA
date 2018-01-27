using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using MyLib;

namespace Server
{
    delegate void Test();

    class Program
    {
        private const string PROJECT_FILE = @"c:\Temp\Projects.txt";
        private const int SERVER_PORT = 9050;
        private const int HEARTBEAT_DELAY = 5000;
        private static List<Project> mProjects;
        private static List<IPEndPoint> mClients;
        private static bool Run = true;
        private static int ProjectListHash = 0;

        /// <summary>
        /// Main method
        /// </summary>
        static void Main()
        {
            mProjects = readFile();
            OutputProjects(mProjects);
            mClients = new List<IPEndPoint>();
            CreateThreads();

            DisplayCommands();
            while (Run)
            {
                string cmd = Console.ReadLine();
                //string cmd = "l";
                switch (cmd)
                {
                    case "l":
                        loadFile();
                        break;
                    case "s":
                        saveFile();
                        break;
                    case "se":
                        saveFile();
                        Run = false;
                        break;
                    case "e":
                        Run = false;
                        break;
                    default:
                        Console.WriteLine("Wrong command!");
                        DisplayCommands();
                        break;
                }
            }
        }

        /// <summary>
        /// Displays the possible commands
        /// </summary>
        private static void DisplayCommands()
        {
            Console.WriteLine("The folling commands a possible:");
            Console.WriteLine("l  ...Reload the file. You will loose all unsaved changes.");
            Console.WriteLine("s  ...Save the file.");
            Console.WriteLine("se ...Save the file and close the application.");
            Console.WriteLine("e  ...Close the application.");
        }

        /// <summary>
        /// Reads a saved project file
        /// </summary>
        /// <returns>List of projects</returns>
        private static List<Project> readFile()
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
        private static void loadFile()
        {
            mProjects.Clear();
            mProjects = readFile();
        }

        /// <summary>
        /// Saves the file
        /// </summary>
        private static void saveFile()
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
        private static int CalculateHash(List<Project> list)
        {
            string tmp = list.Count.ToString();
            foreach (Project p in list)
            {
                tmp = tmp + p.ProjectName + p.MTimeEffortCurrent.ToString() + p.MTimeEffortProjected.ToString();
            }

            return tmp.GetHashCode();
        }

        /// <summary>
        /// Displays all projects on the console
        /// </summary>
        /// <param name="projects"></param>
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

        /// <summary>
        /// Finds a project in the list by its name
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        private static int FindProjectIndex(Project reference)
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
        /// Creates the different threads.
        /// </summary>
        private static void CreateThreads()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(ListenForClients));
            ThreadPool.QueueUserWorkItem(new WaitCallback(HeartBeat));
        }

        /// <summary>
        /// Listens for calls from the clients.
        /// </summary>
        /// <param name="stateInfo"></param>
        static void ListenForClients(Object stateInfo)
        {
            IPEndPoint server_Address = new IPEndPoint(IPAddress.Any, SERVER_PORT);
            Socket server_TcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server_TcpSocket.Bind(server_Address);

            byte[] receivedData;
            int dataCount;
            string receivedMsg;
            typeMessage msgType;
            string msgData;
            string msgHeader;
            MyMessageClass msgClass = new MyMessageClass(); //-1... Server
            byte[] response = new byte[MyMessageClass.BUFFER_SIZE_BYTE];

            server_TcpSocket.Listen(10);

            while (Run)
            {
                //Blocking call for accepting requests.
                Socket client_TcpSocket = server_TcpSocket.Accept();

                //Address of the client:
                IPEndPoint client_Address = (IPEndPoint) client_TcpSocket.RemoteEndPoint;

                receivedData = new byte[MyMessageClass.BUFFER_SIZE_BYTE];
                dataCount = client_TcpSocket.Receive(receivedData);
                if (dataCount != 0)
                {
                    receivedMsg = Encoding.ASCII.GetString(receivedData, 0, dataCount);
                    msgHeader = msgClass.getMessageHeader(receivedMsg);
                    msgData = msgClass.getMessageData(receivedMsg);
                    msgType = msgClass.getMessageType(msgHeader);

                    switch (msgType)
                    {
                        case typeMessage.MSG_CONNECT:
                            if (!mClients.Contains(client_Address))
                            {
                                mClients.Add(client_Address);
                            }

                            response = msgClass.ConnectResponse();
                            break;

                        case typeMessage.MSG_NEWPROJECT:
                            mProjects.Add(new Project(msgData));
                            response = msgClass.NewProjectResponse();
                            break;

                        case typeMessage.MSG_ADDTIME:
                            Project tmp = (msgClass.ParseDataToProjectList(typeMessage.MSG_ADDTIME, msgData))[0];
                            int i = FindProjectIndex(tmp);
                            if (-1 == i)
                            {
                                mProjects[i].AddTime(tmp.MTimeEffortCurrent);
                                response = msgClass.AddTimeResponse();
                            }
                            else
                            {
                                response = msgClass.AddTimeResponseError();
                            }

                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    // send response
                    client_TcpSocket.Send(response, response.Length, SocketFlags.None);
                }

                client_TcpSocket.Close();
            }
        }

        /// <summary>
        /// Sends an Update of the list or an heart beat in the defined intervall
        /// </summary>
        /// <param name="stateInfo"></param>
        static void HeartBeat(Object stateInfo)
        {
            while (Run)
            {
                // message same for all
                MyMessageClass msgClass = new MyMessageClass();
                byte[] message;
                if (ProjectListHash != CalculateHash(mProjects))
                {
                    message = msgClass.UpdateMessage(mProjects);
                    ProjectListHash = CalculateHash(mProjects);
                }
                else
                {
                    message = msgClass.HeartBeatMessage();
                }

                foreach (IPEndPoint client in mClients)
                {
                    IPEndPoint client_Address = new IPEndPoint(IPAddress.Parse(client.Address.ToString()), client.Port);
                    Socket client_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    try
                    {
                        client_Socket.Connect(client_Address);
                    }
                    catch (SocketException e)
                    {
                        Console.WriteLine("Server: unable to connect to client for update/heart-beat!\n" + e.Message);
                        continue;
                    }

                    client_Socket.Send(message);
                    client_Socket.Shutdown(SocketShutdown.Both);
                    client_Socket.Close();
                }

                //Console.WriteLine("HeartBeat");
                Thread.Sleep(HEARTBEAT_DELAY);
            }
        }
    }
}
