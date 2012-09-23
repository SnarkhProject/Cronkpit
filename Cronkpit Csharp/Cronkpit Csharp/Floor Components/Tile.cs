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
    class Tile
    {
        private Texture2D my_Texture;
        private Vector2 my_Position;
        private ContentManager cont;
        private gridCoordinate grid_coord;
        private int tile_type;
        private int random_variation;

        public Tile(int sType, int sVari, ContentManager sCont, Vector2 sPos, gridCoordinate sgCoord)
        {
            cont = sCont;
            my_Position = sPos;
            grid_coord = sgCoord;
            tile_type = sType;
            random_variation = sVari;
            setTexture(sType);
        }

        public void setTexture(int sType)
        {
            tile_type = sType;
            /*
             * Brief guide to tile types:
             * 
             * 0 = Void
             * 1 = Floor
             * 2 = Wall
             */
            switch(sType)
            {
                case 1:
                    if (random_variation < 5)
                        my_Texture = cont.Load<Texture2D>("Background/stonefloorwcrack");
                    else
                        my_Texture = cont.Load<Texture2D>("Background/stonefloor");
                    break;
                case 2:
                    if (random_variation < 15)
                        my_Texture = cont.Load<Texture2D>("Background/stonebrickwtorch");
                    else
                        my_Texture = cont.Load<Texture2D>("Background/stonebrick");
                    break;
                case 4:
                    my_Texture = cont.Load<Texture2D>("Background/exit");
                    break;
                default:
                    my_Texture = cont.Load<Texture2D>("Background/badvoidtile");
                    break;
            }
        }

        public bool isVoid()
        {
            return tile_type == 0;
        }

        public bool isPassable()
        {
            return tile_type == 1 || tile_type == 4;
        }

        public bool isExit()
        {
            return tile_type == 4;
        }

        public gridCoordinate get_grid_c()
        {
            return grid_coord;
        }

        //Don't call this unless you've already started the spritebatch!!!
        public void drawMe(ref SpriteBatch sb)
        {
            if(!isVoid())
                sb.Draw(my_Texture, my_Position, Color.White);
        }
    }
}