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
    class GoreHound: Monster
    {
        public GoreHound(gridCoordinate sGridCoord, ContentManager sCont, int sIndex)
            : base(sGridCoord, sCont, sIndex)
        {
            my_Texture = cont.Load<Texture2D>("Enemies/goreHound");
            hitPoints = 6;
            min_damage = 1;
            max_damage = 2;
            dmg_type = Attack.Damage.Slashing;
            wound_type = wound.Wound_Type.Open;
            can_melee_attack = true;

            //SENSORY
            smell_range = 6;
            smell_threshold = 5;

            //OTHER
            my_name = "Gorehound";
            melee_dodge = 30;
            ranged_dodge = 20;
        }

        public override void Update_Monster(Player pl, Floor fl)
        {
            has_scent = false;
            if(is_smell_i_can_smell_within(my_grid_coord, fl, 0, smell_threshold, smell_range+1))
                sniff_for_trail(fl, 0, smell_range, smell_threshold);
            if (has_scent)
            {
                advance_towards_single_point(strongest_smell_coord, pl, fl, 1);
                if (is_player_within(pl, 1) && !has_moved)
                {
                    fl.addmsg("The Gorehound lands a vicious bite!");
                    fl.add_effect(dmg_type, pl.get_my_grid_C());
                    Attack dmg = dealDamage();
                    pl.take_damage(dmg, fl);
                }
            }
            else
                wander(pl, fl);
        }
    }
}
