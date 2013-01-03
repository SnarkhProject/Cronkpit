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
    class IconBar
    {
        public enum Type_Tracker { None, Weapon, Armor, Potion };
        int number_of_icons;
        int mode;
        bool visible;

        List<Texture2D> icon_textures;
        List<Rectangle> icon_rects;
        List<int> icon_item_IDs;
        List<string> icon_shortcut_keys;
        List<int> icon_quantities;
        List<int> icon_cooldowns;
        List<Type_Tracker> icon_item_types;
        Texture2D default_texture;
        SpriteFont sFont;
        SpriteFont bFont;
        Rectangle client;
        Rectangle my_size;

        Color my_dark_color;
        Color my_red_color;
        Color my_text_color;
        int my_alpha_value;

        int my_xPosition;
        int my_yPosition;

        public IconBar(Texture2D d_texture, SpriteFont default_font, SpriteFont big_font, Rectangle cl)
        {
            number_of_icons = 8;
            visible = true;

            icon_textures = new List<Texture2D>();
            default_texture = d_texture;
            default_texture.SetData(new[] { Color.White });
            icon_rects = new List<Rectangle>();
            icon_item_IDs = new List<int>();
            icon_item_types = new List<Type_Tracker>();
            icon_quantities = new List<int>();
            icon_cooldowns = new List<int>();
            client = cl;
            icon_shortcut_keys = new List<string>();

            sFont = default_font;
            bFont = big_font;

            my_alpha_value = 255;
            my_dark_color = new Color(0, 0, 0, my_alpha_value);
            my_red_color = new Color(255, 0, 0, my_alpha_value);
            my_text_color = new Color(255, 255, 255, my_alpha_value);

            my_xPosition = (client.Width - (number_of_icons * 68)) / 2;
            my_yPosition = client.Height - 68;
            my_size = new Rectangle(my_xPosition, my_yPosition, 48, 48);

            for (int i = 0; i < number_of_icons; i++)
            {
                int next_my_X = my_xPosition + i * (20 + 48);
                icon_rects.Add(new Rectangle(next_my_X, my_yPosition, 48, 48));
            }

            for (int i = 0; i < number_of_icons; i++)
            {
                icon_textures.Add(default_texture);
                icon_item_IDs.Add(-1);
                icon_item_types.Add(Type_Tracker.None);
                icon_quantities.Add(-1);
                icon_cooldowns.Add(-1);
            }

            int functionKey = 2;
            for (int i = 0; i < number_of_icons; i++)
            {
                icon_shortcut_keys.Add("F" + functionKey.ToString());
                functionKey++;
            }
        }

        public void reset_a_texture(Texture2D new_tex, int icon_to_reset)
        {
            icon_textures.RemoveAt(icon_to_reset);
            icon_textures.Insert(icon_to_reset, new_tex);
        }

        public void switch_my_mode()
        {
            mode++;
            if (mode > 3)
                mode = 0;

            switch (mode)
            {
                case 0:
                    show();
                    reset_color_alphas(255);
                    break;
                case 1:
                    reset_color_alphas(100);
                    break;
                case 2:
                    //Make the icon textures themselves transparent
                    break;
                case 3:
                    hide();
                    break;
            }
        }

        public bool is_visible()
        {
            return visible;
        }

        public void show()
        {
            visible = true;
        }

        public void hide()
        {
            visible = false;
        }

        public int get_number_of_icons()
        {
            return number_of_icons;
        }

        public Rectangle get_ico_rects_by_slot(int slot)
        {
            return icon_rects[slot];
        }

        public int get_item_IDs_by_slot(int slot)
        {
            return icon_item_IDs[slot];
        }

        public void assign_icon_to_slot(Texture2D next_icon, int slot)
        {
            icon_textures[slot] = next_icon;
        }

        public void init_item_texture_by_id_number(int idNO, Texture2D next_icon)
        {
            for (int i = 0; i < number_of_icons; i++)
            {
                if (idNO == icon_item_IDs[i])
                    icon_textures[i] = next_icon;
            }
        }

        public void assign_id_number_to_slot(int idNO, int slot)
        {
            icon_item_IDs[slot] = idNO;
        }

        public void assign_type_to_slot(Type_Tracker tp, int slot)
        {
            icon_item_types[slot] = tp;
        }

        public void update_cooldown_and_quant(Player pl)
        {
            for (int i = 0; i < number_of_icons; i++)
            {
                if (icon_item_IDs[i] != -1)
                {
                    if (icon_item_types[i] == Type_Tracker.Weapon)
                    {
                        Weapon w = pl.get_weapon_by_ID(icon_item_IDs[i]);
                        icon_cooldowns[i] = w.get_current_cooldown();
                    }
                    else if (icon_item_types[i] == Type_Tracker.Potion)
                    {
                        icon_quantities[i] = pl.count_full_potions_by_ID(icon_item_IDs[i]);
                    }
                }
            }
        }

        public bool item_is_on_bar(int itemID)
        {
            for (int i = 0; i < number_of_icons; i++)
            {
                if (icon_item_IDs[i] == itemID)
                    return true;
            }

            return false;
        }

        public void wipe()
        {
            for (int i = 0; i < number_of_icons; i++)
            {
                icon_textures[i] = default_texture;
                icon_item_IDs[i] = -1;
                icon_item_types[i] = Type_Tracker.None;
                icon_quantities[i] = -1;
                icon_cooldowns[i] = -1;
            }
        }

        #region drawing stuff

        //Kept for posterity - not used anymore because ha ha screen coordinates.
        public void offset_drawing(Matrix offsetMatrix)
        {
            my_xPosition = 0;
            my_yPosition = 0;

            my_xPosition -= (int)offsetMatrix.M41;
            my_yPosition -= (int)offsetMatrix.M42;
            //msg_pos.X -= (int)offsetMatrix.M41;
            //msg_pos.Y -= (int)offsetMatrix.M42;
        }

        private void reset_color_alphas(float alpha_val)
        {
            my_dark_color.A = (byte)alpha_val;
            my_red_color.A = (byte)alpha_val;
            my_text_color.A = (byte)alpha_val;
        }

        public void draw_my_background(ref SpriteBatch sBatch)
        {
            for (int i = 0; i < number_of_icons; i++)
            {
                sBatch.Draw(default_texture, icon_rects[i], my_dark_color);
            }
        }

        public void draw_border_around_rectangle(ref SpriteBatch sBatch, Rectangle target_rect, int border_width, Color border_color)
        {
            sBatch.Draw(default_texture, new Rectangle(target_rect.Left, target_rect.Top, border_width, target_rect.Height), border_color);
            sBatch.Draw(default_texture, new Rectangle(target_rect.Right, target_rect.Top, border_width, target_rect.Height + border_width), border_color);
            sBatch.Draw(default_texture, new Rectangle(target_rect.Left, target_rect.Top, target_rect.Width, border_width), border_color);
            sBatch.Draw(default_texture, new Rectangle(target_rect.Left, target_rect.Bottom, target_rect.Width, border_width), border_color);
        }

        public void draw_my_borders(ref SpriteBatch sBatch)
        {
            int border_width = 4;
            for (int i = 0; i < number_of_icons; i++)
            {
                int next_my_X = my_xPosition + i * (20 + 48);
                Rectangle a_rect = new Rectangle(next_my_X, my_yPosition, 48, 48);
                sBatch.Draw(default_texture, new Rectangle(a_rect.Left-border_width, a_rect.Top-border_width, border_width, a_rect.Height+(border_width*2)), my_red_color);
                sBatch.Draw(default_texture, new Rectangle(a_rect.Right, a_rect.Top, border_width, a_rect.Height + border_width), my_red_color);
                sBatch.Draw(default_texture, new Rectangle(a_rect.Left-border_width, a_rect.Top-border_width, a_rect.Width+(border_width*2), border_width), my_red_color);
                sBatch.Draw(default_texture, new Rectangle(a_rect.Left, a_rect.Bottom, a_rect.Width, border_width), my_red_color);
            }
        }

        public void draw_my_icons(ref SpriteBatch sBatch)
        {
            for (int i = 0; i < number_of_icons; i++)
            {
                if (icon_item_IDs[i] != -1 && icon_textures[i] != null)
                    sBatch.Draw(icon_textures[i], icon_rects[i], Color.White);
            }
        }

        public void draw_my_cooldown_overlay(ref SpriteBatch sBatch)
        {
            for (int i = 0; i < number_of_icons; i++)
            {
                if(icon_cooldowns[i] > 0)
                    sBatch.Draw(default_texture, icon_rects[i], new Color(0, 0,0, 150));
            }
        }

        public void draw_my_shortcuts(ref SpriteBatch sBatch)
        {
            //Draws the shortcut text. Need to figure out the pattern for this in a bit.
            for (int i = 0; i < number_of_icons; i++)
            {
                float xPosition = icon_rects[i].X + icon_rects[i].Width - sFont.MeasureString(icon_shortcut_keys[i]).X - 5;
                float yPosition = icon_rects[i].Y + icon_rects[i].Height - sFont.LineSpacing - 5;
                Vector2 position = new Vector2(xPosition, yPosition);
                sBatch.DrawString(sFont, icon_shortcut_keys[i], position, Color.White);

                if (icon_quantities[i] > 0)
                {
                    float q_xPosition = icon_rects[i].X + 5;
                    float q_yPosition = icon_rects[i].Y + icon_rects[i].Height - sFont.LineSpacing - 5;
                    Vector2 q_position = new Vector2(q_xPosition, q_yPosition);
                    sBatch.DrawString(bFont, icon_quantities[i].ToString(), q_position, Color.Orange);
                }

                if (icon_cooldowns[i] > 0)
                {
                    string cd_number = icon_cooldowns[i].ToString();
                    float cd_xPosition = icon_rects[i].X + ((icon_rects[i].Width - bFont.MeasureString(cd_number).X) / 2);
                    float cd_yPosition = icon_rects[i].Y + ((icon_rects[i].Height - bFont.LineSpacing) / 2);
                    Vector2 cd_position = new Vector2(cd_xPosition, cd_yPosition);
                    sBatch.DrawString(bFont, cd_number, cd_position, Color.Red);
                }
            }
        }

        public void draw_me(ref SpriteBatch sBatch)
        {
            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_my_background(ref sBatch);
            sBatch.End();

            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_my_borders(ref sBatch);
            sBatch.End();

            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_my_icons(ref sBatch);
            sBatch.End();

            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_my_cooldown_overlay(ref sBatch);
            sBatch.End();

            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_my_shortcuts(ref sBatch);
            sBatch.End();
        }
        
        #endregion
    }
}
