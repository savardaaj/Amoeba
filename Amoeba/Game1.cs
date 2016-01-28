using FarseerPhysics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Common;
using FarseerPhysics.Collision;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision.Shapes; 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

namespace Amoeba
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public const int maxFoodPopulation = 100;
        int currentFoodPopulation;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Texture that displays orange ball for plaer
        Texture2D playerSkin, testSkin, foodSkin;

        //Array to store different color foods
        string[] colorArray;

        //Random number generator to select random food color
        Random randomNumberGen;
        Random randomColorGen;
        string randomColor;
        int randomX, randomY;

        //Used to project the farseer objects into the world
        World world;
        //Body playerBody, foodBody;
        const float unitToPixel = 100.0f;
        const float pixelToUnit = 1 / unitToPixel;

        Vector2 playerPosition;
        float scale, scale2, scale3;

        AmoebaGameModels.Amoeba playerAmoeba;
        AmoebaGameModels.Amoeba foodAmoeba;

        List<AmoebaGameModels.Amoeba> foodAmoebaList;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            //Set the window size
            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 1000;
            graphics.ApplyChanges();

            world = new World(new Vector2(0, 0));
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            //create a new player amoeba
            playerAmoeba = new AmoebaGameModels.Amoeba();

            colorArray = new string[] { "BlueFood", "RedFood", "GreenFood", "YellowFood", "PinkFood" };
            randomNumberGen = new Random();
            randomColorGen = new Random();
            currentFoodPopulation = 0;

            foodAmoebaList = new List<AmoebaGameModels.Amoeba>();
            for (int i = 0; i < 100; i++)
            {
                CreateNewFood();                 
            }

            this.IsMouseVisible = true;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
                
            // TODO: use this.Content to load your game content here
            playerSkin = Content.Load<Texture2D>("GreenPlayer");
            //testSkin = Content.Load<Texture2D>("TestSkin");

            playerAmoeba.Texture = playerSkin;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            //m = y2 - y1 / x2 - x1
            if (Mouse.GetState().Position.X - playerAmoeba.XCoordinate != 0)
            {
                playerAmoeba.Slope = (Mouse.GetState().Position.Y - playerAmoeba.YCoordinate) /
                                     (Mouse.GetState().Position.X - playerAmoeba.XCoordinate);
            }

            //Check for collisions
            collisionCheck();

            world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            setNewPlayerCoordinates(); 

            //If game food population drops below 100, create new foods    
            if (currentFoodPopulation < 1000) {
                CreateNewFood();
            }

            //Draw food to screen
            foreach (AmoebaGameModels.Amoeba randomFood in foodAmoebaList)
            {               
                if (randomFood.Texture != null)
                {
                    float foodScale = (float) randomFood.Radius / (float)randomFood.Texture.Width;
                    Vector2 foodPosition = new Vector2((float)randomFood.XCoordinate + (float)randomFood.Radius * .5f, (float)randomFood.YCoordinate + (float)randomFood.Radius * .5f);
              
                    spriteBatch.Draw(randomFood.Texture, foodPosition, null, Color.White, 0f, new Vector2(randomFood.Texture.Width, randomFood.Texture.Height), foodScale, SpriteEffects.None, 0);      
                
                }
            }         
            ///Player information     
            scale = (float)playerAmoeba.Radius / playerAmoeba.Texture.Width;
            scale2 = (float)playerAmoeba.Radius / (playerSkin.Width / 2f);
            scale3 = (float)(((float)playerAmoeba.Radius / (playerSkin.Width / 2f)) * (1-.9596+1));
            float scale4 = (float)playerAmoeba.Radius / playerSkin.Width;

            float magicNumber = (float)Math.Pow(scale2, 200f * scale2);



            playerPosition = new Vector2((float)playerAmoeba.XCoordinate - ((float)playerAmoeba.Radius) , (float)playerAmoeba.YCoordinate - ((float)playerAmoeba.Radius));
            Vector2 origin = new Vector2((float)Mouse.GetState().Position.X * magicNumber, (float)Mouse.GetState().Position.Y * magicNumber);

            //Draw player      Texture,    position,     rect,    color,   rot, origin, scale,     effects,      depth  
            spriteBatch.Draw(playerSkin, playerPosition, null, Color.White, 0f, origin, scale2, SpriteEffects.None, 0);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        /*
         * This function generates random color and coordinates for food objects between 0 and screen height and screen width. 
         * Creates new food objects at radius = 20
         * It also sets the texture and coordinates for the object 
         */
        protected void CreateNewFood()
        {
            foodAmoeba = new AmoebaGameModels.Amoeba((Decimal) 20);
            //grab random color and x,y for food
            randomColor = colorArray[randomColorGen.Next(0, colorArray.Length - 1)];
            randomX = randomNumberGen.Next(0, graphics.PreferredBackBufferWidth);
            randomY = randomNumberGen.Next(0, graphics.PreferredBackBufferHeight);

            //Set x and y
            foodAmoeba.XCoordinate = randomX;
            foodAmoeba.YCoordinate = randomY;

            //Food successfully created
            foodSkin = Content.Load<Texture2D>(randomColor);
            foodAmoeba.Texture = foodSkin;

            currentFoodPopulation++;

            //Add food to the list of food
            foodAmoebaList.Add(foodAmoeba);
            
        }

        protected void collisionCheck()
        {           
            //Try-catch to prevent game from crashing when removing an object
            try
            {
                foreach (AmoebaGameModels.Amoeba foodAmoeba in foodAmoebaList)
                {
                    Decimal centerx = playerAmoeba.XCoordinate + (playerAmoeba.Radius / 2);
                    Decimal centery = playerAmoeba.YCoordinate + (playerAmoeba.Radius / 2);

                    //TODO: Fix collision detection, radius is not properly represented for collision

                    if (Math.Sqrt(Math.Pow((double)(foodAmoeba.XCoordinate - playerAmoeba.XCoordinate), 2) + Math.Pow((double)(foodAmoeba.YCoordinate - playerAmoeba.YCoordinate), 2)) < (double)playerAmoeba.Radius )
                    {
                        playerAmoeba.Eat(foodAmoeba);
                        foodAmoebaList.Remove(foodAmoeba);
                        currentFoodPopulation--;
                        //Console.WriteLine("Radius: " + playerAmoeba.Radius);
                        //Console.WriteLine("X: " + playerAmoeba.XCoordinate);
                        //Console.WriteLine("Y: " + playerAmoeba.YCoordinate);
                        //Console.WriteLine("Scale: " + scale);
                    }
                }
            }
            catch (InvalidOperationException e)
            {}
        }

        // Determines the new location of the player for the next time it is drawn, stores in playerAmoeba.[X/Y]Coordinate
        protected void setNewPlayerCoordinates()
        {
            Decimal Xmouse = Mouse.GetState().Position.X;
            Decimal Ymouse = Mouse.GetState().Position.Y;
            Decimal Xplayer = playerAmoeba.XCoordinate;
            Decimal Yplayer = playerAmoeba.YCoordinate;
            Decimal Xdif = Xmouse - Xplayer;
            Decimal Ydif = Ymouse - Yplayer;
            Decimal MousePlayerDist = (decimal)Math.Sqrt(Math.Pow((double)Xdif, 2) + Math.Pow((double)Ydif, 2)); // pathagorean theorem

            Decimal Angle;
            Decimal Opposite1;
            Decimal Adjacent1;
            Decimal NewXdistance;
            Decimal NewYdistance;
            int Quadrant;


            if (playerAmoeba.Wordy == true) { 
                Console.WriteLine(" ----------------------------------- ");
                Console.WriteLine("MouseX: " + Xmouse + ", " + Mouse.GetState().Position.X);
                Console.WriteLine("MouseY: " + Ymouse + ", " + Mouse.GetState().Position.Y);
                Console.WriteLine("beginning X: " + Xplayer + ", " + playerAmoeba.XCoordinate);
                Console.WriteLine("beginning Y: " + Yplayer + ", " + playerAmoeba.YCoordinate);
            }

            // Determine quadrant the mouse is in relative to player position
            if (Xdif > 0)
            {
                if (Ydif > 0) { Quadrant = 1; }
                else { Quadrant = 4; }
            }
            else
            {
                if (Ydif > 0) { Quadrant = 2; }
                else { Quadrant = 3; }
            }

            // Set triangle side values for triangle between mouse and player position
            switch (Quadrant)
            {
                case 1:
                    Opposite1 = Xdif;
                    Adjacent1 = Ydif;
                    break;
                case 2:
                    Opposite1 = Ydif;
                    Adjacent1 = Math.Abs(Xdif);
                    break;
                case 3:
                    Opposite1 = Math.Abs(Xdif);
                    Adjacent1 = Math.Abs(Ydif);
                    break;
                case 4:
                    Opposite1 = Math.Abs(Ydif);
                    Adjacent1 = Xdif;
                    break;
                default:
                    Opposite1 = 0;
                    Adjacent1 = 0;
                    break;
            }

            // Use tan to determine the angle of interest
            if (Adjacent1 == 0)
            {
                Angle = 0;
            }
            else
            {
                Angle = (decimal)Math.Atan((double)(Opposite1 / Adjacent1));
            }

            switch (Quadrant)
            {
                case 1:
                    NewXdistance = playerAmoeba.MaxTravelDistance * (decimal)Math.Sin((double)Angle);
                    NewYdistance = playerAmoeba.MaxTravelDistance * (decimal)Math.Cos((double)Angle);
                    break;
                case 2:
                    NewXdistance = playerAmoeba.MaxTravelDistance * (decimal)Math.Cos((double)Angle) * -1;
                    NewYdistance = playerAmoeba.MaxTravelDistance * (decimal)Math.Sin((double)Angle);
                    break;
                case 3:
                    NewXdistance = playerAmoeba.MaxTravelDistance * (decimal)Math.Sin((double)Angle) * -1;
                    NewYdistance = playerAmoeba.MaxTravelDistance * (decimal)Math.Cos((double)Angle) * -1;
                    break;
                case 4:
                    NewXdistance = playerAmoeba.MaxTravelDistance * (decimal)Math.Cos((double)Angle);
                    NewYdistance = playerAmoeba.MaxTravelDistance * (decimal)Math.Sin((double)Angle) * -1;
                    break;
                default:
                    NewXdistance = 0;
                    NewYdistance = 0;
                    break;
            }

            if (MousePlayerDist > playerAmoeba.Radius)
            {

                playerAmoeba.XCoordinate += NewXdistance;
                playerAmoeba.YCoordinate += NewYdistance;
                playerAmoeba.XSpeed = NewXdistance;
                playerAmoeba.YSpeed = NewYdistance;

                if (playerAmoeba.Wordy == true)
                {
                    Console.WriteLine("Mouse outside radius");
                    Console.WriteLine("New X dist: " + NewXdistance);
                    Console.WriteLine("New Y dist: " + NewYdistance);
                    Console.WriteLine("New X: " + playerAmoeba.XCoordinate);
                    Console.WriteLine("New Y: " + playerAmoeba.YCoordinate);
                    //Console.WriteLine("Speed: " + playerAmoeba.Speed);
                }
            }
            else
            {
                playerAmoeba.XCoordinate += ( (Xdif * playerAmoeba.MaxTravelDistance) / playerAmoeba.Radius );
                playerAmoeba.YCoordinate += ( (Ydif * playerAmoeba.MaxTravelDistance) / playerAmoeba.Radius );
                playerAmoeba.XSpeed = (Xdif * playerAmoeba.MaxTravelDistance) / playerAmoeba.Radius;
                playerAmoeba.YSpeed = (Ydif * playerAmoeba.MaxTravelDistance) / playerAmoeba.Radius;

                if (playerAmoeba.Wordy == true)
                {
                    Console.WriteLine("Mouse inside radius");
                    Console.WriteLine("New X dist: " + (Xdif * playerAmoeba.MaxTravelDistance) / playerAmoeba.Radius);
                    Console.WriteLine("New Y dist: " + (Ydif * playerAmoeba.MaxTravelDistance) / playerAmoeba.Radius);
                    Console.WriteLine("New X: " + playerAmoeba.XCoordinate);
                    Console.WriteLine("New Y: " + playerAmoeba.YCoordinate);
                    //Console.WriteLine("Speed: " + playerAmoeba.Speed);
                }
            }

        } // end setNewPlayerCoordinates


    }
}
