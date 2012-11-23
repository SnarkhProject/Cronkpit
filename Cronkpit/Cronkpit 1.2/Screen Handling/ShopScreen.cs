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
        SpriteFont tFont;

        Random rGen;

        //Descriptive stuff
        string descriptive_text;
        int blurb_height;
        Vector2 menu_position;
        Vector2 blurb_position;
        int intro_used;
        int weapon_shopping_used;
        int armor_shopping_used;
        int consumable_shopping_used;
        int talisman_shopping_used;
        int exit_used;
        string shopkeeper;

        Vector2 item_menu_position;
        Vector2 item_info_position;

        //Position stuff
        Rectangle client;

        float height = 0;
        float width = 0;
        float i_menu_height = 0;
        float i_menu_width = 0;
        float inf_menu_height = 0;
        float inf_menu_width = 0;

        //Shopping stuff!
        public enum Shopping_Mode { Main, Armor, Weapons, Consumables, Talismans };
        Shopping_Mode im_shopping_for;

        //Item stuff
        MainItemList all_items;
        List<Armor> armors_in_stock;
        List<Weapon> weapons_in_stock;
        List<string> current_item_info;

        public ShopScreen(List<string> mItems, SpriteFont sf, SpriteFont tf, Rectangle cl, ref ContentManager cm)
        {
            menuItems = new List<string>(mItems);
            all_items = new MainItemList();
            current_item_info = new List<string>();
            cManager = cm;
            sFont = sf;
            tFont = tf;
            client = cl;
            selectedIndex = 0;
            im_shopping_for = Shopping_Mode.Main;
            rGen = new Random();
            //Choose a random blurb, based on what character you picked.
            blurb_position = new Vector2(20f, 20f);
            descriptive_text = "";
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

        private void take_item_menu_measurements(Shopping_Mode target_menu)
        {
            i_menu_height = 0;
            i_menu_width = 0;
            inf_menu_height = 0;
            inf_menu_width = 0;

            if (target_menu == Shopping_Mode.Armor)
            {
                for (int i = 0; i < armors_in_stock.Count; i++)
                {
                    Vector2 size = sFont.MeasureString(armors_in_stock[i].get_my_name());
                    if (size.X > i_menu_width)
                        i_menu_width = size.X;
                    i_menu_height += sFont.LineSpacing + 5;
                }
            }

            if (target_menu == Shopping_Mode.Weapons)
            {
                for (int i = 0; i < weapons_in_stock.Count; i++)
                {
                    Vector2 size = sFont.MeasureString(weapons_in_stock[i].get_my_name());
                    if (size.X > i_menu_width)
                        i_menu_width = size.X;
                    i_menu_height += sFont.LineSpacing + 5;
                }
            }

            for (int i = 0; i < current_item_info.Count; i++)
            {
                Vector2 size = sFont.MeasureString(current_item_info[i]);
                if (size.X > i_menu_width)
                    inf_menu_width = size.X;
                inf_menu_height += sFont.LineSpacing + 5;
            }

            int client_5_zone_split = client.Width / 5;
            item_menu_position = new Vector2(((client.Width - (client_5_zone_split * 3)) - width) / 2,
                                            blurb_height + ((client.Height - height - blurb_height) / 2));
            item_info_position = new Vector2(((client.Width - inf_menu_width) / 2) + item_menu_position.X,
                                            blurb_height + ((client.Height - inf_menu_height - blurb_height) / 2));
        }

        public void scroll_menu(int scroll)
        {
            current_item_info.Clear();
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
                case Shopping_Mode.Armor:
                    selectedIndex = 0;
                    selected_item_index += scroll;
                    if (selected_item_index < 0)
                        selected_item_index = armors_in_stock.Count;
                    if (selected_item_index > armors_in_stock.Count)
                        selected_item_index = 0;
                    if(selected_item_index < armors_in_stock.Count)
                        current_item_info = armors_in_stock[selected_item_index].get_my_information();
                    break;
                case Shopping_Mode.Weapons:
                    selectedIndex = 0;
                    selected_item_index += scroll;
                    if (selected_item_index < 0)
                        selected_item_index = weapons_in_stock.Count;
                    if (selected_item_index > weapons_in_stock.Count)
                        selected_item_index = 0;
                    if (selected_item_index < weapons_in_stock.Count)
                        current_item_info = weapons_in_stock[selected_item_index].get_my_information();
                    break;
            }
        }

        public int get_my_index()
        {
            return selectedIndex;
        }

        public void switch_shopping_mode(Shopping_Mode next_mode)
        {
            im_shopping_for = next_mode;
            switch (im_shopping_for)
            {
                case Shopping_Mode.Main:
                    set_descriptive_intro(intro_used);
                    break;
                case Shopping_Mode.Armor:
                    set_descriptive_armor_shopping(armor_shopping_used);
                    scroll_menu(0);
                    break;
                case Shopping_Mode.Weapons:
                    set_descriptive_weapon_shopping(weapon_shopping_used);
                    scroll_menu(0);
                    break;
            }
        }

        public bool in_submenu()
        {
            return im_shopping_for != Shopping_Mode.Main;
        }

        public bool exit_submenu()
        {
            switch (im_shopping_for)
            {
                case Shopping_Mode.Armor:
                    return selected_item_index == armors_in_stock.Count;
                case Shopping_Mode.Weapons:
                    return selected_item_index == weapons_in_stock.Count;
                default:
                    return false;
            }
        }

        public void buy_item(Player pl)
        {
            int item_gold_value;
            switch (im_shopping_for)
            {
                case Shopping_Mode.Armor:
                    item_gold_value = armors_in_stock[selected_item_index].get_my_gold_value();
                    if (pl.get_my_gold() >= item_gold_value)
                    {
                        pl.pay_gold(item_gold_value);
                        pl.acquire_item((Item)armors_in_stock[selected_item_index]);
                        armors_in_stock.RemoveAt(selected_item_index);
                    }
                    break;
                case Shopping_Mode.Weapons:
                    item_gold_value = weapons_in_stock[selected_item_index].get_my_gold_value();
                    if (pl.get_my_gold() >= item_gold_value)
                    {
                        pl.pay_gold(item_gold_value);
                        pl.acquire_item((Item)weapons_in_stock[selected_item_index]);
                        weapons_in_stock.RemoveAt(selected_item_index);
                    }
                    break;
            }
            scroll_menu(0);
        }

        public void set_variables(Player pl)
        {
            intro_used = rGen.Next(1);
            weapon_shopping_used = rGen.Next(1);
            armor_shopping_used = rGen.Next(1);
            consumable_shopping_used = rGen.Next(1);
            talisman_shopping_used = rGen.Next(1);
            exit_used = rGen.Next(1);

            armors_in_stock = all_items.retrieve_random_shared_armors(4);
            weapons_in_stock = all_items.retrieve_random_shared_weapons(4);

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

            /* This isn't needed for the time being, and who knows! It may never be.
            List<Item> player_inventory = pl.retrieve_inventory();

            for (int i = 0; i < player_inventory.Count; i++)
            {
                //do nothing for now.
            }
             */
        }

        public void set_descriptive_intro(int choice)
        {
            switch (choice)
            {
                case 0:
                    descriptive_text = "You leave the stairs at a brisk trot, only to find yourself in what \n" + 
                                        "seems like a shop. Racks of weapons and suits of armor adorn the \n" +
                                        "walls, with shelves of potions and scrolls in the back. At the other \n" +
                                        "end of the room, there's a counter with a hooded man standing behind \n" +
                                        "it. You raise your hand and greet him cheerfully and he starts to \n" + 
                                        "return your gesture, but stops short and gives you an odd look. You \n" +
                                        "wonder why for a moment and then realize it's probably because of your \n" +
                                        "scorpion tail. \"Don't get venom on my wares...\" he mumbles at you. \n" +
                                        "You shrug and begin to browse.";
                    shopkeeper = "hooded man";
                    blurb_height = ((9 + 3) * sFont.LineSpacing);
                    break;
                default:
                    break;
            }
        }

        public void set_descriptive_weapon_shopping(int choice)
        {
            switch (choice)
            {
                case 0:
                    descriptive_text = "You shop for weapons.";
                    blurb_height = (1 * sFont.LineSpacing);
                    break;
            }
        }

        public void set_descriptive_armor_shopping(int choice)
        {
            switch (choice)
            {
                case 0:
                    descriptive_text = "You head back for the racks of armor, ignoring the piercing look the \n" + 
                                        shopkeeper + " is giving you. You look up and down the rows of the \n" +
                                        "various armors as you stroke your chin thoughtfully, considering \n" + 
                                        "exactly which one you want to wear. Picking out a set of armor is \n" + 
                                        "always so difficult. There are just so many things to consider! You \n" +
                                        "hum to yourself as you lean over and pick up a suit of chainmail, \n" + 
                                        "holding it up to the admittedly poor light.";
                    blurb_height = ((7 + 3) * sFont.LineSpacing);
                    break;
            }
        }

        public void set_descriptive_consumable_shopping(int choice)
        {
            switch (choice)
            {
                case 0:
                    break;
            }
        }
        
        public void set_descriptive_talisman_shopping(int choice)
        {
            switch (choice)
            {
                case 0:
                    break;
            }
        }

        public void set_descriptive_exit(int choice)
        {
            switch (choice)
            {
                case 0:
                    break;
            }
        }

        public void drawMe(ref SpriteBatch sBatch)
        {
            take_measurements();

            sBatch.DrawString(sFont, descriptive_text, blurb_position, Color.White);

            Vector2 position2;
            Color tint;
            Color exit_submenu_tint;

            switch(im_shopping_for)
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
                case Shopping_Mode.Armor:
                    take_item_menu_measurements(im_shopping_for);
                    position2 = new Vector2(item_menu_position.X, item_menu_position.Y);
                    for (int i = 0; i < armors_in_stock.Count; i++)
                    {
                        if (i == selected_item_index)
                            tint = highlighted;
                        else
                            tint = normal;
                        sBatch.DrawString(sFont, armors_in_stock[i].get_my_name(), position2, tint);
                        position2.Y += sFont.LineSpacing;
                    }
                    if (selected_item_index == armors_in_stock.Count)
                        exit_submenu_tint = highlighted;
                    else
                        exit_submenu_tint = normal;
                    sBatch.DrawString(sFont, sub_menu_exit, position2, exit_submenu_tint);
                    break;
                case Shopping_Mode.Weapons:
                    take_item_menu_measurements(im_shopping_for);
                    position2 = new Vector2(item_menu_position.X, item_menu_position.Y);
                    for (int i = 0; i < weapons_in_stock.Count; i++)
                    {
                        if (i == selected_item_index)
                            tint = highlighted;
                        else
                            tint = normal;
                        sBatch.DrawString(sFont, weapons_in_stock[i].get_my_name(), position2, tint);
                        position2.Y += sFont.LineSpacing;
                    }
                    if (selected_item_index == weapons_in_stock.Count)
                        exit_submenu_tint = highlighted;
                    else
                        exit_submenu_tint = normal;
                    sBatch.DrawString(sFont, sub_menu_exit, position2, exit_submenu_tint);
                    break;
            }

            if (im_shopping_for != Shopping_Mode.Main)
            {
                Vector2 item_info_position2 = new Vector2(item_info_position.X, item_info_position.Y);
                for (int i = 0; i < current_item_info.Count; i++)
                {
                    sBatch.DrawString(sFont, current_item_info[i], item_info_position2, Color.White);
                    item_info_position2.Y += sFont.LineSpacing;
                }
            }
        }
    }
}
