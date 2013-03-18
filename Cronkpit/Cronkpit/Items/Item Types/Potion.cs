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
    class Potion: Item
    {
        public enum Potion_Type { Health, Repair };

        Potion_Type my_type;
        Texture2D empty_texture;

        bool is_empty;
        int quantity;
        int potency;
        

        public Potion(int IDno, int goldVal, string myName, Potion_Type ptype, int ppotent)
            : base(IDno, goldVal, myName)
        {
            my_type = ptype;
            is_empty = false;
            quantity = 1;
            potency = ppotent;
        }

        public Potion(int IDno, int goldVal, string myName, Potion p)
            : base(IDno, goldVal, myName)
        {
            my_type = p.get_type();
            is_empty = p.is_potion_empty();
            quantity = p.get_my_quantity();
            potency = p.potion_potency();
        }

        public string get_my_empty_texture_name()
        {
            string tex_name = name.ToLower();
            string empty_tex_name = "empty" + tex_name;
            return empty_tex_name.Replace(" ", String.Empty) + "_icon";
        }

        public void set_empty_texture(Texture2D empty_tex)
        {
            empty_texture = empty_tex;
        }

        public bool is_potion_empty()
        {
            return is_empty;
        }

        public int potion_potency()
        {
            return potency;
        }

        public void drink()
        {
            is_empty = true;
        }

        public void refill()
        {
            is_empty = false;
        }

        public void adjust_quantity(int quan)
        {
            quantity += quan;
        }

        public void set_quantity(int quan)
        {
            quantity = quan;
        }

        public int get_my_quantity()
        {
            return quantity;
        }

        public Potion_Type get_type()
        {
            return my_type;
        }

        public override List<string> get_my_information(bool in_shop)
        {
            List<string> return_array = new List<string>();

            if (is_empty)
                return_array.Add("Empty " + name);
            else
                return_array.Add(name);
            return_array.Add("Cost: " + cost.ToString());
            return_array.Add("Quantity: " + quantity.ToString());
            return_array.Add(" ");

            string section1 = "";
            if (my_type == Potion_Type.Repair)
                section1 = "Repairs " + potency + " damage to chosen armor.";
            else if (my_type == Potion_Type.Health)
                section1 = "Heals " + potency + " wounds.";

            return_array.Add(section1);

            string section2 = "This potion ";
            if (my_type == Potion_Type.Repair)
                section2 += "repairs " + (int)(potency * 1.7) + " damage";
            else if (my_type == Potion_Type.Health)
                section2 += "heals " + (int)(potency * 1.7) + " wounds";
            
            return_array.Add(section2);
            return_array.Add("when the potion is ingested;");
            return_array.Add("however, you cannot control");

            string section3 = "where ";
            if (my_type == Potion_Type.Repair)
                section3 += "the damage is repaired.";
            else if (my_type == Potion_Type.Health)
                section3 += "the wounds are healed.";

            return_array.Add(section3);

            return return_array;
        }

        public override void draw_me(Rectangle location, ref SpriteBatch sBatch)
        {
            Texture2D current_texture = my_texture;
            if (is_empty)
                current_texture = empty_texture;

            sBatch.Draw(current_texture, location, Color.White);
        }
    }
}
