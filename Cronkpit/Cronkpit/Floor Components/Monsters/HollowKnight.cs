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
    class HollowKnight: Monster
    {
        
        public HollowKnight(gridCoordinate sGridCoord, ContentManager sCont, int sIndex)
            : base(sGridCoord, sCont, sIndex, Monster_Size.Normal)
        {
            my_Texture = cont.Load<Texture2D>("Enemies/hollowKnight_idle");
            can_hear = true;
            hitPoints = 5;
            armorPoints = 22;
            min_damage = 1;
            max_damage = 3;
            dmg_type = Attack.Damage.Piercing;
            wound_type = wound.Wound_Type.Open;
            can_melee_attack = true;

            //SENSORY
            listen_threshold = 8;

            //OTHER
            speed_denominator = 1;
            my_name = "Hollow Knight";
            melee_dodge = 5;
            ranged_dodge = 95;
            armor_effectiveness = 95;
        }

        public void set_to_activeTexture()
        {
            my_Texture = cont.Load<Texture2D>("Enemies/hollowKnight");
        }

        public override void Update_Monster(Player pl, Floor fl)
        {
            if (heard_something && !active)
            {
                active = true;
                set_to_activeTexture();
                listen_threshold = 2;
                fl.addmsg("The Hollow Knight awakens with a lurch and a strange creak!");
                fl.add_new_popup("Awakens!", Popup.popup_msg_color.Red, my_grid_coords[0]);
            }

            has_moved = false;
            if (active)
            {
                if (speed_numerator < speed_denominator)
                {
                    follow_path_to_sound(fl, pl);
                    speed_numerator++;
                }
                else
                    speed_numerator = 0;

                if (is_player_within(pl, 1) && !has_moved)
                {
                    fl.addmsg("The Hollow Knight savagely impales you!");
                    fl.add_effect(dmg_type, pl.get_my_grid_C());
                    Attack dmg = dealDamage();
                    pl.take_damage(dmg, fl, "");
                }
            }
        }
    }
}
