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
    class MenuScreen
    {
        List<string> menuItems;
        string titleItem;

        int selectedIndex;

        Color normal = Color.White;
        Color highlighted = Color.Red;

        SpriteFont sFont;
        SpriteFont tFont;

        Vector2 menu_position;
        Vector2 title_position;

        Rectangle client;

        float height = 0;
        float width = 0;

        float t_height = 0;
        float t_width = 0;

        public MenuScreen(List<string> mItems, string tItem, SpriteFont sf, SpriteFont tf, Rectangle cl)
        {
            menuItems = new List<string>(mItems);
            titleItem = tItem;
            sFont = sf;
            tFont = tf;
            client = cl;
            selectedIndex = 0;
            take_measurements();
        }

        private void take_measurements()
        {
            height = 0;
            width = 0;

            for (int i = 0; i < menuItems.Count; i++)
            {
                Vector2 size = sFont.MeasureString(menuItems[i]);
                if (size.X > width)
                    width = size.X;
                height += sFont.LineSpacing + 5;
            }

            menu_position = new Vector2((client.Width - width) / 2, (client.Height - height) / 2);

            t_height = 0;
            t_width = 0;

            Vector2 t_size = tFont.MeasureString(titleItem);
            if (t_size.X > t_width)
                t_width = t_size.X;
            t_height = tFont.LineSpacing + 300;

            title_position = new Vector2((client.Width - t_width) / 2, (client.Height - t_height) / 2);
        }

        public void selected_index_up()
        {
            selectedIndex++;
            if (selectedIndex >= menuItems.Count)
                selectedIndex = 0;
        }

        public void selected_index_down()
        {
            selectedIndex--;
            if(selectedIndex < 0)
                selectedIndex = menuItems.Count - 1;
        }

        public int selected_index()
        {
            return selectedIndex;
        }

        public void drawMe(ref SpriteBatch sBatch)
        {
            Vector2 m_loc = menu_position;
            Vector2 t_loc = title_position;
            Color tint;
            //Don't call this unless you've called spritebatch.begin already!
            //I mean seriously there's no begin call in here so don't ing do it.
            sBatch.DrawString(tFont, titleItem, t_loc, Color.White);
            for (int i = 0; i < menuItems.Count; i++)
            {
                if (i == selectedIndex)
                    tint = highlighted;
                else
                    tint = normal;
                sBatch.DrawString(sFont, menuItems[i], m_loc, tint);
                m_loc.Y += sFont.LineSpacing + 5;
            }
        }
    }
}
