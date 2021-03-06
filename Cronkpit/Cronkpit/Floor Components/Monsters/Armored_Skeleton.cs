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
    class Armored_Skeleton: Monster
    {
        public enum Armor_Skeleton_Weapon { Greatsword, Crossbow, Magic, Halberd };
        Armor_Skeleton_Weapon my_weapon_type;
        public gridCoordinate last_seen_player_at;
        bool have_i_seen_player;
        
        //Flamebolt
        int flamebolt_mana_cost = 30;
        int flamebolt_min_damage = 2;
        int flamebolt_max_damage = 7;
        Attack.Damage flamebolt_dmg_type = Attack.Damage.Fire;
        
        //AcidCloud
        int acidcloud_mana_cost = 50;
        int acidcloud_min_damage = 1;
        int acidcloud_max_damage = 2;
        int acid_cloud_cooldown;
        Attack.Damage acidcloud_dmg_type = Attack.Damage.Acid;

        int attack_range = 4;

        public Armored_Skeleton(gridCoordinate sGridCoord, ContentManager sCont, int sIndex, Armor_Skeleton_Weapon wType)
            : base(sGridCoord, sCont, sIndex, Monster_Size.Normal)
        {
            my_weapon_type = wType;
            switch (my_weapon_type)
            {
                case Armor_Skeleton_Weapon.Greatsword:
                    my_Texture = cont.Load<Texture2D>("Enemies/armored_skeleton_swordsman");
                    min_damage = 3;
                    max_damage = 5;
                    dmg_type = Attack.Damage.Slashing;
                    break;
                case Armor_Skeleton_Weapon.Crossbow:
                    my_Texture = cont.Load<Texture2D>("Enemies/armored_skeleton_crossbowman");
                    min_damage = 2;
                    max_damage = 5;
                    dmg_type = Attack.Damage.Piercing;
                    break;
                case Armor_Skeleton_Weapon.Halberd:
                    my_Texture = cont.Load<Texture2D>("Enemies/armored_skeleton_halberdier");
                    min_damage = 2;
                    max_damage = 4;
                    dmg_type = Attack.Damage.Piercing;
                    break;
                case Armor_Skeleton_Weapon.Magic:
                    my_Texture = cont.Load<Texture2D>("Enemies/armored_skeleton_elementalmage");
                    min_damage = 1;
                    max_damage = 1;
                    dmg_type = Attack.Damage.Crushing;
                    break;
            }

            max_hitPoints = 18;
            hitPoints = max_hitPoints;
            armorPoints = 20;

            last_seen_player_at = new gridCoordinate(my_grid_coords[0]);

            //SENSORY
            base_sight_range = 5;
            can_hear = true;
            sounds_i_can_hear.Add(SoundPulse.Sound_Types.Player);
            sounds_i_can_hear.Add(SoundPulse.Sound_Types.Voidwraith_Scream);
            base_listen_threshold.Add(7);
            base_listen_threshold.Add(1);

            set_senses_to_baseline();

            //OTHER
            my_name = "Skeleton";
            melee_dodge = 15;
            ranged_dodge = 15;
            armor_effectiveness = 70;
            acid_cloud_cooldown = 0;
            set_initial_dodge_values();
            smart_monster = true;
        }

        public void halberdier_linear_spear_stab(Player pl, Floor fl, int x_difference, int y_difference)
        {
            int x_incr = 0;
            if (x_difference > 0)
                x_incr = 1;
            else if (x_difference < 0)
                x_incr = -1;

            if (y_difference == 0)
            {
                int y_value = my_grid_coords[0].y;
                for (int i = 0; i < 2; i++)
                {
                    int x_value = my_grid_coords[0].x + (x_incr * (i + 1));
                    gridCoordinate target_location = new gridCoordinate(x_value, y_value);
                    if (pl.get_my_grid_C().x == target_location.x && pl.get_my_grid_C().y == target_location.y)
                    {
                        fl.addmsg("The Skeleton thrusts its spear forward in a heavy stab!");
                        int dmgVal = rGen.Next(min_damage, (max_damage + 1));
                        Attack dmg = new Attack(dmg_type, dmgVal);
                        pl.take_damage(dmg, fl, "");
                    }
                    else
                    {
                        int mon_ID;
                        fl.is_monster_here(target_location, out mon_ID);
                        if (mon_ID != -1)
                        {
                            int dmg_value = rGen.Next(min_damage, (max_damage + 1)) * 2;
                            fl.add_new_popup("- " + dmg_value.ToString(), Popup.popup_msg_color.Red, target_location);
                            fl.damage_monster_single_atk(new Attack(dmg_type, dmg_value), null, mon_ID, true, true);
                        }
                    }
                    if (fl.is_tile_passable(target_location))
                        fl.add_effect(dmg_type, target_location);
                }
            }

            int y_incr = 0;
            if (y_difference > 0)
                y_incr = 1;
            else if (y_difference < 0)
                y_incr = -1;

            if (x_difference == 0)
            {
                int x_value = my_grid_coords[0].x;
                for (int i = 0; i < 2; i++)
                {
                    int y_value = my_grid_coords[0].y + (y_incr * (i + 1));
                    gridCoordinate target_location = new gridCoordinate(x_value, y_value);
                    if (pl.get_my_grid_C().x == target_location.x && pl.get_my_grid_C().y == target_location.y)
                    {
                        fl.addmsg("The Skeleton thrusts its spear forward in a heavy stab!");
                        int dmgVal = rGen.Next(min_damage, (max_damage + 1));
                        Attack dmg = new Attack(dmg_type, dmgVal);
                        pl.take_damage(dmg, fl, "");
                    }
                    else
                    {
                        int mon_ID;
                        fl.is_monster_here(target_location, out mon_ID);
                        if (mon_ID != -1)
                        {
                            int dmg_value = rGen.Next(min_damage, (max_damage + 1)) * 2;
                            fl.add_new_popup("- " + dmg_value.ToString(), Popup.popup_msg_color.Red, target_location);
                            fl.damage_monster_single_atk(new Attack(dmg_type, dmg_value), null, mon_ID, true, true);
                        }
                    }
                    if (fl.is_tile_passable(target_location))
                        fl.add_effect(dmg_type, target_location);
                }
            }
        }

        public void cast_acid_cloud(Floor fl, gridCoordinate target_GC)
        {
            Projectile prj = new Projectile(my_grid_coords[0], target_GC,
                                                                Projectile.projectile_type.AcidCloud,
                                                                ref cont, true, Scroll.Atk_Area_Type.cloudAOE);
            prj.set_AOE_size(3);
            prj.set_damage_range(acidcloud_min_damage, acidcloud_max_damage);
            prj.set_damage_type(acidcloud_dmg_type);
            fl.create_new_projectile(prj);
            fl.consume_mana(acidcloud_mana_cost);
            acid_cloud_cooldown = 5;
        }

        public void cast_flamebolt(Floor fl, gridCoordinate target_GC)
        {
            Projectile prj = new Projectile(my_grid_coords[0], target_GC, Projectile.projectile_type.Flamebolt,
                                            ref cont, true, Scroll.Atk_Area_Type.singleTile);
            prj.set_damage_range(flamebolt_min_damage, flamebolt_max_damage);
            prj.set_damage_type(flamebolt_dmg_type);
            fl.create_new_projectile(prj);
            fl.consume_mana(flamebolt_mana_cost);
        }

        public void fire_crossbow(Floor fl, gridCoordinate target_GC)
        {
            Projectile prj = new Projectile(my_grid_coords[0], target_GC, Projectile.projectile_type.Crossbow_Bolt,
                                            ref cont, true, Scroll.Atk_Area_Type.singleTile);
            prj.set_damage_range(min_damage, max_damage);
            prj.set_damage_type(dmg_type);
            fl.create_new_projectile(prj);
        }

        public void melee_attack(Floor fl, Player pl, string msg)
        {
            int wounds = rGen.Next(min_damage, max_damage + 1);
            Attack dmg = new Attack(dmg_type, wounds);
            pl.take_damage(dmg, fl, "");
            fl.add_effect(dmg_type, pl.get_my_grid_C());
            fl.addmsg(msg);
        }

        public override void Update_Monster(Player pl, Floor fl)
        {
            if (acid_cloud_cooldown > 0)
                acid_cloud_cooldown--;

            heal_near_altar(fl);
            has_moved = false;

            if (is_player_within(pl, sight_range))
                can_see_player = can_i_see_point(fl, pl.get_my_grid_C());
            else
                can_see_player = false;

            if (!stunned)
            {
                if (can_see_player)
                {
                    have_i_seen_player = true;
                    last_seen_player_at = pl.get_my_grid_C();

                    //3 different basic movement patterns.
                    //Swordsman + Spearman - try to get within melee/"melee" range.
                    //for the spearman it's basically like the Red Knight's power attack ability.
                    //Crossbowman - try to get within a + pattern. Range 4.
                    //Mage - try to get within a diamond. Range 4.
                    //Mage uses acid cloud > firebolt II

                    switch (my_weapon_type)
                    {
                        case Armor_Skeleton_Weapon.Halberd:
                            if (is_player_within_diamond(pl, 2))
                            {
                                int pl_x_difference = pl.get_my_grid_C().x - my_grid_coords[0].x;
                                int pl_y_difference = pl.get_my_grid_C().y - my_grid_coords[0].y;

                                if (pl_x_difference == 0 || pl_y_difference == 0)
                                    //stab!
                                    halberdier_linear_spear_stab(pl, fl, pl_x_difference, pl_y_difference);
                                else
                                {
                                    melee_attack(fl, pl, "The Skeleton slashes its halberd at you!");
                                }
                            }
                            else
                                advance_towards_single_point(pl.get_my_grid_C(), pl, fl, 1, corporeal);
                            break;
                        case Armor_Skeleton_Weapon.Greatsword:
                            if (is_player_within(pl, 1))
                                melee_attack(fl, pl, "The Skeleton slashes at you with its sword!");
                            else
                                advance_towards_single_point(pl.get_my_grid_C(), pl, fl, 1, corporeal);
                            break;
                        case Armor_Skeleton_Weapon.Crossbow:
                            gridCoordinate player_grid_coord = pl.get_my_grid_C();
                            int x_move = 0;
                            int y_move = 0;
                            //Figure out the x difference and the y difference.
                            //It tries to get the shortest one to 0 and the other one to
                            //+/- 4.
                            int x_difference = positive_difference(player_grid_coord.x, my_grid_coords[0].x);
                            int y_difference = positive_difference(player_grid_coord.y, my_grid_coords[0].y);

                            if (x_difference < y_difference)
                            {
                                if (my_grid_coords[0].x < player_grid_coord.x)
                                    x_move = 1;
                                else if (my_grid_coords[0].x > player_grid_coord.x)
                                    x_move = -1;

                                if (player_grid_coord.y > my_grid_coords[0].y + attack_range)
                                    y_move = 1;
                                else if (player_grid_coord.y < my_grid_coords[0].y - attack_range)
                                    y_move = -1;
                            }
                            else
                            {
                                if (my_grid_coords[0].y < player_grid_coord.y)
                                    y_move = 1;
                                else if (my_grid_coords[0].y > player_grid_coord.y)
                                    y_move = -1;

                                if (player_grid_coord.x > my_grid_coords[0].x + attack_range)
                                    x_move = 1;
                                else if (player_grid_coord.x < my_grid_coords[0].x - attack_range)
                                    x_move = -1;
                            }
                            gridCoordinate desired_coordinate = new gridCoordinate(my_grid_coords[0].x + x_move,
                                                                                   my_grid_coords[0].y + y_move);

                            if (x_move != 0 || y_move != 0 && !rooted)
                                advance_towards_single_point(desired_coordinate, pl, fl, 0, corporeal);
                            else
                                fire_crossbow(fl, pl.get_my_grid_C());
                            break;
                        case Armor_Skeleton_Weapon.Magic:
                            if (is_player_within_diamond(pl, attack_range))
                            {
                                if (!is_player_within(pl, 1) && can_cast(acid_cloud_cooldown, acidcloud_mana_cost, fl))
                                    cast_acid_cloud(fl, pl.get_my_grid_C());
                                else
                                {
                                    if (can_cast(0, flamebolt_mana_cost, fl))
                                        cast_flamebolt(fl, pl.get_my_grid_C());
                                }
                                
                                if(!can_cast(0, flamebolt_mana_cost, fl) &&
                                   !can_cast(acid_cloud_cooldown, acidcloud_mana_cost, fl))
                                {
                                    if (is_player_within(pl, 1))
                                            melee_attack(fl, pl, "The Skeleton takes a swing at you!");
                                        else
                                            advance_towards_single_point(pl.get_my_grid_C(), pl, fl, 1, corporeal);
                                }
                            }
                            else
                            {
                                advance_towards_single_point(pl.get_my_grid_C(), pl, fl, 1, corporeal);
                            }
                            break;
                    }
                }
                else if (!can_see_player && have_i_seen_player)
                {
                    //fl.add_new_popup("The Grendel goes to your last position!", Popup.popup_msg_color.Red, my_grid_coord);
                    if (my_weapon_type == Armor_Skeleton_Weapon.Magic &&
                        can_cast(acid_cloud_cooldown, acidcloud_mana_cost, fl) && !is_player_within(pl, 1))
                        cast_acid_cloud(fl, last_seen_player_at);
                    else
                        advance_towards_single_point(last_seen_player_at, pl, fl, 0, corporeal);

                    if (occupies_tile(last_seen_player_at))
                        have_i_seen_player = false;
                }
                else if (!can_see_player && heard_something)
                {
                    follow_path_to_sound(fl, pl);
                }
                else
                {
                    int should_i_wander = rGen.Next(6);
                    if (should_i_wander == 1)
                        wander(pl, fl, corporeal);
                }
            }

            base.Update_Monster(pl, fl);
        }
    }
}
