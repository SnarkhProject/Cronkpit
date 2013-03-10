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
        public enum fineness { Roughest, Rough, Average, Fine };
        gridCoordinate my_start_coordinate;
        gridCoordinate my_end_coordinate;
        public Vector2 my_end_position;
        public Vector2 my_current_position;
        int steps;

        public VisionRay(gridCoordinate my_start_gridC, gridCoordinate my_end_gridC, 
                         fineness fn = fineness.Average)
        {
            my_start_coordinate = new gridCoordinate(my_start_gridC);
            my_end_coordinate = new gridCoordinate(my_end_gridC);

            my_current_position = new Vector2((my_start_gridC.x * 32) + 16, (my_start_gridC.y * 32) + 16);
            my_end_position = new Vector2((my_end_gridC.x * 32) + 16, (my_end_gridC.y * 32) + 16);

            find_steps(fn);
        }

        public VisionRay(Vector2 start_position, Vector2 end_position, 
                         fineness fn = fineness.Average)
        {
            my_current_position = start_position;
            my_end_position = end_position;

            find_steps(fn);
        }

        public void find_steps(fineness fn)
        {
            switch (fn)
            {
                case fineness.Roughest:
                    steps = 10;
                    break;
                case fineness.Rough:
                    steps = 20;
                    break;
                case fineness.Average:
                    steps = 30;
                    break;
                case fineness.Fine:
                    steps = 80;
                    break;
                default:
                    steps = 50;
                    break;
            }
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
