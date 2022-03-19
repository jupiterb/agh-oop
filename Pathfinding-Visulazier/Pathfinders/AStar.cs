using Pathfinding_Visulazier.Basic;

namespace Pathfinding_Visulazier.Pathfinders
{
    public class AStar : Dijsktra
    {
        protected override void Realx(Vector2D<int> vertex, Vector2D<int> neighbour, int distance, int predict = 0)
        {
            base.Realx(vertex, neighbour, distance, Graph.MinDistance(neighbour, Finish));
        }
    }
}
