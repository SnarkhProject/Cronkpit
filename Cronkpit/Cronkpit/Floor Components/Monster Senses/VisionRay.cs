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
            int xDif = positive_difference((int)my_current_position.X, (int)my_end_position.X) / 32;
            int yDif = positive_difference((int)my_current_position.Y, (int)my_end_position.Y) / 32;
            int Hyp = 0;
            if (xDif != 0 && yDif != 0)
                Hyp = (int)Math.Sqrt((xDif * xDif) + (yDif * yDif));

            int step_coef = 0;
            switch (fn)
            {
                case fineness.Roughest:
                    step_coef = 2;
                    break;
                case fineness.Rough:
                    step_coef = 3;
                    break;
                case fineness.Average:
                    step_coef = 4;
                    break;
                case fineness.Fine:
                    step_coef = 5;
                    break;
                default:
                    step_coef = 6;
                    break;
            }

            if (xDif == 0)
                steps = Math.Max(step_coef, yDif * step_coef);
            else if (yDif == 0)
                steps = Math.Max(step_coef, xDif * step_coef);
            else
                steps = Math.Max(step_coef, Hyp * step_coef);
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

        private int positive_difference(int i1, int i2)
        {
            if (i1 > i2)
                return i1 - i2;
            else
                return i2 - i1;
        }
    }
}
