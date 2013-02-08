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
    class ZombieFanatic: Monster
    {
        public ZombieFanatic(gridCoordinate sGridCoord, ContentManager sCont, int sIndex)
            : base(sGridCoord, sCont, sIndex, Monster_Size.Normal)
        {
            my_Texture = cont.Load<Texture2D>("Enemies/zombie_fanatic");
            hitPoints = 15;
            armorPoints = 20;
            min_damage = 2;
            max_damage = 4;
            dmg_type = Attack.Damage.Slashing;
            wound_type = wound.Wound_Type.Open;
            can_melee_attack = true;

            //SENSORY
            sight_range = 4;

            //OTHER
            my_name = "Zombie Fanatic";
            melee_dodge = 10;
            ranged_dodge = 5;
            armor_effectiveness = 50;
        }
    }
}
