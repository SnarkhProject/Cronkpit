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
    class Tile
    {
        public enum Aura { None, Attack, SmellTarget };

        private Texture2D my_Texture;
        private Texture2D my_blank_texture;
        private Aura my_Aura;
        private Vector2 my_Position;
        private ContentManager cont;
        private gridCoordinate grid_coord;
        private List<Scent> smells;
        private int tile_type;
        private int random_variation;
        bool opaque;
        bool passable;
        int sound_absorbtion_value;
        bool deflect_sound;

        private Vector2 corner_1; //Upper left
        private Vector2 corner_2; //Upper right
        private Vector2 corner_3; //Lower left
        private Vector2 corner_4; //Lower right

        public Tile(int sType, int sVari, ContentManager sCont, Texture2D bText, Vector2 sPos, gridCoordinate sgCoord)
        {
            cont = sCont;
            my_Position = sPos;
            grid_coord = sgCoord;
            tile_type = sType;
            random_variation = sVari;
            set_tile_type(sType);
            smells = new List<Scent>();
            my_Aura = Aura.None;
            my_blank_texture = bText;

            corner_1 = new Vector2(sPos.X + 16, sPos.Y + 13);
            corner_2 = new Vector2(sPos.X + 16, sPos.Y + 19);
            corner_3 = new Vector2(sPos.X + 13, sPos.Y + 16);
            corner_4 = new Vector2(sPos.X + 19, sPos.Y + 16);
        }

        public void set_tile_type(int sType)
        {
            tile_type = sType;
            /*
             * Brief guide to tile types:
             * 
             * 0 = Void
             * 1 = stone floor
             * 2 = Wall
             * 4 = Exit
             * 5 = dirt floor
             * 6 = dirt wall
             * 7 = rubble
             * 8 = harsh rock
             */
            switch(sType)
            {
                case 1:
                    opaque = false;
                    deflect_sound = false;
                    passable = true;
                    sound_absorbtion_value = 1;
                    if (random_variation < 5)
                        my_Texture = cont.Load<Texture2D>("Background/stonefloorwcrack");
                    else
                        my_Texture = cont.Load<Texture2D>("Background/stonefloor");
                    break;
                case 2:
                    opaque = true;
                    deflect_sound = true;
                    passable = false;
                    sound_absorbtion_value = 1;
                    if (random_variation < 15)
                        my_Texture = cont.Load<Texture2D>("Background/stonebrickwtorch");
                    else
                        my_Texture = cont.Load<Texture2D>("Background/stonebrick");
                    break;
                case 4:
                    opaque = false;
                    deflect_sound = false;
                    passable = true;
                    sound_absorbtion_value = 1;
                    my_Texture = cont.Load<Texture2D>("Background/exit");
                    break;
                case 5:
                    opaque = false;
                    deflect_sound = false;
                    passable = true;
                    sound_absorbtion_value = 2;
                    my_Texture = cont.Load<Texture2D>("Background/dirtfloor");
                    break;
                case 6:
                    opaque = true;
                    deflect_sound = true;
                    passable = false;
                    sound_absorbtion_value = 3;
                    if (random_variation < 15)
                        my_Texture = cont.Load<Texture2D>("Background/dirtwallwtorch");
                    else
                        my_Texture = cont.Load<Texture2D>("Background/dirtwall");
                    break;
                case 7:
                    opaque = false;
                    deflect_sound = false;
                    passable = true;
                    sound_absorbtion_value = 1;
                    my_Texture = cont.Load<Texture2D>("Background/rubble_floor");
                    break;
                case 8:
                    opaque = true;
                    deflect_sound = true;
                    passable = false;
                    sound_absorbtion_value = 2;
                    my_Texture = cont.Load<Texture2D>("Background/rubble_wall");
                    break;
                default:
                    opaque = true;
                    deflect_sound = false;
                    passable = false;
                    sound_absorbtion_value = 1000;
                    my_Texture = cont.Load<Texture2D>("Background/badvoidtile");
                    break;
            }
        }

        public void mossify()
        {
            switch (tile_type)
            {
                case 1:
                    if (random_variation < 5)
                        my_Texture = cont.Load<Texture2D>("Background/Moss Tiles/mossy_stonefloorwcrack");
                    else
                        my_Texture = cont.Load<Texture2D>("Background/Moss Tiles/mossy_stonefloor");
                    break;
                case 2:
                    if (random_variation >= 15)
                        my_Texture = cont.Load<Texture2D>("Background/Moss Tiles/mossy_stonebrick");
                    break;
                case 5:
                    my_Texture = cont.Load<Texture2D>("Background/Moss Tiles/mossy_dirtfloor");
                    break;
                case 6:
                    if (random_variation >= 15)
                        my_Texture = cont.Load<Texture2D>("Background/Moss Tiles/mossy_dirtwall");
                    break;
            }

            int temp_sound_absorb = (2 * sound_absorbtion_value) + 1;
            sound_absorbtion_value = temp_sound_absorb;
        }

        public bool isVoid()
        {
            return tile_type == 0;
        }

        public bool isPassable()
        {
            return passable;
        }

        public bool isExit()
        {
            return tile_type == 4;
        }

        public gridCoordinate get_grid_c()
        {
            return grid_coord;
        }

        public Vector2 get_corner(int corner)
        {
            switch (corner)
            {
                case 1:
                    return corner_1;
                case 2:
                    return corner_2;
                case 3:
                    return corner_3;
                case 4:
                    return corner_4;
            }

            return new Vector2(-1, -1);
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

        public bool any_scent_present()
        {
            return smells.Count > 0;
        }

        public void set_my_aura(Aura target_aura)
        {
            my_Aura = target_aura;
        }

        public Aura get_my_aura()
        {
            return my_Aura;
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

        public void draw_my_aura(ref SpriteBatch sb)
        {
            Rectangle my_rect = new Rectangle((int)my_Position.X, (int)my_Position.Y, 32, 32);
            Color my_color = Color.White;

            switch (my_Aura)
            {
                case Aura.Attack:
                    my_color = new Color(255, 0, 0, 100);
                    break;
                case Aura.SmellTarget:
                    my_color = new Color(255, 120, 10, 100);
                    break;
            }

            if (!isVoid() && my_Aura != Aura.None)
                sb.Draw(my_blank_texture, my_rect, my_color);
        }

        public int get_my_tile_type()
        {
            return tile_type;
        }
    }
}