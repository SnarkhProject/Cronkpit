using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cronkpit
{
    class Scent
    {
        public int type;
        public int strength;

        //Scent type guide.
        //0 = player.
        public Scent(int sType, int sStr)
        {
            type = sType;
            strength = sStr;
        }
    }
}
