using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Cronkpit
{
    public struct rect_inv_item_pair
    {
        public int item_rect;
        public int corr_item;

        public rect_inv_item_pair(int rect, int item)
        {
            item_rect = rect;
            corr_item = item;
        }
    }

    class InvAndHealthScreen
    {
        ContentManager cManager;
        Vector2 mousePosition;
        bool is_holding_mouse;
        bool is_click_mouse;

        SpriteFont stdFont;

        Rectangle my_size;
        int height = 480;
        int width = 580;
        int my_xPosition = 110;
        int my_yPosition = 20;

        Rectangle BGElement_portraitBackground;
        int ptBG_height = 390;
        int ptBG_width = 200;
        int ptBG_xPos = 10;
        int ptBG_yPos = 10;
        int wframe_xpos;
        int wframe_ypos;
        Texture2D character_wireframe;

        //Player portrait and name
        Rectangle BGElement_playerPortrait;
        int plPT_width_height = 80;
        string player_name;
        Vector2 player_name_pos;
        Texture2D character_portrait;
        SpriteFont player_name_font;

        Rectangle BGElement_backpackBackground;
        int bpbBG_height = 70;
        int bpbBG_width = 560;
        int bpbBG_xPos = 10;
        int bpbBG_yPos = 405;

        //Tab system!
        public enum Current_Tab { Equipment, Injuries };
        Current_Tab current_tab_selected;
        //General stuff
        Rectangle BGElement_ctabBackground;
        Rectangle BGElement_equipmentTab; //Tab 1
        Rectangle BGElement_healthTab; //Tab 2
        int tab_index_height = 30;
        int ctab_height = 360;
        int ctab_width = 350;
        int ctab_xPos;
        int ctab_yPos;

        Vector2 ctab_msg_pos;

        //Equipment stuff
        //string weapons_title;
        string equips_title;
        Vector2 equip_title_pos;

        int player_gold;
        Vector2 goldmsg_pos;

        int equip_msg_start_index;

        //Literally all of this is injury related stuff
        string inj_title;
        Vector2 inj_title_pos;
        int inj_start_index;
        int total_messages_shown;

        Texture2D my_back_texture;
        Texture2D[] texture_masks;

        Texture2D injSummary_scroll_up_max;
        Texture2D injSummary_scroll_up_one;
        Texture2D injSummary_scroll_down_max;
        Texture2D injSummary_scroll_down_one;

        Texture2D inv_left_scroll_texture;
        Texture2D inv_right_scroll_texture;

        Rectangle injSummary_scroll_up_max_rect;
        Rectangle injSummary_scroll_up_one_rect;
        Rectangle injSummary_scroll_down_max_rect;
        Rectangle injSummary_scroll_down_one_rect;

        Rectangle inv_left_scroll_rect;
        Rectangle inv_right_scroll_rect;

        //We finally leave that here to go into COLORS
        Color my_dark_color;
        Color my_grey_color;
        Color my_red_color;
        Color my_text_color;
        Color my_d_grey_color;

        int mode;

        bool visible;

        SpriteFont section_titleFont;

        //Player information:
        int[] wounds_by_part;
        int[] max_health_by_part;

        List<string> pl_injury_report;

        Rectangle main_hand_equip_slot;
        Rectangle off_hand_equip_slot;
        Rectangle over_armor_equip_slot;
        Rectangle under_armor_equip_slot;
        Rectangle helmet_equip_slot;
        Rectangle draggable_item_rect;
        List<Rectangle> armor_equip_slots;

        List<string> pl_equipment_report;

        //Item information!
        int item_start_index = 0;
        int number_of_items_shown = 8;
        int index_of_mouse_selected_item;
        int index_of_c_equipped_selected_item;
        int target_draggable_index;
        Vector2 item_start_position;
        List<Rectangle> item_icon_rects;
        List<Item> player_inv;
        List<rect_inv_item_pair> pair_list;

        //And now the reference to the icon box because we want to be able to manipulate that from here.
        IconBar the_icoBar;

        //There's gonna have to be fonts and stuff here too for now, but this will be okay
        //FOR THE TIME BEING...

        public InvAndHealthScreen(Texture2D my_default_backTex, SpriteFont smallFont, SpriteFont largeFont, SpriteFont pname_font, ref ContentManager cm, ref IconBar s_icoBar)
        {
            cManager = cm;

            visible = false;
            mode = 0;
            section_titleFont = largeFont;
            stdFont = smallFont;
            player_name_font = pname_font;

            my_size = new Rectangle(my_xPosition, my_yPosition, width, height);
            BGElement_portraitBackground = new Rectangle(my_xPosition + ptBG_xPos, my_yPosition + ptBG_yPos, 
                                                        ptBG_width, ptBG_height);
            int player_portrait_xpos = my_xPosition + ptBG_xPos + BGElement_portraitBackground.Width - plPT_width_height - 10;
            int player_portrait_ypos = my_yPosition + ptBG_yPos + 10;
            BGElement_playerPortrait = new Rectangle(player_portrait_xpos, player_portrait_ypos, plPT_width_height, plPT_width_height);
            
            player_name_pos = new Vector2(my_xPosition + ptBG_xPos + 10, my_yPosition + ptBG_yPos + 10);
            goldmsg_pos = new Vector2(player_name_pos.X, player_name_pos.Y + player_name_font.LineSpacing + 5);

            BGElement_backpackBackground = new Rectangle(my_xPosition + bpbBG_xPos, my_yPosition + bpbBG_yPos,
                                                        bpbBG_width, bpbBG_height);
            
            inj_title = "Health:";
            equips_title = "Armor:";
            //Equip stuff
            //equip_start_X = my_xPosition + ptBG_xPos + ptBG_width + esbBG_xPos;
            //equip_start_Y = my_yPosition + esbBG_yPos;
            
            //Rectangles for various equip slots.
            int e_rects_start_Y = my_yPosition + ptBG_yPos;
            main_hand_equip_slot = new Rectangle(my_xPosition + ptBG_xPos + 10, e_rects_start_Y + 225, 48, 48);
            off_hand_equip_slot = new Rectangle(my_xPosition + ptBG_xPos + 140, e_rects_start_Y + 225, 48, 48);
            over_armor_equip_slot = new Rectangle(my_xPosition + ptBG_xPos + ((ptBG_width - 48) / 2), e_rects_start_Y + 180, 48, 48);
            under_armor_equip_slot = new Rectangle(my_xPosition + ptBG_xPos + ((ptBG_width - 48 ) / 2), e_rects_start_Y + 250, 48, 48);
            helmet_equip_slot = new Rectangle(my_xPosition + ptBG_xPos + ((ptBG_width - 48) / 2), e_rects_start_Y + 115, 48, 48);
            draggable_item_rect = new Rectangle(0, 0, 48, 48);
            index_of_mouse_selected_item = -1;
            armor_equip_slots = new List<Rectangle>();
            armor_equip_slots.Add(over_armor_equip_slot);
            armor_equip_slots.Add(under_armor_equip_slot);
            armor_equip_slots.Add(helmet_equip_slot);

            //Tab system. Better than the other stuff.
            current_tab_selected = Current_Tab.Equipment;
            ctab_xPos = my_xPosition + ptBG_xPos + ptBG_width + 10;
            ctab_yPos = my_yPosition + 40;
            BGElement_ctabBackground = new Rectangle(ctab_xPos, ctab_yPos, ctab_width, ctab_height);
            ctab_msg_pos = new Vector2(BGElement_ctabBackground.X + 10, BGElement_ctabBackground.Y + 10);
            //Tab 1 + related things.
            BGElement_equipmentTab = new Rectangle(ctab_xPos, ctab_yPos-30, 20+(int)stdFont.MeasureString(equips_title).X, tab_index_height);

            equip_title_pos = new Vector2(BGElement_equipmentTab.X + 10, BGElement_equipmentTab.Y + 10);
            equip_msg_start_index = 0;
            //Tab 2 + related things.
            int h_tab_start_X = ctab_xPos + BGElement_equipmentTab.Width + 10;
            BGElement_healthTab = new Rectangle(h_tab_start_X, ctab_yPos-30, 20 + (int)stdFont.MeasureString(inj_title).X, tab_index_height);

            inj_title_pos = new Vector2(BGElement_healthTab.X + 10, BGElement_healthTab.Y + 10);
            //General tab stuff
            inj_start_index = 0;
            total_messages_shown = (int)( (BGElement_ctabBackground.Height - 10) / stdFont.LineSpacing);
            
            //Scrollbox options.         
            int scrollElements_x = ctab_xPos + BGElement_ctabBackground.Width - 20;
            int spacing = 4;
            int first_y = BGElement_ctabBackground.Y + spacing;
            int second_y = BGElement_ctabBackground.Y + 18 + (spacing * 2);
            int third_y = BGElement_ctabBackground.Y + BGElement_ctabBackground.Height - 18 - spacing;
            int fourth_y = BGElement_ctabBackground.Y + BGElement_ctabBackground.Height - (18 * 2) - (spacing * 2);
            injSummary_scroll_up_max_rect = new Rectangle(scrollElements_x, first_y, 18, 18);
            injSummary_scroll_up_one_rect = new Rectangle(scrollElements_x, second_y, 18, 18);
            injSummary_scroll_down_max_rect = new Rectangle(scrollElements_x, third_y, 18, 18);
            injSummary_scroll_down_one_rect = new Rectangle(scrollElements_x, fourth_y, 18, 18);
            
            //Item stuff
            item_start_position = new Vector2(my_xPosition + bpbBG_xPos + 40, my_yPosition + bpbBG_yPos+10);
            Vector2 item_start_position2 = new Vector2(item_start_position.X, item_start_position.Y);
            item_icon_rects = new List<Rectangle>();
            pair_list = new List<rect_inv_item_pair>();
            for (int i = item_start_index; i < number_of_items_shown; i++)
            {
                item_icon_rects.Add(new Rectangle((int)item_start_position2.X, (int)item_start_position2.Y, 48, 48));
                item_start_position2.X += 60;
            }
            inv_left_scroll_rect = new Rectangle(my_xPosition + bpbBG_xPos + 10, my_yPosition + bpbBG_yPos+10, 16, 48);
            inv_right_scroll_rect = new Rectangle(my_xPosition+ bpbBG_width - 26, my_yPosition + bpbBG_yPos+10, 16, 48);

            my_dark_color = new Color(0, 0, 0);
            my_grey_color = new Color(100, 100, 100);
            my_red_color = new Color(255, 0, 0);
            my_text_color = new Color(255, 255, 255);
            my_d_grey_color = new Color(70, 70, 70);

            my_back_texture = my_default_backTex;
            my_back_texture.SetData(new[] { Color.White });
            pl_injury_report = new List<string>();

            the_icoBar = s_icoBar;
        }

        public void init_textures(Texture2D inj_scum, Texture2D inj_scuo, 
                                  Texture2D inj_scdm, Texture2D inj_scdo, 
                                  Texture2D inv_scleft, Texture2D inv_scright)
        {
            injSummary_scroll_up_max = inj_scum;
            injSummary_scroll_up_one = inj_scuo;
            injSummary_scroll_down_max = inj_scdm;
            injSummary_scroll_down_one = inj_scdo;
            inv_left_scroll_texture = inv_scleft;
            inv_right_scroll_texture = inv_scright;
        }

        public void init_wframes(Texture2D wFrameTex, Texture2D[] tex_masks)
        {
            character_wireframe = wFrameTex;
            texture_masks = tex_masks;
            //set the wframe position.
            wframe_xpos = ((BGElement_portraitBackground.X + BGElement_portraitBackground.Width + my_xPosition - wFrameTex.Width) / 2) + 5;
            wframe_ypos = (BGElement_portraitBackground.Y + BGElement_portraitBackground.Height) - (wFrameTex.Height + 10);
        }

        public void init_necessary_textures(Player pl)
        {
            cManager.Unload();

            List<Item> player_inv = pl.retrieve_inventory();

            for (int i = 0; i < player_inv.Count; i++)
            {
                if (player_inv[i] != null)
                {
                    string texture_loc = "Icons/";
                    if (player_inv[i] is Armor)
                        texture_loc += "Armors/";
                    if (player_inv[i] is Weapon)
                        texture_loc += "Weapons/";
                    if (player_inv[i] is Potion)
                        texture_loc += "Consumables/";
                    if (player_inv[i] is Scroll || player_inv[i] is Talisman)
                        texture_loc += "Scrolls/";

                    texture_loc += player_inv[i].get_my_texture_name();
                    Texture2D current_texture = cManager.Load<Texture2D>(texture_loc);
                    player_inv[i].set_texture(current_texture);
                    //Set the empty texture for the potion if it is one.
                    if (player_inv[i] is Potion)
                    {
                        Potion pt = (Potion)player_inv[i];
                        string empty_textureloc = "Icons/Consumables/" + pt.get_my_empty_texture_name();
                        pt.set_empty_texture(cManager.Load<Texture2D>(empty_textureloc));
                    }
                    
                    if (the_icoBar.item_is_on_bar(player_inv[i].get_my_IDno()))
                    {
                        the_icoBar.init_item_texture_by_id_number(player_inv[i].get_my_IDno(), current_texture);
                    }
                }
            }

            if (pl.show_main_hand() != null)
            {
                Texture2D current_texture = cManager.Load<Texture2D>("Icons/Weapons/" + pl.show_main_hand().get_my_texture_name());
                pl.show_main_hand().set_texture(current_texture);
                the_icoBar.init_item_texture_by_id_number(pl.show_main_hand().get_my_IDno(), current_texture);
            }
            if (pl.show_off_hand() != null)
            {
                Texture2D current_texture = cManager.Load<Texture2D>("Icons/Weapons/" + pl.show_off_hand().get_my_texture_name());
                pl.show_off_hand().set_texture(current_texture);
                the_icoBar.init_item_texture_by_id_number(pl.show_off_hand().get_my_IDno(), current_texture);
            }
            if (pl.show_over_armor() != null)
            {
                Texture2D current_texture = cManager.Load<Texture2D>("Icons/Armors/" + pl.show_over_armor().get_my_texture_name());
                pl.show_over_armor().set_texture(current_texture);
                the_icoBar.init_item_texture_by_id_number(pl.show_over_armor().get_my_IDno(), current_texture);
            }
            if (pl.show_under_armor() != null)
            {
                Texture2D current_texture = cManager.Load<Texture2D>("Icons/Armors/" + pl.show_under_armor().get_my_texture_name());
                pl.show_under_armor().set_texture(current_texture);
                the_icoBar.init_item_texture_by_id_number(pl.show_under_armor().get_my_IDno(), current_texture);
            }
            if (pl.show_helmet() != null)
            {
                Texture2D current_texture = cManager.Load<Texture2D>("Icons/Armors/" + pl.show_helmet().get_my_texture_name());
                pl.show_helmet().set_texture(current_texture);
                the_icoBar.init_item_texture_by_id_number(pl.show_helmet().get_my_IDno(), current_texture);
            }

            player_name = pl.my_chara_as_string();
            switch (player_name)
            {
                case "Falsael":
                    character_portrait = cManager.Load<Texture2D>("Player/Portraits/falsael_fine");
                    break;
                default:
                    character_portrait = null;
                    break;
            }
        }

        public void show()
        {
            visible = true;
        }

        public void hide()
        {
            visible = false;
        }

        public bool is_visible()
        {
            return visible;
        }

        public void switch_my_mode()
        {
            mode++;
            if (mode >= 2)
            {
                mode = 0;
            }
            switch (mode)
            {
                case 0:
                    hide();
                    break;
                case 1:
                    show();
                    break;
            }
        }

        //This actually updates a hell of a lot more than mouse info. It's the central function for
        //Clicking and dragging stuff into inventory slots / equip slots
        public void update_mouse_info(Vector2 mousePos, Player pl,
                                            bool holdingmouse, bool clickedmouse, out bool bad_turn)
        {
            mousePosition = mousePos;
            is_holding_mouse = holdingmouse;
            is_click_mouse = clickedmouse;

            //On the click, snap the rectangle to the mouse spot, and then pick an icon to draw in it.
            if (is_click_mouse)
            {
                draggable_item_rect.X = (int)mousePosition.X - 24;
                draggable_item_rect.Y = (int)mousePosition.Y - 24;
                for (int i = 0; i < pair_list.Count; i++)
                {
                    if (item_icon_rects[pair_list[i].item_rect].Contains((int)mousePosition.X, (int)mousePosition.Y))
                        index_of_mouse_selected_item = pair_list[i].corr_item;
                }

                if (main_hand_equip_slot.Contains((int)mousePosition.X, (int)mousePosition.Y))
                    index_of_c_equipped_selected_item = 1;
                if (off_hand_equip_slot.Contains((int)mousePosition.X, (int)mousePosition.Y))
                    index_of_c_equipped_selected_item = 2;
                if (over_armor_equip_slot.Contains((int)mousePosition.X, (int)mousePosition.Y))
                    index_of_c_equipped_selected_item = 3;
                if (under_armor_equip_slot.Contains((int)mousePosition.X, (int)mousePosition.Y))
                    index_of_c_equipped_selected_item = 4;
                if(helmet_equip_slot.Contains((int)mousePosition.X, (int)mousePosition.Y))
                    index_of_c_equipped_selected_item = 5;

                if(BGElement_equipmentTab.Contains((int)mousePosition.X, (int)mousePosition.Y))
                {
                    current_tab_selected = Current_Tab.Equipment;
                }

                if(BGElement_healthTab.Contains((int)mousePosition.X, (int)mousePosition.Y))
                {
                    current_tab_selected = Current_Tab.Injuries;
                }

                if (injSummary_scroll_up_max_rect.Contains((int)mousePosition.X, (int)mousePosition.Y))
                {
                    scroll_ctab_MSG(-1000);
                }

                if (injSummary_scroll_up_one_rect.Contains((int)mousePosition.X, (int)mousePosition.Y))
                {
                    scroll_ctab_MSG(-1);
                }

                if (injSummary_scroll_down_one_rect.Contains((int)mousePosition.X, (int)mousePosition.Y))
                {
                    scroll_ctab_MSG(1);
                }

                if (injSummary_scroll_down_max_rect.Contains((int)mousePosition.X, (int)mousePosition.Y))
                {
                    scroll_ctab_MSG(1000);
                }

                if(inv_left_scroll_rect.Contains((int)mousePosition.X, (int)mousePosition.Y))
                {
                    item_start_index--;
                    if(item_start_index < 0)
                        item_start_index = 0;
                }

                if(inv_right_scroll_rect.Contains((int)mousePosition.X, (int)mousePosition.Y))
                {
                    item_start_index++;
                    if(item_start_index + number_of_items_shown >= player_inv.Count)
                        item_start_index = Math.Max(0, player_inv.Count - number_of_items_shown);
                }
            }
            //On the hold, move the rectangle.
            if (is_holding_mouse)
            {
                draggable_item_rect.X = (int)mousePosition.X - 24;
                draggable_item_rect.Y = (int)mousePosition.Y - 24;
            }
            //On the release, try and equip the item. If it's a talisman and over another item
            //Try to add it to the item
            bool equipped_new_item = false;
            bool equipped_talisman = false;
            if (!is_holding_mouse && !is_click_mouse)
            {
                if (index_of_mouse_selected_item != -1)
                {
                    if (player_inv[index_of_mouse_selected_item] is Talisman)
                    {
                        Talisman T = (Talisman)player_inv[index_of_mouse_selected_item];
                        Item attach_to = null;
                        for (int i = 0; i < item_icon_rects.Count; i++)
                            if (check_overlap(item_icon_rects[i], draggable_item_rect) && i < player_inv.Count)
                                attach_to = player_inv[i];

                        if(check_overlap(main_hand_equip_slot, draggable_item_rect))
                            attach_to = pl.show_main_hand();
                        else if(check_overlap(off_hand_equip_slot, draggable_item_rect))
                            attach_to = pl.show_off_hand();
                        else if(check_overlap(helmet_equip_slot, draggable_item_rect))
                            attach_to = pl.show_helmet();
                        else if(check_overlap(over_armor_equip_slot, draggable_item_rect))
                            attach_to = pl.show_over_armor();
                        else if(check_overlap(under_armor_equip_slot, draggable_item_rect))
                            attach_to = pl.show_under_armor();

                        if(attach_to != null &&
                           (attach_to is Armor ||
                            attach_to is Scroll ||
                            attach_to is Weapon))
                        {
                            if (attach_to is Armor && T.armor_talisman() && attach_to.can_add_talisman(T))
                            {
                                attach_to.add_talisman(T);
                                equipped_talisman = true;
                            }
                            else if ((attach_to is Weapon || attach_to is Scroll) &&
                                    !T.armor_talisman() && attach_to.can_add_talisman(T))
                            {
                                attach_to.add_talisman(T);
                                equipped_talisman = true;
                            }
                            
                            if(equipped_talisman)
                                player_inv.RemoveAt(index_of_mouse_selected_item);
                        }
                    }

                    if (check_overlap(main_hand_equip_slot, draggable_item_rect) && !equipped_new_item && !equipped_talisman)
                        if (player_inv[index_of_mouse_selected_item] is Weapon)
                        {
                            pl.equip_main_hand((Weapon)player_inv[index_of_mouse_selected_item]);
                            equipped_new_item = true;
                        }

                    if (check_overlap(off_hand_equip_slot, draggable_item_rect) && !equipped_new_item && !equipped_talisman)
                        if (player_inv[index_of_mouse_selected_item] is Weapon)
                        {
                            pl.equip_off_hand((Weapon)player_inv[index_of_mouse_selected_item]);
                            equipped_new_item = true;
                        }

                    for (int i = 0; i < armor_equip_slots.Count; i++)
                        if (check_overlap(armor_equip_slots[i], draggable_item_rect) && !equipped_new_item && !equipped_talisman)
                            if(player_inv[index_of_mouse_selected_item] is Armor)
                            {
                                pl.equip_armor((Armor)player_inv[index_of_mouse_selected_item]);
                                equipped_new_item = true;
                            }

                    //spot to check if I should put it in the icobar
                    bool done = false;
                    for (int i = 0; i < the_icoBar.get_number_of_icons(); i++)
                    {
                        if (check_overlap(the_icoBar.get_ico_rects_by_slot(i), draggable_item_rect) && !done)
                        {
                            the_icoBar.assign_id_number_to_slot(player_inv[index_of_mouse_selected_item].get_my_IDno(), i);
                            the_icoBar.assign_icon_to_slot(player_inv[index_of_mouse_selected_item].get_my_texture(), i);
                            if (player_inv[index_of_mouse_selected_item] is Potion)
                                the_icoBar.assign_type_to_slot(IconBar.Type_Tracker.Potion, i);
                            else if (player_inv[index_of_mouse_selected_item] is Armor)
                                the_icoBar.assign_type_to_slot(IconBar.Type_Tracker.Armor, i);
                            else if (player_inv[index_of_mouse_selected_item] is Weapon)
                                the_icoBar.assign_type_to_slot(IconBar.Type_Tracker.Weapon, i);
                            else if (player_inv[index_of_mouse_selected_item] is Scroll)
                                the_icoBar.assign_type_to_slot(IconBar.Type_Tracker.Scroll, i);
                            the_icoBar.update_cooldown_and_quant(pl);
                            done = true;
                        }
                    }
                }

                if (index_of_c_equipped_selected_item != -1)
                {
                    //try to put it back in the inventory. It will go back in on any item rect slot
                    for (int i = 0; i < item_icon_rects.Count; i++)
                    {
                        if(check_overlap(item_icon_rects[i], draggable_item_rect))
                        {
                            //Do something depending on the slot.
                            switch (index_of_c_equipped_selected_item)
                            {
                                case 1:
                                    pl.unequip(Player.Equip_Slot.Mainhand);
                                    equipped_new_item = true;
                                    break;
                                case 2:
                                    pl.unequip(Player.Equip_Slot.Offhand);
                                    equipped_new_item = true;
                                    break;
                                case 3:
                                    pl.unequip(Player.Equip_Slot.Overarmor);
                                    equipped_new_item = true;
                                    break;
                                case 4:
                                    pl.unequip(Player.Equip_Slot.Underarmor);
                                    equipped_new_item = true;
                                    break;
                                case 5:
                                    pl.unequip(Player.Equip_Slot.Helmet);
                                    equipped_new_item = true;
                                    break;
                            }
                        }
                    }

                    bool done = false;
                    for (int i = 0; i < the_icoBar.get_number_of_icons(); i++)
                    {
                        if (check_overlap(the_icoBar.get_ico_rects_by_slot(i), draggable_item_rect) && !done)
                        {
                            switch (index_of_c_equipped_selected_item)
                            {
                                case 1:
                                    the_icoBar.assign_id_number_to_slot(pl.show_main_hand().get_my_IDno(), i);
                                    the_icoBar.assign_icon_to_slot(pl.show_main_hand().get_my_texture(), i);
                                    the_icoBar.assign_type_to_slot(IconBar.Type_Tracker.Weapon, i);
                                    done = true;
                                    break;
                                case 2:
                                    the_icoBar.assign_id_number_to_slot(pl.show_off_hand().get_my_IDno(), i);
                                    the_icoBar.assign_icon_to_slot(pl.show_off_hand().get_my_texture(), i);
                                    the_icoBar.assign_type_to_slot(IconBar.Type_Tracker.Weapon, i);
                                    done = true;
                                    break;
                                case 3:
                                    the_icoBar.assign_id_number_to_slot(pl.show_over_armor().get_my_IDno(), i);
                                    the_icoBar.assign_icon_to_slot(pl.show_over_armor().get_my_texture(), i);
                                    the_icoBar.assign_type_to_slot(IconBar.Type_Tracker.Armor, i);
                                    done = true;
                                    break;
                                case 4:
                                    the_icoBar.assign_id_number_to_slot(pl.show_under_armor().get_my_IDno(), i);
                                    the_icoBar.assign_icon_to_slot(pl.show_under_armor().get_my_texture(), i);
                                    the_icoBar.assign_type_to_slot(IconBar.Type_Tracker.Armor, i);
                                    done = true;
                                    break;
                                case 5:
                                    the_icoBar.assign_id_number_to_slot(pl.show_helmet().get_my_IDno(), i);
                                    the_icoBar.assign_icon_to_slot(pl.show_helmet().get_my_texture(), i);
                                    the_icoBar.assign_type_to_slot(IconBar.Type_Tracker.Armor, i);
                                    done = true;
                                    break;
                            }

                            the_icoBar.update_cooldown_and_quant(pl);
                        }
                    }
                }

                if(equipped_new_item || equipped_talisman)
                    update_player_info(ref pl);
                index_of_mouse_selected_item = -1;
                index_of_c_equipped_selected_item = -1;
            }

            bad_turn = equipped_new_item;
            if (String.Compare(pl.my_chara_as_string(), "Halephon") == 0)
                bad_turn = false;
        }

        public bool check_overlap(Rectangle rect_A, Rectangle rect_B)
        {
            return rect_A.Left < rect_B.Right && rect_A.Right > rect_B.Left &&
                rect_A.Top < rect_B.Bottom && rect_A.Bottom > rect_B.Top;
        }

        public void update_player_info(ref Player pl)
        {
            pl.clear_inv_nulls();
            player_inv = pl.retrieve_inventory();
            player_gold = pl.get_my_gold();
            init_necessary_textures(pl);
            pl.wound_report(out wounds_by_part, out max_health_by_part);
            inj_start_index = 0;
            pl_injury_report.Clear();
            pl_injury_report = pl.detailed_wound_report();
            pl_equipment_report = pl.detailed_equip_report();
        }

        public void scroll_ctab_MSG(int scrollvalue)
        {
            switch (current_tab_selected)
            {
                case Current_Tab.Injuries:
                    inj_start_index += scrollvalue;
                    if (inj_start_index > pl_injury_report.Count)
                    {
                        inj_start_index = pl_injury_report.Count;
                    }
                    else if (inj_start_index < 0)
                        inj_start_index = 0;
                    break;
                case Current_Tab.Equipment:
                    equip_msg_start_index += scrollvalue;
                    if (equip_msg_start_index > pl_equipment_report.Count)
                    {
                        equip_msg_start_index = pl_equipment_report.Count;
                    }
                    else if (equip_msg_start_index < 0)
                        equip_msg_start_index = 0;
                    break;
            }
        }

        #region drawing stuff

        public void draw_my_back(ref SpriteBatch sBatch)
        {
            sBatch.Draw(my_back_texture, my_size, my_dark_color);
        }

        public void draw_my_front(ref SpriteBatch sBatch)
        {
            sBatch.Draw(my_back_texture, BGElement_portraitBackground, my_grey_color);
            sBatch.Draw(my_back_texture, BGElement_backpackBackground, my_grey_color);
            sBatch.Draw(my_back_texture, BGElement_ctabBackground, my_grey_color);
            //Draw tabs.
            if (current_tab_selected == Current_Tab.Equipment)
            {
                sBatch.Draw(my_back_texture, BGElement_equipmentTab, my_grey_color);
                sBatch.Draw(my_back_texture, BGElement_healthTab, my_d_grey_color);
            }
            else if (current_tab_selected == Current_Tab.Injuries)
            {
                sBatch.Draw(my_back_texture, BGElement_equipmentTab, my_d_grey_color);
                sBatch.Draw(my_back_texture, BGElement_healthTab, my_grey_color);
            }            
        }

        public void draw_border_around_rectangle(ref SpriteBatch sBatch, Rectangle target_rect, int border_width, Color border_color)
        {
            sBatch.Draw(my_back_texture, new Rectangle(target_rect.Left, target_rect.Top, border_width, target_rect.Height), border_color);
            sBatch.Draw(my_back_texture, new Rectangle(target_rect.Right, target_rect.Top, border_width, target_rect.Height + border_width), border_color);
            sBatch.Draw(my_back_texture, new Rectangle(target_rect.Left, target_rect.Top, target_rect.Width, border_width), border_color);
            sBatch.Draw(my_back_texture, new Rectangle(target_rect.Left, target_rect.Bottom, target_rect.Width, border_width), border_color);
        }

        public void draw_my_borders(ref SpriteBatch sBatch)
        {
            int std_border_width = 4;

            draw_border_around_rectangle(ref sBatch, my_size, std_border_width, my_red_color);
            draw_border_around_rectangle(ref sBatch, BGElement_playerPortrait, 2, my_dark_color);

            for (int i = 0; i < item_icon_rects.Count; i++)
            {
                draw_border_around_rectangle(ref sBatch, item_icon_rects[i], std_border_width, my_text_color);
            }
        }

        public void draw_my_wireframe_outline(ref SpriteBatch sBatch)
        {
            sBatch.Draw(character_wireframe, BGElement_portraitBackground, Color.White);
        }

        public void draw_my_wireframe(ref SpriteBatch sBatch)
        {
            //Wireframe first.
            Vector2 w_frame_position = new Vector2(wframe_xpos, wframe_ypos);
            if(character_portrait != null)  
                sBatch.Draw(character_portrait, BGElement_playerPortrait, Color.White);

            //This suddenly became much more involved.
            for (int i = 0; i < 6; i++)
            {
                Color part_color = Color.Blue;
                if (wounds_by_part[i] >= max_health_by_part[i])
                    part_color = Color.Red;

                else if (max_health_by_part[i] == 3)
                {
                    if (wounds_by_part[i] == 1)
                        part_color = new Color(0, 255, 0);
                    else if (wounds_by_part[i] == 2)
                        part_color = Color.Yellow;
                }

                sBatch.Draw(texture_masks[i], BGElement_portraitBackground, part_color);
            }
        }

        public void draw_my_text(ref SpriteBatch sBatch)
        {
            sBatch.DrawString(stdFont, equips_title, equip_title_pos, my_text_color);
            sBatch.DrawString(stdFont, inj_title, inj_title_pos, my_text_color);
            if (current_tab_selected == Current_Tab.Injuries)
            {
                Vector2 injury_msg_pos2 = new Vector2(ctab_msg_pos.X, ctab_msg_pos.Y);
                //Then text
                //Injury Text!
                for (int i = inj_start_index; i < Math.Min(inj_start_index + total_messages_shown, pl_injury_report.Count); i++)
                {
                    sBatch.DrawString(stdFont, pl_injury_report[i], injury_msg_pos2, my_text_color);
                    injury_msg_pos2.Y += stdFont.LineSpacing;
                }
            }

            if (current_tab_selected == Current_Tab.Equipment)
            {
                Vector2 equip_msg_pos2 = new Vector2(ctab_msg_pos.X, ctab_msg_pos.Y);
                //Then text
                //Injury Text!
                for (int i = equip_msg_start_index; i < Math.Min(equip_msg_start_index + total_messages_shown, pl_equipment_report.Count); i++)
                {
                    sBatch.DrawString(stdFont, pl_equipment_report[i], equip_msg_pos2, my_text_color);
                    equip_msg_pos2.Y += stdFont.LineSpacing;
                }
            }

            //Then equipment text! + scrolling arrows.
            sBatch.DrawString(player_name_font, player_name, player_name_pos, my_text_color);
            sBatch.DrawString(stdFont, "Gold: " + player_gold, goldmsg_pos, my_text_color);

            sBatch.Draw(injSummary_scroll_up_max, injSummary_scroll_up_max_rect, Color.White);
            sBatch.Draw(injSummary_scroll_up_one, injSummary_scroll_up_one_rect, Color.White);
            sBatch.Draw(injSummary_scroll_down_max, injSummary_scroll_down_max_rect, Color.White);
            sBatch.Draw(injSummary_scroll_down_one, injSummary_scroll_down_one_rect, Color.White);
            sBatch.Draw(inv_right_scroll_texture, inv_right_scroll_rect, Color.White);
            sBatch.Draw(inv_left_scroll_texture, inv_left_scroll_rect, Color.White);
        }

        public void draw_tooltip_box(ref SpriteBatch sBatch, Item tooltip_target)
        {
            List<string> tooltip = tooltip_target.get_my_information(false);
            //We have that. Now we decide how big the tooltip box is gonna be.
            int tooltip_box_width = 0;
            int tooltip_box_height = 0;
            for (int i = 0; i < tooltip.Count; i++)
            {
                Vector2 size = stdFont.MeasureString(tooltip[i]);
                if (size.X > tooltip_box_width)
                    tooltip_box_width = (int)size.X;
                tooltip_box_height += stdFont.LineSpacing + 5;
            }
            //NOW THAT WE KNOW HOW BIG IT IS, WE CAN MAKE A RECTANGLE YEAH.
            Rectangle tooltip_box_rect = new Rectangle((int)mousePosition.X, (int)mousePosition.Y-tooltip_box_height, 
                                                        tooltip_box_width+10, tooltip_box_height+10);
            //Now we draw the rectangle, then draw the text in it I guess? I hope this works right =/
            sBatch.Begin(SpriteSortMode.BackToFront, null);
            sBatch.Draw(my_back_texture, tooltip_box_rect, my_dark_color);
            sBatch.End();

            //Then we draw the white borders b/c why not
            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_border_around_rectangle(ref sBatch, tooltip_box_rect, 2, my_text_color);
            sBatch.End();

            //Then the text woo.
            Vector2 ttip_position2 = new Vector2(tooltip_box_rect.X+5, tooltip_box_rect.Y+5);
            sBatch.Begin(SpriteSortMode.BackToFront, null);
            for (int i = 0; i < tooltip.Count; i++)
            {
                sBatch.DrawString(stdFont, tooltip[i], ttip_position2, my_text_color);
                ttip_position2.Y += stdFont.LineSpacing + 5;
            }
            sBatch.End();
        }

        public void draw_equipment_slot_rects(ref SpriteBatch sBatch)
        {
            sBatch.Draw(my_back_texture, main_hand_equip_slot, new Color(0,0,0,100));
            sBatch.Draw(my_back_texture, off_hand_equip_slot, new Color(0, 0, 0, 100));
            sBatch.Draw(my_back_texture, over_armor_equip_slot, new Color(0, 0, 0, 100));
            sBatch.Draw(my_back_texture, under_armor_equip_slot, new Color(0, 0, 0, 100));
            sBatch.Draw(my_back_texture, helmet_equip_slot, new Color(0, 0, 0, 100));
        }

        public void draw_equipment_slot_borders(ref SpriteBatch sBatch)
        {
            int std_border_width = 4;

            draw_border_around_rectangle(ref sBatch, main_hand_equip_slot, std_border_width, my_text_color);
            draw_border_around_rectangle(ref sBatch, off_hand_equip_slot, std_border_width, my_text_color);
            draw_border_around_rectangle(ref sBatch, over_armor_equip_slot, std_border_width, my_text_color);
            draw_border_around_rectangle(ref sBatch, under_armor_equip_slot, std_border_width, my_text_color);
            draw_border_around_rectangle(ref sBatch, helmet_equip_slot, std_border_width, my_text_color);
        }

        public void draw_item_icons(ref SpriteBatch sBatch, Player pl)
        {
            //Then items - the backpack stuff.
            pair_list.Clear();
            int rect_index = 0;
            for (int i = item_start_index; i < Math.Min(player_inv.Count, item_start_index + number_of_items_shown); i++)
            {
                if (player_inv[i] != null && i != index_of_mouse_selected_item)
                {
                    player_inv[i].draw_me(item_icon_rects[rect_index], ref sBatch);
                    //sBatch.Draw(player_inv[i].get_my_texture(), item_icon_rects[rect_index], Color.White);
                    pair_list.Add(new rect_inv_item_pair(rect_index, i));
                    rect_index++;
                }
                if (player_inv[i] != null && i == index_of_mouse_selected_item)
                {
                    target_draggable_index = i;
                    //sBatch.Draw(player_inv[i].get_my_texture(), draggable_item_rect, Color.White);
                    pair_list.Add(new rect_inv_item_pair(rect_index, i));
                    rect_index++;
                }
            }

            if (pl.show_main_hand() != null)
            {
                Rectangle rect;
                if (index_of_c_equipped_selected_item == 1)
                    rect = draggable_item_rect;
                else
                    rect = main_hand_equip_slot;
                sBatch.Draw(pl.show_main_hand().get_my_texture(), rect, Color.White);
            }
            if (pl.show_off_hand() != null)
            {
                Rectangle rect;
                if (index_of_c_equipped_selected_item == 2)
                    rect = draggable_item_rect;
                else
                    rect = off_hand_equip_slot;
                sBatch.Draw(pl.show_off_hand().get_my_texture(), rect, Color.White);
            }
            if (pl.show_under_armor() != null)
            {
                Rectangle rect;
                if (index_of_c_equipped_selected_item == 4)
                    rect = draggable_item_rect;
                else
                    rect = under_armor_equip_slot;
                sBatch.Draw(pl.show_under_armor().get_my_texture(), rect, Color.White);
            }
            if (pl.show_over_armor() != null)
            {
                Rectangle rect;
                if (index_of_c_equipped_selected_item == 3)
                    rect = draggable_item_rect;
                else
                    rect = over_armor_equip_slot;
                sBatch.Draw(pl.show_over_armor().get_my_texture(), rect, Color.White);
            }
            if (pl.show_helmet() != null)
            {
                Rectangle rect;
                if (index_of_c_equipped_selected_item == 5)
                    rect = draggable_item_rect;
                else
                    rect = helmet_equip_slot;
                sBatch.Draw(pl.show_helmet().get_my_texture(), rect, Color.White);
            }
        }

        public void draw_me(ref SpriteBatch sBatch, Player pl)
        {
            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_my_back(ref sBatch);
            sBatch.End();

            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_my_front(ref sBatch);
            sBatch.End();

            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_my_borders(ref sBatch);
            sBatch.End();

            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_my_wireframe(ref sBatch);
            sBatch.End();

            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_my_wireframe_outline(ref sBatch);
            sBatch.End();

            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_my_text(ref sBatch);
            sBatch.End();

            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_equipment_slot_rects(ref sBatch);
            sBatch.End();

            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_equipment_slot_borders(ref sBatch);
            sBatch.End();

            target_draggable_index = -1;
            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_item_icons(ref sBatch, pl);
            sBatch.End();

            if (target_draggable_index > -1)
            {
                sBatch.Begin(SpriteSortMode.BackToFront, null);
                player_inv[target_draggable_index].draw_me(draggable_item_rect, ref sBatch);
                sBatch.End();
            }

            for (int i = 0; i < pair_list.Count; i++)
                if (item_icon_rects[i].Contains((int)mousePosition.X, (int)mousePosition.Y) &&
                    !is_click_mouse && !is_holding_mouse)
                    draw_tooltip_box(ref sBatch, player_inv[pair_list[i].corr_item]);
        }        

        #endregion
    }
}
