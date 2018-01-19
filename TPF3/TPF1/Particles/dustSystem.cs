using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TPF1
{
    class dustSystem
    {
        // mdelo da particula e lista de particulas
        Model dustModelParticle;
        List<dustParticle> dustParticles;

        // quantidade de particulas por update
        int particlePerUpdate;

        //random
        Random rand;
        Vector3 particleSpawnCenter, particleStartPosition, particleRandomOrientation;


        public dustSystem(Model dustModel)
        {
            // inicia o sistema de particulas
            dustParticles = new List<dustParticle>();
            this.dustModelParticle = dustModel;
            this.particlePerUpdate = 1;
            rand = new Random();
        }

        // Update
        public void Update(bool isMoving, Vector3 tankPosition, Matrix tankNormal, int totalSecondsTime)
        {
            //remove todas as particulas que atingirem o tempo maximo de vida
            dustParticles.RemoveAll(s => s.getDeathState == true);

            // faz o update de todas as particulas 
            foreach (dustParticle particle in dustParticles)
                particle.Update(totalSecondsTime);

            // cria particulas quando existir movimento dos tanques
            if (isMoving)
                for (int i = 0; i < particlePerUpdate; i++)
                {
                    // calcula o centro da linha de spawn de particulas
                    particleSpawnCenter = tankPosition + tankNormal.Forward + tankNormal.Up * rand.Next(1,20);

                    // na linha anteriormente calculada, aleatoriamente define um ponto
                    particleStartPosition = particleSpawnCenter + (tankNormal.Right * rand.Next(-70,70));

                    // determina a orientaçao da particula
                    particleRandomOrientation = Vector3.Transform(tankNormal.Forward, Matrix.CreateFromAxisAngle(tankNormal.Right, MathHelper.ToRadians(rand.Next(5, 45))));
                    particleRandomOrientation.Normalize();

                    dustParticles.Add(new dustParticle(dustModelParticle, particleRandomOrientation, particleStartPosition, totalSecondsTime, rand.Next(0,2)));
                }
        }

        // Draw
        public void Draw(GraphicsDevice device, Matrix view, Matrix projection, lightControllerCls lightCtrl)
        {
            // desenha cada uma das particulas existentes
            foreach (dustParticle particle in dustParticles)
                particle.Draw(device, view, projection, lightCtrl);
        }

        public int getParticleCount
        { get { return dustParticles.Count; } }
    }
}
