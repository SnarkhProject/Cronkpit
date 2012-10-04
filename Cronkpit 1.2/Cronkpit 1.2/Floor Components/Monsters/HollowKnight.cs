﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Cronkpit_1._2
{
    class HollowKnight: Monster
    {
        
        public HollowKnight(gridCoordinate sGridCoord, ContentManager sCont, int sIndex)
            : base(sGridCoord, sCont, sIndex)
        {
            my_Texture = cont.Load<Texture2D>("Enemies/hollowKnight_idle");
            can_hear = true;
            hitPoints = 12;
            min_damage = 10;
            max_damage = 30;
            can_melee_attack = true;

            //SENSORY
            listen_threshold = 8;

            //OTHER
            speed_denominator = 1;
            my_name = "Hollow Knight";
        }

        public void set_to_activeTexture()
        {
            my_Texture = cont.Load<Texture2D>("Enemies/hollowKnight");
        }

        public override void Update_Monster(Player pl, Floor fl)
        {
            if (heard_something == true && active == false)
            {
                active = true;
                set_to_activeTexture();
                listen_threshold = 2;
                fl.addmsg("The Hollow Knight awakens with a lurch and a strange creak!");
            }

            if (active)
            {
                if (speed_numerator < speed_denominator)
                {
                    follow_path_to_sound(fl, pl);
                    speed_numerator++;
                }
                else
                {
                    has_moved = false;
                    speed_numerator = 0;
                }
                if (is_player_within(pl, 1) && !has_moved)
                {
                    int dmg_value = dealDamage();
                    pl.take_damage(dmg_value);
                    fl.addmsg("The Hollow Knight savagely impales you, dealing " + dmg_value + " damage!");
                }
            }
        }
    }
}
