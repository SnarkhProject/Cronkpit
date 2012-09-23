using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Cronkpit_Csharp
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState oldState;
        Camera cam;

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
        Floor f1;
        Player p1;
        SpriteFont sfont_thesecond;
        bool bad_turn;
        bool victory_condition;
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //shit with constructors
            f1 = new Floor(Content);
            p1 = new Player(Content, f1.random_valid_position());
            cam = new Camera(GraphicsDevice.Viewport.Bounds);
            //shit without constructors
            bad_turn = false;
            victory_condition = false;
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
            sfont_thesecond = Content.Load<SpriteFont>("sfont");
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            updateInput();
            if (p1.is_spot_exit(f1))
                victory_condition = true;

            if (bad_turn)
            {
                f1.update_dungeon_floor(p1);
                bad_turn = false;
            }

            cam.Pos = p1.get_my_Position();
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        private void updateInput()
        {
            KeyboardState newState = Keyboard.GetState();
            //Down Left first
            if (newState.IsKeyDown(Keys.NumPad1))
            {
                // If not down last update, key has just been pressed.
                if (!oldState.IsKeyDown(Keys.NumPad1))
                {
                    if (p1.is_alive() && !victory_condition)
                    {
                        p1.move("downleft", f1);
                        bad_turn = true;
                    }
                }
            }
            else if (oldState.IsKeyDown(Keys.NumPad2))
            {
                // Key was down last update, but not down now, so
                // it has just been released. Do nothing.
            }

            // Is the DOWN key down?
            if (newState.IsKeyDown(Keys.NumPad2))
            {
                // If not down last update, key has just been pressed.
                if (!oldState.IsKeyDown(Keys.NumPad2))
                {
                    if (p1.is_alive() && !victory_condition)
                    {
                        p1.move("down", f1);
                        bad_turn = true;
                    }
                }
            }
            else if (oldState.IsKeyDown(Keys.NumPad2))
            {
                // Key was down last update, but not down now, so
                // it has just been released. Do nothing.
            }

            // Is the DOWN RIGHT key down?
            if (newState.IsKeyDown(Keys.NumPad3))
            {
                // If not down last update, key has just been pressed.
                if (!oldState.IsKeyDown(Keys.NumPad3))
                {
                    if (p1.is_alive() && !victory_condition)
                    {
                        p1.move("downright", f1);
                        bad_turn = true;
                    }
                }
            }
            else if (oldState.IsKeyDown(Keys.NumPad3))
            {
                // Key was down last update, but not down now, so
                // it has just been released. Do nothing.
            }

            // Is the UP key down?
            if (newState.IsKeyDown(Keys.NumPad8))
            {
                // If not down last update, key has just been pressed.
                if (!oldState.IsKeyDown(Keys.NumPad8))
                {
                    if (p1.is_alive() && !victory_condition)
                    {
                        p1.move("up", f1);
                        bad_turn = true;
                    }
                }
            }
            else if (oldState.IsKeyDown(Keys.NumPad8))
            {
                // Key was down last update, but not down now, so
                // it has just been released. Do nothing.
            }

            if (newState.IsKeyDown(Keys.NumPad7))
            {
                // If not down last update, key has just been pressed.
                if (!oldState.IsKeyDown(Keys.NumPad7))
                {
                    if (p1.is_alive() && !victory_condition)
                    {
                        p1.move("upleft", f1);
                        bad_turn = true;
                    }
                }
            }
            else if (oldState.IsKeyDown(Keys.NumPad7))
            {
                // Key was down last update, but not down now, so
                // it has just been released. Do nothing.
            }

            if (newState.IsKeyDown(Keys.NumPad9))
            {
                // If not down last update, key has just been pressed.
                if (!oldState.IsKeyDown(Keys.NumPad9))
                {
                    if (p1.is_alive() && !victory_condition)
                    {
                        p1.move("upright", f1);
                        bad_turn = true;
                    }
                }
            }
            else if (oldState.IsKeyDown(Keys.NumPad9))
            {
                // Key was down last update, but not down now, so
                // it has just been released. Do nothing.
            }

            // Is the LEFT key down?
            if (newState.IsKeyDown(Keys.NumPad4))
            {
                // If not down last update, key has just been pressed.
                if (!oldState.IsKeyDown(Keys.NumPad4))
                {
                    if (p1.is_alive() && !victory_condition)
                    {
                        p1.move("left", f1);
                        bad_turn = true;
                    }
                }
            }

            else if (oldState.IsKeyDown(Keys.NumPad4))
            {
                // Key was down last update, but not down now, so
                // it has just been released. Do nothing.
            }

            // Is the RIGHT key down?
            if (newState.IsKeyDown(Keys.NumPad6))
            {
                // If not down last update, key has just been pressed.
                if (!oldState.IsKeyDown(Keys.NumPad6))
                {
                    if (p1.is_alive() && !victory_condition)
                    {
                        p1.move("right", f1);
                        bad_turn = true;
                    }
                }
            }
            else if (oldState.IsKeyDown(Keys.NumPad6))
            {
                // Key was down last update, but not down now, so
                // it has just been released. Do nothing.
            }

            // Update saved state.
            oldState = newState;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, cam.viewMatrix);
            f1.drawBackground(ref spriteBatch);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, cam.viewMatrix);
            f1.drawEntities(ref spriteBatch);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, cam.viewMatrix);
            f1.drawEnemies(ref spriteBatch);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, cam.viewMatrix);
            p1.drawMe(ref spriteBatch);
            spriteBatch.End();

            if (victory_condition)
            {
                Vector2 vec = new Vector2(p1.get_my_Position().X-60, p1.get_my_Position().Y-60);
                spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, cam.viewMatrix);
                spriteBatch.DrawString(sfont_thesecond, "You won!", vec, Color.White);
                spriteBatch.End();
            }
            // TODO: Add your drawing code here
            
            base.Draw(gameTime);
        }
    }
}
