using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TPF1
{
    class axis3DAcls
    {
        BasicEffect axisEffect;
        Matrix axisWorldMatrix;

        VertexBuffer vertexBuffer;

        // vertices dos eixos
        private VertexPositionColor[] vertex;

        float axisLenght = 1000.0f;

        public axis3DAcls(GraphicsDevice device)
        {
            axisWorldMatrix = Matrix.Identity;
            axisEffect = new BasicEffect(device);
            axisEffect.LightingEnabled = false;
            axisEffect.VertexColorEnabled = true;

            // define vertices dos eixos
            createAxis(device);
        }

        private void createAxis(GraphicsDevice device)
        {
            vertex = new VertexPositionColor[6];
            vertex[0] = new VertexPositionColor(new Vector3(-axisLenght, 0.0f, 0.0f), Color.Blue);
            vertex[1] = new VertexPositionColor(new Vector3(axisLenght, 0.0f, 0.0f), Color.Blue);
            vertex[2] = new VertexPositionColor(new Vector3(0.0f, -axisLenght, 0.0f), Color.Red);
            vertex[3] = new VertexPositionColor(new Vector3(0.0f, axisLenght, 0.0f), Color.Red);
            vertex[4] = new VertexPositionColor(new Vector3(0.0f, 0.0f, -axisLenght), Color.Green);
            vertex[5] = new VertexPositionColor(new Vector3(0.0f, 0.0f, axisLenght), Color.Green);

            vertexBuffer = new VertexBuffer(device, typeof(VertexPositionColor), vertex.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColor>(vertex);
        }
        public void Draw(GraphicsDevice device, Matrix view, Matrix projection)
        {
            axisEffect.View = view;
            axisEffect.Projection = projection;

            axisEffect.World = axisWorldMatrix;

            device.SetVertexBuffer(vertexBuffer);

            foreach(EffectPass pass in axisEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawPrimitives(PrimitiveType.LineList, 0, 3);
            }
        }
    }
}
