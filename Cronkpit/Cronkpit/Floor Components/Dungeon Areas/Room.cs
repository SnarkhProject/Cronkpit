using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using CKPLibrary;

namespace Cronkpit
{
    class Room
    {
        public enum Room_Type
        {
            Generic, GenericCircular, GHound_Kennel, Library, TrapRoom, 
            DarkRoom, CorpseStorage, SewerRoom, SewerShaft, KnightArmory, 
            Jail, Destroyed, MineShaft
        };

        public int roomHeight;
        public int roomWidth;
        public int startXPos;
        public int startYPos;
        Room_Type c_room_type;
        Tile.Tile_Type c_floor_type;
        bool has_doors;
        Random rGen;

        List<string> room_tiles;
        List<KeyValuePair<Doodad.Doodad_Type, gridCoordinate>> doodad_list;
        List<KeyValuePair<string, gridCoordinate>> monster_list;
        List<KeyValuePair<Goldpile, gridCoordinate>> gold_list;

        //Necropolis relevant variables
        bool corpses_in_corner;
        int corners_that_have_corpses;

        //Situational variables
        List<List<bool>> circular_room_matrix;

        public Room(int srH, int srW, int srX, int srY, 
                    Room_Type rType, Tile.Tile_Type fType, bool doors)
        {
            roomHeight = srH;
            roomWidth = srW;
            startXPos = srX;
            startYPos = srY;
            has_doors = doors;
            c_room_type = rType;
            if (c_room_type == Room_Type.GenericCircular)
                calculate_circular_matrix(roomWidth);
            c_floor_type = fType;
            rGen = new Random();

            room_tiles = new List<string>();
            doodad_list = new List<KeyValuePair<Doodad.Doodad_Type, gridCoordinate>>();
            monster_list = new List<KeyValuePair<string, gridCoordinate>>();
            gold_list = new List<KeyValuePair<Goldpile, gridCoordinate>>();
        }

        public Room(RoomDC base_room, SpawnTable sTable)
        {
            roomHeight = base_room.RoomHeight;
            roomWidth = base_room.RoomWidth;
            startXPos = 0;
            startYPos = 0;
            has_doors = false;
            rGen = new Random();
            
            //Necropolis only.
            corpses_in_corner = false;
            corners_that_have_corpses = 0;

            //More general purpose stuff
            c_room_type = Room_Type.Generic;
            c_floor_type = Tile.Tile_Type.StoneFloor;

            room_tiles = base_room.Room_Matrix;
            doodad_list = new List<KeyValuePair<Doodad.Doodad_Type, gridCoordinate>>();
            monster_list = new List<KeyValuePair<string, gridCoordinate>>();
            gold_list = new List<KeyValuePair<Goldpile, gridCoordinate>>();

            switch (base_room.RoomType)
            {
                case "Gorehound Kennels":
                    c_room_type = Room.Room_Type.GHound_Kennel;
                    break;
                case "Library":
                    c_room_type = Room.Room_Type.Library;
                    break;
                case "Darkroom":
                    c_room_type = Room.Room_Type.DarkRoom;
                    break;
                case "Corpse Storage":
                    c_room_type = Room.Room_Type.CorpseStorage;
                    break;
                case "Sewer":
                    c_room_type = Room.Room_Type.SewerRoom;
                    break;
                case "Rubble Room":
                    c_room_type = Room.Room_Type.Destroyed;
                    break;
                case "Knight Armory":
                    c_room_type = Room.Room_Type.KnightArmory;
                    break;
                case "Sewer Shaft":
                    c_room_type = Room.Room_Type.SewerShaft;
                    break;
                case "Jail":
                    c_room_type = Room.Room_Type.Jail;
                    break;
                case "Mine Shaft":
                    c_room_type = Room.Room_Type.MineShaft;
                    break;
            }

            //Now to add the appropriate list of monsters & doodads
            //Doodads first.
            for (int i = 0; i < base_room.Room_Doodads.Count; i++)
            {
                Doodad.Doodad_Type c_doodad_type = 0; //Altar by default - not a problem.
                switch (base_room.Room_Doodads[i])
                {
                    case "Altar":
                        c_doodad_type = Doodad.Doodad_Type.Altar;
                        break;
                    case "ArmorSuit":
                        c_doodad_type = Doodad.Doodad_Type.ArmorSuit;
                        break;
                    case "BloodSplatter":
                        c_doodad_type = Doodad.Doodad_Type.Blood_Splatter;
                        break;
                    case "Cage":
                        c_doodad_type = Doodad.Doodad_Type.Cage;
                        break;
                    case "CorpsePile":
                        c_doodad_type = Doodad.Doodad_Type.CorpsePile;
                        break;
                    case "DestroyedArmorSuit":
                        c_doodad_type = Doodad.Doodad_Type.Destroyed_ArmorSuit;
                        break;
                    case "Bookshelf":
                        c_doodad_type = Doodad.Doodad_Type.Bookshelf;
                        break;
                    case "DestroyedBookshelf":
                        c_doodad_type = Doodad.Doodad_Type.Destroyed_Bookshelf;
                        break;
                }
                gridCoordinate c_grid_coord = grid_c_from_matrix_c(base_room.Room_Doodad_Coordinates[i]);
                c_grid_coord.x += startXPos;
                c_grid_coord.y += startYPos;
                int doodad_chance = 0;
                Int32.TryParse(base_room.Room_Doodad_Chances[i], out doodad_chance);
                if(rGen.Next(100) < doodad_chance)
                    doodad_list.Add(new KeyValuePair<Doodad.Doodad_Type,gridCoordinate>(c_doodad_type, c_grid_coord));
            }

            //Then monsters.
            for (int i = 0; i < base_room.Room_Monsters.Count; i++)
            {
                if (sTable.monster_in_table(base_room.Room_Monsters[i]))
                {
                    int monster_chance = 0;
                    Int32.TryParse(base_room.Room_Monster_Chances[i], out monster_chance);
                    gridCoordinate c_grid_coord = grid_c_from_matrix_c(base_room.Room_Monster_Coordinates[i]);
                    if (rGen.Next(100) < monster_chance)
                        monster_list.Add(new KeyValuePair<string, gridCoordinate>(base_room.Room_Monsters[i], c_grid_coord));
                }
            }
        }

        public int findCenter(string whichCenter)
        {
            if (String.Compare("x", whichCenter) == 0)
                return startXPos + (roomWidth / 2);
            else
                return startYPos + (roomHeight / 2);
        }

        public void set_doors(bool doors)
        {
            has_doors = doors;
        }

        public void set_to_room_type(Room_Type next_type)
        {
            c_room_type = next_type;
            if (c_room_type == Room_Type.GenericCircular)
                calculate_circular_matrix(roomWidth);
        }

        public void reset_starting_position(int x, int y)
        {
            startXPos = x;
            startYPos = y;
        }

        public Room_Type get_room_type()
        {
            return c_room_type;
        }

        public Tile.Tile_Type get_floor_type()
        {
            return c_floor_type;
        }

        public bool room_has_doors()
        {
            return has_doors;
        }

        public void render_to_board(List<List<Tile>> board, List<KeyValuePair<Tile.Tile_Type, Texture2D>> textures)
        {
            switch (c_room_type)
            {
                case Room_Type.Generic:
                case Room_Type.GenericCircular:
                    for (int x = startXPos; x < startXPos + roomWidth; x++)
                        for (int y = startYPos; y < startYPos + roomHeight; y++)
                            if (c_room_type == Room_Type.GenericCircular &&
                                circular_room_matrix[x - startXPos][y - startYPos])
                                board[x][y].set_tile_type(c_floor_type, textures);
                            else
                                board[x][y].set_tile_type(c_floor_type, textures);
                    break;      
                default:
                    for (int y = startYPos; y < startYPos + roomHeight; y++)
                    {
                        string[] row = room_tiles[y - startYPos].Split(' ');
                        for (int x = startXPos; x < startXPos + roomWidth; x++)
                            if(String.Compare(row[x - startXPos], "V") != 0)
                                board[x][y].set_tile_type_str(row[x - startXPos], textures);
                    }
                    break;
            }
        }

        public void render_doodads_to_board(ref List<Doodad> fl_doodads, ContentManager cmgr)
        {
            for (int i = 0; i < doodad_list.Count; i++)
            {
                gridCoordinate doodad_coord = doodad_list[i].Value;
                gridCoordinate new_doodad_coord = new gridCoordinate(doodad_coord.x + startXPos,
                                                                     doodad_coord.y + startYPos);
                fl_doodads.Add(new Doodad(doodad_list[i].Key, cmgr, new_doodad_coord, fl_doodads.Count));
            }
        }

        public void render_monsters_to_board(ref List<Monster> fl_monsters, Floor fl)
        {
            for (int i = 0; i < monster_list.Count; i++)
            {
                string monster_name = monster_list[i].Key;
                gridCoordinate monster_coord = monster_list[i].Value;
                gridCoordinate new_monster_coord = new gridCoordinate(monster_coord.x + startXPos,
                                                                      monster_coord.y + startYPos);
                monster_list[i] = new KeyValuePair<string, gridCoordinate>(monster_name, new_monster_coord);
            }
            fl.add_specific_monsters(monster_list);
        }

        public void calculate_circular_matrix(int diameter)
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

            for (int rx = 0; rx < diameter; rx++)
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
                circle_matrix[diameter - (tiles_off_corners + 1)][diameter - 1] = false;

                //The two rightmost ones
                circle_matrix[diameter - 1][tiles_off_corners] = false;
                circle_matrix[diameter - 1][diameter - (tiles_off_corners + 1)] = false;
            }

            circular_room_matrix = circle_matrix;
        }

        private gridCoordinate grid_c_from_matrix_c(string matrix_c)
        {
            string[] x_y = matrix_c.Split(' ');
            string[] x = x_y[0].Split(':');
            string[] y = x_y[1].Split(':');
            int x_coord = 0;
            Int32.TryParse(x[1], out x_coord);
            int y_coord = 0;
            Int32.TryParse(y[1], out y_coord);

            return new gridCoordinate(x_coord, y_coord);
        }

        #region Necropolis-related functions

        public void set_corpses(int base_corpse_chance)
        {
            int corpse_roll = rGen.Next(100);
            int modified_corpse_chance = base_corpse_chance;
            if (c_floor_type == Tile.Tile_Type.DirtFloor)
                modified_corpse_chance += 20;

            if (modified_corpse_chance < corpse_roll)
            {
                corpses_in_corner = true;
                corners_that_have_corpses = rGen.Next(1, 5);
            }           
        }

        public bool has_corpses()
        {
            return corpses_in_corner;
        }

        public int how_many_corners_have_corpses()
        {
            return corners_that_have_corpses;
        }

        #endregion
    }
}
