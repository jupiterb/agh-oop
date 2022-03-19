using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pathfinding_Visulazier.Basic;

namespace Pathfinding_Visulazier.Labyrinth.Generators
{
    public class SpaghettiGenerator : AbstarctMazeGenerator
    {
        public SpaghettiGenerator(AbstarctLabyrinth labyrinth) : base(labyrinth) { }

        private void WallUp()
        {
            for (int i = AbstarctLabyrinth.UpperLeft.X; i<Labyrinth.LowerRight.X; i++)
            {
                for (int j = AbstarctLabyrinth.UpperLeft.Y; j < Labyrinth.LowerRight.Y; j++)
                {
                    Labyrinth.SetStatus(new Vector2D<int>(i, j), NodeStatus.Wall);
                }
            }
        }

        private bool IsSurrounded(Vector2D<int> position, Vector2D<int> expect)
        {
            foreach (Vector2D<int> neighbour in Labyrinth.GetNeigboursOf(position))
            {
                if (Labyrinth.OnBoard(neighbour) && !neighbour.Equals(expect) 
                    && Labyrinth.GetStatus(neighbour) != NodeStatus.Wall && _random.Next(TansitionDensity) != 0)
                {
                    return false;
                }
            }
            return true;
        }

        private void MakeCorridor(Vector2D<int> position)
        {
            Labyrinth.SetStatus(position, NodeStatus.Empty);
            Empties.Add(position);
            var shuffledNeighbours = Labyrinth.GetNeigboursOf(position).OrderBy(a => Guid.NewGuid()).ToList();
            foreach (Vector2D<int> neighbour in shuffledNeighbours)
            {
                if (Labyrinth.OnBoard(neighbour) && IsSurrounded(neighbour, position) && Labyrinth.GetStatus(neighbour) == NodeStatus.Wall)
                {
                    MakeCorridor(neighbour);
                }
            }
        }

        private void OpenNode(Vector2D<int> position)
        {
            if (Labyrinth.GetStatus(position) != NodeStatus.Empty && Labyrinth.OnBoard(position))
            {
                Labyrinth.SetStatus(position, NodeStatus.Empty);
                Empties.Add(position);
                foreach (Vector2D<int> neighbour in Labyrinth.GetNeigboursOf(position))
                {
                    OpenNode(neighbour);
                }
            }
        }

        public override void GenerateMaze(int transitionDensity, int weightedDensity, int weight = 1)
        {
            TansitionDensity = transitionDensity;
            Labyrinth.ClearAll();
            WallUp();
            Empties.Clear();
            MakeCorridor(Labyrinth.Start);
            OpenNode(Labyrinth.Finish);
            Add_WeightedPositions(weightedDensity, weight);
        }
    }
}
