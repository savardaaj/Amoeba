﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;

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
        public Decimal xCenter { get; set; }
        public Decimal yCenter { get; set; }
        //Exists because moving purely along the y axis (as in x=b) is not a function so slope is undefined
        public Boolean IsMovingAlongTheY  { get; set; }
        #endregion

        #region Public Properties
        //All players will begin at radius
        public Decimal Speed { get { return this.speed; } }
        public Decimal MaxTravelDistance { get { return this.maxtraveldistance; } }
        public Texture2D Texture { get; set; }
        public Decimal Radius { get { return this.radius; } }
        #endregion

        #region Private Fields
        private Decimal speed;
        private Decimal maxtraveldistance;
        private Decimal radius;
        #endregion

        #region Public Constructors
        public Amoeba()
        {
            CellId = Guid.NewGuid ();
            this.speed = (Decimal).05;
            this.radius = 50;
            this.maxtraveldistance = (Decimal)25 * Convert.ToDecimal(Math.Pow(Convert.ToDouble(this.radius), -0.439));
            
        }

        //Food Constructor
        public Amoeba(Decimal radius)
        {
            CellId = Guid.NewGuid();
            this.radius = radius;
            this.maxtraveldistance = (Decimal).00005;
        }
        #endregion

        #region Public Methods

        public Decimal Eat(Amoeba Food)
        {
            this.radius = Convert.ToDecimal(Math.Pow(Math.Pow((double) this.radius, 2) + Math.Pow((double) Food.radius, 2), (double) .5));
            this.maxtraveldistance = (Decimal)25 * Convert.ToDecimal(Math.Pow(Convert.ToDouble(this.radius), -0.439));
            return this.radius;
        }

        public Decimal SpeedBySize()
        {
            // determine the speed based on the size of the cell
            return 1;
        }

        #endregion
    }
}
