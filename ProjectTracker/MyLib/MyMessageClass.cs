using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace MyLib
{
    public enum typeMessage
    {
        MSG_CONNECT = 1,
        MSG_CONNECTRESPONSE = 11,
        MSG_NEWPROJECT = 2,
        MSG_ADDTIME = 3, MSG_UPDATE = 4,
        MSG_HEARTBEAT = 5,
        MSG_HEARTBEATRESPONSE = 51,
        MSG_DISCONNECT = 6,
        MSG_ADDTIMEERROR = 99
    };
    public class MyMessageClass
    {
        #region Members
        public const int BUFFER_SIZE_BYTE = 256;
        public const int BFFER_SIZE_BIT = BUFFER_SIZE_BYTE * 8;
        public const char SEPmessage = '?';
        public const char SEPdata = '#';
        private const char TIMESPANSEPARATOR = ':';

        private int myID;
        #endregion

        /// <summary>
        /// CTor
        /// </summary>
        public MyMessageClass(int id)
        {
            myID = id;
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
            string[] parts = messageHeader.Split(SEPdata);
            type = convertMessageType(parts[0]);
            return type;
        }

        /// <summary>
        /// Returns the ID of the client.
        /// </summary>
        /// <param name="messageHeader">The header of a message as 'string'.</param>
        /// <returns>The ID of the client as 'int'.</returns>
        public int getClientID(string messageHeader)
        {
            string[] parts = messageHeader.Split(SEPdata);
            return Convert.ToInt32(parts[1]);
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
            return ((int)type).ToString() + SEPdata + myID.ToString();
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

        public Project ParseDataToProject(typeMessage type, string data)
        {
            string[] parts;
            switch (type)
            {
                case typeMessage.MSG_ADDTIME:
                    parts = data.Split(SEPdata);
                    return new Project(parts[0], parts[1]);
                    break;
                default:
                    return null;
                    break;
            }
        }

        public ClientData ParseDataToClientData(typeMessage type, string data)
        {
            string[] parts;
            ClientData tmp;
            string address;
            int port;
            IPEndPoint clientAddress;
            switch (type)
            {
                case typeMessage.MSG_CONNECT:
                    parts = data.Split(SEPdata);    
                    tmp = new ClientData(parts[1], (ClientStati)Convert.ToInt32(parts[2]));
                    return tmp;
                    break;
                case typeMessage.MSG_CONNECTRESPONSE:                    
                    parts = data.Split(SEPdata);
                    address = parts[3].Split(':')[0];
                    port = Convert.ToInt32(parts[3].Split(':')[1]);
                    clientAddress = new IPEndPoint(IPAddress.Parse(address), port);
                    tmp = new ClientData(parts[1], Convert.ToInt32(parts[0]), (ClientStati) Convert.ToInt32(parts[2]), clientAddress);
                    return tmp;
                    break;
                case typeMessage.MSG_HEARTBEAT:
                    parts = data.Split(SEPdata);
                    address = parts[3].Split(':')[0];
                    port = Convert.ToInt32(parts[3].Split(':')[1]);
                    clientAddress = new IPEndPoint(IPAddress.Parse(address), port);
                    tmp = new ClientData(parts[1], Convert.ToInt32(parts[0]), (ClientStati) Convert.ToInt32(parts[2]), clientAddress);
                    return tmp;
                    break;
                default:
                    return null;
                    break;
            }
        }

        public int ParseDataToHashValue(typeMessage type, string data)
        {
            string[] parts;
            switch (type)
            {
                case typeMessage.MSG_CONNECT:
                    return 0;
                    break;
                case typeMessage.MSG_CONNECTRESPONSE:
                    parts = data.Split(SEPdata);
                    return Convert.ToInt32(parts[4]);
                    break;
                case typeMessage.MSG_HEARTBEAT:
                default:
                    return 0;
                    break;
            }
        }
        #endregion


        #region Connection message
        /// <summary>
        /// Creates a connect message.
        /// </summary>
        /// <returns>The connection message as 'byte[]'.</returns>
        public byte[] ConnectMessage(ClientData sender)
        {
            byte[] tmp = new byte[BUFFER_SIZE_BYTE];
            string msgHeader = createMsgHeader(typeMessage.MSG_CONNECT);
            string msgData = createConnectData(sender);
            string msg = msgHeader + SEPmessage + msgData;
            tmp = Encoding.ASCII.GetBytes(msg);
            return tmp;
        }

        /// <summary>
        /// Creates the data block of a connection message
        /// </summary>
        /// <param name="data">Clientdata of the sender.</param>
        /// <returns></returns>
        private string createConnectData(ClientData data)
        {
            StringBuilder tmp = new StringBuilder();
            tmp.Append(data.Number);
            tmp.Append(SEPdata);
            tmp.Append(data.Name);
            tmp.Append(SEPdata);
            tmp.Append((int)data.Status);
            return tmp.ToString();
        }

        /// <summary>
        /// Creates the response for a connect message.
        /// </summary>
        /// <returns></returns>
        public byte[] ConnectResponse(ClientData receiver, int hashValueList)
        {
            byte[] tmp = new byte[BUFFER_SIZE_BYTE];
            string msgHeader = createMsgHeader(typeMessage.MSG_CONNECTRESPONSE);
            string msgData = createConnectResponseData(receiver, hashValueList);
            string msg = msgHeader + SEPmessage + msgData;
            tmp = Encoding.ASCII.GetBytes(msg);
            return tmp;
        }

        /// <summary>
        /// Creates the data block of the response for a connection message.
        /// </summary>
        /// <param name="data">Clientdata of the receiver</param>
        /// <param name="hashvalue">Hash-Value of the project list on the server</param>
        /// <returns></returns>
        private string createConnectResponseData(ClientData data, int hashvalue)
        {
            StringBuilder tmp = new StringBuilder();
            tmp.Append(data.Number);
            tmp.Append(SEPdata);
            tmp.Append(data.Name);
            tmp.Append(SEPdata);
            tmp.Append((int)data.Status);
            tmp.Append(SEPdata);
            tmp.Append(data.Address.ToString());
            tmp.Append(SEPdata);
            tmp.Append(hashvalue.ToString());
            return tmp.ToString();
        }
        #endregion

        #region New project message
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
        public byte[] NewProjectResponse(int hashValueList)
        {
            byte[] tmp = new byte[BUFFER_SIZE_BYTE];
            string msgHeader = createMsgHeader(typeMessage.MSG_NEWPROJECT);
            string msg = msgHeader + SEPmessage + hashValueList.ToString();
            tmp = Encoding.ASCII.GetBytes(msg);
            return tmp;
        }
        #endregion

        #region Add time message
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
            string msgData = data.ProjectName + SEPdata + TimeToLongString(time);
            string msg = msgHeader + SEPmessage + msgData;
            tmp = Encoding.ASCII.GetBytes(msg);
            return tmp;
        }

        private string TimeToLongString(TimeSpan time)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(time.Days);
            sb.Append(TIMESPANSEPARATOR);
            sb.Append(time.Hours);
            sb.Append(TIMESPANSEPARATOR);
            sb.Append(time.Minutes);
            sb.Append(TIMESPANSEPARATOR);
            sb.Append(time.Seconds);
            return sb.ToString();
        }

        /// <summary>
        /// Creates the response for a AddTime-message
        /// </summary>
        /// <returns></returns>
        public byte[] AddTimeResponse(int hashValueList)
        {
            byte[] tmp = new byte[BUFFER_SIZE_BYTE];
            string msgHeader = createMsgHeader(typeMessage.MSG_ADDTIME);
            string msg = msgHeader + SEPmessage + hashValueList.ToString();
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
        #endregion

        #region Heart-Beat message
        /// <summary>
        /// Creates a HeartBeat-message
        /// </summary>
        /// <returns></returns>
        public byte[] HeartBeatMessage(ClientData sender)
        {
            byte[] tmp = new byte[BUFFER_SIZE_BYTE];
            string msgHeader = createMsgHeader(typeMessage.MSG_HEARTBEAT);
            string msgData = createHeartBeatData(sender);
            string msg = msgHeader + SEPmessage + msgData;
            tmp = Encoding.ASCII.GetBytes(msg);
            return tmp;
        }

        private string createHeartBeatData(ClientData data)
        {
            StringBuilder tmp = new StringBuilder();
            tmp.Append(data.Number);
            tmp.Append(SEPdata);
            tmp.Append(data.Name);
            tmp.Append(SEPdata);
            tmp.Append((int)data.Status);
            tmp.Append(SEPdata);
            tmp.Append(data.Address.ToString());
            return tmp.ToString();
        }

        /// <summary>
        /// Creates the response for a connect message.
        /// </summary>
        /// <returns></returns>
        public byte[] HeartBeatResponse(ClientData receiver, int hashValueList)
        {
            byte[] tmp = new byte[BUFFER_SIZE_BYTE];
            string msgHeader = createMsgHeader(typeMessage.MSG_HEARTBEATRESPONSE);
            string msgdata = createHearBeatResponseData(receiver, hashValueList);
            string msg = msgHeader + SEPmessage + msgdata;
            tmp = Encoding.ASCII.GetBytes(msg);
            return tmp;
        }

        private string createHearBeatResponseData(ClientData data, int hashvalue)
        {
            return createConnectResponseData(data, hashvalue);
        }
        #endregion

        private string createResponseData(ClientData data, int hashvalue)
        {
            StringBuilder tmp = new StringBuilder();
            tmp.Append(data.Number);
            tmp.Append(SEPdata);
            tmp.Append(data.Name);
            tmp.Append(SEPdata);
            tmp.Append((int)data.Status);
            tmp.Append(SEPdata);
            tmp.Append(data.Address.ToString());
            tmp.Append(SEPdata);
            tmp.Append(hashvalue.ToString());
            return tmp.ToString();
        }
    }
}
