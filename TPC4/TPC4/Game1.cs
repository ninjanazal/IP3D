using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TPC4
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // camara
        cameraCls cam;

        // plano
        float planeSize;
        Texture2D planeTexture;
        planeCls plane;

        // chuva
        rainCls rain;

        // debug
        SpriteFont font;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            // Define tamanho da viewport para 900*720

            graphics.PreferredBackBufferWidth = 900;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();

            Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

            cam = new cameraCls(GraphicsDevice);

            // plano
            planeSize = 700.0f;
            planeTexture = Content.Load<Texture2D>("stoneFloor");
            plane = new planeCls(GraphicsDevice, planeTexture, planeSize);

            // rain
            rain = new rainCls(GraphicsDevice, 800, new Vector3(0.0f,1000.0f,0.0f));

            font = Content.Load<SpriteFont>("font");

            base.Initialize();
        }

       
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

       
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            cam.Update(Keyboard.GetState(), Mouse.GetState(), GraphicsDevice);

            rain.Update(GraphicsDevice);

            // repoem a posicao do rato no centro do viewport
            Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

            base.Update(gameTime);
        }

       
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            // deseha o plano e a chuva
            plane.Draw(GraphicsDevice, cam.getView, cam.getProjection);
            rain.Draw(GraphicsDevice, cam.getView, cam.getProjection);

            // desenha texto para debug
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Particle Count: " + rain.getParticleCount, new Vector2(1.0f,0.0f), Color.White);
            spriteBatch.DrawString(font, "CamPos: " + cam.getPosition, new Vector2(1.0f, 12.0f), Color.White);
            spriteBatch.DrawString(font, "CamTarget: " + cam.getTarget, new Vector2(1.0f, 24.0f), Color.White);
            spriteBatch.End();

            // repoem os estados do grapfics device para o default
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            base.Draw(gameTime);
        }
    }
}
