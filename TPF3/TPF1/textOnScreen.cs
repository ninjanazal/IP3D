using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPF1
{
    class textOnScreen
    {
        SpriteFont textFont;
        double lastSecond, gameTime;
        int fps;

        public textOnScreen (SpriteFont sF)
        {
            this.textFont = sF;
        }

        public void update(double seconds)
        {
            this.gameTime = seconds;
        }

        public void Draw(SpriteBatch sB)
        {
            sB.Begin();
            sB.DrawString(textFont, "time: " + gameTime, Vector2.Zero, Color.White);
            sB.End();
        }
    }
}
