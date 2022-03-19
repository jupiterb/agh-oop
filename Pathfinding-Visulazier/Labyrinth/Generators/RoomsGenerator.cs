using System;

namespace Pathfinding_Visulazier.Labyrinth.Generators
{
    class RoomsGenerator : AbstarctMazeGenerator
    {
        public RoomsGenerator(AbstarctLabyrinth labyrinth) : base(labyrinth) { }
       
        public override void GenerateMaze(int transitionDensity, int weightedDensity, int weight = 1)
        {
            throw new NotImplementedException();
        }
    }
}
