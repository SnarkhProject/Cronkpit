using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cronkpit_Csharp
{
    class Hall
    {
        public int startRoomID;
        public int endRoomID;
        public int startX;
        public int startY;
        public int endX;
        public int endY;
        public int drawnDirection;

        public Hall(int sRoomID, int eRoomID, int stX, int stY, int enX, int enY)
        {
            startRoomID = sRoomID;
            endRoomID = eRoomID;
            startX = stX;
            startY = stY;
            endX = enX;
            endY = enY;
        }
    }
}
