using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cronkpit
{
    struct gridCoordinate 
    {
        public enum direction { UpLeft, Up, UpRight, Left, Right, DownLeft, Down, DownRight };
        public enum coord { xCoord, yCoord };
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

        public gridCoordinate(gridCoordinate cpy, direction shifted_direction)
        {
            x = cpy.x;
            y = cpy.y;
            shift_direction(shifted_direction);
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

        public void combineCoords(gridCoordinate g)
        {
            x += g.x;
            y += g.y;
        }

        public int get_a_coord(coord c)
        {
            if (c == coord.xCoord)
                return x;
            else
                return y;
        }
    }
}
