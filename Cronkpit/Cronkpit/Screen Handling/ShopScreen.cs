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
    class ShopScreen
    {
        List<string> menuItems;
        string sub_menu_exit = "Back";

        ContentManager cManager;

        int selectedIndex;
        int selected_item_index;

        Color normal = Color.White;
        Color highlighted = Color.Red;

        SpriteFont sFont;
        SpriteFont smaller_Font;

        Random rGen;

        Rectangle info_scrollup_ctrl;
        Rectangle info_scrolldwn_ctrl;
        Texture2D info_scrollup_tex;
        Texture2D info_scrolldwn_tex;
        Texture2D blank_texture;

        //Descriptive stuff
        int blurb_height;
        Vector2 menu_position;
        Vector2 blurb_position;
        List<string> intro_prompt;
        List<string> weapon_shopping_prompt;
        List<string> armor_shopping_prompt;
        List<string> consumable_shopping_prompt;
        List<string> talisman_shopping_prompt;
        List<string> sell_shopping_prompt;
        List<string> buyback_shopping_prompt;
        List<string> scroll_shopping_prompt;
        List<string> current_prompt;
        string shopkeeper;

        Vector2 item_menu_position;
        Vector2 item_info_position;

        //Position stuff
        Rectangle client;

        float height = 0;
        float width = 0;

        int item_menu_offset;
        int item_info_offset;
        int max_info_offset;
        int lines_available;
        int small_lines_avilable;

        //Shopping stuff!
        public enum Shopping_Mode { Main, Armor, Weapons, Consumables, Talismans, Scrolls, Sell, Buyback };
        Shopping_Mode im_shopping_for;

        //Item stuff
        ShopXManager shopManager;
        List<Item> armors_in_stock;
        List<Item> weapons_in_stock;
        List<Item> scrolls_in_stock;
        List<Item> consumables_in_stock;
        List<Item> talismans_in_stock;
        List<Item> items_to_sell;
        List<Item> sold_items;
        List<Item> current_list;
        List<string> current_item_info;
        int player_gold;

        //Item comparison stuff
        ComparisonPopup comparatorPopup;

        //Misc.
        int adtl_spacing = 25;

        public ShopScreen(List<string> mItems, SpriteFont sf, SpriteFont small_f, Rectangle cl, ref ContentManager cm,
                          Texture2D blnkTex)
        {
            menuItems = new List<string>(mItems);
            shopManager = new ShopXManager(cm);
            current_item_info = new List<string>();
            cManager = cm;
            sFont = sf;
            smaller_Font = small_f;
            client = cl;
            selectedIndex = 0;
            im_shopping_for = Shopping_Mode.Main;
            rGen = new Random();
            //Choose a random blurb, based on what character you picked.
            blurb_position = new Vector2(20f, 20f);
            comparatorPopup = null;
            blank_texture = blnkTex;
        }

        public void init_controls(Texture2D scrollup_tex, Texture2D scrolldown_tex)
        {
            info_scrollup_tex = scrollup_tex;
            info_scrolldwn_tex = scrolldown_tex;
            info_scrollup_ctrl = new Rectangle(client.Width - 32, 0, 32, 32);
            info_scrolldwn_ctrl = new Rectangle(client.Width - 32, client.Height - 50, 32, 32);
        }

        private void take_measurements()
        {
            height = 0;
            width = 0;

            for (int i = 0; i < menuItems.Count; i++)
            {
                Vector2 size = sFont.MeasureString(menuItems[i]);
                if (size.X > width)
                    width = size.X;
                height += sFont.LineSpacing + 5;
            }

            menu_position = new Vector2((client.Width - width) / 2, 
                                        blurb_height + ((client.Height - height - blurb_height) / 2));
        }

        public void scroll_menu(int scroll)
        {
            current_item_info.Clear();
            item_info_offset = 0;
            switch(im_shopping_for)
            {
                case Shopping_Mode.Main:
                    selected_item_index = 0;
                    selectedIndex += scroll;
                    if (selectedIndex < 0)
                        selectedIndex = menuItems.Count - 1;
                    if (selectedIndex >= menuItems.Count)
                        selectedIndex = 0;
                    break;
                default:
                    selectedIndex = 0;
                    selected_item_index += scroll;
                    if (selected_item_index > (item_menu_offset + lines_available - 1))
                        item_menu_offset++;
                    else if (selected_item_index < item_menu_offset && item_menu_offset > 0)
                        item_menu_offset--;

                    if (selected_item_index < 0)
                    {
                        selected_item_index = current_list.Count;
                        item_menu_offset = current_list.Count+1 - lines_available;
                        if (item_menu_offset < 0)
                            item_menu_offset = 0;
                    }
                    if (selected_item_index > current_list.Count)
                    {
                        selected_item_index = 0;
                        item_menu_offset = 0;
                    }
                    if (selected_item_index < current_list.Count)
                    {
                        current_item_info = current_list[selected_item_index].get_my_information(true);
                        max_info_offset = Math.Max(current_item_info.Count- lines_available, 0);
                    }
                    break;
            }
        }

        public void scroll_iteminfo_menu(int scroll)
        {
            if((item_info_offset < max_info_offset && item_info_offset > 0) ||
               (item_info_offset == max_info_offset && scroll == -1 && max_info_offset > 0) ||
               (item_info_offset == 0 && scroll == 1 && max_info_offset > 0))
                item_info_offset += scroll;
        }

        public void mouse_click(Vector2 clickloc)
        {
            if (info_scrollup_ctrl.Contains((int)clickloc.X, (int)clickloc.Y))
                scroll_iteminfo_menu(-1);

            if (info_scrolldwn_ctrl.Contains((int)clickloc.X, (int)clickloc.Y))
                scroll_iteminfo_menu(1);
        }

        public int get_my_index()
        {
            return selectedIndex;
        }

        public void switch_shopping_mode(Shopping_Mode next_mode)
        {
            selected_item_index = 0;
            im_shopping_for = next_mode;
            string section = "";
            switch (im_shopping_for)
            {
                case Shopping_Mode.Main:
                    section = "Intro";
                    break;
                case Shopping_Mode.Armor:
                    section = "Armor";
                    current_list = armors_in_stock;
                    scroll_menu(0);
                    break;
                case Shopping_Mode.Weapons:
                    section = "Weapons";
                    current_list = weapons_in_stock;
                    scroll_menu(0);
                    break;
                case Shopping_Mode.Consumables:
                    section = "Consumables";
                    current_list = consumables_in_stock;
                    scroll_menu(0);
                    break;
                case Shopping_Mode.Sell:
                    section = "Sell";
                    current_list = items_to_sell;
                    scroll_menu(0);
                    break;
                case Shopping_Mode.Buyback:
                    section = "Buyback";
                    current_list = sold_items;
                    scroll_menu(0);
                    break;
                case Shopping_Mode.Scrolls:
                    section = "Scrolls";
                    current_list = scrolls_in_stock;
                    scroll_menu(0);
                    break;
                case Shopping_Mode.Talismans:
                    section = "Talismans";
                    current_list = talismans_in_stock;
                    scroll_menu(0);
                    break;
            }
            set_prompt_by_section(section);
            item_menu_position = new Vector2(75, Math.Max(250, blurb_height + adtl_spacing));
            lines_available = (client.Height - (Math.Max(250, blurb_height + adtl_spacing) + 50)) / sFont.LineSpacing;
            small_lines_avilable = (client.Height - (Math.Max(250, blurb_height + adtl_spacing) + 50)) / smaller_Font.LineSpacing;
            info_scrollup_ctrl.Y = (int)item_menu_position.Y;
        }

        public bool in_submenu()
        {
            return im_shopping_for != Shopping_Mode.Main;
        }

        public bool in_comparable_submenu()
        {
            return im_shopping_for == Shopping_Mode.Weapons || im_shopping_for == Shopping_Mode.Armor;
        }

        public bool exit_submenu()
        {
            switch (im_shopping_for)
            {
                case Shopping_Mode.Armor:
                    return selected_item_index == armors_in_stock.Count;
                case Shopping_Mode.Weapons:
                    return selected_item_index == weapons_in_stock.Count;
                case Shopping_Mode.Consumables:
                    return selected_item_index == consumables_in_stock.Count;
                case Shopping_Mode.Sell:
                    return selected_item_index == items_to_sell.Count;
                case Shopping_Mode.Buyback:
                    return selected_item_index == sold_items.Count;
                case Shopping_Mode.Scrolls:
                    return selected_item_index == scrolls_in_stock.Count;
                case Shopping_Mode.Talismans:
                    return selected_item_index == talismans_in_stock.Count;
                default:
                    return false;
            }
        }

        public void buy_item(Player pl)
        {
            int item_gold_value;
            switch (im_shopping_for)
            {
                case Shopping_Mode.Sell:
                    sell_item(pl);
                    break;
                case Shopping_Mode.Scrolls:
                    item_gold_value = scrolls_in_stock[selected_item_index].get_my_gold_value();
                    if (pl.get_my_gold() >= item_gold_value)
                    {
                        pl.pay_gold(item_gold_value);
                        pl.acquire_item(scrolls_in_stock[selected_item_index]);
                        scrolls_in_stock.RemoveAt(selected_item_index);
                    }
                    break;
                default:
                    item_gold_value = current_list[selected_item_index].get_my_gold_value();
                    int buyback_value = (int)Math.Max(item_gold_value * .93, item_gold_value - 500);

                    if (im_shopping_for == Shopping_Mode.Buyback)
                        item_gold_value = buyback_value;

                    if (pl.get_my_gold() >= item_gold_value)
                    {
                        pl.pay_gold(item_gold_value);
                        if (current_list[selected_item_index] is Potion)
                        {
                            Potion p = (Potion)current_list[selected_item_index];
                            p.adjust_quantity(-1);
                            Potion p2 = new Potion(p.get_my_IDno(), p.get_my_gold_value(), p.get_my_name(), p);
                            p2.set_quantity(1);
                            pl.acquire_potion(p2);

                            if (p.get_my_quantity() == 0)
                                current_list.RemoveAt(selected_item_index);
                        }
                        else
                        {
                            pl.acquire_item(current_list[selected_item_index]);
                            current_list.RemoveAt(selected_item_index);
                        }
                    }
                    break;
            }
            player_gold = pl.get_my_gold();
            scroll_menu(0);
        }

        public void sell_item(Player pl)
        {
            int item_gold_value = items_to_sell[selected_item_index].get_my_gold_value();
            int sale_value = (int)Math.Max(item_gold_value * .93, item_gold_value - 500);
            if (items_to_sell[selected_item_index] is Potion)
            {
                Potion p = (Potion)items_to_sell[selected_item_index];
                Potion p2 = new Potion(p.get_my_IDno(), p.get_my_gold_value(), p.get_my_name(), p);
                p2.set_quantity(1);
                p.adjust_quantity(-1);
                if (p.get_my_quantity() == 0)
                    items_to_sell.RemoveAt(selected_item_index);

                bool stacked = false;
                for (int i = 0; i < sold_items.Count; i++)
                {
                    if (sold_items[i] is Potion)
                    {
                        Potion p3 = (Potion)sold_items[i];
                        if (p3.get_my_IDno() == p2.get_my_IDno())
                        {
                            stacked = true;
                            p3.adjust_quantity(1);
                        }
                    }
                }
                
                if(!stacked)
                    sold_items.Add(p2);
            }
            else
            {
                sold_items.Add(items_to_sell[selected_item_index]);
                items_to_sell.RemoveAt(selected_item_index);
            }
            pl.add_gold(sale_value);
        }

        public void set_variables(Player pl)
        {
            player_gold = pl.get_my_gold();
            //set all prompts
            set_shopkeep();
            intro_prompt = slot_keywords(shopManager.get_prompt(pl.my_chara(), "Intro"));
            weapon_shopping_prompt = slot_keywords(shopManager.get_prompt(pl.my_chara(), "Weapons"));
            armor_shopping_prompt = slot_keywords(shopManager.get_prompt(pl.my_chara(), "Armor"));
            consumable_shopping_prompt = slot_keywords(shopManager.get_prompt(pl.my_chara(), "Consumables"));
            talisman_shopping_prompt = slot_keywords(shopManager.get_prompt(pl.my_chara(), "Talismans"));
            sell_shopping_prompt = slot_keywords(shopManager.get_prompt(pl.my_chara(), "Sell"));
            buyback_shopping_prompt = slot_keywords(shopManager.get_prompt(pl.my_chara(), "Buyback"));
            scroll_shopping_prompt = slot_keywords(shopManager.get_prompt(pl.my_chara(), "Scrolls"));

            armors_in_stock = shopManager.retrieve_random_items(ShopXManager.Permanent_ITypes.Armor, pl, 4);
            weapons_in_stock = shopManager.retrieve_random_items(ShopXManager.Permanent_ITypes.Weapon, pl, 4);
            scrolls_in_stock = shopManager.retrieve_random_items(ShopXManager.Permanent_ITypes.Scroll, pl, 4);
            consumables_in_stock = shopManager.retrieve_random_consumables(pl, 2);
            talismans_in_stock = shopManager.retrieve_random_talismans(4);
            items_to_sell = pl.retrieve_inventory();
            sold_items = new List<Item>();

            init_necessary_textures(pl);
        }

        public void init_necessary_textures(Player pl)
        {
            cManager.Unload();
            
            for (int i = 0; i < armors_in_stock.Count; i++)
            {
                string texturename = armors_in_stock[i].get_my_texture_name();
                armors_in_stock[i].set_texture(cManager.Load<Texture2D>("Icons/Armors/" + texturename));
            }

            for (int i = 0; i < weapons_in_stock.Count; i++)
            {
                string texturename = weapons_in_stock[i].get_my_texture_name();
                weapons_in_stock[i].set_texture(cManager.Load<Texture2D>("Icons/Weapons/" + texturename));
            }

            for (int i = 0; i < scrolls_in_stock.Count; i++)
            {
                string texturename = scrolls_in_stock[i].get_my_texture_name();
                scrolls_in_stock[i].set_texture(cManager.Load<Texture2D>("Icons/Scrolls/" + texturename));
            }

            for (int i = 0; i < consumables_in_stock.Count; i++)
            {
                string texturename = consumables_in_stock[i].get_my_texture_name();
                consumables_in_stock[i].set_texture(cManager.Load<Texture2D>("Icons/Consumables/" + texturename));
                if (consumables_in_stock[i] is Potion)
                {
                    Potion p = (Potion)consumables_in_stock[i];
                    string empty_texturename = p.get_my_empty_texture_name();
                    p.set_empty_texture(cManager.Load<Texture2D>("Icons/Consumables/" + empty_texturename));
                }
            }

            /* This isn't needed for the time being, and who knows! It may never be.
            List<Item> player_inventory = pl.retrieve_inventory();

            for (int i = 0; i < player_inventory.Count; i++)
            {
                //do nothing for now.
            }
             */
        }

        public void set_prompt_by_section(string section)
        {
            switch (section)
            {
                case "Intro":
                    current_prompt = intro_prompt;
                    break;
                case "Weapons":
                    current_prompt = weapon_shopping_prompt;
                    break;
                case "Armor":
                    current_prompt = armor_shopping_prompt;
                    break;
                case "Consumables":
                    current_prompt = consumable_shopping_prompt;
                    break;
                case "Talismans":
                    current_prompt = talisman_shopping_prompt;
                    break;
                case "Scrolls":
                    current_prompt = scroll_shopping_prompt;
                    break;
                case "Sell":
                    current_prompt = sell_shopping_prompt;
                    break;
                case "Buyback":
                    current_prompt = buyback_shopping_prompt;
                    break;
            }
            blurb_height = (current_prompt.Count + 3) * sFont.LineSpacing;
        }

        public void set_shopkeep()
        {
            int skeeper = rGen.Next(2);
            switch (skeeper)
            {
                case 0:
                    shopkeeper = "ancient goblin";
                    break;
                case 1:
                    shopkeeper = "hooded man";
                    break;
            }
        }

        public List<string> slot_keywords(List<string> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                string nextString = list[i].Replace("[SHOPKEEPER]", shopkeeper);
                list[i] = nextString;
            }

            return list;
        }

        public void run_comparison(Player pl)
        {
            if (comparatorPopup == null)
            {
                if (im_shopping_for == Shopping_Mode.Armor)
                {
                    Armor p_armor = null;
                    Armor c_armor = (Armor)current_list[selected_item_index];
                    if (c_armor.what_armor_type() == Armor.Armor_Type.Helmet)
                        p_armor = pl.show_helmet();
                    else if (c_armor.what_armor_type() == Armor.Armor_Type.OverArmor)
                        p_armor = pl.show_over_armor();
                    else
                        p_armor = pl.show_under_armor();
                    comparatorPopup = new ComparisonPopup(blank_texture, sFont, smaller_Font, cManager, p_armor, c_armor);
                }
                else
                    comparatorPopup = new ComparisonPopup(blank_texture, sFont, smaller_Font, cManager, pl.show_main_hand(),
                                                          (Weapon)current_list[selected_item_index],
                                                          pl.show_off_hand());
            }
        }

        public void clear_comparison()
        {
            comparatorPopup = null;
        }

        public void draw_text(ref SpriteBatch sBatch)
        {
            take_measurements();

            Vector2 blurb_position2 = new Vector2(blurb_position.X, blurb_position.Y);
            for (int i = 0; i < current_prompt.Count; i++)
            {
                sBatch.DrawString(sFont, current_prompt[i], blurb_position2, Color.White);
                blurb_position2.Y += sFont.LineSpacing;
            }
            Vector2 position2;
            Color tint;
            Vector2 player_gold_position = new Vector2(10, client.Height - sFont.LineSpacing - 10);

            sBatch.DrawString(sFont, "Gold Remaining: " + player_gold.ToString(), player_gold_position, Color.White);
            switch (im_shopping_for)
            {
                case Shopping_Mode.Main:
                    position2 = new Vector2(menu_position.X, menu_position.Y);
                    for (int i = 0; i < menuItems.Count; i++)
                    {
                        if (i == selectedIndex)
                            tint = highlighted;
                        else
                            tint = normal;
                        sBatch.DrawString(sFont, menuItems[i], position2, tint);
                        position2.Y += sFont.LineSpacing;
                    }
                    break;
                default:
                    draw_item_type_menu(current_list, ref sBatch);
                    break;
            }

            if (im_shopping_for != Shopping_Mode.Main)
            {
                SpriteFont target_font = sFont;
                int current_lines_avail = lines_available;
                if (current_item_info.Count > 10)
                {
                    target_font = smaller_Font;
                    current_lines_avail = small_lines_avilable;
                }

                Vector2 item_info_position2 = new Vector2(item_info_position.X, item_info_position.Y);

                for (int i = item_info_offset;
                     i < Math.Min(current_item_info.Count, item_info_offset + current_lines_avail); i++)
                {
                    sBatch.DrawString(target_font, current_item_info[i], item_info_position2, Color.White);
                    item_info_position2.Y += target_font.LineSpacing;
                }

                sBatch.Draw(info_scrollup_tex, info_scrollup_ctrl, Color.White);
                sBatch.Draw(info_scrolldwn_tex, info_scrolldwn_ctrl, Color.White);
            }
        }

        public void draw_comparator(ref SpriteBatch sBatch)
        {
            comparatorPopup.drawMe(ref sBatch);
        }

        public void drawMe(ref SpriteBatch sBatch)
        {
            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_text(ref sBatch);
            sBatch.End();

            if (comparatorPopup != null)
                draw_comparator(ref sBatch);
        }

        public void draw_item_type_menu(List<Item> target_menu, ref SpriteBatch sBatch)
        {
            item_info_position = new Vector2(425, Math.Max(250, blurb_height + adtl_spacing));
            Vector2 position2 = new Vector2(item_menu_position.X, item_menu_position.Y);
            Color tint;
            Color exit_submenu_tint;

            for (int i = item_menu_offset; 
                 i < Math.Min(target_menu.Count, item_menu_offset+lines_available); i++)
            {
                if (i == selected_item_index)
                    tint = highlighted;
                else
                    tint = normal;
                sBatch.DrawString(sFont, target_menu[i].get_my_name(), position2, tint);
                position2.Y += sFont.LineSpacing;
            }

            if (item_menu_offset + lines_available >= target_menu.Count+1)
            {
                if (selected_item_index == target_menu.Count)
                    exit_submenu_tint = highlighted;
                else
                    exit_submenu_tint = normal;
                sBatch.DrawString(sFont, sub_menu_exit, position2, exit_submenu_tint);
            }
        }
    }
}
