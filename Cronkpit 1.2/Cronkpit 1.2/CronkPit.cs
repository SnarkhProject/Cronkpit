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
        ContentManager Secondary_cManager;

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
        ShopScreen shopScr;

        //Game things.
        Floor f1;
        Player p1;
        SpriteFont sfont_thesecond;
        bool bad_turn;
        bool victory_condition;

        //message buffer things
        List<string> msgBuf;
        float clear_msg_buffer_event;
        const float time_to_clear_msg = 1.2f;

        public CronkPit()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Secondary_cManager = new ContentManager(Services);
            Secondary_cManager.RootDirectory = "Content";

            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 800;
            //this.IsMouseVisible = true;
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
            //stuff with constructors
            msgBuf = new List<string>();
            f1 = new Floor(Content, ref msgBuf);
            p1 = new Player(Content, f1.random_valid_position(), ref msgBuf, Player.Chara_Class.Warrior);
            cam = new Camera(GraphicsDevice.Viewport.Bounds);
            clear_msg_buffer_event = time_to_clear_msg;
            //stuff without constructors
            bad_turn = false;
            victory_condition = false;
            gameState = 0;

            //Some base components for stuff with big constructors
            Texture2D blank_texture = new Texture2D(GraphicsDevice, 1, 1);
            SpriteFont big_font = Content.Load<SpriteFont>("Fonts/tfont");
            SpriteFont normal_font = Content.Load<SpriteFont>("Fonts/sfont");
            SpriteFont tiny_font = Content.Load<SpriteFont>("Fonts/smallfont");
            List<string> menuItems = new List<string>();
            menuItems.Add("Start");
            menuItems.Add("Exit");
            List<string> shopMenuItems = new List<string>();
            shopMenuItems.Add("Shop Weapons");
            shopMenuItems.Add("Shop Armor");
            shopMenuItems.Add("Shop Consumables");
            shopMenuItems.Add("Shop Talismans");
            shopMenuItems.Add("Exit to next floor");
            //stuff with big constructors
            sMenu = new MenuScreen(menuItems, "CronkPit", normal_font, big_font, client_rect());
            msgBufBox = new MessageBufferBox(client_rect(), tiny_font, blank_texture, ref msgBuf);
            icoBar = new IconBar(blank_texture, tiny_font, client_rect());
            invScr = new InvAndHealthBox(blank_texture, tiny_font, big_font, ref Secondary_cManager);
            shopScr = new ShopScreen(shopMenuItems, normal_font, big_font, client_rect(), ref Secondary_cManager);
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
            //Make texture arrays for wireframe, etc.
            Texture2D[] chest_texes = new Texture2D[4];
            load_wireframe_textures(ref chest_texes, "chest_");
            Texture2D[] rarm_texes = new Texture2D[4];
            load_wireframe_textures(ref rarm_texes, "rarm_");
            Texture2D[] larm_texes = new Texture2D[4];
            load_wireframe_textures(ref larm_texes, "larm_");
            Texture2D[] rleg_texes = new Texture2D[4];
            load_wireframe_textures(ref rleg_texes, "rleg_");
            Texture2D[] lleg_texes = new Texture2D[4];
            load_wireframe_textures(ref lleg_texes, "lleg_");
            Texture2D[] head_texes = new Texture2D[2];
            head_texes[0] = Content.Load<Texture2D>("UI Elements/Inventory Screen/head_blue");
            head_texes[1] = Content.Load<Texture2D>("UI Elements/Inventory Screen/head_red");
            //Init textures
            msgBufBox.init_textures(Content.Load<Texture2D>("UI Elements/MSG Buffer Box/msgbox_one_scrollup"),
                                    Content.Load<Texture2D>("UI Elements/MSG Buffer Box/msgbox_max_scrollup"),
                                    Content.Load<Texture2D>("UI Elements/MSG Buffer Box/msgbox_one_scrolldown"),
                                    Content.Load<Texture2D>("UI Elements/MSG Buffer Box/msgbox_max_scrolldown"));
            invScr.init_textures(Content.Load<Texture2D>("UI Elements/Inventory Screen/wireframe"), 
                                chest_texes, larm_texes, rarm_texes, lleg_texes, rleg_texes, head_texes,
                                Content.Load<Texture2D>("UI Elements/Inventory Screen/invBox_max_scrollup"),
                                Content.Load<Texture2D>("UI Elements/Inventory Screen/invBox_one_scrollup"),
                                Content.Load<Texture2D>("UI Elements/Inventory Screen/invBox_max_scrolldown"),
                                Content.Load<Texture2D>("UI Elements/Inventory Screen/invBox_one_scrolldown"));
            // TODO: use this.Content to load your game content here
        }

        public void load_wireframe_textures(ref Texture2D[] targetArray, string bodyPart)
        {
            string basePath = "UI Elements/Inventory Screen/" + bodyPart;
            for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 0:
                        targetArray[i] = Content.Load<Texture2D>(basePath + "blue");
                        break;
                    case 1:
                        targetArray[i] = Content.Load<Texture2D>(basePath + "green");
                        break;
                    case 2:
                        targetArray[i] = Content.Load<Texture2D>(basePath + "yellow");
                        break;
                    case 3:
                        targetArray[i] = Content.Load<Texture2D>(basePath + "red");
                        break;
                }
            }
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
            //State updates
            newState = Keyboard.GetState();
            mouse_newState = Mouse.GetState();

            mousePosition.X = Mouse.GetState().X;
            mousePosition.Y = Mouse.GetState().Y;

            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if(!f1.projectiles_remaining_to_update() && !f1.popups_remaining_to_update())
                updateInput();

            f1.update_all_projectiles(p1, elapsedTime);
            f1.update_all_effects(elapsedTime);
            f1.update_all_popups(elapsedTime);

            if (p1.is_spot_exit(f1))
            {
                shopScr.set_variables(p1);
                shopScr.switch_shopping_mode(ShopScreen.Shopping_Mode.Main);
                p1.heal_naturally();
                p1.repair_all_armor();
                new_floor();
                gameState = 3;
            }

            if (bad_turn)
            {
                f1.update_dungeon_floor(p1);
                bad_turn = false;
            }

            cam.Pos = p1.get_my_Position();
            //Only do this if it's visible!                

            #region clear message buffer code

            
            clear_msg_buffer_event -= elapsedTime;
            
            if (clear_msg_buffer_event < 0 && msgBufBox.is_scrolling())
            {
                msgBufBox.scrollMSG(1);
                clear_msg_buffer_event = time_to_clear_msg;
            }

            #endregion
            // TODO: Add your update logic here
            //Hah, more like TODONE.

            //State updates - the sequel
            mouse_oldState = mouse_newState;
            oldState = newState;

            base.Update(gameTime);
        }

        private void updateInput()
        {
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
                        msgBufBox.scrollMSG(1);

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
                    invScr.update_player_info(ref p1);
                    if (invScr.is_visible())
                        gameState = 2;
                    else
                        gameState = 1;
                }
            }

            #endregion

            #region keypresses for when the inventory screen is showing, this is mostly a mouse thing (GS = 2)

            if (gameState == 2)
            {
                invScr.update_mouse_info(mousePosition, p1, check_mouse_hold_left(), check_mouse_left_click());

                if (check_mouse_left_click())
                {
                    msgBufBox.mouseClick(mousePosition);
                }
            }

            #endregion

            #region keypresses for when there's a shop screen (GS = 3)

            if (gameState == 3)
            {
                if (check_key_release(Keys.Up))
                    shopScr.scroll_menu(-1);

                if (check_key_release(Keys.Down))
                    shopScr.scroll_menu(1);

                if (check_key_release(Keys.Enter))
                {
                    int the_index = shopScr.get_my_index();
                    if (!shopScr.in_submenu())
                    {
                        switch (the_index)
                        {
                            case 0:
                                //Weapons
                                shopScr.switch_shopping_mode(ShopScreen.Shopping_Mode.Weapons);
                                break;
                            case 1:
                                //Armor
                                shopScr.switch_shopping_mode(ShopScreen.Shopping_Mode.Armor);
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                gameState = 1;
                                break;
                        }
                    }
                    else if (shopScr.in_submenu())
                    {
                        if (shopScr.exit_submenu())
                            shopScr.switch_shopping_mode(ShopScreen.Shopping_Mode.Main);
                        else
                            shopScr.buy_item(p1);
                    }
                }
            }

            #endregion

            // Update saved state.            
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

        private bool check_mouse_hold_left()
        {
            return mouse_newState.LeftButton == ButtonState.Pressed && mouse_oldState.LeftButton == ButtonState.Pressed;
        }

        public void new_floor()
        {
            f1 = new Floor(Content, ref msgBuf);
            p1.teleport(f1.random_valid_position());
            bad_turn = false;
            msgBuf.Clear();
            msgBufBox.scrollMSG(-1000);
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
                case 3:
                    spriteBatch.Begin(SpriteSortMode.BackToFront, null);
                    shopScr.drawMe(ref spriteBatch);
                    spriteBatch.End();
                    break;
            }

            spriteBatch.Begin(SpriteSortMode.BackToFront, null);
            spriteBatch.Draw(mouse_tex, new Vector2(mousePosition.X-8, mousePosition.Y-6), Color.White);
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

            spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, cam.viewMatrix);
            f1.drawProjectile(ref spriteBatch);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, cam.viewMatrix);
            f1.drawEffect(ref spriteBatch);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, cam.viewMatrix);
            f1.drawPopup(ref spriteBatch);
            spriteBatch.End();

            /*
            Texture2D blank_texture = new Texture2D(GraphicsDevice, 1, 1);
            blank_texture.SetData(new[] { Color.White });
            spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, cam.viewMatrix);
            f1.draw_vision_log(ref spriteBatch, blank_texture);
            spriteBatch.End();
             */

            if (msgBufBox.is_visible())
                msgBufBox.draw_me(ref spriteBatch);

            if (icoBar.is_visible())
                icoBar.draw_me(ref spriteBatch);

            if (invScr.is_visible())
                invScr.draw_me(ref spriteBatch, p1);
        }
    }
}
