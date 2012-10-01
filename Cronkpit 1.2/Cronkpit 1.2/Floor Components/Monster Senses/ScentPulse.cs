using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cronkpit_1._2
{
    class ScentPulse
    {
        gridCoordinate my_grid_position;
        int my_direction;
        int my_monster_origin;
        int strength;

        public ScentPulse(gridCoordinate start_grid_C, int sDirection, int sOrigin, int sStr)
        {
            my_grid_position = start_grid_C;
            my_direction = sDirection;
            my_monster_origin = sOrigin;
            strength = sStr;
        }

        public int my_strength()
        {
            return strength;
        }

        public gridCoordinate my_coord()
        {
            return my_grid_position;
        }

        public int my_monster_ID()
        {
            return my_monster_origin;
        }

        public void update(Floor fl)
        {
            //First calculate strength based on current tile. Then if strength > 0, advance 1 tile.
            //It's going to have to make new tiles every time it advances a diagonal augh.
            if (fl.is_tile_opaque(my_grid_position))
                strength = 0;
            else
                strength--;                                              
            //directions:
            //0 = up, 1 = down, 2 = left, 3 = right
            //4 = downright, 5 = downleft, 6 = upright, 7 = upleft
            //The diagonals have to multiply unfortunately. Augh.
            if (strength > 0)
            {
                switch (my_direction)
                {
                    case 0:
                        my_grid_position.y--;
                        break;
                    case 1:
                        my_grid_position.y++;
                        break;
                    case 2:
                        my_grid_position.x--;
                        break;
                    case 3:
                        my_grid_position.x++;
                        break;
                    case 4:
                        //Down Right
                        my_grid_position.y++;
                        my_grid_position.x++;
                        //Pulse to the left going down
                        fl.add_single_scent_pulse(new ScentPulse(new gridCoordinate(my_grid_position.x - 1, my_grid_position.y),
                                                        1, my_monster_origin, strength));
                        //Pulse above going right
                        fl.add_single_scent_pulse(new ScentPulse(new gridCoordinate(my_grid_position.x, my_grid_position.y - 1),
                                                        3, my_monster_origin, strength));
                        break;
                    case 5:
                        //Down Left
                        my_grid_position.y++;
                        my_grid_position.x--;
                        //Pulse to the right going down
                        fl.add_single_scent_pulse(new ScentPulse(new gridCoordinate(my_grid_position.x, my_grid_position.y + 1),
                                                        1, my_monster_origin, strength));
                        //Pulse above going left
                        fl.add_single_scent_pulse(new ScentPulse(new gridCoordinate(my_grid_position.x, my_grid_position.y - 1),
                                                        2, my_monster_origin, strength));
                        break;
                    case 6:
                        //Up Right
                        my_grid_position.y--;
                        my_grid_position.x++;
                        //Pulse to the left going up
                        fl.add_single_scent_pulse(new ScentPulse(new gridCoordinate(my_grid_position.x - 1, my_grid_position.y),
                                                        0, my_monster_origin, strength));
                        //Pulse below going right
                        fl.add_single_scent_pulse(new ScentPulse(new gridCoordinate(my_grid_position.x, my_grid_position.y + 1),
                                                        3, my_monster_origin, strength));
                        break;
                    case 7:
                        //Up Left
                        my_grid_position.y--;
                        my_grid_position.x--;
                        //Pulse to the right going up
                        fl.add_single_scent_pulse(new ScentPulse(new gridCoordinate(my_grid_position.x + 1, my_grid_position.y),
                                                        0, my_monster_origin, strength));
                        //Pulse below going left
                        fl.add_single_scent_pulse(new ScentPulse(new gridCoordinate(my_grid_position.x, my_grid_position.y + 1),
                                                        2, my_monster_origin, strength));
                        break;
                }
            }
        }
    }
}
