﻿using System;
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
        public Decimal Slope    { get; set; }
        public Guid    CellId   { get; set; }
        public Boolean Wordy    { get; set; }
        
        public Decimal XCoordinate { get; set; }
        public Decimal YCoordinate { get; set; }
        //Exists because moving purely along the y axis (as in x=b) is not a function so slope is undefined
        public Boolean IsMovingAlongTheY  { get; set; }
        #endregion

        #region Public Properties
        //All players will begin at size 1.  Default size for food is .25
        public Decimal Size { get { return this.size; } }
<<<<<<< HEAD
        public Decimal Speed { get { return /*this.speed;*/ this.maxspeed; } }
        public Decimal MaxSpeed { get { return this.maxspeed; } }
        public Decimal MaxTravelDistance { get { return this.maxtraveldistance; } }

=======
        public Decimal Speed { get { return this.speed; } }
        public Decimal XCoordinate { get; set; }
        public Decimal YCoordinate { get; set; }
        public Texture2D texture { get; set; }
>>>>>>> 9ff5f857fe03186f028e1baff407c919ce6a9ea0
        #endregion

        #region Private Fields
        private Decimal size;
        private Decimal speed;
        private Decimal maxspeed;
        private Decimal maxtraveldistance;
        #endregion

        #region Public Constructors
        public Amoeba()
        {
            this.Wordy = false;
            CellId = Guid.NewGuid ();
            this.size = (Decimal).25;
            this.maxspeed = (Decimal).05;
            this.maxtraveldistance = (Decimal)2;
        }

        //public Amoeba(Decimal size, Decimal maxspeed)
        //{
        //    CellId = Guid.NewGuid ();
        //    this.size = size;
        //    this.maxspeed = (Decimal).00005;
        //}
        #endregion

        #region Public Methods

        public Decimal Eat(Amoeba Food)
        {
            this.size += Food.Size;
            //this.speed = (Decimal)1.5 * Convert.ToDecimal(Math.Pow(Convert.ToDouble(this.size), -0.439));
            //this.maxspeed = 2;
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
