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

        public Item(int IDno, int goldVal, string my_name)
        {
            identification = IDno;
            cost = goldVal;
            name = my_name;
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

        public string get_my_texture_name()
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

        public virtual void draw_me(Rectangle location, ref SpriteBatch sBatch)
        {
            sBatch.Draw(my_texture, location, Color.White);
        }
    }
}
