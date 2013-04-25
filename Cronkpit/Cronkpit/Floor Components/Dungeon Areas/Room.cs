using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        bool dirt_room;
        bool circular_room;
        bool has_doors;

        List<string> room_tiles;
        List<KeyValuePair<Doodad.Doodad_Type, gridCoordinate>> doodad_list;
        List<KeyValuePair<Monster, gridCoordinate>> monster_list;
        List<KeyValuePair<Goldpile, gridCoordinate>> gold_list;

        //Necropolis relevant variables
        bool corpses_in_corner;
        int corners_that_have_corpses;

        //Situational variables
        List<List<bool>> circular_room_matrix;

        public Room(int srH, int srW, int srX, int srY, 
                    bool dirtRoom, bool doorRoom, bool circleRoom)
        {
            roomHeight = srH;
            roomWidth = srW;
            startXPos = srX;
            startYPos = srY;
            dirt_room = dirtRoom;
            has_doors = doorRoom;
            circular_room = circleRoom;

            room_tiles = new List<string>();
            doodad_list = new List<KeyValuePair<Doodad.Doodad_Type, gridCoordinate>>();
            monster_list = new List<KeyValuePair<Monster, gridCoordinate>>();
            gold_list = new List<KeyValuePair<Goldpile, gridCoordinate>>();
        }

        public int findCenter(string whichCenter)
        {
            if (String.Compare("x", whichCenter) == 0)
                return startXPos + (roomWidth / 2);
            else
                return startYPos + (roomHeight / 2);
        }

        public void set_circular_room_matrix(List<List<bool>> matrix)
        {
            circular_room_matrix = matrix;
        }

        public void set_doors(bool doors)
        {
            has_doors = doors;
        }

        public void set_to_circular_room()
        {
            circular_room = true;
        }

        public bool is_dirt_room()
        {
            return dirt_room;
        }

        public bool is_circular_room()
        {
            return circular_room;
        }

        public bool room_has_doors()
        {
            return has_doors;
        }

        public List<List<bool>> retrieve_circular_matrix()
        {
            return circular_room_matrix;
        }

        #region Necropolis-related functions

        public void set_corpses(bool corpses)
        {
            corpses_in_corner = corpses;
        }

        public void set_corners_with_corpses(int corners)
        {
            corners_that_have_corpses = corners;
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
