using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace TPC4
{
    class rainCls
    {

        // particulas
        List<particleCls> particles;
        int particleCount, particlePerUpdate, particleMax, particleCreateOnSecond;

        // forças
        Vector3 gravityVector, windVector;

        // spawn particulas
        Vector3 center, particlePos, particleRandomDirection;
        float randomAngle, randomDistance, randomResistence;
        Random rand;
        int radius;


        public rainCls(GraphicsDevice device, int r, Vector3 center)
        {
            // circulo de spawn
            this.radius = r;
            this.center = center;
            rand = new Random();
            particleRandomDirection = Vector3.Backward;

            // forças
            windVector = new Vector3(0.3f, -1.0f, 0.3f) * 5.0f;
            //windVector = Vector3.Zero;
            gravityVector = (Vector3.Down * 9.8f);
            particles = new List<particleCls>();

            // quantidade
            particleMax = 900;
            particlePerUpdate = 14;
        }

        public void Update(GraphicsDevice device)
        {
            // remove todas as particulas que atingiram o chao
            particles.RemoveAll(s => s.getDeathState == true);

            // calcula quantas particulas estao a ser desenhadas
            particleCount = particles.Count;

            // o proximo update passe o limite de particulas
            if (particleCount + particlePerUpdate < particleMax)
            {
                // cria o numero de particulas defenido como particulas por update
                for (int i = 0; i < particlePerUpdate; i++)
                {
                    // gera o um angulo aleatorio para determinar a posiçao da particula, a distancia do centro e uma resistencia aleatoria
                    randomAngle = rand.Next(0, 360);
                    randomDistance = (float)rand.Next(1, radius);
                    randomResistence = (float)rand.NextDouble() * (1.0f - 0.85f) + 0.85f;
                    // gera um vector com uma  direclao aleatoria
                    Vector3 randDirection = new Vector3(rand.Next(-3, 3), rand.Next(-3, -1), rand.Next(-3, 3));

                    // calcula a posicao do da particula
                    particlePos = center + (Vector3.Transform(particleRandomDirection * randomDistance, Matrix.CreateRotationY(MathHelper.ToRadians(randomAngle))));
                    // adiciona a particula nova
                    particles.Add(new particleCls(device, gravityVector * randomResistence, windVector, randDirection ,particlePos));
                }
            }
            // caso nao aconteça
            
            // actualiza as particulas
            if (particles.Count != 0)
                foreach (particleCls particle in particles)
                    particle.Update();
        }

        public void Draw(GraphicsDevice device, Matrix view, Matrix projection)
        {
            // desenha as particulas
            if (particles.Count != 0)
            {
                foreach (particleCls particle in particles)
                    particle.Draw(device, view, projection);
            }
        }

        public int getParticleCount
        { get { return particles.Count; } }

    }
}
