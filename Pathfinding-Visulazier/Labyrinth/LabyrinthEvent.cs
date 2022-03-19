using Pathfinding_Visulazier.Basic;

namespace Pathfinding_Visulazier.Labyrinth
{
    public class LabyrinthEvent
    {
        public Vector2D<int>[] Positions { get; }
        public NodeStatus? Status { get; }

        public int? Weight { get; }
        public AbstarctLabyrinth Resized { get; }

        public LabyrinthEvent(Vector2D<int>[] positions, NodeStatus? status, int? weight, AbstarctLabyrinth resized = null)
        {
            Positions = positions;
            Status = status;
            Weight = weight;
            Resized = resized;
        }
    }
}
