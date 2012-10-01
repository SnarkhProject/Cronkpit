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
        bool additionalMessages;

        Rectangle client;
        Rectangle my_size;

        SpriteFont sFont;
        Texture2D myTex;
        Vector2 msg_pos;

        string my_msg;

        int width = 0;
        int height = 0;

        public MessageBufferBox(Rectangle cl, SpriteFont sf, Texture2D tx)
        {
            isVisible = true;
            additionalMessages = false;

            sFont = sf;
            myTex = tx;
            myTex.SetData(new[] {Color.White});

            client = cl;
        }

        private void decide_my_size()
        {
            Vector2 size = sFont.MeasureString(my_msg);
            if (size.X > width)
                width = (int)size.X;
            height = sFont.LineSpacing;

            int initialX = (client.Width - (width + 20)) / 2;
            int initialY = (client.Height - (height + 20)) - 10;
            msg_pos = new Vector2(((float)initialX + 10), ((float)initialY + 10));
            my_size = new Rectangle(initialX, initialY, width + 20, height + 20);
        }

        public void set_my_msg(string msg)
        {
            my_msg = msg;
            decide_my_size();
        }

        public bool is_visible()
        {
            return isVisible;
        }

        public void show()
        {
            isVisible = true;
        }

        public void hide()
        {
            isVisible = false;
        }

        //Don't call before you call a sb.begin() duh.
        public void offset_drawing(Matrix offsetMatrix)
        {
            my_size.X -= (int)offsetMatrix.M41;
            my_size.Y -= (int)offsetMatrix.M42;
            msg_pos.X -= (int)offsetMatrix.M41;
            msg_pos.Y -= (int)offsetMatrix.M42;
        }

        public void draw_my_rect(ref SpriteBatch sBatch)
        {
            
            //Draw rectangle first
            sBatch.Draw(myTex, my_size, Color.Black);
        }

        public void are_there_multiple_messages(bool yesno)
        {
            additionalMessages = yesno;
        }

        public void draw_my_borders(ref SpriteBatch sBatch)
        {
            //Then borders
            int border_width = 4;
            sBatch.Draw(myTex, new Rectangle(my_size.Left, my_size.Top, border_width, my_size.Height), Color.Red);
            sBatch.Draw(myTex, new Rectangle(my_size.Right, my_size.Top, border_width, my_size.Height+border_width), Color.Red);
            sBatch.Draw(myTex, new Rectangle(my_size.Left, my_size.Top, my_size.Width, border_width), Color.Red);
            sBatch.Draw(myTex, new Rectangle(my_size.Left, my_size.Bottom, my_size.Width, border_width), Color.Red);
        }

        public void draw_my_text(ref SpriteBatch sBatch)
        {
            //Then text
            sBatch.DrawString(sFont, my_msg, msg_pos, Color.White);
            if (additionalMessages)
            {
                //do nothing so far.
            }
        }
    }
}
