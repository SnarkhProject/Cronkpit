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
    class IconBar
    {
        int number_of_icons;
        int mode;
        bool visible;

        List<Texture2D> icon_textures;
        Texture2D default_texture;
        SpriteFont sFont;
        Rectangle client;
        Rectangle my_size;

        Color my_dark_color;
        Color my_red_color;
        Color my_text_color;
        int my_alpha_value;

        int my_xPosition;
        int my_yPosition;

        public IconBar(Texture2D d_texture, SpriteFont default_font, Rectangle cl)
        {
            number_of_icons = 8;
            visible = true;

            icon_textures = new List<Texture2D>();
            default_texture = d_texture;
            default_texture.SetData(new[] { Color.White });
            for (int i = 0; i < number_of_icons; i++)
            {
                icon_textures.Add(default_texture);
            }
            client = cl;

            sFont = default_font;

            my_alpha_value = 255;
            my_dark_color = new Color(0, 0, 0, my_alpha_value);
            my_red_color = new Color(255, 0, 0, my_alpha_value);
            my_text_color = new Color(255, 255, 255, my_alpha_value);

            my_xPosition = client.Width - 68;
            my_yPosition = 20;
            my_size = new Rectangle(my_xPosition, my_yPosition, 48, 48);
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

        #region drawing stuff

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

        public void draw_my_icons(ref SpriteBatch sBatch)
        {
            for (int i = 0; i < number_of_icons; i++)
            {
                int next_my_Y = my_yPosition + i * (20 + 48);
                Rectangle a_rect = new Rectangle(my_xPosition, next_my_Y, 48, 48);
                sBatch.Draw(icon_textures[i], a_rect, my_dark_color);
            }
        }

        public void draw_my_borders(ref SpriteBatch sBatch)
        {
            int border_width = 4;
            for (int i = 0; i < number_of_icons; i++)
            {
                int next_my_Y = my_yPosition + i * (20 + 48);
                Rectangle a_rect = new Rectangle(my_xPosition, next_my_Y, 48, 48);
                sBatch.Draw(default_texture, new Rectangle(a_rect.Left, a_rect.Top, border_width, a_rect.Height), my_red_color);
                sBatch.Draw(default_texture, new Rectangle(a_rect.Right, a_rect.Top, border_width, a_rect.Height + border_width), my_red_color);
                sBatch.Draw(default_texture, new Rectangle(a_rect.Left, a_rect.Top, a_rect.Width, border_width), my_red_color);
                sBatch.Draw(default_texture, new Rectangle(a_rect.Left, a_rect.Bottom, a_rect.Width, border_width), my_red_color);
            }
        }

        public void draw_my_shortcuts(ref SpriteBatch sBatch)
        {
            //Draws the shortcut text. Need to figure out the pattern for this in a bit.
        }

        public void draw_me(ref SpriteBatch sBatch)
        {
            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_my_icons(ref sBatch);
            sBatch.End();

            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_my_borders(ref sBatch);
            sBatch.End();

            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_my_shortcuts(ref sBatch);
            sBatch.End();
        }

        #endregion
    }
}
