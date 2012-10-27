using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cronkpit_1._2
{
    //Deprecated! replaced by visionRay.
    class SightPulse
    {
        gridCoordinate my_grid_position;
        int my_direction;
        int my_monster_origin;
        int strength;
        bool propogate;

        public SightPulse(gridCoordinate start_grid_C, int sDirection, int sOrigin, int sStr, bool sProp)
        {
            my_grid_position = start_grid_C;
            my_direction = sDirection;
            my_monster_origin = sOrigin;
            strength = sStr;
            propogate = sProp;
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
                        if (!fl.is_tile_opaque(my_grid_position) && propogate)
                        {
                            //Pulse to the left going up left
                            fl.add_single_sight_pulse(new SightPulse(new gridCoordinate(my_grid_position.x+1, my_grid_position.y),
                                                            6, my_monster_origin, strength, true));
                            //Pulse to the right going up right
                            fl.add_single_sight_pulse(new SightPulse(new gridCoordinate(my_grid_position.x-1, my_grid_position.y),
                                                            7, my_monster_origin, strength, true));
                        }
                        break;
                    case 1:
                        my_grid_position.y++;
                        if (!fl.is_tile_opaque(my_grid_position) && propogate)
                        {
                            //Pulse to the left going down left
                            fl.add_single_sight_pulse(new SightPulse(new gridCoordinate(my_grid_position.x - 1, my_grid_position.y),
                                                            5, my_monster_origin, strength, true));
                            //Pulse to the right going down right
                            fl.add_single_sight_pulse(new SightPulse(new gridCoordinate(my_grid_position.x + 1, my_grid_position.y),
                                                            4, my_monster_origin, strength, true));
                        }
                        break;
                    case 2:
                        my_grid_position.x--;
                        if (!fl.is_tile_opaque(my_grid_position) && propogate)
                        {
                            //Pulse above going up left
                            fl.add_single_sight_pulse(new SightPulse(new gridCoordinate(my_grid_position.x, my_grid_position.y-1),
                                                            7, my_monster_origin, strength, true));
                            //Pulse below going down left
                            fl.add_single_sight_pulse(new SightPulse(new gridCoordinate(my_grid_position.x, my_grid_position.y+1),
                                                            5, my_monster_origin, strength, true));
                        }
                        break;
                    case 3:
                        my_grid_position.x++;
                        if (!fl.is_tile_opaque(my_grid_position) && propogate)
                        {
                            //Pulse above going up right
                            fl.add_single_sight_pulse(new SightPulse(new gridCoordinate(my_grid_position.x, my_grid_position.y - 1),
                                                            6, my_monster_origin, strength, true));
                            //Pulse below going down right
                            fl.add_single_sight_pulse(new SightPulse(new gridCoordinate(my_grid_position.x, my_grid_position.y + 1),
                                                            4, my_monster_origin, strength, true));
                        }
                        break;
                    case 4:
                        //Down Right
                        my_grid_position.y++;
                        my_grid_position.x++;
                        if (!fl.is_tile_opaque(my_grid_position) && propogate)
                        {
                            //Pulse to the left going down
                            fl.add_single_sight_pulse(new SightPulse(new gridCoordinate(my_grid_position.x - 1, my_grid_position.y),
                                                            1, my_monster_origin, strength, true));
                            //Pulse above going right
                            fl.add_single_sight_pulse(new SightPulse(new gridCoordinate(my_grid_position.x, my_grid_position.y - 1),
                                                            3, my_monster_origin, strength, true));
                        }
                        break;
                    case 5:
                        //Down Left
                        my_grid_position.y++;
                        my_grid_position.x--;
                        if (!fl.is_tile_opaque(my_grid_position) && propogate)
                        {
                            //Pulse to the right going down
                            fl.add_single_sight_pulse(new SightPulse(new gridCoordinate(my_grid_position.x + 1, my_grid_position.y),
                                                            1, my_monster_origin, strength, true));
                            //Pulse above going left
                            fl.add_single_sight_pulse(new SightPulse(new gridCoordinate(my_grid_position.x, my_grid_position.y - 1),
                                                            2, my_monster_origin, strength, true));
                        }
                        break;
                    case 6:
                        //Up Right
                        my_grid_position.y--;
                        my_grid_position.x++;
                        if (!fl.is_tile_opaque(my_grid_position) && propogate)
                        {
                            //Pulse to the left going up
                            fl.add_single_sight_pulse(new SightPulse(new gridCoordinate(my_grid_position.x - 1, my_grid_position.y),
                                                            0, my_monster_origin, strength, true));
                            //Pulse below going right
                            fl.add_single_sight_pulse(new SightPulse(new gridCoordinate(my_grid_position.x, my_grid_position.y + 1),
                                                            3, my_monster_origin, strength, true));
                        }
                        break;
                    case 7:
                        //Up Left
                        my_grid_position.y--;
                        my_grid_position.x--;
                        if (!fl.is_tile_opaque(my_grid_position) && propogate)
                        {
                            //Pulse to the right going up
                            fl.add_single_sight_pulse(new SightPulse(new gridCoordinate(my_grid_position.x + 1, my_grid_position.y),
                                                            0, my_monster_origin, strength, true));
                            //Pulse below going left
                            fl.add_single_sight_pulse(new SightPulse(new gridCoordinate(my_grid_position.x, my_grid_position.y + 1),
                                                            2, my_monster_origin, strength, true));
                        }
                        break;
                }
            }
        }
    }
}
