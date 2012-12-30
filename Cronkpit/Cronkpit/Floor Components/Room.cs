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

        public bool is_dirt_room()
        {
            return dirt_room;
        }
    }
}
