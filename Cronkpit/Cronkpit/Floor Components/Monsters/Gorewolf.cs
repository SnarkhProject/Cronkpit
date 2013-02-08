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
    class Gorewolf: Monster
    {
        int savage_cooldown;

        public Gorewolf(gridCoordinate sGridCoord, ContentManager sCont, int sIndex)
            : base(sGridCoord, sCont, sIndex, Monster_Size.Normal)
        {
            my_Texture = cont.Load<Texture2D>("Enemies/goreWolf");
            hitPoints = 22;
            armorPoints = 0;
            min_damage = 2;
            max_damage = 4;
            dmg_type = Attack.Damage.Slashing;
            wound_type = wound.Wound_Type.Open;
            can_melee_attack = true;

            //SENSORY
            smell_range = 5;
            smell_threshold = 5;

            //OTHER
            my_name = "Gorewolf";
            melee_dodge = 25;
            ranged_dodge = 15;
            savage_cooldown = 0;
        }

        public override void Update_Monster(Player pl, Floor fl)
        {
            if (savage_cooldown > 0)
                savage_cooldown--;

            Tile target_tile = strongest_smell_within(fl, 0, smell_threshold, smell_range);

            if (target_tile == null)
                has_scent = false;
            else
            {
                has_scent = true;
                strongest_smell_coord = target_tile.get_grid_c();
            }

            if (has_scent)
            {
                if (is_player_within(pl, 1))
                    advance_towards_single_point(strongest_smell_coord, pl, fl, 1, corporeal);
                else
                    advance_towards_single_point(strongest_smell_coord, pl, fl, 0, corporeal);

                if (is_player_within(pl, 1) && !has_moved)
                {
                    if (savage_cooldown == 0)
                    {
                        Attack dmg = dealDamage();
                        Attack dmg2 = dealDamage();
                        fl.addmsg("The Gorewolf savagely tears at your legs!");
                        pl.take_damage(dmg, fl, "RLeg");
                        pl.take_damage(dmg2, fl, "LLeg");
                        savage_cooldown = 3;
                    }
                    else
                    {
                        fl.addmsg("The Gorewolf lands a vicious bite!");
                        fl.add_effect(dmg_type, pl.get_my_grid_C());
                        Attack dmg = dealDamage();
                        pl.take_damage(dmg, fl, "");
                    }
                }
            }
            else
                wander(pl, fl, corporeal);
        }
    }
}
