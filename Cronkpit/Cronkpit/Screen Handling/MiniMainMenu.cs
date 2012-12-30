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
    class MiniMainMenu
    {
        Rectangle my_size;
        int my_xsize = 300;
        int my_ysize = 550;

        Rectangle client_rect;

        bool visible;

        List<string> my_menu_items;
        int c_selected_index;

        Color my_red_color;
        Color my_dark_color;
        Color my_text_color;

        Texture2D my_blank_texture;

        SpriteFont menuFont;

        public MiniMainMenu(List<string> menu_items, Rectangle cli_rect, Texture2D blank_texture, SpriteFont mFont)
        {
            my_menu_items = menu_items;
            c_selected_index = 0;

            client_rect = cli_rect;

            int my_x_position = (client_rect.Width - my_xsize) / 2;
            int my_y_position = (client_rect.Height - my_ysize) / 2;
            my_size = new Rectangle(my_x_position, my_y_position, my_xsize, my_ysize);

            my_blank_texture = blank_texture;
            menuFont = mFont;

            my_red_color = new Color(255, 0, 0);
            my_dark_color = new Color(0,0,0);
            my_text_color = new Color(255, 255, 255);
        }

        public void scroll_menu(int scroll)
        {
            c_selected_index += scroll;

            if (c_selected_index < 0)
                c_selected_index = my_menu_items.Count - 1;
            else if (c_selected_index == my_menu_items.Count)
                c_selected_index = 0;
        }

        public int get_index()
        {
            return c_selected_index;
        }

        public void set_index(int next_index)
        {
            c_selected_index = next_index;
        }

        #region showing and hiding options

        public void show()
        {
            visible = true;
        }

        public void hide()
        {
            visible = false;
        }

        public bool is_visible()
        {
            return visible;
        }

        #endregion

        #region drawing stuff

        public void draw_my_background(ref SpriteBatch sBatch)
        {
            sBatch.Draw(my_blank_texture, my_size, my_dark_color);
        }

        public void draw_border_around_rectangle(ref SpriteBatch sBatch, Rectangle target_rect, int border_width, Color border_color)
        {
            sBatch.Draw(my_blank_texture, new Rectangle(target_rect.Left, target_rect.Top, border_width, target_rect.Height), border_color);
            sBatch.Draw(my_blank_texture, new Rectangle(target_rect.Right, target_rect.Top, border_width, target_rect.Height + border_width), border_color);
            sBatch.Draw(my_blank_texture, new Rectangle(target_rect.Left, target_rect.Top, target_rect.Width, border_width), border_color);
            sBatch.Draw(my_blank_texture, new Rectangle(target_rect.Left, target_rect.Bottom, target_rect.Width, border_width), border_color);
        }

        public void draw_my_text(ref SpriteBatch sBatch)
        {
            Vector2 item_position = new Vector2(my_size.X + 10, my_size.Y + 10);

            for (int i = 0; i < my_menu_items.Count; i++)
            {
                item_position.X = my_size.X + (my_size.Width - menuFont.MeasureString(my_menu_items[i]).X) / 2;
                Color tint = my_text_color;
                if (i == c_selected_index)
                    tint = my_red_color;
                sBatch.DrawString(menuFont, my_menu_items[i], item_position, tint);
                item_position.Y += (menuFont.LineSpacing * 11) / 6;
            }
        }

        public void draw_me(ref SpriteBatch sBatch)
        {
            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_my_background(ref sBatch);
            sBatch.End();

            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_border_around_rectangle(ref sBatch, my_size, 4, my_red_color);
            sBatch.End();

            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_my_text(ref sBatch);
            sBatch.End();
        }

        #endregion
    }
}
