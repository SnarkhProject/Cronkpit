using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Cronkpit_1._2
{
    class GoreHound: Monster
    {
        public GoreHound(gridCoordinate sGridCoord, ContentManager sCont, int sIndex)
            : base(sGridCoord, sCont, sIndex)
        {
            my_Texture = cont.Load<Texture2D>("Enemies/goreHound");
            hitPoints = 6;
            min_damage = 2;
            max_damage = 8;
            can_melee_attack = true;

            //SENSORY
            smell_range = 6;
            smell_threshold = 5;
        }

        public override void Update_Monster(Player pl, Floor fl)
        {
            has_scent = false;
            sniff_for_trail(fl, 0, smell_range, smell_threshold);
            if (has_scent)
            {
                advance_towards_single_point(strongest_smell_coord, pl, fl);
                if (is_player_within(pl, 1))
                {
                    int dmg_value = dealDamage();
                    pl.take_damage(dmg_value);
                    fl.addmsg("The Gorehound lands a vicious bite, dealing " + dmg_value + " damage!");
                }
            }
            else
                wander(pl, fl);
        }
    }
}
