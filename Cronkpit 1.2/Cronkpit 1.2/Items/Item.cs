using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Cronkpit_1._2
{
    class Item
    {
        int identification;
        Texture2D my_texture;
        int cost;
        string name;

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

        public int get_my_gold_value()
        {
            return cost;
        }

        public void set_texture(Texture2D target_tex)
        {
            my_texture = target_tex;
        }

        public void draw_me(Rectangle location, ref SpriteBatch sBatch)
        {
            sBatch.Begin(SpriteSortMode.BackToFront, null);
            sBatch.Draw(my_texture, location, Color.White);
            sBatch.End();
        }
    }
}
