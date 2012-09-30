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
    public class MainScreen : BaseScreen
    {
        KeyboardState oldState;
        Camera cam;
        Floor c_floor;
        Player c_player;
        ContentManager c_manager;

        SpriteFont sfont_thesecond;
        bool bad_turn;
        bool victory_condition;

        public MainScreen(Game game, Floor fl, Player pl, Camera ca, ContentManager cm, ref SpriteBatch sBatch)
            : base(game, ref sBatch)
        {
            // TODO: Construct any child components here
            c_floor = fl;
            c_player = pl;
            cam = ca;
            sb = sBatch;
            c_manager = cm;

            //Game stuff without constructors
            bad_turn = false;
            victory_condition = false;

            //Dumb shit that I'm going to put in later.
            sfont_thesecond = c_manager.Load<SpriteFont>("Fonts/sfont");
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
            updateInput();
            if (c_player.is_spot_exit(c_floor))
                victory_condition = true;

            if (bad_turn)
            {
                c_floor.update_dungeon_floor(c_player);
                bad_turn = false;
            }

            cam.Pos = c_player.get_my_Position();

            base.Update(gameTime);
        }


        private void updateInput()
        {
            KeyboardState newState = Keyboard.GetState();
            //Down Left first
            if (newState.IsKeyDown(Keys.NumPad1))
            {
                // If not down last update, key has just been pressed.
                if (!oldState.IsKeyDown(Keys.NumPad1))
                {
                    if (c_player.is_alive() && !victory_condition)
                    {
                        c_player.move("downleft", c_floor);
                        bad_turn = true;
                    }
                }
            }
            else if (oldState.IsKeyDown(Keys.NumPad2))
            {
                // Key was down last update, but not down now, so
                // it has just been released. Do nothing.
            }

            // Is the DOWN key down?
            if (newState.IsKeyDown(Keys.NumPad2))
            {
                // If not down last update, key has just been pressed.
                if (!oldState.IsKeyDown(Keys.NumPad2))
                {
                    if (c_player.is_alive() && !victory_condition)
                    {
                        c_player.move("down", c_floor);
                        bad_turn = true;
                    }
                }
            }
            else if (oldState.IsKeyDown(Keys.NumPad2))
            {
                // Key was down last update, but not down now, so
                // it has just been released. Do nothing.
            }

            // Is the DOWN RIGHT key down?
            if (newState.IsKeyDown(Keys.NumPad3))
            {
                // If not down last update, key has just been pressed.
                if (!oldState.IsKeyDown(Keys.NumPad3))
                {
                    if (c_player.is_alive() && !victory_condition)
                    {
                        c_player.move("downright", c_floor);
                        bad_turn = true;
                    }
                }
            }
            else if (oldState.IsKeyDown(Keys.NumPad3))
            {
                // Key was down last update, but not down now, so
                // it has just been released. Do nothing.
            }

            // Is the UP key down?
            if (newState.IsKeyDown(Keys.NumPad8))
            {
                // If not down last update, key has just been pressed.
                if (!oldState.IsKeyDown(Keys.NumPad8))
                {
                    if (c_player.is_alive() && !victory_condition)
                    {
                        c_player.move("up", c_floor);
                        bad_turn = true;
                    }
                }
            }
            else if (oldState.IsKeyDown(Keys.NumPad8))
            {
                // Key was down last update, but not down now, so
                // it has just been released. Do nothing.
            }

            if (newState.IsKeyDown(Keys.NumPad7))
            {
                // If not down last update, key has just been pressed.
                if (!oldState.IsKeyDown(Keys.NumPad7))
                {
                    if (c_player.is_alive() && !victory_condition)
                    {
                        c_player.move("upleft", c_floor);
                        bad_turn = true;
                    }
                }
            }
            else if (oldState.IsKeyDown(Keys.NumPad7))
            {
                // Key was down last update, but not down now, so
                // it has just been released. Do nothing.
            }

            if (newState.IsKeyDown(Keys.NumPad9))
            {
                // If not down last update, key has just been pressed.
                if (!oldState.IsKeyDown(Keys.NumPad9))
                {
                    if (c_player.is_alive() && !victory_condition)
                    {
                        c_player.move("upright", c_floor);
                        bad_turn = true;
                    }
                }
            }
            else if (oldState.IsKeyDown(Keys.NumPad9))
            {
                // Key was down last update, but not down now, so
                // it has just been released. Do nothing.
            }

            // Is the LEFT key down?
            if (newState.IsKeyDown(Keys.NumPad4))
            {
                // If not down last update, key has just been pressed.
                if (!oldState.IsKeyDown(Keys.NumPad4))
                {
                    if (c_player.is_alive() && !victory_condition)
                    {
                        c_player.move("left", c_floor);
                        bad_turn = true;
                    }
                }
            }

            else if (oldState.IsKeyDown(Keys.NumPad4))
            {
                // Key was down last update, but not down now, so
                // it has just been released. Do nothing.
            }

            // Is the RIGHT key down?
            if (newState.IsKeyDown(Keys.NumPad6))
            {
                // If not down last update, key has just been pressed.
                if (!oldState.IsKeyDown(Keys.NumPad6))
                {
                    if (c_player.is_alive() && !victory_condition)
                    {
                        c_player.move("right", c_floor);
                        bad_turn = true;
                    }
                }
            }
            else if (oldState.IsKeyDown(Keys.NumPad6))
            {
                // Key was down last update, but not down now, so
                // it has just been released. Do nothing.
            }

            // Update saved state.
            oldState = newState;
        }

        public override void Draw(GameTime gameTime)
        {
            if (Enabled)
            {
                sb.End();

                sb.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, cam.viewMatrix);
                c_floor.drawBackground(ref sb);
                sb.End();

                sb.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, cam.viewMatrix);
                c_floor.drawEntities(ref sb);
                sb.End();

                sb.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, cam.viewMatrix);
                c_floor.drawEnemies(ref sb);
                sb.End();

                sb.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, cam.viewMatrix);
                c_player.drawMe(ref sb);
                sb.End();

                if (victory_condition)
                {
                    Vector2 vec = new Vector2(c_player.get_my_Position().X - 60, c_player.get_my_Position().Y - 60);
                    sb.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, cam.viewMatrix);
                    sb.DrawString(sfont_thesecond, "You won!", vec, Color.White);
                    sb.End();
                }

                sb.Begin();
            }
            base.Draw(gameTime);
        }
    }
}
