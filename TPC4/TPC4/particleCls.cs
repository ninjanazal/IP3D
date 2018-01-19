using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TPC4
{
    class particleCls
    {
        // direçao
        Vector3 direction, particlePosition;

        // particula
        VertexPositionColor[] vertex;

        BasicEffect particleEffect;
        Matrix particleWorld;

        // detruir particlula
        private bool isDeath;

        public particleCls(GraphicsDevice device, Vector3 gravity, Vector3 wind,Vector3 randWind ,Vector3 pos)
        {
            // effeito
            particleEffect = new BasicEffect(device);
            particleEffect.LightingEnabled = false;
            particleEffect.VertexColorEnabled = true;

            particleWorld = Matrix.Identity;

            isDeath = false;
            this.direction = gravity + wind + randWind;
            particlePosition = pos;

            // inicia o array de posiçoes
            vertex = new VertexPositionColor[2];
            vertex[0] = new VertexPositionColor(particlePosition, Color.Blue);
            vertex[1] = vertex[0];
        }

        public void Update()
        {
            vertex[0].Position = vertex[1].Position + (Vector3.Up * 10.0f);

            // actualiza a posiçao da particula
            particlePosition += direction;

            vertex[1].Position = particlePosition;

            // determina se a posicao da particula passou a posiçao do plano
            if (particlePosition.Y < 0)
                isDeath = true;

        }

        public void Draw(GraphicsDevice device, Matrix view, Matrix projection)
        {
            particleEffect.World = particleWorld;
            particleEffect.View = view;
            particleEffect.Projection = projection;

            foreach (EffectPass pass in particleEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertex, 0, 1);
            }
        }

        public bool getDeathState
        { get { return isDeath; } }

    }
}
