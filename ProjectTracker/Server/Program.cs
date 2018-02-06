using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using MyLib;

namespace Server
{
    class Program
    {
        private const int SERVER_PORT = 9050;
        private const int SERVER_ID = -99;
        private static bool Run = true;
        private static bool ProjectListChanged = false;
        private const int TIMEOUT = 1000; // in ms

        private static ServerClass mServer;
        private static Updater mUpdater;

        /// <summary>
        /// Main method
        /// </summary>
        static void Main()
        {
            mServer = new ServerClass();
            OutputProjects(mServer.GetProjectList());

            ThreadPool.QueueUserWorkItem(new WaitCallback(SocketThread));
            ThreadPool.QueueUserWorkItem(new WaitCallback(WCFThread));
            ThreadPool.QueueUserWorkItem(new WaitCallback(CheckClients));
            ThreadPool.QueueUserWorkItem(new WaitCallback(SaveFile));

            DisplayCommands();
            while (Run)
            {
                string cmd = Console.ReadLine();
                //string cmd = "l";
                switch (cmd)
                {
                    case "l":
                        mServer.LoadFile();
                        break;
                    case "s":
                        mServer.SaveFile();
                        break;
                    case "se":
                        mServer.SaveFile();
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
                Console.WriteLine("CURRENT PROJECTS at the SERVER:");
                Console.WriteLine("========================================");
                foreach (Project project in projects)
                {
                    Console.WriteLine(project.ToString());
                }

                Console.WriteLine("========================================\n");
            }
        }

        /// <summary>
        /// Starts the WCF-Updater in a new thread.
        /// </summary>
        /// <param name="stateInfo"></param>
        private static void WCFThread(Object stateInfo)
        {
            while (mServer == null)
            {
            }

            mUpdater = new Updater(mServer);
            ServiceHost mValueExchangerServiceHost = new ServiceHost(mUpdater);
            mValueExchangerServiceHost.Open();
            while (Run)
            {
            }
            mValueExchangerServiceHost.Close();
        }

        /// <summary>
        /// Listens for calls from the clients.
        /// </summary>
        /// <param name="stateInfo"></param>
        static void SocketThread(Object stateInfo)
        {
            Socket server_TcpSocket = null;
            try
            {
                IPEndPoint server_Address = new IPEndPoint(IPAddress.Any, SERVER_PORT);
                server_TcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server_TcpSocket.Bind(server_Address);

                byte[] receivedData;
                int dataCount;
                string receivedMsg;
                typeMessage msgType;
                string msgData;
                string msgHeader;
                MyMessageClass msgClass = new MyMessageClass(SERVER_ID); //-99... Server
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
                        ClientData client;

                        switch (msgType)
                        {
                            case typeMessage.MSG_CONNECT:
                                client = msgClass.ParseDataToClientData(msgType, msgData);
                                client.Number = mServer.GetClientList().Count + 1;
                                client.Address = client_Address;
                                client.Status = ClientStati.Connected;
                                mServer.AddClient(client);
                                response = msgClass.ConnectResponse(client, mServer.GetProjectListHash());
                                Console.WriteLine("Client added!");
                                break;

                            case typeMessage.MSG_NEWPROJECT:
                                mServer.AddProject(new Project(msgData));
                                response = msgClass.NewProjectResponse(mServer.GetProjectListHash());
                                ProjectListChanged = true;
                                Console.WriteLine("Project added!");
                                break;

                            case typeMessage.MSG_ADDTIME:
                                Project tmp = msgClass.ParseDataToProject(typeMessage.MSG_ADDTIME, msgData);
                                response = mServer.UpdateTime(tmp)
                                    ? msgClass.AddTimeResponse(mServer.GetProjectListHash())
                                    : msgClass.AddTimeResponseError();
                                ProjectListChanged = true;
                                Console.WriteLine("Time added to project!");
                                break;

                            case typeMessage.MSG_HEARTBEAT:
                                client = msgClass.ParseDataToClientData(msgType, msgData);
                                int index = mServer.FindClientIndex(client);
                                if (index == -1)
                                {
                                    throw new ArgumentOutOfRangeException();
                                }

                                mServer.GetClientList()[index].LastHeartBeat = DateTime.Now;
                                response = msgClass.HeartBeatResponse(mServer.GetClientList()[index],
                                    mServer.GetProjectListHash());
                                Console.WriteLine("HeartBeat to Client: " + index);
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
            catch (Exception e)
            {
                Console.WriteLine("Error:\n" + e.Message);
            }
            finally
            {
                server_TcpSocket.Close();
            }
        }

        /// <summary>
        /// Checks every 30s if all clients have sent a heart beat
        /// </summary>
        /// <param name="stateInfo"></param>
        static void CheckClients(Object stateInfo)
        {
            Thread.Sleep(5 * TIMEOUT);
            DateTime timeThreshold;
            TimeSpan offset = new TimeSpan(0, 0, 0, 0, 2 * TIMEOUT);
            while (Run)
            {
                timeThreshold = DateTime.Now - offset;
                foreach (ClientData client in mServer.GetClientList())
                {
                    if (client.LastHeartBeat < timeThreshold)
                    {
                        mServer.RemoveClient(client);
                    }
                }
                Thread.Sleep(30 * TIMEOUT);
            }
        }

        /// <summary>
        /// Automatically saves the file every 5min
        /// </summary>
        /// <param name="stateInfo"></param>
        static void SaveFile(Object stateInfo)
        {
            Thread.Sleep(5 * TIMEOUT);
            DateTime lastSave = DateTime.Now;
            TimeSpan offset = new TimeSpan(0, 0, 0, 0, 300 * TIMEOUT);
            while (Run)
            {
                if (ProjectListChanged || DateTime.Now > lastSave + offset)
                {
                    mServer.SaveFile();
                }

                Thread.Sleep(300 * TIMEOUT);
            }
        }
    }
}
