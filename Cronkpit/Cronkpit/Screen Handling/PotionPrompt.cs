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
    class PotionPrompt
    {
        int x_position = 290;
        int y_position = 40;
        int height = 410;
        int width = 220;
        int std_border_width = 4;

        Color my_dark_color;
        Color my_red_color;
        Color my_grey_color;
        Color my_text_color;
        Color my_d_grey_color;
        Color my_orange_color;
        Color my_m_yellow_color;

        Rectangle my_size;
        Rectangle my_grey_foreground;
        Rectangle head_zone;
        Rectangle larm_zone;
        Rectangle rarm_zone;
        Rectangle torso_zone;
        Rectangle r_leg_zone;
        Rectangle l_leg_zone;
        Rectangle ingest_zone;
        Rectangle cancel_zone;

        Vector2 torso_center;
        Vector2 larm_center;
        Vector2 rarm_center;
        Vector2 lleg_center;
        Vector2 rleg_center;

        Rectangle over_armor_tab;
        Rectangle under_armor_tab;
        bool repair_over_armor;

        Texture2D wireframe_texture;
        Texture2D[] head_textures;
        Texture2D[] larm_textures;
        Texture2D[] rarm_textures;
        Texture2D[] lleg_textures;
        Texture2D[] rleg_textures;
        Texture2D[] chest_textures;
        Texture2D my_blank_texture;

        SpriteFont text_font;
        SpriteFont big_text_font;

        int larm_wounds;
        int rarm_wounds;
        int chest_wounds;
        int lleg_wounds;
        int rleg_wounds;
        int head_wounds;

        Armor player_oa;
        Armor player_ua;

        string ingest_prompt = "Ingest Potion";
        string cancel_prompt = "Nevermind";
        string under_tab_prompt = "Underarmor";
        string over_tab_prompt = "Overarmor";

        bool visible = false;

        Potion p;

        public PotionPrompt(Texture2D blank_tex, SpriteFont tFont, SpriteFont tFont_bold)
        {
            my_size = new Rectangle(x_position, y_position, width, height);
            my_grey_foreground = new Rectangle(x_position + 10, y_position + 10, width - 20, height - 20);

            head_zone = new Rectangle(x_position + 79, y_position + 60, 60, 60);
            larm_zone = new Rectangle(x_position + 29, y_position + 120, 60, 90);
            rarm_zone = new Rectangle(x_position + 129, y_position + 120, 60, 90);
            torso_zone = new Rectangle(x_position + 89, y_position + 120, 40, 90);
            r_leg_zone = new Rectangle(x_position + 109, y_position + 210, 40, 125);
            l_leg_zone = new Rectangle(x_position + 69, y_position + 210, 40, 125);
            ingest_zone = new Rectangle(x_position + 20, y_position + height - 70, width - 40, 20);
            cancel_zone = new Rectangle(x_position + 20, y_position + height - 40, width - 40, 20);

            torso_center = new Vector2(torso_zone.X + torso_zone.Width / 2, torso_zone.Y + torso_zone.Height / 2);
            larm_center = new Vector2(larm_zone.X + larm_zone.Width / 2, larm_zone.Y + larm_zone.Height / 2);
            rarm_center = new Vector2(rarm_zone.X + rarm_zone.Width / 2, rarm_zone.Y + rarm_zone.Height / 2);
            lleg_center = new Vector2(l_leg_zone.X + l_leg_zone.Width / 2, l_leg_zone.Y + l_leg_zone.Height / 2);
            rleg_center = new Vector2(r_leg_zone.X + r_leg_zone.Width / 2, r_leg_zone.Y + r_leg_zone.Height / 2);

            int tab_width = (int)(tFont.MeasureString(under_tab_prompt).X + 5);
            int tab_height = (int)tFont.LineSpacing;
            int tab_x_position = x_position + 10 + my_grey_foreground.Width - tab_width;
            over_armor_tab = new Rectangle(tab_x_position, y_position + 40, tab_width, tab_height);
            under_armor_tab = new Rectangle(tab_x_position, y_position + 40 + tab_height + 5, tab_width, tab_height);

            repair_over_armor = true;

            my_blank_texture = blank_tex;
            text_font = tFont;
            big_text_font = tFont_bold;

            my_dark_color = new Color(0, 0, 0);
            my_grey_color = new Color(100, 100, 100);
            my_red_color = new Color(255, 0, 0);
            my_text_color = new Color(255, 255, 255);
            my_d_grey_color = new Color(70, 70, 70);
            my_orange_color = new Color(255, 153, 0);
            my_m_yellow_color = new Color(170, 170, 50);
        }

        public void init_textures(Texture2D wframe_tex, Texture2D[] head_texes, Texture2D[] larm_texes,
                                Texture2D[] rarm_texes, Texture2D[] lleg_texes, Texture2D[] rleg_texes,
                                Texture2D[] chest_texes)
        {
            wireframe_texture = wframe_tex;
            head_textures = head_texes;
            chest_textures = chest_texes;
            larm_textures = larm_texes;
            rarm_textures = rarm_texes;
            lleg_textures = lleg_texes;
            rleg_textures = rleg_texes;
        }

        public void grab_injury_report(Player pl)
        {
            pl.wound_report(out head_wounds, out chest_wounds, out rarm_wounds, 
                            out larm_wounds, out lleg_wounds, out rleg_wounds);
            player_oa = pl.show_over_armor();
            player_ua = pl.show_under_armor();
        }

        #region zone clicks

        public bool clicked_head_zone(Vector2 mousePosition)
        {
            return head_zone.Contains((int)mousePosition.X, (int)mousePosition.Y);
        }

        public bool clicked_torso_zone(Vector2 mousePosition)
        {
            return torso_zone.Contains((int)mousePosition.X, (int)mousePosition.Y);
        }

        public bool clicked_larm_zone(Vector2 mousePosition)
        {
            return larm_zone.Contains((int)mousePosition.X, (int)mousePosition.Y);
        }

        public bool clicked_rarm_zone(Vector2 mousePosition)
        {
            return rarm_zone.Contains((int)mousePosition.X, (int)mousePosition.Y);
        }

        public bool clicked_lleg_zone(Vector2 mousePosition)
        {
            return l_leg_zone.Contains((int)mousePosition.X, (int)mousePosition.Y);
        }

        public bool clicked_rleg_zone(Vector2 mousePosition)
        {
            return r_leg_zone.Contains((int)mousePosition.X, (int)mousePosition.Y);
        }

        public bool clicked_ingest_zone(Vector2 mousePosition)
        {
            return ingest_zone.Contains((int)mousePosition.X, (int)mousePosition.Y);
        }

        public bool clicked_cancel_zone(Vector2 mousePosition)
        {
            return cancel_zone.Contains((int)mousePosition.X, (int)mousePosition.Y);
        }

        public bool clicked_OA_tab(Vector2 mousePosition)
        {
            return over_armor_tab.Contains((int)mousePosition.X, (int)mousePosition.Y);
        }

        public bool clicked_UA_tab(Vector2 mousePosition)
        {
            return under_armor_tab.Contains((int)mousePosition.X, (int)mousePosition.Y);
        }

        public bool clicked_a_zone(Vector2 mousePosition)
        {
            return (clicked_cancel_zone(mousePosition) || clicked_ingest_zone(mousePosition) ||
                    clicked_head_zone(mousePosition) || clicked_torso_zone(mousePosition) ||
                    clicked_larm_zone(mousePosition) || clicked_rarm_zone(mousePosition) ||
                    clicked_lleg_zone(mousePosition) || clicked_rleg_zone(mousePosition));
        }

        #endregion

        #region show() + hide() / fetch + set current potion

        public void current_potion(Potion pt)
        {
            p = pt;
        }

        public Potion fetch_current_potion()
        {
            return p;
        }

        public void clear_potion()
        {
            p = null;
        }

        public void hide()
        {
            visible = false;
        }

        public void show()
        {
            visible = true;
        }

        #endregion

        public void set_repair_armor(bool overArmor)
        {
            repair_over_armor = overArmor;
        }

        public bool get_repair_armor()
        {
            return repair_over_armor;
        }

        public bool is_visible()
        {
            return visible;
        }

        #region drawing stuff

        public void draw_my_background(ref SpriteBatch sBatch)
        {
            sBatch.Draw(my_blank_texture, my_size, my_dark_color);
        }

        public void draw_my_foreground(ref SpriteBatch sBatch)
        {
            sBatch.Draw(my_blank_texture, my_grey_foreground, my_grey_color);
        }

        public void draw_my_zone_backgrounds(ref SpriteBatch sBatch)
        {
            sBatch.Draw(my_blank_texture, cancel_zone, my_d_grey_color);
            sBatch.Draw(my_blank_texture, ingest_zone, my_d_grey_color);

            if (p.get_type() == Potion.Potion_Type.Repair)
            {
                Color OA_tab_color = my_d_grey_color;
                if (repair_over_armor)
                    OA_tab_color = my_grey_color;

                Color UA_tab_color = my_d_grey_color;
                if (!repair_over_armor)
                    UA_tab_color = my_grey_color;

                sBatch.Draw(my_blank_texture, over_armor_tab, OA_tab_color);
                sBatch.Draw(my_blank_texture, under_armor_tab, UA_tab_color);
            }
        }

        public void draw_border_around_rectangle(ref SpriteBatch sBatch, Rectangle target_rect, int border_width, Color border_color)
        {
            sBatch.Draw(my_blank_texture, new Rectangle(target_rect.Left, target_rect.Top, border_width, target_rect.Height), border_color);
            sBatch.Draw(my_blank_texture, new Rectangle(target_rect.Right, target_rect.Top, border_width, target_rect.Height + border_width), border_color);
            sBatch.Draw(my_blank_texture, new Rectangle(target_rect.Left, target_rect.Top, target_rect.Width, border_width), border_color);
            sBatch.Draw(my_blank_texture, new Rectangle(target_rect.Left, target_rect.Bottom, target_rect.Width, border_width), border_color);
        }

        public void draw_my_wireframes(ref SpriteBatch sBatch)
        {
            Vector2 wframe_position = new Vector2(my_grey_foreground.X, my_grey_foreground.Y - 60);
            sBatch.Draw(wireframe_texture, wframe_position, Color.White);
            sBatch.Draw(head_textures[Math.Min(head_wounds, 1)], wframe_position, Color.White);
            sBatch.Draw(chest_textures[Math.Min(chest_wounds, 3)], wframe_position, Color.White);
            sBatch.Draw(larm_textures[Math.Min(larm_wounds, 3)], wframe_position, Color.White);
            sBatch.Draw(rarm_textures[Math.Min(rarm_wounds, 3)], wframe_position, Color.White);
            sBatch.Draw(lleg_textures[Math.Min(lleg_wounds, 3)], wframe_position, Color.White);
            sBatch.Draw(rleg_textures[Math.Min(rleg_wounds, 3)], wframe_position, Color.White);
        }

        public void draw_my_text(ref SpriteBatch sBatch)
        {
            Vector2 prompt_position = new Vector2(my_grey_foreground.X + 5, my_grey_foreground.Y + 5);
            string prompt_prompt = "Apply " + p.get_my_name() + ":";

            sBatch.DrawString(text_font, prompt_prompt, prompt_position, my_text_color);

            Vector2 cancel_position = new Vector2(cancel_zone.X + (cancel_zone.Width - text_font.MeasureString(cancel_prompt).X)/2, cancel_zone.Y + 5);
            sBatch.DrawString(text_font, cancel_prompt, cancel_position, my_text_color);
            
            Vector2 ingest_position = new Vector2(ingest_zone.X + (ingest_zone.Width - text_font.MeasureString(ingest_prompt).X)/2, ingest_zone.Y + 5);
            sBatch.DrawString(text_font, ingest_prompt, ingest_position, my_text_color);

            if (p.get_type() == Potion.Potion_Type.Repair)
            {
                Vector2 over_prompt_position = new Vector2(over_armor_tab.X + 2, over_armor_tab.Y);
                Vector2 under_prompt_position = new Vector2(under_armor_tab.X + 2, under_armor_tab.Y);
                sBatch.DrawString(text_font, over_tab_prompt, over_prompt_position, my_orange_color);
                sBatch.DrawString(text_font, under_tab_prompt, under_prompt_position, my_m_yellow_color);
            }

            if (player_oa != null)
                draw_armor_integrity_ratio(sBatch, player_oa, my_orange_color, true);

            if (player_ua != null)
            {
                draw_armor_integrity_ratio(sBatch, player_ua, my_m_yellow_color, false);
            }
        }

        public void draw_armor_integrity_ratio(SpriteBatch sBatch, Armor target_armor, Color target_color, bool above)
        {
            string armor_max_c_integ = target_armor.get_max_c_integ().ToString();
            string armor_max_integ = target_armor.get_max_integ().ToString();

            int line_spacing = big_text_font.LineSpacing;
            if (above)
                line_spacing *= -1;

            string armor_chest_report = target_armor.get_chest_integ().ToString() + "/" + armor_max_c_integ;
            Vector2 armor_chest_rep_position = new Vector2(torso_center.X - big_text_font.MeasureString(armor_chest_report).X / 2, torso_center.Y + line_spacing);
            sBatch.DrawString(big_text_font, armor_chest_report, armor_chest_rep_position, target_color);

            string armor_larm_report = target_armor.get_larm_integ().ToString() + "/" + armor_max_integ;
            Vector2 armor_larm_rep_position = new Vector2(larm_center.X - big_text_font.MeasureString(armor_larm_report).X / 2, larm_center.Y + line_spacing);
            sBatch.DrawString(big_text_font, armor_larm_report, armor_larm_rep_position, target_color);

            string armor_rarm_report = target_armor.get_rarm_integ().ToString() + "/" + armor_max_integ;
            Vector2 armor_rarm_rep_position = new Vector2(rarm_center.X - big_text_font.MeasureString(armor_rarm_report).X / 2, rarm_center.Y + line_spacing);
            sBatch.DrawString(big_text_font, armor_rarm_report, armor_rarm_rep_position, target_color);

            string armor_lleg_report = target_armor.get_lleg_integ().ToString() + "/" + armor_max_integ;
            Vector2 armor_lleg_rep_position = new Vector2(lleg_center.X - big_text_font.MeasureString(armor_lleg_report).X / 2, lleg_center.Y + line_spacing);
            sBatch.DrawString(big_text_font, armor_lleg_report, armor_lleg_rep_position, target_color);

            string armor_rleg_report = target_armor.get_rleg_integ().ToString() + "/" + armor_max_integ;
            Vector2 armor_rleg_rep_position = new Vector2(rleg_center.X - big_text_font.MeasureString(armor_rleg_report).X / 2, rleg_center.Y + line_spacing);
            sBatch.DrawString(big_text_font, armor_rleg_report, armor_rleg_rep_position, target_color);
        }

        public void draw_me(ref SpriteBatch sBatch)
        {
            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_my_background(ref sBatch);
            sBatch.End();

            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_my_foreground(ref sBatch);
            sBatch.End();

            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_my_zone_backgrounds(ref sBatch);
            sBatch.End();

            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_my_wireframes(ref sBatch);
            sBatch.End();

            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_border_around_rectangle(ref sBatch, my_size, std_border_width, my_red_color);
            //draw_border_around_rectangle(ref sBatch, head_zone, std_border_width, my_red_color);
            //draw_border_around_rectangle(ref sBatch, larm_zone, std_border_width, my_red_color);
            //draw_border_around_rectangle(ref sBatch, rarm_zone, std_border_width, my_red_color);
            //draw_border_around_rectangle(ref sBatch, torso_zone, std_border_width, my_red_color);
            //draw_border_around_rectangle(ref sBatch, r_leg_zone, std_border_width, my_red_color);
            //draw_border_around_rectangle(ref sBatch, l_leg_zone, std_border_width, my_red_color);
            draw_border_around_rectangle(ref sBatch, ingest_zone, std_border_width/2, my_text_color);
            draw_border_around_rectangle(ref sBatch, cancel_zone, std_border_width/2, my_text_color);
            sBatch.End();

            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_my_text(ref sBatch);
            sBatch.End();
        }

        #endregion
    }
}
