using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Cronkpit_Csharp.Screen_Handling
{
    class StartMenu : BaseScreen
    {
        MenuComponent menucomponent;

        public StartMenu(Game game, ref SpriteBatch sBatch, SpriteFont sFont)
            : base(game, ref sBatch)
        {
            List<string> menuItems = new List<string>();
            menuItems.Add("Start");
            menuItems.Add("Exit");
            menucomponent = new MenuComponent(game, ref sBatch, sFont, menuItems);
            game_components.Add(menucomponent);
        }

        public int get_selected_index()
        {
            return menucomponent.selectedIndex;
        }

        public int next_selected_index(int nextIndex)
        {
            return menucomponent.selectedIndex = nextIndex;
        }
    }
}
