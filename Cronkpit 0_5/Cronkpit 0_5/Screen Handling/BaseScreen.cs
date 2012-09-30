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


namespace Cronkpit_Csharp.Screen_Handling
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public abstract class BaseScreen : Microsoft.Xna.Framework.DrawableGameComponent
    {
        protected List<GameComponent> game_components = new List<GameComponent>();
        protected Game my_game;
        protected SpriteBatch sb;

        public List<GameComponent> getComponents()
        {
            return game_components;
        }

        public BaseScreen(Game game, ref SpriteBatch sBatch)
            : base(game)
        {
            // TODO: Construct any child components here
            my_game = game;
            sb = sBatch;
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

            base.Update(gameTime);
            foreach (GameComponent comp in game_components)
                if (comp.Enabled)
                    comp.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            foreach (GameComponent comp in game_components)
                if (comp is DrawableGameComponent && ((DrawableGameComponent)comp).Visible)
                    ((DrawableGameComponent)comp).Draw(gameTime);
        }

        public virtual void show()
        {
            this.Enabled = true;
            this.Visible = true;
            foreach (GameComponent comp in game_components)
            {
                comp.Enabled = true;
                if (comp is DrawableGameComponent)
                    ((DrawableGameComponent)comp).Visible = true;
            }
        }

        public virtual void hide()
        {
            this.Enabled = false;
            this.Enabled = false;

            foreach (GameComponent comp in game_components)
            {
                comp.Enabled = false;
                if (comp is DrawableGameComponent)
                    ((DrawableGameComponent)comp).Visible = false;
            }
        }
    }
}
