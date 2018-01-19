using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TPC4
{
    class planeCls
    {
        // world matrix e effect
        BasicEffect planeEffect;
        Matrix planeWorldMatrix;

        //vertices do plano
        VertexPositionNormalTexture[] planeVertex;

        // vertex Buffer
        VertexBuffer vertexBuffer;

        public planeCls(GraphicsDevice device, Texture2D texture, float pSize)
        {
            // define a matriz de mundo
            planeWorldMatrix = Matrix.Identity;

            // inicia o efeito do plano
            planeEffect = new BasicEffect(device);
            planeEffect.TextureEnabled = true;
            planeEffect.LightingEnabled = false;

            // define a textura
            planeEffect.Texture = texture;

            // chama a funçao para a criacao do plano
            createPlane(pSize, device);
        }

        private void createPlane(float pSize, GraphicsDevice device)
        {
            planeVertex = new VertexPositionNormalTexture[4];
            planeVertex[0] = new VertexPositionNormalTexture(new Vector3(-pSize, 0.0f, -pSize), Vector3.Up, new Vector2(0.0f, 0.0f));
            planeVertex[1] = new VertexPositionNormalTexture(new Vector3(+pSize, 0.0f, -pSize), Vector3.Up, new Vector2(1.0f, 0.0f));
            planeVertex[2] = new VertexPositionNormalTexture(new Vector3(-pSize, 0.0f, +pSize), Vector3.Up, new Vector2(0.0f, 1.0f));
            planeVertex[3] = new VertexPositionNormalTexture(new Vector3(+pSize, 0.0f, +pSize), Vector3.Up, new Vector2(1.0f, 1.0f));

            vertexBuffer = new VertexBuffer(device, typeof(VertexPositionNormalTexture), planeVertex.Length, BufferUsage.None);
            vertexBuffer.SetData<VertexPositionNormalTexture>(planeVertex);
        }

        public void Draw(GraphicsDevice device, Matrix view, Matrix projection)
        {
            planeEffect.View = view;
            planeEffect.Projection = projection;
            planeEffect.World = planeWorldMatrix;

            foreach(EffectPass pass in planeEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.SetVertexBuffer(vertexBuffer);
                device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            }
        }

    }
}
