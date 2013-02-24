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

            //Specifically overwrites the hyperion spear to do fire damage
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
            return_array.Add(" ");
            switch (talismans_equipped.Count)
            {
                case 0:
                    return_array.Add("[ ] - No Talisman");
                    return_array.Add("[ ] - No Talisman");
                    break;
                case 1:
                    return_array.Add("[X] - " + talismans_equipped[0].get_my_name());
                    return_array.Add("[ ] - No Talisman");
                    break;
                case 2:
                    return_array.Add("[X] - " + talismans_equipped[0].get_my_name());
                    return_array.Add("[X] - " + talismans_equipped[1].get_my_name());
                    break;
            }
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
            int modified_min_damage = min_damage;
            int modified_max_damage = max_damage;
            int modifier_value = 0;

            for (int i = 0; i < talismans_equipped.Count; i++)
            {
                if (talismans_equipped[i].get_my_type() == Talisman.Talisman_Type.Expediency)
                {
                    switch (talismans_equipped[i].get_my_prefix())
                    {
                        case Talisman.Talisman_Prefix.Rough:
                            modifier_value = 2;
                            break;
                        case Talisman.Talisman_Prefix.Flawed:
                            modifier_value = 3;
                            break;
                        case Talisman.Talisman_Prefix.Average:
                            modifier_value = 4;
                            break;
                        case Talisman.Talisman_Prefix.Great:
                            modifier_value = 5;
                            break;
                        case Talisman.Talisman_Prefix.Perfect:
                            modifier_value = 6;
                            break;
                    }
                    modified_min_damage += modifier_value;
                    modified_max_damage += (modifier_value * 2);
                }
            }

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
