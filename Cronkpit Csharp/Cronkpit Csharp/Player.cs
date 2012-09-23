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
    class Player
    {
        //Constructor shit
        private Texture2D my_Texture;
        private Texture2D my_dead_texture;
        private Vector2 my_Position;
        private ContentManager cont;
        private gridCoordinate my_grid_coord;
        private Random rGen;
        //!Constructor shit
        private int my_gold;
        private int max_hp;
        private int current_hp;

        public Player(ContentManager sCont, gridCoordinate sGridCoord)
        {
            //Constructor shit
            cont = sCont;
            my_grid_coord = sGridCoord;
            my_Position = new Vector2(sGridCoord.x * 32, sGridCoord.y * 32);
            my_Texture = cont.Load<Texture2D>("Entities/lmfaoplayer");
            my_dead_texture = cont.Load<Texture2D>("Entities/playercorpse");
            rGen = new Random();
            //!Constructor shit
            my_gold = 0;
            max_hp = 100;
            current_hp = max_hp;
        }

        public void drawMe(ref SpriteBatch sb)
        {
            if (is_alive())
                sb.Draw(my_Texture, my_Position, Color.White);
            else
                sb.Draw(my_dead_texture, my_Position, Color.White);
        }

        public void move(string direction, Floor fl)
        {
            int numeric_direction = -1;
            if (String.Compare("up", direction) == 0)
                numeric_direction = 0;
            else if (String.Compare("down", direction) == 0)
                numeric_direction = 1;
            else if (String.Compare("left", direction) == 0)
                numeric_direction = 2;
            else if (String.Compare("right", direction) == 0)
                numeric_direction = 3;
            else if (String.Compare("downright", direction) == 0)
                numeric_direction = 4;
            else if (String.Compare("downleft", direction) == 0)
                numeric_direction = 5;
            else if (String.Compare("upright", direction) == 0)
                numeric_direction = 6;
            else if (String.Compare("upleft", direction) == 0)
                numeric_direction = 7;
            //0 = up, 1 = down, 2 = left, 3 = right
            //4 = downright, 5 = downleft, 6 = upright, 7 = upleft
            int MonsterID;
            switch (numeric_direction)
            {
                //up, y-
                case 0:
                    my_grid_coord.y--;
                    if (is_spot_free(fl))
                        reset_my_drawing_position();
                    else if(is_monster_present(fl, out MonsterID))
                    {
                        fl.damage_monster(unarmed_damage(), MonsterID);
                        my_grid_coord.y++;
                    }
                    else
                        my_grid_coord.y++;
                    break;
                //down, y+
                case 1:
                    my_grid_coord.y++;
                    if (is_spot_free(fl))
                        reset_my_drawing_position();
                    else if(is_monster_present(fl, out MonsterID))
                    {
                        fl.damage_monster(unarmed_damage(), MonsterID);
                        my_grid_coord.y--;
                    }
                    else
                        my_grid_coord.y--;
                    break;
                //left, x-
                case 2:
                    my_grid_coord.x--;
                    if (is_spot_free(fl))
                        reset_my_drawing_position();
                    else if(is_monster_present(fl, out MonsterID))
                    {
                        fl.damage_monster(unarmed_damage(), MonsterID);
                        my_grid_coord.x++;
                    }
                    else
                        my_grid_coord.x++;
                    break;
                //right, x+
                case 3:
                    my_grid_coord.x++;
                    if (is_spot_free(fl))
                        reset_my_drawing_position();
                    else if (is_monster_present(fl, out MonsterID))
                    {
                        fl.damage_monster(unarmed_damage(), MonsterID);
                        my_grid_coord.x--;
                    }
                    else
                        my_grid_coord.x--;
                    break;
                //down right, x+ y+
                case 4:
                    my_grid_coord.x++;
                    my_grid_coord.y++;
                    if (is_spot_free(fl))
                    {
                        reset_my_drawing_position();
                    }
                    else if (is_monster_present(fl, out MonsterID))
                    {
                        fl.damage_monster(unarmed_damage(), MonsterID);
                        my_grid_coord.x--;
                        my_grid_coord.y--;
                    }
                    else
                    {
                        my_grid_coord.x--;
                        my_grid_coord.y--;
                    }
                    break;
                //down left, x- y+
                case 5:
                    my_grid_coord.x--;
                    my_grid_coord.y++;
                    if (is_spot_free(fl))
                    {
                        reset_my_drawing_position();
                    }
                    else if (is_monster_present(fl, out MonsterID))
                    {
                        fl.damage_monster(unarmed_damage(), MonsterID);
                        my_grid_coord.x++;
                        my_grid_coord.y--;
                    }    
                    else
                    {
                        my_grid_coord.x++;
                        my_grid_coord.y--;
                    }
                    break;
                //up right, x+ y-
                case 6:
                    my_grid_coord.x++;
                    my_grid_coord.y--;
                    if (is_spot_free(fl))
                    {
                        reset_my_drawing_position();
                    }
                    else if (is_monster_present(fl, out MonsterID))
                    {
                        fl.damage_monster(unarmed_damage(), MonsterID);
                        my_grid_coord.x--;
                        my_grid_coord.y++;
                    }
                    else
                    {
                        my_grid_coord.x--;
                        my_grid_coord.y++;
                    }
                    break;
                //up left, x- y-
                case 7:
                    my_grid_coord.x--;
                    my_grid_coord.y--;
                    if (is_spot_free(fl))
                    {
                        reset_my_drawing_position();
                    }
                    else if (is_monster_present(fl, out MonsterID))
                    {
                        fl.damage_monster(unarmed_damage(), MonsterID);
                        my_grid_coord.x++;
                        my_grid_coord.y++;
                    }
                    else
                    {
                        my_grid_coord.x++;
                        my_grid_coord.y++;
                    }
                    break;
                default:
                    break;
            }
            //after moving, loot
            loot(fl);
        }

        public void reset_my_drawing_position()
        {
            my_Position.X = my_grid_coord.x * 32;
            my_Position.Y = my_grid_coord.y * 32;
        }

        public void loot(Floor fl)
        {
            for (int i = 0; i < fl.show_me_the_money().Count; i++)
            {
                gridCoordinate moneyPos = fl.show_me_the_money()[i].get_my_grid_C();
                if (my_grid_coord.x == moneyPos.x && my_grid_coord.y == moneyPos.y)
                {
                    add_gold(fl.show_me_the_money()[i].my_quantity);
                    fl.show_me_the_money().RemoveAt(i);
                    break;
                }
            }
        }

        public void add_gold(int gold_amt)
        {
            my_gold += gold_amt;
        }

        public void take_damage(int dmg)
        {
            current_hp -= dmg;
        }

        public Vector2 get_my_Position()
        {
            return my_Position;
        }

        public gridCoordinate get_my_grid_C()
        {
            return my_grid_coord;
        }

        public bool is_monster_present(Floor fl, out int bad_guy_ID)
        {
            bad_guy_ID = -1;
            for (int i = 0; i < fl.see_badGuys().Count; i++)
                if (my_grid_coord.x == fl.see_badGuys()[i].my_grid_coord.x &&
                   my_grid_coord.y == fl.see_badGuys()[i].my_grid_coord.y)
                {
                    bad_guy_ID = i;
                    return true;
                }
            return false;
        }

        public bool is_spot_free(Floor fl)
        {
            int whoCares;
            return (fl.isWalkable(my_grid_coord) && is_monster_present(fl, out whoCares) == false);
        }

        public bool is_alive()
        {
            return current_hp > 0;
        }

        public bool is_spot_exit(Floor fl)
        {
            return fl.isExit(my_grid_coord);
        }

        public int unarmed_damage()
        {
            return rGen.Next(5, 10);
        }
    }
}
