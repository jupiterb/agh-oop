using System;
using Pathfinding_Visulazier.Labyrinth;
using Pathfinding_Visulazier.Basic;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;

namespace Pathfinding_Visulazier.Visualizer.LabyrinthVisulaziers
{
    public abstract class AbstarctLabyrinthVisualizer : IObserver<LabyrinthEvent>
    {
        protected readonly Vector2D<double> UpperLeft;
        protected readonly Vector2D<double> Size;
        private IDisposable Unsucsriber = null;
        private readonly PositionPolygonDictionary Fields = new PositionPolygonDictionary();
        protected double FieldSize;
        private readonly UIElementCollection Children;
        private AbstarctLabyrinth Labyrinth;

        private const int MaxWeight = 20;
        private int Weight;
        private enum DragEvent
        {
            Cleaning,
            Walling,
            Weighting
        }
        DragEvent _dragEvent = DragEvent.Walling;
        private bool MousePressed = false;
        private bool StartDragged = false;
        private bool FinishDragged = false;
        public bool AcceptDrag = true;
        
        public AbstarctLabyrinthVisualizer(Vector2D<double> upperLeftposition, Vector2D<double> size, UIElementCollection children)
        {
            UpperLeft = upperLeftposition;
            Size = size;
            Children = children;
        }

        public void Subscribe(AbstarctLabyrinth labyrinth)
        {
            if (labyrinth != null)
            {
                OnCompleted();
                Unsucsriber = labyrinth.Subscribe(this);
                Labyrinth = labyrinth;
                FieldSize = CountFieldSize(labyrinth.LowerRight);
                CreateFields(labyrinth);
            }      
        }

        private void Unsubscribe()
        {
            Unsucsriber.Dispose();
        }

        public void OnCompleted()
        {
            foreach(var field in Fields.Polygons())
            {
                Children.Remove(field);
            }
            Fields.Clear();
            if (Unsucsriber!= null)
                Unsubscribe();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(LabyrinthEvent value)
        {
            if (value.Resized != null)
            {
                foreach (var field in Fields.Polygons())
                {
                    Children.Remove(field);
                }
                Fields.Clear();
                FieldSize = CountFieldSize(value.Resized.LowerRight);
                CreateFields(value.Resized);
            }
            if (value.Status != null)
            {
                foreach (var position in value.Positions)
                {
                    UpdateFieldStatus(Fields.Get(position), value.Status);
                }
            }
            if (value.Weight.HasValue)
            {
                int weight = value.Weight.Value;
                foreach (var position in value.Positions)
                {
                    UpdateFieldWeight(Fields.Get(position), weight);
                }
            }
        }

        protected abstract double CountFieldSize(Vector2D<int> labyrinthSzie);

        private void CreateFields(AbstarctLabyrinth labyrinth)
        {
            for (int i=0; i<labyrinth.LowerRight.X; i++)
            {
                for (int j=0; j<labyrinth.LowerRight.Y; j++)
                {
                    Vector2D<int> position = new Vector2D<int>(i, j);
                    if (labyrinth.OnBoard(position))
                    {
                        Fields.Add(position, MakeField(position));
                        Children.Add(Fields.Get(position));
                        UpdateFieldStatus(Fields.Get(position), labyrinth.GetStatus(position));
                        Fields.Get(position).MouseDown += MouseDown;
                        Fields.Get(position).MouseMove += MouseMove;
                        Fields.Get(position).MouseUp += MouseUp;
                    }
                }
            }
        }

        private void MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (AcceptDrag)
            {
                MousePressed = false;
                Vector2D<int> position = Fields.Get((Polygon)sender);
                if (StartDragged)
                    Labyrinth.SetStatus(position, NodeStatus.Start);
                if (FinishDragged)
                    Labyrinth.SetStatus(position, NodeStatus.Finish);
            }
        }

        private void MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (AcceptDrag)
            {
                MousePressed = true;
                Vector2D<int> position = Fields.Get((Polygon)sender);
                StartDragged = (Labyrinth.GetStatus(position) == NodeStatus.Start);
                FinishDragged = (Labyrinth.GetStatus(position) == NodeStatus.Finish);
            }
        }

        private void MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (AcceptDrag && MousePressed && !StartDragged && !FinishDragged)
            {
                Vector2D<int> position = Fields.Get((Polygon)sender);
                switch (_dragEvent)
                {
                    case DragEvent.Cleaning:
                        Labyrinth.SetStatus(position, NodeStatus.Empty);
                        Labyrinth.SetWeight(position, 1);
                        break;
                    case DragEvent.Walling:
                        Labyrinth.SetStatus(position, NodeStatus.Wall);
                        break;
                    case DragEvent.Weighting:
                        Labyrinth.SetWeight(position, Math.Min(Weight, MaxWeight));
                        break;
                }
            }
        }

        public void SetManualCleaning()
        {
            _dragEvent = DragEvent.Cleaning;
        }

        public void SetManualWalling()
        {
            _dragEvent = DragEvent.Walling;
        }

        public void SetManualWeighting(int weight)
        {
            Weight = weight;
            _dragEvent = DragEvent.Weighting;
        }

        protected abstract Polygon MakeField(Vector2D<int> position);

        private void UpdateFieldStatus(Polygon field, NodeStatus? status)
        {
            switch (status)
            {
                case NodeStatus.Empty:
                    field.Fill = Brushes.Transparent;
                    break;
                case NodeStatus.Wall:
                    field.Fill = Brushes.Black;
                    break;
                case NodeStatus.Visited:
                    field.Fill = Brushes.Violet;
                    break;
                case NodeStatus.Path:
                    field.Fill = Brushes.Gold;
                    break;
                case NodeStatus.Start:
                    field.Fill = Brushes.Green;
                    break;
                case NodeStatus.Finish:
                    field.Fill = Brushes.Red;
                    break;
                case NodeStatus.Walker:
                    field.Fill = Brushes.Orange;
                    break;
            }
        }

        private void UpdateFieldWeight(Polygon field, int weight)
        {
            field.StrokeThickness = Math.Max(((double)weight / MaxWeight) * FieldSize / 3.0, 1.0);
        }
    }
}
