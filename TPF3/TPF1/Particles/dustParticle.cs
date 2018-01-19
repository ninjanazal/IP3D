using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TPF1
{
    class dustParticle
    {
        // Modelo
        Model dust;

        // tempo de vida da particula
        int lifeTime;
        int deathTime;

        // matriz de posiçao, escala e rotaçao
        Matrix[] boneTransform;
        Matrix translation, scale, rotation;

        // vectores de direclao, posiçao inicial e velocidade;
        Vector3 direction, particlePosition;
        float velocity;

        // escala
        float scaleObj, alpha, aScale;

        //a particla esta morta
        private bool isDeath;

        public dustParticle(Model d, Vector3 direction, Vector3 startPosition, int startTimeSeconds, int randLife)
        {
            // inicia variaveis com informaçao para tempo de vida, tempo em que morre e velocidade
            this.isDeath = false;
            this.lifeTime = 1 + randLife;
            this.deathTime = startTimeSeconds + lifeTime;
            this.velocity = 1.0f;

            // recebe modelo, direçao da particula e a posiçao inicial
            this.dust = d;
            this.direction = direction;
            this.particlePosition = startPosition;

            // define a matriz de escala , rotaçao e translaçao
            scaleObj = 0.0f;
            alpha = 0.4f;

            aScale = alpha / (lifeTime * 60);

            this.scale = Matrix.CreateScale(scaleObj);
            this.rotation = Matrix.Identity;
            this.translation = Matrix.CreateTranslation(particlePosition);

            this.boneTransform = new Matrix[dust.Bones.Count];
        }

        public void Update(int totalGameSeconds)
        {
            // verifica se o tempo de vida foi ultrapassado
            if (totalGameSeconds > deathTime)
                isDeath = true;
            else
            {
                // actualiza a posiçao da particula
                particlePosition += direction * velocity;
                particlePosition += Vector3.Left;

                // actualiza a escala da particula e a opacidade
                scaleObj += 0.08f;
                alpha -= aScale;

                this.scale = Matrix.CreateScale(scaleObj);
                translation = Matrix.CreateTranslation(particlePosition);
            }
        }

        public void Draw(GraphicsDevice device, Matrix view, Matrix projection, lightControllerCls lightCtrl)
        {
            dust.Root.Transform = scale * rotation * translation;

            dust.CopyAbsoluteBoneTransformsTo(boneTransform);

            device.BlendState = BlendState.AlphaBlend;
            device.DepthStencilState = DepthStencilState.DepthRead;
            foreach (ModelMesh mesh in dust.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = boneTransform[mesh.ParentBone.Index];
                    effect.View = view;
                    effect.Projection = projection;

                    // iluminaçao
                    effect.Alpha = alpha;
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
            device.DepthStencilState = DepthStencilState.Default;
            device.BlendState = BlendState.Opaque;


        }

        public bool getDeathState
        { get { return isDeath; } }


    }
}
