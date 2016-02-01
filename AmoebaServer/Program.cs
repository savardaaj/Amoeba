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
            
            Task.Factory.StartNew (() =>
            {
                if (Console.ReadLine ().Equals ("cancel", StringComparison.InvariantCultureIgnoreCase))
                {
                    isCanceled = true;
                }
            });

            serverObject.Listen ();

            while (isCanceled != true)
            {
                foreach (Amoeba amoeba in board.GamePieces.Values)
                {
                    serverObject.SendToAll (amoeba.ToByteArray ());
                }

                Parallel.ForEach (serverObject.DataList.Values, amoebaArray =>
                {
                    Amoeba.TryParse (amoebaArray.Skip (8).ToArray (), out recievedAmoeba);
                    board.GamePieces [recievedAmoeba.CellId] = recievedAmoeba;
                });

                board.SearchForCellsEating ();
                board.MoveFoodPieces ();
                
            }
        }
    }
}
