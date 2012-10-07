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
    class MessageBufferBox
    {
        bool isVisible;

        Rectangle client;
        Rectangle my_size;

        SpriteFont sFont;
        Texture2D myTex;
        Vector2 msg_pos;

        Texture2D scroll_up_max;
        Texture2D scroll_up_one;
        Texture2D scroll_down_max;
        Texture2D scroll_down_one;

        Rectangle scroll_up_max_rect;
        Rectangle scroll_up_one_rect;
        Rectangle scroll_down_max_rect;
        Rectangle scroll_down_one_rect;

        Color my_dark_color;
        Color my_red_color;
        Color my_text_color;

        List<string> my_messages;

        int mode;

        int width;
        int height;

        int c_start_index;
        int messages_shown;

        bool autoscroll;

        public MessageBufferBox(Rectangle cl, SpriteFont sf, Texture2D tx, ref List<string> myMsgs)
        {
            isVisible = true;

            sFont = sf;
            myTex = tx;
            myTex.SetData(new[] {Color.White});
            my_messages = myMsgs;

            width = 0;
            height = 0;

            mode = 0;

            c_start_index = 0;
            messages_shown = 4;

            autoscroll = true;

            client = cl;
            my_dark_color = new Color(0, 0, 0);
            my_red_color = new Color(255, 0, 0);
            my_text_color = new Color(255, 255, 255);

            my_messages.Add("You start off your adventure in high spirits! Or something. YOU ALSO DIE AHAHAHAH");
            decide_my_size();
            my_messages.Clear();
        }

        private void decide_my_size()
        {
            height = messages_shown * sFont.LineSpacing;
            for (int i = c_start_index; i < Math.Min(c_start_index + messages_shown, my_messages.Count); i++)
            {
                Vector2 size = sFont.MeasureString(my_messages[i]);
                if (size.X > width)
                    width = (int)size.X;
            }

            int initialX = (client.Width - (width + 40)) / 2;
            int initialY = (client.Height - (height + 20)) - 10;
            msg_pos = new Vector2(((float)initialX + 10), ((float)initialY + 10));
            my_size = new Rectangle(initialX, initialY, width + 40, height + 20);

            //Calculate X value. Easy.
            int scrollElements_x = initialX + width + 19;
            //Calculate Y values. A little more complicated.
            int spacing = 4;
            int y_factor = height / 4;
            //Done from the top down
            int first_y = initialY + spacing;
            int second_y = initialY + (spacing * 2) + y_factor;
            int third_y = initialY + (spacing * 3) + (y_factor * 2);
            int fourth_y = initialY + (spacing * 4) + (y_factor * 3);
            scroll_up_max_rect = new Rectangle(scrollElements_x, first_y, 18, 18);
            scroll_up_one_rect = new Rectangle(scrollElements_x, second_y, 18, 18);
            scroll_down_one_rect = new Rectangle(scrollElements_x, third_y, 18, 18);
            scroll_down_max_rect = new Rectangle(scrollElements_x, fourth_y, 18, 18);
        }

        public void init_textures(Texture2D upOne, Texture2D upMax, Texture2D downOne, Texture2D downMax)
        {
            scroll_up_max = upMax;
            scroll_up_one = upOne;
            scroll_down_max = downMax;
            scroll_down_one = downOne;
        }

        public void add_a_msg(string msg)
        {
            my_messages.Add(msg);
        }

        public bool is_visible()
        {
            return isVisible;
        }

        public bool is_scrolling()
        {
            return autoscroll;
        }

        public void scrollMSG(int scrollvalue)
        {
            c_start_index += scrollvalue;
            if (c_start_index > my_messages.Count)
            {
                c_start_index = my_messages.Count;
            }
            else if (c_start_index < 0)
                c_start_index = 0;
        }

        public void show()
        {
            isVisible = true;
        }

        public void hide()
        {
            isVisible = false;
        }

        public void switch_my_mode()
        {
            mode++;
            if (mode > 3)
                mode = 0;
            switch (mode)
            {
                case 0:
                    reset_color_alphas(255f);
                    show();
                    break;
                case 1:
                    //Do nothing since this manages its own alpha values.
                    break;
                case 2:
                    reset_color_alphas(100f);
                    break;
                case 3:
                    hide();
                    break;
            }
        }

        public void mouseClick(Vector2 clickLoc)
        {
            if (scroll_up_max_rect.Contains((int)clickLoc.X, (int)clickLoc.Y))
            {
                autoscroll = false;
                scrollMSG(-1000);
            }

            if (scroll_up_one_rect.Contains((int)clickLoc.X, (int)clickLoc.Y))
            {
                autoscroll = false;
                scrollMSG(-1);
            }

            if (scroll_down_one_rect.Contains((int)clickLoc.X, (int)clickLoc.Y))
            {
                scrollMSG(1);
                if (c_start_index >= my_messages.Count)
                    autoscroll = true;
            }

            if (scroll_down_max_rect.Contains((int)clickLoc.X, (int)clickLoc.Y))
            {
                autoscroll = true;
                scrollMSG(1000);
            }
        }

        #region drawing stuff
        //Don't call before you call a sb.begin() duh.
        public void offset_drawing(Matrix offsetMatrix)
        {
            decide_my_size();
            my_size.X -= (int)offsetMatrix.M41;
            my_size.Y -= (int)offsetMatrix.M42;
            msg_pos.X -= (int)offsetMatrix.M41;
            msg_pos.Y -= (int)offsetMatrix.M42;
        }

        private void reset_color_alphas(float alpha_val)
        {
            my_dark_color.A = (byte)alpha_val;
            my_red_color.A = (byte)alpha_val;
            my_text_color.A = (byte)alpha_val;
        }

        public void draw_my_rect(ref SpriteBatch sBatch)
        {
            //Set alphas
            if (mode == 1)
            {
                if (c_start_index >= my_messages.Count)
                    reset_color_alphas(100f);
                //Otherwise, set to opaque
                else
                    reset_color_alphas(255f);
            }
            //Draw rectangle first
            sBatch.Draw(myTex, my_size, my_dark_color);
        }

        public void draw_my_elements(ref SpriteBatch sBatch)
        {
            sBatch.Draw(scroll_down_max, scroll_down_max_rect, Color.White);
            sBatch.Draw(scroll_down_one, scroll_down_one_rect, Color.White);
            sBatch.Draw(scroll_up_max, scroll_up_max_rect, Color.White);
            sBatch.Draw(scroll_up_one, scroll_up_one_rect, Color.White);
        }

        public void draw_my_borders(ref SpriteBatch sBatch)
        {
            //Then borders
            int border_width = 4;
            sBatch.Draw(myTex, new Rectangle(my_size.Left, my_size.Top, border_width, my_size.Height), my_red_color);
            sBatch.Draw(myTex, new Rectangle(my_size.Right, my_size.Top, border_width, my_size.Height + border_width), my_red_color);
            sBatch.Draw(myTex, new Rectangle(my_size.Left, my_size.Top, my_size.Width, border_width), my_red_color);
            sBatch.Draw(myTex, new Rectangle(my_size.Left, my_size.Bottom, my_size.Width, border_width), my_red_color);
        }

        public void draw_my_text(ref SpriteBatch sBatch)
        {
            Vector2 msg_pos2 = msg_pos;
            //Then text
            for (int i = c_start_index; i < Math.Min(c_start_index + messages_shown, my_messages.Count); i++)
            {
                sBatch.DrawString(sFont, my_messages[i], msg_pos2, my_text_color);
                msg_pos2.Y += sFont.LineSpacing;
            }
        }

        public void draw_me(ref SpriteBatch sBatch)
        {
            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_my_rect(ref sBatch);
            sBatch.End();

            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_my_elements(ref sBatch);
            sBatch.End();

            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_my_borders(ref sBatch);
            sBatch.End();

            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_my_text(ref sBatch);
            sBatch.End();
        }

        #endregion
    }
}
