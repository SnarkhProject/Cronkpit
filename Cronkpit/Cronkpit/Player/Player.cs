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
    class Player
    {
        public enum Character { Falsael };
        public enum Chara_Class { Warrior, Mage, Rogue };
        public enum Equip_Slot { Mainhand, Offhand, Overarmor, Underarmor };
        //Constructor stuff
        private Texture2D my_Texture;
        private Texture2D my_dead_texture;
        private Vector2 my_Position;
        private ContentManager cont;
        private gridCoordinate my_grid_coord;
        private Random rGen;
        List<string> message_buffer;
        PaperDoll pDoll;
        //Player info stuff
        Chara_Class my_class;
        Character my_character;
        //Related to health.
        Limb Head;
        Limb Torso;
        Limb R_Arm;
        Limb L_Arm;
        Limb R_Leg;
        Limb L_Leg;
        int dodge_chance;
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
        public int total_sound;
        public int total_scent;

        int standard_wpn_cooldown = 6;

        //Green text. Function here.
        public Player(ContentManager sCont, gridCoordinate sGridCoord, ref List<string> msgBuffer, 
                      Chara_Class myClass, Character myChara, ref PaperDoll pd)
        {
            //Constructor stuff
            cont = sCont;
            my_grid_coord = new gridCoordinate(sGridCoord);
            my_Position = new Vector2(sGridCoord.x * 32, sGridCoord.y * 32);
            rGen = new Random();
            message_buffer = msgBuffer;
            //!Constructor stuff
            my_gold = 9000;
            base_smell_value = 10;
            base_sound_value = 10;
            //Player stuff
            my_class = myClass;
            my_character = myChara;

            switch (my_character)
            {
                case Character.Falsael:
                    my_Texture = cont.Load<Texture2D>("Player/falsael_sprite");
                    my_dead_texture = cont.Load<Texture2D>("Player/playercorpse");
                    break;
                default:
                    my_Texture = cont.Load<Texture2D>("Player/lmfaoplayer");
                    my_dead_texture = cont.Load<Texture2D>("Player/playercorpse");
                    break;
            }
            //Health stuff.
            Head = new Limb(true, ref rGen);
            Torso = new Limb(false, ref rGen);
            R_Arm = new Limb(false, ref rGen);
            L_Arm = new Limb(false, ref rGen);
            R_Leg = new Limb(false, ref rGen);
            L_Leg = new Limb(false, ref rGen);
            calculate_dodge_chance();
            //Inventory stuff
            main_hand = new Weapon(0, 100, "Knife", Weapon.Type.Sword, 1, 2, 4, 1);
            off_hand = null;
            over_armor = new Armor(1, 100, "Shoddy Leather", 0, 1, 2, 1, 1, 3, true);
            under_armor = new Armor(2, 100, "Linen Rags", 0, 2, 2, 0, 0, 2, false);
            inventory = new List<Item>();
            //Character stuff

            pDoll = pd;
        }

        public string my_chara_as_string()
        {
            switch (my_character)
            {
                case Character.Falsael:
                    return "Falsael";
            }

            return "Default";
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
            int DoodadID = -1;
            int my_smell = my_scent_value();
            int my_sound = my_sound_value();
            //damage stuff

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
                    else if(fl.is_destroyable_doodad_here(my_grid_coord, out DoodadID))
                    {
                        my_grid_coord.y++;
                    }
                    else
                    {
                        my_grid_coord.y++;
                    }
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
                    else if(fl.is_destroyable_doodad_here(my_grid_coord, out DoodadID))
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
                    else if(fl.is_destroyable_doodad_here(my_grid_coord, out DoodadID))
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
                    else if(fl.is_destroyable_doodad_here(my_grid_coord, out DoodadID))
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
                    else if(fl.is_destroyable_doodad_here(my_grid_coord, out DoodadID))
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
                    else if(fl.is_destroyable_doodad_here(my_grid_coord, out DoodadID))
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
                    else if(fl.is_destroyable_doodad_here(my_grid_coord, out DoodadID))
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
                    else if(fl.is_destroyable_doodad_here(my_grid_coord, out DoodadID))
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
                melee_attack(fl, my_grid_coord, fl.badguy_by_monster_id(MonsterID).my_grid_coord);
            }
            if(DoodadID != -1)
            {
                melee_attack(fl, my_grid_coord, fl.doodad_by_index(DoodadID).get_g_coord());
            }
            //after moving, loot and then add smell to current tile.
            loot(fl);
            total_sound = my_sound_value();
            total_scent = my_scent_value();
        }

        #region attack and related functions

        public void melee_attack(Floor fl, gridCoordinate pl_gc, gridCoordinate monster_gc)
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
                    case Weapon.Type.Mace:
                        squares_to_attack_mh.Add(new gridCoordinate(monster_gc));
                        break;
                    case Weapon.Type.Spear:
                        squares_to_attack_mh = return_spear_patterns(pl_gc, monster_gc, squares_to_attack_mh, fl, true);
                        break;
                    //Axe!
                    case Weapon.Type.Axe:
                        squares_to_attack_mh = return_axe_patterns(pl_gc, monster_gc, squares_to_attack_mh, fl);
                        break;
                    case Weapon.Type.Lance:
                        squares_to_attack_mh.Add(new gridCoordinate(monster_gc));
                        break;
                }
            }

            if (off_hand != null)
            {
                switch (off_hand.get_my_weapon_type())
                {
                    case Weapon.Type.Sword:
                    case Weapon.Type.Mace:
                        squares_to_attack_oh.Add(new gridCoordinate(monster_gc));
                        break;
                    case Weapon.Type.Spear:
                        squares_to_attack_oh = return_spear_patterns(pl_gc, monster_gc, squares_to_attack_oh, fl, false);
                        break;
                    case Weapon.Type.Axe:
                        squares_to_attack_oh = return_axe_patterns(pl_gc, monster_gc, squares_to_attack_oh, fl);
                        break;
                    case Weapon.Type.Lance:
                        squares_to_attack_oh.Add(new gridCoordinate(monster_gc));
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
                int c_doodadID;

                string w_name = "";
                int dmg_val = 0;

                if (main_hand != null)
                {
                    dmg_val = main_hand.damage(ref rGen);
                    w_name = main_hand.get_my_name();
                }
                else
                {
                    dmg_val = rGen.Next(1, 4);
                    w_name = "fists";
                }

                //Damage penalty of 50% for lances.
                if (main_hand != null && main_hand.get_my_weapon_type() == Weapon.Type.Lance)
                    dmg_val /= 4;

                if (fl.is_monster_here(squares_to_attack_mh[i], out c_monsterID))
                    attack_monster_in_grid(fl, dmg_val, c_monsterID, squares_to_attack_mh[i], w_name, true);

                if (fl.is_destroyable_doodad_here(squares_to_attack_mh[i], out c_doodadID))
                    attack_doodad_in_grid(fl, dmg_val, c_doodadID, squares_to_attack_mh[i], w_name);

                if (main_hand != null)
                    fl.add_effect(main_hand.get_my_damage_type(), squares_to_attack_mh[i]);
                else
                    fl.add_effect(Attack.Damage.Crushing, squares_to_attack_mh[i]);
            }

            for (int i = 0; i < squares_to_attack_oh.Count; i++)
            {
                int c_monsterID;
                int c_doodadID;

                string w_name = off_hand.get_my_name();
                int dmg_val = off_hand.damage(ref rGen);

                //Damage penalty of 50% for lances.
                if (off_hand.get_my_weapon_type() == Weapon.Type.Lance)
                    dmg_val /= 4;

                if (fl.is_monster_here(squares_to_attack_oh[i], out c_monsterID))
                    attack_monster_in_grid(fl, dmg_val, c_monsterID, squares_to_attack_oh[i], w_name, true);

                if (fl.is_destroyable_doodad_here(squares_to_attack_oh[i], out c_doodadID))
                    attack_doodad_in_grid(fl, dmg_val, c_doodadID, squares_to_attack_oh[i], w_name);

                fl.add_effect(off_hand.get_my_damage_type(), squares_to_attack_oh[i]);
            }

            for (int i = 0; i < squares_to_attack_both.Count; i++)
            {
                int c_monsterID;
                int c_doodadID;

                string w_name = "";

                if (off_hand == main_hand)
                    w_name = main_hand.get_my_name();
                else
                    w_name = main_hand.get_my_name() + " and your " + off_hand.get_my_name();

                //Both of these have a 50% damage penalty for lances.
                int mh_dmg = main_hand.damage(ref rGen);
                if (main_hand.get_my_weapon_type() == Weapon.Type.Lance)
                    mh_dmg /= 4;

                int oh_dmg = off_hand.damage(ref rGen);
                if (off_hand.get_my_weapon_type() == Weapon.Type.Lance)
                    oh_dmg /= 4;

                int dmg_val = mh_dmg + oh_dmg;

                if (fl.is_monster_here(squares_to_attack_both[i], out c_monsterID))
                    attack_monster_in_grid(fl, dmg_val, c_monsterID, squares_to_attack_both[i], w_name, true);

                if (fl.is_destroyable_doodad_here(squares_to_attack_both[i], out c_doodadID))
                    attack_doodad_in_grid(fl, dmg_val, c_doodadID, squares_to_attack_both[i], w_name);

                if (off_hand == main_hand)
                    fl.add_effect(main_hand.get_my_damage_type(), squares_to_attack_both[i]);
                else
                {
                    fl.add_effect(main_hand.get_my_damage_type(), squares_to_attack_both[i]);
                    fl.add_effect(off_hand.get_my_damage_type(), squares_to_attack_both[i]);
                }
            }
        }

        public List<gridCoordinate> return_axe_patterns(gridCoordinate pl_gc, gridCoordinate monster_gc, List<gridCoordinate> coord_list, Floor fl)
        {
            int x_difference = monster_gc.x - pl_gc.x;
            int y_difference = monster_gc.y - pl_gc.y;

            List<gridCoordinate> temporary_list = new List<gridCoordinate>();

            if (x_difference == -1)
            {
                if (y_difference == -1)
                {
                    temporary_list.Add(new gridCoordinate(monster_gc.x + 1, monster_gc.y));
                    temporary_list.Add(new gridCoordinate(monster_gc.x, monster_gc.y + 1));
                }
                else if (y_difference == 0)
                {
                    temporary_list.Add(new gridCoordinate(monster_gc.x, monster_gc.y - 1));
                    temporary_list.Add(new gridCoordinate(monster_gc.x, monster_gc.y + 1));
                }
                else if (y_difference == 1)
                {
                    temporary_list.Add(new gridCoordinate(monster_gc.x + 1, monster_gc.y));
                    temporary_list.Add(new gridCoordinate(monster_gc.x, monster_gc.y - 1));
                }
            }
            else if (x_difference == 1)
            {
                if (y_difference == 1)
                {
                    temporary_list.Add(new gridCoordinate(monster_gc.x - 1, monster_gc.y));
                    temporary_list.Add(new gridCoordinate(monster_gc.x, monster_gc.y - 1));
                }
                else if (y_difference == 0)
                {
                    temporary_list.Add(new gridCoordinate(monster_gc.x, monster_gc.y - 1));
                    temporary_list.Add(new gridCoordinate(monster_gc.x, monster_gc.y + 1));
                }
                if (y_difference == -1)
                {
                    temporary_list.Add(new gridCoordinate(monster_gc.x - 1, monster_gc.y));
                    temporary_list.Add(new gridCoordinate(monster_gc.x, monster_gc.y + 1));
                }
            }
            else if (x_difference == 0)
            {
                temporary_list.Add(new gridCoordinate(monster_gc.x - 1, monster_gc.y));
                temporary_list.Add(new gridCoordinate(monster_gc.x + 1, monster_gc.y));
            }

            for (int i = 0; i < temporary_list.Count; i++)
            {
                if(fl.isWalkable(temporary_list[i]))
                    coord_list.Add(new gridCoordinate(temporary_list[i]));
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
                if(fl.is_tile_passable(square_to_attack) && !blocked)
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

        public void attack_monster_in_grid(Floor fl, int dmg_val, int c_monsterID, gridCoordinate current_gc, string weapon_name, bool melee_attack)
        {
            string attack_msg = "You attack the " + fl.badguy_by_monster_id(c_monsterID).my_name + " with your " + weapon_name + "!";
            message_buffer.Add(attack_msg);
            fl.damage_monster(dmg_val, c_monsterID, melee_attack);
        }

        public void attack_doodad_in_grid(Floor fl, int dmg_val, int c_DoodadID, gridCoordinate current_gc, string weapon_name)
        {
            string attack_msg = "You attack the " + fl.doodad_by_index(c_DoodadID).my_name() + " with your " + weapon_name + "!";
            message_buffer.Add(attack_msg);
            fl.damage_doodad(dmg_val, c_DoodadID);
        }

        public void set_ranged_attack_aura(Floor fl, gridCoordinate pl_gc)
        {
            int bow_range = 0;
            if (main_hand != null && (main_hand.get_my_weapon_type() == Weapon.Type.Bow ||
                                      main_hand.get_my_weapon_type() == Weapon.Type.Crossbow))
                bow_range = main_hand.get_my_range();
            else
                bow_range = off_hand.get_my_range();

            List<gridCoordinate> endpoints = new List<gridCoordinate>();
            if (is_bow_equipped() && !is_cbow_equipped())
            {
                endpoints = calculate_endpoints(pl_gc, bow_range);
            }
            else
            {
                endpoints.Add(new gridCoordinate(pl_gc.x, pl_gc.y + bow_range));
                endpoints.Add(new gridCoordinate(pl_gc.x, pl_gc.y - bow_range));
                endpoints.Add(new gridCoordinate(pl_gc.x + bow_range, pl_gc.y));
                endpoints.Add(new gridCoordinate(pl_gc.x - bow_range, pl_gc.y));
            }
            List<VisionRay> range_rays = new List<VisionRay>();

            for (int i = 0; i < endpoints.Count; i++)
            {
                range_rays.Add(new VisionRay(pl_gc, endpoints[i]));
            }

            int monsterID = -1;
            while (range_rays.Count > 0)
            {
                for (int i = 0; i < range_rays.Count; i++)
                {
                    bool remove = false;
                    int whoCares = -1;

                    range_rays[i].update();
                    gridCoordinate current_ray_position = new gridCoordinate((int)range_rays[i].my_current_position.X / 32, (int)range_rays[i].my_current_position.Y / 32);
                    int x_difference = positive_difference(pl_gc.x, current_ray_position.x);
                    int y_difference = positive_difference(pl_gc.y, current_ray_position.y);
                    
                    if (fl.isWalkable(current_ray_position) && (x_difference > 1 || y_difference > 1))
                            fl.set_tile_aura(current_ray_position, Tile.Aura.Attack);

                    if (!fl.isWalkable(current_ray_position) && (x_difference > 1 || y_difference > 1) &&
                        fl.is_destroyable_doodad_here(current_ray_position, out whoCares))
                        fl.set_tile_aura(current_ray_position, Tile.Aura.Attack);
                    
                    if(!fl.isWalkable(current_ray_position))
                        remove = true;

                    if ((main_hand != null && main_hand.get_my_weapon_type() == Weapon.Type.Crossbow) ||
                        (off_hand != null && off_hand.get_my_weapon_type() == Weapon.Type.Crossbow))
                        if (fl.is_monster_here(current_ray_position, out monsterID))
                            remove = true;

                    if (range_rays[i].is_at_end() || remove)
                        range_rays.RemoveAt(i);
                }
            }
        }

        public void bow_attack(Floor fl, ref ContentManager Secondary_cManager, int monsterID, int doodadID)
        {
            int dmg_to_monster = 0;
            int splash_dmg = 0;
            string wName = "";
            gridCoordinate opposition_coord = new gridCoordinate(-1, -1);
            if (monsterID != -1)
                opposition_coord = fl.badguy_by_monster_id(monsterID).my_grid_coord;
            else
                opposition_coord = fl.doodad_by_index(doodadID).get_g_coord();

            int cbow_xsplash = 0;
            int cbow_ysplash = 0;
            if (opposition_coord.x < my_grid_coord.x)
                cbow_xsplash = -1;
            else if (opposition_coord.x > my_grid_coord.x)
                cbow_xsplash = 1;

            if (opposition_coord.y < my_grid_coord.y)
                cbow_ysplash = -1;
            else if (opposition_coord.y > my_grid_coord.y)
                cbow_ysplash = 1;
            gridCoordinate splash_coord = new gridCoordinate(opposition_coord.x + cbow_xsplash, opposition_coord.y + cbow_ysplash);

            if (main_hand != null && (main_hand.get_my_weapon_type() == Weapon.Type.Bow ||
                                      main_hand.get_my_weapon_type() == Weapon.Type.Crossbow))
            {
                dmg_to_monster = main_hand.damage(ref rGen);
                splash_dmg = main_hand.damage(ref rGen);
                wName = main_hand.get_my_name();
            }
            else
            {
                dmg_to_monster = off_hand.damage(ref rGen);
                splash_dmg = off_hand.damage(ref rGen);
                wName = off_hand.get_my_name();
            }

            fl.create_new_projectile(new Projectile(get_my_grid_C(), opposition_coord, Projectile.projectile_type.Arrow, ref Secondary_cManager));
            fl.add_effect(Attack.Damage.Piercing, opposition_coord);
            if (is_cbow_equipped() && fl.isWalkable(splash_coord))
                fl.add_effect(Attack.Damage.Piercing, splash_coord);

            if (monsterID != -1)
                attack_monster_in_grid(fl, dmg_to_monster, monsterID, opposition_coord, wName, false);
            else
                attack_doodad_in_grid(fl, dmg_to_monster, doodadID, opposition_coord, wName);

            if (is_cbow_equipped())
            {
                int splash_monster_ID = -1;
                int splash_doodad_ID = -1;
                fl.is_monster_here(splash_coord, out splash_monster_ID);
                fl.is_destroyable_doodad_here(splash_coord, out splash_doodad_ID);
                if (splash_monster_ID != -1)
                    attack_monster_in_grid(fl, splash_dmg, splash_monster_ID, splash_coord, wName, false);
                if (splash_doodad_ID != -1)
                    attack_doodad_in_grid(fl, splash_dmg, splash_doodad_ID, splash_coord, wName);
            }
            total_sound += my_sound_value() + (my_sound_value() / 2);
            total_scent += my_scent_value();
        }

        public void set_charge_attack_aura(Floor fl, int lanceID, gridCoordinate pl_gc)
        {
            Weapon c_lance = null;
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i].get_my_IDno() == lanceID)
                    c_lance = (Weapon)inventory[i];
            }
            int lance_range = c_lance.get_my_range();

            List<gridCoordinate> endPoints = calculate_endpoints(pl_gc, lance_range);
            List<VisionRay> charge_rays = new List<VisionRay>();

            for (int i = 0; i < endPoints.Count; i++)
            {
               charge_rays.Add(new VisionRay(pl_gc, endPoints[i]));
            }

            int monsterID = -1;
            while (charge_rays.Count > 0)
            {
                for (int i = 0; i < charge_rays.Count; i++)
                {
                    bool remove = false;
                    int whoCares = -1;
                    charge_rays[i].update();
                    gridCoordinate current_ray_position = new gridCoordinate((int)charge_rays[i].my_current_position.X / 32, (int)charge_rays[i].my_current_position.Y / 32);
                    int x_difference = positive_difference(pl_gc.x, current_ray_position.x);
                    int y_difference = positive_difference(pl_gc.y, current_ray_position.y);

                    if (fl.isWalkable(current_ray_position) && (x_difference > 1 || y_difference > 1))
                        fl.set_tile_aura(current_ray_position, Tile.Aura.Attack);

                    if (!fl.isWalkable(current_ray_position) && (x_difference > 1 || y_difference > 1) &&
                        fl.is_destroyable_doodad_here(current_ray_position, out whoCares))
                        fl.set_tile_aura(current_ray_position, Tile.Aura.Attack);

                    if (!fl.isWalkable(current_ray_position) || fl.is_monster_here(current_ray_position, out monsterID))
                        remove = true;

                    if (charge_rays[i].is_at_end() || remove)
                        charge_rays.RemoveAt(i);
                }
            }
        }

        public void charge_attack(Floor fl, int lanceID, int monsterID, int doodadID)
        {
            bool attacked_doodad = false;
            gridCoordinate my_original_position = new gridCoordinate(my_grid_coord);
            Weapon c_lance = null;
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i].get_my_IDno() == lanceID)
                    c_lance = (Weapon)inventory[i];
            }
            int dmg_val = c_lance.damage(ref rGen);
            string wName = c_lance.get_my_name();

            VisionRay attack_ray = null;
            if(fl.badguy_by_monster_id(monsterID) == null)
            {
                attack_ray = new VisionRay(my_grid_coord, fl.doodad_by_index(doodadID).get_g_coord());
            }
            else
                attack_ray = new VisionRay(my_grid_coord, fl.badguy_by_monster_id(monsterID).my_grid_coord);

            gridCoordinate monster_coord = new gridCoordinate(-1, -1);
            gridCoordinate doodad_coord = new gridCoordinate(-1, -1);

            bool done = false;
            while (!done)
            {
                int old_xPosition = (int)attack_ray.my_current_position.X / 32;
                int old_yPosition = (int)attack_ray.my_current_position.Y / 32;
                gridCoordinate previous_ray_position = new gridCoordinate(old_xPosition, old_yPosition);

                attack_ray.update();

                int new_xPosition = (int)attack_ray.my_current_position.X / 32;
                int new_yPosition = (int)attack_ray.my_current_position.Y / 32;
                gridCoordinate next_ray_position = new gridCoordinate(new_xPosition, new_yPosition);

                int mon_ID;
                int dood_ID;
                fl.is_monster_here(next_ray_position, out mon_ID);
                fl.is_destroyable_doodad_here(next_ray_position, out dood_ID);
                if (mon_ID == monsterID && monsterID > -1)
                {
                    monster_coord = new gridCoordinate(fl.badguy_by_monster_id(monsterID).my_grid_coord);
                    teleport(previous_ray_position);
                    attack_monster_in_grid(fl, dmg_val, monsterID, fl.badguy_by_monster_id(monsterID).my_grid_coord, wName, true);
                    done = true;
                }

                if (dood_ID == doodadID && doodadID > -1)
                {
                    attacked_doodad = true;
                    doodad_coord = new gridCoordinate(fl.doodad_by_index(doodadID).get_g_coord());
                    teleport(previous_ray_position);
                    attack_doodad_in_grid(fl, dmg_val, doodadID, fl.doodad_by_index(doodadID).get_g_coord(), wName);
                    done = true;
                }
            }

            gridCoordinate opposition_coord = new gridCoordinate(-1, -1);
            if (attacked_doodad)
                opposition_coord = doodad_coord;
            else
                opposition_coord = monster_coord;

            if (!is_spot_free(fl))
            {
                int xdif = my_original_position.x - opposition_coord.x;
                int ydif = my_original_position.y - opposition_coord.y;

                int whocares = -1;
                if (fl.is_monster_here(my_grid_coord, out whocares) || fl.is_destroyable_doodad_here(my_grid_coord, out whocares))
                {
                    if (xdif == 0)
                        if (my_original_position.x < my_grid_coord.x)
                            my_grid_coord.x--;
                        else
                            my_grid_coord.x++;

                    if (ydif == 0)
                        if (my_original_position.y < my_grid_coord.y)
                            my_grid_coord.y--;
                        else
                            my_grid_coord.y++;

                }
                else if (!fl.is_monster_here(my_grid_coord, out whocares) && !fl.isWalkable(my_grid_coord))
                {
                    int xshift = 0;
                    int yshift = 0;

                    if (xdif < 0)
                        xshift = -1;
                    else
                        xshift = 1;

                    if (ydif < 0)
                        yshift = -1;
                    else
                        yshift = 1;

                    gridCoordinate x_shifted = new gridCoordinate(my_grid_coord.x + xshift, my_grid_coord.y);
                    gridCoordinate y_shifted = new gridCoordinate(my_grid_coord.x, my_grid_coord.y + yshift);
                    bool x_ok = fl.isWalkable(x_shifted);
                    bool y_ok = fl.isWalkable(y_shifted);

                    if (x_ok && !y_ok)
                        my_grid_coord = x_shifted;
                    else if (!x_ok && y_ok)
                        my_grid_coord = y_shifted;
                    else
                    {
                        int go_x = rGen.Next(2);
                        if (go_x == 0)
                            my_grid_coord = x_shifted;
                        else
                            my_grid_coord = y_shifted;
                    }
                }
            }

            reset_my_drawing_position();

            loot(fl);
            total_sound += my_sound_value() + (my_sound_value() / 2);
            total_scent += my_scent_value();
        }
        
        public void set_bash_attack_aura(Floor fl)
        {
            int whoCares = -1;
            for (int x = my_grid_coord.x - 1; x <= my_grid_coord.x + 1; x++)
                for (int y = my_grid_coord.y - 1; y <= my_grid_coord.y + 1; y++)
                {
                    gridCoordinate target_coord = new gridCoordinate(x, y);
                    if (!(x == my_grid_coord.x && y == my_grid_coord.y) && fl.isWalkable(target_coord))
                        fl.set_tile_aura(target_coord, Tile.Aura.Attack);

                    if (!(x == my_grid_coord.x && y == my_grid_coord.y) && !fl.isWalkable(target_coord) &&
                        fl.is_destroyable_doodad_here(target_coord, out whoCares))
                        fl.set_tile_aura(target_coord, Tile.Aura.Attack);
                }
        }

        public void bash_attack(Floor fl, Monster m, Doodad d, Weapon wp)
        {
            int xdif = 0;
            int ydif = 0;
            if (m != null)
            {
                xdif = m.my_grid_coord.x - my_grid_coord.x;
                ydif = m.my_grid_coord.y - my_grid_coord.y;
            }
            else
            {
                xdif = d.get_g_coord().x - my_grid_coord.x;
                ydif = d.get_g_coord().y - my_grid_coord.y;
            }

            int damage_value = (int)(wp.damage(ref rGen) * 1.4);
            if (wp.get_hand_count() == 2)
                damage_value = (int)((wp.damage(ref rGen)*2) * 1.6);

            int m_hp = 0;
            if(m!= null)
                m_hp = m.hitPoints;

            gridCoordinate opposition_coord = new gridCoordinate(-1, -1);
            if(m != null)
                opposition_coord = m.my_grid_coord;
            else
                opposition_coord = d.get_g_coord();

            fl.add_effect(wp.get_my_damage_type(), opposition_coord);
            if (m != null)
                attack_monster_in_grid(fl, damage_value, m.my_Index, m.my_grid_coord, wp.get_my_name(), true);
            else
                attack_doodad_in_grid(fl, damage_value, d.get_my_index(), d.get_g_coord(), wp.get_my_name());

            if (m!= null && m.hitPoints > 0 && m_hp > m.hitPoints)
            {
                int whocares = -1;
                gridCoordinate next_m_coord = new gridCoordinate(m.my_grid_coord.x + xdif, m.my_grid_coord.y + ydif);
                if (fl.isWalkable(next_m_coord) && !fl.is_monster_here(next_m_coord, out whocares))
                {
                    m.my_grid_coord = next_m_coord;
                    m.snap_to_grid();
                }
                else
                {
                    int secondary_damage_value = wp.damage(ref rGen) / 2;
                    fl.add_effect(wp.get_my_damage_type(), m.my_grid_coord);
                    attack_monster_in_grid(fl, damage_value, m.my_Index, m.my_grid_coord, wp.get_my_name(), true);
                }
            }
            wp.set_cooldown(standard_wpn_cooldown);
        }

        public void whirlwind_attack(Floor fl, Weapon target_weapon)
        {
            List<gridCoordinate> target_coordinates = new List<gridCoordinate>();
            for (int x = my_grid_coord.x - 1; x <= my_grid_coord.x + 1; x++)
                for (int y = my_grid_coord.y - 1; y <= my_grid_coord.y + 1; y++)
                    if(!(x== my_grid_coord.x && y== my_grid_coord.y))
                        target_coordinates.Add(new gridCoordinate(x, y));

            int monster_ID = -1;
            int doodad_ID = -1;
            for (int i = 0; i < target_coordinates.Count; i++)
            {
                int attack_dmg = (int)(target_weapon.damage(ref rGen) * .8);
                if (target_weapon.get_hand_count() == 2)
                    attack_dmg = (int)((target_weapon.damage(ref rGen) * 2) * .9);

                if (fl.is_monster_here(target_coordinates[i], out monster_ID))
                    attack_monster_in_grid(fl, attack_dmg, monster_ID, target_coordinates[i],
                                            target_weapon.get_my_name(), true);

                if (fl.is_destroyable_doodad_here(target_coordinates[i], out doodad_ID))
                    attack_doodad_in_grid(fl, attack_dmg, doodad_ID, target_coordinates[i],
                                            target_weapon.get_my_name());

                if (fl.isWalkable(target_coordinates[i]))
                    fl.add_effect(target_weapon.get_my_damage_type(), target_coordinates[i]);
            }
            target_weapon.set_cooldown(standard_wpn_cooldown);
        }

        private int positive_difference(int i1, int i2)
        {
            if (i1 > i2)
                return i1 - i2;
            else
                return i2 - i1;
        }

        private List<gridCoordinate> calculate_endpoints(gridCoordinate pl_gc, int wpn_range)
        {
            List<gridCoordinate> eps = new List<gridCoordinate>();

            for (int x = pl_gc.x - wpn_range; x <= pl_gc.x + wpn_range; x++)
            {
                for (int y = pl_gc.y - wpn_range; y <= pl_gc.y + wpn_range; y++)
                {
                    int x_difference = positive_difference(x, pl_gc.x);
                    int y_difference = positive_difference(y, pl_gc.y);

                    if (x_difference + y_difference == wpn_range)
                        eps.Add(new gridCoordinate(x, y));
                }
            }

            return eps;
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
            if (thing is Armor)
            {
                Armor armor_thing = (Armor)thing;
                Armor acquired_armor = new Armor(armor_thing.get_my_IDno(), 
                                                armor_thing.get_my_gold_value(), 
                                                armor_thing.get_my_name(), armor_thing);
                inventory.Add(acquired_armor);
            }
            else if (thing is Weapon)
            {
                Weapon weapon_thing = (Weapon)thing;
                Weapon acquired_weapon = new Weapon(weapon_thing.get_my_IDno(),
                                                    weapon_thing.get_my_gold_value(),
                                                    weapon_thing.get_my_name(), weapon_thing);
                inventory.Add(acquired_weapon);
            }
            else if (thing is Scroll)
            {
                Scroll scroll_thing = (Scroll)thing;
                Scroll acquired_scroll = new Scroll(scroll_thing.get_my_IDno(),
                                                    scroll_thing.get_my_gold_value(),
                                                    scroll_thing.get_my_name(), scroll_thing);
                inventory.Add(acquired_scroll);
            }

            //inventory.Add(thing);
        }

        public void acquire_potion(Potion pt)
        {
            //search for a potion stack with a matching ID + fullness value and add to that stack.
            //If there isn't one, just add it to the inventory.
            bool stacked = false;
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i] is Potion)
                {
                    Potion po = (Potion)inventory[i];
                    if (po.get_my_IDno() == pt.get_my_IDno() && 
                        po.is_potion_empty() == pt.is_potion_empty() &&
                        po.get_my_quantity() < 7)
                    {
                        po.adjust_quantity(pt.get_my_quantity());
                        stacked = true;
                    }
                }
            }

            if (!stacked)
                inventory.Add(pt);
        }

        //Green text. Function here.
        public void take_damage(Attack atk, ref Floor fl)
        {
            //OKAY THIS IS GONNA BE COMPLICATED.
            //First, figure out where the attack is gonna hit. The breakdown is as follows:
            //head, 5%, chest = 25%, arm = 17%, leg = 18%
            int hit_location = rGen.Next(100);
            int dodge_roll = rGen.Next(100);
            bool dodged = false;

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

            if (dodge_roll < dodge_chance)
            {
                dodged = true;
                fl.add_new_popup("Dodged!", Popup.popup_msg_color.LimeGreen, my_grid_coord);
                message_buffer.Add("You dodge the attack!");
            }

            if (!dodged)
            {
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
                    if (under_armor != null)
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
                    if (over_armor != null)
                        atk = over_armor.absorb_damage(atk, atkzone, my_grid_coord, ref rGen, ref message_buffer, ref fl);
                    if (under_armor != null)
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
                calculate_dodge_chance();
                update_pdoll();
            }

            if (L_Arm.is_disabled())
                unequip(Equip_Slot.Mainhand);

            if (R_Arm.is_disabled())
                unequip(Equip_Slot.Offhand);

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

            calculate_dodge_chance();
            update_pdoll();
        }

        public void heal_via_potion(Potion pt, string bodypart, bool repair_over_armor, Floor fl)
        {
            if (pt.get_type() == Potion.Potion_Type.Health)
            { 
                switch (bodypart)
                {
                    case "Head":
                        Head.heal_via_potion(pt.potion_potency());
                        fl.add_new_popup("+" + pt.potion_potency() + " Head", Popup.popup_msg_color.VividGreen, my_grid_coord);
                        break;
                    case "Chest":
                        Torso.heal_via_potion(pt.potion_potency());
                        fl.add_new_popup("+" + pt.potion_potency() + " Chest", Popup.popup_msg_color.VividGreen, my_grid_coord);
                        break;
                    case "LArm":
                        L_Arm.heal_via_potion(pt.potion_potency());
                        fl.add_new_popup("+" + pt.potion_potency() + " LArm", Popup.popup_msg_color.VividGreen, my_grid_coord);
                        break;
                    case "RArm":
                        R_Arm.heal_via_potion(pt.potion_potency());
                        fl.add_new_popup("+" + pt.potion_potency() + " RArm", Popup.popup_msg_color.VividGreen, my_grid_coord);
                        break;
                    case "LLeg":
                        L_Leg.heal_via_potion(pt.potion_potency());
                        fl.add_new_popup("+" + pt.potion_potency() + " LLeg", Popup.popup_msg_color.VividGreen, my_grid_coord);
                        break;
                    case "RLeg":
                        R_Leg.heal_via_potion(pt.potion_potency());
                        fl.add_new_popup("+" + pt.potion_potency() + " RLeg", Popup.popup_msg_color.VividGreen, my_grid_coord);
                        break;
                }
            }
            else if (pt.get_type() == Potion.Potion_Type.Repair)
            {
                if (repair_over_armor)
                    over_armor.repair_by_zone(pt.potion_potency(), bodypart);
                else
                    under_armor.repair_by_zone(pt.potion_potency(), bodypart);
                fl.add_new_popup("+" + pt.potion_potency() + " " + bodypart, Popup.popup_msg_color.Blue, my_grid_coord);
            }

            calculate_dodge_chance();
            pDoll.update_wound_report(Head.count_debilitating_injuries(),
                                          Torso.count_debilitating_injuries(),
                                          L_Arm.count_debilitating_injuries(),
                                          R_Arm.count_debilitating_injuries(),
                                          L_Leg.count_debilitating_injuries(),
                                          R_Leg.count_debilitating_injuries());
            pt.drink();
        }

        public void ingest_potion(Potion pt, Floor fl, bool repair_over_armor)
        {
            int potency = (int)(pt.potion_potency() * 1.6);
            bool done = false;
            Armor target_armor = null;

            int head_tally = 0;
            int chest_tally = 0;
            int rarm_tally = 0;
            int larm_tally = 0;
            int lleg_tally = 0;
            int rleg_tally = 0;

            if (pt.get_type() == Potion.Potion_Type.Health)
            {
                while (!done)
                {
                    int target_part = rGen.Next(6);
                    switch (target_part)
                    {
                        case 0:
                            if (!Head.is_uninjured())
                            {
                                Head.heal_random_wound();
                                head_tally++;
                                potency--;
                            }
                            break;
                        case 1:
                            if (!Torso.is_uninjured())
                            {
                                Torso.heal_random_wound();
                                chest_tally++;
                                potency--;
                            }
                            break;
                        case 2:
                            if (!R_Arm.is_uninjured())
                            {
                                R_Arm.heal_random_wound();
                                rarm_tally++;
                                potency--;
                            }
                            break;
                        case 3:
                            if (!L_Arm.is_uninjured())
                            {
                                L_Arm.heal_random_wound();
                                larm_tally++;
                                potency--;
                            }
                            break;
                        case 4:
                            if (!R_Leg.is_uninjured())
                            {
                                R_Leg.heal_random_wound();
                                rleg_tally++;
                                potency--;
                            }
                            break;
                        case 5:
                            if (!L_Leg.is_uninjured())
                            {
                                L_Leg.heal_random_wound();
                                lleg_tally++;
                                potency--;
                            }
                            break;
                    }

                    if (potency == 0)
                        done = true;

                    if (Head.is_uninjured() && Torso.is_uninjured() &&
                        L_Arm.is_uninjured() && L_Leg.is_uninjured() &&
                        R_Arm.is_uninjured() && R_Leg.is_uninjured())
                        done = true;
                }

                //display messages of how many wounds were healed for the player.
                if (head_tally > 0)
                    fl.add_new_popup("+" + head_tally + " Head", Popup.popup_msg_color.VividGreen, my_grid_coord);
                if(chest_tally > 0)
                    fl.add_new_popup("+" + chest_tally + " Chest", Popup.popup_msg_color.VividGreen, my_grid_coord);
                if (larm_tally > 0)
                    fl.add_new_popup("+" + larm_tally + " LArm", Popup.popup_msg_color.VividGreen, my_grid_coord);
                if (rarm_tally > 0)
                    fl.add_new_popup("+" + rarm_tally + " RArm", Popup.popup_msg_color.VividGreen, my_grid_coord);
                if (lleg_tally > 0)
                    fl.add_new_popup("+" + lleg_tally + " LLeg", Popup.popup_msg_color.VividGreen, my_grid_coord);
                if (rleg_tally > 0)
                    fl.add_new_popup("+" + rleg_tally + " RLeg", Popup.popup_msg_color.VividGreen, my_grid_coord);
            }
            else if (pt.get_type() == Potion.Potion_Type.Repair)
            {
                if (repair_over_armor && over_armor != null)
                    target_armor = over_armor;
                else if (!repair_over_armor && under_armor != null)
                    target_armor = under_armor;

                while (!done && target_armor != null)
                {
                    int repair_zone = rGen.Next(5);
                    string target_zone = "";
                    switch (repair_zone)
                    {
                        case 0:
                            target_zone = "Chest";
                            break;
                        case 1:
                            target_zone = "LArm";
                            break;
                        case 2:
                            target_zone = "RArm";
                            break;
                        case 3:
                            target_zone = "LLeg";
                            break;
                        case 4:
                            target_zone = "RLeg";
                            break;
                    }

                    if (target_armor.is_zone_damaged(target_zone))
                    {
                        target_armor.repair_by_zone(1, target_zone);
                        potency--;

                        switch (target_zone)
                        {
                            case "Chest":
                                chest_tally++;
                                break;
                            case "RArm":
                                rarm_tally++;
                                break;
                            case "LArm":
                                larm_tally++;
                                break;
                            case "LLeg":
                                lleg_tally++;
                                break;
                            case "RLeg":
                                rleg_tally++;
                                break;
                        }
                    }

                    if (potency == 0 || target_armor.is_undamaged())
                        done = true;
                }

                //Again, add messages.
                if (chest_tally > 0)
                    fl.add_new_popup("+" + chest_tally + " Chest", Popup.popup_msg_color.Blue, my_grid_coord);
                if (larm_tally > 0)
                    fl.add_new_popup("+" + larm_tally + " LArm", Popup.popup_msg_color.Blue, my_grid_coord);
                if (rarm_tally > 0)
                    fl.add_new_popup("+" + rarm_tally + " RArm", Popup.popup_msg_color.Blue, my_grid_coord);
                if (lleg_tally > 0)
                    fl.add_new_popup("+" + lleg_tally + " LLeg", Popup.popup_msg_color.Blue, my_grid_coord);
                if (rleg_tally > 0)
                    fl.add_new_popup("+" + rleg_tally + " RLeg", Popup.popup_msg_color.Blue, my_grid_coord);
            }

            calculate_dodge_chance();
            pDoll.update_wound_report(Head.count_debilitating_injuries(),
                                          Torso.count_debilitating_injuries(),
                                          L_Arm.count_debilitating_injuries(),
                                          R_Arm.count_debilitating_injuries(),
                                          L_Leg.count_debilitating_injuries(),
                                          R_Leg.count_debilitating_injuries());

            if(pt.get_type() == Potion.Potion_Type.Repair && target_armor != null)
                pt.drink();

            if (pt.get_type() == Potion.Potion_Type.Health)
                pt.drink();
        }

        public void repair_all_armor()
        {
            if (over_armor != null)
                over_armor.full_repair();
            if (under_armor != null)
                under_armor.full_repair();
        }

        public void refill_all_potions()
        {
            List<Potion> temporary_PTs = new List<Potion>();

            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i] is Potion)
                {
                    Potion oldPT = (Potion)inventory[i];
                    if (oldPT.is_potion_empty())
                    {
                        int nPTID = oldPT.get_my_IDno();
                        int nPTco = oldPT.get_my_gold_value();
                        string nPTnm = oldPT.get_my_name();
                        Potion nextPT = new Potion(nPTID, nPTco, nPTnm, oldPT);
                        nextPT.refill();
                        temporary_PTs.Add(nextPT);
                    }
                }
            }

            for(int i = inventory.Count-1; i >= 0; i--)
                if (inventory[i] is Potion)
                {
                    Potion P = (Potion)inventory[i];
                    if (P.is_potion_empty())
                        inventory.RemoveAt(i);
                }

            for (int i = 0; i < temporary_PTs.Count; i++)
                acquire_potion(temporary_PTs[i]);
            temporary_PTs.Clear();
        }

        public void repair_via_potion(Potion pt)
        {
        }

        public void clear_inv_nulls()
        {
            for (int i = 0; i < inventory.Count; i++)
                if (inventory[i] == null)
                    inventory.RemoveAt(i);
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
            return (fl.isWalkable(my_grid_coord) && !is_monster_present(fl, out whoCares));
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

        public void reset_sound_and_scent()
        {
            total_sound = 0;
            total_scent = 0;
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

            wRep.Add("Chance to dodge attacks: " + dodge_chance + "%");
            wRep.Add(" ");
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
                eRep.Add("Chance to absorb frost attack: " + calc_absorb_chance(oa_pa_val, oa_in_val) + "%");
                eRep.Add("Chance to absorb electric attack: " + calc_absorb_chance(oa_in_val, oa_pa_val) + "%");
                eRep.Add("Chance to absorb acid attack: " + calc_absorb_chance(oa_in_val, oa_ab_val) + "%");
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
                eRep.Add("Chance to absorb frost attack: " + calc_absorb_chance(ua_pa_val, ua_in_val) + "%");
                eRep.Add("Chance to absorb electric attack: " + calc_absorb_chance(ua_in_val, ua_pa_val) + "%");
                eRep.Add("Chance to absorb acid attack: " + calc_absorb_chance(ua_in_val, ua_ab_val) + "%");
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
            if ((mh.get_hand_count() == 1 && !L_Arm.is_disabled()) ||
                (!L_Arm.is_disabled() && !R_Arm.is_disabled()))
            {
                unequip(Equip_Slot.Mainhand);
                if (mh.get_hand_count() == 2)
                    unequip(Equip_Slot.Offhand);
                //Okay, we've un-equipped both weapons.

                //Now, check how many hands our current weapon has.
                int hnd_cnt = 0;
                if (main_hand != null)
                    hnd_cnt = main_hand.get_hand_count();
                main_hand = mh;
                if (main_hand.get_hand_count() == 1 && hnd_cnt == 2)
                    off_hand = null;
                if (main_hand.get_hand_count() == 2)
                    off_hand = mh;

                //Now that we've equipped, check to see if the offhand is a bow. If it is, unequip it.
                check_for_1_5_hands(true);

                remove_item_from_inventory(main_hand.get_my_IDno());
            }
        }

        public void equip_off_hand(Weapon oh)
        {
            if ((oh.get_hand_count() == 1 && !R_Arm.is_disabled()) ||
                (!L_Arm.is_disabled() && !R_Arm.is_disabled()))
            {
                unequip(Equip_Slot.Offhand);
                if (oh.get_hand_count() == 2)
                    unequip(Equip_Slot.Mainhand);

                int hnd_cnt = 0;
                if (off_hand != null)
                    hnd_cnt = off_hand.get_hand_count();
                off_hand = oh;
                if (off_hand.get_hand_count() == 1 && hnd_cnt == 2)
                    main_hand = null;
                if (off_hand.get_hand_count() == 2)
                    main_hand = oh;

                check_for_1_5_hands(false);

                remove_item_from_inventory(off_hand.get_my_IDno());
            }
        }

        public void check_for_1_5_hands (bool mainHand)
        {
            if (main_hand != null && off_hand != null)
            {
                Weapon.Type mh_wtype = main_hand.get_my_weapon_type();
                Weapon.Type oh_wtype = off_hand.get_my_weapon_type();

                if ((mh_wtype == Weapon.Type.Bow || mh_wtype == Weapon.Type.Crossbow || mh_wtype == Weapon.Type.Lance) &&
                    (oh_wtype == Weapon.Type.Bow || oh_wtype == Weapon.Type.Crossbow || oh_wtype == Weapon.Type.Lance))
                    if (mainHand)
                        unequip(Equip_Slot.Offhand);
                    else
                        unequip(Equip_Slot.Mainhand);
            }
        }

        public void equip_over_armor(Armor oa)
        {
            unequip(Equip_Slot.Overarmor);

            over_armor = oa;

            remove_item_from_inventory(over_armor.get_my_IDno());
        }

        public void equip_under_armor(Armor ua)
        {
            unequip(Equip_Slot.Underarmor);

            under_armor = ua;

            remove_item_from_inventory(under_armor.get_my_IDno());
        }

        public void remove_item_from_inventory(int itemID)
        {
            for (int i = 0; i < inventory.Count; i++)
                if (inventory[i].get_my_IDno() == itemID)
                    inventory.RemoveAt(i);
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
                    if(main_hand != null)
                        inventory.Add(main_hand);
                    main_hand = null;
                    if (mh_handcount == 2)
                        off_hand = null;
                    break;
                case Equip_Slot.Offhand:
                    if(off_hand != null)
                        inventory.Add(off_hand);
                    off_hand = null;
                    if (mh_handcount == 2)
                        main_hand = null;
                    break;
                case Equip_Slot.Underarmor:
                    if(under_armor != null)
                        inventory.Add(under_armor);
                    under_armor = null;
                    break;
                case Equip_Slot.Overarmor:
                    if(over_armor != null)
                        inventory.Add(over_armor);
                    over_armor = null;
                    break;
            }
        }

        public bool is_bow_equipped()
        {
            bool retvalue = false;
            if (main_hand != null)
                retvalue = main_hand.get_my_weapon_type() == Weapon.Type.Bow ||
                           main_hand.get_my_weapon_type() == Weapon.Type.Crossbow;

            if (off_hand != null && !retvalue)
                retvalue = off_hand.get_my_weapon_type() == Weapon.Type.Bow ||
                           off_hand.get_my_weapon_type() == Weapon.Type.Crossbow;

            return retvalue;
        }

        public bool is_cbow_equipped()
        {
            bool retvalue = false;
            if (main_hand != null)
                retvalue = main_hand.get_my_weapon_type() == Weapon.Type.Crossbow;

            if (off_hand != null && !retvalue)
                retvalue = off_hand.get_my_weapon_type() == Weapon.Type.Crossbow;

            return retvalue;
        }

        //Functions for the sake of the icon bar.
        public string get_item_type_by_ID(int IDno)
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i].get_my_IDno() == IDno)
                {
                    if (inventory[i] is Weapon)
                        return "Weapon";
                    else if (inventory[i] is Armor)
                    {
                        Armor c_armor = (Armor)inventory[i];
                        if (c_armor.is_over_armor())
                            return "Overarmor";
                        else
                            return "Underarmor";
                    }
                    else if (inventory[i] is Potion)
                        return "Potion";
                }
            }
            //If it hasn't returned yet, it's not in the inventory.
            if ((main_hand != null && main_hand.get_my_IDno() == IDno) || 
                (off_hand != null && off_hand.get_my_IDno() == IDno))
                return "Weapon";
            if (over_armor.get_my_IDno() == IDno)
                return "Overarmor";
            if (under_armor.get_my_IDno() == IDno)
                return "Underarmor";

            return "Something else";
        }

        public bool is_item_equipped(int IDno)
        {
            if (main_hand != null && main_hand.get_my_IDno() == IDno)
                return true;
            if (off_hand != null && off_hand.get_my_IDno() == IDno)
                return true;
            if (under_armor != null && under_armor.get_my_IDno() == IDno)
                return true;
            if (over_armor != null && over_armor.get_my_IDno() == IDno)
                return true;

            return false;
        }

        public Weapon get_weapon_by_ID(int IDno)
        {
            for (int i = 0; i < inventory.Count; i++)
                if (inventory[i] is Weapon && inventory[i].get_my_IDno() == IDno)
                    return (Weapon)inventory[i];

            if (main_hand != null && main_hand.get_my_IDno() == IDno)
                return main_hand;
            if (off_hand != null && off_hand.get_my_IDno() == IDno)
                return off_hand;

            return null;
        }

        public Armor get_armor_by_ID(int IDno)
        {
            for (int i = 0; i < inventory.Count; i++)
                if (inventory[i] is Armor && inventory[i].get_my_IDno() == IDno)
                    return (Armor)inventory[i];

            if (over_armor != null && over_armor.get_my_IDno() == IDno)
                return over_armor;
            if (under_armor != null && under_armor.get_my_IDno() == IDno)
                return under_armor;

            return null;
        }

        public Potion get_potion_by_ID(int IDno)
        {
            int target_index = -1;
            int lowest_quantity = 8;
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i] is Potion && inventory[i].get_my_IDno() == IDno)
                {
                    Potion p = (Potion)inventory[i];
                    if (p.get_my_quantity() < lowest_quantity && !p.is_potion_empty())
                    {
                        target_index = i;
                        lowest_quantity = p.get_my_quantity();
                    }
                }
            }

            if (target_index != -1)
            {
                Potion pt = (Potion)inventory[target_index];
                int rtp_IDno = pt.get_my_IDno();
                int rtp_cost = pt.get_my_gold_value();
                string rtp_name = pt.get_my_name();
                Potion rtp = new Potion(rtp_IDno, rtp_cost, rtp_name, pt);

                rtp.set_quantity(1);
                pt.adjust_quantity(-1);
                if (pt.get_my_quantity() == 0)
                    inventory.RemoveAt(target_index);

                return rtp;
            }

            return null;
        }

        public int count_full_potions_by_ID(int IDno)
        {
            int potion_quantity = 0;
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i] is Potion)
                {
                    Potion p = (Potion)inventory[i];
                    if (p.get_my_IDno() == IDno && !p.is_potion_empty())
                        potion_quantity += p.get_my_quantity();
                }
            }

            return potion_quantity;
        }

        public void calculate_dodge_chance()
        {
            dodge_chance = 0;
            if (!R_Leg.is_disabled())
                dodge_chance += 10;
            if (!L_Leg.is_disabled())
                dodge_chance += 10;
        }

        public void deincrement_cooldowns()
        {
            if (main_hand != null && main_hand.get_current_cooldown() > 0)
                main_hand.set_cooldown(-1);

            if ((off_hand != null && main_hand != null &&
                main_hand.get_hand_count() == 1 &&
                off_hand.get_current_cooldown() > 0) || (off_hand != null && main_hand == null))
                off_hand.set_cooldown(-1);
        }

        public void update_pdoll()
        {
            pDoll.update_wound_report(Head.count_debilitating_injuries(),
                                          Torso.count_debilitating_injuries(),
                                          L_Arm.count_debilitating_injuries(),
                                          R_Arm.count_debilitating_injuries(),
                                          L_Leg.count_debilitating_injuries(),
                                          R_Leg.count_debilitating_injuries());
        }
    }
}
