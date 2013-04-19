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

        public enum Game_State
        {
            main_menu, select_character, mini_main_menu, normal,
            shop_screen, inv_screen, drinking_potion, ranged_attack,
            charging_attack, msg_log, bashing_attack, casting_spell,
            force_attack, won_game
        };
        public enum Dungeon
        {
            Necropolis, Gelid_Peaks,
            Flamerunner_Mine,
            Sunken_Citadel
        };

        Keys[] hotbar_keymap = {Keys.F2, Keys.F3, Keys.F4, Keys.F5,
                                Keys.F6, Keys.F7, Keys.F8, Keys.F9 };
        Keys[] direction_keymap = {Keys.NumPad7, Keys.NumPad8, Keys.NumPad9,
                                   Keys.NumPad4, Keys.NumPad6, Keys.NumPad1,
                                   Keys.NumPad2, Keys.NumPad3 };
        gridCoordinate.direction[] direction_map = { gridCoordinate.direction.UpLeft,
                                                     gridCoordinate.direction.Up,
                                                     gridCoordinate.direction.UpRight,
                                                     gridCoordinate.direction.Left,
                                                     gridCoordinate.direction.Right,
                                                     gridCoordinate.direction.DownLeft,
                                                     gridCoordinate.direction.Down,
                                                     gridCoordinate.direction.DownRight };

        Game_State current_state;
        Dungeon current_dungeon;
        //int gameState;
        //Menu stuff!
        MenuScreen sMenu;
        MessageBufferBox msgBufBox;
        IconBar icoBar;
        InvAndHealthScreen invScr;
        ShopScreen shopScr;
        PotionPrompt potiPrompt;
        MiniMainMenu miniMain;
        PaperDoll pDoll;
        ManaBall mBall;
        CharSelect cSelect;

        //Game things.
        int current_floor;
        Floor f1;
        Player p1;
        RACursor ra1;
        SpriteFont sfont_thesecond;
        bool bad_turn;
        bool won_game;

        //Icon bar management stuff
        int locked_slot;
        int selected_lance;
        int selected_mace;
        int selected_scroll;

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
            msgBuf = new List<string>();
            ra1 = new RACursor(Content.Load<Texture2D>("Player/ranged attack cursor"),
                                Content.Load<Texture2D>("Player/charge attack cursor"), 
                                Content.Load<Texture2D>("Player/bash attack cursor"), 
                                Content.Load<Texture2D>("Player/spell cursor"), new gridCoordinate(-1, -1));
            cam = new Camera(GraphicsDevice.Viewport.Bounds);
            //stuff without constructors
            bad_turn = false;
            current_state = Game_State.main_menu;
            locked_slot = -1;
            //gameState = 0;

            //Some base components for stuff with big constructors
            SpriteFont big_font = Content.Load<SpriteFont>("Fonts/tfont");
            SpriteFont normal_font = Content.Load<SpriteFont>("Fonts/sfont");
            SpriteFont smaller_font = Content.Load <SpriteFont>("Fonts/smaller_sfont");
            SpriteFont tiny_font = Content.Load<SpriteFont>("Fonts/smallfont");
            SpriteFont tiny_bold_font = Content.Load<SpriteFont>("Fonts/smallfont_bold");
            SpriteFont pname_font = Content.Load<SpriteFont>("Fonts/mfont");
            List<string> menuItems = new List<string>();
            menuItems.Add("New Game");
            menuItems.Add("Exit");
            List<string> shopMenuItems = new List<string>();
            shopMenuItems.Add("Shop Weapons");
            shopMenuItems.Add("Shop Armor");
            shopMenuItems.Add("Shop Scrolls");
            shopMenuItems.Add("Shop Consumables");
            shopMenuItems.Add("Shop Talismans");
            shopMenuItems.Add("Sell");
            shopMenuItems.Add("Buyback");
            shopMenuItems.Add("Exit to next floor");
            List<string> miniMenuItems = new List<string>();
            miniMenuItems.Add("New Game");
            miniMenuItems.Add("Save Game");
            miniMenuItems.Add("Load Game");
            miniMenuItems.Add("Options");
            miniMenuItems.Add("End Game");
            miniMenuItems.Add("Exit to OS");
            //stuff with big constructors
            sMenu = new MenuScreen(menuItems, "CronkPit", normal_font, big_font, client_rect());
            msgBufBox = new MessageBufferBox(client_rect(), tiny_font, blank_texture, ref msgBuf);
            icoBar = new IconBar(blank_texture, tiny_font, normal_font, client_rect());
            invScr = new InvAndHealthScreen(blank_texture, tiny_font, big_font, pname_font, ref Secondary_cManager, ref icoBar);
            shopScr = new ShopScreen(shopMenuItems, normal_font, smaller_font, client_rect(), ref Secondary_cManager, blank_texture);
            potiPrompt = new PotionPrompt(blank_texture, tiny_font, tiny_bold_font);
            miniMain = new MiniMainMenu(miniMenuItems, client_rect(), blank_texture, big_font);
            cSelect = new CharSelect(ref Secondary_cManager, normal_font, big_font, client_rect());
            //Stuff with smaller constructors
            mBall = new ManaBall(Content.Load<Texture2D>("UI Elements/ManaBall/cball_background"),
                                 Content.Load<Texture2D>("UI Elements/ManaBall/cball_effect_mask"),
                                 client_rect());
            pDoll = new PaperDoll(client_rect());

            won_game = false;
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
            List<KeyValuePair<Scroll.Status_Type, Texture2D>> status_icon_list = new List<KeyValuePair<Scroll.Status_Type, Texture2D>>();
            status_icon_list.Add(new KeyValuePair<Scroll.Status_Type, Texture2D>(Scroll.Status_Type.LynxFer, Content.Load<Texture2D>("Icons/StatEffects/lynx_ferocity_icon")));
            status_icon_list.Add(new KeyValuePair<Scroll.Status_Type, Texture2D>(Scroll.Status_Type.PantherFer, Content.Load<Texture2D>("Icons/StatEffects/panther_ferocity_icon")));
            status_icon_list.Add(new KeyValuePair<Scroll.Status_Type, Texture2D>(Scroll.Status_Type.TigerFer, Content.Load<Texture2D>("Icons/StatEffects/tiger_ferocity_icon")));
            status_icon_list.Add(new KeyValuePair<Scroll.Status_Type, Texture2D>(Scroll.Status_Type.Hemorrhage, Content.Load<Texture2D>("Icons/StatEffects/hemorrhage_icon")));
            //Preload all of the arrrow textures so that they only need to be loaded once.
            Texture2D scroll_up_one_arrow = Content.Load<Texture2D>("UI Elements/Inventory Screen/invBox_one_scrollup");
            Texture2D scroll_up_max_arrow = Content.Load<Texture2D>("UI Elements/Inventory Screen/invBox_max_scrollup");
            Texture2D scroll_down_one_arrow = Content.Load<Texture2D>("UI Elements/Inventory Screen/invbox_one_scrolldown");
            Texture2D scroll_down_max_arrow = Content.Load<Texture2D>("UI Elements/Inventory Screen/invbox_max_scrolldown");
            //Init textures
            msgBufBox.init_textures(scroll_up_one_arrow, scroll_up_max_arrow, scroll_down_one_arrow, 
                                    scroll_down_max_arrow);
            invScr.init_textures(scroll_up_max_arrow, scroll_up_one_arrow, scroll_down_max_arrow, 
                                scroll_down_one_arrow);
            shopScr.init_controls(scroll_up_one_arrow, scroll_down_one_arrow);
            //init the iconbar status icon textures, the function is a lil ambiguous.
            icoBar.init_textures(status_icon_list);
        }

        public void initalize_menu_wireframes()
        {
            Texture2D[] texture_masks = new Texture2D[6];
            load_wireframe_textures(ref texture_masks, p1.my_chara_as_string());

            string wframe_path = "";
            switch (p1.my_chara_as_string())
            {
                case "Falsael":
                    wframe_path = "UI Elements/Inventory Screen/falsael_wires/fal_wireframe";
                    break;
                case "Petaer":
                    wframe_path = "UI Elements/Inventory Screen/petaer_wires/pet_wireframe";
                    break;
                case "Halephon":
                    wframe_path = "UI Elements/Inventory Screen/halephon_wires/hal_wireframe";
                    break;
                case "Ziktofel":
                    wframe_path = "UI Elements/Inventory Screen/ziktofel_wires/zik_wireframe";
                    break;
            }

            invScr.init_wframes(Content.Load<Texture2D>(wframe_path), texture_masks);
            potiPrompt.init_textures(Content.Load<Texture2D>(wframe_path), texture_masks);
            pDoll.initialize_wframes(Content.Load<Texture2D>(wframe_path), texture_masks);
        }

        public void load_wireframe_textures(ref Texture2D[] targetArray, string character)
        {
            string short_character_name = "";
            string long_character_name = "";
            switch (character)
            {
                case "Falsael":
                    short_character_name = "fal_";
                    long_character_name = "falsael_";
                    break;
                case "Halephon":
                    short_character_name = "hal_";
                    long_character_name = "halephon_";
                    break;
                case "Ziktofel":
                    short_character_name = "zik_";
                    long_character_name = "ziktofel_";
                    break;
                case "Petaer":
                    short_character_name = "pet_";
                    long_character_name = "petaer_";
                    break;
            }

            string basePath = "UI Elements/Inventory Screen/" + long_character_name + "wires/" + short_character_name;
            targetArray[0] = Content.Load<Texture2D>(basePath + "head_mask");
            targetArray[1] = Content.Load<Texture2D>(basePath + "chest_mask");
            targetArray[2] = Content.Load<Texture2D>(basePath + "larm_mask");
            targetArray[3] = Content.Load<Texture2D>(basePath + "rarm_mask");
            targetArray[4] = Content.Load<Texture2D>(basePath + "lleg_mask");
            targetArray[5] = Content.Load<Texture2D>(basePath + "rleg_mask");
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

            if (current_state != Game_State.main_menu && current_state != Game_State.shop_screen &&
                current_state != Game_State.select_character && current_state != Game_State.won_game)
            {
                if (!f1.projectiles_remaining_to_update())
                    updateInput();

                f1.update_all_projectiles(p1, elapsedTime);
                f1.update_all_effects(elapsedTime);
                f1.update_all_popups(elapsedTime);
                mBall.update(elapsedTime);

                if (p1.is_spot_exit(f1))
                {
                    shopScr.set_variables(p1);
                    shopScr.switch_shopping_mode(ShopScreen.Shopping_Mode.Main);
                    p1.heal_naturally();
                    p1.refill_all_potions();
                    new_floor();
                    current_state = Game_State.shop_screen;
                    //gameState = 3;
                }

                if (p1.is_spot_dungeon_exit(f1))
                {
                    won_game = true;
                }

                if (bad_turn && !f1.projectiles_remaining_to_update())
                {
                    p1.deincrement_cooldowns_and_seffects(f1);
                    f1.add_smell_to_tile(p1.get_my_grid_C(), 0, p1.total_scent);
                    f1.sound_pulse(p1.get_my_grid_C(), p1.total_sound, SoundPulse.Sound_Types.Player);
                    p1.reset_sound_and_scent();

                    f1.update_dungeon_floor(p1);
                    bad_turn = false;
                    
                    icoBar.update_cooldown_and_quant(p1);
                    mBall.calculate_opacity(f1.check_mana());
                }

                if (!f1.done_smooth_transitions())
                    f1.smooth_transition_monster(elapsedTime);

                cam.Pos = p1.get_my_Position();
            }
            else
                updateInput();

            if (won_game)
            {
                current_state = Game_State.won_game;
                f1 = null;
            }

            //Only do this if it's visible!                

            // TODO: Add your update logic here
            //Hah, more like TODONE.

            //State updates - the sequel
            mouse_oldState = mouse_newState;
            oldState = newState;

            base.Update(gameTime);
        }

        private bool valid_iconbar_state()
        {
            return current_state == Game_State.bashing_attack ||
                   current_state == Game_State.casting_spell ||
                   current_state == Game_State.ranged_attack ||
                   current_state == Game_State.charging_attack ||
                   current_state == Game_State.normal ||
                   current_state == Game_State.drinking_potion;
        }

        private void updateInput()
        {
            bool just_changed_states = false;

            #region keypresses for the hotkey bar

            for (int i = 0; i < 8; i++)
            {
                if (check_key_press(hotbar_keymap[i]) && valid_iconbar_state() && p1.is_alive())
                    use_slot_on_icoBar_full(i, out just_changed_states);
            }

            #endregion

            #region keypresses for the main menu (GS = main_menu)
            //gameState == 0
            if (current_state == Game_State.main_menu)
            {
                if (check_key_release(Keys.Up) || check_key_release(Keys.NumPad8))
                {
                    sMenu.selected_index_up();
                }

                if (check_key_release(Keys.Down) || check_key_release(Keys.NumPad2))
                {
                    sMenu.selected_index_down();
                }

                if (check_key_release(Keys.Enter))
                {
                    switch (sMenu.selected_index())
                    {
                        case 0:
                            //gameState = 1;
                            current_state = Game_State.select_character;
                            cSelect.init_character_textures();
                            just_changed_states = true;
                            break;
                        default:
                            this.Exit();
                            break;
                    }

                }
            }
            #endregion

            #region keypresses for mini main menu or normal state

            if (current_state == Game_State.mini_main_menu || current_state == Game_State.normal)
            {
                if(check_key_press(Keys.Escape) || check_key_press(Keys.F10))
                {
                    if (current_state == Game_State.normal)
                    {
                        icoBar.hide();
                        miniMain.show();
                        current_state = Game_State.mini_main_menu;
                    }
                    else if (current_state == Game_State.mini_main_menu)
                    {
                        icoBar.show();
                        miniMain.hide();
                        current_state = Game_State.normal;
                    }
                } 
            }

            #endregion

            #region keypresses for mini main menu only

            if (current_state == Game_State.mini_main_menu)
            {
                if (check_key_release(Keys.NumPad8) || check_key_release(Keys.Up))
                {
                    miniMain.scroll_menu(-1);
                }

                if (check_key_release(Keys.NumPad2) || check_key_release(Keys.Down))
                {
                    miniMain.scroll_menu(1);
                }

                if (check_key_release(Keys.Enter))
                {
                    switch (miniMain.get_index())
                    {
                        case 0:
                            current_state = Game_State.select_character;
                            cSelect.switch_mode();
                            cSelect.init_character_textures();
                            just_changed_states = true;
                            icoBar.show();
                            miniMain.hide();
                            break;
                        case 4:
                            current_state = Game_State.main_menu;
                            icoBar.show();
                            miniMain.hide();
                            break;
                        case 5:
                            this.Exit();
                            break;
                    }
                }
            }

            #endregion

            #region keypresses for when the game is playing (GS = normal)
            //gameState == 1
            if (current_state == Game_State.normal)
            {
                for (int i = 0; i < direction_keymap.Count(); i++)
                    if (check_key_press(direction_keymap[i]) && p1.is_alive())
                    {
                        p1.move(direction_map[i], f1);
                        bad_turn = true;
                    }

                if (check_key_press(Keys.NumPad5))
                {
                    p1.wait();
                    bad_turn = true;
                }

                if (check_mouse_left_click())
                {
                    int x_position = (int)((-cam.viewMatrix.Translation.X + mousePosition.X) / 32);
                    int y_position = (int)((-cam.viewMatrix.Translation.Y + mousePosition.Y) / 32);
                    gridCoordinate click_loc = new gridCoordinate(x_position, y_position);
                    f1.check_click(p1, click_loc, out bad_turn);
                }

                if (check_key_press(Keys.Space))
                    if (msgBuf.Count > 0)
                        msgBufBox.scrollMSG(1);

                if (check_key_press(Keys.N))
                    icoBar.switch_my_mode();
            }
            #endregion

            #region keypresses for either game or inventory screen (GS = 1 || 2)
            //gameState == 1 || gameState == 2
            if (current_state == Game_State.normal || current_state == Game_State.inv_screen)
            {
                if (check_key_press(Keys.I))
                {
                    invScr.switch_my_mode();
                    invScr.update_player_info(ref p1);
                    if (invScr.is_visible())
                        current_state = Game_State.inv_screen;
                    //gameState = 2;
                    else
                        current_state = Game_State.normal;
                        //gameState = 1;
                }
            }
            #endregion

            #region keypresses for when the inventory screen is showing, this is mostly a mouse thing (GS = 2)
            //gameState == 2
            if (current_state == Game_State.inv_screen)
            {
                invScr.update_mouse_info(mousePosition, p1, check_mouse_hold_left(), check_mouse_left_click(), out bad_turn);

                if (check_mouse_left_click())
                {
                    msgBufBox.mouseClick(mousePosition);
                }

                if (check_key_press(Keys.Escape))
                {
                    invScr.switch_my_mode();
                    if (!invScr.is_visible())
                        current_state = Game_State.normal;
                        //gameState = 1;
                }
            }
            #endregion

            #region keypresses for when there's a shop screen (GS = 3)
            //gameState == 3
            if (current_state == Game_State.shop_screen)
            {
                if ((check_key_release(Keys.Up) || check_key_release(Keys.NumPad8)) && 
                    !check_key_hold(Keys.LeftShift))
                    shopScr.scroll_menu(-1);

                if ((check_key_release(Keys.Down) || check_key_release(Keys.NumPad2)) &&
                    !check_key_hold(Keys.LeftShift))
                    shopScr.scroll_menu(1);

                if(check_key_release(Keys.NumPad9) && shopScr.in_submenu())
                    shopScr.scroll_iteminfo_menu(-1);

                if (check_key_release(Keys.NumPad3) && shopScr.in_submenu())
                    shopScr.scroll_iteminfo_menu(1);

                if (check_mouse_left_click())
                    shopScr.mouse_click(mousePosition);

                if (check_key_hold(Keys.LeftShift) && shopScr.in_comparable_submenu())
                    shopScr.run_comparison(p1);
                else
                    shopScr.clear_comparison();

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
                                shopScr.switch_shopping_mode(ShopScreen.Shopping_Mode.Scrolls);
                                break;
                            case 3:
                                //Consumables
                                shopScr.switch_shopping_mode(ShopScreen.Shopping_Mode.Consumables);
                                break;
                            case 4:
                                //Talismans
                                shopScr.switch_shopping_mode(ShopScreen.Shopping_Mode.Talismans);
                                break;
                            case 5:
                                //Sell
                                shopScr.switch_shopping_mode(ShopScreen.Shopping_Mode.Sell);
                                break;
                            case 6:
                                //buy back
                                shopScr.switch_shopping_mode(ShopScreen.Shopping_Mode.Buyback);
                                break;
                            case 7:
                                icoBar.purge_sold_items(p1);
                                icoBar.update_cooldown_and_quant(p1);
                                invScr.init_necessary_textures(p1);
                                current_state = Game_State.normal;
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
            //gameState == 1 || gameState == 4
            if (current_state == Game_State.normal || current_state == Game_State.msg_log)
            {
                if (check_key_press(Keys.M))
                {
                    //gameState == 1
                    if (current_state == Game_State.normal)
                    {
                        msgBufBox.switch_my_mode();
                        current_state = Game_State.msg_log;
                        //gameState = 4;
                    }
                    else
                    {
                        msgBufBox.switch_my_mode();
                        current_state = Game_State.normal;
                        //gameState = 1;
                    }
                }
            }
            #endregion

            #region keypresses for when the message log is showing (GS = 4, mouse only)
            //gameState == 4
            if (current_state == Game_State.msg_log)
            {   
                if (check_mouse_left_click())
                    msgBufBox.mouseClick(new Vector2(mouse_newState.X, mouse_newState.Y));
            }
            #endregion

            #region keypresses for either game or shooting (GS = 1 || 5)
            //gameState == 1 || gameState == 5
            if (current_state == Game_State.normal || current_state == Game_State.ranged_attack)
            {
                if (check_key_press(Keys.Enter) && p1.is_bow_equipped() && p1.is_alive())
                {
                    //gameState == 1
                    if (current_state == Game_State.normal)
                    {
                        start_ranged_attack();
                    }
                    else
                    {
                        ranged_attack_via_cursor(ra1.my_grid_coord);
                    }
                }
            }
            #endregion

            #region keypresses for shooting or charging or bashing or spellcasting! (GS = 5 || 6)
            //gameState == 5 || gameState == 6
            if (current_state == Game_State.ranged_attack || current_state == Game_State.charging_attack ||
                current_state == Game_State.bashing_attack || current_state == Game_State.casting_spell)
            {
                for (int i = 0; i < direction_keymap.Count(); i++)
                    if (check_key_press(direction_keymap[i]))
                        ra1.shift_coordinates(direction_map[i]);

                #region only while shooting (GS = 5)
                //gameState == 5
                if (current_state == Game_State.ranged_attack)
                {
                    if (check_mouse_left_click())
                    {
                        int gc_xloc = (int)((mousePosition.X - cam.viewMatrix.Translation.X) / 32);
                        int gc_yloc = (int)((mousePosition.Y - cam.viewMatrix.Translation.Y) / 32);
                        gridCoordinate click_location = new gridCoordinate(gc_xloc, gc_yloc);
                        ranged_attack_via_cursor(click_location);
                        cancel_all_specials();
                    }
                }
                #endregion

                #region only while charging (GS = 6)
                //gameState == 6
                if (current_state == Game_State.charging_attack)
                {
                    if(check_key_press(Keys.Enter))
                    {
                        Weapon lance = p1.get_weapon_by_ID(selected_lance);
                        charge_attack_via_cursor(lance, ra1.my_grid_coord);
                    }

                    if (check_mouse_left_click())
                    {
                        int gc_xloc = (int)((mousePosition.X - cam.viewMatrix.Translation.X) / 32);
                        int gc_yloc = (int)((mousePosition.Y - cam.viewMatrix.Translation.Y) / 32);
                        gridCoordinate click_location = new gridCoordinate(gc_xloc, gc_yloc);
                        Weapon lance = p1.get_weapon_by_ID(selected_lance);
                        charge_attack_via_cursor(lance, ra1.my_grid_coord);
                        cancel_all_specials();
                    }
                }

                #endregion

                #region only while bashing
                if (current_state == Game_State.bashing_attack)
                {
                    if (check_key_press(Keys.Enter))
                    {
                        Weapon mace = p1.get_weapon_by_ID(selected_mace);
                        bashing_attack_via_cursor(mace, ra1.my_grid_coord);
                    }

                    if (check_mouse_left_click())
                    {
                        int gc_xloc = (int)((mousePosition.X - cam.viewMatrix.Translation.X) / 32);
                        int gc_yloc = (int)((mousePosition.Y - cam.viewMatrix.Translation.Y) / 32);
                        gridCoordinate click_location = new gridCoordinate(gc_xloc, gc_yloc);
                        Weapon mace = p1.get_weapon_by_ID(selected_mace);
                        bashing_attack_via_cursor(mace, click_location);
                        cancel_all_specials();
                    }
                }
                #endregion

                #region while spellcasting

                if (current_state == Game_State.casting_spell)
                {
                    if (check_key_press(Keys.Enter))
                    {
                        spell_attack_via_cursor(selected_scroll, ra1.my_grid_coord);
                    }

                    if (check_mouse_left_click())
                    {
                        int gc_xloc = (int)((mousePosition.X - cam.viewMatrix.Translation.X) / 32);
                        int gc_yloc = (int)((mousePosition.Y - cam.viewMatrix.Translation.Y) / 32);
                        gridCoordinate click_location = new gridCoordinate(gc_xloc, gc_yloc);
                        spell_attack_via_cursor(selected_scroll, click_location);
                        cancel_all_specials();
                    }
                }

                #endregion
            }
            #endregion

            #region keypresses for taking potions (GS = 7)
            //gameState == 7
            if (current_state == Game_State.drinking_potion)
            {
                if (check_mouse_left_click())
                {
                    if (potiPrompt.clicked_a_zone(mousePosition))
                    {
                        string healing_zone = "";
                        bool ingested_potion = false;

                        if (potiPrompt.clicked_ingest_zone(mousePosition))
                        {
                            p1.ingest_potion(potiPrompt.fetch_current_potion(), f1, potiPrompt.get_repair_armor());
                            ingested_potion = true;
                            if(String.Compare(p1.my_chara_as_string(), "Halephon") != 0)
                                bad_turn = true;
                        }
                        else if (potiPrompt.clicked_head_zone(mousePosition))
                            healing_zone = "Head";
                        else if (potiPrompt.clicked_torso_zone(mousePosition))
                            healing_zone = "Chest";
                        else if (potiPrompt.clicked_larm_zone(mousePosition))
                            healing_zone = "LArm";
                        else if (potiPrompt.clicked_rarm_zone(mousePosition))
                            healing_zone = "RArm";
                        else if (potiPrompt.clicked_lleg_zone(mousePosition))
                            healing_zone = "LLeg";
                        else if (potiPrompt.clicked_rleg_zone(mousePosition))
                            healing_zone = "RLeg";

                        if (!ingested_potion && !potiPrompt.clicked_cancel_zone(mousePosition))
                        {
                            p1.heal_via_potion(potiPrompt.fetch_current_potion(), healing_zone, potiPrompt.get_repair_armor(), f1);
                            bad_turn = true;
                        }

                        //gameState = 1;
                        cancel_all_specials();
                    }

                    if (potiPrompt.clicked_OA_tab(mousePosition) &&
                        potiPrompt.fetch_current_potion().get_type() == Potion.Potion_Type.Repair)
                        potiPrompt.set_repair_armor(true);
                    else if (potiPrompt.clicked_UA_tab(mousePosition) &&
                        potiPrompt.fetch_current_potion().get_type() == Potion.Potion_Type.Repair)
                        potiPrompt.set_repair_armor(false);
                }
            }

            #endregion

            #region keypresses for character selection

            if (current_state == Game_State.select_character)
            {
                if(check_key_release(Keys.Left) || check_key_release(Keys.NumPad4))
                    cSelect.scroll_menu(-1);

                if(check_key_release(Keys.Right) || check_key_release(Keys.NumPad6))
                    cSelect.scroll_menu(1);

                if (check_key_release(Keys.Up) || check_key_release(Keys.NumPad8))
                    cSelect.scroll_menu(-2);

                if (check_key_release(Keys.Down) || check_key_release(Keys.NumPad2))
                    cSelect.scroll_menu(2);

                if (check_key_release(Keys.Enter) && !just_changed_states)
                {
                    if (cSelect.selecting_character())
                        cSelect.switch_mode();
                    else
                    {
                        start_new_game(cSelect.get_current_character_selection(),
                                       cSelect.get_current_class_selection());
                        current_state = Game_State.normal;
                    }   
                }
            }

            #endregion

            #region keypresses for winning the game

            if (current_state == Game_State.won_game)
            {
                if(check_key_release(Keys.F10))
                {
                    current_state = Game_State.main_menu;

                    won_game = false;
                    if (!cSelect.selecting_character())
                        cSelect.switch_mode();
                    cSelect.init_character_textures();
                    icoBar.show();
                }
            }

            #endregion

            // Update saved state.            
        }

        //ranged + charge attack region
        #region some ranged / charge attack stuff - stuffed in a function to avoid repetition
        private void start_ranged_attack()
        {
            ra1.shift_modes(RACursor.Mode.Ranged);
            //gameState = 5;
            current_state = Game_State.ranged_attack;
            p1.set_ranged_attack_aura(f1, p1.get_my_grid_C(), null);
            ra1.am_i_visible = true;
            ra1.my_grid_coord = new gridCoordinate(p1.get_my_grid_C().x, p1.get_my_grid_C().y - 1);
            ra1.reset_drawing_position();
        }

        private void ranged_attack_via_cursor(gridCoordinate click_location)
        {
            int monster_no = -1;
            int Doodad_no = -1;
            if ((f1.is_monster_here(click_location, out monster_no) ||
                f1.is_destroyable_Doodad_here(click_location, out Doodad_no)) && 
                f1.aura_of_specific_tile(click_location) == Tile.Aura.Attack)
            {
                p1.bow_attack(f1, ref Secondary_cManager, click_location, monster_no, Doodad_no);
                bad_turn = true;
            }

            //gameState = 1;
            cancel_all_specials();
        }

        private void start_charge_attack(int lanceID)
        {
            ra1.shift_modes(RACursor.Mode.Charge);
            //gameState = 6;
            current_state = Game_State.charging_attack;
            p1.set_charge_attack_aura(f1, lanceID, p1.get_my_grid_C());
            ra1.am_i_visible = true;
            ra1.my_grid_coord = new gridCoordinate(p1.get_my_grid_C().x, p1.get_my_grid_C().y - 1);
            ra1.reset_drawing_position();
        }

        private void charge_attack_via_cursor(Weapon lance, gridCoordinate click_location)
        {
            int monster_no = -1;
            int Doodad_no = -1;
            if ((f1.is_monster_here(click_location, out monster_no) || 
                f1.is_destroyable_Doodad_here(click_location, out Doodad_no)) && 
                f1.aura_of_specific_tile(click_location) == Tile.Aura.Attack)
            {
                gridCoordinate effect_coord = new gridCoordinate(-1, -1);
                if(f1.is_monster_here(click_location, out monster_no))
                    effect_coord = click_location;
                else
                    effect_coord = f1.Doodad_by_index(Doodad_no).get_g_coord();
                f1.add_effect(Attack.Damage.Piercing, effect_coord);
                p1.charge_attack(f1, lance, click_location, monster_no, Doodad_no);
                bad_turn = true;
            }

            //gameState = 1;
            cancel_all_specials();
        }

        private void start_bash_attack()
        {
            ra1.shift_modes(RACursor.Mode.Bash);
            current_state = Game_State.bashing_attack;
            p1.set_melee_attack_aura(f1);
            ra1.am_i_visible = true;
            ra1.my_grid_coord = new gridCoordinate(p1.get_my_grid_C().x, p1.get_my_grid_C().y - 1);
            ra1.reset_drawing_position();
        }

        private void bashing_attack_via_cursor(Weapon w, gridCoordinate click_location)
        {
            int monster_no = -1;
            int Doodad_no = -1;
            if ((f1.is_monster_here(click_location, out monster_no) ||
                f1.is_destroyable_Doodad_here(click_location, out Doodad_no)) && 
                f1.aura_of_specific_tile(click_location) == Tile.Aura.Attack)
            {
                p1.bash_attack(f1, f1.badguy_by_monster_id(monster_no), click_location, f1.Doodad_by_index(Doodad_no), w);
                bad_turn = true;
            }

            cancel_all_specials();
        }

        private void start_spell_attack(Scroll s)
        {
            if (s.get_spell_type() != Scroll.Atk_Area_Type.personalBuff)
            {
                ra1.shift_modes(RACursor.Mode.Spell);
                selected_scroll = s.get_my_IDno();
                //gameState = 5;
                current_state = Game_State.casting_spell;
                if (s.is_melee_range_spell())
                    p1.set_melee_attack_aura(f1);
                else
                    p1.set_ranged_attack_aura(f1, p1.get_my_grid_C(), s);
                ra1.am_i_visible = true;
                ra1.my_grid_coord = new gridCoordinate(p1.get_my_grid_C().x, p1.get_my_grid_C().y - 1);
                ra1.reset_drawing_position();
            }
            else
            {
                int floor_mana_consumed = s.get_manaCost();
                int mana_on_floor = f1.check_mana();

                if (mana_on_floor >= floor_mana_consumed)
                {
                    f1.consume_mana(floor_mana_consumed);
                    p1.cast_spell(s, f1, new gridCoordinate(-1, -1), 0, 0);
                    bad_turn = true;
                }
                else
                    f1.add_new_popup("Not enough mana!", Popup.popup_msg_color.Purple, p1.get_my_grid_C());
                cancel_all_specials();
            }
        }

        private void spell_attack_via_cursor(int Scroll_ID, gridCoordinate click_location)
        {
            int monster_no = -1;
            int Doodad_no = -1;

            Scroll s = p1.get_scroll_by_ID(Scroll_ID);
            int floor_mana_consumed = s.get_manaCost();
            int mana_on_floor = f1.check_mana();

            if (f1.aura_of_specific_tile(click_location) == Tile.Aura.Attack)
            {
                if (s.is_AoE_Spell())
                {
                    if (mana_on_floor >= floor_mana_consumed)
                    {
                        f1.consume_mana(floor_mana_consumed);
                        p1.cast_spell(s, f1, click_location, monster_no, Doodad_no);
                        bad_turn = true;
                    }
                    else
                        f1.add_new_popup("Not enough mana!", Popup.popup_msg_color.Purple, p1.get_my_grid_C());
                }
                else
                {
                    if ((f1.is_monster_here(click_location, out monster_no) ||
                        f1.is_destroyable_Doodad_here(click_location, out Doodad_no)) &&
                        f1.aura_of_specific_tile(click_location) == Tile.Aura.Attack)
                    {
                        if (mana_on_floor >= floor_mana_consumed)
                        {
                            f1.consume_mana(floor_mana_consumed);
                            p1.cast_spell(s, f1, click_location, monster_no, Doodad_no);
                            bad_turn = true;
                        }
                        else
                            f1.add_new_popup("Not enough mana!", Popup.popup_msg_color.Purple, p1.get_my_grid_C());
                    }
                }
            }
            mBall.calculate_opacity((double)f1.check_mana());

            cancel_all_specials();
        }

        private void cancel_all_specials()
        {
            if (current_state == Game_State.drinking_potion)
            {
                p1.acquire_potion(potiPrompt.fetch_current_potion());
                potiPrompt.clear_potion();
                potiPrompt.hide();
            }

            current_state = Game_State.normal;
            locked_slot = -1;
            f1.scrub_all_auras();
            ra1.am_i_visible = false;
        }
        #endregion

        private void use_slot_on_icoBar_full(int slot, out bool changed_states)
        {
            bool use_slot = true;
            if (slot != locked_slot && locked_slot != -1)
                use_slot = false;

            changed_states = false;
            if (use_slot)
            {
                int item_ID = icoBar.get_item_IDs_by_slot(slot);
                if (item_ID > -1)
                {
                    string item_type = icoBar.get_item_type_by_slot(slot);
                    bool is_equipped = p1.is_item_equipped(item_ID);
                    if (!is_equipped)
                    {
                        if (String.Compare(item_type, "Weapon") == 0)
                        {
                            Weapon c_weapon = p1.get_weapon_by_ID(item_ID);
                            if (c_weapon.get_my_weapon_type() != Weapon.Type.Lance)
                            {
                                if (!p1.body_part_disabled("LArm") && !p1.body_part_disabled("RArm"))
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
                                    if (c_weapon.get_hand_count() == 1)
                                    {
                                        if (p1.body_part_disabled("LArm") && !p1.body_part_disabled("RArm"))
                                            p1.equip_off_hand(c_weapon);
                                        if (!p1.body_part_disabled("LArm") && p1.body_part_disabled("RArm"))
                                            p1.equip_main_hand(c_weapon);
                                    }
                                }

                            }
                            else if (c_weapon.get_my_weapon_type() == Weapon.Type.Lance)
                            {
                                if (current_state == Game_State.charging_attack)
                                {
                                    cancel_all_specials();
                                }
                                else
                                {
                                    locked_slot = slot;
                                    start_charge_attack(item_ID);
                                    selected_lance = item_ID;
                                    changed_states = true;
                                }
                            }
                        }
                        else if (String.Compare(item_type, "Armor") == 0)
                        {
                            Armor c_armor = p1.get_armor_by_ID(item_ID);
                            if (c_armor.what_armor_type() == Armor.Armor_Type.Helmet)
                                p1.equip_helmet(c_armor);
                            else if (c_armor.what_armor_type() == Armor.Armor_Type.OverArmor)
                                p1.equip_over_armor(c_armor);
                            else if (c_armor.what_armor_type() == Armor.Armor_Type.UnderArmor)
                                p1.equip_under_armor(c_armor);
                        }
                        else if (String.Compare(item_type, "Potion") == 0)
                        {
                            Potion c_potion = p1.get_potion_by_ID(item_ID);
                            if (current_state == Game_State.drinking_potion)
                            {
                                if (c_potion != null)
                                    p1.acquire_potion(c_potion);
                                cancel_all_specials();
                            }
                            else
                            {
                                if (c_potion != null)
                                {
                                    locked_slot = slot;
                                    potiPrompt.show();
                                    potiPrompt.current_potion(c_potion);
                                    potiPrompt.grab_injury_report(p1);
                                    //gameState = 7;
                                    current_state = Game_State.drinking_potion;
                                    changed_states = true;
                                }
                            }
                        }
                        else if (string.Compare(item_type, "Scroll") == 0)
                        {
                            if (current_state == Game_State.casting_spell)
                            {
                                cancel_all_specials();
                            }
                            else
                            {
                                locked_slot = slot;
                                Scroll s = p1.get_scroll_by_ID(item_ID);
                                start_spell_attack(s);
                                changed_states = true;
                            }
                        }
                    }
                    else //if it is equipped...
                    {
                        if (String.Compare(item_type, "Weapon") == 0)
                        {
                            Weapon c_weapon = p1.get_weapon_by_ID(item_ID);
                            if (c_weapon.get_my_weapon_type() == Weapon.Type.Bow ||
                                c_weapon.get_my_weapon_type() == Weapon.Type.Crossbow)
                            {
                                if (current_state == Game_State.ranged_attack)
                                {
                                    cancel_all_specials();
                                }
                                else
                                {
                                    start_ranged_attack();
                                    changed_states = true;
                                    locked_slot = slot;
                                }
                            }

                            if (c_weapon.get_my_weapon_type() == Weapon.Type.Sword)
                            {
                                if (c_weapon.get_current_cooldown() == 0)
                                {
                                    p1.whirlwind_attack(f1, c_weapon);
                                    bad_turn = true;
                                }
                            }

                            if (c_weapon.get_my_weapon_type() == Weapon.Type.Mace)
                            {
                                if (c_weapon.get_current_cooldown() == 0)
                                {
                                    if (current_state == Game_State.bashing_attack)
                                    {
                                        cancel_all_specials();
                                    }
                                    else
                                    {
                                        selected_mace = c_weapon.get_my_IDno();
                                        start_bash_attack();
                                        changed_states = true;
                                        locked_slot = slot;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        //check to see how they / mouse button is being used region.
        #region input key check types

        private bool check_key_press(Keys theKey)
        {
            return newState.IsKeyDown(theKey) && !oldState.IsKeyDown(theKey);
        }

        private bool check_key_release(Keys theKey)
        {
            return !newState.IsKeyDown(theKey) && oldState.IsKeyDown(theKey);
        }

        private bool check_key_hold(Keys theKey)
        {
            return newState.IsKeyDown(theKey) && oldState.IsKeyDown(theKey);
        }

        private bool check_mouse_left_click()
        {
            return mouse_newState.LeftButton == ButtonState.Pressed && mouse_oldState.LeftButton == ButtonState.Released;
        }

        private bool check_mouse_hold_left()
        {
            return mouse_newState.LeftButton == ButtonState.Pressed && mouse_oldState.LeftButton == ButtonState.Pressed;
        }

        #endregion

        public void start_new_game(int character_number, int class_number)
        {
            Player.Character chara = Player.Character.Falsael;
            Player.Chara_Class chClass = Player.Chara_Class.Warrior;
            switch (character_number)
            {
                case 0:
                    chara = Player.Character.Petaer;
                    break;
                case 1:
                    chara = Player.Character.Ziktofel;
                    break;
                case 2:
                    chara = Player.Character.Halephon;
                    break;
                case 3:
                    chara = Player.Character.Falsael;
                    break;
            }

            switch (class_number)
            {
                case 0:
                    chClass = Player.Chara_Class.Warrior;
                    break;
                case 1:
                    chClass = Player.Chara_Class.Rogue;
                    break;
                case 2:
                    chClass = Player.Chara_Class.Mage;
                    break;
                case 3:
                    chClass = Player.Chara_Class.ExPriest;
                    break;
            }

            current_dungeon = Dungeon.Necropolis;
            miniMain.set_index(0);
            current_floor = 0;
            p1 = new Player(Content, new gridCoordinate(-1, -1), ref msgBuf, chClass, 
                            chara, ref pDoll);
            //Attach the iconbar status effect tracker to the player's
            List<StatusEffect> sEffects = p1.get_status_effects();
            icoBar.attach_player_sEffects(ref sEffects);
            //Then init wireframes based on which character you picked
            initalize_menu_wireframes();
            p1.update_pdoll();
            icoBar.wipe();
            new_floor();
        }

        public void new_floor()
        {
            Texture2D blank_texture = new Texture2D(GraphicsDevice, 1, 1);
            current_floor++;
            f1 = new Floor(Content, ref msgBuf, blank_texture, current_floor, current_dungeon);
            p1.teleport(f1.get_entrance_coord());
            bad_turn = false;
            msgBuf.Clear();
            msgBufBox.scrollMSG(-1000);
            mBall.calculate_opacity(f1.check_mana());
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            switch (current_state)
            {
                case Game_State.main_menu:
                    spriteBatch.Begin(SpriteSortMode.BackToFront, null);
                    sMenu.drawMe(ref spriteBatch);
                    spriteBatch.End();
                    break;
                case Game_State.shop_screen:
                    shopScr.drawMe(ref spriteBatch);
                    break;
                case Game_State.select_character:
                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied);
                    cSelect.draw_me(ref spriteBatch);
                    spriteBatch.End();
                    break;
                case Game_State.won_game:
                    SpriteFont vicMsgFnt = Content.Load<SpriteFont>("Fonts/tfont");
                    string vicMsgStr = "You won!!! Press F10 for new game.";
                    Vector2 vicMsgPos = new Vector2((client_rect().Width - vicMsgFnt.MeasureString(vicMsgStr).X)/2,
                                                    (client_rect().Height - vicMsgFnt.LineSpacing) /2);
                    spriteBatch.Begin(SpriteSortMode.BackToFront, null);
                    spriteBatch.DrawString(vicMsgFnt, vicMsgStr, vicMsgPos, Color.White);
                    spriteBatch.End();
                    break;
                default:
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
            {
                icoBar.draw_me(ref spriteBatch);
                pDoll.draw_me(ref spriteBatch);
                mBall.draw_me(ref spriteBatch);
            }

            if (invScr.is_visible())
                invScr.draw_me(ref spriteBatch, p1);

            if (potiPrompt.is_visible())
                potiPrompt.draw_me(ref spriteBatch);

            if (miniMain.is_visible())
                miniMain.draw_me(ref spriteBatch);
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
