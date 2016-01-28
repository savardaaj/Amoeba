using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using System.Web.Script.Serialization;

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
        public Decimal XSpeed { get; set; }
        public Decimal YSpeed { get; set; }
        public Decimal xCenter { get; set; }
        public Decimal yCenter { get; set; }
        //Exists because moving purely along the y axis (as in x=b) is not a function so slope is undefined
        public Boolean IsMovingAlongTheY  { get; set; }
        public float Scale { get; set; }
        #endregion

        #region Public Properties
        //All players will begin at radius
        public Decimal MaxTravelDistance { get { return this.maxtraveldistance; } }
        public Texture2D Texture { get; set; }
        public Decimal Radius { get { return this.radius; } }     
        #endregion

        #region Private Fields
        private Decimal maxtraveldistance;
        private Decimal radius;
        #endregion

        #region Public Static Methods

        public static Boolean TryParse (String input, out Amoeba output)
        {
            try
            {
                output = new JavaScriptSerializer ().Deserialize <Amoeba> (input);
                return true;
            }
            catch (Exception e)
            {
                output = null;
                return false;
            }
        }

        public static Boolean TryParse (Byte [] input, out Amoeba output)
        {
            try
            {
                output = new JavaScriptSerializer ().Deserialize <Amoeba> (System.Text.Encoding.UTF8.GetString (input));
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
            CellId = Guid.NewGuid ();
            this.radius = 35;
            this.XSpeed = 0;
            this.YSpeed = 0;
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
            //this.radius = (Convert.ToDecimal(Math.Pow(Math.Pow((double) this.radius, 2) + Math.Pow((double) Food.radius, 2), (double) .5)));
            this.radius += 1;
            this.maxtraveldistance = (Decimal)25 * Convert.ToDecimal(Math.Pow(Convert.ToDouble(this.radius), -0.439));
            Scale += .1f;
            return this.radius;
        }

        public Decimal SpeedBySize()
        {
            // determine the speed based on the size of the cell
            return 1;
        }


        public String ToString ()
        {
            String json = new JavaScriptSerializer ().Serialize (this);
            return json;
        }

        public Byte [] ToByteArray ()
        {
            return System.Text.Encoding.UTF8.GetBytes (this.ToString ());
        }

        #endregion
    }
}