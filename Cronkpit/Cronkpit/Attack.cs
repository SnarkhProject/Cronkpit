using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cronkpit
{
    class Attack
    {
        public enum Damage { Slashing, Piercing, Crushing, Fire, Frost, Acid, Electric };
        wound assoc_wound;
        Damage damage_type;

        public Attack(Damage dmg_type, wound harm)
        {
            damage_type = dmg_type;
            assoc_wound = new wound(harm);
        }

        public Damage get_dmg_type()
        {
            return damage_type;
        }

        public wound get_assoc_wound()
        {
            return assoc_wound;
        }
    }
}
