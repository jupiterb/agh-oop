using Pathfinding_Visulazier.Basic;
using System.Collections.Generic;

namespace Pathfinding_Visulazier.Pathfinders
{
    public abstract class AbstractPathfinder
    {
        protected IGraph Graph { get; private set; }
        protected Dictionary<Vector2D<int>, int> Distances = new Dictionary<Vector2D<int>, int>();
        protected Dictionary<Vector2D<int>, Vector2D<int>> Parents = new Dictionary<Vector2D<int>, Vector2D<int>>();
        public int? ShortesPath { get; protected set; } = null;
        public int VisitedCount { get; protected set; } = 0;
        public bool Complete { get; protected set; } = false;

        protected Vector2D<int> Start { get; private set; }
        protected Vector2D<int> Finish { get; private set; }

        public virtual void Sucscribe(IGraph graph, Vector2D<int> start, Vector2D<int> finish)
        {
            Graph = graph;
            Start = start;
            Finish = finish;
            Distances.Clear();
            Parents.Clear();
            Distances.Add(start, graph.GetWeight(start));
            Parents.Add(start, null);

            ShortesPath = null;
            VisitedCount = 0;
            Complete = false;
        }

        public abstract void ApplyNextStep();

        public abstract void ApplyWithoutVisualize();

        protected void ReconstructPath()
        {
            if (ShortesPath != null)
            {
                Finish = Parents[Finish];
                Graph.SetPath(Finish);
                if (Finish.Equals(Start))
                {
                    Complete = true;
                }
            }
            else
            {
                Complete = true;
            }
        }
    }
}
