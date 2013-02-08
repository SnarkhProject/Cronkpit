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
    class Popup
    {
        public enum popup_msg_color { Red, Blue, Yellow, Purple, LimeGreen, VividGreen };
        Color my_color;
        string my_text;
        public float time_until_vanish = 150f;

        public gridCoordinate gc_origin;
        public Vector2 my_position;
        float lost_time_per_second = 250f;

        SpriteFont my_font;

        public Popup(string txt, popup_msg_color clr, SpriteFont fnt, gridCoordinate g_c)
        {
            my_text = txt;
            my_font = fnt;

            float x_position = ((g_c.x * 32) + 16) - (fnt.MeasureString(txt).X / 2);
            float y_position = g_c.y * 32 + 16 - (fnt.LineSpacing);
            my_position = new Vector2(x_position, y_position);
            gc_origin = new gridCoordinate(g_c);

            switch (clr)
            {
                case popup_msg_color.Red:
                    my_color = new Color(255, 0, 0);
                    break;
                case popup_msg_color.Blue:
                    my_color = new Color(0, 0, 255);
                    break;
                case popup_msg_color.Yellow:
                    my_color = new Color(255, 255, 0);
                    break;
                case popup_msg_color.Purple:
                    my_color = new Color(255, 0, 255);
                    break;
                case popup_msg_color.LimeGreen:
                    my_color = new Color(80, 235, 50);
                    break;
                case popup_msg_color.VividGreen:
                    my_color = new Color(50, 255, 30);
                    break;
            }
        }

        public Popup(string txt, popup_msg_color clr, SpriteFont fnt, Vector2 s_position)
        {
            my_text = txt;
            my_font = fnt;

            float x_position = s_position.X - (fnt.MeasureString(txt).X / 2);
            float y_position = s_position.Y = s_position.Y - (fnt.LineSpacing);
            my_position = new Vector2(x_position, y_position);
            gc_origin = new gridCoordinate((int)Math.Round(s_position.X), (int)Math.Round(s_position.Y));

            switch (clr)
            {
                case popup_msg_color.Red:
                    my_color = new Color(255, 0, 0);
                    break;
                case popup_msg_color.Blue:
                    my_color = new Color(0, 0, 255);
                    break;
                case popup_msg_color.Yellow:
                    my_color = new Color(255, 255, 0);
                    break;
                case popup_msg_color.Purple:
                    my_color = new Color(255, 0, 255);
                    break;
                case popup_msg_color.LimeGreen:
                    my_color = new Color(80, 235, 50);
                    break;
                case popup_msg_color.VividGreen:
                    my_color = new Color(50, 255, 30);
                    break;
            }
        }

        public void update(float delta_time)
        {
            my_position.Y -= 50 * delta_time;
            if (time_until_vanish > 0)
                time_until_vanish -= lost_time_per_second * delta_time;
        }

        public void draw_me(ref SpriteBatch sBatch)
        {
            sBatch.DrawString(my_font, my_text, my_position, my_color);
        }
    }
}
