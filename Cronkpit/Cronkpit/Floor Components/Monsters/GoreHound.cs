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
            : base(sGridCoord, sCont, sIndex, Monster_Size.Normal)
        {
            my_Texture = cont.Load<Texture2D>("Enemies/goreHound");
            max_hitPoints = 6;
            hitPoints = max_hitPoints;
            armorPoints = 0;
            min_damage = 1;
            max_damage = 2;
            dmg_type = Attack.Damage.Slashing;
            can_melee_attack = true;

            //SENSORY
            smell_range = 4;
            smell_threshold = 10;
            can_hear = true;
            sounds_i_can_hear.Add(SoundPulse.Sound_Types.Voidwraith_Scream);
            listen_threshold.Add(1);

            //OTHER
            my_name = "Gorehound";
            melee_dodge = 30;
            ranged_dodge = 20;
            set_initial_dodge_values();
        }

        public override void Update_Monster(Player pl, Floor fl)
        {
            Tile target_tile = strongest_smell_within(fl, 0, smell_threshold, smell_range);
            heal_near_altar(fl);

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
                    fl.addmsg("The Gorehound lands a vicious bite!");
                    fl.add_effect(dmg_type, pl.get_my_grid_C());
                    Attack dmg = dealDamage();
                    pl.take_damage(dmg, fl, "");
                }
            }
            else if (!has_scent && heard_something)
                follow_path_to_sound(fl, pl);
            else
                wander(pl, fl, corporeal);
        }
    }
}
