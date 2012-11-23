using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cronkpit
{
    class Potion: Item
    {
        public Potion(int IDno, int goldVal, string myName)
            : base(IDno, goldVal, myName)
        {
        }
    }
}
