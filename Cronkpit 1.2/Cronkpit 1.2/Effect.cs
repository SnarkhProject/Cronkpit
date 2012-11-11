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
    class Effect
    {
        Texture2D my_texture;
        Vector2 my_position = Vector2.Zero;
        Color my_color = Color.White;
        float my_rotation = 0f;
        float my_scale = 1f;
        SpriteEffects my_spriteEffects;
        List<Rectangle> frame_list;
        int my_frame_index = 0;

        private float time_elapsed;
        private bool is_looping = false;
        private float time_to_update = 0.05f;
        public int frames_per_second = 20;

        public Effect(Texture2D my_tex, int frames, gridCoordinate g_c)
        {
            my_texture = my_tex;
            int rect_width = my_tex.Width / frames;
            frame_list = new List<Rectangle>();
            for (int i = 0; i < frames; i++)
            {
                frame_list.Add(new Rectangle(i*rect_width, 0, rect_width, my_tex.Height));
            }

            my_spriteEffects = SpriteEffects.None;
            my_position.X = g_c.x * 32;
            my_position.Y = g_c.y * 32;
            set_FPS(frames_per_second + frames);
        }

        public void set_FPS(int new_fps)
        {
            time_to_update = 1f / new_fps;
        }

        public void update(float delta_time)
        {
            time_elapsed += delta_time;

            if (time_elapsed > time_to_update)
            {
                time_elapsed -= time_to_update;
                if (my_frame_index < frame_list.Count)
                    my_frame_index++;
                else if (is_looping)
                    my_frame_index = 0;
            }
        }

        public Vector2 get_my_position()
        {
            return my_position;
        }

        public bool slated_for_removal()
        {
            return my_frame_index == frame_list.Count;
        }

        public void draw_me(ref SpriteBatch sBatch)
        {
            if(my_frame_index < frame_list.Count)
                sBatch.Draw(my_texture, my_position, frame_list[my_frame_index], my_color, my_rotation, Vector2.Zero, my_scale, my_spriteEffects, 0f);
        }
    }
}
