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
        private int stdfloorSize = 50;
        public enum specific_effect { Power_Strike, Cleave };
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
        List<Projectile> Pew_Pews;
        List<Effect> eff_ex;
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

        //Green text. Function here.
        public Floor(ContentManager sCont, ref List<string> msgBuffer, Texture2D blnkTex, int floor_number)
        {
            //Init floor components.
            floorTiles = new List<List<Tile>>();
            scentedTiles = new List<Tile>();
            roomLayout = new List<Room>();
            hallLayout = new List<Hall>();
            mossLayout = new List<MossyPatch>();
            badGuys = new List<Monster>();
            Money = new List<Goldpile>();
            Vision_Rc = new List<VisionRay>();
            //Vision_Log = new List<VisionRay>(); Deprecated!
            Noises = new List<SoundPulse>();
            Pew_Pews = new List<Projectile>();
            eff_ex = new List<Effect>();
            popup_alerts = new List<Popup>();
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
                    floorTiles[x].Add(new Tile(0, randGen.Next(100), cManager, blank_texture, tilePos, new gridCoordinate(x, y)));
                }
            }

            //Randomly generate rooms and hallways.
            //First rooms.
            int number_of_rooms = 5 + randGen.Next(4);
            int dirt_threshold = Math.Max((50 - ((fl_number - 1) * 2)), 20);
            for (int i = 0; i < number_of_rooms; i++)
            {
                int next_room_height = randGen.Next(4, 9);
                int next_room_width = randGen.Next(4, 9);
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

                roomLayout.Add(new Room(next_room_height,
                                    next_room_width,
                                    next_room_startX,
                                    next_room_startY,
                                    dirt_room));
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
                    {
                        if (!roomLayout[i].is_dirt_room())
                            floorTiles[x][y].setTexture(1);
                        else
                            floorTiles[x][y].setTexture(5);
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
                        replace_surrounding_void(floorTiles[x][y], 2);

            //Next, do mossy tiles.
            int moss_patches = randGen.Next(Math.Min(5, Math.Max(fl_number - 3, 0))+1);
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
                                floorTiles[x_position][y_position].mossify();
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
                    int amt_to_subtract = Math.Min(randGen.Next(0, (50-Money[i].my_quantity)+1), gold_per_floor);
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
                        switch (skelwpn)
                        {
                            case 0:
                                badGuys.Add(new Skeleton(random_valid_position(), cManager, i, Skeleton.Skeleton_Weapon_Type.Fist));
                                break;
                            case 1:
                                badGuys.Add(new Skeleton(random_valid_position(), cManager, i, Skeleton.Skeleton_Weapon_Type.Axe));
                                break;
                            case 2:
                                badGuys.Add(new Skeleton(random_valid_position(), cManager, i, Skeleton.Skeleton_Weapon_Type.Bow));
                                break;
                            case 3:
                                badGuys.Add(new Skeleton(random_valid_position(), cManager, i, Skeleton.Skeleton_Weapon_Type.Sword));
                                break;
                            case 4:
                                badGuys.Add(new Skeleton(random_valid_position(), cManager, i, Skeleton.Skeleton_Weapon_Type.Spear));
                                break;
                            case 5:
                                badGuys.Add(new Skeleton(random_valid_position(), cManager, i, Skeleton.Skeleton_Weapon_Type.Flamebolt));
                                break;
                        }
                        break;
                    case "GoldMimic":
                        badGuys.Add(new GoldMimic(random_valid_position(), cManager, i));
                        break;
                    case "GoreHound":
                        badGuys.Add(new GoreHound(random_valid_position(), cManager, i));
                        break;
                    case "Zombie":
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
                        badGuys.Add(new Necromancer(random_valid_position(), cManager, i, false));
                        break;
                }                    
            }
            //Have monsters wander
            /*
            for(int i = 0; i < stdfloorSize; i++)
                for(int j = 0; j < badGuys.Count; j++)
                    badGuys[j].wander(
             */
        }

        public void draw_hallway_tiles(Tile target_tile)
        {
            if (target_tile.get_my_tile_type() != 1 && target_tile.get_my_tile_type() != 5)
                target_tile.setTexture(1);
        }

        public void replace_surrounding_void(Tile target_tile, int replacement_tex)
        {
            gridCoordinate target_grid_c = target_tile.get_grid_c();
            bool dirt_room = (target_tile.get_my_tile_type() == 5);

            for (int x = target_grid_c.x - 1; x < target_grid_c.x + 2; x++)
                for (int y = target_grid_c.y - 1; y < target_grid_c.y + 2; y++)
                    if (floorTiles[x][y].isVoid())
                    {
                        if (!dirt_room)
                            floorTiles[x][y].setTexture(2);
                        else
                            floorTiles[x][y].setTexture(6);
                    }
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

        public bool is_monster_here(gridCoordinate gc, out int monsterID)
        {
            monsterID = -1;
            for (int i = 0; i < badGuys.Count; i++)
            {
                if (badGuys[i].my_grid_coord.x == gc.x && badGuys[i].my_grid_coord.y == gc.y)
                {
                    monsterID = badGuys[i].my_Index;
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region projectile management

        public void update_all_projectiles(Player pl, float delta_time)
        {
            for (int i = 0; i < Pew_Pews.Count; i++)
            {
                Pew_Pews[i].update(delta_time);

                if(check_overlap(Pew_Pews[i].my_rect(), new Rectangle((int)Pew_Pews[i].get_my_end_coord().x*32, (int)Pew_Pews[i].get_my_end_coord().y*32, 32, 32)))
                    Pew_Pews.RemoveAt(i);
            }
        }

        public void create_new_projectile(Projectile proj)
        {
            Pew_Pews.Add(proj);
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

        public Monster badguy_by_monster_id(int index)
        {
            for (int i = 0; i < badGuys.Count; i++)
                if (badGuys[i].my_Index == index)
                    return badGuys[i];

            return null;
        }

        public List<Goldpile> show_me_the_money()
        {
            return Money;
        }

        public void damage_monster(int dmg, int monsterID, bool melee_attack)
        {
            for (int i = 0; i < badGuys.Count; i++)
            {
                if (badGuys[i].my_Index == monsterID)
                {
                    badGuys[i].takeDamage(dmg, melee_attack, message_buffer, this);
                    if (badGuys[i].hitPoints <= 0)
                        badGuys.RemoveAt(i);
                }
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

        #endregion
    }
}