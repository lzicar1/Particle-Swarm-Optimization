using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace ParticleSwarm{

class Swarm
{
    /*
    A swarm is a collection of particles
    Class variables:
        particles: the particles in the swarm
        bestPosition: the best position the swarm has been in
        bestValue: the value of the best position the swarm has been in
    */

    public Particle[] particles;
    public double[] bestPosition;
    public int historySize = 100;
    public double[,,] positionHistory;
    public double bestValue;
    public int dimension;
    public int iteration = 0;

    public Swarm(
        int particleCount,
        int dimension,
        double[] lowerBounds,
        double[] upperBounds,
        Random random
    )
    {
        /*
        Constructor for the swarm class
        Parameters:
            particleCount: the number of particles in the swarm
            dimension: the dimension of the search space
            lowerBounds: the lower bounds of the search space
            upperBounds: the upper bounds of the search space
            random: the random number generator
        Particles are initialized with random positions and velocities
        */
        particles = new Particle[particleCount];
        positionHistory = new double[particleCount, dimension + 1, historySize]; // number of recalled positions is 10

        for (int i = 0; i < particleCount; i++)
        {
            particles[i] = new Particle(dimension);

            for (int j = 0; j < dimension; j++)
            {
                particles[i].position[j] =
                    lowerBounds[j] + (upperBounds[j] - lowerBounds[j]) * random.NextDouble();
                particles[i].velocity[j] = random.NextDouble();
                particles[i].bestPosition[j] = particles[i].position[j];
            }
        }

        bestPosition = new double[dimension];
        bestValue = double.MaxValue;
        dimension = dimension;
    }

    public void UpdateBest()
    {
        /*
        Update the best position and value of the swarm
        */

        foreach (var particle in particles)
        {
            if (particle.bestValue < bestValue)
            {
                bestValue = particle.bestValue;
                bestPosition = particle.bestPosition;
            }
        }
    }

    public void UpdateVelocity(double w, double c1, double c2, Random random)
    {
        /*
        Update the velocity of each particle in the swarm
        */

        foreach (var particle in particles)
        {
            for (int i = 0; i < particle.velocity.Length; i++)
            {
                particle.velocity[i] =
                    w * particle.velocity[i]
                    + c1 * random.NextDouble() * (particle.bestPosition[i] - particle.position[i])
                    + c2 * random.NextDouble() * (bestPosition[i] - particle.position[i]);
            }
        }
    }

    public void UpdatePosition()
    {
        /*
        Update the position of each particle in the swarm
        */

        foreach (var particle in particles)
        {
            for (int i = 0; i < particle.position.Length; i++)
            {
                particle.position[i] += particle.velocity[i];
            }
        }
    }

    public void UpdateValue(Func<double[], double> evaluate)
    {
        /*
        Update the value of each particle in the swarm
        */

        foreach (var particle in particles)
        {
            particle.value = evaluate(particle.position);

            if (particle.value < particle.bestValue)
            {
                particle.bestValue = particle.value;
                particle.bestPosition = particle.position;
            }
        }
    }

    public void UpdateHistory()
    {
        /*
        Update the history of positions and values of each particle in the swarm
        */


        for (int i = 0; i < particles.Length; i++)
        {
            positionHistory[i, 0, iteration % historySize] = particles[i].position[0];
            positionHistory[i, 1, iteration % historySize] = particles[i].position[1];
            positionHistory[i, 2, iteration % historySize] = particles[i].value;
        }

        iteration++;
    }

    public void Update(
        Func<double[], double> evaluate,
        double w,
        double c1,
        double c2,
        Random random
    )
    {
        /*
        Update the swarm
        */

        UpdateValue(evaluate);
        UpdateBest();
        UpdateVelocity(w, c1, c2, random);
        UpdatePosition();
        UpdateHistory();
    }
}

}
