using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmoebaGameModels
{
    public class Amoeba
    {
        #region Public Fields
        //The slope in the equation y=mx+b
        public Decimal Slope { get; set; }

        //The boolean indicates that the cell is moving positive to negative along the x or y depending on  IsMovingAlongTheY's value
        public Boolean IsMovingNegative   { get; set; }
        //Exists because moving purely along the y axis (as in x=b) is not a function so slope is undefined
        public Boolean IsMovingAlongTheY  { get; set; }
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
        public Amoeba()
        {
            this.size = (Decimal).25;
        }

        public Amoeba(Decimal size, Decimal speed)
        {
            this.size = size;
            this.speed = speed;
        }
        #endregion

        #region Public Methods

        public Decimal Eat(Amoeba Food)
        {
            this.size += Food.Size;
            this.speed = 22 * Convert.ToDecimal(Math.Pow(Convert.ToDouble(this.size), -0.439));
            return this.size;
        }



        #endregion
    }
}
