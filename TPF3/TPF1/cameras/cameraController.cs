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
    class cameraController
    {
        // variavis das camaras selec
        freeCameraCls freeCam;
        surfaceFollowCameraCls surfaceFollowCam;
        _3rdPersonCameraCls _3rdPersonCam, _3rdPersonCam2;
        int cameraSelector;
        // posiçao das camaras lives inicial
        Vector3 camPos;

        public cameraController(GraphicsDevice device, Vector2 sFterrainSize, Vector3[] sFvertexP, float sFgroundscale, Vector2 sfSpacing, Vector3 _3Target1, float _3direction1)
        {
            // inicia as camera e o selecionador
            cameraSelector = 3;
            camPos = new Vector3(sFgroundscale / 2, 200.0f, sFgroundscale / 2);
            freeCam = new freeCameraCls(device, camPos, sFgroundscale);
            surfaceFollowCam = new surfaceFollowCameraCls(device, sFterrainSize, sFvertexP, sFgroundscale, sfSpacing, camPos);
            _3rdPersonCam = new _3rdPersonCameraCls(device, _3Target1, _3direction1, sFterrainSize, sFvertexP, sfSpacing);
        }

        // Construtor  caso exista mais que 1 tanque
        public cameraController(GraphicsDevice device, Vector2 sFterrainSize, Vector3[] sFvertexP, float sFgroundscale, Vector2 sfSpacing, Vector3 _3Target1, float _3direction1, Vector3 _3Target2, float _3direction2)
        {
            // inicia as camera e o selecionador
            cameraSelector = 3;
            camPos = new Vector3(sFgroundscale / 2, 200.0f, sFgroundscale / 2);
            freeCam = new freeCameraCls(device, camPos, sFgroundscale);
            surfaceFollowCam = new surfaceFollowCameraCls(device, sFterrainSize, sFvertexP, sFgroundscale, sfSpacing, camPos);
            _3rdPersonCam = new _3rdPersonCameraCls(device, _3Target1, _3direction1, sFterrainSize, sFvertexP, sfSpacing);
            _3rdPersonCam2 = new _3rdPersonCameraCls(device, _3Target2, _3direction2, sFterrainSize, sFvertexP, sfSpacing);

        }

        public void Update(KeyboardState kS, MouseState mS, GraphicsDevice device, Vector3 _3Target1, float _3direction1)
        {

            // update nas cameras
            if (cameraSelector == 1)
                freeCam.Update(kS, mS, device);
            if (cameraSelector == 2)
                surfaceFollowCam.Update(kS, mS, device);
            if (cameraSelector == 3)
                _3rdPersonCam.Update(_3Target1, _3direction1);


            // caso o utilizador percione as teclas para mudar de camera muda o selecionador
            if (kS.IsKeyDown(Keys.F1) && cameraSelector != 1 && cameraSelector == 2)
            {
                cameraSelector = 1;
                freeCam.getPosition = surfaceFollowCam.getPosition;
                freeCam.getYaw = surfaceFollowCam.getYaw;
                freeCam.getPitch = surfaceFollowCam.getPitch;
            }
            if (kS.IsKeyDown(Keys.F2) && cameraSelector != 2 && cameraSelector == 1)
            {
                cameraSelector = 2;
                surfaceFollowCam.getPosition = freeCam.getPosition;
                surfaceFollowCam.getYaw = freeCam.getYaw;
                surfaceFollowCam.getPitch = freeCam.getPitch;
            }
            if (kS.IsKeyDown(Keys.F1) && cameraSelector != 1 && cameraSelector == 3)
            {
                cameraSelector = 1;
                freeCam.getPosition = _3rdPersonCam.getPosition;
            }
            if (kS.IsKeyDown(Keys.F2) && cameraSelector != 2 && cameraSelector == 3)
            {
                cameraSelector = 2;
                surfaceFollowCam.getPosition = _3rdPersonCam.getPosition;
            }
            if (kS.IsKeyDown(Keys.F3) && cameraSelector != 3)
                cameraSelector = 3;



        }
        public void Update(KeyboardState kS, MouseState mS, GraphicsDevice device, Vector3 _3Target1, float _3direction1, Vector3 _3Target2, float _3direction2)
        {

            // update nas cameras
            if (cameraSelector == 1)
                freeCam.Update(kS, mS, device);
            if (cameraSelector == 2)
                surfaceFollowCam.Update(kS, mS, device);
            if (cameraSelector == 3)
                _3rdPersonCam.Update(_3Target1, _3direction1);
            if (cameraSelector == 4)
                _3rdPersonCam2.Update(_3Target2, _3direction2);

            // caso o utilizador percione as teclas para mudar de camera muda o selecionador
            // mudar de surface follow para free
            if (kS.IsKeyDown(Keys.F1) && cameraSelector != 1 && cameraSelector == 2)
            {
                cameraSelector = 1;
                freeCam.getPosition = surfaceFollowCam.getPosition;
                freeCam.getYaw = surfaceFollowCam.getYaw;
                freeCam.getPitch = surfaceFollowCam.getPitch;
            }
            // mudar de free para surface
            if (kS.IsKeyDown(Keys.F2) && cameraSelector != 2 && cameraSelector == 1)
            {
                cameraSelector = 2;
                surfaceFollowCam.getPosition = freeCam.getPosition;
                surfaceFollowCam.getYaw = freeCam.getYaw;
                surfaceFollowCam.getPitch = freeCam.getPitch;
            }
            // mudar de tanque 1 para free
            if (kS.IsKeyDown(Keys.F1) && cameraSelector != 1 && cameraSelector == 3)
            {
                cameraSelector = 1;
                freeCam.getPosition = _3rdPersonCam.getPosition;
            }
            // mudar de tanque 1 para surface follow
            if (kS.IsKeyDown(Keys.F2) && cameraSelector != 2 && cameraSelector == 3)
            {
                cameraSelector = 2;
                surfaceFollowCam.getPosition = _3rdPersonCam.getPosition;
            }
            // mudar de tanque 2 para free
            if (kS.IsKeyDown(Keys.F1) && cameraSelector != 1 && cameraSelector == 4)
            {
                cameraSelector = 1;
                freeCam.getPosition = _3rdPersonCam2.getPosition;
            }
            // mudar de tanque 2 para surface follow
            if (kS.IsKeyDown(Keys.F2) && cameraSelector != 2 && cameraSelector == 4)
            {
                cameraSelector = 2;
                surfaceFollowCam.getPosition = _3rdPersonCam2.getPosition;
            }
            if (kS.IsKeyDown(Keys.F3) && cameraSelector != 3)
                cameraSelector = 3;
            if (kS.IsKeyDown(Keys.F4) && cameraSelector != 4)
                cameraSelector = 4;

            float gS1 = _3rdPersonCam.getSetGroundSpacing;
            float gS2 = _3rdPersonCam2.getSetGroundSpacing;

            // ajusta o ground spacing das cameras de 3a pessoa
            if (kS.IsKeyDown(Keys.PageUp) && cameraSelector == 3)
                _3rdPersonCam.getSetGroundSpacing = gS1 + 2.0f;

            if (kS.IsKeyDown(Keys.PageDown) && cameraSelector == 3 && gS1 > 10.0f)
                _3rdPersonCam.getSetGroundSpacing = gS1 - 2.0f;


            if (kS.IsKeyDown(Keys.PageUp) && cameraSelector == 4)
                _3rdPersonCam2.getSetGroundSpacing = gS2 + 2.0f;

            if (kS.IsKeyDown(Keys.PageDown) && cameraSelector == 4 && gS2 > 10.0f)
                _3rdPersonCam2.getSetGroundSpacing = gS2 - 2.0f;


        }

        // retorna a view da camera selecionada
        public Matrix getView()
        {
            if (cameraSelector == 1)
                return freeCam.getView;
            else if (cameraSelector == 2)
                return surfaceFollowCam.getView;
            else if (cameraSelector == 3)
                return _3rdPersonCam.getView;
            else if (cameraSelector == 4)
                return _3rdPersonCam2.getView;
            else
                return Matrix.Identity;
        }

        // retorna a projection da camera selecionada
        public Matrix getProjection()
        {
            if (cameraSelector == 1)
                return freeCam.getProjection;
            else if (cameraSelector == 2)
                return surfaceFollowCam.getProjection;
            else if (cameraSelector == 3)
                return _3rdPersonCam.getProjection;
            else if (cameraSelector == 4)
                return _3rdPersonCam2.getProjection;
            else
                return Matrix.Identity;
        }

        // retorna a camera selecionada
        public int getCamSelector
        { get { return cameraSelector; } }

    }
}
