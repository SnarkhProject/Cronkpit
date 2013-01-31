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
    class CharSelect
    {
        int current_character_selected;
        List<Texture2D> character_images;
        List<string> character_names;
        ContentManager cManager;

        Rectangle client_rect;

        List<string> petaer_info;
        List<string> ziktofel_info;
        List<string> halephon_info;
        List<string> falsael_info;

        SpriteFont text_font;
        SpriteFont title_font;
        
        public CharSelect(ref ContentManager content, SpriteFont s_font, SpriteFont b_font,
                          Rectangle client)
        {
            character_images = new List<Texture2D>();
            character_names = new List<string>();
            cManager = content;

            petaer_info = new List<string>();
            ziktofel_info = new List<string>();
            halephon_info = new List<string>();
            falsael_info = new List<string>();

            current_character_selected = 0;
            text_font = s_font;
            title_font = b_font;
            client_rect = client;
        }

        public void init_character_textures()
        {
            character_images.Clear();
            character_images.Add(cManager.Load<Texture2D>("UI Elements/Large Chara Images/Petaer_large"));
            character_images.Add(cManager.Load<Texture2D>("UI Elements/Large Chara Images/Ziktofel_large"));
            character_images.Add(cManager.Load<Texture2D>("UI Elements/Large Chara Images/Halephon_large"));
            character_images.Add(cManager.Load<Texture2D>("UI Elements/Large Chara Images/Falsael_large"));

            character_names.Add("Petaer");
            character_names.Add("Ziktofel");
            character_names.Add("Halephon");
            character_names.Add("Falsael");

            petaer_info.Clear();
            petaer_info.Add("- 20% more damage");
            petaer_info.Add("with spells");
            petaer_info.Add("- Personal buff spells");
            petaer_info.Add("last 30% longer");

            ziktofel_info.Clear();
            ziktofel_info.Add("- Potions heal");
            ziktofel_info.Add("60% more wounds");

            falsael_info.Clear();
            falsael_info.Add("- 20% more damage");
            falsael_info.Add("with melee weapons");
            falsael_info.Add("- 20% more damage");
            falsael_info.Add("with crossbows");

            halephon_info.Clear();
            halephon_info.Add("- Using a potion or");
            halephon_info.Add("changing equipment");
            halephon_info.Add("does not end your");
            halephon_info.Add("turn");
        }

        public void scroll_menu(int scroll_value)
        {
            current_character_selected += scroll_value;
            if (current_character_selected == character_images.Count)
                current_character_selected = 0;
            else if (current_character_selected < 0)
                current_character_selected = character_images.Count - 1;
        }

        public int get_current_selection()
        {
            return current_character_selected;
        }

        public void draw_me(ref SpriteBatch sBatch)
        {
            float name_X = client_rect.Width / 2;
            for (int i = 0; i < character_images.Count; i++)
            {
                Color tint;
                if (i != current_character_selected)
                    tint = new Color(255, 255, 255, 80);
                else
                    tint = Color.White;

                Vector2 name_measurement = title_font.MeasureString(character_names[i]);
                Vector2 name_position = new Vector2((client_rect.Width / 2) - (name_measurement.X / 2),
                                                     client_rect.Height / 8);
                name_X = name_position.X;

                if(i == current_character_selected)
                    sBatch.DrawString(title_font, character_names[i], name_position,Color.White);

                sBatch.Draw(character_images[i], new Vector2(0, 0), tint);
            }

            List<string> chara_info = new List<string>();
            switch (current_character_selected)
            {
                case 0:
                    chara_info = petaer_info;
                    break;
                case 1:
                    chara_info = ziktofel_info;
                    break;
                case 2:
                    chara_info = halephon_info;
                    break;
                case 3:
                    chara_info = falsael_info;
                    break;
            }
            Vector2 info_position = new Vector2(name_X, client_rect.Height / 4);
            for (int j = 0; j < chara_info.Count; j++)
            {
                sBatch.DrawString(text_font, chara_info[j], info_position, Color.White);
                info_position.Y += text_font.LineSpacing;
            }
        }
    }
}
