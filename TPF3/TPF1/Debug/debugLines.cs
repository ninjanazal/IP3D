using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPF1
{
    class debugLines
    {
        bool enable;
        KeyboardState lastState;

        BasicEffect debugEffect;
        Matrix debugWordMatrix;

        VertexPositionColor[] vertex;
        VertexBuffer vertexBuffer;

        Vector3[] position, normal;

        public debugLines(GraphicsDevice device, Vector3[] vertexP, Vector3[] vertexN)
        {
            enable = false;
            debugWordMatrix = Matrix.Identity;
            debugEffect = new BasicEffect(device);
            position = vertexP;
            normal = vertexN;
            CreateLines(device);
        }


        private void CreateLines(GraphicsDevice device)
        {
            vertex = new VertexPositionColor[2 * position.Length];
            for (int i = 0; i < position.Length; i++)
            {
                vertex[2 * i + 0] = new VertexPositionColor(position[i], Color.Blue);
                vertex[2 * i + 1] = new VertexPositionColor(position[i] + normal[i] * 40.0f, Color.Green);
            }

            vertexBuffer = new VertexBuffer(device, typeof(VertexPositionColor), vertex.Length, BufferUsage.None);
            vertexBuffer.SetData<VertexPositionColor>(vertex);

        }
        public void Update(GraphicsDevice device, KeyboardState kS)
        {
            if (kS.IsKeyDown(Keys.P) && lastState.IsKeyUp(Keys.P))
                if (enable == true)
                    enable = false;
                else
                    enable = true;
            lastState = kS;


        }

        public void Draw(GraphicsDevice device, Matrix view, Matrix projection)
        {
            debugEffect.View = view;
            debugEffect.Projection = projection;
            debugEffect.World = debugWordMatrix;

            debugEffect.LightingEnabled = false;
            debugEffect.VertexColorEnabled = true;

            device.SetVertexBuffer(vertexBuffer);
            if (enable)
            {
                foreach (EffectPass pass in debugEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    device.DrawPrimitives(PrimitiveType.LineList, 0, vertex.Length / 2);
                }
            }


        }
    }
}
