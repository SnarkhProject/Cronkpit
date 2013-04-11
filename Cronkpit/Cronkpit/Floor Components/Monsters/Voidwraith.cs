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
    class Voidwraith: Monster
    {
        int blood_vomit_cooldown = 0;
        int blood_vomit_min_dmg = 1;
        int blood_vomit_max_dmg = 3;
        int blood_vomit_range = 4;
        Attack.Damage blood_vomit_dmgtyp = Attack.Damage.Acid;

        int half_HP = 7;

        public Voidwraith(gridCoordinate sGridCoord, ContentManager sCont, int sIndex)
            : base(sGridCoord, sCont, sIndex, Monster_Size.Normal)
        {
            my_Texture = cont.Load<Texture2D>("Enemies/voidwraith");
            max_hitPoints = 15;
            hitPoints = max_hitPoints;
            min_damage = 1;
            max_damage = 2;
            dmg_type = Attack.Damage.Acid;
            corporeal = false;

            //SENSORY
            base_sight_range = 5;

            set_senses_to_baseline();

            //OTHER
            my_name = "Void Wraith";
            melee_dodge = 40;
            ranged_dodge = 40;
            set_initial_dodge_values();
        }

        public override void Update_Monster(Player pl, Floor fl)
        {
            if (blood_vomit_cooldown > 0)
                blood_vomit_cooldown--;
            has_moved = false;

            if (!stunned)
            {
                if (hitPoints > half_HP)
                {
                    if (is_player_within(pl, sight_range))
                    {
                        if (blood_vomit_cooldown == 0 && is_player_within(pl, blood_vomit_range))
                        {
                            int xdir = 0;
                            int ydir = 0;
                            if (pl.get_my_grid_C().x > my_grid_coords[0].x)
                                xdir = 1;
                            else if (pl.get_my_grid_C().x < my_grid_coords[0].x)
                                xdir = -1;

                            if (pl.get_my_grid_C().y > my_grid_coords[0].y)
                                ydir = 1;
                            else if (pl.get_my_grid_C().y < my_grid_coords[0].y)
                                ydir = -1;

                            int target_index = 0;
                            for (int i = 0; i < 8; i++)
                            {
                                if (movement_indexes[i][0] == xdir && movement_indexes[i][1] == ydir)
                                    target_index = i;
                            }
                            gridCoordinate.direction vomit_direction = direction_indexes[target_index];
                            fl.cone_attack(blood_vomit_range, my_grid_coords[0], vomit_direction,
                                           blood_vomit_max_dmg + 1, blood_vomit_min_dmg, blood_vomit_dmgtyp,
                                           pl, true, Floor.specific_effect.Acid_Blood);
                            blood_vomit_cooldown = 5;
                        }
                        else
                        {
                            if (is_player_within(pl, 1))
                            {
                                fl.addmsg("The Void Wraith's essence burns you!");
                                Attack dmg = dealDamage();
                                fl.add_specific_effect(Floor.specific_effect.Acid_Blood, pl.get_my_grid_C());
                                pl.take_damage(dmg, fl, "");
                                if (rGen.Next(2) == 0)
                                {
                                    fl.addmsg("You start bleeding profusely from the attack!");
                                    pl.add_single_statusEffect(new StatusEffect(Scroll.Status_Type.Hemorrhage, 5));
                                }
                            }
                            else
                                advance_towards_single_point(pl.get_my_grid_C(), pl, fl, 1, corporeal);
                        }
                    }
                }
                else
                {
                    fl.sound_pulse(my_grid_coords[0], 20, SoundPulse.Sound_Types.Voidwraith_Scream);
                    fl.add_new_popup("SCREAMS!", Popup.popup_msg_color.Red, my_grid_coords[0]);
                }
            }

            base.Update_Monster(pl, fl);
        }
    }
}
