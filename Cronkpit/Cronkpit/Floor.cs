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
    class Floor
    {
        //Constants
        protected int stdfloorSize = 50;
        public enum specific_effect { None, Power_Strike, Cleave, Earthquake,
                                      Acid_Blood, Bite, Big_Bite, Alert,
                                      Warning_Bracket };
        //Floor components
        int fl_number;
        int ambient_manavalue;
        List<List<Tile>> floorTiles;
        List<Tile> scentedTiles;
        List<Room> roomLayout;
        List<Hall> hallLayout;
        List<MossyPatch> mossLayout;
        List<Monster> badGuys;
        List<Goldpile> Money;
        List<Doodad> Doodads;
        List<Projectile> Pew_Pews;
        List<Effect> eff_ex;
        List<PersistentEffect> persistent_effects;
        List<Popup> popup_alerts;
        SpawnTable floorSpawns;
        SpawnTable floor_Sub_Spawns;

        List<string> message_buffer;

        SpriteFont popup_msg_font;
        Texture2D blank_texture;
        //Sensory lists
        List<SoundPulse> Noises;
        List<VisionRay> Vision_Rc;
        //List<VisionRay> Vision_Log;

        //Other stuff needed for the thing to function
        ContentManager cManager;
        Random randGen;

        //Various lists of textures.
        Texture2D[] rubble_wall = new Texture2D[1];
        Texture2D[] rubble_floor = new Texture2D[1];
        Texture2D[] the_void = new Texture2D[1];
        Texture2D[] stone_walls = new Texture2D[2];
        Texture2D[] stone_floors = new Texture2D[2];
        Texture2D[] dirt_floors = new Texture2D[1];
        Texture2D[] dirt_walls = new Texture2D[2];
        Texture2D[] exit_tile = new Texture2D[1];
        Texture2D[] dungeon_exit = new Texture2D[2];
        //Mossy textures - present in the necropolis
        Texture2D[] mossy_stonewalls = new Texture2D[1];
        Texture2D[] mossy_stonefloors = new Texture2D[2];
        Texture2D[] mossy_dirtfloors = new Texture2D[1];
        Texture2D[] mossy_dirtwalls = new Texture2D[1];

        //Green text. Function here.
        public Floor(ContentManager sCont, ref List<string> msgBuffer, Texture2D blnkTex, int floor_number,
                     Cronkpit.CronkPit.Dungeon dungeon)
        {
            //Init floor components.
            floorTiles = new List<List<Tile>>();
            scentedTiles = new List<Tile>();
            roomLayout = new List<Room>();
            hallLayout = new List<Hall>();
            mossLayout = new List<MossyPatch>();
            badGuys = new List<Monster>();
            Money = new List<Goldpile>();
            Doodads = new List<Doodad>();
            Vision_Rc = new List<VisionRay>();
            //Vision_Log = new List<VisionRay>(); //Still useful.
            Noises = new List<SoundPulse>();
            Pew_Pews = new List<Projectile>();
            eff_ex = new List<Effect>();
            popup_alerts = new List<Popup>();
            persistent_effects = new List<PersistentEffect>();
            fl_number = floor_number;
            floorSpawns = new SpawnTable(fl_number, false);
            floor_Sub_Spawns = new SpawnTable(fl_number, true);

            message_buffer = msgBuffer;

            //Next init other stuff
            blank_texture = blnkTex;
            blank_texture.SetData(new[] { Color.White });
            cManager = sCont;
            randGen = new Random((int)DateTime.Now.Ticks);
            popup_msg_font = sCont.Load<SpriteFont>("Fonts/popup_msg");
            ambient_manavalue = 1000;
        
            //Then do stuff for real
            buildFloor(dungeon);
        }

        #region super important stuff

        //All of this is dungeon building stuff and will eventually get torn out
        //And put into a dungeon that inherits this class specifically.
        public void buildFloor(Cronkpit.CronkPit.Dungeon cDungeon)
        {
            rubble_floor[0] = cManager.Load<Texture2D>("Background/rubble_floor");
            rubble_wall[0] = cManager.Load<Texture2D>("Background/rubble_wall");
            switch (cDungeon)
            {
                case CronkPit.Dungeon.Necropolis:
                    build_necropolis_floor();
                    break;
            }
        }

        protected void add_monsters(ref List<Monster> monsters, int number_of_monsters, 
                                    Cronkpit.CronkPit.Dungeon cDungeon)
        {
            for (int i = 0; i < number_of_monsters; i++)
            {
                int monsterType = randGen.Next(100);
                string monster_to_add = floorSpawns.find_monster_by_number(monsterType);

                switch (monster_to_add)
                {
                    case "HollowKnight":
                        if (randGen.Next(100) <= floor_Sub_Spawns.return_spawn_chance_by_monster("RedKnight"))
                            badGuys.Add(new RedKnight(valid_hollowKnight_spawn(), cManager, i));
                        else
                            badGuys.Add(new HollowKnight(valid_hollowKnight_spawn(), cManager, i));
                        break;
                    case "Skeleton":
                        int skelwpn = randGen.Next(6);
                        Skeleton.Skeleton_Weapon_Type reg_skel_wpn = 0;
                        switch (skelwpn)
                        {
                            case 0:
                                reg_skel_wpn = Skeleton.Skeleton_Weapon_Type.Fist;
                                break;
                            case 1:
                                reg_skel_wpn = Skeleton.Skeleton_Weapon_Type.Axe;
                                break;
                            case 2:
                                reg_skel_wpn = Skeleton.Skeleton_Weapon_Type.Bow;
                                break;
                            case 3:
                                reg_skel_wpn = Skeleton.Skeleton_Weapon_Type.Sword;
                                break;
                            case 4:
                                reg_skel_wpn = Skeleton.Skeleton_Weapon_Type.Spear;
                                break;
                            case 5:
                                reg_skel_wpn = Skeleton.Skeleton_Weapon_Type.Flamebolt;
                                break;
                        }
                        badGuys.Add(new Skeleton(random_valid_position(), cManager, i, reg_skel_wpn));
                        break;
                    case "GoldMimic":
                        badGuys.Add(new GoldMimic(random_valid_position(), cManager, i));
                        break;
                    case "GoreHound":
                        badGuys.Add(new GoreHound(random_valid_position(), cManager, i));
                        break;
                    case "Zombie":
                        if (randGen.Next(100) <= floor_Sub_Spawns.return_spawn_chance_by_monster("ZombieFanatic"))
                            badGuys.Add(new ZombieFanatic(random_valid_position(), cManager, i));
                        else
                            badGuys.Add(new Zombie(random_valid_position(), cManager, i));
                        break;
                    case "Grendel":
                        int grenwpn = randGen.Next(3);
                        switch (grenwpn)
                        {
                            case 0:
                                badGuys.Add(new Grendel(random_valid_position(), cManager, i, Grendel.Grendel_Weapon_Type.Club));
                                break;
                            case 1:
                                badGuys.Add(new Grendel(random_valid_position(), cManager, i, Grendel.Grendel_Weapon_Type.Frostbolt));
                                break;
                        }
                        break;
                    case "Necromancer":
                        badGuys.Add(new Necromancer(random_valid_position(), cManager, i));
                        break;
                    case "GoreWolf":
                        badGuys.Add(new Gorewolf(random_valid_position(), cManager, i));
                        break;
                    case "Ghost":
                        badGuys.Add(new Ghost(random_valid_position(), cManager, i));
                        break;
                    case "ArmoredSkel":
                        int a_skelwpn = randGen.Next(4);
                        Armored_Skeleton.Armor_Skeleton_Weapon arm_skel_wpn = 0;
                        switch (a_skelwpn)
                        {
                            case 0:
                                arm_skel_wpn = Armored_Skeleton.Armor_Skeleton_Weapon.Halberd;
                                break;
                            case 1:
                                arm_skel_wpn = Armored_Skeleton.Armor_Skeleton_Weapon.Crossbow;
                                break;
                            case 2:
                                arm_skel_wpn = Armored_Skeleton.Armor_Skeleton_Weapon.Magic;
                                break;
                            case 3:
                                arm_skel_wpn = Armored_Skeleton.Armor_Skeleton_Weapon.Greatsword;
                                break;
                        }
                        badGuys.Add(new Armored_Skeleton(random_valid_position(), cManager, i, arm_skel_wpn));
                        break;
                    case "VoidWraith":
                        badGuys.Add(new Voidwraith(random_valid_position(), cManager, i));
                        break;
                }
            }

            if(cDungeon == CronkPit.Dungeon.Necropolis && fl_number == 12)
                badGuys.Add(new Boneyard(random_valid_position(Monster.Monster_Size.Large), cManager, badGuys.Count, true));
        }

        public void draw_hallway_tiles(Tile target_tile)
        {
            if (target_tile.get_my_tile_type() !=  Tile.Tile_Type.StoneFloor && 
                target_tile.get_my_tile_type() != Tile.Tile_Type.DirtFloor)
                target_tile.set_tile_type(Tile.Tile_Type.StoneFloor, stone_floors);
        }

        public void replace_surrounding_void(Tile target_tile, Texture2D[] stone_walls, 
                                                               Texture2D[] dirt_walls,
                                                               Texture2D[] rubble_walls)
        {
            gridCoordinate target_grid_c = target_tile.get_grid_c();
            Tile.Tile_Type tileType = target_tile.get_my_tile_type();

            for (int x = target_grid_c.x - 1; x < target_grid_c.x + 2; x++)
                for (int y = target_grid_c.y - 1; y < target_grid_c.y + 2; y++)
                    if (floorTiles[x][y].isVoid())
                    {
                        switch (target_tile.get_my_tile_type())
                        {
                            //Dirt
                            case Tile.Tile_Type.DirtFloor:
                                floorTiles[x][y].set_tile_type(Tile.Tile_Type.DirtWall, dirt_walls);
                                break;
                            case Tile.Tile_Type.Rubble_Floor:
                                floorTiles[x][y].set_tile_type(Tile.Tile_Type.Rubble_Wall, rubble_walls);
                                break;
                            //Stone
                            default:
                                floorTiles[x][y].set_tile_type(Tile.Tile_Type.StoneWall, stone_walls);
                                break;
                        }   
                    }
        }

        List<List<bool>> circular_room_matrix(int diameter)
        {
            List<List<bool>> circle_matrix = new List<List<bool>>();

            int radius = (int)Math.Floor((double)diameter / 2);
            //the "r" here stands for room, so room x, room y, etc.
            for (int rx = 0; rx < diameter; rx++)
            {
                circle_matrix.Add(new List<bool>());
                for (int ry = 0; ry < diameter; ry++)
                    circle_matrix[rx].Add(true);
            }

            int tiles_off_corners = Math.Max(radius - 2, 1);
            
            for(int rx = 0; rx < diameter; rx++)
                for (int ry = 0; ry < diameter; ry++)
                {
                    if ((rx < tiles_off_corners || rx >= diameter - tiles_off_corners) &&
                       (ry < tiles_off_corners || ry >= diameter - tiles_off_corners))
                        circle_matrix[rx][ry] = false;
                }

            if (radius - 2 > 0)
            {
                //The two leftmost tiles.
                circle_matrix[0][diameter - (tiles_off_corners + 1)] = false;
                circle_matrix[0][tiles_off_corners] = false;

                //The two next ones to the right
                circle_matrix[tiles_off_corners][0] = false;
                circle_matrix[tiles_off_corners][diameter - 1] = false;

                //Two next to the right
                circle_matrix[diameter - (tiles_off_corners + 1)][0] = false;
                circle_matrix[diameter - (tiles_off_corners +1)][diameter - 1] = false;

                //The two rightmost ones
                circle_matrix[diameter - 1][tiles_off_corners] = false;
                circle_matrix[diameter - 1][diameter - (tiles_off_corners + 1)] = false;
            }

            return circle_matrix;
        }

        public void add_door(gridCoordinate target_coord, List<gridCoordinate> side_coords)
        {
            bool wood_frame_possible = false;
            bool stone_frame_possible = false;

            for (int i = 0; i < side_coords.Count; i++)
            {
                if (floorTiles[side_coords[i].x][side_coords[i].y].get_my_tile_type() == Tile.Tile_Type.StoneWall)
                    stone_frame_possible = true;

                if (floorTiles[side_coords[i].x][side_coords[i].y].get_my_tile_type() == Tile.Tile_Type.DirtWall)
                    wood_frame_possible = true;
            }

            bool stone_frame = false;
            if (wood_frame_possible && stone_frame_possible)
            {
                if (randGen.Next(2) == 0)
                    stone_frame = true;
            }
            else if (!wood_frame_possible && stone_frame_possible)
                stone_frame = true;

            Doodads.Add(new Doodad(Doodad.Doodad_Type.Door, cManager, target_coord, Doodads.Count, stone_frame));
            int door_state = randGen.Next(5);
            if (door_state == 0)
                Doodads[Doodads.Count - 1].set_door_state(Doodad.Door_State.Locked);
            else if (door_state == 1)
                Doodads[Doodads.Count - 1].set_door_state(Doodad.Door_State.Stuck);
        }

        //This is all non-dungeon building stuff pertaining to monsters, etc.
        //Green text. Function here.
        public void update_dungeon_floor(Player Pl)
        {
            //Vision_Log.Clear();
            //scrub_all_auras();
            execute_persistent_effects(Pl);
            update_all_monsters(Pl);
            decay_all_scents();
        }

        //Green text. Function here.
        public void update_all_monsters(Player pl)
        {
            for (int i = 0; i < badGuys.Count; i++)
            {
                badGuys[i].Update_Monster(pl, this);
            }
        }

        public void smooth_transition_monster(float delta_time)
        {
            for(int i = 0; i < badGuys.Count; i++)
                badGuys[i].increment_my_drawing_position(delta_time);
        }

        public bool done_smooth_transitions()
        {
            bool done = true;
            for (int i = 0; i < badGuys.Count; i++)
                if (!badGuys[i].at_destination())
                    done = false;

            return done;
        }

        public bool acceptable_destruction_coordinate(gridCoordinate t_coord)
        {
            return t_coord.x > 0 && t_coord.x < stdfloorSize-1 && t_coord.y > 0 && t_coord.y < stdfloorSize-1;
        }

        public void unlock_dungeon_exit()
        {
            for (int x = 0; x < stdfloorSize; x++)
                for (int y = 0; y < stdfloorSize; y++)
                    if (floorTiles[x][y].get_my_tile_type() == Tile.Tile_Type.Locked_Dungeon_Exit)
                        floorTiles[x][y].set_tile_type(Tile.Tile_Type.Dungeon_Exit, dungeon_exit);
        }

        #endregion

        #region specific dungeon building functions

        public void build_necropolis_floor()
        {
            string basePath = "Background/Necropolis/Necro_";
            //Make the base tiles.
            //Void
            the_void[0] = cManager.Load<Texture2D>("Background/badvoidtile");
            //Stone Walls
            stone_walls[0] = cManager.Load<Texture2D>(basePath + "StoneWall");
            stone_walls[1] = cManager.Load<Texture2D>(basePath + "StoneWallTorch");
            //Stone Floors
            stone_floors[0] = cManager.Load<Texture2D>(basePath + "StoneFloor");
            stone_floors[1] = cManager.Load<Texture2D>(basePath + "StoneFloorCracked");
            //Dirt Floors
            dirt_floors[0] = cManager.Load<Texture2D>(basePath + "Dirtfloor");
            //Dirt Walls
            dirt_walls[0] = cManager.Load<Texture2D>(basePath + "DirtWall");
            dirt_walls[1] = cManager.Load<Texture2D>(basePath + "DirtWallTorch");
            //Exit Tile
            exit_tile[0] = cManager.Load<Texture2D>("Background/exit");
            //Dungeon Exit Tile
            dungeon_exit[0] = cManager.Load<Texture2D>(basePath + "dungeon_exit_open");
            dungeon_exit[1] = cManager.Load<Texture2D>(basePath + "dungeon_exit_closed");
            //MOSS
            mossy_dirtfloors[0] = cManager.Load<Texture2D>(basePath + "MossDirtFloor");
            mossy_stonefloors[0] = cManager.Load<Texture2D>(basePath + "StoneFloorMoss");
            mossy_stonefloors[1] = cManager.Load<Texture2D>(basePath + "StoneFloorCrackedMoss");
            mossy_dirtwalls[0] = cManager.Load<Texture2D>(basePath + "MossDirtWall");
            mossy_stonewalls[0] = cManager.Load<Texture2D>(basePath + "StoneWallMoss");

            for (int x = 0; x < stdfloorSize; x++)
            {
                floorTiles.Add(new List<Tile>());
                for (int y = 0; y < stdfloorSize; y++)
                {
                    Vector2 tilePos = new Vector2(x * 32, y * 32);
                    floorTiles[x].Add(new Tile(Tile.Tile_Type.Void, randGen.Next(100), cManager, blank_texture, tilePos, new gridCoordinate(x, y), the_void));
                }
            }

            //Randomly generate rooms and hallways.
            //First rooms.
            int number_of_rooms = 5 + randGen.Next(4);
            int dirt_threshold = Math.Max((50 - ((fl_number - 1) * 2)), 20);
            for (int i = 0; i < number_of_rooms; i++)
            {
                int next_room_height = randGen.Next(4, 10);
                int next_room_width = randGen.Next(4, 10);
                int next_room_startX = randGen.Next(1, ((stdfloorSize - 1) - next_room_width));
                int next_room_startY = randGen.Next(1, ((stdfloorSize - 1) - next_room_height));

                int lower_room_x_edge = next_room_startX + next_room_width;
                int lower_room_y_edge = next_room_startY + next_room_height;
                int upper_room_x_edge = next_room_startX - next_room_width;
                int upper_room_y_edge = next_room_startY - next_room_height;

                bool dirt_room = false;
                int dirt_dice_roll = randGen.Next(100);
                if (dirt_dice_roll < dirt_threshold &&
                    (lower_room_x_edge < 15 || upper_room_x_edge > 35 ||
                    lower_room_y_edge < 15 || upper_room_y_edge > 35))
                    dirt_room = true;

                Room rm = new Room(next_room_height,
                                    next_room_width,
                                    next_room_startX,
                                    next_room_startY,
                                    dirt_room);

                int corpse_chance = fl_number * 5;
                if (rm.is_dirt_room())
                    corpse_chance += 20;

                if (randGen.Next(100) < corpse_chance)
                    rm.set_corpses(true);

                if (rm.has_corpses())
                    rm.set_corners_with_corpses(randGen.Next(1, 5));

                if (next_room_height == next_room_width && next_room_width % 2 == 1)
                {
                    int circular_room_roll = randGen.Next(3);
                    if (circular_room_roll < 2)
                    {
                        rm.set_to_circular_room();
                        rm.set_circular_room_matrix(circular_room_matrix(next_room_width));
                    }
                }

                if (!rm.is_circular_room())
                {
                    int doors = randGen.Next(4);
                    if (doors == 0)
                        rm.set_doors(true);
                }

                roomLayout.Add(rm);
            }
            //Next hallways.
            int number_of_hallways = number_of_rooms - 1;
            int number_of_random_hallways = 2;
            for (int i = 0; i < number_of_hallways; i++)
            {
                int next_hallway_start = 0;
                int next_hallway_end = 0;
                if (i < roomLayout.Count)
                    next_hallway_start = i;
                else
                    next_hallway_start = roomLayout.Count();
                if (i + 1 < roomLayout.Count)
                    next_hallway_end = i + 1;
                else
                    next_hallway_end = roomLayout.Count();
                int next_hallway_startX = roomLayout[next_hallway_start].findCenter("x");
                int next_hallway_endX = roomLayout[next_hallway_end].findCenter("x");
                int next_hallway_startY = roomLayout[next_hallway_start].findCenter("y");
                int next_hallway_endY = roomLayout[next_hallway_end].findCenter("y");
                hallLayout.Add(new Hall(next_hallway_start,
                                        next_hallway_end,
                                        next_hallway_startX,
                                        next_hallway_startY,
                                        next_hallway_endX,
                                        next_hallway_endY));
            }
            for (int i = 0; i < number_of_random_hallways; i++)
            {
                int next_hallway_start = 0;
                int next_hallway_end = 0;
                while (next_hallway_start == next_hallway_end)
                {
                    next_hallway_start = randGen.Next(roomLayout.Count);
                    next_hallway_end = randGen.Next(roomLayout.Count);
                }
                int next_hallway_startX = roomLayout[next_hallway_start].findCenter("x");
                int next_hallway_endX = roomLayout[next_hallway_end].findCenter("x");
                int next_hallway_startY = roomLayout[next_hallway_start].findCenter("y");
                int next_hallway_endY = roomLayout[next_hallway_end].findCenter("y");
                hallLayout.Add(new Hall(next_hallway_start,
                                        next_hallway_end,
                                        next_hallway_startX,
                                        next_hallway_startY,
                                        next_hallway_endX,
                                        next_hallway_endY));
            }

            //Alter void tiles to floor tiles.
            //First rooms
            for (int i = 0; i < roomLayout.Count; i++)
            {
                List<List<bool>> circle_matrix = null;
                if (roomLayout[i].is_circular_room())
                {
                    circle_matrix = roomLayout[i].retrieve_circular_matrix();
                }

                for (int x = roomLayout[i].startXPos; x < roomLayout[i].startXPos + roomLayout[i].roomWidth; x++)
                    for (int y = roomLayout[i].startYPos; y < roomLayout[i].startYPos + roomLayout[i].roomHeight; y++)
                    {
                        if (roomLayout[i].is_circular_room())
                        {
                            if (circle_matrix[x - roomLayout[i].startXPos][y - roomLayout[i].startYPos])
                                if (!roomLayout[i].is_dirt_room())
                                    floorTiles[x][y].set_tile_type(Tile.Tile_Type.StoneFloor, stone_floors);
                                else
                                    floorTiles[x][y].set_tile_type(Tile.Tile_Type.DirtFloor, dirt_floors);
                        }
                        else
                            if (!roomLayout[i].is_dirt_room())
                                floorTiles[x][y].set_tile_type(Tile.Tile_Type.StoneFloor, stone_floors);
                            else
                                floorTiles[x][y].set_tile_type(Tile.Tile_Type.DirtFloor, dirt_floors);
                    }
            }

            //Then hallways
            for (int i = 0; i < hallLayout.Count; i++)
            {
                int x_to_y = randGen.Next(1);
                hallLayout[i].drawnDirection = x_to_y;
                //0 = draw y to x
                if (x_to_y == 0)
                {
                    //draw Y first from startX
                    if (hallLayout[i].startY > hallLayout[i].endY)
                        for (int y = hallLayout[i].startY; y > hallLayout[i].endY; y--)
                            draw_hallway_tiles(floorTiles[hallLayout[i].startX][y]);
                    else
                        for (int y = hallLayout[i].startY; y < hallLayout[i].endY; y++)
                            draw_hallway_tiles(floorTiles[hallLayout[i].startX][y]);

                    //then draw X from endY
                    if (hallLayout[i].startX > hallLayout[i].endX)
                        for (int x = hallLayout[i].startX; x > hallLayout[i].endX; x--)
                            draw_hallway_tiles(floorTiles[x][hallLayout[i].endY]);
                    else
                        for (int x = hallLayout[i].startX; x < hallLayout[i].endX; x++)
                            draw_hallway_tiles(floorTiles[x][hallLayout[i].endY]);
                }
                //1 = draw x to y
                else
                {
                    //draw X first from startY
                    if (hallLayout[i].startX > hallLayout[i].endX)
                        for (int x = hallLayout[i].startX; x > hallLayout[i].endX; x--)
                            draw_hallway_tiles(floorTiles[x][hallLayout[i].startY]);
                    else
                        for (int x = hallLayout[i].startX; x < hallLayout[i].endX; x++)
                            draw_hallway_tiles(floorTiles[x][hallLayout[i].startY]);

                    //draw Y second from endX
                    if (hallLayout[i].startY > hallLayout[i].endY)
                        for (int y = hallLayout[i].startY; y > hallLayout[i].endY; y--)
                            draw_hallway_tiles(floorTiles[hallLayout[i].endX][y]);
                    else
                        for (int y = hallLayout[i].startY; y < hallLayout[i].endY; y++)
                            draw_hallway_tiles(floorTiles[hallLayout[i].endX][y]);
                }
            }
            //place an exit adjacent to a random spot.
            bool exitPlaced = false;
            while (!exitPlaced)
            {
                gridCoordinate exit_coord = new gridCoordinate(random_valid_position());
                //if there's not a void tile adjacent to this floor tile, don't place the exit.
                //if there is though, place it and we're done here!
                for (int x = exit_coord.x - 1; x <= exit_coord.x + 1; x++)
                {
                    if (x < stdfloorSize && x > 0)
                    {
                        if (floorTiles[x][exit_coord.y].isVoid() && !exitPlaced)
                        {
                            if (fl_number < 12)
                            {
                                floorTiles[x][exit_coord.y].set_tile_type(Tile.Tile_Type.Exit, exit_tile);
                                exitPlaced = true;
                            }
                            else
                            {
                                floorTiles[x][exit_coord.y].set_tile_type(Tile.Tile_Type.Locked_Dungeon_Exit, dungeon_exit);
                                exitPlaced = true;
                            }
                        }
                    }
                }

                for (int y = exit_coord.y - 1; y <= exit_coord.y + 1; y++)
                    if (y < stdfloorSize && y > 0)
                    {
                        if (floorTiles[exit_coord.x][y].isVoid() && !exitPlaced)
                        {
                            if (fl_number < 12)
                            {
                                floorTiles[exit_coord.x][y].set_tile_type(Tile.Tile_Type.Exit, exit_tile);
                                exitPlaced = true;
                            }
                            else
                            {
                                floorTiles[exit_coord.x][y].set_tile_type(Tile.Tile_Type.Locked_Dungeon_Exit, dungeon_exit);
                                exitPlaced = true;
                            }
                        }
                    }
            }
            //add walls around all walkable tiles.
            for (int x = 0; x < stdfloorSize; x++)
                for (int y = 0; y < stdfloorSize; y++)
                    if (floorTiles[x][y].isPassable())
                        replace_surrounding_void(floorTiles[x][y], stone_walls, dirt_walls, rubble_wall);

            //Next, do mossy tiles.
            int moss_patches = randGen.Next(Math.Min(5, Math.Max(fl_number - 3, 0)) + 1);
            //int moss_patches = 12;
            for (int i = 0; i < moss_patches; i++)
            {
                int patch_x_size = randGen.Next(3, 7);
                int patch_y_size = randGen.Next(3, 7);
                mossLayout.Add(new MossyPatch(patch_x_size, patch_y_size, random_valid_position(), ref randGen));
            }

            for (int j = 0; j < mossLayout.Count; j++)
            {
                for (int x = 0; x < mossLayout[j].width; x++)
                    for (int y = 0; y < mossLayout[j].height; y++)
                        if (mossLayout[j].mossConfig[x][y] == true)
                        {
                            int x_position = mossLayout[j].grid_position.x + x;
                            int y_position = mossLayout[j].grid_position.y + y;
                            if (x_position > 0 && x_position < 50 && y_position > 0 && y_position < 50)
                            {
                                switch (floorTiles[x_position][y_position].get_my_tile_type())
                                {
                                    case Tile.Tile_Type.DirtFloor:
                                        floorTiles[x_position][y_position].mossify(mossy_dirtfloors);
                                        break;
                                    case Tile.Tile_Type.DirtWall:
                                        floorTiles[x_position][y_position].mossify(mossy_dirtwalls);
                                        break;
                                    case Tile.Tile_Type.StoneWall:
                                        floorTiles[x_position][y_position].mossify(mossy_stonewalls);
                                        break;
                                    default:
                                        floorTiles[x_position][y_position].mossify(mossy_stonefloors);
                                        break;
                                }
                                
                            }
                        }
            }
            //Add Doors first
            for (int i = 0; i < roomLayout.Count; i++)
            {
                if (roomLayout[i].room_has_doors())
                {
                    int roomX = roomLayout[i].startXPos;
                    int roomY = roomLayout[i].startYPos;
                    int roomH = roomLayout[i].roomHeight;
                    int roomW = roomLayout[i].roomWidth;
                    //we pass the side coordinates to the door function so that it knows
                    //what type of frame to give the door.
                    List<gridCoordinate> side_door_coords = new List<gridCoordinate>();
                    for (int x = roomX; x < roomX + roomW; x++)
                    {
                        if (!is_tile_passable(new gridCoordinate(x - 1, roomY - 1)) &&
                            !is_tile_passable(new gridCoordinate(x + 1, roomY - 1)) &&
                            is_tile_passable(new gridCoordinate(x, roomY - 1)) &&
                            !floorTiles[x][roomY - 1].isExit())
                        {
                            side_door_coords.Clear();
                            side_door_coords.Add(new gridCoordinate(x - 1, roomY - 1));
                            side_door_coords.Add(new gridCoordinate(x + 1, roomY - 1));
                            add_door(new gridCoordinate(x, roomY - 1), side_door_coords);
                        }

                        if (!is_tile_passable(new gridCoordinate(x - 1, roomY + roomH)) &&
                            !is_tile_passable(new gridCoordinate(x + 1, roomY + roomH)) &&
                            is_tile_passable(new gridCoordinate(x, roomY + roomH)) &&
                            !floorTiles[x][roomY + roomH].isExit())
                        {
                            side_door_coords.Clear();
                            side_door_coords.Add(new gridCoordinate(x - 1, roomY + roomH));
                            side_door_coords.Add(new gridCoordinate(x + 1, roomY + roomH));
                            add_door(new gridCoordinate(x, roomY + roomH), side_door_coords);
                        }
                    }

                    for (int y = roomY; y < roomY + roomH; y++)
                    {
                        if (!is_tile_passable(new gridCoordinate(roomX - 1, y - 1)) &&
                            !is_tile_passable(new gridCoordinate(roomX - 1, y + 1)) &&
                            is_tile_passable(new gridCoordinate(roomX - 1, y)) &&
                            !floorTiles[roomX - 1][y].isExit())
                        {
                            side_door_coords.Clear();
                            side_door_coords.Add(new gridCoordinate(roomX - 1, y-1));
                            side_door_coords.Add(new gridCoordinate(roomX - 1, y+1));
                            add_door(new gridCoordinate(roomX - 1, y), side_door_coords);
                        }

                        if (!is_tile_passable(new gridCoordinate(roomX + roomW, y - 1)) &&
                            !is_tile_passable(new gridCoordinate(roomX + roomW, y + 1)) &&
                            is_tile_passable(new gridCoordinate(roomX + roomW, y)) &&
                            !floorTiles[roomX + roomW][y].isExit())
                        {
                            side_door_coords.Clear();
                            side_door_coords.Add(new gridCoordinate(roomX + roomW, y - 1));
                            side_door_coords.Add(new gridCoordinate(roomX + roomW, y + 1));
                            add_door(new gridCoordinate(roomX + roomW, y), side_door_coords);
                        }
                    }
                }
            }
            //Add Altars!!!
            if (fl_number > 4)
            {
                for (int i = 0; i < roomLayout.Count; i++)
                {
                    int room_has_altars = randGen.Next(10);
                    int threshold = 0;
                    if (roomLayout[i].is_circular_room())
                        threshold = 4;
                    if (room_has_altars <= threshold)
                    {
                        int x_ctr = roomLayout[i].findCenter("x");
                        int y_ctr = roomLayout[i].findCenter("y");
                        Doodads.Add(new Doodad(Doodad.Doodad_Type.Altar, cManager,
                                    new gridCoordinate(x_ctr, y_ctr), Doodads.Count));
                    }
                }
            }

            //Add Doodads
            int suits = randGen.Next(Math.Max(0, 4 - fl_number), Math.Max(2, 6 - fl_number));
            for (int i = 0; i < suits; i++)
                Doodads.Add(new Doodad(Doodad.Doodad_Type.ArmorSuit, cManager,
                            valid_hollowKnight_spawn(), i));

            for (int i = 0; i < roomLayout.Count; i++)
            {
                if (roomLayout[i].has_corpses())
                {
                    int corners = roomLayout[i].how_many_corners_have_corpses();
                    int roomX = roomLayout[i].startXPos;
                    int roomY = roomLayout[i].startYPos;
                    int roomH = roomLayout[i].roomHeight-1;
                    int roomW = roomLayout[i].roomWidth-1;
                    int x_dir_check = 0;
                    int y_dir_check = 0;
                    gridCoordinate top_left = new gridCoordinate(roomX, roomY);
                    gridCoordinate top_right = new gridCoordinate(roomX+roomW, roomY);
                    gridCoordinate bottom_left = new gridCoordinate(roomX, roomY+roomH);
                    gridCoordinate bottom_right = new gridCoordinate(roomX+roomW, roomY+roomH);
                    for(int c = 0; c < corners; c++)
                    {
                        bool corner_valid = false;
                        gridCoordinate target_corner_gc = new gridCoordinate(-1, -1);
                        int tries = 0;
                        while(!corner_valid && tries < 20)
                        {
                            //pick a random corner.
                            int target_corner = randGen.Next(4);
                            if (target_corner == 0)
                            {
                                target_corner_gc = top_left;
                                x_dir_check = -1;
                                y_dir_check = -1;
                            }
                            else if (target_corner == 1)
                            {
                                target_corner_gc = top_right;
                                x_dir_check = 1;
                                y_dir_check = -1;
                            }
                            else if (target_corner == 2)
                            {
                                target_corner_gc = bottom_left;
                                x_dir_check = -1;
                                y_dir_check = 1;
                            }
                            else if (target_corner == 3)
                            {
                                target_corner_gc = bottom_right;
                                x_dir_check = 1;
                                y_dir_check = 1;
                            }

                            //Next, check to see if there's a corpse pile in that corner
                            //Already.
                            corner_valid = true;
                            for (int d = 0; d < Doodads.Count; d++)
                                if (Doodads[d].get_g_coord().x == target_corner_gc.x &&
                                    Doodads[d].get_g_coord().y == target_corner_gc.y)
                                    corner_valid = false;
                            tries++;
                        }

                        if (corner_valid)
                        {
                            int number_of_corpses = randGen.Next(1, 4); //Math.Min(3, 5 - randGen.Next(6));

                            if (is_tile_passable(target_corner_gc) &&
                               !is_tile_passable(new gridCoordinate(target_corner_gc.x + x_dir_check, target_corner_gc.y)) &&
                               !is_tile_passable(new gridCoordinate(target_corner_gc.x, target_corner_gc.y + y_dir_check)))
                                Doodads.Add(new Doodad(Doodad.Doodad_Type.CorpsePile, cManager, target_corner_gc, Doodads.Count));
                            if (number_of_corpses >= 2)
                            {
                                gridCoordinate pile_2 = new gridCoordinate(target_corner_gc.x + (x_dir_check * -1), target_corner_gc.y);
                                if (is_tile_passable(pile_2) &&
                                    !is_tile_passable(new gridCoordinate(pile_2.x, pile_2.y + y_dir_check)))
                                    Doodads.Add(new Doodad(Doodad.Doodad_Type.CorpsePile, cManager, pile_2, Doodads.Count));
                            }
                            if (number_of_corpses == 3)
                            {
                                gridCoordinate pile_3 = new gridCoordinate(target_corner_gc.x, target_corner_gc.y + (y_dir_check * -1));
                                if (is_tile_passable(pile_3) &&
                                    !is_tile_passable(new gridCoordinate(pile_3.x + x_dir_check, pile_3.y)))
                                    Doodads.Add(new Doodad(Doodad.Doodad_Type.CorpsePile, cManager, pile_3, Doodads.Count));
                            }
                        }
                    }
                }
            }

            //Add gold piles
            int gold_per_floor = 500;
            int low_val_piles = gold_per_floor / 10;
            int high_val_piles = gold_per_floor / 50;
            if (gold_per_floor % 50 != 0)
                high_val_piles++;
            int number_of_goldpiles = randGen.Next(high_val_piles, low_val_piles + 1);
            gold_per_floor -= (number_of_goldpiles * 10);
            for (int i = 0; i < number_of_goldpiles; i++)
                Money.Add(new Goldpile(random_valid_position(), cManager, 10));
            while (gold_per_floor > 0)
            {
                for (int i = 0; i < number_of_goldpiles; i++)
                {
                    int amt_to_subtract = Math.Min(randGen.Next(0, (50 - Money[i].my_quantity) + 1), gold_per_floor);
                    gold_per_floor -= amt_to_subtract;
                    Money[i].my_quantity += amt_to_subtract;
                }
            }
            for (int i = 0; i < number_of_goldpiles; i++)
                Money[i].init_my_texture();

            //Add monsters.
            int monster_lower_bound = 2;
            int monster_upper_bound = 4 + (fl_number / 2);
            int number_of_monsters = 7 + ((fl_number - 5) / 2) + randGen.Next(monster_lower_bound, monster_upper_bound);
            add_monsters(ref badGuys, number_of_monsters, CronkPit.Dungeon.Necropolis);
        }

        #endregion

        #region messaging stuff

        //Green text
        public void addmsg(string message)
        {
            message_buffer.Add(message);
        }

        #endregion

        #region position stuff - includes mouse click functions

        //Green text. Function here.
        public bool isWalkable(gridCoordinate grid_position)
        {
            bool passable_tile;
            if(grid_position.x < stdfloorSize && 
               grid_position.y < stdfloorSize &&
               grid_position.x > 0 &&
               grid_position.y > 0)
                passable_tile = floorTiles[grid_position.x][grid_position.y].isPassable();
            else
                return false;

            bool impassible_Doodad = false;
            for (int i = 0; i < Doodads.Count; i++)
            {
                if (Doodads[i].get_g_coord().x == grid_position.x &&
                    Doodads[i].get_g_coord().y == grid_position.y &&
                    !Doodads[i].is_passable())
                    impassible_Doodad = true;
            }

            return (passable_tile == true && impassible_Doodad == false);
        }

        public bool is_tile_passable(gridCoordinate grid_position)
        {
            bool passable_tile;
            if(grid_position.x < stdfloorSize && 
               grid_position.y < stdfloorSize &&
               grid_position.x > 0 &&
               grid_position.y > 0)
                passable_tile = floorTiles[grid_position.x][grid_position.y].isPassable();
            else
                return false;

            return passable_tile;
        }

        public bool is_void_tile(gridCoordinate grid_position)
        {
            if (grid_position.x >= 0 && grid_position.x < stdfloorSize &&
               grid_position.y >= 0 && grid_position.y < stdfloorSize)
                return floorTiles[grid_position.x][grid_position.y].isVoid();
            else
                return true;
        }

        //Green text. Function here.
        public bool isExit(gridCoordinate grid_position)
        {
            return floorTiles[grid_position.x][grid_position.y].isExit();
        }

        public bool isDungeonExit(gridCoordinate grid_position)
        {
            return floorTiles[grid_position.x][grid_position.y].get_my_tile_type() ==
                   Tile.Tile_Type.Dungeon_Exit;
        }

        //Green text. Function here.
        public bool am_i_on_other_monster(gridCoordinate grid_position, int cIndex)
        {
            bool retValue = false;
            for (int i = 0; i < badGuys.Count; i++)
            {
                if (badGuys[i].occupies_tile(grid_position) &&
                    badGuys[i].my_Index != cIndex)
                    retValue = true;
            }
            return retValue;
        }

        Tile nearest_visible_monster_tile(gridCoordinate origin, int within_Range)
        {
            int origin_x = origin.x;
            int origin_y = origin.y;
            //Vision_Log.Clear();
            //Add endpoints at max y- from x- to x+
            for (int i = -within_Range; i <= within_Range; i++)
            {
                gridCoordinate ray_end_point = new gridCoordinate(origin.x + i, origin.y - within_Range);
                Vision_Rc.Add(new VisionRay(origin, ray_end_point));
            }
            //Add endpoints at max y+ from x- to x+
            for (int i = -within_Range; i <= within_Range; i++)
            {
                gridCoordinate ray_end_point = new gridCoordinate(origin.x + i, origin.y + within_Range);
                Vision_Rc.Add(new VisionRay(origin, ray_end_point));
            }
            //Add endpoints at max x- from y- to y+
            for (int i = -within_Range + 1; i < within_Range; i++)
            {
                gridCoordinate ray_end_point = new gridCoordinate(origin.x - within_Range, origin.y + i);
                Vision_Rc.Add(new VisionRay(origin, ray_end_point));
            }
            //Add endpoints at max x+ from y- to y+
            for (int i = -within_Range + 1; i < within_Range; i++)
            {
                gridCoordinate ray_end_point = new gridCoordinate(origin.x + within_Range, origin.y + i);
                Vision_Rc.Add(new VisionRay(origin, ray_end_point));
            }

            int not_this_monster = -1;
            is_monster_here(origin, out not_this_monster);
            int monsterID = -1;
            Tile target_tile = null;
            while (Vision_Rc.Count > 0)
            {
                for (int i = 0; i < Vision_Rc.Count; i++)
                {
                    int c_coord_x = (int)Vision_Rc[i].my_current_position.X / 32;
                    int c_coord_y = (int)Vision_Rc[i].my_current_position.Y / 32;
                    gridCoordinate next_coord = new gridCoordinate(c_coord_x, c_coord_y);

                    if (is_monster_here(next_coord, out monsterID) && monsterID != not_this_monster)
                    {
                        target_tile = floorTiles[next_coord.x][next_coord.y];
                        Vision_Rc.Clear();
                    }
                    else
                    {
                        if (!is_tile_passable(next_coord) || Vision_Rc[i].is_at_end())
                            Vision_Rc.RemoveAt(i);
                        else
                            Vision_Rc[i].update();
                    }
                }
            }

            return target_tile;
        }

        //Green text. Function here.
        public gridCoordinate random_valid_position(Monster.Monster_Size entitySize = Monster.Monster_Size.Normal)
        {
            bool valid_position = false;
            List<gridCoordinate> g_coords = new List<gridCoordinate>();
            int size_modifier = 0;
            if (entitySize == Monster.Monster_Size.Large)
                size_modifier = 1;

            while (!valid_position)
            {
                int grid_basex = randGen.Next(stdfloorSize - size_modifier);
                int grid_basey = randGen.Next(stdfloorSize - size_modifier);
                g_coords.Add(new gridCoordinate(grid_basex, grid_basey));
                if(entitySize == Monster.Monster_Size.Large)
                {
                    g_coords.Add(new gridCoordinate(grid_basex + 1, grid_basey));
                    g_coords.Add(new gridCoordinate(grid_basex, grid_basey + 1));
                    g_coords.Add(new gridCoordinate(grid_basex + 1, grid_basey + 1));
                }

                bool good_position = true;
                //Check to make sure they can all be walked in
                for (int i = 0; i < g_coords.Count; i++)
                    if (!isWalkable(g_coords[i]))
                        good_position = false;
                //Make sure there are no monsters there.
                for (int i = 0; i < badGuys.Count; i++)
                    for (int j = 0; j < g_coords.Count; j++)
                        if (badGuys[i].occupies_tile(g_coords[j]))
                            good_position = false;
                //Make sure there are no doodads there
                for (int i = 0; i < Doodads.Count; i++)
                    for (int j = 0; j < g_coords.Count; j++)
                        if (Doodads[i].get_g_coord().x == g_coords[j].x &&
                            Doodads[i].get_g_coord().y == g_coords[j].y)
                            good_position = false;
                //make sure there are no gold piles there
                for (int i = 0; i < Money.Count; i++)
                    for (int j = 0; j < g_coords.Count; j++)
                        if (Money[i].get_my_grid_C().x == g_coords[j].x &&
                            Money[i].get_my_grid_C().y == g_coords[j].y)
                            good_position = false;
                //make sure it's not the exit
                for (int i = 0; i < g_coords.Count; i++)
                    if (floorTiles[g_coords[i].x][g_coords[i].y].get_my_tile_type() == Tile.Tile_Type.Exit)
                        good_position = false;

                if (good_position)
                    valid_position = true;
                else
                    g_coords.Clear();
            }

            return g_coords[0];
        }

        //Green text!
        public gridCoordinate valid_hollowKnight_spawn()
        {
            bool goodPosition = false;
            gridCoordinate returnCoord = random_valid_position();
            while (!goodPosition)
            {
                gridCoordinate gc = random_valid_position();
                if ((is_tile_passable(new gridCoordinate(gc.x - 1, gc.y)) && !is_tile_passable(new gridCoordinate(gc.x + 1, gc.y))) ||
                    (is_tile_passable(new gridCoordinate(gc.x + 1, gc.y)) && !is_tile_passable(new gridCoordinate(gc.x - 1, gc.y))) ||
                    (is_tile_passable(new gridCoordinate(gc.x, gc.y + 1)) && !is_tile_passable(new gridCoordinate(gc.x, gc.y - 1))) ||
                    (is_tile_passable(new gridCoordinate(gc.x, gc.y - 1)) && !is_tile_passable(new gridCoordinate(gc.x, gc.y + 1))))
                {
                    goodPosition = true;
                    returnCoord = gc;
                }
            }
            return returnCoord;
        }

        //Green text.
        public bool is_entity_here(gridCoordinate gc)
        {
            for (int i = 0; i < badGuys.Count; i++)
                if (badGuys[i].occupies_tile(gc))
                    return true;

            for (int i = 0; i < Money.Count; i++)
                if (Money[i].get_my_grid_C().x == gc.x && Money[i].get_my_grid_C().y == gc.y)
                    return true;

            for (int i = 0; i < Doodads.Count; i++)
                if (Doodads[i].get_g_coord().x == gc.x && Doodads[i].get_g_coord().y == gc.y)
                    return true;

            return false;
        }

        public bool is_monster_here(gridCoordinate gc, out int monsterID)
        {
            monsterID = -1;
            for (int i = 0; i < badGuys.Count; i++)
            {
                if (badGuys[i].occupies_tile(gc))
                {
                    monsterID = badGuys[i].my_Index;
                    return true;
                }
            }
            return false;
        }

        public bool is_destroyable_Doodad_here(gridCoordinate gc, out int DoodadIndex)
        {
            DoodadIndex = -1;
            for (int i = 0; i < Doodads.Count; i++)
            {
                gridCoordinate Doodad_coord = Doodads[i].get_g_coord();
                if (Doodad_coord.x == gc.x && Doodad_coord.y == gc.y && Doodads[i].is_destructible())
                {
                    DoodadIndex = i;
                    return true;
                }
            }
            return false;
        }

        public bool is_los_blocking_Doodad_here(gridCoordinate gc)
        {
            for (int i = 0; i < Doodads.Count; i++)
            {
                gridCoordinate Doodad_coord = Doodads[i].get_g_coord();
                if (Doodad_coord.x == gc.x && Doodad_coord.y == gc.y && Doodads[i].blocks_los)
                {
                    return true;
                }
            }
            return false;
        }

        List<gridCoordinate> determine_range_by_aoe_type(Scroll.Atk_Area_Type aoe_type,
                                                         gridCoordinate effect_center,
                                                         List<gridCoordinate> small_aoe_matrix,
                                                         int aoe_size)
        {
            List<gridCoordinate> range = new List<gridCoordinate>();

            switch (aoe_type)
            {
                case Scroll.Atk_Area_Type.piercingBolt:
                case Scroll.Atk_Area_Type.singleTile:
                case Scroll.Atk_Area_Type.chainedBolt:
                    range.Add(effect_center);
                    break;
                case Scroll.Atk_Area_Type.smallfixedAOE:
                    range = small_aoe_matrix;
                    break;
                case Scroll.Atk_Area_Type.cloudAOE:
                    List<List<bool>> cloud_matrix = generate_cloud_matrix(aoe_size);
                    List<List<bool>> valid_matrix = generate_valid_cloud_area_matrix(aoe_size, effect_center);
                    int half_size_floor = (int)Math.Floor((double)aoe_size / 2);

                    for (int x = effect_center.x - half_size_floor; x <= effect_center.x + half_size_floor; x++)
                    {
                        for (int y = effect_center.y - half_size_floor; y <= effect_center.y + half_size_floor; y++)
                        {
                            int x_equiv = x - effect_center.x + half_size_floor;
                            int y_equiv = y - effect_center.y + half_size_floor;
                            if (cloud_matrix[x_equiv][y_equiv] == true &&
                                valid_matrix[x_equiv][y_equiv] == true)
                            {
                                gridCoordinate cloud_coord = new gridCoordinate(x, y);
                                range.Add(cloud_coord);
                            }
                        }
                    }
                    break;
                case Scroll.Atk_Area_Type.solidblockAOE:
                    int half_aoe_size = (int)Math.Floor((double)(aoe_size / 2));

                    for (int x = effect_center.x - half_aoe_size; x <= effect_center.x + half_aoe_size; x++)
                        for (int y = effect_center.y - half_aoe_size; y <= effect_center.y + half_aoe_size; y++)
                        {
                            gridCoordinate current_coord = new gridCoordinate(x, y);
                            range.Add(current_coord);
                        }
                    break;
                case Scroll.Atk_Area_Type.randomblockAOE:
                    int half_r_aoe_size = (int)Math.Floor((double)(aoe_size / 2));
                    int effect_chance = 50;

                    for (int x = effect_center.x - half_r_aoe_size; x <= effect_center.x + half_r_aoe_size; x++)
                        for (int y = effect_center.y - half_r_aoe_size; y <= effect_center.y + half_r_aoe_size; y++)
                        {
                            gridCoordinate current_coord = new gridCoordinate(x, y);
                            int effect_roll = randGen.Next(100);
                            if (effect_roll < effect_chance)
                                range.Add(current_coord);
                        }
                    break;
            }

            return range;
        }

        public void check_click(Player pl, gridCoordinate click_loc, out bool bad_turn)
        {
            bad_turn = false;
            int pl_x_difference = pl.get_my_grid_C().x - click_loc.x;
            int pl_y_difference = pl.get_my_grid_C().y - click_loc.y;
            for (int i = 0; i < Doodads.Count; i++)
            {
                if (Doodads[i].get_g_coord().x == click_loc.x &&
                   Doodads[i].get_g_coord().y == click_loc.y &&
                   pl_x_difference > -2 && pl_x_difference < 2 &&
                   pl_y_difference > -2 && pl_y_difference < 2)
                    switch (Doodads[i].get_my_doodad_type())
                    {
                        case Doodad.Doodad_Type.Door:
                            if(!Doodads[i].is_door_destroyed())
                                if (!Doodads[i].is_door_closed())
                                {
                                    Doodads[i].close_door(this);
                                }
                                else
                                {
                                    bad_turn = true;
                                    Doodads[i].open_door(this);
                                }
                            break;
                    }
            }
        }

        #endregion

        #region projectile management (includes cloud generation, cone attacks & player projectile processing)

        public void update_all_projectiles(Player pl, float delta_time)
        {
            List<gridCoordinate> attacked_coordinates = new List<gridCoordinate>();
            bool aoe_effect = true;
            bool remove = false;
            bool damaging_prj = false;

            for (int i = 0; i < Pew_Pews.Count; i++)
            {
                damaging_prj = Pew_Pews[i].is_damaging_projectile();
                Pew_Pews[i].update(delta_time);
                if (Pew_Pews[i].get_atk_area_type() == Scroll.Atk_Area_Type.piercingBolt)
                {
                    gridCoordinate c_position = Pew_Pews[i].get_center_rect_GC();
                    gridCoordinate p_position = Pew_Pews[i].get_prev_coord();

                    if (c_position.x != p_position.x || c_position.y != p_position.y)
                        attacked_coordinates.Add(c_position);

                    Pew_Pews[i].set_prev_coord(c_position);
                    if (!is_tile_passable(c_position))
                        remove = true;
                }

                int min_damage = Pew_Pews[i].get_damage_range(false);
                int max_damage = Pew_Pews[i].get_damage_range(true);
                Attack.Damage dmg_type = Pew_Pews[i].get_dmg_type();

                //Do all this if it's at the end of the line
                if (check_overlap(Pew_Pews[i].my_rect(), new Rectangle((int)Pew_Pews[i].get_my_end_coord().x * 32, 
                                                                       (int)Pew_Pews[i].get_my_end_coord().y * 32, 32, 32)))
                {
                    //discharge stored attack then remove
                    gridCoordinate endCoord = Pew_Pews[i].get_my_end_coord();
                    
                    switch (Pew_Pews[i].get_atk_area_type())
                    {
                        case Scroll.Atk_Area_Type.piercingBolt:
                        case Scroll.Atk_Area_Type.singleTile:
                            aoe_effect = false;
                            break;
                        case Scroll.Atk_Area_Type.chainedBolt:
                            if (Pew_Pews[i].get_remaining_bounces() > 0)
                            {
                                Tile T = nearest_visible_monster_tile(endCoord, Pew_Pews[i].get_bounce());
                                if (T != null)
                                {
                                    Projectile p = new Projectile(endCoord, T.get_grid_c(), Pew_Pews[i].get_proj_type(),
                                                                  ref cManager, Pew_Pews[i].is_monster_projectile(),
                                                                  Scroll.Atk_Area_Type.chainedBolt);
                                    p.set_damage_range(Pew_Pews[i].get_damage_range(false), Pew_Pews[i].get_damage_range(true));
                                    p.set_bounce(Pew_Pews[i].get_bounce());
                                    p.set_bounces_left(Pew_Pews[i].get_remaining_bounces() - 1);
                                    p.set_damage_type(Pew_Pews[i].get_dmg_type());
                                    if (!p.is_monster_projectile())
                                        p.set_talisman_effects(Pew_Pews[i].get_talisman_effects());
                                    create_new_projectile(p);
                                }
                            }
                            break;
                        case Scroll.Atk_Area_Type.cloudAOE:
                            int cloud_size = Pew_Pews[i].get_aoe_size();
                            int cloud_duration = 1 + ((cloud_size-1)/2);
                            Floor.specific_effect pew_effect = specific_effect.None;
                            if (Pew_Pews[i].get_special_anim() == Projectile.special_anim.BloodAcid)
                                pew_effect = specific_effect.Acid_Blood;
                            PersistentEffect ceffect = new PersistentEffect(Scroll.Atk_Area_Type.cloudAOE,
                                                                            pew_effect, endCoord, cloud_duration, 
                                                                            Pew_Pews[i].is_monster_projectile(),
                                                                            dmg_type, cloud_size,
                                                                            min_damage, max_damage);
                            add_new_persistent_effect(ceffect);
                            if (Pew_Pews[i].is_monster_projectile())
                                persistent_effects[persistent_effects.Count - 1].ready_effect();
                            break;
                        case Scroll.Atk_Area_Type.randomblockAOE:
                            if (Pew_Pews[i].get_special_anim() == Projectile.special_anim.Earthquake)
                            {
                                PersistentEffect peffect = new PersistentEffect(Scroll.Atk_Area_Type.randomblockAOE,
                                                                                specific_effect.Earthquake,
                                                                                endCoord, 1, Pew_Pews[i].is_monster_projectile(),
                                                                                dmg_type, Pew_Pews[i].get_aoe_size(),
                                                                                min_damage, max_damage);
                                add_new_persistent_effect(peffect);
                            }
                            break;
                    }

                    attacked_coordinates = determine_range_by_aoe_type(Pew_Pews[i].get_atk_area_type(),
                                                                       endCoord, Pew_Pews[i].get_small_AOE_matrix(),
                                                                       Pew_Pews[i].get_aoe_size());
                    //Range determined, now execute all attacks.
                    remove = true;
                }
                //End of end of the line block

                int monsterID = -1;
                int DoodadID = -1;
                bool destroy_walls = Pew_Pews[i].projectile_destroys_walls();

                for (int j = 0; j < attacked_coordinates.Count; j++)
                {
                    Projectile.special_anim anim = Pew_Pews[i].get_special_anim();
                    if (is_tile_passable(attacked_coordinates[j]) ||
                        destroy_walls && acceptable_destruction_coordinate(attacked_coordinates[j]))
                    {
                        if (anim == Projectile.special_anim.None)
                            add_effect(dmg_type, attacked_coordinates[j]);
                        else
                            switch (anim)
                            {
                                case Projectile.special_anim.Earthquake:
                                    add_specific_effect(specific_effect.Earthquake, attacked_coordinates[j]);
                                    break;
                                case Projectile.special_anim.Alert:
                                    add_specific_effect(specific_effect.Alert, attacked_coordinates[j]);
                                    break;
                                case Projectile.special_anim.BloodAcid:
                                    add_specific_effect(specific_effect.Acid_Blood, attacked_coordinates[j]);
                                    break;
                            }
                        if (!is_tile_passable(attacked_coordinates[j]) && destroy_walls)
                        {
                            gridCoordinate target_coordinate = new gridCoordinate(attacked_coordinates[j]);
                            if (acceptable_destruction_coordinate(target_coordinate))
                            {
                                Tile target_tile = floorTiles[target_coordinate.x][target_coordinate.y];
                                target_tile.set_tile_type(Tile.Tile_Type.Rubble_Floor, rubble_floor);
                                replace_surrounding_void(target_tile, stone_walls, dirt_walls, rubble_wall);
                            }
                        }
                    }

                    if (damaging_prj)
                    {
                        int dmg_val = randGen.Next(min_damage, max_damage + 1);
                        if (Pew_Pews[i].is_monster_projectile())
                        {
                            if (pl.get_my_grid_C().x == attacked_coordinates[j].x &&
                               pl.get_my_grid_C().y == attacked_coordinates[j].y)
                            {
                                if (aoe_effect)
                                {
                                    pl.take_aoe_damage(min_damage, max_damage, dmg_type, this);
                                }
                                else
                                {
                                    Attack atk = new Attack(dmg_type, dmg_val);
                                    pl.take_damage(atk, this, "");
                                }
                            }

                            //Monsters can hurt each other and damage Doodads with AoE attacks too!
                            //Silver lining
                            int mon_on_mon_ID = -1;
                            if (is_monster_here(attacked_coordinates[j], out mon_on_mon_ID))
                                damage_monster_single_atk(new Attack(Pew_Pews[i].get_dmg_type(), dmg_val * 2), mon_on_mon_ID, false, aoe_effect);

                            int mon_on_Doodad_ID = -1;
                            if (is_destroyable_Doodad_here(attacked_coordinates[j], out mon_on_Doodad_ID))
                                damage_Doodad(dmg_val * 2, mon_on_Doodad_ID);
                        }
                        else
                        {
                            if (is_monster_here(attacked_coordinates[j], out monsterID))
                            {
                                Monster mon = badguy_by_monster_id(monsterID);
                                process_player_projectile_attack(Pew_Pews[i], mon, aoe_effect);
                            }

                            if (is_destroyable_Doodad_here(attacked_coordinates[j], out DoodadID))
                                damage_Doodad(dmg_val, DoodadID);

                            if (pl.get_my_grid_C().x == attacked_coordinates[j].x &&
                               pl.get_my_grid_C().y == attacked_coordinates[j].y)
                            {
                                int aoe_dmg_to_player = dmg_val / 2;
                                for (int k = 0; k < aoe_dmg_to_player; k++)
                                    pl.take_damage(new Attack(dmg_type, 1), this, "");
                            }
                        }
                    }
                }

                if (remove)
                    Pew_Pews.RemoveAt(i);
                attacked_coordinates.Clear();
            }
            //End of loop
        }

        public void create_new_projectile(Projectile proj)
        {
            Pew_Pews.Add(proj);
        }

        public void process_player_projectile_attack(Projectile pew, Monster m, bool aoe_effect)
        {
            List<Talisman> projectile_talismans = pew.get_talisman_effects();
            List<Attack> damage_to_monster = new List<Attack>();
            int projectile_damage = randGen.Next(pew.get_damage_range(false), pew.get_damage_range(true)+1);
            int projectile_modified_min_dmg = 0;
            int projectile_modified_max_dmg = 0;
            for (int i = 0; i < projectile_talismans.Count; i++)
            {
                if (projectile_talismans[i].get_my_type() == Talisman.Talisman_Type.Expediency)
                {
                    int base_val = (int)projectile_talismans[i].get_my_prefix() + 2;
                    projectile_modified_min_dmg += base_val;
                    projectile_modified_max_dmg += (base_val * 2);
                }
            }
            projectile_damage += randGen.Next(projectile_modified_min_dmg, projectile_modified_max_dmg + 1);
            damage_to_monster.Add(new Attack(pew.get_dmg_type(), projectile_damage));

            for (int i = 0; i < projectile_talismans.Count; i++)
            {
                if (projectile_talismans[i].extra_damage_specific_type_talisman())
                {
                    int base_val = (int)projectile_talismans[i].get_my_prefix() + 1;
                    Attack.Damage dmg_typ = 0;
                    switch (projectile_talismans[i].get_my_type())
                    {
                        case Talisman.Talisman_Type.Pressure:
                            dmg_typ = Attack.Damage.Crushing;
                            break;
                        case Talisman.Talisman_Type.Heat:
                            dmg_typ = Attack.Damage.Fire;
                            break;
                        case Talisman.Talisman_Type.Snow:
                            dmg_typ = Attack.Damage.Frost;
                            break;
                        case Talisman.Talisman_Type.Razors:
                            dmg_typ = Attack.Damage.Slashing;
                            break;
                        case Talisman.Talisman_Type.Heartsblood:
                            dmg_typ = Attack.Damage.Piercing;
                            break;
                        case Talisman.Talisman_Type.Toxicity:
                            dmg_typ = Attack.Damage.Acid;
                            break;
                        case Talisman.Talisman_Type.Sparks:
                            dmg_typ = Attack.Damage.Electric;
                            break;
                    }
                    damage_to_monster.Add(new Attack(dmg_typ, randGen.Next(base_val, (base_val * 2) + 1)));
                }
            }

            damage_monster(damage_to_monster, m.my_Index, false, aoe_effect);
        }

        public bool check_overlap(Rectangle rect_A, Rectangle rect_B)
        {
            return rect_A.Left < rect_B.Right && rect_A.Right > rect_B.Left &&
                rect_A.Top < rect_B.Bottom && rect_A.Bottom > rect_B.Top;
        }

        public bool projectiles_remaining_to_update()
        {
            return Pew_Pews.Count > 0;
        }

        //Used for a bunch of spells and other projectiles WHICH IS WHY IT'S UNDER THE
        //#PROJECTILES REGION
        public List<List<bool>> generate_cloud_matrix(int cloud_size)
        {
            List<List<bool>> cloud_matrix = new List<List<bool>>();
            int base_density = 50;
            //start with the rows
            for(int x = 0; x < cloud_size; x++)
            {
                cloud_matrix.Add(new List<bool>());
                for (int y = 0; y < cloud_size; y++)
                {
                    //first we figure out what ring we're in. This is based on the smaller
                    //of the 2 values - X or Y.
                    int cloud_ring = 0;
                    if (x < y)
                        cloud_ring = cloud_matrix_ring(cloud_size, x);
                    else
                        cloud_ring = cloud_matrix_ring(cloud_size, y);

                    int modified_density = base_density + ((cloud_ring - 1) * 5);
                    int density_roll = randGen.Next(100);
                    if (density_roll < modified_density)
                        cloud_matrix[x].Add(true);
                    else
                        cloud_matrix[x].Add(false);
                }
            }

            //center spot always hits.
            int middle_row = (int)(Math.Floor((double)(cloud_size / 2)));
            cloud_matrix[middle_row][middle_row] = true;

            return cloud_matrix;
        }

        public List<List<bool>> generate_valid_cloud_area_matrix(int cloud_size, gridCoordinate cloud_center)
        {
            int half_aoe_size_floor = (int)Math.Floor((double)cloud_size / 2);
            List<VisionRay> v_Area_Rays = new List<VisionRay>();
            List<List<bool>> valid_cloud_area = new List<List<bool>>();

            for (int x = 0; x < cloud_size; x++)
            {
                valid_cloud_area.Add(new List<bool>());
                for (int y = 0; y < cloud_size; y++)
                    valid_cloud_area[x].Add(false);
            }

            for (int i = -half_aoe_size_floor; i <= half_aoe_size_floor; i++)
            {
                gridCoordinate ray_end_point = new gridCoordinate(cloud_center.x + i, cloud_center.y - half_aoe_size_floor);
                v_Area_Rays.Add(new VisionRay(cloud_center, ray_end_point));
            }
            //Add endpoints at max y+ from x- to x+
            for (int i = -half_aoe_size_floor; i <= half_aoe_size_floor; i++)
            {
                gridCoordinate ray_end_point = new gridCoordinate(cloud_center.x + i, cloud_center.y + half_aoe_size_floor);
                v_Area_Rays.Add(new VisionRay(cloud_center, ray_end_point));
            }
            //Add endpoints at max x- from y- to y+
            for (int i = -half_aoe_size_floor + 1; i < half_aoe_size_floor; i++)
            {
                gridCoordinate ray_end_point = new gridCoordinate(cloud_center.x - half_aoe_size_floor, cloud_center.y + i);
                v_Area_Rays.Add(new VisionRay(cloud_center, ray_end_point));
            }
            //Add endpoints at max x+ from y- to y+
            for (int i = -half_aoe_size_floor + 1; i < half_aoe_size_floor; i++)
            {
                gridCoordinate ray_end_point = new gridCoordinate(cloud_center.x + half_aoe_size_floor, cloud_center.y + i);
                v_Area_Rays.Add(new VisionRay(cloud_center, ray_end_point));
            }
            int lowest_x_coord = cloud_center.x - half_aoe_size_floor;
            int lowest_y_coord = cloud_center.y - half_aoe_size_floor;
            while (v_Area_Rays.Count > 0)
            {
                for (int i = 0; i < v_Area_Rays.Count; i++)
                {
                    int current_ray_end_X = (int)v_Area_Rays[i].my_end_position.X / 32;
                    int current_ray_end_Y = (int)v_Area_Rays[i].my_end_position.Y / 32;

                    int current_ray_current_X = (int)v_Area_Rays[i].my_current_position.X / 32;
                    int current_ray_current_Y = (int)v_Area_Rays[i].my_current_position.Y / 32;
                    gridCoordinate current_Position = new gridCoordinate(current_ray_current_X,
                                                                         current_ray_current_Y);

                    int grid_x_equiv = current_ray_current_X - lowest_x_coord;
                    int grid_y_equiv = current_ray_current_Y - lowest_y_coord;

                    bool remove = false;
                    if (is_tile_passable(current_Position))
                        valid_cloud_area[grid_x_equiv][grid_y_equiv] = true;
                    else
                        remove = true;

                    if (v_Area_Rays[i].is_at_end())
                        remove = true;
                    
                    if(remove)
                        v_Area_Rays.RemoveAt(i);
                    else
                        v_Area_Rays[i].update();
                }
            }

            return valid_cloud_area;
        }

        int cloud_matrix_ring(int cloud_size, int target_number)
        {
            for (int i = 0; i < (cloud_size / 2) + 1; i++)
            {
                if (target_number == i || target_number == cloud_size - (i + 1))
                    return i + 1;
            }

            return -1;
        }

        public void cone_attack(int cone_range, gridCoordinate origin, gridCoordinate.direction cone_direction,
                                int cone_atk_max_dmg, int cone_atk_min_dmg, Attack.Damage cone_atk_dmg_type,
                                Player pl, bool monsterCone, specific_effect spEfx)
        {
            gridCoordinate target = new gridCoordinate(origin);
            for (int i = 0; i < cone_range; i++)
                target.shift_direction(cone_direction);

            List<gridCoordinate> endPoints = new List<gridCoordinate>();
            endPoints.Add(target);
            switch (cone_direction)
            {
                case gridCoordinate.direction.Up:
                case gridCoordinate.direction.Down:
                    endPoints.Add(new gridCoordinate(target.x + 2, target.y));
                    endPoints.Add(new gridCoordinate(target.x + 1, target.y));
                    endPoints.Add(new gridCoordinate(target.x - 2, target.y));
                    endPoints.Add(new gridCoordinate(target.x - 1, target.y));
                    break;
                case gridCoordinate.direction.Left:
                case gridCoordinate.direction.Right:
                    endPoints.Add(new gridCoordinate(target.x, target.y - 2));
                    endPoints.Add(new gridCoordinate(target.x, target.y - 1));
                    endPoints.Add(new gridCoordinate(target.x, target.y + 2));
                    endPoints.Add(new gridCoordinate(target.x, target.y + 1));
                    break;
                case gridCoordinate.direction.UpLeft:
                    endPoints.Add(new gridCoordinate(target.x, target.y + 2));
                    endPoints.Add(new gridCoordinate(target.x, target.y + 1));
                    endPoints.Add(new gridCoordinate(target.x + 2, target.y));
                    endPoints.Add(new gridCoordinate(target.x + 1, target.y));
                    break;
                case gridCoordinate.direction.UpRight:
                    endPoints.Add(new gridCoordinate(target.x, target.y + 2));
                    endPoints.Add(new gridCoordinate(target.x, target.y + 1));
                    endPoints.Add(new gridCoordinate(target.x - 2, target.y));
                    endPoints.Add(new gridCoordinate(target.x - 1, target.y));
                    break;
                case gridCoordinate.direction.DownLeft:
                    endPoints.Add(new gridCoordinate(target.x, target.y - 2));
                    endPoints.Add(new gridCoordinate(target.x, target.y - 1));
                    endPoints.Add(new gridCoordinate(target.x + 2, target.y));
                    endPoints.Add(new gridCoordinate(target.x + 1, target.y));
                    break;
                case gridCoordinate.direction.DownRight:
                    endPoints.Add(new gridCoordinate(target.x, target.y - 2));
                    endPoints.Add(new gridCoordinate(target.x, target.y - 1));
                    endPoints.Add(new gridCoordinate(target.x - 2, target.y));
                    endPoints.Add(new gridCoordinate(target.x - 1, target.y));
                    break;
            }

            List<gridCoordinate> effected_area = new List<gridCoordinate>();
            for (int i = 0; i < endPoints.Count; i++)
                Vision_Rc.Add(new VisionRay(origin, endPoints[i]));

            while (Vision_Rc.Count > 0)
            {
                for (int i = 0; i < Vision_Rc.Count; i++)
                {
                    int x_coordinate = (int)Vision_Rc[i].my_current_position.X / 32;
                    int y_coordinate = (int)Vision_Rc[i].my_current_position.Y / 32;

                    bool add_to_area = true;
                    for (int j = 0; j < effected_area.Count; j++)
                    {
                        if (effected_area[j].x == x_coordinate && effected_area[j].y == y_coordinate)
                            add_to_area = false;
                    }
                    if (add_to_area)
                        effected_area.Add(floorTiles[x_coordinate][y_coordinate].get_grid_c());

                    bool remove = false;
                    if (!is_tile_passable(new gridCoordinate(x_coordinate, y_coordinate)) ||
                        Vision_Rc[i].is_at_end())
                        remove = true;

                    if (remove)
                        Vision_Rc.RemoveAt(i);
                    else
                        Vision_Rc[i].update();
                }
            }

            int original_size = effected_area.Count;
            for (int i = 0; i < original_size; i++)
                for (int j = 0; j < effected_area.Count; j++)
                    if (effected_area[j].x == origin.x && effected_area[j].y == origin.y)
                        effected_area.RemoveAt(j);

            for (int i = 0; i < effected_area.Count; i++)
            {
                if (is_tile_passable(effected_area[i]))
                    if (spEfx == specific_effect.None)
                        add_effect(cone_atk_dmg_type, effected_area[i]);
                    else
                        add_specific_effect(spEfx, effected_area[i]);

                int monsterID;
                int doodadID;
                if (is_monster_here(effected_area[i], out monsterID))
                {
                    int dmg = randGen.Next(cone_atk_min_dmg, cone_atk_max_dmg + 1);
                    if (monsterCone)
                        dmg = dmg * 2;
                    damage_monster_single_atk(new Attack(cone_atk_dmg_type, dmg), monsterID, false, true);
                }
                else if (is_destroyable_Doodad_here(effected_area[i], out doodadID))
                {
                    int dmg = randGen.Next(cone_atk_min_dmg, cone_atk_max_dmg + 1);
                    if (monsterCone)
                        dmg = dmg * 2;
                    damage_Doodad(dmg, doodadID);
                }
                else if (pl.get_my_grid_C().x == effected_area[i].x &&
                        pl.get_my_grid_C().y == effected_area[i].y)
                {
                    if (monsterCone)
                        pl.take_aoe_damage(cone_atk_min_dmg, cone_atk_max_dmg,
                                           cone_atk_dmg_type, this);
                }
            }
        }

        #endregion

        #region effect management

        public void add_effect(Attack.Damage effect_type, gridCoordinate fx_coord)
        {
            switch (effect_type)
            {
                case Attack.Damage.Slashing:
                    eff_ex.Add(new Effect(cManager.Load<Texture2D>("Projectiles/slashing_spritesheet"), 5, fx_coord));
                    break;
                case Attack.Damage.Piercing:
                    eff_ex.Add(new Effect(cManager.Load<Texture2D>("Projectiles/piercing_spritesheet"), 8, fx_coord));
                    break;
                case Attack.Damage.Crushing:
                    eff_ex.Add(new Effect(cManager.Load<Texture2D>("Projectiles/impact_spritesheet"), 6, fx_coord));
                    break;
                case Attack.Damage.Fire:
                    eff_ex.Add(new Effect(cManager.Load<Texture2D>("Projectiles/fire_spritesheet"), 7, fx_coord));
                    break;
                case Attack.Damage.Frost:
                    eff_ex.Add(new Effect(cManager.Load<Texture2D>("Projectiles/frost_spritesheet"), 7, fx_coord));
                    break;
                case Attack.Damage.Acid:
                    eff_ex.Add(new Effect(cManager.Load<Texture2D>("Projectiles/acid_spritesheet"), 11, fx_coord));
                    break;
                case Attack.Damage.Electric:
                    eff_ex.Add(new Effect(cManager.Load<Texture2D>("Projectiles/electric_spritesheet"), 9, fx_coord));
                    break;
            }
        }

        public void add_specific_effect(specific_effect effect, gridCoordinate fx_coord)
        {
            switch (effect)
            {
                case specific_effect.Power_Strike:
                    eff_ex.Add(new Effect(cManager.Load<Texture2D>("Projectiles/powerstrike_spritesheet"), 6, fx_coord));
                    break;
                case specific_effect.Cleave:
                    eff_ex.Add(new Effect(cManager.Load<Texture2D>("Projectiles/cleave_spritesheet"), 5, fx_coord));
                    break;
                case specific_effect.Earthquake:
                    eff_ex.Add(new Effect(cManager.Load<Texture2D>("Projectiles/earthquake_spritesheet"), 7, fx_coord));
                    break;
                case specific_effect.Acid_Blood:
                    eff_ex.Add(new Effect(cManager.Load<Texture2D>("Projectiles/acidblood_spritesheet"), 11, fx_coord));
                    break;
                case specific_effect.Bite:
                    eff_ex.Add(new Effect(cManager.Load<Texture2D>("Projectiles/bite_spritesheet"), 10, fx_coord));
                    break;
                case specific_effect.Big_Bite:
                    eff_ex.Add(new Effect(cManager.Load<Texture2D>("Projectiles/big_bite_spritesheet"), 10, fx_coord));
                    break;
                case specific_effect.Alert:
                    eff_ex.Add(new Effect(cManager.Load<Texture2D>("Projectiles/alert_spritesheet"), 16, fx_coord));
                    break;
                case specific_effect.Warning_Bracket:
                    eff_ex.Add(new Effect(cManager.Load<Texture2D>("Projectiles/warning_reticule_spritesheet"), 23, fx_coord, 16));
                    break;
            }
        }

        public void update_all_effects(float delta_time)
        {
            for (int i = 0; i < eff_ex.Count; i++)
            {
                if(i == 0)
                    eff_ex[i].update(delta_time);
                else if (i > 0)
                {
                    if (eff_ex[i - 1].get_my_position() != eff_ex[i].get_my_position())
                        eff_ex[i].update(delta_time);
                }

                if (eff_ex[i].slated_for_removal())
                    eff_ex.RemoveAt(i);
            }
        }

        #endregion

        #region Persistent Effect Management

        public void add_new_persistent_effect(PersistentEffect pEffect)
        {
            persistent_effects.Add(pEffect);
        }

        public void execute_persistent_effects(Player pl)
        {
            for(int i = 0; i < persistent_effects.Count; i++)
            {
                if (persistent_effects[i].is_effect_ready())
                {
                    Scroll.Atk_Area_Type eType = persistent_effects[i].get_my_effect_type();
                    Attack.Damage effect_dmg_type = persistent_effects[i].get_my_damage_type();
                    int effect_max_damage = persistent_effects[i].get_specific_damage(true);
                    int effect_min_damage = persistent_effects[i].get_specific_damage(false);

                    gridCoordinate effect_center = persistent_effects[i].get_center();
                    List<gridCoordinate> effected_tiles = new List<gridCoordinate>();

                    effected_tiles = determine_range_by_aoe_type(eType, effect_center, null, 
                                                                 persistent_effects[i].get_effect_size());

                    int monsterID = -1;
                    int DoodadID = -1;

                    for (int j = 0; j < effected_tiles.Count; j++)
                    {
                        if (is_tile_passable(effected_tiles[j]))
                        {
                            if (persistent_effects[i].get_my_special_fx() == specific_effect.None)
                                add_effect(effect_dmg_type, effected_tiles[j]);
                            else if (persistent_effects[i].get_my_special_fx() == specific_effect.Earthquake)
                                add_specific_effect(specific_effect.Earthquake, effected_tiles[j]);
                            else if (persistent_effects[i].get_my_special_fx() == specific_effect.Acid_Blood)
                                add_specific_effect(specific_effect.Acid_Blood, effected_tiles[j]);
                        }

                        int dmg_val = randGen.Next(effect_min_damage, effect_max_damage + 1);
                        if (persistent_effects[i].is_monster_effect())
                        {
                            if (pl.get_my_grid_C().x == effected_tiles[j].x &&
                               pl.get_my_grid_C().y == effected_tiles[j].y)
                            {
                                pl.take_aoe_damage(effect_min_damage, effect_max_damage, effect_dmg_type, this);
                            }
                        }
                        else
                        {
                            if (is_monster_here(effected_tiles[j], out monsterID))
                            {
                                damage_monster_single_atk(new Attack(effect_dmg_type, dmg_val), monsterID, false, true);
                            }

                            if (is_destroyable_Doodad_here(effected_tiles[j], out DoodadID))
                                damage_Doodad(dmg_val, DoodadID);

                            if (pl.get_my_grid_C().x == effected_tiles[j].x &&
                               pl.get_my_grid_C().y == effected_tiles[j].y)
                            {
                                int aoe_dmg_to_player = dmg_val / 2;
                                for (int k = 0; k < aoe_dmg_to_player; k++)
                                    pl.take_damage(new Attack(effect_dmg_type, 1), this, "");
                            }
                        }
                    }

                    persistent_effects[i].adjust_turns_remaining(-1);
                }
                else
                    persistent_effects[i].ready_effect();   
            }

            for (int i = persistent_effects.Count - 1; i >= 0; i--)
                if (persistent_effects[i].get_turns_left() == 0)
                    persistent_effects.RemoveAt(i);
        }

        #endregion

        #region popup management

        public void update_all_popups(float delta_time)
        {
            for (int i = 0; i < popup_alerts.Count; i++)
            {
                if(i == 0)
                    popup_alerts[i].update(delta_time);
                else if (i > 0)
                {
                    float y_difference = popup_alerts[i].my_position.Y - popup_alerts[i - 1].my_position.Y;
                    float desired_y_difference = popup_msg_font.LineSpacing;
                    bool is_in_same_gc = (popup_alerts[i - 1].gc_origin.x == popup_alerts[i].gc_origin.x) &&
                                        (popup_alerts[i - 1].gc_origin.y == popup_alerts[i].gc_origin.y);
                    if (!is_in_same_gc || y_difference > desired_y_difference)
                        popup_alerts[i].update(delta_time);
                }
                if (popup_alerts[i].time_until_vanish <= 0)
                    popup_alerts.RemoveAt(i);
            }
        }

        public void add_new_popup(string txt, Popup.popup_msg_color msg_color, gridCoordinate gc)
        {
            popup_alerts.Add(new Popup(txt, msg_color, popup_msg_font, gc));
        }

        public void add_new_popup(string txt, Popup.popup_msg_color msg_color, Vector2 cd)
        {
            popup_alerts.Add(new Popup(txt, msg_color, popup_msg_font, cd));
        }

        public bool popups_remaining_to_update()
        {
            return popup_alerts.Count > 0;
        }

        #endregion

        #region all sensory stuff

        #region smell stuff
        //Green text. Function here.
        public void add_smell_to_tile(gridCoordinate grid_position, int sType, int sValue)
        {
            floorTiles[grid_position.x][grid_position.y].addScent(sType, sValue);
            scentedTiles.Add(floorTiles[grid_position.x][grid_position.y]);
        }

        //Green text.
        public void decay_all_scents()
        {
            for (int i = 0; i < scentedTiles.Count; i++)
            {
                scentedTiles[i].decayScents();
                if (!scentedTiles[i].any_scent_present())
                    scentedTiles.RemoveAt(i);
            }
        }

        //Green text.
        public void scent_pulse_raycast(gridCoordinate origin, int targetSmell, Monster theMon, int smellRange, int smellThreshold)
        {
            int origin_x = origin.x;
            int origin_y = origin.y;
            //Vision_Log.Clear();
            //Add endpoints at max y- from x- to x+
            for (int i = -smellRange; i <= smellRange; i++)
            {
                gridCoordinate ray_end_point = new gridCoordinate(origin.x + i, origin.y - smellRange);
                Vision_Rc.Add(new VisionRay(origin, ray_end_point));
                //Vision_Log.Add(new VisionRay(origin, ray_end_point));
            }
            //Add endpoints at max y+ from x- to x+
            for (int i = -smellRange; i <= smellRange; i++)
            {
                gridCoordinate ray_end_point = new gridCoordinate(origin.x + i, origin.y + smellRange);
                Vision_Rc.Add(new VisionRay(origin, ray_end_point));
                //Vision_Log.Add(new VisionRay(origin, ray_end_point));
            }
            //Add endpoints at max x- from y- to y+
            for (int i = -smellRange + 1; i < smellRange; i++)
            {
                gridCoordinate ray_end_point = new gridCoordinate(origin.x - smellRange, origin.y + i);
                Vision_Rc.Add(new VisionRay(origin, ray_end_point));
                //Vision_Log.Add(new VisionRay(origin, ray_end_point));
            }
            //Add endpoints at max x+ from y- to y+
            for (int i = -smellRange + 1; i < smellRange; i++)
            {
                gridCoordinate ray_end_point = new gridCoordinate(origin.x + smellRange, origin.y + i);
                Vision_Rc.Add(new VisionRay(origin, ray_end_point));
                //Vision_Log.Add(new VisionRay(origin, ray_end_point));
            }
            int strongest_smell_value = 0;
            while (Vision_Rc.Count > 0)
            {
                for (int i = 0; i < Vision_Rc.Count; i++)
                {
                    int xPosition = (int)Vision_Rc[i].my_current_position.X / 32;
                    int yPosition = (int)Vision_Rc[i].my_current_position.Y / 32;
                    int current_smell = floorTiles[xPosition][yPosition].strength_of_scent(targetSmell);
                    if (current_smell > strongest_smell_value && current_smell > smellThreshold)
                    {
                        strongest_smell_value = current_smell;
                        theMon.has_scent = true;
                        theMon.strongest_smell_coord.x = xPosition;
                        theMon.strongest_smell_coord.y = yPosition;
                    }

                    if (Vision_Rc[i].is_at_end() || floorTiles[xPosition][yPosition].isOpaque())
                        Vision_Rc.RemoveAt(i);
                    else
                        Vision_Rc[i].update();
                }
            }
        }

        public Tile establish_los_strongest_smell(gridCoordinate origin, int smellrange, 
                                                  int targetSmell, int smell_threshold)
        {
            //We sort these by scent.
            Tile target_tile = null;
            int strongest_smell = 0;
            for (int i = scentedTiles.Count - 1; i >= 0; i--)
            {
                if ((scentedTiles[i].get_grid_c().x <= origin.x + smellrange || scentedTiles[i].get_grid_c().x >= origin.x - smellrange) &&
                    (scentedTiles[i].get_grid_c().y <= origin.y + smellrange || scentedTiles[i].get_grid_c().y >= origin.y - smellrange) &&
                    scentedTiles[i].strength_of_scent(targetSmell) >= smell_threshold &&
                    scentedTiles[i].strength_of_scent(targetSmell) > strongest_smell)
                {
                    Tile test_tile = scentedTiles[i];
                    if (establish_los(origin, test_tile.get_grid_c()))
                    {
                        strongest_smell = scentedTiles[i].strength_of_scent(targetSmell);
                        target_tile = scentedTiles[i];
                        //target_tile.set_my_aura(Tile.Aura.SmellTarget);
                    }
                }
            }

            return target_tile;
        }

        public bool check_for_smellable_smell(gridCoordinate my_grid_coord, int targetSmell, int smell_threshold, int radius)
        {
            int min_x_val = my_grid_coord.x - radius;
            int max_x_val = my_grid_coord.x + radius;
            int min_y_val = my_grid_coord.y - radius;
            int max_y_val = my_grid_coord.y + radius;
            for (int i = 0; i < scentedTiles.Count; i++)
            {
                gridCoordinate g_c = scentedTiles[i].get_grid_c();
                if (g_c.x >= min_x_val && g_c.x <= max_x_val &&
                    g_c.y >= min_y_val && g_c.y <= max_y_val &&
                    scentedTiles[i].strength_of_scent(targetSmell) >= smell_threshold)
                    return true;
            }

            return false;
        }

        #endregion

        #region sound stuff
        //Green text.
        public bool does_tile_deflect(gridCoordinate grid_position)
        {
            if (grid_position.x > 0 && grid_position.x < stdfloorSize &&
                grid_position.y > 0 && grid_position.y < stdfloorSize)
                return floorTiles[grid_position.x][grid_position.y].isDeflector();
            else
                return true;
        }

        //Green text.
        public int tile_absorbtion_value(gridCoordinate grid_position)
        {
            if (grid_position.x > 0 && grid_position.x < stdfloorSize &&
                grid_position.y > 0 && grid_position.y < stdfloorSize)
                return floorTiles[grid_position.x][grid_position.y].sound_absorb_val();
            else
                return 1000;
        }

        //Green text.
        public void sound_pulse(gridCoordinate origin, int soundRange, SoundPulse.Sound_Types soundType)
        {
            int mults = 4;
            //Do this 8 times, once for each direction.
            Noises.Add(new SoundPulse(new gridCoordinate(origin.x, origin.y - 1), origin, 0, soundRange, ref randGen, mults, soundType));
            Noises.Add(new SoundPulse(new gridCoordinate(origin.x, origin.y + 1), origin, 1, soundRange, ref randGen, mults, soundType));
            Noises.Add(new SoundPulse(new gridCoordinate(origin.x - 1, origin.y), origin, 2, soundRange, ref randGen, mults, soundType));
            Noises.Add(new SoundPulse(new gridCoordinate(origin.x + 1, origin.y), origin, 3, soundRange, ref randGen, mults, soundType));
            Noises.Add(new SoundPulse(new gridCoordinate(origin.x + 1, origin.y + 1), origin, 4, soundRange, ref randGen, mults, soundType));
            Noises.Add(new SoundPulse(new gridCoordinate(origin.x - 1, origin.y + 1), origin, 5, soundRange, ref randGen, mults, soundType));
            Noises.Add(new SoundPulse(new gridCoordinate(origin.x + 1, origin.y - 1), origin, 6, soundRange, ref randGen, mults, soundType));
            Noises.Add(new SoundPulse(new gridCoordinate(origin.x - 1, origin.y - 1), origin, 7, soundRange, ref randGen, mults, soundType));

            while (Noises.Count > 0)
            {
                for (int i = 0; i < Noises.Count; i++)
                {
                    for (int j = 0; j < badGuys.Count; j++)
                    {
                        if(badGuys[j].occupies_tile(Noises[i].my_coord()))
                            if(badGuys[j].can_hear && badGuys[j].can_hear_sound(Noises[i].get_my_type(), Noises[i].my_strength()))
                                badGuys[j].next_path_to_sound(Noises[i].my_path(), Noises[i].get_my_type());
                    }

                    if (Noises[i].my_strength() > 0)
                        Noises[i].update(this);
                    else
                        Noises.RemoveAt(i);
                }
            }
        }

        //Green text.
        public void add_single_sound_pulse(SoundPulse pulse)
        {
            Noises.Add(pulse);
        }

        #endregion

        #region sight stuff
        //Green text.
        public bool is_tile_opaque(gridCoordinate grid_position)
        {
            return floorTiles[grid_position.x][grid_position.y].isOpaque();
        }

        //This IS be deprecated.
        //I kept it in the code because it might be useful for a future release and it's also
        //nice to see how much progress you've made from a primitive method of doing things.
        public void sight_pulse_raycast(gridCoordinate origin, Player pl, Monster theMon, int sightRange)
        {
            int origin_x = origin.x;
            int origin_y = origin.y;
            //Vision_Log.Clear();
            //Add endpoints at max y- from x- to x+
            for (int i = -sightRange; i <= sightRange; i++)
            {
                gridCoordinate ray_end_point = new gridCoordinate(origin.x + i, origin.y - sightRange);
                Vision_Rc.Add(new VisionRay(origin, ray_end_point));
                //Vision_Log.Add(new VisionRay(origin, ray_end_point));
            }
            //Add endpoints at max y+ from x- to x+
            for (int i = -sightRange; i <= sightRange; i++)
            {
                gridCoordinate ray_end_point = new gridCoordinate(origin.x + i, origin.y + sightRange);
                Vision_Rc.Add(new VisionRay(origin, ray_end_point));
                //Vision_Log.Add(new VisionRay(origin, ray_end_point));
            }
            //Add endpoints at max x- from y- to y+
            for (int i = -sightRange+1; i < sightRange; i++)
            {
                gridCoordinate ray_end_point = new gridCoordinate(origin.x - sightRange, origin.y + i);
                Vision_Rc.Add(new VisionRay(origin, ray_end_point));
                //Vision_Log.Add(new VisionRay(origin, ray_end_point));
            }
            //Add endpoints at max x+ from y- to y+
            for (int i = -sightRange+1; i < sightRange; i++)
            {
                gridCoordinate ray_end_point = new gridCoordinate(origin.x + sightRange, origin.y + i);
                Vision_Rc.Add(new VisionRay(origin, ray_end_point));
                //Vision_Log.Add(new VisionRay(origin, ray_end_point));
            }
            while (Vision_Rc.Count > 0)
            {
                bool has_seen_player = false;
                for (int i = 0; i < Vision_Rc.Count; i++)
                {
                    int my_grid_x_position = (int)(Vision_Rc[i].my_current_position.X / 32);
                    int my_grid_y_position = (int)(Vision_Rc[i].my_current_position.Y / 32);
                    if (pl.get_my_grid_C().x == my_grid_x_position && pl.get_my_grid_C().y == my_grid_y_position)
                    {
                        theMon.can_see_player = true;
                        has_seen_player = true;
                    }

                    if (Vision_Rc[i].is_at_end() || floorTiles[my_grid_x_position][my_grid_y_position].isOpaque())
                        Vision_Rc.RemoveAt(i);
                    else
                        Vision_Rc[i].update();

                    if (has_seen_player)
                        Vision_Rc.Clear();
                }
            }
        }

        public bool establish_los(gridCoordinate origin, gridCoordinate destination, 
                                  VisionRay.fineness fineness = VisionRay.fineness.Average)
        {
            Tile originTile = floorTiles[origin.x][origin.y];
            Tile destinationTile = floorTiles[destination.x][destination.y];
            
            Vision_Rc.Add(new VisionRay(originTile.get_corner(1), destinationTile.get_corner(1), fineness));
            Vision_Rc.Add(new VisionRay(originTile.get_corner(2), destinationTile.get_corner(2), fineness));
            Vision_Rc.Add(new VisionRay(originTile.get_corner(3), destinationTile.get_corner(3), fineness));
            Vision_Rc.Add(new VisionRay(originTile.get_corner(4), destinationTile.get_corner(4), fineness));
            Vision_Rc.Add(new VisionRay(originTile.get_corner(1), destinationTile.get_corner(3), fineness));
            Vision_Rc.Add(new VisionRay(originTile.get_corner(2), destinationTile.get_corner(4), fineness));

            //Comment this out when we're done with the vision log
            /*
            Vision_Log.Add(new VisionRay(originTile.get_corner(1), destinationTile.get_corner(1)));
            Vision_Log.Add(new VisionRay(originTile.get_corner(2), destinationTile.get_corner(2)));
            Vision_Log.Add(new VisionRay(originTile.get_corner(3), destinationTile.get_corner(3)));
            Vision_Log.Add(new VisionRay(originTile.get_corner(4), destinationTile.get_corner(4)));
            Vision_Log.Add(new VisionRay(originTile.get_corner(3), destinationTile.get_corner(1)));
            Vision_Log.Add(new VisionRay(originTile.get_corner(2), destinationTile.get_corner(4)));
            */

            bool established_los = false;
            while (Vision_Rc.Count > 0)
            {
                for (int i = 0; i < Vision_Rc.Count; i++)
                {
                    bool remove = false;
                    int my_grid_x_position = (int)(Vision_Rc[i].my_current_position.X / 32);
                    int my_grid_y_position = (int)(Vision_Rc[i].my_current_position.Y / 32);

                    if (floorTiles[my_grid_x_position][my_grid_y_position].isOpaque() &&
                        !is_same_coordinate(origin, new gridCoordinate(my_grid_x_position, my_grid_y_position)))
                        remove = true;

                    for (int j = 0; j < Doodads.Count; j++)
                        if (Doodads[j].get_g_coord().x == my_grid_x_position && 
                            Doodads[j].get_g_coord().y == my_grid_y_position &&
                            Doodads[j].blocks_los)
                            remove = true;

                    if (Vision_Rc[i].is_at_end() &&
                       floorTiles[my_grid_x_position][my_grid_y_position].get_grid_c().x == destination.x &&
                       floorTiles[my_grid_x_position][my_grid_y_position].get_grid_c().y == destination.y)
                    {
                        remove = true;
                        established_los = true;
                    }

                    if (remove)
                        Vision_Rc.RemoveAt(i);
                    else
                        Vision_Rc[i].update();
                }
            }

            Vision_Rc.Clear();
            return established_los;
        }

        #endregion

        #endregion

        #region drawing stuff
        //Green text. Function here.
        public void drawBackground(ref SpriteBatch sBatch)
        {
            for (int x = 0; x < stdfloorSize; x++)
            {
                for (int y = 0; y < stdfloorSize; y++)
                {
                    floorTiles[x][y].drawMe(ref sBatch);
                }
            }
        }

        public void draw_tile_auras(ref SpriteBatch sBatch)
        {
            for (int x = 0; x < stdfloorSize; x++)
            {
                for (int y = 0; y < stdfloorSize; y++)
                {
                    floorTiles[x][y].draw_my_aura(ref sBatch);
                }
            }
        }

        public void drawProjectile(ref SpriteBatch sBatch)
        {
            for (int i = 0; i < Pew_Pews.Count; i++)
            {
                Pew_Pews[i].drawMe(ref sBatch);
            }
        }

        public void drawEffect(ref SpriteBatch sBatch)
        {
            for (int i = 0; i < eff_ex.Count; i++)
            {
                if (i == 0)
                    eff_ex[i].draw_me(ref sBatch);
                else if (i > 0)
                {
                    if (eff_ex[i - 1].get_my_position() != eff_ex[i].get_my_position())
                        eff_ex[i].draw_me(ref sBatch);
                }
            }
        }

        public void drawPopup(ref SpriteBatch sBatch)
        {
            for (int i = 0; i < popup_alerts.Count; i++)
            {
                if (i == 0)
                    popup_alerts[i].draw_me(ref sBatch);
                else if (i > 0)
                {
                    float y_difference = popup_alerts[i].my_position.Y - popup_alerts[i - 1].my_position.Y;
                    float desired_y_difference = popup_msg_font.LineSpacing;
                    bool is_in_same_gc = (popup_alerts[i - 1].gc_origin.x == popup_alerts[i].gc_origin.x) &&
                                        (popup_alerts[i - 1].gc_origin.y == popup_alerts[i].gc_origin.y);
                    if (!is_in_same_gc || y_difference > desired_y_difference)
                        popup_alerts[i].draw_me(ref sBatch);
                }
            }
        }

        //Green text. Function here.
        public void drawEntities(ref SpriteBatch sBatch)
        {
            for (int i = 0; i < Money.Count; i++)
                Money[i].drawMe(ref sBatch);

            for (int i = 0; i < Doodads.Count; i++)
                Doodads[i].draw_me(ref sBatch);
        }
        
        /*
        public void draw_vision_log(ref SpriteBatch sBatch, Texture2D blank_tex)
        {
            for (int i = 0; i < Vision_Log.Count; i++)
            {
                Vector2 point2 = new Vector2(Vision_Log[i].my_end_position.X, Vision_Log[i].my_end_position.Y);
                Vector2 point1 = new Vector2(Vision_Log[i].my_current_position.X, Vision_Log[i].my_current_position.Y);
                float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
                float length = Vector2.Distance(point1, point2);

                sBatch.Draw(blank_tex, point1, null, Color.White,
                            angle, Vector2.Zero, new Vector2(length, 1),
                            SpriteEffects.None, 0);
            }
        }
        */

        //Green text.
        public void drawEnemies(ref SpriteBatch sBatch)
        {
            Texture2D sound_shit = cManager.Load<Texture2D>("sound shit");
            for (int i = 0; i < badGuys.Count; i++)
            {
                badGuys[i].drawMe(ref sBatch);
                /*
                if(badGuys[i] is Skeleton)
                {
                    Skeleton skelskel = (Skeleton)badGuys[i];
                    sBatch.Draw(sound_shit, new Vector2(skelskel.last_seen_player_at.x * 32, skelskel.last_seen_player_at.y * 32), Color.White);
                }
                */
            }
        }

        #endregion

        #region some miscellaneous access stuff
        //Green text. Function here.
        public List<Monster> see_badGuys()
        {
            return badGuys;
        }

        public void force_monster_wander(int monsterID, Player pl)
        {
            for (int i = 0; i < badGuys.Count; i++)
                if (badGuys[i].my_Index == monsterID)
                    badGuys[i].wander(pl, this, badGuys[i].is_corporeal());
        }

        public Monster badguy_by_monster_id(int index)
        {
            for (int i = 0; i < badGuys.Count; i++)
                if (badGuys[i].my_Index == index)
                    return badGuys[i];

            return null;
        }

        public Doodad Doodad_by_index(int index)
        {
            if (index != -1 && index < Doodads.Count)
                return Doodads[index];
            else
                return null;
        }

        public List<Goldpile> show_me_the_money()
        {
            return Money;
        }

        public void damage_monster(List<Attack> atks, int monsterID, bool melee_attack, bool aoe_attack)
        {
            for (int i = 0; i < badGuys.Count; i++)
            {
                if (badGuys[i].my_Index == monsterID)
                {
                    badGuys[i].takeDamage(atks, melee_attack, aoe_attack, message_buffer, this);
                    if (badGuys[i].hitPoints <= 0)
                    {
                        if (badGuys[i] is HollowKnight)
                        {
                            Doodads.Add(new Doodad(Doodad.Doodad_Type.Destroyed_ArmorSuit, cManager, badGuys[i].randomly_chosen_personal_coord(), Doodads.Count));
                        }
                        if (badGuys[i].boss_monster)
                        {
                            unlock_dungeon_exit();
                        }
                        badGuys.RemoveAt(i);
                    }
                }
            }
        }

        public void damage_monster_single_atk(Attack atk, int monsterID, bool melee_attack, bool aoe_attack)
        {
            List<Attack> atks = new List<Attack>();
            atks.Add(atk);
            damage_monster(atks, monsterID, melee_attack, aoe_attack);
        }

        public void damage_Doodad(int dmg, int DoodadID)
        {
            Doodads[DoodadID].take_damage(dmg, message_buffer, this);
        }

        public void remove_doodad_at_index(int index)
        {
            for (int i = 0; i < Doodads.Count; i++)
            {
                if (Doodads[i].get_my_index() == index)
                    Doodads.RemoveAt(i);
            }
        }

        public void set_tile_aura(gridCoordinate target_tile, Tile.Aura target_aura)
        {
            floorTiles[target_tile.x][target_tile.y].set_my_aura(target_aura);
        }

        public void scrub_all_auras()
        {
            for (int x = 0; x < stdfloorSize; x++)
                for (int y = 0; y < stdfloorSize; y++)
                    floorTiles[x][y].set_my_aura(Tile.Aura.None);
        }

        public Tile.Aura aura_of_specific_tile(gridCoordinate target_tile)
        {
            return floorTiles[target_tile.x][target_tile.y].get_my_aura();
        }

        public void open_door_here(gridCoordinate door_coord)
        {
            for (int i = 0; i < Doodads.Count; i++)
            {
                if (Doodads[i].get_g_coord().x == door_coord.x &&
                    Doodads[i].get_g_coord().y == door_coord.y &&
                    Doodads[i].get_my_doodad_type() == Doodad.Doodad_Type.Door)
                    Doodads[i].open_door(this);
            }
        }

        public void consume_mana(int mana)
        {
            ambient_manavalue -= mana;
        }

        public int check_mana()
        {
            return ambient_manavalue;
        }

        private bool is_same_coordinate(gridCoordinate gc1, gridCoordinate gc2)
        {
            return gc1.x == gc2.x && gc1.y == gc2.y;
        }

        public int get_fl_size()
        {
            return stdfloorSize;
        }

        #endregion
    }
}