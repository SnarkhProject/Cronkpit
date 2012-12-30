using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cronkpit
{
    class SpawnData
    {
        public string my_assoc_monster;
        public int my_assoc_number;

        public SpawnData(string monster, int number)
        {
            my_assoc_monster = monster;
            my_assoc_number = number;
        }
    }
}
