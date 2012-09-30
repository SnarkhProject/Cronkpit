//System namespaces.
using System;
using System.Collections.Generic;
using System.Linq;
//XNA Namespaces
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
//Other Namespaces
using Cronkpit_Csharp.Screen_Handling;

namespace Cronkpit_Csharp
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class CronkPit : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Camera cam;

        KeyboardState oldState;
        KeyboardState newState;

        BaseScreen activeScreen;
        MainScreen actionScreen;
        StartMenu menuScreen;


        public CronkPit()
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
        Floor f1;
        Player p1;
        
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //shit with constructors
            f1 = new Floor(Content);
            p1 = new Player(Content, f1.random_valid_position());
            cam = new Camera(GraphicsDevice.Viewport.Bounds);
            //shit without constructors
            
            //then init the base
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

            menuScreen = new StartMenu(this, ref spriteBatch, Content.Load<SpriteFont>("Fonts/sfont"));
            Components.Add(menuScreen);
            menuScreen.hide();

            actionScreen = new MainScreen(this, f1, p1, cam, Content, ref spriteBatch);
            Components.Add(actionScreen);
            actionScreen.hide();

            activeScreen = menuScreen;
            activeScreen.show();
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            newState = Keyboard.GetState();
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (activeScreen == menuScreen)
            {
                if (checkKeys(Keys.Enter))
                {
                    if (menuScreen.get_selected_index() == 0)
                    {
                        activeScreen.hide();
                        activeScreen = actionScreen;
                        activeScreen.show();
                    }
                    if (menuScreen.get_selected_index() == 1)
                        this.Exit();
                }
            }

            base.Update(gameTime);
            oldState = newState;
        }

        private bool checkKeys(Keys theKey)
        {
            return newState.IsKeyUp(theKey) && oldState.IsKeyDown(theKey);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            base.Draw(gameTime);

            spriteBatch.End();
        }
    }
}
