using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TPC3
{
    class prismCls
    {
        // BasicEffect e matrixWorld
        BasicEffect prismEffect;
        Matrix prismWorldMatrix;

        //vertex buffer e indexBuffer
        VertexBuffer vertexBuffer, vertexBufferTop, vertexBufferBottum;
        IndexBuffer indexBuffer, indexTopBuffer, indexBottumBuffer;

        //vertex e index
        int sides;
        int vertexCount, indexCount;
        VertexPositionNormalTexture[] vertex, vertexTop, vertexBottum;
        short[] index, indexTop, indexBottum;

        //movimento
        float yaw, velocity;
        Vector3 direction, position;
        float radius;

        //textura
        Texture2D texture, textureTopBottum;


        public prismCls(GraphicsDevice device, int sides, float height, float radius, Matrix view, Matrix projection, Texture2D texture, Texture2D texture2)
        {
            prismWorldMatrix = Matrix.Identity;

            prismEffect = new BasicEffect(device);
            prismEffect.View = view;
            prismEffect.Projection = projection;
            prismEffect.LightingEnabled = true;

            prismEffect.TextureEnabled = true;

            this.radius = radius;
            this.sides = sides;
            this.texture = texture;
            this.textureTopBottum = texture2;

            yaw = 0.0f;
            direction = Vector3.UnitX;
            position = Vector3.Zero;
            velocity = 0.5f;

            createsides(device, sides, height, radius);
            
        }

        private void createsides(GraphicsDevice device, int sideN, float h, float r)
        {

            //vertices
            vertexCount = 2 * sideN;
            vertex = new VertexPositionNormalTexture[vertexCount + 2];

            // calcula os vertices com uma normal basica e coordenadas para a textura
            for (int i = 0; i < sideN; i++)
            {
                double angle = i * (2 * Math.PI / sideN);
                double x = r * Math.Cos(angle);
                double z = -r * Math.Sin(angle);

                vertex[2 * i + 0] = new VertexPositionNormalTexture(new Vector3((float)x, 0.0f, (float)z), Vector3.Up, new Vector2((float)1 / sideN * i , 1.0f));
                vertex[2 * i + 1] = new VertexPositionNormalTexture(new Vector3((float)x, h, (float)z), Vector3.Up, new Vector2((float)1 / sideN * i , 0.0f));
            }

            // para que a textura dos lados seja aplicada correctamente

            // a posicao destes vertices é a mesma que os vertices na posicao 0 e 1
            vertex[vertexCount].Position = vertex[0].Position;
            vertex[vertexCount + 1].Position = vertex[1].Position;

            // coordenadas
            vertex[vertexCount].TextureCoordinate = new Vector2(1.0f, 1.0f);
            vertex[vertexCount + 1].TextureCoordinate = new Vector2(1.0f, 0.0f);

            // normal basica
            vertex[vertexCount].Normal = Vector3.Up;
            vertex[vertexCount + 1].Normal = Vector3.Up;

            // calcular normais dos vertices
            // para que a normal seja perpendicular à superficie do prisma é calculado o vector entre o centro ao vertice
            Vector3 prismCenter = new Vector3(vertex[0].Position.X - radius, 0.0f, vertex[0].Position.Z);
            Vector3 prismTopCenter = prismCenter;
            prismTopCenter.Y = h;

            // calcula a normal para cada vertice do lado
            for (int i = 0; i<sideN;i++)
            {
                vertex[2 * i + 0].Normal = vertex[2 * i + 0].Position - prismCenter;
                vertex[2 * i + 0].Normal.Normalize();
                vertex[2 * i + 1].Normal = vertex[2 * i + 1].Position - prismTopCenter;
                vertex[2 * i + 1].Normal.Normalize();
            }
            // para que a textura esteja colocada correctamente 
            vertex[vertexCount].Normal = vertex[0].Normal;
            vertex[vertexCount + 1].Normal = vertex[1].Normal;

            // vertices do topo
            vertexTop = new VertexPositionNormalTexture[sideN + 1];
            for(int i = 0;i<sideN; i++)
            {
                vertexTop[i].Position = vertex[2 * i + 1].Position;
                vertexTop[i].Normal = Vector3.Up;
                vertexTop[i].TextureCoordinate = new Vector2(0.5f + (vertexTop[i].Position.X - prismTopCenter.X) / (2 * radius), 0.5f + (vertexTop[i].Position.Z - prismTopCenter.Z) / (2 * radius));
            }
            // vertice centro do topo
            vertexTop[sideN] = new VertexPositionNormalTexture(prismTopCenter, Vector3.Up, new Vector2(0.5f, 0.5f));

            // vertices do fundo
            vertexBottum = new VertexPositionNormalTexture[sideN + 1];
            for(int i = 0; i<sideN;i++)
            {
                vertexBottum[i].Position = vertex[2 * i].Position;
                vertexBottum[i].Normal = Vector3.Down;
                vertexBottum[i].TextureCoordinate = new Vector2(0.5f + (vertexBottum[i].Position.X - prismCenter.X) / (2 * radius), 0.5f + (vertexBottum[i].Position.Z - prismCenter.Z) / (2 * radius));
            }
            // vertice centro do fundo
            vertexBottum[sideN] = new VertexPositionNormalTexture(prismCenter, Vector3.Down, new Vector2(0.5f, 0.5f));

            //indices
            //indices da parte lateral
            indexCount = 2 * sideN + 2;

            index = new short[indexCount];
            for (int v = 0; v < sideN * 2; v++)
            {
                index[v] = (short)(v);
            }
            //vertices sobrepostos para fechar a mesh
            index[sideN * 2] = (short)vertexCount;
            index[sideN * 2 + 1] = (short)(vertexCount + 1.0f);

            //indices da parte superior
            indexTop = new short[2 * sideN + 1];
            for (int i = 0; i < sideN; i++)
            {
                indexTop[2 * i + 0] = (short)(i);
                indexTop[2 * i + 1] = (short)(sideN);
            }
            //vertices sobrepostos para fechar a mesh
            indexTop[2 * sideN] = indexTop[0];

            //indices para a parte inferior
            indexBottum = new short[2 * sideN + 1];
            for(int i = 0; i<sideN;i++)
            {
                indexBottum[2 * i + 0] = (short)(sideN - i);
                indexBottum[2 * i + 1] = (short)(sideN);

            }
            indexBottum[0] = 0;
            indexBottum[2 * sideN] = 0; 

            //set buffers
            vertexBuffer = new VertexBuffer(device, typeof(VertexPositionNormalTexture), vertex.Length, BufferUsage.None);
            vertexBuffer.SetData<VertexPositionNormalTexture>(vertex);

            vertexBufferTop = new VertexBuffer(device, typeof(VertexPositionNormalTexture), vertexTop.Length, BufferUsage.None);
            vertexBufferTop.SetData<VertexPositionNormalTexture>(vertexTop);

            vertexBufferBottum = new VertexBuffer(device, typeof(VertexPositionNormalTexture), vertexBottum.Length, BufferUsage.None);
            vertexBufferBottum.SetData<VertexPositionNormalTexture>(vertexBottum);

            indexBuffer = new IndexBuffer(device, typeof(short), index.Length, BufferUsage.None);
            indexBuffer.SetData<short>(index);

            indexTopBuffer = new IndexBuffer(device, typeof(short), indexTop.Length, BufferUsage.None);
            indexTopBuffer.SetData<short>(indexTop);

            indexBottumBuffer = new IndexBuffer(device, typeof(short), indexBottum.Length, BufferUsage.None);
            indexBottumBuffer.SetData<short>(indexBottum);
        }
  

        public void Update(KeyboardState kS, float planeSize)
        {
            //roda o prisma
            if (kS.IsKeyDown(Keys.Left))
                yaw += MathHelper.ToRadians(1.0f);
            if (kS.IsKeyDown(Keys.Right))
                yaw -= MathHelper.ToRadians(1.0f);
            //calcula a rotaçao
            Vector3 rotation = Vector3.Transform(direction, Matrix.CreateFromYawPitchRoll(yaw, 0.0f, 0.0f));

            //move o prisma dado a direcçao calculada
            if (kS.IsKeyDown(Keys.Up))
                position += rotation * velocity;
            if (kS.IsKeyDown(Keys.Down))
                position -= rotation * velocity;

            if (position.X > planeSize - radius)
                position.X = planeSize - radius;
            if (position.X < -planeSize + radius)
                position.X = -planeSize + radius;
            if (position.Z > planeSize - radius)
                position.Z = planeSize - radius;
            if (position.Z < -planeSize + radius)
                position.Z = -planeSize + radius;

            //altera a matrix
            prismWorldMatrix = Matrix.CreateFromYawPitchRoll(yaw, 0.0f, 0.0f) * Matrix.CreateTranslation(position);

        }

        public void Draw(GraphicsDevice device, Matrix view, lightCls light)
        {
            //matrix e view
            prismEffect.View = view;
            prismEffect.World = prismWorldMatrix;
            prismEffect.Texture = this.texture;

            // iluminaçao
            prismEffect.PreferPerPixelLighting = true;
            prismEffect.EmissiveColor = light.getEmissive;

            prismEffect.AmbientLightColor = light.getAColor * 1.5f;
            prismEffect.DiffuseColor = light.getADifuse * 0.7f;
            prismEffect.SpecularColor = light.getASpecColor * 10.0f;
            prismEffect.SpecularPower = light.getASpecularPower;

            prismEffect.DirectionalLight0.Enabled = true;
            prismEffect.DirectionalLight0.Direction = light.getDDirection;
            prismEffect.DirectionalLight0.DiffuseColor = light.getDDifuse * 1.8f;
            prismEffect.DirectionalLight0.SpecularColor = light.getDSpecular * 0.5f;

            //define buffer de vertex e index
            device.SetVertexBuffer(vertexBuffer);
            device.Indices = indexBuffer;

            prismEffect.CurrentTechnique.Passes[0].Apply();

            // desenha lateral do prisma
            device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0, sides * 2 + 2);

            //define novos indices para a parte superior e desenha
            prismEffect.Texture = textureTopBottum;
            prismEffect.CurrentTechnique.Passes[0].Apply();

            device.SetVertexBuffer(vertexBufferTop);
            device.Indices = indexTopBuffer;
            device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0, sides * 2);

            //define novos indices para a parte inferior e desenha
            device.SetVertexBuffer(vertexBufferBottum);
            device.Indices = indexBottumBuffer;
            device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0, sides * 2);
        }

    }
}
