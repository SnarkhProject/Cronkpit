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
        
        public CharSelect(ref ContentManager content)
        {
            character_images = new List<Texture2D>();
            character_names = new List<string>();
            character_images.Add(content.Load<Texture2D>("UI Elements/Large Chara Images/Petaer_large"));
            character_images.Add(content.Load<Texture2D>("UI Elements/Large Chara Images/Ziktofel_large"));
            character_images.Add(content.Load<Texture2D>("UI Elements/Large Chara Images/Halephon_large"));
            character_images.Add(content.Load<Texture2D>("UI Elements/Large Chara Images/Falsael_large"));
            character_names.Add("Petaer");
            character_names.Add("Ziktofel");
            character_names.Add("Halephon");
            character_names.Add("Falsael");

            current_character_selected = 0;
        }

        public void scroll_menu(int scroll_value)
        {
            current_character_selected += scroll_value;
            if (current_character_selected == character_images.Count)
                current_character_selected = 0;
            else if (current_character_selected < 0)
                current_character_selected = character_images.Count - 1;
        }

        public void draw_me(ref SpriteBatch sBatch)
        {
            for (int i = 0; i < character_images.Count; i++)
            {
                Color tint;
                if (i != current_character_selected)
                    tint = new Color(255, 255, 255, 100);
                else
                    tint = Color.White;

                sBatch.Draw(character_images[i], new Vector2(0, 0), tint);
            }
        }
    }
}
