using Pathfinding_Visulazier.Basic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pathfinding_Visulazier.Pathfinders
{
    public class Dijsktra : AbstractPathfinder
    {
        private class PrioritedVertex : IComparable
        {
            private int Priority;
            private int Time;
            public Vector2D<int> Vertex { get; private set; }

            public PrioritedVertex(int priority, Vector2D<int> vertex, int time)
            {
                Priority = priority;
                Vertex = vertex;
                Time = time;
            }
            
            public int CompareTo(object obj)
            {
                if (obj is PrioritedVertex other)
                {
                    if (Priority != other.Priority)
                        return Priority - other.Priority;
                    if (Time != other.Time)
                        return other.Time - Time;
                    else if (Vertex.X != other.Vertex.X)
                        return Vertex.X - other.Vertex.X;
                    else
                        return Vertex.Y - other.Vertex.Y;
                }
                else
                    return 0;
            }
        }

        private readonly SortedSet<PrioritedVertex> PriorityQueue = new SortedSet<PrioritedVertex>();
        private int SearchCount;

        public Dijsktra() : base() { }

        public override void Sucscribe(IGraph graph, Vector2D<int> start, Vector2D<int> finish)
        {
            base.Sucscribe(graph, start, finish);
            PriorityQueue.Clear();
            PriorityQueue.Add(new PrioritedVertex(0, start, VisitedCount));
            SearchCount = 0;
        }

        protected virtual void Realx(Vector2D<int> vertex, Vector2D<int> neighbour, int distance, int predict = 0)
        {
            if (!Distances.ContainsKey(neighbour) || Distances[vertex] + distance < Distances[neighbour])
            {
                Distances[neighbour] = Distances[vertex] + distance;
                Parents[neighbour] = vertex;
                PriorityQueue.Add(new PrioritedVertex(Distances[neighbour] + predict, neighbour, SearchCount));
            }
        }

        public override void ApplyNextStep()
        {
            if (PriorityQueue.Any())
            {
                // searching for finish vertex
                PrioritedVertex pVertex;
                do
                {
                    pVertex = PriorityQueue.Min();
                    PriorityQueue.Remove(pVertex);

                } while (pVertex != null && Graph.IsVisited(pVertex.Vertex)); // in normal Dijkstra it is not required
                if (pVertex == null)
                    return;
                VisitedCount++;
                var vertex = pVertex.Vertex;
                Graph.SetVisited(vertex);
                if (vertex.Equals(Finish))
                {
                    PriorityQueue.Clear();
                    ShortesPath = Distances[vertex];
                }
                else
                {
                    foreach (var neighbour in Graph.AvailableNeighbours(vertex))
                    {
                        Realx(vertex, neighbour, Graph.GetWeight(neighbour));
                        SearchCount++;
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
            var visisted = new HashSet<Vector2D<int>>();
            while (PriorityQueue.Any())
            {
                PrioritedVertex pVertex = PriorityQueue.Min();
                PriorityQueue.Remove(pVertex);
                var vertex = pVertex.Vertex;
                visisted.Add(vertex);
                if (vertex.Equals(Finish))
                {
                    PriorityQueue.Clear();
                    ShortesPath = Distances[vertex];
                }
                else
                {
                    foreach (var neighbour in Graph.AvailableNeighbours(vertex))
                    {
                        Realx(vertex, neighbour, Graph.GetWeight(neighbour));
                        SearchCount++;
                    }
                }
            }
        }
    }
}
