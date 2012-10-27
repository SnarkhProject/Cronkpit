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
        }

        public override void Update_Monster(Player pl, Floor fl)
        {
            has_scent = false;
            sniff_for_trail(fl, 0, smell_range, smell_threshold);
            if (has_scent)
            {
                advance_towards_single_point(strongest_smell_coord, pl, fl);
                if (is_player_within(pl, 1) && !has_moved)
                {
                    Attack dmg = dealDamage();
                    pl.take_damage(dmg);
                    fl.addmsg("The Gorehound lands a vicious bite! You take " + dmg.get_assoc_wound().severity + " open wounds!");
                }
            }
            else
                wander(pl, fl);
        }
    }
}
