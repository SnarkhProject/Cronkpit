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
    class InvAndHealthBox
    {
        Rectangle my_size;
        int height = 480;
        int width = 580;
        int my_xPosition = 110;
        int my_yPosition = 20;

        Rectangle BGElement_portraitBackground;
        int ptBG_height = 390;
        int ptBG_width = 200;
        int ptBG_xPos = 10;
        int ptBG_yPos = 10;
        Texture2D character_wireframe;

        Rectangle BGElement_backpackBackground;
        int bpbBG_height = 70;
        int bpbBG_width = 560;
        int bpbBG_xPos = 10;
        int bpbBG_yPos = 405;

        Rectangle BGElement_equipSummaryBackground;
        int esbBG_height = 390;
        int esbBG_width = 170;
        int esbBG_xPos = 10;
        int esbBG_yPos = 10;

        Rectangle BGElement_injurySummaryBackground;
        int isbBG_height = 390;
        int isbBG_width = 170;
        int isbBG_xPos = 10;
        int isbBG_yPos = 10;

        Texture2D my_back_texture;

        Color my_dark_color;
        Color my_grey_color;
        Color my_red_color;
        Color my_text_color;

        int mode;

        bool visible;

        //There's gonna have to be fonts and stuff here too for now, but this will be okay
        //FOR THE TIME BEING...

        public InvAndHealthBox(Texture2D my_default_backTex)
        {
            visible = false;
            mode = 0;
            my_size = new Rectangle(my_xPosition, my_yPosition, width, height);
            BGElement_portraitBackground = new Rectangle(my_xPosition + ptBG_xPos, my_yPosition + ptBG_yPos, 
                                                        ptBG_width, ptBG_height);
            BGElement_backpackBackground = new Rectangle(my_xPosition + bpbBG_xPos, my_yPosition + bpbBG_yPos,
                                                        bpbBG_width, bpbBG_height);
            int equip_start_X = my_xPosition + ptBG_xPos + ptBG_width + esbBG_xPos;
            int injury_start_X = equip_start_X + esbBG_width + isbBG_xPos;
            BGElement_equipSummaryBackground = new Rectangle(equip_start_X, my_yPosition + esbBG_yPos, 
                                                        esbBG_width, esbBG_height);
            BGElement_injurySummaryBackground = new Rectangle(injury_start_X, my_yPosition + isbBG_yPos,
                                                            isbBG_width, isbBG_height);

            my_dark_color = new Color(0, 0, 0);
            my_grey_color = new Color(100, 100, 100);
            my_red_color = new Color(255, 0, 0);
            my_text_color = new Color(255, 255, 255);

            my_back_texture = my_default_backTex;
            my_back_texture.SetData(new[] { Color.White });
        }

        public void init_textures(Texture2D wFrameTex)
        {
            character_wireframe = wFrameTex;
        }

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

        public void switch_my_mode()
        {
            mode++;
            if (mode >= 2)
            {
                mode = 0;
            }
            switch (mode)
            {
                case 0:
                    hide();
                    break;
                case 1:
                    show();
                    break;
            }
        }

        #region drawing stuff

        public void draw_my_back(ref SpriteBatch sBatch)
        {
            sBatch.Draw(my_back_texture, my_size, my_dark_color);
        }

        public void draw_my_front(ref SpriteBatch sBatch)
        {
            sBatch.Draw(my_back_texture, BGElement_portraitBackground, my_grey_color);
            sBatch.Draw(my_back_texture, BGElement_backpackBackground, my_grey_color);
            sBatch.Draw(my_back_texture, BGElement_equipSummaryBackground, my_grey_color);
            sBatch.Draw(my_back_texture, BGElement_injurySummaryBackground, my_grey_color);
        }

        public void draw_my_borders(ref SpriteBatch sBatch)
        {
            int border_width = 4;
            sBatch.Draw(my_back_texture, new Rectangle(my_size.Left, my_size.Top, border_width, my_size.Height), my_red_color);
            sBatch.Draw(my_back_texture, new Rectangle(my_size.Right, my_size.Top, border_width, my_size.Height + border_width), my_red_color);
            sBatch.Draw(my_back_texture, new Rectangle(my_size.Left, my_size.Top, my_size.Width, border_width), my_red_color);
            sBatch.Draw(my_back_texture, new Rectangle(my_size.Left, my_size.Bottom, my_size.Width, border_width), my_red_color);
        }

        public void draw_my_text(ref SpriteBatch sBatch)
        {
            sBatch.Draw(character_wireframe, BGElement_portraitBackground, Color.White);
        }

        #endregion
    }
}
