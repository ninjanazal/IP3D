using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPC3
{
    class lightCls
    {
        // variaveis de iluminaçao
        Vector3 _emissiveColor;
        Vector3 _ambienteLightColor, _ambienteDifuse, _ambienteSpecularColor;
        Vector3 _directionalDirection, _directionalDifuseColor, _directionalSpecular;
        float _ambienteSpecularPower;

        public lightCls()
        {
            // define valores para a iluminaçao
            _emissiveColor = new Vector3(0.1f, 0.1f, 0.1f);

            _ambienteLightColor = Color.SkyBlue.ToVector3() * 0.2f;
            _ambienteDifuse = Color.White.ToVector3() * 0.5f;
            _ambienteSpecularColor = Color.White.ToVector3() * 5.5f;
            _ambienteSpecularPower = 150f;

            _directionalDirection = new Vector3(0.0f, -1.0f, 1.0f);
            _directionalDifuseColor = Color.White.ToVector3() * 1.2f;
            _directionalSpecular = Color.White.ToVector3();

        }

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
