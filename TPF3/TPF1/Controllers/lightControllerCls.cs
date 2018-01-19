using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPF1
{
    class lightControllerCls
    {
        // light info
        Vector3 _emissiveColor;
        Vector3 _ambienteLightColor, _ambienteDifuse, _ambienteSpecularColor;
        Vector3 _directionalDirection, _directionalDifuseColor, _directionalSpecular;
        float _ambienteSpecularPower;

        public lightControllerCls()
        {
            // define valores para a iluminaçao
            _emissiveColor = new Vector3(0.1f, 0.1f, 0.1f);

            _ambienteLightColor = Color.White.ToVector3() * 1.8f;
            _ambienteDifuse = Color.White.ToVector3() * 0.05f;
            _ambienteSpecularColor = Color.White.ToVector3() * 1.5f;
            _ambienteSpecularPower = 200f;

            _directionalDirection = new Vector3(0.5f, -0.2f, 0.5f);
            _directionalDirection.Normalize();
            _directionalDirection *= 4.0f;
            _directionalDifuseColor = Color.White.ToVector3() * 2.0f;
            _directionalSpecular = Color.White.ToVector3() * 0.45f;
        }

        // permite aceder aos valors da luz
        public Vector3 getEmissive
        { get { return _emissiveColor; } }
        public Vector3 getAColor
        { get { return _ambienteLightColor; } }
        public Vector3 getADifuse
        { get { return _ambienteDifuse; } }
        public Vector3 getASpecColor
        { get { return _ambienteSpecularColor; } }
        public float getASpecularPower
        { get { return _ambienteSpecularPower; } }
        public Vector3 getDDirection
        { get { return _directionalDirection; } }
        public Vector3 getDDifuse
        { get { return _directionalDifuseColor; } }
        public Vector3 getDSpecular
        { get { return _directionalSpecular; } }

    }
}
