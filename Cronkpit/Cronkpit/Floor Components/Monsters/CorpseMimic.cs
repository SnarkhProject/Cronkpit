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
    class CorpseMimic: Monster
    {
        Texture2D my_active_texture;
        Texture2D my_idle_texture;
        int turns_idle;

        public CorpseMimic(gridCoordinate sGridCoord, ContentManager sCont, int sIndex, int fl_number)
            : base(sGridCoord, sCont, sIndex, Monster_Size.Normal)
        {
            my_active_texture = cont.Load<Texture2D>("Enemies/corpsemimic");
            my_idle_texture = cont.Load<Texture2D>("Background/Doodads/corpsepile");
            max_hitPoints = Math.Max(8, 2 * fl_number);
            hitPoints = max_hitPoints;
            min_damage = Math.Max(fl_number / 6, 1);
            max_damage = Math.Max((fl_number / 6)*2, 2);
            dmg_type = Attack.Damage.Slashing;
            
            turns_idle = 5;
            my_Texture = my_idle_texture;
            //SENSORY
            base_sight_range = 3;

            set_senses_to_baseline();

            //OTHER
            my_name = "Corpse Mimic";
            melee_dodge = 5;
            ranged_dodge = 0;
            set_initial_dodge_values();
        }

        public override void Update_Monster(Player pl, Floor fl)
        {
            has_moved = false;
            if (is_player_within(pl, sight_range))
                can_see_player = can_i_see_point(fl, pl.get_my_grid_C());
            else
                can_see_player = false;

            if (!stunned)
            {
                if (can_see_player)
                {
                    if (turns_idle > 2)
                        fl.add_new_popup("Awakens!", Popup.popup_msg_color.Red, my_grid_coords[0]);
                    my_Texture = my_active_texture;
                    turns_idle = 0;
                    ranged_dodge = 10;
                    advance_towards_single_point(pl.get_my_grid_C(), pl, fl, 1, corporeal);

                    if (!has_moved && is_player_within(pl, 1))
                    {
                        fl.addmsg("The Corpse Mimic slashes at you!");
                        Attack dmg = dealDamage();
                        fl.add_effect(dmg_type, pl.get_my_grid_C());
                        pl.take_damage(dmg, fl, "");
                    }
                }
                else
                {
                    turns_idle++;
                    if (turns_idle > 2)
                    {
                        my_Texture = my_idle_texture;
                        ranged_dodge = 0;
                    }
                    else
                        wander(pl, fl, corporeal);
                }
            }

            base.Update_Monster(pl, fl);
        }
    }
}
