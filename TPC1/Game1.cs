using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TPC1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // matrix e effect geral
        BasicEffect effect;
        Matrix worldMatrix;
        // Camera
        float aspectRatio;

        //piramide
        int numSides;
        float height;
        clsPyramid pyramid;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            ///inicializar matrix e effect geral
            worldMatrix = Matrix.Identity;
            effect = new BasicEffect(GraphicsDevice);
            aspectRatio = (float)GraphicsDevice.Viewport.Width / GraphicsDevice.Viewport.Height;
            effect.View = Matrix.CreateLookAt(new Vector3(1.0f, 2.0f, 3.0f), new Vector3(0.0f,0.6f,0.0f), Vector3.Up);
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 20.0f);
            effect.LightingEnabled = false;
            effect.VertexColorEnabled = true;


            //inicializar piramide
            numSides = 7;
            height = 1.5f;

            //numero de lados tem se ser entre 3 e 10
            if (numSides > 10)
                numSides = 10;
            else if (numSides < 3)
                numSides = 3;
            pyramid = new clsPyramid(GraphicsDevice, effect, worldMatrix, numSides, height);
            

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

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            // roda a piramide ao usar as setas direita e esquerda
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                pyramid.Rotate(0.02f);
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                pyramid.Rotate(-0.02f);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            // TODO: Add your drawing code here

            spriteBatch.Begin();
            spriteBatch.DrawString(Content.Load<SpriteFont>("font"), "Numero de lados: " + numSides, new Vector2(0.5f, 0.5f), Color.Black);
            spriteBatch.End();

            pyramid.Draw(GraphicsDevice);


            base.Draw(gameTime);
        }
    }
}
