using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cronkpit
{
    struct gridCoordinate 
    {
        public enum direction { UpLeft, Up, UpRight, Left, Right, DownLeft, Down, DownRight };
        public int x;
        public int y;

        public gridCoordinate(int sx, int sy)
        {
            x = sx;
            y = sy;
        }

        public gridCoordinate(gridCoordinate cpy)
        {
            x = cpy.x;
            y = cpy.y;
        }

        public void shift_direction(direction dir)
        {
            switch (dir)
            {
                case direction.UpLeft:
                    x--;
                    y--;
                    break;
                case direction.Up:
                    y--;
                    break;
                case direction.UpRight:
                    y--;
                    x++;
                    break;
                case direction.Left:
                    x--;
                    break;
                case direction.Right:
                    x++;
                    break;
                case direction.DownLeft:
                    y++;
                    x--;
                    break;
                case direction.Down:
                    y++;
                    break;
                case direction.DownRight:
                    y++;
                    x++;
                    break;
            }
        }
    }
}
