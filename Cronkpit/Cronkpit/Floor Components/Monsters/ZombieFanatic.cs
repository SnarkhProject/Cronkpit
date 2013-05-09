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
    class ZombieFanatic: Monster
    {
        gridCoordinate last_seen_player_at;
        bool have_i_seen_player = false;

        public ZombieFanatic(gridCoordinate sGridCoord, ContentManager sCont, int sIndex)
            : base(sGridCoord, sCont, sIndex, Monster_Size.Normal)
        {
            my_Texture = cont.Load<Texture2D>("Enemies/zombie_fanatic");
            max_hitPoints = 15;
            hitPoints = max_hitPoints;
            armorPoints = 20;
            min_damage = 2;
            max_damage = 3;
            dmg_type = Attack.Damage.Slashing;

            //SENSORY
            base_sight_range = 4;
            can_hear = true;
            sounds_i_can_hear.Add(SoundPulse.Sound_Types.Voidwraith_Scream);
            sounds_i_can_hear.Add(SoundPulse.Sound_Types.Fanatic_Scream);
            base_listen_threshold.Add(1);
            base_listen_threshold.Add(1);

            set_senses_to_baseline();

            //OTHER
            speed_denominator = 1;
            my_name = "Zombie Fanatic";
            melee_dodge = 10;
            ranged_dodge = 5;
            armor_effectiveness = 50;
            last_seen_player_at = my_grid_coords[0];
            set_initial_dodge_values();
            smart_monster = true;
        }

        public override void Update_Monster(Player pl, Floor fl)
        {
            has_moved = false;
            heal_near_altar(fl);

            if (is_player_within(pl, sight_range))
                can_see_player = can_i_see_point(fl, pl.get_my_grid_C());
            else
                can_see_player = false;

            if (!stunned)
            {
                if (can_see_player)
                {
                    last_seen_player_at = pl.get_my_grid_C();
                    have_i_seen_player = true;
                }

                if (speed_numerator < speed_denominator)
                {
                    if (can_see_player)
                    {
                        if (is_player_within(pl, 1))
                        {
                            fl.addmsg("The Zombie Fanatic slashes at you with its knife!");
                            Attack dmg = dealDamage();
                            fl.add_effect(dmg_type, pl.get_my_grid_C());
                            pl.take_damage(dmg, fl, "");
                        }
                        else
                            advance_towards_single_point(pl.get_my_grid_C(), pl, fl, 1, corporeal);
                        speed_numerator++;
                    }
                    else if (!can_see_player && have_i_seen_player)
                    {
                        advance_towards_single_point(last_seen_player_at, pl, fl, 0, corporeal);
                        if (occupies_tile(last_seen_player_at))
                            have_i_seen_player = false;
                        speed_numerator++;
                    }
                    else if (!can_see_player && !have_i_seen_player && heard_something)
                    {
                        follow_path_to_sound(fl, pl);
                        speed_numerator++;
                    }
                    else
                    {
                        int wander_chance = rGen.Next(3);
                        if (wander_chance == 1)
                            wander(pl, fl, corporeal);
                    }
                }
                else
                {
                    if (can_see_player || have_i_seen_player || heard_something)
                    {
                        fl.add_new_popup("Screams!", Popup.popup_msg_color.Red, my_grid_coords[0]);
                        fl.sound_pulse(my_grid_coords[0], 10, SoundPulse.Sound_Types.Voidwraith_Scream);
                        speed_numerator = 0;
                    }
                    else
                    {
                        int wander_chance = rGen.Next(3);
                        if (wander_chance == 1)
                            wander(pl, fl, corporeal);
                    }
                }
            }

            base.Update_Monster(pl, fl);
        }
    }
}
