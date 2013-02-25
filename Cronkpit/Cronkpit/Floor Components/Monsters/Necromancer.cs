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
        int acid_splash_cooldown = 0;
        Attack.Damage acid_splash_dmgtyp;
        Attack.Damage melee_damage_type;

        bool female;
        string pronoun;

        bool can_create_major;
        int frostbolt_range;
        int acidsplash_range;

        int frostbolt_manacost = 30;
        int acidsplash_manacost = 50;
        int create_minor_manacost = 100;

        int min_melee_damage;
        int max_melee_damage;
        int min_acidsplash_dmg;
        int max_acidsplash_dmg;

        public Necromancer(gridCoordinate sGridCoord, ContentManager sCont, int sIndex, bool createMajor)
            : base(sGridCoord, sCont, sIndex, Monster_Size.Normal)
        {
            female = false;
            int gender_roll = rGen.Next(2);
            if (gender_roll == 0)
            {
                my_Texture = sCont.Load<Texture2D>("Enemies/necromancer_male");
                pronoun = "him";
                min_melee_damage = 1;
                max_melee_damage = 3;
                melee_damage_type = Attack.Damage.Crushing;
            }
            else
            {
                female = true;
                my_Texture = sCont.Load<Texture2D>("Enemies/necromancer_female");
                pronoun = "her";
                min_melee_damage = 1;
                max_melee_damage = 4;
                melee_damage_type = Attack.Damage.Slashing;
            }

            hitPoints = 34;
            armorPoints = 5;

            min_acidsplash_dmg = 1;
            max_acidsplash_dmg = 2;
            acid_splash_dmgtyp = Attack.Damage.Acid;

            min_damage = 2;
            max_damage = 6;
            dmg_type = Attack.Damage.Frost;
            can_melee_attack = true;
            can_hear = true;

            frostbolt_range = 4;
            acidsplash_range = 5;
            can_create_major = createMajor;

            //Sensory
            sight_range = 2;
            sounds_i_can_hear.Add(SoundPulse.Sound_Types.Player);
            listen_threshold.Add(8);

            melee_dodge = 5;
            ranged_dodge = 5;
            armor_effectiveness = 10;
        }

        public List<gridCoordinate> acid_splash_matrix(gridCoordinate target_coord)
        {
            List<gridCoordinate> splash_matrix = new List<gridCoordinate>();
            splash_matrix.Add(target_coord);
            splash_matrix.Add(new gridCoordinate(target_coord.x - 1, target_coord.y));
            splash_matrix.Add(new gridCoordinate(target_coord.x + 1, target_coord.y));
            splash_matrix.Add(new gridCoordinate(target_coord.x, target_coord.y + 1));
            splash_matrix.Add(new gridCoordinate(target_coord.x, target_coord.y - 1));

            return splash_matrix;
        }

        public void create_minor_undead(gridCoordinate pl_gc, Floor fl)
        {
            List<Monster> fl_monsters = fl.see_badGuys();
            int monsterType = rGen.Next(3);
            int skeletonType = rGen.Next(6);
            gridCoordinate monster_position = new gridCoordinate();
            bool valid_position_found = false;

            for(int i = 0; i < my_grid_coords.Count; i++)
                for(int x = -1; x <= 1; x++)
                    for (int y = -1; y <= 1; y++)
                    {
                        int whocares = -1;
                        gridCoordinate next_position = new gridCoordinate(my_grid_coords[i].x + x, my_grid_coords[i].y + y);
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
            if (is_player_within(pl, sight_range))
                can_see_player = can_i_see_point(fl, pl.get_my_grid_C());
            else
                can_see_player = false;

            has_moved = false;

            if (create_minor_undead_cooldown > 0)
                create_minor_undead_cooldown--;
            if (create_major_undead_cooldown > 0)
                create_major_undead_cooldown--;
            if (acid_splash_cooldown > 0)
                acid_splash_cooldown--;

            if (!active)
            {
                if (heard_something || can_see_player)
                {
                    active = true;
                    listen_threshold[0] = 4;
                    sight_range = 6;
                    fl.addmsg("The Necromancer stops mumbling arcane phrases to " + pronoun + "self!");
                    fl.add_new_popup("Awakens!", Popup.popup_msg_color.Red, my_grid_coords[0]);
                }
                else
                {
                    int should_i_wander = rGen.Next(10);
                    if (should_i_wander == 1)
                        wander(pl, fl, corporeal);
                } 
            }
            else
            {
                if (can_see_player)
                {
                    if (can_create_major && create_major_undead_cooldown == 0)
                    {
                    }
                    else
                    {
                        if (create_minor_undead_cooldown == 0 && fl.check_mana() >= create_minor_manacost)
                        {
                            create_minor_undead(pl.get_my_grid_C(), fl);
                            fl.consume_mana(create_minor_manacost);
                        }
                        else
                        {
                            if (acid_splash_cooldown == 0 && is_player_within_diamond(pl, acidsplash_range) &&
                                fl.check_mana() >= acidsplash_manacost && !is_player_within(pl, 1))
                            {
                                fl.addmsg("The Necromancer tosses a glob of acid at you!");
                                Projectile prj = new Projectile(randomly_chosen_personal_coord(), pl.get_my_grid_C(), Projectile.projectile_type.AcidCloud, ref cont, true, Scroll.Atk_Area_Type.smallfixedAOE);
                                prj.set_small_AOE_matrix(acid_splash_matrix(pl.get_my_grid_C()));
                                prj.set_damage_range(min_acidsplash_dmg, max_acidsplash_dmg);
                                prj.set_damage_type(acid_splash_dmgtyp);
                                fl.create_new_projectile(prj);
                                acid_splash_cooldown = 5;
                                fl.consume_mana(acidsplash_manacost);
                            }
                            else
                            {
                                if (is_player_within_diamond(pl, frostbolt_range) 
                                    && fl.check_mana() >= frostbolt_manacost)
                                {
                                    fl.addmsg("The Necromancer fires a frostbolt at you!");
                                    Projectile prj = new Projectile(randomly_chosen_personal_coord(), pl.get_my_grid_C(), Projectile.projectile_type.Frostbolt, ref cont, true, Scroll.Atk_Area_Type.singleTile);
                                    prj.set_damage_range(min_damage, max_damage);
                                    prj.set_damage_type(dmg_type);
                                    fl.create_new_projectile(prj);
                                    fl.consume_mana(frostbolt_manacost);
                                }
                                else
                                {
                                    advance_towards_single_point(pl.get_my_grid_C(), pl, fl, 0, corporeal);
                                    if(!has_moved)
                                        if(is_player_within(pl, 1))
                                        {
                                            int wounds = rGen.Next(min_melee_damage, max_melee_damage + 1);
                                            Attack dmg = new Attack(melee_damage_type, wounds);
                                            pl.take_damage(dmg, fl, "");
                                            fl.add_effect(melee_damage_type, pl.get_my_grid_C());
                                            if(female)
                                                fl.addmsg("The necromancer swipes at you with her dagger!");
                                            else
                                                fl.addmsg("The necromancer tries to club you with his censer!");
                                        }
                                }
                            }
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
                        wander(pl, fl, corporeal);
                }
            }
        }
    }
}
