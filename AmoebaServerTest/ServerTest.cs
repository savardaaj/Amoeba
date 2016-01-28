using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AmoebaServer;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace AmoebaServerTest
{
    [TestClass]
    public class ServerTest
    {
        [TestMethod]
        public void TestRecievePacket ()
        {
            Server serverObject = new Server ();
            String testString = "123abc";
            EndPoint localHost = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Server.listenPort);
            Int32 waits = 0;
            serverObject.Listen ();


            //Wait for server to start listening
            while (serverObject.Status != ServerStatus.Running) { System.Threading.Thread.Sleep (1000); }
            Socket serverSocket = new Socket (AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            serverSocket.SetSocketOption (SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            try
            {
                serverSocket.SendTo (System.Text.Encoding.UTF8.GetBytes(testString), localHost);
            }
            catch (System.Net.Sockets.SocketException)
            {            }
            
            while (serverObject.DataList.Count < 1 && waits < 10)
            {
                System.Threading.Thread.Sleep (1000);
                waits++;
            }

            Assert.AreEqual (testString, System.Text.Encoding.UTF8.GetString(serverObject.DataList[0].Item2, 0, 6));
            serverObject.StopListening ();
        }

        [TestMethod]
        public void TestSendPacket ()
        {
            Server serverObject = new Server ();
            UdpClient udpClient = new UdpClient (11111);
            String testString = "123abc";
            IPEndPoint localHost = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11111);
            Int32 waits = 0;
            Byte [] byteData = new Byte [6];

            Task.Run (async () =>
            {
                UdpReceiveResult receivedResults = await udpClient.ReceiveAsync ();
                byteData = receivedResults.Buffer;
                    
            });

            System.Threading.Thread.Sleep (1000);
            serverObject.SendTo (System.Text.Encoding.UTF8.GetBytes (testString), localHost);

            while (byteData[1] == 0  && waits < 10)
            {
                System.Threading.Thread.Sleep (1000);
                waits++;
            }

            Assert.AreEqual (testString, System.Text.Encoding.UTF8.GetString(byteData, 0, 6));
            serverObject.StopListening ();
            udpClient.Close ();
        }


    }
}
