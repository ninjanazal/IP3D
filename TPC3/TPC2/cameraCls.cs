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
    class cameraCls
    {
        // variaveis para controlo de posicao e target da camera
        Matrix cameraView, cameraProjection;
        float aspectRatio, cameraVelocity, cameraVerticalVelocity;
        Vector3 cameraPosition, cameraTarget;

        //rato
        float mouseVelocity;
        Vector3 cameraDirection;
        float yaw, pitch;
        Vector2 mouseMove;


        public cameraCls(GraphicsDevice device)
        {
            // determinar aspect ratio, posicao da camera, target e a velocidade de deslocamento da camera
            aspectRatio = (float)device.Viewport.Width / device.Viewport.Height;
            cameraPosition = new Vector3(1.0f, 34.0f, 55.0f);
            cameraVelocity = 1.5f;
            cameraVerticalVelocity = 1.0f;

            //rato
            // yaw e pitch da camera
            yaw = 0.0f;
            pitch = 0.0f;
            //deslocacao do rato
            mouseMove = Vector2.Zero;

            // camera direcao base da camera , rotacao e target
            cameraDirection = Vector3.Forward;

            //calcula o target da camera baseado na rotacao e posicao
            cameraTarget = cameraDirection;

            mouseVelocity = MathHelper.ToRadians(2.0f);

            //cria view w projection da camera
            cameraView = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);
            cameraProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1.0f, 500.0f);

        }

        public void Update(KeyboardState kS, MouseState mS, GraphicsDevice device)
        {
            mouseMove.X = mS.Position.X - device.Viewport.Width / 2;
            mouseMove.Y = mS.Position.Y - device.Viewport.Height / 2;

            yaw -= MathHelper.ToRadians(mouseMove.X) * mouseVelocity;
            pitch -= MathHelper.ToRadians(mouseMove.Y) * mouseVelocity;

            //impede que a camera olhe para la de -85 graus e 85 graus no pitch
            if (pitch < MathHelper.ToRadians(-85.0f))
                pitch = MathHelper.ToRadians(-85.0f);
            if (pitch > MathHelper.ToRadians(85.0f))
                pitch = MathHelper.ToRadians(85.0f);

            Vector3 rotation = Vector3.Transform(cameraDirection, Matrix.CreateFromYawPitchRoll(yaw, pitch, 0.0f));
            rotation.Normalize();

            //controlo camera
            if (kS.IsKeyDown(Keys.W))
                cameraPosition += rotation * cameraVelocity;
            
            if (kS.IsKeyDown(Keys.S))
                cameraPosition -= rotation * cameraVelocity;
            
            if (kS.IsKeyDown(Keys.D))
            {
                float yPosition = cameraPosition.Y;
                Vector3 rotateRight = Vector3.Transform(rotation, Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(-90.0f), 0.0f, 0.0f));
                rotateRight.Normalize();
                cameraPosition += rotateRight * cameraVelocity;

                //impede que a altura da camera se altere com a deslocacao horizontal
                cameraPosition.Y = yPosition;

            }
            if (kS.IsKeyDown(Keys.A))
            {
                float yPosition = cameraPosition.Y;
                Vector3 rotateLeft = Vector3.Transform(rotation, Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(90.0f), 0.0f, 0.0f));
                rotateLeft.Normalize();
                cameraPosition += rotateLeft * cameraVelocity;

                //impede que a altura da camera se altere com a deslocacao horizontal
                cameraPosition.Y = yPosition;
            }

            if (kS.IsKeyDown(Keys.LeftShift))
                cameraPosition += Vector3.Up * cameraVerticalVelocity;
            
            if (kS.IsKeyDown(Keys.LeftControl))
                cameraPosition += Vector3.Down * cameraVerticalVelocity;
            
            cameraTarget = cameraPosition + rotation;

            //update na matrix view
            cameraView = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);
            Console.WriteLine(cameraPosition);
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

        //retorna Target
        public Vector3 getTarget
        { get { return cameraTarget; } }
    }
}
