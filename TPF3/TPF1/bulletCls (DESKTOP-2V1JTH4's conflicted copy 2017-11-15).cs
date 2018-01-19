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
        Matrix tankUp;
        Vector3 tankNormal;
        Vector3 bulletDirection;

        // CTRL da Bala
        Matrix[] boneTransform;
        float bulletScale;
        float bulletVelocity;
        Vector3 bulletPosition;
        Matrix translation, scale, rotation;
        float bulletRoll, bulletPitch, bulletYaw;


        public bulletCls(Model bulletM, Vector2 spacing, Vector3[] vertexP, Vector2 terrainSize, Vector3 tankPosition, float tankRotation, Matrix tankUp, float cannonAngle, float turretAngle)
        {
            // recebe elementos da posiçao do tanque que disparou
            this.bulletModel = bulletM;
            this.boneTransform = new Matrix[bulletModel.Bones.Count];

            // define a direçao inicial da bala
            this.bulletDirection = Vector3.UnitZ;

            // escala 
            this.bulletScale = 1.0f;
            this.tankUp = tankUp;

            // calcula a normal do tank
            this.tankNormal = Vector3.Transform(Vector3.Up, tankUp);

            // calcula o yaw da bala
            this.bulletYaw = tankRotation + turretAngle;
            this.bulletPosition = tankPosition + (tankNormal * 100.0f) + (Vector3.Transform(bulletDirection,Matrix.CreateRotationY(bulletYaw)) * 40.0f);
            this.bulletPitch = cannonAngle;
            this.bulletRoll = 0.0f;
            bulletVelocity = 2.0f;

            bulletDirection = Vector3.Transform(bulletDirection, Matrix.CreateFromYawPitchRoll(bulletYaw, bulletPitch, 0.0f));


            // define a transaçao rotaçao e escala do modelo
            translation = Matrix.CreateTranslation(bulletPosition);
            scale = Matrix.CreateScale(bulletScale);
            rotation = Matrix.CreateFromYawPitchRoll(bulletYaw, cannonAngle, bulletRoll);

        }

        public void Update(GameTime time)
        {
        
            //actualiza a posiçao da bala 
            bulletPosition += bulletDirection * bulletVelocity;
            bulletPosition += Vector3.Down * time.ElapsedGameTime.Seconds;

            bulletRoll += MathHelper.ToRadians(5.0f);
            scale = Matrix.CreateFromYawPitchRoll(0.0f, bulletPitch, bulletRoll);
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
        }
    }
}