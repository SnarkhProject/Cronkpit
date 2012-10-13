using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cronkpit_1._2
{
    class Attack
    {
        public enum Damage { Slashing, Piercing, Crushing };
        wound assoc_wound;
        Damage damage_type;

        public Attack(Damage dmg_type, wound harm)
        {
            damage_type = dmg_type;
            assoc_wound = new wound(harm);
        }
    }
}
