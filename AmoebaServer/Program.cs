using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace AmoebaServer
{
    class Program
    {
        static void Main (string [] args)
        {
            Server serverObject = new Server ();

            Task.Factory.StartNew (async () =>
            {
                
                serverObject.Listen ();
                
            });
            int packets = 0;
            do
            {
                if(serverObject.DataList.Count > packets)
                { 
                    Console.WriteLine (serverObject.DataList[packets]);
                    packets++;
                }
            } while (true);
        }
    }
}
