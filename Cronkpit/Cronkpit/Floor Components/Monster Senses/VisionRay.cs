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
    class VisionRay
    {
        gridCoordinate my_start_coordinate;
        gridCoordinate my_end_coordinate;
        public Vector2 my_end_position;
        public Vector2 my_current_position;
        int steps = 50;

        public VisionRay(gridCoordinate my_start_gridC, gridCoordinate my_end_gridC)
        {
            my_start_coordinate = new gridCoordinate(my_start_gridC);
            my_end_coordinate = new gridCoordinate(my_end_gridC);

            my_current_position = new Vector2((my_start_gridC.x * 32) + 16, (my_start_gridC.y * 32) + 16);
            my_end_position = new Vector2((my_end_gridC.x * 32) + 16, (my_end_gridC.y * 32) + 16);
        }

        public VisionRay(Vector2 start_position, Vector2 end_position)
        {
            my_current_position = start_position;
            my_end_position = end_position;
        }

        public void update()
        {
            Vector2 direction = my_end_position - my_current_position;
            direction.Normalize();

            my_current_position.X += direction.X / steps;
            my_current_position.Y += direction.Y / steps;
        }

        public bool is_at_end()
        {
            return Math.Round(my_current_position.X) == Math.Round(my_end_position.X) && 
                    Math.Round(my_current_position.Y) == Math.Round(my_end_position.Y);
        }
    }
}
