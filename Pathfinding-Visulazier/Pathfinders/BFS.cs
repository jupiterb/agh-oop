using Pathfinding_Visulazier.Basic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pathfinding_Visulazier.Pathfinders
{
    public class BFS : AbstractPathfinder
    {
        private readonly Queue<Vector2D<int>> Queue = new Queue<Vector2D<int>>();

        public BFS() : base() { }

        public override void Sucscribe(IGraph graph, Vector2D<int> start, Vector2D<int> finish)
        {
            base.Sucscribe(graph, start, finish);
            Queue.Clear();
            Queue.Enqueue(start);
            Graph.SetVisited(start);
        }

        public override void ApplyNextStep()
        {
            if (Queue.Any())
            {
                // searching for finish vertex
                var vertex = Queue.Dequeue();
                if (vertex.Equals(Finish))
                {
                    Queue.Clear();
                    ShortesPath = Distances[vertex];
                }
                else
                {
                    foreach (var neighbour in Graph.UnvisitedNeighbours(vertex))
                    {
                        Distances[neighbour] = Distances[vertex] + 1;
                        Parents[neighbour] = vertex;
                        Graph.SetVisited(neighbour);
                        VisitedCount++;
                        Queue.Enqueue(neighbour);
                    }
                }
            }
            else
            {
                // completing shortest path
                ReconstructPath();
            }
        }

        public override void ApplyWithoutVisualize()
        {
            throw new NotImplementedException();
        }
    }
}
