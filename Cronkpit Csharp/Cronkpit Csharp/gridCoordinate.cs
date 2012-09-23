using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cronkpit_Csharp
{
    class gridCoordinate
    {
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
    }
}
