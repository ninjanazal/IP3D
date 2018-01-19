using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;


namespace TPF1
{
    class BoidCls
    {

        // info da AI
        float angularVelocity, velocity;
        float distance;
        string state;

        Vector3 positionToTarget;
        float angleToIdle, angleToSeek, angleToFlee;
        Vector3 orientationLeftHelper, orientationRightHelper;
        float distanceLeftHelper, distanceRightHelper;

        String moveState;

        // tank
        float tankYaw;
        Vector3 tankPosition;

        // seek
        Vector3 seekTargetPosition;



        // random
        Random rand;

        Vector3 idleRandomtarget, aiIdleTargetPosition;
        Vector3 aiCalculatedDirection;
        int lastIdleTarget;


        public BoidCls()
        {
            rand = new Random();
            lastIdleTarget = 0;
            velocity = 3.0f;
        }

        public void Update(Vector3 position, Vector3 targetPosition, Matrix aiDirection, float yaw, int aiHp, int totalSeconds)
        {
            distance = Vector3.Distance(position, targetPosition);
            angularVelocity = MathHelper.ToRadians(0.5f);

            moveState = "";

            tankYaw = yaw;
            tankPosition = position;
            tankPosition.Y = 0;

            aiCalculatedDirection = aiDirection.Backward;
            aiCalculatedDirection.Y = 0;
            aiCalculatedDirection.Normalize();

            seekTargetPosition = targetPosition;
            seekTargetPosition.Y = 0;

            // define o estado em que o boid esta
            if (distance >= 4500)
                state = "idle";
            if (distance < 4500 && aiHp > 25)
                state = "seek";
            if (distance < 4500 && aiHp <= 25)
                state = "flee";

            // -------------- idle
            if (state == "idle")
            {
                velocity = 3.0f;

                // caso o tempo para atingir a posiçao idle tenha terminado é definida uma nova posiçao
                if (lastIdleTarget < totalSeconds || Vector3.Distance(tankPosition, aiIdleTargetPosition) < 5.0f)
                {
                    // tempo ate nova posicao idle seja calculada
                    lastIdleTarget = totalSeconds + 15;

                    // centro da circunferencia á frente do tanque para encontrar nova posicao
                    idleRandomtarget = tankPosition + this.aiCalculatedDirection * 200f;

                    // posiçao alvo enquanto idle
                    // centro da circunferencia calculada + vector de tamanho 10(raio) rodado aleatoriamente 
                    aiIdleTargetPosition = idleRandomtarget + Vector3.Transform(Vector3.Backward * 100f, Matrix.CreateRotationY(MathHelper.ToRadians(rand.Next(-90, 90))));
                }

                // vector que indica a direcao desejada, vector que liga a posicao actual com a posicao desejada
                positionToTarget = aiIdleTargetPosition - tankPosition;
                positionToTarget.Normalize();

                // calcula o angulo entre a direcao do tanque e a direcao nessecaria para atingir o alvo
                angleToIdle = (float)Math.Acos(Vector3.Dot(aiCalculatedDirection, positionToTarget));

                orientationLeftHelper = Vector3.Transform(aiCalculatedDirection, Matrix.CreateRotationY(angularVelocity));
                orientationRightHelper = Vector3.Transform(aiCalculatedDirection, Matrix.CreateRotationY(-angularVelocity));

                distanceLeftHelper = Vector3.Distance(orientationLeftHelper, positionToTarget);
                distanceRightHelper = Vector3.Distance(orientationRightHelper, positionToTarget);

                // calcula para que lado o tanque deve rodar para apontar naa direcao do alvo
                if (distanceLeftHelper < distanceRightHelper && angleToIdle > angularVelocity)
                    angularVelocity = MathHelper.ToRadians(0.4f);
                else if (distanceLeftHelper > distanceRightHelper && angleToIdle > angularVelocity)
                    angularVelocity = MathHelper.ToRadians(-0.4f);
                else if (distanceLeftHelper < distanceRightHelper && angleToIdle < angularVelocity)
                    angleToIdle = 0;
                else if (distanceLeftHelper > distanceRightHelper && angleToIdle < angularVelocity)
                    angleToIdle = 0;

                // roda o tanque aque que fique voltado para a posiçao desejada
                if (angleToIdle != 0)
                {
                    if (angularVelocity > 0)
                        moveState = "right";
                    if (angularVelocity < 0)
                        moveState = "left";

                    tankYaw += angularVelocity;
                }
                // assim que o tanque estiver voltado para o alvo, avanca
                if (angleToIdle == 0)
                {
                    moveState = "move";
                    tankPosition += positionToTarget * velocity;
                }
            }

            // -------------- Seek
            if (state == "seek")
            {

                // vector que indica a direcao desejada, vector que liga a posicao actual com a posicao desejada
                positionToTarget = seekTargetPosition - tankPosition;
                positionToTarget.Normalize();

                // calcula o angulo entre a direcao do tanque e a direcao necessaria para atingir o ponto
                angleToSeek = (float)Math.Acos(Vector3.Dot(aiCalculatedDirection, positionToTarget));

                orientationLeftHelper = Vector3.Transform(aiCalculatedDirection, Matrix.CreateRotationY(angularVelocity));
                orientationRightHelper = Vector3.Transform(aiCalculatedDirection, Matrix.CreateRotationY(-angularVelocity));

                distanceLeftHelper = Vector3.Distance(orientationLeftHelper, positionToTarget);
                distanceRightHelper = Vector3.Distance(orientationRightHelper, positionToTarget);

                // calcula para que lado o tanque deve rodar para apontar naa direcao do alvo
                if (distanceLeftHelper < distanceRightHelper && angleToSeek > angularVelocity)
                    angularVelocity = MathHelper.ToRadians(0.4f);
                else if (distanceLeftHelper > distanceRightHelper && angleToSeek > angularVelocity)
                    angularVelocity = MathHelper.ToRadians(-0.4f);
                else if (distanceLeftHelper < distanceRightHelper && angleToSeek < angularVelocity)
                    angleToSeek = 0;
                else if (distanceLeftHelper > distanceRightHelper && angleToSeek < angularVelocity)
                    angleToSeek = 0;

                // roda o tanque aque que fique voltado para a posiçao desejada

                if (angleToSeek != 0)
                {
                    if (angularVelocity > 0)
                        moveState = "right";
                    if (angularVelocity < 0)
                        moveState = "left";

                    tankYaw += angularVelocity;
                }

                if (angleToSeek == 0 && Vector3.Distance(tankPosition, seekTargetPosition) > 250.0f)
                {
                    moveState = "move";
                    tankPosition += positionToTarget * velocity;
                }
            }

            // -------------- Flee
            if (state == "flee")
            {
                // define as velocidades para os estados
                velocity = 5.0f;
                angularVelocity = MathHelper.ToRadians(0.9f);

                // calcula o vector direcionado para o oposto da posicao do target
                positionToTarget = tankPosition - seekTargetPosition;
                positionToTarget.Normalize();

                // calcula o angulo entre o vector direcao do tanque e o vector de flee
                angleToFlee = (float)Math.Acos(Vector3.Dot(aiCalculatedDirection, positionToTarget));

                // determina para qual dos landos deve girar para demorar o menos possivel
                orientationLeftHelper = Vector3.Transform(aiCalculatedDirection, Matrix.CreateRotationY(angularVelocity));
                orientationRightHelper = Vector3.Transform(aiCalculatedDirection, Matrix.CreateRotationY(-angularVelocity));

                distanceLeftHelper = Vector3.Distance(orientationLeftHelper, positionToTarget);
                distanceRightHelper = Vector3.Distance(orientationRightHelper, positionToTarget);

                // calcula para que lado o tanque deve rodar para apontar naa direcao do alvo
                if (distanceLeftHelper < distanceRightHelper && angleToFlee > angularVelocity)
                    angularVelocity = MathHelper.ToRadians(0.8f);
                else if (distanceLeftHelper > distanceRightHelper && angleToFlee > angularVelocity)
                    angularVelocity = MathHelper.ToRadians(-0.8f);
                else if (distanceLeftHelper < distanceRightHelper && angleToFlee < angularVelocity)
                    angleToFlee = 0;
                else if (distanceLeftHelper > distanceRightHelper && angleToFlee < angularVelocity)
                    angleToFlee = 0;

                // roda o tanque aque que fique voltado para a posiçao desejada

                if (angleToFlee != 0)
                {
                    if (angularVelocity > 0)
                        moveState = "right";
                    if (angularVelocity < 0)
                        moveState = "left";

                    tankYaw += angularVelocity;
                }
                // move o tanque aquando este apontar para a posicao alvo
                if (angleToFlee == 0)
                {
                    moveState = "move";
                    tankPosition += positionToTarget * velocity;
                }
            }

        }

        // retorna em que estado o boid encontra-se
        public string getState
        { get { return state; } }


        // ---------------------------- IDLE
        // retorna o yaw calculado
        public float getCalculatedYaw
        { get { return tankYaw; } }

        //retorna a posicao a posicao calaculada do tanque
        public Vector3 getCalculatedPosition
        { get { return tankPosition; } }

        // retorna qual é o tipo de movimento que o tanque executou no ultimo update
        public string getMoveState
        { get { return moveState; } }


    }
}
