using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cronkpit
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

        int cooldown;

        public Weapon(int IDno, int goldVal, string myName,
                    Type typ, int hnd, int min_dmg, int max_dmg, int wpn_range)
            : base(IDno, goldVal, myName)
        {
            weaponType = typ;
            hands = hnd;
            min_damage = min_dmg;
            max_damage = max_dmg;
            weapon_range = wpn_range;
            cooldown = 0;

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

            if (IDno == 12)
                damageType = Attack.Damage.Fire;
        }

        public Weapon(int IDno, int goldVal, string myName, Weapon w)
            : base(IDno, goldVal, myName)
        {
            weaponType = w.get_my_weapon_type();
            hands = w.get_hand_count();
            min_damage = w.specific_damage_val(false);
            max_damage = w.specific_damage_val(true);
            weapon_range = w.get_my_range();
            cooldown = 0;

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

            if (IDno == 12)
                damageType = Attack.Damage.Fire;
        }

        public override List<string> get_my_information()
        {
            List<string> return_array = new List<string>();

            return_array.Add(name);
            return_array.Add("Price: " + cost.ToString());
            return_array.Add(" ");
            string w_type = "Weapon type: ";
            switch (weaponType)
            {
                case Type.Axe:
                    w_type += "Axe";
                    break;
                case Type.Bow:
                    w_type += "Bow";
                    break;
                case Type.Crossbow:
                    w_type += "Crossbow";
                    break;
                case Type.Lance:
                    w_type += "Lance";
                    break;
                case Type.Mace:
                    w_type += "Mace";
                    break;
                case Type.Spear:
                    w_type += "Spear";
                    break;
                case Type.Sword:
                    w_type += "Sword";
                    break;
            }
            return_array.Add(w_type);
            string d_type = "Damage Type: ";
            switch (damageType)
            {
                case Attack.Damage.Crushing:
                    d_type += "Crushing";
                    break;
                case Attack.Damage.Piercing:
                    d_type += "Piercing";
                    break;
                case Attack.Damage.Slashing:
                    d_type += "Slashing";
                    break;
                case Attack.Damage.Fire:
                    d_type += "Fire";
                    break;
                default:
                    d_type += "Unknown";
                    break;
            }
            return_array.Add(d_type);
            return_array.Add(" ");
            return_array.Add("Minimum Damage: " + min_damage*hands);
            return_array.Add("Maximum Damage: " + max_damage*hands);
            return_array.Add("Range: " + weapon_range);

            switch (weaponType)
            {
                case Weapon.Type.Crossbow:
                case Weapon.Type.Bow:
                    return_array.Add(" ");
                    return_array.Add("Cannot equip two bows, crossbows,");
                    return_array.Add("or a bow and a crossbow.");
                    break;
            }

            return return_array;
        }

        public int specific_damage_val(bool maxdmg)
        {
            if (maxdmg)
                return max_damage;
            else
                return min_damage;
        }

        public int damage(ref Random rGen)
        {
            return rGen.Next(min_damage, max_damage + 1);
        }

        public int get_hand_count()
        {
            return hands;
        }

        public Attack.Damage get_my_damage_type()
        {
            return damageType;
        }

        public Type get_my_weapon_type()
        {
            return weaponType;
        }

        public int get_my_range()
        {
            return weapon_range;
        }

        public int get_current_cooldown()
        {
            return cooldown;
        }

        public void set_cooldown(int cdown)
        {
            cooldown += cdown;
        }
    }
}
