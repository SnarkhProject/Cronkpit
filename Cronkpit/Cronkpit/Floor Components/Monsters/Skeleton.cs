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
    class Skeleton : Monster
    {
        public enum Skeleton_Weapon_Type { Fist, Sword, Spear, Bow, Flamebolt, Axe };
        Skeleton_Weapon_Type my_weapon_type;
        public gridCoordinate last_seen_player_at;
        bool have_i_seen_player;
        //Flamebolt
        int flamebolt_mana_cost = 20;
        int flamebolt_min_dmg = 1;
        int flamebolt_max_dmg = 5;
        Attack.Damage flamebolt_dmg_type = Attack.Damage.Fire;

        public Skeleton(gridCoordinate sGridCoord, ContentManager sCont, int sIndex, Skeleton_Weapon_Type wType)
            : base(sGridCoord, sCont, sIndex, Monster_Size.Normal)
        {
            my_weapon_type = wType;
            switch (my_weapon_type)
            {
                case Skeleton_Weapon_Type.Fist:
                    my_Texture = cont.Load<Texture2D>("Enemies/skeleton");
                    min_damage = 1;
                    max_damage = 1;
                    dmg_type = Attack.Damage.Crushing;
                    break;
                case Skeleton_Weapon_Type.Sword:
                    my_Texture = cont.Load<Texture2D>("Enemies/skeleton_warrior");
                    min_damage = 1;
                    max_damage = 2;
                    dmg_type = Attack.Damage.Slashing;
                    break;
                case Skeleton_Weapon_Type.Spear:
                    my_Texture = cont.Load<Texture2D>("Enemies/skeleton_spearman");
                    min_damage = 1;
                    max_damage = 3;
                    dmg_type = Attack.Damage.Piercing;
                    break;
                case Skeleton_Weapon_Type.Bow:
                    my_Texture = cont.Load<Texture2D>("Enemies/skeleton_archer");
                    min_damage = 1;
                    max_damage = 3;
                    dmg_type = Attack.Damage.Piercing;
                    break;
                case Skeleton_Weapon_Type.Flamebolt:
                    my_Texture = cont.Load<Texture2D>("Enemies/skeleton_mage");
                    min_damage = 1;
                    max_damage = 1;
                    dmg_type = Attack.Damage.Crushing;
                    break;
                case Skeleton_Weapon_Type.Axe:
                    my_Texture = cont.Load<Texture2D>("Enemies/skeleton_axeman");
                    min_damage = 1;
                    max_damage = 2;
                    dmg_type = Attack.Damage.Slashing;
                    break;
            }

            max_hitPoints = 12;
            hitPoints = max_hitPoints;
            last_seen_player_at = new gridCoordinate(my_grid_coords[0]);

            //SENSORY
            base_sight_range = 5;
            can_hear = true;
            sounds_i_can_hear.Add(SoundPulse.Sound_Types.Voidwraith_Scream);
            base_listen_threshold.Add(1);

            set_senses_to_baseline();

            //OTHER
            my_name = "Skeleton";
            melee_dodge = 10;
            ranged_dodge = 10;
            set_initial_dodge_values();
        }

        public void fire_flamebolt(Floor fl, gridCoordinate target_GC)
        {
            Projectile prj = new Projectile(randomly_chosen_personal_coord(), target_GC,
                                            Projectile.projectile_type.Flamebolt, ref cont, true, 
                                            Scroll.Atk_Area_Type.singleTile);
            prj.set_damage_range(flamebolt_min_dmg, flamebolt_max_dmg);
            prj.set_damage_type(flamebolt_dmg_type);
            fl.create_new_projectile(prj);
            fl.consume_mana(flamebolt_mana_cost);
        }

        public void fire_arrow(Floor fl, gridCoordinate target_GC)
        {
            Projectile prj = new Projectile(randomly_chosen_personal_coord(), target_GC,
                                            Projectile.projectile_type.Arrow, ref cont, true,
                                            Scroll.Atk_Area_Type.singleTile);
            prj.set_damage_range(min_damage, max_damage);
            prj.set_damage_type(dmg_type);
            fl.create_new_projectile(prj);
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
                    have_i_seen_player = true;
                    last_seen_player_at = new gridCoordinate(pl.get_my_grid_C());

                    if (my_weapon_type == Skeleton_Weapon_Type.Bow ||
                       (my_weapon_type == Skeleton_Weapon_Type.Flamebolt && can_cast(0, flamebolt_mana_cost, fl)))
                    {
                        if (!is_player_within_diamond(pl, 4))
                            advance_towards_single_point(pl.get_my_grid_C(), pl, fl, 1, corporeal);
                        else
                        {
                            fl.addmsg("The Skeleton attacks you!");
                            if (my_weapon_type == Skeleton_Weapon_Type.Bow)
                                fire_arrow(fl, pl.get_my_grid_C());
                            else if (my_weapon_type == Skeleton_Weapon_Type.Flamebolt)
                                fire_flamebolt(fl, pl.get_my_grid_C());
                        }
                    }
                    else
                    {
                        if (!is_player_within(pl, 1))
                            advance_towards_single_point(pl.get_my_grid_C(), pl, fl, 1, corporeal);
                        else
                        {
                            fl.addmsg("The Skeleton attacks you!");
                            fl.add_effect(dmg_type, pl.get_my_grid_C());
                            Attack dmg = dealDamage();
                            pl.take_damage(dmg, fl, "");
                        }
                    }
                }
                else if (!can_see_player && have_i_seen_player && !has_moved)
                {
                    advance_towards_single_point(last_seen_player_at, pl, fl, 0, corporeal);
                    if (occupies_tile(last_seen_player_at))
                        have_i_seen_player = false;
                }
                else if (!can_see_player && !have_i_seen_player && heard_something)
                {
                    follow_path_to_sound(fl, pl);
                }
                else if (!can_see_player && !have_i_seen_player && !heard_something && !has_moved)
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
