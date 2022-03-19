using Pathfinding_Visulazier.Basic;
using System.Collections.Generic;

namespace Pathfinding_Visulazier.Pathfinders
{
    public interface IGraph
    {
        List<Vector2D<int>> UnvisitedNeighbours(Vector2D<int> vertex);

        List<Vector2D<int>> AvailableNeighbours(Vector2D<int> vertex);

        void SetVisited(Vector2D<int> vertex);

        void SetPath(Vector2D<int> vertex);

        int GetWeight(Vector2D<int> vertex);

        bool IsVisited(Vector2D<int> vertex);

        int MinDistance(Vector2D<int> from, Vector2D<int> to);
    }
}
