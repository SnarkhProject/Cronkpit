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
    class FloorBuilder
    {
        //Lists that change after being initalized
        List<Monster> badguys;
        List<Doodad> props;
        List<Goldpile> money;

        //Lists that are static - they only matter here.
        List<Room> roomlayout;
        List<Hall> halllayout;
        List<NaturalFeature> featurelayout;
        List<MossyPatch> mosslayout;

        //List of tiles on the floor. This is the only thing the floor really needs to keep afterwards
        List<List<Tile>> floorTiles;
        //Exit and entrance locations.
        gridCoordinate dungeon_exit_coord;
        gridCoordinate dungeon_entrance_coord;

        //Monster tables.
        SpawnTable floorSpawns;
        SpawnTable floor_Sub_Spawns;

        //Textures
        List<KeyValuePair<Texture2D, Tile.Tile_Type>> master_texture_list;

        public FloorBuilder(ref List<List<Tile>> map, ref List<Monster> mons, 
                            ref List<Doodad> doods, ref List<Goldpile> dollars,
                            ref gridCoordinate entrance, ref gridCoordinate exit)
        {
            floorTiles = map;
            badguys = mons;
            props = doods;
            money = dollars;
            dungeon_entrance_coord = entrance;
            dungeon_exit_coord = exit;

            master_texture_list = new List<KeyValuePair<Texture2D, Tile.Tile_Type>>();
        }

        public void build_floor(Cronkpit.CronkPit.Dungeon c_dungeon)
        {

        }
    }
}
