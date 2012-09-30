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
            }

            if (active)
            {
                if (speed_numerator < speed_denominator)
                {
                    follow_path_to_sound(fl, pl);
                    speed_numerator++;
                }
                else
                    speed_numerator = 0;
            }
        }
    }
}
