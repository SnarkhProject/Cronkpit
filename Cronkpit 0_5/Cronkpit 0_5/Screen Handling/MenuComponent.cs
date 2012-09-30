using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace Cronkpit_Csharp.Screen_Handling
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class MenuComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        List<string> menuItems;
        public int selectedIndex;

        Color normal = Color.White;
        Color highlighted = Color.Red;

        KeyboardState oldKBstate;
        KeyboardState newKBstate;

        SpriteBatch sb;
        SpriteFont sf;

        Vector2 pos;
        float width = 0;
        float height = 0;

        public MenuComponent(Game game, ref SpriteBatch sBatch, SpriteFont sFont, List<string> mItems)
            : base(game)
        {
            // TODO: Construct any child components here
            selectedIndex = 0;
            menuItems = new List<string>(mItems);
            sf = sFont;
            sb = sBatch;
            measureMenu();
        }

        public void nextIndex(int nextInd)
        {
            selectedIndex = nextInd;
            if (selectedIndex >= menuItems.Count)
                selectedIndex = 0;
            if (selectedIndex < 0)
                selectedIndex = menuItems.Count - 1;
        }

        private void measureMenu()
        {
            height = 0;
            width = 0;

            foreach (string item in menuItems)
            {
                Vector2 size = sf.MeasureString(item);
                if (size.X > width)
                    width = size.X;
                height += sf.LineSpacing + 5;
            }

            pos = new Vector2((Game.Window.ClientBounds.Width - width) / 2,
                              (Game.Window.ClientBounds.Height - height) / 2);

        }

        private bool checkKeys(Keys theKey)
        {
            return newKBstate.IsKeyUp(theKey) && oldKBstate.IsKeyDown(theKey);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            newKBstate = Keyboard.GetState();

            if (checkKeys(Keys.Down))
                nextIndex(selectedIndex + 1);
            if (checkKeys(Keys.Up))
                nextIndex(selectedIndex - 1);

            base.Update(gameTime);

            oldKBstate = newKBstate;
        }

        public override void Draw(GameTime gameTime)
        {

            base.Draw(gameTime);
            Vector2 location = pos;
            Color tint;

            for (int i = 0; i < menuItems.Count; i++)
            {
                if (i == selectedIndex)
                    tint = highlighted;
                else
                    tint = normal;
                sb.DrawString(sf, menuItems[i], location, tint);
                location.Y += sf.LineSpacing + 5;
            }
        }
    }
}
