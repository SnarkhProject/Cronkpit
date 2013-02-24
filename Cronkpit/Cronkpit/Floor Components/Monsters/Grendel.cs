using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Cronkpit
{
    class Grendel: Monster
    {
        public enum Grendel_Weapon_Type { Club, Frostbolt };
        Grendel_Weapon_Type my_weapon_type;
        gridCoordinate last_seen_player_at;
        bool have_i_seen_player;
        int frostbolt_manacost = 30;

        public Grendel(gridCoordinate sGridCoord, ContentManager sCont, int sIndex, Grendel_Weapon_Type wType)
            : base(sGridCoord, sCont, sIndex, Monster_Size.Normal)
        {
            my_weapon_type = wType;
            switch (my_weapon_type)
            {
                case Grendel_Weapon_Type.Club:
                    my_Texture = cont.Load<Texture2D>("Enemies/Grendel");
                    min_damage = 3;
                    max_damage = 5;
                    dmg_type = Attack.Damage.Crushing;
                    wound_type = wound.Wound_Type.Impact;
                    break;
                case Grendel_Weapon_Type.Frostbolt:
                    my_Texture = cont.Load<Texture2D>("Enemies/Grendel_frostmage");
                    min_damage = 2;
                    max_damage = 6;
                    dmg_type = Attack.Damage.Frost;
                    wound_type = wound.Wound_Type.Frostburn;
                    break;
            }

            hitPoints = 37;
            can_melee_attack = true;
            last_seen_player_at = new gridCoordinate(my_grid_coords[0]);

            //SENSORY
            sight_range = 5;
            can_hear = true;
            sounds_i_can_hear.Add(SoundPulse.Sound_Types.Player);
            listen_threshold.Add(5);
            smell_range = 4;
            smell_threshold = 3;

            //OTHER
            my_name = "Grendel";
            melee_dodge = 5;
            ranged_dodge = 5;
        }

        public override void Update_Monster(Player pl, Floor fl)
        {
            has_moved = false;
            Tile target_tile = strongest_smell_within(fl, 0, smell_threshold, smell_range);

            if (target_tile == null)
                has_scent = false;
            else
            {
                has_scent = true;
                strongest_smell_coord = target_tile.get_grid_c();
            }

            if (is_player_within(pl, sight_range))
                can_see_player = can_i_see_point(fl, pl.get_my_grid_C());
            else
                can_see_player = false;

            if (can_see_player)
            {
                have_i_seen_player = true;
                last_seen_player_at = new gridCoordinate(pl.get_my_grid_C());
                //fl.add_new_popup("The Grendel sees you!", Popup.popup_msg_color.Red, my_grid_coord);

                if (my_weapon_type == Grendel_Weapon_Type.Frostbolt && fl.check_mana() >= frostbolt_manacost)
                {
                    if (!is_player_within_diamond(pl, 4))
                        advance_towards_single_point(last_seen_player_at, pl, fl, 0, corporeal);
                    else
                    {
                        fl.addmsg("The Grendel attacks you!");
                        Attack dmg = dealDamage();
                        Projectile prj = new Projectile(randomly_chosen_personal_coord(), last_seen_player_at, Projectile.projectile_type.Frostbolt, ref cont, true, Scroll.Atk_Area_Type.singleTile);
                        prj.set_damage_range(min_damage, max_damage);
                        prj.set_damage_type(dmg_type);
                        prj.set_wound_type(wound_type);
                        fl.create_new_projectile(prj);
                    }
                }
                else
                {
                    if (my_weapon_type == Grendel_Weapon_Type.Frostbolt && fl.check_mana() < frostbolt_manacost)
                    {
                        min_damage = 3;
                        max_damage = 5;
                        dmg_type = Attack.Damage.Crushing;
                        wound_type = wound.Wound_Type.Impact;
                    }

                    if(!is_player_within(pl, 1))
                        advance_towards_single_point(last_seen_player_at, pl, fl, 1, corporeal);
                    else
                    {
                        fl.addmsg("The Grendel attacks you!");
                        fl.add_effect(dmg_type, pl.get_my_grid_C());
                        Attack dmg = dealDamage();
                        pl.take_damage(dmg, fl, "");
                    }
                }
            }
            else if(!can_see_player && have_i_seen_player)
            {
                //fl.add_new_popup("The Grendel goes to your last position!", Popup.popup_msg_color.Red, my_grid_coord);
                advance_towards_single_point(last_seen_player_at, pl, fl, 0, corporeal);
                if (occupies_tile(last_seen_player_at))
                    have_i_seen_player = false;
            }
            else if (!can_see_player && !have_i_seen_player && has_scent)
            {
                //fl.add_new_popup("The Grendel smells you!", Popup.popup_msg_color.Red, my_grid_coord);
                if (is_player_within(pl, 1))
                    advance_towards_single_point(strongest_smell_coord, pl, fl, 1, corporeal);
                else
                    advance_towards_single_point(strongest_smell_coord, pl, fl, 0, corporeal);

            }
            else
            {
                if (heard_something)
                    follow_path_to_sound(fl, pl);
                else
                {
                    //50% chance to wander.
                    int should_i_wander = rGen.Next(2);
                    if (should_i_wander == 1)
                    {
                        //fl.add_new_popup("The Grendel wanders!", Popup.popup_msg_color.Red, my_grid_coord);
                        wander(pl, fl, corporeal);
                    }
                }
            }
        }
    }
}
