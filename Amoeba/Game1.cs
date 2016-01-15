using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Common;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Factories;
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
        Texture2D foodSkin;

        //Array to store different color foods
        string[] colorArray;

        //Random number generator to select random food color
        Random randomNumberGen;
        Random randomColorGen;
        string randomColor;
        int randomX, randomY;

        int drawFoodCount;

        //Used to project the farseer objects into the world
        World world;
        Body body;
        const float unitToPixel = 100.0f;
        const float pixelToUnit = 1 / unitToPixel;
        CircleShape farseerCircle;
        Vector2 playerPosition;
        Vector2 scale;

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
            playerAmoeba = new AmoebaGameModels.Amoeba(1, (decimal).02);//(Decimal) 2.2 * Convert.ToDecimal(Math.Pow(Convert.ToDouble(1), -0.439)));

            colorArray = new string[] { "BlueFood", "RedFood", "GreenFood", "YellowFood", "PinkFood" };
            randomNumberGen = new Random();
            randomColorGen = new Random();
            currentFoodPopulation = 0;

            //initialize the farseer CircleShape object with radius 0.5f and density 1f
            farseerCircle = new CircleShape(0.5f, 1f);

            foodAmoebaList = new List<AmoebaGameModels.Amoeba>();
            for (int i = 0; i < 100; i++)
            {
                foodAmoeba = new AmoebaGameModels.Amoeba();
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
            world = new World(new Vector2(0, 0));

            playerSkin = Content.Load<Texture2D>("AmoebaPlayer");
            
            Vector2 size = new Vector2(50, 50);
            body = BodyFactory.CreateCircle(world, size.X * pixelToUnit, 2, 1);
            body.BodyType = BodyType.Dynamic;
            body.Position = new Vector2((GraphicsDevice.Viewport.Width / 2.0f) * pixelToUnit, 0);
            
            
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
            //foreach (AmoebaGameModels.Amoeba randomFood in foodAmoebaList)
            //{
            //    if (playerFixture.OnCollision)
            //    {
                    
            //        currentFoodPopulation--;
            //    }
            //}    

            world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //increment drawFoodCount
            drawFoodCount++;

            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            // TODO: Add your drawing code here

            //Draw food every 2 seconds
            if (currentFoodPopulation < 100) {
                GetRandomsForFood(out foodAmoeba);
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

            ///Draw player stuff
            playerAmoeba.XCoordinate = (decimal)((Mouse.GetState().Position.X - playerAmoeba.XCoordinate) * playerAmoeba.Speed) + playerAmoeba.XCoordinate;
            playerAmoeba.YCoordinate = (decimal)((Mouse.GetState().Position.Y - playerAmoeba.YCoordinate) * playerAmoeba.Speed) + playerAmoeba.YCoordinate;
            playerPosition = new Vector2((float)playerAmoeba.XCoordinate, (float)playerAmoeba.YCoordinate);
            
            scale = new Vector2(50 / (float)playerSkin.Width, 50 / (float)playerSkin.Height);
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
            foodSkin = Content.Load <Texture2D>(randomColor);
            foodAmoeba.texture = foodSkin;
            foodAmoeba.XCoordinate = randomX;
            foodAmoeba.YCoordinate = randomY;
              
        }
    }
}
