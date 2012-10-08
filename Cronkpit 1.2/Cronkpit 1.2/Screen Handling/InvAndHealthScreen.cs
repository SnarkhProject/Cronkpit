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

        //Literally all of this is injury related stuff
        string inj_title;
        Rectangle BGElement_injurySummaryBackground;
        int isbBG_height = 390;
        int isbBG_width = 170;
        int isbBG_xPos = 10;
        int isbBG_yPos = 10;
        Vector2 inj_msg_pos;
        Vector2 inj_title_pos;
        SpriteFont injFont;
        int inj_start_index;
        int inj_messages_shown;

        Texture2D my_back_texture;
        Texture2D[] my_chest_textures;
        Texture2D[] my_larm_textures;
        Texture2D[] my_rarm_textures;
        Texture2D[] my_lleg_textures;
        Texture2D[] my_rleg_textures;
        Texture2D[] my_head_textures;

        Texture2D injSummary_scroll_up_max;
        Texture2D injSummary_scroll_up_one;
        Texture2D injSummary_scroll_down_max;
        Texture2D injSummary_scroll_down_one;

        Rectangle injSummary_scroll_up_max_rect;
        Rectangle injSummary_scroll_up_one_rect;
        Rectangle injSummary_scroll_down_max_rect;
        Rectangle injSummary_scroll_down_one_rect;

        //We finally leave that here to go into COLORS
        Color my_dark_color;
        Color my_grey_color;
        Color my_red_color;
        Color my_text_color;

        int mode;

        bool visible;

        SpriteFont section_titleFont;

        //Player information:
        int pl_head_wounds;
        int pl_chest_wounds;
        int pl_larm_wounds;
        int pl_rarm_wounds;
        int pl_lleg_wounds;
        int pl_rleg_wounds;

        List<string> pl_injury_report;

        //There's gonna have to be fonts and stuff here too for now, but this will be okay
        //FOR THE TIME BEING...

        public InvAndHealthBox(Texture2D my_default_backTex, SpriteFont smallFont, SpriteFont largeFont)
        {
            visible = false;
            mode = 0;
            section_titleFont = largeFont;

            my_size = new Rectangle(my_xPosition, my_yPosition, width, height);
            BGElement_portraitBackground = new Rectangle(my_xPosition + ptBG_xPos, my_yPosition + ptBG_yPos, 
                                                        ptBG_width, ptBG_height);
            BGElement_backpackBackground = new Rectangle(my_xPosition + bpbBG_xPos, my_yPosition + bpbBG_yPos,
                                                        bpbBG_width, bpbBG_height);
            int equip_start_X = my_xPosition + ptBG_xPos + ptBG_width + esbBG_xPos;
            int injury_start_X = equip_start_X + esbBG_width + isbBG_xPos;
            BGElement_equipSummaryBackground = new Rectangle(equip_start_X, my_yPosition + esbBG_yPos, 
                                                        esbBG_width, esbBG_height);
            //Injury stuff.
            inj_title = "Health:";
            int title_spacing = section_titleFont.LineSpacing;
            BGElement_injurySummaryBackground = new Rectangle(injury_start_X, my_yPosition + isbBG_yPos,
                                                            isbBG_width, isbBG_height);
            inj_title_pos = new Vector2(injury_start_X + 10, my_yPosition + isbBG_yPos);
            inj_msg_pos = new Vector2(injury_start_X + 10, my_yPosition + isbBG_yPos + title_spacing + 10);
            inj_start_index = 0;
            injFont = smallFont;
            inj_messages_shown = (int)((isbBG_height - title_spacing) / injFont.LineSpacing);
            

            int scrollElements_x = injury_start_X + isbBG_width - 20;
            int spacing = 4;
            int first_y = my_yPosition + isbBG_yPos + title_spacing + spacing;
            int second_y = my_yPosition + isbBG_yPos + 18 + title_spacing + (spacing * 2);
            int third_y = my_yPosition + isbBG_yPos + isbBG_height - 18 - spacing;
            int fourth_y = my_yPosition + isbBG_yPos + isbBG_height - (18*2) - (spacing*2);
            injSummary_scroll_up_max_rect = new Rectangle(scrollElements_x, first_y, 18, 18);
            injSummary_scroll_up_one_rect = new Rectangle(scrollElements_x, second_y, 18, 18);
            injSummary_scroll_down_max_rect = new Rectangle(scrollElements_x, third_y, 18, 18);
            injSummary_scroll_down_one_rect = new Rectangle(scrollElements_x, fourth_y, 18, 18);

            

            my_dark_color = new Color(0, 0, 0);
            my_grey_color = new Color(100, 100, 100);
            my_red_color = new Color(255, 0, 0);
            my_text_color = new Color(255, 255, 255);

            my_back_texture = my_default_backTex;
            my_back_texture.SetData(new[] { Color.White });
            pl_injury_report = new List<string>();
        }

        public void init_textures(Texture2D wFrameTex, Texture2D[] chest_textures, 
                                    Texture2D[] larm_textures, Texture2D[] rarm_textures,
                                    Texture2D[] lleg_textures, Texture2D[] rleg_textures,
                                    Texture2D[] head_textures, 
                                    Texture2D inj_scum, Texture2D inj_scuo, 
                                    Texture2D inj_scdm, Texture2D inj_scdo)
        {
            character_wireframe = wFrameTex;
            my_chest_textures = chest_textures;
            my_larm_textures = larm_textures;
            my_rarm_textures = rarm_textures;
            my_lleg_textures = lleg_textures;
            my_rleg_textures = rleg_textures;
            my_head_textures = head_textures;

            injSummary_scroll_up_max = inj_scum;
            injSummary_scroll_up_one = inj_scuo;
            injSummary_scroll_down_max = inj_scdm;
            injSummary_scroll_down_one = inj_scdo;
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

        public void update_player_info(ref Player pl)
        {
            pl.wound_report(out pl_head_wounds, out pl_chest_wounds, out pl_rarm_wounds,
                            out pl_larm_wounds, out pl_lleg_wounds, out pl_rleg_wounds);
            inj_start_index = 0;
            pl_injury_report.Clear();
            pl_injury_report = pl.detailed_wound_report();
        }

        public void scroll_inj_MSG(int scrollvalue)
        {
            inj_start_index += scrollvalue;
            if (inj_start_index > pl_injury_report.Count)
            {
                inj_start_index = pl_injury_report.Count;
            }
            else if (inj_start_index < 0)
                inj_start_index = 0;
        }

        public void mouseClick(Vector2 clickLoc)
        {
            if (injSummary_scroll_up_max_rect.Contains((int)clickLoc.X, (int)clickLoc.Y))
            {
                scroll_inj_MSG(-1000);
            }

            if (injSummary_scroll_up_one_rect.Contains((int)clickLoc.X, (int)clickLoc.Y))
            {
                scroll_inj_MSG(-1);
            }

            if (injSummary_scroll_down_one_rect.Contains((int)clickLoc.X, (int)clickLoc.Y))
            {
                scroll_inj_MSG(1);
            }

            if (injSummary_scroll_down_max_rect.Contains((int)clickLoc.X, (int)clickLoc.Y))
            {
                scroll_inj_MSG(1000);
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
            sBatch.Draw(my_chest_textures[Math.Min(pl_chest_wounds, 3)], BGElement_portraitBackground, Color.White);
            sBatch.Draw(my_larm_textures[Math.Min(pl_larm_wounds, 3)], BGElement_portraitBackground, Color.White);
            sBatch.Draw(my_rarm_textures[Math.Min(pl_rarm_wounds, 3)], BGElement_portraitBackground, Color.White);
            sBatch.Draw(my_lleg_textures[Math.Min(pl_lleg_wounds, 3)], BGElement_portraitBackground, Color.White);
            sBatch.Draw(my_rleg_textures[Math.Min(pl_rleg_wounds, 3)], BGElement_portraitBackground, Color.White);
            sBatch.Draw(my_head_textures[Math.Min(pl_head_wounds, 1)], BGElement_portraitBackground, Color.White);

            Vector2 injury_msg_pos2 = new Vector2(inj_msg_pos.X, inj_msg_pos.Y);
            //Then text
            sBatch.DrawString(section_titleFont, inj_title, inj_title_pos, my_text_color);
            for (int i = inj_start_index; i < Math.Min(inj_start_index + inj_messages_shown, pl_injury_report.Count); i++)
            {
                sBatch.DrawString(injFont, pl_injury_report[i], injury_msg_pos2, my_text_color);
                injury_msg_pos2.Y += injFont.LineSpacing;
            }

            sBatch.Draw(injSummary_scroll_up_max, injSummary_scroll_up_max_rect, Color.White);
            sBatch.Draw(injSummary_scroll_up_one, injSummary_scroll_up_one_rect, Color.White);
            sBatch.Draw(injSummary_scroll_down_max, injSummary_scroll_down_max_rect, Color.White);
            sBatch.Draw(injSummary_scroll_down_one, injSummary_scroll_down_one_rect, Color.White);
        }

        public void draw_me(ref SpriteBatch sBatch)
        {
            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_my_back(ref sBatch);
            sBatch.End();

            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_my_front(ref sBatch);
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
