using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace AmoebaGameModels
{
    public class GameBoard
    {
        public Int32 Width { get; private set; }
        public Int32 Height { get; private set; }
        public Decimal FoodSize { get; private set; }
        public  Int32 MaxFoodPieces { get; private set; }

        public Dictionary <Guid, Amoeba> GamePieces;

        public GameBoard ()
        { }

        public List <Amoeba> GetFoodPieces ()
        {
            return GamePieces.Values.Where (x => x.Radius == this.FoodSize).ToList ();
        }

        public void GenerateFood ()
        {
            Int32 currentFoodPieces = this.GetFoodPieces ().Count;

            while (currentFoodPieces < MaxFoodPieces)
            {
                Amoeba amoeba = CreateNewFood ();
                GamePieces.Add (amoeba.CellId, amoeba);
            }
        }

        
        /*
         * This function generates random color and coordinates for food objects. 
         * It also sets the texture and coordinates for the object 
         */
        public Amoeba CreateNewFood ()
        {
            Amoeba newFood = new Amoeba (FoodSize);
            //grab random color and x,y for food
            String [] colorArray  = new string[] { "BlueFood", "RedFood", "GreenFood", "YellowFood", "PinkFood" };
            Random randomNumberGen  = new Random();
            Random randomColorGen  = new Random();
            String randomColor = colorArray[randomColorGen.Next(0, colorArray.Length - 1)];
            Int32 randomX = randomNumberGen.Next(0, Width);
            Int32 randomY = randomNumberGen.Next(0, Height);

            //Set x and y
            newFood.XCoordinate = randomX;
            newFood.YCoordinate = randomY;

            return newFood;
        }

    }
}
