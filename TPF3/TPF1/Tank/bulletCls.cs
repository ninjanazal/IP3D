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
    class bulletCls
    {
        // modelo
        Model bulletModel;

        // posicao do disparo
        Vector3 bulletDirection;

        // CTRL da Bala
        double shootTime;
        Matrix[] boneTransform;
        float bulletScale;
        float bulletVelocity;
        Vector3 bulletPosition, gravity;
        Matrix translation, scale, rotation, turretM;
        float bulletRoll, bulletPitch, bulletYaw;
        bool isDeath;
        Vector3 deathPosition;

        BoundingSphere bSphere;

        // pontos proximos
        int minVertex, maxVertexX, maxVertexZ, maxVertex;
        Vector3 minVector;



        public bulletCls(Model bulletM, Vector3 tankPosition, Matrix tankNormal, Matrix turretMatrix, float tankYaw, float cannonAngle, float turretAngle, GameTime time)
        {
            isDeath = false;

            // recebe elementos da posiçao do tanque que disparou
            this.turretM = turretMatrix;
            this.bulletModel = bulletM;
            this.boneTransform = new Matrix[bulletModel.Bones.Count];

            // tempo do disparo
            this.shootTime = time.TotalGameTime.TotalSeconds;
        
            // escala 
            this.bulletScale = 0.6f;

            // gravidade
            gravity = new Vector3(0.0f, -9.8f, 0.0f);


            // calcula o yaw da bala
            this.bulletYaw = tankYaw + turretAngle;
            this.bulletPitch = cannonAngle;
            this.bulletRoll = 0.0f;
            this.bulletVelocity = 40.0f;

            // define a direçao e posiçao inicial da bala
            this.bulletDirection = Vector3.Transform(turretM.Backward, 
                Matrix.CreateFromAxisAngle(turretM.Right, cannonAngle - MathHelper.ToRadians(1.0f)));
            this.bulletDirection.Normalize();

            // calcula a posiçao inicial da bala
            this.bulletPosition = (tankPosition - tankNormal.Backward * 8.5f) + turretM.Up * 100.0f + turretM.Backward * 35.0f + bulletDirection * 10.0f;

            //Colisao
            bSphere = new BoundingSphere(bulletPosition, 15.0f);
            
            // define a transaçao rotaçao e escala do modelo
            translation = Matrix.CreateTranslation(bulletPosition);
            scale = Matrix.CreateScale(bulletScale);
            rotation = Matrix.CreateFromYawPitchRoll(bulletYaw, bulletPitch, bulletRoll);

        }

        public void Update(GameTime time, Vector2 spacing, Vector3[] terrainVertex, Vector2 terrainSize, float groundScale)
        {

            //actualiza a posiçao da bala 
            bulletPosition += bulletDirection * bulletVelocity;
            bulletPosition += gravity * (float)(time.TotalGameTime.TotalSeconds - shootTime);

            if (bulletPosition.X > 0 && bulletPosition.X < terrainVertex[terrainVertex.Length - 1].X && bulletPosition.Z > 0 &&
                bulletPosition.Z < terrainVertex[terrainVertex.Length - 1].Z)
                if (getHeight(spacing, terrainVertex, terrainSize) >= bulletPosition.Y)
                {
                    isDeath = true;
                    deathPosition = bulletPosition;
                }
                else
                    isDeath = false;
            else
            {
                isDeath = true;
                deathPosition = bulletPosition;
            }

            // actualiza a posiçao da bala
            bSphere.Center = bulletPosition;
            //rotacao da bala
            bulletRoll += MathHelper.ToRadians(10.0f);

            rotation = Matrix.CreateFromYawPitchRoll(bulletYaw, bulletPitch, bulletRoll);
            translation = Matrix.CreateTranslation(bulletPosition);
        }

        public void Draw(GraphicsDevice device, Matrix view, Matrix projection, lightControllerCls lightCtrl)
        {
            bulletModel.Root.Transform = scale * rotation * translation;

            bulletModel.CopyAbsoluteBoneTransformsTo(boneTransform);
            foreach (ModelMesh mesh in bulletModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = boneTransform[mesh.ParentBone.Index];
                    effect.View = view;
                    effect.Projection = projection;

                    // iluminaçao
                    effect.TextureEnabled = true;
                    effect.LightingEnabled = true;
                    effect.PreferPerPixelLighting = true;
                    // luz emissora
                    effect.EmissiveColor = lightCtrl.getEmissive * 5.0f;

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
        }

        private float getHeight(Vector2 spacing, Vector3[] terrainVertex, Vector2 terrainSize)
        {
            // determina a posicao do vertice menor mais proximo da camera
            // reduz a escala para 128, retem o valor inteiro e retorna os valores x e z do vertice
            minVector.X = (int)(bulletPosition.X / spacing.X) * spacing.X;
            minVector.Z = (int)(bulletPosition.Z / spacing.Y) * spacing.Y;

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
            float pAX = 1.0f - ((bulletPosition.X - terrainVertex[minVertex].X) / spacing.X);
            float pXB = 1.0f - ((terrainVertex[maxVertexX].X - bulletPosition.X) / spacing.X);

            // calcula a media pesada da altura em x anterior á posicao da camera
            float height1 = (terrainVertex[minVertex].Y * pAX) + (terrainVertex[maxVertexX].Y * pXB);

            // calcula a infuencia dos vertices em x  posteriores á camera
            float pCX = 1.0f - (bulletPosition.X - terrainVertex[maxVertexZ].X) / spacing.X;
            float pxD = 1.0f - ((terrainVertex[maxVertex].X - bulletPosition.X) / spacing.X);

            // calcula a media pesada da altura em x posterior á posicao da camera
            float height2 = (terrainVertex[maxVertexZ].Y * pCX) + (terrainVertex[maxVertex].Y * pxD);

            // calcula a infuencia das anteriores altura em z
            float pH1X = (bulletPosition.Z - terrainVertex[minVertex].Z) / spacing.Y;
            float pxH2 = (terrainVertex[maxVertexZ].Z - bulletPosition.Z) / spacing.Y;

            // calcula a media pesada da altura em z na posicao da camera
            float h = ((height1 * pxH2) + (height2 * pH1X));
            return h;
        }

        public bool getDethState
        { get { return isDeath; }
            set { isDeath = value; } }
          
        public BoundingSphere getBSphere
        { get { return bSphere; } }

        public Vector3 getDeathPosition
        { get { return deathPosition; } }
    }
}