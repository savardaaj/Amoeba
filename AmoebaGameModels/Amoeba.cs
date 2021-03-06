﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Web.Script.Serialization;

namespace AmoebaGameModels
{
    public class Amoeba
    {
        #region Public Fields
        //The slope in the equation y=mx+b
        public Guid CellId { get; set; }
        public Boolean Wordy = false;

        public double XCoordinate { get; set; }
        public double YCoordinate { get; set; }
        public Decimal XSpeed { get; set; }
        public Decimal YSpeed { get; set; }

        public float Rotation { get; set; }
        public Vector2 Velocity { get; set; }
        public float Scale { get; set; }
        public float Spin { get; set; }
        public double EjectedMassTravelDistance { get; set; }
        #endregion

        #region Public Properties
        //All players will begin at radius
        public Decimal MaxTravelDistance { get { return this.maxtraveldistance; } }
        public Texture2D Texture { get; set; }
        public Decimal Radius { get; set; }
        public string Name { get; set; }
        public float randomRotation;
        public double randomX, randomY;
        #endregion

        #region Private Fields
        private Decimal maxtraveldistance;

        private Random randomGenerator;
        #endregion

        #region Public Static Methods

        public static Boolean TryParse(String input, out Amoeba output)
        {
            try
            {
                output = new JavaScriptSerializer().Deserialize<Amoeba>(input);
                return true;
            }
            catch (Exception e)
            {
                output = null;
                return false;
            }
        }

        public static Boolean TryParse(Byte[] input, out Amoeba output)
        {
            try
            {
                output = new JavaScriptSerializer().Deserialize<Amoeba>(System.Text.Encoding.UTF8.GetString(input));
                return true;
            }
            catch (Exception e)
            {
                output = null;
                return false;
            }
        }

        #endregion

        #region Public Constructors
        public Amoeba()
        {
            CellId = Guid.NewGuid();
            this.Radius = 20;
            this.XSpeed = 0;
            this.YSpeed = 0;
            this.maxtraveldistance = (Decimal)25 * Convert.ToDecimal(Math.Pow(Convert.ToDouble(this.Radius), -0.439));

        }

        //Food Constructor
        public Amoeba(Decimal radius)
        {
            CellId = Guid.NewGuid();
            randomGenerator = new Random();
            this.Radius = radius;
            this.maxtraveldistance = (Decimal).00005;
            GenerateRotation();
            GenerateVelocity();
        }

        public Amoeba(Decimal radius, string name)
        {
            CellId = Guid.NewGuid();
            this.Radius = radius;
            this.Name = name;
            this.XSpeed = 0;
            this.YSpeed = 0;
            this.EjectedMassTravelDistance = 0;
            this.maxtraveldistance = (Decimal)25 * Convert.ToDecimal(Math.Pow(Convert.ToDouble(this.Radius), -0.439));
        }
        #endregion

        #region Public Methods

        public Decimal Eat(Amoeba Food)
        {
            //this.radius = (Convert.ToDecimal(Math.Pow(Math.Pow((double) this.radius, 2) + Math.Pow((double) Food.radius, 2), (double) .5)));
            this.Radius += (Decimal) 1.2;
            this.maxtraveldistance = (Decimal)25 * Convert.ToDecimal(Math.Pow(Convert.ToDouble(this.Radius), -0.439));
            Scale += .1f;
            return this.Radius;
        }

        public Decimal EatEjectedMass(Amoeba Mass)
        {
            //this.radius = (Convert.ToDecimal(Math.Pow(Math.Pow((double) this.radius, 2) + Math.Pow((double) Food.radius, 2), (double) .5)));
            this.Radius += (Decimal) 1.82;
            this.maxtraveldistance = (Decimal)25 * Convert.ToDecimal(Math.Pow(Convert.ToDouble(this.Radius), -0.439));
            Scale += .1f;
            return this.Radius;
        }

        public Decimal SpeedBySize()
        {
            // determine the speed based on the size of the cell
            return 1;
        }

        public String ToString ()

        {
            String json = new JavaScriptSerializer().Serialize(this);
            return json;
        }

        public Byte[] ToByteArray()
        {
            return System.Text.Encoding.UTF8.GetBytes(this.ToString());
        }

        public void GenerateVelocity()
        {

            randomX = randomGenerator.NextDouble() * (.5 - (-.5)) + -.5;
            randomY = randomGenerator.NextDouble() * (.5 - (-.5)) + -.5;
        }

        public void GenerateRotation()
        {
            Rotation = randomGenerator.Next(1, 360);
            if (Rotation > 180)
            {
                Spin = -.01f;
            }
            else
            {
                Spin = .01f;
            }
        }
        #endregion



    }
}