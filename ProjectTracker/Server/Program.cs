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
        private static List<IPEndPoint> mClients = new List<IPEndPoint>();
        private static bool Run = true;

        private static ServerClass mServer;
        private static Updater mUpdater;

        /// <summary>
        /// Main method
        /// </summary>
        static void Main()
        {
            mServer = new ServerClass();
            //ThreadPool.QueueUserWorkItem(new WaitCallback(startServer));
            ThreadPool.QueueUserWorkItem(new WaitCallback(startUpdater));
                    
            OutputProjects(mServer.GetProjectList());

            ThreadPool.QueueUserWorkItem(new WaitCallback(ListenForClients));
            ThreadPool.QueueUserWorkItem(new WaitCallback(HeartBeat));

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
                            mServer.AddProject(new Project(msgData));
                            response = msgClass.NewProjectResponse();
                            break;

                        case typeMessage.MSG_ADDTIME:
                            Project tmp = (msgClass.ParseDataToProjectList(typeMessage.MSG_ADDTIME, msgData))[0];
                            if (mServer.UpdateTime(tmp))
                            {
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
                if(mServer.ListChanged())
                {
                    message = msgClass.UpdateMessage(mServer.GetProjectList());
                    mServer.UpdateProjectListHash();
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
