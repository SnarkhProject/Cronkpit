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
    class ComparisonPopup
    {
        Rectangle my_size;
        Texture2D blank_texture;

        Color my_red_color;
        Color my_dark_color;

        Rectangle i1rect;
        Rectangle i2rect;
        Rectangle i3rect;

        Texture2D i1text;
        Texture2D i2text;
        Texture2D i3text;

        Vector2 textposition;

        SpriteFont std_textfont;
        SpriteFont small_textfont;

        Weapon w1;
        Weapon w2;
        Weapon w3;

        Armor a1;
        Armor a2;

        bool comparingArmor;

        public ComparisonPopup(Texture2D blnkTx, SpriteFont stdFont, SpriteFont smallFont, ContentManager cmg, Weapon pMainHand, Weapon comparisonWep, Weapon pOffHand)
        {
            blank_texture = blnkTx;
            my_size = new Rectangle(100, 100, 600, 400);
            comparingArmor = false;
            std_textfont = stdFont;
            small_textfont = smallFont;

            textposition = new Vector2(my_size.X + 20, my_size.Y + 160);

            i1rect = new Rectangle(my_size.X + 320, my_size.Y + 30, 48, 48);
            i2rect = new Rectangle(my_size.X + 420, my_size.Y + 30, 48, 48);
            i3rect = new Rectangle(my_size.X + 520, my_size.Y + 30, 48, 48);

            string bpath = "Icons/Weapons/";
            i1text = cmg.Load<Texture2D>(bpath + comparisonWep.get_my_texture_name());
            if (pMainHand != null)
                i2text = cmg.Load<Texture2D>(bpath + pMainHand.get_my_texture_name());
            else
                i2text = blank_texture;
            if (pOffHand != null)
                i3text = cmg.Load<Texture2D>(bpath + pOffHand.get_my_texture_name());
            else
                i3text = blank_texture;

            w1 = comparisonWep;
            w2 = pMainHand;
            w3 = pOffHand;           
            
            my_red_color = new Color(255, 0, 0);
            my_dark_color = new Color(0, 0, 0);
        }

        public ComparisonPopup(Texture2D blnkTx, SpriteFont stdFont, SpriteFont smallFont, ContentManager cmg, Armor pCArmor, Armor comparisonArmor)
        {
            blank_texture = blnkTx;
            my_size = new Rectangle(175, 90, 450, 420);
            comparingArmor = true;
            std_textfont = stdFont;
            small_textfont = smallFont;

            textposition = new Vector2(my_size.X + 20, my_size.Y + 140);

            i1rect = new Rectangle(my_size.X + 220, my_size.Y + 30, 48, 48);
            i2rect = new Rectangle(my_size.X + 320, my_size.Y + 30, 48, 48);

            string bpath = "Icons/Armors/";
            i1text = cmg.Load<Texture2D>(bpath + comparisonArmor.get_my_texture_name());
            if (pCArmor != null)
                i2text = cmg.Load<Texture2D>(bpath + pCArmor.get_my_texture_name());
            else
                i2text = blank_texture;

            a1 = comparisonArmor;
            a2 = pCArmor;

            my_red_color = new Color(255, 0, 0);
            my_dark_color = new Color(0, 0, 0);
        }

        public string dmgType2String(Attack.Damage dmgType)
        {
            switch (dmgType)
            {
                case Attack.Damage.Acid:
                    return "Acid";
                case Attack.Damage.Crushing:
                    return "Crushing";
                case Attack.Damage.Electric:
                    return "Electric";
                case Attack.Damage.Fire:
                    return "Fire";
                case Attack.Damage.Frost:
                    return "Frost";
                case Attack.Damage.Piercing:
                    return "Piercing";
                case Attack.Damage.Slashing:
                    return "Slashing";
            }

            return "Unknown";
        }

        public void draw_border_around_rectangle(ref SpriteBatch sBatch, Rectangle target_rect, int border_width, Color border_color)
        {
            sBatch.Draw(blank_texture, new Rectangle(target_rect.Left - border_width, target_rect.Top - border_width, border_width, target_rect.Height + (border_width * 2)), my_red_color);
            sBatch.Draw(blank_texture, new Rectangle(target_rect.Right, target_rect.Top, border_width, target_rect.Height + border_width), my_red_color);
            sBatch.Draw(blank_texture, new Rectangle(target_rect.Left - border_width, target_rect.Top - border_width, target_rect.Width + (border_width * 2), border_width), my_red_color);
            sBatch.Draw(blank_texture, new Rectangle(target_rect.Left, target_rect.Bottom, target_rect.Width, border_width), my_red_color);
        }

        public void draw_all_text(ref SpriteBatch sBatch)
        {
            Vector2 textposition2 = new Vector2(textposition.X, textposition.Y);
            Vector2 i1textPosition = new Vector2(i1rect.X + 16, textposition.Y);
            Vector2 i2textPosition = new Vector2(i2rect.X + 16, textposition.Y);
            Vector2 i3textPosition = new Vector2(i3rect.X + 16, textposition.Y);

            if (comparingArmor)
            {
                sBatch.DrawString(small_textfont, "Equipped:", new Vector2(i2rect.X - 12, i2rect.Top - (small_textfont.LineSpacing+8)), Color.White);
                int linedif = 34;
                sBatch.DrawString(std_textfont, "Slashing Absorb:", textposition2, Color.White);
                textposition2.Y += linedif;
                sBatch.DrawString(std_textfont, "Crushing Absorb:", textposition2, Color.White);
                textposition2.Y += linedif;
                sBatch.DrawString(std_textfont, "Piercing Absorb:", textposition2, Color.White);
                textposition2.Y += linedif;
                sBatch.DrawString(std_textfont, "Fire Absorb:", textposition2, Color.White);
                textposition2.Y += linedif;
                sBatch.DrawString(std_textfont, "Frost Absorb:", textposition2, Color.White);
                textposition2.Y += linedif;
                sBatch.DrawString(std_textfont, "Electric Absorb:", textposition2, Color.White);
                textposition2.Y += linedif;
                sBatch.DrawString(std_textfont, "Acid Absorb:", textposition2, Color.White);
                textposition2.Y += linedif;
                sBatch.DrawString(std_textfont, "Integrity:", textposition2, Color.White);

                draw_armorText(ref sBatch, a1, i1textPosition, false);
                if(a2 != null)
                    draw_armorText(ref sBatch, a2, i2textPosition, true);
            }
            else
            {
                sBatch.DrawString(small_textfont, "Equipped:", new Vector2(i2rect.X - 12, i2rect.Top - (small_textfont.LineSpacing + 8)), Color.White);
                sBatch.DrawString(small_textfont, "Equipped:", new Vector2(i3rect.X - 12, i3rect.Top - (small_textfont.LineSpacing + 8)), Color.White);
                int linedif = 50;
                sBatch.DrawString(std_textfont, "Min Damage:", textposition2, Color.White);
                textposition2.Y += linedif;
                sBatch.DrawString(std_textfont, "Max Damage:", textposition2, Color.White);
                textposition2.Y += linedif;
                sBatch.DrawString(std_textfont, "Damage Type:", textposition2, Color.White);
                textposition2.Y += linedif;
                sBatch.DrawString(std_textfont, "Hands:", textposition2, Color.White);
                textposition2.Y += linedif;
                sBatch.DrawString(std_textfont, "Range:", textposition2, Color.White);

                draw_weaponText(ref sBatch, w1, i1textPosition, false);
                if(w2 != null)
                    draw_weaponText(ref sBatch, w2, i2textPosition, true);
                if (w3 != null)
                    draw_weaponText(ref sBatch, w3, i3textPosition, true);
            }
        }

        public void draw_weaponText(ref SpriteBatch sBatch, Weapon wx, Vector2 ixtextPosition, bool compareW1)
        {
            int linedif = 50;
            int wmindmg = wx.specific_damage_val(false)*wx.get_hand_count();
            int wmaxdmg = wx.specific_damage_val(true)*wx.get_hand_count();
            Attack.Damage wdmgtyp = wx.get_my_damage_type();
            int whands = wx.get_hand_count();
            int wrange = wx.get_my_range();
            string[] wname = wx.get_my_name().Split(' ');
            SpriteFont nameFont = std_textfont;
            if (wx.get_my_name().Length > 10)
                nameFont = small_textfont;

            //Draw item name.
            Vector2 namePos = new Vector2(ixtextPosition.X, i1rect.Bottom + (nameFont.LineSpacing-10));
            for (int i = 0; i < wname.Count(); i++)
            {
                namePos.X = ixtextPosition.X - (nameFont.MeasureString(wname[i]).X / 2);
                sBatch.DrawString(nameFont, wname[i], namePos, Color.White);
                namePos.Y += (nameFont.LineSpacing - 5);
            }

            //Draw minimum damage
            sBatch.DrawString(std_textfont, wmindmg.ToString(), ixtextPosition, Color.White);
            //if comparison.
            if (compareW1)
            {
                int diff = (w1.specific_damage_val(false)*w1.get_hand_count()) - (wx.specific_damage_val(false)*wx.get_hand_count());
                draw_difference(diff, ixtextPosition, wmindmg.ToString(), ref sBatch, false);
            }
            ixtextPosition.Y += linedif;

            //draw maximum damage
            sBatch.DrawString(std_textfont, wmaxdmg.ToString(), ixtextPosition, Color.White);
            if (compareW1)
            {
                int diff = (w1.specific_damage_val(true) * w1.get_hand_count()) - (wx.specific_damage_val(true) * wx.get_hand_count());
                draw_difference(diff, ixtextPosition, wmaxdmg.ToString(), ref sBatch, false);
            }
            ixtextPosition.Y += linedif;

            //draw damage type
            string dtyp = dmgType2String(wdmgtyp);
            sBatch.DrawString(std_textfont, dtyp,
                              new Vector2(ixtextPosition.X - (std_textfont.MeasureString(dtyp).X / 2),
                              ixtextPosition.Y), Color.White);
            ixtextPosition.Y += linedif;

            //draw hands
            sBatch.DrawString(std_textfont, whands.ToString(), ixtextPosition, Color.White);
            if (compareW1)
            {
                int diff = w1.get_hand_count() - wx.get_hand_count();
                draw_difference(diff, ixtextPosition, whands.ToString(), ref sBatch, false);
            }
            ixtextPosition.Y += linedif;

            //draw range
            sBatch.DrawString(std_textfont, wrange.ToString(), ixtextPosition, Color.White);
            if (compareW1)
            {
                int diff = w1.get_my_range() - wx.get_my_range();
                draw_difference(diff, ixtextPosition, wrange.ToString(), ref sBatch, false);
            }
        }

        public void draw_armorText(ref SpriteBatch sBatch, Armor ax, Vector2 ixtextPosition, bool compareA1)
        {
            int linedif = 34;
            int axablative = ax.get_armor_value(Armor.Armor_Value.Ablative);
            int axinsulat = ax.get_armor_value(Armor.Armor_Value.Insulative);
            int axpadding = ax.get_armor_value(Armor.Armor_Value.Padding);
            int axhardness = ax.get_armor_value(Armor.Armor_Value.Hardness);
            int axrigidty = ax.get_armor_value(Armor.Armor_Value.Rigidness);
            int slashingAbsorb = (axhardness * 4) + (axrigidty * 2);
            int piercingAbsorb = (axhardness * 4) + (axpadding * 2);
            int crushingAbsorb = (axrigidty * 4) + (axpadding * 2);
            int fireAbsorb = (axablative * 4) + (axrigidty * 2);
            int frostAbsorb = (axpadding * 4) + (axinsulat * 2);
            int elecAbsorb = (axinsulat * 4) + (axpadding * 2);
            int acidAbsorb = (axinsulat * 4) + (axablative * 2);
            int integrity = ax.get_max_integ();
            string[] wname = ax.get_my_name().Split(' ');
            SpriteFont nameFont = std_textfont;
            if (ax.get_my_name().Length > 10 || wname.Count() > 1)
                nameFont = small_textfont;

            //Draw item name.
            Vector2 namePos = new Vector2(ixtextPosition.X, i1rect.Bottom + (nameFont.LineSpacing - 10));
            for (int i = 0; i < wname.Count(); i++)
            {
                namePos.X = ixtextPosition.X - (nameFont.MeasureString(wname[i]).X / 2);
                sBatch.DrawString(nameFont, wname[i], namePos, Color.White);
                namePos.Y += (nameFont.LineSpacing - 5);
            }

            //Draw slashing absorb
            sBatch.DrawString(std_textfont, slashingAbsorb.ToString() + "%", ixtextPosition, Color.White);
            //if comparison.
            if (compareA1)
            {
                int a1slashingabsorb = (a1.get_armor_value(Armor.Armor_Value.Hardness) * 4) + (a1.get_armor_value(Armor.Armor_Value.Rigidness) * 2);
                draw_difference(a1slashingabsorb - slashingAbsorb, ixtextPosition, slashingAbsorb.ToString(), ref sBatch, true);
            }
            ixtextPosition.Y += linedif;

            //draw crushing absorb
            sBatch.DrawString(std_textfont, crushingAbsorb.ToString() + "%", ixtextPosition, Color.White);
            if (compareA1)
            {
                int a1crushingabsorb = (a1.get_armor_value(Armor.Armor_Value.Hardness) * 4) + (a1.get_armor_value(Armor.Armor_Value.Padding) * 2);
                draw_difference(a1crushingabsorb - crushingAbsorb, ixtextPosition, crushingAbsorb.ToString(), ref sBatch, true);
            }
            ixtextPosition.Y += linedif;

            //draw piercing absorb
            sBatch.DrawString(std_textfont, piercingAbsorb.ToString() + "%", ixtextPosition, Color.White);
            if (compareA1)
            {
                int a1piercingabsorb = (a1.get_armor_value(Armor.Armor_Value.Hardness) * 4) + (a1.get_armor_value(Armor.Armor_Value.Padding) * 2);
                draw_difference(a1piercingabsorb - piercingAbsorb, ixtextPosition, piercingAbsorb.ToString(), ref sBatch, true);
            }
            ixtextPosition.Y += linedif;

            //draw fire absorb
            sBatch.DrawString(std_textfont, fireAbsorb.ToString() + "%", ixtextPosition, Color.White);
            if (compareA1)
            {
                int a1fireabsorb = (a1.get_armor_value(Armor.Armor_Value.Ablative) * 4) + (a1.get_armor_value(Armor.Armor_Value.Hardness) * 2);
                draw_difference(a1fireabsorb - fireAbsorb, ixtextPosition, fireAbsorb.ToString(), ref sBatch, true);
            }
            ixtextPosition.Y += linedif;

            //draw frost absorb
            sBatch.DrawString(std_textfont, frostAbsorb.ToString() + "%", ixtextPosition, Color.White);
            if (compareA1)
            {
                int a1frostabsorb = (a1.get_armor_value(Armor.Armor_Value.Padding) * 4) + (a1.get_armor_value(Armor.Armor_Value.Insulative) * 2);
                draw_difference(a1frostabsorb - frostAbsorb, ixtextPosition, frostAbsorb.ToString(), ref sBatch, true);
            }
            ixtextPosition.Y += linedif;

            //draw electric absorb
            sBatch.DrawString(std_textfont, elecAbsorb.ToString() + "%", ixtextPosition, Color.White);
            if (compareA1)
            {
                int a1elecabsorb = (a1.get_armor_value(Armor.Armor_Value.Insulative) * 4) + (a1.get_armor_value(Armor.Armor_Value.Padding) * 2);
                draw_difference(a1elecabsorb - elecAbsorb, ixtextPosition, elecAbsorb.ToString(), ref sBatch, true);
            }
            ixtextPosition.Y += linedif;

            //draw acid absorb
            sBatch.DrawString(std_textfont, acidAbsorb.ToString() + "%", ixtextPosition, Color.White);
            if (compareA1)
            {
                int a1acidabsorb = (a1.get_armor_value(Armor.Armor_Value.Insulative) * 4) + (a1.get_armor_value(Armor.Armor_Value.Ablative) * 2);
                draw_difference(a1acidabsorb - acidAbsorb, ixtextPosition, acidAbsorb.ToString(), ref sBatch, true);
            }
            ixtextPosition.Y += linedif;

            //draw integrity
            sBatch.DrawString(std_textfont, integrity.ToString(), ixtextPosition, Color.White);
            if (compareA1)
            {
                int diff = a1.get_max_integ() - ax.get_max_integ();
                draw_difference(diff, ixtextPosition, integrity.ToString(), ref sBatch, false);
            }
            ixtextPosition.Y += linedif;
        }

        public void draw_difference(int diff, Vector2 textPos, string measureString, ref SpriteBatch sBatch, bool addpercent)
        {
            string diffString;
            Color diffColor;
            Vector2 diffPos;

            diffString = "";
            diffColor = Color.Red;
            if (diff > 0)
            {
                diffColor = new Color(0, 255, 0);
                diffString = "+" + diff.ToString();
            }
            else
                diffString = diff.ToString();
            if (addpercent)
            {
                diffString += "%";
                measureString += "%";
            }

            diffPos = new Vector2(textPos.X, textPos.Y);
            diffPos.X += std_textfont.MeasureString(measureString).X + 10;
            sBatch.DrawString(std_textfont, diffString, diffPos, diffColor);
        }

        public void draw_icon_textures(ref SpriteBatch sBatch)
        {
            Color targetColor = Color.White;

            sBatch.Draw(i1text, i1rect, Color.White);
            if (!comparingArmor)
            {
                targetColor = Color.White;
                if (w2 == null)
                    targetColor = Color.Black;
                sBatch.Draw(i2text, i2rect, targetColor);
                targetColor = Color.White;
                if (w3 == null)
                    targetColor = Color.Black;
                sBatch.Draw(i3text, i3rect, targetColor);
            }
            else
            {
                if (a2 == null)
                    targetColor = Color.Black;
                sBatch.Draw(i2text, i2rect, targetColor);
            }
        }

        public void drawMe(ref SpriteBatch sBatch)
        {
            sBatch.Begin(SpriteSortMode.BackToFront, null);
            sBatch.Draw(blank_texture, my_size, my_dark_color);
            sBatch.End();

            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_all_text(ref sBatch);
            sBatch.End();

            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_icon_textures(ref sBatch);
            sBatch.End();

            sBatch.Begin(SpriteSortMode.BackToFront, null);
            draw_border_around_rectangle(ref sBatch, i1rect, 4, my_red_color);
            draw_border_around_rectangle(ref sBatch, i2rect, 4, my_red_color);
            if (!comparingArmor)
                draw_border_around_rectangle(ref sBatch, i3rect, 4, my_red_color);
            draw_border_around_rectangle(ref sBatch, my_size, 4, my_red_color);
            sBatch.End();
        }
    }
}
