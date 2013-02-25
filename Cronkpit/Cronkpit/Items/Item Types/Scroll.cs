using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cronkpit
{
    class Scroll: Item
    {
        public enum Atk_Area_Type { singleTile, cloudAOE, solidblockAOE, randomblockAOE, 
                                    personalBuff, piercingBolt, smallfixedAOE, chainedBolt };
        int scroll_tier;
        Atk_Area_Type my_sType;
        Attack.Damage spell_dmg_type;
        int aoe_size;
        int max_range;
        int min_damage;
        int max_damage;
        bool melee_range;
        int total_impacts;
        bool destroys_walls;
        int mana_cost;

        public Scroll(int IDno, int goldVal, string myName, int stier, int smana, int srange, int sAoe, int minDmg, int maxDmg, bool meleeSpell,
                      Atk_Area_Type s_spell_type, Attack.Damage dmg_type, bool destroyWalls)
            : base(IDno, goldVal, myName)
        {
            scroll_tier = stier;
            aoe_size = sAoe;
            max_range = srange;
            min_damage = minDmg;
            max_damage = maxDmg;
            melee_range = meleeSpell;
            my_sType = s_spell_type;
            spell_dmg_type = dmg_type;
            total_impacts = 0;
            destroys_walls = destroyWalls;
            mana_cost = smana;
        }

        public Scroll(int IDno, int goldVal, string myName, Scroll s)
            : base(IDno, goldVal, myName)
        {
            scroll_tier = s.get_tier();
            aoe_size = s.get_aoe_size();
            max_range = s.get_range();
            min_damage = s.get_specific_damage(false);
            max_damage = s.get_specific_damage(true);
            melee_range = s.is_melee_range_spell();
            my_sType = s.get_spell_type();
            spell_dmg_type = s.get_damage_type();
            total_impacts = s.get_t_impacts();
            destroys_walls = s.spell_destroys_walls();
            mana_cost = s.get_manaCost();
        }

        public int get_tier()
        {
            return scroll_tier;
        }

        public int get_range()
        {
            return max_range;
        }

        public int get_aoe_size()
        {
            return aoe_size;
        }

        public int get_specific_damage(bool maxdamage)
        {
            if (maxdamage)
                return max_damage;
            else
                return min_damage;
        }

        public int damage_val(ref Random rGen)
        {
            return rGen.Next(min_damage, max_damage + 1);
        }

        public Attack.Damage get_damage_type()
        {
            return spell_dmg_type;
        }

        public Atk_Area_Type get_spell_type()
        {
            return my_sType;
        }

        public bool is_AoE_Spell()
        {
            return my_sType != Atk_Area_Type.singleTile && 
                   my_sType != Atk_Area_Type.personalBuff &&
                   my_sType != Atk_Area_Type.chainedBolt;
        }

        public bool is_melee_range_spell()
        {
            return melee_range;
        }

        public override string get_my_texture_name()
        {
            switch (scroll_tier)
            {
                case 1:
                    return "tier1scroll_icon";
                case 2:
                    return "tier2scroll_icon";
                case 3:
                    return "tier3scroll_icon";
                default:
                    if (scroll_tier > 3)
                        return "tier3scroll_icon";
                    else
                        return "tier1scroll_icon";
            }
        }

        public override List<string> get_my_information()
        {
            List<string> info = new List<string>();

            info.Add(name);
            info.Add("Cost: " + cost.ToString());
            info.Add(" ");
            string spelldmg_type = "";
            switch (spell_dmg_type)
            {
                case Attack.Damage.Acid:
                    spelldmg_type += "Acid";
                    break;
                case Attack.Damage.Crushing:
                    spelldmg_type += "Crushing";
                    break;
                case Attack.Damage.Electric:
                    spelldmg_type += "Electric";
                    break;
                case Attack.Damage.Fire:
                    spelldmg_type += "Fire";
                    break;
                case Attack.Damage.Frost:
                    spelldmg_type += "Frost";
                    break;
                case Attack.Damage.Piercing:
                    spelldmg_type += "Piercing";
                    break;
                case Attack.Damage.Slashing:
                    spelldmg_type += "Slashing";
                    break;
            }
            info.Add("Consumes " + mana_cost.ToString() + " mana.");
            info.Add("Damage type: " + spelldmg_type);
            info.Add("Minimum Damage: " + min_damage);
            info.Add("Maximum Damage: " + max_damage);
            info.Add("Range: " + max_range);
            if (melee_range)
                info.Add("Cast at melee range.");
            info.Add(" ");
            switch (my_sType)
            {
                case Atk_Area_Type.chainedBolt:
                    info.Add("Damages a single target.");
                    info.Add("Bolt can bounce to all");
                    info.Add("Targets within " + max_range.ToString() + " tiles.");
                    break;
                case Atk_Area_Type.cloudAOE:
                    info.Add("Effects random blocks");
                    info.Add("within a " + aoe_size.ToString() + " x " + aoe_size.ToString() + " area.");
                    info.Add("Tiles closer to the center");
                    info.Add("more likely to be effected.");
                    info.Add("Cloud lasts " + Math.Ceiling((double)aoe_size / 2) + " more turns.");
                    break;
                case Atk_Area_Type.personalBuff:
                    break;
                case Atk_Area_Type.piercingBolt:
                    info.Add("Fires a bolt that pierces");
                    info.Add("all targets in its path,");
                    info.Add("inflicting damage to each.");
                    break;
                case Atk_Area_Type.randomblockAOE:
                    info.Add("Effects random blocks");
                    info.Add("within a " + aoe_size.ToString() + " x " + aoe_size.ToString() + " area.");
                    break;
                case Atk_Area_Type.singleTile:
                    info.Add("Deals damage to a single target.");
                    break;
                case Atk_Area_Type.solidblockAOE:
                    info.Add("Effects all blocks");
                    info.Add("within a " + aoe_size.ToString() + " x " + aoe_size.ToString() + " area.");
                    break;
            }
            if (destroys_walls)
                info.Add("Destroys walls.");
            return info;
        }

        public void set_t_impacts(int nt)
        {
            total_impacts = nt;
        }

        public int get_t_impacts()
        {
            return total_impacts;
        }

        public bool spell_destroys_walls()
        {
            return destroys_walls;
        }

        public int get_manaCost()
        {
            return mana_cost;
        }
    }
}
