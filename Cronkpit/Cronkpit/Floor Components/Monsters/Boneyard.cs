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
    class Boneyard: Monster
    {
        private enum attack_sequence { Biting, Volley, None };
        Texture2D bitey_texture;
        Texture2D normal_texture;
        attack_sequence current_sequence;

        gridCoordinate last_seen_player_loc;
        bool have_i_seen_player;

        int bite_min_damage;
        int bite_max_damage;

        int bone_spear_mindmg;
        int bone_spear_maxdmg;
        Attack.Damage bone_spear_dmgtyp = Attack.Damage.Piercing;
        gridCoordinate c_bspear_target_endp;
        gridCoordinate c_bspear_target_startp;

        int blood_spray_mindmg;
        int blood_spray_maxdmg;
        Attack.Damage blood_spray_dmgtyp = Attack.Damage.Acid;

        int sequence_cooldown;
        int bite_stage;
        int volley_stage;
        bool bite_available;
        bool volley_available;

        public Boneyard(gridCoordinate sGridCoord, ContentManager sCont, int sIndex, bool bossmonster)
            : base(sGridCoord, sCont, sIndex, Monster_Size.Large)
        {
            normal_texture = sCont.Load<Texture2D>("Enemies/boneyard");
            bitey_texture = sCont.Load<Texture2D>("Enemies/boneyard_biting");
            my_Texture = normal_texture;

            if (bossmonster)
            {
                max_hitPoints = 250;
                boss_monster = true;

                min_damage = 1;
                max_damage = 3;
                bone_spear_mindmg = 2;
                bone_spear_maxdmg = 3;
                bite_min_damage = 4;
                bite_max_damage = 6;
                blood_spray_mindmg = 1;
                blood_spray_maxdmg = 1;
            }
            else
            {
                max_hitPoints = 60;

                min_damage = 2;
                max_damage = 5;
                bone_spear_mindmg = 3;
                bone_spear_maxdmg = 4;
                bite_min_damage = 5;
                bite_max_damage = 7;
                blood_spray_mindmg = 1;
                blood_spray_maxdmg = 2;
            }
            hitPoints = max_hitPoints;

            //SENSORY
            sight_range = 6;
            can_hear = true;
            sounds_i_can_hear.Add(SoundPulse.Sound_Types.Player);
            sounds_i_can_hear.Add(SoundPulse.Sound_Types.Voidwraith_Scream);
            listen_threshold.Add(4);
            listen_threshold.Add(1);

            my_name = "Boneyard";
            melee_dodge = 5;
            ranged_dodge = 5;
            set_initial_dodge_values();

            //Other values
            bite_available = true;
            volley_available = true;
            volley_stage = 0;
            bite_stage = 0;
            sequence_cooldown = 6;

            have_i_seen_player = false;
            last_seen_player_loc = my_grid_coords[0];
            current_sequence = attack_sequence.None;
        }

        public void execute_bite(Floor fl, Player pl)
        {
            int pl_x_dir = 0;
            int pl_y_dir = 0;

            gridCoordinate top_left = my_grid_coords[0];
            gridCoordinate bottom_right = my_grid_coords[3];

            int pl_x_c = pl.get_my_grid_C().x;
            int pl_y_c = pl.get_my_grid_C().y;

            //Determine X direction
            if (pl_x_c > top_left.x && pl_x_c > bottom_right.x)
                pl_x_dir = 1;
            else if (pl_x_c < top_left.x && pl_x_c < bottom_right.x)
                pl_x_dir = -1;

            //Determine Y direction
            if (pl_y_c > top_left.y && pl_y_c > bottom_right.y)
                pl_y_dir = 1;
            else if (pl_y_c < top_left.y && pl_y_c < bottom_right.y)
                pl_y_dir = -1;

            int target_index = -1;
            for (int i = 0; i < movement_indexes.Count; i++)
            {
                if (movement_indexes[i][0] == pl_x_dir && movement_indexes[i][1] == pl_y_dir)
                    target_index = i;
            }

            List<gridCoordinate> biting_coords = new List<gridCoordinate>();
            for (int i = 0; i < my_grid_coords.Count; i++)
                biting_coords.Add(new gridCoordinate(my_grid_coords[i].x + (pl_x_dir*2),
                                                     my_grid_coords[i].y + (pl_y_dir*2)));

            fl.add_specific_effect(Floor.specific_effect.Big_Bite, biting_coords[0]);
            for(int i = 0; i < my_grid_coords.Count; i++)
                if (biting_coords[i].x == pl.get_my_grid_C().x && biting_coords[i].y == pl.get_my_grid_C().y)
                {
                    List<string> bparts = new List<string>();
                    if(rGen.Next(2) == 0)
                    {
                        bparts.Add("RLeg");
                        bparts.Add("RArm");
                    }
                    else
                    {
                        bparts.Add("LArm");
                        bparts.Add("LLeg");
                    }

                    fl.addmsg("The boneyard's jaws snap shut, and your bones snap with it!");
                    pl.take_damage(new Attack(Attack.Damage.Crushing, rGen.Next(bite_min_damage, bite_max_damage + 1)), fl, bparts[0]);
                    pl.take_damage(new Attack(Attack.Damage.Slashing, rGen.Next(bite_min_damage, bite_max_damage + 1)), fl, bparts[0]);
                    pl.take_damage(new Attack(Attack.Damage.Crushing, rGen.Next(bite_min_damage, bite_max_damage + 1)), fl, bparts[1]);
                    pl.take_damage(new Attack(Attack.Damage.Slashing, rGen.Next(bite_min_damage, bite_max_damage + 1)), fl, bparts[1]);
                }
        }

        public void bite_alert(Floor fl)
        {
            for (int x = my_grid_coords[0].x - 2; x <= my_grid_coords[1].x + 2; x++)
                for (int y = my_grid_coords[0].y - 2; y <= my_grid_coords[2].y + 2; y++)
                {
                    gridCoordinate target_coord = new gridCoordinate(x, y);
                    if (x > 0 && x < fl.get_fl_size() &&
                       y > 0 && y < fl.get_fl_size() &&
                       fl.is_tile_passable(target_coord) &&
                       !occupies_tile(target_coord))
                        fl.add_specific_effect(Floor.specific_effect.Alert, new gridCoordinate(x, y));
                }
        }

        public void target_bonespear(Floor fl, Player pl)
        {
            int pl_x_dir = 0;
            int pl_y_dir = 0;

            gridCoordinate top_left = my_grid_coords[0];
            gridCoordinate bottom_right = my_grid_coords[3];

            int pl_x_c = pl.get_my_grid_C().x;
            int pl_y_c = pl.get_my_grid_C().y;

            //Determine X direction
            if (pl_x_c > top_left.x && pl_x_c > bottom_right.x)
                pl_x_dir = 1;
            else if (pl_x_c < top_left.x && pl_x_c < bottom_right.x)
                pl_x_dir = -1;

            //Determine Y direction
            if (pl_y_c > top_left.y && pl_y_c > bottom_right.y)
                pl_y_dir = 1;
            else if (pl_y_c < top_left.y && pl_y_c < bottom_right.y)
                pl_y_dir = -1;

            //Okay so these are the easy ways to do it.
            //If these don't work we get messy.
            //Easy way 1.
            if (pl_x_dir == 0)
            {
                int spear_start_xCoord = 0;
                if (pl_x_c == my_grid_coords[0].x)
                    spear_start_xCoord = my_grid_coords[0].x;
                else
                    spear_start_xCoord = my_grid_coords[1].x;

                if (pl_y_dir == 1)
                {
                    c_bspear_target_startp = new gridCoordinate(spear_start_xCoord, my_grid_coords[3].y + 1);
                    c_bspear_target_endp = new gridCoordinate(spear_start_xCoord, Math.Min(my_grid_coords[3].y + 6, fl.get_fl_size() - 1));
                }
                else
                {
                    c_bspear_target_startp = new gridCoordinate(spear_start_xCoord, my_grid_coords[0].y - 1);
                    c_bspear_target_endp = new gridCoordinate(spear_start_xCoord, Math.Max(my_grid_coords[0].y - 6, 0));
                }
            }
            //Easy way 2.
            if (pl_y_dir == 0)
            {
                int spear_start_yCoord = 0;
                if (pl_y_c == my_grid_coords[0].y)
                    spear_start_yCoord = my_grid_coords[0].y;
                else
                    spear_start_yCoord = my_grid_coords[2].y;

                if (pl_x_dir == 1)
                {
                    c_bspear_target_startp = new gridCoordinate(my_grid_coords[1].x + 1, spear_start_yCoord);
                    c_bspear_target_endp = new gridCoordinate(Math.Min(my_grid_coords[1].x + 6, fl.get_fl_size() - 1), spear_start_yCoord);
                }
                else
                {
                    c_bspear_target_startp = new gridCoordinate(my_grid_coords[0].x - 1, spear_start_yCoord);
                    c_bspear_target_endp = new gridCoordinate(Math.Max(my_grid_coords[0].x - 6, 0), spear_start_yCoord);
                }
            }

            //Now, we raycast to see if we can get a good ray that hits the player.
            List<gridCoordinate> endPoints = new List<gridCoordinate>();
            //We first base this on a single coordinate that corresponds to one of the corners of the boneyard
            //for example if the player is above and to the left of the boneyard, we start out with this:
            /* [M] = Monster / [X] = target / [_] = blank spot
             * 
             * [X][_][_]
             * [_][M][M]
             * [_][M][M]
             */
            if (pl_x_dir != 0 && pl_y_dir != 0)
            {
                int target_index = -1;
                if (pl_x_dir == -1 && pl_y_dir == -1)
                    target_index = 0;
                else if (pl_x_dir == 1 && pl_y_dir == -1)
                    target_index = 1;
                else if (pl_x_dir == -1 && pl_y_dir == 1)
                    target_index = 2;
                else if (pl_x_dir == 1 && pl_y_dir == 1)
                    target_index = 3;

                gridCoordinate start_coord = new gridCoordinate(my_grid_coords[target_index].x + pl_x_dir,
                                                                my_grid_coords[target_index].y + pl_y_dir);
                c_bspear_target_startp = start_coord;
                //Then we make a list of endpoints based on that. It will need two for loops;
                //One for the X-axis and one for the Y-axis
                //5/5 offset
                endPoints.Add(new gridCoordinate(start_coord.x + (pl_x_dir*5), start_coord.y + (pl_y_dir*5)));
                //5/4 offsets
                endPoints.Add(new gridCoordinate(start_coord.x + (pl_x_dir * 4), start_coord.y + (pl_y_dir * 5)));
                endPoints.Add(new gridCoordinate(start_coord.x + (pl_x_dir * 5), start_coord.y + (pl_y_dir * 4)));
                //5/3 offsets
                endPoints.Add(new gridCoordinate(start_coord.x + (pl_x_dir * 3), start_coord.y + (pl_y_dir * 5)));
                endPoints.Add(new gridCoordinate(start_coord.x + (pl_x_dir * 5), start_coord.y + (pl_y_dir * 3)));
                //5/2 offsets
                endPoints.Add(new gridCoordinate(start_coord.x + (pl_x_dir * 2), start_coord.y + (pl_y_dir * 5)));
                endPoints.Add(new gridCoordinate(start_coord.x + (pl_x_dir * 5), start_coord.y + (pl_y_dir * 2)));
                //5/1 offsets
                endPoints.Add(new gridCoordinate(start_coord.x + (pl_x_dir * 1), start_coord.y + (pl_y_dir * 5)));
                endPoints.Add(new gridCoordinate(start_coord.x + (pl_x_dir * 5), start_coord.y + (pl_y_dir * 1)));
                //I avoided doing this in a for loop because I want the boneyard to prioritize shooting the spear
                //diagonally if it can and doing it with loops would shoot it either vertically or horizontally.
                
                List<VisionRay> bspear_rays = new List<VisionRay>();
                for (int i = 0; i < endPoints.Count; i++)
                    bspear_rays.Add(new VisionRay(start_coord, endPoints[i]));

                while (bspear_rays.Count > 0)
                {
                    bool found_player = false;
                    for (int i = 0; i < bspear_rays.Count; i++)
                    {
                        bool remove = false;
                        int c_x_coord = (int)bspear_rays[i].my_current_position.X / 32;
                        int c_y_coord = (int)bspear_rays[i].my_current_position.Y / 32;
                        gridCoordinate c_coord = new gridCoordinate(c_x_coord, c_y_coord);

                        if (!fl.is_tile_passable(c_coord) || bspear_rays[i].is_at_end())
                            remove = true;

                        if (pl.get_my_grid_C().x == c_coord.x && pl.get_my_grid_C().y == c_coord.y)
                        {
                            c_bspear_target_endp = new gridCoordinate((int)bspear_rays[i].my_end_position.X / 32,
                                                                      (int)bspear_rays[i].my_end_position.Y / 32);
                            found_player = true;
                        }

                        if (remove)
                            bspear_rays.RemoveAt(i);
                        else
                            bspear_rays[i].update();

                        if (found_player)
                            bspear_rays.Clear();
                    }
                }
            }

            Projectile prj = new Projectile(c_bspear_target_startp, c_bspear_target_endp, Projectile.projectile_type.Blank,
                                            ref cont, true, Scroll.Atk_Area_Type.piercingBolt, false);
            prj.set_special_anim(Projectile.special_anim.Alert);
            prj.set_damage_range(0, 0);
            prj.set_damage_type(bone_spear_dmgtyp);
            fl.create_new_projectile(prj);

        }

        public void fire_bonespear(Floor fl)
        {
            Projectile boneSpear = new Projectile(c_bspear_target_startp, c_bspear_target_endp,
                                                  Projectile.projectile_type.Bonespear, ref cont,
                                                  true, Scroll.Atk_Area_Type.piercingBolt);
            boneSpear.set_damage_type(bone_spear_dmgtyp);
            boneSpear.set_damage_range(bone_spear_mindmg, bone_spear_maxdmg);
            fl.create_new_projectile(boneSpear);
        }

        public void bloodspray(Floor fl, Player pl)
        {
            int pl_x_dir = 0;
            int pl_y_dir = 0;

            gridCoordinate top_left = my_grid_coords[0];
            gridCoordinate bottom_right = my_grid_coords[3];

            int pl_x_c = pl.get_my_grid_C().x;
            int pl_y_c = pl.get_my_grid_C().y;

            //Determine X direction
            if (pl_x_c > top_left.x && pl_x_c > bottom_right.x)
                pl_x_dir = 1;
            else if (pl_x_c < top_left.x && pl_x_c < bottom_right.x)
                pl_x_dir = -1;

            //Determine Y direction
            if (pl_y_c > top_left.y && pl_y_c > bottom_right.y)
                pl_y_dir = 1;
            else if (pl_y_c < top_left.y && pl_y_c < bottom_right.y)
                pl_y_dir = -1;

            List<gridCoordinate> potential_spray_targets = new List<gridCoordinate>();
            List<gridCoordinate> blood_spray_targets = new List<gridCoordinate>();

            if (pl_x_dir == 0)
            {
                int y_coord = top_left.y;
                if (pl_y_dir > 0)
                    y_coord = bottom_right.y;

                for (int x = top_left.x - 1; x <= bottom_right.x + 1; x++)
                    for (int y = y_coord + (pl_y_dir * 2); y != y_coord + (pl_y_dir * 4); y += pl_y_dir)
                        potential_spray_targets.Add(new gridCoordinate(x, y));
            }
            else if (pl_y_dir == 0)
            {
                int x_coord = top_left.x;
                if (pl_x_dir > 0)
                    x_coord = bottom_right.x;

                for (int x = x_coord + (pl_x_dir * 2); x != x_coord + (pl_x_dir * 4); x += pl_x_dir)
                    for (int y = top_left.y - 1; y <= bottom_right.y + 1; y++)
                        potential_spray_targets.Add(new gridCoordinate(x, y));
            }
            else
            {
                for(int i = 0; i < my_grid_coords.Count; i++)
                    potential_spray_targets.Add(new gridCoordinate(my_grid_coords[i].x + (pl_x_dir*3),
                                                                   my_grid_coords[i].y + (pl_y_dir*3)));

                int upper_x = 0;
                int lower_x = 0;
                if (pl_x_dir == 1)
                {
                    upper_x = 0;
                    lower_x = 2;
                }
                else if (pl_x_dir == -1)
                {
                    upper_x = 1;
                    lower_x = 3;
                }

                int left_y = 0;
                int right_y = 0;
                if (pl_y_dir == 1)
                {
                    left_y = 0;
                    right_y = 1;
                }
                else if (pl_y_dir == -1)
                {
                    left_y = 2;
                    right_y = 3;
                }

                //The above code gives us a 2x2 zone. For this, we want to extend the zone by one
                //x tile and y tile in the opposite direction of where the player is relative to the boneyard
                //That's why the above code picks out the 2 coordinates from the x axis and y axis, because 
                potential_spray_targets.Add(new gridCoordinate(potential_spray_targets[upper_x].x + (pl_x_dir*-1),
                                                                   potential_spray_targets[upper_x].y));
                potential_spray_targets.Add(new gridCoordinate(potential_spray_targets[lower_x].x + (pl_x_dir*-1),
                                                               potential_spray_targets[lower_x].y));
                potential_spray_targets.Add(new gridCoordinate(potential_spray_targets[left_y].x,
                                                                   potential_spray_targets[left_y].y + (pl_y_dir*-1)));
                potential_spray_targets.Add(new gridCoordinate(potential_spray_targets[right_y].x,
                                                               potential_spray_targets[right_y].y + (pl_y_dir*-1)));

            }

            int potential_targets = 3;
            //for (int i = 0; i < potential_spray_targets.Count; i++)
                //fl.set_tile_aura(potential_spray_targets[i], Tile.Aura.SmellTarget);
            for (int i = 0; i < potential_targets; i++)
            {
                bool found_valid_target = false;
                while (!found_valid_target && potential_spray_targets.Count > 0)
                {
                    int chosen_coord = rGen.Next(potential_spray_targets.Count);
                    if (can_i_see_point(fl, potential_spray_targets[chosen_coord], VisionRay.fineness.Roughest))
                    {
                        blood_spray_targets.Add(potential_spray_targets[chosen_coord]);
                        found_valid_target = true;
                    }
                    potential_spray_targets.RemoveAt(chosen_coord);
                }
            }


            for (int i = 0; i < blood_spray_targets.Count; i++)
            {
                Projectile bspray = new Projectile(randomly_chosen_personal_coord(), blood_spray_targets[i],
                                                   Projectile.projectile_type.Bloody_AcidCloud, ref cont,
                                                   true, Scroll.Atk_Area_Type.cloudAOE, true);
                bspray.set_damage_type(blood_spray_dmgtyp);
                bspray.set_damage_range(blood_spray_mindmg, blood_spray_maxdmg);
                bspray.set_special_anim(Projectile.special_anim.BloodAcid);
                bspray.set_AOE_size(1);
                fl.create_new_projectile(bspray);
            }
        }

        public override void Update_Monster(Player pl, Floor fl)
        {
            heal_near_altar(fl);

            if (is_player_within(pl, sight_range))
                can_see_player = can_i_see_point(fl, pl.get_my_grid_C());
            else
                can_see_player = false;

            has_moved = false;
            if (stunned_turns_remaining == 0)
            {
                if (can_see_player)
                {
                    if (sequence_cooldown == 0)
                    {
                        if (!bite_available && !volley_available)
                        {
                            bite_available = true;
                            volley_available = true;
                            bite_stage = 0;
                            volley_stage = 0;
                        }

                        if (bite_available && volley_available)
                        {
                            if (rGen.Next(2) == 0)
                            {
                                current_sequence = attack_sequence.Biting;
                                bite_available = false;
                            }
                            else
                            {
                                current_sequence = attack_sequence.Volley;
                                volley_available = false;
                            }
                            sequence_cooldown = 4;
                        }
                        else
                        {
                            if (!bite_available)
                            {
                                current_sequence = attack_sequence.Volley;
                                volley_available = false;
                            }
                            else
                            {
                                current_sequence = attack_sequence.Biting;
                                bite_available = false;
                            }
                            sequence_cooldown = 4;
                        }
                    }

                    switch (current_sequence)
                    {
                        case attack_sequence.None:
                            if(is_player_within(pl, 1))
                            {
                                fl.addmsg("The Boneyard slashes at you!");
                                Attack dmg = dealDamage();
                                fl.add_effect(dmg_type, pl.get_my_grid_C());
                                pl.take_damage(dmg, fl, "");
                                sequence_cooldown--;
                            }
                            else
                            {
                                advance_towards_single_point(pl.get_my_grid_C(), pl, fl, 1, corporeal);
                                if (!has_moved)
                                {
                                    Projectile bs = new Projectile(randomly_chosen_personal_coord(), pl.get_my_grid_C(),
                                                                   Projectile.projectile_type.Bonespear, ref cont, true,
                                                                   Scroll.Atk_Area_Type.singleTile);
                                    bs.set_damage_range(bone_spear_mindmg, bone_spear_maxdmg);
                                    bs.set_damage_type(bone_spear_dmgtyp);
                                    fl.create_new_projectile(bs);
                                }
                                else
                                    sequence_cooldown--;
                            }
                            break;
                        case attack_sequence.Biting:
                            switch (bite_stage)
                            {
                                case 0:
                                    my_Texture = bitey_texture;
                                    fl.add_new_popup("Roars!", Popup.popup_msg_color.Red, my_center_coordinate());
                                    bloodspray(fl, pl);
                                    bite_stage++;
                                    break;
                                case 1:
                                    bite_alert(fl);
                                    bite_stage++;
                                    break;
                                case 2:
                                    //Bite goes off
                                    execute_bite(fl, pl);
                                    bite_stage = 0;
                                    my_Texture = normal_texture;
                                    current_sequence = attack_sequence.None;
                                    break;
                            }
                            break;
                        case attack_sequence.Volley:
                            switch (volley_stage)
                            {
                                case 0:
                                    //target player
                                    target_bonespear(fl, pl);
                                    fl.add_specific_effect(Floor.specific_effect.Warning_Bracket, pl.get_my_grid_C());
                                    volley_stage++;
                                    break;
                                case 1:
                                    //shoot spear
                                    fire_bonespear(fl);
                                    //target player
                                    target_bonespear(fl, pl);
                                    volley_stage++;
                                    break;
                                case 2:
                                    //shoot spear
                                    fire_bonespear(fl);
                                    //target player
                                    target_bonespear(fl, pl);
                                    volley_stage++;
                                    break;
                                case 3:
                                    //shoot spear
                                    fire_bonespear(fl);
                                    volley_stage = 0;
                                    current_sequence = attack_sequence.None;
                                    break;
                            }
                            break;
                    }
                }
                else if (!can_see_player && have_i_seen_player)
                {
                    advance_towards_single_point(last_seen_player_loc, pl, fl, 0, corporeal);
                    if (occupies_tile(last_seen_player_loc))
                    {
                        last_seen_player_loc = my_grid_coords[0];
                        have_i_seen_player = false;
                    }
                }
                else if (!can_see_player && !have_i_seen_player && heard_something)
                {
                    follow_path_to_sound(fl, pl);
                }
            }
            else
                stunned_turns_remaining--;
        }
    }
}
