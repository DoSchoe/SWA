#define TestList

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using Client.Model;
using Client.Views;
using MyLib;

namespace Client.Controller
{

    class ControllerMain : IController
    {
        private IModel mModelMain;
        private IRemoteUpdate mRemoteUpdater;
        private ViewMain mViewMain;
        private ViewAddProject mViewAddProject;
        private ViewEvaluation mViewEvaluation;
        private const int SERVER_PORT = 9050;
        private const string SERVER_IP = "127.0.0.1";
        private const long TIMEOUT = 1000;
        private bool MessageWaiting;
        private bool Connected = false;
        private byte[] Message;
        private Stopwatch mStopWatch = new Stopwatch();
        private MyMessageClass msgClass;

        public ControllerMain(IModel modelMain, ViewMain viewMain, ViewAddProject viewAdd, ViewEvaluation viewEvaluation)
        {
            mModelMain = modelMain;
            mViewMain = viewMain;
            mViewAddProject = viewAdd;
            mViewEvaluation = viewEvaluation;
            mViewMain.setController(this);
            mViewAddProject.setController(this);
            mViewEvaluation.setController(this);

            if (mModelMain.MyData.Status != ClientStati.Connected)
            {
                ConnectToServer();
            }

            CreateChannel();
            CreateThreads();

#if TestList
            mModelMain.mProjects = new List<Project>()
            {
                new Project("12345", new TimeSpan(1, 0, 0)),
                new Project("23456", new TimeSpan(2, 0, 0)),
                new Project("34567", new TimeSpan(3, 0, 0))
            };
            mViewMain.UpdateProjects(mModelMain.mProjects);
#endif
        }

        public void AddProject()
        {
            //ViewAddProject addDialog = new ViewAddProject();
            if (mViewAddProject.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Project projectToAdd = new Project(mViewAddProject.projectName, mViewAddProject.projectedTime);
                mModelMain.mProjects.Add(projectToAdd);
                mViewMain.UpdateProjects(mModelMain.mProjects);
                QueueMessage(msgClass.NewProjectMessage(projectToAdd));
            }
        }

        public void StartStopwatch()
        {
            mStopWatch.Reset();
            mStopWatch.Start();
        }

        public void StopStopwatch()
        {
            mStopWatch.Stop();
        }

        public void CommitTime()
        {
            TimeSpan timeToAdd = mStopWatch.Elapsed;
            DialogResult dialogResult = MessageBox.Show("Do you want to commit " + timeToAdd + " [hours:minutes:seconds] to project " + mModelMain.mCurrentProject.ProjectName, "Commit", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                mModelMain.mCurrentProject.AddTime(timeToAdd);
                mViewMain.UpdateProjects(mModelMain.mProjects);
                QueueMessage(msgClass.AddTimeMessage(mModelMain.mCurrentProject, timeToAdd));
            }
        }

        public void Evaluate()
        {
            ViewEvaluation formEvaluation = new ViewEvaluation();
            formEvaluation.Show();
            formEvaluation.displayEvaluation(mModelMain.mCurrentProject);
        }

        public void SetCurrentProject(Project project)
        {
            mModelMain.mCurrentProject = project;
        }

        public List<Project> GetProjects()
        {
            return mModelMain.mProjects;
        }

        private void QueueMessage(byte[] message)
        {
            MessageBox.Show(System.Text.Encoding.Default.GetString(message));
            if (mModelMain.MyData.Status == ClientStati.Connected)
            {
                Message = message;
                MessageWaiting = true;
            }
            else
            {
                //write to logfile
            }
        }

        private void CreateChannel()
        {
            try
            {
                ChannelFactory<IRemoteUpdate> cFactory = new ChannelFactory<IRemoteUpdate>("WSHttpBinding_IRemoteUpdate");
                mRemoteUpdater = cFactory.CreateChannel();
            }
            catch (Exception e)
            {
                string msg = "Client: " + mModelMain.MyData.Number.ToString() + " (" + mModelMain.MyData.Name + ")\n" +
                             e.Message;
                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        /// <summary>
        /// Creates the different threads.
        /// </summary>
        private void CreateThreads()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(ListenForHeartbeat));
            ThreadPool.QueueUserWorkItem(new WaitCallback(SendMessages));
        }

        /// <summary>
        /// Sends an Messages
        /// </summary>
        /// <param name="stateInfo"></param>
        void SendMessages(Object stateInfo)
        {
            MyMessageClass msgClass = new MyMessageClass(mModelMain.MyData.Number);
            string receivedMsg;
            string msgHeader;
            string msgData;
            int hash;
            typeMessage msgType;
            ClientData tmp;
            while (true)
            {
                try
                {
                    // if addTime or addProject message is waiting
                    if (MessageWaiting)
                    {
                        receivedMsg = sendToReceiveFromServer(Message);
                        Message = null;
                        MessageWaiting = false;

                        msgHeader = msgClass.getMessageHeader(receivedMsg);
                        msgData = msgClass.getMessageData(receivedMsg);
                        msgType = msgClass.getMessageType(msgHeader);

                        //tbd
                        switch (msgType)
                        {
                            case typeMessage.MSG_NEWPROJECT:
                                if (!CheckHashValue(msgData))
                                    throw new Exception("Unexpected hash value");
                                break;
                            case typeMessage.MSG_ADDTIME:
                                if (!CheckHashValue(msgData))
                                    throw new Exception("Unexpected hash value");
                                break;
                            case typeMessage.MSG_ADDTIMEERROR:
                                throw new Exception("Add time failed");
                                break;

                            default:
                                throw new Exception("Wrong message type");
                        }
                    }
                    // else send heartbeat
                    else
                    {
                        receivedMsg = sendToReceiveFromServer(msgClass.HeartBeatMessage(mModelMain.MyData, mModelMain.ProjectListHash));

                        msgHeader = msgClass.getMessageHeader(receivedMsg);
                        msgData = msgClass.getMessageData(receivedMsg);
                        msgType = msgClass.getMessageType(msgHeader);

                        if (msgType == typeMessage.MSG_HEARTBEAT)
                        {
                            // @Dominik: stimmt des parsen der heatbeatResponse so?
                            if (!CheckHashValue(msgData))
                                throw new Exception("Unexpected hash value");
                        }
                        else
                            throw new Exception("Wrong message type");
                    }
                }
                catch (SocketException e)
                {
                    string msg = "Client: " + mModelMain.MyData.Number.ToString() + " (" + mModelMain.MyData.Name + ")\n" +
                                 e.Message;
                    MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Listens for calls from the clients.
        /// </summary>
        /// <param name="stateInfo"></param>
        void ListenForHeartbeat(Object stateInfo)
        {
            byte[] receivedData;
            int dataCount;
            string receivedMsg;
            typeMessage msgType;
            string msgData;
            string msgHeader;
            Stopwatch mHeartbeat = new Stopwatch();
            mHeartbeat.Start();
            MyMessageClass msgClass = new MyMessageClass(mModelMain.MyData.Number);
            IPEndPoint client_Address = new IPEndPoint(IPAddress.Any, SERVER_PORT);
            Socket client_TcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client_TcpSocket.Bind(client_Address);
            client_TcpSocket.Listen(10);
            while (true)
            {
                Socket server_TcpSocket = client_TcpSocket.Accept();

                receivedData = new byte[MyMessageClass.BUFFER_SIZE_BYTE];
                dataCount = server_TcpSocket.Receive(receivedData);
                if (dataCount != 0)
                {
                    receivedMsg = Encoding.ASCII.GetString(receivedData, 0, dataCount);
                    msgData = msgClass.getMessageData(receivedMsg);
                    msgHeader = msgClass.getMessageHeader(receivedMsg);
                    msgType = msgClass.getMessageType(msgHeader);

                    if (msgType == typeMessage.MSG_HEARTBEAT)
                    {
                        mHeartbeat.Restart();
                        Connected = true;
                    }
                    else if (msgType == typeMessage.MSG_UPDATE)
                    {
                        mHeartbeat.Restart();
                        Connected = true;
                        UpdateProjects();
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                }
                server_TcpSocket.Close();
                if (mHeartbeat.ElapsedMilliseconds > TIMEOUT)
                    Connected = false;
                Thread.Sleep(100);
            }
        }

        private void UpdateProjects()
        {
            mModelMain.mProjects = mRemoteUpdater.updatedProjectList();
            // @Dominik: do muas i nu in Hashwert berechnen, oda?
            mViewMain.UpdateProjects(mModelMain.mProjects);
        }

        private void ConnectToServer()
        {
            MyMessageClass msgClass = new MyMessageClass(mModelMain.MyData.Number);
            byte[] dataToSend = msgClass.ConnectMessage(mModelMain.MyData, mModelMain.ProjectListHash);
            string receivedMsg;
            string msgHeader;
            string msgData;
            typeMessage msgType;
            ClientData tmp;
            try
            {
                receivedMsg = sendToReceiveFromServer(dataToSend);

                msgHeader = msgClass.getMessageHeader(receivedMsg);
                msgData = msgClass.getMessageData(receivedMsg);
                msgType = msgClass.getMessageType(msgHeader);

                if (msgType != typeMessage.MSG_CONNECT)
                {
                    throw new Exception("Wrong message type");
                }

                tmp = msgClass.ParseDataToClientData(msgType, msgData);

                if (tmp != mModelMain.MyData)
                {
                    throw new Exception("Wrong client number/name!");
                }

                mModelMain.MyData = tmp;
                mModelMain.MyData.Status = ClientStati.Connected;
                //Update Statusbar
            }
            catch (Exception e)
            {
                string msg = "Client: " + mModelMain.MyData.Number.ToString() + " (" + mModelMain.MyData.Name + ")\n" +
                             e.Message;
                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sends a message and receive the response.
        /// </summary>
        /// <param name="dataToSend">Data to send as 'byte[]'</param>
        /// <returns>The received message as 'string'.</returns>
        private String sendToReceiveFromServer(byte[] dataToSend)
        {
            string receivedMessage = "";

            IPEndPoint server_Address = new IPEndPoint(IPAddress.Parse(SERVER_IP), SERVER_PORT);
            Socket server_TcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                server_TcpSocket.Connect(server_Address);
            }
            catch (SocketException e)
            {
                throw new Exception("Unable to Connect to server!\n(" + e.Message + ")");
            }
            server_TcpSocket.Send(dataToSend);

            // receive response and convert to string
            int dataCount;
            byte[] receivedData = new byte[MyMessageClass.BUFFER_SIZE_BYTE];

            dataCount = server_TcpSocket.Receive(receivedData);

            if (dataCount != 0)
            {
                receivedMessage = Encoding.ASCII.GetString(receivedData, 0, dataCount);
                server_TcpSocket.Shutdown(SocketShutdown.Both); // only one side does this
                server_TcpSocket.Close();
            }

            return receivedMessage;
        }


        private bool CheckHashValue(string hashMsg)
        {
            int hash;
            if (Int32.TryParse(hashMsg, out hash))
            {
                if (hash != mModelMain.ProjectListHash)
                    UpdateProjects();
                return true;
            }
            else
                return false;
        }
    }
}