using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cronkpit
{
    class Scroll: Item
    {
        public enum Atk_Area_Type { singleTile, cloudAOE, solidblockAOE, randomblockAOE, 
                                    personalBuff, piercingBolt, smallfixedAOE, chainedBolt,
                                    enemyDebuff };
        public enum Spell_Status_Effect { Blind, Deaf, Anosmia, LynxFer, PantherFer, TigerFer };

        //Enums
        Atk_Area_Type my_sType;
        Attack.Damage spell_dmg_type;
        Projectile.projectile_type my_prjType;
        Projectile.special_anim my_specAnim;
        Spell_Status_Effect myBufforDebuff;
        //Ints
        int scroll_tier;
        int aoe_size;
        int max_range;
        int min_damage;
        int max_damage;
        int buffDebuff_duration;
        int total_impacts;
        int mana_cost;
        //Bools
        bool destroys_walls;
        bool melee_range;

        public Scroll(int IDno, int goldVal, string myName, int stier, int smana, int srange, int sAoe, int minDmg, int maxDmg, bool meleeSpell,
                      Atk_Area_Type s_spell_type, Projectile.projectile_type s_prj_type, Attack.Damage dmg_type, bool destroyWalls, int t_impacts,
                      Spell_Status_Effect s_buffDebuff, int spell_duration, Projectile.special_anim s_prj_special_anim)
            : base(IDno, goldVal, myName)
        {
            //Enums
            my_sType = s_spell_type;
            spell_dmg_type = dmg_type;
            my_prjType = s_prj_type;
            my_specAnim = s_prj_special_anim;
            myBufforDebuff = s_buffDebuff;
            //Ints
            scroll_tier = stier;
            aoe_size = sAoe;
            max_range = srange;
            min_damage = minDmg;
            max_damage = maxDmg;
            total_impacts = t_impacts;
            mana_cost = smana;
            buffDebuff_duration = spell_duration;
            //Bools
            destroys_walls = destroyWalls;
            melee_range = meleeSpell;
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

        //Enum getters
        public Attack.Damage get_damage_type()
        {
            return spell_dmg_type;
        }

        public Atk_Area_Type get_spell_type()
        {
            return my_sType;
        }

        public Projectile.projectile_type get_assoc_projectile()
        {
            return my_prjType;
        }

        public Spell_Status_Effect get_status_effect()
        {
            return myBufforDebuff;
        }

        public Projectile.special_anim get_spec_impact_anim()
        {
            return my_specAnim;
        }

        //Int getters
        public int get_tier()
        {
            return scroll_tier;
        }

        public int get_range()
        {
            int modified_range = max_range;
            for (int i = 0; i < talismans_equipped.Count; i++)
            {
                if (talismans_equipped[i].get_my_type() == Talisman.Talisman_Type.Reach)
                    modified_range += (int)Math.Floor((double)((int)talismans_equipped[i].get_my_prefix() + 1 / 2));
            }

            return modified_range;
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

        public int get_t_impacts()
        {
            return total_impacts;
        }

        public int get_manaCost()
        {
            return mana_cost;
        }

        public int get_duration()
        {
            return buffDebuff_duration;
        }

        //Bool getters
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

        public bool spell_destroys_walls()
        {
            return destroys_walls;
        }

        //String getters
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

        public override List<string> get_my_information(bool in_shop)
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
            if (!in_shop)
            {
                info.Add(" ");
                switch (talismans_equipped.Count)
                {
                    case 0:
                        info.Add("[ ] - No Talisman");
                        info.Add("[ ] - No Talisman");
                        break;
                    case 1:
                        info.Add("[X] - " + talismans_equipped[0].get_my_name());
                        info.Add("[ ] - No Talisman");
                        break;
                    case 2:
                        info.Add("[X] - " + talismans_equipped[0].get_my_name());
                        info.Add("[X] - " + talismans_equipped[1].get_my_name());
                        break;
                }
            }
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

        //Void setters
        public void set_t_impacts(int nt)
        {
            total_impacts = nt;
        }        
    }
}
