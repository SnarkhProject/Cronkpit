using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cronkpit
{
    class Attack
    {
        public enum Damage { Slashing, Piercing, Crushing, Fire, Frost, Acid, Electric };
        int attack_damage;
        Damage damage_type;

        public Attack(Damage dmg_type, int damage)
        {
            damage_type = dmg_type;
            attack_damage = damage;
        }

        public Damage get_dmg_type()
        {
            return damage_type;
        }

        public int get_damage_amt()
        {
            return attack_damage;
        }

        public void decrease_severity(int dec)
        {
            attack_damage -= dec;
        }
    }
}
