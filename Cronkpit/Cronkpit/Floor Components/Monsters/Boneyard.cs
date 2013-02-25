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
    class Boneyard: Monster
    {
        Texture2D bitey_texture;
        Texture2D normal_texture;

        int bite_min_damage;
        int bite_max_damage;
        int bite_cooldown = 0;

        int bone_spear_mindmg;
        int bone_spear_maxdmg;
        Attack.Damage bone_spear_dmgtyp = Attack.Damage.Piercing;

        int blood_spray_mindmg;
        int blood_spray_maxdmg;
        Attack.Damage blood_spray_dmgtyp = Attack.Damage.Acid;


        public Boneyard(gridCoordinate sGridCoord, ContentManager sCont, int sIndex, bool bossmonster)
            : base(sGridCoord, sCont, sIndex, Monster_Size.Large)
        {
            normal_texture = sCont.Load<Texture2D>("Enemies/Boneyard");
            //bitey_texture = 
            my_Texture = normal_texture;

            if (bossmonster)
            {
                hitPoints = 250;

                min_damage = 1;
                max_damage = 3;
                bone_spear_mindmg = 2;
                bone_spear_maxdmg = 3;
                bite_min_damage = 4;
                bite_max_damage = 6;
                blood_spray_mindmg = 1;
                blood_spray_maxdmg = 1;
            }
            else
            {
                hitPoints = 60;

                min_damage = 2;
                max_damage = 5;
                bone_spear_mindmg = 3;
                bone_spear_maxdmg = 4;
                bite_min_damage = 5;
                bite_max_damage = 7;
                blood_spray_mindmg = 1;
                blood_spray_maxdmg = 2;
            }

            //SENSORY
            sight_range = 6;
            can_hear = true;
            sounds_i_can_hear.Add(SoundPulse.Sound_Types.Player);
            sounds_i_can_hear.Add(SoundPulse.Sound_Types.Voidwraith_Scream);
            listen_threshold.Add(4);
            listen_threshold.Add(1);

            my_name = "Boneyard";
            melee_dodge = 5;
            ranged_dodge = 5;
        }
    }
}
