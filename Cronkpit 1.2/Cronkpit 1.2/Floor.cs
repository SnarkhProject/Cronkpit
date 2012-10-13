using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Cronkpit_1._2
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

        List<string> message_buffer;
        //Sensory lists
        List<SightPulse> Vision;
        List<ScentPulse> Sniffs;
        List<SoundPulse> Noises;
        //Other stuff needed for the thing to function
        ContentManager cManager;
        Random randGen;

        //Green text. Function here.
        public Floor(ContentManager sCont, ref List<string> msgBuffer)
        {
            //Init floor components.
            floorTiles = new List<List<Tile>>();
            roomLayout = new List<Room>();
            hallLayout = new List<Hall>();
            badGuys = new List<Monster>();
            Money = new List<Goldpile>();
            Vision = new List<SightPulse>();
            Sniffs = new List<ScentPulse>();
            Noises = new List<SoundPulse>();

            message_buffer = msgBuffer;

            //Next init other stuff
            cManager = sCont;
            randGen = new Random();
        
            //Then do stuff for real
            buildFloor();
        }

        #region super important stuff

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
            int number_of_rooms = 5 + randGen.Next(4);
            for (int i = 0; i < number_of_rooms; i++)
            {
                int next_room_height = randGen.Next(4, 9);
                int next_room_width = randGen.Next(4, 9);
                int next_room_startX = randGen.Next(1, ((stdfloorSize - 1) - next_room_width));
                int next_room_startY = randGen.Next(1, ((stdfloorSize - 1) - next_room_height));
                roomLayout.Add(new Room(next_room_height,
                                    next_room_width,
                                    next_room_startX,
                                    next_room_startY));
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
            int number_of_goldpiles = 7 + randGen.Next(8);
            for (int i = 0; i < number_of_goldpiles; i++)
                Money.Add(new Goldpile(random_valid_position(), cManager, 10 + randGen.Next(41)));

            //Add monsters.
            int number_of_monsters = 7 + randGen.Next(4, 8);
            for (int i = 0; i < number_of_monsters; i++)
            {
                int monsterType = randGen.Next(100);
                if(monsterType < 10)
                    badGuys.Add(new HollowKnight(valid_hollowKnight_spawn(), cManager, i));
                else if(monsterType > 10 && monsterType < 30)
                    badGuys.Add(new GoreHound(random_valid_position(), cManager, i));
                else
                    badGuys.Add(new Zombie(random_valid_position(), cManager, i));
            }
            //Have monsters wander
            /*
            for(int i = 0; i < stdfloorSize; i++)
                for(int j = 0; j < badGuys.Count; j++)
                    badGuys[j].wander(
             */
        }

        //Green text. Function here.
        public void update_dungeon_floor(Player Pl)
        {
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

        #endregion

        #region messaging stuff
        
        //Green text
        public void addmsg(string message)
        {
            message_buffer.Add(message);
        }

        #endregion

        #region position stuff

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

            if (isWalkable(floorTiles[gridx][gridy].get_grid_c()) && 
                !isExit(floorTiles[gridx][gridy].get_grid_c()) &&
                !is_entity_here(floorTiles[gridx][gridy].get_grid_c()))
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
                        if (isWalkable(g_c) && !isExit(g_c) && !is_entity_here(g_c))
                            return new gridCoordinate(nextx, gridy - offset);
                    }

                    for (int nexty = gridy - offset; nexty <= gridy + offset; nexty++)
                    {
                        gridCoordinate g_c = new gridCoordinate(gridx - offset, nexty);
                        if (isWalkable(g_c) && !isExit(g_c) && !is_entity_here(g_c))
                            return new gridCoordinate(gridx - offset, nexty);
                    }

                    for (int nextx = gridx - offset; nextx <= gridx + offset; nextx++)
                    {
                        gridCoordinate g_c = new gridCoordinate(nextx, gridy + offset);
                        if (isWalkable(g_c) && !isExit(g_c) && !is_entity_here(g_c))
                            return new gridCoordinate(nextx, gridy + offset);
                    }

                    for (int nexty = gridy - offset; nexty <= gridy + offset; nexty++)
                    {
                        gridCoordinate g_c = new gridCoordinate(gridx + offset, nexty);
                        if (isWalkable(g_c) && !isExit(g_c) && !is_entity_here(g_c))
                            return new gridCoordinate(gridx + offset, nexty);
                    }

                    offset++;
                }
                //and if we STILL can't find one, just return the starting coordinate of room 0
                return new gridCoordinate(roomLayout[0].startXPos, roomLayout[0].startYPos);
            }
        }

        //Green text!
        public gridCoordinate valid_hollowKnight_spawn()
        {
            bool goodPosition = false;
            gridCoordinate returnCoord = random_valid_position();
            while (!goodPosition)
            {
                gridCoordinate gc = random_valid_position();
                if ((isWalkable(new gridCoordinate(gc.x - 1, gc.y)) && !isWalkable(new gridCoordinate(gc.x + 1, gc.y))) ||
                    (isWalkable(new gridCoordinate(gc.x + 1, gc.y)) && !isWalkable(new gridCoordinate(gc.x - 1, gc.y))) ||
                    (isWalkable(new gridCoordinate(gc.x, gc.y + 1)) && !isWalkable(new gridCoordinate(gc.x, gc.y - 1))) ||
                    (isWalkable(new gridCoordinate(gc.x, gc.y - 1)) && !isWalkable(new gridCoordinate(gc.x, gc.y + 1))))
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
                if (badGuys[i].my_grid_coord.x == gc.x && badGuys[i].my_grid_coord.y == gc.y)
                    return true;

            for (int i = 0; i < Money.Count; i++)
                if (Money[i].get_my_grid_C().x == gc.x && Money[i].get_my_grid_C().y == gc.y)
                    return true;

            return false;
        }

        #endregion

        #region all sensory stuff

        #region smell stuff
        //Green text. Function here.
        public void add_smell_to_tile(gridCoordinate grid_position, int sType, int sValue)
        {
            floorTiles[grid_position.x][grid_position.y].addScent(sType, sValue);
        }

        //Green text.
        public void decay_all_scents()
        {
            for (int x = 0; x < stdfloorSize; x++)
                for (int y = 0; y < stdfloorSize; y++)
                    floorTiles[x][y].decayScents();
        }

        //Green text.
        public void scent_pulse(gridCoordinate origin, int targetSmell, int monsterID, int smellRange, int smellThreshold)
        {
            //Do this 8 times, once for each direction.
            Sniffs.Add(new ScentPulse(new gridCoordinate(origin.x, origin.y - 1), 0, monsterID, smellRange));
            Sniffs.Add(new ScentPulse(new gridCoordinate(origin.x, origin.y + 1), 1, monsterID, smellRange));
            Sniffs.Add(new ScentPulse(new gridCoordinate(origin.x - 1, origin.y), 2, monsterID, smellRange));
            Sniffs.Add(new ScentPulse(new gridCoordinate(origin.x + 1, origin.y), 3, monsterID, smellRange));
            Sniffs.Add(new ScentPulse(new gridCoordinate(origin.x + 1, origin.y + 1), 4, monsterID, smellRange));
            Sniffs.Add(new ScentPulse(new gridCoordinate(origin.x - 1, origin.y + 1), 5, monsterID, smellRange));
            Sniffs.Add(new ScentPulse(new gridCoordinate(origin.x + 1, origin.y - 1), 6, monsterID, smellRange));
            Sniffs.Add(new ScentPulse(new gridCoordinate(origin.x - 1, origin.y - 1), 7, monsterID, smellRange));
            int strongest_smell_value = 0;
            while (Sniffs.Count > 0)
            {
                for (int i = 0; i < Sniffs.Count; i++)
                {
                    //check for the target scent being present on the tile.
                    if (floorTiles[Sniffs[i].my_coord().x][Sniffs[i].my_coord().y].is_scent_present(targetSmell))
                    {
                        int current_smell_value = floorTiles[Sniffs[i].my_coord().x][Sniffs[i].my_coord().y].strength_of_scent(targetSmell);
                        //if it is, get the value, then compare it to our current strongest smell.
                        //It also must be greater than the threshold
                        if (current_smell_value > strongest_smell_value &&
                            current_smell_value > smellThreshold)
                        {
                            strongest_smell_value = current_smell_value;
                            //This means a scent has been found! Pass it on to the monster... if it still exists.
                            for (int j = 0; j < badGuys.Count; j++)
                            {
                                if (badGuys[j].my_Index == monsterID)
                                {
                                    strongest_smell_value = current_smell_value;
                                    badGuys[j].has_scent = true;
                                    badGuys[j].strongest_smell_coord.x = Sniffs[i].my_coord().x;
                                    badGuys[j].strongest_smell_coord.y = Sniffs[i].my_coord().y;
                                }
                            }
                        }
                    }
                    if (Sniffs[i].my_strength() > 0)
                        Sniffs[i].update(this);
                    else
                        Sniffs.RemoveAt(i);
                }
            }
        }

        //Green text.
        public void add_single_scent_pulse(ScentPulse pulse)
        {
            Sniffs.Add(pulse);
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
        public void sound_pulse(gridCoordinate origin, int soundRange, int soundType)
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
                        if(Noises[i].my_coord().x == badGuys[j].my_grid_coord.x &&
                            Noises[i].my_coord().y == badGuys[j].my_grid_coord.y)
                            if (badGuys[j].can_hear && Noises[i].my_strength() > badGuys[j].listen_threshold)
                            {
                                badGuys[j].next_path_to_sound(Noises[i].my_path());
                            }
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

        //Green text.
        public void sight_pulse(gridCoordinate origin,  Player pl, int monsterID, int sightRange)
        {
            //Do this 8 times, once for each direction.
            Vision.Add(new SightPulse(new gridCoordinate(origin.x, origin.y - 1), 0, monsterID, sightRange));
            Vision.Add(new SightPulse(new gridCoordinate(origin.x, origin.y + 1), 1, monsterID, sightRange));
            Vision.Add(new SightPulse(new gridCoordinate(origin.x - 1, origin.y), 2, monsterID, sightRange));
            Vision.Add(new SightPulse(new gridCoordinate(origin.x + 1, origin.y), 3, monsterID, sightRange));
            Vision.Add(new SightPulse(new gridCoordinate(origin.x + 1, origin.y + 1), 4, monsterID, sightRange));
            Vision.Add(new SightPulse(new gridCoordinate(origin.x - 1, origin.y + 1), 5, monsterID, sightRange));
            Vision.Add(new SightPulse(new gridCoordinate(origin.x + 1, origin.y - 1), 6, monsterID, sightRange));
            Vision.Add(new SightPulse(new gridCoordinate(origin.x - 1, origin.y - 1), 7, monsterID, sightRange));
            while (Vision.Count > 0)
            {
                for (int i = 0; i < Vision.Count; i++)
                {
                    //check for the player being on the particle.
                    if (Vision[i].my_coord().x == pl.get_my_grid_C().x &&
                        Vision[i].my_coord().y == pl.get_my_grid_C().y)
                    {
                        for (int j = 0; j < badGuys.Count; j++)
                        {
                            if(badGuys[j].my_Index == Vision[i].my_monster_ID())
                                badGuys[j].can_see_player = true;
                        }
                    }
                    if (Vision[i].my_strength() > 0)
                        Vision[i].update(this);
                    else
                        Vision.RemoveAt(i);
                }
            }
        }

        //Green text
        public void add_single_sight_pulse(SightPulse pulse)
        {
            Vision.Add(pulse);
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

        //Green text. Function here.
        public void drawEntities(ref SpriteBatch sBatch)
        {
            for (int i = 0; i < Money.Count; i++)
                Money[i].drawMe(ref sBatch);
        }

        public void drawEnemies(ref SpriteBatch sBatch)
        {
            for (int i = 0; i < badGuys.Count; i++)
                badGuys[i].drawMe(ref sBatch);
        }

        #endregion

        #region some miscellaneous access stuff
        //Green text. Function here.
        public List<Monster> see_badGuys()
        {
            return badGuys;
        }

        public Monster specific_badguy(int index)
        {
            return badGuys[index];
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

        #endregion
    }
}