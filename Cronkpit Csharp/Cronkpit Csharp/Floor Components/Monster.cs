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
    class Monster
    {
        public Texture2D my_Texture;
        public Vector2 my_Position;
        public ContentManager cont;
        public gridCoordinate my_grid_coord;
        public Random rGen;

        public bool aggro;
        public int hitPoints;
        public int my_Index;
        public int min_damage;
        public int max_damage;

        public Monster(gridCoordinate sGridCoord, ContentManager sCont, int sIndex)
        {
            cont = sCont;
            my_grid_coord = sGridCoord;
            my_Position = new Vector2(sGridCoord.x * 32, sGridCoord.y * 32);
            rGen = new Random();
            my_Index = sIndex;
            hitPoints = 0;
            min_damage = 0;
            max_damage = 0;
        }

        //don't call unless you've started the spritebatch!
        public void drawMe(ref SpriteBatch sb)
        {
            sb.Draw(my_Texture, my_Position, Color.White);
        }

        public virtual void Update_Monster(Player pl, Floor fl)
        {
        }

        public void reset_my_drawing_position()
        {
            my_Position.X = my_grid_coord.x * 32;
            my_Position.Y = my_grid_coord.y * 32;
        }

        public bool am_i_on_player(Player pl)
        {
            return (my_grid_coord.x == pl.get_my_grid_C().x &&
                    my_grid_coord.y == pl.get_my_grid_C().y);
        }

        public bool is_spot_free(Floor fl, Player pl)
        {
            return (am_i_on_player(pl) == false && 
                    fl.isWalkable(my_grid_coord) &&
                    fl.am_i_on_other_monster(my_grid_coord, my_Index) == false);
        }

        public void takeDamage(int dmg)
        {
            hitPoints -= dmg;
        }

        public int dealDamage()
        {
            return rGen.Next(min_damage, max_damage);
        }

        public bool is_player_within(Player pl, int radius)
        {
            if (pl.get_my_grid_C().x > (my_grid_coord.x - radius) &&
                pl.get_my_grid_C().x < (my_grid_coord.x + radius) &&
                pl.get_my_grid_C().y > (my_grid_coord.y - radius) &&
                pl.get_my_grid_C().y < (my_grid_coord.y + radius))
                return true;
            else
                return false;
        }
    }
}
