using Pathfinding_Visulazier.Basic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pathfinding_Visulazier.Labyrinth.Generators
{
    public abstract class AbstarctMazeGenerator
    {
        public AbstarctLabyrinth Labyrinth { get; set; }
        protected int TansitionDensity;
        protected static readonly Random _random = new Random();

        protected readonly List<Vector2D<int>> Empties = new List<Vector2D<int>>();
        public AbstarctMazeGenerator(AbstarctLabyrinth labyrinth)
        {
            Labyrinth = labyrinth;
        }

        public abstract void GenerateMaze(int transitionDensity, int weightedDensity, int weight = 1);

        protected void Add_WeightedPositions(int weightedDensity, int weight = 1)
        {
            int howMuch = Empties.Count * weightedDensity / 100;
            var weighted = Empties.OrderBy(x => Guid.NewGuid()).ToList();
            for (int i=0; i<howMuch; i++)
            {
                Labyrinth.SetWeight(weighted[i], weight);
            }
        }
    }
}
