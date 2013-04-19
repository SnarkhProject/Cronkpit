using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cronkpit
{
    class Weapon: Item
    {
        public enum Type { Lance, Spear, Sword, Axe, Mace, Bow, Crossbow, Staff };
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
                //Spears, lances, bows and crossbows do piercing damage
                case Type.Spear:
                case Type.Lance:
                case Type.Bow:
                case Type.Crossbow:
                    damageType = Attack.Damage.Piercing;
                    break;
                //Swords and axes do slashing damage
                case Type.Sword:
                case Type.Axe:
                    damageType = Attack.Damage.Slashing;
                    break;
                //Maces and staffs do crushing damage
                case Type.Mace:
                case Type.Staff:
                    damageType = Attack.Damage.Crushing;
                    break;
            }

            if (hands == 2)
                max_talismans = 4;

            //Specifically overwrites the hyperion spear to do fire damage
            if (IDno == 12)
                damageType = Attack.Damage.Fire;
        }

        public Weapon(Weapon w)
            : base(w.get_my_IDno(), w.get_my_gold_value(), w.get_my_name())
        {
            weaponType = w.get_my_weapon_type();
            hands = w.get_hand_count();
            min_damage = w.specific_damage_val(false);
            max_damage = w.specific_damage_val(true);
            weapon_range = w.get_my_range();
            cooldown = 0;

            switch (weaponType)
            {
                //Spears, lances, bows and crossbows do piercing damage
                case Type.Spear:
                case Type.Lance:
                case Type.Bow:
                case Type.Crossbow:
                    damageType = Attack.Damage.Piercing;
                    break;
                //Swords and axes do slashing damage
                case Type.Sword:
                case Type.Axe:
                    damageType = Attack.Damage.Slashing;
                    break;
                //Maces and staffs do crushing damage
                case Type.Mace:
                case Type.Staff:
                    damageType = Attack.Damage.Crushing;
                    break;
            }

            if (hands == 2)
                max_talismans = 4;

            if (identification == 12)
                damageType = Attack.Damage.Fire;
        }

        public override List<string> get_my_information(bool in_shop)
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
                case Type.Staff:
                    w_type += "Staff";
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
            if (!in_shop)
            {
                return_array.Add(" ");
                for (int i = 0; i < max_talismans; i++)
                {
                    if(i >= talismans_equipped.Count || talismans_equipped[i] == null)
                        return_array.Add("[ ] - No Talisman");
                    else
                        return_array.Add("[X] - " + talismans_equipped[i].get_my_name());
                }
            }
            return_array.Add(" ");
            return_array.Add("Minimum Damage: " + min_damage*hands);
            return_array.Add("Maximum Damage: " + max_damage*hands);
            return_array.Add("Range: " + weapon_range);

            if(weaponType == Type.Crossbow || 
               weaponType == Type.Bow || 
               weaponType == Type.Lance)
            {
                    return_array.Add(" ");
                    return_array.Add("Cannot equip two bows,");
                    return_array.Add("crossbows, or a bow and");
                    return_array.Add("a crossbow.");
            }

            if (weaponType == Type.Lance)
            {
                return_array.Add(" ");
                return_array.Add("Must be in inventory to");
                return_array.Add("use charge attack.");
                return_array.Add("75% damage penalty when");
                return_array.Add("equipped.");
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
            int modified_range = weapon_range;
            for (int i = 0; i < talismans_equipped.Count; i++)
            {
                if (talismans_equipped[i].get_my_type() == Talisman.Talisman_Type.Reach)
                    modified_range += (int)Math.Floor((double)(((int)talismans_equipped[i].get_my_prefix() + 1) / 2));
            }

            return modified_range;
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
