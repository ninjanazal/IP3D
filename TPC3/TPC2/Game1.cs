using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TPC3
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //camera
        cameraCls camera;

        // iluminaçao
        lightCls luz;

        //prisma
        prismCls prisma;
        Texture2D prismaTexture, prismaTexture2;

        //plano
        planeCls plano;
        Texture2D planoTextura;
        float larguraPlano;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

            //Iluminaçao
            luz = new lightCls();

            //textura do plano e largura
            planoTextura = Content.Load<Texture2D>("pattern");
            larguraPlano = 100.0f;

            //textura do prisma
            prismaTexture = Content.Load<Texture2D>("texture2");
            prismaTexture2 = Content.Load<Texture2D>("wood");

            // inicia classe camera, plano
            camera = new cameraCls(GraphicsDevice);
            plano = new planeCls(GraphicsDevice, camera.getView, camera.getProjection, planoTextura, larguraPlano);
            prisma = new prismCls(GraphicsDevice, 30, 40.0f, 20.0f, camera.getView, camera.getProjection, prismaTexture, prismaTexture2);


            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

        }

        protected override void UnloadContent()
        {

        }

        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            //actualiza classes
            camera.Update(Keyboard.GetState(), Mouse.GetState(), GraphicsDevice);
            prisma.Update(Keyboard.GetState(),larguraPlano);

            // repoem a posicao do rato no centro do viewport
            Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            // TODO: Add your drawing code here
            plano.Draw(GraphicsDevice, camera.getView,luz);
            prisma.Draw(GraphicsDevice, camera.getView,luz);
            base.Draw(gameTime);
        }
    }
}
