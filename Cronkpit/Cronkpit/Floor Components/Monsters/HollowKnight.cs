﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Cronkpit
{
    class HollowKnight: Monster
    {
        
        public HollowKnight(gridCoordinate sGridCoord, ContentManager sCont, int sIndex)
            : base(sGridCoord, sCont, sIndex, Monster_Size.Normal)
        {
            my_Texture = cont.Load<Texture2D>("Enemies/hollowKnight_idle");
            can_hear = true;
            max_hitPoints = 5;
            hitPoints = max_hitPoints;
            armorPoints = 22;
            min_damage = 1;
            max_damage = 3;
            dmg_type = Attack.Damage.Piercing;

            //SENSORY
            sounds_i_can_hear.Add(SoundPulse.Sound_Types.Player);
            base_listen_threshold.Add(8);

            set_senses_to_baseline();

            //OTHER
            speed_denominator = 1;
            my_name = "Hollow Knight";
            melee_dodge = 5;
            ranged_dodge = 95;
            armor_effectiveness = 95;
            set_initial_dodge_values();
            dodge_values_degrade = false;
        }

        public void set_to_activeTexture()
        {
            my_Texture = cont.Load<Texture2D>("Enemies/hollowKnight");
        }

        public override void Update_Monster(Player pl, Floor fl)
        {
            if (heard_something && !active)
            {
                active = true;
                set_to_activeTexture();
                base_listen_threshold[0] = 2;
                set_senses_to_baseline();

                fl.addmsg("The Hollow Knight awakens with a lurch and a strange creak!");
                fl.add_new_popup("Awakens!", Popup.popup_msg_color.Red, my_grid_coords[0]);
            }

            if (!stunned)
            {
                has_moved = false;
                if (active)
                {
                    if (is_player_within(pl, 1) && !has_moved)
                    {
                        fl.addmsg("The Hollow Knight savagely impales you!");
                        fl.add_effect(dmg_type, pl.get_my_grid_C());
                        Attack dmg = dealDamage();
                        pl.take_damage(dmg, fl, "");
                    }
                    else
                        if (speed_numerator < speed_denominator)
                        {
                            follow_path_to_sound(fl, pl);
                            speed_numerator++;
                        }
                        else
                            speed_numerator = 0;
                }
            }

            base.Update_Monster(pl, fl);
        }
    }
}
