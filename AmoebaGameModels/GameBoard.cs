using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using System.Threading.Tasks;

namespace AmoebaGameModels
{
    public class GameBoard
    {
        public Int32 Width { get; private set; }
        public Int32 Height { get; private set; }
        public Decimal FoodSize { get; private set; }
        public Decimal PlayerSize { get; private set; }
        public Int32 MaxFoodPieces { get; private set; }

        public Dictionary <Guid, Amoeba> GamePieces;

        public GameBoard ()
        {
            this.GamePieces = new Dictionary<Guid, Amoeba> ();
        }

        public List <Amoeba> GetFoodPieces ()
        {
            return GamePieces.Values.Where (x => x.Name == null).ToList ();
        }

        public List <Amoeba> GetPlayerPieces ()
        {
            return GamePieces.Values.Where (x => x.Name != null).ToList ();
        }

        /// <summary>
        /// Searches list of amoebas and food, updates if any ate other ones, and removes the eaten ones
        /// </summary>
        public void SearchForCellsEating ()
        {
            List<Amoeba> collidedAmoebas = null;
            Parallel.ForEach (this.GamePieces.Values, amoeba =>
            {
                collidedAmoebas = this.GamePieces.Values.Where (x => Math.Pow ((double)(x.XCoordinate - amoeba.XCoordinate), 2) +
                    Math.Pow ((double)(x.YCoordinate - amoeba.YCoordinate), 2) < (double)amoeba.Radius).ToList ();
                if(collidedAmoebas.Count > 0)
                {
                    foreach (Amoeba collision in collidedAmoebas)
                    {
                        amoeba.Eat (collision);
                        this.GamePieces.Remove (collision.CellId);
                    }
                }
            });
        }

        /// <summary>
        /// Updates all of the food pieces managed by the server to new positions 
        /// </summary>
        public void MoveFoodPieces ()
        {
            List<Amoeba> foodPieces = GetFoodPieces ();
            Parallel.ForEach (foodPieces, food =>
            {
                food.XCoordinate += food.randomX;
                food.YCoordinate += food.randomY;
            });
        }

        public void GenerateFood ()
        {
            Int32 currentFoodPieces = this.GetFoodPieces ().Count;

            while (currentFoodPieces < MaxFoodPieces)
            {
                Amoeba amoeba = CreateNewFood ();
                GamePieces.Add (amoeba.CellId, amoeba);
                currentFoodPieces++;
            }
        }

        /*
         * This function generates random color and coordinates for food objects. 
         * It also sets the texture and coordinates for the object 
         */
        public Amoeba CreateNewFood ()
        {
            Random randomNumberGen = new Random ();
            Amoeba newFood = new Amoeba (FoodSize);
            newFood.GenerateRotation ();
            newFood.GenerateVelocity ();
            newFood.XCoordinate = randomNumberGen.Next (0, this.Width);
            newFood.YCoordinate = randomNumberGen.Next (0, this.Height);
            return newFood;
        }


        /// <summary>
        /// Creates a new amoeba object and ensures that it is spawned in an open area
        /// </summary>
        /// <returns>The newly spawned amoeba</returns>
        public Amoeba PlaceNewPlayer (String playerName)
        {
            Random randomNumberGen = new Random ();
            Amoeba newAmoeba = new Amoeba (PlayerSize, playerName);
            List<Amoeba> players = this.GetPlayerPieces ();
            int randomX = randomNumberGen.Next (0, this.Width);
            int randomY = randomNumberGen.Next (0, this.Height);
     
            while(this.GamePieces.Values.Where (x => Math.Pow ((double)(x.XCoordinate - randomX), 2) +
                    Math.Pow ((double)(x.YCoordinate - randomY), 2) < (double)newAmoeba.Radius).ToList ().Count > 0)
            {
                randomX = randomNumberGen.Next (0, this.Width);
                randomY = randomNumberGen.Next (0, this.Height);
            }

            return newAmoeba;
        }

    }
}
