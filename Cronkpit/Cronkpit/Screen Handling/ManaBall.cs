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
    class ManaBall
    {
        int rect_side_size = 75;
        int spacing = 20;

        Vector2 screen_position;
        Texture2D background;
        Texture2D manaball_mask;
        Color manaball_mask_color;

        bool red_rising;
        float c_opacity;
        float max_opacity;
        float red_value;
        float red_value_per_update;

        public ManaBall(Texture2D background_texture, Texture2D cball_mask, Rectangle client)
        {
            int screen_posx = client.Right - spacing - rect_side_size;
            int screen_posy = client.Bottom - spacing - rect_side_size;
            screen_position = new Vector2(screen_posx, screen_posy);

            background = background_texture;
            manaball_mask = cball_mask;
            red_value = 0f;
            red_value_per_update = 1f;
            manaball_mask_color = new Color(red_value, 0, 255);
            red_rising = true;
            max_opacity = 255f;
        }

        public void calculate_opacity(double mana)
        {
            double opacity_percentage = mana / 1000;
            c_opacity = max_opacity * (float)opacity_percentage;

            manaball_mask_color.A = (byte)c_opacity;
        }

        public void update(float delta_time)
        {
            float local_red_value = red_value_per_update * delta_time;

            if (red_rising)
                red_value += local_red_value;
            else
                red_value -= local_red_value;

            if (Math.Round(red_value) == 255)
                red_rising = false;
            if (Math.Round(red_value) == 0)
                red_rising = true;

            manaball_mask_color.R = (byte)red_value;
        }

        public void draw_me(ref SpriteBatch sBatch)
        {   
            sBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied);
            sBatch.Draw(manaball_mask, screen_position, manaball_mask_color);
            sBatch.End();

            sBatch.Begin(SpriteSortMode.BackToFront, null);
            sBatch.Draw(background, screen_position, Color.White);
            sBatch.End();
        }
    }
}
