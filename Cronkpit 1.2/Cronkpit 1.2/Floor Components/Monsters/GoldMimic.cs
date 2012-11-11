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
    class GoldMimic: Monster
    {
        Texture2D my_idle_texture;
        Texture2D my_active_texture;
        int turns_idle;

        public GoldMimic(gridCoordinate sGridCoord, ContentManager sCont, int sIndex)
            : base(sGridCoord, sCont, sIndex)
        {
            my_active_texture = cont.Load<Texture2D>("Enemies/goldenMimic");
            hitPoints = 8;
            min_damage = 1;
            max_damage = 2;
            dmg_type = Attack.Damage.Slashing;
            wound_type = wound.Wound_Type.Open;
            can_melee_attack = true;

            int idle_texture_choice = rGen.Next(5);
            switch (idle_texture_choice)
            {
                case 0:
                    my_idle_texture = cont.Load<Texture2D>("Entities/time2getpaid");
                    break;
                case 1:
                    my_idle_texture = cont.Load<Texture2D>("Entities/tonsoGold");
                    break;
                case 2:
                    my_idle_texture = cont.Load<Texture2D>("Entities/alilmoreGold");
                    break;
                case 3:
                    my_idle_texture = cont.Load<Texture2D>("Entities/someGold");
                    break;
                case 4:
                    my_idle_texture = cont.Load<Texture2D>("Entities/lowGold");
                    break;
            }
            turns_idle = 0;
            my_Texture = my_idle_texture;
            //SENSORY
            sight_range = 3;

            //OTHER
            my_name = "Gold Mimic";
        }

        public override void Update_Monster(Player pl, Floor fl)
        {
            can_see_player = false;
            look_for_player(fl, pl, sight_range);
            if (can_see_player)
            {
                my_Texture = my_active_texture;
                turns_idle = 0;
                advance_towards_single_point(pl.get_my_grid_C(), pl, fl, 1);
                
                if(!has_moved && is_player_within(pl, 1))
                {
                    fl.addmsg("The Gold Mimic slashes at you!");
                    Attack dmg = dealDamage();
                    fl.add_effect(dmg_type, pl.get_my_grid_C());
                    pl.take_damage(dmg, ref fl);  
                }
            }
            else
            {
                turns_idle++;
                if (turns_idle > 2)
                    my_Texture = my_idle_texture;
                else
                    wander(pl, fl);
            }
        }
    }
}
