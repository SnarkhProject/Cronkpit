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
        public enum Doodad_Type { ArmorSuit, Destroyed_ArmorSuit, Door, CorpsePile };
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

        //General functions used for both armor suits and doors.
        public bool passable;
        public bool destroyable;
        public bool blocks_los;
        int HP;
        int index;
        string name;

        public Doodad(Doodad_Type dType, ContentManager cManage, gridCoordinate s_coord, int s_ind)
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
                    my_impassable_texture = cManage.Load<Texture2D>("Background/Doodads/stonewall_woodendoor");
                    my_passable_texture = cManage.Load<Texture2D>("Background/Doodads/stonewall_woodendoor_open");
                    my_destroyed_texture = cManage.Load<Texture2D>("Background/Doodads/stonewall_woodendoor_destroyed");
                    name = "door";
                    my_door_state = Door_State.Closed;
                    passable = false;
                    destroyable = true;
                    blocks_los = true;
                    HP = 17;
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
    }
}
