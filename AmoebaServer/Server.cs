using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AmoebaServer
{

    class Server
    {
        private const int listenPort = 11000;

        private Socket serverSocket = null;
        private List<EndPoint> clientList = new List<EndPoint> ();
        private List<Tuple<EndPoint, Byte []>> dataList = new List<Tuple<EndPoint, Byte []>> ();
        private Byte [] byteData = new Byte [1024];
        private IAsyncResult currentAsyncResult;

        public List<Tuple<EndPoint, Byte []>> DataList
        {
            get { return this.dataList; }
            private set { this.dataList = value; }
        }

        public Server ()
        {

        }
        
        public void Listen ()
        {
            this.serverSocket = new Socket (AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.serverSocket.SetSocketOption (SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            this.serverSocket.Bind (new IPEndPoint (IPAddress.Any, listenPort));
            EndPoint newClientEP = new IPEndPoint (IPAddress.Any, 0);
            this.currentAsyncResult = this.serverSocket.BeginReceiveFrom (this.byteData, 0, this.byteData.Length, SocketFlags.None, ref newClientEP, DoReceiveFrom, newClientEP);
        }

        public void DoReceiveFrom (IAsyncResult ar)
        {
            try
            {
                EndPoint clientEP = new IPEndPoint (IPAddress.Any, 0);
                if (ar == this.currentAsyncResult)
                {
                    
                    Int32 dataLength = this.serverSocket.EndReceiveFrom(ar, ref clientEP);
                    Byte [] data = new Byte [dataLength];
                    Array.Copy (this.byteData, data, dataLength);

                    if (this.clientList.Any (client => client.Equals(clientEP)) != true)
                    {
                        this.clientList.Add (clientEP);
                    }

                    EndPoint newClientEP = new IPEndPoint (IPAddress.Any, 0);
                    DataList.Add (Tuple.Create (clientEP, data));
                    Console.WriteLine (BitConverter.ToDouble(data,0).ToString ());
                }
                    this.currentAsyncResult = this.serverSocket.BeginReceiveFrom (this.byteData, 0, this.byteData.Length, SocketFlags.None, ref clientEP, DoReceiveFrom, clientEP);

                    
                
            }
            catch (ObjectDisposedException)
            { }
        }

        public void SendTo (Byte [] data, EndPoint clientEP)
        {
            try
            {
                this.serverSocket.SendTo (data, clientEP);
            }
            catch (System.Net.Sockets.SocketException)
            {
                this.clientList.Remove (clientEP);
            }
        }

        public void SendToAll (Byte [] data)
        {
            foreach (EndPoint client in this.clientList)
            {
                this.SendTo (data, client);
            }
        }
    }
}
