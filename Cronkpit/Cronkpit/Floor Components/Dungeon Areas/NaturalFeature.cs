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
    class NaturalFeature
    {
        public enum Feature_Type { Generic, River, Lake, Chasm };
        Feature_Type my_fType;

        //River only
        gridCoordinate river_startCoord;
        gridCoordinate river_endCoord;
        int deepwater_thickness;
        int shallow_thickness;
        int banks_thickness;

        //Lake only


        //Chasm only

        /// <summary>
        /// Constructor to initalize NaturalFeature as a River.
        /// </summary>
        /// <param name="river_start"></param>
        /// <param name="river_end"></param>
        /// <param name="deep_thick"></param>
        /// <param name="shallow_thick"></param>
        /// <param name="bank_thick"></param>
        public NaturalFeature(gridCoordinate river_start, gridCoordinate river_end, int deep_thick,
                              int shallow_thick, int bank_thick)
        {
            river_startCoord = river_start;
            river_endCoord = river_end;
            deepwater_thickness = deep_thick;
            shallow_thickness = shallow_thick;
            banks_thickness = bank_thick;
            my_fType = Feature_Type.River;
        }

        public Feature_Type get_type()
        {
            return my_fType;
        }

        public bool is_river_finished(double c_x, double c_y)
        {
            int current_x = (int)c_x / 32;
            int current_y = (int)c_y / 32;

            if (Math.Abs(current_x - river_endCoord.x) <= 2 &&
                Math.Abs(current_y - river_endCoord.y) <= 2)
                return true;

            return false;
        }

        public void draw_river(ref List<List<Tile>> grid, Random rGen,
                               Tile.Tile_Type deep_water_tiltyp, 
                               Tile.Tile_Type shallows_tiltyp,
                               Tile.Tile_Type shoreline_tiltyp,
                               List<KeyValuePair<Tile.Tile_Type, Texture2D>> textureList)
        {
            //First determine whether it's a mostly horizontal or vertical river.
            //Get differences.
            int abs_xDif = Math.Abs(river_startCoord.x - river_endCoord.x);
            int abs_yDif = Math.Abs(river_startCoord.y - river_endCoord.y);
            int xDif = river_startCoord.x - river_endCoord.x;
            int yDif = river_startCoord.y - river_endCoord.y;
            //Get Vector points.
            Vector2 end_position = new Vector2(river_endCoord.x * 32, river_endCoord.y * 32);
            //Get pixel differences
            double xPixels = xDif * 32;
            double yPixels = yDif * 32;

            gridCoordinate[,] dirs =  {{new gridCoordinate(1, 0), new gridCoordinate(-1, 0)},
                                       {new gridCoordinate(0, 1), new gridCoordinate(0, -1)}};
            int direct = 0;
            gridCoordinate.coord xy = gridCoordinate.coord.xCoord;
            if (abs_xDif > abs_yDif)
            {
                direct = 1;
                xy = gridCoordinate.coord.yCoord;
            }

            Vector2 c_position = new Vector2(river_startCoord.x * 32, river_startCoord.y * 32);
            double c_xvalue = c_position.X;
            double c_yvalue = c_position.Y;
            bool done = false;
            gridCoordinate current_position = new gridCoordinate(-1, -1);
            gridCoordinate previous_position = new gridCoordinate(-1, -1);
            while (!done)
            {
                done = is_river_finished(c_xvalue, c_yvalue);
                current_position = new gridCoordinate((int)(c_xvalue / 32), (int)(c_yvalue / 32));

                if (!(current_position.x == previous_position.x && current_position.y == previous_position.y))
                {
                    int[] rr = {rGen.Next(deepwater_thickness+1),
                                    rGen.Next(1, shallow_thickness+1),
                                    rGen.Next(1, banks_thickness+1)};
                    int[] rEdge = new int[3];
                    rEdge[0] = current_position.get_a_coord(xy) + 1 + rr[0];
                    rEdge[1] = rEdge[0] + rr[1];
                    rEdge[2] = rEdge[1] + rr[2];

                    grid[current_position.x][current_position.y].set_tile_type(deep_water_tiltyp, textureList);
                    for (gridCoordinate i = new gridCoordinate(current_position);
                         i.get_a_coord(xy) < rEdge[2];
                         i.combineCoords(dirs[direct, 0]))
                    {
                        if (i.get_a_coord(xy) >= grid.Count)
                            break;

                        if (i.get_a_coord(xy) < rEdge[0])
                            grid[i.x][i.y].set_tile_type(deep_water_tiltyp, textureList);
                        else if (i.get_a_coord(xy) < rEdge[1])
                            grid[i.x][i.y].set_tile_type(shallows_tiltyp, textureList);
                        else
                            grid[i.x][i.y].set_tile_type(shoreline_tiltyp, textureList);
                    }

                    int[] lr = {rGen.Next(deepwater_thickness+1),
                                    rGen.Next(1, shallow_thickness+1),
                                    rGen.Next(1, banks_thickness+1)};
                    int[] lEdge = new int[3];
                    lEdge[0] = current_position.get_a_coord(xy) - 1 - lr[0];
                    lEdge[1] = lEdge[0] - lr[1];
                    lEdge[2] = lEdge[1] - lr[2];

                    for (gridCoordinate i = new gridCoordinate(current_position);
                         i.get_a_coord(xy) > lEdge[2];
                         i.combineCoords(dirs[direct, 1]))
                    {
                        if (i.get_a_coord(xy) < 0)
                            break;

                        if (i.get_a_coord(xy) > lEdge[0])
                            grid[i.x][i.y].set_tile_type(deep_water_tiltyp, textureList);
                        else if (i.get_a_coord(xy) > lEdge[1])
                            grid[i.x][i.y].set_tile_type(shallows_tiltyp, textureList);
                        else
                            grid[i.x][i.y].set_tile_type(shoreline_tiltyp, textureList);
                    }
                }

                Vector2 direction = end_position - c_position;
                direction.Normalize();

                c_xvalue += direction.X;
                c_yvalue += direction.Y;

                previous_position = current_position;
            }
        }
    }
}
