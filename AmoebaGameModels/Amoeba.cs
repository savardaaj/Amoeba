using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;



namespace AmoebaGameModels
{
    public class Amoeba
    {
        #region Public Fields
        //The slope in the equation y=mx+b
        public Decimal Slope  { get; set; }
        public Guid    CellId { get; set; }

        //Exists because moving purely along the y axis (as in x=b) is not a function so slope is undefined
        public Boolean IsMovingAlongTheY  { get; set; }
        #endregion

        #region Public Properties
        //All players will begin at size 1.  Default size for food is .25
        public Decimal Size { get { return this.size; } }
        public Decimal Speed { get { return this.speed; } }
        public Decimal XCoordinate { get; set; }
        public Decimal YCoordinate { get; set; }
        public Texture2D texture { get; set; }
        #endregion

        #region Private Fields
        private Decimal size;
        private Decimal speed;
        #endregion

        #region Public Constructors
        public Amoeba()
        {
            CellId = Guid.NewGuid ();
            this.size = (Decimal).25;
        }

        public Amoeba(Decimal size, Decimal speed)
        {
            CellId = Guid.NewGuid ();
            this.size = size;
            this.speed = speed;
        }
        #endregion

        #region Public Methods

        public Decimal Eat(Amoeba Food)
        {
            this.size += Food.Size;
            //this.speed = (Decimal)1.5 * Convert.ToDecimal(Math.Pow(Convert.ToDouble(this.size), -0.439));
            this.speed = 2;
            return this.size;
        }



        #endregion
    }
}
