using System;
using System.Net;

namespace MyLib
{
    public enum ClientStati { Connected, Unknown, Dead, NotStarted, NotConnected }
    public class ClientData
    {
        #region Members
        private string name;
        private int number;
        private ClientStati status;
        private IPEndPoint address;
        private int startingAttempts;
        private DateTime lastHeartBeat;
        public string Name { get => name; set => name = value; }
        public int Number { get => number; set => number = value; }
        public ClientStati Status { get => status; set => status = value; }
        public IPEndPoint Address { get => address; set => address = value; }
        public int StartingAttempts { get => startingAttempts; set => startingAttempts = value; }
        public DateTime LastHeartBeat { get => lastHeartBeat; set => lastHeartBeat = value; }
        #endregion

        /// <summary>
        /// CTor
        /// </summary>
        /// <param name="cName">Name of the client</param>
        /// <param name="cNumber">Number of the client</param>
        /// <param name="cStatus">Status of the client</param>
        public ClientData(string cName, ClientStati cStatus)
        {
            name = cName;
            number = -1;
            status = cStatus;
            address = null;
            lastHeartBeat = DateTime.MinValue;
        }

        /// <summary>
        /// CTor with IP and number
        /// </summary>
        /// <param name="cName">Name of the client</param>
        /// <param name="cNumber">Number of the client</param>
        /// <param name="cStatus">Status of the client</param>
        /// <param name="cAddress">IP and port of the client</param>
        public ClientData(string cName, int cNumber, ClientStati cStatus, IPEndPoint cAddress)
        {
            name = cName;
            number = cNumber;
            status = cStatus;
            address = cAddress;
            startingAttempts = 0;
            lastHeartBeat = DateTime.MinValue;
        }

        #region Operators
        public static bool operator ==(ClientData obj1, ClientData obj2)
        {
            return (obj1.name == obj2.name
                    && obj1.address.Equals(obj2.address));
        }

        public static bool operator !=(ClientData obj1, ClientData obj2)
        {
            if (obj1 == null || obj2 == null)
            {
                return false;
            }
            else
            {
                return !(obj1.name.Equals(obj2.name));
            }           
        }
        #endregion
    }
}
