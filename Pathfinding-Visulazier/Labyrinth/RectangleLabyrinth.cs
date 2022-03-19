using Pathfinding_Visulazier.Basic;
using System;
using System.Collections.Generic;

namespace Pathfinding_Visulazier.Labyrinth
{
    public class RectangleLabyrinth : AbstarctLabyrinth
    {
        public RectangleLabyrinth(Vector2D<int> size) : base(size) 
        {
            StandardRoomSize = new Vector2D<int>(1, 1);
        }

        public override List<Vector2D<int>> GetNeigboursOf(Vector2D<int> position)
        {
            List<Vector2D<int>> neighbours = new List<Vector2D<int>>
            {
                new Vector2D<int>(position.X, position.Y - 1),
                new Vector2D<int>(position.X, position.Y + 1),
                new Vector2D<int>(position.X - 1, position.Y),
                new Vector2D<int>(position.X + 1, position.Y)
            };
            return neighbours;
        }

        public override bool MoveWalker(char direction)
        {
            Vector2D<int> newPosition = null;
            switch (direction)
            {
                case 'W': case 'w':
                    newPosition = new Vector2D<int>(Walker.X, Walker.Y - 1);
                    break;
                case 'S': case 's':
                    newPosition = new Vector2D<int>(Walker.X, Walker.Y + 1);
                    break;
                case 'A': case 'a':
                    newPosition = new Vector2D<int>(Walker.X - 1, Walker.Y);
                    break;
                case 'D': case 'd':
                    newPosition = new Vector2D<int>(Walker.X + 1, Walker.Y);
                    break;
            }
            if (newPosition != null && CanGoTo(newPosition))
            {
                SetStatus(Walker, NodeStatus.Visited);
                Walker = newPosition;
                if (!Walker.Equals(Finish) && !Walker.Equals(Start))
                    NodeChangedTo(Walker, NodeStatus.Walker, null);
                return true;
            }
            return false;
        }

        public override bool OnBoard(Vector2D<int> position)
        {
            return position.CompareTo(LowerRight) < 0 && position.CompareTo(UpperLeft) > 0;
        }

        protected override Vector2D<int> AdjustedSize(Vector2D<int> size)
        {
            return size;
        }

        protected override int Min_Distance(Vector2D<int> from, Vector2D<int> to)
        {
            return Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
        }
    }
}
