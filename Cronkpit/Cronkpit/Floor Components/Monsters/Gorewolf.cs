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
    class Gorewolf: Monster
    {
        int savage_cooldown;

        public Gorewolf(gridCoordinate sGridCoord, ContentManager sCont, int sIndex)
            : base(sGridCoord, sCont, sIndex, Monster_Size.Normal)
        {
            my_Texture = cont.Load<Texture2D>("Enemies/goreWolf");
            max_hitPoints = 22;
            hitPoints = max_hitPoints;
            armorPoints = 0;
            min_damage = 2;
            max_damage = 4;
            dmg_type = Attack.Damage.Slashing;

            //SENSORY
            base_smell_range = 5;
            base_smell_threshold = 5;
            can_hear = true;
            sounds_i_can_hear.Add(SoundPulse.Sound_Types.Voidwraith_Scream);
            base_listen_threshold.Add(1);

            set_senses_to_baseline();

            //OTHER
            my_name = "Gorewolf";
            melee_dodge = 25;
            ranged_dodge = 15;
            savage_cooldown = 0;
            set_initial_dodge_values();
        }

        public override void Update_Monster(Player pl, Floor fl)
        {
            if (savage_cooldown > 0)
                savage_cooldown--;

            heal_near_altar(fl);
            has_moved = false;

            Tile target_tile = strongest_smell_within(fl, 0, smell_threshold, smell_range);

            if (target_tile == null)
                has_scent = false;
            else
            {
                has_scent = true;
                strongest_smell_coord = target_tile.get_grid_c();
            }

            if (!stunned)
            {
                if (has_scent)
                {
                    if (is_player_within(pl, 1))
                        advance_towards_single_point(strongest_smell_coord, pl, fl, 1, corporeal);
                    else
                        advance_towards_single_point(strongest_smell_coord, pl, fl, 0, corporeal);

                    if (is_player_within(pl, 1) && !has_moved)
                    {
                        if (savage_cooldown == 0)
                        {
                            Attack dmg = dealDamage();
                            Attack dmg2 = dealDamage();
                            fl.addmsg("The Gorewolf savagely tears at your legs!");
                            fl.add_specific_effect(Floor.specific_effect.Bite, pl.get_my_grid_C());
                            pl.take_damage(dmg, fl, "RLeg");
                            pl.take_damage(dmg2, fl, "LLeg");
                            if (rGen.Next(2) == 0)
                            {
                                fl.addmsg("You start bleeding profusely from the attack!");
                                pl.add_single_statusEffect(new StatusEffect(Scroll.Status_Type.Hemorrhage, 5));
                            }
                            savage_cooldown = 3;
                        }
                        else
                        {
                            fl.addmsg("The Gorewolf lands a vicious bite!");
                            fl.add_effect(dmg_type, pl.get_my_grid_C());
                            Attack dmg = dealDamage();
                            pl.take_damage(dmg, fl, "");
                        }
                    }
                }
                else if (!has_scent && heard_something)
                    follow_path_to_sound(fl, pl);
                else
                    wander(pl, fl, corporeal);
            }

            base.Update_Monster(pl, fl);
        }
    }
}
