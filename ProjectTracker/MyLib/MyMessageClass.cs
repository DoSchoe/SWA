using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace MyLib
{
    public enum typeMessage { MSG_CONNECT = 1, MSG_NEWPROJECT = 2, MSG_ADDTIME = 3, MSG_UPDATE = 4, MSG_HEARTBEAT = 5, MSG_ADDTIMEERROR};
    public class MyMessageClass
    {
        #region Members
        public const int BUFFER_SIZE_BYTE = 256;
        public const int BFFER_SIZE_BIT = BUFFER_SIZE_BYTE * 8;
        public const char SEPmessage = '?';
        public const char SEPdata = '#';


        #endregion

        /// <summary>
        /// CTor
        /// </summary>
        public MyMessageClass()
        {
        }

        #region Message Methods
        /// <summary>
        /// Separates the message into the header and data
        /// </summary>
        /// <param name="message">The message as 'string'.</param>
        /// <returns>header = 'string[0]', data = 'string[1]'</returns>
        private string[] separateMessage(string message)
        {
            string[] tmp = message.Split(SEPmessage);
            if (2 != tmp.Length)
            {
                throw new Exception("Unkown format of the message!");
            }

            return tmp;
        }

        /// <summary>
        /// Returns the type of a message.
        /// </summary>
        /// <param name="messageHeader">The header of a message as 'string'.</param>
        /// <returns>The type of the message as 'typeMessage'.</returns>
        public typeMessage getMessageType(string messageHeader)
        {
            typeMessage type;
            type = convertMessageType(messageHeader);
            return type;
        }

        /// <summary>
        /// Converts the message type to a 'typeMessage'
        /// </summary>
        /// <param name="messageType">Message type as 'string'.</param>
        /// <returns>Message type as 'typeMessage'.</returns>
        private typeMessage convertMessageType(string messageType)
        {
            try
            {
                return (typeMessage)Convert.ToInt32(messageType);
            }
            catch
            {
                throw new Exception("Unkown Message type!");
            }
        }
        #endregion

        #region Message header
        /// <summary>
        /// Returns the header of a message.
        /// </summary>
        /// <param name="message">The message as 'string'.</param>
        /// <returns>The header of the message as 'string'.</returns>
        public String getMessageHeader(string message)
        {
            string[] msgParts;
            msgParts = separateMessage(message);
            return msgParts[0];
        }

        /// <summary>
        /// Creates the header of the message.
        /// </summary>
        /// <param name="type">Type of the message.</param>
        /// <returns>The header as 'string'.</returns>
        private string createMsgHeader(typeMessage type)
        {
            return ((int)type).ToString();
        }
        #endregion

        #region Message data
        /// <summary>
        /// Returns the data of the message
        /// </summary>
        /// <param name="message">The message as 'string'</param>
        /// <returns>The data as 'string'</returns>
        public String getMessageData(string message)
        {
            string[] msgParts;
            msgParts = separateMessage(message);
            return msgParts[1];
        }

        /// <summary>
        /// Creates the message data string.
        /// </summary>
        /// <param name="client">Client parameter as 'ClientData'.</param>
        /// <returns>The data of the message as 'string'.</returns>
        private string createMsgData(List<Project> data)
        {
            StringBuilder tmp = new StringBuilder();
            tmp.Append(data[0].ToString());
            for (int i = 1; i < data.Count; i++)
            {
                tmp.Append(SEPdata);
                tmp.Append(data[i].ToString());
            }

            return tmp.ToString();
        }

        public List<Project> ParseDataToProjectList(typeMessage type, string data)
        {
            string[] parts;
            switch (type)
            {
                case typeMessage.MSG_CONNECT:
                    return new List<Project>();
                    break;
                case typeMessage.MSG_NEWPROJECT:
                    return new List<Project>();
                    break;
                case typeMessage.MSG_ADDTIME:
                    parts = data.Split(SEPdata);
                    return new List<Project>() {new Project(parts[0], parts[1])};
                    break;
                case typeMessage.MSG_UPDATE:
                    parts = data.Split(SEPdata);
                    List<Project> tmp = new List<Project>();
                    foreach (string part in parts)
                    {
                        tmp.Add(new Project(part));
                    }
                    return tmp;
                    break;
                case typeMessage.MSG_HEARTBEAT:
                    return new List<Project>();
                    break;
                default:
                    return new List<Project>();
                    break;
            }
        }
        #endregion


        #region Connection message
        /// <summary>
        /// Creates a connect message.
        /// </summary>
        /// <returns>The connection message as 'byte[]'.</returns>
        public byte[] ConnectMessage()
        {
            byte[] tmp = new byte[BUFFER_SIZE_BYTE];
            string msgHeader = createMsgHeader(typeMessage.MSG_CONNECT);
            string msgData = "";
            string msg = msgHeader + SEPmessage + msgData;
            tmp = Encoding.ASCII.GetBytes(msg);
            return tmp;
        }
        
        /// <summary>
        /// Creates the response for a connect message.
        /// </summary>
        /// <returns></returns>
        public byte[] ConnectResponse()
        {
            byte[] tmp = new byte[BUFFER_SIZE_BYTE];
            string msgHeader = createMsgHeader(typeMessage.MSG_CONNECT);
            string msg = msgHeader + SEPmessage + "Connected";
            tmp = Encoding.ASCII.GetBytes(msg);
            return tmp;
        }

        /// <summary>
        /// Creates a message for adding a new project
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] NewProjectMessage(Project data)
        {
            byte[] tmp = new byte[BUFFER_SIZE_BYTE];
            string msgHeader = createMsgHeader(typeMessage.MSG_NEWPROJECT);
            string msgData = data.ToString();
            string msg = msgHeader + SEPmessage + msgData;
            tmp = Encoding.ASCII.GetBytes(msg);
            return tmp;
        }

        /// <summary>
        /// Creates the response for a NewProject-message
        /// </summary>
        /// <returns></returns>
        public byte[] NewProjectResponse()
        {
            byte[] tmp = new byte[BUFFER_SIZE_BYTE];
            string msgHeader = createMsgHeader(typeMessage.MSG_NEWPROJECT);
            string msg = msgHeader + SEPmessage + "NewProjectAdded";
            tmp = Encoding.ASCII.GetBytes(msg);
            return tmp;
        }

        /// <summary>
        /// Creates a message for adding time to a project
        /// </summary>
        /// <param name="data"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public byte[] AddTimeMessage(Project data, TimeSpan time)
        {
            byte[] tmp = new byte[BUFFER_SIZE_BYTE];
            string msgHeader = createMsgHeader(typeMessage.MSG_ADDTIME);
            string msgData = data.ProjectName + SEPdata + time.ToString();
            string msg = msgHeader + SEPmessage + msgData;
            tmp = Encoding.ASCII.GetBytes(msg);
            return tmp;
        }

        /// <summary>
        /// Creates the response for a AddTime-message
        /// </summary>
        /// <returns></returns>
        public byte[] AddTimeResponse()
        {
            byte[] tmp = new byte[BUFFER_SIZE_BYTE];
            string msgHeader = createMsgHeader(typeMessage.MSG_ADDTIME);
            string msg = msgHeader + SEPmessage + "TimeAdded";
            tmp = Encoding.ASCII.GetBytes(msg);
            return tmp;
        }

        /// <summary>
        /// Creates a message if adding time to a project was failed.
        /// </summary>
        /// <returns></returns>
        public byte[] AddTimeResponseError()
        {
            byte[] tmp = new byte[BUFFER_SIZE_BYTE];
            string msgHeader = createMsgHeader(typeMessage.MSG_ADDTIMEERROR);
            string msg = msgHeader + SEPmessage + "ERROR";
            tmp = Encoding.ASCII.GetBytes(msg);
            return tmp;
        }

        /// <summary>
        /// Creates a HeartBeat-message
        /// </summary>
        /// <returns></returns>
        public byte[] HeartBeatMessage()
        {
            byte[] tmp = new byte[BUFFER_SIZE_BYTE];
            string msgHeader = createMsgHeader(typeMessage.MSG_HEARTBEAT);
            string msg = msgHeader + SEPmessage + "HeartBeat";
            tmp = Encoding.ASCII.GetBytes(msg);
            return tmp;
        }

        /// <summary>
        /// Creates a message for updating the project list.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] UpdateMessage(List<Project> data)
        {
            byte[] tmp = new byte[BUFFER_SIZE_BYTE];
            string msgHeader = createMsgHeader(typeMessage.MSG_UPDATE);
            string msgData = createMsgData(data);
            string msg = msgHeader + SEPmessage + msgData;
            tmp = Encoding.ASCII.GetBytes(msg);
            return tmp;
        }
        #endregion
    }
}
