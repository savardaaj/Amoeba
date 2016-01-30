using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AmoebaGameModels;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Linq;

namespace AmoebaGameModelsTest
{
    [TestClass]
    public class ServerTest
    {
        [TestMethod]
        public void TestRecievePacket ()
        {
            Server serverObject = new Server (11000);
            Byte [] testString = System.Text.Encoding.UTF8.GetBytes ("123abc");
            UdpClient udpClient = new UdpClient (11111);
            IPEndPoint localHost = new IPEndPoint(IPAddress.Parse("127.0.0.1"), serverObject.listenPort);
            IPEndPoint sentFrom = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11111);
            Int32 waits = 0;
            serverObject.Listen ();


            //Wait for server to start listening
            while (serverObject.Status != ServerStatus.Running) { System.Threading.Thread.Sleep (1000); }
            try
            {
                udpClient.Send (testString, testString.Length, localHost);
            }
            catch (System.Net.Sockets.SocketException)
            {            }
            
            while (serverObject.DataList.Count < 1 && waits < 10)
            {
                System.Threading.Thread.Sleep (500);
                waits++;
            }
            Assert.IsTrue (serverObject.DataList[sentFrom].SequenceEqual(testString));
            serverObject.Dispose ();
            udpClient.Close ();
        }

        [TestMethod]
        public void TestSendPacket ()
        {
            Server serverObject = new Server (11000);
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
                System.Threading.Thread.Sleep (500);
                waits++;
            }

            Assert.AreEqual (testString, System.Text.Encoding.UTF8.GetString(byteData.Skip (8).ToArray (), 0, 6));
            serverObject.Dispose ();
            udpClient.Close ();
        }

        [TestMethod]
        public void TestRecieveMultiplePackets ()
        {
            Server serverObject = new Server (11000);
            Server sendingObject = new Server (11111);
            Byte[] testString = System.Text.Encoding.UTF8.GetBytes ("123abc");
            Byte[] testString2 = System.Text.Encoding.UTF8.GetBytes ("testString2");
            Byte[] testString3 = System.Text.Encoding.UTF8.GetBytes ("testString3");
            Byte[] testString4 = System.Text.Encoding.UTF8.GetBytes ("testString4");
            IPEndPoint localHost = new IPEndPoint(IPAddress.Parse("127.0.0.1"), serverObject.listenPort);
            IPEndPoint sentFrom = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11111);
            Byte [] byteData = new Byte [6];

            serverObject.Listen ();

            System.Threading.Thread.Sleep (500);
            sendingObject.SendTo (testString, localHost);
            System.Threading.Thread.Sleep (500);
            Assert.IsTrue (serverObject.DataList[sentFrom].Skip(8).ToArray().SequenceEqual(testString));

            sendingObject.SendTo (testString2, localHost);
            System.Threading.Thread.Sleep (500);
            Assert.IsTrue (serverObject.DataList[sentFrom].Skip(8).ToArray().SequenceEqual(testString2));

            sendingObject.SendTo (testString3, localHost);
            System.Threading.Thread.Sleep (500);
            Assert.IsTrue (serverObject.DataList[sentFrom].Skip(8).ToArray().SequenceEqual(testString3));

            sendingObject.SendTo (testString4, localHost);
            System.Threading.Thread.Sleep (500);
            Assert.IsTrue (serverObject.DataList[sentFrom].Skip(8).ToArray().SequenceEqual(testString4));
            
            serverObject.Dispose ();
            sendingObject.Dispose ();
        }


        [TestMethod]
        public void TestRecieveFromMultipleClients ()
        {
            Server serverObject = new Server (11000);
            Server sendingObject = new Server (11111);
            Server sendingObject2 = new Server (11112);
            Server sendingObject3 = new Server (11113);
            Server sendingObject4 = new Server (11114);
            Byte[] testString = System.Text.Encoding.UTF8.GetBytes ("123abc");
            Byte[] testString2 = System.Text.Encoding.UTF8.GetBytes ("testString2");
            Byte[] testString3 = System.Text.Encoding.UTF8.GetBytes ("testString3");
            Byte[] testString4 = System.Text.Encoding.UTF8.GetBytes ("testString4");
            IPEndPoint localHost = new IPEndPoint(IPAddress.Parse("127.0.0.1"), serverObject.listenPort);
            IPEndPoint sentFrom = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11111);
            IPEndPoint sentFrom2 = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11112);
            IPEndPoint sentFrom3 = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11113);
            IPEndPoint sentFrom4 = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11114);
            Byte [] byteData = new Byte [6];

            serverObject.Listen ();

            System.Threading.Thread.Sleep (500);
            sendingObject.SendTo (testString, localHost);

            sendingObject2.SendTo (testString2, localHost);

            sendingObject3.SendTo (testString3, localHost);

            sendingObject4.SendTo (testString4, localHost);
            System.Threading.Thread.Sleep (500);

            Assert.IsTrue (serverObject.DataList [sentFrom].Skip (8).ToArray ().SequenceEqual (testString));
            Assert.IsTrue (serverObject.DataList [sentFrom2].Skip (8).ToArray ().SequenceEqual (testString2));
            Assert.IsTrue (serverObject.DataList [sentFrom3].Skip (8).ToArray ().SequenceEqual (testString3));
            Assert.IsTrue (serverObject.DataList [sentFrom4].Skip (8).ToArray ().SequenceEqual (testString4));
            Assert.IsFalse (serverObject.DataList [sentFrom].Skip (8).ToArray ().SequenceEqual (testString4));

            serverObject.Dispose ();
            sendingObject.Dispose ();
            sendingObject2.Dispose ();
            sendingObject3.Dispose ();
            sendingObject4.Dispose ();
        }

        [TestMethod]
        public void TestSendToAllClients ()
        {
            Server serverObject = new Server (11000);
            Server listener1 = new Server (11111);
            Server listener2 = new Server (11112);
            IPEndPoint localHost = new IPEndPoint(IPAddress.Parse("127.0.0.1"), serverObject.listenPort);
            IPEndPoint listenPoint1 = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11111);
            IPEndPoint listenPoint2 = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11112);
            Byte[] testString = System.Text.Encoding.UTF8.GetBytes ("123abc");
            Byte[] testString2 = System.Text.Encoding.UTF8.GetBytes ("testString2");

            serverObject.Listen ();
            listener1.Listen ();
            listener2.Listen ();

            System.Threading.Thread.Sleep (500);
            listener1.SendTo (testString, localHost);
            listener2.SendTo (testString, localHost);
            System.Threading.Thread.Sleep (500);
            serverObject.SendToAll (testString2);
            System.Threading.Thread.Sleep (500);

            Assert.IsTrue (listener1.DataList [localHost].Skip (8).ToArray ().SequenceEqual (testString2));
            Assert.IsTrue (listener2.DataList [localHost].Skip (8).ToArray ().SequenceEqual (testString2));
            listener1.Dispose ();
            listener2.Dispose ();
            serverObject.Dispose ();
        }
    }
}
