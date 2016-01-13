using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmoebaGameModels
{
    static class GameBoard
    {
        public static Int32 Width  { get { return GameBoard.width;  } }
        public static Int32 Height 
        { 
            get 
            { 
                return GameBoard.height; 
            } 
        }


        private static Int32 width  = -1;
        private static Int32 height = -1;

        public static Dictionary <Guid, Amoeba> GamePieces;

    }
}
