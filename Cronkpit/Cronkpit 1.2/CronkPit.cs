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

namespace Cronkpit
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
        int current_floor;
        Floor f1;
        Player p1;
        RACursor ra1;
        SpriteFont sfont_thesecond;
        bool bad_turn;
        bool victory_condition;

        //Some other misc stuff
        int selected_lance;

        //message buffer things
        List<string> msgBuf;

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
            Texture2D blank_texture = new Texture2D(GraphicsDevice, 1, 1);
            current_floor = 1;
            msgBuf = new List<string>();
            f1 = new Floor(Content, ref msgBuf, blank_texture, current_floor);
            p1 = new Player(Content, f1.random_valid_position(), ref msgBuf, Player.Chara_Class.Warrior);
            ra1 = new RACursor(Content.Load<Texture2D>("Player/ranged attack cursor"),
                                Content.Load<Texture2D>("Player/charge attack cursor"), f1.random_valid_position());
            cam = new Camera(GraphicsDevice.Viewport.Bounds);
            //stuff without constructors
            bad_turn = false;
            victory_condition = false;
            gameState = 0;

            //Some base components for stuff with big constructors
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
            invScr = new InvAndHealthBox(blank_texture, tiny_font, big_font, ref Secondary_cManager, ref icoBar);
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
            //Preload all of the arrrow textures so that they only need to be loaded once.
            Texture2D scroll_up_one_arrow = Content.Load<Texture2D>("UI Elements/Inventory Screen/invBox_one_scrollup");
            Texture2D scroll_up_max_arrow = Content.Load<Texture2D>("UI Elements/Inventory Screen/invBox_max_scrollup");
            Texture2D scroll_down_one_arrow = Content.Load<Texture2D>("UI Elements/Inventory Screen/invbox_one_scrolldown");
            Texture2D scroll_down_max_arrow = Content.Load<Texture2D>("UI Elements/Inventory Screen/invbox_max_scrolldown");
            //Init textures
            msgBufBox.init_textures(scroll_up_one_arrow, scroll_up_max_arrow, scroll_down_one_arrow, scroll_down_max_arrow);
            invScr.init_textures(Content.Load<Texture2D>("UI Elements/Inventory Screen/wireframe"), 
                                chest_texes, larm_texes, rarm_texes, lleg_texes, rleg_texes, head_texes,
                                scroll_up_max_arrow, scroll_up_one_arrow, 
                                scroll_down_max_arrow, scroll_down_one_arrow);
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

            if(!f1.projectiles_remaining_to_update())
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
                f1.add_smell_to_tile(p1.get_my_grid_C(), 0, p1.total_scent);
                f1.sound_pulse(p1.get_my_grid_C(), p1.total_sound, 0);
                p1.reset_sound_and_scent();
                f1.update_dungeon_floor(p1);
                bad_turn = false;
            }

            cam.Pos = p1.get_my_Position();
            //Only do this if it's visible!                

            // TODO: Add your update logic here
            //Hah, more like TODONE.

            //State updates - the sequel
            mouse_oldState = mouse_newState;
            oldState = newState;

            base.Update(gameTime);
        }

        private void updateInput()
        {
            bool just_changed_states = false;

            #region keypresses for the main menu (GS = 0)
            if (gameState == 0)
            {
                if (check_key_release(Keys.Up))
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

                if (check_key_press(Keys.N))
                    icoBar.switch_my_mode();

                //Now we do the icon bar! Woo.
                if (check_key_press(Keys.F2))
                    use_slot_on_icoBar_full(0, out just_changed_states);

                if (check_key_press(Keys.F3))
                    use_slot_on_icoBar_full(1, out just_changed_states);

                if (check_key_press(Keys.F4))
                    use_slot_on_icoBar_full(2, out just_changed_states);

                if (check_key_press(Keys.F5))
                    use_slot_on_icoBar_full(3, out just_changed_states);

                if (check_key_press(Keys.F6))
                    use_slot_on_icoBar_full(4, out just_changed_states);

                if (check_key_press(Keys.F7))
                    use_slot_on_icoBar_full(5, out just_changed_states);

                if (check_key_press(Keys.F8))
                    use_slot_on_icoBar_full(6, out just_changed_states);

                if (check_key_press(Keys.F9))
                    use_slot_on_icoBar_full(7, out just_changed_states);
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

                if (check_key_press(Keys.Escape))
                {
                    invScr.switch_my_mode();
                    if (!invScr.is_visible())
                        gameState = 1;
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
                                invScr.init_necessary_textures(p1);
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

            #region keypresses for when the game is playing or the message log is showing (GS = 1 || 4)
            if (gameState == 1 || gameState == 4)
            {
                if (check_key_press(Keys.M))
                {
                    if (gameState == 1)
                    {
                        msgBufBox.switch_my_mode();
                        gameState = 4;
                    }
                    else
                    {
                        msgBufBox.switch_my_mode();
                        gameState = 1;
                    }
                }
            }
            #endregion

            #region keypresses for when the message log is showing (GS = 4, mouse only)
            if (gameState == 4)
            {   
                if (check_mouse_left_click())
                    msgBufBox.mouseClick(new Vector2(mouse_newState.X, mouse_newState.Y));
            }
            #endregion

            #region keypresses for either game or shooting (GS = 1 || 5)
            if (gameState == 1 || gameState == 5)
            {
                if (check_key_press(Keys.Enter) && p1.is_bow_equipped())
                {
                    if (gameState == 1)
                    {
                        start_ranged_attack();
                    }
                    else
                    {
                        ranged_attack_via_cursor();
                    }
                }
            }
            #endregion

            #region keypresses for shooting or charging (GS = 5 || 6)
            if (gameState == 5 || gameState == 6)
            {
                if (check_key_press(Keys.NumPad1))
                {
                    ra1.shift_coordinates(-1, 1);
                }

                if (check_key_press(Keys.NumPad2))
                {
                    ra1.shift_coordinates(0, 1);
                }

                if (check_key_press(Keys.NumPad3))
                {
                    ra1.shift_coordinates(1, 1);
                }

                if (check_key_press(Keys.NumPad4))
                {
                    ra1.shift_coordinates(-1, 0);
                }

                if (check_key_press(Keys.NumPad6))
                {
                    ra1.shift_coordinates(1, 0);
                }

                if (check_key_press(Keys.NumPad7))
                {
                    ra1.shift_coordinates(-1, -1);
                }

                if (check_key_press(Keys.NumPad8))
                {
                    ra1.shift_coordinates(0, -1);
                }

                if (check_key_press(Keys.NumPad9))
                {
                    ra1.shift_coordinates(1, -1);
                }

                #region only while shooting (GS = 5)
                if (gameState == 5)
                {
                    if (check_mouse_left_click())
                    {
                        int gc_xloc = (int)((mousePosition.X - cam.viewMatrix.Translation.X) / 32);
                        int gc_yloc = (int)((mousePosition.Y - cam.viewMatrix.Translation.Y) / 32);
                        gridCoordinate click_location = new gridCoordinate(gc_xloc, gc_yloc);
                        int monster_no;
                        if (f1.is_monster_here(click_location, out monster_no) && f1.aura_of_specific_tile(click_location) == Tile.Aura.Attack)
                        {
                            f1.create_new_projectile(new Projectile(p1.get_my_grid_C(), f1.badguy_by_monster_id(monster_no).my_grid_coord, Projectile.projectile_type.Arrow, ref Secondary_cManager));
                            f1.add_effect(Attack.Damage.Piercing, f1.badguy_by_monster_id(monster_no).my_grid_coord);
                            p1.bow_attack(f1, monster_no);

                            gameState = 1;
                            f1.scrub_all_auras();
                            ra1.am_i_visible = false;
                            bad_turn = true;
                        }
                    }

                    if (check_key_press(Keys.F2))
                        use_slot_on_icoBar_RA_Only(0, just_changed_states);

                    if (check_key_press(Keys.F3))
                        use_slot_on_icoBar_RA_Only(1, just_changed_states);

                    if (check_key_press(Keys.F4))
                        use_slot_on_icoBar_RA_Only(2, just_changed_states);

                    if (check_key_press(Keys.F5))
                        use_slot_on_icoBar_RA_Only(3, just_changed_states);

                    if (check_key_press(Keys.F6))
                        use_slot_on_icoBar_RA_Only(4, just_changed_states);

                    if (check_key_press(Keys.F7))
                        use_slot_on_icoBar_RA_Only(5, just_changed_states);

                    if (check_key_press(Keys.F8))
                        use_slot_on_icoBar_RA_Only(6, just_changed_states);

                    if (check_key_press(Keys.F9))
                        use_slot_on_icoBar_RA_Only(7, just_changed_states);
                }
                #endregion

                #region only while charging (GS = 6)

                if (gameState == 6)
                {
                    if (check_key_press(Keys.F2))
                        use_slot_on_icoBar_CA_Only(0, just_changed_states);

                    if (check_key_press(Keys.F3))
                        use_slot_on_icoBar_CA_Only(1, just_changed_states);

                    if (check_key_press(Keys.F4))
                        use_slot_on_icoBar_CA_Only(2, just_changed_states);

                    if (check_key_press(Keys.F5))
                        use_slot_on_icoBar_CA_Only(3, just_changed_states);

                    if (check_key_press(Keys.F6))
                        use_slot_on_icoBar_CA_Only(4, just_changed_states);

                    if (check_key_press(Keys.F7))
                        use_slot_on_icoBar_CA_Only(5, just_changed_states);

                    if (check_key_press(Keys.F8))
                        use_slot_on_icoBar_CA_Only(6, just_changed_states);

                    if (check_key_press(Keys.F9))
                        use_slot_on_icoBar_CA_Only(7, just_changed_states);

                    if(check_key_press(Keys.Enter))
                    {
                        charge_attack_via_cursor(selected_lance);
                    }

                    if (check_mouse_left_click())
                    {
                        int gc_xloc = (int)((mousePosition.X - cam.viewMatrix.Translation.X) / 32);
                        int gc_yloc = (int)((mousePosition.Y - cam.viewMatrix.Translation.Y) / 32);
                        gridCoordinate click_location = new gridCoordinate(gc_xloc, gc_yloc);
                        int monster_no;
                        if (f1.is_monster_here(click_location, out monster_no) && f1.aura_of_specific_tile(click_location) == Tile.Aura.Attack)
                        {
                            f1.add_effect(Attack.Damage.Piercing, f1.badguy_by_monster_id(monster_no).my_grid_coord);
                            p1.charge_attack(f1, selected_lance, monster_no);

                            gameState = 1;
                            f1.scrub_all_auras();
                            ra1.am_i_visible = false;
                            bad_turn = true;
                        }
                    }
                }

                #endregion
            }
            #endregion


            /*
            if (check_mouse_left_click())
            {
                int x_position = (int)((-cam.viewMatrix.Translation.X + mousePosition.X) / 32);
                int y_position = (int)((-cam.viewMatrix.Translation.Y + mousePosition.Y) / 32);
                gridCoordinate click_loc = new gridCoordinate(x_position, y_position);
                f1.add_new_popup("Clicked here!", Popup.popup_msg_color.Blue, click_loc);
            }
            */

            // Update saved state.            
        }

        #region some ranged / charge attack stuff - stuffed in a function to avoid repetition
        private void start_ranged_attack()
        {
            ra1.shift_modes(RACursor.Mode.Ranged);
            gameState = 5;
            p1.set_ranged_attack_aura(f1, p1.get_my_grid_C());
            ra1.am_i_visible = true;
            ra1.my_grid_coord = new gridCoordinate(p1.get_my_grid_C().x, p1.get_my_grid_C().y - 1);
            ra1.reset_drawing_position();
        }

        private void ranged_attack_via_cursor()
        {
            gridCoordinate click_location = new gridCoordinate(ra1.my_grid_coord);
            int monster_no;
            if (f1.is_monster_here(click_location, out monster_no) && f1.aura_of_specific_tile(click_location) == Tile.Aura.Attack)
            {
                f1.create_new_projectile(new Projectile(p1.get_my_grid_C(), f1.badguy_by_monster_id(monster_no).my_grid_coord, Projectile.projectile_type.Arrow, ref Secondary_cManager));
                f1.add_effect(Attack.Damage.Piercing, f1.badguy_by_monster_id(monster_no).my_grid_coord);
                p1.bow_attack(f1, monster_no);
                bad_turn = true;
            }

            gameState = 1;
            f1.scrub_all_auras();
            ra1.am_i_visible = false;
        }

        private void start_charge_attack(int lanceID)
        {
            ra1.shift_modes(RACursor.Mode.Charge);
            gameState = 6;
            p1.set_charge_attack_aura(f1, lanceID, p1.get_my_grid_C());
            ra1.am_i_visible = true;
            ra1.my_grid_coord = new gridCoordinate(p1.get_my_grid_C().x, p1.get_my_grid_C().y - 1);
            ra1.reset_drawing_position();
        }

        private void charge_attack_via_cursor(int lanceID)
        {
            gridCoordinate click_location = new gridCoordinate(ra1.my_grid_coord);
            int monster_no;
            if (f1.is_monster_here(click_location, out monster_no) && f1.aura_of_specific_tile(click_location) == Tile.Aura.Attack)
            {
                f1.add_effect(Attack.Damage.Piercing, f1.badguy_by_monster_id(monster_no).my_grid_coord);
                p1.charge_attack(f1, lanceID, monster_no);
                bad_turn = true;
            }

            gameState = 1;
            f1.scrub_all_auras();
            ra1.am_i_visible = false;
        }
        #endregion

        private void use_slot_on_icoBar_full(int slot, out bool changed_states)
        {
            changed_states = false;
            int item_ID = icoBar.get_item_IDs_by_slot(slot);
            if (item_ID > -1)
            {
                string item_type = p1.get_item_type_by_ID(item_ID);
                bool is_equipped = p1.is_item_equipped(item_ID);
                if (!is_equipped)
                {
                    if (String.Compare(item_type, "Weapon") == 0)
                    {
                        Weapon c_weapon = p1.get_weapon_by_ID(item_ID);
                        if (c_weapon.get_my_weapon_type() != Weapon.Type.Lance)
                        {
                            if (c_weapon.get_hand_count() == 2)
                                p1.equip_main_hand(c_weapon);
                            else
                            {
                                if (p1.show_main_hand() == null)
                                    p1.equip_main_hand(c_weapon);
                                else if (p1.show_main_hand() != null && p1.show_off_hand() == null)
                                    p1.equip_off_hand(c_weapon);
                                else if (p1.show_main_hand() != null && p1.show_off_hand() != null)
                                    p1.equip_main_hand(c_weapon);
                            }
                        }
                        else
                        {
                            start_charge_attack(item_ID);
                            selected_lance = item_ID;
                            changed_states = true;
                        }
                    }
                    else if (String.Compare(item_type, "Underarmor") == 0)
                    {
                        Armor c_armor = p1.get_armor_by_ID(item_ID);
                        p1.equip_under_armor(c_armor);
                    }
                    else if (String.Compare(item_type, "Overarmor") == 0)
                    {
                        Armor c_armor = p1.get_armor_by_ID(item_ID);
                        p1.equip_over_armor(c_armor);
                    }
                }
                else //if it is equipped...
                {
                    if (String.Compare(item_type, "Weapon") == 0)
                    {
                        Weapon c_weapon = p1.get_weapon_by_ID(item_ID);
                        if (c_weapon.get_my_weapon_type() == Weapon.Type.Bow)
                        {
                            start_ranged_attack();
                            changed_states = true;
                        }
                    }
                }
            }
        }

        private void use_slot_on_icoBar_RA_Only(int slot, bool changed_states)
        {
            int item_ID = icoBar.get_item_IDs_by_slot(slot);
            if (item_ID > -1)
            {
                string item_type = p1.get_item_type_by_ID(item_ID);
                bool is_equipped = p1.is_item_equipped(item_ID);
                if (is_equipped)
                {
                    if (String.Compare(item_type, "Weapon") == 0)
                    {
                        Weapon c_weapon = p1.get_weapon_by_ID(item_ID);
                        if (c_weapon.get_my_weapon_type() == Weapon.Type.Bow && !changed_states)
                            ranged_attack_via_cursor();
                    }
                }
            }
        }

        private void use_slot_on_icoBar_CA_Only(int slot, bool changed_states)
        {
            int item_ID = icoBar.get_item_IDs_by_slot(slot);
            if (item_ID > -1)
            {
                string item_type = p1.get_item_type_by_ID(item_ID);
                bool is_equipped = p1.is_item_equipped(item_ID);
                if (!is_equipped)
                {
                    if (String.Compare(item_type, "Weapon") == 0)
                    {
                        Weapon c_weapon = p1.get_weapon_by_ID(item_ID);
                        if (c_weapon.get_my_weapon_type() == Weapon.Type.Lance && !changed_states)
                            charge_attack_via_cursor(item_ID);
                    }
                }
            }
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
            Texture2D blank_texture = new Texture2D(GraphicsDevice, 1, 1);
            current_floor++;
            f1 = new Floor(Content, ref msgBuf, blank_texture, current_floor);
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
                case 4:
                    draw_game();
                    break;
                case 5:
                    draw_game();
                    break;
                case 6:
                    draw_game();
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
            f1.draw_tile_auras(ref spriteBatch);
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

            if (ra1.am_i_visible)
            {
                spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, cam.viewMatrix);
                ra1.draw_me(ref spriteBatch);
                spriteBatch.End();
            }

            /*
            Texture2D blank_texture = new Texture2D(GraphicsDevice, 1, 1);
            blank_texture.SetData(new[] { Color.White });
            spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, cam.viewMatrix);
            f1.draw_vision_log(ref spriteBatch, blank_texture);
            spriteBatch.End();
             */

            if (msgBufBox.is_visible())
                msgBufBox.draw_me(ref spriteBatch);

            if (icoBar.is_visible() && !msgBufBox.is_visible())
                icoBar.draw_me(ref spriteBatch);

            if (invScr.is_visible())
                invScr.draw_me(ref spriteBatch, p1);
        }

        public void draw_coordinate_data()
        {
            int fixed_height = 15;
            SpriteFont mFont = Content.Load<SpriteFont>("Fonts/smallfont");
            spriteBatch.Begin(SpriteSortMode.BackToFront, null);
            float h1 = mousePosition.Y + fixed_height;
            float h2 = mousePosition.Y + fixed_height + mFont.LineSpacing;
            float h3 = mousePosition.Y + fixed_height + (mFont.LineSpacing * 2);
            float h4 = mousePosition.Y + fixed_height + (mFont.LineSpacing * 3);
            float h5 = mousePosition.Y + fixed_height + (mFont.LineSpacing * 4);
            float h6 = mousePosition.Y + fixed_height + (mFont.LineSpacing * 5);
            spriteBatch.DrawString(mFont, "Mouse X: " + mousePosition.X.ToString(), new Vector2(mousePosition.X, h1), Color.White);
            spriteBatch.DrawString(mFont, "Mouse Y: " + mousePosition.Y.ToString(), new Vector2(mousePosition.X, h2), Color.White);
            spriteBatch.DrawString(mFont, "Player X: " + p1.get_my_Position().X.ToString(), new Vector2(mousePosition.X, h3), Color.White);
            spriteBatch.DrawString(mFont, "Player Y: " + p1.get_my_Position().Y.ToString(), new Vector2(mousePosition.X, h4), Color.White);
            spriteBatch.DrawString(mFont, "Mouse ?X: " + cam.viewMatrix.Translation.X.ToString() + " + " + mousePosition.X.ToString(), new Vector2(mousePosition.X, h5), Color.White);
            spriteBatch.DrawString(mFont, "Mouse ?Y: " + cam.viewMatrix.Translation.Y.ToString() + " + " + mousePosition.Y.ToString(), new Vector2(mousePosition.X, h6), Color.White);
            spriteBatch.End();
        }
    }
}
