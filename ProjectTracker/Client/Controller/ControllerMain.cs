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
        private const long TIMEOUT = 1000;
        private bool MessageWaiting;
        private bool Connected = false;
        private byte[] Message;
        private Stopwatch mStopWatch = new Stopwatch();
        private MyMessageClass msgClass = new MyMessageClass();

        public ControllerMain(IModel modelMain, ViewMain viewMain, ViewAddProject viewAdd, ViewEvaluation viewEvaluation)
        {
            mModelMain = modelMain;
            mViewMain = viewMain;
            mViewAddProject = viewAdd;
            mViewEvaluation = viewEvaluation;
            mModelMain.mProjects = null;
            mViewMain.setController(this);
            mViewAddProject.setController(this);
            mViewEvaluation.setController(this);
            CreateChannel();
            CreateThreads();
#region TestInit
            List<Project> NewList = new List<Project>();
            Project projectToAdd1 = new Project("12345", new TimeSpan(1, 0, 0));
            Project projectToAdd2 = new Project("23456", new TimeSpan(2, 0, 0));
            Project projectToAdd3 = new Project("34567", new TimeSpan(3, 0, 0));
            NewList.Add(projectToAdd1);
            NewList.Add(projectToAdd2);
            NewList.Add(projectToAdd3);
            mModelMain.mProjects = NewList;
            mViewMain.UpdateProjects(mModelMain.mProjects);
#endregion
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
            if (Connected)
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
                // Connect to the service by using channel
                ChannelFactory<IRemoteUpdate> cFactory = new ChannelFactory<IRemoteUpdate>("WSHttpBinding_IRemoteUpdate");
                mRemoteUpdater = cFactory.CreateChannel();
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
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
            MyMessageClass msgClass = new MyMessageClass();
            IPEndPoint serverAddress = new IPEndPoint(IPAddress.Any, SERVER_PORT);
            Socket server_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            while (true)
            {
                if (MessageWaiting)
                {
                    // message same for all
                    try
                    {
                        server_Socket.Connect(serverAddress);
                        server_Socket.Send(Message);
                        Message = null;
                        MessageWaiting = false;
                        server_Socket.Shutdown(SocketShutdown.Both);
                        server_Socket.Close();
                    }
                    catch (SocketException e)
                    {
                        Console.WriteLine("Client: unable to connect to server!\n" + e.Message);
                    }
                }
                Thread.Sleep(100);
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
            MyMessageClass msgClass = new MyMessageClass();
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
            mViewMain.UpdateProjects(mModelMain.mProjects);
        }

    }
}