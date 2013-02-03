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

        public List<List<bool>> retrieve_circular_matrix()
        {
            return circular_room_matrix;
        }
    }
}
