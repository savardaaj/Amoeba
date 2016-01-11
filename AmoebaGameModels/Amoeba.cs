using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmoebaServer
{
    public enum Direction
    {
        Still     = 0,
        North     = 1,
        South     = 2,
        East      = 3,
        West      = 4,
        Northeast = 5,
        Northwest = 6,
        Southeast = 7,
        Southwest = 8,
    }
    class Amoeba
    {
        
        public Direction Direction { get; set; }
        private Int32 Size { get; set; }
        private Int32 Speed { get; set; }
        private Int32 XCoordinate { get; set; }
        private Int32 YCoordinate { get; set; }
    }
}
