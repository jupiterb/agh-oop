using Pathfinding_Visulazier.Basic;
using System;
using System.Collections.Generic;

namespace Pathfinding_Visulazier.Labyrinth
{
    class ParallelogramLabyrinyh : AbstarctLabyrinth
    {
        public ParallelogramLabyrinyh(Vector2D<int> size) : base(size)
        {
            StandardRoomSize = new Vector2D<int>(1, 4);
        }

        public override List<Vector2D<int>> GetNeigboursOf(Vector2D<int> position)
        {
            List<Vector2D<int>> neighbours = new List<Vector2D<int>>
            {
                new Vector2D<int>(position.X, position.Y - 1),
                new Vector2D<int>(position.X, position.Y + 1),
                (position.Y % 2 == 0)?  new Vector2D<int>(position.X - 1, position.Y + 1) : 
                                        new Vector2D<int>(position.X + 1, position.Y - 1)
            };
            return neighbours;
        }

        public override bool MoveWalker(char direction)
        {
            Vector2D<int> newPosition = null;
            switch (direction)
            {
                case 'W':
                case 'w':
                    if (Walker.Y % 2 == 0)
                        newPosition = new Vector2D<int>(Walker.X - 1, Walker.Y + 1);
                    else
                        newPosition = new Vector2D<int>(Walker.X, Walker.Y + 1);
                    break;
                case 'S':
                case 's':
                    if (Walker.Y % 2 == 0)
                        newPosition = new Vector2D<int>(Walker.X, Walker.Y - 1);
                    else
                        newPosition = new Vector2D<int>(Walker.X + 1, Walker.Y - 1);
                    break;
                case 'A':
                case 'a':
                    if (Walker.Y % 2 == 0)
                        return MoveWalker('S');
                    else
                        newPosition = new Vector2D<int>(Walker.X , Walker.Y - 1);
                    break;
                case 'E':
                case 'e':
                    if (Walker.Y % 2 == 0)
                        newPosition = new Vector2D<int>(Walker.X, Walker.Y + 1);
                    else
                        return MoveWalker('W');
                    break;
                case 'D':
                case 'd':
                    if (Walker.Y % 2 == 0)
                        return MoveWalker('E');
                    else
                        return MoveWalker('S');
                case 'Q':
                case 'q':
                    if (Walker.Y % 2 == 0)
                        return MoveWalker('W');
                    else
                        return MoveWalker('A');
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
            if (size.Y % 2 == 1)
                return new Vector2D<int>(size.X, size.Y + 1);
            else
                return size;
        }

        protected override int Min_Distance(Vector2D<int> from, Vector2D<int> to)
        {
            return Math.Max( 2 * Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y) -
                ((Math.Sign(from.X - to.X) == -Math.Sign(from.Y - to.Y)) ? 2 : 0), 0);
        }
    }
}
