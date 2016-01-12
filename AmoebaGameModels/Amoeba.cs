using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmoebaGameModels
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
        #region Public Fields
        public Direction Direction { get; set; }
        #endregion

        #region Public Properties
        //All players will begin at size 1.  Default size for food is .25
        public Decimal Size { get { return this.size; } }
        public Decimal Speed { get; }
        public Decimal XCoordinate { get; }
        public Decimal YCoordinate { get; }
        #endregion

        #region Private Fields
        private Decimal size;
        private Decimal speed;
        private Decimal xcoordinate;
        private Decimal ycoordinate;
        #endregion

        #region Public Constructors
        public Amoeba ()
        {
            
        }
        #endregion

        #region Public Methods

        public Decimal Eat (Amoeba Food)
        {
            this.size += Food.Size;
            this.speed = 22 * Math.Pow(this.size, -0.439);
            return this.size;
        }



        #endregion
    }
}
