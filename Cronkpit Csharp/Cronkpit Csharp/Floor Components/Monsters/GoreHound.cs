using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Cronkpit_Csharp
{
    class GoreHound: Monster
    {
        public GoreHound(gridCoordinate sGridCoord, ContentManager sCont, int sIndex)
            : base(sGridCoord, sCont, sIndex)
        {
            my_Texture = cont.Load<Texture2D>("Enemies/goreHound");
            can_see_player = false;
            has_scent = false;
            hitPoints = 6;
            min_damage = 2;
            max_damage = 8;
            can_melee_attack = true;

            //SENSORY
            sight_range = 3;
            smell_range = 6;
            smell_threshold = 5;
        }

        public override void Update_Monster(Player pl, Floor fl)
        {
            can_see_player = false;
            has_scent = false;
            look_for_player(fl, pl, sight_range);
            if (can_see_player)
            {
                advance_towards_single_point(pl.get_my_grid_C(), pl, fl);
            }
            else
            {
                sniff_for_trail(fl, 0, smell_range, smell_threshold);
                if(has_scent)
                    advance_towards_single_point(strongest_smell_coord, pl, fl);
                wander(pl, fl);
            }
        }
    }
}
