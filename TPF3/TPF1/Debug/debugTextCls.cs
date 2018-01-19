using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPF1
{
    class debugTextCls
    {
        SpriteBatch spriteBatch;
        SpriteFont font;
        int fpsCounter, fps;
        int lastSec;
        Vector2 viewportSize;
        
        public debugTextCls(GraphicsDevice device, SpriteFont sF)
        {
            this.spriteBatch = new SpriteBatch(device);
            this.fpsCounter = 0;
            this.font = sF;
            this.lastSec = 1;

            this.viewportSize = new Vector2(device.Viewport.Width, device.Viewport.Height);

        }

        public void Update(GameTime time)
        {
            if (time.TotalGameTime.Seconds == lastSec)
            {
                fps = fpsCounter;
                fpsCounter = 0;
                lastSec = time.TotalGameTime.Seconds + 1;
            }
            else
                fpsCounter++;
        }


        public void Draw(GameTime time, Vector3 tank1Pos, Vector3 tank2Pos, int tank1Bullet, int tank2Bullet, int tank1Hp, int tank2Hp, int tank1Reload, int tank2Reload, bool aiEnable, string aiState)
        {
            spriteBatch.Begin();

            spriteBatch.DrawString(font, " TIME: " + time.TotalGameTime.Minutes + " : " + time.TotalGameTime.Seconds, Vector2.Zero, Color.White);
            spriteBatch.DrawString(font, " FPS: " + fps, new Vector2(0.0f,1 * 13.0f), Color.White);
            spriteBatch.DrawString(font, " Tank1 position: " + tank1Pos, new Vector2(0.0f, 2 * 13.0f), Color.White);
            spriteBatch.DrawString(font, "    bullets: " + tank1Bullet + "  HP: " + tank1Hp + "%" + " Reload: " + tank1Reload, new Vector2(0.0f, 3 * 13.0f), Color.White);
            spriteBatch.DrawString(font, " Tank2 position: " + tank2Pos, new Vector2(0.0f, 4 * 13.0f), Color.White);
            spriteBatch.DrawString(font, "    bullets: " + tank2Bullet + "  HP: " + tank2Hp + "%" + " Reload: " + tank2Reload + " AI Enable: " + aiEnable, new Vector2(0.0f, 5 * 13.0f), Color.White);
            if (aiEnable)
                spriteBatch.DrawString(font, "AI Mode: " + aiState, new Vector2(0.0f, 6 * 13.0f), Color.White);

            spriteBatch.DrawString(font, "PageUp/PageDown: change camera height", new Vector2(0.0f, 10 * 13.0f), Color.White);
            spriteBatch.DrawString(font, "Press M to enable AI", new Vector2(0.0f, 11 * 13.0f), Color.White);
            spriteBatch.DrawString(font, "Press LShift to boost", new Vector2(0.0f, 12 * 13.0f), Color.White);

            spriteBatch.End();
        }
    }
}
