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
    class Necromancer: Monster
    {
        int create_minor_undead_cooldown = 0;
        int create_major_undead_cooldown = 0;
        bool can_create_major;
        int frostbolt_range;

        public Necromancer(gridCoordinate sGridCoord, ContentManager sCont, int sIndex, bool createMajor)
            : base(sGridCoord, sCont, sIndex)
        {
            my_Texture = cont.Load<Texture2D>("Enemies/Necromancer");
            hitPoints = 30;
            min_damage = 2;
            max_damage = 6;
            dmg_type = Attack.Damage.Frost;
            wound_type = wound.Wound_Type.Frostburn;
            can_melee_attack = true;
            can_hear = true;

            frostbolt_range = 4;
            can_create_major = createMajor;

            sight_range = 3;
            listen_threshold = 7;

            melee_dodge = 5;
            ranged_dodge = 5;
        }

        public void create_minor_undead(gridCoordinate pl_gc, Floor fl)
        {
            List<Monster> fl_monsters = fl.see_badGuys();
            int monsterType = rGen.Next(3);
            int skeletonType = rGen.Next(6);
            gridCoordinate monster_position = new gridCoordinate();
            bool valid_position_found = false;

            for(int x = -1; x <= 1; x++)
                for(int y = -1; y <= 1; y++)
                {
                    int whocares = -1;
                    gridCoordinate next_position = new gridCoordinate(my_grid_coord.x + x, my_grid_coord.y + y);
                    if (!valid_position_found && !fl.is_tile_opaque(next_position) &&
                        !fl.is_entity_here(next_position) && !fl.is_monster_here(next_position, out whocares) &&
                        pl_gc.x != next_position.x && pl_gc.y != next_position.y)
                    {
                        monster_position = next_position;
                        valid_position_found = true;
                    }
                }

            if (valid_position_found)
            {
                int next_index = -1;
                bool found_valid_number = false;
                while (!found_valid_number)
                {
                    next_index++;
                    bool valid_number = true;
                    for (int i = 0; i < fl_monsters.Count; i++)
                    {
                        if (next_index == fl_monsters[i].my_Index)
                            valid_number = false;
                    }

                    if (valid_number)
                        found_valid_number = true;
                }

                switch (monsterType)
                {
                    case 0:
                        fl_monsters.Add(new Zombie(monster_position, cont, next_index));
                        break;
                    case 1:
                        fl_monsters.Add(new GoreHound(monster_position, cont, next_index));
                        break;
                    case 2:
                        Skeleton.Skeleton_Weapon_Type skelweapon = 0;
                        switch (skeletonType)
                        {
                            case 0:
                                skelweapon = Skeleton.Skeleton_Weapon_Type.Fist;
                                break;
                            case 1:
                                skelweapon = Skeleton.Skeleton_Weapon_Type.Axe;
                                break;
                            case 2:
                                skelweapon = Skeleton.Skeleton_Weapon_Type.Bow;
                                break;
                            case 3:
                                skelweapon = Skeleton.Skeleton_Weapon_Type.Flamebolt;
                                break;
                            case 4:
                                skelweapon = Skeleton.Skeleton_Weapon_Type.Spear;
                                break;
                            case 5:
                                skelweapon = Skeleton.Skeleton_Weapon_Type.Sword;
                                break;
                        }
                        fl_monsters.Add(new Skeleton(monster_position, cont, next_index, skelweapon));
                        break;
                }
                create_minor_undead_cooldown = 10;
                fl.add_new_popup("Summoned!", Popup.popup_msg_color.Purple, monster_position);
            }
            else
                create_minor_undead_cooldown = 5;
        }

        public override void Update_Monster(Player pl, Floor fl)
        {
            can_see_player = false;

            if (create_minor_undead_cooldown > 0)
                create_minor_undead_cooldown--;
            if (create_major_undead_cooldown > 0)
                create_major_undead_cooldown--;

            if (!active)
            {
                look_for_player(fl, pl, sight_range);

                if (heard_something || can_see_player)
                {
                    active = true;
                    listen_threshold = 4;
                    sight_range = 5;
                    fl.addmsg("The Necromancer stops mumbling arcane phrases to themself!");
                    fl.add_new_popup("Awakens!", Popup.popup_msg_color.Red, my_grid_coord);
                }
                else
                {
                    int should_i_wander = rGen.Next(10);
                    if (should_i_wander == 1)
                        wander(pl, fl);
                } 
            }
            else
            {
                look_for_player(fl, pl, sight_range);
                if (can_see_player)
                {
                    if (can_create_major && create_major_undead_cooldown == 0)
                    {
                    }
                    else
                    {
                        if (create_minor_undead_cooldown == 0)
                        {
                            create_minor_undead(pl.get_my_grid_C(), fl);
                        }
                        else
                        {
                            if (is_player_within_diamond(pl, frostbolt_range))
                            {
                                fl.create_new_projectile(new Projectile(my_grid_coord, pl.get_my_grid_C(), Projectile.projectile_type.Frostbolt, ref cont));
                                fl.addmsg("The Necromancer fires a frostbolt at you!");
                                fl.add_effect(dmg_type, pl.get_my_grid_C());
                                Attack dmg = dealDamage();
                                pl.take_damage(dmg, ref fl);
                            }
                            else
                                advance_towards_single_point(pl.get_my_grid_C(), pl, fl, 0);
                        }
                    }
                }
                else if (!can_see_player && heard_something)
                {
                    follow_path_to_sound(fl, pl);
                }
                else
                {
                    int should_i_wander = rGen.Next(10);
                    if (should_i_wander == 1)
                        wander(pl, fl);
                }
            }
        }
    }
}
