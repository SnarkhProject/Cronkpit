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
        MouseState mouse_oldState;
        MouseState mouse_newState;

        Vector2 mousePosition;
        Texture2D mouse_tex;

        Camera cam;

        int gameState;
        //Menu stuff!
        MenuScreen sMenu;
        MessageBufferBox msgBufBox;
        IconBar icoBar;
        InvAndHealthBox invScr;

        List<string> msgBuf;

        float clear_msg_buffer_event;
        const float time_to_clear_msg = 1.2f;

        public CronkPit()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 800;
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
            clear_msg_buffer_event = time_to_clear_msg;
            //shit without constructors
            bad_turn = false;
            victory_condition = false;
            gameState = 0;

            //shit with super uber constructors
            List<string> menuItems = new List<string>();
            menuItems.Add("Start");
            menuItems.Add("Exit");
            Texture2D blank_texture = new Texture2D(GraphicsDevice, 1, 1);
            sMenu = new MenuScreen(menuItems, "CronkPit", 
                                    Content.Load<SpriteFont>("Fonts/sfont"), 
                                    Content.Load<SpriteFont>("Fonts/tfont"), 
                                    client_rect());
            msgBufBox = new MessageBufferBox(client_rect(), 
                                    Content.Load<SpriteFont>("Fonts/smallfont"),
                                    blank_texture, ref msgBuf);
            icoBar = new IconBar(blank_texture, 
                                    Content.Load<SpriteFont>("Fonts/smallfont"), client_rect());
            invScr = new InvAndHealthBox(blank_texture);
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
            mouse_tex = Content.Load<Texture2D>("MouseTexture");

            msgBufBox.init_textures(Content.Load<Texture2D>("UI Elements/MSG Buffer Box/msgbox_one_scrollup"),
                                    Content.Load<Texture2D>("UI Elements/MSG Buffer Box/msgbox_max_scrollup"),
                                    Content.Load<Texture2D>("UI Elements/MSG Buffer Box/msgbox_one_scrolldown"),
                                    Content.Load<Texture2D>("UI Elements/MSG Buffer Box/msgbox_max_scrolldown"));
            invScr.init_textures(Content.Load<Texture2D>("UI Elements/Inventory Screen/wireframe"));
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
        /// 
        
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            updateInput();
            if (p1.is_spot_exit(f1))
            {
                if (!victory_condition)
                {
                    msgBuf.Add("You quickly dash down the stairs to the next level of the dungeon.");
                    victory_condition = true;
                }
            }

            if (bad_turn)
            {
                f1.update_dungeon_floor(p1);
                bad_turn = false;
            }

            cam.Pos = p1.get_my_Position();
            mousePosition.X = Mouse.GetState().X;
            mousePosition.Y = Mouse.GetState().Y;

            #region clear message buffer code

            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            clear_msg_buffer_event -= elapsedTime;
            
            if (clear_msg_buffer_event < 0 && msgBufBox.is_scrolling())
            {
                msgBufBox.scrollMSG(1);
                clear_msg_buffer_event = time_to_clear_msg;
            }

            #endregion
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        private void updateInput()
        {
            newState = Keyboard.GetState();
            mouse_newState = Mouse.GetState();

            #region keypresses for the main menu (GS = 0)

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

            #region keypresses for when the game is playing (GS = 1)

            if (gameState == 1)
            {
                if (check_mouse_left_click())
                {
                    msgBufBox.mouseClick(new Vector2(mouse_newState.X, mouse_newState.Y));
                }

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
                        msgBufBox.scrollMSG(1);
                    }

                if (check_key_press(Keys.M))
                    msgBufBox.switch_my_mode();

                if (check_key_press(Keys.N))
                    icoBar.switch_my_mode();
            }

            #endregion

            #region keypresses for either game or inventory screen (GS = 1 || 2)

            if (gameState == 1 || gameState == 2)
            {
                if (check_key_press(Keys.I))
                {
                    invScr.switch_my_mode();
                    if (invScr.is_visible())
                        gameState = 2;
                    else
                        gameState = 1;
                }
            }

            #endregion

            #region keypresses for when the inventory screen is showing, this is mostly a mouse thing (GS = 2)

            //do nothing

            #endregion

            // Update saved state.
            mouse_oldState = mouse_newState;
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

        private bool check_mouse_left_click()
        {
            return mouse_newState.LeftButton == ButtonState.Pressed && mouse_oldState.LeftButton == ButtonState.Released;
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
                    spriteBatch.Begin(SpriteSortMode.BackToFront, null);
                    sMenu.drawMe(ref spriteBatch);
                    spriteBatch.End();
                    break;
                case 1:
                    draw_game();
                    break;
                case 2:
                    draw_game();
                    break;
            }

            spriteBatch.Begin(SpriteSortMode.BackToFront, null);
            spriteBatch.Draw(mouse_tex, mousePosition, Color.White);
            spriteBatch.End();
            
            // TODO: Add your drawing code here
            
            base.Draw(gameTime);
        }

        public void draw_game()
        {
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

            if (msgBufBox.is_visible())
            {
                spriteBatch.Begin(SpriteSortMode.BackToFront, null);
                msgBufBox.draw_my_rect(ref spriteBatch);
                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.BackToFront, null);
                msgBufBox.draw_my_elements(ref spriteBatch);
                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.BackToFront, null);
                msgBufBox.draw_my_borders(ref spriteBatch);
                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.BackToFront, null);
                msgBufBox.draw_my_text(ref spriteBatch);
                spriteBatch.End();
            }

            if (icoBar.is_visible())
            {
                spriteBatch.Begin(SpriteSortMode.BackToFront, null);
                icoBar.draw_my_icons(ref spriteBatch);
                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.BackToFront, null);
                icoBar.draw_my_borders(ref spriteBatch);
                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.BackToFront, null);
                //msgBufBox.draw_my_text(ref spriteBatch);
                spriteBatch.End();
            }

            if (invScr.is_visible())
            {
                spriteBatch.Begin(SpriteSortMode.BackToFront, null);
                invScr.draw_my_back(ref spriteBatch);
                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.BackToFront, null);
                invScr.draw_my_front(ref spriteBatch);
                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.BackToFront, null);
                invScr.draw_my_borders(ref spriteBatch);
                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.BackToFront, null);
                invScr.draw_my_text(ref spriteBatch);
                spriteBatch.End();
            }
        }
    }
}
