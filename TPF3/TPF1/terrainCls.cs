using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TPF1
{
    class terrainCls
    {
        //matrix e effeito
        Matrix terrainMatrixWorld;
        BasicEffect terrainEffect;

        // Tamanho do terreno
        Vector2 terrainSize;
        public Vector2 vertexSpacing;

        //vertices e indices
        Vector3[] terreinVertexPosition, terrainNormalVertex;
        VertexPositionNormalTexture[] terrainVertex;
        short[] terrainIndex;
        int vertexCount;
        float size;

        // texturas
        Texture2D heightMap, groundTexture;
        float textureStep;

        // altura do mapa
        float scale;
        Color[] heightColor;

        //Buffers
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;



        public terrainCls(GraphicsDevice device, Texture2D hM, Texture2D texture, float groundSize)
        {
            terrainMatrixWorld = Matrix.Identity;

            terrainEffect = new BasicEffect(device);

            this.size = groundSize;
            this.heightMap = hM;
            this.groundTexture = texture;
            scale = 2.0f;

            createGround(device);
        }

        private void createGround(GraphicsDevice device)
        {
            // obtem o tamanho do mapa a partir do mapa de alturas
            terrainSize.X = heightMap.Width;
            terrainSize.Y = heightMap.Height;

            // transfere a informaçao da imagem num array de cores
            vertexCount = (int)heightMap.Width * heightMap.Height;
            heightColor = new Color[vertexCount];
            heightMap.GetData(heightColor);

            // calcula o espaçamento entre vertices
            vertexSpacing.X = (size / terrainSize.X);
            vertexSpacing.Y = (size / terrainSize.Y);

            // textura step
            int repeat = 6;

            // define o avanço de cada posiçao de textura nos vertices
            textureStep = 1 / (terrainSize.X - 1);
            textureStep *= repeat;

            // inicia o array de vertices
            terrainVertex = new VertexPositionNormalTexture[vertexCount];

            // calcula a posiçao dos vertices 
            for (int x = 0; x < terrainSize.X; x++)
                for (int y = 0; y < terrainSize.Y; y++)
                    terrainVertex[y + x * (int)terrainSize.X] = new VertexPositionNormalTexture(new Vector3(vertexSpacing.X * x, (float)heightColor[y + x * (int)terrainSize.X].R * scale, vertexSpacing.Y * y), Vector3.Up, new Vector2(x * textureStep, y * textureStep));

            // calcula normais dos vertices de acordo com o terreno
            getNormals();

            // inicia o array de indices
            terrainIndex = new short[vertexCount * 2 - (short)(terrainSize.X * 2)];


            for (int i = 0; i < terrainIndex.Length / 2; i++)
            {
                terrainIndex[2 * i + 0] = (short)(i);
                terrainIndex[2 * i + 1] = (short)(terrainSize.X + i);
            }

            // inicia os buffers
            vertexBuffer = new VertexBuffer(device, typeof(VertexPositionNormalTexture), terrainVertex.Length, BufferUsage.None);
            vertexBuffer.SetData<VertexPositionNormalTexture>(terrainVertex);

            indexBuffer = new IndexBuffer(device, typeof(short), terrainIndex.Length, BufferUsage.None);
            indexBuffer.SetData<short>(terrainIndex);

        }

        public void Draw(GraphicsDevice device, Matrix view, Matrix projection, lightControllerCls lightCtrl)
        {
            //define worldMatrix , view e projection
            terrainEffect.World = terrainMatrixWorld;

            terrainEffect.View = view;
            terrainEffect.Projection = projection;

            // iluminacao
            terrainEffect.PreferPerPixelLighting = true;
            terrainEffect.LightingEnabled = true;

            terrainEffect.EmissiveColor = lightCtrl.getEmissive;

            terrainEffect.AmbientLightColor = lightCtrl.getAColor;
            terrainEffect.DiffuseColor = lightCtrl.getADifuse;
            terrainEffect.SpecularColor = lightCtrl.getASpecColor;
            terrainEffect.SpecularPower = lightCtrl.getASpecularPower;

            terrainEffect.DirectionalLight0.Enabled = true;
            terrainEffect.DirectionalLight0.Direction = lightCtrl.getDDirection;
            terrainEffect.DirectionalLight0.DiffuseColor = lightCtrl.getDDifuse;
            terrainEffect.DirectionalLight0.SpecularColor = lightCtrl.getDSpecular;

            // textura
            terrainEffect.Texture = groundTexture;
            terrainEffect.TextureEnabled = true;

            device.SetVertexBuffer(vertexBuffer);
            device.Indices = indexBuffer;


            foreach (EffectPass pass in terrainEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                for (int i = 0; i < terrainSize.X; i++)
                    device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, i * (int)terrainSize.X * 2, 2 * (int)terrainSize.X - 2);

            }
            //define buffer de vertex e index

        }

        private void getNormals()
        {
            //normal dos vertices
            for (int x = 0; x < terrainSize.X; x++)
                for (int y = 0; y < terrainSize.Y; y++)
                {
                    short normalCount;
                    // vertice menor
                    if (x == 0 && y == 0)
                    {
                        Vector3 v1 = terrainVertex[(y + x * (int)terrainSize.X) + 1].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;
                        Vector3 v2 = terrainVertex[(y + x * (int)(terrainSize.X)) + (int)terrainSize.X + 1].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;
                        Vector3 v3 = terrainVertex[(y + x * (int)(terrainSize.X)) + (int)terrainSize.X].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;

                        v1.Normalize(); v2.Normalize(); v3.Normalize();

                        Vector3 normal1 = Vector3.Cross(v1, v2);
                        Vector3 normal2 = Vector3.Cross(v2, v3);

                        normalCount = 2;
                        Vector3 average = new Vector3((normal1.X + normal2.X) / normalCount, (normal1.Y + normal2.Y) / normalCount, (normal1.Z + normal2.Z) / normalCount);
                        average.Normalize();

                        terrainVertex[y + x * (int)terrainSize.X].Normal = average;


                    }
                    //vertice maior primeira coluna
                    else if (x == 0 && y == terrainSize.Y - 1.0f)
                    {
                        Vector3 v1 = terrainVertex[(y + x * (int)terrainSize.X) + (int)terrainSize.X].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;
                        Vector3 v2 = terrainVertex[(y + x * (int)terrainSize.X) + (int)terrainSize.X - 1].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;
                        Vector3 v3 = terrainVertex[(y + x * (int)terrainSize.X) - 1].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;

                        v1.Normalize(); v2.Normalize(); v3.Normalize();

                        Vector3 normal1 = Vector3.Cross(v1, v2);
                        Vector3 normal2 = Vector3.Cross(v2, v3);

                        normalCount = 2;
                        Vector3 average = new Vector3((normal1.X + normal2.X) / normalCount, (normal1.Y + normal2.Y) / normalCount, (normal1.Z + normal2.Z) / normalCount);
                        average.Normalize();
                        terrainVertex[y + x * (int)terrainSize.X].Normal = average;

                    }
                    //vertice maior primeira linha
                    else if (x == terrainSize.X - 1.0f && y == 0)
                    {
                        Vector3 v1 = terrainVertex[(y + x * (int)terrainSize.X) - (int)terrainSize.X].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;
                        Vector3 v2 = terrainVertex[(y + x * (int)(terrainSize.X) - (int)terrainSize.X + 1)].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;
                        Vector3 v3 = terrainVertex[(y + x * (int)terrainSize.X) + 1].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;

                        v1.Normalize(); v2.Normalize(); v3.Normalize();

                        Vector3 normal1 = Vector3.Cross(v1, v2);
                        Vector3 normal2 = Vector3.Cross(v2, v3);

                        normalCount = 2;
                        Vector3 average = new Vector3((normal1.X + normal2.X) / normalCount, (normal1.Y + normal2.Y) / normalCount, (normal1.Z + normal2.Z) / normalCount);
                        average.Normalize();
                        terrainVertex[y + x * (int)terrainSize.X].Normal = average;

                    }
                    // vertice maior ultima coluna
                    else if (x == terrainSize.X - 1.0f && y == terrainSize.Y - 1.0f)
                    {
                        Vector3 v1 = terrainVertex[(y + x * (int)terrainSize.X) - (int)terrainSize.X - 1].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;
                        Vector3 v2 = terrainVertex[(y + x * (int)(terrainSize.X) - (int)terrainSize.X)].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;
                        Vector3 v3 = terrainVertex[y + x * (int)(terrainSize.X) - 1].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;

                        v1.Normalize(); v2.Normalize(); v3.Normalize();

                        Vector3 normal1 = Vector3.Cross(v1, v2);
                        Vector3 normal2 = Vector3.Cross(v3, v2);

                        normalCount = 2;
                        Vector3 average = new Vector3((normal1.X + normal2.X) / normalCount, (normal1.Y + normal2.Y) / normalCount, (normal1.Z + normal2.Z) / normalCount);
                        average.Normalize();
                        terrainVertex[y + x * (int)terrainSize.X].Normal = average;

                    }
                    // interior da primeira coluna
                    else if (x == 0 & y > 0 && y < terrainSize.Y - 1.0)
                    {
                        Vector3 v1 = terrainVertex[(y + x * (int)(terrainSize.X) + 1)].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;
                        Vector3 v2 = terrainVertex[(y + x * (int)terrainSize.X) + (int)terrainSize.X + 1].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;
                        Vector3 v3 = terrainVertex[y + x * (int)(terrainSize.X) + (int)terrainSize.X].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;
                        Vector3 v4 = terrainVertex[(y + x * (int)terrainSize.X) + (int)terrainSize.X - 1].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;
                        Vector3 v5 = terrainVertex[(y + x * (int)terrainSize.X) - 1].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;

                        v1.Normalize(); v2.Normalize(); v3.Normalize(); v4.Normalize(); v5.Normalize();

                        Vector3 normal1 = Vector3.Cross(v1, v2);
                        Vector3 normal2 = Vector3.Cross(v2, v3);
                        Vector3 normal3 = Vector3.Cross(v3, v4);
                        Vector3 normal4 = Vector3.Cross(v4, v5);

                        normalCount = 4;
                        Vector3 average = new Vector3((normal1.X + normal2.X + normal3.X + normal4.X) / normalCount, (normal1.Y + normal2.Y + normal3.Y + normal4.Y) / normalCount, (normal1.Z + normal2.Z + normal3.Z + normal4.Z) / normalCount);
                        average.Normalize();
                        terrainVertex[y + x * (int)terrainSize.X].Normal = average;
                    }
                    // interior ultima coluna
                    else if (x == terrainSize.X - 1.0f && y > 0 && y < terrainSize.Y - 1.0f)
                    {
                        Vector3 v1 = terrainVertex[(y + x * (int)terrainSize.X) - 1].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;
                        Vector3 v2 = terrainVertex[(y + x * (int)terrainSize.X) - (int)terrainSize.X - 1].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;
                        Vector3 v3 = terrainVertex[(y + x * (int)terrainSize.X) - (int)terrainSize.X].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;
                        Vector3 v4 = terrainVertex[(y + x * (int)terrainSize.X) - (int)terrainSize.X + 1].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;
                        Vector3 v5 = terrainVertex[(y + x * (int)terrainSize.X) + 1].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;

                        v1.Normalize(); v2.Normalize(); v3.Normalize(); v4.Normalize(); v5.Normalize();

                        Vector3 normal1 = Vector3.Cross(v1, v2);
                        Vector3 normal2 = Vector3.Cross(v2, v3);
                        Vector3 normal3 = Vector3.Cross(v3, v4);
                        Vector3 normal4 = Vector3.Cross(v4, v5);
                        normalCount = 4;
                        Vector3 average = new Vector3((normal1.X + normal2.X + normal3.X + normal4.X) / normalCount, (normal1.Y + normal2.Y + normal3.Y + normal4.Y) / normalCount, (normal1.Z + normal2.Z + normal3.Z + normal4.Z) / normalCount);
                        average.Normalize();
                        terrainVertex[y + x * (int)terrainSize.X].Normal = average;
                    }
                    // interior primeira linha
                    else if (x > 0 && x < terrainSize.X - 1.0 && y == 0)
                    {
                        Vector3 v1 = terrainVertex[(y + x * (int)terrainSize.X) - (int)terrainSize.X].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;
                        Vector3 v2 = terrainVertex[(y + x * (int)terrainSize.X) - (int)terrainSize.X + 1].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;
                        Vector3 v3 = terrainVertex[(y + x * (int)terrainSize.X) + 1].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;
                        Vector3 v4 = terrainVertex[(y + x * (int)terrainSize.X) + (int)terrainSize.X + 1].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;
                        Vector3 v5 = terrainVertex[(y + x * (int)terrainSize.X) + (int)terrainSize.X].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;

                        v1.Normalize(); v2.Normalize(); v3.Normalize(); v4.Normalize(); v5.Normalize();

                        Vector3 normal1 = Vector3.Cross(v1, v2);
                        Vector3 normal2 = Vector3.Cross(v2, v3);
                        Vector3 normal3 = Vector3.Cross(v3, v4);
                        Vector3 normal4 = Vector3.Cross(v4, v5);
                        normalCount = 4;

                        Vector3 average = new Vector3((normal1.X + normal2.X + normal3.X + normal4.X) / normalCount, (normal1.Y + normal2.Y + normal3.Y + normal4.Y) / normalCount, (normal1.Z + normal2.Z + normal3.Z + normal4.Z) / normalCount);
                        average.Normalize();
                        terrainVertex[y + x * (int)terrainSize.X].Normal = average;
                    }
                    // interior ultima linha
                    else if (x > 0 && x < terrainSize.X - 1.0 && y == terrainSize.Y - 1.0)
                    {
                        Vector3 v1 = terrainVertex[(y + x * (int)terrainSize.X) + (int)terrainSize.X].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;
                        Vector3 v2 = terrainVertex[(y + x * (int)terrainSize.X) + (int)terrainSize.X - 1].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;
                        Vector3 v3 = terrainVertex[(y + x * (int)terrainSize.X) - 1].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;
                        Vector3 v4 = terrainVertex[(y + x * (int)terrainSize.X) - (int)terrainSize.X - 1].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;
                        Vector3 v5 = terrainVertex[(y + x * (int)terrainSize.X) - (int)terrainSize.X].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;

                        v1.Normalize(); v2.Normalize(); v3.Normalize(); v4.Normalize(); v5.Normalize();

                        Vector3 normal1 = Vector3.Cross(v1, v2);
                        Vector3 normal2 = Vector3.Cross(v2, v3);
                        Vector3 normal3 = Vector3.Cross(v3, v4);
                        Vector3 normal4 = Vector3.Cross(v4, v5);
                        normalCount = 4;

                        Vector3 average = new Vector3((normal1.X + normal2.X + normal3.X + normal4.X) / normalCount, (normal1.Y + normal2.Y + normal3.Y + normal4.Y) / normalCount, (normal1.Z + normal2.Z + normal3.Z + normal4.Z) / normalCount);
                        average.Normalize();
                        terrainVertex[y + x * (int)terrainSize.X].Normal = average;
                    }
                    // miolo do terreno
                    else if (x > 0 && x < terrainSize.X - 1.0 && y > 0 && y < terrainSize.Y - 1.0)
                    {
                        Vector3 v1 = terrainVertex[(y + x * (int)terrainSize.X) - (int)terrainSize.X].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;
                        Vector3 v2 = terrainVertex[(y + x * (int)terrainSize.X) - (int)terrainSize.X + 1].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;
                        Vector3 v3 = terrainVertex[(y + x * (int)terrainSize.X) + 1].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;
                        Vector3 v4 = terrainVertex[(y + x * (int)terrainSize.X) + (int)terrainSize.X + 1].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;
                        Vector3 v5 = terrainVertex[(y + x * (int)terrainSize.X) + (int)terrainSize.X].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;
                        Vector3 v6 = terrainVertex[(y + x * (int)terrainSize.X) + (int)terrainSize.X - 1].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;
                        Vector3 v7 = terrainVertex[(y + x * (int)terrainSize.X) - 1].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;
                        Vector3 v8 = terrainVertex[(y + x * (int)terrainSize.X) - (int)terrainSize.X - 1].Position - terrainVertex[(y + x * (int)terrainSize.X)].Position;

                        v1.Normalize(); v2.Normalize(); v3.Normalize(); v4.Normalize(); v5.Normalize(); v6.Normalize(); v7.Normalize(); v8.Normalize();

                        Vector3 normal1 = Vector3.Cross(v1, v2);
                        Vector3 normal2 = Vector3.Cross(v2, v3);
                        Vector3 normal3 = Vector3.Cross(v3, v4);
                        Vector3 normal4 = Vector3.Cross(v4, v5);
                        Vector3 normal5 = Vector3.Cross(v5, v6);
                        Vector3 normal6 = Vector3.Cross(v6, v7);
                        Vector3 normal7 = Vector3.Cross(v7, v8);
                        Vector3 normal8 = Vector3.Cross(v8, v1);
                        normalCount = 8;

                        Vector3 average = new Vector3((normal1.X + normal2.X + normal3.X + normal4.X + normal5.X + normal6.X + normal7.X + normal8.X) / normalCount,
                            (normal1.Y + normal2.Y + normal3.Y + normal4.Y + normal5.Y + normal6.Y + normal7.Y + normal8.Y) / normalCount,
                            (normal1.Z + normal2.Z + normal3.Z + normal4.Z + normal5.Z + normal6.Z + normal7.Z + normal8.Z) / normalCount);
                        average.Normalize();
                        terrainVertex[y + x * (int)terrainSize.X].Normal = average;
                    }
                }

        }

        public Vector3[] getVertex()
        {
            // retorna um array de vectores com a posiçao dos vertices
            terreinVertexPosition = new Vector3[vertexCount];
            for (int i = 0; i < vertexCount; i++)
                terreinVertexPosition[i] = terrainVertex[i].Position;

            return terreinVertexPosition;
        }
        public Vector3[] getNormal()
        {
            // retorna um array de vectores com a normais dos vertices
            terrainNormalVertex = new Vector3[vertexCount];
            for (int i = 0; i < vertexCount; i++)
                terrainNormalVertex[i] = terrainVertex[i].Normal;

            return terrainNormalVertex;
        }
    }
}
