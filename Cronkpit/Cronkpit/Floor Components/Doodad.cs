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
    class Doodad
    {
        public enum Doodad_Type { ArmorSuit, Destroyed_ArmorSuit, Door, CorpsePile, Altar,
                                  Destroyed_Altar };
        Doodad_Type my_doodad_type;
        Texture2D my_impassable_texture;
        Texture2D my_passable_texture;
        gridCoordinate my_grid_coord;
        Vector2 drawing_position;
        Random rGen;

        //Door only stuff
        public enum Door_State { Open, Locked, Stuck, Destroyed, Closed };
        Door_State my_door_state;
        Texture2D my_destroyed_texture;

        //Altar only stuff
        ContentManager cmgr;

        //General functions used for both armor suits and doors.
        public bool passable;
        public bool destroyable;
        public bool blocks_los;
        int HP;
        int index;
        string name;

        public Doodad(Doodad_Type dType, ContentManager cManage, gridCoordinate s_coord, int s_ind,
                      bool stone_doorframe = false)
        {
            my_doodad_type = dType;
            switch (my_doodad_type)
            {
                case Doodad_Type.ArmorSuit:
                    my_impassable_texture = cManage.Load<Texture2D>("Enemies/hollowKnight_idle");
                    my_passable_texture = cManage.Load<Texture2D>("Entities/broken_armor");
                    name = "suit of armor";
                    passable = false;
                    destroyable = true;
                    blocks_los = false;
                    HP = 22;
                    break;
                case Doodad_Type.Destroyed_ArmorSuit:
                    my_impassable_texture = cManage.Load<Texture2D>("Entities/broken_armor");
                    my_passable_texture = cManage.Load<Texture2D>("Entities/broken_armor");
                    name = "suit of armor";
                    passable = true;
                    destroyable = false;
                    blocks_los = false;
                    HP = 0;
                    break;
                case Doodad_Type.CorpsePile:
                    my_impassable_texture = cManage.Load<Texture2D>("Background/Doodads/corpsepile");
                    my_passable_texture = my_impassable_texture;
                    name = "corpse pile";
                    passable = true;
                    destroyable = false;
                    blocks_los = false;
                    HP = 0;
                    break;
                case Doodad_Type.Door:
                    if (stone_doorframe)
                    {
                        my_impassable_texture = cManage.Load<Texture2D>("Background/Doodads/stonewall_woodendoor");
                        my_passable_texture = cManage.Load<Texture2D>("Background/Doodads/stonewall_woodendoor_open");
                        my_destroyed_texture = cManage.Load<Texture2D>("Background/Doodads/stonewall_woodendoor_destroyed");
                    }
                    else
                    {
                        my_impassable_texture = cManage.Load<Texture2D>("Background/Doodads/woodwall_woodendoor");
                        my_passable_texture = cManage.Load<Texture2D>("Background/Doodads/woodwall_woodendoor_open");
                        my_destroyed_texture = cManage.Load<Texture2D>("Background/Doodads/woodwall_woodendoor_destroyed");
                    }
                    name = "door";
                    my_door_state = Door_State.Closed;
                    passable = false;
                    destroyable = true;
                    blocks_los = true;
                    HP = 17;
                    break;
                case Doodad_Type.Altar:
                    cmgr = cManage;
                    my_impassable_texture = cManage.Load<Texture2D>("Background/Doodads/altar");
                    my_passable_texture = cManage.Load<Texture2D>("Background/Doodads/altar_destroyed");
                    name = "altar";
                    passable = false;
                    destroyable = false;
                    blocks_los = false;
                    break;
            }
            rGen = new Random();
            index = s_ind;
            my_grid_coord = s_coord;
            drawing_position = new Vector2(my_grid_coord.x * 32, my_grid_coord.y * 32);
        }

        public string my_name()
        {
            return name;
        }

        public void take_damage(int dmg, List<string> msgBuf, Floor fl)
        {
            msgBuf.Add("The " + name + " takes " + dmg + " damage!");
            fl.add_new_popup("-" + dmg, Popup.popup_msg_color.Red, my_grid_coord);
            HP -= dmg;
            if (my_doodad_type == Doodad_Type.Door)
            {
                fl.sound_pulse(my_grid_coord, dmg * 3, SoundPulse.Sound_Types.Player);
                int door_unstick_chance = 25 + dmg * 2;
                if (my_door_state == Door_State.Stuck && rGen.Next(100) < door_unstick_chance)
                {
                    my_door_state = Door_State.Closed;
                    fl.add_new_popup("Unstuck!", Popup.popup_msg_color.Orange, my_grid_coord);
                }
            }

            if (HP <= 0)
            {
                passable = true;
                destroyable = false;
                blocks_los = false;
                if (my_doodad_type == Doodad_Type.Door)
                {
                    my_door_state = Door_State.Destroyed;
                    my_passable_texture = my_destroyed_texture;
                }
            }
        }

        public void draw_me(ref SpriteBatch sBatch)
        {
            if (!passable)
                sBatch.Draw(my_impassable_texture, drawing_position, Color.White);
            else
                sBatch.Draw(my_passable_texture, drawing_position, Color.White);

        }

        public gridCoordinate get_g_coord()
        {
            return my_grid_coord;
        }

        public bool is_passable()
        {
            return passable;
        }

        public Doodad_Type get_my_doodad_type()
        {
            return my_doodad_type;
        }

        public int get_my_index()
        {
            return index;
        }

        public bool is_destructible()
        {
            return destroyable;
        }

        #region door only stuff

        public void set_door_state(Door_State next_state)
        {
            my_door_state = next_state;
        }

        public void open_door(Floor fl)
        {
            switch (my_door_state)
            {
                case Door_State.Stuck:
                    fl.add_new_popup("Stuck!", Popup.popup_msg_color.Orange, my_grid_coord);
                    break;
                case Door_State.Locked:
                    fl.add_new_popup("Locked!", Popup.popup_msg_color.Orange, my_grid_coord);
                    break;
                default:
                    my_door_state = Door_State.Open;
                    destroyable = false;
                    passable = true;
                    blocks_los = false;
                    break;
            }
        }

        public void close_door(Floor fl)
        {
            fl.add_new_popup("Closed!", Popup.popup_msg_color.Orange, my_grid_coord);
            my_door_state = Door_State.Closed;
            destroyable = true;
            passable = false;
            blocks_los = true;
        }

        public bool is_door_closed()
        {
            return my_door_state == Door_State.Stuck ||
                   my_door_state == Door_State.Locked ||
                   my_door_state == Door_State.Closed;
        }

        public bool is_door_destroyed()
        {
            return HP <= 0;
        }

        #endregion

        #region altar only stuff

        public void destroy_altar(Player pl, Floor fl)
        {
            my_doodad_type = Doodad_Type.Destroyed_Altar;
            passable = true;
            destroyable = false;
            name = "destroyed altar";
            create_minor_undead(pl, fl);
        }

        public void create_minor_undead(Player pl, Floor fl)
        {
            gridCoordinate pl_gc = pl.get_my_grid_C();
            List<Monster> fl_monsters = fl.see_badGuys();
            int monsterType = rGen.Next(3);
            int skeletonType = rGen.Next(6);
            gridCoordinate monster_position = new gridCoordinate();
            bool valid_position_found = false;

            for (int x = -1; x <= 1; x++)
                for (int y = -1; y <= 1; y++)
                {
                    int whocares = -1;
                    gridCoordinate next_position = new gridCoordinate(my_grid_coord.x + x, my_grid_coord.y + y);
                    if (!valid_position_found && !fl.is_tile_opaque(next_position) &&
                        !fl.is_entity_here(next_position) && !fl.is_monster_here(next_position, out whocares) &&
                        pl_gc.x != next_position.x && pl_gc.y != next_position.y)
                    {
                        monster_position = next_position;
                        valid_position_found = true;
                    }
                }

            if (valid_position_found)
            {
                int next_index = -1;
                bool found_valid_number = false;
                while (!found_valid_number)
                {
                    next_index++;
                    bool valid_number = true;
                    for (int i = 0; i < fl_monsters.Count; i++)
                    {
                        if (next_index == fl_monsters[i].my_Index)
                            valid_number = false;
                    }

                    if (valid_number)
                        found_valid_number = true;
                }
                fl.add_new_popup("Summoned!", Popup.popup_msg_color.Purple, monster_position);
                add_minor_undead(fl, monsterType, skeletonType,
                                 next_index, monster_position, false, pl);
            }
        }

        public void add_minor_undead(Floor fl, int minor_monster_type,
                                     int minor_skeleton_type, int next_monster_index,
                                     gridCoordinate monster_position, bool force_wander, Player pl)
        {
            switch (minor_monster_type)
            {
                case 0:
                    fl.see_badGuys().Add(new Zombie(monster_position, cmgr, next_monster_index));
                    break;
                case 1:
                    fl.see_badGuys().Add(new GoreHound(monster_position, cmgr, next_monster_index));
                    break;
                case 2:
                    Skeleton.Skeleton_Weapon_Type skelweapon = 0;
                    switch (minor_skeleton_type)
                    {
                        case 0:
                            skelweapon = Skeleton.Skeleton_Weapon_Type.Fist;
                            break;
                        case 1:
                            skelweapon = Skeleton.Skeleton_Weapon_Type.Axe;
                            break;
                        case 2:
                            skelweapon = Skeleton.Skeleton_Weapon_Type.Bow;
                            break;
                        case 3:
                            skelweapon = Skeleton.Skeleton_Weapon_Type.Flamebolt;
                            break;
                        case 4:
                            skelweapon = Skeleton.Skeleton_Weapon_Type.Spear;
                            break;
                        case 5:
                            skelweapon = Skeleton.Skeleton_Weapon_Type.Sword;
                            break;
                    }
                    fl.see_badGuys().Add(new Skeleton(monster_position, cmgr, next_monster_index, skelweapon));
                    break;
            }
            if (force_wander)
                fl.force_monster_wander(next_monster_index, pl);
        }


        #endregion
    }
}
