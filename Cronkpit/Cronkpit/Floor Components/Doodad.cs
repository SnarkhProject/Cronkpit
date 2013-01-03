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
        Texture2D my_texture;
        gridCoordinate my_grid_coord;
        Vector2 drawing_position;
        public bool passable;

        public Doodad(Texture2D s_tex, gridCoordinate s_coord, bool s_pass)
        {
            my_texture = s_tex;
            my_grid_coord = s_coord;
            drawing_position = new Vector2(my_grid_coord.x * 32, my_grid_coord.y * 32);
            passable = s_pass;
        }

        public void draw_me(ref SpriteBatch sBatch)
        {
            sBatch.Draw(my_texture, drawing_position, Color.White);
        }

        public bool is_passable()
        {
            return passable;
        }
    }
}
