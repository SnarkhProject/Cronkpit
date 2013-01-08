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
        int sell_shopping_used;
        int buyback_shopping_used;
        int scroll_shopping_used;
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
        public enum Shopping_Mode { Main, Armor, Weapons, Consumables, Talismans, Scrolls, Sell, Buyback };
        Shopping_Mode im_shopping_for;

        //Item stuff
        MainItemList all_items;
        List<Armor> armors_in_stock;
        List<Weapon> weapons_in_stock;
        List<Scroll> scrolls_in_stock;
        List<Item> consumables_in_stock;
        List<Item> items_to_sell;
        List<Item> sold_items;
        List<string> current_item_info;
        int player_gold;

        public ShopScreen(List<string> mItems, SpriteFont sf, SpriteFont small_f, Rectangle cl, ref ContentManager cm)
        {
            menuItems = new List<string>(mItems);
            all_items = new MainItemList();
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

            if (target_menu == Shopping_Mode.Scrolls)
            {
                for (int i = 0; i < scrolls_in_stock.Count; i++)
                {
                    Vector2 size = sFont.MeasureString(scrolls_in_stock[i].get_my_name());
                    if (size.X > i_menu_width)
                        i_menu_width = size.X;
                    i_menu_height += sFont.LineSpacing + 5;
                }
            }

            if (target_menu == Shopping_Mode.Sell)
            {
                for (int i = 0; i < items_to_sell.Count; i++)
                {
                    Vector2 size = sFont.MeasureString(items_to_sell[i].get_my_name());
                    if (size.X > i_menu_width)
                        i_menu_width = size.X;
                    i_menu_height += sFont.LineSpacing + 5;
                }
            }

            if (target_menu == Shopping_Mode.Buyback)
            {
                for (int i = 0; i < sold_items.Count; i++)
                {
                    Vector2 size = sFont.MeasureString(sold_items[i].get_my_name());
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
                case Shopping_Mode.Consumables:
                    selectedIndex = 0;
                    selected_item_index += scroll;
                    if (selected_item_index < 0)
                        selected_item_index = consumables_in_stock.Count;
                    if (selected_item_index > consumables_in_stock.Count)
                        selected_item_index = 0;
                    if (selected_item_index < consumables_in_stock.Count)
                        current_item_info = consumables_in_stock[selected_item_index].get_my_information();
                    break;
                case Shopping_Mode.Sell:
                    selectedIndex = 0;
                    selected_item_index += scroll;
                    if (selected_item_index < 0)
                        selected_item_index = items_to_sell.Count;
                    if (selected_item_index > items_to_sell.Count)
                        selected_item_index = 0;
                    if (selected_item_index < items_to_sell.Count)
                        current_item_info = items_to_sell[selected_item_index].get_my_information();
                    break;
                case Shopping_Mode.Buyback:
                    selectedIndex = 0;
                    selected_item_index += scroll;
                    if (selected_item_index < 0)
                        selected_item_index = sold_items.Count;
                    if (selected_item_index > sold_items.Count)
                        selected_item_index = 0;
                    if (selected_item_index < sold_items.Count)
                        current_item_info = sold_items[selected_item_index].get_my_information();
                    break;
                case Shopping_Mode.Scrolls:
                    selectedIndex = 0;
                    selected_item_index += scroll;
                    if (selected_item_index < 0)
                        selected_item_index = scrolls_in_stock.Count;
                    if (selected_item_index > scrolls_in_stock.Count)
                        selected_item_index = 0;
                    if (selected_item_index < scrolls_in_stock.Count)
                        current_item_info = scrolls_in_stock[selected_item_index].get_my_information();
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
                case Shopping_Mode.Consumables:
                    set_descriptive_consumable_shopping(consumable_shopping_used);
                    scroll_menu(0);
                    break;
                case Shopping_Mode.Sell:
                    set_descriptive_selling(sell_shopping_used);
                    scroll_menu(0);
                    break;
                case Shopping_Mode.Buyback:
                    set_descriptive_buyback(buyback_shopping_used);
                    scroll_menu(0);
                    break;
                case Shopping_Mode.Scrolls:
                    set_descriptive_scroll_shopping(scroll_shopping_used);
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
                case Shopping_Mode.Consumables:
                    return selected_item_index == consumables_in_stock.Count;
                case Shopping_Mode.Sell:
                    return selected_item_index == items_to_sell.Count;
                case Shopping_Mode.Buyback:
                    return selected_item_index == sold_items.Count;
                case Shopping_Mode.Scrolls:
                    return selected_item_index == scrolls_in_stock.Count;
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
                case Shopping_Mode.Consumables:
                    item_gold_value = consumables_in_stock[selected_item_index].get_my_gold_value();
                    if (pl.get_my_gold() >= item_gold_value)
                    {
                        pl.pay_gold(item_gold_value);
                        if (consumables_in_stock[selected_item_index] is Potion)
                        {
                            int pt_ID = consumables_in_stock[selected_item_index].get_my_IDno();
                            int pt_cost = consumables_in_stock[selected_item_index].get_my_gold_value();
                            string pt_name = consumables_in_stock[selected_item_index].get_my_name();

                            Potion pt = new Potion(pt_ID, pt_cost, pt_name, (Potion)consumables_in_stock[selected_item_index]);
                            pl.acquire_potion(pt);
                        }
                        consumables_in_stock.RemoveAt(selected_item_index);
                    }
                    break;
                case Shopping_Mode.Sell:
                    sell_item(pl);
                    break;
                case Shopping_Mode.Buyback:
                    item_gold_value = sold_items[selected_item_index].get_my_gold_value();
                    int buyback_value = (int)Math.Max(item_gold_value * .93, item_gold_value - 500);
                    if (pl.get_my_gold() >= buyback_value)
                    {
                        pl.pay_gold(buyback_value);
                        if (sold_items[selected_item_index] is Potion)
                        {
                            Potion p = (Potion)sold_items[selected_item_index];
                            p.adjust_quantity(-1);
                            Potion p2 = new Potion(p.get_my_IDno(), p.get_my_gold_value(), p.get_my_name(), p);
                            p2.set_quantity(1);
                            pl.acquire_potion(p2);

                            if (p.get_my_quantity() == 0)
                                sold_items.RemoveAt(selected_item_index);
                        }
                        else
                        {
                            pl.acquire_item(sold_items[selected_item_index]);
                            sold_items.RemoveAt(selected_item_index);
                        }
                    }
                    break;
                case Shopping_Mode.Scrolls:
                    item_gold_value = scrolls_in_stock[selected_item_index].get_my_gold_value();
                    if (pl.get_my_gold() >= item_gold_value)
                    {
                        pl.pay_gold(item_gold_value);
                        pl.acquire_item((Item)scrolls_in_stock[selected_item_index]);
                        scrolls_in_stock.RemoveAt(selected_item_index);
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
            intro_used = rGen.Next(2);
            weapon_shopping_used = rGen.Next(1);
            armor_shopping_used = rGen.Next(1);
            consumable_shopping_used = rGen.Next(1);
            talisman_shopping_used = rGen.Next(1);
            sell_shopping_used = rGen.Next(1);
            buyback_shopping_used = rGen.Next(1);
            scroll_shopping_used = rGen.Next(1);
            exit_used = rGen.Next(1);

            armors_in_stock = all_items.retrieve_random_shared_armors(4);
            weapons_in_stock = all_items.retrieve_random_shared_weapons(4);
            consumables_in_stock = all_items.retrieve_random_shared_consumables(2);
            scrolls_in_stock = all_items.retrieve_random_shared_scrolls(3);
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
                case 1:
                    descriptive_text = "You take a step off the stairs, blood running off of your armor in \n" +
                                       "rivulets and pooling on the floor below. Truth be told, you can't \n" +
                                       "tell who's blood it is, whether it's yours or that of your enemies. \n" +
                                       "An ancient goblin rushes over and starts to dab at your armor with a \n" +
                                       "towel, sopping up the worst of the mess. You watch him with a wary \n" +
                                       "look in your eyes, your tail ready to strike at moment's notice. His \n" +
                                       "attentions seem sincere, as he returns to the counter a moment later. \n" +
                                       "\"Thanks,\" you say shortly, giving him a curt nod. \"Don't flatter \n" +
                                       "yourself,\" he snaps back. \" I'm just making sure you don't bleed on \n" +
                                       "my wares.\"";
                    shopkeeper = "ancient goblin";
                    blurb_height = ((10+3) * sFont.LineSpacing);
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
                    descriptive_text = "You shop for consumables.";
                    blurb_height = (1 * sFont.LineSpacing);
                    break;
            }
        }

        public void set_descriptive_scroll_shopping(int choice)
        {
            switch (choice)
            {
                case 0:
                    descriptive_text = "You take a look at the scroll and squint at it, carefully reading the \n" +
                                       "runes. Your brows furrow as you take a moment to go over what Petaer \n" +
                                       "told you in his lengthy explanation, carefully reciting to yourself. \n" +
                                       "\"Okay. I think I got this. This one makes you half-translucent, like \n" +
                                       "looking through stained glass.\" You pause. \"That's stupid. Why would \n" +
                                       "anyone want that?\" The " + shopkeeper + " looks at you strangely. \"You're \n" +
                                       "holding it upside down,\" he 'helpfully' points out. \"Shut the fuck \n" +
                                       "up. I didn't want your fucking opinion,\" you snap back, flipping \n" +
                                       "him your middle finger. He takes the hint.";
                    blurb_height = ((9+3) * sFont.LineSpacing);
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

        public void set_descriptive_selling(int choice)
        {
            switch (choice)
            {
                case 0:
                    descriptive_text = "\"I want to get rid of some of this shit. What's the going price around \n" +
                                       "here?\" You ask the shopkeeper, narrowing your eyes slightly at him. \n" +
                                       "The " + shopkeeper + " thinks for a moment, then replies \"All items will be \n" +
                                       "sold at half their market value.\" He barely has a moment to let out a \n" +
                                       "strangled gasp as you lunge at him, picking him up by the collar and \n" +
                                       "holding him in the air, your tail pressed to his throat. A single  \n" +
                                       "trickle of venom rolls down his neck as he makes small whimpering \n" +
                                       "noises. \"None of that shit.\" you hiss at him. \"You WILL give me a \n" +
                                       "fair price. Or I will kill you.\" \"F-f-fine!\" he stammers, \"7% less \n" +
                                       "than the original value or less 500 gold, whichever is less! Oh \n" +
                                       "gods please don't kill me.\" You put him down and hold up your items.";
                    blurb_height = ((11 + 3) * sFont.LineSpacing);
                    break;
            }
        }

        public void set_descriptive_buyback(int choice)
        {
            switch (choice)
            {
                case 0:
                    descriptive_text = "\"Hey, I want some of that back,\" you point to your items behind the \n" +
                                       "counter, then wave your hand impatiently. The " + shopkeeper + " grumbles \n" +
                                       "but dutifully complies, grabbing a few of your items and putting them \n" +
                                       "back on the counter. \"And you'd better not even fuucking think of \n" +
                                       "trying to sell them back to me for the full value, do you understand?\" \n" +
                                       "For emphasis, you bury the tip of your tail in the counter. He gulps, \n" +
                                       "but gives you a quick nod.";
                    blurb_height = ((7 + 3) * sFont.LineSpacing);
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

            SpriteFont item_info_font = sFont;
            if (blurb_height > (sFont.LineSpacing * 10))
                item_info_font = smaller_Font;

            Vector2 position2;
            Color tint;
            Color exit_submenu_tint;
            Vector2 player_gold_position = new Vector2(10, client.Height - sFont.LineSpacing - 10);

            sBatch.DrawString(sFont, "Gold Remaining: " + player_gold.ToString(), player_gold_position, Color.White);
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
                case Shopping_Mode.Consumables:
                    take_item_menu_measurements(im_shopping_for);
                    position2 = new Vector2(item_menu_position.X, item_menu_position.Y);
                    for (int i = 0; i < consumables_in_stock.Count; i++)
                    {
                        if (i == selected_item_index)
                            tint = highlighted;
                        else
                            tint = normal;
                        sBatch.DrawString(sFont, consumables_in_stock[i].get_my_name(), position2, tint);
                        position2.Y += sFont.LineSpacing;
                    }
                    if (selected_item_index == consumables_in_stock.Count)
                        exit_submenu_tint = highlighted;
                    else
                        exit_submenu_tint = normal;
                    sBatch.DrawString(sFont, sub_menu_exit, position2, exit_submenu_tint);
                    break;
                case Shopping_Mode.Sell:
                    draw_item_type_menu(items_to_sell, ref sBatch);
                    break;
                case Shopping_Mode.Buyback:
                    draw_item_type_menu(sold_items, ref sBatch);
                    break;
                case Shopping_Mode.Scrolls:
                    take_item_menu_measurements(im_shopping_for);
                    position2 = new Vector2(item_menu_position.X, item_menu_position.Y);
                    for (int i = 0; i < scrolls_in_stock.Count; i++)
                    {
                        if (i == selected_item_index)
                            tint = highlighted;
                        else
                            tint = normal;
                        sBatch.DrawString(sFont, scrolls_in_stock[i].get_my_name(), position2, tint);
                        position2.Y += sFont.LineSpacing;
                    }
                    if (selected_item_index == scrolls_in_stock.Count)
                        exit_submenu_tint = highlighted;
                    else
                        exit_submenu_tint = normal;
                    sBatch.DrawString(sFont, sub_menu_exit, position2, exit_submenu_tint);
                    break;
            }

            if (im_shopping_for != Shopping_Mode.Main)
            {
                Vector2 item_info_position2 = new Vector2(item_info_position.X, item_info_position.Y);
                if (blurb_height > sFont.LineSpacing*10 && current_item_info.Count > 10)
                    item_info_position2.Y += 20;

                for (int i = 0; i < current_item_info.Count; i++)
                {
                    sBatch.DrawString(item_info_font, current_item_info[i], item_info_position2, Color.White);
                    item_info_position2.Y += item_info_font.LineSpacing;
                }
            }
        }

        public void draw_item_type_menu(List<Item> target_menu, ref SpriteBatch sBatch)
        {
            take_item_menu_measurements(im_shopping_for);
            Vector2 position2 = new Vector2(item_menu_position.X, item_menu_position.Y);
            Color tint;
            Color exit_submenu_tint;

            for (int i = 0; i < target_menu.Count; i++)
            {
                if (i == selected_item_index)
                    tint = highlighted;
                else
                    tint = normal;
                sBatch.DrawString(sFont, target_menu[i].get_my_name(), position2, tint);
                position2.Y += sFont.LineSpacing;
            }
            if (selected_item_index == target_menu.Count)
                exit_submenu_tint = highlighted;
            else
                exit_submenu_tint = normal;
            sBatch.DrawString(sFont, sub_menu_exit, position2, exit_submenu_tint);
        }
    }
}
