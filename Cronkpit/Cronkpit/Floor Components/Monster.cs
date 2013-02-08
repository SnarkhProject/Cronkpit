﻿using System;
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
        public enum Monster_Type { Undead, Human, Ethereal, Grendel, Elf };
        public enum Monster_Size { Normal, Large, Huge };
        public Texture2D my_Texture;
        //FOR DEBUGGING PURPOSES - PERFECTLY SAFE TO UNCOMMENT
        //public Texture2D sound_stuff;
        public Vector2 my_Position;
        public Monster_Size my_monster_size;
        public ContentManager cont;
        protected List<gridCoordinate> my_grid_coords;
        public Random rGen;
        //Strictly for movement purposes
        List<int[]> movement_indexes;
        List<gridCoordinate.direction> direction_indexes;

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
        public List<int> sounds_i_can_hear;
        /*
         * For reference: 0 = player sound, 1 = zombie scream, 2 = voidwraith scream
         */
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
        protected int armor_effectiveness;
        protected bool corporeal;

        //Damage related - will be overhauling later.
        public int hitPoints;
        public int armorPoints;
        public int my_Index;
        public int min_damage;
        public int max_damage;
        public Attack.Damage dmg_type;
        public wound.Wound_Type wound_type;

        public Monster(gridCoordinate sGridCoord, ContentManager sCont, int sIndex, Monster_Size monSize)
        {
            my_grid_coords = new List<gridCoordinate>();
            cont = sCont;
            switch (monSize)
            {
                case Monster_Size.Normal:
                    my_grid_coords.Add(new gridCoordinate(sGridCoord));
                    my_monster_size = Monster_Size.Normal;
                    break;
                case Monster_Size.Large:
                    my_grid_coords.Add(new gridCoordinate(sGridCoord));
                    my_grid_coords.Add(new gridCoordinate(sGridCoord.x+1, sGridCoord.y));
                    my_grid_coords.Add(new gridCoordinate(sGridCoord.x, sGridCoord.y+1));
                    my_grid_coords.Add(new gridCoordinate(sGridCoord.x+1, sGridCoord.y+1));
                    my_monster_size = Monster_Size.Large;
                    break;
                default:
                    my_grid_coords.Add(new gridCoordinate(sGridCoord));
                    break;
            }

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
            sounds_i_can_hear = new List<int>();
            listen_threshold = 0;
            
            //Damage stuff
            can_melee_attack = false;
            hitPoints = 0;
            armorPoints = 0;
            min_damage = 0;
            max_damage = 0;
            corporeal = true;

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
            armor_effectiveness = 0;
            my_name = "";

            movement_indexes = new List<int[]>();
            direction_indexes = new List<gridCoordinate.direction>();
            build_movement_and_direction_indexes();
        }

        public void build_movement_and_direction_indexes()
        {
            movement_indexes.Add(new int[] { 1, 0 });
            movement_indexes.Add(new int[] { 1, -1 });
            movement_indexes.Add(new int[] { 0, -1 });
            movement_indexes.Add(new int[] { -1, -1 });
            movement_indexes.Add(new int[] { -1, 0 });
            movement_indexes.Add(new int[] { -1, 1 });
            movement_indexes.Add(new int[] { 0, 1 });
            movement_indexes.Add(new int[] { 1, 1 });

            direction_indexes.Add(gridCoordinate.direction.Right);
            direction_indexes.Add(gridCoordinate.direction.UpRight);
            direction_indexes.Add(gridCoordinate.direction.Up);
            direction_indexes.Add(gridCoordinate.direction.UpLeft);
            direction_indexes.Add(gridCoordinate.direction.Left);
            direction_indexes.Add(gridCoordinate.direction.DownLeft);
            direction_indexes.Add(gridCoordinate.direction.Down);
            direction_indexes.Add(gridCoordinate.direction.DownRight);
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
            //my_grid_coords[0] should always be the upper left of the monster.
            my_Position.X = my_grid_coords[0].x * 32;
            my_Position.Y = my_grid_coords[0].y * 32;
        }

        public Vector2 my_center_coordinate()
        {
            switch (my_monster_size)
            {
                case Monster_Size.Large:
                    return new Vector2(my_grid_coords[0].x * 32 + 32, my_grid_coords[0].y * 32 + 32);
                case Monster_Size.Huge:
                    return new Vector2(my_grid_coords[0].x * 32 + 48, my_grid_coords[0].y * 32 + 48);
                default:
                    return new Vector2(my_grid_coords[0].x * 32 + 16, my_grid_coords[0].y * 32 + 16);
            }
        }

        public void increment_my_drawing_position(float delta_time)
        {
            Vector2 destination = new Vector2(my_grid_coords[0].x * 32, my_grid_coords[0].y * 32);
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
            if (my_Position.X / 32 == my_grid_coords[0].x && my_Position.Y / 32 == my_grid_coords[0].y)
                return true;
            else
                return false;
        }

        //All positioning stuff.
        public List<gridCoordinate> get_my_grid_coords()
        {
            return my_grid_coords;
        }

        public gridCoordinate randomly_chosen_personal_coord()
        {
            int rCoord = rGen.Next(my_grid_coords.Count);
            return my_grid_coords[rCoord];
        }

        public bool occupies_tile(gridCoordinate targetPoint)
        {
            for (int i = 0; i < my_grid_coords.Count; i++)
            {
                if (my_grid_coords[i].x == targetPoint.x &&
                   my_grid_coords[i].y == targetPoint.y)
                    return true;
            }

            return false;
        }

        public bool am_i_on_player(gridCoordinate targetpoint, Player pl)
        {
            return (targetpoint.x == pl.get_my_grid_C().x &&
                    targetpoint.y == pl.get_my_grid_C().y);
        }

        public bool is_spot_free(gridCoordinate targetpoint, Floor fl, Player pl, bool corporeal)
        {
            if(corporeal)
                return (!am_i_on_player(targetpoint, pl) && 
                        fl.isWalkable(targetpoint) &&
                        !fl.am_i_on_other_monster(targetpoint, my_Index));
            else
                return (!am_i_on_player(targetpoint, pl) && 
                        !fl.am_i_on_other_monster(targetpoint, my_Index) &&
                        !fl.is_void_tile(targetpoint));
        }

        //pick a random direction and walk 1 square in it.
        //Will retry 5 times if the square is full.
        public void wander(Player pl, Floor fl, bool corporeal)
        {
            bool walked = false;
            int tries = 0;
            while (tries < 5 && !walked)
            {
                gridCoordinate.direction target_direction = 0;
                int numeric_direction = rGen.Next(8);
                switch (numeric_direction)
                {
                    //up, y-
                    case 0:
                        target_direction = gridCoordinate.direction.Up;
                        break;
                    //down, y+
                    case 1:
                        target_direction = gridCoordinate.direction.Down;
                        break;
                    //left, x-
                    case 2:
                        target_direction = gridCoordinate.direction.Left;
                        break;
                    //right, x+
                    case 3:
                        target_direction = gridCoordinate.direction.Right;
                        break;
                    //down right, x+ y+
                    case 4:
                        target_direction = gridCoordinate.direction.DownRight;
                        break;
                    //down left, x- y+
                    case 5:
                        target_direction = gridCoordinate.direction.DownLeft;
                        break;
                    //up right, x+ y-
                    case 6:
                        target_direction = gridCoordinate.direction.UpRight;
                        break;
                    //up left, x- y-
                    case 7:
                        target_direction = gridCoordinate.direction.UpLeft;
                        break;
                }

                List<gridCoordinate> test_coordinates = new List<gridCoordinate>();
                for (int i = 0; i < my_grid_coords.Count; i++)
                    test_coordinates.Add(new gridCoordinate(my_grid_coords[i], target_direction));

                walked = true;
                for (int i = 0; i < my_grid_coords.Count; i++)
                    if (!is_spot_free(test_coordinates[i], fl, pl, corporeal))
                        walked = false;

                if (walked)
                    my_grid_coords = test_coordinates;
            }
        }

        //advance towards the target point - usually the player.
        //If the monster collides with the player, it attacks them in melee if it can.
        public void advance_towards_single_point(gridCoordinate target_point, Player pl, Floor fl, int mode, bool corporeal)
        {
            //mode 0 = precise, mode 1 = nonprecise
            has_moved = false;

            if(mode == 0)
                if (!occupies_tile(target_point))
                    advance_xy_position(target_point, pl, fl, corporeal);

            if (mode == 1)
            {
                List<gridCoordinate> tiles_to_check = new List<gridCoordinate>();
                tiles_to_check.Add(new gridCoordinate(target_point.x - 1, target_point.y));
                tiles_to_check.Add(new gridCoordinate(target_point.x + 1, target_point.y));
                tiles_to_check.Add(new gridCoordinate(target_point.x, target_point.y - 1));
                tiles_to_check.Add(new gridCoordinate(target_point.x, target_point.y + 1));
                bool at_coordinate = false;
                for (int i = 0; i < tiles_to_check.Count; i++)
                    if (occupies_tile(tiles_to_check[i]))
                        at_coordinate = true;

                if(!at_coordinate)
                    advance_xy_position(target_point, pl, fl, corporeal);
            }
        }

        public void advance_xy_position(gridCoordinate target_point, Player pl, Floor fl, bool corporeal)
        {
            has_moved = false;
            gridCoordinate top_left_edge = new gridCoordinate(-1, -1);
            gridCoordinate top_right_edge = new gridCoordinate(-1, -1);
            gridCoordinate bottom_left_edge = new gridCoordinate(-1, -1);
            gridCoordinate bottom_right_edge = new gridCoordinate(-1, -1);
            switch(my_monster_size)
            {
                case Monster_Size.Normal:
                    top_left_edge = my_grid_coords[0];
                    top_right_edge = my_grid_coords[0];
                    bottom_left_edge = my_grid_coords[0];
                    bottom_right_edge = my_grid_coords[0];
                    break;
                case Monster_Size.Large:
                    top_left_edge = my_grid_coords[0];
                    top_right_edge = my_grid_coords[1];
                    bottom_left_edge = my_grid_coords[2];
                    bottom_right_edge = my_grid_coords[3];
                    break;
                case Monster_Size.Huge:
                    top_left_edge = my_grid_coords[0];
                    top_right_edge = my_grid_coords[2];
                    bottom_left_edge = my_grid_coords[6];
                    bottom_right_edge = my_grid_coords[8];
                    break;
            }

            int x_move = 0;
            int y_move = 0;
            if (top_left_edge.x < target_point.x && top_right_edge.x < target_point.x)
                x_move = 1;
            else if (top_left_edge.x > target_point.x && top_right_edge.x > target_point.x)
                x_move = -1;

            if (top_left_edge.y < target_point.y && bottom_left_edge.y < target_point.y)
                y_move = 1;
            else if (top_left_edge.y > target_point.y && bottom_left_edge.y > target_point.y)
                y_move = -1;

            gridCoordinate.direction desired_direction = 0;
            for(int i = 0; i < 8; i++)
            {
                if(movement_indexes[i][0] == x_move && movement_indexes[i][1] == y_move)
                    desired_direction = direction_indexes[i];
            }

            List<gridCoordinate> desired_grid_coords = new List<gridCoordinate>();
            List<gridCoordinate> diagonal_1_grid_coords = new List<gridCoordinate>();
            List<gridCoordinate> diagonal_2_grid_coords = new List<gridCoordinate>();
            for (int i = 0; i < my_grid_coords.Count; i++)
            {
                desired_grid_coords.Add(new gridCoordinate(my_grid_coords[i], desired_direction));
                diagonal_1_grid_coords.Add(new gridCoordinate(my_grid_coords[i], (desired_direction) + 1 % 8));
                diagonal_2_grid_coords.Add(new gridCoordinate(my_grid_coords[i], (desired_direction) + 7 % 8));
            }

            bool desired_direction_valid = true;
            for(int i = 0; i < desired_grid_coords.Count; i++)
                if(!is_spot_free(desired_grid_coords[i], fl, pl, corporeal))
                    desired_direction_valid = false;

            if (desired_direction_valid)
            {
                my_grid_coords = desired_grid_coords;
                has_moved = true;
            }
            else
            {
                bool diagonal_1_valid = true;
                bool diagonal_2_valid = true;
                for (int i = 0; i < my_grid_coords.Count; i++)
                {
                    if (!is_spot_free(diagonal_1_grid_coords[i], fl, pl, corporeal))
                        diagonal_1_valid = false;
                    if (!is_spot_free(diagonal_2_grid_coords[i], fl, pl, corporeal))
                        diagonal_2_valid = false;
                }

                if (diagonal_1_valid && !diagonal_2_valid)
                {
                    my_grid_coords = diagonal_1_grid_coords;
                    has_moved = true;
                }
                else if (!diagonal_1_valid && diagonal_2_valid)
                {
                    my_grid_coords = diagonal_2_grid_coords;
                    has_moved = true;
                }
                else
                {
                    int use_diagonal_1 = rGen.Next(2);
                    if (use_diagonal_1 == 1)
                        my_grid_coords = diagonal_1_grid_coords;
                    else
                        my_grid_coords = diagonal_2_grid_coords;
                    has_moved = true;
                }
            }
        }
        //damage stuff
        public void takeDamage(int dmg, bool melee_attack, List<string> msg_buf, Floor fl)
        {
            bool dodged = false;
            bool absorbed = false;
            int dodge_roll = rGen.Next(100);
            int armor_roll = rGen.Next(100);

            if (melee_attack && dodge_roll < melee_dodge)
                dodged = true;
            else if (!melee_attack && dodge_roll < ranged_dodge)
                dodged = true;

            if (armorPoints > 0  && armor_roll < armor_effectiveness)
                absorbed = true;

            if (!dodged)
            {
                if (absorbed)
                {
                    if (armorPoints >= dmg)
                    {
                        msg_buf.Add("The " + my_name + "'s armor absorbs " + dmg + " damage!");
                        fl.add_new_popup("-" + dmg, Popup.popup_msg_color.Blue, my_center_coordinate());
                        armorPoints -= dmg;
                    }
                    else
                    {
                        fl.add_new_popup("-" + armorPoints, Popup.popup_msg_color.Blue, my_center_coordinate());
                        dmg -= armorPoints;
                        fl.add_new_popup("-" + dmg, Popup.popup_msg_color.Red, my_center_coordinate());
                        msg_buf.Add("The " + my_name + "'s armor absorbs " + armorPoints + " damage!");
                        msg_buf.Add("The " + my_name + " takes " + dmg + " damage!");
                        armorPoints = 0;
                        hitPoints -= dmg;
                    }
                }
                else
                {
                    msg_buf.Add("The " + my_name + " takes " + dmg + " damage!");
                    fl.add_new_popup("-" + dmg, Popup.popup_msg_color.Red, my_center_coordinate());
                    hitPoints -= dmg;
                }
            }
            else
            {
                msg_buf.Add("The " + my_name + " dodges your attack!");
                fl.add_new_popup("Dodged!", Popup.popup_msg_color.LimeGreen, my_center_coordinate());
            }
        }

        public bool shove(int xDirection, int yDirection, Player pl, Floor fl)
        {
            gridCoordinate.direction shove_dir = 0;
            for (int i = 0; i < 8; i++)
            {
                if (movement_indexes[i][0] == xDirection && movement_indexes[i][1] == yDirection)
                    shove_dir = direction_indexes[i];
            }

            List<gridCoordinate> test_coords = new List<gridCoordinate>();
            for (int i = 0; i < my_grid_coords.Count; i++)
                test_coords.Add(new gridCoordinate(my_grid_coords[i], shove_dir));

            bool valid_location = true;
            for(int i = 0; i < test_coords.Count; i++)
                if(!is_spot_free(test_coords[i], fl, pl, corporeal))
                    valid_location = false;

            if (valid_location)
            {
                my_grid_coords = test_coords;
                snap_to_grid();
                return true;
            }
            else
            {
                return false;
            }
        }

        public Attack dealDamage()
        {
            int dmgValue = rGen.Next(min_damage, (max_damage+1));
            return new Attack(dmg_type, new wound(wound_type, dmgValue));
        }

        //now for the good stuff - SENSORY STUFF.
        //Probably not going to use this.
        public Tile strongest_smell_within(Floor fl, int targetSmell, int smell_threshold, int radius)
        {
            List<gridCoordinate> smelled_from = new List<gridCoordinate>();
            for (int i = 0; i < my_grid_coords.Count; i++)
                if (fl.check_for_smellable_smell(my_grid_coords[i], targetSmell, smell_threshold, radius))
                    smelled_from.Add(my_grid_coords[i]);

            List<Tile> strongest_smells = new List<Tile>();
            for (int i = 0; i < smelled_from.Count; i++)
                strongest_smells.Add(fl.establish_los_strongest_smell(smelled_from[i], targetSmell, smell_threshold));

            int strongest_smell = -1;
            int target_index = 0;
            for (int i = 0; i < strongest_smells.Count; i++)
                if (strongest_smells[i] != null && strongest_smells[i].strength_of_scent(targetSmell) > strongest_smell)
                {
                    target_index = i;
                    strongest_smell = strongest_smells[i].strength_of_scent(targetSmell);
                }

            if (strongest_smell > -1)
                return strongest_smells[target_index];
            else
                return null;
        }

        public bool is_player_within(Player pl, int radius)
        {
            for (int i = 0; i < my_grid_coords.Count; i++)
            {
                if (pl.get_my_grid_C().x >= (my_grid_coords[i].x - radius) &&
                    pl.get_my_grid_C().x <= (my_grid_coords[i].x + radius) &&
                    pl.get_my_grid_C().y >= (my_grid_coords[i].y - radius) &&
                    pl.get_my_grid_C().y <= (my_grid_coords[i].y + radius))
                    return true;
            }

            return false;
        }

        public bool is_player_within_diamond(Player pl, int radius)
        {
            //get the difference between the player's x and the monster's x.
            //get the difference between the player's y and the monster's y.
            //Add the two differences together
            //If the difference is less than or equal to the radius we're good!
            bool within_radius = false;
            for (int i = 0; i < my_grid_coords.Count; i++)
            {
                gridCoordinate player_gc = pl.get_my_grid_C();
                int xdifference = Math.Abs(player_gc.x - my_grid_coords[i].x);
                int ydifference = Math.Abs(player_gc.y - my_grid_coords[i].y);
                if ((xdifference + ydifference) <= radius)
                    within_radius = true;
            }

            return within_radius;
        }

        public bool can_i_see_point(Floor fl, gridCoordinate target_point)
        {
            bool can_see_target = false;
            for (int i = 0; i < my_grid_coords.Count; i++)
            {
                if (fl.establish_los(my_grid_coords[i], target_point))
                    can_see_target = true;
            }

            return can_see_target;
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
                advance_towards_single_point(last_path_to_sound[path_length], pl, fl, 0, corporeal);
                if (occupies_tile(last_path_to_sound[path_length]))
                {
                    last_path_to_sound.RemoveAt(path_length);
                }
            }
        }
    }
}
