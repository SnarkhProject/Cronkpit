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
    class Doodad
    {
        public enum Doodad_Type { ArmorSuit };
        Texture2D my_impassable_texture;
        Texture2D my_passable_texture;
        gridCoordinate my_grid_coord;
        Vector2 drawing_position;
        public bool passable;
        public bool destroyable;
        int HP;
        int index;
        string name;

        public Doodad(Doodad_Type dType, ContentManager cManage, gridCoordinate s_coord, int s_ind)
        {
            switch (dType)
            {
                case Doodad_Type.ArmorSuit:
                    my_impassable_texture = cManage.Load<Texture2D>("Enemies/hollowKnight_idle");
                    my_passable_texture = cManage.Load<Texture2D>("Entities/broken_armor");
                    name = "suit of armor";
                    passable = false;
                    destroyable = true;
                    HP = 22;
                    break;
            }
            index = s_ind;
            my_grid_coord = s_coord;
            drawing_position = new Vector2(my_grid_coord.x * 32, my_grid_coord.y * 32);
        }

        public string my_name()
        {
            return name;
        }

        public void take_damage(int dmg, List<string> msgBuf, Floor fl)
        {
            msgBuf.Add("The " + name + " takes " + dmg + " damage!");
            fl.add_new_popup("-" + dmg, Popup.popup_msg_color.Red, my_grid_coord);
            HP -= dmg;
            if (HP <= 0)
            {
                passable = true;
                destroyable = false;
            }
        }

        public void draw_me(ref SpriteBatch sBatch)
        {
            if (!passable)
                sBatch.Draw(my_impassable_texture, drawing_position, Color.White);
            else
                sBatch.Draw(my_passable_texture, drawing_position, Color.White);

        }

        public gridCoordinate get_g_coord()
        {
            return my_grid_coord;
        }

        public bool is_passable()
        {
            return passable;
        }

        public int get_my_index()
        {
            return index;
        }

        public bool is_destructible()
        {
            return destroyable;
        }

        public void destroy()
        {
            HP = 0;
            passable = true;
        }
    }
}
