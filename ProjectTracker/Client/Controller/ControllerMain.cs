//#define TestList

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
using System.IO;

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
        private const string FILEPATH = @"C:\temp\ProjectTrackerMessages.txt";
        private const int TIMEOUT = 1000; // in ms
        private bool MessageWaiting;
        private bool UpdateProjectList = false;
        private byte[] Message = null;
        private Stopwatch mStopWatch = new Stopwatch();
        private MyMessageClass msgClass;

        public ControllerMain(IModel modelMain, ViewMain viewMain, ViewAddProject viewAdd,
            ViewEvaluation viewEvaluation)
        {
            mModelMain = modelMain;
            mViewMain = viewMain;
            mViewAddProject = viewAdd;
            mViewEvaluation = viewEvaluation;
            mViewMain.setController(this);
            mViewAddProject.setController(this);
            mViewEvaluation.setController(this);

            ChangeStatus(ClientStati.NotConnected);
            msgClass = new MyMessageClass(mModelMain.MyData.Number);

            //CreateChannel();
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
            ViewAddProject addDialog = new ViewAddProject();
            addDialog.setController(this);
            if (addDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Project projectToAdd = new Project(addDialog.projectName, addDialog.projectedTime);
                QueueMessage(msgClass.NewProjectMessage(projectToAdd));
                mModelMain.mProjects.Add(projectToAdd);
                UpdateCombobox();
            }
        }

        public void CommitTime()
        {
            TimeSpan timeToAdd = mStopWatch.Elapsed;
            DialogResult dialogResult =
                MessageBox.Show(
                    "Do you want to commit " + timeToAdd + " [hours:minutes:seconds] to project " +
                    mModelMain.mCurrentProject.ProjectName, "Commit", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                QueueMessage(msgClass.AddTimeMessage(mModelMain.mCurrentProject, timeToAdd));
                mModelMain.mCurrentProject.AddTime(timeToAdd);
                UpdateCombobox();
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
            //MessageBox.Show(System.Text.Encoding.Default.GetString(message));
            // Send message
            if (mModelMain.MyData.Status == ClientStati.Connected && Message == null)
            {
                Message = message;
            }
            // Write to logfile
            else
            {
                try
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(FILEPATH, true))
                    {
                        file.WriteLine(System.Text.Encoding.Default.GetString(message));
                    }

                    MessageWaiting = true;
                }
                catch (Exception e)
                {
                    ShowError(e);
                }
            }
        }

        /// <summary>
        /// Creates the different threads.
        /// </summary>
        private void CreateThreads()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(SocketThread));
            ThreadPool.QueueUserWorkItem(new WaitCallback(WCFThread));
        }

        /// <summary>
        /// Thread for a Socket-Connection
        /// </summary>
        /// <param name="stateInfo"></param>
        void SocketThread(Object stateInfo)
        {
            while (true)
            {
                string receivedMsg;
                string msgHeader;
                string msgData;
                typeMessage msgType;
                ClientData tmp;

                switch (mModelMain.MyData.Status)
                {
                    case ClientStati.Connected:
                        try
                        {
                            // if addTime or addProject message is waiting
                            if (Message != null)
                            {
                                receivedMsg = SendToReceiveFromServer(Message);
                                Message = null;
                                if (receivedMsg != "")
                                {
                                    msgHeader = msgClass.getMessageHeader(receivedMsg);
                                    msgData = msgClass.getMessageData(receivedMsg);
                                    msgType = msgClass.getMessageType(msgHeader);
                                    switch (msgType)
                                    {
                                        case typeMessage.MSG_NEWPROJECT:
                                            CheckHashValue();
                                            break;
                                        case typeMessage.MSG_ADDTIME:
                                            CheckHashValue();
                                            break;
                                        case typeMessage.MSG_ADDTIMEERROR:
                                            // @Dominik: is da sunsd nu was zu tun? irgendwie drauf reagieren?
                                            // @Neuwirt: Eventuell in des LOG-File schreibn? und später nochmal probieren?
                                            throw new Exception("Adding time failed");
                                            break;

                                        default:
                                            throw new Exception("Wrong message type received");
                                    }
                                }
                            }
                            // message in file
                            else if (MessageWaiting)
                            {
                                if (File.Exists(FILEPATH))
                                    try
                                    {
                                        if (new FileInfo(FILEPATH).Length != 0)
                                        {
                                            List<string> lines = File.ReadAllLines(FILEPATH).ToList();
                                            if (lines.Count != 0)
                                            {
                                                Message = Encoding.ASCII.GetBytes(lines.First());
                                                lines.RemoveAt(0);
                                                File.WriteAllLines(FILEPATH, lines);
                                            }
                                        }
                                        else
                                            MessageWaiting = false;

                                    }
                                    catch
                                    {
                                        MessageWaiting = false;
                                        throw new Exception("Command file corrupted");
                                    }
                                else
                                {
                                    MessageWaiting = false;
                                    throw new Exception("Command file missing");
                                }
                            }
                            // else send heartbeat
                            else
                            {
                                byte[] thisData = msgClass.HeartBeatMessage(mModelMain.MyData);
                                receivedMsg = SendToReceiveFromServer(thisData);
                                msgHeader = msgClass.getMessageHeader(receivedMsg);
                                msgData = msgClass.getMessageData(receivedMsg);
                                msgType = msgClass.getMessageType(msgHeader);

                                if (msgType == typeMessage.MSG_HEARTBEATRESPONSE)
                                {
                                    mModelMain.MyData.LastHeartBeat = DateTime.Now;
                                    mModelMain.ServerProjectListHash = msgClass.ParseDataToHashValue(msgType, msgData);
                                    CheckHashValue();
                                }
                                else
                                {
                                    throw new Exception("Disconnected");
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            ChangeStatus(ClientStati.NotConnected);
                            ShowError(e);
                        }
                        break;

                    default:
                        byte[] dataToSend = msgClass.ConnectMessage(mModelMain.MyData);
                        try
                        {
                            receivedMsg = SendToReceiveFromServer(dataToSend);
                            if (receivedMsg != "")
                            {
                                msgHeader = msgClass.getMessageHeader(receivedMsg);
                                msgData = msgClass.getMessageData(receivedMsg);
                                msgType = msgClass.getMessageType(msgHeader);

                                if (msgType != typeMessage.MSG_CONNECTRESPONSE)
                                {
                                    throw new Exception("Wrong message type received");
                                }

                                tmp = msgClass.ParseDataToClientData(msgType, msgData);

                                if (tmp.Name.Equals(mModelMain.MyData.Name))
                                {
                                    mModelMain.MyData = tmp;
                                    ChangeStatus(ClientStati.Connected);
                                    mModelMain.MyData.LastHeartBeat = DateTime.Now;
                                    mModelMain.ServerProjectListHash = msgClass.ParseDataToHashValue(msgType, msgData);
                                    msgClass = new MyMessageClass(mModelMain.MyData.Number);
                                    CheckHashValue();
                                }
                                else
                                {
                                    throw new Exception("Wrong client number/name!");
                                }
                            }
                            else
                            {
                                ChangeStatus(ClientStati.NotConnected);
                            }
                        }
                        catch (Exception e)
                        {
                            ChangeStatus(ClientStati.NotConnected);
                            ShowError(e);
                        }
                        break;
                }
                Thread.Sleep(TIMEOUT);
            }
        }

        /// <summary>
        /// Thread for the remote call for getting a new list.
        /// </summary>
        /// <param name="stateinfo"></param>
        private void WCFThread(Object stateinfo)
        {
            while (true)
            {
                if (UpdateProjectList && mModelMain.MyData.Status == ClientStati.Connected)
                {
                    try
                    {
                        ChannelFactory<IRemoteUpdate> cFactory =
                            new ChannelFactory<IRemoteUpdate>("WSHttpBinding_IRemoteUpdate");
                        mRemoteUpdater = cFactory.CreateChannel();
                        string response = mRemoteUpdater.updateProjectListAsString();
                        if (!(String.IsNullOrEmpty(response)))
                        {
                            mModelMain.mProjects.Clear();
                            UpdateProjectList = false;
                            string[] parts = response.Split('#');
                            foreach (string part in parts)
                            {
                                mModelMain.mProjects.Add(new Project(part));
                            }
                            mModelMain.ClientProjectListHash = mModelMain.ServerProjectListHash;
                            UpdateCombobox();
                        }
                    }
                    catch (Exception e)
                    {
                        ShowError(e);
                        ChangeStatus(ClientStati.NotConnected);
                        throw;
                    }
                    (mRemoteUpdater as ICommunicationObject).Close();
                }
                Thread.Sleep(TIMEOUT);
            }
        }

        /// <summary>
        /// Sends a message and receive the response.
        /// </summary>
        /// <param name="dataToSend">Data to send as 'byte[]'</param>
        /// <returns>The received message as 'string'.</returns>
        private String SendToReceiveFromServer(byte[] dataToSend)
        {
            string receivedMessage = "";

            IPEndPoint server_Address = new IPEndPoint(IPAddress.Parse(SERVER_IP), SERVER_PORT);
            Socket server_TcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                server_TcpSocket.Connect(server_Address);
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

            }
            catch (SocketException e)
            {
                ChangeStatus(ClientStati.NotConnected);
            }
            return receivedMessage;
        }

        /// <summary>
        /// Checks the received hash value of the list with the currently saved value
        /// </summary>
        private void CheckHashValue()
        {
            if (mModelMain.ServerProjectListHash != mModelMain.ClientProjectListHash)
            {
                UpdateProjectList = true;
            }
        }

        /// <summary>
        /// Shows an error as message box.
        /// </summary>
        /// <param name="e"></param>
        private void ShowError(Exception e)
        {
            string msg = "Client: " + mModelMain.MyData.Number.ToString() + " (" + mModelMain.MyData.Name + ")\n" +
                         e.Message;
            MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Updates the status bar
        /// </summary>
        /// <param name="status"></param>
        private void ChangeStatus(ClientStati status)
        {
            mModelMain.MyData.Status = status;
            switch (status)
            {
                case ClientStati.Connected:
                    mViewMain.setStatus("Connected");
                    mViewMain.EnableDisableAdd(true);
                    break;
                case ClientStati.NotConnected:
                    mViewMain.setStatus("Not Connected");
                    mViewMain.EnableDisableAdd(false);
                    break;
                case ClientStati.Dead:
                    mViewMain.setStatus("Dead");
                    mViewMain.EnableDisableAdd(false);
                    break;
                case ClientStati.NotStarted:
                    mViewMain.setStatus("NotStarted");
                    mViewMain.EnableDisableAdd(false);
                    break;
                default:
                    mViewMain.setStatus("?");
                    mViewMain.EnableDisableAdd(false);
                    break;
            }
        }

        private void UpdateCombobox()
        {
            mViewMain.UpdateProjects(mModelMain.mProjects);
            mViewMain.Update();
        }
        
    }
}