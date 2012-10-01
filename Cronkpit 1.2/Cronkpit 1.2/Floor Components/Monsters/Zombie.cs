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
    class Zombie: Monster
    {
        public Zombie(gridCoordinate sGridCoord, ContentManager sCont, int sIndex)
            : base(sGridCoord, sCont, sIndex)
        {
            my_Texture = cont.Load<Texture2D>("Enemies/lolzombie");
            hitPoints = 10;
            min_damage = 2;
            max_damage = 8;
            can_melee_attack = true;

            //SENSORY
            sight_range = 3;
        }

        public override void Update_Monster(Player pl, Floor fl)
        {
            //ZOMBIE AGGRO RULES:
            //When not aggroed, there is a 25% chance that a zombie will wander in a random direction.
            //If it cannot wander in the first direction, it will try up to 5 times for another one.
            //Aggroed when the player comes within 7 blocks of it. Then it will move towards the player.
            can_see_player = false;
            look_for_player(fl, pl, sight_range);
            if (!can_see_player)
            {
                int should_i_wander = rGen.Next(4);
                if (should_i_wander == 1)
                {
                    wander(pl, fl);
                }
            }
            else
            {
                //the monster is aggroed!
                advance_towards_single_point(pl.get_my_grid_C(), pl, fl);
                if (is_player_within(pl, 1))
                {
                    int dmg_value = dealDamage();
                    pl.take_damage(dmg_value);
                    fl.addmsg("The Zombie claws at you, dealing " + dmg_value + " damage!");
                }
            }
        }
    }
}
