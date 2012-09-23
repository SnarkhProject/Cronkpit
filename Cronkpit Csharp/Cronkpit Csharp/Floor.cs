using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Cronkpit_Csharp
{
    class Floor
    {
        //Constants
        private int stdfloorSize = 50;
        //Floor components
        List<List<Tile>> floorTiles;
        List<Room> roomLayout;
        List<Hall> hallLayout;
        List<Monster> badGuys;
        List<Goldpile> Money;
        //Other shit needed for the thing to function
        ContentManager cManager;
        Random randGen;

        //Green text. Function here.
        public Floor(ContentManager sCont)
        {
            //Init floor components.
            floorTiles = new List<List<Tile>>();
            roomLayout = new List<Room>();
            hallLayout = new List<Hall>();
            badGuys = new List<Monster>();
            Money = new List<Goldpile>();

            //Next init other shit
            cManager = sCont;
            randGen = new Random();
        
            //Then do stuff for real
            buildFloor();
        }

        //Super important stuff
        //Green text. Function here.
        public void buildFloor()
        {
            //Make the base tiles.
            for (int x = 0; x < stdfloorSize; x++)
            {
                floorTiles.Add(new List<Tile>());
                for (int y = 0; y < stdfloorSize; y++)
                {
                    Vector2 tilePos = new Vector2(x * 32, y * 32);
                    floorTiles[x].Add(new Tile(0, randGen.Next(100), cManager, tilePos, new gridCoordinate(x, y)));
                }
            }

            //Randomly generate rooms and hallways.
            //First rooms.
            int number_of_rooms = 4 + randGen.Next(3);
            for (int i = 0; i < number_of_rooms; i++)
            {
                int next_room_height = randGen.Next(3, 7);
                int next_room_width = randGen.Next(3, 7);
                int next_room_startX = randGen.Next(1, ((stdfloorSize - 1) - next_room_width));
                int next_room_startY = randGen.Next(1, ((stdfloorSize - 1) - next_room_height));
                roomLayout.Add(new Room(next_room_height,
                                    next_room_width,
                                    next_room_startX,
                                    next_room_startY));
            }
            //Next hallways.
            int number_of_hallways = number_of_rooms - 1;
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
            
            //Alter void tiles to floor tiles.
            //First rooms
            for (int i = 0; i < roomLayout.Count; i++)
            {
                for (int x = roomLayout[i].startXPos; x < roomLayout[i].startXPos + roomLayout[i].roomWidth; x++)
                    for (int y = roomLayout[i].startYPos; y < roomLayout[i].startYPos + roomLayout[i].roomHeight; y++)
                        floorTiles[x][y].setTexture(1);
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
                            floorTiles[hallLayout[i].startX][y].setTexture(1);
                    else
                        for (int y = hallLayout[i].startY; y < hallLayout[i].endY; y++)
                            floorTiles[hallLayout[i].startX][y].setTexture(1);

                    //then draw X from endY
                    if (hallLayout[i].startX > hallLayout[i].endX)
                        for (int x = hallLayout[i].startX; x > hallLayout[i].endX; x--)
                            floorTiles[x][hallLayout[i].endY].setTexture(1);
                    else
                        for (int x = hallLayout[i].startX; x < hallLayout[i].endX; x++)
                            floorTiles[x][hallLayout[i].endY].setTexture(1);
                }
                //1 = draw x to y
                else
                {
                    //draw X first from startY
                    if (hallLayout[i].startX > hallLayout[i].endX)
                        for (int x = hallLayout[i].startX; x > hallLayout[i].endX; x--)
                            floorTiles[x][hallLayout[i].startY].setTexture(1);
                    else
                        for (int x = hallLayout[i].startX; x < hallLayout[i].endX; x++)
                            floorTiles[x][hallLayout[i].startY].setTexture(1);

                    //draw Y second from endX
                    if (hallLayout[i].startY > hallLayout[i].endY)
                        for (int y = hallLayout[i].startY; y > hallLayout[i].endY; y--)
                            floorTiles[hallLayout[i].endX][y].setTexture(1);
                    else
                        for (int y = hallLayout[i].startY; y < hallLayout[i].endY; y++)
                            floorTiles[hallLayout[i].endX][y].setTexture(1);
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
                            floorTiles[x][exit_coord.y].setTexture(4);
                            exitPlaced = true;
                        }
                    }
                }
                
                for (int y = exit_coord.y - 1; y <= exit_coord.y + 1; y++)
                    if (y < stdfloorSize && y > 0)
                    {
                        if (floorTiles[exit_coord.x][y].isVoid() && !exitPlaced)
                        {
                            floorTiles[exit_coord.x][y].setTexture(4);
                            exitPlaced = true;
                        }
                    }
            }
            //add walls around all walkable tiles.
            for (int x = 0; x < stdfloorSize; x++)
                for (int y = 0; y < stdfloorSize; y++)
                    if (floorTiles[x][y].isPassable())
                        replace_surrounding_void(floorTiles[x][y].get_grid_c(), 2);

            //Add gold piles
            int number_of_goldpiles = 5 + randGen.Next(6);
            for (int i = 0; i < number_of_goldpiles; i++)
                Money.Add(new Goldpile(random_valid_position(), cManager, 10 + randGen.Next(41)));

            //Add monsters. They can all be zombies for now.
            int number_of_monsters = 6 + randGen.Next(2, 6);
            for (int i = 0; i < number_of_monsters; i++)
                badGuys.Add(new Zombie(random_valid_position(), cManager, i));
        }

        //Green text. Function here.
        public void update_all_monsters(Player pl)
        {
            for (int i = 0; i < badGuys.Count; i++)
            {
                badGuys[i].Update_Monster(pl, this);
            }
        }

        //Position stuff
        //Green text. Function here.
        public void replace_surrounding_void(gridCoordinate target_tile, int replacement_tex)
        {
            for (int x = target_tile.x - 1; x < target_tile.x + 2; x++)
                for (int y = target_tile.y - 1; y < target_tile.y + 2; y++)
                    if (floorTiles[x][y].isVoid())
                        floorTiles[x][y].setTexture(2);
        }

        //Green text. Function here.
        public bool isWalkable(gridCoordinate grid_position)
        {
            if(grid_position.x < stdfloorSize && 
               grid_position.y < stdfloorSize &&
               grid_position.x > 0 &&
               grid_position.y > 0)
                return floorTiles[grid_position.x][grid_position.y].isPassable();
            else
                return false;
        }

        //Green text. Function here.
        public bool isExit(gridCoordinate grid_position)
        {
            return floorTiles[grid_position.x][grid_position.y].isExit();
        }

        //Green text. Function here.
        public bool am_i_on_other_monster(gridCoordinate grid_position, int cIndex)
        {
            bool retValue = false;
            for (int i = 0; i < badGuys.Count; i++)
            {
                if (badGuys[i].my_grid_coord.x == grid_position.x &&
                    badGuys[i].my_grid_coord.y == grid_position.y &&
                    badGuys[i].my_Index != cIndex)
                    retValue = true;
            }
            return retValue;
        }

        //Green text. Function here.
        public gridCoordinate random_valid_position()
        {
            int gridx = randGen.Next(stdfloorSize);
            int gridy = randGen.Next(stdfloorSize);

            if (isWalkable(floorTiles[gridx][gridy].get_grid_c()))
                return new gridCoordinate(gridx, gridy);
            else
            {
                //Now we do this the long way...
                int offset = 1;
                while (offset < 50)
                {
                    for (int nextx = gridx - offset; nextx <= gridx + offset; nextx++)
                    {
                        gridCoordinate g_c = new gridCoordinate(nextx, gridy - offset);
                        if (isWalkable(g_c) && !isExit(g_c))
                            return new gridCoordinate(nextx, gridy - offset);
                    }

                    for (int nexty = gridy - offset; nexty <= gridy + offset; nexty++)
                    {
                        gridCoordinate g_c = new gridCoordinate(gridx - offset, nexty);
                        if (isWalkable(g_c) && !isExit(g_c))
                            return new gridCoordinate(gridx - offset, nexty);
                    }

                    for (int nextx = gridx - offset; nextx <= gridx + offset; nextx++)
                    {
                        gridCoordinate g_c = new gridCoordinate(nextx, gridy + offset);
                        if (isWalkable(g_c) && !isExit(g_c))
                            return new gridCoordinate(nextx, gridy + offset);
                    }

                    for (int nexty = gridy - offset; nexty <= gridy + offset; nexty++)
                    {
                        gridCoordinate g_c = new gridCoordinate(gridx + offset, nexty);
                        if (isWalkable(g_c) && !isExit(g_c))
                            return new gridCoordinate(gridx + offset, nexty);
                    }

                    offset++;
                }
                //and if we STILL can't find one, just return the starting coordinate of room 0
                return new gridCoordinate(roomLayout[0].startXPos, roomLayout[0].startYPos);
            }
        }

        //Drawing stuff
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

        //Green text. Function here.
        public void drawEntities(ref SpriteBatch sBatch)
        {
            for (int i = 0; i < Money.Count; i++)
                Money[i].drawMe(ref sBatch);

            for (int i = 0; i < badGuys.Count; i++)
                badGuys[i].drawMe(ref sBatch);
        }

        //Some access shit
        //Green text. Function here.
        public List<Monster> see_badGuys()
        {
            return badGuys;
        }

        public List<Goldpile> show_me_the_money()
        {
            return Money;
        }

        public void damage_monster(int dmg, int monsterID)
        {
            badGuys[monsterID].takeDamage(dmg);
            if (badGuys[monsterID].hitPoints <= 0)
                badGuys.RemoveAt(monsterID);
        }
    }
}