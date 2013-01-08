using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cronkpit
{
    class MainItemList
    {
        Random rGen;
        List<Item> shared_items;

        public MainItemList()
        {
            rGen = new Random();
            shared_items = new List<Item>();
            add_all_shared_items();
        }

        public void add_all_shared_items()
        {
            shared_items.Add(new Armor(3, 250, "Plate Mail", 8, 0, 0, 10, 10, 8, true));
            shared_items.Add(new Armor(4, 250, "Chain Mail", 8, 0, 2, 0, 10, 5, true));
            shared_items.Add(new Armor(5, 250, "Rubber Underwear", 3, 10, 3, 0, 0, 3, false));
            shared_items.Add(new Armor(6, 250, "Quilted Armor", 0, 4, 10, 0, 0, 3, false));
            shared_items.Add(new Weapon(7, 250, "Axe", Weapon.Type.Axe, 1, 3, 6, 1));
            shared_items.Add(new Weapon(8, 250, "Sword", Weapon.Type.Sword, 1, 3, 6, 1));
            shared_items.Add(new Weapon(9, 250, "Spear", Weapon.Type.Spear, 2, 2, 4, 2));
            shared_items.Add(new Weapon(10, 3500, "Katana", Weapon.Type.Sword, 2, 5, 12, 1));
            shared_items.Add(new Armor(11, 3000, "Enchanted Rags", 20, 15, 20, 5, 5, 10, true));
            shared_items.Add(new Weapon(12, 4500, "Hyperion", Weapon.Type.Spear, 2, 5, 12, 4));
            shared_items.Add(new Weapon(13, 250, "Shortbow", Weapon.Type.Bow, 1, 3, 6, 3));
            shared_items.Add(new Weapon(14, 3500, "Heavy Siege Bow", Weapon.Type.Bow, 1, 10, 24, 5));
            shared_items.Add(new Weapon(15, 250, "Training Lance", Weapon.Type.Lance, 1, 5, 8, 5));
            shared_items.Add(new Weapon(16, 3500, "Templar Lance", Weapon.Type.Lance, 1, 9, 29, 5));
            shared_items.Add(new Armor(17, 3000, "Padded Dragonscale", 5, 5, 10, 20, 20, 8, false));
            shared_items.Add(new Armor(18, 3000, "Obsidian Plate", 6, 1, 0, 5, 4, 30, true));
            shared_items.Add(new Potion(19, 500, "Minor Health Potion", Potion.Potion_Type.Health, 2));
            shared_items.Add(new Potion(20, 500, "Minor Repair Potion", Potion.Potion_Type.Repair, 4));
            shared_items.Add(new Potion(21, 1500, "Major Health Potion", Potion.Potion_Type.Health, 7));
            shared_items.Add(new Potion(22, 1500, "Major Repair Potion", Potion.Potion_Type.Repair, 13));
            shared_items.Add(new Weapon(23, 250, "Light Crossbow", Weapon.Type.Crossbow, 1, 3, 6, 3));
            shared_items.Add(new Weapon(24, 1750, "Heavy Crossbow", Weapon.Type.Crossbow, 1, 7, 14, 4));
            shared_items.Add(new Weapon(25, 3500, "Ballista", Weapon.Type.Crossbow, 1, 10, 24, 5));
            shared_items.Add(new Weapon(26, 1750, "Broadsword", Weapon.Type.Sword, 1, 4, 11, 1));
            shared_items.Add(new Weapon(27, 1750, "Reaver", Weapon.Type.Axe, 2, 4, 8, 1));
            shared_items.Add(new Weapon(28, 250, "Flanged Mace", Weapon.Type.Mace, 1, 3, 6, 1));
            shared_items.Add(new Weapon(29, 1750, "Pitsteel Warhammer", Weapon.Type.Mace, 1, 4, 11, 1));
            shared_items.Add(new Scroll(30, 250, "Firebolt I", 1));
            shared_items.Add(new Scroll(31, 250, "Acid Cloud", 1));
            shared_items.Add(new Scroll(32, 250, "Lightning Bolt", 1));
            shared_items.Add(new Scroll(33, 1750, "Chain Lightning", 2));
            shared_items.Add(new Scroll(34, 1750, "Greater Acid Cloud", 2));
            shared_items.Add(new Scroll(35, 1750, "Firebolt II", 2));
            shared_items.Add(new Scroll(36, 3500, "Earthquake", 3));
            shared_items.Add(new Scroll(37, 3500, "Firebolt III", 3));
        }

        public List<Armor> retrieve_random_shared_armors(int number)
        {
            List<Armor> fetched_list = new List<Armor>();
            for (int i = 0; i < number; i++)
            {
                bool done = false;
                while (!done)
                {
                    int item_index = rGen.Next(shared_items.Count);
                    //if valid item add it to the list and set done to true
                    //otherwise do nothing and force it to find a new list
                    //for now though we'll just add it and set done to true.
                    if (shared_items[item_index] is Armor)
                    {
                        fetched_list.Add((Armor)shared_items[item_index]);
                        done = true;
                    }
                }
            }
            return fetched_list;
        }

        public List<Weapon> retrieve_random_shared_weapons(int number)
        {
            List<Weapon> fetched_list = new List<Weapon>();
            for (int i = 0; i < number; i++)
            {
                bool done = false;
                while (!done)
                {
                    int item_index = rGen.Next(shared_items.Count);
                    //if valid item add it to the list and set done to true
                    //otherwise do nothing and force it to find a new list
                    //for now though we'll just add it and set done to true.
                    if (shared_items[item_index] is Weapon)
                    {
                        fetched_list.Add((Weapon)shared_items[item_index]);
                        done = true;
                    }
                }
            }
            return fetched_list;
        }

        public List<Item> retrieve_random_shared_consumables(int number)
        {
            List<Item> fetched_list = new List<Item>();
            for (int i = 0; i < number; i++)
            {
                bool done = false;
                while (!done)
                {
                    int item_index = rGen.Next(shared_items.Count);
                    if (shared_items[item_index] is Potion)
                    {
                        fetched_list.Add(shared_items[item_index]);
                        done = true;
                    }
                }
            }
            return fetched_list;
        }

        public List<Scroll> retrieve_random_shared_scrolls(int number)
        {
            List<Scroll> fetched_list = new List<Scroll>();
            for (int i = 0; i < number; i++)
            {
                bool done = false;
                while (!done)
                {
                    int item_index = rGen.Next(shared_items.Count);
                    //if valid item add it to the list and set done to true
                    //otherwise do nothing and force it to find a new list
                    //for now though we'll just add it and set done to true.
                    if (shared_items[item_index] is Scroll)
                    {
                        fetched_list.Add((Scroll)shared_items[item_index]);
                        done = true;
                    }
                }
            }
            return fetched_list;
        }
    }
}
