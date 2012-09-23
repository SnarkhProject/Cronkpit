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
    class Monster
    {
        public Texture2D my_Texture;
        public Vector2 my_Position;
        public ContentManager cont;
        public gridCoordinate my_grid_coord;
        public Random rGen;

        //Sight
        public bool can_see_player;
        public int sight_range;
        //Smell
        public bool has_scent;
        public gridCoordinate strongest_smell_coord;
        public int smell_range;
        public int smell_threshold;

        public bool can_melee_attack;

        public int hitPoints;
        public int my_Index;
        public int min_damage;
        public int max_damage;

        public Monster(gridCoordinate sGridCoord, ContentManager sCont, int sIndex)
        {
            cont = sCont;
            my_grid_coord = sGridCoord;
            strongest_smell_coord = sGridCoord;
            my_Position = new Vector2(sGridCoord.x * 32, sGridCoord.y * 32);
            rGen = new Random();
            my_Index = sIndex;
            hitPoints = 0;
            min_damage = 0;
            max_damage = 0;
            can_melee_attack = false;
            sight_range = 0;
            has_scent = false;
            smell_range = 0;
        }

        //don't call unless you've started the spritebatch!
        public void drawMe(ref SpriteBatch sb)
        {
            sb.Draw(my_Texture, my_Position, Color.White);
        }

        //overidden on a per-monster basis. The goal is to ONLY have this function
        //in each monster class.
        public virtual void Update_Monster(Player pl, Floor fl)
        {
        }

        //drawing shit. Maybe make more advanced later.
        public void reset_my_drawing_position()
        {
            my_Position.X = my_grid_coord.x * 32;
            my_Position.Y = my_grid_coord.y * 32;
        }

        //All positioning stuff.
        public bool am_i_on_player(Player pl)
        {
            return (my_grid_coord.x == pl.get_my_grid_C().x &&
                    my_grid_coord.y == pl.get_my_grid_C().y);
        }

        public bool is_spot_free(Floor fl, Player pl)
        {
            return (am_i_on_player(pl) == false && 
                    fl.isWalkable(my_grid_coord) &&
                    fl.am_i_on_other_monster(my_grid_coord, my_Index) == false);
        }

        //pick a random direction and walk 1 square in it.
        //Will retry 5 times if the square is full.
        public void wander(Player pl, Floor fl)
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
        }

        //advance towards the target point - usually the player.
        //If the monster collides with the player, it attacks them in melee if it can.
        public void advance_towards_single_point(gridCoordinate target_point, Player pl, Floor fl)
        {
            bool attacked = false;
            if (target_point.x != my_grid_coord.x)
                if (my_grid_coord.x > target_point.x)
                {
                    my_grid_coord.x--;
                    if (is_spot_free(fl, pl))
                        reset_my_drawing_position();
                    else
                    {
                        if (am_i_on_player(pl))
                            if (!attacked && can_melee_attack)
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
                            if (!attacked && can_melee_attack)
                            {
                                pl.take_damage(dealDamage());
                                attacked = true;
                            }
                        my_grid_coord.x--;
                    }
                }

            if (target_point.y != my_grid_coord.y)
                if (my_grid_coord.y > target_point.y)
                {
                    my_grid_coord.y--;
                    if (is_spot_free(fl, pl))
                        reset_my_drawing_position();
                    else
                    {
                        if (am_i_on_player(pl))
                            if (!attacked && can_melee_attack)
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
                            if (!attacked && can_melee_attack)
                            {
                                pl.take_damage(dealDamage());
                                attacked = true;
                            }
                        my_grid_coord.y--;
                    }
                }
        }

        //damage stuff
        public void takeDamage(int dmg)
        {
            hitPoints -= dmg;
        }

        public int dealDamage()
        {
            return rGen.Next(min_damage, max_damage);
        }

        //now for the good stuff - SENSORY STUFF.
        //Probably not going to use this.
        public bool is_player_within(Player pl, int radius)
        {
            if (pl.get_my_grid_C().x > (my_grid_coord.x - radius) &&
                pl.get_my_grid_C().x < (my_grid_coord.x + radius) &&
                pl.get_my_grid_C().y > (my_grid_coord.y - radius) &&
                pl.get_my_grid_C().y < (my_grid_coord.y + radius))
                return true;
            else
                return false;
        }

        //Sight pulse to find player
        public void look_for_player(Floor fl, Player pl, int sight_range)
        {
            fl.sight_pulse(my_grid_coord, pl, my_Index, sight_range);
        }

        //Scent pulse to find highest smell tile
        public void sniff_for_trail(Floor fl, int target_scent, int smell_range, int smell_threshold)
        {
            fl.scent_pulse(my_grid_coord, target_scent, my_Index, smell_range, smell_threshold);
        }
    }
}
