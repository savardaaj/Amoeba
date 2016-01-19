using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Amoeba
{
    public class UDPClient
    {
        private const Int32 listenPort      = 11000;
        EndPoint gameServer = null;
        private Socket clientSocket = null;
        private List <Byte []> dataList = new List <Byte []> ();
        private Byte [] byteData = new Byte [1024];

        public UDPClient (IPAddress serverIP)
        {
            gameServer = new IPEndPoint (serverIP, listenPort);
        }

        public void Listen ()
        {
            this.clientSocket = new Socket (AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.clientSocket.SetSocketOption (SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            this.clientSocket.Bind (new IPEndPoint (IPAddress.Any, listenPort));
            this.clientSocket.BeginReceiveFrom (this.byteData, 0, this.byteData.Length, SocketFlags.None, ref gameServer, DoReceiveFrom, gameServer);
        }

        public void DoReceiveFrom (IAsyncResult ar)
        {
            EndPoint serverEP = new IPEndPoint (IPAddress.Any, 0);
            Int32 dataLength = this.clientSocket.EndReceiveFrom (ar, ref serverEP);
            
            if (this.gameServer.Equals(serverEP) == true)
            {
                Byte [] data = new Byte [dataLength];
                Array.Copy (this.byteData, data, dataLength);
                dataList.Add (data);
            }

            this.clientSocket.BeginReceiveFrom (this.byteData, 0, this.byteData.Length, SocketFlags.None, ref gameServer, DoReceiveFrom, gameServer);
            
        }

        public void SendToServer (Byte [] data)
        {
            try
            {
                this.clientSocket.SendTo (data, gameServer);
            }
            catch (System.Net.Sockets.SocketException)
            { }
        }
    }
}
