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
    class RedKnight: Monster
    {
        int javelin_range;
        int jav_min_dmg;
        int jav_max_dmg;
        Attack.Damage javelin_damage_type;

        int cleave_min_dmg;
        int cleave_max_dmg;
        int cleave_cooldown;

        int pwr_strike_min_dmg;
        int pwr_strike_max_dmg;
        int pwr_strike_cooldown;

        public RedKnight(gridCoordinate sGridCoord, ContentManager sCont, int sIndex)
            : base(sGridCoord, sCont, sIndex)
        {
            my_Texture = cont.Load<Texture2D>("Enemies/redKnightIdle");
            can_hear = true;
            hitPoints = 45;
            min_damage = 2;
            max_damage = 5;
            dmg_type = Attack.Damage.Slashing;
            wound_type = wound.Wound_Type.Open;
            can_melee_attack = true;
            active = false;

            //SENSORY
            listen_threshold = 5;
            sight_range = 2;

            //OTHER
            speed_denominator = 1;
            my_name = "Red Knight";

            //Red Knight specials
            javelin_range = 5;
            jav_min_dmg = 1;
            jav_max_dmg = 4;
            javelin_damage_type = Attack.Damage.Piercing;

            //cleave has a cooldown of 3 turns
            cleave_min_dmg = 2;
            cleave_max_dmg = 3;
            cleave_cooldown = 0;

            //power strike has a 5 turn cooldown
            pwr_strike_min_dmg = 3;
            pwr_strike_max_dmg = 7;
            pwr_strike_cooldown = 0;

            melee_dodge = 5;
            ranged_dodge = 75;
        }

        public Attack generate_javelin_attack()
        {
            int dmgVal = rGen.Next(jav_min_dmg, (jav_max_dmg + 1));
            return new Attack(javelin_damage_type, new wound(wound_type, dmgVal));
        }

        public Attack generate_cleave_attack()
        {
            int dmgVal = rGen.Next(cleave_min_dmg, (cleave_max_dmg + 1));
            return new Attack(dmg_type, new wound(wound_type, dmgVal));
        }

        public Attack generate_power_strike_attack()
        {
            int dmgVal = rGen.Next(pwr_strike_min_dmg, (pwr_strike_max_dmg + 1));
            return new Attack(dmg_type, new wound(wound_type, dmgVal));
        }

        public void execute_power_strike(Player pl, Floor fl, int x_difference, int y_difference)
        {
            int x_incr = 0;
            if (x_difference > 0)
                x_incr = 1;
            else if (x_difference < 0)
                x_incr = -1;

            if (y_difference == 0)
            {
                int y_value = my_grid_coord.y;
                for (int i = 0; i < 2; i++)
                {
                    int x_value = my_grid_coord.x + (x_incr * (i+1));
                    gridCoordinate target_location = new gridCoordinate(x_value, y_value);
                    if (pl.get_my_grid_C().x == target_location.x && pl.get_my_grid_C().y == target_location.y)
                    {
                        fl.addmsg("The Red Knight winds up, then unleashes an incredible attack!");
                        pl.take_damage(generate_power_strike_attack(), ref fl);
                    }
                    else
                    {
                        int mon_ID;
                        fl.is_monster_here(target_location, out mon_ID);
                        if (mon_ID != -1)
                        {
                            int dmg_value = generate_power_strike_attack().get_assoc_wound().severity * 2;
                            fl.add_new_popup("- " + dmg_value.ToString(), Popup.popup_msg_color.Red, target_location);
                            fl.damage_monster(dmg_value, mon_ID, true);
                        }
                    }
                    if(!fl.is_tile_opaque(target_location))
                        fl.add_specific_effect(Floor.specific_effect.Power_Strike, target_location);
                }
            }

            int y_incr = 0;
            if (y_difference > 0)
                y_incr = 1;
            else if (y_difference < 0)
                y_incr = -1;

            if (x_difference == 0)
            {
                int x_value = my_grid_coord.x;
                for (int i = 0; i < 2; i++)
                {
                    int y_value = my_grid_coord.y + (y_incr * (i + 1));
                    gridCoordinate target_location = new gridCoordinate(x_value, y_value);
                    if (pl.get_my_grid_C().x == target_location.x && pl.get_my_grid_C().y == target_location.y)
                    {
                        fl.addmsg("The Red Knight winds up, then unleashes an incredible attack!");
                        pl.take_damage(generate_power_strike_attack(), ref fl);
                    }
                    else
                    {
                        int mon_ID;
                        fl.is_monster_here(target_location, out mon_ID);
                        if (mon_ID != -1)
                        {
                            int dmg_value = generate_power_strike_attack().get_assoc_wound().severity * 2;
                            fl.add_new_popup("- " + dmg_value.ToString(), Popup.popup_msg_color.Red, target_location);
                            fl.damage_monster(dmg_value, mon_ID, true);
                        }
                    }
                    if (!fl.is_tile_opaque(target_location))
                        fl.add_specific_effect(Floor.specific_effect.Power_Strike, target_location);
                }
            }
        }

        public void execute_cleave_attack(Player pl, Floor fl, int x_difference, int y_difference)
        {
            gridCoordinate playerCoord = new gridCoordinate(pl.get_my_grid_C());
            fl.addmsg("The Red Knight swings its sword in a wide arc!");
            for(int i = 0; i < 3; i++)
                pl.take_damage(generate_cleave_attack(), ref fl);
            fl.add_specific_effect(Floor.specific_effect.Cleave, playerCoord);

            if (x_difference == -1)
            {
                if (y_difference == 1)
                {
                    cleave_splash_damage(fl, new gridCoordinate(playerCoord.x + 1, playerCoord.y));
                    cleave_splash_damage(fl, new gridCoordinate(playerCoord.x, playerCoord.y - 1));
                }
                else if (y_difference == 0)
                {
                    cleave_splash_damage(fl, new gridCoordinate(playerCoord.x, playerCoord.y - 1));
                    cleave_splash_damage(fl, new gridCoordinate(playerCoord.x, playerCoord.y + 1));
                }
                else if (y_difference == -1)
                {
                    cleave_splash_damage(fl, new gridCoordinate(playerCoord.x + 1, playerCoord.y));
                    cleave_splash_damage(fl, new gridCoordinate(playerCoord.x, playerCoord.y + 1));
                }
            }
            else if (x_difference == 1)
            {
                if (y_difference == 1)
                {
                    cleave_splash_damage(fl, new gridCoordinate(playerCoord.x - 1, playerCoord.y));
                    cleave_splash_damage(fl, new gridCoordinate(playerCoord.x, playerCoord.y - 1));
                }
                else if (y_difference == 0)
                {
                    cleave_splash_damage(fl, new gridCoordinate(playerCoord.x, playerCoord.y - 1));
                    cleave_splash_damage(fl, new gridCoordinate(playerCoord.x, playerCoord.y + 1));
                }
                else if (y_difference == -1)
                {
                    cleave_splash_damage(fl, new gridCoordinate(playerCoord.x - 1, playerCoord.y));
                    cleave_splash_damage(fl, new gridCoordinate(playerCoord.x, playerCoord.y + 1));
                }
            }
            else if (x_difference == 0)
            {
                cleave_splash_damage(fl, new gridCoordinate(playerCoord.x - 1, playerCoord.y));
                cleave_splash_damage(fl, new gridCoordinate(playerCoord.x + 1, playerCoord.y));
            }
        }

        public void cleave_splash_damage(Floor fl, gridCoordinate target_location)
        {
            int mon_ID;
            fl.is_monster_here(target_location, out mon_ID);
            if (mon_ID != -1)
            {
                for (int i = 0; i < 3; i++)
                {
                    int dmg_value = generate_cleave_attack().get_assoc_wound().severity * 2;
                    fl.add_new_popup("- " + dmg_value.ToString(), Popup.popup_msg_color.Red, target_location);
                    fl.damage_monster(dmg_value, mon_ID, true);
                }
            }
            if (!fl.is_tile_opaque(target_location))
                fl.add_specific_effect(Floor.specific_effect.Cleave, target_location);
        }

        public void execute_melee_attack(Floor fl, Player pl)
        {
            fl.addmsg("The Red Knight swings its blade at you!");
            fl.add_effect(dmg_type, pl.get_my_grid_C());
            Attack dmg = dealDamage();
            pl.take_damage(dmg, ref fl);
        }

        public void set_to_activeTexture()
        {
            my_Texture = cont.Load<Texture2D>("Enemies/redKnight");
        }

        public void throw_javelin(Player pl, Floor fl)
        {
            if (is_player_within_diamond(pl, javelin_range))
            {
                bool temp_sight = can_see_player;
                can_see_player = false;
                fl.sight_pulse_raycast(my_grid_coord, pl, this, javelin_range);

                if (can_see_player)
                {
                    fl.create_new_projectile(new Projectile(my_grid_coord, pl.get_my_grid_C(), Projectile.projectile_type.Javelin, ref cont));
                    fl.addmsg("The Red Knight hurls a javelin at you!");
                    fl.add_effect(javelin_damage_type, pl.get_my_grid_C());
                    Attack dmg = generate_javelin_attack();
                    pl.take_damage(dmg, ref fl);
                }

                can_see_player = temp_sight;
            }
        }

        public override void Update_Monster(Player pl, Floor fl)
        {
            //First, check if the knight can see the player.
            //if it can't see the player, it can't use any of its mega attacks
            //it uses power strike, then cleave.
            //then, move the knight towards the player if it can move towards the player.
            //if it can't, it throws a javelin at the player if it can hear them.
            can_see_player = false;
            look_for_player(fl, pl, sight_range);

            int pl_x_difference = pl.get_my_grid_C().x - my_grid_coord.x;
            int pl_y_difference = pl.get_my_grid_C().y - my_grid_coord.y;

            if (pwr_strike_cooldown > 0)
                pwr_strike_cooldown--;
            if (cleave_cooldown > 0)
                cleave_cooldown--;

            if (active)
            {
                if (can_see_player)
                {
                    if ((pl_x_difference == 0 || pl_y_difference == 0) &&
                        is_player_within(pl, 2) && pwr_strike_cooldown == 0)
                    {
                        //power strike
                        execute_power_strike(pl, fl, pl_x_difference, pl_y_difference);
                        pwr_strike_cooldown = 5;
                    }
                    else
                    {
                        //cleave or melee attack
                        if (is_player_within(pl, 1))
                        {
                            if (cleave_cooldown == 0)
                            {
                                //cleave
                                execute_cleave_attack(pl, fl, pl_x_difference, pl_y_difference);
                                cleave_cooldown = 3;
                            }
                            else
                            {
                                if (is_player_within(pl, 1))
                                    execute_melee_attack(fl, pl);
                            }
                        }
                        else
                        {
                            //chase the player, or throw a jav if it can't
                            if (speed_numerator < speed_denominator)
                            {
                                advance_towards_single_point(pl.get_my_grid_C(), pl, fl, 1);
                            }
                            else
                            {
                                throw_javelin(pl, fl);
                            }
                        }
                    }
                }
                else
                {
                    if (heard_something && speed_numerator < speed_denominator)
                    {
                        follow_path_to_sound(fl, pl);
                    }
                    else if (heard_something && speed_numerator == speed_denominator)
                    {
                        //throw jav
                        throw_javelin(pl, fl);
                    }
                    //if heard something and can move, move
                    //else if heard something, throw javelin
                }

                if (speed_numerator == speed_denominator)
                    speed_numerator = 0;
                else
                    speed_numerator++;
            }
            else
            {
                if (heard_something)
                {
                    listen_threshold = 2;
                    active = true;
                    fl.addmsg("The Red Knight awakens with an unearthly moan!");
                    set_to_activeTexture();
                    fl.add_new_popup("Awakens!", Popup.popup_msg_color.Red, my_grid_coord);
                }
            }
        }
    }
}
