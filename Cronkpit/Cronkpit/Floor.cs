using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using CKPLibrary;

namespace Cronkpit
{
    class Floor
    {
        //Constants
        protected int stdfloorSize = 50;
        public enum specific_effect { None, Power_Strike, Cleave, Earthquake,
                                      Acid_Blood, Bite, Big_Bite, Alert,
                                      Warning_Bracket };
        public enum random_coord_restrictions { None, Monster, Entrance };
        //Floor components
        int fl_number;
        int ambient_manavalue;
        //Controls what's actually displayed.
        List<List<Tile>> floorTiles;
        List<Tile> scentedTiles;
        List<Room> roomLayout;
        List<Hall> hallLayout;
        List<NaturalFeature> featureLayout;
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
        gridCoordinate dungeon_exit_coord;
        gridCoordinate dungeon_entrance_coord;

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
        List<KeyValuePair<Tile.Tile_Type, Texture2D>> master_textureList;
        //Mossy textures - present in the necropolis
        Texture2D[] mossy_stonewalls = new Texture2D[2];
        Texture2D[] mossy_stonefloors = new Texture2D[5];
        Texture2D[] mossy_dirtfloors = new Texture2D[2];
        Texture2D[] mossy_dirtwalls = new Texture2D[2];

        //Green text. Function here.
        public Floor(ContentManager sCont, ref List<string> msgBuffer, Texture2D blnkTex, int floor_number,
                     Cronkpit.CronkPit.Dungeon dungeon)
        {
            //Init floor components.
            floorTiles = new List<List<Tile>>();
            scentedTiles = new List<Tile>();
            roomLayout = new List<Room>();
            hallLayout = new List<Hall>();
            featureLayout = new List<NaturalFeature>();
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
            master_textureList = new List<KeyValuePair<Tile.Tile_Type, Texture2D>>();

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

        //All of this is dungeon building stuff
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

        public void unlock_dungeon_exit()
        {
            for (int x = 0; x < stdfloorSize; x++)
                for (int y = 0; y < stdfloorSize; y++)
                    if (floorTiles[x][y].get_my_tile_type() == Tile.Tile_Type.Locked_Dungeon_Exit)
                        floorTiles[x][y].set_tile_type(Tile.Tile_Type.Dungeon_Exit, master_textureList);
        }

        #endregion       

        #region dungeon building functions

        public void buildFloor(Cronkpit.CronkPit.Dungeon cDungeon)
        {
            master_textureList.Add(new KeyValuePair<Tile.Tile_Type,Texture2D>(Tile.Tile_Type.Rubble_Floor, cManager.Load<Texture2D>("Background/rubble_floor")));
            master_textureList.Add(new KeyValuePair<Tile.Tile_Type,Texture2D>(Tile.Tile_Type.Rubble_Wall, cManager.Load<Texture2D>("Background/rubble_wall")));
            switch (cDungeon)
            {
                case CronkPit.Dungeon.Necropolis:
                    build_necropolis_floor();
                    break;
            }
        }

        #region specific dungeon building functions

        public void build_necropolis_floor()
        {
            string basePath = "Background/Necropolis/Necro_";
            //Make the base tiles.
            //Void
            master_textureList.Add(new KeyValuePair<Tile.Tile_Type,Texture2D>(Tile.Tile_Type.Void,cManager.Load<Texture2D>("Background/badvoidtile")));
            //Stone Walls
            master_textureList.Add(new KeyValuePair<Tile.Tile_Type,Texture2D>(Tile.Tile_Type.StoneWall, cManager.Load<Texture2D>(basePath + "StoneWall")));
            master_textureList.Add(new KeyValuePair<Tile.Tile_Type,Texture2D>(Tile.Tile_Type.StoneWall, cManager.Load<Texture2D>(basePath + "StoneWall2")));
            master_textureList.Add(new KeyValuePair<Tile.Tile_Type,Texture2D>(Tile.Tile_Type.StoneWall, cManager.Load<Texture2D>(basePath + "StoneWallTorch")));
            //Stone Floors
            master_textureList.Add(new KeyValuePair<Tile.Tile_Type,Texture2D>(Tile.Tile_Type.StoneFloor, cManager.Load<Texture2D>(basePath + "StoneFloor")));
            master_textureList.Add(new KeyValuePair<Tile.Tile_Type,Texture2D>(Tile.Tile_Type.StoneFloor, cManager.Load<Texture2D>(basePath + "StoneFloorCracked")));
            master_textureList.Add(new KeyValuePair<Tile.Tile_Type,Texture2D>(Tile.Tile_Type.StoneFloor, cManager.Load<Texture2D>(basePath + "StoneFloorCracked2")));
            //Dirt Floors
            master_textureList.Add(new KeyValuePair<Tile.Tile_Type,Texture2D>(Tile.Tile_Type.DirtFloor, cManager.Load<Texture2D>(basePath + "Dirtfloor")));
            master_textureList.Add(new KeyValuePair<Tile.Tile_Type,Texture2D>(Tile.Tile_Type.DirtFloor, cManager.Load<Texture2D>(basePath + "DirtFloor2")));
            //Dirt Walls
            master_textureList.Add(new KeyValuePair<Tile.Tile_Type,Texture2D>(Tile.Tile_Type.DirtWall, cManager.Load<Texture2D>(basePath + "DirtWall")));
            master_textureList.Add(new KeyValuePair<Tile.Tile_Type,Texture2D>(Tile.Tile_Type.DirtWall, cManager.Load<Texture2D>(basePath + "DirtWall2")));
            master_textureList.Add(new KeyValuePair<Tile.Tile_Type,Texture2D>(Tile.Tile_Type.DirtWall, cManager.Load<Texture2D>(basePath + "DirtWallTorch")));
            //Exit + Entrance Tile
            master_textureList.Add(new KeyValuePair<Tile.Tile_Type,Texture2D>(Tile.Tile_Type.Exit, cManager.Load<Texture2D>("Background/exit")));
            master_textureList.Add(new KeyValuePair<Tile.Tile_Type,Texture2D>(Tile.Tile_Type.Entrance, cManager.Load<Texture2D>("Background/entrance")));
            //Dungeon Exit Tile
            master_textureList.Add(new KeyValuePair<Tile.Tile_Type,Texture2D>(Tile.Tile_Type.Dungeon_Exit, cManager.Load<Texture2D>(basePath + "dungeon_exit_open")));
            master_textureList.Add(new KeyValuePair<Tile.Tile_Type,Texture2D>(Tile.Tile_Type.Dungeon_Exit, cManager.Load<Texture2D>(basePath + "dungeon_exit_closed")));
            //Water
            master_textureList.Add(new KeyValuePair<Tile.Tile_Type, Texture2D>(Tile.Tile_Type.Shallow_Water, cManager.Load<Texture2D>("Background/shallow_water")));
            //Deep water
            master_textureList.Add(new KeyValuePair<Tile.Tile_Type, Texture2D>(Tile.Tile_Type.Deep_Water, cManager.Load<Texture2D>("Background/deep_water")));
            //Gravel
            master_textureList.Add(new KeyValuePair<Tile.Tile_Type, Texture2D>(Tile.Tile_Type.Gravel, cManager.Load<Texture2D>(basePath + "Gravel")));
            //Blood
            master_textureList.Add(new KeyValuePair<Tile.Tile_Type, Texture2D>(Tile.Tile_Type.Shallow_Blood, cManager.Load<Texture2D>("Background/shallow_blood")));
            //MOSS
            mossy_dirtfloors[0] = cManager.Load<Texture2D>(basePath + "MossDirtFloor");
            mossy_dirtfloors[1] = cManager.Load<Texture2D>(basePath + "MossDirtFloor2");
            mossy_stonefloors[0] = cManager.Load<Texture2D>(basePath + "StoneFloorMoss");
            mossy_stonefloors[1] = cManager.Load<Texture2D>(basePath + "StoneFloorMoss2");
            mossy_stonefloors[2] = cManager.Load<Texture2D>(basePath + "StoneFloorMoss3");
            mossy_stonefloors[3] = cManager.Load<Texture2D>(basePath + "StoneFloorCrackedMoss");
            mossy_stonefloors[4] = cManager.Load<Texture2D>(basePath + "StoneFloorCrackedMoss2");
            mossy_dirtwalls[0] = cManager.Load<Texture2D>(basePath + "MossDirtWall");
            mossy_dirtwalls[1] = cManager.Load<Texture2D>(basePath + "MossDirtWall2");
            mossy_stonewalls[0] = cManager.Load<Texture2D>(basePath + "StoneWallMoss");
            mossy_stonewalls[1] = cManager.Load<Texture2D>(basePath + "StoneWallMoss2");

            //Next, load up the instructions for that specific floor.
            FloorThemeDC[] Fl_theme = cManager.Load<FloorThemeDC[]>("XmlData/Dungeons/necropolis_floors");
            FloorThemeDC my_floorTheme = null;
            //Then find the one pertinent to this floor.
            for (int i = 0; i < Fl_theme.Count(); i++)
            {
                if (Fl_theme[i].FloorNumber == fl_number)
                    my_floorTheme = Fl_theme[i];
            }
            List<Room.Room_Type> room_types;
            List<NaturalFeature.Feature_Type> features;
            parse_theme_instructions(my_floorTheme, out room_types, out features);

            for (int x = 0; x < stdfloorSize; x++)
            {
                floorTiles.Add(new List<Tile>());
                for (int y = 0; y < stdfloorSize; y++)
                {
                    Vector2 tilePos = new Vector2(x * 32, y * 32);
                    floorTiles[x].Add(new Tile(Tile.Tile_Type.Void, randGen.Next(100), cManager, blank_texture, tilePos, new gridCoordinate(x, y), master_textureList));
                }
            }

            //Randomly generate rooms and hallways.
            //First rooms.
            int number_of_rooms = 5 + randGen.Next(4) - room_types.Count;
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

                Room.Room_Type r_type = Room.Room_Type.Generic;
                Tile.Tile_Type f_type = Tile.Tile_Type.StoneFloor;

                int dirt_dice_roll = randGen.Next(100);
                if (dirt_dice_roll < dirt_threshold &&
                    (lower_room_x_edge < 15 || upper_room_x_edge > 35 ||
                    lower_room_y_edge < 15 || upper_room_y_edge > 35))
                    f_type = Tile.Tile_Type.DirtFloor;

                if (next_room_height == next_room_width && next_room_width % 2 == 1)
                    if (randGen.Next(3) < 2)
                        r_type = Room.Room_Type.GenericCircular;

                bool door_room = false;
                if (r_type != Room.Room_Type.GenericCircular && randGen.Next(4) == 0)
                    door_room = true;

                Room rm = new Room(next_room_height,
                                    next_room_width,
                                    next_room_startX,
                                    next_room_startY,
                                    r_type,
                                    f_type,
                                    door_room);

                int corpse_chance = fl_number * 5;
                rm.set_corpses(corpse_chance);

                roomLayout.Add(rm);
            }

            roomLayout.AddRange(parse_room_instructions(cManager.Load<RoomDC[]>("XmlData/Dungeons/general_rooms"), room_types));
            
            //Next hallways.
            int number_of_hallways = number_of_rooms + room_types.Count - 1;
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

            //next, put in features.
            for (int i = 0; i < features.Count; i++)
            {
                if (features[i] == NaturalFeature.Feature_Type.River)
                {
                    bool river_is_okay = false;
                    int river_length = randGen.Next(10, 31);
                    int min_x_coord;
                    int max_x_coord;
                    int min_y_coord;
                    int max_y_coord;
                    clamp_river_coordinates(out min_x_coord, out max_x_coord, out min_y_coord, out max_y_coord);
                    gridCoordinate river_start = new gridCoordinate(randGen.Next(50), randGen.Next(50));
                    gridCoordinate river_end = new gridCoordinate(randGen.Next(50), randGen.Next(50));
                    while (!river_is_okay)
                    {
                        river_start = new gridCoordinate(randGen.Next(50), randGen.Next(50));
                        river_end = new gridCoordinate(randGen.Next(50), randGen.Next(50));
                        int c_river_length = 0;

                        if (river_start.x == river_end.x)
                            c_river_length = Math.Abs(river_start.x - river_end.x);
                        else if (river_start.y == river_end.y)
                            c_river_length = Math.Abs(river_start.y - river_end.y);
                        else
                        {
                            int a = river_start.x - river_end.x;
                            int b = river_start.y - river_end.y;
                            c_river_length = (int)Math.Sqrt((a * a) + (b * b));
                        }

                        if (c_river_length == river_length)
                            river_is_okay = true;
                    }
                    int deepwater_thickness = randGen.Next(2);
                    int shallows_thickness = 1 + randGen.Next(3);
                    int banks_thickness = 1 + randGen.Next(2);
                    featureLayout.Add(new NaturalFeature(river_start, river_end, deepwater_thickness,
                                                                                 shallows_thickness,
                                                                                 banks_thickness));
                }
            }

            //Alter void tiles to floor tiles.
            //First rooms
            for (int i = 0; i < roomLayout.Count; i++)
                roomLayout[i].render_to_board(floorTiles, master_textureList);

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

            //Then we put any features over them, damn the consequences.
            for (int i = 0; i < featureLayout.Count; i++)
            {
                switch (featureLayout[i].get_type())
                {
                    case NaturalFeature.Feature_Type.River:
                        featureLayout[i].draw_river(ref floorTiles, randGen,
                                                    Tile.Tile_Type.Deep_Water,
                                                    Tile.Tile_Type.Shallow_Water,
                                                    Tile.Tile_Type.Gravel, master_textureList);
                        break;
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
                                floorTiles[x][exit_coord.y].set_tile_type(Tile.Tile_Type.Exit, master_textureList);
                                dungeon_exit_coord = new gridCoordinate(x, exit_coord.y);
                                exitPlaced = true;
                            }
                            else
                            {
                                floorTiles[x][exit_coord.y].set_tile_type(Tile.Tile_Type.Locked_Dungeon_Exit, master_textureList);
                                dungeon_exit_coord = new gridCoordinate(x, exit_coord.y);
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
                                floorTiles[exit_coord.x][y].set_tile_type(Tile.Tile_Type.Exit, master_textureList);
                                dungeon_exit_coord = new gridCoordinate(exit_coord.x, y);
                                exitPlaced = true;
                            }
                            else
                            {
                                floorTiles[exit_coord.x][y].set_tile_type(Tile.Tile_Type.Locked_Dungeon_Exit, master_textureList);
                                dungeon_exit_coord = new gridCoordinate(exit_coord.x, y);
                                exitPlaced = true;
                            }
                        }
                    }
            }

            //Then place an entrance.
            bool entranceplaced = false;
            while (!entranceplaced)
            {
                gridCoordinate entrance_coord = random_valid_position(Monster.Monster_Size.Normal,
                                                                      random_coord_restrictions.Entrance);

                for(int x = entrance_coord.x - 1; x <= entrance_coord.x + 1; x++)
                    for(int y = entrance_coord.y - 1; y <= entrance_coord.y + 1; y++)
                        if (x < stdfloorSize && x > 0 && y < stdfloorSize && y > 0 &&
                           (x == entrance_coord.x || y == entrance_coord.y) && !entranceplaced &&
                           floorTiles[x][y].isVoid())
                        {
                            floorTiles[x][y].set_tile_type(Tile.Tile_Type.Entrance, master_textureList);
                            dungeon_entrance_coord = new gridCoordinate(x, y);
                            entranceplaced = true;
                        }
            }

            Texture2D[] lookup_table = generate_overlay_table();
            //add walls around all walkable tiles & figure out overlays.
            for (int x = 0; x < stdfloorSize; x++)
                for (int y = 0; y < stdfloorSize; y++)
                {
                    if (floorTiles[x][y].isPassable())
                        replace_surrounding_void(floorTiles[x][y]);
                    if (floorTiles[x][y].isOverlaid())
                        floorTiles[x][y].set_all_overlays(floorTiles, lookup_table);
                }

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
                            !floorTiles[x][roomY - 1].isExitorEntrance())
                        {
                            side_door_coords.Clear();
                            side_door_coords.Add(new gridCoordinate(x - 1, roomY - 1));
                            side_door_coords.Add(new gridCoordinate(x + 1, roomY - 1));
                            add_door(new gridCoordinate(x, roomY - 1), side_door_coords);
                        }

                        if (!is_tile_passable(new gridCoordinate(x - 1, roomY + roomH)) &&
                            !is_tile_passable(new gridCoordinate(x + 1, roomY + roomH)) &&
                            is_tile_passable(new gridCoordinate(x, roomY + roomH)) &&
                            !floorTiles[x][roomY + roomH].isExitorEntrance())
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
                            !floorTiles[roomX - 1][y].isExitorEntrance())
                        {
                            side_door_coords.Clear();
                            side_door_coords.Add(new gridCoordinate(roomX - 1, y-1));
                            side_door_coords.Add(new gridCoordinate(roomX - 1, y+1));
                            add_door(new gridCoordinate(roomX - 1, y), side_door_coords);
                        }

                        if (!is_tile_passable(new gridCoordinate(roomX + roomW, y - 1)) &&
                            !is_tile_passable(new gridCoordinate(roomX + roomW, y + 1)) &&
                            is_tile_passable(new gridCoordinate(roomX + roomW, y)) &&
                            !floorTiles[roomX + roomW][y].isExitorEntrance())
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
                    if (roomLayout[i].get_room_type() == Room.Room_Type.GenericCircular)
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
            //Add Doodads based on room first
            for (int i = 0; i < roomLayout.Count; i++)
                roomLayout[i].render_doodads_to_board(ref Doodads, cManager);

            //Add other Doodads
            int suits = randGen.Next(Math.Max(0, 4 - fl_number), Math.Max(2, 6 - fl_number));
            for (int i = 0; i < suits; i++)
                Doodads.Add(new Doodad(Doodad.Doodad_Type.ArmorSuit, cManager,
                            valid_hollowKnight_spawn(), i));
            
            //Add corpses
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

                            if (isWalkable(target_corner_gc) &&
                               !isWalkable(new gridCoordinate(target_corner_gc.x + x_dir_check, target_corner_gc.y)) &&
                               !isWalkable(new gridCoordinate(target_corner_gc.x, target_corner_gc.y + y_dir_check)))
                                Doodads.Add(new Doodad(Doodad.Doodad_Type.CorpsePile, cManager, target_corner_gc, Doodads.Count));
                            if (number_of_corpses >= 2)
                            {
                                gridCoordinate pile_2 = new gridCoordinate(target_corner_gc.x + (x_dir_check * -1), target_corner_gc.y);
                                if (isWalkable(pile_2) &&
                                    !isWalkable(new gridCoordinate(pile_2.x, pile_2.y + y_dir_check)))
                                    Doodads.Add(new Doodad(Doodad.Doodad_Type.CorpsePile, cManager, pile_2, Doodads.Count));
                            }
                            if (number_of_corpses == 3)
                            {
                                gridCoordinate pile_3 = new gridCoordinate(target_corner_gc.x, target_corner_gc.y + (y_dir_check * -1));
                                if (isWalkable(pile_3) &&
                                    !isWalkable(new gridCoordinate(pile_3.x + x_dir_check, pile_3.y)))
                                    Doodads.Add(new Doodad(Doodad.Doodad_Type.CorpsePile, cManager, pile_3, Doodads.Count));
                            }
                        }
                    }
                }
            }
            //Add corpse mimics
            List<Doodad> corpses = new List<Doodad>();
            for (int i = 0; i < Doodads.Count; i++)
                if (Doodads[i].get_my_doodad_type() == Doodad.Doodad_Type.CorpsePile)
                    corpses.Add(Doodads[i]);

            int corpse_mimic_cap = Math.Max(2, fl_number - 8);
            if (fl_number < 4)
                corpse_mimic_cap = 0;
            int c_corpse_mimics = 0;
            int corpse_mimic_chance = fl_number * 2;
            for (int i = 0; i < corpse_mimic_cap; i++)
            {
                if (randGen.Next(100) < corpse_mimic_chance)
                {
                    int chosen_corpse = randGen.Next(corpses.Count);
                    gridCoordinate mimic_Position = corpses[chosen_corpse].get_g_coord();
                    int doodad_ID = corpses[chosen_corpse].get_my_index();
                    for (int j = 0; j < Doodads.Count; j++)
                        if(Doodads[j].get_my_index() == doodad_ID)
                            Doodads.RemoveAt(j);
                    c_corpse_mimics++;
                    badGuys.Add(new CorpseMimic(mimic_Position, cManager, badGuys.Count, fl_number));
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
            add_random_monsters(number_of_monsters, CronkPit.Dungeon.Necropolis, ref badGuys);
            for (int i = 0; i < roomLayout.Count; i++)
                roomLayout[i].render_monsters_to_board(ref badGuys, this);
        }

        #endregion

        public void parse_theme_instructions(FloorThemeDC theme_instructions,
                                             out List<Room.Room_Type> rTypes,
                                             out List<NaturalFeature.Feature_Type> fTypes)
        {
            rTypes = new List<Room.Room_Type>();
            fTypes = new List<NaturalFeature.Feature_Type>();

            if (theme_instructions != null)
            {
                List<string> instructions = theme_instructions.Roomlist;
                for (int i = 0; i < instructions.Count; i++)
                {
                    String[] command = instructions[i].Split(' ');
                    int qty = -1;
                    Int32.TryParse(command[0], out qty);

                    Room.Room_Type c_rType = Room.Room_Type.Generic;
                    NaturalFeature.Feature_Type c_fType = NaturalFeature.Feature_Type.Generic;

                    string command_remainder = "";
                    for (int s = 1; s < command.Count(); s++)
                        command_remainder += command[s] + " ";
                    command_remainder = command_remainder.Trim();
                    switch (command_remainder)
                    {
                        case "Gorehound Kennels":
                            c_rType = Room.Room_Type.GHound_Kennel;
                            break;
                        case "Library":
                            c_rType = Room.Room_Type.Library;
                            break;
                        case "Darkroom":
                            c_rType = Room.Room_Type.DarkRoom;
                            break;
                        case "Corpse Storage":
                            c_rType = Room.Room_Type.CorpseStorage;
                            break;
                        case "Sewer":
                            c_rType = Room.Room_Type.SewerRoom;
                            break;
                        case "Rubble Room":
                            c_rType = Room.Room_Type.Destroyed;
                            break;
                        case "Knight Armory":
                            c_rType = Room.Room_Type.KnightArmory;
                            break;
                        case "Sewer Shaft":
                            c_rType = Room.Room_Type.SewerShaft;
                            break;
                        case "Jail":
                            c_rType = Room.Room_Type.Jail;
                            break;
                        case "Mine Shaft":
                            c_rType = Room.Room_Type.MineShaft;
                            break;
                        case "Chasm":
                            c_fType = NaturalFeature.Feature_Type.Chasm;
                            break;
                        case "River":
                            c_fType = NaturalFeature.Feature_Type.River;
                            break;
                        case "Lake":
                            c_fType = NaturalFeature.Feature_Type.Lake;
                            break;
                    }

                    if (c_rType != Room.Room_Type.Generic)
                        for (int j = 0; j < qty; j++)
                            rTypes.Add(c_rType);

                    if (c_fType != NaturalFeature.Feature_Type.Generic)
                        for (int j = 0; j < qty; j++)
                            fTypes.Add(c_fType);
                }
            }
        }

        public List<Room> parse_room_instructions(RoomDC[] room_list, List<Room.Room_Type> roomTypes)
        {
            List<Room> rooms = new List<Room>();
            List<RoomDC> valid_rooms = new List<RoomDC>();

            for (int i = 0; i < roomTypes.Count; i++)
            {
                if(valid_rooms.Count == 0)
                    for (int j = 0; j < room_list.Count(); j++)
                        if (room_type_match(room_list[j].RoomType, roomTypes[i]))
                            valid_rooms.Add(room_list[j]);

                if (valid_rooms.Count > 0)
                {
                    int chosen_room = randGen.Next(valid_rooms.Count);
                    rooms.Add(new Room(valid_rooms[chosen_room], floorSpawns));
                }

                if (roomTypes[i] != roomTypes[Math.Min(roomTypes.Count - 1, i + 1)])
                    valid_rooms.Clear();
            }

            for (int i = 0; i < rooms.Count; i++)
            {
                int nextX = randGen.Next(1, ((stdfloorSize - 1) - rooms[i].roomWidth));
                int nextY = randGen.Next(1, ((stdfloorSize - 1) - rooms[i].roomHeight));
                rooms[i].reset_starting_position(nextX, nextY);
            }

            return rooms;
        }

        public Texture2D[] generate_overlay_table()
        {
            Texture2D texture_1 = cManager.Load<Texture2D>("Background/Water Overlays/3"); //North Edge only
            Texture2D texture_2 = cManager.Load<Texture2D>("Background/Water Overlays/1"); //East Edge only
            Texture2D texture_3 = cManager.Load<Texture2D>("Background/Water Overlays/12"); //East Edge + North Edge
            Texture2D texture_4 = cManager.Load<Texture2D>("Background/Water Overlays/4"); //South Edge only
            Texture2D texture_5 = cManager.Load<Texture2D>("Background/Water Overlays/35"); //South Edge + North Edge
            Texture2D texture_6 = cManager.Load<Texture2D>("Background/Water Overlays/9"); //South Edge + East Edge
            Texture2D texture_7 = cManager.Load<Texture2D>("Background/Water Overlays/13"); //All edges but west
            Texture2D texture_8 = cManager.Load<Texture2D>("Background/Water Overlays/2"); //Only West Edge
            Texture2D texture_9 = cManager.Load<Texture2D>("Background/Water Overlays/11"); //West + North Edge
            Texture2D texture_10 = cManager.Load<Texture2D>("Background/Water Overlays/36"); //East + West Edge
            Texture2D texture_11 = cManager.Load<Texture2D>("Background/Water Overlays/15"); //All edges but south
            Texture2D texture_12 = cManager.Load<Texture2D>("Background/Water Overlays/10"); //West + South Edge
            Texture2D texture_13 = cManager.Load<Texture2D>("Background/Water Overlays/14"); //All edges but east
            Texture2D texture_14 = cManager.Load<Texture2D>("Background/Water Overlays/16"); //All edges but north
            Texture2D texture_15 = cManager.Load<Texture2D>("Background/Water Overlays/17"); //All edges

            Texture2D texture_16 = cManager.Load<Texture2D>("Background/Water Overlays/8"); //NW corner
            Texture2D texture_17 = cManager.Load<Texture2D>("Background/Water Overlays/18"); //East edge + NW corner
            Texture2D texture_18 = cManager.Load<Texture2D>("Background/Water Overlays/23"); //Sourth edge + NW corner
            Texture2D texture_19 = cManager.Load<Texture2D>("Background/Water Overlays/33"); //South Edge + East Edge + NW Corner
            
            Texture2D texture_20 = cManager.Load<Texture2D>("Background/Water Overlays/7"); //NE corner
            Texture2D texture_21 = cManager.Load<Texture2D>("Background/Water Overlays/22"); //South Edge + NE Corner
            Texture2D texture_22 = cManager.Load<Texture2D>("Background/Water Overlays/21"); //West Edge + NE Corner
            Texture2D texture_23 = cManager.Load<Texture2D>("Background/Water Overlays/32"); //South Edge + west Edge + NE Corner
 
            Texture2D texture_24 = cManager.Load<Texture2D>("Background/Water Overlays/1"); //ne corner + nw corner NEED TO MAKE THIS
            Texture2D texture_25 = cManager.Load<Texture2D>("Background/Water Overlays/27"); //south edge + ne corner + nw corner
 
            Texture2D texture_26 = cManager.Load<Texture2D>("Background/Water Overlays/6"); //SE Corner
            Texture2D texture_27 = cManager.Load<Texture2D>("Background/Water Overlays/24"); //North Edge + SE corner
            Texture2D texture_28 = cManager.Load<Texture2D>("Background/Water Overlays/20"); //West Edge + SE corner
            Texture2D texture_29 = cManager.Load<Texture2D>("Background/Water Overlays/31"); //West Edge + North Edge + SE Corner

            Texture2D texture_30 = cManager.Load<Texture2D>("Background/Water Overlays/1"); //SE corner + NW corner NEED TO MAKE THIS

            Texture2D texture_31 = cManager.Load<Texture2D>("Background/Water Overlays/1"); //SE corner + NE corner NEED TO MAKE THIS
            Texture2D texture_32 = cManager.Load<Texture2D>("Background/Water Overlays/29"); //West Edge + SE Corner + NE corner

            Texture2D texture_33 = cManager.Load<Texture2D>("Background/Water Overlays/1"); //SE NE NW corners NEED TO MAKE THIS

            Texture2D texture_34 = cManager.Load<Texture2D>("Background/Water Overlays/5"); //SW Corner
            Texture2D texture_35 = cManager.Load<Texture2D>("Background/Water Overlays/25"); //North Edge + SW Corner
            Texture2D texture_36 = cManager.Load<Texture2D>("Background/Water Overlays/19"); //East Edge + SW Corner
            Texture2D texture_37 = cManager.Load<Texture2D>("Background/Water Overlays/31"); //East Edge + North Edge + SW Corner
 
            Texture2D texture_38 = cManager.Load<Texture2D>("Background/Water Overlays/1"); //SW NW Corners NEED TO MAKE THIS
            Texture2D texture_39 = cManager.Load<Texture2D>("Background/Water Overlays/28"); //East Edge + SW corner + NW Corner

            Texture2D texture_40 = cManager.Load<Texture2D>("Background/Water Overlays/1"); //SW NE Corners NEED TO MAKE THIS

            Texture2D texture_41 = cManager.Load<Texture2D>("Background/Water Overlays/1"); //SW NE NW Corners NEED TO MAKE THIS

            Texture2D texture_42 = cManager.Load<Texture2D>("Background/Water Overlays/1"); //SW SE Corners NEED TO MAKE THIS
            Texture2D texture_43 = cManager.Load<Texture2D>("Background/Water Overlays/26"); //North Edge + SE + SW Corners
            
            Texture2D texture_44 = cManager.Load<Texture2D>("Background/Water Overlays/1"); //SW SE NW Corners NEED TO MAKE THIS

            Texture2D texture_45 = cManager.Load<Texture2D>("Background/Water Overlays/1"); //SW SE NE Corners NEED TO MAKE THIS

            Texture2D texture_46 = cManager.Load<Texture2D>("Background/Water Overlays/34"); //All corners

            Texture2D[] lookups = 
            {  null,       texture_1,  texture_2,  texture_3,  texture_4,  texture_5, texture_6,  texture_7, texture_8,  texture_9,  texture_10, texture_11, texture_12, texture_13, texture_14, texture_15,
               texture_16, texture_1,  texture_17, texture_3,  texture_18, texture_5, texture_19, texture_7, texture_8,  texture_9,  texture_10, texture_11, texture_12, texture_13, texture_14, texture_15, // nw
               texture_20, texture_1,  texture_2,  texture_3,  texture_21, texture_5, texture_6,  texture_7, texture_22, texture_9,  texture_10, texture_11, texture_23, texture_13, texture_14, texture_15, // ne
               texture_24, texture_1,  texture_17, texture_3,  texture_25, texture_5, texture_19, texture_7, texture_22, texture_9,  texture_10, texture_11, texture_23, texture_13, texture_14, texture_15, // ne + nw
               texture_26, texture_27, texture_2,  texture_3,  texture_4,  texture_5, texture_6,  texture_7, texture_28, texture_29, texture_10, texture_11, texture_12, texture_13, texture_14, texture_15, // se
               texture_30, texture_27, texture_17, texture_3,  texture_18, texture_5, texture_19, texture_7, texture_28, texture_29, texture_10, texture_11, texture_12, texture_13, texture_14, texture_15, // se + nw
               texture_31, texture_27, texture_2,  texture_3,  texture_21, texture_5, texture_6,  texture_7, texture_32, texture_29, texture_10, texture_11, texture_23, texture_13, texture_14, texture_15, // se + ne
               texture_33, texture_27, texture_17, texture_3,  texture_25, texture_5, texture_19, texture_7, texture_32, texture_29, texture_10, texture_11, texture_23, texture_13, texture_14, texture_15, // se + ne + nw
               texture_34, texture_35, texture_36, texture_37, texture_4,  texture_5, texture_6,  texture_7, texture_8,  texture_9,  texture_10, texture_11, texture_12, texture_13, texture_14, texture_15, // sw
               texture_38, texture_35, texture_39, texture_37, texture_18, texture_5, texture_19, texture_7, texture_8,  texture_9,  texture_10, texture_11, texture_12, texture_13, texture_14, texture_15, // sw + nw
               texture_40, texture_35, texture_36, texture_37, texture_21, texture_5, texture_6,  texture_7, texture_22, texture_9,  texture_10, texture_11, texture_23, texture_13, texture_14, texture_15, // sw + ne
               texture_41, texture_35, texture_39, texture_37, texture_25, texture_5, texture_19, texture_7, texture_22, texture_9,  texture_10, texture_11, texture_23, texture_13, texture_14, texture_15, // sw + ne + nw
               texture_42, texture_43, texture_36, texture_37, texture_4,  texture_5, texture_6,  texture_7, texture_28, texture_29, texture_10, texture_11, texture_12, texture_13, texture_14, texture_15, // sw + se
               texture_44, texture_43, texture_39, texture_37, texture_18, texture_5, texture_19, texture_7, texture_28, texture_29, texture_10, texture_11, texture_12, texture_13, texture_14, texture_15, // sw + se + nw
               texture_45, texture_43, texture_39, texture_37, texture_21, texture_5, texture_6,  texture_7, texture_32, texture_29, texture_10, texture_11, texture_23, texture_13, texture_14, texture_15, // sw + se + ne
               texture_46, texture_43, texture_39, texture_37, texture_25, texture_5, texture_19, texture_7, texture_32, texture_29, texture_10, texture_11, texture_23, texture_13, texture_14, texture_15, // all corners
            };

            return lookups;
        }

        public bool room_type_match(string t1, Room.Room_Type t2)
        {
            if (String.Compare(t1, "Gorehound Kennels") == 0 && t2 == Room.Room_Type.GHound_Kennel)
                return true;
            else if(String.Compare(t1, "Library") == 0 && t2 == Room.Room_Type.Library)
                return true;
            else if(String.Compare(t1, "Darkroom") == 0 && t2 == Room.Room_Type.DarkRoom)
                return true;
            else if(String.Compare(t1, "Corpse Storage") == 0 && t2 == Room.Room_Type.CorpseStorage)
                return true;
            else if(String.Compare(t1, "Sewer") == 0 && t2 == Room.Room_Type.SewerRoom)
                return true;
            else if(String.Compare(t1, "Rubble Room") == 0 && t2 == Room.Room_Type.Destroyed)
                return true;
            else if(String.Compare(t1, "Knight Armory") == 0 && t2 == Room.Room_Type.KnightArmory)
                return true;
            else if(String.Compare(t1, "Sewer Shaft") == 0 && t2 == Room.Room_Type.SewerShaft)
                return true;
            else if(String.Compare(t1, "Jail") == 0 && t2 == Room.Room_Type.Jail)
                return true;
            else if(String.Compare(t1, "Mine Shaft") == 0 && t2 == Room.Room_Type.MineShaft)
                return true;

            return false;
        }

        private void add_random_monsters(int number_of_monsters,
                                         Cronkpit.CronkPit.Dungeon cDungeon,
                                         ref List<Monster> monsters)
        {
            
            for (int i = 0; i < number_of_monsters; i++)
            {
                int monsterType = randGen.Next(100);
                string monster_to_add = floorSpawns.find_monster_by_number(monsterType);
                add_a_monster(monster_to_add, ref badGuys);
            }

            //Boss monster section
            if (cDungeon == CronkPit.Dungeon.Necropolis && fl_number == 12)
                badGuys.Add(new Boneyard(random_valid_position(Monster.Monster_Size.Large,
                                                               random_coord_restrictions.Monster), cManager, badGuys.Count, true));
        }

        public void add_specific_monsters(List<KeyValuePair<string, gridCoordinate>> monsterList)
        {
            for (int i = 0; i < monsterList.Count; i++)
                add_a_monster(monsterList[i].Key, ref badGuys, monsterList[i].Value.x, monsterList[i].Value.y);
        }

        private void add_a_monster(string monsterName, ref List<Monster> monsters, int x = -1, int y = -1)
        {
            int next_index = monsters.Count;
            gridCoordinate norm_nonhk_spawn = new gridCoordinate(x, y);
            if(x == -1 && y == -1)
                norm_nonhk_spawn = random_valid_position(restrictions: random_coord_restrictions.Monster);
            switch (monsterName)
            {
                case "HollowKnight":
                    if (randGen.Next(100) <= floor_Sub_Spawns.return_spawn_chance_by_monster("RedKnight"))
                        badGuys.Add(new RedKnight(valid_hollowKnight_spawn(), cManager, next_index));
                    else
                        badGuys.Add(new HollowKnight(valid_hollowKnight_spawn(), cManager, next_index));
                    break;
                case "RedKnight":
                    badGuys.Add(new RedKnight(valid_hollowKnight_spawn(), cManager, next_index));
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
                    badGuys.Add(new Skeleton(norm_nonhk_spawn, cManager, next_index, reg_skel_wpn));
                    break;
                case "GoldMimic":
                    badGuys.Add(new GoldMimic(norm_nonhk_spawn, cManager, next_index));
                    break;
                case "GoreHound":
                    badGuys.Add(new GoreHound(norm_nonhk_spawn, cManager, next_index));
                    break;
                case "Zombie":
                    if (randGen.Next(100) <= floor_Sub_Spawns.return_spawn_chance_by_monster("ZombieFanatic"))
                        badGuys.Add(new ZombieFanatic(norm_nonhk_spawn, cManager, next_index));
                    else
                        badGuys.Add(new Zombie(norm_nonhk_spawn, cManager, next_index));
                    break;
                case "ZombieFanatic":
                    badGuys.Add(new ZombieFanatic(norm_nonhk_spawn, cManager, next_index));
                    break;
                case "Grendel":
                    int grenwpn = randGen.Next(3);
                    switch (grenwpn)
                    {
                        case 0:
                            badGuys.Add(new Grendel(norm_nonhk_spawn, cManager, next_index, Grendel.Grendel_Weapon_Type.Club));
                            break;
                        case 1:
                            badGuys.Add(new Grendel(norm_nonhk_spawn, cManager, next_index, Grendel.Grendel_Weapon_Type.Frostbolt));
                            break;
                    }
                    break;
                case "Necromancer":
                    badGuys.Add(new Necromancer(norm_nonhk_spawn, cManager, next_index));
                    break;
                case "GoreWolf":
                    badGuys.Add(new Gorewolf(norm_nonhk_spawn, cManager, next_index));
                    break;
                case "Ghost":
                    badGuys.Add(new Ghost(norm_nonhk_spawn, cManager, next_index));
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
                    badGuys.Add(new Armored_Skeleton(norm_nonhk_spawn, cManager, next_index, arm_skel_wpn));
                    break;
                case "VoidWraith":
                    badGuys.Add(new Voidwraith(norm_nonhk_spawn, cManager, next_index));
                    break;
            }
        }

        private void clamp_river_coordinates(out int min_x, out int max_x, out int min_y, out int max_y)
        {
            min_x = 0;
            max_x = 0;
            min_y = 0;
            max_y = 0;

            for (int i = 0; i < roomLayout.Count; i++)
            {
                if (roomLayout[i].startXPos > max_x)
                    max_x = roomLayout[i].startXPos;
                if (roomLayout[i].startYPos > max_y)
                    max_y = roomLayout[i].startYPos;
            }

            min_x = max_x;
            min_y = max_y;

            for (int i = 0; i < roomLayout.Count; i++)
            {
                if (roomLayout[i].startXPos < min_x)
                    min_x = roomLayout[i].startXPos;
                if (roomLayout[i].startYPos < min_y)
                    min_y = roomLayout[i].startYPos;
            }

            min_x = Math.Max(0, min_x - 5);
            max_x = Math.Min(50, max_x + 5);
            min_y = Math.Max(0, min_y - 5);
            max_y = Math.Min(50, max_y + 5);
        }

        public void draw_hallway_tiles(Tile target_tile)
        {
            if (!target_tile.isPassable())
                target_tile.set_tile_type(Tile.Tile_Type.StoneFloor, master_textureList);
        }

        public void replace_surrounding_void(Tile target_tile)
        {
            gridCoordinate target_grid_c = target_tile.get_grid_c();
            Tile.Tile_Type tileType = target_tile.get_my_tile_type();

            for (int x = Math.Max(0, target_grid_c.x - 1); x < Math.Min(target_grid_c.x + 2, stdfloorSize); x++)
                for (int y = Math.Max(0, target_grid_c.y - 1); y < Math.Min(target_grid_c.y + 2, stdfloorSize); y++)
                    if (floorTiles[x][y].isVoid())
                    {
                        switch (target_tile.get_my_tile_type())
                        {
                            //Dirt
                            case Tile.Tile_Type.DirtFloor:
                                floorTiles[x][y].set_tile_type(Tile.Tile_Type.DirtWall, master_textureList);
                                break;
                            case Tile.Tile_Type.Rubble_Floor:
                            case Tile.Tile_Type.Gravel:
                            case Tile.Tile_Type.Deep_Water:
                            case Tile.Tile_Type.Shallow_Water:
                                floorTiles[x][y].set_tile_type(Tile.Tile_Type.Rubble_Wall, master_textureList);
                                break;
                            //Stone
                            default:
                                floorTiles[x][y].set_tile_type(Tile.Tile_Type.StoneWall, master_textureList);
                                break;
                        }
                    }
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
        public gridCoordinate random_valid_position(Monster.Monster_Size entitySize = Monster.Monster_Size.Normal,
                                                    random_coord_restrictions restrictions = random_coord_restrictions.None)
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
                //make sure it's not the exit or entrance
                for (int i = 0; i < g_coords.Count; i++)
                    if (floorTiles[g_coords[i].x][g_coords[i].y].get_my_tile_type() == Tile.Tile_Type.Exit ||
                        floorTiles[g_coords[i].x][g_coords[i].y].get_my_tile_type() == Tile.Tile_Type.Entrance)
                        good_position = false;
                if (restrictions == random_coord_restrictions.Entrance)
                {
                    for (int i = 0; i < g_coords.Count; i++)
                        if (g_coords[i].x >= dungeon_exit_coord.x - 10 && g_coords[i].x <= dungeon_exit_coord.x + 10 &&
                            g_coords[i].y >= dungeon_exit_coord.y - 10 && g_coords[i].y <= dungeon_exit_coord.y + 10)
                            good_position = false;
                }
                else if (restrictions == random_coord_restrictions.Monster)
                {
                    for (int i = 0; i < g_coords.Count; i++)
                        if (g_coords[i].x >= dungeon_entrance_coord.x - 6 && g_coords[i].x <= dungeon_entrance_coord.x + 6 &&
                            g_coords[i].y >= dungeon_entrance_coord.y - 6 && g_coords[i].y <= dungeon_entrance_coord.y + 6)
                            good_position = false;
                }
                        

                if (good_position)
                    valid_position = true;
                else
                    g_coords.Clear();
            }

            return g_coords[0];
        }

        public gridCoordinate get_entrance_coord()
        {
            return dungeon_entrance_coord;
        }
        //Green text!
        public gridCoordinate valid_hollowKnight_spawn()
        {
            bool goodPosition = false;
            gridCoordinate returnCoord = random_valid_position(restrictions: random_coord_restrictions.Monster);
            while (!goodPosition)
            {
                gridCoordinate gc = random_valid_position();
                List<gridCoordinate> coords = new List<gridCoordinate>();
                coords.Add(new gridCoordinate(gc, gridCoordinate.direction.Left));  //[0] x - 1
                coords.Add(new gridCoordinate(gc, gridCoordinate.direction.Right)); //[1] x + 1
                coords.Add(new gridCoordinate(gc, gridCoordinate.direction.Up));    //[2] y - 1
                coords.Add(new gridCoordinate(gc, gridCoordinate.direction.Down));  //[3] y + 1
                for (int i = 0; i < coords.Count; i++)
                {
                    gridCoordinate current_coord = coords[i];
                    current_coord.x = Math.Max(0, Math.Min(current_coord.x, stdfloorSize - 1));
                    current_coord.y = Math.Max(0, Math.Min(current_coord.y, stdfloorSize - 1));
                }
                if ((is_tile_passable(coords[0]) && !is_tile_passable(coords[1]) && //check 0 vs 1
                     floorTiles[coords[1].x][coords[1].y].get_my_tile_type() != Tile.Tile_Type.Deep_Water) ||
                    (is_tile_passable(coords[1]) && !is_tile_passable(coords[0]) && //check 1 vs 0
                     floorTiles[coords[0].x][coords[0].y].get_my_tile_type() != Tile.Tile_Type.Deep_Water) ||
                    (is_tile_passable(coords[3]) && !is_tile_passable(coords[2]) && //check 3 vs 2
                     floorTiles[coords[2].x][coords[2].y].get_my_tile_type() != Tile.Tile_Type.Deep_Water) ||
                    (is_tile_passable(coords[2]) && !is_tile_passable(coords[3]) && //check 2 vs 3
                     floorTiles[coords[3].x][coords[3].y].get_my_tile_type() != Tile.Tile_Type.Deep_Water))
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
                case Scroll.Atk_Area_Type.enemyDebuff:
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
                        case Doodad.Doodad_Type.Altar:
                            if (!Doodads[i].is_passable())
                            {
                                Doodads[i].destroy_altar(pl, this);
                                ambient_manavalue += 100;
                                add_new_popup("Descrated!", Popup.popup_msg_color.Red, Doodads[i].get_g_coord());
                                add_new_popup("+Mana!", Popup.popup_msg_color.Blue, Doodads[i].get_g_coord());
                            }
                            break;
                    }
            }
        }

        List<Tile> all_passable_neighbors(gridCoordinate targetCoord)
        {
            List<Tile> return_list = new List<Tile>();
            for (int x = targetCoord.x - 1; x <= targetCoord.x + 1; x++)
                for (int y = targetCoord.y - 1; y <= targetCoord.y + 1; y++)
                    if (x > 0 && x < stdfloorSize && y > 0 && y < stdfloorSize &&
                        !(x == targetCoord.x && y == targetCoord.y) &&
                        floorTiles[x][y].isPassable())
                        return_list.Add(floorTiles[x][y]);

            return return_list;
        }

        public bool acceptable_destruction_coordinate(gridCoordinate t_coord)
        {
            return t_coord.x > 0 && t_coord.x < stdfloorSize - 1 && t_coord.y > 0 && t_coord.y < stdfloorSize - 1;
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
                        case Scroll.Atk_Area_Type.enemyDebuff:
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
                                target_tile.set_tile_type(Tile.Tile_Type.Rubble_Floor, master_textureList);
                                replace_surrounding_void(target_tile);
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
                                damage_monster_single_atk(new Attack(Pew_Pews[i].get_dmg_type(), dmg_val * 2),
                                                          Pew_Pews[i].get_attached_statuses(),
                                                          mon_on_mon_ID, false, aoe_effect);

                            int mon_on_Doodad_ID = -1;
                            if (is_destroyable_Doodad_here(attacked_coordinates[j], out mon_on_Doodad_ID))
                                damage_Doodad(dmg_val * 2, mon_on_Doodad_ID);
                        }
                        else
                        {
                            if (is_monster_here(attacked_coordinates[j], out monsterID))
                            {
                                Monster mon = badguy_by_monster_id(monsterID);
                                process_player_projectile_attack(pl, Pew_Pews[i], mon, aoe_effect);
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

        public void process_player_projectile_attack(Player pl, Projectile pew, Monster m, bool aoe_effect)
        {
            List<Attack> damage_to_monster = new List<Attack>();
            List<StatusEffect> debuffs_to_monster = new List<StatusEffect>();

            pl.handle_attack_damage(pew.get_attached_weapon(), pew.get_attached_scroll(),
                                    1.0, false, ref damage_to_monster, ref debuffs_to_monster);
            damage_monster(damage_to_monster, debuffs_to_monster, m.my_Index, false, aoe_effect);
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
                    damage_monster_single_atk(new Attack(cone_atk_dmg_type, dmg), null, monsterID, false, true);
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
                                damage_monster_single_atk(new Attack(effect_dmg_type, dmg_val), null, monsterID, false, true);
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

        public List<Tile> find_smellable_tiles(gridCoordinate origin, int smellRange)
        {
            Smellqueue open = new Smellqueue();
            Smellqueue closed = new Smellqueue();

            open.add_to_end(floorTiles[origin.x][origin.y], 0);

            while (!open.is_empty())
            {
                KeyValuePair<Tile, int> next_pair = open.pop_first();
                Tile T = next_pair.Key;
                int cost = next_pair.Value;

                closed.add_to_end(T, cost);

                int next_cost = cost + 1;
                if (cost < smellRange)
                {
                    List<Tile> neighbors = all_passable_neighbors(T.get_grid_c());
                    for (int i = 0; i < neighbors.Count; i++)
                    {
                        int open_index = open.in_list(neighbors[i]);
                        int closed_index = closed.in_list(neighbors[i]);
                        if (open_index > -1)
                        {
                            if (open_index > -1)
                                open.update_if_smaller_cost(open_index, neighbors[i], next_cost);
                        }
                        else if (open_index == -1 && closed_index == -1)
                            open.add_to_end(neighbors[i], next_cost);
                    }
                }
            }

            return closed.return_all_tiles();
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

        public bool establish_los(gridCoordinate origin, gridCoordinate destination)
        {
            Tile originTile = floorTiles[origin.x][origin.y];
            Tile destinationTile = floorTiles[destination.x][destination.y];
            
            Vision_Rc.Add(new VisionRay(originTile.get_corner(1), destinationTile.get_corner(1)));
            Vision_Rc.Add(new VisionRay(originTile.get_corner(2), destinationTile.get_corner(2)));
            Vision_Rc.Add(new VisionRay(originTile.get_corner(3), destinationTile.get_corner(3)));
            Vision_Rc.Add(new VisionRay(originTile.get_corner(4), destinationTile.get_corner(4)));
            Vision_Rc.Add(new VisionRay(originTile.get_corner(1), destinationTile.get_corner(3)));
            Vision_Rc.Add(new VisionRay(originTile.get_corner(2), destinationTile.get_corner(4)));

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
                for (int y = 0; y < stdfloorSize; y++)
                    floorTiles[x][y].drawMe(ref sBatch);
        }

        public void draw_tile_overlays(ref SpriteBatch sBatch)
        {
            for (int x = 0; x < stdfloorSize; x++)
                for (int y = 0; y < stdfloorSize; y++)
                    floorTiles[x][y].draw_my_overlay(ref sBatch);
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

        public void damage_monster(List<Attack> atks, List<StatusEffect> debuffs,
                                   int monsterID, bool melee_attack, bool aoe_attack)
        {
            for (int i = 0; i < badGuys.Count; i++)
            {
                if (badGuys[i].my_Index == monsterID)
                {
                    badGuys[i].takeDamage(atks, debuffs, melee_attack, aoe_attack, message_buffer, this);
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

        public void damage_monster_single_atk(Attack atk, List<StatusEffect> debuffs,
                                              int monsterID, bool melee_attack, bool aoe_attack)
        {
            List<Attack> atks = new List<Attack>();
            atks.Add(atk);
            damage_monster(atks, debuffs, monsterID, melee_attack, aoe_attack);
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

        public void open_door_here(gridCoordinate door_coord, out bool opened_door)
        {
            for (int i = 0; i < Doodads.Count; i++)
            {
                if (Doodads[i].get_g_coord().x == door_coord.x &&
                    Doodads[i].get_g_coord().y == door_coord.y &&
                    Doodads[i].get_my_doodad_type() == Doodad.Doodad_Type.Door)
                {
                    Doodads[i].open_door(this);
                    opened_door = !Doodads[i].is_door_closed();
                }
            }

            opened_door = false;
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