using System;
using System.Collections.Generic;
using System.Linq;
using Pathfinding_Visulazier.Basic;
using Pathfinding_Visulazier.Pathfinders;

namespace Pathfinding_Visulazier.Labyrinth
{
    public abstract class AbstarctLabyrinth : IObservable<LabyrinthEvent>, IGraph
    {
        public static Vector2D<int> UpperLeft { get; } = new Vector2D<int>(-1, -1);
        public Vector2D<int> LowerRight { get; protected set; } // size
        public Vector2D<int> StandardRoomSize { get; protected set; }

        public Vector2D<int> Start { get; private set; } = new Vector2D<int>(0, 0);
        public Vector2D<int> Finish { get; private set; }
        public Vector2D<int> Walker { get; protected set; }

        protected readonly HashSet<Vector2D<int>> Walls    = new HashSet<Vector2D<int>>();
        protected readonly HashSet<Vector2D<int>> Visisted = new HashSet<Vector2D<int>>();
        protected readonly HashSet<Vector2D<int>> Path     = new HashSet<Vector2D<int>>();
        private readonly HashSet<Vector2D<int>> UnvisibleVisited = new HashSet<Vector2D<int>>(); // use when Start/Finish/Walker is visited (SetVisited from IGraph)
                                                                                               
        protected readonly Dictionary<Vector2D<int>, int> Weighted = new Dictionary<Vector2D<int>, int>();

        private List<IObserver<LabyrinthEvent>> observers = new List<IObserver<LabyrinthEvent>>();

        public AbstarctLabyrinth(Vector2D<int> size)
        {
            LowerRight = AdjustedSize(size);
            Finish = new Vector2D<int>(LowerRight.X - 1, LowerRight.Y - 1);
        }

        // IObservable methods

        public IDisposable Subscribe(IObserver<LabyrinthEvent> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);
            return new Unsubscriber(observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<LabyrinthEvent>> _observers;
            private IObserver<LabyrinthEvent> _observer;

            public Unsubscriber(List<IObserver<LabyrinthEvent>> observers, IObserver<LabyrinthEvent> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }

        protected void NodeChangedTo(Vector2D<int> position, NodeStatus? status, int? weight, bool resized = false)
        {
            Vector2D<int>[] positions = { position };
            NodesChangedTo(positions, status, weight, resized);
        }

        protected void NodesChangedTo(Vector2D<int>[] positions, NodeStatus? status, int? weight, bool resized = false)
        {
            LabyrinthEvent nodesDetail = new LabyrinthEvent(positions, status, weight, resized? this : null);
            foreach (var observer in observers)
                observer.OnNext(nodesDetail);
        }

        // basic functionals methods

        public NodeStatus GetStatus(Vector2D<int> position)
        {
            if (OnBoard(position))
            {
                if (Walls.Contains(position))
                    return NodeStatus.Wall;
                else if (Visisted.Contains(position))
                    return NodeStatus.Visited;
                else if (Path.Contains(position))
                    return NodeStatus.Path;
                else if (position.Equals(Start))
                    return NodeStatus.Start;
                else if (position.Equals(Finish))
                    return NodeStatus.Finish;
                else if (position.Equals(Walker))
                    return NodeStatus.Walker;
                else
                    return NodeStatus.Empty;
            }
            else
                return NodeStatus.Empty;
        }

        public bool SetStatus(Vector2D<int> position, NodeStatus status)
        {
            if (OnBoard(position) && !position.Equals(Start) && !position.Equals(Finish))
            {
                if (GetStatus(position) != status)
                {
                    Walls.Remove(position);
                    Visisted.Remove(position);
                    Path.Remove(position);
                    switch (status)
                    {
                        case NodeStatus.Wall:
                            Walls.Add(position);
                            break;
                        case NodeStatus.Visited:
                            Visisted.Add(position);
                            break;
                        case NodeStatus.Path:
                            Path.Add(position);
                            break;
                        case NodeStatus.Start:
                            NodeChangedTo(Start, NodeStatus.Empty, null);
                            Start = position;
                            break;
                        case NodeStatus.Finish:
                            NodeChangedTo(Finish, NodeStatus.Empty, null);
                            Finish = position;
                            break;
                    }
                    NodeChangedTo(position, status, null);
                }
                return true;
            }
            else
                return false;
        }

        public int GetWeight(Vector2D<int> position)
        {
            if (Weighted.TryGetValue(position, out int weight))
                return weight;
            else
                return 1;
        }

        public void SetWeight(Vector2D<int> position, int weight)
        {
            Weighted.Remove(position);
            if (weight > 1)
                Weighted.Add(position, weight);
            NodeChangedTo(position, null, Math.Max(weight, 1));
        }

        public bool CanGoTo(Vector2D<int> position)
        {
            return OnBoard(position) && GetStatus(position) != NodeStatus.Wall;
        }

        public void ClearWalls()
        {
            NodesChangedTo(Walls.ToArray(), NodeStatus.Empty, null);
            Walls.Clear();
        }

        public void ClearVisited()
        {
            NodesChangedTo(Visisted.ToArray(), NodeStatus.Empty, null);
            Visisted.Clear();
            UnvisibleVisited.Clear();
        }

        public void ClearPath()
        {
            NodesChangedTo(Path.ToArray(), NodeStatus.Empty, null);
            Path.Clear();
        }

        public void ClearWeighted()
        {
            NodesChangedTo(Weighted.Keys.ToArray(), null, 1);
            Weighted.Clear();
        }

        public void ClearAll()
        {
            ClearWalls();
            ClearVisited();
            ClearPath();
            ClearWeighted();
        }

        public void Resize(Vector2D<int> size)
        {
            ClearAll();
            LowerRight = AdjustedSize(size);
            Start = new Vector2D<int>(0, 0);
            Finish = new Vector2D<int>(LowerRight.X - 1, LowerRight.Y - 1);
            NodeChangedTo(Start, NodeStatus.Start, null, true);
            NodeChangedTo(Finish, NodeStatus.Finish, null);
        }

        // abstract methods

        public abstract bool OnBoard(Vector2D<int> position);

        public abstract List<Vector2D<int>> GetNeigboursOf(Vector2D<int> position);

        public abstract bool MoveWalker(Char direction);

        protected abstract Vector2D<int> AdjustedSize(Vector2D<int> size);

        // IGraph methodts

        private HashSet<Vector2D<int>> Specials()
        {
            return new HashSet<Vector2D<int>>
            {
                Start,
                Walker,
                Finish
            };
        }

        public void SetVisited(Vector2D<int> vertex)
        {
            if (!SetStatus(vertex, NodeStatus.Visited) && Specials().Contains(vertex))
                UnvisibleVisited.Add(vertex);
        }

        public void SetPath(Vector2D<int> vertex)
        {
            SetStatus(vertex, NodeStatus.Path);
        }

        public List<Vector2D<int>> UnvisitedNeighbours(Vector2D<int> vertex)
        {
            List<Vector2D<int>> neighbours = AvailableNeighbours(vertex);
            neighbours.RemoveAll(IsVisited);
            return neighbours;
        }

        public List<Vector2D<int>> AvailableNeighbours(Vector2D<int> vertex)
        {
            List<Vector2D<int>> neighbours = GetNeigboursOf(vertex);
            bool NotAvailable (Vector2D<int> position)
            {
                return !OnBoard(position) || !(GetStatus(position) == NodeStatus.Empty || Specials().Contains(position));
            }
            neighbours.RemoveAll(NotAvailable);
            return neighbours;
        }

        public bool IsVisited(Vector2D<int> vertex)
        {
            return !(GetStatus(vertex) == NodeStatus.Empty || (Specials().Contains(vertex) && !UnvisibleVisited.Contains(vertex)));
        }

        public int MinDistance(Vector2D<int> from, Vector2D<int> to)
        {
            return Min_Distance(from, to);
        }

        protected abstract int Min_Distance(Vector2D<int> from, Vector2D<int> to);

        // walker methods

        public void SetOrDeleteWalker(bool set)
        {
            Walker = set ? new Vector2D<int>(Start.X, Start.Y) : null;
        }
    }
}
