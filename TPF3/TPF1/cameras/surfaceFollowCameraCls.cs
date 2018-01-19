using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TPF1
{
    class surfaceFollowCameraCls
    {
        // variaveis para controlo de posicao e target da camera
        Matrix cameraView, cameraProjection;
        float aspectRatio, cameraVelocity, groundSpacing;
        Vector3 cameraPosition, cameraTarget;
        float calculatedHeight;

        // rotaçao da camera
        Vector3 cameraDirection;

        //tamanho do terrenho
        Vector2 terrainSize;
        Vector3[] terrainVertex;
        float groundScale;
        Vector2 spacing;

        // pontos proximos
        int minVertex, maxVertexX, maxVertexZ, maxVertex;
        Vector3 minVector;

        //rato
        float mouseVelocity;
        float yaw, pitch;
        Vector2 mouseMove;


        public surfaceFollowCameraCls(GraphicsDevice device, Vector2 terrainSize, Vector3[] vertexP, float groundscale, Vector2 spacing, Vector3 pos)
        {
            // determinar aspect ratio, posicao da camera, target e a velocidade de deslocamento da camera
            aspectRatio = (float)device.Viewport.Width / device.Viewport.Height;
            cameraPosition = pos;
            cameraVelocity = 5.5f;

            // rato
            mouseVelocity = MathHelper.ToRadians(2.0f);
            // yaw e pitch da camera
            yaw = (float)Math.PI;
            pitch = 0.0f;
            //deslocacao do rato
            mouseMove = Vector2.Zero;


            // camera direcao base da camera , rotacao e target
            cameraDirection = Vector3.Forward;

            //calcula o target da camera baseado na rotacao e posicao
            cameraTarget = cameraPosition + cameraDirection;

            // get tamanho do terreno e os vertices do terrenho
            this.groundScale = groundscale;
            this.terrainSize = terrainSize;
            terrainVertex = vertexP;
            this.spacing = spacing;



            // espaço entre o chao e a camera
            groundSpacing = 200.0f;
            calculatedHeight = getHeight();

            cameraPosition.Y = calculatedHeight;

            //cria view w projection da camera
            cameraView = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);
            cameraProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1.0f, 100000.0f);

        }

        // calcula vertice

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
            h += groundSpacing;
            return h;

        }




        public void Update(KeyboardState kS, MouseState mS, GraphicsDevice device)
        {
            // recebe a quantidade de pixeis que o rato se moveu nesse update
            mouseMove.X = mS.Position.X - device.Viewport.Width / 2;
            mouseMove.Y = mS.Position.Y - device.Viewport.Height / 2;

            // decrementa o angulo gerado pelo movimento do rato
            yaw -= MathHelper.ToRadians(mouseMove.X) * mouseVelocity;
            pitch -= MathHelper.ToRadians(mouseMove.Y) * mouseVelocity;

            //impede que a camera olhe para la de -85 graus e 85 graus no pitch
            // impede que a camera de voltas de 360 graus em cada eixo
            if (pitch < MathHelper.ToRadians(-85.0f))
                pitch = MathHelper.ToRadians(-85.0f);
            if (pitch > MathHelper.ToRadians(85.0f))
                pitch = MathHelper.ToRadians(85.0f);

            Vector3 rotation = Vector3.Transform(cameraDirection, Matrix.CreateFromYawPitchRoll(yaw, pitch, 0.0f));

            // desloca a posicao na camera em direccao ao target quando carregado nas teclas do numpad 8546
            if (kS.IsKeyDown(Keys.NumPad8))
                cameraPosition += rotation * cameraVelocity;

            if (kS.IsKeyDown(Keys.NumPad5))
                cameraPosition -= rotation * cameraVelocity;


            if (kS.IsKeyDown(Keys.NumPad6))
            {
                // gira o vector direccao para a direita
                Vector3 rotateRight = Vector3.Transform(cameraDirection, Matrix.CreateFromYawPitchRoll(yaw + MathHelper.ToRadians(-90.0f), 0.0f, 0.0f));
                cameraPosition += rotateRight * cameraVelocity;
            }

            if (kS.IsKeyDown(Keys.NumPad4))
            {
                // gira o vector direccao para a esquerda
                Vector3 rotateLeft = Vector3.Transform(cameraDirection, Matrix.CreateFromYawPitchRoll(yaw + MathHelper.ToRadians(90.0f), 0.0f, 0.0f));
                cameraPosition += rotateLeft * cameraVelocity;
            }

            // impede que a camera abandone o terreno
            if (cameraPosition.X < 0.0f)
                cameraPosition.X = 0.0f;
            if (cameraPosition.X > groundScale - 100.0f)
                cameraPosition.X = groundScale - 100.0f;
            if (cameraPosition.Z < 0.0f)
                cameraPosition.Z = 0.0f;
            if (cameraPosition.Z > groundScale - 100.0f)
                cameraPosition.Z = groundScale - 100.0f;

            // recebe a altura calculada e define na posicao da camera
            calculatedHeight = getHeight();
            cameraPosition.Y = calculatedHeight;
            cameraTarget = cameraPosition + rotation;

            //update na matrix view
            cameraView = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);
        }

        // retorna o View
        public Matrix getView
        { get { return cameraView; } }

        //retorna Projection
        public Matrix getProjection
        { get { return cameraProjection; } }

        //retorna Posicao
        public Vector3 getPosition
        { get { return cameraPosition; } set { cameraPosition = value; } }

        //retorna yaw
        public float getYaw
        { get { return yaw; } set { yaw = value; } }

        //retorna pitch
        public float getPitch
        { get { return pitch; } set { pitch = value; } }



    }
}
