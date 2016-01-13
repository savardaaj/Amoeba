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
            playerAmoeba = new AmoebaGameModels.Amoeba(1, (decimal).02);//(Decimal) 2.2 * Convert.ToDecimal(Math.Pow(Convert.ToDouble(1), -0.439)));

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
            Console.WriteLine("MouseX: " + Mouse.GetState().Position.X);
            Console.WriteLine("MouseY: " + Mouse.GetState().Position.Y);
            Console.WriteLine("playerX: " + playerAmoeba.XCoordinate);
            Console.WriteLine("playerY: " + playerAmoeba.YCoordinate);
            Console.WriteLine("Speed: " + playerAmoeba.Speed);


            playerAmoeba.XCoordinate = (decimal)((Mouse.GetState().Position.X - playerAmoeba.XCoordinate) * playerAmoeba.Speed) + playerAmoeba.XCoordinate;
            playerAmoeba.YCoordinate = (decimal)((Mouse.GetState().Position.Y - playerAmoeba.YCoordinate) * playerAmoeba.Speed) + playerAmoeba.YCoordinate;
            spriteBatch.Draw(playerTexture, new Vector2((float) playerAmoeba.XCoordinate, (float) playerAmoeba.YCoordinate ), Color.White);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
