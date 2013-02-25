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
    class GoldMimic: Monster
    {
        Texture2D my_idle_texture;
        Texture2D my_active_texture;
        int turns_idle;

        public GoldMimic(gridCoordinate sGridCoord, ContentManager sCont, int sIndex)
            : base(sGridCoord, sCont, sIndex, Monster_Size.Normal)
        {
            my_active_texture = cont.Load<Texture2D>("Enemies/goldenMimic");
            hitPoints = 8;
            armorPoints = 10;
            min_damage = 1;
            max_damage = 2;
            dmg_type = Attack.Damage.Slashing;
            can_melee_attack = true;
            turns_idle = 5;

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
            turns_idle = 5;
            my_Texture = my_idle_texture;
            //SENSORY
            sight_range = 3;

            //OTHER
            my_name = "Gold Mimic";
            melee_dodge = 5;
            ranged_dodge = 0;
            armor_effectiveness = 10;
        }

        public override void Update_Monster(Player pl, Floor fl)
        {
            if (is_player_within(pl, sight_range))
                can_see_player = can_i_see_point(fl, pl.get_my_grid_C());
            else
                can_see_player = false;

            if (can_see_player)
            {
                my_Texture = my_active_texture;
                turns_idle = 0;
                ranged_dodge = 10;
                advance_towards_single_point(pl.get_my_grid_C(), pl, fl, 1, corporeal);
                
                if(!has_moved && is_player_within(pl, 1))
                {
                    fl.addmsg("The Gold Mimic slashes at you!");
                    Attack dmg = dealDamage();
                    fl.add_effect(dmg_type, pl.get_my_grid_C());
                    pl.take_damage(dmg, fl, "");  
                }
            }
            else
            {
                turns_idle++;
                if (turns_idle > 2)
                {
                    my_Texture = my_idle_texture;
                    ranged_dodge = 0;
                }
                else
                    wander(pl, fl, corporeal);
            }
        }
    }
}
