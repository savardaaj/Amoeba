using AmoebaGameModels;
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
            Server serverObject = new Server (11000);
            Boolean isCanceled = false;
            GameBoard board = new GameBoard ();
            Amoeba recievedAmoeba;

            serverObject.Listen ();

            Task.Factory.StartNew (() =>
            {
                if (Console.ReadLine ().Equals ("cancel", StringComparison.InvariantCultureIgnoreCase))
                {
                    isCanceled = true;
                }
            });

            while (isCanceled != true)
            {
                foreach (Amoeba amoeba in board.GamePieces.Values)
                {
                    serverObject.SendToAll (amoeba.ToByteArray ());
                }

                foreach (Byte [] amoebaArray in serverObject.DataList.Values)
                {
                    Amoeba.TryParse (amoebaArray.Skip(8).ToArray (), out recievedAmoeba);
                    board.GamePieces [recievedAmoeba.CellId] = recievedAmoeba;
                }

            }
        }
    }
}
