using System;
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
        public enum Equip_Slot { Mainhand, Offhand, Overarmor, Underarmor };
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
            my_gold = 9000;
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
            main_hand = new Weapon(0, 100, "Knife", Weapon.Type.Sword, 1, 2, 4, 1);
            off_hand = null;
            over_armor = new Armor(0, 100, "Shoddy Leather", 0, 1, 2, 1, 1, 3, true);
            under_armor = new Armor(0, 100, "Linen Rags", 0, 2, 2, 0, 0, 2, false);
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
            int MonsterID = -1;
            int my_smell = my_scent_value();
            int my_sound = my_sound_value();
            //damage stuff
            int damage_val = deal_damage();

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

            if (MonsterID != -1)
            {
                attack(fl, my_grid_coord, fl.badguy_by_monster_id(MonsterID).my_grid_coord);
            }
            //after moving, loot and then add smell to current tile.
            loot(fl);
            fl.add_smell_to_tile(my_grid_coord, 0, my_scent_value());
            fl.sound_pulse(my_grid_coord, my_sound, 0);
        }

        #region attack and related functions

        public void attack(Floor fl, gridCoordinate pl_gc, gridCoordinate monster_gc)
        {
            List<gridCoordinate> squares_to_attack_mh = new List<gridCoordinate>();
            List<gridCoordinate> squares_to_attack_oh = new List<gridCoordinate>();
            List<gridCoordinate> squares_to_attack_both = new List<gridCoordinate>();

            //Monster GC is the origin - set up all patterns from there.
            if (main_hand != null)
            {
                switch (main_hand.get_my_weapon_type())
                {
                    case Weapon.Type.Sword:
                        squares_to_attack_mh.Add(new gridCoordinate(monster_gc));
                        break;
                    case Weapon.Type.Spear:
                        squares_to_attack_mh = return_spear_patterns(pl_gc, monster_gc, squares_to_attack_mh, fl, true);
                        break;
                    //Axe!
                    case Weapon.Type.Axe:
                        squares_to_attack_mh = return_axe_patterns(pl_gc, monster_gc, squares_to_attack_mh);
                        break;
                }
            }

            if (off_hand != null)
            {
                switch (off_hand.get_my_weapon_type())
                {
                    case Weapon.Type.Sword:
                        squares_to_attack_oh.Add(new gridCoordinate(monster_gc));
                        break;
                    case Weapon.Type.Spear:
                        squares_to_attack_oh = return_spear_patterns(pl_gc, monster_gc, squares_to_attack_oh, fl, false);
                        break;
                    case Weapon.Type.Axe:
                        squares_to_attack_oh = return_axe_patterns(pl_gc, monster_gc, squares_to_attack_oh);
                        break;
                }
            }

            //then get rid of common elements from both lists and put them into the new list.
            for (int i = 0; i < squares_to_attack_mh.Count; i++)
                for (int j = 0; j < squares_to_attack_oh.Count; j++) 
                    if (squares_to_attack_mh[i].x == squares_to_attack_oh[j].x &&
                        squares_to_attack_mh[i].y == squares_to_attack_oh[j].y)
                        squares_to_attack_both.Add(new gridCoordinate(squares_to_attack_mh[i]));

            //Then we trim these elements out of the old lists.
            remove_duplicate_elements(ref squares_to_attack_both, ref squares_to_attack_mh);
            remove_duplicate_elements(ref squares_to_attack_both, ref squares_to_attack_oh);

            //if both weapons are null...
            if (main_hand == null && off_hand == null)
            {
                squares_to_attack_mh.Add(new gridCoordinate(monster_gc));
            }

            //then we go through all 3 lists.
            for (int i = 0; i < squares_to_attack_mh.Count; i++)
            {
                int c_monsterID;
                if (fl.is_monster_here(squares_to_attack_mh[i], out c_monsterID))
                {
                    string w_name = "";
                    int dmg_val = 0;

                    if (main_hand != null)
                    {
                        dmg_val = main_hand.damage(ref rGen);
                        w_name = main_hand.get_my_name();
                    }
                    else
                    {
                        dmg_val = deal_damage();
                        w_name = "fists";
                    }

                    attack_monster_in_grid(fl, dmg_val, c_monsterID, squares_to_attack_mh[i], w_name);
                }
                if (main_hand != null)
                    fl.add_effect(main_hand.get_my_damage_type(), squares_to_attack_mh[i]);
                else
                    fl.add_effect(Attack.Damage.Crushing, squares_to_attack_mh[i]);
            }

            for (int i = 0; i < squares_to_attack_oh.Count; i++)
            {
                int c_monsterID;
                if (fl.is_monster_here(squares_to_attack_oh[i], out c_monsterID))
                {
                    string w_name = off_hand.get_my_name();
                    int dmg_val = off_hand.damage(ref rGen);

                    attack_monster_in_grid(fl, dmg_val, c_monsterID, squares_to_attack_oh[i], w_name);
                }
                fl.add_effect(off_hand.get_my_damage_type(), squares_to_attack_oh[i]);
            }

            for (int i = 0; i < squares_to_attack_both.Count; i++)
            {
                int c_monsterID;
                if (fl.is_monster_here(squares_to_attack_both[i], out c_monsterID))
                {
                    string w_name = "";
                    if (off_hand == main_hand)
                        w_name = main_hand.get_my_name();
                    else
                        w_name = main_hand.get_my_name() + " and your " + off_hand.get_my_name();
                    int dmg_val = main_hand.damage(ref rGen) + off_hand.damage(ref rGen);
                    attack_monster_in_grid(fl, dmg_val, c_monsterID, squares_to_attack_both[i], w_name);
                }
                if (off_hand == main_hand)
                    fl.add_effect(main_hand.get_my_damage_type(), squares_to_attack_both[i]);
                else
                {
                    fl.add_effect(main_hand.get_my_damage_type(), squares_to_attack_both[i]);
                    fl.add_effect(off_hand.get_my_damage_type(), squares_to_attack_both[i]);
                }
            }
        }

        public List<gridCoordinate> return_axe_patterns(gridCoordinate pl_gc, gridCoordinate monster_gc, List<gridCoordinate> coord_list)
        {
            int x_difference = monster_gc.x - pl_gc.x;
            int y_difference = monster_gc.y - pl_gc.y;

            if (x_difference == -1)
            {
                if (y_difference == -1)
                {
                    coord_list.Add(new gridCoordinate(monster_gc.x + 1, monster_gc.y));
                    coord_list.Add(new gridCoordinate(monster_gc.x, monster_gc.y + 1));
                }
                else if (y_difference == 0)
                {
                    coord_list.Add(new gridCoordinate(monster_gc.x, monster_gc.y - 1));
                    coord_list.Add(new gridCoordinate(monster_gc.x, monster_gc.y + 1));
                }
                else if (y_difference == 1)
                {
                    coord_list.Add(new gridCoordinate(monster_gc.x + 1, monster_gc.y));
                    coord_list.Add(new gridCoordinate(monster_gc.x, monster_gc.y - 1));
                }
            }
            else if (x_difference == 1)
            {
                if (y_difference == 1)
                {
                    coord_list.Add(new gridCoordinate(monster_gc.x - 1, monster_gc.y));
                    coord_list.Add(new gridCoordinate(monster_gc.x, monster_gc.y - 1));
                }
                else if (y_difference == 0)
                {
                    coord_list.Add(new gridCoordinate(monster_gc.x, monster_gc.y - 1));
                    coord_list.Add(new gridCoordinate(monster_gc.x, monster_gc.y + 1));
                }
                if (y_difference == -1)
                {
                    coord_list.Add(new gridCoordinate(monster_gc.x - 1, monster_gc.y));
                    coord_list.Add(new gridCoordinate(monster_gc.x, monster_gc.y + 1));
                }
            }
            else if (x_difference == 0)
            {
                coord_list.Add(new gridCoordinate(monster_gc.x - 1, monster_gc.y));
                coord_list.Add(new gridCoordinate(monster_gc.x + 1, monster_gc.y));
            }

            coord_list.Add(new gridCoordinate(monster_gc));
            return coord_list;
        }

        public List<gridCoordinate> return_spear_patterns(gridCoordinate pl_gc, gridCoordinate monster_gc, List<gridCoordinate> coord_list, Floor fl, bool mainHand)
        {
            int x_difference = monster_gc.x - pl_gc.x;
            int y_difference = monster_gc.y - pl_gc.y;

            int spear_range;
            if (mainHand)
            {
                if (x_difference == 0 || y_difference == 0)
                    spear_range = main_hand.get_my_range();
                else
                    spear_range = Math.Max(1, (main_hand.get_my_range()-1));
            }
            else
            {
                if (x_difference == 0 || y_difference == 0)
                    spear_range = off_hand.get_my_range();
                else
                    spear_range = Math.Max(1, (off_hand.get_my_range()-1));
            }

            bool blocked = false;
            for (int i = 0; i < spear_range; i++)
            {
                gridCoordinate square_to_attack = new gridCoordinate(pl_gc.x + (x_difference*(i+1)), pl_gc.y + (y_difference*(i+1)));
                if(fl.isWalkable(square_to_attack) && !blocked)
                    coord_list.Add(square_to_attack);
                else
                    blocked = true;
            }
            
            return coord_list;
        }

        public void remove_duplicate_elements(ref List<gridCoordinate> L1, ref List<gridCoordinate> L2)
        {
            for (int i = 0; i < L1.Count; i++)
                for (int j = 0; j < L2.Count; j++)
                    if (L1[i].x == L2[j].x && L1[i].y == L2[j].y)
                        L2.RemoveAt(j);
        }

        public void attack_monster_in_grid(Floor fl, int dmg_val, int c_monsterID, gridCoordinate current_gc, string weapon_name)
        {
            string attack_msg = "You attack the " + fl.badguy_by_monster_id(c_monsterID).my_name + " with your " + weapon_name;
            attack_msg += ", doing " + dmg_val + " damage!";
            message_buffer.Add(attack_msg);
            fl.add_new_popup("-" + dmg_val, Popup.popup_msg_color.Red, current_gc);
            fl.damage_monster(dmg_val, c_monsterID);
        }

        #endregion

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
                    fl.add_new_popup("+ " + gold_val + " gold!", Popup.popup_msg_color.Yellow, my_grid_coord);
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
        public void take_damage(Attack atk, ref Floor fl)
        {
            //OKAY THIS IS GONNA BE COMPLICATED.
            //First, figure out where the attack is gonna hit. The breakdown is as follows:
            //head, 5%, chest = 25%, arm = 17%, leg = 18%
            int hit_location = rGen.Next(100);

            string w_type = "";
            switch (atk.get_assoc_wound().type)
            {
                case wound.Wound_Type.Burn:
                    w_type = "burn";
                    break;
                case wound.Wound_Type.Impact:
                    w_type = "impact";
                    break;
                case wound.Wound_Type.Open:
                    w_type = "open";
                    break;
            }

            if (hit_location < 5 && !Head.is_disabled())
            {
                wound dmg = new wound(atk.get_assoc_wound());
                Head.add_injury(dmg);
                if (dmg.severity > 0)
                    fl.add_new_popup("-" + dmg.severity + " Head", Popup.popup_msg_color.Red, my_grid_coord);
                message_buffer.Add("Your head takes " + dmg.severity + " " + w_type + " wounds!");
            }
            else if (hit_location >= 5 && hit_location < 22 && !R_Arm.is_disabled())
            {
                Armor.Attack_Zone atkzone = Armor.Attack_Zone.R_Arm;
                if (over_armor != null)
                    atk = over_armor.absorb_damage(atk, atkzone, my_grid_coord, ref rGen, ref message_buffer, ref fl);
                if(under_armor != null)
                    atk = under_armor.absorb_damage(atk, atkzone, my_grid_coord, ref rGen, ref message_buffer, ref fl);
                wound dmg = new wound(atk.get_assoc_wound());
                R_Arm.add_injury(dmg);
                if (dmg.severity > 0)
                    fl.add_new_popup("-" + dmg.severity + " R Arm", Popup.popup_msg_color.Red, my_grid_coord);
                message_buffer.Add("Your right arm takes " + dmg.severity + " " + w_type + " wounds!");
            }
            else if (hit_location >= 22 && hit_location < 39 && !L_Arm.is_disabled())
            {
                Armor.Attack_Zone atkzone = Armor.Attack_Zone.L_Arm;
                if(over_armor != null)
                    atk = over_armor.absorb_damage(atk, atkzone, my_grid_coord, ref rGen, ref message_buffer, ref fl);
                if(under_armor != null)
                    atk = under_armor.absorb_damage(atk, atkzone, my_grid_coord, ref rGen, ref message_buffer, ref fl);
                wound dmg = new wound(atk.get_assoc_wound());
                L_Arm.add_injury(dmg);
                if (dmg.severity > 0)
                    fl.add_new_popup("-" + dmg.severity + " L Arm", Popup.popup_msg_color.Red, my_grid_coord);
                message_buffer.Add("Your left arm takes " + dmg.severity + " " + w_type + " wounds!");
            }
            else if (hit_location >= 39 && hit_location < 57 && !R_Leg.is_disabled())
            {
                Armor.Attack_Zone atkzone = Armor.Attack_Zone.R_Leg;
                if (over_armor != null)
                    atk = over_armor.absorb_damage(atk, atkzone, my_grid_coord, ref rGen, ref message_buffer, ref fl);
                if (under_armor != null)
                    atk = under_armor.absorb_damage(atk, atkzone, my_grid_coord, ref rGen, ref message_buffer, ref fl);
                wound dmg = new wound(atk.get_assoc_wound());
                R_Leg.add_injury(dmg);
                if (dmg.severity > 0)
                    fl.add_new_popup("-" + dmg.severity + " R Leg", Popup.popup_msg_color.Red, my_grid_coord);
                message_buffer.Add("Your right leg takes " + dmg.severity + " " + w_type + " wounds!");
            }
            else if (hit_location >= 57 && hit_location < 75 && !L_Leg.is_disabled())
            {
                Armor.Attack_Zone atkzone = Armor.Attack_Zone.L_Leg;
                if (over_armor != null)
                    atk = over_armor.absorb_damage(atk, atkzone, my_grid_coord, ref rGen, ref message_buffer, ref fl);
                if (under_armor != null)
                    atk = under_armor.absorb_damage(atk, atkzone, my_grid_coord, ref rGen, ref message_buffer, ref fl);
                wound dmg = new wound(atk.get_assoc_wound());
                L_Leg.add_injury(dmg);
                if (dmg.severity > 0)
                    fl.add_new_popup("-" + dmg.severity + " L Leg", Popup.popup_msg_color.Red, my_grid_coord);
                message_buffer.Add("Your left leg takes " + dmg.severity + " " + w_type + " wounds!");
            }
            else
            {
                Armor.Attack_Zone atkzone = Armor.Attack_Zone.Chest;
                if (over_armor != null)
                    atk = over_armor.absorb_damage(atk, atkzone, my_grid_coord, ref rGen, ref message_buffer, ref fl);
                if (under_armor != null)
                    atk = under_armor.absorb_damage(atk, atkzone, my_grid_coord, ref rGen, ref message_buffer, ref fl);
                wound dmg = new wound(atk.get_assoc_wound());
                Torso.add_injury(dmg);
                if (dmg.severity > 0)
                    fl.add_new_popup("-" + dmg.severity + " Chest", Popup.popup_msg_color.Red, my_grid_coord);
                message_buffer.Add("Your chest takes " + dmg.severity + " " + w_type + " wounds!");
            }

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

        public void repair_all_armor()
        {
            if (over_armor != null)
                over_armor.full_repair();
            if (under_armor != null)
                under_armor.full_repair();
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
                    bad_guy_ID = fl.see_badGuys()[i].my_Index;
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
        public int deal_damage()
        {
            //If there's a mainhand and an offhand
            if (main_hand != null && off_hand != null)
                return main_hand.damage(ref rGen) + off_hand.damage(ref rGen);
            //If there's a mainhand and no offhand
            else if (main_hand != null && off_hand == null)
                return main_hand.damage(ref rGen);
            //If there's an offhand and no mainhand
            else if(main_hand == null && off_hand != null)
                return off_hand.damage(ref rGen);
            else
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

        //Inventory stuff
        public int calc_absorb_chance(int primary_resist, int secondary_resist)
        {
            return ((4 * primary_resist) + (2 * secondary_resist));
        }

        public List<string> detailed_equip_report()
        {
            List<string> eRep = new List<string>();

            int oa_ab_val;
            int oa_in_val;
            int oa_pa_val;
            int oa_ha_val;
            int oa_rg_val;

            int oa_chest_integ;
            int oa_rarm_integ;
            int oa_larm_integ;
            int oa_rleg_integ;
            int oa_lleg_integ;

            if (over_armor != null)
            {
                oa_ab_val = over_armor.get_ab_val();
                oa_in_val = over_armor.get_ins_val();
                oa_pa_val = over_armor.get_pad_val();
                oa_ha_val = over_armor.get_hard_val();
                oa_rg_val = over_armor.get_rigid_val();

                oa_chest_integ = over_armor.get_chest_integ();
                oa_rarm_integ = over_armor.get_rarm_integ();
                oa_larm_integ = over_armor.get_larm_integ();
                oa_rleg_integ = over_armor.get_rleg_integ();
                oa_lleg_integ = over_armor.get_lleg_integ();

                eRep.Add("Over Armor: " + over_armor.get_my_name());
                eRep.Add(" ");
                eRep.Add("Protective values:");
                eRep.Add("Ablative: " + oa_ab_val);
                eRep.Add("Insulation: " + oa_in_val);
                eRep.Add("Padding: " + oa_pa_val);
                eRep.Add("Hardness: " + oa_ha_val);
                eRep.Add("Rigidity: " + oa_rg_val);
                eRep.Add(" ");
                eRep.Add("Chance to absorb slashing attack: " + calc_absorb_chance(oa_ha_val, oa_rg_val) + "%");
                eRep.Add("Chance to absorb crushing attack: " + calc_absorb_chance(oa_rg_val, oa_pa_val) + "%");
                eRep.Add("Chance to absorb piercing attack: " + calc_absorb_chance(oa_ha_val, oa_pa_val) + "%");
                eRep.Add("Chance to absorb fire attack: " + calc_absorb_chance(oa_ab_val, oa_rg_val) + "%");
                eRep.Add(" ");
                eRep.Add("Integrity:");
                eRep.Add("Chest integrity: " + oa_chest_integ);
                eRep.Add("L Arm integrity: " + oa_larm_integ + " / R Arm integrity: " + oa_rarm_integ);
                eRep.Add("L Leg integrity: " + oa_lleg_integ + " / R Leg integrity: " + oa_rleg_integ);
                eRep.Add(" ");
            }

            int ua_ab_val;
            int ua_in_val;
            int ua_pa_val;
            int ua_ha_val;
            int ua_rg_val;

            int ua_chest_integ;
            int ua_rarm_integ;
            int ua_larm_integ;
            int ua_rleg_integ;
            int ua_lleg_integ;

            if (under_armor != null)
            {
                ua_ab_val = under_armor.get_ab_val();
                ua_in_val = under_armor.get_ins_val();
                ua_pa_val = under_armor.get_pad_val();
                ua_ha_val = under_armor.get_hard_val();
                ua_rg_val = under_armor.get_rigid_val();

                ua_chest_integ = under_armor.get_chest_integ();
                ua_rarm_integ = under_armor.get_rarm_integ();
                ua_larm_integ = under_armor.get_larm_integ();
                ua_rleg_integ = under_armor.get_rleg_integ();
                ua_lleg_integ = under_armor.get_lleg_integ();

                eRep.Add("Under Armor: " + under_armor.get_my_name());
                eRep.Add(" ");
                eRep.Add("Protective values:");
                eRep.Add("Ablative: " + ua_ab_val);
                eRep.Add("Insulation: " + ua_in_val);
                eRep.Add("Padding: " + ua_pa_val);
                eRep.Add("Hardness: " + ua_ha_val);
                eRep.Add("Rigidity: " + ua_rg_val);
                eRep.Add(" ");
                eRep.Add("Chance to absorb slashing attack: " + calc_absorb_chance(ua_ha_val, ua_rg_val) + "%");
                eRep.Add("Chance to absorb crushing attack: " + calc_absorb_chance(ua_rg_val, ua_pa_val) + "%");
                eRep.Add("Chance to absorb piercing attack: " + calc_absorb_chance(ua_ha_val, ua_pa_val) + "%");
                eRep.Add("Chance to absorb fire attack: " + calc_absorb_chance(ua_ab_val, ua_rg_val) + "%");
                eRep.Add(" ");
                eRep.Add("Integrity:");
                eRep.Add("Chest integrity: " + ua_chest_integ);
                eRep.Add("L Arm integrity: " + ua_larm_integ + " / R Arm integrity: " + ua_rarm_integ);
                eRep.Add("L Leg integrity: " + ua_lleg_integ + " / R Leg integrity: " + ua_rleg_integ);
                eRep.Add(" ");
            }

            return eRep;
        }

        public List<Item> retrieve_inventory()
        {
            return inventory;
        }

        public void equip_main_hand(Weapon mh)
        {
            if(main_hand != null)
                inventory.Add(main_hand);
            if (mh.get_hand_count() == 2)
                if (off_hand != null)
                    inventory.Add(off_hand);
            //Okay, we've un-equipped both weapons.
            
            //Now, check how many hands our current weapon has.
            int hnd_cnt = 0;
            if(main_hand != null)
                hnd_cnt = main_hand.get_hand_count();
            main_hand = mh;
            if (main_hand.get_hand_count() == 1 && hnd_cnt == 2)
                off_hand = null;
            if (main_hand.get_hand_count() == 2)
                off_hand = mh;
        }

        public void equip_off_hand(Weapon oh)
        {
            if (off_hand != null)
                inventory.Add(off_hand);
            if (oh.get_hand_count() == 2)
                if (main_hand != null)
                    inventory.Add(main_hand);

            int hnd_cnt = 0;
            if(off_hand != null)
                hnd_cnt = off_hand.get_hand_count();
            off_hand = oh;
            if (off_hand.get_hand_count() == 1 && hnd_cnt == 2)
                main_hand = null;
            if (off_hand.get_hand_count() == 2)
                main_hand = oh;
        }

        public void equip_over_armor(Armor oa)
        {
            if (over_armor != null)
                inventory.Add(over_armor);

            over_armor = oa;
        }

        public void equip_under_armor(Armor ua)
        {
            if (under_armor != null)
                inventory.Add(under_armor);

            under_armor = ua;
        }

        public Weapon show_main_hand()
        {
            return main_hand;
        }

        public Weapon show_off_hand()
        {
            return off_hand;
        }

        public Armor show_over_armor()
        {
            return over_armor;
        }

        public Armor show_under_armor()
        {
            return under_armor;
        }

        public void unequip(Equip_Slot slot)
        {
            int mh_handcount = 0;
            if (main_hand != null)
                mh_handcount = main_hand.get_hand_count();
            int oh_handcount = 0;
            if(off_hand != null)
                oh_handcount = off_hand.get_hand_count();
            switch (slot)
            {
                case Equip_Slot.Mainhand:
                    main_hand = null;
                    if (mh_handcount == 2)
                        off_hand = null;
                    break;
                case Equip_Slot.Offhand:
                    off_hand = null;
                    if (mh_handcount == 2)
                        main_hand = null;
                    break;
                case Equip_Slot.Underarmor:
                    under_armor = null;
                    break;
                case Equip_Slot.Overarmor:
                    over_armor = null;
                    break;
            }
        }
    }
}
