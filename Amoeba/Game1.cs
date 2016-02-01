using MonoGame.Framework;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Amoeba
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        #region Game Related Fields
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Camera2D2 camera;
        KeyboardState keyboardState, prevKeyboardState;
        GameStates gameState;
        enum GameStates {Menu, Playing}
        float gameClockSeconds, gameClockMinutes, gameClockHours;
        EventHandler<TextInputEventArgs> onTextEntered;

        Quadtree quadTree;
        #endregion

        #region login screen related fields
        SpriteFont playerNameEntrySpriteFont;
        SpriteFont playerNameLabelSpriteFont;
        String playerNameString;
        Texture2D playerNameBox;
        #endregion

        #region Background Related Fields
        List<Vector2> starPositions;
        List<float> starSizes;
        Texture2D starTexture;
        SpriteFont radiusSpriteFont, playerSpriteFont, playerTextShadowSpriteFont, gameClockSpriteFont;

        Vector2 radiusFontPosition;
        #endregion

        #region Player and Food Related Fields
        public const int maxFoodPopulation = 1000;
        int currentFoodPopulation;

        AmoebaGameModels.Amoeba playerAmoeba;
        AmoebaGameModels.Amoeba foodAmoeba;
        List<AmoebaGameModels.Amoeba> foodAmoebaList;

        //Array to store different color foods
        string[] colorArray;

        Random randomNumberGen;
        Random randomColorGen;
        string randomColor;
        int randomX, randomY;

        Vector2 playerPosition;
        float scale2;
        float foodScale;
        Vector2 foodPosition;
        #endregion

        Vector2 v1;
        AmoebaGameModels.Amoeba food1, food2;
        int collide = 0;
        Rectangle nearbyFood;
        
        AmoebaGameModels.Amoeba targetFood;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            //Set the window size
            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 1000;
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
            //create a new player amoeba
            playerAmoeba = new AmoebaGameModels.Amoeba();

            quadTree = new Quadtree(0, new Rectangle(0, 0, 10000, 10000));

            gameState = GameStates.Menu;
            playerNameString = "";

            colorArray = new string[] { "asteroid" };
            randomNumberGen = new Random();
            randomColorGen = new Random();
            currentFoodPopulation = 0;

            playerAmoeba.XCoordinate = (double) graphics.PreferredBackBufferWidth / 2;
            playerAmoeba.YCoordinate = (double) graphics.PreferredBackBufferHeight / 2;

            foodAmoebaList = new List<AmoebaGameModels.Amoeba>();
            for (int i = 0; i < 100; i++)
            {
                CreateNewFood();                 
            }

            //Create and initialize the star positions and sizes
            //Store into lists to later reference and draw
            starPositions = new List<Vector2>();
            starSizes = new List<float>();
            for (int i = 0; i < 10000; i++)
            {
                //Generate positions for stars
                randomX = randomNumberGen.Next(0, 10000);
                randomY = randomNumberGen.Next(0, 10000);
                starPositions.Add(new Vector2(randomX, randomY));

                //Generate sizes for stars
                randomX = randomNumberGen.Next(10, 50);
                randomY = randomNumberGen.Next(1, 30);
                starSizes.Add(randomX);
                starSizes.Add(randomY);
            }


            Window.TextInput += HandleInput;

            this.IsMouseVisible = true;
            camera = new Camera2D2(GraphicsDevice.Viewport);
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
            
            playerAmoeba.Texture = Content.Load<Texture2D>("GreenPlayer");
            starTexture = Content.Load<Texture2D>("star");

            radiusSpriteFont = Content.Load<SpriteFont>("Radius");
            playerSpriteFont = Content.Load<SpriteFont>("PlayerFont");
            playerTextShadowSpriteFont = Content.Load<SpriteFont>("PlayerFontBackground");
            gameClockSpriteFont = Content.Load<SpriteFont>("GameClock");
            playerNameEntrySpriteFont = Content.Load<SpriteFont>("PlayerNameEntry");
            playerNameLabelSpriteFont = Content.Load<SpriteFont>("PlayerNameLabel");

            playerNameBox = Content.Load<Texture2D>("WhiteBackgroundLogin");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Unload any non ContentManager content here
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
         

            // TODO Camera zoom needs to be more substantial when player gets larger
            camera.Position = new Vector2((float) playerAmoeba.XCoordinate - graphics.PreferredBackBufferWidth / 2, (float) playerAmoeba.YCoordinate - graphics.PreferredBackBufferHeight / 2);            
            camera.Zoom = (float) 3/((float) playerAmoeba.Radius - 10) + .8f;

            radiusFontPosition = new Vector2((float) playerAmoeba.XCoordinate, (float) (playerAmoeba.YCoordinate - (double) playerAmoeba.Radius - 30));

            ////food collision stuff
            quadTree.clear();
            for (int i = 0; i < foodAmoebaList.Count; i++)
            {
                quadTree.insert(new Rectangle((int)foodAmoebaList[i].XCoordinate, (int)foodAmoebaList[i].YCoordinate, (int)foodAmoebaList[i].Radius, (int)foodAmoebaList[i].Radius));
            }


            //Returned objects are the objects that are next to the food amoeba at i
            ArrayList returnObjects = new ArrayList();
            
            for (int i = 0; i < foodAmoebaList.Count; i++)
            {
                returnObjects.Clear();
                returnObjects = quadTree.retrieve(returnObjects, new Rectangle((int)foodAmoebaList[i].XCoordinate, (int)foodAmoebaList[i].YCoordinate, (int)foodAmoebaList[i].Radius, (int)foodAmoebaList[i].Radius));
                targetFood = foodAmoebaList[i];
                v1 = new Vector2((float)targetFood.XCoordinate, (float)targetFood.YCoordinate);
                for (int x = 0; x < returnObjects.Count; x++)
                {
                    nearbyFood = (Rectangle) returnObjects[x];
                    
                    // Run collision detection algorithm between objects
                    // TODO Detecting Every object for some reason, bad. fix. FIX
                    //Console.WriteLine("TargetX: {0} \n" + 
                     //                  "TargetY: {1} \n" +
                      //                 "Nearby: {2} \n" +
                       //                "NearbyY: {3} ", );
                    if (targetFood.XCoordinate + (double) targetFood.Radius > nearbyFood.X &&
                        targetFood.YCoordinate + (double) targetFood.Radius > nearbyFood.Y)
                    {
                        OnFoodCollision();
                    }

                }
            }


            //Check for collisions
            collisionCheck();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);      
            var viewMatrix = camera.GetViewMatrix();
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, transformMatrix: viewMatrix);

            if (gameState == GameStates.Menu)
            {
                GraphicsDevice.Clear(Color.Black);
                
                //keyboard related code start
                drawLoginScreen();
                drawBackgroundObjects();
                drawFood();

                KeyboardState keyState = Keyboard.GetState();
                prevKeyboardState = keyState;
            }
            
            else if (gameState == GameStates.Playing)
            {
                gameClockSeconds = (float)gameTime.TotalGameTime.TotalSeconds;
                gameClockMinutes = (float)gameTime.TotalGameTime.TotalMinutes * .5f;
                gameClockHours = (float)gameTime.TotalGameTime.TotalHours;

                drawStrings();
                drawBackgroundObjects();
                drawFood();
                drawPlayer();
            }
       
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
            //TODO Add event listener to food to detect collisions with player

            foodAmoeba = new AmoebaGameModels.Amoeba((Decimal) 20);
            FoodCollision += HandleFoodCollision;

            //grab random color and x,y for food
            randomColor = colorArray[randomColorGen.Next(0, colorArray.Length - 1)];
            foodAmoeba.XCoordinate = randomNumberGen.Next(0, 10000);
            foodAmoeba.YCoordinate = randomNumberGen.Next(0, 10000);

            //Food successfully created
            foodAmoeba.Texture = Content.Load<Texture2D>(randomColor);

            currentFoodPopulation++;

            //Add food to the list of food
            foodAmoebaList.Add(foodAmoeba);          
        }

        public void drawLoginScreen()
        {
            spriteBatch.Draw(playerNameBox, new Rectangle(graphics.PreferredBackBufferWidth / 2 - 100, graphics.PreferredBackBufferHeight / 2 - 30, 200, 25), null, Color.White, 0f, new Vector2(0,0), SpriteEffects.None, 1f);
            Vector2 playerNameEntryOrigin = playerNameEntrySpriteFont.MeasureString(playerNameString) / 2;
            string playerNameLabelString = "Name:";
            Vector2 playerNameLabelOrigin = playerNameLabelSpriteFont.MeasureString(playerNameLabelString) / 2;
            spriteBatch.DrawString(playerNameLabelSpriteFont, playerNameLabelString, new Vector2(graphics.PreferredBackBufferWidth/2, graphics.PreferredBackBufferHeight/2 - 50),
                                    Color.White, 0f, playerNameLabelOrigin, 1f, SpriteEffects.None, .5f);
            spriteBatch.DrawString(playerNameEntrySpriteFont, playerNameString, new Vector2(graphics.PreferredBackBufferWidth/2, graphics.PreferredBackBufferHeight/2 -15),
                                    Color.Black, 0f, playerNameEntryOrigin, 1f, SpriteEffects.None, .5f);
        }

        //Use this function to draw the SpriteFonts into the game
        public void drawStrings() {
            string radiusString = "Radius: " + playerAmoeba.Radius;
            string playerString = playerNameString;
            string playerBackgroundString = playerNameString;
            string gameClockString = String.Format("Gametime: {0:00}:{1:00}:{2:00}", Math.Round(gameClockHours, 0) % 60, Math.Round(gameClockMinutes, 0) % 60, Math.Round(gameClockSeconds, 0) % 60);
            Vector2 FontOrigin = radiusSpriteFont.MeasureString(radiusString) / 2;
            Vector2 playerFontOrigin = playerSpriteFont.MeasureString(playerString) / 2;
            Vector2 gameClockOrigin = gameClockSpriteFont.MeasureString(gameClockString) / 2;

            spriteBatch.DrawString(radiusSpriteFont, radiusString, radiusFontPosition, Color.Green, 0, FontOrigin, 1.0f, SpriteEffects.None, .1f);

            spriteBatch.DrawString(playerTextShadowSpriteFont, playerBackgroundString, new Vector2((float)playerAmoeba.XCoordinate, (float)playerAmoeba.YCoordinate), Color.Black, 0f, playerFontOrigin, 1.1f, SpriteEffects.None, .1f);
            spriteBatch.DrawString(gameClockSpriteFont, gameClockString, new Vector2((float)playerAmoeba.XCoordinate + 700, (float)playerAmoeba.YCoordinate - 500), Color.White, 0f, gameClockOrigin, 1.0f, SpriteEffects.None, .1f);
        }

        public void drawFood()
        {
            //If game food population drops below 100, create new foods    
            if (currentFoodPopulation < maxFoodPopulation)
            {
                CreateNewFood();
            }

            //Draw food to screen
            foreach (AmoebaGameModels.Amoeba randomFood in foodAmoebaList)
            {
                if (randomFood.Texture != null)
                {
                    // TODO Check to see if food collides with another food, make them bounce off of eachother 
                    foodScale = (float)randomFood.Radius / (float)randomFood.Texture.Width;
                    //Add the "floating" velocity
                    randomFood.XCoordinate += (double)randomFood.randomX;
                    randomFood.YCoordinate += (double)randomFood.randomY;
                    foodPosition = new Vector2((float) randomFood.XCoordinate + (float)randomFood.Radius * .5f, (float)randomFood.YCoordinate + (float)randomFood.Radius * .5f + randomFood.Velocity.Y);

                    spriteBatch.Draw(randomFood.Texture, new Vector2(foodPosition.X, foodPosition.Y), null, Color.White, randomFood.Rotation += randomFood.Spin, new Vector2(randomFood.Texture.Width / 2, randomFood.Texture.Height / 2), foodScale, SpriteEffects.None, .3f);
                }
            }         
        }

        /* *****summary*****
         * This function draws the player texture to the screen
         */
        public void drawPlayer()
        {
            setNewPlayerCoordinates();
            ///Player information     
            scale2 = (float)playerAmoeba.Radius / (playerAmoeba.Texture.Width / 2f);

            float magicNumber = (float)Math.Pow(scale2, 200f * scale2);

            playerPosition = new Vector2((float)playerAmoeba.XCoordinate - ((float)playerAmoeba.Radius), (float)playerAmoeba.YCoordinate - ((float)playerAmoeba.Radius));
            Vector2 origin = new Vector2((float)Mouse.GetState().Position.X * magicNumber, (float)Mouse.GetState().Position.Y * magicNumber);

            //Draw player      Texture,    position,     rect,    color,   rot, origin, scale,     effects,      depth  
            spriteBatch.Draw(playerAmoeba.Texture, playerPosition, null, Color.White, 0f, origin, scale2, SpriteEffects.None, .2f);
        }

        /* *****summary*****
         * This function draws the stars onto the background of the game at varying sizes and locations
         */
        public void drawBackgroundObjects()
        {          
            for(int i = 0; i < 500; i++)
                spriteBatch.Draw(starTexture, starPositions[i], null, Color.White, 0f, new Vector2(0, 0), (starSizes[i] / starTexture.Width), SpriteEffects.None, .4f);           
        }

        /*
        * This method loops through each food object in the game and checks for a collision with the player
        * If a collision is detected, the food object is removed from the game, population decreases
        * 
        * TODO: Create listeners for each food object so that looping through each one becomes unnecessary
        */
        protected void collisionCheck()
        {
            
            //Try-catch to prevent game from crashing when removing an object
            try
            {
                foreach (AmoebaGameModels.Amoeba foodAmoeba in foodAmoebaList)
                {
                    
                    //Check to see if foodAmoeba collides with the player
                    if ((Math.Sqrt(Math.Pow((double)(foodAmoeba.XCoordinate - playerAmoeba.XCoordinate), 2) +
                        Math.Pow((double)(foodAmoeba.YCoordinate - playerAmoeba.YCoordinate), 2)) < (double)playerAmoeba.Radius) && gameState == GameStates.Playing)
                    {
                        playerAmoeba.Eat(foodAmoeba);
                        foodAmoebaList.Remove(foodAmoeba);
                        currentFoodPopulation--;
                    }
                    else if (foodAmoeba.XCoordinate > 10000)
                    {
                        foodAmoeba.XCoordinate = 0;
                    }
                    else if (foodAmoeba.XCoordinate < 0)
                    {
                        foodAmoeba.XCoordinate = 10000;
                    }
                    else if (foodAmoeba.YCoordinate > 10000)
                    {
                        foodAmoeba.YCoordinate = 0;
                    }
                    else if (foodAmoeba.YCoordinate < 0)
                    {
                        foodAmoeba.YCoordinate = 10000;
                    }

                }
            }
            catch (InvalidOperationException e)
            { }
        }

        /* *****summary*****
         * Invokes the event handler function HandleInput when input is detected
         */
        protected void TextEntered(object sender, TextInputEventArgs e) {
            if(onTextEntered != null)
                onTextEntered.Invoke(sender, e);
        }

        /* *****summary*****
         * This function handles and interrets what to do with user input from keyboard
         * Use this function to add more features or keys and special cases
         */
        protected void HandleInput(object sender, TextInputEventArgs e)
        {
            char charEntered = e.Character;

            if (charEntered == '\r')
            {
                gameState = GameStates.Playing;
            }
            else if (charEntered == '\b')
            {
                playerNameString = playerNameString.Remove(playerNameString.Length - 1);
            }
            else 
                playerNameString += charEntered;
        }


        public event EventHandler FoodCollision;

        public void HandleFoodCollision(object sender, EventArgs eventArgs)
        {
            // TODO Calculate new food velocity when two foods collide
            //u = (v * n / n * n) n: v is velocity, n is surface normal (x*x + y*y) - dot product of two vectors
            //Console.WriteLine(collide);
            //collide++;
            //Vector2 n1 = new Vector2(nearbyFood.X, nearbyFood.Y);
            //Vector2 u1 = ((v1 * n1)) * n1;
            //Vector2 w1 = v1 - u1;
            //Vector2 vP1 = w1 - u1;

            //Vector2 u2 = ((n1 * v1)) * v1;
            //Vector2 w2 = n1 - u2;
            //Vector2 vP2 = w2 - u2;

            //nearbyFood.X += (int)vP2.X;
            //targetFood.XCoordinate += vP1.X;
            //targetFood.YCoordinate += vP1.Y;
            //nearbyFood.Y += (int)vP2.Y;

            //targetFood.XCoordinate += -targetFood.XCoordinate;
            //targetFood.YCoordinate += -targetFood.YCoordinate;

            //w = v - u: new Vector(targetFood.X, targetFood.Y) - u;
            //v' = w - u: w parallel to wall, u perpendicular to wall
            //v' = 
        }

        public void OnFoodCollision()
        {
            if (FoodCollision != null)
                HandleFoodCollision(this, EventArgs.Empty);
        }

        // Determines the new location of the player for the next time it is drawn, stores in playerAmoeba.[X/Y]Coordinate
        protected void setNewPlayerCoordinates()
        {
            Decimal Xmouse = Mouse.GetState().Position.X + (decimal) camera.Position.X;
            Decimal Ymouse = Mouse.GetState().Position.Y + (decimal) camera.Position.Y;
            double Xplayer = playerAmoeba.XCoordinate;
            double Yplayer = playerAmoeba.YCoordinate;
            Decimal Xdif = Xmouse - (Decimal) Xplayer;
            Decimal Ydif = Ymouse - (Decimal) Yplayer;
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
                if (playerAmoeba.XCoordinate > 10000) 
                {
                    playerAmoeba.XCoordinate = 0;
                }
                else if (playerAmoeba.XCoordinate < 0)
                {
                    playerAmoeba.XCoordinate = 10000;
                    
                }
                if (playerAmoeba.YCoordinate > 10000)
                {
                    playerAmoeba.YCoordinate = 0;
                }
                else if (playerAmoeba.YCoordinate < 0)
                {
                    playerAmoeba.YCoordinate = 10000;
                }

                playerAmoeba.YCoordinate += (double) NewYdistance;
                playerAmoeba.XCoordinate += (double) NewXdistance;
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
                if (playerAmoeba.XCoordinate > 10000)
                {
                    playerAmoeba.XCoordinate = 0;
                }
                else if (playerAmoeba.XCoordinate < 0)
                {
                    playerAmoeba.XCoordinate = 10000;

                }
                if (playerAmoeba.YCoordinate > 10000)
                {
                    playerAmoeba.YCoordinate = 0;
                }
                else if (playerAmoeba.YCoordinate < 0)
                {
                    playerAmoeba.YCoordinate = 10000;
                }
                playerAmoeba.XCoordinate += (double) ( (Xdif * playerAmoeba.MaxTravelDistance) / playerAmoeba.Radius );
                playerAmoeba.YCoordinate += (double) ( (Ydif * playerAmoeba.MaxTravelDistance) / playerAmoeba.Radius );
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
