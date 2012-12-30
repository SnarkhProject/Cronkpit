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
    class Projectile
    {
        public enum projectile_type { Arrow, Flamebolt, Frostbolt, Javelin };
        Texture2D my_texture;
        Rectangle my_rectangle;
        projectile_type my_type;
        gridCoordinate my_start_coordinate;
        gridCoordinate my_end_coordinate;
        Vector2 my_end_position;
        Vector2 my_current_position;

        public Projectile(gridCoordinate start_gCoord, gridCoordinate end_gCoord, projectile_type myType, ref ContentManager cmanage)
        {
            my_type = myType;
            int offset = 0;

            switch (my_type)
            {
                case projectile_type.Arrow:
                    my_texture = cmanage.Load<Texture2D>("Projectiles/arrow");
                    break;
                case projectile_type.Flamebolt:
                    my_texture = cmanage.Load<Texture2D>("Projectiles/flamebolt");
                    break;
                case projectile_type.Frostbolt:
                    my_texture = cmanage.Load<Texture2D>("Projectiles/frostbolt");
                    break;
                case projectile_type.Javelin:
                    my_texture = cmanage.Load<Texture2D>("Projectiles/javelin");
                    break;
            }

            my_start_coordinate = new gridCoordinate(start_gCoord);
            my_end_coordinate = new gridCoordinate(end_gCoord);
            my_current_position = new Vector2((my_start_coordinate.x * 32)+offset, (my_start_coordinate.y * 32)+offset);
            my_end_position = new Vector2((my_end_coordinate.x * 32)+offset, (my_end_coordinate.y * 32)+offset);

            my_rectangle = new Rectangle((int)my_current_position.X, (int)my_current_position.Y, 32, 32);
        }

        public Rectangle my_rect()
        {
            return my_rectangle;
        }

        public void update(float delta_time)
        {
            Vector2 direction = my_end_position - my_current_position;
            direction.Normalize();

            my_current_position.X += (direction.X * delta_time)*240;
            my_current_position.Y += (direction.Y * delta_time)*240;

            my_rectangle.X = (int)my_current_position.X;
            my_rectangle.Y = (int)my_current_position.Y;
        }

        public gridCoordinate get_my_end_coord()
        {
            return my_end_coordinate;
        }

        public void drawMe(ref SpriteBatch sb)
        {
            float angle = (float)Math.Atan2(my_current_position.Y - my_end_position.Y, my_current_position.X - my_end_position.X);

            sb.Draw(my_texture, new Vector2(my_rectangle.X + 16, my_rectangle.Y + 16), null, Color.White, angle, new Vector2(16, 16), 1, SpriteEffects.None, 0f);
            //sb.Draw(my_texture, my_rectangle, Color.White);
        }
    }
}
