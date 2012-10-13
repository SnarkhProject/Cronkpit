using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cronkpit_1._2
{
    class Weapon: Item
    {
        public enum Type { Lance, Spear, Sword, Axe, Mace, Bow, Crossbow };
        int hands;

        Type weaponType;
        Attack.Damage damageType;
        int min_damage;
        int max_damage;

        int weapon_range;

        public Weapon(int IDno, int goldVal, string myName,
                    Type typ, int hnd, int min_dmg, int max_dmg, int wpn_range)
            : base(IDno, goldVal, myName)
        {
            weaponType = typ;
            hands = hnd;
            min_damage = min_dmg;
            max_damage = max_dmg;
            weapon_range = wpn_range;

            switch (weaponType)
            {
                case Type.Lance:
                    damageType = Attack.Damage.Piercing;
                    break;
                case Type.Spear:
                    damageType = Attack.Damage.Piercing;
                    break;
                case Type.Sword:
                    damageType = Attack.Damage.Slashing;
                    break;
                case Type.Axe:
                    damageType = Attack.Damage.Slashing;
                    break;
                case Type.Mace:
                    damageType = Attack.Damage.Crushing;
                    break;
                case Type.Bow:
                    damageType = Attack.Damage.Piercing;
                    break;
                case Type.Crossbow:
                    damageType = Attack.Damage.Piercing;
                    break;
            }
        }

        public int damage(ref Random rGen)
        {
            return rGen.Next(min_damage, max_damage + 1);
        }
    }
}
