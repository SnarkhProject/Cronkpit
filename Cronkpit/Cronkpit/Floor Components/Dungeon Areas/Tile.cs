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
        public enum Aura { None, Attack, GonkTarget, Interact };
        public enum Tile_Type { StoneWall, DirtWall, StoneFloor, DirtFloor,
                                Rubble_Wall, Rubble_Floor, Exit, Void,
                                Dungeon_Exit, Locked_Dungeon_Exit, Entrance,
                                Gravel, Shallow_Water, Deep_Water };

        private Texture2D my_Texture;
        private Texture2D my_blank_texture;
        private Aura my_Aura;
        private Vector2 my_Position;
        private ContentManager cont;
        private gridCoordinate grid_coord;
        private List<Scent> smells;
        private Tile_Type tile_typ;
        private int random_variation;
        bool opaque;
        bool passable;
        int sound_absorbtion_value;
        bool deflect_sound;

        private Vector2 corner_1; //Upper left
        private Vector2 corner_2; //Upper right
        private Vector2 corner_3; //Lower left
        private Vector2 corner_4; //Lower right

        public Tile(Tile_Type sType, int sVari, ContentManager sCont, Texture2D bText, Vector2 sPos, gridCoordinate sgCoord,
                    List<KeyValuePair<Tile_Type, Texture2D>> textures)
        {
            cont = sCont;
            my_Position = sPos;
            grid_coord = sgCoord;
            tile_typ = sType;
            random_variation = sVari;
            set_tile_type(sType, textures);
            smells = new List<Scent>();
            my_Aura = Aura.None;
            my_blank_texture = bText;

            corner_1 = new Vector2(sPos.X + 16, sPos.Y + 13);
            corner_2 = new Vector2(sPos.X + 16, sPos.Y + 19);
            corner_3 = new Vector2(sPos.X + 13, sPos.Y + 16);
            corner_4 = new Vector2(sPos.X + 19, sPos.Y + 16);
        }

        public void set_tile_type(Tile_Type sType, List<KeyValuePair<Tile_Type, Texture2D>> textures)
        {
            tile_typ = sType;

            List<Texture2D> relevant_textures = new List<Texture2D>();
            for (int i = 0; i < textures.Count; i++)
                if (tile_typ == textures[i].Key)
                    relevant_textures.Add(textures[i].Value);
            
            switch(sType)
            {
                case Tile_Type.StoneFloor:
                    opaque = false;
                    deflect_sound = false;
                    passable = true;
                    sound_absorbtion_value = 1;
                    if (random_variation < 5)
                        my_Texture = relevant_textures[2];
                    else if (random_variation >= 5 && random_variation <= 10)
                        my_Texture = relevant_textures[1];
                    else
                        my_Texture = relevant_textures[0];
                    break;
                case Tile_Type.StoneWall:
                    opaque = true;
                    deflect_sound = true;
                    passable = false;
                    sound_absorbtion_value = 1;
                    if (random_variation < 15)
                        my_Texture = relevant_textures[2];
                    else if (random_variation >= 15 && random_variation <= 57)
                        my_Texture = relevant_textures[1];
                    else
                        my_Texture = relevant_textures[0];
                    break;
                case Tile_Type.Exit:
                    opaque = false;
                    deflect_sound = false;
                    passable = true;
                    sound_absorbtion_value = 1;
                    my_Texture = relevant_textures[0];
                    break;
                case Tile_Type.Entrance:
                    opaque = false;
                    deflect_sound = false;
                    passable = true;
                    sound_absorbtion_value = 1;
                    my_Texture = relevant_textures[0];
                    break;
                case Tile_Type.DirtFloor:
                    opaque = false;
                    deflect_sound = false;
                    passable = true;
                    sound_absorbtion_value = 2;
                    if (random_variation <= 50)
                        my_Texture = relevant_textures[0];
                    else
                        my_Texture = relevant_textures[1];
                    break;
                case Tile_Type.DirtWall:
                    opaque = true;
                    deflect_sound = true;
                    passable = false;
                    sound_absorbtion_value = 3;
                    if (random_variation < 15)
                        my_Texture = relevant_textures[2];
                    else if (random_variation >= 15 && random_variation <= 57)
                        my_Texture = relevant_textures[1];
                    else
                        my_Texture = relevant_textures[0];
                    break;
                case Tile_Type.Rubble_Floor:
                    opaque = false;
                    deflect_sound = false;
                    passable = true;
                    sound_absorbtion_value = 1;
                    my_Texture = relevant_textures[0];
                    break;
                case Tile_Type.Rubble_Wall:
                    opaque = true;
                    deflect_sound = true;
                    passable = false;
                    sound_absorbtion_value = 2;
                    my_Texture = relevant_textures[0];
                    break;
                case Tile_Type.Dungeon_Exit:
                    opaque = false;
                    deflect_sound = false;
                    passable = true;
                    sound_absorbtion_value = 1;
                    my_Texture = relevant_textures[0];
                    break;
                case Tile_Type.Locked_Dungeon_Exit:
                    opaque = false;
                    deflect_sound = false;
                    passable = false;
                    sound_absorbtion_value = 2;
                    my_Texture = relevant_textures[1];
                    break;
                case Tile_Type.Gravel:
                    opaque = false;
                    deflect_sound = false;
                    passable = true;
                    sound_absorbtion_value = 2;
                    my_Texture = relevant_textures[0];
                    break;
                case Tile_Type.Shallow_Water:
                    opaque = false;
                    deflect_sound = false;
                    passable = true;
                    sound_absorbtion_value = 1;
                    my_Texture = relevant_textures[0];
                    break;
                case Tile_Type.Deep_Water:
                    opaque = false;
                    deflect_sound = false;
                    passable = false;
                    sound_absorbtion_value = 1;
                    my_Texture = relevant_textures[0];
                    break;
                case Tile_Type.Void:
                default:
                    opaque = true;
                    deflect_sound = false;
                    passable = false;
                    sound_absorbtion_value = 1000;
                    my_Texture = relevant_textures[0];
                    break;
            }
        }

        public void mossify(Texture2D[] textures)
        {
            switch (tile_typ)
            {
                case Tile_Type.StoneFloor:
                    if (random_variation < 5)
                        my_Texture = textures[4];
                    else if (random_variation >= 5 && random_variation < 10)
                        my_Texture = textures[3];
                    else if (random_variation >= 10 && random_variation < 40)
                        my_Texture = textures[2];
                    else if (random_variation >= 40 && random_variation < 70)
                        my_Texture = textures[1];
                    else
                        my_Texture = textures[0];
                    break;
                case Tile_Type.DirtWall:
                case Tile_Type.StoneWall:
                    if (random_variation >= 15)
                        if (random_variation < 50)
                            my_Texture = textures[0];
                        else
                            my_Texture = textures[1];
                    break;
                case Tile_Type.DirtFloor:
                    if (random_variation < 50)
                        my_Texture = textures[0];
                    else
                        my_Texture = textures[1];
                    break;
            }

            int temp_sound_absorb = (2 * sound_absorbtion_value) + 1;
            sound_absorbtion_value = temp_sound_absorb;
        }

        public bool isVoid()
        {
            return tile_typ == Tile_Type.Void;
        }

        public bool isPassable()
        {
            return passable;
        }

        public bool isExit()
        {
            return tile_typ == Tile_Type.Exit;
        }

        public bool isExitorEntrance()
        {
            return tile_typ == Tile_Type.Exit || tile_typ == Tile_Type.Entrance;
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
            int scent_decay_factor = 1;
            if (tile_typ == Tile_Type.Shallow_Water || tile_typ == Tile_Type.Deep_Water)
                scent_decay_factor = 3;
            for (int i = 0; i < smells.Count; i++)
            {
                smells[i].strength -= scent_decay_factor;
                if (smells[i].strength <= 0)
                    smells.RemoveAt(i);
            }
        }

        public void addScent(int sm_type, int value)
        {
                if (!is_scent_present(sm_type))
                    smells.Add(new Scent(sm_type, value));
                else
                    for (int i = 0; i < smells.Count; i++)
                    {
                        if (smells[i].type == sm_type && value > smells[i].strength)
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
                case Aura.GonkTarget:
                    my_color = new Color(255, 120, 10, 100);
                    break;
            }

            if (!isVoid() && my_Aura != Aura.None)
                sb.Draw(my_blank_texture, my_rect, my_color);
        }

        public Tile_Type get_my_tile_type()
        {
            return tile_typ;
        }
    }
}