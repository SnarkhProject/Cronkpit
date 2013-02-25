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
    class Zombie: Monster
    {
        public Zombie(gridCoordinate sGridCoord, ContentManager sCont, int sIndex)
            : base(sGridCoord, sCont, sIndex, Monster_Size.Normal)
        {
            my_Texture = cont.Load<Texture2D>("Enemies/lolzombie");
            hitPoints = 10;
            min_damage = 1;
            max_damage = 1;
            dmg_type = Attack.Damage.Crushing;
            can_melee_attack = true;

            //SENSORY
            sight_range = 3;
            can_hear = true;
            sounds_i_can_hear.Add(SoundPulse.Sound_Types.Voidwraith_Scream);
            sounds_i_can_hear.Add(SoundPulse.Sound_Types.Fanatic_Scream);
            listen_threshold.Add(1);
            listen_threshold.Add(1);

            //OTHER
            my_name = "Zombie";
            melee_dodge = 0;
            ranged_dodge = 0;
        }

        public override void Update_Monster(Player pl, Floor fl)
        {
            //ZOMBIE AGGRO RULES:
            //When not aggroed, there is a 25% chance that a zombie will wander in a random direction.
            //If it cannot wander in the first direction, it will try up to 5 times for another one.
            //Aggroed when the player comes within 3 blocks of it. Then it will move towards the player.

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
                    fl.addmsg("The Zombie swings at you!");
                    Attack dmg = dealDamage();
                    fl.add_effect(dmg_type, pl.get_my_grid_C());
                    pl.take_damage(dmg, fl, "");
                }
            }
            else if (!can_see_player && heard_something)
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
