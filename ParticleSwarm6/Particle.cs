
namespace ParticleSwarm
{
    class Particle{
        /*
        A particle is a point in the search space with a velocity and a position
        Class variables:
            position: the position of the particle in the search space
            velocity: the velocity of the particle in the search space
            bestPosition: the best position the particle has been in
            bestFitness: the fitness of the best position the particle has been in
        */

        public double[] position;
        public double[] velocity;
        public double[] bestPosition;
        public double bestValue;
        public double value;

        public Particle(int dimension){
            /*
            Constructor for the particle class
            Parameters:
                dimension: the dimension of the search space
            */

            position = new double[dimension];
            velocity = new double[dimension];
            bestPosition = new double[dimension];
            bestValue = double.MaxValue;
            value = double.MaxValue;

            for (int i = 0; i < dimension; i++){
                position[i] = 0;
                velocity[i] = 0;
                bestPosition[i] = 0;
            }
        }

        public override string ToString(){
            /*
            Returns a string representation of the particle
            */
            return string.Format("Position: {0}, Velocity: {1}, BestPosition: {2}, BestValue: {3}, Value: {4}", 
                string.Join(",", position), string.Join(",", velocity), 
                string.Join(",", bestPosition), bestValue, value);
        }
    }
}