using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using CKPLibrary;

namespace Cronkpit
{
    class CharSelect
    {
        private enum Menu_Mode { CharSelect, ClassSelect };
        int current_character_selected;
        int current_class_selected;
        List<Texture2D> char_class_img;
        List<Texture2D> character_images;
        List<string> character_names;
        ContentManager cManager;

        Rectangle client_rect;
        Rectangle char_img_rect;

        List<string> petaer_info;
        List<string> ziktofel_info;
        List<string> halephon_info;
        List<string> falsael_info;

        SpriteFont text_font;
        SpriteFont title_font;
        Menu_Mode my_mode;

        List<Rectangle> class_iconrects;
        List<Texture2D> class_icontexts;
        List<string> class_names;
        
        
        public CharSelect(ref ContentManager content, SpriteFont s_font, SpriteFont b_font,
                          Rectangle client)
        {
            character_images = new List<Texture2D>();
            char_class_img = new List<Texture2D>();
            character_names = new List<string>();
            cManager = content;
            my_mode = Menu_Mode.CharSelect;

            petaer_info = new List<string>();
            ziktofel_info = new List<string>();
            halephon_info = new List<string>();
            falsael_info = new List<string>();

            current_character_selected = 0;
            text_font = s_font;
            title_font = b_font;
            client_rect = client;

            int baseXcoord = 270;
            int baseYcoord = 80;
            int spacing = 70;
            int size = 64;
            class_iconrects = new List<Rectangle>();
            class_iconrects.Add(new Rectangle(baseXcoord, baseYcoord, size, size));
            class_iconrects.Add(new Rectangle(baseXcoord + (spacing), baseYcoord, size, size));
            class_iconrects.Add(new Rectangle(baseXcoord, baseYcoord + (spacing), size, size));
            class_iconrects.Add(new Rectangle(baseXcoord + (spacing), baseYcoord + (spacing), size, size));
            char_img_rect = new Rectangle(15, 15, 240, 575);
            class_icontexts = new List<Texture2D>();
            class_names = new List<string>();
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

            class_icontexts.Clear();
            class_icontexts.Add(cManager.Load<Texture2D>("Icons/Class/warriorclass_icon"));
            class_icontexts.Add(cManager.Load<Texture2D>("Icons/Class/rogueclass_icon"));
            class_icontexts.Add(cManager.Load<Texture2D>("Icons/Class/mageclass_icon"));
            class_icontexts.Add(cManager.Load<Texture2D>("Icons/Class/expriestclass_icon"));
            class_names.Add("Warrior");
            class_names.Add("Rogue");
            class_names.Add("Mage");
            class_names.Add("ExPriest");

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
            falsael_info.Add("with melee attacks");
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
            switch (my_mode)
            {
                case Menu_Mode.CharSelect:
                    current_character_selected += scroll_value;
                    if (current_character_selected == character_images.Count)
                        current_character_selected = 0;
                    else if (current_character_selected < 0)
                        current_character_selected = character_images.Count - 1;
                    break;
                case Menu_Mode.ClassSelect:
                    current_class_selected += scroll_value;
                    if (current_class_selected >= class_iconrects.Count)
                        current_class_selected = current_class_selected % class_iconrects.Count;
                    else if (current_class_selected < 0)
                        current_class_selected = class_iconrects.Count - 1;
                    break;
            }
        }

        public int get_current_character_selection()
        {
            return current_character_selected;
        }

        public int get_current_class_selection()
        {
            return current_class_selected;
        }

        public void switch_mode()
        {
            if (my_mode == Menu_Mode.CharSelect)
            {
                my_mode = Menu_Mode.ClassSelect;

                string basePath = "UI Elements/Large Chara Images/";
                //Order of class descriptors is
                //ExPriest
                //Warrior
                //Rogue
                //Mage
                char_class_img.Clear();
                switch (current_character_selected)
                {
                    case 0:
                        char_class_img.Add(cManager.Load<Texture2D>(basePath + "Petaer_large_condensed"));
                        char_class_img.Add(cManager.Load<Texture2D>(basePath + "Petaer_large_condensed"));
                        char_class_img.Add(cManager.Load<Texture2D>(basePath + "Petaer_large_condensed"));
                        char_class_img.Add(cManager.Load<Texture2D>(basePath + "Petaer_large_condensed"));
                        break;
                    case 1:
                        char_class_img.Add(cManager.Load<Texture2D>(basePath + "Ziktofel_large_condensed"));
                        char_class_img.Add(cManager.Load<Texture2D>(basePath + "Ziktofel_large_condensed"));
                        char_class_img.Add(cManager.Load<Texture2D>(basePath + "Ziktofel_large_condensed"));
                        char_class_img.Add(cManager.Load<Texture2D>(basePath + "Ziktofel_large_condensed"));
                        break;
                    case 2:
                        char_class_img.Add(cManager.Load<Texture2D>(basePath + "halephon_cleric"));
                        char_class_img.Add(cManager.Load<Texture2D>(basePath + "halephon_warrior"));
                        char_class_img.Add(cManager.Load<Texture2D>(basePath + "halephon_rogue"));
                        char_class_img.Add(cManager.Load<Texture2D>(basePath + "halephon_mage"));
                        break;
                    case 3:
                        char_class_img.Add(cManager.Load<Texture2D>(basePath + "Falsael_large_condensed"));
                        char_class_img.Add(cManager.Load<Texture2D>(basePath + "Falsael_large_condensed"));
                        char_class_img.Add(cManager.Load<Texture2D>(basePath + "Falsael_large_condensed"));
                        char_class_img.Add(cManager.Load<Texture2D>(basePath + "Falsael_large_condensed"));
                        break;
                }
            }
            else
                my_mode = Menu_Mode.CharSelect;
        }

        public bool selecting_character()
        {
            return my_mode == Menu_Mode.CharSelect;
        }

        public void draw_me(ref SpriteBatch sBatch)
        {
            switch (my_mode)
            {
                case Menu_Mode.CharSelect:
                    draw_charselect(ref sBatch);
                    break;
                case Menu_Mode.ClassSelect:
                    draw_classselect(ref sBatch);
                    break;
            }
        }

        public void draw_charselect(ref SpriteBatch sBatch)
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

                if (i == current_character_selected)
                    sBatch.DrawString(title_font, character_names[i], name_position, Color.White);

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

        public void draw_classselect(ref SpriteBatch sBatch)
        {
            //draw condensed icon on the right of the screen
            //draw classes in a 2-wide row to the left of that
            //put info to the left of that.
            sBatch.DrawString(text_font, "Select class:", new Vector2(270, 50), Color.White);

            string classname = "";
            for (int i = 0; i < class_iconrects.Count; i++)
            {
                Color tint = new Color(255, 255, 255, 80);
                if (i == current_class_selected)
                {
                    tint = Color.White;
                    classname = class_names[i];
                    sBatch.DrawString(title_font, classname, new Vector2(440, 80 - title_font.LineSpacing), Color.White);
                }

                sBatch.Draw(class_icontexts[i], class_iconrects[i], tint);
            }

            Vector2 classDesc_baseCoord = new Vector2(440, 80);
            
            //Draw class descriptions
            ClassDescDC[] class_descriptors = cManager.Load<ClassDescDC[]>("XmlData/classdescriptions");
            int target_index = -1;
            for (int a = 0; a < class_descriptors.Count(); a++)
            {
                if (String.Compare(class_descriptors[a].Selected_Class, classname) == 0)
                {
                    target_index = a;
                    break;
                }
            }

            if (target_index > -1)
            {
                for (int i = 0; i < class_descriptors[target_index].Description.Count; i++)
                {
                    sBatch.DrawString(text_font, class_descriptors[target_index].Description[i], classDesc_baseCoord, Color.White);
                    classDesc_baseCoord.Y += text_font.LineSpacing;
                }
            }

            if (target_index > -1)
            {
                sBatch.Draw(char_class_img[target_index], char_img_rect, Color.White);
            }
        }
    }
}
