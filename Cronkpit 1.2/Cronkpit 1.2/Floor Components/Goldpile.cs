using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Cronkpit_1._2
{
    class Goldpile
    {
        private Texture2D my_Texture;
        private Vector2 my_Position;
        private ContentManager cont;
        private gridCoordinate my_grid_coord;

        public int my_quantity;

        public Goldpile(gridCoordinate sGridCoord, ContentManager sCont, int sQuan)
        {
            cont = sCont;
            my_grid_coord = sGridCoord;
            my_Position = new Vector2(sGridCoord.x * 32, sGridCoord.y * 32);

            if(sQuan < 20)
                my_Texture = cont.Load<Texture2D>("Entities/lowGold");
            else if(sQuan > 20 && sQuan < 30)
                my_Texture = cont.Load<Texture2D>("Entities/alilmoreGold");
            else if(sQuan > 30 && sQuan < 40)
                my_Texture = cont.Load<Texture2D>("Entities/someGold");
            else if(sQuan > 40 && sQuan < 45)
                my_Texture = cont.Load<Texture2D>("Entities/tonsoGold");
            else
                my_Texture = cont.Load<Texture2D>("Entities/time2getpaid");

            my_quantity = sQuan;
        }

        public gridCoordinate get_my_grid_C()
        {
            return my_grid_coord;
        }

        //don't call unless you've started the spritebatch!
        public void drawMe(ref SpriteBatch sb)
        {
            sb.Draw(my_Texture, my_Position, Color.White);
        }
    }
}
