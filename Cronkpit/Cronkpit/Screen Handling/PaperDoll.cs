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
    class PaperDoll
    {
        Rectangle my_size;
        Rectangle client_rect;

        Texture2D wireFrame;
        Texture2D[] head_textures;
        Texture2D[] chest_textures;
        Texture2D[] larm_textures;
        Texture2D[] rarm_textures;
        Texture2D[] lleg_textures;
        Texture2D[] rleg_textures;

        int head_wounds;
        int chest_wounds;
        int rarm_wounds;
        int larm_wounds;
        int lleg_wounds;
        int rleg_wounds;

        int xsize = 80;
        int ysize = 125;

        public PaperDoll(Rectangle cli_rect)
        {
            head_wounds = 0;
            chest_wounds = 0;
            rarm_wounds = 0;
            larm_wounds = 0;
            lleg_wounds = 0;
            rleg_wounds = 0;

            client_rect = cli_rect;

            my_size = new Rectangle(30, client_rect.Height - (ysize+20), xsize, ysize);
        }

        public void initialize_wframes(Texture2D wFrame, Texture2D[] head_texes, Texture2D[] chest_texes,
                                        Texture2D[] larm_texes, Texture2D[] rarm_texes,
                                        Texture2D[] lleg_texes, Texture2D[] rleg_texes)
        {
            wireFrame = wFrame;
            head_textures = head_texes;
            chest_textures = chest_texes;
            larm_textures = larm_texes;
            rarm_textures = rarm_texes;
            lleg_textures = lleg_texes;
            rleg_textures = rleg_texes;
        }

        public void update_wound_report(int hw, int cw, int law, int raw, int llw, int rlw)
        {
            head_wounds = hw;
            chest_wounds = cw;
            larm_wounds = law;
            rarm_wounds = raw;
            lleg_wounds = llw;
            rleg_wounds = rlw;
        }

        public void draw_me(ref SpriteBatch sBatch)
        {
            sBatch.Begin(SpriteSortMode.BackToFront, null);
            sBatch.Draw(wireFrame, my_size, Color.White);
            sBatch.End();

            sBatch.Begin(SpriteSortMode.BackToFront, null);
            sBatch.Draw(chest_textures[Math.Min(chest_wounds, 3)], my_size, Color.White);
            sBatch.Draw(larm_textures[Math.Min(larm_wounds, 3)], my_size, Color.White);
            sBatch.Draw(rarm_textures[Math.Min(rarm_wounds, 3)], my_size, Color.White);
            sBatch.Draw(lleg_textures[Math.Min(lleg_wounds, 3)], my_size, Color.White);
            sBatch.Draw(rleg_textures[Math.Min(rleg_wounds, 3)], my_size, Color.White);
            sBatch.Draw(head_textures[Math.Min(head_wounds, 1)], my_size, Color.White);
            sBatch.End();
        }
    }
}
