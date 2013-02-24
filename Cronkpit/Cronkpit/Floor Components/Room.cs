using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cronkpit
{
    class Room
    {
        public int roomHeight;
        public int roomWidth;
        public int startXPos;
        public int startYPos;
        bool dirt_room;
        bool circular_room;
        bool has_doors;

        //Necropolis relevant variables
        bool corpses_in_corner;
        int corners_that_have_corpses;

        //Situational variables
        List<List<bool>> circular_room_matrix;

        public Room(int srH, int srW, int srX, int srY, bool dRoom)
        {
            roomHeight = srH;
            roomWidth = srW;
            startXPos = srX;
            startYPos = srY;
            dirt_room = dRoom;
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
