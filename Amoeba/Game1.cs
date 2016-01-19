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
        //CircleShape farseerCircle, foodCircle;
        Vector2 playerPosition, scale;

        //Fixtures attach shapes to bodies
        //
        Fixture playerFixture, foodFixture;

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

            //create a new player amoeba
            playerAmoeba = new AmoebaGameModels.Amoeba();//(1, (decimal).02);//(Decimal) 2.2 * Convert.ToDecimal(Math.Pow(Convert.ToDouble(1), -0.439)));

            colorArray = new string[] { "BlueFood", "RedFood", "GreenFood", "YellowFood", "PinkFood" };
            randomNumberGen = new Random();
            randomColorGen = new Random();
            currentFoodPopulation = 0;

            foodAmoebaList = new List<AmoebaGameModels.Amoeba>();
            for (int i = 0; i < 100; i++)
            {
                foodAmoeba = new AmoebaGameModels.Amoeba(world, ((Decimal) ((float).15 * pixelToUnit)));
                GetRandomsForFood(out foodAmoeba);              
                foodAmoebaList.Add(foodAmoeba);              
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

            //TODO: Collision detection 
            

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
            // TODO: Add your drawing code here

            Decimal Xmouse = Mouse.GetState().Position.X;
            Decimal Ymouse = Mouse.GetState().Position.Y;
            Decimal Xplayer = playerAmoeba.XCoordinate;
            Decimal Yplayer = playerAmoeba.YCoordinate;

            if (playerAmoeba.Wordy == true)
            {
                Console.WriteLine(" ----------------------------------- ");
                Console.WriteLine("MouseX: " + Xmouse);
                Console.WriteLine("MouseY: " + Ymouse);
                Console.WriteLine("beginning X: " + Xplayer);
                Console.WriteLine("beginning Y: " + Yplayer);
                //Console.WriteLine("Speed: " + playerAmoeba.Speed);
            }
            #region calculate position KEVIN
            // if it would be exceeding max travel distance in either direction...
            if (Math.Abs((Xmouse - Xplayer) * playerAmoeba.Speed) > playerAmoeba.MaxTravelDistance || Math.Abs((Ymouse - Yplayer) * playerAmoeba.Speed) > playerAmoeba.MaxTravelDistance)
            {
                if (playerAmoeba.Wordy == true) { Console.WriteLine("---Exceeding MTD"); }
                // if traveling farther X than Y...
                if (Math.Abs((Xmouse - Xplayer) * playerAmoeba.Speed) > Math.Abs((Ymouse - Yplayer) * playerAmoeba.Speed))
                {
                    if (playerAmoeba.Wordy == true) { Console.WriteLine("X direction is larger"); }

                    if (Xmouse - Xplayer != 0)
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
                        if (Math.Abs((Ymouse - Yplayer) * playerAmoeba.Speed) > playerAmoeba.MaxTravelDistance)
                        {
                            if (Ymouse > Yplayer)
                            {
                                // Yplayer = Yplayer + |MTD / (Xmouse-Xplayer)| * Ymouse
                                playerAmoeba.YCoordinate = Yplayer + (Math.Abs(playerAmoeba.MaxTravelDistance / (Xmouse - Xplayer)) * Ymouse);
                            }
                            else if(Ymouse < Yplayer)
                            {
                                playerAmoeba.YCoordinate = Yplayer - (Math.Abs(playerAmoeba.MaxTravelDistance / (Xmouse - Xplayer)) * Ymouse);
                            }
                        }
                        else 
                        {
                            if (Ymouse > Yplayer)
                            {
                                playerAmoeba.YCoordinate = Yplayer + Math.Abs((Ymouse - Yplayer) * playerAmoeba.Speed);
                            }
                            else if (Ymouse < Yplayer)
                            {
                                playerAmoeba.YCoordinate = Yplayer - Math.Abs((Ymouse - Yplayer) * playerAmoeba.Speed);
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
                    if (Ymouse - Yplayer != 0)
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
                        if (Math.Abs((Xmouse - Xplayer) * playerAmoeba.Speed) > playerAmoeba.MaxTravelDistance)
                        {
                            if (Xmouse > Xplayer)
                            {
                                // Yplayer = Yplayer + |MTD / (Xmouse-Xplayer)| * Ymouse
                                playerAmoeba.XCoordinate = Xplayer + (Math.Abs(playerAmoeba.MaxTravelDistance / (Ymouse - Yplayer)) * Xmouse);
                            }
                            else if (Xmouse < Xplayer)
                            {
                                playerAmoeba.XCoordinate = Xplayer - (Math.Abs(playerAmoeba.MaxTravelDistance / (Ymouse - Yplayer)) * Xmouse);
                            }
                        }
                        else
                        {
                            if (Xmouse > Xplayer)
                            {
                                playerAmoeba.XCoordinate = Xplayer + Math.Abs((Xmouse - Xplayer) * playerAmoeba.Speed);
                            }
                            else if (Xmouse < Xplayer)
                            {
                                playerAmoeba.XCoordinate = Xplayer - Math.Abs((Xmouse - Xplayer) * playerAmoeba.Speed);
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
                playerAmoeba.XCoordinate = Xplayer + ((Xmouse - Xplayer) * playerAmoeba.Speed);
                playerAmoeba.YCoordinate = Yplayer + ((Ymouse - Yplayer) * playerAmoeba.Speed);

                if (playerAmoeba.Wordy == true)
                {
                    Console.WriteLine("---Not Exceeding MTD");
                    Console.WriteLine("New X: " + playerAmoeba.XCoordinate);
                    Console.WriteLine("New Y: " + playerAmoeba.YCoordinate);
                }
            }
            #endregion

            ///Food information
            //Draw food every 2 seconds
            if (currentFoodPopulation < 100) {
                GetRandomsForFood(out foodAmoeba);
                //Need to add new food to list array
                //Need to generate a replacement for food that eaten
            }

            //Draw food to screen
            foreach (AmoebaGameModels.Amoeba randomFood in foodAmoebaList)
            {
                if (randomFood.texture != null)
                {
                    spriteBatch.Draw(randomFood.texture, new Vector2((float)randomFood.XCoordinate, (float)randomFood.YCoordinate), Color.White);
                    currentFoodPopulation++;
                }
            }         
            ///Player information     
            scale = new Vector2(((float) playerAmoeba.Size*200) / (float) playerSkin.Width, ((float) playerAmoeba.Size*200) / (float)playerSkin.Height);
          
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
        protected void GetRandomsForFood(out AmoebaGameModels.Amoeba foodAmoeba)
        {
            foodAmoeba = new AmoebaGameModels.Amoeba();
            //grab random color and x,y for food
            randomColor = colorArray[randomColorGen.Next(0, colorArray.Length - 1)];
            randomX = randomNumberGen.Next(0, 1024);
            randomY = randomNumberGen.Next(0, 768);

            //Set texture
            foodSkin = Content.Load <Texture2D>(randomColor);
            foodAmoeba.texture = foodSkin;

            //Set x and y
            foodAmoeba.XCoordinate = randomX;
            foodAmoeba.YCoordinate = randomY;           
        }
        
        private bool OnCollision(Fixture b1, Fixture b2, Contact contact)
        {
            return true;
        }
    }
}
