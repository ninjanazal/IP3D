using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace TPF1
{
    class _3rdPersonCameraCls
    {
        // variaveis para controla da posicao e target da camera
        Matrix cameraView, cameraProjection;
        float aspectRatio, groundSpacing;
        Vector3 cameraPosition, cameraTarget;

        // pontos proximos
        int minVertex, maxVertexX, maxVertexZ, maxVertex;
        Vector3 minVector;

        // terreno
        Vector2 terrainSize;
        Vector3[] terrainVertex;
        Vector2 spacing;

        // altura
        float lastHeight;

        public _3rdPersonCameraCls(GraphicsDevice device, Vector3 targetPosition, float targetDirection, Vector2 terrainSize, Vector3[] vertexP, Vector2 spacing)
        {
            // calcula aspect ratio
            aspectRatio = (float)device.Viewport.Width / device.Viewport.Height;

            // o target da camera sendo o tanque mais um offset
            //a posicao da camera baseada na posicao do alvo mais offset

            this.cameraTarget = targetPosition + new Vector3(0.0f, 300.0f, 0.0f);
            this.cameraPosition = targetPosition + Vector3.Transform(new Vector3(0.0f, 0.0f, -800.0f) * 2, Matrix.CreateFromYawPitchRoll(targetDirection, 0.0f, 0.0f));

            // espaco entre o terreno
            this.groundSpacing = 400.0f;

            // guarda em variaveis elementos necessario para determinar a altura baseado o terreno
            this.terrainSize = terrainSize;
            this.spacing = spacing;
            this.terrainVertex = vertexP;
            // calcula a altura para a posicao
            cameraPosition.Y = getHeight() + groundSpacing;

            //cria view w projection da camera
            cameraView = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);
            cameraProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1.0f, 100000.0f);
        }


        public void Update(Vector3 targetPosition, float targetDirection)
        {
            // o target da camera sendo o tanque mais um offset
            //a posicao da camera baseada na posicao do alvo mais offset

            this.cameraTarget = targetPosition + new Vector3(0.0f, 200.0f, 0.0f);
            this.cameraPosition = targetPosition + Vector3.Transform(new Vector3(0.0f, 0.0f, -1000.0f), Matrix.CreateFromYawPitchRoll(targetDirection, 0.0f, 0.0f));

            // calcula a altura para a posicao
            if (cameraPosition.X > 0 && cameraPosition.X < spacing.X * terrainSize.X - 100.0f && cameraPosition.Z > 0 && cameraPosition.Z < spacing.Y * terrainSize.Y - 100.0f)
            {
                cameraPosition.Y = getHeight() + groundSpacing;
                lastHeight = cameraPosition.Y + 80.0f;
            }
            else cameraPosition.Y = groundSpacing;

            cameraView = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);
        }


        private float getHeight()
        {
            // determina a posicao do vertice menor mais proximo da camera
            // reduz a escala para 128, retem o valor inteiro e retorna os valores x e z do vertice
            minVector.X = (int)(cameraPosition.X / spacing.X) * spacing.X;
            minVector.Z = (int)(cameraPosition.Z / spacing.Y) * spacing.Y;

            // procura no array de vertices qual o vertice com os valores determindos anteriormente
            for (int i = 0; i < terrainVertex.Length; i++)
                if (terrainVertex[i].X == minVector.X && terrainVertex[i].Z == minVector.Z)
                {
                    minVertex = i;
                    break;
                }
            // calcula o valor dos indices dos vertices em volta da camera
            maxVertexX = minVertex + (int)terrainSize.X;
            maxVertexZ = minVertex + 1;
            maxVertex = maxVertexX + 1;

            // calcula a infuencia dos vertices em x anteriores á camera
            float pAX = 1.0f - ((cameraPosition.X - terrainVertex[minVertex].X) / spacing.X);
            float pXB = 1.0f - ((terrainVertex[maxVertexX].X - cameraPosition.X) / spacing.X);

            // calcula a media pesada da altura em x anterior á posicao da camera
            float height1 = (terrainVertex[minVertex].Y * pAX) + (terrainVertex[maxVertexX].Y * pXB);

            // calcula a infuencia dos vertices em x  posteriores á camera
            float pCX = 1.0f - (cameraPosition.X - terrainVertex[maxVertexZ].X) / spacing.X;
            float pxD = 1.0f - ((terrainVertex[maxVertex].X - cameraPosition.X) / spacing.X);

            // calcula a media pesada da altura em x posterior á posicao da camera
            float height2 = (terrainVertex[maxVertexZ].Y * pCX) + (terrainVertex[maxVertex].Y * pxD);

            // calcula a infuencia das anteriores altura em z
            float pH1X = (cameraPosition.Z - terrainVertex[minVertex].Z) / spacing.Y;
            float pxH2 = (terrainVertex[maxVertexZ].Z - cameraPosition.Z) / spacing.Y;

            // calcula a media pesada da altura em z na posicao da camera
            float h = ((height1 * pxH2) + (height2 * pH1X));

            // adiciona o espacamento da camera 
            return h;

        }

        // retorna o View
        public Matrix getView
        { get { return cameraView; } }

        //retorna Projection
        public Matrix getProjection
        { get { return cameraProjection; } }

        //retorna Posicao
        public Vector3 getPosition
        { get { return cameraPosition; } }

        // retonar e define o groundSpacing
        public float getSetGroundSpacing
        {
            get { return groundSpacing; }
            set { groundSpacing = value; }
        }
    }
}

