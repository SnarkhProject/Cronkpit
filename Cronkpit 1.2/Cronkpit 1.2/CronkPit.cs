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

namespace Cronkpit_1._2
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class CronkPit : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        KeyboardState oldState;
        KeyboardState newState;

        Camera cam;

        int gameState;
        MenuScreen sMenu;
        MessageBufferBox msgBufBox;

        List<string> msgBuf;

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
        SpriteFont sfont_thesecond;
        bool bad_turn;
        bool victory_condition;
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //shit with constructors
            msgBuf = new List<string>();
            f1 = new Floor(Content, ref msgBuf);
            p1 = new Player(Content, f1.random_valid_position(), ref msgBuf);
            cam = new Camera(GraphicsDevice.Viewport.Bounds);
            //shit without constructors
            bad_turn = false;
            victory_condition = false;
            gameState = 0;

            List<string> menuItems = new List<string>();
            menuItems.Add("Start");
            menuItems.Add("Exit");
            sMenu = new MenuScreen(menuItems, "CronkPit", 
                                    Content.Load<SpriteFont>("Fonts/sfont"), 
                                    Content.Load<SpriteFont>("Fonts/tfont"), 
                                    client_rect());
            msgBufBox = new MessageBufferBox(client_rect(), 
                                    Content.Load<SpriteFont>("Fonts/sfont"), 
                                    new Texture2D(GraphicsDevice, 1, 1));
            //then init the base
            base.Initialize();
        }

        private Rectangle client_rect()
        {
            Rectangle ret_rect = new Rectangle(GraphicsDevice.Viewport.X,
                                                GraphicsDevice.Viewport.Y,
                                                GraphicsDevice.Viewport.Width,
                                                GraphicsDevice.Viewport.Height);
            return ret_rect;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            sfont_thesecond = Content.Load<SpriteFont>("Fonts/sfont");
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
                msgBuf.Clear();
                f1.update_dungeon_floor(p1);
                bad_turn = false;
            }

            cam.Pos = p1.get_my_Position();
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        private void updateInput()
        {
            newState = Keyboard.GetState();

            #region keypresses for the main menu

            if (gameState == 0)
            {
                if(check_key_release(Keys.Up))
                {
                    sMenu.selected_index_up();
                }

                if (check_key_release(Keys.Down))
                {
                    sMenu.selected_index_down();
                }

                if (check_key_release(Keys.Enter))
                {
                    switch (sMenu.selected_index())
                    {
                        case 0:
                            gameState = 1;
                            break;
                        default:
                            this.Exit();
                            break;
                    }

                }
            }

            #endregion

            #region keypresses for when the game is playing

            if (gameState == 1)
            {
                //Down Left first
                if (check_key_press(Keys.NumPad1))
                    if (p1.is_alive() && !victory_condition)
                    {
                        p1.move("downleft", f1);
                        bad_turn = true;
                    }

                // Is the DOWN key down?
                if (check_key_press(Keys.NumPad2))
                    if (p1.is_alive() && !victory_condition)
                    {
                        p1.move("down", f1);
                        bad_turn = true;
                    }

                // Is the DOWN RIGHT key down?
                if (check_key_press(Keys.NumPad3))
                    if (p1.is_alive() && !victory_condition)
                    {
                        p1.move("downright", f1);
                        bad_turn = true;
                    }

                // Is the UP key down?
                if (check_key_press(Keys.NumPad8))
                    if (p1.is_alive() && !victory_condition)
                    {
                        p1.move("up", f1);
                        bad_turn = true;
                    }

                if (check_key_press(Keys.NumPad7))
                    if (p1.is_alive() && !victory_condition)
                    {
                        p1.move("upleft", f1);
                        bad_turn = true;
                    }

                if (check_key_press(Keys.NumPad9))
                    if (p1.is_alive() && !victory_condition)
                    {
                        p1.move("upright", f1);
                        bad_turn = true;
                    }

                // Is the LEFT key down?
                if (check_key_press(Keys.NumPad4))
                    if (p1.is_alive() && !victory_condition)
                    {
                        p1.move("left", f1);
                        bad_turn = true;
                    }


                // Is the RIGHT key down?
                if (check_key_press(Keys.NumPad6))
                    if (p1.is_alive() && !victory_condition)
                    {
                        p1.move("right", f1);
                        bad_turn = true;
                    }

                if (check_key_press(Keys.Space))
                    if (msgBuf.Count > 0)
                    {
                        msgBuf.RemoveAt(msgBuf.Count - 1);
                    }
            }

            #endregion

            // Update saved state.
            oldState = newState;
        }

        private bool check_key_press(Keys theKey)
        {
            return newState.IsKeyDown(theKey) && !oldState.IsKeyDown(theKey);
        }

        private bool check_key_release(Keys theKey)
        {
            return !newState.IsKeyDown(theKey) && oldState.IsKeyDown(theKey);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            switch (gameState)
            {
                case 0:
                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                    sMenu.drawMe(ref spriteBatch);
                    spriteBatch.End();
                    break;
                case 1:
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

                    if (msgBuf.Count > 0 && msgBufBox.is_visible())
                    {
                        if (msgBuf.Count > 1)
                            msgBufBox.are_there_multiple_messages(true);
                        else
                            msgBufBox.are_there_multiple_messages(false);
                        
                        msgBufBox.set_my_msg(msgBuf[msgBuf.Count - 1]);
                        msgBufBox.offset_drawing(cam.viewMatrix);

                        spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, cam.viewMatrix);
                        msgBufBox.draw_my_rect(ref spriteBatch);
                        spriteBatch.End();

                        spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, cam.viewMatrix);
                        msgBufBox.draw_my_borders(ref spriteBatch);
                        spriteBatch.End();

                        spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, cam.viewMatrix);
                        msgBufBox.draw_my_text(ref spriteBatch);
                        spriteBatch.End();
                    }

                    if (victory_condition)
                    {
                        Vector2 vec = new Vector2(p1.get_my_Position().X-60, p1.get_my_Position().Y-60);
                        spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, cam.viewMatrix);
                        spriteBatch.DrawString(sfont_thesecond, "You won!", vec, Color.White);
                        spriteBatch.End();
                    }
                    break;
            }
            
            // TODO: Add your drawing code here
            
            base.Draw(gameTime);
        }
    }
}
