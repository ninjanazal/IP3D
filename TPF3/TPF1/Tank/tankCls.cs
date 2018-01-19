using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TPF1
{
    class tankCls
    {

        // modelo
        Model tankModel;

        // Balas
        Model bulletModel;
        List<bulletCls> bullets;
        bool reloading;
        int reloadingTime;

        // Bones e transformaçoes
        ModelBone turretBone, canonBone, rightFrontWheelBone, leftFrontWheelBone, rightBackWheelBone, leftBackWheelBone, hatchBone;
        Matrix turretTransform, canonTransform, rightFrontWheelTransform, leftFrontWheelTransform, rightBackWheelTransform, leftBackWheelTransform, hatchTransform;

        // CTRL do tanque
        float turrentAngle, canonAngle;
        float tankScale;
        Vector3 tankPosition;
        Matrix translation, rotation, scale;
        Matrix[] bonesTransf;
        float tankvelocity, wheelRotationVelocity, canonVelocity, turretVelocity;
        float tankYaw;
        Vector3 tankBaseDirection, tankRotation;
        float rFWheelAngle, rBWheelAngle, lFWheelAngle, lBWheelAngle;

        //tamanho do terrenho
        float groundScale;
        Vector2 terrainSize;
        Vector3[] terrainVertex, terrainNormal;
        Vector2 spacing;

        // pontos proximos
        int minVertex, maxVertexX, maxVertexZ, maxVertex;
        Vector3 minVector;

        // jogabilidade
        int NumJogadores, hp;
        KeyboardState lastKS;
        float tankBoost;
        bool hitOnBoost, hitOnBullet;


        //Colisao
        collisionTestCls cTest;
        BoundingSphere bSpere;
        float oldTankYaw;
        Vector3 oldTankPosition;
        float bSphereRadius;

        // particulas
        dustSystem dust;
        bool isMoving;
        int reloadTimer;

        // Boids
        BoidCls boid;
        bool aiEnable;

        public tankCls(Model tankM, Model bulletM, Model dustParticleModel, Vector2 spacing, Vector3[] vertexP, Vector2 terrainSize, Vector3[] vertexN, float gSize, Vector3 tankPos, int pNum)
        {
            // recebe modelo do tanque, define escala, posiçao inicial e rotaçao
            this.tankModel = tankM;
            tankScale = 0.3f;
            tankYaw = 0.0f;
            tankPosition = tankPos;
            tankBaseDirection = Vector3.UnitZ;


            // guarda informaçao em variaveis locais
            this.groundScale = gSize;
            this.terrainSize = terrainSize;
            this.spacing = spacing;
            this.terrainVertex = vertexP;
            this.tankvelocity = 5.5f;
            this.terrainNormal = vertexN;
            this.tankRotation = tankBaseDirection;
            this.wheelRotationVelocity = MathHelper.ToRadians(tankvelocity * 1.1f);
            this.turretVelocity = MathHelper.ToRadians(0.5f);
            this.canonVelocity = MathHelper.ToRadians(0.5f);

            //jogadores
            this.NumJogadores = pNum;
            this.tankBoost = 0.0f;
            aiEnable = false;

            // vida
            hp = 100;

            // Bala
            this.bulletModel = bulletM;
            bullets = new List<bulletCls>();
            reloading = false;
            reloadTimer = 0;

            // calcula a altura para a posiçao de acordo com o terreno
            tankPosition.Y = getHeight();
            translation = Matrix.CreateTranslation(tankPosition);
            scale = Matrix.CreateScale(tankScale);
            rotation = Matrix.CreateRotationX(tankYaw) * getTankNormal();

            // guarda os bones do tank nas variaveis 
            turretBone = tankModel.Bones["turret_geo"];
            canonBone = tankModel.Bones["canon_geo"];
            rightFrontWheelBone = tankModel.Bones["r_front_wheel_geo"];
            leftFrontWheelBone = tankModel.Bones["l_front_wheel_geo"];
            rightBackWheelBone = tankModel.Bones["r_back_wheel_geo"];
            leftBackWheelBone = tankModel.Bones["l_back_wheel_geo"];
            hatchBone = tankModel.Bones["hatch_geo"];

            // angulo da torre e canhao
            turrentAngle = 0.0f;
            canonAngle = 0.0f;

            turretTransform = turretBone.Transform;
            canonTransform = canonBone.Transform;
            rightFrontWheelTransform = rightFrontWheelBone.Transform;
            leftFrontWheelTransform = leftFrontWheelBone.Transform;
            rightBackWheelTransform = rightBackWheelBone.Transform;
            leftBackWheelTransform = leftBackWheelBone.Transform;
            hatchTransform = hatchBone.Transform;

            // cria array de bones
            bonesTransf = new Matrix[tankModel.Bones.Count];

            // Colisao
            bSphereRadius = 100.0f;
            cTest = new collisionTestCls();
            bSpere = new BoundingSphere(tankPosition, bSphereRadius);

            // particulas
            dust = new dustSystem(dustParticleModel);
            isMoving = false;

            //boid
            boid = new BoidCls();

        }


        public void Update(GraphicsDevice device, KeyboardState kS, int cameraSelected, BoundingSphere bTank2, GameTime time)
        {
            // guarda valores da antiga posiçao do tanque caso ocorra colisao retornar ao estado correcto
            oldTankYaw = tankYaw;
            oldTankPosition = tankPosition;

            // controlo de dano por parte de colisao com impulso ou bala
            hitOnBoost = false;
            hitOnBullet = false;

            isMoving = false;

            if (tankBoost > 0)
            {
                tankBoost -= 0.25f;
                wheelRotationVelocity = MathHelper.ToRadians(tankvelocity * 4f);
            }
            else
                wheelRotationVelocity = MathHelper.ToRadians(tankvelocity * 1.1f);


            // ------------------------------------------------------------ Jogador 2
            if (NumJogadores == 1)
            {
                if (kS.IsKeyDown(Keys.A) && cameraSelected == 3)
                {
                    rFWheelAngle += wheelRotationVelocity / 2;
                    rBWheelAngle += wheelRotationVelocity / 2;
                    lFWheelAngle -= wheelRotationVelocity / 2;
                    lBWheelAngle -= wheelRotationVelocity / 2;

                    tankYaw += MathHelper.ToRadians(0.5f);
                }
                if (kS.IsKeyDown(Keys.D) && cameraSelected == 3)
                {
                    rFWheelAngle -= wheelRotationVelocity / 2;
                    rBWheelAngle -= wheelRotationVelocity / 2;
                    lFWheelAngle += wheelRotationVelocity / 2;
                    lBWheelAngle += wheelRotationVelocity / 2;
                    tankYaw -= MathHelper.ToRadians(0.5f);
                }

                // calcula o vector direcçao do tanque apos a rotaçao
                tankRotation = Vector3.Transform(tankBaseDirection, Matrix.CreateFromYawPitchRoll(tankYaw, 0.0f, 0.0f));

                if (kS.IsKeyDown(Keys.LeftShift) && lastKS.IsKeyUp(Keys.LeftShift) && cameraSelected == 3 && tankBoost == 0.0f)
                    tankBoost = 15.0f;

                // impede o deslocamento a quando o utilizador roda o tanque
                if (kS.IsKeyDown(Keys.W) && !kS.IsKeyDown(Keys.A) && !kS.IsKeyDown(Keys.D) && cameraSelected == 3)
                {
                    // roda as rodas para a frente
                    rFWheelAngle += wheelRotationVelocity;
                    rBWheelAngle += wheelRotationVelocity;
                    lFWheelAngle += wheelRotationVelocity;
                    lBWheelAngle += wheelRotationVelocity;

                    tankPosition += tankRotation * (tankvelocity + tankBoost);

                    isMoving = true;
                }
                if (kS.IsKeyDown(Keys.S) && !kS.IsKeyDown(Keys.A) && !kS.IsKeyDown(Keys.D) && cameraSelected == 3)
                {
                    //roda as rodas para tras
                    rFWheelAngle -= wheelRotationVelocity;
                    rBWheelAngle -= wheelRotationVelocity;
                    lFWheelAngle -= wheelRotationVelocity;
                    lBWheelAngle -= wheelRotationVelocity;

                    tankPosition -= tankRotation * (tankvelocity + tankBoost);
                    isMoving = true;
                }
                // roda a torre e o canhao
                if (cameraSelected == 3 && kS.IsKeyDown(Keys.Left))
                    turrentAngle += turretVelocity;
                if (cameraSelected == 3 && kS.IsKeyDown(Keys.Right))
                    turrentAngle -= turretVelocity;
                if (cameraSelected == 3 && kS.IsKeyDown(Keys.Up) && canonAngle > -MathHelper.ToRadians(60.0f))
                    canonAngle -= canonVelocity;
                if (cameraSelected == 3 && kS.IsKeyDown(Keys.Down) && canonAngle < 0.0f)
                    canonAngle += canonVelocity;


            }
            // ------------------------------------------------------------ Jogador 2
            if (NumJogadores == 2 && !aiEnable)
            {
                if (kS.IsKeyDown(Keys.J) && cameraSelected == 4)
                {
                    rFWheelAngle += wheelRotationVelocity / 2;
                    rBWheelAngle += wheelRotationVelocity / 2;
                    lFWheelAngle -= wheelRotationVelocity / 2;
                    lBWheelAngle -= wheelRotationVelocity / 2;

                    tankYaw += MathHelper.ToRadians(0.5f);
                }
                if (kS.IsKeyDown(Keys.L) && cameraSelected == 4)
                {
                    rFWheelAngle -= wheelRotationVelocity / 2;
                    rBWheelAngle -= wheelRotationVelocity / 2;
                    lFWheelAngle += wheelRotationVelocity / 2;
                    lBWheelAngle += wheelRotationVelocity / 2;
                    tankYaw -= MathHelper.ToRadians(0.5f);
                }

                // calcula o vector direcçao do tanque apos a rotaçao
                tankRotation = Vector3.Transform(tankBaseDirection, Matrix.CreateFromYawPitchRoll(tankYaw, 0.0f, 0.0f));

                if (kS.IsKeyDown(Keys.LeftShift) && cameraSelected == 4 && tankBoost == 0.0f)
                    tankBoost = 15.0f;


                // impede o deslocamento a quando o utilizador roda o tanque
                if (kS.IsKeyDown(Keys.I) && !kS.IsKeyDown(Keys.J) && !kS.IsKeyDown(Keys.L) && cameraSelected == 4)
                {
                    // roda as rodas para a frente
                    rFWheelAngle += wheelRotationVelocity;
                    rBWheelAngle += wheelRotationVelocity;
                    lFWheelAngle += wheelRotationVelocity;
                    lBWheelAngle += wheelRotationVelocity;

                    tankPosition += tankRotation * (tankvelocity + tankBoost);

                    isMoving = true;
                }
                if (kS.IsKeyDown(Keys.K) && !kS.IsKeyDown(Keys.J) && !kS.IsKeyDown(Keys.L) && cameraSelected == 4)
                {
                    //roda as rodas para tras
                    rFWheelAngle -= wheelRotationVelocity;
                    rBWheelAngle -= wheelRotationVelocity;
                    lFWheelAngle -= wheelRotationVelocity;
                    lBWheelAngle -= wheelRotationVelocity;

                    tankPosition -= tankRotation * (tankvelocity + tankBoost);
                    isMoving = true;
                }
                if (cameraSelected == 4 && kS.IsKeyDown(Keys.Left))
                    turrentAngle += turretVelocity;
                if (cameraSelected == 4 && kS.IsKeyDown(Keys.Right))
                    turrentAngle -= turretVelocity;
                if (cameraSelected == 4 && kS.IsKeyDown(Keys.Up) && canonAngle > -MathHelper.ToRadians(60.0f))
                    canonAngle -= canonVelocity;
                if (cameraSelected == 4 && (kS.IsKeyDown(Keys.Down) && canonAngle < 0.0f))
                    canonAngle += canonVelocity;
            }
            // ------------------------------------------------------------ AI Boids
            if (kS.IsKeyDown(Keys.M) && lastKS.IsKeyUp(Keys.M) && NumJogadores == 2)
                if (aiEnable)
                    aiEnable = false;
                else
                    aiEnable = true;

            if (NumJogadores == 2 && aiEnable)
            {
                boid.Update(tankPosition, bTank2.Center, translation * rotation, tankYaw, hp, (int)time.TotalGameTime.TotalSeconds);
                if (boid.getMoveState == "right")
                {
                    rFWheelAngle += wheelRotationVelocity / 4;
                    rBWheelAngle += wheelRotationVelocity / 4;
                    lFWheelAngle -= wheelRotationVelocity / 4;
                    lBWheelAngle -= wheelRotationVelocity / 4;
                }
                else if (boid.getMoveState == "left")
                {
                    rFWheelAngle -= wheelRotationVelocity / 4;
                    rBWheelAngle -= wheelRotationVelocity / 4;
                    lFWheelAngle += wheelRotationVelocity / 4;
                    lBWheelAngle += wheelRotationVelocity / 4;
                }
                else if (boid.getMoveState == "move")
                {
                    rFWheelAngle += wheelRotationVelocity / 2;
                    rBWheelAngle += wheelRotationVelocity / 2;
                    lFWheelAngle += wheelRotationVelocity / 2;
                    lBWheelAngle += wheelRotationVelocity / 2;
                    isMoving = true;
                }
                tankYaw = boid.getCalculatedYaw;
                tankPosition = boid.getCalculatedPosition;
            }


            // impede que o tanque abandone o terreno
            if (tankPosition.X < 0.0f)
                tankPosition.X = 0.0f;
            if (tankPosition.X > groundScale - 100.0f)
                tankPosition.X = groundScale - 100.0f;
            if (tankPosition.Z < 0.0f)
                tankPosition.Z = 0.0f;
            if (tankPosition.Z > groundScale - 100.0f)
                tankPosition.Z = groundScale - 100.0f;


            // define a altura do tanque de acordo com a altura do terreno
            tankPosition.Y = getHeight();

            // aplica ás matrix de translaçao e rotaçao do tanque atraves da deslocaçao usando as teclas wasd
            translation = Matrix.CreateTranslation(tankPosition);
            rotation = Matrix.CreateRotationY(tankYaw) * getTankNormal();

            // colisao
            bSpere.Center = tankPosition;

            if (cTest.testCollisionBullet(bSpere, bTank2))
            {
                if (tankBoost > 5.0f)
                {
                    hitOnBoost = true;
                    hit(hitOnBoost, 10);
                    tankBoost = 0.0f;
                }

                tankPosition = oldTankPosition;
                tankYaw = oldTankYaw; ;

                // define a altura do tanque de acordo com a altura do terreno
                tankPosition.Y = getHeight();

                // aplica ás matrix de translaçao e rotaçao do tanque atraves da deslocaçao usando as teclas wasd
                translation = Matrix.CreateTranslation(tankPosition);
                rotation = Matrix.CreateRotationY(tankYaw) * getTankNormal();

                bSpere.Center = tankPosition;
            }

            // tempo de recarregar o canhao
            if (time.TotalGameTime.TotalSeconds > reloadTimer)
            {
                reloadingTime = 0;
                reloading = false;
            }
            else
                reloadingTime = reloadTimer - (int)time.TotalGameTime.TotalSeconds;

            // sempre que o jogador 1 carregar no espaço é disparado uma nova bala
            if (NumJogadores == 1 && cameraSelected == 3 && kS.IsKeyDown(Keys.Space) && lastKS.IsKeyUp(Keys.Space) && reloading == false)
            {
                bullets.Add(new bulletCls(bulletModel, tankPosition, getTankNormal(), (Matrix.CreateRotationY(turrentAngle + tankYaw) * getTankNormal()) * turretTransform,
                    tankYaw, canonAngle, turrentAngle, time));
                reloading = true;
                reloadTimer = (int)time.TotalGameTime.TotalSeconds + 6;
            }

            // sempre que o jogador 2 carregar no espaço é disparado uma nova bala
            if (NumJogadores == 2 && cameraSelected == 4 && kS.IsKeyDown(Keys.Space) && lastKS.IsKeyUp(Keys.Space) && reloading == false && !aiEnable)
            {
                bullets.Add(new bulletCls(bulletModel, tankPosition, getTankNormal(), (Matrix.CreateRotationY(turrentAngle + tankYaw) * getTankNormal()) * turretTransform,
                    tankYaw, canonAngle, turrentAngle, time));
                reloading = true;
                reloadTimer = (int)time.TotalGameTime.TotalSeconds + 6;
            }
            // atualiza as balas disparadas
            foreach (bulletCls bullet in bullets)
            {
                bullet.Update(time, spacing, terrainVertex, terrainSize, groundScale);
                if (cTest.testCollisionBullet(bSpere, bullet.getBSphere))
                {
                    bulletHit(true);
                    bullet.getDethState = true;
                }
                if (cTest.testCollisionBullet(bTank2, bullet.getBSphere))
                {
                    hitOnBullet = true;
                    bullet.getDethState = true;
                }
            }
            bullets.RemoveAll(s => s.getDethState == true);


            // actualiza o sistema de particulas
            dust.Update(isMoving, tankPosition, rotation * translation, (int)time.TotalGameTime.TotalSeconds);


            // impede que fique a carregar no espaço para disparar
            lastKS = kS;
        }


        public void Draw(GraphicsDevice device, Matrix view, Matrix projection, lightControllerCls lightCtrl)
        {
            translation = Matrix.CreateTranslation(tankPosition);
            bSpere.Center = tankPosition;

            // aplica tranformaçoes aos bones do modelo 
            // transformaçao ao bone root
            tankModel.Root.Transform = scale * rotation * translation;

            // transformaçao das rodas
            rightFrontWheelBone.Transform = Matrix.CreateRotationX(rFWheelAngle) * rightFrontWheelTransform;
            rightBackWheelBone.Transform = Matrix.CreateRotationX(rBWheelAngle) * rightBackWheelTransform;

            leftFrontWheelBone.Transform = Matrix.CreateRotationX(lFWheelAngle) * leftFrontWheelTransform;
            leftBackWheelBone.Transform = Matrix.CreateRotationX(lBWheelAngle) * leftBackWheelTransform;

            // transformaçao do canhao e torre
            turretBone.Transform = Matrix.CreateRotationY(turrentAngle) * turretTransform;
            canonBone.Transform = Matrix.CreateRotationX(canonAngle) * canonTransform;

            // aplica transformaçoes aos bones
            tankModel.CopyAbsoluteBoneTransformsTo(bonesTransf);
            foreach (ModelMesh mesh in tankModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {

                    effect.World = bonesTransf[mesh.ParentBone.Index];
                    effect.View = view;
                    effect.Projection = projection;


                    // iluminaçao
                    effect.PreferPerPixelLighting = true;
                    effect.LightingEnabled = true;
                    // luz emissora
                    effect.EmissiveColor = lightCtrl.getEmissive;

                    // luz ambiente
                    effect.AmbientLightColor = lightCtrl.getAColor;
                    effect.DiffuseColor = lightCtrl.getADifuse;
                    effect.SpecularColor = lightCtrl.getASpecColor;
                    effect.SpecularPower = lightCtrl.getASpecularPower;

                    // luz direcional
                    effect.DirectionalLight0.Enabled = true;
                    effect.DirectionalLight0.Direction = lightCtrl.getDDirection;
                    effect.DirectionalLight0.DiffuseColor = lightCtrl.getDDifuse;
                    effect.DirectionalLight0.SpecularColor = lightCtrl.getDSpecular;

                }
                // desenha as meshes do tanque
                mesh.Draw();
            }
            foreach (bulletCls bullet in bullets)
                bullet.Draw(device, view, projection, lightCtrl);

            dust.Draw(device, view, projection, lightCtrl);
            device.BlendState = BlendState.Opaque;

        }

        private float getHeight()
        {
            // determina a posicao do vertice menor mais proximo da camera
            // reduz a escala para 128, retem o valor inteiro e retorna os valores x e z do vertice
            minVector.X = (int)(tankPosition.X / spacing.X) * spacing.X;
            minVector.Z = (int)(tankPosition.Z / spacing.Y) * spacing.Y;

            // procura no array de vertices qual o vertice com os valores determindos anteriormente
            for (int i = 0; i < terrainVertex.Length; i++)
                if (terrainVertex[i].X == minVector.X && terrainVertex[i].Z == minVector.Z)
                {
                    minVertex = i;
                    break;
                }
            // calcula o valor dos indices dos vertices em volta da camera
            maxVertexX = minVertex + (int)terrainSize.X;
            maxVertexZ = minVertex + 1;
            maxVertex = maxVertexX + 1;

            // calcula a infuencia dos vertices em x anteriores á camera
            float pAX = 1.0f - ((tankPosition.X - terrainVertex[minVertex].X) / spacing.X);
            float pXB = 1.0f - ((terrainVertex[maxVertexX].X - tankPosition.X) / spacing.X);

            // calcula a media pesada da altura em x anterior á posicao da camera
            float height1 = (terrainVertex[minVertex].Y * pAX) + (terrainVertex[maxVertexX].Y * pXB);

            // calcula a infuencia dos vertices em x  posteriores á camera
            float pCX = 1.0f - (tankPosition.X - terrainVertex[maxVertexZ].X) / spacing.X;
            float pxD = 1.0f - ((terrainVertex[maxVertex].X - tankPosition.X) / spacing.X);

            // calcula a media pesada da altura em x posterior á posicao da camera
            float height2 = (terrainVertex[maxVertexZ].Y * pCX) + (terrainVertex[maxVertex].Y * pxD);

            // calcula a infuencia das anteriores altura em z
            float pH1X = (tankPosition.Z - terrainVertex[minVertex].Z) / spacing.Y;
            float pxH2 = (terrainVertex[maxVertexZ].Z - tankPosition.Z) / spacing.Y;

            // calcula a media pesada da altura em z na posicao da camera
            float h = ((height1 * pxH2) + (height2 * pH1X));
            return h;
        }

        private Matrix getTankNormal()
        {
            // determina a posicao do vertice menor mais proximo da camera
            // reduz a escala para 128, retem o valor inteiro e retorna os valores x e z do vertice
            minVector.X = (int)(tankPosition.X / spacing.X) * spacing.X;
            minVector.Z = (int)(tankPosition.Z / spacing.Y) * spacing.Y;

            // procura no array de vertices qual o vertice com os valores determindos anteriormente
            for (int i = 0; i < terrainVertex.Length; i++)
                if (terrainVertex[i].X == minVector.X && terrainVertex[i].Z == minVector.Z)
                {
                    minVertex = i;
                    break;
                }
            // calcula o valor dos indices dos vertices em volta da camera
            maxVertexX = minVertex + (int)terrainSize.X;
            maxVertexZ = minVertex + 1;
            maxVertex = maxVertexX + 1;

            // calcula a infuencia dos vertices em x anteriores á camera
            float pAX = 1.0f - ((tankPosition.X - terrainVertex[minVertex].X) / spacing.X);
            float pXB = 1.0f - ((terrainVertex[maxVertexX].X - tankPosition.X) / spacing.X);

            // calcula a media pesada da normal em x anterior á posicao da camera
            Vector3 normal1 = (terrainNormal[minVertex] * pAX) + (terrainNormal[maxVertexX] * pXB);

            // calcula a infuencia dos vertices em x  posteriores á camera
            float pCX = 1.0f - (tankPosition.X - terrainVertex[maxVertexZ].X) / spacing.X;
            float pxD = 1.0f - ((terrainVertex[maxVertex].X - tankPosition.X) / spacing.X);

            // calcula a media pesada da normal em x posterior á posicao da camera
            Vector3 normal2 = (terrainNormal[maxVertexZ] * pCX) + (terrainNormal[maxVertex] * pxD);
            // calcula a infuencia das anteriores altura em z
            float pH1X = (tankPosition.Z - terrainVertex[minVertex].Z) / spacing.Y;
            float pxH2 = (terrainVertex[maxVertexZ].Z - tankPosition.Z) / spacing.Y;

            // calcula a media pesada das normais forward, up e right da posiçao do tanque
            Vector3 n = ((normal1 * pxH2) + (normal2 * pH1X));
            Vector3 r = Vector3.Cross(n, tankBaseDirection);
            Vector3 f = Vector3.Cross(n, r);

            // define uma matrix para a rotaçao de acordo com o terreno
            Matrix normalMatrix = Matrix.Identity;
            normalMatrix.Up = n;
            normalMatrix.Forward = f;
            normalMatrix.Right = r;

            // retorna a matrix normal do tanque em relaçao ao terreno
            return normalMatrix;

        }

        // quando o tanque é atingido por uma bala
        public void hit(bool a, int dmg)
        {
            if (a)
                hp -= dmg;
        }

        public void bulletHit(bool h)
        {
            if (h)
                hp -= 25;
        }

        // retorna posicao e direccao do tanque
        public Vector3 getPosition
        { get { return tankPosition; }
        set { tankPosition = value; } }

        // retorna o yaw
        public float getDirection
        { get { return tankYaw; } }

        // retorna a vida do tanque
        public int getTankHp
        { get { return hp; } }

        // retorna a esfera de colisao
        public BoundingSphere getBSphere
        { get { return bSpere; } }

        // retorna a quantidade de balas em cena do tanque
        public int bulletCount
        { get { return bullets.Count; } }

        // provoca dano por impacto
        public bool giveDamage
        { get { return hitOnBoost; } }

        // provoca dano de bala
        public bool giveBulletDamage
        { get { return hitOnBullet; } }

        // retorna o tempo de recarga
        public int getReloadTime
        { get { return reloadingTime; } }

        // retorna se a inteligencia artificial esta activada
        public bool getAIState
        { get { return aiEnable; } }

        public string getAIMode
        { get { return boid.getState; } }

    }
}
