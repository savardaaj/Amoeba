using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;



namespace AmoebaGameModels
{
    public class Amoeba
    {
        #region Public Fields
        //The slope in the equation y=mx+b
        public Decimal Slope    { get; set; }
        public Guid    CellId   { get; set; }
        public Boolean Wordy = false;
        
        public Decimal XCoordinate { get; set; }
        public Decimal YCoordinate { get; set; }
        //Exists because moving purely along the y axis (as in x=b) is not a function so slope is undefined
        public Boolean IsMovingAlongTheY  { get; set; }
        #endregion

        #region Public Properties
        //All players will begin at size 1.  Default size for food is .25
        public Decimal Size { get { return this.size; } }
        public Decimal Speed { get { return this.speed; } }
        public Decimal MaxTravelDistance { get { return this.maxtraveldistance; } }
        public Texture2D texture { get; set; }
        public Body body { get; set; } 
       
        #endregion

        #region Private Fields
        private Decimal size;
        private Decimal speed;
        private Decimal maxtraveldistance;
        

        #endregion

        #region Public Constructors
        public Amoeba()
        {
            CellId = Guid.NewGuid ();
            this.size = (Decimal)1;
            this.speed = (Decimal).05;
            this.maxtraveldistance = (Decimal)1.5 * Convert.ToDecimal(Math.Pow(Convert.ToDouble(this.size), -0.439));
        }

        public Amoeba(World world, Decimal size)
        {
            CellId = Guid.NewGuid();
            this.size = size;
            this.maxtraveldistance = (Decimal).00005;
            body = new Body(world);
            body = BodyFactory.CreateCircle(world, .5f, 1f);
        }
        #endregion

        #region Public Methods

        public Decimal Eat(Amoeba Food)
        {
            this.size += Food.Size;
            this.maxtraveldistance = (Decimal)1.5 * Convert.ToDecimal(Math.Pow(Convert.ToDouble(this.size), -0.439));
            return this.size;
        }

        public Decimal SpeedBySize()
        {
            // determine the speed based on the size of the cell
            return 1;
        }

        #endregion
    }
}
