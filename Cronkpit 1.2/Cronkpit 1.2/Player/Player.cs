﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Cronkpit_1._2
{
    class Player
    {
        public enum Chara_Class { Warrior };
        //Constructor stuff
        private Texture2D my_Texture;
        private Texture2D my_dead_texture;
        private Vector2 my_Position;
        private ContentManager cont;
        private gridCoordinate my_grid_coord;
        private Random rGen;
        List<string> message_buffer;
        //Player info stuff
        Chara_Class my_class;
        //Related to health.
        Limb Head;
        Limb Torso;
        Limb R_Arm;
        Limb L_Arm;
        Limb R_Leg;
        Limb L_Leg;
        //Equipped items
        Weapon main_hand;
        Weapon off_hand;
        Armor over_armor;
        Armor under_armor;
        //Inventory
        List<Item> inventory;
        //!Constructor stuff
        private int my_gold;
        //Sensory
        private int base_smell_value;
        private int base_sound_value;

        //Green text. Function here.
        public Player(ContentManager sCont, gridCoordinate sGridCoord, ref List<string> msgBuffer, Chara_Class myClass)
        {
            //Constructor stuff
            cont = sCont;
            my_grid_coord = new gridCoordinate(sGridCoord);
            my_Position = new Vector2(sGridCoord.x * 32, sGridCoord.y * 32);
            my_Texture = cont.Load<Texture2D>("Player/lmfaoplayer");
            my_dead_texture = cont.Load<Texture2D>("Player/playercorpse");
            rGen = new Random();
            message_buffer = msgBuffer;
            //!Constructor stuff
            my_gold = 0;
            base_smell_value = 10;
            base_sound_value = 10;
            //Player stuff
            my_class = myClass;
            //Health stuff.
            Head = new Limb(true, ref rGen);
            Torso = new Limb(false, ref rGen);
            R_Arm = new Limb(false, ref rGen);
            L_Arm = new Limb(false, ref rGen);
            R_Leg = new Limb(false, ref rGen);
            L_Leg = new Limb(false, ref rGen);
            //Inventory stuff
            main_hand = null;
            off_hand = null;
            over_armor = null;
            under_armor = null;
            inventory = new List<Item>();
        }

        //Voids here.
        //Green text. Function here.
        public void drawMe(ref SpriteBatch sb)
        {
            if (is_alive())
                sb.Draw(my_Texture, my_Position, Color.White);
            else
                sb.Draw(my_dead_texture, my_Position, Color.White);
        }

        //Green text. Function here.
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
            int my_smell = my_scent_value();
            int my_sound = my_sound_value();
            switch (numeric_direction)
            {
                //up, y-
                case 0:
                    my_grid_coord.y--;
                    if (is_spot_free(fl))
                    {
                        reset_my_drawing_position();
                    }
                    else if (is_monster_present(fl, out MonsterID))
                    {
                        int damage_val = unarmed_damage();
                        message_buffer.Add("You punch the " + fl.specific_badguy(MonsterID).my_name + 
                                            ", dealing " + damage_val + " damage!");
                        fl.damage_monster(damage_val, MonsterID);
                        my_grid_coord.y++;
                    }
                    else
                        my_grid_coord.y++;
                    break;
                //down, y+
                case 1:
                    my_grid_coord.y++;
                    if (is_spot_free(fl))
                    {
                        reset_my_drawing_position();
                    }
                    else if (is_monster_present(fl, out MonsterID))
                    {
                        int damage_val = unarmed_damage();
                        message_buffer.Add("You punch the " + fl.specific_badguy(MonsterID).my_name +
                                            ", dealing " + damage_val + " damage!");
                        fl.damage_monster(damage_val, MonsterID);
                        my_grid_coord.y--;
                    }
                    else
                        my_grid_coord.y--;
                    break;
                //left, x-
                case 2:
                    my_grid_coord.x--;
                    if (is_spot_free(fl))
                    {
                        reset_my_drawing_position();
                    }
                    else if (is_monster_present(fl, out MonsterID))
                    {
                        int damage_val = unarmed_damage();
                        message_buffer.Add("You punch the " + fl.specific_badguy(MonsterID).my_name +
                                            ", dealing " + damage_val + " damage!");
                        fl.damage_monster(damage_val, MonsterID);
                        my_grid_coord.x++;
                    }
                    else
                        my_grid_coord.x++;
                    break;
                //right, x+
                case 3:
                    my_grid_coord.x++;
                    if (is_spot_free(fl))
                    {
                        reset_my_drawing_position();
                    }
                    else if (is_monster_present(fl, out MonsterID))
                    {
                        int damage_val = unarmed_damage();
                        message_buffer.Add("You punch the " + fl.specific_badguy(MonsterID).my_name +
                                            ", dealing " + damage_val + " damage!");
                        fl.damage_monster(damage_val, MonsterID);
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
                        int damage_val = unarmed_damage();
                        message_buffer.Add("You punch the " + fl.specific_badguy(MonsterID).my_name +
                                            ", dealing " + damage_val + " damage!");
                        fl.damage_monster(damage_val, MonsterID);
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
                        int damage_val = unarmed_damage();
                        message_buffer.Add("You punch the " + fl.specific_badguy(MonsterID).my_name +
                                            ", dealing " + damage_val + " damage!");
                        fl.damage_monster(damage_val, MonsterID);
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
                        int damage_val = unarmed_damage();
                        message_buffer.Add("You punch the " + fl.specific_badguy(MonsterID).my_name +
                                            ", dealing " + damage_val + " damage!");
                        fl.damage_monster(damage_val, MonsterID);
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
                        int damage_val = unarmed_damage();
                        message_buffer.Add("You punch the " + fl.specific_badguy(MonsterID).my_name +
                                            ", dealing " + damage_val + " damage!");
                        fl.damage_monster(damage_val, MonsterID);
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
            //after moving, loot and then add smell to current tile.
            loot(fl);
            fl.add_smell_to_tile(my_grid_coord, 0, my_scent_value());
            fl.sound_pulse(my_grid_coord, my_sound, 0);
        }

        //Green text. Function here.
        public void reset_my_drawing_position()
        {
            my_Position.X = my_grid_coord.x * 32;
            my_Position.Y = my_grid_coord.y * 32;
        }

        //Green text. Function here.
        public void loot(Floor fl)
        {
            for (int i = 0; i < fl.show_me_the_money().Count; i++)
            {
                gridCoordinate moneyPos = fl.show_me_the_money()[i].get_my_grid_C();
                if (my_grid_coord.x == moneyPos.x && my_grid_coord.y == moneyPos.y)
                {
                    int gold_val = fl.show_me_the_money()[i].my_quantity;
                    add_gold(gold_val);
                    message_buffer.Add("You loot " + gold_val + " gold!");
                    fl.show_me_the_money().RemoveAt(i);
                    break;
                }
            }
        }

        //Green text. Function here.
        public void add_gold(int gold_amt)
        {
            my_gold += gold_amt;
        }

        public void pay_gold(int gold_amt)
        {
            my_gold -= gold_amt;
        }

        public void acquire_item(Item thing)
        {
            inventory.Add(thing);
        }

        //Green text. Function here.
        public void take_damage(wound dmg)
        {
            //OKAY THIS IS GONNA BE COMPLICATED.
            //First, figure out where the attack is gonna hit. The breakdown is as follows:
            //head, 5%, chest = 25%, arm = 17%, leg = 18%
            int hit_location = rGen.Next(100);
            if (hit_location < 5 && !Head.is_disabled())
                Head.add_injury(dmg);
            else if (hit_location >= 5 && hit_location < 22 && !R_Arm.is_disabled())
                R_Arm.add_injury(dmg);
            else if (hit_location >= 22 && hit_location < 39 && !L_Arm.is_disabled())
                L_Arm.add_injury(dmg);
            else if (hit_location >= 39 && hit_location < 57 && !R_Leg.is_disabled())
                R_Leg.add_injury(dmg);
            else if (hit_location >= 57 && hit_location < 75 && !L_Leg.is_disabled())
                L_Leg.add_injury(dmg);
            else
                Torso.add_injury(dmg);

            if (!is_alive())
                message_buffer.Add("Your wounds are too much for you. You collapse and your vision fades.");
        }

        public void teleport(gridCoordinate gc)
        {
            my_grid_coord = new gridCoordinate(gc);
            reset_my_drawing_position();
        }

        public void heal_naturally()
        {
            Head.heal_random_wound();
            L_Arm.heal_random_wound();
            L_Leg.heal_random_wound();
            R_Arm.heal_random_wound();
            R_Leg.heal_random_wound();
            Torso.heal_random_wound();
        }

        //Some other stuff - grid coordinates and vectors.
        //Green text. Function here.
        public Vector2 get_my_Position()
        {
            return my_Position;
        }

        //Green text. Function here.
        public gridCoordinate get_my_grid_C()
        {
            return my_grid_coord;
        }

        //Bool returns - gets whether there's a monster on your current grid coordinate.
        //Or whether the spot is free, or if you're alive, OR if that spot is an exit.
        //Green text. Function here.
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

        //Green text. Function here.
        public bool is_spot_free(Floor fl)
        {
            int whoCares;
            return (fl.isWalkable(my_grid_coord) && is_monster_present(fl, out whoCares) == false);
        }

        //Green text. Function here.
        public bool is_alive()
        {
            return !Head.is_disabled() && !Torso.is_disabled();
        }

        //Green text. Function here.
        public bool is_spot_exit(Floor fl)
        {
            return fl.isExit(my_grid_coord);
        }

        //Int returns - damage value + scent values are here.
        //Green text. Function here.
        public int unarmed_damage()
        {
            return rGen.Next(1, 4);
        }

        public int get_my_gold()
        {
            return my_gold;
        }

        //Green text. Function here.
        public int my_scent_value()
        {
            return base_smell_value; //+1/2 armor + wounds
        }

        public int my_sound_value()
        {
            return base_sound_value;
        }

        //Wounding stuff
        public void wound_report(out int h_wounds, out int t_wounds, out int ra_wounds, out int la_wounds, out int ll_wounds, out int rl_wounds)
        {
            h_wounds = Head.count_debilitating_injuries();
            t_wounds = Torso.count_debilitating_injuries();
            ra_wounds = R_Arm.count_debilitating_injuries();
            la_wounds = L_Arm.count_debilitating_injuries();
            ll_wounds = L_Leg.count_debilitating_injuries();
            rl_wounds = R_Leg.count_debilitating_injuries();
        }

        public List<string> detailed_wound_report()
        {
            List<string> wRep = new List<string>();

            //Head
            wRep.Add("Your head has:");
            wound_by_section(Head, ref wRep);
            //Torso
            wRep.Add("Your chest has:");
            wound_by_section(Torso, ref wRep);
            //Left Arm
            wRep.Add("Your left arm has:");
            wound_by_section(L_Arm, ref wRep);
            //Right Arm
            wRep.Add("Your right arm has:");
            wound_by_section(R_Arm, ref wRep);
            //Left Leg
            wRep.Add("Your left leg has:");
            wound_by_section(L_Leg, ref wRep);
            //Right leg
            wRep.Add("Your right leg has:");
            wound_by_section(R_Leg, ref wRep);

            return wRep;
        }

        public void wound_by_section(Limb bodyPart, ref List<string> wReport)
        {
            if (bodyPart.is_uninjured())
                wReport.Add(" - No injuries");
            bodyPart.consolidate_injury_report(ref wReport);
            if(bodyPart.is_disabled())
                wReport.Add("It is useless");
        }
    }
}
