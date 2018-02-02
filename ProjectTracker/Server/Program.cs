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
        private const int HEARTBEAT_DELAY = 5000;
        private const int SERVER_ID = -99;
        private static List<ClientData> mClients = new List<ClientData>();
        private static bool Run = true;

        private static ServerClass mServer;
        private static Updater mUpdater;

        /// <summary>
        /// Main method
        /// </summary>
        static void Main()
        {
            mServer = new ServerClass();
            OutputProjects(mServer.GetProjectList());

            ThreadPool.QueueUserWorkItem(new WaitCallback(ListenForClients));
            ThreadPool.QueueUserWorkItem(new WaitCallback(startUpdater));

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
        private static void startUpdater(Object stateInfo)
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
        static void ListenForClients(Object stateInfo)
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

                        switch (msgType)
                        {
                            case typeMessage.MSG_CONNECT:
                                ClientData connectClient = msgClass.ParseDataToClientData(msgType, msgData);
                                connectClient.Number = mClients.Count;
                                connectClient.Address = client_Address;
                                connectClient.Status = ClientStati.Connected;
                                mClients.Add(connectClient);
                                response = msgClass.ConnectResponse(connectClient, mServer.GetProjectListHash());
                                break;

                            case typeMessage.MSG_NEWPROJECT:
                                mServer.AddProject(new Project(msgData));
                                response = msgClass.NewProjectResponse(mServer.GetProjectListHash());
                                break;

                            case typeMessage.MSG_ADDTIME:
                                Project tmp = (msgClass.ParseDataToProjectList(typeMessage.MSG_ADDTIME, msgData))[0];
                                response = mServer.UpdateTime(tmp) ? msgClass.AddTimeResponse(mServer.GetProjectListHash()) : msgClass.AddTimeResponseError();
                                break;

                            case typeMessage.MSG_HEARTBEAT:
 
                                    response = msgClass.HeartBeatResponse(mServer.GetProjectListHash());

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
    }
}
