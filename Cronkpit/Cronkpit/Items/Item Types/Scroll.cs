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
        wound.Wound_Type spell_wnd_type;
        int aoe_size;
        int max_range;
        int min_damage;
        int max_damage;
        bool melee_range;
        int total_impacts;

        public Scroll(int IDno, int goldVal, string myName, int stier, int srange, int sAoe, int minDmg, int maxDmg, bool meleeSpell,
                      Atk_Area_Type s_spell_type, Attack.Damage dmg_type)
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

            assign_wound_type();
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

            assign_wound_type();
        }

        void assign_wound_type()
        {
            switch (spell_dmg_type)
            {
                case Attack.Damage.Acid:
                    spell_wnd_type = wound.Wound_Type.Burn;
                    break;
                case Attack.Damage.Crushing:
                    spell_wnd_type = wound.Wound_Type.Impact;
                    break;
                case Attack.Damage.Electric:
                    break;
                case Attack.Damage.Fire:
                    spell_wnd_type = wound.Wound_Type.Burn;
                    break;
                case Attack.Damage.Frost:
                    spell_wnd_type = wound.Wound_Type.Frostburn;
                    break;
                case Attack.Damage.Piercing:
                    spell_wnd_type = wound.Wound_Type.Open;
                    break;
                case Attack.Damage.Slashing:
                    spell_wnd_type = wound.Wound_Type.Open;
                    break;
            }
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

        public wound.Wound_Type get_wound_type()
        {
            return spell_wnd_type;
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

        public void set_t_impacts(int nt)
        {
            total_impacts = nt;
        }

        public int get_t_impacts()
        {
            return total_impacts;
        }
    }
}
