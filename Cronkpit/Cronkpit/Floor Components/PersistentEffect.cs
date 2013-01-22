using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cronkpit
{
    class PersistentEffect
    {
        public enum special_effect_type { None, Earthquake };

        gridCoordinate effect_center;
        int turns_remaining;
        int effect_size;
        Scroll.Atk_Area_Type my_effect_type;
        special_effect_type my_special_fx;

        Attack.Damage effect_damage_type;
        wound.Wound_Type effect_wound_type;
        int min_damage;
        int max_damage;

        bool monster_effect;
        bool effect_ready;

        public PersistentEffect(Scroll.Atk_Area_Type etyp, special_effect_type setyp, 
                                gridCoordinate ectr, int turnsr, bool m_effect,
                                Attack.Damage fx_dmg_type, wound.Wound_Type fx_wnd_type,
                                int aoe_size, int min_dmg, int max_dmg)
        {
            my_effect_type = etyp;
            my_special_fx = setyp;
            effect_center = ectr;
            turns_remaining = turnsr;
            monster_effect = m_effect;
            effect_ready = false;
            effect_size = aoe_size;

            effect_damage_type = fx_dmg_type;
            effect_wound_type = fx_wnd_type;

            min_damage = min_dmg;
            max_damage = max_dmg;
        }

        public gridCoordinate get_center()
        {
            return effect_center;
        }

        public int get_effect_size()
        {
            return effect_size;
        }

        public int get_turns_left()
        {
            return turns_remaining;
        }

        public Scroll.Atk_Area_Type get_my_effect_type()
        {
            return my_effect_type;
        }

        public special_effect_type get_my_special_fx()
        {
            return my_special_fx;
        }

        public Attack.Damage get_my_damage_type()
        {
            return effect_damage_type;
        }

        public wound.Wound_Type get_my_wound_type()
        {
            return effect_wound_type;
        }

        public int get_specific_damage(bool maxdamage)
        {
            if (maxdamage)
                return max_damage;
            else
                return min_damage;
        }

        public void adjust_turns_remaining(int adjustment)
        {
            turns_remaining += adjustment;
        }

        public bool is_monster_effect()
        {
            return monster_effect;
        }

        public bool is_effect_ready()
        {
            return effect_ready;
        }

        public void ready_effect()
        {
            effect_ready = true;
        }
    }
}
