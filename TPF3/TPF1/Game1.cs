using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace TPF1
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //iluminaçao
        lightControllerCls lightControl;

        // controlador de cameras
        int cameraSelected;
        cameraController cC;

        // terreno
        terrainCls terrain;
        Texture2D height, groundTexture;
        Vector2 terrainBaseSize;
        float groundSize;

        //tank
        Model tankModel;
        tankCls tank, tank2;
        // bala
        Model bulletModel;

        // dust
        Model dustParticleModel;

        // Eixos
        axis3DAcls axis;

        // Debug
        debugLines dB;
        debugTextCls dBText;

       
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {

            // Define tamanho da viewport para 1280*720

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            
            graphics.ApplyChanges();
            

            // define posicao do rato no centro da viewport
            Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

            // inicia eixos
            axis = new axis3DAcls(GraphicsDevice);



            // iluminaçao
            lightControl = new lightControllerCls();

            // inicia o terreno
            groundSize = 10000.0f;
            height = Content.Load<Texture2D>("heightMap");
            terrainBaseSize = new Vector2(height.Width, height.Height);
            groundTexture = Content.Load<Texture2D>("groundTexture");
            terrain = new terrainCls(GraphicsDevice, height, groundTexture, groundSize);



            // inicia o tank
            tankModel = Content.Load<Model>("tank");
            bulletModel = Content.Load<Model>("bullet");
            dustParticleModel = Content.Load<Model>("dustParticle");
            // jogador 1
            tank = new tankCls(tankModel, bulletModel, dustParticleModel, terrain.vertexSpacing, terrain.getVertex(), terrainBaseSize, terrain.getNormal(), groundSize, new Vector3(7000.0f, 0.0f, 7200.0f), 1);
            // jogador 2
            tank2 = new tankCls(tankModel, bulletModel, dustParticleModel, terrain.vertexSpacing, terrain.getVertex(), terrainBaseSize, terrain.getNormal(), groundSize, new Vector3(2800.0f, 0.0f, 2000.0f), 2);

            // inicia o controlados de camera
            cC = new cameraController(GraphicsDevice, terrainBaseSize, terrain.getVertex(), groundSize, terrain.vertexSpacing, tank.getPosition, tank.getDirection, tank2.getPosition, tank2.getDirection);
            cameraSelected = cC.getCamSelector;


            // --------------------------------------
            // DEBUG

            // debug
            dB = new debugLines(GraphicsDevice, terrain.getVertex(), terrain.getNormal());
            dBText = new debugTextCls(GraphicsDevice, Content.Load<SpriteFont>("font"));

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

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // update das cameras
            cC.Update(Keyboard.GetState(), Mouse.GetState(), GraphicsDevice, tank.getPosition, tank.getDirection, tank2.getPosition, tank2.getDirection);
            cameraSelected = cC.getCamSelector;

            // update tank
            // confirma se o tanque ainda tem vida
            if(tank.getTankHp>0)
            tank.Update(GraphicsDevice, Keyboard.GetState(), cameraSelected, tank2.getBSphere, gameTime);
            if(tank2.getTankHp>0)
            tank2.Update(GraphicsDevice, Keyboard.GetState(), cameraSelected, tank.getBSphere, gameTime);

            // aquando a morte a posiçao do tanque é alterada para fora do mapa
            if (tank.getTankHp <= 0)
                tank.getPosition = Vector3.Down * 2000f;

            if (tank2.getTankHp <= 0)
                tank2.getPosition = Vector3.Down * 2000f;
            
            // dano aos tanques
            tank.hit(tank2.giveDamage, 10);
            tank2.hit(tank.giveDamage, 10);
            tank.bulletHit(tank2.giveBulletDamage);
            tank2.bulletHit(tank.giveBulletDamage);

            // repoem o rato no centro da viewport
            Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);


            // --------------------------------------
            // DEBUG

            //debug update
            dB.Update(GraphicsDevice, Keyboard.GetState());
            dBText.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.SlateGray);
            // desenha eixos
            axis.Draw(GraphicsDevice, cC.getView(), cC.getProjection());

            // desenha o terreno
            terrain.Draw(GraphicsDevice, cC.getView(), cC.getProjection(), lightControl);

            // desenha o tanque
            tank.Draw(GraphicsDevice, cC.getView(), cC.getProjection(), lightControl);
            tank2.Draw(GraphicsDevice, cC.getView(), cC.getProjection(), lightControl);


            // --------------------------------------
            // DEBUG


            // desenha debug
            dB.Draw(GraphicsDevice, cC.getView(), cC.getProjection());
            dBText.Draw(gameTime, tank.getPosition, tank2.getPosition, tank.bulletCount, tank2.bulletCount, tank.getTankHp, tank2.getTankHp, tank.getReloadTime,tank2.getReloadTime,tank2.getAIState,tank2.getAIMode);
            


            // repoem os estados de render para que seja desenhado correctamente todos os elementos 3d

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;


            base.Draw(gameTime);
        }
    }
}
