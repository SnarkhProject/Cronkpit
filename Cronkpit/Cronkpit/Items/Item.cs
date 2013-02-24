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
    class Item
    {
        protected int identification;
        protected Texture2D my_texture;
        protected int cost;
        protected string name;
        protected List<Talisman> talismans_equipped;

        public Item(int IDno, int goldVal, string my_name)
        {
            identification = IDno;
            cost = goldVal;
            name = my_name;
            talismans_equipped = new List<Talisman>();
        }

        public string get_my_name()
        {
            return name;
        }

        public int get_my_IDno()
        {
            return identification;
        }

        public int get_my_gold_value()
        {
            return cost;
        }

        public void set_texture(Texture2D target_tex)
        {
            my_texture = target_tex;
        }

        public virtual string get_my_texture_name()
        {
            string tex_name = name.ToLower();
            return tex_name.Replace(" ", String.Empty) + "_icon";
        }

        public Texture2D get_my_texture()
        {
            return my_texture;
        }

        public virtual List<string> get_my_information()
        {
            List<string> return_array = new List<string>();

            return_array.Add(name);
            return_array.Add(cost.ToString());

            return return_array;
        }

        public void add_talisman(Talisman T)
        {
            talismans_equipped.Add(T);
        }

        public bool can_add_talisman(Talisman T)
        {
            bool can_add_talisman = true;

            if (talismans_equipped.Count >= 2)
                can_add_talisman = false;

            for (int i = 0; i < talismans_equipped.Count; i++)
                if (T.get_my_type() == talismans_equipped[i].get_my_type() && !T.stackable_talisman())
                    can_add_talisman = false;

            return can_add_talisman;
        }

        public List<Talisman> get_my_equipped_talismans()
        {
            return talismans_equipped;
        }

        public virtual void draw_me(Rectangle location, ref SpriteBatch sBatch)
        {
            sBatch.Draw(my_texture, location, Color.White);
        }
    }
}
