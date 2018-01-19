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
    class freeCameraCls
    {
        // variaveis para controlo de posicao e target da camera
        Matrix cameraView, cameraProjection;
        float aspectRatio, cameraVelocity;
        Vector3 cameraPosition, cameraTarget;

        //terreno
        float groundScale;
        //rato
        float mouseVelocity;
        Vector3 cameraDirection;
        float yaw, pitch;
        Vector2 mouseMove;


        public freeCameraCls(GraphicsDevice device, Vector3 pos, float freeGroundScale)
        {
            // determinar aspect ratio, posicao da camera, target e a velocidade de deslocamento da camera
            aspectRatio = (float)device.Viewport.Width / device.Viewport.Height;
            cameraPosition = pos;
            cameraVelocity = 7.0f;

            this.groundScale = freeGroundScale;

            //rato
            // yaw e pitch da camera
            yaw = (float)Math.PI;
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
            cameraProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1.0f, 100000.0f);

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
            cameraTarget = cameraPosition + rotation;

            //controlo camera
            if (kS.IsKeyDown(Keys.NumPad8))
                cameraPosition += rotation * cameraVelocity;

            if (kS.IsKeyDown(Keys.NumPad5))
                cameraPosition -= rotation * cameraVelocity;

            if (kS.IsKeyDown(Keys.NumPad6))
            {
                float yPosition = cameraPosition.Y;
                Vector3 rotateRight = Vector3.Transform(cameraDirection, Matrix.CreateFromYawPitchRoll(yaw + MathHelper.ToRadians(-90.0f), 0.0f, 0.0f));
                rotateRight.Normalize();
                cameraPosition += rotateRight * cameraVelocity;

                //impede que a altura da camera se altere com a deslocacao horizontal
                cameraPosition.Y = yPosition;

            }
            if (kS.IsKeyDown(Keys.NumPad4))
            {
                float yPosition = cameraPosition.Y;
                Vector3 rotateLeft = Vector3.Transform(cameraDirection, Matrix.CreateFromYawPitchRoll(yaw + MathHelper.ToRadians(90.0f), 0.0f, 0.0f));
                rotateLeft.Normalize();
                cameraPosition += rotateLeft * cameraVelocity;

                //impede que a altura da camera se altere com a deslocacao horizontal
                cameraPosition.Y = yPosition;
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
