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
    class Monster
    {
        public Texture2D my_Texture;
        //FOR DEBUGGING PURPOSES - PERFECTLY SAFE TO UNCOMMENT
        //public Texture2D sound_stuff;
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
        //Sound
        public bool can_hear;
        public bool heard_something;
        public List<gridCoordinate> shortest_path_to_sound;
        public List<gridCoordinate> last_path_to_sound;
        public int listen_threshold;

        //Other stuff
        public bool can_melee_attack;
        public bool active;
        public int speed_numerator;
        public int speed_denominator;
        public bool has_moved;
        public string my_name;
        protected int melee_dodge;
        protected int ranged_dodge;

        //Damage related - will be overhauling later.
        public int hitPoints;
        public int my_Index;
        public int min_damage;
        public int max_damage;
        public Attack.Damage dmg_type;
        public wound.Wound_Type wound_type;

        public Monster(gridCoordinate sGridCoord, ContentManager sCont, int sIndex)
        {
            cont = sCont;
            my_grid_coord = new gridCoordinate(sGridCoord);
            strongest_smell_coord = new gridCoordinate(sGridCoord);
            my_Position = new Vector2(sGridCoord.x * 32, sGridCoord.y * 32);
            rGen = new Random();
            my_Index = sIndex;

            // FOR DEBUGGING PURPOSES - PERFECTLY SAFE TO UNCOMMENT
            //sound_stuff = sCont.Load<Texture2D>("sound shit");

            //Sensory stuff
            //Sight
            sight_range = 0;
            //Smell
            has_scent = false;
            smell_range = 0;
            //Sound
            can_hear = false;
            heard_something = false;
            shortest_path_to_sound = new List<gridCoordinate>();
            last_path_to_sound = new List<gridCoordinate>();
            listen_threshold = 0;
            
            //Damage stuff
            can_melee_attack = false;
            hitPoints = 0;
            min_damage = 0;
            max_damage = 0;

            //other
            /*
             * Brief explanation of how this works. if you set the denominator to
             * any nonzero number and increment the numerator every turn it means that the monster
             * will take [denominator] actions every [denominator+1] turns. So for example setting it
             * to 2 would mean that the monster will take 2 actions every 3 turns.
             */
            active = false;
            speed_numerator = 0;
            speed_denominator = 0;
            melee_dodge = 0;
            ranged_dodge = 0;
            my_name = "";
        }

        //don't call unless you've started the spritebatch!
        public void drawMe(ref SpriteBatch sb)
        {
            sb.Draw(my_Texture, my_Position, Color.White);
            // FOR DEBUGGING PURPOSES - PERFECTLY SAFE TO UNCOMMENT.
            //for (int i = 0; i < shortest_path_to_sound.Count; i++)
            //{
                //sb.Draw(sound_stuff, new Vector2(shortest_path_to_sound[i].x*32, shortest_path_to_sound[i].y*32), Color.White);
            //}
             
        }

        //overidden on a per-monster basis. The goal is to ONLY have this function
        //in each monster class.
        public virtual void Update_Monster(Player pl, Floor fl)
        {
        }

        //drawing stuff. Maybe make more advanced later.
        public void snap_to_grid()
        {
            my_Position.X = my_grid_coord.x * 32;
            my_Position.Y = my_grid_coord.y * 32;
        }

        public void increment_my_drawing_position(float delta_time)
        {
            Vector2 destination = new Vector2(my_grid_coord.x * 32, my_grid_coord.y * 32);
            Vector2 direction = destination - my_Position;
            direction.Normalize();

            if (within_pixels_of_destination(3, destination))
                snap_to_grid();
            else
            {
                my_Position.X += (direction.X * delta_time) * 160;
                my_Position.Y += (direction.Y * delta_time) * 160;
            }
        }

        public bool within_pixels_of_destination(int pixels, Vector2 destination)
        {
            float xdif = 0;
            float ydif = 0;
            if (destination.X > my_Position.X)
                xdif = destination.X - my_Position.X;
            else
                xdif = my_Position.X - destination.X;

            if (destination.Y > my_Position.Y)
                ydif = destination.Y - my_Position.Y;
            else
                ydif = my_Position.Y - destination.Y;

            return xdif <= pixels && ydif <= pixels;
        }

        public bool at_destination()
        {
            if (my_Position.X / 32 == my_grid_coord.x && my_Position.Y / 32 == my_grid_coord.y)
                return true;
            else
                return false;
        }

        //All positioning stuff.
        public bool am_i_on_player(gridCoordinate targetpoint, Player pl)
        {
            return (targetpoint.x == pl.get_my_grid_C().x &&
                    targetpoint.y == pl.get_my_grid_C().y);
        }

        public bool is_spot_free(gridCoordinate targetpoint, Floor fl, Player pl)
        {
            return (am_i_on_player(targetpoint, pl) == false && 
                    fl.isWalkable(targetpoint) &&
                    fl.am_i_on_other_monster(targetpoint, my_Index) == false);
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
                        if (is_spot_free(my_grid_coord, fl, pl))
                        {
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
                        if (is_spot_free(my_grid_coord, fl, pl))
                        {
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
                        if (is_spot_free(my_grid_coord, fl, pl))
                        {
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
                        if (is_spot_free(my_grid_coord, fl, pl))
                        {
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
                        if (is_spot_free(my_grid_coord, fl, pl))
                        {
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
                        if (is_spot_free(my_grid_coord, fl, pl))
                        {
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
                        if (is_spot_free(my_grid_coord, fl, pl))
                        {
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
                        if (is_spot_free(my_grid_coord, fl, pl))
                        {
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
        public void advance_towards_single_point(gridCoordinate target_point, Player pl, Floor fl, int mode)
        {
            //mode 0 = precise, mode 1 = nonprecise
            has_moved = false;

            if(mode == 0)
                if ((my_grid_coord.x != target_point.x) || (my_grid_coord.y != target_point.y))
                    advance_xy_position(target_point, pl, fl);
            
            if(mode == 1)
                if((my_grid_coord.x < target_point.x-1 || my_grid_coord.x > target_point.x+1) ||
                (my_grid_coord.y < target_point.y-1 || my_grid_coord.y > target_point.y+1))
                    advance_xy_position(target_point, pl, fl);


            #region large comment
            /*
            #region directions 5, 1, 6

            if (my_grid_coord.x < target_point.x && my_grid_coord.y < target_point.y && !has_moved)
            {
                my_grid_coord.x++;
                my_grid_coord.y++;
                if (is_spot_free(fl, pl))
                {
                    reset_my_drawing_position();
                    has_moved = true;
                }
                else
                    my_grid_coord = oldCoord;
            }

            if (my_grid_coord.x > target_point.x && my_grid_coord.y < target_point.y && !has_moved)
            {
                my_grid_coord.x--;
                my_grid_coord.y++;
                if (is_spot_free(fl, pl))
                {
                    reset_my_drawing_position();
                    has_moved = true;
                }
                else
                    my_grid_coord = oldCoord;
            }

            if (my_grid_coord.x == target_point.x && my_grid_coord.y < target_point.y && !has_moved)
            {
                my_grid_coord.y++;
                if (is_spot_free(fl, pl))
                {
                    reset_my_drawing_position();
                    has_moved = true;
                }
                else
                    my_grid_coord = oldCoord;
            }

            #endregion

            #region directions 7, 0, 6

            if (my_grid_coord.x > target_point.x && my_grid_coord.y > target_point.y && !has_moved)
            {
                my_grid_coord.x--;
                my_grid_coord.y--;
                if (is_spot_free(fl, pl))
                {
                    reset_my_drawing_position();
                    has_moved = true;
                }
                else
                    my_grid_coord = oldCoord;
            }

            if (my_grid_coord.x < target_point.x && my_grid_coord.y > target_point.y && !has_moved)
            {
                my_grid_coord.x++;
                my_grid_coord.y--;
                if (is_spot_free(fl, pl))
                {
                    reset_my_drawing_position();
                    has_moved = true;
                }
                else
                    my_grid_coord = oldCoord;
            }

            if (my_grid_coord.x == target_point.x && my_grid_coord.y > target_point.y && !has_moved)
            {
                my_grid_coord.y--;
                if (is_spot_free(fl, pl))
                {
                    reset_my_drawing_position();
                    has_moved = true;
                }
                else
                    my_grid_coord = oldCoord;
            }

            #endregion

            #region directions 2, 3

            if (my_grid_coord.x > target_point.x && my_grid_coord.y == target_point.y && !has_moved)
            {
                my_grid_coord.x--;
                if (is_spot_free(fl, pl))
                {
                    reset_my_drawing_position();
                    has_moved = true;
                }
                else
                    my_grid_coord = oldCoord;
            }

            if (my_grid_coord.x < target_point.x && my_grid_coord.y == target_point.y && !has_moved)
            {
                my_grid_coord.x++;
                if (is_spot_free(fl, pl))
                {
                    reset_my_drawing_position();
                    has_moved = true;
                }
                else
                    my_grid_coord = oldCoord;
            }

            #endregion
             */
            #endregion
        }

        public void advance_xy_position(gridCoordinate target_point, Player pl, Floor fl)
        {
            int x_move = 0;
            int y_move = 0;
            if (my_grid_coord.x < target_point.x)
                x_move = 1;
            else if (my_grid_coord.x > target_point.x)
                x_move = -1;

            if (my_grid_coord.y < target_point.y)
                y_move = 1;
            else if (my_grid_coord.y > target_point.y)
                y_move = -1;

            gridCoordinate test_grid_coord = new gridCoordinate(my_grid_coord);
            test_grid_coord.x += x_move;
            test_grid_coord.y += y_move;

            if (is_spot_free(test_grid_coord, fl, pl))
            {
                my_grid_coord = test_grid_coord;
                has_moved = true;
            }
            else
            {
                gridCoordinate test_grid_coord_xaxis = new gridCoordinate(my_grid_coord);
                gridCoordinate test_grid_coord_yaxis = new gridCoordinate(my_grid_coord);
                test_grid_coord_xaxis.x += x_move;
                test_grid_coord_yaxis.y += y_move;

                bool xaxis_free = false;
                bool yaxis_free = false;
                if (is_spot_free(test_grid_coord_xaxis, fl, pl) && x_move != 0)
                    xaxis_free = true;
                if (is_spot_free(test_grid_coord_yaxis, fl, pl) && y_move != 0)
                    yaxis_free = true;

                if (xaxis_free && !yaxis_free)
                {
                    my_grid_coord = test_grid_coord_xaxis;
                    has_moved = true;
                }
                else if (!xaxis_free && yaxis_free)
                {
                    my_grid_coord = test_grid_coord_yaxis;
                    has_moved = true;
                }
                else if (xaxis_free && yaxis_free)
                {
                    int directionpick = rGen.Next(2);
                    if (directionpick == 0)
                    {
                        my_grid_coord = test_grid_coord_xaxis;
                        has_moved = true;
                    }

                    if (directionpick == 1)
                    {
                        my_grid_coord = test_grid_coord_yaxis;
                        has_moved = true;
                    }
                }
            }
        }
        //damage stuff
        public void takeDamage(int dmg, bool melee_attack, List<string> msg_buf, Floor fl)
        {
            bool dodged = false;
            int dodge_roll = rGen.Next(100);

            if (melee_attack && dodge_roll < melee_dodge)
                dodged = true;
            else if (!melee_attack && dodge_roll < ranged_dodge)
                dodged = true;

            if (!dodged)
            {
                msg_buf.Add("The " + my_name + " takes " + dmg + " damage!");
                fl.add_new_popup("-" + dmg, Popup.popup_msg_color.Red, my_grid_coord);
                hitPoints -= dmg;
            }
            else
            {
                msg_buf.Add("The " + my_name + " dodges your attack!");
                fl.add_new_popup("Dodged!", Popup.popup_msg_color.LimeGreen, my_grid_coord);
            }
        }

        public Attack dealDamage()
        {
            int dmgValue = rGen.Next(min_damage, (max_damage+1));
            return new Attack(dmg_type, new wound(wound_type, dmgValue));
        }

        //now for the good stuff - SENSORY STUFF.
        //Probably not going to use this.
        public bool is_smell_i_can_smell_within(gridCoordinate my_grid_coord, Floor fl, int targetSmell, int smell_threshold, int radius)
        {
            return fl.check_for_smellable_smell(my_grid_coord, targetSmell, smell_threshold, radius);
        }

        public bool is_player_within(Player pl, int radius)
        {
            if (pl.get_my_grid_C().x >= (my_grid_coord.x - radius) &&
                pl.get_my_grid_C().x <= (my_grid_coord.x + radius) &&
                pl.get_my_grid_C().y >= (my_grid_coord.y - radius) &&
                pl.get_my_grid_C().y <= (my_grid_coord.y + radius))
                return true;
            else
                return false;
        }

        public bool is_player_within_diamond(Player pl, int radius)
        {
            //get the difference between the player's x and the monster's x.
            //get the difference between the player's y and the monster's y.
            //Add the two differences together
            //If the difference is less than or equal to the radius we're good!
            gridCoordinate player_gc = pl.get_my_grid_C();
            int xdifference = Math.Abs(player_gc.x - my_grid_coord.x);
            int ydifference = Math.Abs(player_gc.y - my_grid_coord.y);

            return (xdifference + ydifference) <= radius;
        }

        //Sight pulse to find player
        public void look_for_player(Floor fl, Player pl, int sight_range)
        {
            //fl.sight_pulse(my_grid_coord, pl, my_Index, sight_range);
            fl.sight_pulse_raycast(my_grid_coord, pl, this, sight_range);
        }

        //Scent pulse to find highest smell tile
        public void sniff_for_trail(Floor fl, int target_scent, int smell_range, int smell_threshold)
        {
            fl.scent_pulse_raycast(my_grid_coord, target_scent, this, smell_range, smell_threshold);
        }

        //Get a new path to the sound
        public void next_path_to_sound(List<gridCoordinate> path)
        {
            if (!heard_something)
            {
                shortest_path_to_sound = new List<gridCoordinate>(path);
                heard_something = true;
            }
            else if (heard_something && path.Count < shortest_path_to_sound.Count)
            {
                shortest_path_to_sound = new List<gridCoordinate>(path);
            }
        }

        //Follow the path to the sound
        public void follow_path_to_sound(Floor fl, Player pl)
        {
            //Get rid of the coordinate that we're standing on.
            if (heard_something)
            {
                last_path_to_sound = new List<gridCoordinate>(shortest_path_to_sound);
                shortest_path_to_sound.Clear();
                heard_something = false;
            }
            int path_length = last_path_to_sound.Count - 1;
             
            if (path_length >= 0)
            {
                advance_towards_single_point(last_path_to_sound[path_length], pl, fl, 0);
                if (my_grid_coord.x == last_path_to_sound[path_length].x &&
                    my_grid_coord.y == last_path_to_sound[path_length].y)
                {
                    last_path_to_sound.RemoveAt(path_length);
                }
            }
        }
    }
}
