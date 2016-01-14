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
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D playerTexture;

        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        AmoebaGameModels.Amoeba playerAmoeba;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
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

            playerTexture = Content.Load<Texture2D>("AmoebaPlayer");

            // TODO: use this.Content to load your game content here
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
                //Console.WriteLine("Slope:" + playerAmoeba.Slope);
                //Console.WriteLine("PLayerX:" + playerAmoeba.XCoordinate);
                //Console.WriteLine("PlayerY:" + playerAmoeba.YCoordinate);
                //Console.WriteLine("MouseX:" + Mouse.GetState().Position.X);
                //Console.WriteLine("MouseY:" + Mouse.GetState().Position.Y);
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();
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

            //playerAmoeba.XCoordinate = (decimal)((Mouse.GetState().Position.X - playerAmoeba.XCoordinate) * playerAmoeba.Speed) + playerAmoeba.XCoordinate;
            //playerAmoeba.YCoordinate = (decimal)((Mouse.GetState().Position.Y - playerAmoeba.YCoordinate) * playerAmoeba.Speed) + playerAmoeba.YCoordinate;
            spriteBatch.Draw(playerTexture, new Vector2((float) playerAmoeba.XCoordinate, (float) playerAmoeba.YCoordinate ), Color.White);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
