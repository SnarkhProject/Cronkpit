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
    class Ghost: Monster
    {
        public Ghost(gridCoordinate sGridCoord, ContentManager sCont, int sIndex)
            : base(sGridCoord, sCont, sIndex, Monster_Size.Normal)
        {
            my_Texture = cont.Load<Texture2D>("Enemies/ghost");
            max_hitPoints = 12;
            hitPoints = max_hitPoints;
            min_damage = 1;
            max_damage = 3;
            dmg_type = Attack.Damage.Frost;

            //SENSORY
            sight_range = 4;
            can_hear = true;
            sounds_i_can_hear.Add(SoundPulse.Sound_Types.Voidwraith_Scream);
            listen_threshold.Add(1);

            //OTHER
            my_name = "Ghost";
            melee_dodge = 50;
            ranged_dodge = 50;
            corporeal = false;
            set_initial_dodge_values();
        }

        public override void Update_Monster(Player pl, Floor fl)
        {
            heal_near_altar(fl);

            if (is_player_within(pl, sight_range))
                can_see_player = can_i_see_point(fl, pl.get_my_grid_C());
            else
                can_see_player = false;

            if (can_see_player)
            {
                //the monster is aggroed!
                advance_towards_single_point(pl.get_my_grid_C(), pl, fl, 1, corporeal);
                if (is_player_within(pl, 1) && !has_moved)
                {
                    fl.addmsg("The Ghost reaches past the veil and touches you!");
                    Attack dmg = dealDamage();
                    fl.add_effect(dmg_type, pl.get_my_grid_C());
                    pl.take_damage(dmg, fl, "");
                }
            }
            else if(!can_see_player && heard_something)
            {
                follow_path_to_sound(fl, pl);
            }
            else
            {
                int should_i_wander = rGen.Next(5);
                if (should_i_wander == 1)
                {
                    wander(pl, fl, corporeal);
                }
            }
        }
    }
}
