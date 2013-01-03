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
    class RACursor
    {
        public enum Mode { Ranged, Charge, Bash };
        public gridCoordinate my_grid_coord;
        Texture2D my_RA_texture;
        Texture2D my_CA_texture;
        Texture2D my_BA_texture;
        Texture2D my_active_texture;
        Vector2 my_position;
        public bool am_i_visible;

        public RACursor(Texture2D sRAText, Texture2D sCAText, Texture2D sBAText, gridCoordinate sGrid_c)
        {
            my_grid_coord = sGrid_c;
            my_position = new Vector2(my_grid_coord.x * 32, my_grid_coord.y * 32);
            my_RA_texture = sRAText;
            my_CA_texture = sCAText;
            my_BA_texture = sBAText;
            my_active_texture = my_RA_texture;
            am_i_visible = false;
        }

        public void shift_coordinates(int xshift, int yshift)
        {
            my_grid_coord.x += xshift;
            my_grid_coord.y += yshift;

            reset_drawing_position();
        }

        public void reset_drawing_position()
        {
            my_position.X = my_grid_coord.x * 32;
            my_position.Y = my_grid_coord.y * 32;
        }

        public void shift_modes(Mode md)
        {
            switch (md)
            {
                case Mode.Charge:
                    my_active_texture = my_CA_texture;
                    break;
                case Mode.Ranged:
                    my_active_texture = my_RA_texture;
                    break;
                case Mode.Bash:
                    my_active_texture = my_BA_texture;
                    break;
            }
        }

        public void draw_me(ref SpriteBatch sBatch)
        {
            sBatch.Draw(my_active_texture, my_position, Color.White);
        }
    }
}
