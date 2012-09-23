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
    class Zombie: Monster
    {
        public Zombie(gridCoordinate sGridCoord, ContentManager sCont, int sIndex)
            : base(sGridCoord, sCont, sIndex)
        {
            my_Texture = cont.Load<Texture2D>("Entities/lolzombie");
            aggro = false;
            hitPoints = 10;
            min_damage = 2;
            max_damage = 8;
        }

        public override void Update_Monster(Player pl, Floor fl)
        {
            //ZOMBIE AGGRO RULES:
            //When not aggroed, there is a 25% chance that a zombie will wander in a random direction.
            //If it cannot wander in the first direction, it will try up to 5 times for another one.
            //Aggroed when the player comes within 7 blocks of it. Then it will move towards the player.
            if (!aggro)
            {
                int wander = rGen.Next(4);
                if (wander == 1)
                {                    
                    bool walked = false;
                    int tries = 0;
                    while (tries < 5 && !walked)
                    {
                        int numeric_direction = rGen.Next(8);
                        switch (numeric_direction)
                        {
                            //up, y-
                            case 0:
                                my_grid_coord.y--;
                                if (is_spot_free(fl, pl))
                                {
                                    reset_my_drawing_position();
                                    walked = true;
                                }
                                else
                                {
                                    my_grid_coord.y++;
                                    tries++;
                                }
                                break;
                            //down, y+
                            case 1:
                                my_grid_coord.y++;
                                if (is_spot_free(fl, pl))
                                {
                                    reset_my_drawing_position();
                                    walked = true;
                                }
                                else
                                {
                                    my_grid_coord.y--;
                                    tries++;
                                }
                                break;
                            //left, x-
                            case 2:
                                my_grid_coord.x--;
                                if (is_spot_free(fl, pl))
                                {
                                    reset_my_drawing_position();
                                    walked = true;
                                }
                                else
                                {
                                    my_grid_coord.x++;
                                    tries++;
                                }
                                break;
                            //right, x+
                            case 3:
                                my_grid_coord.x++;
                                if (is_spot_free(fl, pl))
                                {
                                    reset_my_drawing_position();
                                    walked = true;
                                }
                                else
                                {
                                    my_grid_coord.x--;
                                    tries++;
                                }
                                break;
                            //down right, x+ y+
                            case 4:
                                my_grid_coord.x++;
                                my_grid_coord.y++;
                                if (is_spot_free(fl, pl))
                                {
                                    reset_my_drawing_position();
                                    walked = true;
                                }
                                else
                                {
                                    my_grid_coord.x--;
                                    my_grid_coord.y--;
                                    tries++;
                                }
                                break;
                            //down left, x- y+
                            case 5:
                                my_grid_coord.x--;
                                my_grid_coord.y++;
                                if (is_spot_free(fl, pl))
                                {
                                    reset_my_drawing_position();
                                    walked = true;
                                }
                                else
                                {
                                    my_grid_coord.x++;
                                    my_grid_coord.y--;
                                    tries++;
                                }
                                break;
                            //up right, x+ y-
                            case 6:
                                my_grid_coord.x++;
                                my_grid_coord.y--;
                                if (is_spot_free(fl, pl))
                                {
                                    reset_my_drawing_position();
                                    walked = true;
                                }
                                else
                                {
                                    my_grid_coord.x--;
                                    my_grid_coord.y++;
                                    tries++;
                                }
                                break;
                            //up left, x- y-
                            case 7:
                                my_grid_coord.x--;
                                my_grid_coord.y--;
                                if (is_spot_free(fl, pl))
                                {
                                    reset_my_drawing_position();
                                    walked = true;
                                }
                                else
                                {
                                    my_grid_coord.x++;
                                    my_grid_coord.y++;
                                    tries++;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    //Then aggro the monster if the player is within 5 tiles.
                    aggro = is_player_within(pl, 5);
                }
            }
            else
            {
                //the monster is aggroed!
                bool attacked = false;
                if (pl.get_my_grid_C().x != my_grid_coord.x)
                    if (my_grid_coord.x > pl.get_my_grid_C().x)
                    {
                        my_grid_coord.x--;
                        if (is_spot_free(fl, pl))
                            reset_my_drawing_position();
                        else
                        {
                            if(am_i_on_player(pl))
                                if (!attacked)
                                {
                                    pl.take_damage(dealDamage());
                                    attacked = true;
                                }
                            my_grid_coord.x++;
                        }
                    }
                    else
                    {
                        my_grid_coord.x++;
                        if (is_spot_free(fl, pl))
                            reset_my_drawing_position();
                        else
                        {
                            if (am_i_on_player(pl))
                                if (!attacked)
                                {
                                    pl.take_damage(dealDamage());
                                    attacked = true;
                                }
                            my_grid_coord.x--;
                        }
                    }

                if (pl.get_my_grid_C().y != my_grid_coord.y)
                    if (my_grid_coord.y > pl.get_my_grid_C().y)
                    {
                        my_grid_coord.y--;
                        if (is_spot_free(fl, pl))
                            reset_my_drawing_position();
                        else
                        {
                            if (am_i_on_player(pl))
                                if (!attacked)
                                {
                                    pl.take_damage(dealDamage());
                                    attacked = true;
                                }
                            my_grid_coord.y++;
                        }
                    }
                    else
                    {
                        my_grid_coord.y++;
                        if (is_spot_free(fl, pl))
                            reset_my_drawing_position();
                        else
                        {
                            if (am_i_on_player(pl))
                                if (!attacked)
                                {
                                    pl.take_damage(dealDamage());
                                    attacked = true;
                                }
                            my_grid_coord.y--;
                        }
                    }
            }
        }
    }
}
