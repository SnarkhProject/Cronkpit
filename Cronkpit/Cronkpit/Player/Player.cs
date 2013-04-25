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
        public enum Character { Falsael, Ziktofel, Halephon, Petaer, 
                                Belia, Tavec, Zacul, Sir_Placeholder };
        public enum Chara_Class { Warrior, Mage, Rogue, ExPriest };
        public enum Equip_Slot { Mainhand, Offhand, Overarmor, Underarmor, Helmet };
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
        int base_hp;
        //Equipped items
        Weapon main_hand;
        Weapon off_hand;
        Armor helm;
        Armor over_armor;
        Armor under_armor;
        //Inventory
        List<Item> inventory;
        //!Constructor stuff
        private int my_gold;
        private int lifetime_gold;
        //Sensory
        private int base_smell_value;
        private int base_sound_value;
        public int total_sound;
        public int total_scent;
        //Buffs and debuffs
        List<StatusEffect> BuffDebuffTracker;

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
            my_gold = 0;
            base_smell_value = 10;
            base_sound_value = 10;
            //Player stuff
            my_class = myClass;
            my_character = myChara;
            base_hp = 3;

            switch (my_character)
            {
                case Character.Falsael:
                    my_Texture = cont.Load<Texture2D>("Player/falsael_sprite");
                    my_dead_texture = cont.Load<Texture2D>("Player/falsael_dead");
                    break;
                case Character.Petaer:
                    my_Texture = cont.Load<Texture2D>("Player/petaer_sprite");
                    my_dead_texture = cont.Load<Texture2D>("Player/petaer_dead");
                    break;
                case Character.Ziktofel:
                    my_Texture = cont.Load<Texture2D>("Player/ziktofel_sprite");
                    my_dead_texture = cont.Load<Texture2D>("Player/ziktofel_dead");
                    break;
                case Character.Halephon:
                    my_Texture = cont.Load<Texture2D>("Player/halephon_sprite");
                    my_dead_texture = cont.Load<Texture2D>("Player/halephon_dead");
                    break;
                default:
                    my_Texture = cont.Load<Texture2D>("Player/lmfaoplayer");
                    my_dead_texture = cont.Load<Texture2D>("Player/playercorpse");
                    break;
            }
            //Health stuff.
            Head = new Limb(ref rGen, "Head", "Head", base_hp - 2);
            Torso = new Limb(ref rGen, "Chest", "Chest", base_hp);
            R_Arm = new Limb(ref rGen, "Right Arm", "RArm", base_hp);
            L_Arm = new Limb(ref rGen, "Left Arm", "LArm", base_hp);
            R_Leg = new Limb(ref rGen, "Right Leg", "RLeg", base_hp);
            L_Leg = new Limb(ref rGen, "Left Leg", "LLeg", base_hp);
            calculate_dodge_chance();
            //Inventory stuff
            main_hand = new Weapon(0, 100, "Knife", Weapon.Type.Sword, 1, 2, 4, 1);
            off_hand = null;
            over_armor = new Armor(1, 100, "Shoddy Leather", 0, 1, 2, 1, 1, 3, Armor.Armor_Type.OverArmor);
            under_armor = new Armor(2, 100, "Linen Rags", 0, 2, 2, 0, 0, 2, Armor.Armor_Type.UnderArmor);
            inventory = new List<Item>();
            //Character stuff
            BuffDebuffTracker = new List<StatusEffect>();

            pDoll = pd;
        }

        public string my_chara_as_string()
        {
            switch (my_character)
            {
                case Character.Falsael:
                    return "Falsael";
                case Character.Ziktofel:
                    return "Ziktofel";
                case Character.Halephon:
                    return "Halephon";
                case Character.Petaer:
                    return "Petaer";
            }

            return "Default";
        }

        public Character my_chara()
        {
            return my_character;
        }

        public string my_class_as_string()
        {
            switch (my_class)
            {
                case Chara_Class.Warrior:
                    return "Warrior";
                case Chara_Class.Mage:
                    return "Mage";
                case Chara_Class.Rogue:
                    return "Rogue";
                case Chara_Class.ExPriest:
                    return "ExPriest";
            }

            return "Error";
        }

        public void drawMe(ref SpriteBatch sb)
        {
            if (is_alive())
                sb.Draw(my_Texture, my_Position, Color.White);
            else
                sb.Draw(my_dead_texture, my_Position, Color.White);
        }

        public void move(gridCoordinate.direction dir, Floor fl)
        {
            int MonsterID = -1;
            int DoodadID = -1;
            gridCoordinate test_coord = new gridCoordinate(my_grid_coord);
            test_coord.shift_direction(dir);

            if (is_spot_free(fl, test_coord))
            {
                my_grid_coord = test_coord;
                reset_my_drawing_position();
            }
            else
            {
                //Check for monsters / doodads first.
                fl.is_monster_here(test_coord, out MonsterID);
                fl.is_destroyable_Doodad_here(test_coord, out DoodadID);
                //Check to see if there's a door there. If there is, attempt to open it.
                fl.open_door_here(test_coord);
            }

            if (MonsterID != -1)
                melee_attack(fl, my_grid_coord, test_coord);

            if(DoodadID != -1)
                melee_attack(fl, my_grid_coord, test_coord);
            //after moving, loot the current tile, and set sound / smell values.
            loot(fl);
            total_sound = my_sound_value();
            total_scent = my_scent_value();
        }

        public void wait()
        {
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
                    case Weapon.Type.Staff:
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
                    case Weapon.Type.Staff:
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
                int c_DoodadID;

                if (fl.is_monster_here(squares_to_attack_mh[i], out c_monsterID))
                    attack_monster_in_grid(fl, main_hand, c_monsterID, squares_to_attack_mh[i], 1.0);

                if (fl.is_destroyable_Doodad_here(squares_to_attack_mh[i], out c_DoodadID))
                    attack_Doodad_in_grid(fl, main_hand, c_DoodadID, squares_to_attack_mh[i], 1.0);

                if (main_hand != null)
                    fl.add_effect(main_hand.get_my_damage_type(), squares_to_attack_mh[i]);
                else
                    fl.add_effect(Attack.Damage.Crushing, squares_to_attack_mh[i]);
            }

            for (int i = 0; i < squares_to_attack_oh.Count; i++)
            {
                int c_monsterID;
                int c_DoodadID;

                if (fl.is_monster_here(squares_to_attack_oh[i], out c_monsterID))
                    attack_monster_in_grid(fl, off_hand, c_monsterID, squares_to_attack_oh[i], 1.0);

                if (fl.is_destroyable_Doodad_here(squares_to_attack_oh[i], out c_DoodadID))
                    attack_Doodad_in_grid(fl, off_hand, c_DoodadID, squares_to_attack_oh[i], 1.0);

                fl.add_effect(off_hand.get_my_damage_type(), squares_to_attack_oh[i]);
            }

            for (int i = 0; i < squares_to_attack_both.Count; i++)
            {
                int c_monsterID;
                int c_DoodadID;

                if (fl.is_monster_here(squares_to_attack_both[i], out c_monsterID))
                {
                    attack_monster_in_grid(fl, main_hand, c_monsterID, squares_to_attack_both[i], 1.0);
                    if (main_hand.get_hand_count() != 2)
                        attack_monster_in_grid(fl, off_hand, c_monsterID, squares_to_attack_both[i], 1.0);
                }

                if (fl.is_destroyable_Doodad_here(squares_to_attack_both[i], out c_DoodadID))
                {
                    attack_Doodad_in_grid(fl, main_hand, c_DoodadID, squares_to_attack_both[i], 1.0);
                    if (main_hand.get_hand_count() != 2)
                        attack_Doodad_in_grid(fl, off_hand, c_DoodadID, squares_to_attack_both[i], 1.0);
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

        public void attack_monster_in_grid(Floor fl, Weapon w, int c_monsterID, gridCoordinate current_gc, double multiplier, bool charge_attack = false)
        {
            List<Attack> modified_attacks = new List<Attack>();
            if (fl.badguy_by_monster_id(c_monsterID) != null)
            {
                if (w != null)
                {
                    string attack_msg = "You attack the " + fl.badguy_by_monster_id(c_monsterID).my_name + " with your " + w.get_my_name() + "!";
                    message_buffer.Add(attack_msg);
                    List<Attack> attacks = new List<Attack>();
                    List<StatusEffect> debuffs = new List<StatusEffect>();

                    bool aoe_effect = false;
                    if (w.get_my_weapon_type() == Weapon.Type.Axe || w.get_my_weapon_type() == Weapon.Type.Lance)
                        aoe_effect = true;

                    handle_attack_damage(w, null, multiplier, charge_attack, ref attacks, ref debuffs);
                    fl.damage_monster(attacks, debuffs, c_monsterID, true, aoe_effect);
                }
                else //Unarmed attack.
                {
                    double base_dmg_val = (double)rGen.Next(1, 4) * multiplier;
                    int modified_dmg_val = (int)base_dmg_val;
                    if (my_character == Character.Falsael)
                        modified_dmg_val = (int)Math.Ceiling(base_dmg_val * 1.2);

                    modified_attacks.Add(new Attack(Attack.Damage.Crushing, modified_dmg_val));
                    string attack_msg = "You attack the " + fl.badguy_by_monster_id(c_monsterID).my_name + " with your fists!";
                    message_buffer.Add(attack_msg);
                    fl.damage_monster(modified_attacks, null, c_monsterID, true, false);
                }
            }
        }

        public void attack_Doodad_in_grid(Floor fl, Weapon w, int c_DoodadID, gridCoordinate current_gc, double multiplier, bool charge_attack = false)
        {
            if (w != null)
            {
                string attack_msg = "You attack the " + fl.Doodad_by_index(c_DoodadID).my_name() + " with your " + w.get_my_name() + "!";
                message_buffer.Add(attack_msg);
                List<Attack> attacks = new List<Attack>();
                List<StatusEffect> debuffs = new List<StatusEffect>();
                handle_attack_damage(w, null, multiplier, charge_attack, ref attacks, ref debuffs);

                for (int i = 0; i < attacks.Count; i++)
                {
                    fl.damage_Doodad(attacks[i].get_damage_amt(), c_DoodadID);
                }
            }
            else
            {
                double base_dmg_val = (double)rGen.Next(1, 4) * multiplier;
                int modified_dmg_val = (int)base_dmg_val;
                if (my_character == Character.Falsael)
                    modified_dmg_val = (int)Math.Ceiling(base_dmg_val * 1.2);

                string attack_msg = "You attack the " + fl.Doodad_by_index(c_DoodadID).my_name() + " with your fists!";
                message_buffer.Add(attack_msg);
                fl.damage_Doodad(modified_dmg_val, c_DoodadID);
            }
        }

        public void set_ranged_attack_aura(Floor fl, gridCoordinate pl_gc, Scroll s)
        {
            int bow_range = 0;
            if (s == null)
            {
                if (main_hand != null && (main_hand.get_my_weapon_type() == Weapon.Type.Bow ||
                                          main_hand.get_my_weapon_type() == Weapon.Type.Crossbow))
                    bow_range = main_hand.get_my_range();
                else
                    bow_range = off_hand.get_my_range();
            }
            else
                bow_range = s.get_range();

            List<gridCoordinate> endpoints = new List<gridCoordinate>();
            if ((is_bow_equipped() && !is_cbow_equipped()) || s != null)
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
                    
                    if (fl.is_tile_passable(current_ray_position) && (x_difference > 1 || y_difference > 1))
                            fl.set_tile_aura(current_ray_position, Tile.Aura.Attack);

                    if (!fl.is_tile_passable(current_ray_position) ||
                        fl.is_los_blocking_Doodad_here(current_ray_position))
                    {
                        remove = true;
                        if(s != null && s.spell_destroys_walls())
                            fl.set_tile_aura(current_ray_position, Tile.Aura.Attack);
                    }

                    if ((main_hand != null && main_hand.get_my_weapon_type() == Weapon.Type.Crossbow) ||
                        (off_hand != null && off_hand.get_my_weapon_type() == Weapon.Type.Crossbow))
                        if (fl.is_monster_here(current_ray_position, out monsterID) ||
                            fl.is_destroyable_Doodad_here(current_ray_position, out whoCares))
                            remove = true;

                    if (range_rays[i].is_at_end() || remove)
                        range_rays.RemoveAt(i);
                }
            }
        }

        public void bow_attack(Floor fl, ref ContentManager Secondary_cManager, gridCoordinate attack_location, int monsterID, int DoodadID)
        {
            Weapon Bow = null;
            if (main_hand != null && (main_hand.get_my_weapon_type() == Weapon.Type.Bow ||
                                     main_hand.get_my_weapon_type() == Weapon.Type.Crossbow))
                Bow = main_hand;
            else if (off_hand != null && (off_hand.get_my_weapon_type() == Weapon.Type.Bow ||
                                          off_hand.get_my_weapon_type() == Weapon.Type.Crossbow))
                Bow = off_hand;

            gridCoordinate opposition_coord = new gridCoordinate(-1, -1);
            if (monsterID != -1)
                opposition_coord = attack_location;
            else
                opposition_coord = fl.Doodad_by_index(DoodadID).get_g_coord();

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

            int base_min_dmg_to_monster = Bow.specific_damage_val(false);
            int base_max_dmg_to_monster = Bow.specific_damage_val(true);

            int max_dmg_to_monster = (int)base_max_dmg_to_monster;
            int min_dmg_to_monster = (int)base_min_dmg_to_monster;

            if (is_cbow_equipped() && my_character == Character.Falsael)
            {
                max_dmg_to_monster = (int)Math.Ceiling(base_max_dmg_to_monster * 1.2);
                min_dmg_to_monster = (int)Math.Ceiling(base_min_dmg_to_monster * 1.2);
            }

            if (Bow.get_my_weapon_type() == Weapon.Type.Bow)
            {
                Projectile prj = new Projectile(get_my_grid_C(), opposition_coord, Projectile.projectile_type.Arrow, ref Secondary_cManager, false, Scroll.Atk_Area_Type.singleTile);
                prj.attach_weapon(Bow);
                fl.create_new_projectile(prj);
            }
            else if(Bow.get_my_weapon_type() == Weapon.Type.Crossbow)
            {
                Projectile prj = new Projectile(get_my_grid_C(), opposition_coord, Projectile.projectile_type.Crossbow_Bolt, ref Secondary_cManager, false, Scroll.Atk_Area_Type.smallfixedAOE);
                List<gridCoordinate> crossbow_aoe = new List<gridCoordinate>();
                crossbow_aoe.Add(opposition_coord);
                crossbow_aoe.Add(splash_coord);
                prj.set_small_AOE_matrix(crossbow_aoe);
                prj.attach_weapon(Bow);
                fl.create_new_projectile(prj);
            }

            string attack_msg = "";
            if (monsterID != -1)
                attack_msg = "You attack the " + fl.badguy_by_monster_id(monsterID).my_name + " with your " + Bow.get_my_name() + "!";
            else
                attack_msg = "You attack the " + fl.Doodad_by_index(DoodadID).my_name() + " with your " + Bow.get_my_name() + "!";
            message_buffer.Add(attack_msg);

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
                        fl.is_destroyable_Doodad_here(current_ray_position, out whoCares))
                        fl.set_tile_aura(current_ray_position, Tile.Aura.Attack);

                    if (!fl.isWalkable(current_ray_position) || fl.is_monster_here(current_ray_position, out monsterID))
                        remove = true;

                    if (charge_rays[i].is_at_end() || remove)
                        charge_rays.RemoveAt(i);
                }
            }
        }

        public void charge_attack(Floor fl, Weapon lance, gridCoordinate charge_coordinate, int monsterID, int DoodadID)
        {
            bool attacked_Doodad = false;
            gridCoordinate my_original_position = new gridCoordinate(my_grid_coord);
            Weapon c_lance = lance;

            VisionRay attack_ray = null;
            if(fl.badguy_by_monster_id(monsterID) == null)
            {
                attack_ray = new VisionRay(my_grid_coord, fl.Doodad_by_index(DoodadID).get_g_coord());
            }
            else
                attack_ray = new VisionRay(my_grid_coord, charge_coordinate);

            gridCoordinate monster_coord = new gridCoordinate(-1, -1);
            gridCoordinate Doodad_coord = new gridCoordinate(-1, -1);

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
                fl.is_destroyable_Doodad_here(next_ray_position, out dood_ID);
                if (mon_ID == monsterID && monsterID > -1)
                {
                    monster_coord = new gridCoordinate(charge_coordinate);
                    teleport(previous_ray_position);
                    attack_monster_in_grid(fl, lance, monsterID, charge_coordinate, 1.0, true);
                    done = true;
                }

                if (dood_ID == DoodadID && DoodadID > -1)
                {
                    attacked_Doodad = true;
                    Doodad_coord = new gridCoordinate(fl.Doodad_by_index(DoodadID).get_g_coord());
                    teleport(previous_ray_position);
                    attack_Doodad_in_grid(fl, lance, DoodadID, fl.Doodad_by_index(DoodadID).get_g_coord(), 1.0, true);
                    done = true;
                }
            }

            gridCoordinate opposition_coord = new gridCoordinate(-1, -1);
            if (attacked_Doodad)
                opposition_coord = Doodad_coord;
            else
                opposition_coord = monster_coord;

            if (!is_spot_free(fl, my_grid_coord))
            {
                int xdif = my_original_position.x - opposition_coord.x;
                int ydif = my_original_position.y - opposition_coord.y;

                int whocares = -1;
                if (fl.is_monster_here(my_grid_coord, out whocares) || fl.is_destroyable_Doodad_here(my_grid_coord, out whocares))
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
        
        public void set_melee_attack_aura(Floor fl)
        {
            for (int x = my_grid_coord.x - 1; x <= my_grid_coord.x + 1; x++)
                for (int y = my_grid_coord.y - 1; y <= my_grid_coord.y + 1; y++)
                {
                    gridCoordinate target_coord = new gridCoordinate(x, y);
                    if (!(x == my_grid_coord.x && y == my_grid_coord.y) && fl.is_tile_passable(target_coord))
                        fl.set_tile_aura(target_coord, Tile.Aura.Attack);
                }
        }

        public void set_melee_interact_aura(Floor fl)
        {
            for (int x = my_grid_coord.x - 1; x <= my_grid_coord.x + 1; x++)
                for (int y = my_grid_coord.y - 1; y <= my_grid_coord.y + 1; y++)
                {
                    gridCoordinate target_coord = new gridCoordinate(x, y);
                    if (!(x == my_grid_coord.x && y == my_grid_coord.y) && fl.is_tile_passable(target_coord))
                        fl.set_tile_aura(target_coord, Tile.Aura.Interact);
                }
        }

        public void bash_attack(Floor fl, Monster m, gridCoordinate target_coord, Doodad d, Weapon wp)
        {
            int xdif = 0;
            int ydif = 0;
            if (m != null)
            {
                xdif = target_coord.x - my_grid_coord.x;
                ydif = target_coord.y - my_grid_coord.y;
            }
            else
            {
                xdif = d.get_g_coord().x - my_grid_coord.x;
                ydif = d.get_g_coord().y - my_grid_coord.y;
            }

            double multiplier = 1.4;
            if (wp.get_hand_count() == 2)
                multiplier = 1.6;

            int m_hp = 0;
            if(m!= null)
                m_hp = m.hitPoints;

            gridCoordinate opposition_coord = new gridCoordinate(-1, -1);
            if(m != null)
                opposition_coord = target_coord;
            else
                opposition_coord = d.get_g_coord();

            fl.add_effect(wp.get_my_damage_type(), opposition_coord);
            if (m != null)
                attack_monster_in_grid(fl, wp, m.my_Index, target_coord, multiplier);
            else
                attack_Doodad_in_grid(fl, wp, d.get_my_index(), d.get_g_coord(), multiplier);

            if (m!= null && m.hitPoints > 0 && m_hp > m.hitPoints)
            {
                if(!m.shove(xdif, ydif, this, fl))
                {
                    fl.add_effect(wp.get_my_damage_type(), target_coord);
                    attack_monster_in_grid(fl, wp, m.my_Index, target_coord, 0.5);
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
            int Doodad_ID = -1;
            for (int i = 0; i < target_coordinates.Count; i++)
            {
                double multiplier = .8;
                if (target_weapon.get_hand_count() == 2)
                    multiplier = .9;

                if (fl.is_monster_here(target_coordinates[i], out monster_ID))
                    attack_monster_in_grid(fl, target_weapon, monster_ID, target_coordinates[i], multiplier);

                if (fl.is_destroyable_Doodad_here(target_coordinates[i], out Doodad_ID))
                    attack_Doodad_in_grid(fl, target_weapon, Doodad_ID, target_coordinates[i], multiplier);

                if (fl.isWalkable(target_coordinates[i]))
                    fl.add_effect(target_weapon.get_my_damage_type(), target_coordinates[i]);
            }
            target_weapon.set_cooldown(standard_wpn_cooldown);
        }

        public void cast_spell(Scroll s, Floor fl, gridCoordinate spell_target, int target_monster_ID, int target_Doodad_ID)
        {
            string spell_name = s.get_my_name();
            Projectile.projectile_type prj_type = s.get_assoc_projectile();
            Attack.Damage spell_dmg_type = s.get_damage_type();
            gridCoordinate starting_coord = my_grid_coord;
            Projectile.special_anim spec_prj_anim = s.get_spec_impact_anim();
 
            if (s.get_spell_type() == Scroll.Atk_Area_Type.piercingBolt)
            {
                int spell_range = s.get_range();
                int relative_x = (spell_target.x - my_grid_coord.x) * spell_range;
                int relative_y = (spell_target.y - my_grid_coord.y) * spell_range;
                starting_coord = new gridCoordinate(spell_target);
                spell_target = new gridCoordinate(my_grid_coord.x + relative_x, my_grid_coord.y + relative_y);
            }

            if (s.get_spell_type() != Scroll.Atk_Area_Type.personalBuff)
            {
                Projectile prj = new Projectile(starting_coord, spell_target, prj_type,
                                                 ref cont, false, s.get_spell_type());
                prj.attach_scroll(s);
                prj.set_wall_destroying(s.spell_destroys_walls());
                prj.set_special_anim(spec_prj_anim);

                if (s.get_spell_type() == Scroll.Atk_Area_Type.enemyDebuff)
                {
                    prj.attach_status_effect(s.get_status_effect(), s.get_duration());
                }

                if (s.get_spell_type() == Scroll.Atk_Area_Type.cloudAOE ||
                    s.get_spell_type() == Scroll.Atk_Area_Type.solidblockAOE ||
                    s.get_spell_type() == Scroll.Atk_Area_Type.randomblockAOE)
                    prj.set_AOE_size(s.get_aoe_size());

                if (s.get_spell_type() == Scroll.Atk_Area_Type.chainedBolt)
                {
                    prj.set_bounce(s.get_range());
                    prj.set_bounces_left(s.get_t_impacts());
                }

                if (String.Compare(s.get_my_name(), "Earthquake") == 0)
                    prj.set_special_anim(Projectile.special_anim.Earthquake);

                prj.set_talisman_effects(s.get_my_equipped_talismans());
                fl.create_new_projectile(prj);
            }
            else
                add_single_statusEffect(new StatusEffect(s.get_status_effect(), s.get_duration()+1));
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
            lifetime_gold += gold_amt;
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
                Weapon acquired_weapon = new Weapon((Weapon)thing);
                inventory.Add(acquired_weapon);
            }
            else if (thing is Scroll)
            {
                Scroll acquired_scroll = new Scroll((Scroll)thing);
                inventory.Add(acquired_scroll);
            }
            else if (thing is Talisman)
            {
                Talisman talisman_thing = (Talisman)thing;
                Talisman acquired_talisman = new Talisman(talisman_thing.get_my_IDno(),
                                                          talisman_thing.get_my_gold_value(),
                                                          talisman_thing.get_my_name(), talisman_thing);
                inventory.Add(acquired_talisman);
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

        #region repair, heal and refill

        public void heal_naturally()
        {
            Head.heal_via_potion(1);
            L_Arm.heal_via_potion(1);
            L_Leg.heal_via_potion(1);
            R_Arm.heal_via_potion(1);
            R_Leg.heal_via_potion(1);
            Torso.heal_via_potion(1);

            calculate_dodge_chance();
            update_pdoll();
        }

        public void heal_via_potion(Potion pt, string bodypart, bool repair_over_armor, Floor fl)
        {
            double potion_potency = (double)pt.potion_potency();
            if (my_character == Character.Ziktofel)
                potion_potency = Math.Ceiling((double)pt.potion_potency() * 1.6);
            int heal_value = (int)potion_potency;

            if (pt.get_type() == Potion.Potion_Type.Health)
            {
                Limb target_limb = null;
                switch (bodypart)
                {
                    case "Head":
                        target_limb = Head;
                        break;
                    case "Chest":
                        target_limb = Torso;
                        break;
                    case "LArm":
                        target_limb = L_Arm;
                        break;
                    case "RArm":
                        target_limb = R_Arm;
                        break;
                    case "LLeg":
                        target_limb = L_Leg;
                        break;
                    case "RLeg":
                        target_limb = R_Leg;
                        break;
                }

                target_limb.heal_via_potion(heal_value);
                fl.add_new_popup("+" + heal_value + " " + target_limb.get_shortname(), Popup.popup_msg_color.VividGreen, my_grid_coord);
            }
            else if (pt.get_type() == Potion.Potion_Type.Repair)
            {
                if (String.Compare(bodypart, "Head") == 0)
                    helm.repair_by_zone(heal_value, bodypart);

                if (repair_over_armor)
                    over_armor.repair_by_zone(heal_value, bodypart);
                else
                    under_armor.repair_by_zone(heal_value, bodypart);
                fl.add_new_popup("+" + pt.potion_potency() + " " + bodypart, Popup.popup_msg_color.Blue, my_grid_coord);
            }

            calculate_dodge_chance();
            update_pdoll();
            pt.drink();
        }

        public void ingest_potion(Potion pt, Floor fl, bool repair_over_armor)
        {
            double base_potency = (double)pt.potion_potency() * 1.6;
            if (my_character == Character.Ziktofel)
                base_potency *= 1.6;
            int potency = (int)Math.Ceiling(base_potency);

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
                                Head.heal_via_potion(1);
                                head_tally++;
                                potency--;
                            }
                            break;
                        case 1:
                            if (!Torso.is_uninjured())
                            {
                                Torso.heal_via_potion(1);
                                chest_tally++;
                                potency--;
                            }
                            break;
                        case 2:
                            if (!R_Arm.is_uninjured())
                            {
                                R_Arm.heal_via_potion(1);
                                rarm_tally++;
                                potency--;
                            }
                            break;
                        case 3:
                            if (!L_Arm.is_uninjured())
                            {
                                L_Arm.heal_via_potion(1);
                                larm_tally++;
                                potency--;
                            }
                            break;
                        case 4:
                            if (!R_Leg.is_uninjured())
                            {
                                R_Leg.heal_via_potion(1);
                                rleg_tally++;
                                potency--;
                            }
                            break;
                        case 5:
                            if (!L_Leg.is_uninjured())
                            {
                                L_Leg.heal_via_potion(1);
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
                if (chest_tally > 0)
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
            update_pdoll();

            if (pt.get_type() == Potion.Potion_Type.Repair && target_armor != null)
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

            for (int i = inventory.Count - 1; i >= 0; i--)
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

        public bool is_alive()
        {
            return !Head.is_disabled() && !Torso.is_disabled();
        }

        //Wounding stuff
        public void wound_report(out int[] wounds_by_part, out int[] max_health_by_part)
        {
            wounds_by_part = new int[6];
            wounds_by_part[0] = Head.count_inj_severity_factor();
            wounds_by_part[1] = Torso.count_inj_severity_factor();
            wounds_by_part[2] = L_Arm.count_inj_severity_factor();
            wounds_by_part[3] = R_Arm.count_inj_severity_factor();
            wounds_by_part[4] = L_Leg.count_inj_severity_factor();
            wounds_by_part[5] = R_Leg.count_inj_severity_factor();

            max_health_by_part = new int[6];
            max_health_by_part[0] = Head.get_max_health();
            max_health_by_part[1] = Torso.get_max_health();
            max_health_by_part[2] = L_Arm.get_max_health();
            max_health_by_part[3] = R_Arm.get_max_health();
            max_health_by_part[4] = L_Leg.get_max_health();
            max_health_by_part[5] = R_Leg.get_max_health();
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
            bodyPart.consolidate_injury_report(ref wReport);
            if (bodyPart.is_disabled())
                wReport.Add("It is useless");
        }

        #endregion

        #region taking damage + buff/debuff management

        public void take_damage(Attack atk, Floor fl, string specific_part)
        {
            //OKAY THIS IS GONNA BE COMPLICATED.
            //First, figure out where the attack is gonna hit. The breakdown is as follows:
            //head, 5%, chest = 25%, arm = 17%, leg = 18%
            int hit_location = rGen.Next(100);
            switch (specific_part)
            {
                case "Head":
                    hit_location = 0;
                    break;
                case "Chest":
                    hit_location = 76;
                    break;
                case "LArm":
                    hit_location = 23;
                    break;
                case "RArm":
                    hit_location = 6;
                    break;
                case "LLeg":
                    hit_location = 58;
                    break;
                case "RLeg":
                    hit_location = 40;
                    break;
            }

            int dodge_roll = rGen.Next(100);
            bool dodged = false;

            if (dodge_roll < dodge_chance)
            {
                dodged = true;
                fl.add_new_popup("Dodged!", Popup.popup_msg_color.LimeGreen, my_grid_coord);
                message_buffer.Add("You dodge the attack!");
            }

            if (!dodged)
            {
                bool head_shot = false;
                Armor.Attack_Zone atkzone = 0;
                Limb target_limb = null;

                if (hit_location < 5 && !Head.is_disabled())
                {
                    head_shot = true;
                    target_limb = Head;
                }
                else if (hit_location >= 5 && hit_location < 22 && !R_Arm.is_disabled())
                {
                    atkzone = Armor.Attack_Zone.R_Arm;
                    target_limb = R_Arm;
                }
                else if (hit_location >= 22 && hit_location < 39 && !L_Arm.is_disabled())
                {
                    atkzone = Armor.Attack_Zone.L_Arm;
                    target_limb = L_Arm;
                }
                else if (hit_location >= 39 && hit_location < 57 && !R_Leg.is_disabled())
                {
                    atkzone = Armor.Attack_Zone.R_Leg;
                    target_limb = R_Leg;
                }
                else if (hit_location >= 57 && hit_location < 75 && !L_Leg.is_disabled())
                {
                    atkzone = Armor.Attack_Zone.L_Leg;
                    target_limb = L_Leg;
                }
                else
                {
                    atkzone = Armor.Attack_Zone.Chest;
                    target_limb = Torso;
                }

                if (!head_shot)
                {
                    if (over_armor != null)
                        atk = over_armor.absorb_damage(atk, atkzone, my_grid_coord, ref rGen, ref message_buffer, ref fl);
                    if (under_armor != null)
                        atk = under_armor.absorb_damage(atk, atkzone, my_grid_coord, ref rGen, ref message_buffer, ref fl);
                }
                else
                {
                    if(helm != null)
                        atk = helm.absorb_damage(atk, atkzone, my_grid_coord, ref rGen, ref message_buffer, ref fl);
                }

                int dmg = atk.get_damage_amt();
                target_limb.add_injury(atk.get_dmg_type(), dmg);
                if (dmg > 0)
                        fl.add_new_popup("-" + dmg + " " + target_limb.get_shortname(), Popup.popup_msg_color.Red, my_grid_coord);
                    message_buffer.Add("Your " + target_limb.get_longname() + " takes " + dmg + " wounds!");
            }

            handle_postdamage_calculations();

            if (!is_alive())
                message_buffer.Add("Your wounds are too much for you. You collapse and your vision fades.");
        }

        //this is used for hemorrhage.
        public void damage_random_part(int dmg, Attack.Damage dmgTyp, Floor fl)
        {
            int chosen_part = rGen.Next(6);
            Limb target_part = null;
            switch (chosen_part)
            {
                case 0:
                    target_part = Head;
                    break;
                case 1:
                    target_part = L_Arm;
                    break;
                case 2:
                    target_part = R_Arm;
                    break;
                case 3:
                    target_part = L_Leg;
                    break;
                case 4:
                    target_part = R_Leg;
                    break;
                case 5:
                    target_part = Torso;
                    break;
            }
            if (target_part.is_disabled())
                target_part = Torso;

            target_part.add_injury(dmgTyp, dmg);
            fl.add_new_popup("-1 " + target_part.get_shortname(), Popup.popup_msg_color.Red, my_grid_coord);
            message_buffer.Add("Your " + target_part.get_longname() + " takes " + dmg.ToString() + " wounds.");

            handle_postdamage_calculations();
        }

        public void take_aoe_damage(int min_dmg, int max_dmg, 
                                    Attack.Damage dmg_type, Floor fl)
        {
            List<Limb> target_limbs = new List<Limb>();
            List<Armor.Attack_Zone> target_zones = new List<Armor.Attack_Zone>();

            //Head and chest
            int hc_dodge_roll = rGen.Next(100);
            if (hc_dodge_roll < dodge_chance)
            {
                fl.add_new_popup("Dodged!", Popup.popup_msg_color.LimeGreen, my_grid_coord);
                message_buffer.Add("You dodge the attack!");
            }
            else
            {
                
                int h_wnd = rGen.Next(min_dmg, max_dmg+1);
                Attack head_attack = new Attack(dmg_type, h_wnd);
                if (helm != null)
                    head_attack = helm.absorb_damage(head_attack, Armor.Attack_Zone.Head, my_grid_coord, 
                                                     ref rGen, ref message_buffer, ref fl);
                Head.add_injury(dmg_type, h_wnd);

                target_limbs.Add(Torso);
                target_zones.Add(Armor.Attack_Zone.Chest);
            }

            //R Arm / Leg
            int rs_dodge_roll = rGen.Next(100);
            if (rs_dodge_roll < dodge_chance)
            {
                fl.add_new_popup("Dodged!", Popup.popup_msg_color.LimeGreen, my_grid_coord);
                message_buffer.Add("You dodge the attack!");
            }
            else
            {
                target_limbs.Add(R_Arm);
                target_zones.Add(Armor.Attack_Zone.R_Arm);
                target_limbs.Add(R_Leg);
                target_zones.Add(Armor.Attack_Zone.R_Leg);
            }

            //L Arm / Leg
            int ls_dodge_roll = rGen.Next(100);
            if (ls_dodge_roll < dodge_chance)
            {
                fl.add_new_popup("Dodged!", Popup.popup_msg_color.LimeGreen, my_grid_coord);
                message_buffer.Add("You dodge the attack!");
            }
            else
            {
                target_limbs.Add(L_Arm);
                target_zones.Add(Armor.Attack_Zone.L_Arm);
                target_limbs.Add(L_Leg);
                target_zones.Add(Armor.Attack_Zone.L_Leg);
            }

            for (int i = 0; i < target_limbs.Count; i++)
            {
                Attack next_attack = new Attack(dmg_type, rGen.Next(min_dmg, max_dmg+1));
                if (over_armor != null)
                    next_attack = over_armor.absorb_damage(next_attack, target_zones[i], my_grid_coord, ref rGen, ref message_buffer, ref fl);
                if (under_armor != null)
                    next_attack = under_armor.absorb_damage(next_attack, target_zones[i], my_grid_coord, ref rGen, ref message_buffer, ref fl);

                target_limbs[i].add_injury(next_attack.get_dmg_type(), next_attack.get_damage_amt());
                if (next_attack.get_damage_amt() > 0)
                    fl.add_new_popup("-" + next_attack.get_damage_amt() + " " + target_limbs[i].get_shortname(), Popup.popup_msg_color.Red, my_grid_coord);
                message_buffer.Add("Your " + target_limbs[i].get_longname() + " takes " + next_attack.get_damage_amt() + " wounds!");
            }

            handle_postdamage_calculations();
        }

        public void add_single_statusEffect(StatusEffect se)
        {
            if (se.my_type == Scroll.Status_Type.PantherFer ||
               se.my_type == Scroll.Status_Type.TigerFer)
                remove_specific_effect(Scroll.Status_Type.LynxFer);
            if (se.my_type == Scroll.Status_Type.TigerFer)
                remove_specific_effect(Scroll.Status_Type.PantherFer);

            int sEffect_index = check_for_status_effect(se);
            if (sEffect_index >= 0)
                BuffDebuffTracker[sEffect_index].my_duration = se.my_duration;
            else
                BuffDebuffTracker.Add(se);
        }

        public void remove_specific_effect(Scroll.Status_Type effect)
        {
            for (int i = 0; i < BuffDebuffTracker.Count; i++)
                if (BuffDebuffTracker[i].my_type == effect)
                    BuffDebuffTracker.RemoveAt(i);
        }

        public int check_for_status_effect(StatusEffect se)
        {
            int target_index = -1;
            for (int i = 0; i < BuffDebuffTracker.Count; i++)
                if (se.my_type == BuffDebuffTracker[i].my_type)
                    target_index = i;

            return target_index;
        }

        public bool body_part_disabled(string bpart)
        {
            switch (bpart)
            {
                case "Head":
                    return Head.is_disabled();
                case "Chest":
                case "Torso":
                    return Torso.is_disabled();
                case "LArm":
                    return L_Arm.is_disabled();
                case "RArm":
                    return R_Arm.is_disabled();
                case "LLeg":
                    return L_Leg.is_disabled();
                case "RLeg":
                    return R_Leg.is_disabled();
            }

            return false;
        }

        #endregion

        public void teleport(gridCoordinate gc)
        {
            my_grid_coord = new gridCoordinate(gc);
            reset_my_drawing_position();
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
        public bool is_spot_free(Floor fl, gridCoordinate test_coord)
        {
            int whoCares;
            return (fl.isWalkable(test_coord) && !fl.is_monster_here(test_coord, out whoCares));
        }
        
        //Green text. Function here.
        public bool is_spot_exit(Floor fl)
        {
            return fl.isExit(my_grid_coord);
        }

        public bool is_spot_dungeon_exit(Floor fl)
        {
            return fl.isDungeonExit(my_grid_coord);
        }

        //Int returns - damage value + scent values are here.
        //Green text. Function here.
        public int get_my_gold()
        {
            return my_gold;
        }

        public int get_my_lifetime_gold()
        {
            return lifetime_gold;
        }

        //Green text. Function here.
        public int my_scent_value()
        {
            int total_burn_wounds = 0;
            int total_open_wounds = 0;

            total_open_wounds += Head.get_open_wounds();
            total_open_wounds += Torso.get_open_wounds();
            total_open_wounds += L_Arm.get_open_wounds();
            total_open_wounds += R_Arm.get_open_wounds();
            total_open_wounds += L_Leg.get_open_wounds();
            total_open_wounds += R_Leg.get_open_wounds();

            total_burn_wounds += Head.get_burn_wounds();
            total_burn_wounds += Torso.get_burn_wounds();
            total_burn_wounds += L_Arm.get_burn_wounds();
            total_burn_wounds += R_Arm.get_burn_wounds();
            total_burn_wounds += L_Leg.get_burn_wounds();
            total_burn_wounds += R_Leg.get_burn_wounds();

            return base_smell_value + total_burn_wounds + (total_open_wounds/2);
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
        
        //Inventory stuff
        public int calc_absorb_chance(int primary_resist, int secondary_resist)
        {
            return ((4 * primary_resist) + (2 * secondary_resist));
        }

        public List<string> detailed_equip_report()
        {
            List<string> eRep = new List<string>();

            int oa_ab_all;
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
                oa_ab_all = over_armor.get_armor_value(Armor.Armor_Value.Absorb_All);
                oa_ab_val = over_armor.get_armor_value(Armor.Armor_Value.Ablative);
                oa_in_val = over_armor.get_armor_value(Armor.Armor_Value.Insulative);
                oa_pa_val = over_armor.get_armor_value(Armor.Armor_Value.Padding);
                oa_ha_val = over_armor.get_armor_value(Armor.Armor_Value.Hardness);
                oa_rg_val = over_armor.get_armor_value(Armor.Armor_Value.Rigidness);

                oa_chest_integ = over_armor.get_chest_integ();
                oa_rarm_integ = over_armor.get_rarm_integ();
                oa_larm_integ = over_armor.get_larm_integ();
                oa_rleg_integ = over_armor.get_rleg_integ();
                oa_lleg_integ = over_armor.get_lleg_integ();

                eRep.Add("Over Armor: " + over_armor.get_my_name());
                eRep.Add(" ");
                eRep.Add("Chance to absorb slashing attack: " + (calc_absorb_chance(oa_ha_val, oa_rg_val)+oa_ab_all) + "%");
                eRep.Add("Chance to absorb crushing attack: " + (calc_absorb_chance(oa_rg_val, oa_pa_val)+oa_ab_all) + "%");
                eRep.Add("Chance to absorb piercing attack: " + (calc_absorb_chance(oa_ha_val, oa_pa_val)+oa_ab_all) + "%");
                eRep.Add("Chance to absorb fire attack: " + (calc_absorb_chance(oa_ab_val, oa_rg_val)+oa_ab_all) + "%");
                eRep.Add("Chance to absorb frost attack: " + (calc_absorb_chance(oa_pa_val, oa_in_val)+oa_ab_all) + "%");
                eRep.Add("Chance to absorb electric attack: " + (calc_absorb_chance(oa_in_val, oa_pa_val)+oa_ab_all) + "%");
                eRep.Add("Chance to absorb acid attack: " + (calc_absorb_chance(oa_in_val, oa_ab_val)+oa_ab_all) + "%");
                eRep.Add(" ");
                eRep.Add("Integrity:");
                eRep.Add("Chest integrity: " + oa_chest_integ);
                eRep.Add("L Arm integrity: " + oa_larm_integ + " / R Arm integrity: " + oa_rarm_integ);
                eRep.Add("L Leg integrity: " + oa_lleg_integ + " / R Leg integrity: " + oa_rleg_integ);
                eRep.Add(" ");
            }

            int ua_ab_all;
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
                ua_ab_all = under_armor.get_armor_value(Armor.Armor_Value.Absorb_All);
                ua_ab_val = under_armor.get_armor_value(Armor.Armor_Value.Ablative);
                ua_in_val = under_armor.get_armor_value(Armor.Armor_Value.Insulative);
                ua_pa_val = under_armor.get_armor_value(Armor.Armor_Value.Padding);
                ua_ha_val = under_armor.get_armor_value(Armor.Armor_Value.Hardness);
                ua_rg_val = under_armor.get_armor_value(Armor.Armor_Value.Rigidness);

                ua_chest_integ = under_armor.get_chest_integ();
                ua_rarm_integ = under_armor.get_rarm_integ();
                ua_larm_integ = under_armor.get_larm_integ();
                ua_rleg_integ = under_armor.get_rleg_integ();
                ua_lleg_integ = under_armor.get_lleg_integ();

                eRep.Add("Under Armor: " + under_armor.get_my_name());
                eRep.Add(" ");
                eRep.Add("Chance to absorb slashing attack: " + (calc_absorb_chance(ua_ha_val, ua_rg_val)+ua_ab_all) + "%");
                eRep.Add("Chance to absorb crushing attack: " + (calc_absorb_chance(ua_rg_val, ua_pa_val)+ua_ab_all) + "%");
                eRep.Add("Chance to absorb piercing attack: " + (calc_absorb_chance(ua_ha_val, ua_pa_val)+ua_ab_all) + "%");
                eRep.Add("Chance to absorb fire attack: " + (calc_absorb_chance(ua_ab_val, ua_rg_val)+ua_ab_all) + "%");
                eRep.Add("Chance to absorb frost attack: " + (calc_absorb_chance(ua_pa_val, ua_in_val)+ua_ab_all) + "%");
                eRep.Add("Chance to absorb electric attack: " + (calc_absorb_chance(ua_in_val, ua_pa_val)+ua_ab_all) + "%");
                eRep.Add("Chance to absorb acid attack: " + (calc_absorb_chance(ua_in_val, ua_ab_val)+ua_ab_all) + "%");
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

        public void equip_armor(Armor a)
        {
            switch (a.what_armor_type())
            {
                case Armor.Armor_Type.Helmet:
                    equip_helmet(a);
                    break;
                case Armor.Armor_Type.OverArmor:
                    equip_over_armor(a);
                    break;
                case Armor.Armor_Type.UnderArmor:
                    equip_under_armor(a);
                    break;
            }

            calculate_dodge_chance();
            calculate_limb_HP();
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

        public void equip_helmet(Armor hm)
        {
            unequip(Equip_Slot.Helmet);

            helm = hm;

            remove_item_from_inventory(helm.get_my_IDno());
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

        public Armor show_helmet()
        {
            return helm;
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
                case Equip_Slot.Helmet:
                    if (helm != null)
                        inventory.Add(helm);
                    helm = null;
                    break;
            }

            calculate_dodge_chance();
            calculate_limb_HP();
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
                        if (c_armor.what_armor_type() == Armor.Armor_Type.OverArmor)
                            return "Overarmor";
                        else if (c_armor.what_armor_type() == Armor.Armor_Type.UnderArmor)
                            return "Underarmor";
                        else
                            return "Helmet";
                    }
                    else if (inventory[i] is Potion)
                        return "Potion";
                    else if (inventory[i] is Scroll)
                        return "Scroll";
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

        public Scroll get_scroll_by_ID(int IDno)
        {
            for (int i = 0; i < inventory.Count; i++)
                if (inventory[i] is Scroll && inventory[i].get_my_IDno() == IDno)
                    return (Scroll)inventory[i];

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

        #region calculate stats area

        public void handle_postdamage_calculations()
        {
            update_pdoll();
            handle_limb_unequip();
            calculate_dodge_chance();
        }

        public void handle_limb_unequip()
        {
            if (L_Arm.is_disabled())
                unequip(Equip_Slot.Mainhand);

            if (R_Arm.is_disabled())
                unequip(Equip_Slot.Offhand);
        }

        public void calculate_dodge_chance()
        {
            dodge_chance = 0;
            if (!R_Leg.is_disabled())
                dodge_chance += 10;
            if (!L_Leg.is_disabled())
                dodge_chance += 10;

            List<Talisman> full_talisman_list = new List<Talisman>();
            if (over_armor != null)
                full_talisman_list.AddRange(over_armor.get_my_equipped_talismans());
            if (under_armor != null)
                full_talisman_list.AddRange(under_armor.get_my_equipped_talismans());
            if (helm != null)
                full_talisman_list.AddRange(helm.get_my_equipped_talismans());

            for (int i = 0; i < full_talisman_list.Count; i++)
            {
                if (full_talisman_list[i].get_my_type() == Talisman.Talisman_Type.Skill)
                {
                    dodge_chance++;
                    dodge_chance += (int)full_talisman_list[i].get_my_prefix();
                }
            }
        }

        public void calculate_limb_HP()
        {
            int bonus_vital_hp = 0;
            int bonus_nonvital_hp = 0;

            List<Talisman> all_armor_talismans = new List<Talisman>();
            if (helm != null)
                all_armor_talismans.AddRange(helm.get_my_equipped_talismans());
            if (over_armor != null)
                all_armor_talismans.AddRange(over_armor.get_my_equipped_talismans());
            if (under_armor != null)
                all_armor_talismans.AddRange(under_armor.get_my_equipped_talismans());

            for (int i = 0; i < all_armor_talismans.Count; i++)
            {
                int bonus = 0;
                if (all_armor_talismans[i].get_my_prefix() == Talisman.Talisman_Prefix.Average)
                    bonus = 1;
                else if (all_armor_talismans[i].get_my_prefix() == Talisman.Talisman_Prefix.Perfect)
                    bonus = 2;

                if (all_armor_talismans[i].get_my_type() == Talisman.Talisman_Type.Endurance)
                {
                    bonus_nonvital_hp += bonus;
                }
                else if (all_armor_talismans[i].get_my_type() == Talisman.Talisman_Type.Tenacity)
                {
                    bonus_vital_hp += bonus;
                }
            }

            Head.set_HP(base_hp + bonus_vital_hp - 2);
            Torso.set_HP(base_hp + bonus_vital_hp);
            L_Arm.set_HP(base_hp + bonus_nonvital_hp);
            R_Arm.set_HP(base_hp + bonus_nonvital_hp);
            L_Leg.set_HP(base_hp + bonus_nonvital_hp);
            R_Leg.set_HP(base_hp + bonus_nonvital_hp);
        }

        #endregion

        public void deincrement_cooldowns_and_seffects(Floor fl)
        {
            //Make all cooldowns go down by 1
            if (main_hand != null && main_hand.get_current_cooldown() > 0)
                main_hand.set_cooldown(-1);

            if ((off_hand != null && main_hand != null &&
                main_hand.get_hand_count() == 1 &&
                off_hand.get_current_cooldown() > 0) || (off_hand != null && main_hand == null))
                off_hand.set_cooldown(-1);

            for(int i = 0; i < inventory.Count; i++)
                if (inventory[i] is Weapon)
                {
                    Weapon w = (Weapon)inventory[i];
                    if (w.get_current_cooldown() > 0)
                        w.set_cooldown(-1);
                }

            //Now that that's done, we handle the status effect side of things.
            for (int i = 0; i < BuffDebuffTracker.Count; i++)
            {
                switch (BuffDebuffTracker[i].my_type)
                {
                    case Scroll.Status_Type.Hemorrhage:
                        damage_random_part(1, Attack.Damage.Slashing, fl);
                        break;
                }
                BuffDebuffTracker[i].my_duration--;
            }

            int original_size = BuffDebuffTracker.Count;
            for (int i = 0; i < original_size; i++)
                for (int j = 0; j < BuffDebuffTracker.Count; j++)
                    if (BuffDebuffTracker[j].my_duration == 0)
                        BuffDebuffTracker.RemoveAt(j);
        }

        public List<StatusEffect> get_status_effects()
        {
            return BuffDebuffTracker;
        }

        public void update_pdoll()
        {
            int[] wounds;
            int[] max_health;
            wound_report(out wounds, out max_health);
            pDoll.update_wound_report(wounds, max_health);
        }

        //Functions for handling attack lists and talismans
        public void handle_attack_damage(Weapon w, Scroll s, double initialMultiplier, bool charge_attack,
                                         ref List<Attack> attacksOut, ref List<StatusEffect> effectsOut)
        {
            int mindmg = 0;
            int maxdmg = 0;
            Attack.Damage dmgTyp = 0;
            List<Talisman> equipped_talismans = new List<Talisman>();
            List<Attack> temp_attacks = new List<Attack>();
            if (w != null)
            {
                mindmg = w.specific_damage_val(false) * w.get_hand_count();
                maxdmg = w.specific_damage_val(true) * w.get_hand_count();
                dmgTyp = w.get_my_damage_type();
                equipped_talismans = w.get_my_equipped_talismans();
            }
            else if (s != null)
            {
                mindmg = s.get_specific_damage(false);
                maxdmg = s.get_specific_damage(true);
                dmgTyp = s.get_damage_type();
                equipped_talismans = s.get_my_equipped_talismans();
            }

            int baseDamage = rGen.Next(mindmg, maxdmg + 1);
            //next we check for a talisman of expediency.
            for (int i = 0; i < equipped_talismans.Count; i++)
            {
                if (equipped_talismans[i].get_my_type() == Talisman.Talisman_Type.Expediency)
                {
                    int base_val = (int)equipped_talismans[i].get_my_prefix() + 2;
                    int min_damage_modifier = base_val;
                    int max_damage_modifier = (base_val * 2);
                    if (baseDamage > 0)
                        baseDamage += rGen.Next(min_damage_modifier, max_damage_modifier + 1);
                }
            }

            //Now we add any talisman based attacks.
            for (int i = 0; i < equipped_talismans.Count; i++)
            {
                if (equipped_talismans[i].extra_damage_specific_type_talisman())
                {
                    int base_val = (int)equipped_talismans[i].get_my_prefix() + 1;
                    Attack.Damage dmg_typ = 0;
                    switch (equipped_talismans[i].get_my_type())
                    {
                        case Talisman.Talisman_Type.Pressure:
                            dmg_typ = Attack.Damage.Crushing;
                            break;
                        case Talisman.Talisman_Type.Heat:
                            dmg_typ = Attack.Damage.Fire;
                            break;
                        case Talisman.Talisman_Type.Snow:
                            dmg_typ = Attack.Damage.Frost;
                            break;
                        case Talisman.Talisman_Type.Razors:
                            dmg_typ = Attack.Damage.Slashing;
                            break;
                        case Talisman.Talisman_Type.Heartsblood:
                            dmg_typ = Attack.Damage.Piercing;
                            break;
                        case Talisman.Talisman_Type.Toxicity:
                            dmg_typ = Attack.Damage.Acid;
                            break;
                        case Talisman.Talisman_Type.Sparks:
                            dmg_typ = Attack.Damage.Electric;
                            break;
                    }
                    temp_attacks.Add(new Attack(dmg_typ, rGen.Next(base_val, (base_val * 2) + 1)));
                }
            }
            //next, for characters. Falsael gets a bonus to all nonbow weapons
            //and melee spells. Petaer gets a bonus to all spells.
            if ((my_character == Character.Petaer && s != null) ||
               (my_character == Character.Falsael && ((w != null && w.get_my_weapon_type() != Weapon.Type.Bow) ||
                                                      (s != null && s.is_melee_range_spell()))))
                baseDamage = (int)(Math.Ceiling((double)baseDamage * 1.2));

            //Okay we're not quite done yet. First we need to check for weapon damage enhancing buffs.
            for (int i = 0; i < BuffDebuffTracker.Count; i++)
            {
                if (w != null && BuffDebuffTracker[i].my_type == Scroll.Status_Type.LynxFer)
                    initialMultiplier += .2;
                else if (w != null && BuffDebuffTracker[i].my_type == Scroll.Status_Type.PantherFer)
                    initialMultiplier += .4;
                else if (w != null && BuffDebuffTracker[i].my_type == Scroll.Status_Type.TigerFer)
                    initialMultiplier += .6;
            }
            //Then we multiply all the attacks by that.
            baseDamage = (int)((double)baseDamage * initialMultiplier);
            for (int i = 0; i < temp_attacks.Count; i++)
            {
                double attack_basedmg = (double)temp_attacks[i].get_damage_amt();
                attack_basedmg *= initialMultiplier;
                temp_attacks[i].reset_dmg((int)attack_basedmg);
            }

            if (baseDamage > 0)
            {
                Attack baseAttack = new Attack(dmgTyp, baseDamage);
                attacksOut.Add(baseAttack);
            }
            attacksOut.AddRange(temp_attacks);

            //Now we apply damage penalties where appropriate
            if (!charge_attack && w != null && w.get_my_weapon_type() == Weapon.Type.Lance)
                for (int i = 0; i < attacksOut.Count; i++)
                {
                    int atkdmg = attacksOut[i].get_damage_amt();
                    attacksOut[i].reset_dmg(atkdmg / 4);
                }
            //Horray, we're done with attacks! But we still need to do debuffs.
            if (s != null && s.get_spell_type() == Scroll.Atk_Area_Type.enemyDebuff)
                effectsOut.Add(new StatusEffect(s.get_status_effect(), s.get_duration()));

            for (int i = 0; i < equipped_talismans.Count; i++)
            {
                bool add_fx = false;
                int talisman_qualVal = 1 + (int)equipped_talismans[i].get_my_prefix();
                int fx_chance = 0;
                int fx_duration = 0;
                Scroll.Status_Type fx_type = Scroll.Status_Type.None;
                switch (equipped_talismans[i].get_my_type())
                {
                    case Talisman.Talisman_Type.Distruption:
                        add_fx = true;
                        fx_chance = talisman_qualVal * 4;
                        fx_duration = talisman_qualVal + 2;
                        fx_type = Scroll.Status_Type.Disrupt;
                        break;
                    case Talisman.Talisman_Type.Thunder:
                        add_fx = true;
                        fx_chance = talisman_qualVal * 2;
                        fx_duration = Math.Max(talisman_qualVal - 1, 1);
                        fx_type = Scroll.Status_Type.Stun;
                        break;
                    case Talisman.Talisman_Type.Grasping:
                        add_fx = true;
                        fx_chance = talisman_qualVal * 4;
                        fx_duration = talisman_qualVal;
                        fx_type = Scroll.Status_Type.Root;
                        break;
                }

                if (add_fx)
                {
                    if (rGen.Next(100) < fx_chance)
                        effectsOut.Add(new StatusEffect(fx_type, fx_duration));
                }
            }
        }
    }
}
