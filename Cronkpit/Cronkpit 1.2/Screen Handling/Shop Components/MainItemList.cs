﻿using System;
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
            shared_items.Add(new Weapon(8, 200, "Sword", Weapon.Type.Sword, 1, 3, 6, 1));
            shared_items.Add(new Weapon(9, 250, "Spear", Weapon.Type.Spear, 2, 2, 3, 2));
            shared_items.Add(new Weapon(10, 3000, "Katana", Weapon.Type.Sword, 2, 5, 12, 1));
            shared_items.Add(new Armor(11, 3000, "Enchanted Rags", 20, 15, 20, 5, 5, 10, true));
            shared_items.Add(new Weapon(12, 4500, "Hyperion", Weapon.Type.Spear, 2, 5, 12, 4));
            shared_items.Add(new Weapon(13, 250, "Shortbow", Weapon.Type.Bow, 1, 3, 6, 3));
            shared_items.Add(new Weapon(14, 3750, "Heavy Siege Bow", Weapon.Type.Bow, 1, 10, 24, 5));
            shared_items.Add(new Weapon(15, 250, "Training Lance", Weapon.Type.Lance, 1, 5, 8, 5));
            shared_items.Add(new Weapon(16, 3750, "Templar Lance", Weapon.Type.Lance, 1, 9, 29, 5));
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
    }
}