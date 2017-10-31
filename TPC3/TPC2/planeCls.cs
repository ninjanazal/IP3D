using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace TPC3
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


        public planeCls(GraphicsDevice device, Matrix view, Matrix projection, Texture2D texture, float pSize)
        {

            // definir matrix de mundo
            planeWorldMatrix = Matrix.Identity;

            //iniciar efeito do plano
            planeEffect = new BasicEffect(device);
            planeEffect.Projection = projection;
            planeEffect.LightingEnabled = true;
            planeEffect.TextureEnabled = true;

            // definir textura
            planeEffect.Texture = texture;

            //inicia a funcao para criar o plano
            createPlane(pSize, device);

        }

        // cria um plano com uma textura aplicada
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

        
        public void Draw(GraphicsDevice device, Matrix view, lightCls light)
        {
            // effect vindo da classe camera
            planeEffect.View = view;

            // world 
            planeEffect.World = planeWorldMatrix;

            // iluminaçao
            planeEffect.PreferPerPixelLighting = true;
            planeEffect.EmissiveColor = light.getEmissive;

            planeEffect.AmbientLightColor = light.getAColor * 0.8f;
            planeEffect.DiffuseColor = light.getADifuse * 0.8f;
            planeEffect.SpecularColor = light.getASpecColor * 0.8f;
            planeEffect.SpecularPower = light.getASpecularPower;

            planeEffect.DirectionalLight0.Enabled = true;
            planeEffect.DirectionalLight0.Direction = light.getDDirection;
            planeEffect.DirectionalLight0.DiffuseColor = light.getDDifuse * 1.1f;
            planeEffect.DirectionalLight0.SpecularColor = light.getDSpecular * 1.0f;

            planeEffect.CurrentTechnique.Passes[0].Apply();

            device.SetVertexBuffer(vertexBuffer);
            device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
        }
    }
}
