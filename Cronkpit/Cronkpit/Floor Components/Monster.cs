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
        protected List<int[]> movement_indexes;
        protected List<gridCoordinate.direction> direction_indexes;

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
        protected SoundPulse.Sound_Types last_sound_i_heard;
        protected List<SoundPulse.Sound_Types> sounds_i_can_hear;
        public List<int> listen_threshold;

        //Other stuff
        public bool can_melee_attack;
        public bool active;
        public int speed_numerator;
        public int speed_denominator;
        public bool has_moved;
        public string my_name;
        protected int melee_dodge;
        protected int ranged_dodge;
        protected double modified_melee_dodge;
        protected double modified_ranged_dodge;
        protected bool dodge_values_degrade;
        protected int armor_effectiveness;
        protected bool corporeal;
        protected bool smart_monster;
        public bool boss_monster;

        //Status afflictions
        protected int stunned_turns_remaining;
        protected int rooted_turns_remaining;
        protected int disrupted_turns_remaining;

        //Damage related - will be overhauling later.
        protected int max_hitPoints;
        public int hitPoints;
        public int armorPoints;
        public int my_Index;
        public int min_damage;
        public int max_damage;
        public Attack.Damage dmg_type;

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
            sounds_i_can_hear = new List<SoundPulse.Sound_Types>();
            listen_threshold = new List<int>();
            last_sound_i_heard = SoundPulse.Sound_Types.None;
            
            //Damage stuff
            can_melee_attack = false;
            max_hitPoints = 0;
            hitPoints = 0;
            armorPoints = 0;
            min_damage = 0;
            max_damage = 0;
            corporeal = true;
            dodge_values_degrade = true;

            //status affliction stuff
            stunned_turns_remaining = 0;
            rooted_turns_remaining = 0;
            disrupted_turns_remaining = 0;

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
            smart_monster = false;
            boss_monster = false;

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
        //in each monster class, along with other monster-specific functions
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
            bool spot_free = true;
            if(corporeal)
                spot_free = (!am_i_on_player(targetpoint, pl) && 
                             fl.isWalkable(targetpoint) &&
                             !fl.am_i_on_other_monster(targetpoint, my_Index));
            else
                spot_free = (!am_i_on_player(targetpoint, pl) && 
                             !fl.am_i_on_other_monster(targetpoint, my_Index) &&
                             !fl.is_void_tile(targetpoint));

            if(smart_monster)
                fl.open_door_here(targetpoint);

            return spot_free;
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
                    {
                        walked = false;
                        tries++;
                    }

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
                tiles_to_check.Add(new gridCoordinate(target_point.x - 1, target_point.y - 1));
                tiles_to_check.Add(new gridCoordinate(target_point.x + 1, target_point.y - 1));
                tiles_to_check.Add(new gridCoordinate(target_point.x + 1, target_point.y + 1));
                tiles_to_check.Add(new gridCoordinate(target_point.x - 1, target_point.y + 1));
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

            int target_index = -1;
            for(int i = 0; i < 8; i++)
            {
                if(movement_indexes[i][0] == x_move && movement_indexes[i][1] == y_move)
                    target_index = i;
            }
            //Slightly less efficient than just delcaring these in the gridCoord arrays, but
            //It's way, way easier to debug like this, since you can tell what it's doing before it
            //actually gets into those sections.
            if (target_index != -1)
            {
                gridCoordinate.direction desired_direction = direction_indexes[target_index];
                gridCoordinate.direction diagonal_1_direction = direction_indexes[(target_index + 1) % 8];
                gridCoordinate.direction diagonal_2_direction = direction_indexes[(target_index + 7) % 8];

                List<gridCoordinate> desired_grid_coords = new List<gridCoordinate>();
                List<gridCoordinate> diagonal_1_grid_coords = new List<gridCoordinate>();
                List<gridCoordinate> diagonal_2_grid_coords = new List<gridCoordinate>();

                for (int i = 0; i < my_grid_coords.Count; i++)
                {
                    desired_grid_coords.Add(new gridCoordinate(my_grid_coords[i], desired_direction));
                    diagonal_1_grid_coords.Add(new gridCoordinate(my_grid_coords[i], diagonal_1_direction));
                    diagonal_2_grid_coords.Add(new gridCoordinate(my_grid_coords[i], diagonal_2_direction));
                }

                bool desired_direction_valid = true;
                for (int i = 0; i < desired_grid_coords.Count; i++)
                    if (!is_spot_free(desired_grid_coords[i], fl, pl, corporeal))
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
                    else if (diagonal_1_valid && diagonal_2_valid)
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
        }
        //damage stuff
        public void takeDamage(List<Attack> atks, bool melee_attack, bool aoe_attack, List<string> msg_buf, Floor fl)
        {
            bool dodged = false;
            bool absorbed = false;
            int dodge_roll = rGen.Next(100);
            int dodge_bonus = 0;

            if (!corporeal)
            {
                if (my_monster_size == Monster_Size.Normal && !fl.is_tile_passable(my_grid_coords[0]))
                    dodge_bonus = 25;
            }

            if (melee_attack && dodge_roll < ((int)modified_melee_dodge + dodge_bonus))
            {
                dodged = true;
                if (dodge_values_degrade)
                    modified_melee_dodge *= .9;
            }
            else if (!melee_attack && dodge_roll < ((int)modified_ranged_dodge + dodge_bonus))
            {
                dodged = true;
                if (dodge_values_degrade)
                    modified_ranged_dodge *= .9;
            }

            if (!dodged)
            {
                set_initial_dodge_values();
                for (int i = 0; i < atks.Count; i++)
                {
                    int base_dmg = atks[i].get_damage_amt();

                    //Large and huge monsters take half damage from AoE attacks to keep
                    //the AoE attacks from just fucking destroying them utterly.
                    int dmg = base_dmg;
                    if (my_monster_size != Monster_Size.Normal && aoe_attack)
                        dmg = base_dmg / 2;

                    int armor_roll = rGen.Next(100);
                    if (armorPoints > 0 && armor_roll < armor_effectiveness)
                        absorbed = true;

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
                            fl.add_new_popup("-" + dmg, Popup.popup_msg_color.Purple, my_center_coordinate());
                            dmg -= armorPoints;
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
            return new Attack(dmg_type, dmgValue);
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
                strongest_smells.Add(fl.establish_los_strongest_smell(smelled_from[i], radius, targetSmell, smell_threshold));

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

        public bool is_doodad_within(Floor fl, int radius, Doodad.Doodad_Type target_doodad)
        {
            int doodad_index = 0;
            while (fl.Doodad_by_index(doodad_index) != null)
            {
                Doodad d = fl.Doodad_by_index(doodad_index);
                if (d.get_my_doodad_type() == target_doodad)
                {
                    gridCoordinate doodad_gcoord = d.get_g_coord();
                    for (int i = 0; i < my_grid_coords.Count; i++)
                    {
                        if (doodad_gcoord.x >= (my_grid_coords[i].x - radius) &&
                            doodad_gcoord.x <= (my_grid_coords[i].x + radius) &&
                            doodad_gcoord.y >= (my_grid_coords[i].y - radius) &&
                            doodad_gcoord.y <= (my_grid_coords[i].y + radius))
                            return true;
                    }
                }
                doodad_index++;
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

        public bool can_i_see_point(Floor fl, gridCoordinate target_point,
                                    VisionRay.fineness fineness = VisionRay.fineness.Average)
        {
            bool can_see_target = false;
            for (int i = 0; i < my_grid_coords.Count; i++)
            {
                if (fl.establish_los(my_grid_coords[i], target_point, fineness))
                    can_see_target = true;
            }

            return can_see_target;
        }

        public bool can_hear_sound(SoundPulse.Sound_Types soundID, int soundStrength)
        {
            for (int i = 0; i < sounds_i_can_hear.Count; i++)
                if (sounds_i_can_hear[i] == soundID && soundStrength > listen_threshold[i])
                    return true;

            return false;
        }

        private bool sound_has_priority(SoundPulse.Sound_Types soundID)
        {
            int current_sound_priority = sounds_i_can_hear.Count+1;
            int soundID_priority = 0;
            for (int i = 0; i < sounds_i_can_hear.Count; i++)
            {
                if (sounds_i_can_hear[i] == last_sound_i_heard)
                    current_sound_priority = i;
                if (sounds_i_can_hear[i] == soundID)
                    soundID_priority = i;
            }

            return soundID_priority <= current_sound_priority;
        }

        //Get a new path to the sound
        public void next_path_to_sound(List<gridCoordinate> path, SoundPulse.Sound_Types soundID)
        {
            if (!heard_something)
            {
                if (sound_has_priority(soundID))
                {
                    shortest_path_to_sound = new List<gridCoordinate>(path);
                    heard_something = true;
                    last_sound_i_heard = soundID;
                }
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
                last_sound_i_heard = SoundPulse.Sound_Types.None;
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

        protected int positive_difference(int i1, int i2)
        {
            if (i1 > i2)
                return i1 - i2;
            else
                return i2 - i1;
        }

        protected void set_initial_dodge_values()
        {
            modified_melee_dodge = melee_dodge;
            modified_ranged_dodge = ranged_dodge;
        }

        public bool is_corporeal()
        {
            return corporeal;
        }

        #region special functions that are common among some monster groups

        public void heal_near_altar(Floor fl)
        {
            if (is_doodad_within(fl, 4, Doodad.Doodad_Type.Altar))
            {
                int heal_value = (int)Math.Ceiling((double)(max_hitPoints/5));
                if (hitPoints <= max_hitPoints - heal_value)
                {
                    hitPoints += (int)Math.Ceiling((double)(max_hitPoints / 5));
                    fl.add_new_popup("+" + heal_value.ToString() + "HP", Popup.popup_msg_color.VividGreen, my_center_coordinate());
                }
                else
                {
                    int difference = max_hitPoints - hitPoints;
                    if (difference > 0)
                    {
                        hitPoints = max_hitPoints;
                        fl.add_new_popup("+" + difference.ToString() + "HP", Popup.popup_msg_color.VividGreen, my_center_coordinate());
                    }
                }
            }
        }

        #endregion
    }
}
