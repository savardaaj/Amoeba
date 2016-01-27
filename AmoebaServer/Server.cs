using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AmoebaServer
{
    enum ServerStatus
    {
        Starting,
        Running,
        Stopping,
        Off,
        Unknown
    }

    class Server
    {
        public const int listenPort = 11000;

        private List<IPEndPoint> clientList = new List<IPEndPoint> ();
        private List<Tuple<IPEndPoint, Byte []>> dataList = new List<Tuple<IPEndPoint, Byte []>> ();
        private UdpClient udpClient = new UdpClient (listenPort);
        private Boolean isCanceled = false;

        public ServerStatus Status { get; private set; }

        

        public List<Tuple<IPEndPoint, Byte []>> DataList
        {
            get { return this.dataList; }
            private set { this.dataList = value; }
        }

        public Server ()
        {
            this.Status = ServerStatus.Off;
        }
        
        public void Listen ()
        {
            Status = ServerStatus.Starting;
            
            Task.Run (async () =>
            {
                Status = ServerStatus.Running;
                while (this.isCanceled != true)
                { 
                    UdpReceiveResult receivedResults = await udpClient.ReceiveAsync ();
                    dataList.Add (new Tuple<IPEndPoint, Byte []> (receivedResults.RemoteEndPoint, receivedResults.Buffer));
                    if (this.clientList.Any (client => client.Equals(receivedResults.RemoteEndPoint)) != true)
                    {
                        this.clientList.Add (receivedResults.RemoteEndPoint);
                    }
                }
            });            
        }

        public void StopListening ()
        {
            this.Status = ServerStatus.Stopping;
            this.isCanceled = true;
            this.udpClient.Close ();
            this.Status = ServerStatus.Off;
            
        }

        public void SendTo (Byte [] data, IPEndPoint clientEP)
        {
            try
            {
                this.udpClient.Send (data, data.Length, clientEP);
            }
            catch (System.Net.Sockets.SocketException)
            {
                this.clientList.Remove (clientEP);
            }
        }

        public void SendToAll (Byte [] data)
        {
            foreach (IPEndPoint client in this.clientList)
            {
                this.udpClient.Send (data, data.Length, client);
            }
        }
    }
}
