using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Pathfinding_Visulazier.Basic;
using Pathfinding_Visulazier.Labyrinth;
using Pathfinding_Visulazier.Labyrinth.Generators;
using Pathfinding_Visulazier.Pathfinders;
using Pathfinding_Visulazier.Visualizer.LabyrinthVisulaziers;
using Pathfinding_Visulazier.Visualizer.Massages;

namespace Pathfinding_Visulazier.Visualizer
{
    public class LabyrinthContainer : IObservable<LabyrinthContainerStatus>
    {
        private enum LabyrinthType
        {
            Rectangle,
            Paralleogram
        }

        private readonly Vector2D<double> VisulazierUpperLeft;
        private readonly Vector2D<double> VisulazierSize;
        private Vector2D<int> LabyrinthSize;
        private readonly UIElementCollection Children;

        private LabyrinthType _labyrinthType = LabyrinthType.Paralleogram;

        internal AbstarctLabyrinth Labyrinth { get; private set; } = null;
        internal  AbstarctLabyrinthVisualizer LabyrinthVisualizer { get; private set; }

        private SpaghettiGenerator Spaghetti = new SpaghettiGenerator(null);
        private RoomsGenerator Rooms = new RoomsGenerator(null);

        private BFS BFSPathfinder = new BFS();
        private Dijsktra DijkstraPathfinder = new Dijsktra();
        private AStar AStarPathfinder = new AStar();
        private AbstractPathfinder Pathfinder = null;
        private bool PathfinderMode = true;

        private int? StartFinishDistance;

        private List<IObserver<LabyrinthContainerStatus>> observers = new List<IObserver<LabyrinthContainerStatus>>();

        public LabyrinthContainer(Vector2D<double> visulazierUpperLeft, Vector2D<double> visulazierSize, Vector2D<int> labyrinthSize, UIElementCollection children)
        {
            VisulazierUpperLeft = visulazierUpperLeft;
            VisulazierSize = visulazierSize;
            LabyrinthSize = labyrinthSize;
            Children = children;
            SetRectangle();
        }

        // basic functionals methods

        private bool AcceptChanges()
        {
            return Pathfinder == null && PathfinderMode;
        }

        private void SetLabyrinthType(LabyrinthType labyrinthType) 
        {
            if (AcceptChanges() && labyrinthType != _labyrinthType)
            {
                if (LabyrinthVisualizer != null)
                    LabyrinthVisualizer.OnCompleted();
                switch (labyrinthType)
                {
                    case LabyrinthType.Rectangle:
                        Labyrinth = new RectangleLabyrinth(LabyrinthSize);
                        LabyrinthVisualizer = new RectangleLabyrinthVisualizer(VisulazierUpperLeft, VisulazierSize, Children);
                        break;
                    case LabyrinthType.Paralleogram:
                        Labyrinth = new ParallelogramLabyrinyh(LabyrinthSize);
                        LabyrinthVisualizer = new ParalleogramLabyrinthVisualizer(VisulazierUpperLeft, VisulazierSize, Children);
                        break;
                }
                LabyrinthVisualizer.Subscribe(Labyrinth);
                _labyrinthType = labyrinthType;
            }
        } 

        public void SetRectangle()
        {
            SetLabyrinthType(LabyrinthType.Rectangle);
        }

        public void SetParalleogram()
        {
            SetLabyrinthType(LabyrinthType.Paralleogram);
        }

        public void GenearteSpaghettiMaze(int transitionDensity, int weightedDensity, int weight = 1)
        {
            if (AcceptChanges())
            {
                Spaghetti.Labyrinth = Labyrinth;
                Spaghetti.GenerateMaze(transitionDensity, weightedDensity, weight);
            }
        }

        public void GenerateRoomsMaze(int transitionDensity, int weightedDensity, int weight = 1)
        {
            if (AcceptChanges())
            {
                Rooms.Labyrinth = Labyrinth;
                Rooms.GenerateMaze(transitionDensity, weightedDensity, weight);
            }
        }

        public void Resize(Vector2D<int> size)
        {
            if (AcceptChanges())
            {
                Labyrinth.Resize(size);
                LabyrinthSize = size;
            }
        }

        // Pathfinding methods

        private void PreActionSetUp()
        {
            Labyrinth.ClearVisited();
            Labyrinth.ClearPath(); 
            LabyrinthVisualizer.AcceptDrag = false;
            UpdateStatus(PathfinderMode);
        }

        public void VisulaizeBFS()
        {
            if (PathfinderMode)
            {
                BFSPathfinder.Sucscribe(Labyrinth, Labyrinth.Start, Labyrinth.Finish);
                Pathfinder = BFSPathfinder;
                PreActionSetUp();
            }
        }

        public void VisulaizeDijkstra()
        {
            if (PathfinderMode)
            {
                DijkstraPathfinder.Sucscribe(Labyrinth, Labyrinth.Start, Labyrinth.Finish);
                Pathfinder = DijkstraPathfinder;
                PreActionSetUp();
            }
            
        }

        public void VisulaizeAStar()
        {
            if (PathfinderMode)
            {
                AStarPathfinder.Sucscribe(Labyrinth, Labyrinth.Start, Labyrinth.Finish);
                Pathfinder = AStarPathfinder;
                PreActionSetUp();
            }
        }

        public void SetPathfinderMode()
        {
            PathfinderMode = true;
            UpdateStatus(null);
            Labyrinth.SetOrDeleteWalker(false);
            LabyrinthVisualizer.AcceptDrag = true;
        }

        public void SimualtePathfinding()
        {
            if (PathfinderMode)
            {
                if (Pathfinder != null)
                {
                    Pathfinder.ApplyNextStep();
                    LabyrinthVisualizer.AcceptDrag = Pathfinder.Complete;
                    UpdateStatus_Pathfinding();
                    bool? pathfinderWorking_Status = true;
                    if (Pathfinder.Complete)
                    {
                        pathfinderWorking_Status = null;
                        Pathfinder = null;
                    }
                    UpdateStatus(pathfinderWorking_Status);
                }
            }
        }

        // Labyrinth methods

        public void SetLabyrinthMode()
        {
            PathfinderMode = false;
            Labyrinth.SetOrDeleteWalker(true);
            PreActionSetUp(); 
            DijkstraPathfinder.Sucscribe(Labyrinth, Labyrinth.Start, Labyrinth.Finish);
            DijkstraPathfinder.ApplyWithoutVisualize();
            StartFinishDistance = DijkstraPathfinder.ShortesPath;
            UpdateStatus(false);
            UpdateStatus_Walker(Labyrinth.GetWeight(Labyrinth.Walker));
        }

        public void Walk(char direction)
        {
            if (!PathfinderMode && Labyrinth.MoveWalker(direction))
            {
                UpdateStatus_Walker(Labyrinth.GetWeight(Labyrinth.Walker));
            }
        }

        // IObservable methods

        public IDisposable Subscribe(IObserver<LabyrinthContainerStatus> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);
            UpdateStatus(null);
            return new Unsubscriber(observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<LabyrinthContainerStatus>> _observers;
            private IObserver<LabyrinthContainerStatus> _observer;

            public Unsubscriber(List<IObserver<LabyrinthContainerStatus>> observers, IObserver<LabyrinthContainerStatus> observer)
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

        private void UpdateStatus(bool? pathfinderWorking_Status)
        {
            var status = new LabyrinthContainerStatus
            {
                PathfinderWorking = pathfinderWorking_Status
            };
            foreach (var observer in observers)
                observer.OnNext(status);
        }

        private void UpdateStatus_Pathfinding()
        {
            var status = new LabyrinthContainerStatus
            {
                ShortedDistance_Start = Pathfinder.ShortesPath,
                VisitedVetexes = Pathfinder.VisitedCount
            };
            foreach (var observer in observers)
                observer.OnNext(status);
        }

        private void UpdateStatus_Walker(int disatnceWalked)
        {
            DijkstraPathfinder.Sucscribe(Labyrinth, Labyrinth.Walker, Labyrinth.Finish);
            DijkstraPathfinder.ApplyWithoutVisualize();
            var status = new LabyrinthContainerStatus
            {
                Last_DisatnceWalked = disatnceWalked,
                ShortedDistance_Start = StartFinishDistance,
                ShortedDistance_Walker = DijkstraPathfinder.ShortesPath,
                WalkerOnFinish = Labyrinth.Walker.Equals(Labyrinth.Finish)
            };
            foreach (var observer in observers)
                observer.OnNext(status);
        }
    }
}
