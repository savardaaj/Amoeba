using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AmoebaGameModels
{
    public enum ServerStatus
    {
        Starting,
        Running,
        Stopping,
        Off,
        Unknown
    }

    public class Server
    {
        public int listenPort;

        private List<IPEndPoint> clientList = new List<IPEndPoint> ();
        private Dictionary<IPEndPoint, Byte []> dataList = new Dictionary<IPEndPoint, Byte []> ();
        private UdpClient udpClient;
        private Boolean isCanceled = false;

        public ServerStatus Status { get; private set; }

        

        public Dictionary<IPEndPoint, Byte []> DataList
        {
            get { return this.dataList; }
            private set { this.dataList = value; }
        }

        public Server (int port)
        {
            this.Status = ServerStatus.Off;
            this.listenPort = port;
            udpClient = new UdpClient (port);
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
                    
                    if (this.clientList.Any (client => client.Equals(receivedResults.RemoteEndPoint)) != true)
                    {
                        this.clientList.Add (receivedResults.RemoteEndPoint);
                        dataList.Add (receivedResults.RemoteEndPoint, receivedResults.Buffer);
                    }
                    else
                    {
                        if(DateTime.FromBinary(BitConverter.ToInt64(dataList [receivedResults.RemoteEndPoint], 0)) < DateTime.FromBinary(BitConverter.ToInt64(receivedResults.Buffer, 0)))
                        { 
                            this.dataList [receivedResults.RemoteEndPoint] = receivedResults.Buffer;
                        }
                    }
                }
            });            
        }

        public void Dispose ()
        {
            this.Status = ServerStatus.Stopping;
            this.isCanceled = true;
            this.udpClient.Close ();
            this.Status = ServerStatus.Off;
            
        }

        public void SendTo (Byte [] data, IPEndPoint clientEP)
        {
            DateTime utcNow = DateTime.UtcNow;
            Int64 utcNowAsLong = utcNow.ToBinary ();
            Byte [] utcNowBytes = BitConverter.GetBytes (utcNowAsLong);
            Byte [] dataArray = new Byte [data.Length + utcNowBytes.Length];
            Array.Copy(utcNowBytes, 0,  dataArray, 0, utcNowBytes.Length);
            Array.Copy(data, 0,  dataArray, utcNowBytes.Length, data.Length);
            try
            {
                this.udpClient.Send (dataArray, dataArray.Length, clientEP);
            }
            catch (System.Net.Sockets.SocketException)
            {
                this.clientList.Remove (clientEP);
            }
        }

        public void SendToAll (Byte [] data)
        {
            Parallel.ForEach (this.clientList, client =>
            {
                this.SendTo (data, client);
            });
        }
    }
}
