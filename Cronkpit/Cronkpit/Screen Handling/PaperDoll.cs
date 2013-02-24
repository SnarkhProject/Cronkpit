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
        Texture2D[] texture_masks;

        int[] wounds_by_part;
        int[] max_health_by_part;

        int xsize = 80;
        int ysize = 125;

        public PaperDoll(Rectangle cli_rect)
        {
            client_rect = cli_rect;

            my_size = new Rectangle(30, client_rect.Height - (ysize+20), xsize, ysize);
        }

        public void initialize_wframes(Texture2D wFrame, Texture2D[] tex_masks)
        {
            wireFrame = wFrame;
            texture_masks = tex_masks;
        }

        public void update_wound_report(int[] wounds, int[] max_health )
        {
            wounds_by_part = wounds;
            max_health_by_part = max_health;
        }

        public void draw_me(ref SpriteBatch sBatch)
        {
            sBatch.Begin(SpriteSortMode.BackToFront, null);
            for (int i = 0; i < 6; i++)
            {
                Color part_color = Color.Blue;
                if (wounds_by_part[i] >= max_health_by_part[i])
                    part_color = Color.Red;

                else if (max_health_by_part[i] == 3)
                {
                    if (wounds_by_part[i] == 1)
                        part_color = new Color(0, 255, 0);
                    else if (wounds_by_part[i] == 2)
                        part_color = Color.Yellow;
                }

                sBatch.Draw(texture_masks[i], my_size, part_color);
            }
            sBatch.End();

            sBatch.Begin(SpriteSortMode.BackToFront, null);
            sBatch.Draw(wireFrame, my_size, Color.White);
            sBatch.End();
        }
    }
}
