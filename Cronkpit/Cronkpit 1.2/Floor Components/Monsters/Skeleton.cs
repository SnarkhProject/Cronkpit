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

        public Skeleton(gridCoordinate sGridCoord, ContentManager sCont, int sIndex, Skeleton_Weapon_Type wType)
            : base(sGridCoord, sCont, sIndex)
        {
            my_weapon_type = wType;
            switch (my_weapon_type)
            {
                case Skeleton_Weapon_Type.Fist:
                    my_Texture = cont.Load<Texture2D>("Enemies/skeleton");
                    min_damage = 1;
                    max_damage = 1;
                    dmg_type = Attack.Damage.Crushing;
                    wound_type = wound.Wound_Type.Impact;
                    break;
                case Skeleton_Weapon_Type.Sword:
                    my_Texture = cont.Load<Texture2D>("Enemies/skeleton_warrior");
                    min_damage = 1;
                    max_damage = 2;
                    dmg_type = Attack.Damage.Slashing;
                    wound_type = wound.Wound_Type.Open;
                    break;
                case Skeleton_Weapon_Type.Spear:
                    my_Texture = cont.Load<Texture2D>("Enemies/skeleton_spearman");
                    min_damage = 1;
                    max_damage = 3;
                    dmg_type = Attack.Damage.Piercing;
                    wound_type = wound.Wound_Type.Open;
                    break;
                case Skeleton_Weapon_Type.Bow:
                    my_Texture = cont.Load<Texture2D>("Enemies/skeleton_archer");
                    min_damage = 1;
                    max_damage = 3;
                    dmg_type = Attack.Damage.Piercing;
                    wound_type = wound.Wound_Type.Open;
                    break;
                case Skeleton_Weapon_Type.Flamebolt:
                    my_Texture = cont.Load<Texture2D>("Enemies/skeleton_mage");
                    min_damage = 1;
                    max_damage = 5;
                    dmg_type = Attack.Damage.Fire;
                    wound_type = wound.Wound_Type.Burn;
                    break;
                case Skeleton_Weapon_Type.Axe:
                    my_Texture = cont.Load<Texture2D>("Enemies/skeleton_axeman");
                    min_damage = 1;
                    max_damage = 2;
                    dmg_type = Attack.Damage.Slashing;
                    wound_type = wound.Wound_Type.Open;
                    break;
            }
            
            hitPoints = 12;
            can_melee_attack = true;
            last_seen_player_at = new gridCoordinate(my_grid_coord);

            //SENSORY
            sight_range = 5;

            //OTHER
            my_name = "Skeleton";
        }

        public override void Update_Monster(Player pl, Floor fl)
        {
            has_moved = false;
            can_see_player = false;
            if(is_player_within(pl, sight_range+1))
                look_for_player(fl, pl, sight_range);

            if (my_weapon_type == Skeleton_Weapon_Type.Bow || my_weapon_type == Skeleton_Weapon_Type.Flamebolt)
            {
                if (can_see_player)
                {
                    if(!is_player_within_diamond(pl, 4))
                        advance_towards_single_point(pl.get_my_grid_C(), pl, fl, 1);
                    else
                    {
                        if(!has_moved)
                        {
                            if (my_weapon_type == Skeleton_Weapon_Type.Bow)
                                fl.create_new_projectile(new Projectile(my_grid_coord, pl.get_my_grid_C(), Projectile.projectile_type.Arrow, ref cont));
                            else if (my_weapon_type == Skeleton_Weapon_Type.Flamebolt)
                                fl.create_new_projectile(new Projectile(my_grid_coord, pl.get_my_grid_C(), Projectile.projectile_type.Flamebolt, ref cont));

                            fl.addmsg("The Skeleton attacks you!");
                            fl.add_effect(dmg_type, pl.get_my_grid_C());
                            Attack dmg = dealDamage();
                            pl.take_damage(dmg, ref fl);
                        }
                    }
                }
            }
            else
            {
                if(can_see_player)
                    if(!is_player_within(pl, 1))
                        advance_towards_single_point(pl.get_my_grid_C(), pl, fl, 1);
                    else
                        if(!has_moved)
                        {
                            fl.addmsg("The Skeleton attacks you!");
                            fl.add_effect(dmg_type, pl.get_my_grid_C());
                            Attack dmg = dealDamage();
                            pl.take_damage(dmg, ref fl);
                        }
            }

            if (can_see_player)
            {
                have_i_seen_player = true;
                last_seen_player_at = new gridCoordinate(pl.get_my_grid_C());
            }
            
            if (!can_see_player && have_i_seen_player && !has_moved)
            {
                advance_towards_single_point(last_seen_player_at, pl, fl, 0);
                if (last_seen_player_at.x == my_grid_coord.x && last_seen_player_at.y == my_grid_coord.y)
                    have_i_seen_player = false;
            }
            
            if(!can_see_player && !have_i_seen_player && !has_moved)
            {
                int should_i_wander = rGen.Next(6);
                if (should_i_wander == 1)
                    wander(pl, fl);
            } 
        }
    }
}
