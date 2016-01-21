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
using Drawing;
using initi = Microsoft.Xna.Framework.Graphics;



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
        Texture2D playerSkin;

        //Texture that displays different food colors
        Texture2D foodSkin;

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

        Vector2 playerPosition, scale;

        AmoebaGameModels.Amoeba playerAmoeba;
        AmoebaGameModels.Amoeba foodAmoeba;

        List<AmoebaGameModels.Amoeba> foodAmoebaList;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            //Set the window size
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
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
            //DrawingHelper.Initialize(initi::GraphicsDevice);
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
            playerSkin = Content.Load<Texture2D>("AmoebaPlayer");
            
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

            // TODO: KEVIN FIIIIIXIXXXXXXXX
            #region KEVIN FIX IT
            Decimal Xmouse = Mouse.GetState().Position.X;
            Decimal Ymouse = Mouse.GetState().Position.Y;
            Decimal Xplayer = playerAmoeba.XCoordinate;
            Decimal Yplayer = playerAmoeba.YCoordinate;
            Decimal Xdif = Xmouse - Xplayer;
            Decimal Ydif = Ymouse - Yplayer;

            Decimal MTD;
            Decimal Angle;
            Decimal Opposite1;
            Decimal Adjacent1;
            Decimal NewXdistance;
            Decimal NewYdistance;
            int Quadrant;

            // Determine quadrant the mouse is in relative to player position
            if (Xdif > 0)
            {
                if (Ydif > 0)   { Quadrant = 1; }
                else            { Quadrant = 4; }
            }
            else
            {
                if (Ydif > 0)   { Quadrant = 2; }
                else            { Quadrant = 3; }
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
                    Adjacent1 = Math.Abs (Xdif);
                    break;
                case 3:
                    Opposite1 = Math.Abs (Xdif);
                    Adjacent1 = Math.Abs (Ydif);
                    break;
                case 4:
                    Opposite1 = Math.Abs (Ydif);
                    Adjacent1 = Xdif;
                    break;
                default:
                    Opposite1 = 0;
                    Adjacent1 = 0;
                    break;
            }

            // Use tan to determine the angle of interest
            Angle = (decimal) Math.Atan ((double) (Opposite1 / Adjacent1));

            switch (Quadrant)
            {
                case 1:
                    NewXdistance = playerAmoeba.MaxTravelDistance * (decimal) Math.Sin ((double) Angle);
                    NewYdistance = playerAmoeba.MaxTravelDistance * (decimal) Math.Cos ((double) Angle);
                    break;
                case 2:
                    NewXdistance = playerAmoeba.MaxTravelDistance * (decimal) Math.Cos((double)Angle) * -1;
                    NewYdistance = playerAmoeba.MaxTravelDistance * (decimal) Math.Sin((double)Angle);
                    break;
                case 3:
                    NewXdistance = playerAmoeba.MaxTravelDistance * (decimal) Math.Sin((double)Angle) * -1;
                    NewYdistance = playerAmoeba.MaxTravelDistance * (decimal) Math.Cos((double)Angle) * -1;
                    break;
                case 4:
                    NewXdistance = playerAmoeba.MaxTravelDistance * (decimal) Math.Cos((double)Angle);
                    NewYdistance = playerAmoeba.MaxTravelDistance * (decimal) Math.Sin((double)Angle) * -1;
                    break;
                default:
                    NewXdistance = 0;
                    NewYdistance = 0;
                    break;
            }

            if (playerAmoeba.Wordy == true)
            {
                Console.WriteLine(" ----------------------------------- ");
                Console.WriteLine("MouseX: " + Xmouse);
                Console.WriteLine("MouseY: " + Ymouse);
                Console.WriteLine("beginning X: " + Xplayer);
                Console.WriteLine("beginning Y: " + Yplayer);
                //Console.WriteLine("Speed: " + playerAmoeba.Speed);
            }
            
            // if it would be exceeding max travel distance in either direction...
            if (Math.Abs((Xdif) * playerAmoeba.Speed) > playerAmoeba.MaxTravelDistance || Math.Abs((Ydif) * playerAmoeba.Speed) > playerAmoeba.MaxTravelDistance)
            {
                if (playerAmoeba.Wordy == true) { Console.WriteLine("---Exceeding MTD"); }
                // if traveling farther X than Y...
                if (Math.Abs((Xdif) * playerAmoeba.Speed) > Math.Abs((Ydif) * playerAmoeba.Speed))
                {
                    if (playerAmoeba.Wordy == true) { Console.WriteLine("X direction is larger"); }

                    if (Xdif != 0)
                    {
                        if (Xmouse > Xplayer)
                        {
                            playerAmoeba.XCoordinate = Xplayer + playerAmoeba.MaxTravelDistance;
                        }
                        else if (Xmouse < Xplayer)
                        {
                            playerAmoeba.XCoordinate = Xplayer - playerAmoeba.MaxTravelDistance;
                        }
                        
                        // if Y also exceeds MTD
                        if (Math.Abs((Ydif) * playerAmoeba.Speed) > playerAmoeba.MaxTravelDistance)
                        {
                            if (Ymouse > Yplayer)
                            {
                                // Yplayer = Yplayer + |MTD / (Xmouse-Xplayer)| * Ymouse
                                playerAmoeba.YCoordinate = Yplayer + (Math.Abs(playerAmoeba.MaxTravelDistance / (Xdif)) * Ymouse);
                            }
                            else if(Ymouse < Yplayer)
                            {
                                playerAmoeba.YCoordinate = Yplayer - (Math.Abs(playerAmoeba.MaxTravelDistance / (Xdif)) * Ymouse);
                            }
                        }
                        else 
                        {
                            if (Ymouse > Yplayer)
                            {
                                playerAmoeba.YCoordinate = Yplayer + Math.Abs((Ydif) * playerAmoeba.Speed);
                            }
                            else if (Ymouse < Yplayer)
                            {
                                playerAmoeba.YCoordinate = Yplayer - Math.Abs((Ydif) * playerAmoeba.Speed);
                            }
                            
                        }
                    }
                    if (playerAmoeba.Wordy == true)
                    {
                        Console.WriteLine("New X: " + playerAmoeba.XCoordinate);
                        Console.WriteLine("New Y: " + playerAmoeba.YCoordinate);
                    }

                }
                // else, since traveling farther Y than X...
                else
                {
                    if (Ydif != 0)
                    {
                        if (Ymouse > Yplayer)
                        {
                            playerAmoeba.YCoordinate = Yplayer + playerAmoeba.MaxTravelDistance;
                        }
                        else if (Ymouse < Yplayer)
                        {
                            playerAmoeba.YCoordinate = Yplayer - playerAmoeba.MaxTravelDistance;
                        }

                        // if X also exceeds MTD
                        if (Math.Abs((Xdif) * playerAmoeba.Speed) > playerAmoeba.MaxTravelDistance)
                        {
                            if (Xmouse > Xplayer)
                            {
                                // Yplayer = Yplayer + |MTD / (Xmouse-Xplayer)| * Ymouse
                                playerAmoeba.XCoordinate = Xplayer + (Math.Abs(playerAmoeba.MaxTravelDistance / (Ydif)) * Xmouse);
                            }
                            else if (Xmouse < Xplayer)
                            {
                                playerAmoeba.XCoordinate = Xplayer - (Math.Abs(playerAmoeba.MaxTravelDistance / (Ydif)) * Xmouse);
                            }
                        }
                        else
                        {
                            if (Xmouse > Xplayer)
                            {
                                playerAmoeba.XCoordinate = Xplayer + Math.Abs((Xdif) * playerAmoeba.Speed);
                            }
                            else if (Xmouse < Xplayer)
                            {
                                playerAmoeba.XCoordinate = Xplayer - Math.Abs((Xdif) * playerAmoeba.Speed);
                            }
                        }
                    }
                    if (playerAmoeba.Wordy == true)
                    {
                        Console.WriteLine("New X: " + playerAmoeba.XCoordinate);
                        Console.WriteLine("New Y: " + playerAmoeba.YCoordinate);
                    }
                }
            }
            // else, not exceeding max travel distance so calculate new coordinates
            else
            {
                playerAmoeba.XCoordinate = Xplayer + ((Xdif) * playerAmoeba.Speed);
                playerAmoeba.YCoordinate = Yplayer + ((Ydif) * playerAmoeba.Speed);

                if (playerAmoeba.Wordy == true)
                {
                    Console.WriteLine("---Not Exceeding MTD");
                    Console.WriteLine("New X: " + playerAmoeba.XCoordinate);
                    Console.WriteLine("New Y: " + playerAmoeba.YCoordinate);
                }
            }
            #endregion


            //If game food population drops below 100, create new foods    
            if (currentFoodPopulation < 100) {
                CreateNewFood();
            }

            //Draw food to screen
            foreach (AmoebaGameModels.Amoeba randomFood in foodAmoebaList)
            {               
                if (randomFood.Texture != null)
                {
                    Vector2 foodScale = new Vector2(((float)randomFood.Radius / (float)randomFood.Texture.Width), ((float)randomFood.Radius) / (float)randomFood.Texture.Height);
                    Vector2 foodPosition = new Vector2((float)randomFood.XCoordinate, (float)randomFood.YCoordinate);
                    
                    spriteBatch.Draw(randomFood.Texture, foodPosition, null, Color.White, 0f, new Vector2(randomFood.Texture.Width / 2.0f, randomFood.Texture.Height / 2.0f), foodScale, SpriteEffects.None, 0);                    
                }
            }         
            ///Player information     
            scale = new Vector2(((float) playerAmoeba.Radius / playerSkin.Width), ((float) playerAmoeba.Radius) / playerSkin.Height);
          
            playerPosition = new Vector2((float)playerAmoeba.XCoordinate, (float)playerAmoeba.YCoordinate);
            //Draw player     Texture,   Vector position,  rect,    color,  rot,          origin vector ,                                       scale size ,  effects,    depth  
            spriteBatch.Draw(playerSkin, playerPosition, null, Color.White, 0f, new Vector2(playerSkin.Width / 2.0f, playerSkin.Height / 2.0f), scale, SpriteEffects.None, 0);
            
            spriteBatch.End();
            base.Draw(gameTime);
        }

        /*
         * This function generates random color and coordinates for food objects. 
         * It also sets the texture and coordinates for the object 
         */
        protected void CreateNewFood()
        {
            foodAmoeba = new AmoebaGameModels.Amoeba((Decimal) 25);
            //grab random color and x,y for food
            randomColor = colorArray[randomColorGen.Next(0, colorArray.Length - 1)];
            randomX = randomNumberGen.Next(0, 1024);
            randomY = randomNumberGen.Next(0, 768);

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
            //Try catch to prevent game from crashing when removing an object
            try
            {
                foreach (AmoebaGameModels.Amoeba foodAmoeba in foodAmoebaList)
                {
                    //TODO: Fix collision detection
                    if (((int)Math.Abs(foodAmoeba.XCoordinate - playerAmoeba.XCoordinate) < (double)playerAmoeba.Radius) && 
                        ((int)Math.Abs(foodAmoeba.YCoordinate - playerAmoeba.YCoordinate)) < (double)playerAmoeba.Radius)
                    {
                        playerAmoeba.Eat(foodAmoeba);
                        foodAmoebaList.Remove(foodAmoeba);
                        currentFoodPopulation--;
                    }
                }
            }
            catch (InvalidOperationException e)
            {}
        }
    }
}
