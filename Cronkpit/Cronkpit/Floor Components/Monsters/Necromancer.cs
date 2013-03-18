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

        public Necromancer(gridCoordinate sGridCoord, ContentManager sCont, int sIndex, int rank = 1)
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

            max_hitPoints = 34;
            hitPoints = max_hitPoints;
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
            can_create_major = rank > 1;

            //Sensory
            sight_range = 2;
            sounds_i_can_hear.Add(SoundPulse.Sound_Types.Player);
            listen_threshold.Add(8);

            melee_dodge = 5;
            ranged_dodge = 5;
            armor_effectiveness = 10;
            set_initial_dodge_values();
            smart_monster = true;
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

        public void create_minor_undead(Player pl, Floor fl)
        {
            gridCoordinate pl_gc = pl.get_my_grid_C();
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
                fl.add_new_popup("Summoned!", Popup.popup_msg_color.Purple, monster_position);
                add_minor_undead(fl, monsterType, skeletonType, 
                                 next_index, monster_position, false, pl);
                create_minor_undead_cooldown = 10;
            }
            else
                create_minor_undead_cooldown = 5;

            raise_additional_undead(fl, pl, 1);
        }

        public void raise_additional_undead(Floor fl, Player pl, int grade)
        {
            if (is_doodad_within(fl, sight_range - 1, Doodad.Doodad_Type.CorpsePile))
            {
                List<Doodad> corpses = new List<Doodad>();
                int corpse_index = 0;
                while (fl.Doodad_by_index(corpse_index) != null)
                {
                    Doodad d = fl.Doodad_by_index(corpse_index);
                    gridCoordinate corpse_position = d.get_g_coord();
                    if(d.get_my_doodad_type() == Doodad.Doodad_Type.CorpsePile &&
                       corpse_position.x <= my_grid_coords[0].x + (sight_range-1) && 
                       corpse_position.x >= my_grid_coords[0].x - (sight_range-1) &&
                       corpse_position.y <= my_grid_coords[0].y + (sight_range-1) && 
                       corpse_position.y >= my_grid_coords[0].y - (sight_range-1) &&
                       can_i_see_point(fl, corpse_position, VisionRay.fineness.Roughest) &&
                       !occupies_tile(corpse_position))
                        corpses.Add(fl.Doodad_by_index(corpse_index));
                    corpse_index++;
                }

                int target_corpse = rGen.Next(corpses.Count);
                int corpse_ID = corpses[target_corpse].get_my_index();
                gridCoordinate rezzed_corpse_pos = new gridCoordinate(corpses[target_corpse].get_g_coord());
                fl.remove_doodad_at_index(corpse_ID);

                List<Monster> fl_monsters = fl.see_badGuys();
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

                int whoCares = -1;
                bool force_wander = (pl.get_my_grid_C().x == rezzed_corpse_pos.x &&
                                     pl.get_my_grid_C().y == rezzed_corpse_pos.y) ||
                                     fl.is_monster_here(rezzed_corpse_pos, out whoCares) ||
                                     !fl.isWalkable(rezzed_corpse_pos);
                switch (grade)
                {
                    //Minor undead
                    case 1:
                        int minor_monster_type = rGen.Next(3);
                        int minor_skel_type = rGen.Next(6);
                        add_minor_undead(fl, minor_monster_type, minor_skel_type,
                                         next_index, rezzed_corpse_pos, force_wander, pl);
                        break;
                    //Major undead
                    case 2:
                        break;
                    //Lethal undead
                    case 3:
                        break;
                }
                fl.add_new_popup("Summoned!", Popup.popup_msg_color.Purple, rezzed_corpse_pos);
            }
        }

        public void add_minor_undead(Floor fl, int minor_monster_type, 
                                     int minor_skeleton_type, int next_monster_index, 
                                     gridCoordinate monster_position, bool force_wander, Player pl)
        {
            switch (minor_monster_type)
            {
                case 0:
                    fl.see_badGuys().Add(new Zombie(monster_position, cont, next_monster_index));
                    break;
                case 1:
                    fl.see_badGuys().Add(new GoreHound(monster_position, cont, next_monster_index));
                    break;
                case 2:
                    Skeleton.Skeleton_Weapon_Type skelweapon = 0;
                    switch (minor_skeleton_type)
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
                    fl.see_badGuys().Add(new Skeleton(monster_position, cont, next_monster_index, skelweapon));
                    break;
            }
            if (force_wander)
                fl.force_monster_wander(next_monster_index, pl);
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

            heal_near_altar(fl);

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
                            create_minor_undead(pl, fl);
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
