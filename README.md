# Particle Swarm Optimization

- C# repository for computation and visualisation of global optima using a method called Particle Swarm Optimization.
- Code is written in .NET 6.0
- `Optimizer` class supports implicit plotting for arbitrary 3D functions written in C#
    -    current options are `rastrigin`, `saddle` and `parabolla`
- `Particle` and `Swarm` classes are supporting PSO optimization operations
- `Program` has command-line-interface and allows running and visualising the whole process

**What is Particle Swarm Optimization?**
> In computational science, **particle swarm optimization** (PSO) is a computational method that optimizes a problem by iteratively trying to improve a candidate solution with regard to a given measure of quality. It solves a problem by having a **population of candidate solutions**, here dubbed particles, and moving these particles around in the **search-space** according to simple mathematical formula over the **particle's position and velocity.** Each particle's movement is influenced by its local best known position, but is also guided toward the best known positions in the search-space, which are updated as better positions are found by other particles. This is expected to **move the swarm** toward the **best solutions**. [Wikipedia - Particle Swarm Optimization](https://en.wikipedia.org/wiki/Particle_swarm_optimization?oldformat=true) 

## Example visualisation

**Input Function**
- Input funstion in class `Optimizer`

```csharp
public static double Rastrigin3D(double[] xy){
    /*
    Rastrigin function in 3D
    */

    double x = xy[0] - 30;
    double y = xy[1] - 30;
    return 0.2* Math.Pow(x, 2) - 50 * Math.Cos(0.3 * Math.PI * x) + 0.2*Math.Pow(y, 2) - 50 * Math.Cos(0.3 * Math.PI * y) + 20; 
}
```
<image src="assets/rastrigin.png">

Following visualisation produced by (more info in CLI Usage):
```
dotnet run -- --function rastrigin --iteration 100 --verbosity --plot --num-plots 15 --history
```

<video controls="" width="800" height="500" muted="" loop="true" autoplay="true">
<source src="assets/rastrigin_pso.gif.mp4" type="video/mp4">
</video>

## CLI Usage

```
Description:
  Particle swarm optimization of a function.
  Outputs global minimum and optionally visualises the process in the browser.

Usage:
  ParticleSwarm6 [options]

Options:
  -f, --function <function>    The function to optimize. Options are: 'rastrigin', 'parabolla', 'saddle'. [default: rastrigin]
  -i, --iteration <iteration>  The target number of iterations. [default: 100]
  -p, --particle <particle>    The number of particles in the swarm. [default: 30]
  -v, --verbosity              The verbosity level. Options are: 'True', 'False', . [default: False]
  -t, --plot                   Whether to plot the optimization process. Options are: 'True', 'False', . [default: False]
  -n, --numPlots <numPlots>    The number of plots to show. [default: 5]
  --version                    Show version information
  -?, -h, --help               Show help and usage information
```

**Input Command**
```
dotnet run -- --function rastrigin --iteration 100 --verbosity
```

**Output**
```
Initializing swarm of 30 particles
Initializing rastrigin function
Optimization start...
Iteration: 0
Best value: -7.401020265023011
Best position: 28.8170515240461, 29.053994542533445
Iteration: 1
Best value: -32.96320284892921
Best position: 28.932955892282305, 29.237190687893552
Iteration: 2
Best value: -44.05774256297073
Best position: 28.99090807640041, 29.328788760573605
Iteration: 3
Best value: -49.072409107951074
Best position: 29.01988416845946, 29.374587796913634
...
```

## Particle
- a particle has n-dimensional position and velosity (which controls attractive and repulsive forces)
- best position and best value is saved which affects the velocity and global swarm behaviour
- value is updated in each step

```csharp
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

```

## Swarm
- swarm is a collection of particles
```csharp
public Swarm(
        int particleCount,
        int dimension,
        double[] lowerBounds,
        double[] upperBounds,
        Random random
)
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

```

## Optimization

**PSO Algorithm**
- The main particle swarm optimization loop in `Program.cs` in simplified version
- Firstwall swarm is initialized with defined number of particles
- In every iteration are updated values and positions of particles, changing of velocities attracts particles to global optima in n-dimensional space

```csharp
Console.WriteLine("Initializing Swarm");
Swarm swarm = new Swarm(num_particles, num_dimensions);
Console.WriteLine("Optimization start...");
for (int i = 0; i < iteration; i++){
    swarm.UpdateValue(optimizationFunction); // updating values based on optimization functions
    swarm.UpdateBest(); // updating best global swarm value
    swarm.UpdateVelocity(0.5, 0.01, 0.03, random); // updating velocities based on global and local coefficients
    swarm.UpdatePosition(); // updating positions of particles
    swarm.UpdateHistory(); // optinally tracking particle history
}
```

**Velocity**
- `w` is the momentum of particle: weight affecting the change of velocity based on previous velocity
- `c1` is local coefficient of the velocity updated based on best position which this particle remembers
- `c2` is global coefficient of the velocity updated based on best position of the whole swarm
```csharp
particle.velocity[i] =
    w * particle.velocity[i]
    + c1 * random.NextDouble() * (particle.bestPosition[i] - particle.position[i])
    + c2 * random.NextDouble() * (bestPosition[i] - particle.position[i]);
```


## Pseudocode
Let S be the number of particles in the swarm, each having a position xi ∈ ℝn in the search-space and a velocity vi ∈ ℝn. Let pi be the best known position of particle i and let g be the best known position of the entire swarm. A basic PSO algorithm is then:
```
for each particle i = 1, ..., S do
    Initialize the particle's position with a uniformly distributed random vector: xi ~ U(blo, bup)
    Initialize the particle's best known position to its initial position: pi ← xi
    if f(pi) < f(g) then
        update the swarm's best known position: g ← pi
    Initialize the particle's velocity: vi ~ U(-|bup-blo|, |bup-blo|)
while a termination criterion is not met do:
    for each particle i = 1, ..., S do
        for each dimension d = 1, ..., n do
            Pick random numbers: rp, rg ~ U(0,1)
            Update the particle's velocity: vi,d ← w vi,d + φp rp (pi,d-xi,d) + φg rg (gd-xi,d)
        Update the particle's position: xi ← xi + vi
        if f(xi) < f(pi) then
            Update the particle's best known position: pi ← xi
            if f(pi) < f(g) then
                Update the swarm's best known position: g ← pi
```

## Repository structure

```
├── LICENSE.md
├── ParticleSwarm6
│   ├── Optimizer.cs
│   ├── Particle.cs
│   ├── ParticleSwarm6.csproj
│   ├── Program.cs
│   ├── Swarm.cs
│   ├── bin
│   └── obj
├── README.md
└── assets
    ├── rastrigin.png
    ├── rastrigin_pso.gif
    └── rastrigin_pso.gif.mp4
```

## References
- [Wikipedia - Particle Swarm Optimization](https://en.wikipedia.org/wiki/Particle_swarm_optimization?oldformat=true) 
- [Martin Pilat – Algorithms Inspired by Nature](https://martinpilat.com/cs/prirodou-inspirovane-algoritmy/hejna-kolonie)


## Future updates
- Add the possibility of continuous animation
- Add more functions
- Graphing of loss function
- More PSO options


## Disclaimer
This repository came into being as a school homework ("zápočtový program" at Charles university Programming II course) and implements the main algorithm from scratch: it not intended nor optimized enough for industrial applications.