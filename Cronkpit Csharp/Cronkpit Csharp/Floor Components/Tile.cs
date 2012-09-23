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
        private List<Scent> smells;
        private int tile_type;
        private int random_variation;
        bool opaque;
        int sound_absorbtion_value;
        bool deflect_sound;

        public Tile(int sType, int sVari, ContentManager sCont, Vector2 sPos, gridCoordinate sgCoord)
        {
            cont = sCont;
            my_Position = sPos;
            grid_coord = sgCoord;
            tile_type = sType;
            random_variation = sVari;
            setTexture(sType);
            smells = new List<Scent>();
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
             * 4 = Exit
             */
            switch(sType)
            {
                case 1:
                    opaque = false;
                    deflect_sound = false;
                    sound_absorbtion_value = 1;
                    if (random_variation < 5)
                        my_Texture = cont.Load<Texture2D>("Background/stonefloorwcrack");
                    else
                        my_Texture = cont.Load<Texture2D>("Background/stonefloor");
                    break;
                case 2:
                    opaque = true;
                    deflect_sound = true;
                    sound_absorbtion_value = 2;
                    if (random_variation < 15)
                        my_Texture = cont.Load<Texture2D>("Background/stonebrickwtorch");
                    else
                        my_Texture = cont.Load<Texture2D>("Background/stonebrick");
                    break;
                case 4:
                    opaque = false;
                    deflect_sound = false;
                    sound_absorbtion_value = 1;
                    my_Texture = cont.Load<Texture2D>("Background/exit");
                    break;
                default:
                    opaque = true;
                    deflect_sound = false;
                    sound_absorbtion_value = 1000;
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

        //Smell functions.
        public void decayScents()
        {
            for (int i = 0; i < smells.Count; i++)
            {
                smells[i].strength--;
                if (smells[i].strength <= 0)
                    smells.RemoveAt(i);
            }
        }

        public void addScent(int sm_type, int value)
        {
            if(!is_scent_present(sm_type))
                smells.Add(new Scent(sm_type, value));
            else
                for (int i = 0; i < smells.Count; i++)
                {
                    if (smells[i].type == sm_type)
                        smells[i].strength = value;
                }
        }

        public bool is_scent_present(int target_scent)
        {
            for (int i = 0; i < smells.Count; i++)
            {
                if (smells[i].type == target_scent)
                    return true;
            }
            return false;
        }

        public int strength_of_scent(int target_scent)
        {
            for (int i = 0; i < smells.Count; i++)
            {
                if (smells[i].type == target_scent)
                    return smells[i].strength;
            }
            return -1;
        }

        //Sight functions
        public bool isOpaque()
        {
            return opaque;
        }

        //Sound functions
        public bool isDeflector()
        {
            return deflect_sound;
        }

        public int sound_absorb_val()
        {
            return sound_absorbtion_value;
        }

        //Don't call this unless you've already started the spritebatch!!!
        public void drawMe(ref SpriteBatch sb)
        {
            if(!isVoid())
                sb.Draw(my_Texture, my_Position, Color.White);
        }
    }
}