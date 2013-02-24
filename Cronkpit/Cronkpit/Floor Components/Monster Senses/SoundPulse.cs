using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cronkpit
{
    class SoundPulse
    {
        public enum Sound_Types { None, Player, Fanatic_Scream, Voidwraith_Scream };
        gridCoordinate my_grid_position;
        gridCoordinate sound_origin;
        Random rGen;
        int my_direction;
        int strength;
        int multiplications_remaining;
        Sound_Types my_type;

        List<gridCoordinate> my_path_taken;

        public SoundPulse(gridCoordinate start_grid_C, gridCoordinate sOrigin, 
                          int sDirection, int sStr, ref Random srGen, int sMultiplications, 
                          Sound_Types sType)
        {
            my_grid_position = new gridCoordinate(start_grid_C);
            my_direction = sDirection;
            strength = sStr;
            sound_origin = new gridCoordinate(sOrigin);
            my_path_taken = new List<gridCoordinate>();
            my_path_taken.Add(sound_origin);
            rGen = srGen;
            multiplications_remaining = sMultiplications;
            my_type = sType;
        }

        public SoundPulse(gridCoordinate start_grid_C, gridCoordinate sOrigin,
                          int sDirection, int sStr, ref Random srGen, int sMultiplications, 
                          Sound_Types sType, List<gridCoordinate> sPath)
        {
            my_grid_position = new gridCoordinate(start_grid_C);
            my_direction = sDirection;
            strength = sStr;
            sound_origin = new gridCoordinate(sOrigin);
            my_path_taken = new List<gridCoordinate>(sPath);
            rGen = srGen;
            multiplications_remaining = sMultiplications;
            my_type = sType;
        }

        public int my_strength()
        {
            return strength;
        }

        public gridCoordinate my_coord()
        {
            return my_grid_position;
        }

        public List<gridCoordinate> my_path()
        {
            return my_path_taken;
        }

        public Sound_Types get_my_type()
        {
            return my_type;
        }

        public void update(Floor fl)
        {
            //First calculate strength based on current tile. Then if strength > 0, advance 1 tile.
            //It's going to have to make new tiles every time it advances a diagonal augh.
            while(fl.does_tile_deflect(my_grid_position) && strength > 0)
                bounce(fl);
            
            strength -= fl.tile_absorbtion_value(my_grid_position);
            if(fl.isWalkable(my_grid_position))
                my_path_taken.Add(new gridCoordinate(my_grid_position));
            //directions:
            //0 = up, 1 = down, 2 = left, 3 = right
            //4 = downright, 5 = downleft, 6 = upright, 7 = upleft
            //The diagonals have to multiply unfortunately. Augh.
            if (strength > 0)
            {
                switch (my_direction)
                {
                    case 0:
                        //Up
                        my_grid_position.y--;
                        break;
                    case 1:
                        //Down
                        my_grid_position.y++;
                        break;
                    case 2:
                        //Left
                        my_grid_position.x--;
                        break;
                    case 3:
                        //Right
                        my_grid_position.x++;
                        break;
                    case 4:
                        //Down Right
                        my_grid_position.y++;
                        my_grid_position.x++;
                        break;
                    case 5:
                        //Down Left
                        my_grid_position.y++;
                        my_grid_position.x--;
                        break;
                    case 6:
                        //Up Right
                        my_grid_position.y--;
                        my_grid_position.x++;
                        break;
                    case 7:
                        //Up Left
                        my_grid_position.y--;
                        my_grid_position.x--;
                        break;
                }
                add_new_pulses(fl);
            }
        }

        public void bounce(Floor fl)
        {
            strength -= fl.tile_absorbtion_value(my_grid_position) * 2;
            int randomDir = rGen.Next(3);
            switch (my_direction)
            {
                case 0:
                    //Up
                    if (randomDir == 0)
                    {
                        my_direction = 5;
                        my_grid_position.x--;
                        my_grid_position.y++;
                    }
                    else if (randomDir == 1)
                    {
                        my_direction = 1;
                        my_grid_position.y++;
                    }
                    else if (randomDir == 2)
                    {
                        my_direction = 4;
                        my_grid_position.x++;
                        my_grid_position.y++;
                    }
                    break;
                case 1:
                    //Down
                    if (randomDir == 0)
                    {
                        my_direction = 7;
                        my_grid_position.x--;
                        my_grid_position.y--;
                    }
                    else if (randomDir == 1)
                    {
                        my_direction = 0;
                        my_grid_position.y--;
                    }
                    else if (randomDir == 2)
                    {
                        my_direction = 6;
                        my_grid_position.x++;
                        my_grid_position.y--;
                    }
                    break;
                case 2:
                    //Left
                    if (randomDir == 0)
                    {
                        my_direction = 6;
                        my_grid_position.x++;
                        my_grid_position.y--;
                    }
                    else if (randomDir == 1)
                    {
                        my_direction = 3;
                        my_grid_position.x++;
                    }
                    else if (randomDir == 2)
                    {
                        my_direction = 4;
                        my_grid_position.x++;
                        my_grid_position.y++;
                    }
                    break;
                case 3:
                    //Right
                    if (randomDir == 0)
                    {
                        my_direction = 7;
                        my_grid_position.x--;
                        my_grid_position.y--;
                    }
                    else if (randomDir == 1)
                    {
                        my_direction = 2;
                        my_grid_position.x--;
                    }
                    else if (randomDir == 2)
                    {
                        my_direction = 5;
                        my_grid_position.x--;
                        my_grid_position.y++;
                    }
                    break;
                case 4:
                    //Down Right
                    if (randomDir == 0)
                    {
                        my_direction = 0;
                        my_grid_position.y--;
                    }
                    else if (randomDir == 1)
                    {
                        my_direction = 7;
                        my_grid_position.x--;
                        my_grid_position.y--;
                    }
                    else if (randomDir == 2)
                    {
                        my_direction = 2;
                        my_grid_position.x--;
                    }
                    break;
                case 5:
                    //Down left
                    if (randomDir == 0)
                    {
                        my_direction = 0;
                        my_grid_position.y--;
                    }
                    else if (randomDir == 1)
                    {
                        my_direction = 6;
                        my_grid_position.x++;
                        my_grid_position.y--;
                    }
                    else if (randomDir == 2)
                    {
                        my_direction = 3;
                        my_grid_position.x++;
                    }
                    break;
                case 6:
                    //Up Right
                    if (randomDir == 0)
                    {
                        my_direction = 2;
                        my_grid_position.x--;
                    }
                    else if (randomDir == 1)
                    {
                        my_direction = 5;
                        my_grid_position.x--;
                        my_grid_position.y++;
                    }
                    else if (randomDir == 2)
                    {
                        my_direction = 1;
                        my_grid_position.y++;
                    }
                    break;
                case 7:
                    //Up left
                    if (randomDir == 0)
                    {
                        my_direction = 3;
                        my_grid_position.x++;
                    }
                    else if (randomDir == 1)
                    {
                        my_direction = 4;
                        my_grid_position.x++;
                        my_grid_position.y++;
                    }
                    else if (randomDir == 2)
                    {
                        my_direction = 1;
                        my_grid_position.y++;
                    }
                    break;
            }
        }

        public void add_new_pulses(Floor fl)
        {
            multiplications_remaining--;
            if (multiplications_remaining > 0)
            {
                switch (my_direction)
                {
                    case 0:
                        //Up
                        fl.add_single_sound_pulse(new SoundPulse(my_grid_position, sound_origin, 7, strength, ref rGen, multiplications_remaining, my_type, my_path_taken));
                        fl.add_single_sound_pulse(new SoundPulse(my_grid_position, sound_origin, 6, strength, ref rGen, multiplications_remaining, my_type, my_path_taken));
                        break;
                    case 1:
                        //Down
                        fl.add_single_sound_pulse(new SoundPulse(my_grid_position, sound_origin, 5, strength, ref rGen, multiplications_remaining, my_type, my_path_taken));
                        fl.add_single_sound_pulse(new SoundPulse(my_grid_position, sound_origin, 4, strength, ref rGen, multiplications_remaining, my_type, my_path_taken));
                        break;
                    case 2:
                        //Left
                        fl.add_single_sound_pulse(new SoundPulse(my_grid_position, sound_origin, 7, strength, ref rGen, multiplications_remaining, my_type, my_path_taken));
                        fl.add_single_sound_pulse(new SoundPulse(my_grid_position, sound_origin, 5, strength, ref rGen, multiplications_remaining, my_type, my_path_taken));
                        break;
                    case 3:
                        //Right
                        fl.add_single_sound_pulse(new SoundPulse(my_grid_position, sound_origin, 6, strength, ref rGen, multiplications_remaining, my_type, my_path_taken));
                        fl.add_single_sound_pulse(new SoundPulse(my_grid_position, sound_origin, 4, strength, ref rGen, multiplications_remaining, my_type, my_path_taken));
                        break;
                    case 4:
                        //Down Right
                        fl.add_single_sound_pulse(new SoundPulse(my_grid_position, sound_origin, 3, strength, ref rGen, multiplications_remaining, my_type, my_path_taken));
                        fl.add_single_sound_pulse(new SoundPulse(my_grid_position, sound_origin, 1, strength, ref rGen, multiplications_remaining, my_type, my_path_taken));
                        break;
                    case 5:
                        //Down Left
                        fl.add_single_sound_pulse(new SoundPulse(my_grid_position, sound_origin, 2, strength, ref rGen, multiplications_remaining, my_type, my_path_taken));
                        fl.add_single_sound_pulse(new SoundPulse(my_grid_position, sound_origin, 1, strength, ref rGen, multiplications_remaining, my_type, my_path_taken));
                        break;
                    case 6:
                        //Up Right
                        fl.add_single_sound_pulse(new SoundPulse(my_grid_position, sound_origin, 0, strength, ref rGen, multiplications_remaining, my_type, my_path_taken));
                        fl.add_single_sound_pulse(new SoundPulse(my_grid_position, sound_origin, 3, strength, ref rGen, multiplications_remaining, my_type, my_path_taken));
                        break;
                    case 7:
                        //Up Left
                        fl.add_single_sound_pulse(new SoundPulse(my_grid_position, sound_origin, 2, strength, ref rGen, multiplications_remaining, my_type, my_path_taken));
                        fl.add_single_sound_pulse(new SoundPulse(my_grid_position, sound_origin, 0, strength, ref rGen, multiplications_remaining, my_type, my_path_taken));
                        break;
                }
            }
        }
    }
}
