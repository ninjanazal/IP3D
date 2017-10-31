using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TPC1
{
    class clsPyramid
    {
        Color[] sideColor;
        VertexPositionColor[] vertex;
        BasicEffect generalEffect;
        Matrix generalWorldMatrix;
        GraphicsDevice device;

        Random rand;
        float radius = 1.0f;
        int nside;

        public clsPyramid(GraphicsDevice device , BasicEffect effect, Matrix worldMatrix, int numSides, float height)
        {
            rand = new Random();
            sideColor = new Color[10];

            this.device = device;
            this.generalEffect = effect;
            this.generalWorldMatrix = worldMatrix;

            this.nside = numSides;
            

            //gera aleatoriamente um array de cores
            for (int i = 0; i < 10; i++)
                sideColor[i] = new Color(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255));            

            generatePyramid(this.nside, height,sideColor);     

        }

        public void generatePyramid(int nS, float h,Color[] cor)
        {
            vertex = new VertexPositionColor[nS * 3];
            for(int i = 0; i<nS; i++)
            {
                // calcula o angulo e coordenadas dos vertices dos 2 primeiros vertices
                float angle = i * (2 * (float)Math.PI) / nS;
                float x = radius * (float)Math.Cos(angle);
                float z = -radius * (float)Math.Sin(angle);

                vertex[3 * i] = new VertexPositionColor(new Vector3(x, 0.0f, z), sideColor[i]);
                vertex[3 * i + 1] = new VertexPositionColor(new Vector3(0.0f, h, 0.0f), sideColor[i]);

                // calcula o angulo e coordenadas do vertice 3 de cada lado

                angle = (i + 1) * (2 * (float)Math.PI) / nS;
                x = radius * (float)Math.Cos(angle);
                z = -radius * (float)Math.Sin(angle);

                vertex[3 * i + 2] = new VertexPositionColor(new Vector3(x, 0.0f, z), sideColor[i]);

            }
        }

        public void Rotate(float rotAngle)
        {
            generalWorldMatrix *= Matrix.CreateRotationY(rotAngle);
        }

        public void Draw(GraphicsDevice device)
        {

            generalEffect.World = generalWorldMatrix;
            generalEffect.CurrentTechnique.Passes[0].Apply();
            device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, vertex, 0, nside);
        }
    }
}
